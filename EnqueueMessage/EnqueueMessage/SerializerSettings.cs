using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnqueueMessage
{
    using System.Runtime.Serialization.Formatters;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// This class provides the serialization settings
    /// </summary>
    public static class SerializerSettings
    {
        /// <summary>
        /// The ignore error handler setting.
        /// </summary>
        private static JsonSerializerSettings ignoreErrorHandlerSetting;

        /// <summary>
        /// The JSON settings.
        /// </summary>
        private static JsonSerializerSettings jsonSettings;

        /// <summary>
        /// Gets Serializer Settings
        /// </summary>
        public static JsonSerializerSettings JsonSettings
        {
            get
            {
                if (jsonSettings == null)
                {
                    jsonSettings = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto,
                        TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                        NullValueHandling = NullValueHandling.Ignore,
                        SerializationBinder = new DefaultSerializationBinder() //IngestionSerializationBinder()
                    };
                }

                return jsonSettings;
            }
        }

        /// <summary>
        /// Gets Serializer setting which ignores any parsing error and moves further
        /// </summary>
        public static JsonSerializerSettings IgnoreErrorHandlerSetting
        {
            get
            {
                if (ignoreErrorHandlerSetting == null)
                {
                    ignoreErrorHandlerSetting = new JsonSerializerSettings
                    {
                        Error = (sender, args) => { args.ErrorContext.Handled = true; }
                    };
                }

                return ignoreErrorHandlerSetting;
            }
        }
    }
}
