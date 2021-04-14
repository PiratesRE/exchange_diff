using System;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace Microsoft.com.IPC.WSService
{
	[GeneratedCode("System.Runtime.Serialization", "3.0.0.0")]
	[DataContract(Name = "RmsErrors", Namespace = "http://microsoft.com/IPC/WSService")]
	public enum RmsErrors
	{
		[EnumMember]
		RmsInternalFailure,
		[EnumMember]
		InvalidArgument,
		[EnumMember]
		UnsupportedDataVersion,
		[EnumMember]
		ClusterDecommissioned,
		[EnumMember]
		RequestedFeatureIsDisabled,
		[EnumMember]
		UnauthorizedAccess,
		[EnumMember]
		VerifyMachineCertificateChainFailed,
		[EnumMember]
		EmailAddressVerificationFailure,
		[EnumMember]
		UserNotFoundInActiveDirectory,
		[EnumMember]
		GroupIdentityCredentialHasInvalidSignature,
		[EnumMember]
		GroupIdentityCredentialHasInvalidTimeRange,
		[EnumMember]
		UntrustedGroupIdentityCredential,
		[EnumMember]
		ExcludedGroupIdentityCredential,
		[EnumMember]
		UnexpectedGroupIdentityCredential,
		[EnumMember]
		UnauthorizedEmailAddress,
		[EnumMember]
		IssuanceLicenseHasInvalidSignature,
		[EnumMember]
		IssuanceLicenseHasInvalidTimeRange,
		[EnumMember]
		UntrustedIssuanceLicense,
		[EnumMember]
		UnknownRightsTemplate,
		[EnumMember]
		UnexpectedIssuanceLicense,
		[EnumMember]
		NoRightsForRequestedPrincipal
	}
}
