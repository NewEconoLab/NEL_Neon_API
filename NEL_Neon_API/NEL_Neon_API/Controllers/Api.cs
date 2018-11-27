using System;
using Newtonsoft.Json.Linq;
using NEL_Neon_API.RPC;
using NEL_Neon_API.Service;
using NEL_Common;
using NEL.helper;

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
                    compileService = new CompileService
                    {
                        debugger = CompileDebugger.getInstance(),
                        ossClient = new OssFileService(Config.param.OssAPIUrl),
                    };
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
                        result = compileService.compileFileAndOss(req.@params[0].ToString());
                        break;
                    case "compileContractFileOnly":
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
            catch (Exception ex)
            {
                LogHelper.printEx(ex);
                JsonPRCresponse_Error resE = new JsonPRCresponse_Error(req.id, -100, "Parameter Error", ex.Message);
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

