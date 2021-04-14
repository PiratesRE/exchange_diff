using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "SyncPeopleType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class SyncPeopleRequest : SyncPersonaContactsRequestBase
	{
		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new SyncPeople(callContext, this);
		}
	}
}
