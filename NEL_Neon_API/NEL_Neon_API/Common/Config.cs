﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using NEL.helper;
using System;

namespace NEL_Common
{
    public class Config
    {
        public static IConfigurationRoot loadConfig(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            var config = builder.Build();
            initConfig(config);
            return config;
        }

        private static int appPort;
        public static int getAppPort()
        {
            return appPort;
        }
        private static void initConfig(IConfigurationRoot config)
        {
            initAppPort(config);
            initAppLog(config);
            initAppCfg(config);
        }
        private static void initAppPort(IConfigurationRoot config)
        {
            int port = 0;
            try
            {
                port = int.Parse(config["appPort"].ToString());
            }
            catch (Exception)
            {
            }
            if (port == 0)
            {
                port = 84;
            }
            appPort = port;
        }
        private static void initAppLog(IConfigurationRoot config)
        {
            string logCfg = null;
            try
            {
                logCfg = config["logCfg"].ToString();
            }
            catch (Exception)
            {
            }

            if (logCfg == null || logCfg.Trim().Length == 0)
            {
                logCfg = "log4net.config";
            }
            LogHelper.initLogger(logCfg);
        }

        public static ConfigParam param;
        private static void initAppCfg(IConfigurationRoot config)
        {
            param = new ConfigParam
            {
                OssAPIUrl = config["OssAPIUrl"].ToString()
            };
        }

    }
    public class ConfigParam
    {
        public string OssAPIUrl { get; set; }
    }
}
