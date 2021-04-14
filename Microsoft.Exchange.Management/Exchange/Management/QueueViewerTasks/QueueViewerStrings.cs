using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.QueueViewerTasks
{
	internal static class QueueViewerStrings
	{
		static QueueViewerStrings()
		{
			QueueViewerStrings.stringIDs.Add(317860356U, "UnfreezeMessageTask");
			QueueViewerStrings.stringIDs.Add(2910097809U, "FreezeMessageTask");
			QueueViewerStrings.stringIDs.Add(1959773104U, "Status");
			QueueViewerStrings.stringIDs.Add(1421152990U, "Type");
			QueueViewerStrings.stringIDs.Add(3089884583U, "DateReceived");
			QueueViewerStrings.stringIDs.Add(1316090539U, "MissingName");
			QueueViewerStrings.stringIDs.Add(1572946888U, "UnfreezeQueueTask");
			QueueViewerStrings.stringIDs.Add(3695048262U, "InvalidServerData");
			QueueViewerStrings.stringIDs.Add(973091570U, "InvalidIdentityString");
			QueueViewerStrings.stringIDs.Add(3845883191U, "FreezeQueueTask");
			QueueViewerStrings.stringIDs.Add(1481046174U, "SourceConnector");
			QueueViewerStrings.stringIDs.Add(3815822302U, "InvalidServerVersion");
			QueueViewerStrings.stringIDs.Add(2112399141U, "SuccessMessageRedirectMessageRequestCompleted");
			QueueViewerStrings.stringIDs.Add(2470050549U, "TextMatchingNotSupported");
			QueueViewerStrings.stringIDs.Add(3457520377U, "Size");
			QueueViewerStrings.stringIDs.Add(1800498867U, "MissingSender");
			QueueViewerStrings.stringIDs.Add(287061521U, "Id");
			QueueViewerStrings.stringIDs.Add(4170207812U, "SetMessageResubmitMustBeTrue");
			QueueViewerStrings.stringIDs.Add(2064039061U, "MessageNextRetryTime");
			QueueViewerStrings.stringIDs.Add(337370324U, "InvalidFieldName");
			QueueViewerStrings.stringIDs.Add(3710722188U, "MessageCount");
			QueueViewerStrings.stringIDs.Add(3894225067U, "RetryCount");
			QueueViewerStrings.stringIDs.Add(1638755231U, "FilterTypeNotSupported");
			QueueViewerStrings.stringIDs.Add(3688482442U, "SourceIP");
			QueueViewerStrings.stringIDs.Add(793883224U, "RedirectMessageTask");
			QueueViewerStrings.stringIDs.Add(4178538216U, "QueueResubmitInProgress");
			QueueViewerStrings.stringIDs.Add(1686262562U, "ComparisonNotSupported");
			QueueViewerStrings.stringIDs.Add(3457614570U, "RetryQueueTask");
			QueueViewerStrings.stringIDs.Add(3644361014U, "ExpirationTime");
			QueueViewerStrings.stringIDs.Add(501948002U, "InvalidServerCollection");
			QueueViewerStrings.stringIDs.Add(4294095275U, "RedirectMessageInProgress");
			QueueViewerStrings.stringIDs.Add(2999867432U, "RemoveMessageTask");
			QueueViewerStrings.stringIDs.Add(3834431484U, "MessageLastRetryTime");
			QueueViewerStrings.stringIDs.Add(2455477494U, "InvalidIdentityForEquality");
			QueueViewerStrings.stringIDs.Add(3857609986U, "Priority");
			QueueViewerStrings.stringIDs.Add(1726666628U, "SetMessageTask");
			QueueViewerStrings.stringIDs.Add(1732412034U, "Subject");
			QueueViewerStrings.stringIDs.Add(822915892U, "OldestMessage");
			QueueViewerStrings.stringIDs.Add(3853989365U, "TooManyResults");
			QueueViewerStrings.stringIDs.Add(2816985275U, "AmbiguousParameterSet");
			QueueViewerStrings.stringIDs.Add(1967184194U, "NextRetryTime");
			QueueViewerStrings.stringIDs.Add(523788226U, "InvalidClientData");
			QueueViewerStrings.stringIDs.Add(1850287295U, "LastRetryTime");
			QueueViewerStrings.stringIDs.Add(51203499U, "Sender");
			QueueViewerStrings.stringIDs.Add(3552881163U, "MessageGrid");
		}

		public static LocalizedString UnfreezeMessageTask
		{
			get
			{
				return new LocalizedString("UnfreezeMessageTask", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FreezeMessageTask
		{
			get
			{
				return new LocalizedString("FreezeMessageTask", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Status
		{
			get
			{
				return new LocalizedString("Status", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmationMessageSetMessageFilter(string Filter)
		{
			return new LocalizedString("ConfirmationMessageSetMessageFilter", "", false, false, QueueViewerStrings.ResourceManager, new object[]
			{
				Filter
			});
		}

		public static LocalizedString Type
		{
			get
			{
				return new LocalizedString("Type", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DateReceived
		{
			get
			{
				return new LocalizedString("DateReceived", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmationMessageRedirectMessage(string target)
		{
			return new LocalizedString("ConfirmationMessageRedirectMessage", "", false, false, QueueViewerStrings.ResourceManager, new object[]
			{
				target
			});
		}

		public static LocalizedString MissingName
		{
			get
			{
				return new LocalizedString("MissingName", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnfreezeQueueTask
		{
			get
			{
				return new LocalizedString("UnfreezeQueueTask", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidServerData
		{
			get
			{
				return new LocalizedString("InvalidServerData", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmationMessageSetMessageIdentity(string Identity)
		{
			return new LocalizedString("ConfirmationMessageSetMessageIdentity", "", false, false, QueueViewerStrings.ResourceManager, new object[]
			{
				Identity
			});
		}

		public static LocalizedString InvalidIdentityString
		{
			get
			{
				return new LocalizedString("InvalidIdentityString", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncompleteIdentity(string identity)
		{
			return new LocalizedString("IncompleteIdentity", "", false, false, QueueViewerStrings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString FreezeQueueTask
		{
			get
			{
				return new LocalizedString("FreezeQueueTask", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SourceConnector
		{
			get
			{
				return new LocalizedString("SourceConnector", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidServerVersion
		{
			get
			{
				return new LocalizedString("InvalidServerVersion", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmationMessageSuspendQueueIdentity(string Identity)
		{
			return new LocalizedString("ConfirmationMessageSuspendQueueIdentity", "Ex90C1C2", false, true, QueueViewerStrings.ResourceManager, new object[]
			{
				Identity
			});
		}

		public static LocalizedString SuccessMessageRedirectMessageRequestCompleted
		{
			get
			{
				return new LocalizedString("SuccessMessageRedirectMessageRequestCompleted", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TextMatchingNotSupported
		{
			get
			{
				return new LocalizedString("TextMatchingNotSupported", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ObjectNotFound(string identity)
		{
			return new LocalizedString("ObjectNotFound", "", false, false, QueueViewerStrings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString Size
		{
			get
			{
				return new LocalizedString("Size", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MissingSender
		{
			get
			{
				return new LocalizedString("MissingSender", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmationMessageRetryQueueFilter(string Filter)
		{
			return new LocalizedString("ConfirmationMessageRetryQueueFilter", "ExB8D442", false, true, QueueViewerStrings.ResourceManager, new object[]
			{
				Filter
			});
		}

		public static LocalizedString Id
		{
			get
			{
				return new LocalizedString("Id", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SetMessageResubmitMustBeTrue
		{
			get
			{
				return new LocalizedString("SetMessageResubmitMustBeTrue", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageNextRetryTime
		{
			get
			{
				return new LocalizedString("MessageNextRetryTime", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidFieldName
		{
			get
			{
				return new LocalizedString("InvalidFieldName", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidDomainFormat(string domain)
		{
			return new LocalizedString("InvalidDomainFormat", "", false, false, QueueViewerStrings.ResourceManager, new object[]
			{
				domain
			});
		}

		public static LocalizedString ConfirmationMessageResumeQueueIdentity(string Identity)
		{
			return new LocalizedString("ConfirmationMessageResumeQueueIdentity", "Ex64B180", false, true, QueueViewerStrings.ResourceManager, new object[]
			{
				Identity
			});
		}

		public static LocalizedString MessageCount
		{
			get
			{
				return new LocalizedString("MessageCount", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmationMessageResumeMessageFilter(string Filter)
		{
			return new LocalizedString("ConfirmationMessageResumeMessageFilter", "Ex969910", false, true, QueueViewerStrings.ResourceManager, new object[]
			{
				Filter
			});
		}

		public static LocalizedString InvalidProviderName(string provider)
		{
			return new LocalizedString("InvalidProviderName", "", false, false, QueueViewerStrings.ResourceManager, new object[]
			{
				provider
			});
		}

		public static LocalizedString ConfirmationMessageRetryQueueIdentity(string Identity)
		{
			return new LocalizedString("ConfirmationMessageRetryQueueIdentity", "Ex8AC9D5", false, true, QueueViewerStrings.ResourceManager, new object[]
			{
				Identity
			});
		}

		public static LocalizedString RetryCount
		{
			get
			{
				return new LocalizedString("RetryCount", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FilterTypeNotSupported
		{
			get
			{
				return new LocalizedString("FilterTypeNotSupported", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SourceIP
		{
			get
			{
				return new LocalizedString("SourceIP", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RedirectMessageTask
		{
			get
			{
				return new LocalizedString("RedirectMessageTask", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmationMessageRemoveMessageIdentity(string Identity)
		{
			return new LocalizedString("ConfirmationMessageRemoveMessageIdentity", "ExE29E2E", false, true, QueueViewerStrings.ResourceManager, new object[]
			{
				Identity
			});
		}

		public static LocalizedString GenericError(string message)
		{
			return new LocalizedString("GenericError", "", false, false, QueueViewerStrings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString QueueResubmitInProgress
		{
			get
			{
				return new LocalizedString("QueueResubmitInProgress", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RpcNotRegistered(string computername)
		{
			return new LocalizedString("RpcNotRegistered", "", false, false, QueueViewerStrings.ResourceManager, new object[]
			{
				computername
			});
		}

		public static LocalizedString ConfirmationMessageSuspendQueueFilter(string Filter)
		{
			return new LocalizedString("ConfirmationMessageSuspendQueueFilter", "Ex671206", false, true, QueueViewerStrings.ResourceManager, new object[]
			{
				Filter
			});
		}

		public static LocalizedString ConfirmationMessageSuspendMessageFilter(string Filter)
		{
			return new LocalizedString("ConfirmationMessageSuspendMessageFilter", "ExD0E030", false, true, QueueViewerStrings.ResourceManager, new object[]
			{
				Filter
			});
		}

		public static LocalizedString ConfirmationMessageResumeQueueFilter(string Filter)
		{
			return new LocalizedString("ConfirmationMessageResumeQueueFilter", "Ex2DCBD0", false, true, QueueViewerStrings.ResourceManager, new object[]
			{
				Filter
			});
		}

		public static LocalizedString ComparisonNotSupported
		{
			get
			{
				return new LocalizedString("ComparisonNotSupported", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RetryQueueTask
		{
			get
			{
				return new LocalizedString("RetryQueueTask", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NonAbsolutePath(string path)
		{
			return new LocalizedString("NonAbsolutePath", "", false, false, QueueViewerStrings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString ExpirationTime
		{
			get
			{
				return new LocalizedString("ExpirationTime", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidOperation(string identity)
		{
			return new LocalizedString("InvalidOperation", "", false, false, QueueViewerStrings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString InvalidServerCollection
		{
			get
			{
				return new LocalizedString("InvalidServerCollection", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RedirectMessageInProgress
		{
			get
			{
				return new LocalizedString("RedirectMessageInProgress", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmationMessageExportMessage(string Identity)
		{
			return new LocalizedString("ConfirmationMessageExportMessage", "ExC199BE", false, true, QueueViewerStrings.ResourceManager, new object[]
			{
				Identity
			});
		}

		public static LocalizedString GenericRpcError(string message, string computername)
		{
			return new LocalizedString("GenericRpcError", "", false, false, QueueViewerStrings.ResourceManager, new object[]
			{
				message,
				computername
			});
		}

		public static LocalizedString MessageNotSuspended(string identity)
		{
			return new LocalizedString("MessageNotSuspended", "", false, false, QueueViewerStrings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString RemoveMessageTask
		{
			get
			{
				return new LocalizedString("RemoveMessageTask", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageLastRetryTime
		{
			get
			{
				return new LocalizedString("MessageLastRetryTime", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidIdentityForEquality
		{
			get
			{
				return new LocalizedString("InvalidIdentityForEquality", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Priority
		{
			get
			{
				return new LocalizedString("Priority", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmationMessageResumeMessageIdentity(string Identity)
		{
			return new LocalizedString("ConfirmationMessageResumeMessageIdentity", "ExF21D72", false, true, QueueViewerStrings.ResourceManager, new object[]
			{
				Identity
			});
		}

		public static LocalizedString ConfirmationMessageRemoveMessageFilter(string Filter)
		{
			return new LocalizedString("ConfirmationMessageRemoveMessageFilter", "Ex77FC78", false, true, QueueViewerStrings.ResourceManager, new object[]
			{
				Filter
			});
		}

		public static LocalizedString SetMessageTask
		{
			get
			{
				return new LocalizedString("SetMessageTask", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SetMessageOutboundPoolOutsideRange(int port, int min, int max)
		{
			return new LocalizedString("SetMessageOutboundPoolOutsideRange", "", false, false, QueueViewerStrings.ResourceManager, new object[]
			{
				port,
				min,
				max
			});
		}

		public static LocalizedString RpcUnavailable(string computername)
		{
			return new LocalizedString("RpcUnavailable", "", false, false, QueueViewerStrings.ResourceManager, new object[]
			{
				computername
			});
		}

		public static LocalizedString Subject
		{
			get
			{
				return new LocalizedString("Subject", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OldestMessage
		{
			get
			{
				return new LocalizedString("OldestMessage", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TooManyResults
		{
			get
			{
				return new LocalizedString("TooManyResults", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AmbiguousParameterSet
		{
			get
			{
				return new LocalizedString("AmbiguousParameterSet", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NextRetryTime
		{
			get
			{
				return new LocalizedString("NextRetryTime", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotTransportHubServer(string fqdn)
		{
			return new LocalizedString("NotTransportHubServer", "", false, false, QueueViewerStrings.ResourceManager, new object[]
			{
				fqdn
			});
		}

		public static LocalizedString InvalidClientData
		{
			get
			{
				return new LocalizedString("InvalidClientData", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnknownServer(string fqdn)
		{
			return new LocalizedString("UnknownServer", "", false, false, QueueViewerStrings.ResourceManager, new object[]
			{
				fqdn
			});
		}

		public static LocalizedString ConfirmationMessageSuspendMessageIdentity(string Identity)
		{
			return new LocalizedString("ConfirmationMessageSuspendMessageIdentity", "Ex68F928", false, true, QueueViewerStrings.ResourceManager, new object[]
			{
				Identity
			});
		}

		public static LocalizedString LastRetryTime
		{
			get
			{
				return new LocalizedString("LastRetryTime", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MultipleIdentityMatch(string identity)
		{
			return new LocalizedString("MultipleIdentityMatch", "", false, false, QueueViewerStrings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString Sender
		{
			get
			{
				return new LocalizedString("Sender", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageGrid
		{
			get
			{
				return new LocalizedString("MessageGrid", "", false, false, QueueViewerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(QueueViewerStrings.IDs key)
		{
			return new LocalizedString(QueueViewerStrings.stringIDs[(uint)key], QueueViewerStrings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(45);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Management.QueueViewerStrings", typeof(QueueViewerStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			UnfreezeMessageTask = 317860356U,
			FreezeMessageTask = 2910097809U,
			Status = 1959773104U,
			Type = 1421152990U,
			DateReceived = 3089884583U,
			MissingName = 1316090539U,
			UnfreezeQueueTask = 1572946888U,
			InvalidServerData = 3695048262U,
			InvalidIdentityString = 973091570U,
			FreezeQueueTask = 3845883191U,
			SourceConnector = 1481046174U,
			InvalidServerVersion = 3815822302U,
			SuccessMessageRedirectMessageRequestCompleted = 2112399141U,
			TextMatchingNotSupported = 2470050549U,
			Size = 3457520377U,
			MissingSender = 1800498867U,
			Id = 287061521U,
			SetMessageResubmitMustBeTrue = 4170207812U,
			MessageNextRetryTime = 2064039061U,
			InvalidFieldName = 337370324U,
			MessageCount = 3710722188U,
			RetryCount = 3894225067U,
			FilterTypeNotSupported = 1638755231U,
			SourceIP = 3688482442U,
			RedirectMessageTask = 793883224U,
			QueueResubmitInProgress = 4178538216U,
			ComparisonNotSupported = 1686262562U,
			RetryQueueTask = 3457614570U,
			ExpirationTime = 3644361014U,
			InvalidServerCollection = 501948002U,
			RedirectMessageInProgress = 4294095275U,
			RemoveMessageTask = 2999867432U,
			MessageLastRetryTime = 3834431484U,
			InvalidIdentityForEquality = 2455477494U,
			Priority = 3857609986U,
			SetMessageTask = 1726666628U,
			Subject = 1732412034U,
			OldestMessage = 822915892U,
			TooManyResults = 3853989365U,
			AmbiguousParameterSet = 2816985275U,
			NextRetryTime = 1967184194U,
			InvalidClientData = 523788226U,
			LastRetryTime = 1850287295U,
			Sender = 51203499U,
			MessageGrid = 3552881163U
		}

		private enum ParamIDs
		{
			ConfirmationMessageSetMessageFilter,
			ConfirmationMessageRedirectMessage,
			ConfirmationMessageSetMessageIdentity,
			IncompleteIdentity,
			ConfirmationMessageSuspendQueueIdentity,
			ObjectNotFound,
			ConfirmationMessageRetryQueueFilter,
			InvalidDomainFormat,
			ConfirmationMessageResumeQueueIdentity,
			ConfirmationMessageResumeMessageFilter,
			InvalidProviderName,
			ConfirmationMessageRetryQueueIdentity,
			ConfirmationMessageRemoveMessageIdentity,
			GenericError,
			RpcNotRegistered,
			ConfirmationMessageSuspendQueueFilter,
			ConfirmationMessageSuspendMessageFilter,
			ConfirmationMessageResumeQueueFilter,
			NonAbsolutePath,
			InvalidOperation,
			ConfirmationMessageExportMessage,
			GenericRpcError,
			MessageNotSuspended,
			ConfirmationMessageResumeMessageIdentity,
			ConfirmationMessageRemoveMessageFilter,
			SetMessageOutboundPoolOutsideRange,
			RpcUnavailable,
			NotTransportHubServer,
			UnknownServer,
			ConfirmationMessageSuspendMessageIdentity,
			MultipleIdentityMatch
		}
	}
}
