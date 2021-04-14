using System;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace Microsoft.Online.BOX.Shell
{
	[DataContract(Name = "FaultCode", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.Shell")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public enum FaultCode
	{
		[EnumMember]
		InvalidCertificate,
		[EnumMember]
		InvalidHeader,
		[EnumMember]
		InvalidBrandInfo,
		[EnumMember]
		InvalidUserInfo,
		[EnumMember]
		InvalidOptions,
		[EnumMember]
		OperationFailure,
		[EnumMember]
		InvalidWorkloadId,
		[EnumMember]
		InvalidCultureName,
		[EnumMember]
		ParameterNotSupplied,
		[EnumMember]
		InvalidTenantInfo,
		[EnumMember]
		OperationDisabled
	}
}
