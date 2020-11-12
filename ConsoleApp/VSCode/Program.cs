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
 
            AdomdConnection adomdConnection = new AdomdConnection("Datasource=powerbi://api.powerbi.com/v1.0/myorg/Demo_SwissRe;initial catalog=RLS;User ID=app:cf166a73-946b-4c90-9d84-8b51e4d79ea8@644d9875-54f6-4f5f-b562-2a14755779f7;Password=uJOVq.n55abLr2-9L6G44uWit0L17~-bmr");
            
          
            //AdomdConnection adomdConnection = new AdomdConnection("Datasource=powerbi://api.powerbi.com/v1.0/powerbichamps.net/Demo_SwissRe;initial catalog=RLS;Password=eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Imh1Tjk1SXZQZmVocTM0R3pCRFoxR1hHaXJuTSIsImtpZCI6Imh1Tjk1SXZQZmVocTM0R3pCRFoxR1hHaXJuTSJ9.eyJhdWQiOiJodHRwczovL2FuYWx5c2lzLndpbmRvd3MubmV0L3Bvd2VyYmkvYXBpIiwiaXNzIjoiaHR0cHM6Ly9zdHMud2luZG93cy5uZXQvZDQyY2I2YjctZDgzMC00YTNhLWFhMTYtZTY1MzcwZDdkOTAwLyIsImlhdCI6MTU5NjA4ODU4OCwibmJmIjoxNTk2MDg4NTg4LCJleHAiOjE1OTYwOTI0ODgsImFpbyI6IkUyQmdZQ2pKRVJMZ3UxYjQrc1dYYTQvcWZVdDVBQT09IiwiYXBwaWQiOiI3MWE3NTNjNS0wOTgxLTQ4ZjgtODVhYS01MDg1MTBhOWZmMmIiLCJhcHBpZGFjciI6IjEiLCJpZHAiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC9kNDJjYjZiNy1kODMwLTRhM2EtYWExNi1lNjUzNzBkN2Q5MDAvIiwib2lkIjoiOGU0NDQ3YmUtMDYxMi00MmJhLTg4MzgtYjRhMDI2YTYwMThmIiwicmgiOiIwLkFSOEF0N1lzMUREWU9rcXFGdVpUY05mWkFNVlRwM0dCQ2ZoSWhhcFFoUkNwX3lzZkFBQS4iLCJyb2xlcyI6WyJUZW5hbnQuUmVhZFdyaXRlLkFsbCJdLCJzdWIiOiI4ZTQ0NDdiZS0wNjEyLTQyYmEtODgzOC1iNGEwMjZhNjAxOGYiLCJ0aWQiOiJkNDJjYjZiNy1kODMwLTRhM2EtYWExNi1lNjUzNzBkN2Q5MDAiLCJ1dGkiOiJfUzk3OUxaOFprYXZHVDVBZFZsRUFBIiwidmVyIjoiMS4wIn0.Ad77sgEeqmrOKirhCqZwCJVb7TouNg0xi_E8vWK7d0674vew6twpPOA5G-ILDCX9yO3GR6jsSk7_zKLOLfu5QVij5--iYvmtznZeYo3QPh8aU5kQM2PDYHI2kpccdW3TTbRFDCU9o8uwhqf4pPYvw41JEPz4Yree4NFRPaUjd5tVTltR76qMHjUKLXMFNhKZbPGdpZoF5ZaCLh-bG8rPFJnKilH3S3OBRSYmdWv9ILnH3xvaAHIKPtvBvhisnCvC_S8PAi2BE6vH98Sfrf1Oql8zdh48OBf8p-qK4V4SzWNGNXMPMNrstB4dcnYvDkP2V0ZQCCQWG7An_knFYRDfmQ");
            
            /*******************************************************
                Define Query (as a Command)
                - the AdomdCommant uses the above connection
                - subsitute this for your own query
            *******************************************************/
 
            String query = @"EVALUATE (Sales)";
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