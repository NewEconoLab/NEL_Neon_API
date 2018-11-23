using Newtonsoft.Json.Linq;
using System;

namespace NEL_Neon_API.Service
{
    public class CompileService
    {
        public CompileDebugger debugger = CompileDebugger.getInstance();
        
        public JArray compileFile(string filetext)
        {
            // 编译文件
            byte[] avmtext = null;
            string abitext = null;
            string maptext = null;
            string hash = null;
            string code = CompileResCode.SUCC;
            string errMsg = "";
            bool flag = false;
            try
            {
                flag = debugger.compile(null, filetext, out avmtext, out abitext, out maptext, out hash);
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}-->{1}", ex.Message, ex.StackTrace);
                code = CompileResCode.FAIL;
                errMsg = ex.Message;
            }
            return new JArray(){ new JObject() { { "code", code}, { "message", errMsg}, { "data", new JArray{ new JObject() {
                {"hash", hash },
                {"avm", avmtext },
                {"abi", abitext },
                {"map", maptext },
            } } } } };
        }

        class CompileResCode
        {
            public static string SUCC = "0000";
            public static string FAIL = "1001";
        }

    }
}
