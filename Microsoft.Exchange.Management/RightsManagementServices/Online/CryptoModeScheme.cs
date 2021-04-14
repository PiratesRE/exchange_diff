using System;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace Microsoft.RightsManagementServices.Online
{
	[DataContract(Name = "CryptoModeScheme", Namespace = "http://microsoft.com/RightsManagementServiceOnline/2011/04")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public enum CryptoModeScheme
	{
		[EnumMember]
		Mode1 = 1,
		[EnumMember]
		Mode2
	}
}
