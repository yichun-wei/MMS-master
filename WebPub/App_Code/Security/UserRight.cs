using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Xml;
using System.IO;

using EzCoding;
using EzCoding.DB;
using EzCoding.Collections;
using Seec.Marketing.SystemEngines;

namespace Seec.Marketing
{
    /// <summary>
    /// 使用者權限。
    /// </summary>
    public class UserRight
    {
        /// <summary>
        /// 取得的權限範圍。
        /// </summary>
        public enum Range
        {
            /// <summary>
            /// 僅使用者權限。
            /// 維護用。
            /// </summary>
            OnlyUser,
            /// <summary>
            /// 僅使用者群組權限。
            /// 維護用。
            /// </summary>
            OnlyUserGrp,
            /// <summary>
            /// 複合權限。
            /// </summary>
            Complex
        }

        SimpleDataSet<string, FunctionRight> _userRightList = new SimpleDataSet<string, FunctionRight>();

        /// <summary>
        /// 初始化 FunctionRight 類別的新執行個體。
        /// </summary>
        /// <param name="tgtSId">對象系統代號。</param>
        /// <param name="range">取得的權限範圍。</param>
        public UserRight(ISystemId tgtSId, Range range)
        {
            switch (range)
            {
                case Range.OnlyUser:
                    this.FillUserRight(tgtSId);
                    break;
                case Range.OnlyUserGrp:
                    this.FillUserGrpRight(ConvertLib.ToSIds(tgtSId));
                    break;
                case Range.Complex:
                    this.FillComplexRight(tgtSId);
                    break;
            }
        }

        /// <summary>
        /// 初始化 FunctionRight 類別的新執行個體。
        /// </summary>
        /// <param name="userGrpSIds">使用者群組系統代號陣列集合。</param>
        public UserRight(ISystemId[] userGrpSIds)
        {
            this.FillUserGrpRight(userGrpSIds);
        }

        #region 僅取得使用者權限
        /// <summary>
        /// 僅取得使用者權限。
        /// </summary>
        /// <param name="userSId">使用者系統代號。</param>
        void FillUserRight(ISystemId userSId)
        {
            if (userSId != null)
            {
                //預設的使用者永遠擁有 full control.
                this.IsFullControl = userSId.Equals(SystemId.GetDefaultValue(1));
            }
            else
            {
                return;
            }

            if (this.IsFullControl)
            {
                return;
            }

            Returner returner = null;
            try
            {
                returner = new SysUser(SystemDefine.ConnInfo).GetInfo(new SysUser.InfoConds(ConvertLib.ToSIds(userSId), DefVal.Strs, DefVal.Ints, DefVal.SIds), 1, SqlOrder.Clear, IncludeScope.All, new string[] { "USER_RIGHT" });
                if (returner.IsCompletedAndContinue)
                {
                    DataRow row = returner.DataSet.Tables[0].Rows[0];

                    string userRight = row["USER_RIGHT"].ToString();

                    if (!string.IsNullOrEmpty(userRight))
                    {
                        XmlDocument xmlUserRight = new XmlDocument();
                        xmlUserRight.Load(new MemoryStream(Encoding.UTF8.GetBytes(userRight)));
                        XmlNode rootUserRightNode = xmlUserRight.SelectSingleNode("UserRight/Function");
                        foreach (XmlNode node in rootUserRightNode.ChildNodes)
                        {
                            StringBuilder rigtsCode = new StringBuilder();
                            string item = null;

                            item = XmlLib.GetAttributeValue(node, "View");
                            if (!string.IsNullOrEmpty(item) && "True".Equals(item, StringComparison.OrdinalIgnoreCase)) rigtsCode.Append("V");
                            item = XmlLib.GetAttributeValue(node, "Maintain");
                            if (!string.IsNullOrEmpty(item) && "True".Equals(item, StringComparison.OrdinalIgnoreCase)) rigtsCode.Append("M");
                            item = XmlLib.GetAttributeValue(node, "Delete");
                            if (!string.IsNullOrEmpty(item) && "True".Equals(item, StringComparison.OrdinalIgnoreCase)) rigtsCode.Append("D");
                            item = XmlLib.GetAttributeValue(node, "Batch");
                            if (!string.IsNullOrEmpty(item) && "True".Equals(item, StringComparison.OrdinalIgnoreCase)) rigtsCode.Append("B");

                            this._userRightList.Add(node.Name, new FunctionRight(rigtsCode.ToString()));
                        }
                    }
                }
            }
            finally
            {
                if (returner != null) returner.Dispose();
            }
        }
        #endregion

        #region 僅取得使用者群組權限
        /// <summary>
        /// 僅取得使用者群組權限。
        /// </summary>
        /// <param name="userGrpSIds">使用者群組系統代號陣列集合。</param>
        void FillUserGrpRight(ISystemId[] userGrpSIds)
        {
            if (userGrpSIds != null)
            {
                //預設的使用者永遠擁有 full control.
                this.IsFullControl = userGrpSIds.Contains(SystemId.GetDefaultValue(1));
            }
            else
            {
                return;
            }

            if (this.IsFullControl)
            {
                return;
            }

            Returner returner = null;
            try
            {
                returner = new SysUserGrp(SystemDefine.ConnInfo).GetInfo(new SysUserGrp.InfoConds(userGrpSIds), Int32.MaxValue, SqlOrder.Clear, IncludeScope.All, new string[] { "USER_RIGHT" });
                if (returner.IsCompletedAndContinue)
                {
                    DataRow row = returner.DataSet.Tables[0].Rows[0];

                    string userRight = row["USER_RIGHT"].ToString();

                    if (!string.IsNullOrEmpty(userRight))
                    {
                        XmlDocument xmlUserRight = new XmlDocument();
                        xmlUserRight.Load(new MemoryStream(Encoding.UTF8.GetBytes(userRight)));
                        XmlNode rootUserRightNode = xmlUserRight.SelectSingleNode("UserRight/Function");
                        foreach (XmlNode node in rootUserRightNode.ChildNodes)
                        {
                            StringBuilder rigtsCode = new StringBuilder();
                            string item = null;

                            SimpleData<string, FunctionRight> right = this._userRightList.FindByKey(node.Name);
                            if (right != null)
                            {
                                if (!right.Value.View)
                                {
                                    item = XmlLib.GetAttributeValue(node, "View");
                                    if (!string.IsNullOrEmpty(item) && "True".Equals(item, StringComparison.OrdinalIgnoreCase)) right.Value.SetRight("V");
                                }

                                if (!right.Value.Maintain)
                                {
                                    item = XmlLib.GetAttributeValue(node, "Maintain");
                                    if (!string.IsNullOrEmpty(item) && "True".Equals(item, StringComparison.OrdinalIgnoreCase)) right.Value.SetRight("M");
                                }

                                if (!right.Value.Delete)
                                {
                                    item = XmlLib.GetAttributeValue(node, "Delete");
                                    if (!string.IsNullOrEmpty(item) && "True".Equals(item, StringComparison.OrdinalIgnoreCase)) right.Value.SetRight("D");
                                }

                                if (!right.Value.Batch)
                                {
                                    item = XmlLib.GetAttributeValue(node, "Batch");
                                    if (!string.IsNullOrEmpty(item) && "True".Equals(item, StringComparison.OrdinalIgnoreCase)) right.Value.SetRight("B");
                                }
                            }
                            else
                            {
                                item = XmlLib.GetAttributeValue(node, "View");
                                if (!string.IsNullOrEmpty(item) && "True".Equals(item, StringComparison.OrdinalIgnoreCase)) rigtsCode.Append("V");
                                item = XmlLib.GetAttributeValue(node, "Maintain");
                                if (!string.IsNullOrEmpty(item) && "True".Equals(item, StringComparison.OrdinalIgnoreCase)) rigtsCode.Append("M");
                                item = XmlLib.GetAttributeValue(node, "Delete");
                                if (!string.IsNullOrEmpty(item) && "True".Equals(item, StringComparison.OrdinalIgnoreCase)) rigtsCode.Append("D");
                                item = XmlLib.GetAttributeValue(node, "Batch");
                                if (!string.IsNullOrEmpty(item) && "True".Equals(item, StringComparison.OrdinalIgnoreCase)) rigtsCode.Append("B");

                                this._userRightList.Add(node.Name, new FunctionRight(rigtsCode.ToString()));
                            }
                        }
                    }
                }
            }
            finally
            {
                if (returner != null) returner.Dispose();
            }
        }
        #endregion

        #region 取得複合權限
        /// <summary>
        /// 取得複合權限。
        /// </summary>
        /// <param name="userSId">使用者系統代號。</param>
        void FillComplexRight(ISystemId userSId)
        {
            if (userSId != null)
            {
                //預設的使用者永遠擁有 full control.
                this.IsFullControl = userSId.Equals(SystemId.GetDefaultValue(1));
            }
            else
            {
                return;
            }

            if (this.IsFullControl)
            {
                return;
            }

            Returner returner = null;
            try
            {
                #region 使用者群組
                RelTab entityRelTab = new RelTab(SystemDefine.ConnInfo);
                returner = entityRelTab.GetSysUserGrpInfo(new RelTab.SysUserGrpInfoConds(ConvertLib.ToSIds(userSId), DefVal.SIds, IncludeScope.OnlyNotMarkDeletedAndEnabledBoth), Int32.MaxValue, SqlOrder.Clear, new string[] { "SID", "USER_RIGHT" });
                if (returner.IsCompletedAndContinue)
                {
                    var infos = RelTab.SysUserGrpInfo.Binding(returner.DataSet.Tables[0]);

                    foreach (var info in infos)
                    {
                        if (info.SId.Equals(SystemId.GetDefaultValue(1)))
                        {
                            this.IsFullControl = true;
                            break;
                        }

                        if (!string.IsNullOrWhiteSpace(info.UserRight))
                        {
                            string userRight = info.UserRight;

                            if (!string.IsNullOrEmpty(userRight))
                            {
                                XmlDocument xmlUserRight = new XmlDocument();
                                xmlUserRight.Load(new MemoryStream(Encoding.UTF8.GetBytes(userRight)));
                                XmlNode rootUserRightNode = xmlUserRight.SelectSingleNode("UserRight/Function");
                                foreach (XmlNode node in rootUserRightNode.ChildNodes)
                                {
                                    StringBuilder rigtsCode = new StringBuilder();
                                    string item = null;

                                    SimpleData<string, FunctionRight> right = this._userRightList.FindByKey(node.Name);
                                    if (right != null)
                                    {
                                        if (!right.Value.View)
                                        {
                                            item = XmlLib.GetAttributeValue(node, "View");
                                            if (!string.IsNullOrEmpty(item) && "True".Equals(item, StringComparison.OrdinalIgnoreCase)) right.Value.SetRight("V");
                                        }

                                        if (!right.Value.Maintain)
                                        {
                                            item = XmlLib.GetAttributeValue(node, "Maintain");
                                            if (!string.IsNullOrEmpty(item) && "True".Equals(item, StringComparison.OrdinalIgnoreCase)) right.Value.SetRight("M");
                                        }

                                        if (!right.Value.Delete)
                                        {
                                            item = XmlLib.GetAttributeValue(node, "Delete");
                                            if (!string.IsNullOrEmpty(item) && "True".Equals(item, StringComparison.OrdinalIgnoreCase)) right.Value.SetRight("D");
                                        }

                                        if (!right.Value.Batch)
                                        {
                                            item = XmlLib.GetAttributeValue(node, "Batch");
                                            if (!string.IsNullOrEmpty(item) && "True".Equals(item, StringComparison.OrdinalIgnoreCase)) right.Value.SetRight("B");
                                        }
                                    }
                                    else
                                    {
                                        item = XmlLib.GetAttributeValue(node, "View");
                                        if (!string.IsNullOrEmpty(item) && "True".Equals(item, StringComparison.OrdinalIgnoreCase)) rigtsCode.Append("V");
                                        item = XmlLib.GetAttributeValue(node, "Maintain");
                                        if (!string.IsNullOrEmpty(item) && "True".Equals(item, StringComparison.OrdinalIgnoreCase)) rigtsCode.Append("M");
                                        item = XmlLib.GetAttributeValue(node, "Delete");
                                        if (!string.IsNullOrEmpty(item) && "True".Equals(item, StringComparison.OrdinalIgnoreCase)) rigtsCode.Append("D");
                                        item = XmlLib.GetAttributeValue(node, "Batch");
                                        if (!string.IsNullOrEmpty(item) && "True".Equals(item, StringComparison.OrdinalIgnoreCase)) rigtsCode.Append("B");

                                        this._userRightList.Add(node.Name, new FunctionRight(rigtsCode.ToString()));
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion

                #region 使用者
                if (!this.IsFullControl)
                {
                    returner = new SysUser(SystemDefine.ConnInfo).GetInfo(new SysUser.InfoConds(ConvertLib.ToSIds(userSId), DefVal.Strs, DefVal.Ints, DefVal.SIds), 1, SqlOrder.Clear, IncludeScope.All, new string[] { "USER_RIGHT" });
                    if (returner.IsCompletedAndContinue)
                    {
                        DataRow row = returner.DataSet.Tables[0].Rows[0];

                        string userRight = row["USER_RIGHT"].ToString();

                        if (!string.IsNullOrEmpty(userRight))
                        {
                            XmlDocument xmlUserRight = new XmlDocument();
                            xmlUserRight.Load(new MemoryStream(Encoding.UTF8.GetBytes(userRight)));
                            XmlNode rootUserRightNode = xmlUserRight.SelectSingleNode("UserRight/Function");
                            foreach (XmlNode node in rootUserRightNode.ChildNodes)
                            {
                                StringBuilder rigtsCode = new StringBuilder();
                                string item = null;

                                SimpleData<string, FunctionRight> right = this._userRightList.FindByKey(node.Name);
                                if (right != null)
                                {
                                    if (!right.Value.View)
                                    {
                                        item = XmlLib.GetAttributeValue(node, "View");
                                        if (!string.IsNullOrEmpty(item) && "True".Equals(item, StringComparison.OrdinalIgnoreCase)) right.Value.SetRight("V");
                                    }

                                    if (!right.Value.Maintain)
                                    {
                                        item = XmlLib.GetAttributeValue(node, "Maintain");
                                        if (!string.IsNullOrEmpty(item) && "True".Equals(item, StringComparison.OrdinalIgnoreCase)) right.Value.SetRight("M");
                                    }

                                    if (!right.Value.Delete)
                                    {
                                        item = XmlLib.GetAttributeValue(node, "Delete");
                                        if (!string.IsNullOrEmpty(item) && "True".Equals(item, StringComparison.OrdinalIgnoreCase)) right.Value.SetRight("D");
                                    }

                                    if (!right.Value.Batch)
                                    {
                                        item = XmlLib.GetAttributeValue(node, "Batch");
                                        if (!string.IsNullOrEmpty(item) && "True".Equals(item, StringComparison.OrdinalIgnoreCase)) right.Value.SetRight("B");
                                    }
                                }
                                else
                                {
                                    item = XmlLib.GetAttributeValue(node, "View");
                                    if (!string.IsNullOrEmpty(item) && "True".Equals(item, StringComparison.OrdinalIgnoreCase)) rigtsCode.Append("V");
                                    item = XmlLib.GetAttributeValue(node, "Maintain");
                                    if (!string.IsNullOrEmpty(item) && "True".Equals(item, StringComparison.OrdinalIgnoreCase)) rigtsCode.Append("M");
                                    item = XmlLib.GetAttributeValue(node, "Delete");
                                    if (!string.IsNullOrEmpty(item) && "True".Equals(item, StringComparison.OrdinalIgnoreCase)) rigtsCode.Append("D");
                                    item = XmlLib.GetAttributeValue(node, "Batch");
                                    if (!string.IsNullOrEmpty(item) && "True".Equals(item, StringComparison.OrdinalIgnoreCase)) rigtsCode.Append("B");

                                    this._userRightList.Add(node.Name, new FunctionRight(rigtsCode.ToString()));
                                }
                            }
                        }
                    }
                }
                #endregion
            }
            finally
            {
                if (returner != null) returner.Dispose();
            }
        }
        #endregion

        /// <summary>
        /// 是否為完全控制。
        /// </summary>
        public bool IsFullControl { get; private set; }

        /// <summary>
        /// 取得功能權限。
        /// </summary>
        /// <param name="functionCode">權限代碼。</param>
        public FunctionRight GetFunctionRight(string functionCode)
        {
            FunctionRight functionRight = new FunctionRight();

            if (this.IsFullControl)
            {
                functionRight.SetRight("VMDB");
                return functionRight;
            }

            if (this._userRightList.Count == 0)
            {
                return functionRight;
            }

            SimpleData<string, FunctionRight> right = this._userRightList.FindByKey(functionCode);
            if (right != null)
            {
                return right.Value;
            }
            else
            {
                return functionRight;
            }
        }

        /// <summary>
        /// 依簡碼取得所有符合的功能權限代碼陣列。
        /// </summary>
        /// <param name="brevityCode">表示檢視(V)、維護(M)、刪除(D)、批次作業(B) 的權限簡碼字元。</param>
        public string[] GetFunctionRightByCode(char brevityCode)
        {
            List<string> list = new List<string>();

            bool fullControl = this.IsFullControl;

            for (int i = 0; i < this._userRightList.Count; i++)
            {
                if (fullControl)
                {
                    list.Add(this._userRightList[i].Key);
                }
                else
                {
                    if (this._userRightList[i].Value.View) list.Add(this._userRightList[i].Key);
                }
            }

            return list.ToArray();
        }
    }
}