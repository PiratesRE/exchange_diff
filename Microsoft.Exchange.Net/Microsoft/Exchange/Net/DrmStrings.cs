using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Security.RightsManagement.Protectors;

namespace Microsoft.Exchange.Net
{
	internal static class DrmStrings
	{
		static DrmStrings()
		{
			DrmStrings.stringIDs.Add(258986790U, "AlgorithmNotSupported");
			DrmStrings.stringIDs.Add(2776208532U, "RmExceptionActivationGenericMessage");
			DrmStrings.stringIDs.Add(1462171551U, "ReadAttachRenderingErrorOverflow");
			DrmStrings.stringIDs.Add(3575724597U, "ReadOutlookUnicodeStringErrorOverflow");
			DrmStrings.stringIDs.Add(1929574072U, "RmExceptionGenericMessage");
			DrmStrings.stringIDs.Add(3054250489U, "ReadUnicodeStringErrorBefore");
			DrmStrings.stringIDs.Add(4250413746U, "FederationCertificateAccessFailure");
			DrmStrings.stringIDs.Add(1496643996U, "ReadLicenseStringErrorOverflow");
			DrmStrings.stringIDs.Add(2927959246U, "BadDRMPropsSignature");
			DrmStrings.stringIDs.Add(3586924399U, "ReadAttachRenderingError");
			DrmStrings.stringIDs.Add(1879361533U, "ReadUTF8StringErrorBefore");
			DrmStrings.stringIDs.Add(685132634U, "ReadUTF8StringError");
			DrmStrings.stringIDs.Add(642568182U, "FailedToReadManifestFileLocation");
			DrmStrings.stringIDs.Add(1226314129U, "FailedToInitializeRMSEnvironment");
			DrmStrings.stringIDs.Add(2450801847U, "FailedToInstantiateProtectors");
			DrmStrings.stringIDs.Add(74411116U, "ReadUTF8StringErrorOverflow");
			DrmStrings.stringIDs.Add(2063837220U, "UnableToCreateEncryptorHandleWithoutEditRight");
			DrmStrings.stringIDs.Add(2614709070U, "ReadOutlookUnicodeStringErrorBefore");
			DrmStrings.stringIDs.Add(2206873980U, "ReadUnicodeStringErrorOverflow");
			DrmStrings.stringIDs.Add(1454821145U, "ReadLicenseStringErrorBefore");
			DrmStrings.stringIDs.Add(181941971U, "TemplateTypeDistributed");
			DrmStrings.stringIDs.Add(1786778634U, "ReadLicenseStringError");
			DrmStrings.stringIDs.Add(3623501452U, "FailedToDetermineMode");
			DrmStrings.stringIDs.Add(3997623939U, "FailedToLoadIconForAttachment");
			DrmStrings.stringIDs.Add(2213243530U, "ReadUnicodeStringError");
			DrmStrings.stringIDs.Add(508904078U, "TemplateTypeArchived");
			DrmStrings.stringIDs.Add(4021551436U, "FailedToGetTemplateIdFromLicense");
			DrmStrings.stringIDs.Add(1264615704U, "ReadOutlookAnsiStringErrorBefore");
			DrmStrings.stringIDs.Add(3358696673U, "TemplateTypeAll");
			DrmStrings.stringIDs.Add(2637835443U, "ReadOutlookAnsiStringErrorOverflow");
		}

		public static LocalizedString OpenStreamError(string streamName)
		{
			return new LocalizedString("OpenStreamError", "Ex6F3CFE", false, true, DrmStrings.ResourceManager, new object[]
			{
				streamName
			});
		}

		public static LocalizedString FailedToAcquireServerBoxRac(string url)
		{
			return new LocalizedString("FailedToAcquireServerBoxRac", "Ex37A49D", false, true, DrmStrings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString FailedToAcquireUseLicense(string url)
		{
			return new LocalizedString("FailedToAcquireUseLicense", "ExF607F6", false, true, DrmStrings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString InvalidResponseToServerLicensingRequest(string url)
		{
			return new LocalizedString("InvalidResponseToServerLicensingRequest", "Ex79D3F4", false, true, DrmStrings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString AlgorithmNotSupported
		{
			get
			{
				return new LocalizedString("AlgorithmNotSupported", "ExB2F97D", false, true, DrmStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedAtInitializingProtector(Guid protector, int errorCode)
		{
			return new LocalizedString("FailedAtInitializingProtector", "ExC87957", false, true, DrmStrings.ResourceManager, new object[]
			{
				protector,
				errorCode
			});
		}

		public static LocalizedString RmExceptionActivationGenericMessage
		{
			get
			{
				return new LocalizedString("RmExceptionActivationGenericMessage", "", false, false, DrmStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUnprotectingFile(string filename, MsoIpiStatus status)
		{
			return new LocalizedString("ErrorUnprotectingFile", "Ex55B616", false, true, DrmStrings.ResourceManager, new object[]
			{
				filename,
				status
			});
		}

		public static LocalizedString EndpointNotFound(Uri url)
		{
			return new LocalizedString("EndpointNotFound", "Ex53012C", false, true, DrmStrings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString InvalidRpmsgFormat(string reason)
		{
			return new LocalizedString("InvalidRpmsgFormat", "ExAFD8DB", false, true, DrmStrings.ResourceManager, new object[]
			{
				reason
			});
		}

		public static LocalizedString ReadAttachRenderingErrorOverflow
		{
			get
			{
				return new LocalizedString("ReadAttachRenderingErrorOverflow", "Ex4AA34F", false, true, DrmStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidResponseToPrelicensingRequest(string url)
		{
			return new LocalizedString("InvalidResponseToPrelicensingRequest", "Ex72A266", false, true, DrmStrings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString InvalidRmsUrl(string s)
		{
			return new LocalizedString("InvalidRmsUrl", "Ex834753", false, true, DrmStrings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString ReadOutlookUnicodeStringErrorOverflow
		{
			get
			{
				return new LocalizedString("ReadOutlookUnicodeStringErrorOverflow", "ExE95540", false, true, DrmStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidResponseToTemplateInformationRequest(string url)
		{
			return new LocalizedString("InvalidResponseToTemplateInformationRequest", "Ex3929D9", false, true, DrmStrings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString RmExceptionGenericMessage
		{
			get
			{
				return new LocalizedString("RmExceptionGenericMessage", "ExE0C0C9", false, true, DrmStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReadUnicodeStringErrorBefore
		{
			get
			{
				return new LocalizedString("ReadUnicodeStringErrorBefore", "ExDCEA42", false, true, DrmStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FederationCertificateAccessFailure
		{
			get
			{
				return new LocalizedString("FederationCertificateAccessFailure", "ExCD539A", false, true, DrmStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReadLicenseStringErrorOverflow
		{
			get
			{
				return new LocalizedString("ReadLicenseStringErrorOverflow", "ExF58C3C", false, true, DrmStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToGetServerInfo(string url)
		{
			return new LocalizedString("FailedToGetServerInfo", "ExF77EE1", false, true, DrmStrings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString FailedToFindServiceLocation(string url)
		{
			return new LocalizedString("FailedToFindServiceLocation", "ExBE4692", false, true, DrmStrings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString BadDRMPropsSignature
		{
			get
			{
				return new LocalizedString("BadDRMPropsSignature", "Ex33E912", false, true, DrmStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToAcquireTemplateInformation(string url)
		{
			return new LocalizedString("FailedToAcquireTemplateInformation", "Ex5DC656", false, true, DrmStrings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString InvalidResponseToTemplateRequest(string url)
		{
			return new LocalizedString("InvalidResponseToTemplateRequest", "Ex7D60C6", false, true, DrmStrings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString ReadAttachRenderingError
		{
			get
			{
				return new LocalizedString("ReadAttachRenderingError", "Ex3E5F16", false, true, DrmStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExternalRmsServerFailure(Uri url, string ecode)
		{
			return new LocalizedString("ExternalRmsServerFailure", "ExAB4BF5", false, true, DrmStrings.ResourceManager, new object[]
			{
				url,
				ecode
			});
		}

		public static LocalizedString ReadUTF8StringErrorBefore
		{
			get
			{
				return new LocalizedString("ReadUTF8StringErrorBefore", "Ex4D2772", false, true, DrmStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToAcquireTemplates(string url)
		{
			return new LocalizedString("FailedToAcquireTemplates", "ExE6B8FF", false, true, DrmStrings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString ReadUTF8StringError
		{
			get
			{
				return new LocalizedString("ReadUTF8StringError", "Ex76AC31", false, true, DrmStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidTemplateInformationResponse(string url)
		{
			return new LocalizedString("InvalidTemplateInformationResponse", "ExD2F49C", false, true, DrmStrings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString InvalidResponseToServerBoxRacRequest(string url)
		{
			return new LocalizedString("InvalidResponseToServerBoxRacRequest", "Ex6F874F", false, true, DrmStrings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString FailedToReadManifestFileLocation
		{
			get
			{
				return new LocalizedString("FailedToReadManifestFileLocation", "ExABABE8", false, true, DrmStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorProtectingFile(string filename, MsoIpiStatus status)
		{
			return new LocalizedString("ErrorProtectingFile", "ExC78DC4", false, true, DrmStrings.ResourceManager, new object[]
			{
				filename,
				status
			});
		}

		public static LocalizedString FailedToGetUnboundLicenseObject(string useLicense)
		{
			return new LocalizedString("FailedToGetUnboundLicenseObject", "Ex0E78A4", false, true, DrmStrings.ResourceManager, new object[]
			{
				useLicense
			});
		}

		public static LocalizedString FailedAtGettingStatusOfProtection(string filename, int errorCode)
		{
			return new LocalizedString("FailedAtGettingStatusOfProtection", "Ex0027F4", false, true, DrmStrings.ResourceManager, new object[]
			{
				filename,
				errorCode
			});
		}

		public static LocalizedString FailedToAcquireClc(string url)
		{
			return new LocalizedString("FailedToAcquireClc", "Ex52FA00", false, true, DrmStrings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString FailedToInitializeRMSEnvironment
		{
			get
			{
				return new LocalizedString("FailedToInitializeRMSEnvironment", "ExCD190B", false, true, DrmStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToInstantiateProtectors
		{
			get
			{
				return new LocalizedString("FailedToInstantiateProtectors", "Ex310A03", false, true, DrmStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReadUTF8StringErrorOverflow
		{
			get
			{
				return new LocalizedString("ReadUTF8StringErrorOverflow", "Ex67DE75", false, true, DrmStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnableToCreateEncryptorHandleWithoutEditRight
		{
			get
			{
				return new LocalizedString("UnableToCreateEncryptorHandleWithoutEditRight", "Ex13F009", false, true, DrmStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReadOutlookUnicodeStringErrorBefore
		{
			get
			{
				return new LocalizedString("ReadOutlookUnicodeStringErrorBefore", "Ex119BD8", false, true, DrmStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReadUnicodeStringErrorOverflow
		{
			get
			{
				return new LocalizedString("ReadUnicodeStringErrorOverflow", "Ex0B704C", false, true, DrmStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReadLicenseStringErrorBefore
		{
			get
			{
				return new LocalizedString("ReadLicenseStringErrorBefore", "ExD2F9D4", false, true, DrmStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TemplateTypeDistributed
		{
			get
			{
				return new LocalizedString("TemplateTypeDistributed", "", false, false, DrmStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageSecurityError(Uri url)
		{
			return new LocalizedString("MessageSecurityError", "Ex37CBAB", false, true, DrmStrings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString InvalidUseLicenseResponse(string url)
		{
			return new LocalizedString("InvalidUseLicenseResponse", "Ex78FDD1", false, true, DrmStrings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString FailedToProtectFile(string filename, int errorCode)
		{
			return new LocalizedString("FailedToProtectFile", "Ex33EEB6", false, true, DrmStrings.ResourceManager, new object[]
			{
				filename,
				errorCode
			});
		}

		public static LocalizedString ReadLicenseStringError
		{
			get
			{
				return new LocalizedString("ReadLicenseStringError", "Ex5BE727", false, true, DrmStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToRequestDelegationToken(Uri url, Uri targetUrl)
		{
			return new LocalizedString("FailedToRequestDelegationToken", "Ex805543", false, true, DrmStrings.ResourceManager, new object[]
			{
				url,
				targetUrl
			});
		}

		public static LocalizedString FailedToDetermineMode
		{
			get
			{
				return new LocalizedString("FailedToDetermineMode", "ExE5BA02", false, true, DrmStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidResponseToClcRequest(string url)
		{
			return new LocalizedString("InvalidResponseToClcRequest", "Ex3F83DE", false, true, DrmStrings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString FailedToUnprotectFile(string filename, int errorCode)
		{
			return new LocalizedString("FailedToUnprotectFile", "Ex15268E", false, true, DrmStrings.ResourceManager, new object[]
			{
				filename,
				errorCode
			});
		}

		public static LocalizedString ActionNotSupported(Uri url)
		{
			return new LocalizedString("ActionNotSupported", "ExF01168", false, true, DrmStrings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString TimeoutError(Uri url)
		{
			return new LocalizedString("TimeoutError", "ExB5AA9B", false, true, DrmStrings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString FailedToLoadIconForAttachment
		{
			get
			{
				return new LocalizedString("FailedToLoadIconForAttachment", "ExC58448", false, true, DrmStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReadUnicodeStringError
		{
			get
			{
				return new LocalizedString("ReadUnicodeStringError", "ExC431E0", false, true, DrmStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CommunicationError(Uri url)
		{
			return new LocalizedString("CommunicationError", "Ex8FE8AA", false, true, DrmStrings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString TemplateTypeArchived
		{
			get
			{
				return new LocalizedString("TemplateTypeArchived", "", false, false, DrmStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToGetTemplateIdFromLicense
		{
			get
			{
				return new LocalizedString("FailedToGetTemplateIdFromLicense", "Ex90AF73", false, true, DrmStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReadOutlookAnsiStringErrorBefore
		{
			get
			{
				return new LocalizedString("ReadOutlookAnsiStringErrorBefore", "ExD8E10D", false, true, DrmStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TemplateTypeAll
		{
			get
			{
				return new LocalizedString("TemplateTypeAll", "", false, false, DrmStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToAcquirePreLicense(string url)
		{
			return new LocalizedString("FailedToAcquirePreLicense", "ExDB1526", false, true, DrmStrings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString FailedToOpenManifestFile(string location)
		{
			return new LocalizedString("FailedToOpenManifestFile", "Ex4B2F45", false, true, DrmStrings.ResourceManager, new object[]
			{
				location
			});
		}

		public static LocalizedString FailedToParseUnboundLicense(string useLicense)
		{
			return new LocalizedString("FailedToParseUnboundLicense", "Ex69EEAA", false, true, DrmStrings.ResourceManager, new object[]
			{
				useLicense
			});
		}

		public static LocalizedString FailedAtSettingProtectorLanguageId(Guid protector, int errorCode)
		{
			return new LocalizedString("FailedAtSettingProtectorLanguageId", "ExB2FE84", false, true, DrmStrings.ResourceManager, new object[]
			{
				protector,
				errorCode
			});
		}

		public static LocalizedString OpenStorageError(string storageName)
		{
			return new LocalizedString("OpenStorageError", "ExF1A762", false, true, DrmStrings.ResourceManager, new object[]
			{
				storageName
			});
		}

		public static LocalizedString ReadOutlookAnsiStringErrorOverflow
		{
			get
			{
				return new LocalizedString("ReadOutlookAnsiStringErrorOverflow", "Ex3492A0", false, true, DrmStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToEnumerateProtectors(string registryKey)
		{
			return new LocalizedString("FailedToEnumerateProtectors", "Ex7D4517", false, true, DrmStrings.ResourceManager, new object[]
			{
				registryKey
			});
		}

		public static LocalizedString InvalidResponseToCertificationRequest(string url)
		{
			return new LocalizedString("InvalidResponseToCertificationRequest", "ExC10E85", false, true, DrmStrings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString GetLocalizedString(DrmStrings.IDs key)
		{
			return new LocalizedString(DrmStrings.stringIDs[(uint)key], DrmStrings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(30);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Net.DrmStrings", typeof(DrmStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			AlgorithmNotSupported = 258986790U,
			RmExceptionActivationGenericMessage = 2776208532U,
			ReadAttachRenderingErrorOverflow = 1462171551U,
			ReadOutlookUnicodeStringErrorOverflow = 3575724597U,
			RmExceptionGenericMessage = 1929574072U,
			ReadUnicodeStringErrorBefore = 3054250489U,
			FederationCertificateAccessFailure = 4250413746U,
			ReadLicenseStringErrorOverflow = 1496643996U,
			BadDRMPropsSignature = 2927959246U,
			ReadAttachRenderingError = 3586924399U,
			ReadUTF8StringErrorBefore = 1879361533U,
			ReadUTF8StringError = 685132634U,
			FailedToReadManifestFileLocation = 642568182U,
			FailedToInitializeRMSEnvironment = 1226314129U,
			FailedToInstantiateProtectors = 2450801847U,
			ReadUTF8StringErrorOverflow = 74411116U,
			UnableToCreateEncryptorHandleWithoutEditRight = 2063837220U,
			ReadOutlookUnicodeStringErrorBefore = 2614709070U,
			ReadUnicodeStringErrorOverflow = 2206873980U,
			ReadLicenseStringErrorBefore = 1454821145U,
			TemplateTypeDistributed = 181941971U,
			ReadLicenseStringError = 1786778634U,
			FailedToDetermineMode = 3623501452U,
			FailedToLoadIconForAttachment = 3997623939U,
			ReadUnicodeStringError = 2213243530U,
			TemplateTypeArchived = 508904078U,
			FailedToGetTemplateIdFromLicense = 4021551436U,
			ReadOutlookAnsiStringErrorBefore = 1264615704U,
			TemplateTypeAll = 3358696673U,
			ReadOutlookAnsiStringErrorOverflow = 2637835443U
		}

		private enum ParamIDs
		{
			OpenStreamError,
			FailedToAcquireServerBoxRac,
			FailedToAcquireUseLicense,
			InvalidResponseToServerLicensingRequest,
			FailedAtInitializingProtector,
			ErrorUnprotectingFile,
			EndpointNotFound,
			InvalidRpmsgFormat,
			InvalidResponseToPrelicensingRequest,
			InvalidRmsUrl,
			InvalidResponseToTemplateInformationRequest,
			FailedToGetServerInfo,
			FailedToFindServiceLocation,
			FailedToAcquireTemplateInformation,
			InvalidResponseToTemplateRequest,
			ExternalRmsServerFailure,
			FailedToAcquireTemplates,
			InvalidTemplateInformationResponse,
			InvalidResponseToServerBoxRacRequest,
			ErrorProtectingFile,
			FailedToGetUnboundLicenseObject,
			FailedAtGettingStatusOfProtection,
			FailedToAcquireClc,
			MessageSecurityError,
			InvalidUseLicenseResponse,
			FailedToProtectFile,
			FailedToRequestDelegationToken,
			InvalidResponseToClcRequest,
			FailedToUnprotectFile,
			ActionNotSupported,
			TimeoutError,
			CommunicationError,
			FailedToAcquirePreLicense,
			FailedToOpenManifestFile,
			FailedToParseUnboundLicense,
			FailedAtSettingProtectorLanguageId,
			OpenStorageError,
			FailedToEnumerateProtectors,
			InvalidResponseToCertificationRequest
		}
	}
}
