using System;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace www.outlook.com.highavailability.ServerLocator.v1
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "DatabaseServerInformationType", Namespace = "http://www.outlook.com/highavailability/ServerLocator/v1/")]
	public enum DatabaseServerInformationType
	{
		[EnumMember]
		TransientError = 1000,
		[EnumMember]
		PermanentError = 2000
	}
}
