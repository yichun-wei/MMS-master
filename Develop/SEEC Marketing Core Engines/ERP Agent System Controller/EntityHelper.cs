using EzCoding.DB;
using EzCoding.SystemEngines.EntityController;

namespace Seec.Marketing.Erp.SystemEngines
{
    /// <summary>
    /// 定義資料庫的表格常數名稱。
    /// </summary>
    public partial class DBTableDefine
    {
    }

    /// <summary>
    /// 資料庫存取操作。
    /// </summary>
    internal static class EntityHelper
    {
        /// <summary>
        /// 多搜尋詞的分隔字串常數。
        /// </summary>
        public const string KEYWORD_SEPARATOR = " ";

        #region 取得資料表實體操作實例
        /// <summary>
        /// 取得資料表實體操作實例。
        /// </summary>
        /// <param name="connInfo">資料庫連線資訊。</param>
        /// <param name="tableName">資料表名稱。</param>
        internal static IEntityBase GetEntity(string connInfo, string tableName)
        {
            DataAccess dataAccess = new EzCoding.DB.OracleClient.OracleDataAccess();
            dataAccess.DBConnector = new EzCoding.DB.OracleClient.OracleConnector(connInfo);

            return new Entity(DBProvider.Oracle, dataAccess, tableName);
        }
        #endregion

        #region 取得基底資料表實體操作實例
        /// <summary>
        /// 取得基底資料表實體操作實例。
        /// </summary>
        /// <param name="connInfo">資料庫連線資訊。</param>
        /// <param name="tableName">資料表名稱。</param>
        internal static IEntityBase GetEntityBase(string connInfo, string tableName)
        {
            DataAccess dataAccess = new EzCoding.DB.OracleClient.OracleDataAccess();
            dataAccess.DBConnector = new EzCoding.DB.OracleClient.OracleConnector(connInfo);

            return new EntityBase(DBProvider.Oracle, dataAccess, tableName);
        }
        #endregion
    }
}
