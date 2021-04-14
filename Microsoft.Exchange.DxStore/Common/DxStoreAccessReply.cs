using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.DxStore.Common
{
	[DataContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
	[Serializable]
	public class DxStoreAccessReply : DxStoreReplyBase
	{
		public void Initialize(string self = null)
		{
			base.Responder = ((!string.IsNullOrEmpty(self)) ? self : Environment.MachineName);
			base.ProcessInfo = Utils.CurrentProcessInfo;
			base.TimeReceived = DateTimeOffset.Now;
			base.Version = 1;
		}

		public const int DefaultVersion = 1;

		[DataContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
		[Serializable]
		public class CheckKey : DxStoreAccessReply
		{
			[DataMember]
			public bool IsExist { get; set; }

			[DataMember]
			public bool IsCreated { get; set; }

			[DataMember]
			public ReadResult ReadResult { get; set; }

			[DataMember]
			public WriteResult WriteResult { get; set; }
		}

		[DataContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
		[Serializable]
		public class DeleteKey : DxStoreAccessReply
		{
			[DataMember]
			public bool IsExist { get; set; }

			[DataMember]
			public ReadResult ReadResult { get; set; }

			[DataMember]
			public WriteResult WriteResult { get; set; }
		}

		[DataContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
		[Serializable]
		public class SetProperty : DxStoreAccessReply
		{
			[DataMember]
			public WriteResult WriteResult { get; set; }
		}

		[DataContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
		[Serializable]
		public class DeleteProperty : DxStoreAccessReply
		{
			[DataMember]
			public bool IsExist { get; set; }

			[DataMember]
			public ReadResult ReadResult { get; set; }

			[DataMember]
			public WriteResult WriteResult { get; set; }
		}

		[DataContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
		[Serializable]
		public class ExecuteBatch : DxStoreAccessReply
		{
			[DataMember]
			public WriteResult WriteResult { get; set; }
		}

		[DataContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
		[Serializable]
		public class GetProperty : DxStoreAccessReply
		{
			[DataMember]
			public PropertyValue Value { get; set; }

			[DataMember]
			public ReadResult ReadResult { get; set; }
		}

		[DataContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
		[Serializable]
		public class GetAllProperties : DxStoreAccessReply
		{
			[DataMember]
			public Tuple<string, PropertyValue>[] Values { get; set; }

			[DataMember]
			public ReadResult ReadResult { get; set; }
		}

		[DataContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
		[Serializable]
		public class GetSubkeyNames : DxStoreAccessReply
		{
			[DataMember]
			public string[] Keys { get; set; }

			[DataMember]
			public ReadResult ReadResult { get; set; }
		}

		[DataContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
		[Serializable]
		public class GetPropertyNames : DxStoreAccessReply
		{
			[DataMember]
			public PropertyNameInfo[] Infos { get; set; }

			[DataMember]
			public ReadResult ReadResult { get; set; }
		}
	}
}
