using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

using EzCoding;
using EzCoding.Web.UI;

namespace Seec.Marketing
{
    /// <summary>
    /// 主選單功能表。
    /// </summary>
    public class MainManu
    {
        JQueryMenu _menu;
        XmlDocument _menuXml;
        string[] _userRight;
        string _noneNavigateUrl = "javascript:void(0)";
        string _baseUrl;
        bool _fullControl;

        /// <summary>
        /// 初始化 EzCoding.Web.UI.JQueryMenu 類別的新執行個體。
        /// </summary>
        /// <param name="menu">System.Web.UI.WebControls.Menu。</param>
        /// <param name="menuXml">符合選單 XML 格式的 System.Xml.XmlDocument。</param>
        /// <param name="baseUrl">基底網址。若選單中指定的連結網址為絕對網址，則維持選單中指定的連結網址，否則將在選單指定的連結網址前附加指定的基底網址。</param>
        /// <exception cref="System.ArgumentNullException">menuXml 為 null 值。</exception>
        public MainManu(JQueryMenu menu, XmlDocument menuXml, string baseUrl)
        {
            if (menuXml == null) { throw new ArgumentNullException("menuXml"); }
            this._menu = menu;
            this._menuXml = menuXml;
            this._baseUrl = baseUrl;
        }

        /// <summary>
        /// 初始化 EzCoding.Web.UI.JQueryMenu 類別的新執行個體。
        /// </summary>
        /// <param name="menu">System.Web.UI.WebControls.Menu。</param>
        /// <param name="menuXml">符合選單 XML 格式的 System.Xml.XmlDocument。</param>
        /// <param name="baseUrl">基底網址。若選單中指定的連結網址為絕對網址，則維持選單中指定的連結網址，否則將在選單指定的連結網址前附加指定的基底網址。</param>
        /// <param name="fullControl">是否為完全控制。若為 true，則會忽略 XML 選單中的權限代碼。</param>
        /// <exception cref="System.ArgumentNullException">menuXml 為 null 值。</exception>
        public MainManu(JQueryMenu menu, XmlDocument menuXml, string baseUrl, bool fullControl)
        {
            if (menuXml == null) { throw new ArgumentNullException("menuXml"); }
            this._menu = menu;
            this._menuXml = menuXml;
            this._baseUrl = baseUrl;
            this._fullControl = fullControl;
        }

        /// <summary>
        /// 初始化 EzCoding.Web.UI.JQueryMenu 類別的新執行個體。
        /// </summary>
        /// <param name="menu">System.Web.UI.WebControls.Menu。</param>
        /// <param name="menuXml">符合選單 XML 格式的 System.Xml.XmlDocument。</param>
        /// <param name="baseUrl">基底網址。若選單中指定的連結網址為絕對網址，則維持選單中指定的連結網址，否則將在選單指定的連結網址前附加指定的基底網址。</param>
        /// <param name="userRight">使用者權限。</param>
        /// <exception cref="System.ArgumentNullException">menuXml 為 null 值。</exception>
        public MainManu(JQueryMenu menu, XmlDocument menuXml, string baseUrl, string[] userRight)
        {
            if (menuXml == null) { throw new ArgumentNullException("menuXml"); }
            this._menu = menu;
            this._menuXml = menuXml;
            this._baseUrl = baseUrl;
            this._userRight = userRight;
        }

        /// <summary>
        /// 初始化 EzCoding.Web.UI.JQueryMenu 類別的新執行個體。
        /// </summary>
        /// <param name="menu">System.Web.UI.WebControls.Menu。</param>
        /// <param name="menuXml">符合選單 XML 格式的 System.Xml.XmlDocument。</param>
        /// <param name="baseUrl">基底網址。若選單中指定的連結網址為絕對網址，則維持選單中指定的連結網址，否則將在選單指定的連結網址前附加指定的基底網址。</param>
        /// <param name="fullControl">是否為完全控制。若為 true，則會忽略 XML 選單中的權限代碼。</param>
        /// <param name="userRight">使用者權限。</param>
        /// <exception cref="System.ArgumentNullException">menuXml 為 null 值。</exception>
        public MainManu(JQueryMenu menu, XmlDocument menuXml, string baseUrl, bool fullControl, string[] userRight)
        {
            if (menuXml == null) { throw new ArgumentNullException("menuXml"); }
            this._menu = menu;
            this._menuXml = menuXml;
            this._baseUrl = baseUrl;
            this._userRight = userRight;
            this._fullControl = fullControl;
        }

        /// <summary>
        /// 在指定的 System.Web.UI.WebControls.Menu 中加入所有選單項目。
        /// </summary>
        public void Fill()
        {
            JQueryMenuItemCollection items = new JQueryMenuItemCollection();
            XmlNode node = this._menuXml.SelectSingleNode("Menu/Items");

            this.AddRootMenuItem(node, items);
            for (int i = 0; i < items.Count; i++)
            {
                this._menu.Items.Add(items[i]);
            }
        }

        void AddRootMenuItem(XmlNode node, JQueryMenuItemCollection items)
        {
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (!this.CheckUserRight(childNode))
                {
                    continue;
                }

                JQueryMenuItem childItem = this.FillMenuItem(childNode);
                this.AddChildMenuItem(childNode, childItem);

                if ((string.Compare(childItem.Url, this._noneNavigateUrl, StringComparison.OrdinalIgnoreCase) != 0 || childItem.ChildItems.Count > 0))
                {
                    items.Add(childItem);
                }
            }
        }

        void AddChildMenuItem(XmlNode node, JQueryMenuItem item)
        {
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (!this.CheckUserRight(childNode))
                {
                    continue;
                }

                JQueryMenuItem childItem = this.FillMenuItem(childNode);
                this.AddChildMenuItem(childNode, childItem);

                if ((string.Compare(childItem.Url, this._noneNavigateUrl, StringComparison.OrdinalIgnoreCase) != 0 || childItem.ChildItems.Count > 0))
                {
                    item.ChildItems.Add(childItem);
                }
            }
        }

        /// <summary>
        /// 移除指定的權限節點。
        /// </summary>
        /// <param name="node">節點。</param>
        /// <param name="authCode">權限代碼。</param>
        public static void RemoveAuthNode(XmlNode node, string authCode)
        {
            MainManu.RemoveNode(node, "AuthCode", authCode);
        }

        /// <summary>
        /// 移除指定的節點。
        /// </summary>
        /// <param name="node">節點。</param>
        /// <param name="attr">屬性名稱。</param>
        /// <param name="value">屬性值。</param>
        public static void RemoveNode(XmlNode node, string attr, string value)
        {
            if (string.IsNullOrWhiteSpace(attr) || string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            foreach (XmlNode xn in node.ChildNodes)
            {
                XmlElement xe = (XmlElement)xn;
                string attrVal = xe.GetAttribute(attr);
                if (!string.IsNullOrWhiteSpace(attrVal) && attrVal == value)
                {
                    xn.RemoveAll();
                }

                MainManu.RemoveNode(xn, attr, value);
            }
        }

        bool CheckUserRight(XmlNode node)
        {
            //若為完全控制，則一律回傳 true。
            if (this._fullControl) { return true; }

            string authoritiy = XmlLib.GetAttributeValue(node, "AuthCode");
            if (!string.IsNullOrWhiteSpace(authoritiy))
            {
                if (this._userRight == null || this._userRight.Length == 0)
                {
                    //選單有設定權限，但使用者權限為 null 或陣列長度為 0。
                    return false;
                }

                for (int i = 0; i < this._userRight.Length; i++)
                {
                    if (string.Compare(this._userRight[i], XmlLib.GetAttributeValue(node, "AuthCode"), StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                //null 表示未限定使用權限。
                return true;
            }
        }

        #region FillMenuItem
        /// <summary>
        /// 依指定的 XML 節點設定選單項目。
        /// </summary>
        /// <param name="node">System.Xml.XmlNode。</param>
        /// <returns>System.Web.UI.WebControls.Menu。</returns>
        JQueryMenuItem FillMenuItem(XmlNode node)
        {
            JQueryMenuItem item = new JQueryMenuItem();
            foreach (XmlAttribute attribute in node.Attributes)
            {
                switch (attribute.Name)
                {
                    case "Name":
                        item.Text = attribute.Value;
                        item.Title = item.Text;
                        break;
                    case "ToolTip":
                        item.Title = attribute.Value;
                        break;
                    case "NavigateUrl":
                        Uri uri;
                        if (string.IsNullOrWhiteSpace(attribute.Value) || string.IsNullOrWhiteSpace(this._baseUrl) || Uri.TryCreate(attribute.Value, UriKind.Absolute, out uri))
                        {
                            //若選單指定網址為空值或未指定基底網址或選單指定網址為絕對網址則維持原值。
                            item.Url = attribute.Value;
                        }
                        else
                        {
                            item.Url = this._baseUrl + attribute.Value;
                        }
                        break;
                    case "Target":
                        item.Target = attribute.Value;
                        break;
                    case "Visible":
                        item.Visible = Convert.ToBoolean(attribute.Value);
                        break;
                }
            }
            if (string.IsNullOrWhiteSpace(item.Url))
            {
                item.Url = this._noneNavigateUrl;
            }
            if (string.IsNullOrWhiteSpace(item.Title))
            {
                item.Title = item.Text;
            }
            return item;
        }
        #endregion
    }
}
