using System;

namespace MessageCenter.DataAccess
{
    public class DataAccessSetting
    {
        public string SqlConfigListFilePath
        {
            get;
            set;
        }
        public string DatabaseListFilePath
        {
            get;
            set;
        }

        public string EnvironmentVariable { get; set; }
    }
}
