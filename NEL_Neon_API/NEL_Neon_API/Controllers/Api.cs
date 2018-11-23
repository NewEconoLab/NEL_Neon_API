using System;
using Newtonsoft.Json.Linq;
using NEL_Neon_API.RPC;
using NEL_Neon_API.Service;

namespace NEL_Neon_API.Controllers
{
    public class Api
    {
        private string netnode { get; set; }

        CompileService compileService;


        private static Api testApi = new Api("testnet");
        public static Api getTestApi() { return testApi; }

        public Api(string node)
        {
            netnode = node;
            switch (netnode)
            {
                case "testnet":
                    compileService = new CompileService();
                    break;
                case "mainnet":
                    break;
            }
        }

        public object getRes(JsonRPCrequest req, string reqAddr)
        {
            JArray result = null;
            try
            {
                switch (req.method)
                {
                    case "compileContractFile":
                        result = compileService.compileFile(req.@params[0].ToString());
                        break;

                    // test
                    case "getnodetype":
                        result = new JArray { new JObject { { "nodeType", netnode } } };
                        break;
                }
                if (result.Count == 0)
                {
                    JsonPRCresponse_Error resE = new JsonPRCresponse_Error(req.id, -1, "No Data", "Data does not exist");
                    return resE;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("errMsg:{0},errStack:{1}", e.Message, e.StackTrace);
                JsonPRCresponse_Error resE = new JsonPRCresponse_Error(req.id, -100, "Parameter Error", e.Message);
                return resE;
            }

            JsonPRCresponse res = new JsonPRCresponse();
            res.jsonrpc = req.jsonrpc;
            res.id = req.id;
            res.result = result;

            return res;
        }
    }
}

