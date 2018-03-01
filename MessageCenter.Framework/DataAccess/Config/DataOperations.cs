using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Xml.Serialization;

namespace MessageCenter.DataAccess
{
    [XmlRoot("dataOperations")]
    public class DataOperations
    {
        [XmlElement("dataCommand")]
        public DataCommandConfig[] DataCommand
        {
            get;
            set;
        }
    }

    [XmlRoot("dataCommand")]
    public class DataCommandConfig
    {
        private CommandType m_CommandType = CommandType.Text;
        private int m_TimeOut = 300;
        [XmlElement("commandText")]
        public string CommandText
        {
            get;
            set;
        }
        [XmlAttribute("name")]
        public string Name
        {
            get;
            set;
        }
        [XmlAttribute("database")]
        public string Database
        {
            get;
            set;
        }
        [DefaultValue(CommandType.Text), XmlAttribute("commandType")]
        public CommandType CommandType
        {
            get
            {
                return this.m_CommandType;
            }
            set
            {
                this.m_CommandType = value;
            }
        }
        [DefaultValue(300), XmlAttribute("timeOut")]
        public int TimeOut
        {
            get
            {
                return this.m_TimeOut;
            }
            set
            {
                this.m_TimeOut = value;
            }
        }
        public DataCommandConfig Clone()
        {
            return new DataCommandConfig
            {
                CommandText = this.CommandText,
                CommandType = this.CommandType,
                Database = this.Database,
                Name = this.Name,
                TimeOut = this.TimeOut
            };
        }
    }
}
