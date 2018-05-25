﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using bossdoyKaraoke_NOW.Properties;
using Microsoft.Win32;

namespace bossdoyKaraoke_NOW.Models
{
    class AppSettings
    {
        static Configuration config;
        static string appSetting;
        static TypeConverter converter;

        public static void Set(string key, string value)
        {
            config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            var entry = config.AppSettings.Settings[key];
            if (entry == null)
                config.AppSettings.Settings.Add(key, value);
            else
                config.AppSettings.Settings[key].Value = value;

            config.Save(ConfigurationSaveMode.Modified);
        }

        public static T Get<T>(string key)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
            config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            try
            {
                appSetting = config.AppSettings.Settings[key].Value;
            }
            catch (NullReferenceException) {

                return default(T);
            }

            return (T)(converter.ConvertFromInvariantString(appSetting));
        }

        public static T ConvertTo<T>(string value)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
            return (T)converter.ConvertFromInvariantString(value);
        }

        public static void SetCH1EQ1band()
        {
            Settings.Default.Properties.Cast<SettingsProperty>().OrderBy(s => s.Name).Select(d =>
            {
                bool isname = d.Name.Contains("DEFCH1EQ1band");
                string s = string.Empty;
                if (isname)
                {
                    string name = d.Name.Remove(0, 3);
                    Set(name, d.DefaultValue.ToString());
                }

                return string.Empty;

            }).ToArray();
        }

        public static void SetFxDefaultSettings(string key, string replaceOldStringPortionOfKey = null, string newStringPortionOfKey = null)
        {
            Settings.Default.Properties.Cast<SettingsProperty>().OrderBy(s => s.Name).Select(d =>
            {
                bool isname = d.Name.Contains(key);
                string name = string.Empty;
                if (isname)
                {
                    if (replaceOldStringPortionOfKey == null && newStringPortionOfKey == null)
                        name = d.Name.Remove(0, 3);
                    else
                        name = d.Name.Remove(0, 3).Replace(replaceOldStringPortionOfKey, newStringPortionOfKey);

                    Set(name, d.DefaultValue.ToString());
                }

                return string.Empty;

            }).ToArray();
        }


        public static bool IsApplictionInstalled(string p_name)
        {
            //Code is from: http://mdb-blog.blogspot.com/2010/09/c-check-if-programapplication-is.html

            string keyName;

            // search in: CurrentUser
            keyName = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            if (ExistsInSubKey(Registry.CurrentUser, keyName, "DisplayName", p_name) == true)
            {
                return true;
            }

            // search in: LocalMachine_32
            keyName = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            if (ExistsInSubKey(Registry.LocalMachine, keyName, "DisplayName", p_name) == true)
            {
                return true;
            }

            // search in: LocalMachine_64
            keyName = @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall";
            if (ExistsInSubKey(Registry.LocalMachine, keyName, "DisplayName", p_name) == true)
            {
                return true;
            }

            return false;
        }

        private static bool ExistsInSubKey(RegistryKey p_root, string p_subKeyName, string p_attributeName, string p_name)
        {
            RegistryKey subkey;
            string displayName;

            using (RegistryKey key = p_root.OpenSubKey(p_subKeyName))
            {
                if (key != null)
                {
                    foreach (string kn in key.GetSubKeyNames())
                    {
                        using (subkey = key.OpenSubKey(kn))
                        {
                            displayName = subkey.GetValue(p_attributeName) as string;
                            // if (p_name.Equals(displayName, StringComparison.OrdinalIgnoreCase) == true)
                            //  if(d)
                            if (displayName != null)
                            {
                                if (displayName.Contains(p_name) == true)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}
