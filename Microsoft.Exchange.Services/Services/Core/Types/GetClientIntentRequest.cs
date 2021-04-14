using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("GetClientIntentType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public sealed class GetClientIntentRequest : BaseRequest
	{
		[XmlElement]
		[DataMember(Name = "GlobalObjectId", IsRequired = true)]
		public string GlobalObjectId { get; set; }

		[XmlArrayItem("DeleteFromFolderStateDefinition", typeof(DeleteFromFolderStateDefinition), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArray("StateDefinition")]
		[XmlArrayItem("LocationBasedStateDefinition", typeof(LocationBasedStateDefinition), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[DataMember(Name = "StateDefinition", IsRequired = true)]
		[XmlArrayItem("DeletedOccurrenceStateDefinition", typeof(DeletedOccurrenceStateDefinition), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public BaseCalendarItemStateDefinition[] StateDefinition { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new GetClientIntent(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return null;
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return base.GetResourceKeysFromProxyInfo(false, callContext);
		}
	}
}
