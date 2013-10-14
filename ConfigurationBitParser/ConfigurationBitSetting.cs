using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HtmlAgilityPack;

namespace ConfigurationBitSetting
{
    
    class ChipInfoReader
    {
        string htmlDirectoryPath = null;
        List<ConfigBits> bits;

        public ChipInfoReader(string htmlDirectoryPath, List<ConfigBits> bits)
        {
            this.htmlDirectoryPath = htmlDirectoryPath;
            this.bits = bits;
        }

        public List<ConfigBits> getBits()
        {
            return bits;
        }

        public void Read(string htmlPath)
        {
            int numberOfConfigBits = 0;

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load(htmlPath);
            if (doc.DocumentNode != null)
            {
                HtmlNodeCollection tableNodes = doc.DocumentNode.SelectNodes("//table");
                numberOfConfigBits = tableNodes.Count();
                //Console.WriteLine(numberOfConfigBits.ToString());
                int count = 1;
                foreach (HtmlNode table in tableNodes)
                {
                    ConfigBits bit = new ConfigBits();
                    //Select each configuration bit
                    HtmlNodeCollection tableBody = table.ChildNodes;

                    HtmlNode configBitNameNode = table.SelectSingleNode("/html[1]/body[1]/table[" + count.ToString() + "]/tr[1]/td[1]");
                    HtmlNode configBitDescriptionNode = table.SelectSingleNode("/html[1]/body[1]/table[" + count.ToString() + "]/tr[1]/td[2]");

                    bit.setConfigBitName(configBitNameNode.InnerText);
                    bit.setConfigBitDescription(configBitDescriptionNode.InnerText);


                    HtmlNodeCollection configValueNodes = table.SelectNodes("/html[1]/body[1]/table[" + count.ToString() + "]/tr[position()>1]/td[1]");
                    HtmlNodeCollection configDescriptionNodes = table.SelectNodes("/html[1]/body[1]/table[" + count.ToString() + "]/tr[position()>1]/td[2]");
                    foreach (HtmlNode value in configValueNodes)
                    {
                        bit.addConfigSetting(value.InnerText);
                    }
                    foreach (HtmlNode description in configDescriptionNodes)
                    {
                        bit.addConfigDescription(description.InnerText);
                    }

                    bits.Add(bit);
                    count++;
                }
            }
        }
    }
    class ConfigBits
    {
        string configBitName;
        string configBitDescription;
        List<string> configSetting;
        List<string> configDescription;

        public ConfigBits()
        {
            this.configSetting = new List<string>();
            this.configDescription = new List<string>();
        }

        public void setConfigBitName(string configBitName)
        {
            this.configBitName = configBitName;
        }

        public void setConfigBitDescription(string configBitDescription)
        {
            this.configBitDescription = configBitDescription;
        }

        public void setConfigSetting(List<string> configSetting)
        {
            this.configSetting = configSetting;
        }

        public void setConfigDescription(List<string> configDescription)
        {
            this.configDescription = configDescription;
        }

        public void addConfigInfo(string configValue, string configDescription)
        {
            this.configSetting.Add(configValue);
            this.configDescription.Add(configDescription);
        }

        public void addConfigSetting(string configSetting)
        {
            this.configSetting.Add(configSetting);
        }

        public void addConfigDescription(string configDescription)
        {
            this.configDescription.Add(configDescription);
        }

        public int getConfigSettingCount()
        {
            return this.configSetting.Count;
        }

        public int getConfigDescriptionCount()
        {
            return this.configDescription.Count;
        }

        public string getConfigBitName()
        {
            return configBitName;
        }

        public string getConfigBitDescription()
        {
            return configBitDescription;
        }

        public List<string> getConfigSetting()
        {
            return this.configSetting;
        }
        public string getConfigSetting(int index)
        {
            return this.configSetting[index];
        }

        public List<string> getConfigDescription()
        {
            return this.configDescription;
        }
        public string getConfigDescription(int index)
        {
            return this.configDescription[index];
        }

    }
    
}
