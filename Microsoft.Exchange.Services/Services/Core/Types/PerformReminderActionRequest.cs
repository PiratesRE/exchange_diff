using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "PerformReminderActionType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class PerformReminderActionRequest : BaseRequest
	{
		[DataMember]
		[XmlArray("ReminderItemActions", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages", Order = 1)]
		[XmlArrayItem("ReminderItemAction", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(ReminderItemActionType))]
		public ReminderItemActionType[] ReminderItemActions { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new PerformReminderAction(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return null;
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return null;
		}
	}
}
