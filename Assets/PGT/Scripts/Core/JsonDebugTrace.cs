using System;
using System.Diagnostics;
using Newtonsoft.Json.Serialization;
using Udebug = UnityEngine.Debug;

namespace PGT.Core
{
    public class JsonDebugTrace : ITraceWriter
    {
        TraceLevel levelFilter;

        public JsonDebugTrace(TraceLevel level)
        {
            levelFilter = level;
        }

        public TraceLevel LevelFilter
        {
            get
            {
                return levelFilter;
            }
        }

        public void Trace(TraceLevel level, string message, Exception ex)
        {
            switch (level)
            {
                case TraceLevel.Error:
                    Udebug.LogError(message);
                    break;
                case TraceLevel.Warning:
                    Udebug.LogWarning(message);
                    break;
                case TraceLevel.Info:
                    Udebug.Log(message);
                    break;
            }
            if (ex != null) Udebug.Log(ex);
        }
    }
}
