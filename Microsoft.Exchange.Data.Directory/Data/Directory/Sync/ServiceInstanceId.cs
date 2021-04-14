using System;
using System.Text;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[Serializable]
	public sealed class ServiceInstanceId : ObjectId
	{
		public ServiceInstanceId(string serviceInstanceId)
		{
			if (ServiceInstanceId.IsValidServiceInstanceId(serviceInstanceId))
			{
				this.InstanceId = serviceInstanceId;
				return;
			}
			throw new InvalidServiceInstanceIdException(serviceInstanceId);
		}

		public string InstanceId { get; private set; }

		public override byte[] GetBytes()
		{
			if (this.InstanceId != null)
			{
				return Encoding.Unicode.GetBytes(this.InstanceId);
			}
			return null;
		}

		public override string ToString()
		{
			return this.InstanceId;
		}

		internal static string GetServiceInstanceId(string domainForestName)
		{
			string[] array = domainForestName.Split(new char[]
			{
				'.'
			});
			if (array.Length >= 2)
			{
				return string.Format("Exchange/{0}-{1}", array[0], 1);
			}
			return domainForestName;
		}

		internal static bool IsValidServiceInstanceId(string serviceInstanceId)
		{
			return !string.IsNullOrEmpty(serviceInstanceId) && serviceInstanceId.Length >= 12 && serviceInstanceId.Length <= 256 && serviceInstanceId.StartsWith("Exchange/", StringComparison.OrdinalIgnoreCase);
		}

		internal static string GetShortName(string serviceName)
		{
			if (serviceName.StartsWith("Exchange/", StringComparison.OrdinalIgnoreCase))
			{
				string text = serviceName.Substring("Exchange/".Length);
				if (text.LastIndexOf('-') > "Exchange/".Length)
				{
					text = text.Substring(0, text.LastIndexOf('-'));
				}
				return text;
			}
			return null;
		}

		internal static SyncServiceInstance GetSyncServiceInstance(string serviceInstanceName)
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 160, "GetSyncServiceInstance", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\Sync\\ServiceInstanceId.cs");
			topologyConfigurationSession.UseConfigNC = false;
			ADObjectId serviceInstanceObjectId = SyncServiceInstance.GetServiceInstanceObjectId(serviceInstanceName);
			return topologyConfigurationSession.Read<SyncServiceInstance>(serviceInstanceObjectId);
		}

		public bool Equals(ServiceInstanceId other)
		{
			return !object.ReferenceEquals(null, other) && (object.ReferenceEquals(this, other) || this.InstanceId.Equals(other.InstanceId, StringComparison.OrdinalIgnoreCase));
		}

		private const string ServiceInstanceFormat = "Exchange/{0}-{1}";

		private const string ValidServiceInstanceFormatStartsWith = "Exchange/";

		private const int ValidServiceInstanceMinimumLength = 12;

		private const int ValidServiceInstanceMaximumLength = 256;
	}
}
