using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.RightsManagement
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class Constants
	{
		public const string SamlTokenType11 = "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV1.1";

		public const long MaxSizeOfMexData = 51200L;

		public const long MaxReceivedMessageSizeInBytes = 2097152L;

		public const int MaxStringContentLengthInBytes = 2097152;

		public static readonly TimeSpan BindingTimeout = TimeSpan.FromSeconds(30.0);

		public static readonly byte[] EmptyByteArray = Array<byte>.Empty;

		public enum State
		{
			None,
			BeginAcquireRMSTemplate,
			BeginAcquireRMSTemplateFirstRequest,
			BeginAcquireRMSTemplatePendingRequest,
			AcquireRmsTemplatesCallback,
			EndAcquireRMSTemplate,
			PerTenantQueryControllerInvokeCallback,
			BeginAcquirePreLicense,
			AcquirePreLicenseCallback,
			EndAcquirePreLicense,
			BeginAcquireInternalOrganizationRACAndCLC,
			BeginAcquireInternalOrganizationRACAndCLCFirstRequest,
			BeginAcquireInternalOrganizationRACAndCLCPendingRequest,
			AcquireServerRacCallback,
			BeginAcquireClc,
			AcquireClcCallback,
			EndAcquireInternalOrganizationRACAndCLC,
			BeginAcquireSuperUserUseLicense,
			AcquireTenantLicenseCallback,
			BeginAcquireUseLicense,
			AcquireUseLicenseCallback,
			BeginAcquireFederationRAC,
			BeginAcquireFederationServerLicense,
			AcquireExternalRMSInfoCertificationCallback,
			WCFBeginCertify,
			AcquireTenantExternalRacCallback,
			AcquireFederationRacCallback,
			AcquireExternalRMSInfoLicensingCallback,
			WCFBeginAcquireLicense,
			AcquireFederationLicenseCallback,
			BeginAcquireServerInfo,
			BeginFindServiceLocationsFirstRequest,
			BeginFindServiceLocationsPendingRequest,
			EndAcquireServerInfo,
			AcquireServiceLocationCallback,
			BeginDownloadCertificationMexData,
			AcquireCertificationMexCallback,
			AcquireServerLicensingMexCallback,
			BeginDownloadServerLicensingMexData,
			EndAcquireFederationRAC,
			EndAcquireUseLicense,
			BeginAcquireUseLicenseAndUsageRights,
			AcquireUseLicenseAndUsageRightsCallbackForUseLicense,
			AcquireUseLicenseAndUsageRightsCallbackForPreLicense,
			EndAcquireUseLicenseAndUsageRights
		}
	}
}
