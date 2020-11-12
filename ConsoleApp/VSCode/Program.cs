using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AnalysisServices.AdomdClient;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
 
namespace QueryTool
{
    class Program
    {
        
        static void Main(string[] args)
        {
            /*******************************************************
                Define Connection
            *******************************************************/
 
            AdomdConnection adomdConnection = new AdomdConnection("Datasource=powerbi://api.powerbi.com/v1.0/myorg/<your work space>;initial catalog=<your data set>;User ID=app:<app_id>@<tenant_id>;Password=<app_secret>");
            
                   
            /*******************************************************
                Define Query (as a Command)
                - the AdomdCommant uses the above connection
                - subsitute this for your own query
            *******************************************************/
 
            String query = @"<your_DAX_query>";
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

            String jsonstring = @"[";
            // Create a loop for every row in the resultset
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
                    Console.WriteLine("No Records to display");
                }
                Console.WriteLine(sw.ToString());

            adomdConnection.Close();
        }

    
    }
}