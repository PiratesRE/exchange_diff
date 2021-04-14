using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class ProvisionRequest : BaseRequest
	{
		[DataMember(EmitDefaultValue = false, IsRequired = false, Order = 1)]
		public string DeviceFriendlyName { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = true, Order = 2)]
		public string DeviceID { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = false, Order = 3)]
		public string DeviceImei { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = false, Order = 4)]
		public string DeviceMobileOperator { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = false, Order = 5)]
		public string DeviceOS { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = false, Order = 6)]
		public string DeviceOSLanguage { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = false, Order = 7)]
		public string DevicePhoneNumber { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = true, Order = 8)]
		public string DeviceType { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = false, Order = 9)]
		public string DeviceModel { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = true, Order = 10)]
		public string ClientVersion { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = false, Order = 11)]
		public bool HasPAL { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = false, Order = 13)]
		public bool SpecifyProtocol { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = false, Order = 12)]
		public string Protocol { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new Provision(callContext, this);
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
