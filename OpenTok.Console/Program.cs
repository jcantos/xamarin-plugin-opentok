using OpenTokSDK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OpenTok.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            int ApiKey = 40000000;// api key project, not account
            string ApiSecret = "******************************";

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var openTok = new OpenTokSDK.OpenTok(ApiKey, ApiSecret);
                openTok.Debug = true;

                // Create a session that will attempt to transmit streams directly between clients
                var session = openTok.CreateSession();
                string sessionId = session.Id;
                System.Console.WriteLine("SESSION_ID:" + sessionId);

                // Create token
                string token = openTok.GenerateToken(sessionId);

                System.Console.WriteLine("TOKEN:" + token);
                System.Console.ReadLine();
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
