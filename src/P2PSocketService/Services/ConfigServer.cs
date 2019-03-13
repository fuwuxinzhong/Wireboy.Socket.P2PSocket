﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Wireboy.Socket.P2PService.Models;
using System.Linq;

namespace Wireboy.Socket.P2PService.Services
{
    public static class ConfigServer
    {
        /// <summary>
        /// 配置文件名
        /// </summary>
        public const string ConfigFile = "config.ini";
        /// <summary>
        /// 日志文件名
        /// </summary>
        public const string LogFile = "P2PService.log";
        /// <summary>
        /// 配置
        /// </summary>
        public static ApplicationConfig AppSettings = new ApplicationConfig();

        /// <summary>
        /// 加载配置文件
        /// </summary>
        /// <param name="file"></param>
        public static void LoadFromFile()
        {
            if (File.Exists(ConfigFile))
            {
                StreamReader fileStream = new StreamReader(ConfigFile);
                List<PropertyInfo> properties = AppSettings.GetType().GetProperties().Where(t => t.CustomAttributes.Where(p => p.AttributeType == typeof(ConfigField)).Count() > 0).ToList();
                while (!fileStream.EndOfStream)
                {
                    string lineStr = fileStream.ReadLine();
                    if (!(String.IsNullOrEmpty(lineStr.Trim()) || lineStr.Trim().StartsWith("#")))
                    {
                        string[] lineSplit = lineStr.Split('=');
                        if (lineSplit.Length > 1)
                        {
                            string fieldName = lineSplit[0];
                            string value = lineSplit[1];
                            PropertyInfo property = properties.Where(t => t.Name == fieldName).FirstOrDefault();
                            if (property != null)
                            {
                                property.SetValue(AppSettings, Convert.ChangeType(value, property.PropertyType));
                            }
                        }
                    }
                }
                fileStream.Close();
            }
        }
        /// <summary>
        /// 保存配置文件
        /// </summary>
        public static void SaveToFile()
        {
            StreamWriter fileStream = new StreamWriter(ConfigFile, false);
            List<PropertyInfo> properties = AppSettings.GetType().GetProperties().Where(t => t.CustomAttributes.Where(p => p.AttributeType == typeof(ConfigField)).Count() > 0).ToList();
            foreach (PropertyInfo property in properties)
            {
                ConfigField data = (ConfigField)property.GetCustomAttribute(typeof(ConfigField));
                fileStream.WriteLine("#{0}", data.Remark);
                fileStream.WriteLine("{0}={1}", property.Name, property.GetValue(AppSettings));
                fileStream.WriteLine();
            }
            fileStream.Close();
        }
    }
}