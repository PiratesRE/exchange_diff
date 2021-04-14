using System;
using System.Runtime.Serialization;
using System.ServiceModel.Description;

namespace Microsoft.Exchange.DxStore.Common
{
	[DataContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
	[Serializable]
	public class InstanceManagerConfig
	{
		public IServerNameResolver NameResolver { get; set; }

		[DataMember]
		public string Self { get; set; }

		[DataMember]
		public bool IsZeroboxMode { get; set; }

		[DataMember]
		public string ComponentName { get; set; }

		[DataMember]
		public string NetworkAddress { get; set; }

		[DataMember]
		public string BaseStorageDir { get; set; }

		[DataMember]
		public TimeSpan InstanceMonitorInterval { get; set; }

		[DataMember]
		public int EndpointPortNumber { get; set; }

		[DataMember]
		public string EndpointProtocolName { get; set; }

		[DataMember]
		public WcfTimeout DefaultTimeout { get; set; }

		[DataMember]
		public TimeSpan ManagerStopTimeout { get; set; }

		[DataMember]
		public CommonSettings Settings { get; set; }

		public string Identity
		{
			get
			{
				return string.Format("Manager/{0}/{1}", this.ComponentName, this.Self);
			}
		}

		public virtual ServiceEndpoint GetEndpoint(string target, bool isServerBinding, WcfTimeout timeout = null)
		{
			return EndpointBuilder.GetStoreManagerEndpoint(this, target, isServerBinding, timeout);
		}

		public static class PropertyNames
		{
			public const string BaseStorageDir = "BaseStorageDir";

			public const string DefaultTimeout = "DefaultTimeout";

			public const string EndpointPortNumber = "EndpointPortNumber";

			public const string EndpointProtocolName = "EndpointProtocolName";

			public const string InstanceMonitorIntervalInMSec = "InstanceMonitorIntervalInMSec";

			public const string NetworkAddress = "NetworkAddress";

			public const string ManagerStopTimeoutInMSec = "ManagerStopTimeoutInMSec";
		}

		public static class ContainerNames
		{
			public const string Settings = "Settings";

			public const string Groups = "Groups";
		}
	}
}
