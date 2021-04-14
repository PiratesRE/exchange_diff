using System;
using System.Collections.Generic;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class ClientScanResultStorage
	{
		public List<string> ClassifiedParts { get; set; }

		public List<DiscoveredDataClassification> DlpDetectedClassificationObjects { get; set; }

		public int RecoveryOptions { get; set; }

		public string DetectedClassificationIds { get; set; }

		internal ClientScanResultStorage()
		{
		}

		public static ClientScanResultStorage CreateInstance(string clientData)
		{
			if (string.IsNullOrEmpty(clientData))
			{
				return new ClientScanResultStorage
				{
					ClassifiedParts = new List<string>(),
					DetectedClassificationIds = string.Empty,
					DlpDetectedClassificationObjects = new List<DiscoveredDataClassification>(),
					RecoveryOptions = 0
				};
			}
			return clientData.ToClientScanResultStorage();
		}
	}
}
