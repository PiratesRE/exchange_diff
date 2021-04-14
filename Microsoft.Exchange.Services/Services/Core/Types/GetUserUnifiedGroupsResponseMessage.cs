using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("GetUserUnifiedGroupsResponseMessage", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class GetUserUnifiedGroupsResponseMessage : ResponseMessage
	{
		public GetUserUnifiedGroupsResponseMessage()
		{
		}

		internal GetUserUnifiedGroupsResponseMessage(ServiceResultCode code, ServiceError error, GetUserUnifiedGroupsResponseMessage response) : base(code, error)
		{
			this.groupsSets = null;
			if (response != null)
			{
				this.groupsSets = response.GroupsSets;
			}
		}

		[XmlArray(ElementName = "GroupsSets")]
		[XmlArrayItem(ElementName = "UnifiedGroupsSet", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[DataMember(EmitDefaultValue = false)]
		public UnifiedGroupsSet[] GroupsSets
		{
			get
			{
				return this.groupsSets;
			}
			set
			{
				this.groupsSets = value;
			}
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.GetUserUnifiedGroupsResponseMessage;
		}

		private UnifiedGroupsSet[] groupsSets;
	}
}
