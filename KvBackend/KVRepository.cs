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
        /// <summary>
        /// Prototype of getting offers with all their extended fields.
        /// Have NOT yet evaluated getting data across schemas or dealing with complex filtering or paging.
        /// </summary>
        /// <param name="whereClause">FYI: This is currently very brittle!!!</param>
        /// <param name="selectClause">FYI: This probably isn't necessary - might remove?</param>
        /// <param name="pageStart"></param>
        /// <param name="pageEnd"></param>
        /// <returns></returns>
        public IEnumerable<Offer> GetOffers(string whereClause = null, string selectClause = null, int pageStart = 1, int pageEnd = 99999)
        {
            List<Offer> offers = new List<Offer>();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var cm = this.BuildCommand(conn, whereClause, selectClause);

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

        private SqlCommand BuildCommand(SqlConnection conn, string whereClause, string selectClause, int pageStart = -1, int pageEnd = -1)
        {
            StringBuilder sbCMD = new StringBuilder("exec sp_getKVPairs");
            var cm = conn.CreateCommand();
            if (whereClause != null)
            {
                SqlParameter pWhere = new SqlParameter("@where", SqlDbType.NVarChar, -1) { Value = whereClause };//WHERE Id = ''' + '86C3B9D1-5269-4D8B-BA99-4F7B3CEB371D' + '''
                sbCMD.Append(" @where=@where");
                cm.Parameters.Add(pWhere);
            }
            if (selectClause != null)
            {
                SqlParameter pSelect = new SqlParameter("@select", SqlDbType.NVarChar, -1) { Value = selectClause };
                if (cm.Parameters.Count > 0)
                    sbCMD.Append(",");
                sbCMD.Append(" @select=@select");
                cm.Parameters.Add(pSelect);
            }
            if (pageStart != -1)
            {
                SqlParameter pPageStart = new SqlParameter("@pageStart", SqlDbType.Int) { Value = pageStart };
                if (cm.Parameters.Count > 0)
                    sbCMD.Append(",");
                sbCMD.Append(" @pageStart=@pageStart");
                cm.Parameters.Add(pPageStart);
            }
            if (pageEnd != -1)
            {
                SqlParameter pPageEnd = new SqlParameter("@pageEnd", SqlDbType.Int) { Value = pageEnd };
                if (cm.Parameters.Count > 0)
                    sbCMD.Append(",");
                sbCMD.Append(" @pageEnd=@pageEnd");
                cm.Parameters.Add(pPageEnd);
            }
            cm.CommandText = sbCMD.ToString();
            cm.Prepare();
            return cm;
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
