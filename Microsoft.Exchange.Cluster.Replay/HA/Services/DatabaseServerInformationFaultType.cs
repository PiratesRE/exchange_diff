using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.HA.Services
{
	[DataContract(Name = "DatabaseServerInformationType", Namespace = "http://www.outlook.com/highavailability/ServerLocator/v1/")]
	public enum DatabaseServerInformationFaultType
	{
		[EnumMember(Value = "TransientError")]
		TransientError = 1000,
		[EnumMember(Value = "PermanentError")]
		PermanentError = 2000
	}
}
