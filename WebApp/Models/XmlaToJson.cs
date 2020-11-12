using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AnalysisServices.AdomdClient;
using Newtonsoft.Json;
using System.Text;
using System.IO;

namespace XMLAWEB.Models
{
    public class XmlaToJson
    {
        public static string returnJson(string clientid, string secret, string query, string workspace, string dataset){

            if(String.IsNullOrEmpty(clientid) || String.IsNullOrEmpty(secret) || String.IsNullOrEmpty(query) || String.IsNullOrEmpty(workspace) || String.IsNullOrEmpty(dataset)){
                return "Not all parameters have been specified! clientid, secret, query, workspace, dataset";
            }

            string tenantid = "your_tenant_id";
            string connectionString = "Datasource="+workspace+";initial catalog="+dataset+";User ID=app:"+clientid+"@"+tenantid+";Password="+secret;
        /*******************************************************
                Define Connection
            *******************************************************/
 
            AdomdConnection adomdConnection = new AdomdConnection(connectionString);
            
            /*******************************************************
                Define Query (as a Command)
                - the AdomdCommant uses the above connection
                - subsitute this for your own query
            *******************************************************/
 
            //String query = @"EVALUATE (Sales)";
            AdomdCommand adomdCommand = new AdomdCommand(query,adomdConnection);
 
            /*******************************************************
                Run the Query
                - Open the connection
                - Issue the query
                - Iterate through each row of the reader
                - Iterate through each column of the current row
                - Close the connection
            *******************************************************/
 
            adomdConnection.Open();
             
            AdomdDataReader reader = adomdCommand.ExecuteReader();

            //Create a loop for every row in the resultset
            List < string > curColumn = new List < string > ();
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            string columnName = string.Empty;
            string fieldVal = string.Empty;
            string prevFieldVal = string.Empty;
            if (reader != null)
                {
                    using(JsonWriter myJson = new JsonTextWriter(sw))
                    {
                        myJson.WriteStartArray();
                        while (reader.Read())
                        {
                            myJson.WriteStartObject();
                            int fields = reader.FieldCount;
                            for (int i = 0; i < fields; i++)
                            {
                                if (reader[i] != null)
                                {
                                    fieldVal = reader[i].ToString();
                                    if (i != 0 && reader[i - 1] != null)
                                    prevFieldVal = reader[i - 1].ToString();
                                    else prevFieldVal = "First";
                                    if ((fieldVal == null || fieldVal.ToLower().Trim() == "undefined" ||
                                    fieldVal.ToLower().Trim() == "unknown")
                                    && (prevFieldVal == null || prevFieldVal.ToLower().Trim() ==
                                        "undefined" || prevFieldVal.ToLower().Trim() == "unknown"))
                                    {
                                        continue;
                                    } else
                                    {
                                        columnName = reader.GetName(i).Replace(".[MEMBER_CAPTION]",
                                            "").Trim();
                                        curColumn = columnName.Split(new string[] {                                                "."
                                        },
                                        StringSplitOptions.None).ToList();
                                        columnName = curColumn[curColumn.Count - 1].Replace("[",
                                            "").Replace("]", "");
                                        if (Convert.ToString(columnName.Trim()).ToLower() == "latitude")
                                        columnName = "lat";
                                        if (Convert.ToString(columnName.Trim()).ToLower() == "longitude")
                                        columnName = "lon";
                                        myJson.WritePropertyName(columnName);
                                        myJson.WriteValue(reader[i]);
                                    }
                                }
                            }
                            myJson.WriteEndObject();
                        }
                        myJson.WriteEndArray();
                    }
                } else
                {
                    return "No Records to display";
                }
                //Console.WriteLine(curColumn);

            adomdConnection.Close();
            return sw.ToString();
        }
    }
}