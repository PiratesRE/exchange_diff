using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "ApplyConversationActionType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class ApplyConversationActionRequest : BaseRequest
	{
		[XmlArray(ElementName = "ConversationActions", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[DataMember]
		[XmlArrayItem("ConversationAction", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(ConversationAction))]
		public ConversationAction[] ConversationActions { get; set; }

		[XmlIgnore]
		[DataMember(Name = "ReturnMovedItemIds", IsRequired = false, Order = 2)]
		public bool ReturnMovedItemIds { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new ApplyConversationAction(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return BaseRequest.GetServerInfoForItemId(callContext, this.ConversationActions[0].ConversationId);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			if (this.ConversationActions == null || taskStep > this.ConversationActions.Length)
			{
				return null;
			}
			return base.GetResourceKeysForItemId(true, callContext, this.ConversationActions[taskStep].ConversationId);
		}

		internal const string ConversationActionsElementName = "ConversationActions";
	}
}
