using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[Flags]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public enum ModernGroupRequestResultSet
	{
		General = 1,
		Members = 2,
		Owners = 4,
		ExternalResources = 8,
		GroupMailboxProperties = 16,
		ForceReload = 65536
	}
}
