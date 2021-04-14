using System;
using System.Collections.Generic;
using Microsoft.Exchange.PushNotifications.Utils;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class GcmPayloadReader
	{
		public Dictionary<string, string> Read(string payload)
		{
			return GcmPayloadReader.PropertyReader.Read(payload);
		}

		private static readonly PropertyReader PropertyReader = new PropertyReader(new string[]
		{
			"&",
			"\n",
			"\r\n"
		}, new string[]
		{
			"="
		});
	}
}
