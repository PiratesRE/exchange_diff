using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class MessageClassificationType : ComplianceEntryType
	{
		public MessageClassificationType(string id, string name, string description) : base(id, name, description)
		{
		}
	}
}
