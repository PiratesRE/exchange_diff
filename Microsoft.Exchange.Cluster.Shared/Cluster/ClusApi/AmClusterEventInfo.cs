using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	internal class AmClusterEventInfo
	{
		internal AmClusterEventInfo(string changedObjectName, ClusterNotifyFlags eventCode, IntPtr context)
		{
			this.ObjectName = changedObjectName;
			this.EventCode = eventCode;
			this.Context = context;
		}

		internal string ObjectName { get; private set; }

		internal ClusterNotifyFlags EventCode { get; private set; }

		internal IntPtr Context { get; private set; }

		internal bool IsNotifyHandleClosed
		{
			get
			{
				return this.IsEventTriggered(~(ClusterNotifyFlags.CLUSTER_CHANGE_NODE_STATE | ClusterNotifyFlags.CLUSTER_CHANGE_NODE_DELETED | ClusterNotifyFlags.CLUSTER_CHANGE_NODE_ADDED | ClusterNotifyFlags.CLUSTER_CHANGE_NODE_PROPERTY | ClusterNotifyFlags.CLUSTER_CHANGE_REGISTRY_NAME | ClusterNotifyFlags.CLUSTER_CHANGE_REGISTRY_ATTRIBUTES | ClusterNotifyFlags.CLUSTER_CHANGE_REGISTRY_VALUE | ClusterNotifyFlags.CLUSTER_CHANGE_REGISTRY_SUBTREE | ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_STATE | ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_DELETED | ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_ADDED | ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_PROPERTY | ClusterNotifyFlags.CLUSTER_CHANGE_GROUP_STATE | ClusterNotifyFlags.CLUSTER_CHANGE_GROUP_DELETED | ClusterNotifyFlags.CLUSTER_CHANGE_GROUP_ADDED | ClusterNotifyFlags.CLUSTER_CHANGE_GROUP_PROPERTY | ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_TYPE_DELETED | ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_TYPE_ADDED | ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_TYPE_PROPERTY | ClusterNotifyFlags.CLUSTER_CHANGE_CLUSTER_RECONNECT | ClusterNotifyFlags.CLUSTER_CHANGE_NETWORK_STATE | ClusterNotifyFlags.CLUSTER_CHANGE_NETWORK_DELETED | ClusterNotifyFlags.CLUSTER_CHANGE_NETWORK_ADDED | ClusterNotifyFlags.CLUSTER_CHANGE_NETWORK_PROPERTY | ClusterNotifyFlags.CLUSTER_CHANGE_NETINTERFACE_STATE | ClusterNotifyFlags.CLUSTER_CHANGE_NETINTERFACE_DELETED | ClusterNotifyFlags.CLUSTER_CHANGE_NETINTERFACE_ADDED | ClusterNotifyFlags.CLUSTER_CHANGE_NETINTERFACE_PROPERTY | ClusterNotifyFlags.CLUSTER_CHANGE_QUORUM_STATE | ClusterNotifyFlags.CLUSTER_CHANGE_CLUSTER_STATE | ClusterNotifyFlags.CLUSTER_CHANGE_CLUSTER_PROPERTY));
			}
		}

		internal bool IsGroupStateChanged
		{
			get
			{
				return this.IsEventTriggered(ClusterNotifyFlags.CLUSTER_CHANGE_GROUP_STATE);
			}
		}

		internal bool IsClusterStateChanged
		{
			get
			{
				return this.IsEventTriggered(ClusterNotifyFlags.CLUSTER_CHANGE_CLUSTER_STATE);
			}
		}

		internal bool IsNodeStateChanged
		{
			get
			{
				return this.IsEventTriggered(ClusterNotifyFlags.CLUSTER_CHANGE_NODE_STATE);
			}
		}

		internal bool IsNodeAdded
		{
			get
			{
				return this.IsEventTriggered(ClusterNotifyFlags.CLUSTER_CHANGE_NODE_ADDED);
			}
		}

		internal bool IsNodeRemoved
		{
			get
			{
				return this.IsEventTriggered(ClusterNotifyFlags.CLUSTER_CHANGE_NODE_DELETED);
			}
		}

		internal bool IsRegistryChanged
		{
			get
			{
				return this.IsEventTriggered(ClusterNotifyFlags.CLUSTER_CHANGE_REGISTRY_VALUE);
			}
		}

		internal bool IsNetInterfaceStateChanged
		{
			get
			{
				return this.IsEventTriggered(ClusterNotifyFlags.CLUSTER_CHANGE_NETINTERFACE_STATE);
			}
		}

		public override string ToString()
		{
			string arg;
			if (!AmClusterEventInfo.EventNameMap.TryGetValue(this.EventCode, out arg))
			{
				arg = "0x" + this.EventCode.ToString("X");
			}
			return string.Format("({0}, {1}, {2})", arg, this.ObjectName, this.Context);
		}

		internal static Dictionary<ClusterNotifyFlags, string> InitializeEventNameLookupTable()
		{
			return new Dictionary<ClusterNotifyFlags, string>(32)
			{
				{
					ClusterNotifyFlags.CLUSTER_CHANGE_NODE_STATE,
					"NODE_STATE"
				},
				{
					ClusterNotifyFlags.CLUSTER_CHANGE_NODE_DELETED,
					"NODE_DELETED"
				},
				{
					ClusterNotifyFlags.CLUSTER_CHANGE_NODE_ADDED,
					"NODE_ADDED"
				},
				{
					ClusterNotifyFlags.CLUSTER_CHANGE_NODE_PROPERTY,
					"NODE_PROPERTY"
				},
				{
					ClusterNotifyFlags.CLUSTER_CHANGE_REGISTRY_NAME,
					"REGISTRY_NAME"
				},
				{
					ClusterNotifyFlags.CLUSTER_CHANGE_REGISTRY_ATTRIBUTES,
					"REGISTRY_ATTRIBUTES"
				},
				{
					ClusterNotifyFlags.CLUSTER_CHANGE_REGISTRY_VALUE,
					"REGISTRY_VALUE"
				},
				{
					ClusterNotifyFlags.CLUSTER_CHANGE_REGISTRY_SUBTREE,
					"REGISTRY_SUBTREE"
				},
				{
					ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_STATE,
					"RESOURCE_STATE"
				},
				{
					ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_DELETED,
					"RESOURCE_DELETED"
				},
				{
					ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_ADDED,
					"RESOURCE_ADDED"
				},
				{
					ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_PROPERTY,
					"RESOURCE_PROPERTY"
				},
				{
					ClusterNotifyFlags.CLUSTER_CHANGE_GROUP_STATE,
					"GROUP_STATE"
				},
				{
					ClusterNotifyFlags.CLUSTER_CHANGE_GROUP_DELETED,
					"GROUP_DELETED"
				},
				{
					ClusterNotifyFlags.CLUSTER_CHANGE_GROUP_ADDED,
					"GROUP_ADDED"
				},
				{
					ClusterNotifyFlags.CLUSTER_CHANGE_GROUP_PROPERTY,
					"GROUP_PROPERTY"
				},
				{
					ClusterNotifyFlags.CLUSTER_CHANGE_CLUSTER_RECONNECT,
					"CLUSTER_RECONNECT"
				},
				{
					ClusterNotifyFlags.CLUSTER_CHANGE_NETWORK_STATE,
					"NETWORK_STATE"
				},
				{
					ClusterNotifyFlags.CLUSTER_CHANGE_NETWORK_DELETED,
					"NETWORK_DELETED"
				},
				{
					ClusterNotifyFlags.CLUSTER_CHANGE_NETWORK_ADDED,
					"NETWORK_ADDED"
				},
				{
					ClusterNotifyFlags.CLUSTER_CHANGE_NETWORK_PROPERTY,
					"NETWORK_PROPERTY"
				},
				{
					ClusterNotifyFlags.CLUSTER_CHANGE_NETINTERFACE_STATE,
					"NETINTERFACE_STATE"
				},
				{
					ClusterNotifyFlags.CLUSTER_CHANGE_NETINTERFACE_DELETED,
					"NETINTERFACE_DELETED"
				},
				{
					ClusterNotifyFlags.CLUSTER_CHANGE_NETINTERFACE_ADDED,
					"NETINTERFACE_ADDED"
				},
				{
					ClusterNotifyFlags.CLUSTER_CHANGE_NETINTERFACE_PROPERTY,
					"NETINTERFACE_PROPERTY"
				},
				{
					ClusterNotifyFlags.CLUSTER_CHANGE_QUORUM_STATE,
					"QUORUM_STATE"
				},
				{
					ClusterNotifyFlags.CLUSTER_CHANGE_CLUSTER_STATE,
					"CLUSTER_STATE"
				},
				{
					ClusterNotifyFlags.CLUSTER_CHANGE_CLUSTER_PROPERTY,
					"CLUSTER_PROPERTY"
				},
				{
					~(ClusterNotifyFlags.CLUSTER_CHANGE_NODE_STATE | ClusterNotifyFlags.CLUSTER_CHANGE_NODE_DELETED | ClusterNotifyFlags.CLUSTER_CHANGE_NODE_ADDED | ClusterNotifyFlags.CLUSTER_CHANGE_NODE_PROPERTY | ClusterNotifyFlags.CLUSTER_CHANGE_REGISTRY_NAME | ClusterNotifyFlags.CLUSTER_CHANGE_REGISTRY_ATTRIBUTES | ClusterNotifyFlags.CLUSTER_CHANGE_REGISTRY_VALUE | ClusterNotifyFlags.CLUSTER_CHANGE_REGISTRY_SUBTREE | ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_STATE | ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_DELETED | ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_ADDED | ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_PROPERTY | ClusterNotifyFlags.CLUSTER_CHANGE_GROUP_STATE | ClusterNotifyFlags.CLUSTER_CHANGE_GROUP_DELETED | ClusterNotifyFlags.CLUSTER_CHANGE_GROUP_ADDED | ClusterNotifyFlags.CLUSTER_CHANGE_GROUP_PROPERTY | ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_TYPE_DELETED | ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_TYPE_ADDED | ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_TYPE_PROPERTY | ClusterNotifyFlags.CLUSTER_CHANGE_CLUSTER_RECONNECT | ClusterNotifyFlags.CLUSTER_CHANGE_NETWORK_STATE | ClusterNotifyFlags.CLUSTER_CHANGE_NETWORK_DELETED | ClusterNotifyFlags.CLUSTER_CHANGE_NETWORK_ADDED | ClusterNotifyFlags.CLUSTER_CHANGE_NETWORK_PROPERTY | ClusterNotifyFlags.CLUSTER_CHANGE_NETINTERFACE_STATE | ClusterNotifyFlags.CLUSTER_CHANGE_NETINTERFACE_DELETED | ClusterNotifyFlags.CLUSTER_CHANGE_NETINTERFACE_ADDED | ClusterNotifyFlags.CLUSTER_CHANGE_NETINTERFACE_PROPERTY | ClusterNotifyFlags.CLUSTER_CHANGE_QUORUM_STATE | ClusterNotifyFlags.CLUSTER_CHANGE_CLUSTER_STATE | ClusterNotifyFlags.CLUSTER_CHANGE_CLUSTER_PROPERTY),
					"HANDLE_CLOSE"
				}
			};
		}

		internal bool IsEventTriggered(ClusterNotifyFlags eventToCheck)
		{
			return (this.EventCode & eventToCheck) == eventToCheck;
		}

		internal static Dictionary<ClusterNotifyFlags, string> EventNameMap = AmClusterEventInfo.InitializeEventNameLookupTable();
	}
}
