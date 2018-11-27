using NEL.helper;
using NEL.lib;
using NEL_Common;
using Newtonsoft.Json.Linq;
using System;

namespace NEL_Neon_API.Service
{
    public class CompileService
    {
        public CompileDebugger debugger { get; set; }
        public OssFileService ossClient { get; set; }

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
                {"avm", avmtext.Bytes2HexString() },
                {"abi", abitext },
                {"map", maptext },
            } } } } };
        }

        public JArray compileFileAndOss(string filetext)
        {
            byte[] avmtext = null;
            string abitext = null;
            string maptext = null;
            string hash = null;
            string code = CompileResCode.SUCC;
            string errMsg = "";
            bool flag = false;
            try
            {
                // 编译文件
                flag = debugger.compile(null, filetext, out avmtext, out abitext, out maptext, out hash);
                // 上传oss
                saveCompileRes(filetext, avmtext.Bytes2HexString(), abitext, maptext, hash);
            }
            catch (Exception ex)
            {
                LogHelper.printEx(ex);
                code = CompileResCode.FAIL;
                errMsg = ex.Message;
            }
            
            return new JArray(){ new JObject() { { "code", code}, { "message", errMsg}, { "data", new JArray{ new JObject() {
                {"hash", hash }
            } } } } };
        }
        private void saveCompileRes(string filetext, string avmtext, string abitext, string maptext, string hash)
        {
            ossClient.OssFileUpload(hash + ".cs", filetext);
            ossClient.OssFileUpload(hash + ".avm", avmtext);
            ossClient.OssFileUpload(hash + ".abi.json", abitext);
            ossClient.OssFileUpload(hash + ".map.json", maptext);
        }

        class CompileResCode
        {
            public static string SUCC = "0000"; // 成功
            public static string FAIL = "1001"; // 内部错误
        }

    }
}
