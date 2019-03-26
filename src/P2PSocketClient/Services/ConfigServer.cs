﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Wireboy.Socket.P2PClient.Models;
using System.IO;

namespace Wireboy.Socket.P2PClient
{
    public static class ConfigServer
    {
        /// <summary>
        /// 配置文件名
        /// </summary>
        public const string ConfigFile = "Settings.ini";
        /// <summary>
        /// 日志文件名
        /// </summary>
        public const string LogFile = "P2PSocketClient.log";
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
            if(File.Exists(ConfigFile))
            {
                StreamReader fileStream = new StreamReader(ConfigFile);
                List<PropertyInfo> properties = AppSettings.GetType().GetProperties().Where(t => t.CustomAttributes.Where(p => p.AttributeType == typeof(ConfigField)).Count() > 0).ToList();
                while (!fileStream.EndOfStream)
                {
                    string lineStr = fileStream.ReadLine();
                    if(!(String.IsNullOrEmpty(lineStr.Trim()) || lineStr.Trim().StartsWith("#")))
                    {
                        string[] lineSplit = lineStr.Split('=');
                        if (lineSplit.Length > 1)
                        {
                            string fieldName = lineSplit[0];
                            string value = lineSplit[1];
                            PropertyInfo property = properties.Where(t => t.Name == fieldName).FirstOrDefault();
                            if (property != null)
                            {
                                try
                                {
                                    if (property.PropertyType.BaseType == typeof(Enum))
                                    {
                                        if (property.PropertyType == typeof(LogLevel))
                                        {
                                            LogLevel enumValue = ((LogLevel[])Enum.GetValues(property.PropertyType)).Where(t => t.ToString() == value).FirstOrDefault();
                                            property.SetValue(AppSettings, enumValue);
                                        }
                                        else
                                        {
                                            throw new Exception(string.Format("未配置{0}枚举的转换", property.PropertyType));
                                        }
                                    }
                                    else
                                    {
                                        property.SetValue(AppSettings, Convert.ChangeType(value, property.PropertyType));
                                    }
                                }
                                catch { }
                            }
                        }
                    }
                }
                fileStream.Close();
            }
            SaveToFile();
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
                object value = property.GetValue(AppSettings);
                fileStream.WriteLine("{0}={1}",property.Name, value);
                fileStream.WriteLine();
            }

            fileStream.Close();
        }
    }
}