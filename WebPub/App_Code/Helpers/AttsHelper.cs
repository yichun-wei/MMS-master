using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;
using System.IO;
using System.Globalization;

using EzCoding;
using EzCoding.Collections;
using EzCoding.DB;
using EzCoding.Web.UI;
using Seec.Marketing;
using Seec.Marketing.SystemEngines;

namespace Seec.Marketing
{
    /// <summary>
    /// 附加資料檔 Helper。
    /// </summary>
    public class AttsHelper
    {
        #region 附加資料檔資訊
        /// <summary>
        /// 附加資料檔資訊。
        /// </summary>
        public class AttsInfo
        {
            /// <summary>
            /// 附加資料檔系統代號。
            /// </summary>
            public ISystemId SId { get; set; }
            /// <summary>
            /// 單元代碼。
            /// </summary>
            public int UnitCode { get; set; }
            /// <summary>
            /// 單元系統代號。
            /// </summary>
            public ISystemId UnitSId { get; set; }
            /// <summary>
            /// 自訂代碼。
            /// </summary>
            public int CustCode { get; set; }
            /// <summary>
            /// 附加資料抬頭。
            /// </summary>
            public string AttTitle { get; set; }
            /// <summary>
            /// 附加資料名稱（檔名或網址）。
            /// </summary>
            public string AttName { get; set; }
            /// <summary>
            /// 附加資料內容。
            /// </summary>
            public string AttCont { get; set; }
            /// <summary>
            /// 說明。
            /// </summary>
            public string Desc { get; set; }
            /// <summary>
            /// 附加資料的類型。
            /// </summary>
            public AttachType AttType { get; set; }
            /// <summary>
            /// 附加資料用途。
            /// </summary>
            public AttachUse AttUse { get; set; }
            /// <summary>
            /// 附加資料位置（0:不使用 1:置左 2:置中 3:置右 4:置上 5:置下）。
            /// </summary>
            public int? AttPos { get; set; }
            /// <summary>
            /// 寬度。
            /// </summary>
            public int? Width { get; set; }
            /// <summary>
            /// 高度。
            /// </summary>
            public int? Height { get; set; }
            /// <summary>
            /// 檔案大小。
            /// </summary>
            public int? FileSize { get; set; }

            /// <summary>
            /// 檔案虛擬路徑。
            /// </summary>
            public string FilePath { get; set; }
        }
        #endregion

        #region 複製附加資料檔
        /// <summary>
        /// 複製附加資料檔。
        /// </summary>
        /// <param name="actorSId">操作人系統代號。</param>
        /// <param name="srcUnitCode">來源單元代碼。</param>
        /// <param name="srcSId">來源單元系統代號。</param>
        /// <param name="srcCustCode">來源自訂代碼。</param>
        /// <param name="destUnitCode">目標單元代碼。</param>
        /// <param name="destSId">目標單元系統代號。</param>
        /// <param name="destCustCode">目標自訂代碼。</param>
        /// <param name="attTypes">附加資料類型。</param>
        /// <param name="attUse">附加資料用途。</param>
        public static void Copy(ISystemId actorSId, int srcUnitCode, ISystemId srcSId, int srcCustCode, int destUnitCode, ISystemId destSId, int destCustCode, AttachType[] attTypes, AttachUse? attUse)
        {
            Returner returner = null;
            Returner returnerTmp = null;
            try
            {
                Atts entityAtts = new Atts(SystemDefine.ConnInfo);

                var conds = new Atts.InfoConds(srcUnitCode, srcSId, srcCustCode, attTypes, attUse);

                returner = entityAtts.GetInfo(conds, Int32.MaxValue, SqlOrder.Default, IncludeScope.All);
                if (returner.IsCompletedAndContinue)
                {
                    var infos = Atts.Info.Binding(returner.DataSet.Tables[0]);

                    foreach (var info in infos)
                    {
                        var input = new
                        {
                            attsSId = new SystemId(),
                            unitCode = destUnitCode,
                            unitSId = destSId,
                            custCode = destCustCode,
                            attTitle = info.AttTitle,
                            attName = info.AttName,
                            attCont = info.AttCont,
                            desc = info.Desc,
                            attType = (AttachType)info.AttType,
                            attUse = (AttachUse)info.AttUse,
                            attPos = info.AttPos,
                            width = info.Width,
                            height = info.Height,
                            fileSize = info.FileSize,
                            sort = info.Sort
                        };

                        returnerTmp = entityAtts.Add(actorSId, input.attsSId, input.unitCode, input.unitSId, input.custCode, input.attTitle, input.attName, input.attCont, input.desc, input.attType, input.attUse, input.attPos, input.width, input.height, input.fileSize, input.sort);
                        if (returnerTmp.IsCompletedAndContinue)
                        {
                            try
                            {
                                string srcPath = WebUtilBox.MapPath(string.Format("{0}atts/{1}/", SystemDefine.UploadRoot, info.SId));
                                string destPath = WebUtilBox.MapPath(string.Format("{0}atts/{1}/", SystemDefine.UploadRoot, input.attsSId));

                                if (Directory.Exists(srcPath))
                                {
                                    ComUtil.DeleteDirectory(destPath);
                                    IOLib.CopyDirectory(srcPath, destPath);
                                }
                            }
                            catch (IOException)
                            {
                            }
                        }
                    }
                }
            }
            finally
            {
                if (returner != null) { returner.Dispose(); }
                if (returnerTmp != null) { returnerTmp.Dispose(); }
            }
        }
        #endregion
    }
}