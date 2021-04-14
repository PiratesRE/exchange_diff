using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MapiApplicationId
	{
		internal MapiClientType ClientType { get; private set; }

		internal string ClientInfo { get; private set; }

		internal MapiApplicationId(MapiClientType clientType, string clientInfo)
		{
			if (string.IsNullOrEmpty(clientInfo))
			{
				throw new ArgumentException("ClientInfo cannot be null/empty", "clientInfo");
			}
			this.ClientType = clientType;
			this.ClientInfo = clientInfo;
		}

		internal static bool TryCreateFromApplicationName(string applicationName, out MapiApplicationId applicationId)
		{
			applicationId = null;
			return !string.IsNullOrEmpty(applicationName) && MapiApplicationId.TryCreateFromClientInfoString("Client=" + applicationName, out applicationId);
		}

		internal static bool TryCreateFromClientInfoString(string clientInfo, out MapiApplicationId applicationId)
		{
			applicationId = null;
			try
			{
				applicationId = MapiApplicationId.FromClientInfoString(clientInfo);
				return true;
			}
			catch (ArgumentException)
			{
			}
			return false;
		}

		internal static MapiApplicationId FromClientInfoString(string clientInfo)
		{
			if (string.IsNullOrEmpty(clientInfo))
			{
				throw new ArgumentException("clientInfo cannot be null", "clientInfo");
			}
			MapiClientType clientType;
			string text;
			if (MapiApplicationId.TryGetClientType(clientInfo, out clientType, out text))
			{
				return new MapiApplicationId(clientType, clientInfo);
			}
			throw new ArgumentException("Invalid clientInfo:" + clientInfo, "clientInfo");
		}

		private static bool TryGetClientType(string applicationIdString, out MapiClientType clientType, out string clientTypeString)
		{
			clientType = MapiClientType.User;
			clientTypeString = null;
			if (string.IsNullOrEmpty(applicationIdString))
			{
				return false;
			}
			if (!applicationIdString.StartsWith("Client=", StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			int num = applicationIdString.IndexOf(';');
			if (num == -1)
			{
				clientTypeString = applicationIdString.Substring("Client=".Length);
			}
			else
			{
				clientTypeString = applicationIdString.Substring("Client=".Length, num - "Client=".Length);
			}
			return MapiApplicationId.ClientString2IdMap.TryGetValue(clientTypeString, out clientType);
		}

		internal static bool TryNormalizeClientType(ref string applicationIdString)
		{
			MapiClientType mapiClientType = MapiClientType.User;
			string text = null;
			return MapiApplicationId.TryGetClientType(applicationIdString, out mapiClientType, out text);
		}

		public string GetNormalizedClientInfo()
		{
			if (string.IsNullOrEmpty(this.ClientInfo))
			{
				return this.ClientInfo;
			}
			int num = this.ClientInfo.IndexOf(';');
			if (num != -1)
			{
				return this.ClientInfo.Substring(0, num);
			}
			return this.ClientInfo;
		}

		public string GetNormalizedClientInfoWithoutPrefix()
		{
			string normalizedClientInfo = this.GetNormalizedClientInfo();
			if (!string.IsNullOrEmpty(normalizedClientInfo) && normalizedClientInfo.StartsWith("Client=", StringComparison.OrdinalIgnoreCase))
			{
				return normalizedClientInfo.Substring("Client=".Length).ToLower();
			}
			return normalizedClientInfo.ToLower();
		}

		internal const string ClientInfoPrefix = "Client=";

		private static readonly Dictionary<string, MapiClientType> ClientString2IdMap = new Dictionary<string, MapiClientType>(StringComparer.OrdinalIgnoreCase)
		{
			{
				"ActiveSync",
				MapiClientType.AirSync
			},
			{
				"AS",
				MapiClientType.AvailabilityService
			},
			{
				"CI",
				MapiClientType.ContentIndexing
			},
			{
				"ELC",
				MapiClientType.ELC
			},
			{
				"Management",
				MapiClientType.Management
			},
			{
				"Monitoring",
				MapiClientType.Monitoring
			},
			{
				"StoreActiveMonitoring",
				MapiClientType.StoreActiveMonitoring
			},
			{
				"ResourceHealth",
				MapiClientType.Monitoring
			},
			{
				"EventBased MSExchangeMailboxAssistants",
				MapiClientType.EventBasedAssistants
			},
			{
				"EBA",
				MapiClientType.EventBasedAssistants
			},
			{
				"OWA",
				MapiClientType.OWA
			},
			{
				"POP3/IMAP4",
				MapiClientType.PopImap
			},
			{
				"UM",
				MapiClientType.UnifiedMessaging
			},
			{
				"WebServices",
				MapiClientType.WebServices
			},
			{
				"ApprovalAPI",
				MapiClientType.ApprovalAPI
			},
			{
				"TimeBased MSExchangeMailboxAssistants",
				MapiClientType.TimeBasedAssistants
			},
			{
				"TBA",
				MapiClientType.TimeBasedAssistants
			},
			{
				"MSExchangeRPC",
				MapiClientType.MoMT
			},
			{
				"MSExchangeMigration",
				MapiClientType.Migration
			},
			{
				"MSExchangeSimpleMigration",
				MapiClientType.SimpleMigration
			},
			{
				"TransportSync",
				MapiClientType.TransportSync
			},
			{
				"Hub Transport",
				MapiClientType.Transport
			},
			{
				"HUB",
				MapiClientType.Transport
			},
			{
				"HA",
				MapiClientType.HA
			},
			{
				"Maintenance",
				MapiClientType.Maintenance
			},
			{
				"Inference",
				MapiClientType.Inference
			},
			{
				"SMS",
				MapiClientType.SMS
			},
			{
				"TeamMailbox",
				MapiClientType.TeamMailbox
			},
			{
				"LoadGen",
				MapiClientType.LoadGen
			},
			{
				"PublicFolderSystem",
				MapiClientType.PublicFolderSystem
			},
			{
				"EDiscoverySearch",
				MapiClientType.EDiscoverySearch
			},
			{
				"CIMoveDestination",
				MapiClientType.ContentIndexingMoveDestination
			},
			{
				"AnchorService",
				MapiClientType.AnchorService
			},
			{
				"MSExchangeMailboxLoadBalance",
				MapiClientType.MailboxLoadBalance
			},
			{
				"OutlookService",
				MapiClientType.OutlookService
			},
			{
				"UnifiedPolicy",
				MapiClientType.UnifiedPolicy
			},
			{
				"NotificationBroker",
				MapiClientType.NotificationBroker
			},
			{
				"Pop",
				MapiClientType.Pop
			},
			{
				"LiveIdBasicAuth",
				MapiClientType.LiveIdBasicAuth
			},
			{
				"ADDriver",
				MapiClientType.ADDriver
			},
			{
				"SnackyService",
				MapiClientType.SnackyService
			}
		};
	}
}
