using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.EseRepl
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(1344658319U, "NetworkSecurityFailed");
			Strings.stringIDs.Add(726910307U, "NetworkNoUsableEndpoints");
			Strings.stringIDs.Add(3273471454U, "NetworkReadEOF");
			Strings.stringIDs.Add(1148446483U, "NetworkFailedToAuthServer");
			Strings.stringIDs.Add(3985107625U, "NetworkCancelled");
			Strings.stringIDs.Add(4232351636U, "NetworkIsDisabled");
			Strings.stringIDs.Add(3508567603U, "NetworkDataOverflowGeneric");
			Strings.stringIDs.Add(2364081162U, "NetworkCorruptDataGeneric");
		}

		public static LocalizedString NetworkConnectionTimeout(int waitInsecs)
		{
			return new LocalizedString("NetworkConnectionTimeout", Strings.ResourceManager, new object[]
			{
				waitInsecs
			});
		}

		public static LocalizedString NetworkTimeoutError(string remoteNodeName, string errorText)
		{
			return new LocalizedString("NetworkTimeoutError", Strings.ResourceManager, new object[]
			{
				remoteNodeName,
				errorText
			});
		}

		public static LocalizedString NetworkSecurityFailed
		{
			get
			{
				return new LocalizedString("NetworkSecurityFailed", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NetworkAddressResolutionFailedNoDnsEntry(string nodeName)
		{
			return new LocalizedString("NetworkAddressResolutionFailedNoDnsEntry", Strings.ResourceManager, new object[]
			{
				nodeName
			});
		}

		public static LocalizedString NetworkNoUsableEndpoints
		{
			get
			{
				return new LocalizedString("NetworkNoUsableEndpoints", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NetworkReadEOF
		{
			get
			{
				return new LocalizedString("NetworkReadEOF", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NetworkEndOfData(string nodeName, string messageText)
		{
			return new LocalizedString("NetworkEndOfData", Strings.ResourceManager, new object[]
			{
				nodeName,
				messageText
			});
		}

		public static LocalizedString NetworkNotUsable(string netName, string nodeName, string reason)
		{
			return new LocalizedString("NetworkNotUsable", Strings.ResourceManager, new object[]
			{
				netName,
				nodeName,
				reason
			});
		}

		public static LocalizedString NetworkTransportError(string err)
		{
			return new LocalizedString("NetworkTransportError", Strings.ResourceManager, new object[]
			{
				err
			});
		}

		public static LocalizedString SourceDatabaseNotFound(Guid g, string sourceServer)
		{
			return new LocalizedString("SourceDatabaseNotFound", Strings.ResourceManager, new object[]
			{
				g,
				sourceServer
			});
		}

		public static LocalizedString NetworkRemoteErrorUnknown(string nodeName, string messageText)
		{
			return new LocalizedString("NetworkRemoteErrorUnknown", Strings.ResourceManager, new object[]
			{
				nodeName,
				messageText
			});
		}

		public static LocalizedString UnexpectedEOF(string filename)
		{
			return new LocalizedString("UnexpectedEOF", Strings.ResourceManager, new object[]
			{
				filename
			});
		}

		public static LocalizedString NetworkNameNotFound(string netName)
		{
			return new LocalizedString("NetworkNameNotFound", Strings.ResourceManager, new object[]
			{
				netName
			});
		}

		public static LocalizedString DatabaseNotFound(Guid dbGuid)
		{
			return new LocalizedString("DatabaseNotFound", Strings.ResourceManager, new object[]
			{
				dbGuid
			});
		}

		public static LocalizedString NetworkFailedToAuthServer
		{
			get
			{
				return new LocalizedString("NetworkFailedToAuthServer", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NetworkCancelled
		{
			get
			{
				return new LocalizedString("NetworkCancelled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NetworkReadTimeout(int waitInsecs)
		{
			return new LocalizedString("NetworkReadTimeout", Strings.ResourceManager, new object[]
			{
				waitInsecs
			});
		}

		public static LocalizedString NetworkIsDisabled
		{
			get
			{
				return new LocalizedString("NetworkIsDisabled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SourceLogBreakStallsPassiveError(string sourceServerName, string error)
		{
			return new LocalizedString("SourceLogBreakStallsPassiveError", Strings.ResourceManager, new object[]
			{
				sourceServerName,
				error
			});
		}

		public static LocalizedString NetworkAddressResolutionFailed(string nodeName, string errMsg)
		{
			return new LocalizedString("NetworkAddressResolutionFailed", Strings.ResourceManager, new object[]
			{
				nodeName,
				errMsg
			});
		}

		public static LocalizedString NetworkDataOverflowGeneric
		{
			get
			{
				return new LocalizedString("NetworkDataOverflowGeneric", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NetworkUnexpectedMessage(string nodeName, string messageText)
		{
			return new LocalizedString("NetworkUnexpectedMessage", Strings.ResourceManager, new object[]
			{
				nodeName,
				messageText
			});
		}

		public static LocalizedString NetworkCommunicationError(string remoteNodeName, string errorText)
		{
			return new LocalizedString("NetworkCommunicationError", Strings.ResourceManager, new object[]
			{
				remoteNodeName,
				errorText
			});
		}

		public static LocalizedString NetworkCorruptDataGeneric
		{
			get
			{
				return new LocalizedString("NetworkCorruptDataGeneric", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NetworkRemoteError(string nodeName, string messageText)
		{
			return new LocalizedString("NetworkRemoteError", Strings.ResourceManager, new object[]
			{
				nodeName,
				messageText
			});
		}

		public static LocalizedString NetworkCorruptData(string srcNode)
		{
			return new LocalizedString("NetworkCorruptData", Strings.ResourceManager, new object[]
			{
				srcNode
			});
		}

		public static LocalizedString FileIOonSourceException(string serverName, string fileFullPath, string ioErrorMessage)
		{
			return new LocalizedString("FileIOonSourceException", Strings.ResourceManager, new object[]
			{
				serverName,
				fileFullPath,
				ioErrorMessage
			});
		}

		public static LocalizedString CorruptLogDetectedError(string filename, string errorText)
		{
			return new LocalizedString("CorruptLogDetectedError", Strings.ResourceManager, new object[]
			{
				filename,
				errorText
			});
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(8);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.EseRepl.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			NetworkSecurityFailed = 1344658319U,
			NetworkNoUsableEndpoints = 726910307U,
			NetworkReadEOF = 3273471454U,
			NetworkFailedToAuthServer = 1148446483U,
			NetworkCancelled = 3985107625U,
			NetworkIsDisabled = 4232351636U,
			NetworkDataOverflowGeneric = 3508567603U,
			NetworkCorruptDataGeneric = 2364081162U
		}

		private enum ParamIDs
		{
			NetworkConnectionTimeout,
			NetworkTimeoutError,
			NetworkAddressResolutionFailedNoDnsEntry,
			NetworkEndOfData,
			NetworkNotUsable,
			NetworkTransportError,
			SourceDatabaseNotFound,
			NetworkRemoteErrorUnknown,
			UnexpectedEOF,
			NetworkNameNotFound,
			DatabaseNotFound,
			NetworkReadTimeout,
			SourceLogBreakStallsPassiveError,
			NetworkAddressResolutionFailed,
			NetworkUnexpectedMessage,
			NetworkCommunicationError,
			NetworkRemoteError,
			NetworkCorruptData,
			FileIOonSourceException,
			CorruptLogDetectedError
		}
	}
}
