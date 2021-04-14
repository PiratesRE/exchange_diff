using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class LogPushNotificationDataRequest : BaseRequest
	{
		[DataMember(EmitDefaultValue = false, IsRequired = true, Order = 1)]
		public string AppId { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = true, Order = 2)]
		public string DataType { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = true, Order = 3)]
		public string[] KeyValuePairs { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new LogPushNotificationData(callContext, this);
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
