using LitJson;

namespace Game.Editor
{
    /// <summary>
    /// 编辑器通用 Json 序列化工具。
    /// Game.LitJsonHelper 用于运行时。
    /// </summary>
    public static class JsonUtility
    {
        private static readonly JsonWriter JsonWriter = new JsonWriter { PrettyPrint = true, Validate = true };

        /// <summary>
        /// 将对象序列化为 JSON 字符串。
        /// </summary>
        /// <param name="obj">要序列化的对象。</param>
        /// <returns>序列化后的 JSON 字符串。</returns>
        public static string ToJson(object obj)
        {
            JsonWriter.Reset();
            JsonMapper.ToJson(obj, JsonWriter);
            return JsonWriter.ToString();
        }

        /// <summary>
        /// 将 JSON 字符串反序列化为对象。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="json">要反序列化的 JSON 字符串。</param>
        /// <returns>反序列化后的对象。</returns>
        public static T ToObject<T>(string json)
        {
            return JsonMapper.ToObject<T>(json);
        }
    }
}
