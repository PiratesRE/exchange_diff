using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("SetImListMigrationCompletedRequestType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class SetImListMigrationCompletedRequest : BaseRequest
	{
		[DataMember(Name = "ImListMigrationCompleted", IsRequired = true, Order = 1)]
		[XmlElement(ElementName = "ImListMigrationCompleted")]
		public bool ImListMigrationCompleted { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new SetImListMigrationCompleted(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return IdConverter.GetServerInfoForCallContext(callContext);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return base.GetResourceKeysFromProxyInfo(true, callContext);
		}
	}
}
