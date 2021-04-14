using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.DxStore.Common
{
	[DataContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
	[Serializable]
	public class DxStoreAccessRequest : DxStoreRequestBase
	{
		[DataMember]
		public string FullKeyName { get; set; }

		public void Initialize(string fullKeyName, bool isPrivate = false, string self = null)
		{
			string path = isPrivate ? "Private" : "Public";
			this.FullKeyName = Utils.CombinePathNullSafe(path, fullKeyName);
			base.Requester = ((!string.IsNullOrEmpty(self)) ? self : Environment.MachineName);
			base.ProcessInfo = Utils.CurrentProcessInfo;
			base.TimeRequested = DateTimeOffset.Now;
			base.Id = Guid.NewGuid();
			base.Version = 1;
		}

		public const int DefaultVersion = 1;

		[DataContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
		[Serializable]
		public class CheckKey : DxStoreAccessRequest
		{
			[DataMember]
			public string SubkeyName { get; set; }

			[DataMember]
			public bool IsCreateIfNotExist { get; set; }

			[DataMember]
			public ReadOptions ReadOptions { get; set; }

			[DataMember]
			public WriteOptions WriteOptions { get; set; }
		}

		[DataContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
		[Serializable]
		public class DeleteKey : DxStoreAccessRequest
		{
			[DataMember]
			public string SubkeyName { get; set; }

			[DataMember]
			public ReadOptions ReadOptions { get; set; }

			[DataMember]
			public WriteOptions WriteOptions { get; set; }
		}

		[DataContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
		[Serializable]
		public class SetProperty : DxStoreAccessRequest
		{
			[DataMember]
			public string Name { get; set; }

			[DataMember]
			public PropertyValue Value { get; set; }

			[DataMember]
			public WriteOptions WriteOptions { get; set; }
		}

		[DataContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
		[Serializable]
		public class DeleteProperty : DxStoreAccessRequest
		{
			[DataMember]
			public string Name { get; set; }

			[DataMember]
			public ReadOptions ReadOptions { get; set; }

			[DataMember]
			public WriteOptions WriteOptions { get; set; }
		}

		[DataContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
		[Serializable]
		public class ExecuteBatch : DxStoreAccessRequest
		{
			[DataMember]
			public DxStoreBatchCommand[] Commands { get; set; }

			[DataMember]
			public WriteOptions WriteOptions { get; set; }
		}

		[DataContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
		[Serializable]
		public class GetProperty : DxStoreAccessRequest
		{
			[DataMember]
			public string Name { get; set; }

			[DataMember]
			public ReadOptions ReadOptions { get; set; }
		}

		[DataContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
		[Serializable]
		public class GetAllProperties : DxStoreAccessRequest
		{
			[DataMember]
			public ReadOptions ReadOptions { get; set; }
		}

		[DataContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
		[Serializable]
		public class GetSubkeyNames : DxStoreAccessRequest
		{
			[DataMember]
			public ReadOptions ReadOptions { get; set; }
		}

		[DataContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
		[Serializable]
		public class GetPropertyNames : DxStoreAccessRequest
		{
			[DataMember]
			public ReadOptions ReadOptions { get; set; }
		}
	}
}
