using System;

namespace Seec.Marketing
{
    /// <summary>
    /// 功能權限的操作類別。
    /// </summary>
    /// <remarks></remarks>
    public class FunctionRight
    {
        /// <summary>
        /// 初始化 FunctionRight 類別的新執行個體。
        /// </summary>
        public FunctionRight()
        {

        }

        /// <summary>
        /// 初始化 FunctionRight 類別的新執行個體。
        /// </summary>
        /// <param name="rightsCode">表示檢視(V)、維護(M)、刪除(D) 、批次作業(B) 組成的權限代碼。</param>
        public FunctionRight(string rightsCode)
        {
            this.SetRight(rightsCode);
        }

        /// <summary>
        /// 取得是否擁有檢視權限。
        /// </summary>
        public bool View { get; private set; }

        /// <summary>
        /// 取得是否擁有維護權限。
        /// </summary>
        public bool Maintain { get; private set; }

        /// <summary>
        /// 取得是否擁有刪除權限。
        /// </summary>
        public bool Delete { get; private set; }

        /// <summary>
        /// 取得是否擁有批次作業權限。
        /// </summary>
        public bool Batch { get; private set; }

        /// <summary>
        /// 依簡碼設定功能權限。
        /// </summary>
        /// <param name="brevityCode">表示檢視(V)、維護(M)、刪除(D)、批次作業(B) 的權限簡碼字元。</param>
        public void SetRight(char brevityCode)
        {
            switch (brevityCode)
            {
                case 'V':
                    this.View = true;
                    break;
                case 'M':
                    this.Maintain = true;
                    break;
                case 'D':
                    this.Delete = true;
                    break;
                case 'B':
                    this.Batch = true;
                    break;
            }
        }

        /// <summary>
        /// 依權限代碼設定功能權限。
        /// </summary>
        /// <param name="rightsCode">表示檢視(V)、維護(M)、刪除(D)、批次作業(B) 組成的權限代碼。</param>
        public void SetRight(string rightsCode)
        {
            this.View = rightsCode.IndexOf("V") != -1;
            this.Maintain = rightsCode.IndexOf("M") != -1;
            this.Delete = rightsCode.IndexOf("D") != -1;
            this.Batch = rightsCode.IndexOf("B") != -1;
        }
    }
}