using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(2677654617U, "InvalidAppId");
			Strings.stringIDs.Add(3805371211U, "AzureDeviceMissingPayload");
			Strings.stringIDs.Add(1268441430U, "ApnsChannelAuthenticationTimeout");
			Strings.stringIDs.Add(31607964U, "GcmInvalidPayload");
			Strings.stringIDs.Add(735444327U, "InvalidSubscriptionId");
			Strings.stringIDs.Add(121229349U, "ValidationErrorInvalidAuthenticationKey");
			Strings.stringIDs.Add(3217301529U, "AzureChallengeEmptyUriTemplate");
			Strings.stringIDs.Add(2677736001U, "InvalidWnsBadgeValue");
			Strings.stringIDs.Add(3272335588U, "WebAppInvalidPayload");
			Strings.stringIDs.Add(3255907000U, "AzureDeviceEmptyUriTemplate");
			Strings.stringIDs.Add(2799225028U, "AzureChallengeMissingPayload");
			Strings.stringIDs.Add(4047283153U, "AzureChallengeEmptySasKey");
			Strings.stringIDs.Add(734509627U, "CannotResolveProxyAppFromAD");
			Strings.stringIDs.Add(678895536U, "AzureDeviceEmptySasKey");
			Strings.stringIDs.Add(2472023412U, "WebAppInvalidAction");
			Strings.stringIDs.Add(4200803720U, "InvalidProxyNotificationBatch");
			Strings.stringIDs.Add(1133222470U, "InvalidEmailCount");
			Strings.stringIDs.Add(816696259U, "InvalidPayload");
		}

		public static LocalizedString InvalidPayloadFormat(string error)
		{
			return new LocalizedString("InvalidPayloadFormat", Strings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString AzureChallengeInvalidDeviceId(string deviceId)
		{
			return new LocalizedString("AzureChallengeInvalidDeviceId", Strings.ResourceManager, new object[]
			{
				deviceId
			});
		}

		public static LocalizedString InvalidWnsUriScheme(string uri)
		{
			return new LocalizedString("InvalidWnsUriScheme", Strings.ResourceManager, new object[]
			{
				uri
			});
		}

		public static LocalizedString ValidationErrorRangeVersion(Version minimum, Version maximum)
		{
			return new LocalizedString("ValidationErrorRangeVersion", Strings.ResourceManager, new object[]
			{
				minimum,
				maximum
			});
		}

		public static LocalizedString InvalidPayloadLength(int length, string jsonPayload)
		{
			return new LocalizedString("InvalidPayloadLength", Strings.ResourceManager, new object[]
			{
				length,
				jsonPayload
			});
		}

		public static LocalizedString InvalidAppId
		{
			get
			{
				return new LocalizedString("InvalidAppId", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidReportFromApns(string status)
		{
			return new LocalizedString("InvalidReportFromApns", Strings.ResourceManager, new object[]
			{
				status
			});
		}

		public static LocalizedString ApnsFeedbackFileIdInvalidPseudoAppId(string serializedId)
		{
			return new LocalizedString("ApnsFeedbackFileIdInvalidPseudoAppId", Strings.ResourceManager, new object[]
			{
				serializedId
			});
		}

		public static LocalizedString AzureDeviceMissingPayload
		{
			get
			{
				return new LocalizedString("AzureDeviceMissingPayload", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AzureInvalidRecipientId(string recipientId)
		{
			return new LocalizedString("AzureInvalidRecipientId", Strings.ResourceManager, new object[]
			{
				recipientId
			});
		}

		public static LocalizedString ApnsFeedbackFileIdInvalidDate(string serializedId, string error)
		{
			return new LocalizedString("ApnsFeedbackFileIdInvalidDate", Strings.ResourceManager, new object[]
			{
				serializedId,
				error
			});
		}

		public static LocalizedString ApnsFeedbackFileIdInvalidCharacters(string serializedId, string error)
		{
			return new LocalizedString("ApnsFeedbackFileIdInvalidCharacters", Strings.ResourceManager, new object[]
			{
				serializedId,
				error
			});
		}

		public static LocalizedString ApnsFeedbackPackageFeedbackNotFound(string path)
		{
			return new LocalizedString("ApnsFeedbackPackageFeedbackNotFound", Strings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString ApnsChannelAuthenticationTimeout
		{
			get
			{
				return new LocalizedString("ApnsChannelAuthenticationTimeout", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GcmInvalidPayload
		{
			get
			{
				return new LocalizedString("GcmInvalidPayload", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ApnsCertificatePrivateKeyError(string name, string thumbprint)
		{
			return new LocalizedString("ApnsCertificatePrivateKeyError", Strings.ResourceManager, new object[]
			{
				name,
				thumbprint
			});
		}

		public static LocalizedString InvalidSubscriptionId
		{
			get
			{
				return new LocalizedString("InvalidSubscriptionId", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidWnsPayloadLength(int maxLen, string payload)
		{
			return new LocalizedString("InvalidWnsPayloadLength", Strings.ResourceManager, new object[]
			{
				maxLen,
				payload
			});
		}

		public static LocalizedString ApnsFeedbackResponseInvalidLength(int length)
		{
			return new LocalizedString("ApnsFeedbackResponseInvalidLength", Strings.ResourceManager, new object[]
			{
				length
			});
		}

		public static LocalizedString ValidationErrorNonNegativeInteger(string propertyName, int value)
		{
			return new LocalizedString("ValidationErrorNonNegativeInteger", Strings.ResourceManager, new object[]
			{
				propertyName,
				value
			});
		}

		public static LocalizedString GcmInvalidNotificationReported(string report)
		{
			return new LocalizedString("GcmInvalidNotificationReported", Strings.ResourceManager, new object[]
			{
				report
			});
		}

		public static LocalizedString InvalidWnsUri(string uri, string error)
		{
			return new LocalizedString("InvalidWnsUri", Strings.ResourceManager, new object[]
			{
				uri,
				error
			});
		}

		public static LocalizedString ValidationErrorInvalidAuthenticationKey
		{
			get
			{
				return new LocalizedString("ValidationErrorInvalidAuthenticationKey", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ApnsFeedbackFileGetFilesError(string root, string searchPattern, string error)
		{
			return new LocalizedString("ApnsFeedbackFileGetFilesError", Strings.ResourceManager, new object[]
			{
				root,
				searchPattern,
				error
			});
		}

		public static LocalizedString ApnsFeedbackPackageRemovalFailed(string path, string error)
		{
			return new LocalizedString("ApnsFeedbackPackageRemovalFailed", Strings.ResourceManager, new object[]
			{
				path,
				error
			});
		}

		public static LocalizedString DataProtectionDecryptingError(string protectedText, string error)
		{
			return new LocalizedString("DataProtectionDecryptingError", Strings.ResourceManager, new object[]
			{
				protectedText,
				error
			});
		}

		public static LocalizedString ValidationErrorDeviceRegistrationAppId(string name, string expected)
		{
			return new LocalizedString("ValidationErrorDeviceRegistrationAppId", Strings.ResourceManager, new object[]
			{
				name,
				expected
			});
		}

		public static LocalizedString AzureChallengeEmptyUriTemplate
		{
			get
			{
				return new LocalizedString("AzureChallengeEmptyUriTemplate", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidWnsBadgeValue
		{
			get
			{
				return new LocalizedString("InvalidWnsBadgeValue", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidExpiration(int expiration)
		{
			return new LocalizedString("InvalidExpiration", Strings.ResourceManager, new object[]
			{
				expiration
			});
		}

		public static LocalizedString WebAppInvalidPayload
		{
			get
			{
				return new LocalizedString("WebAppInvalidPayload", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ApnsFeedbackFileIdInsufficientComponents(string serializedId)
		{
			return new LocalizedString("ApnsFeedbackFileIdInsufficientComponents", Strings.ResourceManager, new object[]
			{
				serializedId
			});
		}

		public static LocalizedString AzureDeviceInvalidTag(string tag)
		{
			return new LocalizedString("AzureDeviceInvalidTag", Strings.ResourceManager, new object[]
			{
				tag
			});
		}

		public static LocalizedString AzureDeviceEmptyUriTemplate
		{
			get
			{
				return new LocalizedString("AzureDeviceEmptyUriTemplate", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ApnsFeedbackPackageExtractionFailed(string path, string error)
		{
			return new LocalizedString("ApnsFeedbackPackageExtractionFailed", Strings.ResourceManager, new object[]
			{
				path,
				error
			});
		}

		public static LocalizedString ApnsFeedbackFileIdInvalidExtension(string extension)
		{
			return new LocalizedString("ApnsFeedbackFileIdInvalidExtension", Strings.ResourceManager, new object[]
			{
				extension
			});
		}

		public static LocalizedString ApnsFeedbackFileLoadError(string filePath, string error)
		{
			return new LocalizedString("ApnsFeedbackFileLoadError", Strings.ResourceManager, new object[]
			{
				filePath,
				error
			});
		}

		public static LocalizedString AzureChallengeMissingPayload
		{
			get
			{
				return new LocalizedString("AzureChallengeMissingPayload", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotCreateValidSasToken(string appId, string sasToken)
		{
			return new LocalizedString("CannotCreateValidSasToken", Strings.ResourceManager, new object[]
			{
				appId,
				sasToken
			});
		}

		public static LocalizedString ApnsPublisherExpiredToken(string notification)
		{
			return new LocalizedString("ApnsPublisherExpiredToken", Strings.ResourceManager, new object[]
			{
				notification
			});
		}

		public static LocalizedString InvalidWnsTimeToLive(int ttl)
		{
			return new LocalizedString("InvalidWnsTimeToLive", Strings.ResourceManager, new object[]
			{
				ttl
			});
		}

		public static LocalizedString AzureChallengeEmptySasKey
		{
			get
			{
				return new LocalizedString("AzureChallengeEmptySasKey", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ValidationErrorEmptyString(string propertyName)
		{
			return new LocalizedString("ValidationErrorEmptyString", Strings.ResourceManager, new object[]
			{
				propertyName
			});
		}

		public static LocalizedString ApnsCertificateExternalException(string thumbprint, string errorMessage)
		{
			return new LocalizedString("ApnsCertificateExternalException", Strings.ResourceManager, new object[]
			{
				thumbprint,
				errorMessage
			});
		}

		public static LocalizedString AzureDeviceInvalidDeviceId(string deviceId)
		{
			return new LocalizedString("AzureDeviceInvalidDeviceId", Strings.ResourceManager, new object[]
			{
				deviceId
			});
		}

		public static LocalizedString ValidationErrorInvalidSasToken(string sasToken)
		{
			return new LocalizedString("ValidationErrorInvalidSasToken", Strings.ResourceManager, new object[]
			{
				sasToken
			});
		}

		public static LocalizedString CannotResolveProxyAppFromAD
		{
			get
			{
				return new LocalizedString("CannotResolveProxyAppFromAD", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ValidationErrorRangeInteger(string propertyName, int lowest, int highest, int value)
		{
			return new LocalizedString("ValidationErrorRangeInteger", Strings.ResourceManager, new object[]
			{
				propertyName,
				lowest,
				highest,
				value
			});
		}

		public static LocalizedString AzureChallengeInvalidPlatformOnPayload(string platform)
		{
			return new LocalizedString("AzureChallengeInvalidPlatformOnPayload", Strings.ResourceManager, new object[]
			{
				platform
			});
		}

		public static LocalizedString ApnsFeedbackError(string appId, string exceptionType, string errorMessage)
		{
			return new LocalizedString("ApnsFeedbackError", Strings.ResourceManager, new object[]
			{
				appId,
				exceptionType,
				errorMessage
			});
		}

		public static LocalizedString GcmInvalidTimeToLive(int ttl)
		{
			return new LocalizedString("GcmInvalidTimeToLive", Strings.ResourceManager, new object[]
			{
				ttl
			});
		}

		public static LocalizedString ValidationErrorPositiveInteger(string propertyName, int value)
		{
			return new LocalizedString("ValidationErrorPositiveInteger", Strings.ResourceManager, new object[]
			{
				propertyName,
				value
			});
		}

		public static LocalizedString NotificationDroppedDueToLastUpdateTime(string nextUpdate)
		{
			return new LocalizedString("NotificationDroppedDueToLastUpdateTime", Strings.ResourceManager, new object[]
			{
				nextUpdate
			});
		}

		public static LocalizedString AzureDeviceEmptySasKey
		{
			get
			{
				return new LocalizedString("AzureDeviceEmptySasKey", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ValidationErrorHubCreationAppId(string name, string expected)
		{
			return new LocalizedString("ValidationErrorHubCreationAppId", Strings.ResourceManager, new object[]
			{
				name,
				expected
			});
		}

		public static LocalizedString DataProtectionEncryptingError(string error)
		{
			return new LocalizedString("DataProtectionEncryptingError", Strings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString ApnsFeedbackResponseInvalidFileLine(string line, string error)
		{
			return new LocalizedString("ApnsFeedbackResponseInvalidFileLine", Strings.ResourceManager, new object[]
			{
				line,
				error
			});
		}

		public static LocalizedString ApnsFeedbackFileIdInvalidDirectory(string folderName)
		{
			return new LocalizedString("ApnsFeedbackFileIdInvalidDirectory", Strings.ResourceManager, new object[]
			{
				folderName
			});
		}

		public static LocalizedString ApnsCertificateNotFound(string thumbprint)
		{
			return new LocalizedString("ApnsCertificateNotFound", Strings.ResourceManager, new object[]
			{
				thumbprint
			});
		}

		public static LocalizedString InvalidWnsTemplate(string binding)
		{
			return new LocalizedString("InvalidWnsTemplate", Strings.ResourceManager, new object[]
			{
				binding
			});
		}

		public static LocalizedString AzureCannotResolveExternalOrgId(string error)
		{
			return new LocalizedString("AzureCannotResolveExternalOrgId", Strings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString ApnsFeedbackPackageUnexpectedMetadataResult(string path, int number)
		{
			return new LocalizedString("ApnsFeedbackPackageUnexpectedMetadataResult", Strings.ResourceManager, new object[]
			{
				path,
				number
			});
		}

		public static LocalizedString ValidationErrorChallengeRequestAppId(string name, string expected)
		{
			return new LocalizedString("ValidationErrorChallengeRequestAppId", Strings.ResourceManager, new object[]
			{
				name,
				expected
			});
		}

		public static LocalizedString GcmInvalidPayloadLength(int currentLength, string payloadExtract)
		{
			return new LocalizedString("GcmInvalidPayloadLength", Strings.ResourceManager, new object[]
			{
				currentLength,
				payloadExtract
			});
		}

		public static LocalizedString WebAppInvalidAction
		{
			get
			{
				return new LocalizedString("WebAppInvalidAction", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ValidationErrorInvalidUri(string propertyName, string value, string extra)
		{
			return new LocalizedString("ValidationErrorInvalidUri", Strings.ResourceManager, new object[]
			{
				propertyName,
				value,
				extra
			});
		}

		public static LocalizedString InvalidProxyNotificationBatch
		{
			get
			{
				return new LocalizedString("InvalidProxyNotificationBatch", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidAzurePayloadLength(int maxLen, string payload)
		{
			return new LocalizedString("InvalidAzurePayloadLength", Strings.ResourceManager, new object[]
			{
				maxLen,
				payload
			});
		}

		public static LocalizedString GcmInvalidRegistrationId(string id)
		{
			return new LocalizedString("GcmInvalidRegistrationId", Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString InvalidLastSubscriptionUpdate(string date)
		{
			return new LocalizedString("InvalidLastSubscriptionUpdate", Strings.ResourceManager, new object[]
			{
				date
			});
		}

		public static LocalizedString InvalidWnsLanguage(string lang, string error)
		{
			return new LocalizedString("InvalidWnsLanguage", Strings.ResourceManager, new object[]
			{
				lang,
				error
			});
		}

		public static LocalizedString InvalidEmailCount
		{
			get
			{
				return new LocalizedString("InvalidEmailCount", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidDeviceToken(string token)
		{
			return new LocalizedString("InvalidDeviceToken", Strings.ResourceManager, new object[]
			{
				token
			});
		}

		public static LocalizedString InvalidWnsDeviceUri(string uri, string error)
		{
			return new LocalizedString("InvalidWnsDeviceUri", Strings.ResourceManager, new object[]
			{
				uri,
				error
			});
		}

		public static LocalizedString InvalidWnsAttributeIsMandatory(string attributeName)
		{
			return new LocalizedString("InvalidWnsAttributeIsMandatory", Strings.ResourceManager, new object[]
			{
				attributeName
			});
		}

		public static LocalizedString InvalidPayload
		{
			get
			{
				return new LocalizedString("InvalidPayload", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WnsChannelInvalidNotificationReported(string report)
		{
			return new LocalizedString("WnsChannelInvalidNotificationReported", Strings.ResourceManager, new object[]
			{
				report
			});
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(18);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.PushNotifications.Publishers.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			InvalidAppId = 2677654617U,
			AzureDeviceMissingPayload = 3805371211U,
			ApnsChannelAuthenticationTimeout = 1268441430U,
			GcmInvalidPayload = 31607964U,
			InvalidSubscriptionId = 735444327U,
			ValidationErrorInvalidAuthenticationKey = 121229349U,
			AzureChallengeEmptyUriTemplate = 3217301529U,
			InvalidWnsBadgeValue = 2677736001U,
			WebAppInvalidPayload = 3272335588U,
			AzureDeviceEmptyUriTemplate = 3255907000U,
			AzureChallengeMissingPayload = 2799225028U,
			AzureChallengeEmptySasKey = 4047283153U,
			CannotResolveProxyAppFromAD = 734509627U,
			AzureDeviceEmptySasKey = 678895536U,
			WebAppInvalidAction = 2472023412U,
			InvalidProxyNotificationBatch = 4200803720U,
			InvalidEmailCount = 1133222470U,
			InvalidPayload = 816696259U
		}

		private enum ParamIDs
		{
			InvalidPayloadFormat,
			AzureChallengeInvalidDeviceId,
			InvalidWnsUriScheme,
			ValidationErrorRangeVersion,
			InvalidPayloadLength,
			InvalidReportFromApns,
			ApnsFeedbackFileIdInvalidPseudoAppId,
			AzureInvalidRecipientId,
			ApnsFeedbackFileIdInvalidDate,
			ApnsFeedbackFileIdInvalidCharacters,
			ApnsFeedbackPackageFeedbackNotFound,
			ApnsCertificatePrivateKeyError,
			InvalidWnsPayloadLength,
			ApnsFeedbackResponseInvalidLength,
			ValidationErrorNonNegativeInteger,
			GcmInvalidNotificationReported,
			InvalidWnsUri,
			ApnsFeedbackFileGetFilesError,
			ApnsFeedbackPackageRemovalFailed,
			DataProtectionDecryptingError,
			ValidationErrorDeviceRegistrationAppId,
			InvalidExpiration,
			ApnsFeedbackFileIdInsufficientComponents,
			AzureDeviceInvalidTag,
			ApnsFeedbackPackageExtractionFailed,
			ApnsFeedbackFileIdInvalidExtension,
			ApnsFeedbackFileLoadError,
			CannotCreateValidSasToken,
			ApnsPublisherExpiredToken,
			InvalidWnsTimeToLive,
			ValidationErrorEmptyString,
			ApnsCertificateExternalException,
			AzureDeviceInvalidDeviceId,
			ValidationErrorInvalidSasToken,
			ValidationErrorRangeInteger,
			AzureChallengeInvalidPlatformOnPayload,
			ApnsFeedbackError,
			GcmInvalidTimeToLive,
			ValidationErrorPositiveInteger,
			NotificationDroppedDueToLastUpdateTime,
			ValidationErrorHubCreationAppId,
			DataProtectionEncryptingError,
			ApnsFeedbackResponseInvalidFileLine,
			ApnsFeedbackFileIdInvalidDirectory,
			ApnsCertificateNotFound,
			InvalidWnsTemplate,
			AzureCannotResolveExternalOrgId,
			ApnsFeedbackPackageUnexpectedMetadataResult,
			ValidationErrorChallengeRequestAppId,
			GcmInvalidPayloadLength,
			ValidationErrorInvalidUri,
			InvalidAzurePayloadLength,
			GcmInvalidRegistrationId,
			InvalidLastSubscriptionUpdate,
			InvalidWnsLanguage,
			InvalidDeviceToken,
			InvalidWnsDeviceUri,
			InvalidWnsAttributeIsMandatory,
			WnsChannelInvalidNotificationReported
		}
	}
}
