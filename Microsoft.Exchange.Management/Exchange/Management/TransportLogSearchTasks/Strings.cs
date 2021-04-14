using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.TransportLogSearchTasks
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(170709306U, "OldServerSearchLogic");
			Strings.stringIDs.Add(3002944702U, "WarningMoreResultsAvailable");
			Strings.stringIDs.Add(704769464U, "ServerTooBusy");
			Strings.stringIDs.Add(2480663381U, "Complete");
			Strings.stringIDs.Add(1997066575U, "EmptyTimeRange");
			Strings.stringIDs.Add(2255522196U, "LogNotAvailable");
			Strings.stringIDs.Add(906793263U, "OldServerSchema");
			Strings.stringIDs.Add(3194642979U, "InternalError");
			Strings.stringIDs.Add(2886675525U, "SearchTimeout");
			Strings.stringIDs.Add(2445664782U, "MessageTrackingActivityName");
			Strings.stringIDs.Add(97483436U, "QueryTooComplex");
		}

		public static LocalizedString WarningProxyAddressIsInvalid(string address, string message)
		{
			return new LocalizedString("WarningProxyAddressIsInvalid", "", false, false, Strings.ResourceManager, new object[]
			{
				address,
				message
			});
		}

		public static LocalizedString OldServerSearchLogic
		{
			get
			{
				return new LocalizedString("OldServerSearchLogic", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WarningMoreResultsAvailable
		{
			get
			{
				return new LocalizedString("WarningMoreResultsAvailable", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RpcUnavailable(string computername)
		{
			return new LocalizedString("RpcUnavailable", "", false, false, Strings.ResourceManager, new object[]
			{
				computername
			});
		}

		public static LocalizedString ServerTooBusy
		{
			get
			{
				return new LocalizedString("ServerTooBusy", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RpcNotRegistered(string computername)
		{
			return new LocalizedString("RpcNotRegistered", "", false, false, Strings.ResourceManager, new object[]
			{
				computername
			});
		}

		public static LocalizedString ServerNameAmbiguous(string server)
		{
			return new LocalizedString("ServerNameAmbiguous", "", false, false, Strings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString Complete
		{
			get
			{
				return new LocalizedString("Complete", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchStatus(string server)
		{
			return new LocalizedString("SearchStatus", "", false, false, Strings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString EmptyTimeRange
		{
			get
			{
				return new LocalizedString("EmptyTimeRange", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PreE12Server(string server)
		{
			return new LocalizedString("PreE12Server", "", false, false, Strings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString LogNotAvailable
		{
			get
			{
				return new LocalizedString("LogNotAvailable", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GenericRpcError(string message, string computername)
		{
			return new LocalizedString("GenericRpcError", "", false, false, Strings.ResourceManager, new object[]
			{
				message,
				computername
			});
		}

		public static LocalizedString ServerNotFound(string server)
		{
			return new LocalizedString("ServerNotFound", "", false, false, Strings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString OldServerSchema
		{
			get
			{
				return new LocalizedString("OldServerSchema", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GenericError(string message)
		{
			return new LocalizedString("GenericError", "", false, false, Strings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString NotTransportServer(string server)
		{
			return new LocalizedString("NotTransportServer", "", false, false, Strings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString InternalError
		{
			get
			{
				return new LocalizedString("InternalError", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventIdNotFound(string eventid)
		{
			return new LocalizedString("EventIdNotFound", "", false, false, Strings.ResourceManager, new object[]
			{
				eventid
			});
		}

		public static LocalizedString SearchTimeout
		{
			get
			{
				return new LocalizedString("SearchTimeout", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WarningProxyListUnavailable(string address, string message)
		{
			return new LocalizedString("WarningProxyListUnavailable", "", false, false, Strings.ResourceManager, new object[]
			{
				address,
				message
			});
		}

		public static LocalizedString MessageTrackingActivityName
		{
			get
			{
				return new LocalizedString("MessageTrackingActivityName", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorADTopologyServiceNotAvailable(string server, string errorMessage)
		{
			return new LocalizedString("ErrorADTopologyServiceNotAvailable", "", false, false, Strings.ResourceManager, new object[]
			{
				server,
				errorMessage
			});
		}

		public static LocalizedString MissingServerFQDN(string server)
		{
			return new LocalizedString("MissingServerFQDN", "", false, false, Strings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString QueryTooComplex
		{
			get
			{
				return new LocalizedString("QueryTooComplex", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(11);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Management.LogSearchStrings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			OldServerSearchLogic = 170709306U,
			WarningMoreResultsAvailable = 3002944702U,
			ServerTooBusy = 704769464U,
			Complete = 2480663381U,
			EmptyTimeRange = 1997066575U,
			LogNotAvailable = 2255522196U,
			OldServerSchema = 906793263U,
			InternalError = 3194642979U,
			SearchTimeout = 2886675525U,
			MessageTrackingActivityName = 2445664782U,
			QueryTooComplex = 97483436U
		}

		private enum ParamIDs
		{
			WarningProxyAddressIsInvalid,
			RpcUnavailable,
			RpcNotRegistered,
			ServerNameAmbiguous,
			SearchStatus,
			PreE12Server,
			GenericRpcError,
			ServerNotFound,
			GenericError,
			NotTransportServer,
			EventIdNotFound,
			WarningProxyListUnavailable,
			ErrorADTopologyServiceNotAvailable,
			MissingServerFQDN
		}
	}
}
