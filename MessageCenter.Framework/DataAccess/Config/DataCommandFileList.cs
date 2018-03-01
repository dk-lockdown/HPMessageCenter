using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MessageCenter.DataAccess
{
    [XmlRoot(ElementName ="dataCommandFiles")]
    public class DataCommandFileList
    {
        public class DataCommandFile
        {
            [XmlAttribute("name")]
            public string FileName
            {
                get;
                set;
            }
        }
        [XmlElement("file")]
        public DataCommandFileList.DataCommandFile[] FileList
        {
            get;
            set;
        }
    }
}
