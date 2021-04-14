using System;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace Microsoft.RightsManagementServices.Online
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "ServerErrorCode", Namespace = "http://microsoft.com/RightsManagementServiceOnline/2011/04")]
	public enum ServerErrorCode
	{
		[EnumMember]
		None,
		[EnumMember]
		InternalFailure,
		[EnumMember]
		InvalidArgument,
		[EnumMember]
		TenantIdNotFound,
		[EnumMember]
		TenantIsDeprovisioned,
		[EnumMember]
		TenantIdExist,
		[EnumMember]
		InvalidClientAuthCertificate,
		[EnumMember]
		KeySvcCreateOrUpdateFailure,
		[EnumMember]
		KeySvcCreateTenantKeyFailure,
		[EnumMember]
		ExportCertificateNotFound,
		[EnumMember]
		InvalidExportCertificate,
		[EnumMember]
		UntrustedExportCertificate,
		[EnumMember]
		OutdatedRequest
	}
}
