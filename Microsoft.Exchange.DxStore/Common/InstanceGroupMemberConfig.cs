using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.DxStore.Common
{
	[DataContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
	[Serializable]
	public class InstanceGroupMemberConfig
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string NetworkAddress { get; set; }

		[DataMember]
		public bool IsWitness { get; set; }

		[DataMember]
		public bool IsManagedExternally { get; set; }

		public static class PropertyNames
		{
			public const string IsManagedExternally = "IsManagedExternally";

			public const string IsWitness = "IsWitness";

			public const string NetworkAddress = "NetworkAddress";
		}
	}
}
