using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using ArangoDB.Client;
using System.Net;
using Newtonsoft.Json;

namespace JobModel.AutoFac
{
    public class ArangoConnection
    {
        private const string ArangoID = "seek";

        public ArangoConnection(ArangoOptions options)
        {

            // read arango setting from arango.json
            //string arangoSettingFilename = $"{AppContext.BaseDirectory}/arango.setting";

            //var arangoSetting = JsonConvert.DeserializeObject<ArangoOptions>(File.ReadAllText(arangoSettingFilename));

            ArangoDatabase.ChangeSetting(ArangoID, a =>
            {
                a.Url = options.Url;
                a.Credential = options.Credential;
                a.SystemDatabaseCredential = options.SystemCredential;
                a.Database = options.Database;
            });
        }

        public IArangoDatabase CreateClient()
        {
            return ArangoDatabase.CreateWithSetting(ArangoID);
        }
    }

    public class ArangoOptions
    {
        public string Url { get; set; }
        public string Database { get; set; }
        public NetworkCredential Credential { get; set; }
        public NetworkCredential SystemCredential { get; set; }
    }
}
