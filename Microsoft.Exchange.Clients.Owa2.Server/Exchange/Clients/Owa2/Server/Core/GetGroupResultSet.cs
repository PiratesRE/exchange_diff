using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[Flags]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public enum GetGroupResultSet
	{
		GeneralInfo = 1,
		Members = 2
	}
}
