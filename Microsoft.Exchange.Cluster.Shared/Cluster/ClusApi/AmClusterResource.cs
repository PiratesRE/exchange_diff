using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	internal class AmClusterResource : DisposeTrackableBase, IAmClusterResource, IDisposable
	{
		internal AmClusterResource(string resourceName, IAmCluster owningCluster, AmClusterResourceHandle resourceHandle)
		{
			this.m_name = resourceName;
			this.OwningCluster = owningCluster;
			this.Handle = resourceHandle;
		}

		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.ClusterEventsTracer;
			}
		}

		public string Name
		{
			get
			{
				if (this.m_name == null)
				{
					this.m_name = this.GetCommonProperty<string>("Name");
				}
				return this.m_name;
			}
		}

		internal IAmCluster OwningCluster { get; private set; }

		public AmClusterResourceHandle Handle
		{
			get
			{
				return this.m_handle;
			}
			private set
			{
				this.m_handle = value;
			}
		}

		public MyType GetCommonProperty<MyType>(string key)
		{
			MyType result = default(MyType);
			try
			{
				using (AmClusterRawData resourceControlData = this.GetResourceControlData(AmClusterResourceControlCode.CLUSCTL_RESOURCE_GET_COMMON_PROPERTIES))
				{
					AmClusterPropList amClusterPropList = new AmClusterPropList(resourceControlData.Buffer, resourceControlData.Size);
					result = amClusterPropList.Read<MyType>(key);
				}
			}
			catch (ClusterApiException arg)
			{
				AmClusterResource.Tracer.TraceDebug<string, string, ClusterApiException>((long)this.GetHashCode(), "GetCommonProperty( {0} ) on '{1}' encountered an exception: {2}", key, this.Name, arg);
			}
			return result;
		}

		public MyType GetCommonROProperty<MyType>(string key)
		{
			MyType result = default(MyType);
			try
			{
				using (AmClusterRawData resourceControlData = this.GetResourceControlData(AmClusterResourceControlCode.CLUSCTL_RESOURCE_GET_RO_COMMON_PROPERTIES))
				{
					AmClusterPropList amClusterPropList = new AmClusterPropList(resourceControlData.Buffer, resourceControlData.Size);
					result = amClusterPropList.Read<MyType>(key);
				}
			}
			catch (ClusterApiException arg)
			{
				AmClusterResource.Tracer.TraceDebug<string, string, ClusterApiException>((long)this.GetHashCode(), "GetCommonROProperty( {0} ) on '{1}' encountered an exception: {2}", key, this.Name, arg);
			}
			return result;
		}

		public MyType GetPrivateProperty<MyType>(string key)
		{
			MyType result = default(MyType);
			try
			{
				using (AmClusterRawData resourceControlData = this.GetResourceControlData(AmClusterResourceControlCode.CLUSCTL_RESOURCE_GET_PRIVATE_PROPERTIES))
				{
					AmClusterPropList amClusterPropList = new AmClusterPropList(resourceControlData.Buffer, resourceControlData.Size);
					result = amClusterPropList.Read<MyType>(key);
				}
			}
			catch (ClusterApiException arg)
			{
				AmClusterResource.Tracer.TraceDebug<string, string, ClusterApiException>((long)this.GetHashCode(), "GetPrivateProperty( {0} ) on '{1}' encountered an exception: {2}", key, this.Name, arg);
			}
			return result;
		}

		public void SetPrivateProperty<MyType>(string key, MyType value)
		{
			int num = 0;
			if (typeof(string) == typeof(MyType))
			{
				string value2 = (string)((object)value);
				using (AmClusterPropListDisposable amClusterPropListDisposable = AmClusPropListMaker.CreatePropListString(key, value2, out num))
				{
					this.SetResourceControlData(AmClusterResourceControlCode.CLUSCTL_RESOURCE_SET_PRIVATE_PROPERTIES, amClusterPropListDisposable.RawBuffer, amClusterPropListDisposable.BufferSize);
					return;
				}
			}
			if (typeof(int) == typeof(MyType))
			{
				int value3 = (int)((object)value);
				using (AmClusterPropListDisposable amClusterPropListDisposable2 = AmClusPropListMaker.CreatePropListInt(key, value3, out num))
				{
					this.SetResourceControlData(AmClusterResourceControlCode.CLUSCTL_RESOURCE_SET_PRIVATE_PROPERTIES, amClusterPropListDisposable2.RawBuffer, amClusterPropListDisposable2.BufferSize);
					return;
				}
			}
			if (typeof(string[]) == typeof(MyType))
			{
				string[] value4 = (string[])((object)value);
				using (AmClusterPropListDisposable amClusterPropListDisposable3 = AmClusPropListMaker.CreatePropListMultiString(key, value4, out num))
				{
					this.SetResourceControlData(AmClusterResourceControlCode.CLUSCTL_RESOURCE_SET_PRIVATE_PROPERTIES, amClusterPropListDisposable3.RawBuffer, amClusterPropListDisposable3.BufferSize);
				}
			}
		}

		public void SetPrivatePropertyList(AmClusterPropList propList)
		{
			this.SetResourceControlData(AmClusterResourceControlCode.CLUSCTL_RESOURCE_SET_PRIVATE_PROPERTIES, propList.RawBuffer, propList.BufferSize);
		}

		public AmResourceState GetState()
		{
			uint num = 0U;
			uint num2 = 0U;
			AmResourceState clusterResourceState = ClusapiMethods.GetClusterResourceState(this.Handle, null, ref num, null, ref num2);
			if (clusterResourceState == AmResourceState.Unknown)
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				throw AmExceptionHelper.ConstructClusterApiException(lastWin32Error, "GetClusterResourceState({0})", new object[]
				{
					this.Name
				});
			}
			return clusterResourceState;
		}

		public string GetTypeName()
		{
			return this.GetCommonProperty<string>("Type");
		}

		public bool IsIpv4()
		{
			return SharedHelper.StringIEquals(this.GetTypeName(), "IP Address");
		}

		public bool IsIpv6()
		{
			return SharedHelper.StringIEquals(this.GetTypeName(), "IPv6 Address");
		}

		public override string ToString()
		{
			return string.Format("resource:{0}", this.Name);
		}

		public uint OnlineResource()
		{
			uint num = ClusapiMethods.OnlineClusterResource(this.Handle);
			if (num != 0U)
			{
				ExTraceGlobals.ClusterTracer.TraceDebug<string, uint>((long)this.GetHashCode(), "OnlineClusterResource( '{0}' ) failed with 0x{1:x}", this.Name, num);
			}
			return num;
		}

		public uint OfflineResource()
		{
			uint num = ClusapiMethods.OfflineClusterResource(this.Handle);
			if (num != 0U)
			{
				ExTraceGlobals.ClusterTracer.TraceDebug<string, uint>((long)this.GetHashCode(), "OfflineClusterResource( '{0}' ) failed with 0x{1:x}", this.Name, num);
			}
			return num;
		}

		public void DeleteResource()
		{
			string name = this.Name;
			uint num = ClusapiMethods.DeleteClusterResource(this.Handle);
			if (num != 0U)
			{
				throw AmExceptionHelper.ConstructClusterApiException((int)num, "DeleteClusterResource()", new object[0]);
			}
		}

		public void RemoveDependency(AmClusterResource childDependency)
		{
			string name = this.Name;
			uint num = ClusapiMethods.RemoveClusterResourceDependency(this.Handle, childDependency.Handle);
			if (num == 0U)
			{
				return;
			}
			int num2 = (int)num;
			if (num == 5002U)
			{
				AmClusterResource.Tracer.TraceDebug<string, string, int>((long)this.GetHashCode(), "RemoveClusterResourceDependency( parent={0}, child={1}) returned a non-fatal error 0x{2:x}", this.Name, childDependency.Name, num2);
				return;
			}
			throw AmExceptionHelper.ConstructClusterApiException(num2, "RemoveClusterResourceDependency( parent={0}, child={1})", new object[]
			{
				this.Name,
				childDependency.Name
			});
		}

		public uint SetDependencyExpression(string expression)
		{
			uint num = ClusapiMethods.SetClusterResourceDependencyExpression(this.Handle, expression);
			if (num != 0U)
			{
				throw AmExceptionHelper.ConstructClusterApiException((int)num, "SetClusterResourceDependencyExpression({0})", new object[]
				{
					expression
				});
			}
			return num;
		}

		internal IEnumerable<string> EnumeratePossibleOwnerNames()
		{
			return AmClusterResource.EnumerateObjects(this.Handle, AmClusterResourceEnum.CLUSTER_RESOURCE_ENUM_NODES);
		}

		internal IEnumerable<IAmClusterNode> EnumeratePossibleOwnerNodes()
		{
			return AmCluster.EvaluateAllElements<IAmClusterNode>(this.LazyEnumeratePossibleOwnerNodes());
		}

		internal IEnumerable<string> EnumerateDependentNames()
		{
			return AmClusterResource.EnumerateObjects(this.Handle, AmClusterResourceEnum.CLUSTER_RESOURCE_ENUM_DEPENDS);
		}

		internal IEnumerable<AmClusterResource> EnumerateDependents()
		{
			return AmCluster.EvaluateAllElements<AmClusterResource>(this.LazyEnumerateDependents());
		}

		public void SetAllPossibleOwnerNodes()
		{
			IEnumerable<IAmClusterNode> enumerable = null;
			ClusterApiException ex = null;
			try
			{
				enumerable = this.OwningCluster.EnumerateNodes();
				foreach (IAmClusterNode amClusterNode in enumerable)
				{
					uint num = ClusapiMethods.AddClusterResourceNode(this.Handle, amClusterNode.Handle);
					if (num != 0U && num != 5010U)
					{
						ex = AmExceptionHelper.ConstructClusterApiException((int)num, "AddClusterResourceNode( resource={0}, node={1} )", new object[]
						{
							this.Name,
							amClusterNode.Name
						});
					}
				}
			}
			finally
			{
				if (enumerable != null)
				{
					foreach (IAmClusterNode amClusterNode2 in enumerable)
					{
						amClusterNode2.Dispose();
					}
				}
				if (ex != null)
				{
					throw ex;
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<AmClusterResource>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			lock (this)
			{
				if (disposing && this.Handle != null && !this.Handle.IsInvalid)
				{
					this.Handle.Dispose();
					this.Handle = null;
				}
			}
		}

		private static IEnumerable<string> EnumerateObjects(AmClusterResourceHandle handle, AmClusterResourceEnum objectType)
		{
			new List<string>(16);
			using (AmClusResourceEnumHandle enumHandle = ClusapiMethods.ClusterResourceOpenEnum(handle, objectType))
			{
				if (enumHandle.IsInvalid)
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					throw AmExceptionHelper.ConstructClusterApiException(lastWin32Error, "ClusterOpenResourceEnum(objecttype={0})", new object[]
					{
						objectType
					});
				}
				int entryIndex = 0;
				int objectNameLen = 256;
				StringBuilder objectNameBuffer = new StringBuilder(objectNameLen);
				int errorCode;
				for (;;)
				{
					objectNameLen = objectNameBuffer.Capacity;
					AmClusterResourceEnum objectTypeRetrived;
					errorCode = ClusapiMethods.ClusterResourceEnum(enumHandle, entryIndex, out objectTypeRetrived, objectNameBuffer, ref objectNameLen);
					if (errorCode == 259)
					{
						goto IL_171;
					}
					if (errorCode == 234)
					{
						objectNameBuffer.EnsureCapacity(objectNameLen);
					}
					else
					{
						if (errorCode != 0)
						{
							break;
						}
						if (objectTypeRetrived == objectType)
						{
							yield return objectNameBuffer.ToString();
						}
						entryIndex++;
					}
				}
				throw AmExceptionHelper.ConstructClusterApiException(errorCode, "ClusterResourceEnum()", new object[0]);
				IL_171:;
			}
			yield break;
		}

		private AmClusterRawData GetResourceControlData(AmClusterResourceControlCode code)
		{
			uint num = 1024U;
			AmClusterRawData amClusterRawData = AmClusterRawData.Allocate(num);
			int num2 = ClusapiMethods.ClusterResourceControl(this.Handle, IntPtr.Zero, code, IntPtr.Zero, 0U, amClusterRawData.Buffer, num, out num);
			if (num2 == 234)
			{
				amClusterRawData.Dispose();
				amClusterRawData = AmClusterRawData.Allocate(num);
				num2 = ClusapiMethods.ClusterResourceControl(this.Handle, IntPtr.Zero, code, IntPtr.Zero, 0U, amClusterRawData.Buffer, num, out num);
			}
			if (num2 != 0)
			{
				amClusterRawData.Dispose();
				throw AmExceptionHelper.ConstructClusterApiException(num2, "ClusterResourceControl(controlcode={0})", new object[]
				{
					code
				});
			}
			return amClusterRawData;
		}

		private void SetResourceControlData(AmClusterResourceControlCode code, IntPtr buffer, uint bufferSize)
		{
			uint num = 0U;
			int num2 = ClusapiMethods.ClusterResourceControl(this.Handle, IntPtr.Zero, code, buffer, bufferSize, IntPtr.Zero, 0U, out num);
			if (num2 != 0)
			{
				ClusterApiException ex = AmExceptionHelper.ConstructClusterApiException(num2, "ClusterResourceControl(controlcode={0})", new object[]
				{
					code
				});
				AmClusterResource.Tracer.TraceDebug((long)this.GetHashCode(), ex.Message);
				if ((long)num2 != 5024L)
				{
					throw ex;
				}
			}
		}

		private IEnumerable<IAmClusterNode> LazyEnumeratePossibleOwnerNodes()
		{
			return from nodeName in this.EnumeratePossibleOwnerNames()
			select this.OwningCluster.OpenNode(new AmServerName(nodeName));
		}

		private IEnumerable<AmClusterResource> LazyEnumerateDependents()
		{
			return from resourceName in this.EnumerateDependentNames()
			select this.OwningCluster.OpenResource(resourceName);
		}

		public const string CLUS_RESTYPE_NAME_IPADDR = "IP Address";

		public const string CLUS_RESTYPE_NAME_NETNAME = "Network Name";

		public const string CLUS_RESTYPE_NAME_FSWITNESS = "File Share Witness";

		public const string CLUS_RESTYPE_NAME_IPV6_NATIVE = "IPv6 Address";

		public const string CLUSREG_NAME_RES_NAME = "Name";

		public const string CLUSREG_NAME_RES_TYPE = "Type";

		public const string CLUSREG_NAME_IPADDR_ENABLE_DHCP = "EnableDhcp";

		public const string CLUSREG_NAME_IPADDR_SUBNET_MASK = "SubnetMask";

		public const string CLUSREG_NAME_IPADDR_ADDRESS = "Address";

		public const string CLUSREG_NAME_IPADDR_NETWORK = "Network";

		public const string CLUSREG_NAME_NETNAME_CREATING_DC = "CreatingDC";

		public const string CLUSREG_NAME_NETNAME_REQUIRE_DNS = "RequireDNS";

		public const string CLUSREG_NAME_NETNAME_STATUS_DNS = "StatusDNS";

		public const string CLUSREG_NAME_NETNAME_HOST_TTL = "HostRecordTTL";

		public const string CLUSREG_NAME_NETNAME_REQUIRE_KERBEROS = "RequireKerberos";

		public const string CLUSREG_NAME_NETNAME_NAME = "Name";

		public const string CLUSREG_NAME_FSW_SHAREPATH = "SharePath";

		public const string CLUSREG_NAME_MNS_FILESHARE = "MNSFileShare";

		public const string CLUSREG_NAME_NET_NAME = "Name";

		public const string CLUSREG_NAME_NET_IPV6_ADDRESSES = "IPv6Addresses";

		public const string CLUSREG_NAME_NET_IPV6_PREFIXLENGTHS = "IPv6PrefixLengths";

		public const string CLUSREG_NAME_NET_IPV4_ADDRESSES = "IPv4Addresses";

		public const string CLUSREG_NAME_NET_IPV4_PREFIXLENGTHS = "IPv4PrefixLengths";

		public const string CLUSREG_NAME_NET_ADDRESS = "Address";

		public const string CLUSREG_NAME_NET_ADDRESS_MASK = "AddressMask";

		public const string CLUSREG_NAME_NET_DESC = "Description";

		public const string CLUSREG_NAME_NET_ROLE = "Role";

		private string m_name;

		private AmClusterResourceHandle m_handle;
	}
}
