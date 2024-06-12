using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

using EzCoding;

namespace Seec.Marketing
{
    /// <summary>
    /// 通用工具。
    /// </summary>
    public static class ComUtil
    {
        #region PresaveOperation
        /// <summary>
        /// 儲存檔案的前置操作。
        /// </summary>
        /// <param name="dirPath">指定目錄的絕對路徑。</param>
        public static void PresaveOperation(string dirPath)
        {
            ComUtil.PresaveOperation(dirPath, true);
        }

        /// <summary>
        /// 儲存檔案的前置操作。
        /// </summary>
        /// <param name="dirPath">指定目錄的絕對路徑。</param>
        /// <param name="clearAllFiles">目錄存在時是否清空該目錄的所有檔案。</param>
        public static void PresaveOperation(string dirPath, bool clearAllFiles)
        {
            if (!Directory.Exists(dirPath))
            {
                //不存在則自動建立相對應的目錄
                Directory.CreateDirectory(dirPath);
            }
            else
            {
                if (clearAllFiles)
                {
                    //清空該目錄底下所有檔案
                    ComUtil.DeleteFilesInDirectory(dirPath, SearchOption.AllDirectories);
                }
            }
        }
        #endregion

        #region DeleteFilesInDirectory
        /// <summary>
        /// 刪除指定目錄下的檔案。
        /// </summary>
        /// <param name="dirPath">目錄路徑。</param>
        /// <param name="option">指示刪除指定目錄下的所有檔案外，是否也包含所有子目錄的所有檔案。</param>
        public static void DeleteFilesInDirectory(string dirPath, SearchOption option)
        {
            if (string.IsNullOrEmpty(dirPath))
            {
                return;
            }

            if (Directory.Exists(dirPath))
            {
                ComUtil.DeleteFilesInDirectory(new DirectoryInfo(dirPath), option);
            }
        }

        /// <summary>
        /// 刪除指定目錄下的檔案。
        /// </summary>
        /// <param name="dirPath">System.IO.DirectoryInfo。</param>
        /// <param name="option">指示刪除指定目錄下的所有檔案外，是否也包含所有子目錄的所有檔案。</param>
        public static void DeleteFilesInDirectory(DirectoryInfo dirPath, SearchOption option)
        {
            try
            {
                //清空該目錄底下所有目錄和檔案
                var files = dirPath.GetFiles("*", option);
                foreach (var file in files)
                {
                    file.Attributes = FileAttributes.Normal;
                    file.Delete();
                }
            }
            catch (IOException)
            {
                //不做任何動作
            }
        }
        #endregion

        #region DeleteDirectory
        /// <summary>
        /// 刪除目錄及其子目錄和檔案。
        /// </summary>
        /// <param name="dirPath">目錄路徑。</param>
        public static void DeleteDirectory(string dirPath)
        {
            if (string.IsNullOrEmpty(dirPath))
            {
                return;
            }

            if (Directory.Exists(dirPath))
            {
                try
                {
                    //清空該目錄底下所有目錄和檔案
                    ComUtil.DeleteFilesInDirectory(dirPath, SearchOption.AllDirectories);

                    DirectoryInfo baseDir = new DirectoryInfo(dirPath);
                    var dirs = baseDir.GetDirectories("*", SearchOption.AllDirectories);
                    foreach (var dir in dirs)
                    {
                        dir.Attributes = FileAttributes.Normal;
                    }
                    baseDir.Delete(true);
                }
                catch (IOException)
                {
                    //不做任何動作
                }
            }
        }
        #endregion

        #region GetMD5HashValue
        /// <summary>
        /// 取得 MD5 雜湊值。
        /// </summary>
        /// <param name="value">要雜湊的字串。</param>
        public static string GetMD5HashValue(string value)
        {
            return EzCoding.Security.Cryptography.MD5Lib.GetMD5HashValue(value);
        }
        #endregion

        #region 取得顯示用的電話
        /// <summary>
        /// 取得顯示用的電話。
        /// </summary>
        /// <param name="zip">區碼。</param>
        /// <param name="tel">電話。</param>
        public static string ToDispTel(string zip, string tel)
        {
            if (string.IsNullOrEmpty(zip) && string.IsNullOrEmpty(tel))
            {
                return string.Empty;
            }
            else if (string.IsNullOrEmpty(zip))
            {
                return tel;
            }
            else
            {
                return string.Format("{0}-{1}", zip, tel);
            }
        }
        #endregion
    }
}