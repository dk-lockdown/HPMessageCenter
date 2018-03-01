using MessageCenter.Framework.Cache;
using MessageCenter.Framework.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace MessageCenter.Portal
{
    public class MenuConfig
    {
        private static List<MenuItem> m_MenuItemList = null;

        public static List<MenuItem> GetMenuItemList()
        {
            string menuPath = Path.Combine(Directory.GetCurrentDirectory(),"Configuration/Menu.config");
            List<MenuItem> milist = CacheManager.GetWithLocalCache(menuPath, _GetMenuItemList);            
            if (milist == null)
            {
                milist = new List<MenuItem>();
            }
            milist.Sort(new MenuItemCodeSort());
            return milist;
        }

        public static List<MenuItem> GetMenuTree()
        {
            List<MenuItem> allMenu = GetMenuItemList();
            List<MenuItem> rootMenus = (from m in allMenu where string.IsNullOrWhiteSpace(m.ParentMenuCode) || m.ParentMenuCode == "0" select m).ToList();
            rootMenus.ForEach(m =>
            {
                BuildTree(allMenu, m);
            });
            return rootMenus;
        }

        private static void BuildTree(List<MenuItem> allMenu, MenuItem rootMemu)
        {
            rootMemu.Children = (from m in allMenu where m.ParentMenuCode == rootMemu.MenuCode select m).ToList();
            rootMemu.Children.ForEach(m =>
            {
                BuildTree(allMenu, m);
            });
        }

        private static List<MenuItem> _GetMenuItemList()
        {
            string menuPath = Path.Combine(Directory.GetCurrentDirectory(), "Configuration/Menu.config");
            m_MenuItemList = SerializationUtility.LoadFromXml<List<MenuItem>>(menuPath);
            if (m_MenuItemList == null)
            {
                m_MenuItemList = new List<MenuItem>();
            }
            return m_MenuItemList;
        }

        public static List<MenuItem> GetMenuTreePath(MenuItem curMenuItem)
        {
            List<MenuItem> treePathList = new List<MenuItem>();
            ThroughTree(treePathList, curMenuItem);
            return treePathList;
        }

        private static void ThroughTree(List<MenuItem> treePathList, MenuItem curMenuItem)
        {
            treePathList.Insert(0, curMenuItem);
            if (curMenuItem.ParentMenuCode != "0")
            {
                List<MenuItem> allList = GetMenuItemList();
                MenuItem parentItem = allList.Find(f => f.MenuCode == curMenuItem.ParentMenuCode);
                if (parentItem != null)
                {
                    ThroughTree(treePathList, parentItem);
                }
            }
        }


    }

    [Serializable]
    public class MenuItem
    {
        [XmlAttribute]
        public string Name
        {
            get;
            set;
        }
        [XmlAttribute]
        public string MenuCode
        {
            get;
            set;
        }
        [XmlAttribute]
        public string ParentMenuCode
        {
            get;
            set;
        }
        [XmlAttribute]
        public string IsVisiable
        {
            get;
            set;
        }
        [XmlAttribute]
        public string AuthKey
        {
            get;
            set;
        }
        [XmlAttribute]
        public string LinkUrl
        {
            get;
            set;

        }
        [XmlAttribute]
        public string Icon
        {
            get;
            set;
        }

        [XmlAttribute]
        public string Class
        {
            get;
            set;
        }

        public List<MenuItem> Children { get; set; }

        public bool IsActive { get; set; }
        public bool IsOpen { get; set; }
    }


    public class MenuItemCodeSort : IComparer<MenuItem>
    {
        public int Compare(MenuItem x, MenuItem y)
        {
            return x.MenuCode.CompareTo(y.MenuCode);
        }
    }
}