﻿using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1
{
    class ImageMessage : IAzureQueueMessage
    {
        private string destination;

        public ImageMessage(string destinationSpot)
        {
            this.destination = destinationSpot;
        }

        public string GetSerializedContent()
        {
            return JsonConvert.SerializeObject(this, SerializerSettings.JsonSettings);
        }

        public string GetStringContent()
        {
            return JsonConvert.SerializeObject(this, SerializerSettings.JsonSettings);
        }
    }
}
