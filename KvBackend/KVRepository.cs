using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KvBackend
{
    public class KVRepository
    {
        private readonly string connectionString;
        public KVRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public KVRepository()
        {
            try
            {
                this.connectionString = ConfigurationManager.ConnectionStrings["StuffDb"].ConnectionString;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("StuffDb connection string should exist", ex);
            }
        }

        /// <summary>
        /// One manner of statically avoiding filling ExtendedFields with known fields
        /// </summary>
        private static readonly string[] KnownFields = { "Id", "Description", "OfferStartDate", "OfferEndDate", "ActiveFlag", "IsActive" };

        public Offer GetOffer(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Offer> GetOffers(string whereClause = "", string selectClause = "OfferStartDate,OfferEndDate,ActiveFlag,IsActive")
        {
            List<Offer> offers = new List<Offer>();
            //ConfigurationManager.AppSettings
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var cm = conn.CreateCommand();
                SqlParameter pWhere = new SqlParameter("@where", SqlDbType.NVarChar, -1) { Value = whereClause };//WHERE Id = ''' + '86C3B9D1-5269-4D8B-BA99-4F7B3CEB371D' + '''
                SqlParameter pSelect = new SqlParameter("@select", SqlDbType.NVarChar, -1) { Value = selectClause };
                cm.CommandText = "exec sp_getKVPairs @where, @select";
                cm.Parameters.Add(pWhere);
                cm.Parameters.Add(pSelect);
                cm.Prepare();

                using (IDataReader reader = cm.ExecuteReader())
                {
                    List<string> colNames = new List<string>(reader.FieldCount);
                    for (int i = 0; i < reader.FieldCount; i++)
                        if (!KnownFields.Any(s => s.Equals(reader.GetName(i), StringComparison.OrdinalIgnoreCase)))
                            colNames.Add(reader.GetName(i));

                    while (reader.Read())
                    {
                        //this instantiation is brittle if we actually allow the select clause to be passed
                        //obviously in real impl. we would make this prettier
                        Offer o = new Offer((bool)reader["IsActive"]);
                        o.Id = (Guid)reader["Id"];
                        o.OfferStartDate = reader["OfferStartDate"] == DBNull.Value ? null : (DateTime?)reader["OfferStartDate"];
                        o.OfferEndDate = reader["OfferEndDate"] == DBNull.Value ? null : (DateTime?)reader["OfferEndDate"];
                        o.ActiveFlag = reader["ActiveFlag"] == DBNull.Value ? null : (bool?)reader["ActiveFlag"];
                        foreach (string colName in colNames)
                        {
                            o.ExtendedFields.Add(colName, reader[colName]);
                        }
                        offers.Add(o);
                    }
                }
            }

            return offers;
        }

        public void UpdateOffer(Offer o)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var cm = conn.CreateCommand();

                StringBuilder sb = new StringBuilder();
                //imagine I use parameters here, but for now it's too much work =S
                foreach (var extField in o.ExtendedFields)
                {
                    sb.AppendLine(" IF NOT EXISTS(SELECT * FROM KvPairTable WHERE [Key] = '" + extField.Key + "' AND RootObjectId = '" + o.Id + "') BEGIN "
                    + " INSERT INTO KvPairTable(RootObjectId,[Key],Value,[Schema]) "
                    + " VALUES ('" + o.Id + "'"
                        + ",'" + extField.Key + "'"
                        + ",'" + extField.Value + "'"
                        + ",'" + "Schem1" + "'"
                        + ")"
                    + " END "
                    + " ELSE BEGIN "
                    + " UPDATE KvPairTable SET Value = '" + extField.Value + "' WHERE [Key] = '" + extField.Key + "' AND RootObjectId = '" + o.Id + "'"
                    + " END");
                }

                cm.CommandText = sb.ToString();

                cm.ExecuteNonQuery();
            }
        }
    }
}
