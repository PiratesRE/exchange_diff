using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "GetHoldOnMailboxesType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Name = "GetHoldOnMailboxesRequest", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class GetHoldOnMailboxesRequest : BaseRequest
	{
		[DataMember(Name = "HoldId", IsRequired = true)]
		[XmlElement("HoldId")]
		public string HoldId
		{
			get
			{
				return this.holdId;
			}
			set
			{
				this.holdId = value;
			}
		}

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new GetHoldOnMailboxes(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return null;
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int currentStep)
		{
			return null;
		}

		private string holdId;
	}
}
