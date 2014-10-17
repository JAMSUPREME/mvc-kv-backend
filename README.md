mvc-kv-backend
==============

Demos kv backend and its MVC front


Instructions
-------

1. First step - run all of the scripts in the DBScripts directory. The VERIFY scripts are optional, but just let you know if things are working
2. Second step - Modify the connection strings used in KvBackend.app.config and mvc-kv-backend.web.config - Your machine may not have the same DB location (orig code sample is at `machine\SQLEXPRESS` but I later modified it to use `localhost`)
3. Optional - Run the EnsureFill tests to ensure the connection and retrieval work
4. Hit the endpoints `http://localhost:61172/OfferGrid/Index` and `http://localhost:61172/OfferEdit/Index` (*NOTE* that your port may differ)

The OfferEdit view is rather ugly, and there may be a more elegant solution for iterating over our data, but right now collections are also flattened into columns, so this is just an example of one way we can structure our data.
