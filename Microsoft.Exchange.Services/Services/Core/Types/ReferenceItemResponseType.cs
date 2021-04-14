using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlInclude(typeof(SuppressReadReceiptType))]
	[KnownType(typeof(SuppressReadReceiptType))]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[XmlInclude(typeof(AcceptSharingInvitationType))]
	[KnownType(typeof(AcceptSharingInvitationType))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class ReferenceItemResponseType : ResponseObjectType
	{
	}
}
