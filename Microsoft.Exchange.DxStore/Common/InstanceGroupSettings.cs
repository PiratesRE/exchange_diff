using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.DxStore.Common
{
	[DataContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
	[Serializable]
	public class InstanceGroupSettings : CommonSettings
	{
		[DataMember]
		public string PaxosStorageDir { get; set; }

		[DataMember]
		public string SnapshotStorageDir { get; set; }

		public static class GroupSpecificPropertyNames
		{
			public const string PaxosStorageDir = "PaxosStorageDir";

			public const string SnapshotStorageDir = "SnapshotStorageDir";
		}
	}
}
