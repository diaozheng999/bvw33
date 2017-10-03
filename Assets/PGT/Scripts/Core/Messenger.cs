using System.Collections.Generic;
using PGT.Core.Func;

namespace PGT.Core
{
    public static class Messenger
    {
        static Dictionary<string, object> messages = new Dictionary<string, object>();

        public static bool SendMessage<T>(string key, T message)
        {
            if (messages.ContainsKey(key)) return false;
            messages.Add(key, message);
            return true;
        }

        public static void UpdateMessage<T>(string key, T message)
        {
            messages[key] = message;
        }

        public static bool HasMessage(string key)
        {
            return messages.ContainsKey(key);
        }

        public static T GetMessage<T>(string key)
        {
            if (!messages.ContainsKey(key)) return default(T);
            return (T)messages[key];
        }

        public static void ClearMessages()
        {
            messages = new Dictionary<string, object>();
        }
    }
}
