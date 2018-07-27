using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnqueueMessage
{
    /// <summary>
    /// This interface contains the functions for Azure Queue Message
    /// </summary>
    public interface IAzureQueueMessage
    {
        /// <summary>
        /// Gets the serialized content of the message
        /// </summary>
        /// <returns>Serialized Message as string</returns>
        string GetSerializedContent();

        /// <summary>
        /// Gets the string content of the message
        /// </summary>
        /// <returns>Message as string</returns>
        string GetStringContent();
    }    
}
