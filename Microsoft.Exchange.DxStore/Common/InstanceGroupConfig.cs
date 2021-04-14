using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel.Description;

namespace Microsoft.Exchange.DxStore.Common
{
	[DataContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
	[Serializable]
	public class InstanceGroupConfig
	{
		public IServerNameResolver NameResolver { get; set; }

		[DataMember]
		public bool IsZeroboxMode { get; set; }

		[DataMember]
		public string Self { get; set; }

		[DataMember]
		public string ComponentName { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public bool IsExistInConfigProvider { get; set; }

		[DataMember]
		public bool IsAutomaticActionsAllowed { get; set; }

		[DataMember]
		public bool IsRestartRequested { get; set; }

		[DataMember]
		public bool IsConfigurationReady { get; set; }

		[DataMember]
		public bool IsConfigurationManagedExternally { get; set; }

		public DateTimeOffset ConfigInProgressExpiryTime { get; set; }

		[DataMember]
		public InstanceGroupMemberConfig[] Members { get; set; }

		[DataMember]
		public bool IsDefaultGroup { get; set; }

		[DataMember]
		public InstanceGroupSettings Settings { get; set; }

		[DataMember]
		public string Identity { get; set; }

		public bool IsMembersContainSelf
		{
			get
			{
				return this.IsMember(this.Self, false);
			}
		}

		public bool IsOneNodeGroup
		{
			get
			{
				return this.Members.Length == 1;
			}
		}

		public bool IsMember(string memberName, bool checkExternallyManaged = false)
		{
			if (!checkExternallyManaged)
			{
				return this.Members.Any((InstanceGroupMemberConfig m) => Utils.IsEqual(m.Name, memberName, StringComparison.OrdinalIgnoreCase));
			}
			return this.Members.Any((InstanceGroupMemberConfig m) => Utils.IsEqual(m.Name, memberName, StringComparison.OrdinalIgnoreCase) && !m.IsManagedExternally);
		}

		public virtual ServiceEndpoint GetStoreAccessEndpoint(string target, bool isUseDefaultGroup, bool isServerBinding, WcfTimeout timeout = null)
		{
			return EndpointBuilder.GetStoreAccessEndpoint(this, target, isUseDefaultGroup, isServerBinding, timeout);
		}

		public virtual ServiceEndpoint GetStoreInstanceEndpoint(string target, bool isUseDefaultGroup, bool isServerBinding, WcfTimeout timeout = null)
		{
			return EndpointBuilder.GetStoreInstanceEndpoint(this, target, isUseDefaultGroup, isServerBinding, timeout);
		}

		public static class PropertyNames
		{
			public const string IsAutomaticActionsAllowed = "IsAutomaticActionsAllowed";

			public const string IsRestartRequested = "IsRestartRequested";

			public const string IsConfigurationReady = "IsConfigurationReady";

			public const string IsConfigurationManagedExternally = "IsConfigurationManagedExternally";

			public const string ConfigInProgressExpiryTime = "ConfigInProgressExpiryTime";
		}

		public static class ContainerNames
		{
			public const string Members = "Members";

			public const string Settings = "Settings";
		}
	}
}
