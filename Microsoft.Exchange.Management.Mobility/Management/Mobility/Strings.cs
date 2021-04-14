using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.DeltaSync;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Imap;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pop;

namespace Microsoft.Exchange.Management.Mobility
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(1372907051U, "InsecureConfirmation");
			Strings.stringIDs.Add(1199310315U, "SubscriptionCannotBeChanged");
			Strings.stringIDs.Add(456403923U, "AutoProvisionComplete");
			Strings.stringIDs.Add(4170829984U, "SetLinkedInSubscriptionConfirmation");
			Strings.stringIDs.Add(843454792U, "FacebookCommunicationError");
			Strings.stringIDs.Add(433732448U, "AutoProvisionNoProtocols");
			Strings.stringIDs.Add(1081354883U, "WarningExtensionFeatureDisabled");
			Strings.stringIDs.Add(2674661771U, "FacebookAuthorizationError");
			Strings.stringIDs.Add(3132041271U, "AutoProvisionConnectivity");
			Strings.stringIDs.Add(3962568818U, "ErrorMissingFile");
			Strings.stringIDs.Add(1935884132U, "CreateLinkedInSubscriptionConfirmation");
			Strings.stringIDs.Add(232075358U, "SubscriptionCannotBeEnabled");
			Strings.stringIDs.Add(2121885544U, "ErrorServerCertificateError");
			Strings.stringIDs.Add(3868201023U, "ConfirmationMessageInstallOwaOrgExtension");
			Strings.stringIDs.Add(3256811515U, "ErrorCanNotDownloadPackage");
			Strings.stringIDs.Add(562700869U, "AutoProvisionQueryDNS");
			Strings.stringIDs.Add(424238761U, "IMAPAccountVerificationFailedException");
			Strings.stringIDs.Add(4012336426U, "FacebookEmptyAccessToken");
			Strings.stringIDs.Add(2684496906U, "NullSubscriptionStoreId");
			Strings.stringIDs.Add(966086681U, "ErrorCannotSpecifyParameterWithoutOrgExtensionParameter");
			Strings.stringIDs.Add(408801391U, "FacebookTimeoutError");
			Strings.stringIDs.Add(1307888814U, "CreateFacebookSubscriptionConfirmation");
			Strings.stringIDs.Add(1548922274U, "AutoProvisionResults");
			Strings.stringIDs.Add(3353546040U, "ErrorNoInputForExtensionInstall");
			Strings.stringIDs.Add(2973592384U, "AutoProvisionCreate");
			Strings.stringIDs.Add(4263159413U, "ErrorReasonUserNotAllowedToInstallReadWriteMailbox");
			Strings.stringIDs.Add(2907685502U, "SetFacebookSubscriptionConfirmation");
		}

		public static LocalizedString ErrorTooManyUsersInUserList(int maxCount)
		{
			return new LocalizedString("ErrorTooManyUsersInUserList", Strings.ResourceManager, new object[]
			{
				maxCount
			});
		}

		public static LocalizedString CreateHotmailSubscriptionConfirmation(HotmailSubscriptionProxy subscription)
		{
			return new LocalizedString("CreateHotmailSubscriptionConfirmation", Strings.ResourceManager, new object[]
			{
				subscription
			});
		}

		public static LocalizedString PushNotificationAppPlatformMismatch(string appName, string originalApp)
		{
			return new LocalizedString("PushNotificationAppPlatformMismatch", Strings.ResourceManager, new object[]
			{
				appName,
				originalApp
			});
		}

		public static LocalizedString ConfirmationMessageNewApp(string id)
		{
			return new LocalizedString("ConfirmationMessageNewApp", Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString InsecureConfirmation
		{
			get
			{
				return new LocalizedString("InsecureConfirmation", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmRemoveUserPushNotificationSubscriptions(string identity)
		{
			return new LocalizedString("ConfirmRemoveUserPushNotificationSubscriptions", Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString SubscriptionCannotBeChanged
		{
			get
			{
				return new LocalizedString("SubscriptionCannotBeChanged", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImportContactListConfirmation(MailboxIdParameter identity)
		{
			return new LocalizedString("ImportContactListConfirmation", Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString AutoProvisionComplete
		{
			get
			{
				return new LocalizedString("AutoProvisionComplete", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SetHotmailSubscriptionConfirmation(AggregationSubscriptionIdParameter identity)
		{
			return new LocalizedString("SetHotmailSubscriptionConfirmation", Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString SetLinkedInSubscriptionConfirmation
		{
			get
			{
				return new LocalizedString("SetLinkedInSubscriptionConfirmation", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SetImapSubscriptionConfirmation(AggregationSubscriptionIdParameter identity)
		{
			return new LocalizedString("SetImapSubscriptionConfirmation", Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString FacebookCommunicationError
		{
			get
			{
				return new LocalizedString("FacebookCommunicationError", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PushNotificationAppSecretEncryptionWarning(string appId)
		{
			return new LocalizedString("PushNotificationAppSecretEncryptionWarning", Strings.ResourceManager, new object[]
			{
				appId
			});
		}

		public static LocalizedString PushNotificationFailedToResolveFallbackPartition(string appId, string currentPartition)
		{
			return new LocalizedString("PushNotificationFailedToResolveFallbackPartition", Strings.ResourceManager, new object[]
			{
				appId,
				currentPartition
			});
		}

		public static LocalizedString ErrorAppTargetMailboxNotFound(string mailboxParameterName, string orgAppParameterName)
		{
			return new LocalizedString("ErrorAppTargetMailboxNotFound", Strings.ResourceManager, new object[]
			{
				mailboxParameterName,
				orgAppParameterName
			});
		}

		public static LocalizedString WriteVerboseSerializedSubscription(string suscription)
		{
			return new LocalizedString("WriteVerboseSerializedSubscription", Strings.ResourceManager, new object[]
			{
				suscription
			});
		}

		public static LocalizedString PushNotificationFailedToValidatePartitionWarning(string appId, string partitionName, string fallback)
		{
			return new LocalizedString("PushNotificationFailedToValidatePartitionWarning", Strings.ResourceManager, new object[]
			{
				appId,
				partitionName,
				fallback
			});
		}

		public static LocalizedString AutoProvisionNoProtocols
		{
			get
			{
				return new LocalizedString("AutoProvisionNoProtocols", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmationMessageDisableProxy(string id, string status)
		{
			return new LocalizedString("ConfirmationMessageDisableProxy", Strings.ResourceManager, new object[]
			{
				id,
				status
			});
		}

		public static LocalizedString ConfirmationMessageEnableProxy(string id, string status)
		{
			return new LocalizedString("ConfirmationMessageEnableProxy", Strings.ResourceManager, new object[]
			{
				id,
				status
			});
		}

		public static LocalizedString ConfirmationMessageModifyOwaOrgExtension(string Identity)
		{
			return new LocalizedString("ConfirmationMessageModifyOwaOrgExtension", Strings.ResourceManager, new object[]
			{
				Identity
			});
		}

		public static LocalizedString WarningExtensionFeatureDisabled
		{
			get
			{
				return new LocalizedString("WarningExtensionFeatureDisabled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmationMessageSetApp(string id)
		{
			return new LocalizedString("ConfirmationMessageSetApp", Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString FacebookAuthorizationError
		{
			get
			{
				return new LocalizedString("FacebookAuthorizationError", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AutoProvisionConnectivity
		{
			get
			{
				return new LocalizedString("AutoProvisionConnectivity", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmationMessageUninstallOwaOrgExtension(string Identity)
		{
			return new LocalizedString("ConfirmationMessageUninstallOwaOrgExtension", Strings.ResourceManager, new object[]
			{
				Identity
			});
		}

		public static LocalizedString ConfirmationMessageUninstallOwaExtension(string Identity, string mailbox)
		{
			return new LocalizedString("ConfirmationMessageUninstallOwaExtension", Strings.ResourceManager, new object[]
			{
				Identity,
				mailbox
			});
		}

		public static LocalizedString PushNotificationAppNotFound(string appName)
		{
			return new LocalizedString("PushNotificationAppNotFound", Strings.ResourceManager, new object[]
			{
				appName
			});
		}

		public static LocalizedString ConfirmationMessageRemoveSinglePushNotificationSubscription(string storeId)
		{
			return new LocalizedString("ConfirmationMessageRemoveSinglePushNotificationSubscription", Strings.ResourceManager, new object[]
			{
				storeId
			});
		}

		public static LocalizedString RemoveCacheMessageConfirmation(CacheIdParameter identity)
		{
			return new LocalizedString("RemoveCacheMessageConfirmation", Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString ErrorMissingFile
		{
			get
			{
				return new LocalizedString("ErrorMissingFile", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmationMessageRemovePushNotificationSubscription(string identity)
		{
			return new LocalizedString("ConfirmationMessageRemovePushNotificationSubscription", Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString ErrorUnsupportedRecipientType(string user, string supportedTypeDetails)
		{
			return new LocalizedString("ErrorUnsupportedRecipientType", Strings.ResourceManager, new object[]
			{
				user,
				supportedTypeDetails
			});
		}

		public static LocalizedString AutoProvisionDebug(LocalizedString activity, LocalizedString description)
		{
			return new LocalizedString("AutoProvisionDebug", Strings.ResourceManager, new object[]
			{
				activity,
				description
			});
		}

		public static LocalizedString CreateLinkedInSubscriptionConfirmation
		{
			get
			{
				return new LocalizedString("CreateLinkedInSubscriptionConfirmation", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoveSubscriptionConfirmation(AggregationSubscriptionIdParameter identity)
		{
			return new LocalizedString("RemoveSubscriptionConfirmation", Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString ErrorCannotReadManifestStream(string ErrorMessage)
		{
			return new LocalizedString("ErrorCannotReadManifestStream", Strings.ResourceManager, new object[]
			{
				ErrorMessage
			});
		}

		public static LocalizedString SubscriptionCannotBeEnabled
		{
			get
			{
				return new LocalizedString("SubscriptionCannotBeEnabled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AutoProvisionConfirmation(PimSubscriptionProxy subscription)
		{
			return new LocalizedString("AutoProvisionConfirmation", Strings.ResourceManager, new object[]
			{
				subscription
			});
		}

		public static LocalizedString ErrorServerCertificateError
		{
			get
			{
				return new LocalizedString("ErrorServerCertificateError", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CacheRpcServerFailed(string rpcServer, string failureReason)
		{
			return new LocalizedString("CacheRpcServerFailed", Strings.ResourceManager, new object[]
			{
				rpcServer,
				failureReason
			});
		}

		public static LocalizedString CacheRpcInvalidServerVersionIssue(string rpcServer)
		{
			return new LocalizedString("CacheRpcInvalidServerVersionIssue", Strings.ResourceManager, new object[]
			{
				rpcServer
			});
		}

		public static LocalizedString ConfirmationMessageInstallOwaOrgExtension
		{
			get
			{
				return new LocalizedString("ConfirmationMessageInstallOwaOrgExtension", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCanNotDownloadPackage
		{
			get
			{
				return new LocalizedString("ErrorCanNotDownloadPackage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AutoProvisionQueryDNS
		{
			get
			{
				return new LocalizedString("AutoProvisionQueryDNS", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CacheRpcExceptionEncountered(Exception exception)
		{
			return new LocalizedString("CacheRpcExceptionEncountered", Strings.ResourceManager, new object[]
			{
				exception
			});
		}

		public static LocalizedString ErrorUninstallProvidedExtension(string ExtensionName)
		{
			return new LocalizedString("ErrorUninstallProvidedExtension", Strings.ResourceManager, new object[]
			{
				ExtensionName
			});
		}

		public static LocalizedString IMAPAccountVerificationFailedException
		{
			get
			{
				return new LocalizedString("IMAPAccountVerificationFailedException", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmationMessageEnableOwaExtension(string Identity, string mailbox)
		{
			return new LocalizedString("ConfirmationMessageEnableOwaExtension", Strings.ResourceManager, new object[]
			{
				Identity,
				mailbox
			});
		}

		public static LocalizedString PushNotificationSucceededToValidatePartition(string appId, string partitionName)
		{
			return new LocalizedString("PushNotificationSucceededToValidatePartition", Strings.ResourceManager, new object[]
			{
				appId,
				partitionName
			});
		}

		public static LocalizedString FacebookEmptyAccessToken
		{
			get
			{
				return new LocalizedString("FacebookEmptyAccessToken", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CreatePopSubscriptionConfirmation(PopSubscriptionProxy subscription)
		{
			return new LocalizedString("CreatePopSubscriptionConfirmation", Strings.ResourceManager, new object[]
			{
				subscription
			});
		}

		public static LocalizedString ErrorDeserializingSubscription(string serializedSubscription, string error)
		{
			return new LocalizedString("ErrorDeserializingSubscription", Strings.ResourceManager, new object[]
			{
				serializedSubscription,
				error
			});
		}

		public static LocalizedString CreateIMAPSubscriptionConfirmation(IMAPSubscriptionProxy subscription)
		{
			return new LocalizedString("CreateIMAPSubscriptionConfirmation", Strings.ResourceManager, new object[]
			{
				subscription
			});
		}

		public static LocalizedString NullSubscriptionStoreId
		{
			get
			{
				return new LocalizedString("NullSubscriptionStoreId", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SubscriptionPasswordTooLong(int length, string subscription)
		{
			return new LocalizedString("SubscriptionPasswordTooLong", Strings.ResourceManager, new object[]
			{
				length,
				subscription
			});
		}

		public static LocalizedString InvalidCacheActionResult(uint cacheActionResult)
		{
			return new LocalizedString("InvalidCacheActionResult", Strings.ResourceManager, new object[]
			{
				cacheActionResult
			});
		}

		public static LocalizedString SetPopSubscriptionConfirmation(AggregationSubscriptionIdParameter identity)
		{
			return new LocalizedString("SetPopSubscriptionConfirmation", Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString acheRpcServerFailed(string rpcServer, string failureReason)
		{
			return new LocalizedString("acheRpcServerFailed", Strings.ResourceManager, new object[]
			{
				rpcServer,
				failureReason
			});
		}

		public static LocalizedString ConfirmationMessageRemoveApp(string id)
		{
			return new LocalizedString("ConfirmationMessageRemoveApp", Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString ErrorCannotSpecifyParameterWithoutOrgExtensionParameter
		{
			get
			{
				return new LocalizedString("ErrorCannotSpecifyParameterWithoutOrgExtensionParameter", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmationMessageInstallOwaExtension(string mailbox)
		{
			return new LocalizedString("ConfirmationMessageInstallOwaExtension", Strings.ResourceManager, new object[]
			{
				mailbox
			});
		}

		public static LocalizedString FacebookTimeoutError
		{
			get
			{
				return new LocalizedString("FacebookTimeoutError", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CacheRpcServerStopped(string rpcServer)
		{
			return new LocalizedString("CacheRpcServerStopped", Strings.ResourceManager, new object[]
			{
				rpcServer
			});
		}

		public static LocalizedString PushNotificationAppFound(string appName, string app)
		{
			return new LocalizedString("PushNotificationAppFound", Strings.ResourceManager, new object[]
			{
				appName,
				app
			});
		}

		public static LocalizedString SetCacheMessageConfirmation(CacheIdParameter identity)
		{
			return new LocalizedString("SetCacheMessageConfirmation", Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString CreateFacebookSubscriptionConfirmation
		{
			get
			{
				return new LocalizedString("CreateFacebookSubscriptionConfirmation", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AutoProvisionResults
		{
			get
			{
				return new LocalizedString("AutoProvisionResults", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNoInputForExtensionInstall
		{
			get
			{
				return new LocalizedString("ErrorNoInputForExtensionInstall", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmationMessageDisableOwaExtension(string Identity, string mailbox)
		{
			return new LocalizedString("ConfirmationMessageDisableOwaExtension", Strings.ResourceManager, new object[]
			{
				Identity,
				mailbox
			});
		}

		public static LocalizedString AutoProvisionCreate
		{
			get
			{
				return new LocalizedString("AutoProvisionCreate", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorReasonUserNotAllowedToInstallReadWriteMailbox
		{
			get
			{
				return new LocalizedString("ErrorReasonUserNotAllowedToInstallReadWriteMailbox", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SetFacebookSubscriptionConfirmation
		{
			get
			{
				return new LocalizedString("SetFacebookSubscriptionConfirmation", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUninstallDefaultExtension(string ExtensionName)
		{
			return new LocalizedString("ErrorUninstallDefaultExtension", Strings.ResourceManager, new object[]
			{
				ExtensionName
			});
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(27);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Management.Mobility.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			InsecureConfirmation = 1372907051U,
			SubscriptionCannotBeChanged = 1199310315U,
			AutoProvisionComplete = 456403923U,
			SetLinkedInSubscriptionConfirmation = 4170829984U,
			FacebookCommunicationError = 843454792U,
			AutoProvisionNoProtocols = 433732448U,
			WarningExtensionFeatureDisabled = 1081354883U,
			FacebookAuthorizationError = 2674661771U,
			AutoProvisionConnectivity = 3132041271U,
			ErrorMissingFile = 3962568818U,
			CreateLinkedInSubscriptionConfirmation = 1935884132U,
			SubscriptionCannotBeEnabled = 232075358U,
			ErrorServerCertificateError = 2121885544U,
			ConfirmationMessageInstallOwaOrgExtension = 3868201023U,
			ErrorCanNotDownloadPackage = 3256811515U,
			AutoProvisionQueryDNS = 562700869U,
			IMAPAccountVerificationFailedException = 424238761U,
			FacebookEmptyAccessToken = 4012336426U,
			NullSubscriptionStoreId = 2684496906U,
			ErrorCannotSpecifyParameterWithoutOrgExtensionParameter = 966086681U,
			FacebookTimeoutError = 408801391U,
			CreateFacebookSubscriptionConfirmation = 1307888814U,
			AutoProvisionResults = 1548922274U,
			ErrorNoInputForExtensionInstall = 3353546040U,
			AutoProvisionCreate = 2973592384U,
			ErrorReasonUserNotAllowedToInstallReadWriteMailbox = 4263159413U,
			SetFacebookSubscriptionConfirmation = 2907685502U
		}

		private enum ParamIDs
		{
			ErrorTooManyUsersInUserList,
			CreateHotmailSubscriptionConfirmation,
			PushNotificationAppPlatformMismatch,
			ConfirmationMessageNewApp,
			ConfirmRemoveUserPushNotificationSubscriptions,
			ImportContactListConfirmation,
			SetHotmailSubscriptionConfirmation,
			SetImapSubscriptionConfirmation,
			PushNotificationAppSecretEncryptionWarning,
			PushNotificationFailedToResolveFallbackPartition,
			ErrorAppTargetMailboxNotFound,
			WriteVerboseSerializedSubscription,
			PushNotificationFailedToValidatePartitionWarning,
			ConfirmationMessageDisableProxy,
			ConfirmationMessageEnableProxy,
			ConfirmationMessageModifyOwaOrgExtension,
			ConfirmationMessageSetApp,
			ConfirmationMessageUninstallOwaOrgExtension,
			ConfirmationMessageUninstallOwaExtension,
			PushNotificationAppNotFound,
			ConfirmationMessageRemoveSinglePushNotificationSubscription,
			RemoveCacheMessageConfirmation,
			ConfirmationMessageRemovePushNotificationSubscription,
			ErrorUnsupportedRecipientType,
			AutoProvisionDebug,
			RemoveSubscriptionConfirmation,
			ErrorCannotReadManifestStream,
			AutoProvisionConfirmation,
			CacheRpcServerFailed,
			CacheRpcInvalidServerVersionIssue,
			CacheRpcExceptionEncountered,
			ErrorUninstallProvidedExtension,
			ConfirmationMessageEnableOwaExtension,
			PushNotificationSucceededToValidatePartition,
			CreatePopSubscriptionConfirmation,
			ErrorDeserializingSubscription,
			CreateIMAPSubscriptionConfirmation,
			SubscriptionPasswordTooLong,
			InvalidCacheActionResult,
			SetPopSubscriptionConfirmation,
			acheRpcServerFailed,
			ConfirmationMessageRemoveApp,
			ConfirmationMessageInstallOwaExtension,
			CacheRpcServerStopped,
			PushNotificationAppFound,
			SetCacheMessageConfirmation,
			ConfirmationMessageDisableOwaExtension,
			ErrorUninstallDefaultExtension
		}
	}
}
