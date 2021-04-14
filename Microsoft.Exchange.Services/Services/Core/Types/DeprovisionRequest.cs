using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class DeprovisionRequest : BaseRequest
	{
		[DataMember(EmitDefaultValue = false, IsRequired = true)]
		public string DeviceID { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = true)]
		public string DeviceType { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		public bool HasPAL { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		public bool SpecifyProtocol { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		public string Protocol { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new Deprovision(callContext, this);
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
