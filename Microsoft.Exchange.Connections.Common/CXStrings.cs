using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Connections.Common
{
	internal static class CXStrings
	{
		static CXStrings()
		{
			CXStrings.stringIDs.Add(3102706246U, "Pop3DisabledResponseMsg");
			CXStrings.stringIDs.Add(89833565U, "ImapSent");
			CXStrings.stringIDs.Add(118956886U, "ImapDeletedItems");
			CXStrings.stringIDs.Add(3142549709U, "ImapTrash");
			CXStrings.stringIDs.Add(3395599741U, "ImapAllMail");
			CXStrings.stringIDs.Add(3870026763U, "ImapJunkEmail");
			CXStrings.stringIDs.Add(3095449466U, "ImapDeletedMessages");
			CXStrings.stringIDs.Add(1506270660U, "Pop3MirroredAccountNotPossibleMsg");
			CXStrings.stringIDs.Add(4069538910U, "ImapMaxBytesReceivedExceeded");
			CXStrings.stringIDs.Add(1496473782U, "ImapNoExistsData");
			CXStrings.stringIDs.Add(3295210032U, "ImapServerShutdown");
			CXStrings.stringIDs.Add(451010195U, "DownloadedLimitExceededError");
			CXStrings.stringIDs.Add(932388783U, "ImapSocketException");
			CXStrings.stringIDs.Add(3366710326U, "ImapServerNetworkError");
			CXStrings.stringIDs.Add(3719135992U, "ImapSpam");
			CXStrings.stringIDs.Add(3818046208U, "Pop3LeaveOnServerNotPossibleMsg");
			CXStrings.stringIDs.Add(3777833512U, "ConnectionAlreadyOpenError");
			CXStrings.stringIDs.Add(625985317U, "Pop3AuthErrorMsg");
			CXStrings.stringIDs.Add(1286062087U, "ImapSecurityStatusError");
			CXStrings.stringIDs.Add(1699298936U, "ConnectionClosedError");
			CXStrings.stringIDs.Add(2300253185U, "ImapSentMessages");
			CXStrings.stringIDs.Add(941137818U, "ImapSelectMailboxFailed");
			CXStrings.stringIDs.Add(2876093320U, "ImapSentMail");
			CXStrings.stringIDs.Add(2828988683U, "ImapServerTimeout");
			CXStrings.stringIDs.Add(2125287154U, "Pop3NonCompliantServerMsg");
			CXStrings.stringIDs.Add(2277965524U, "EasMissingOrBadUrlOnRedirectMsg");
			CXStrings.stringIDs.Add(2645419332U, "Pop3TransientLoginDelayedAuthErrorMsg");
			CXStrings.stringIDs.Add(2621146879U, "ImapSentItems");
			CXStrings.stringIDs.Add(913261964U, "Pop3TransientSystemAuthErrorMsg");
			CXStrings.stringIDs.Add(1010454791U, "ImapJunk");
			CXStrings.stringIDs.Add(4258277560U, "ImapDraft");
			CXStrings.stringIDs.Add(2506606049U, "ImapServerDisconnected");
			CXStrings.stringIDs.Add(726012886U, "ImapServerConnectionClosed");
			CXStrings.stringIDs.Add(2965122554U, "Pop3CapabilitiesNotSupportedMsg");
			CXStrings.stringIDs.Add(169570007U, "ImapDrafts");
			CXStrings.stringIDs.Add(3286312367U, "ImapMaxBytesSentExceeded");
			CXStrings.stringIDs.Add(2004202859U, "TlsRemoteCertificateInvalidError");
			CXStrings.stringIDs.Add(3137388257U, "Pop3TransientInUseAuthErrorMsg");
		}

		public static LocalizedString EasWebExceptionMsg(string msg)
		{
			return new LocalizedString("EasWebExceptionMsg", CXStrings.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString ImapInvalidResponseErrorMsg(string failureReason)
		{
			return new LocalizedString("ImapInvalidResponseErrorMsg", CXStrings.ResourceManager, new object[]
			{
				failureReason
			});
		}

		public static LocalizedString Pop3DisabledResponseMsg
		{
			get
			{
				return new LocalizedString("Pop3DisabledResponseMsg", CXStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImapSent
		{
			get
			{
				return new LocalizedString("ImapSent", CXStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImapDeletedItems
		{
			get
			{
				return new LocalizedString("ImapDeletedItems", CXStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImapTrash
		{
			get
			{
				return new LocalizedString("ImapTrash", CXStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImapAllMail
		{
			get
			{
				return new LocalizedString("ImapAllMail", CXStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImapJunkEmail
		{
			get
			{
				return new LocalizedString("ImapJunkEmail", CXStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImapDeletedMessages
		{
			get
			{
				return new LocalizedString("ImapDeletedMessages", CXStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Pop3MirroredAccountNotPossibleMsg
		{
			get
			{
				return new LocalizedString("Pop3MirroredAccountNotPossibleMsg", CXStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImapMaxBytesReceivedExceeded
		{
			get
			{
				return new LocalizedString("ImapMaxBytesReceivedExceeded", CXStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ItemLevelPermanentExceptionMsg(string msg)
		{
			return new LocalizedString("ItemLevelPermanentExceptionMsg", CXStrings.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString ImapNoExistsData
		{
			get
			{
				return new LocalizedString("ImapNoExistsData", CXStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImapServerShutdown
		{
			get
			{
				return new LocalizedString("ImapServerShutdown", CXStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Pop3ErrorResponseMsg(string command, string response)
		{
			return new LocalizedString("Pop3ErrorResponseMsg", CXStrings.ResourceManager, new object[]
			{
				command,
				response
			});
		}

		public static LocalizedString DownloadedLimitExceededError
		{
			get
			{
				return new LocalizedString("DownloadedLimitExceededError", CXStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EasCommandFailed(string responseStatus, string httpStatus)
		{
			return new LocalizedString("EasCommandFailed", CXStrings.ResourceManager, new object[]
			{
				responseStatus,
				httpStatus
			});
		}

		public static LocalizedString EasWBXmlPermanentExceptionMsg(string msg)
		{
			return new LocalizedString("EasWBXmlPermanentExceptionMsg", CXStrings.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString UnexpectedCapabilitiesError(string unexpectedCapabilitiesMsg)
		{
			return new LocalizedString("UnexpectedCapabilitiesError", CXStrings.ResourceManager, new object[]
			{
				unexpectedCapabilitiesMsg
			});
		}

		public static LocalizedString ImapSocketException
		{
			get
			{
				return new LocalizedString("ImapSocketException", CXStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImapServerNetworkError
		{
			get
			{
				return new LocalizedString("ImapServerNetworkError", CXStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TlsError(string msg)
		{
			return new LocalizedString("TlsError", CXStrings.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString ImapCommunicationErrorMsg(string imapCommunicationErrorMsg, RetryPolicy retryPolicy)
		{
			return new LocalizedString("ImapCommunicationErrorMsg", CXStrings.ResourceManager, new object[]
			{
				imapCommunicationErrorMsg,
				retryPolicy
			});
		}

		public static LocalizedString ImapSpam
		{
			get
			{
				return new LocalizedString("ImapSpam", CXStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImapConnectionErrorMsg(string imapConnectionErrorMsg, RetryPolicy retryPolicy)
		{
			return new LocalizedString("ImapConnectionErrorMsg", CXStrings.ResourceManager, new object[]
			{
				imapConnectionErrorMsg,
				retryPolicy
			});
		}

		public static LocalizedString Pop3LeaveOnServerNotPossibleMsg
		{
			get
			{
				return new LocalizedString("Pop3LeaveOnServerNotPossibleMsg", CXStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConnectionAlreadyOpenError
		{
			get
			{
				return new LocalizedString("ConnectionAlreadyOpenError", CXStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImapUnsupportedAuthenticationErrorMsg(string authErrorMsg, string authMechanismName, RetryPolicy retryPolicy)
		{
			return new LocalizedString("ImapUnsupportedAuthenticationErrorMsg", CXStrings.ResourceManager, new object[]
			{
				authErrorMsg,
				authMechanismName,
				retryPolicy
			});
		}

		public static LocalizedString EasUnexpectedHttpStatusMsg(string msg)
		{
			return new LocalizedString("EasUnexpectedHttpStatusMsg", CXStrings.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString EasRetryAfterExceptionMsg(TimeSpan delay, string msg)
		{
			return new LocalizedString("EasRetryAfterExceptionMsg", CXStrings.ResourceManager, new object[]
			{
				delay,
				msg
			});
		}

		public static LocalizedString Pop3AuthErrorMsg
		{
			get
			{
				return new LocalizedString("Pop3AuthErrorMsg", CXStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImapSecurityStatusError
		{
			get
			{
				return new LocalizedString("ImapSecurityStatusError", CXStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OperationLevelPermanentExceptionMsg(string msg)
		{
			return new LocalizedString("OperationLevelPermanentExceptionMsg", CXStrings.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString ConnectionClosedError
		{
			get
			{
				return new LocalizedString("ConnectionClosedError", CXStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImapSentMessages
		{
			get
			{
				return new LocalizedString("ImapSentMessages", CXStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImapErrorMsg(string failureReason)
		{
			return new LocalizedString("ImapErrorMsg", CXStrings.ResourceManager, new object[]
			{
				failureReason
			});
		}

		public static LocalizedString ImapSelectMailboxFailed
		{
			get
			{
				return new LocalizedString("ImapSelectMailboxFailed", CXStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ItemLevelTransientExceptionMsg(string msg)
		{
			return new LocalizedString("ItemLevelTransientExceptionMsg", CXStrings.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString ImapSentMail
		{
			get
			{
				return new LocalizedString("ImapSentMail", CXStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NonPromotableTransientExceptionMsg(string msg)
		{
			return new LocalizedString("NonPromotableTransientExceptionMsg", CXStrings.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString ItemLimitExceededExceptionMsg(string limitExceededMsg)
		{
			return new LocalizedString("ItemLimitExceededExceptionMsg", CXStrings.ResourceManager, new object[]
			{
				limitExceededMsg
			});
		}

		public static LocalizedString ImapServerTimeout
		{
			get
			{
				return new LocalizedString("ImapServerTimeout", CXStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TlsFailureOccurredError(string securityStatus)
		{
			return new LocalizedString("TlsFailureOccurredError", CXStrings.ResourceManager, new object[]
			{
				securityStatus
			});
		}

		public static LocalizedString EasRequiresFolderSyncExceptionMsg(string msg)
		{
			return new LocalizedString("EasRequiresFolderSyncExceptionMsg", CXStrings.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString MessageSizeLimitExceededError(string limitExceededMsg)
		{
			return new LocalizedString("MessageSizeLimitExceededError", CXStrings.ResourceManager, new object[]
			{
				limitExceededMsg
			});
		}

		public static LocalizedString EasRequiresSyncKeyResetExceptionMsg(string msg)
		{
			return new LocalizedString("EasRequiresSyncKeyResetExceptionMsg", CXStrings.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString Pop3NonCompliantServerMsg
		{
			get
			{
				return new LocalizedString("Pop3NonCompliantServerMsg", CXStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EasMissingOrBadUrlOnRedirectMsg
		{
			get
			{
				return new LocalizedString("EasMissingOrBadUrlOnRedirectMsg", CXStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Pop3TransientLoginDelayedAuthErrorMsg
		{
			get
			{
				return new LocalizedString("Pop3TransientLoginDelayedAuthErrorMsg", CXStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImapAuthenticationErrorMsg(string imapAuthenticationErrorMsg, string authMechanismName, RetryPolicy retryPolicy)
		{
			return new LocalizedString("ImapAuthenticationErrorMsg", CXStrings.ResourceManager, new object[]
			{
				imapAuthenticationErrorMsg,
				authMechanismName,
				retryPolicy
			});
		}

		public static LocalizedString MissingCapabilitiesError(string missingCapabilitiesMsg)
		{
			return new LocalizedString("MissingCapabilitiesError", CXStrings.ResourceManager, new object[]
			{
				missingCapabilitiesMsg
			});
		}

		public static LocalizedString ImapSentItems
		{
			get
			{
				return new LocalizedString("ImapSentItems", CXStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Pop3TransientSystemAuthErrorMsg
		{
			get
			{
				return new LocalizedString("Pop3TransientSystemAuthErrorMsg", CXStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnhandledError(string typeName)
		{
			return new LocalizedString("UnhandledError", CXStrings.ResourceManager, new object[]
			{
				typeName
			});
		}

		public static LocalizedString ImapBadResponseErrorMsg(string failureReason)
		{
			return new LocalizedString("ImapBadResponseErrorMsg", CXStrings.ResourceManager, new object[]
			{
				failureReason
			});
		}

		public static LocalizedString Pop3PermErrorResponseMsg(string command, string response)
		{
			return new LocalizedString("Pop3PermErrorResponseMsg", CXStrings.ResourceManager, new object[]
			{
				command,
				response
			});
		}

		public static LocalizedString ImapJunk
		{
			get
			{
				return new LocalizedString("ImapJunk", CXStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImapDraft
		{
			get
			{
				return new LocalizedString("ImapDraft", CXStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImapServerDisconnected
		{
			get
			{
				return new LocalizedString("ImapServerDisconnected", CXStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImapServerConnectionClosed
		{
			get
			{
				return new LocalizedString("ImapServerConnectionClosed", CXStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Pop3CapabilitiesNotSupportedMsg
		{
			get
			{
				return new LocalizedString("Pop3CapabilitiesNotSupportedMsg", CXStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OperationLevelTransientExceptionMsg(string msg)
		{
			return new LocalizedString("OperationLevelTransientExceptionMsg", CXStrings.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString ImapDrafts
		{
			get
			{
				return new LocalizedString("ImapDrafts", CXStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImapMaxBytesSentExceeded
		{
			get
			{
				return new LocalizedString("ImapMaxBytesSentExceeded", CXStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EasWBXmlExceptionMsg(string msg)
		{
			return new LocalizedString("EasWBXmlExceptionMsg", CXStrings.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString TlsRemoteCertificateInvalidError
		{
			get
			{
				return new LocalizedString("TlsRemoteCertificateInvalidError", CXStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImapConnectionClosedErrorMsg(string imapConnectionClosedErrMsg)
		{
			return new LocalizedString("ImapConnectionClosedErrorMsg", CXStrings.ResourceManager, new object[]
			{
				imapConnectionClosedErrMsg
			});
		}

		public static LocalizedString Pop3TransientInUseAuthErrorMsg
		{
			get
			{
				return new LocalizedString("Pop3TransientInUseAuthErrorMsg", CXStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Pop3BrokenResponseMsg(string command, string response)
		{
			return new LocalizedString("Pop3BrokenResponseMsg", CXStrings.ResourceManager, new object[]
			{
				command,
				response
			});
		}

		public static LocalizedString Pop3UnknownResponseMsg(string command, string response)
		{
			return new LocalizedString("Pop3UnknownResponseMsg", CXStrings.ResourceManager, new object[]
			{
				command,
				response
			});
		}

		public static LocalizedString GetLocalizedString(CXStrings.IDs key)
		{
			return new LocalizedString(CXStrings.stringIDs[(uint)key], CXStrings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(38);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Connections.Common.CXStrings", typeof(CXStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			Pop3DisabledResponseMsg = 3102706246U,
			ImapSent = 89833565U,
			ImapDeletedItems = 118956886U,
			ImapTrash = 3142549709U,
			ImapAllMail = 3395599741U,
			ImapJunkEmail = 3870026763U,
			ImapDeletedMessages = 3095449466U,
			Pop3MirroredAccountNotPossibleMsg = 1506270660U,
			ImapMaxBytesReceivedExceeded = 4069538910U,
			ImapNoExistsData = 1496473782U,
			ImapServerShutdown = 3295210032U,
			DownloadedLimitExceededError = 451010195U,
			ImapSocketException = 932388783U,
			ImapServerNetworkError = 3366710326U,
			ImapSpam = 3719135992U,
			Pop3LeaveOnServerNotPossibleMsg = 3818046208U,
			ConnectionAlreadyOpenError = 3777833512U,
			Pop3AuthErrorMsg = 625985317U,
			ImapSecurityStatusError = 1286062087U,
			ConnectionClosedError = 1699298936U,
			ImapSentMessages = 2300253185U,
			ImapSelectMailboxFailed = 941137818U,
			ImapSentMail = 2876093320U,
			ImapServerTimeout = 2828988683U,
			Pop3NonCompliantServerMsg = 2125287154U,
			EasMissingOrBadUrlOnRedirectMsg = 2277965524U,
			Pop3TransientLoginDelayedAuthErrorMsg = 2645419332U,
			ImapSentItems = 2621146879U,
			Pop3TransientSystemAuthErrorMsg = 913261964U,
			ImapJunk = 1010454791U,
			ImapDraft = 4258277560U,
			ImapServerDisconnected = 2506606049U,
			ImapServerConnectionClosed = 726012886U,
			Pop3CapabilitiesNotSupportedMsg = 2965122554U,
			ImapDrafts = 169570007U,
			ImapMaxBytesSentExceeded = 3286312367U,
			TlsRemoteCertificateInvalidError = 2004202859U,
			Pop3TransientInUseAuthErrorMsg = 3137388257U
		}

		private enum ParamIDs
		{
			EasWebExceptionMsg,
			ImapInvalidResponseErrorMsg,
			ItemLevelPermanentExceptionMsg,
			Pop3ErrorResponseMsg,
			EasCommandFailed,
			EasWBXmlPermanentExceptionMsg,
			UnexpectedCapabilitiesError,
			TlsError,
			ImapCommunicationErrorMsg,
			ImapConnectionErrorMsg,
			ImapUnsupportedAuthenticationErrorMsg,
			EasUnexpectedHttpStatusMsg,
			EasRetryAfterExceptionMsg,
			OperationLevelPermanentExceptionMsg,
			ImapErrorMsg,
			ItemLevelTransientExceptionMsg,
			NonPromotableTransientExceptionMsg,
			ItemLimitExceededExceptionMsg,
			TlsFailureOccurredError,
			EasRequiresFolderSyncExceptionMsg,
			MessageSizeLimitExceededError,
			EasRequiresSyncKeyResetExceptionMsg,
			ImapAuthenticationErrorMsg,
			MissingCapabilitiesError,
			UnhandledError,
			ImapBadResponseErrorMsg,
			Pop3PermErrorResponseMsg,
			OperationLevelTransientExceptionMsg,
			EasWBXmlExceptionMsg,
			ImapConnectionClosedErrorMsg,
			Pop3BrokenResponseMsg,
			Pop3UnknownResponseMsg
		}
	}
}
