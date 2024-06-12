using System;
using System.IO;
using Newtonsoft.Json;

namespace Seec.Marketing
{
    /// <summary>
    /// 自訂 JSON 控制類別。
    /// </summary>
    public static class CustJson
    {
        /// <summary>
        /// 序列化物件為 JSON 格式。
        /// </summary>
        public static string SerializeObject(object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// 還原 JSON 格式為 object。
        /// </summary>
        public static object DeserializeObject(string value)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject(value);
        }

        /// <summary>
        /// 還原 JSON 格式為指定物件。
        /// </summary>
        public static T DeserializeObject<T>(string value)
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new CustJsonConverter());
            return serializer.Deserialize<T>(new JsonTextReader(new StringReader(value)));
        }
    }

    class CustJsonConverter : JsonConverter
    {
        static readonly string ISID_FULLNAME = typeof(EzCoding.ISystemId).FullName;

        public override bool CanConvert(Type objectType)
        {
            var objectFullName = objectType.FullName;

            if (objectFullName == ISID_FULLNAME)
            {
                return true;
            }

            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var objectFullName = objectType.FullName;

            if (objectFullName == ISID_FULLNAME)
            {
                return serializer.Deserialize(reader, typeof(EzCoding.SystemId));
            }

            throw new NotSupportedException(string.Format("Type {0} unexpected.", objectType));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
