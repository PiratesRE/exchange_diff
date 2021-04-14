using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	internal class AmClusterNetInterface : DisposeTrackableBase
	{
		internal AmClusterNetInterface(string netInterfaceName, AmClusterNetInterfaceHandle netInterfaceHandle)
		{
			this.Name = netInterfaceName;
			this.Handle = netInterfaceHandle;
		}

		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.ClusterEventsTracer;
			}
		}

		internal string Name { get; private set; }

		internal AmClusterNetInterfaceHandle Handle { get; private set; }

		internal AmNetInterfaceState State
		{
			get
			{
				return this.GetState(false);
			}
		}

		public string GetNodeName()
		{
			string text = string.Empty;
			using (AmClusterRawData netInterfaceControlData = this.GetNetInterfaceControlData(AmClusterNetInterfaceControlCode.CLUSCTL_NETINTERFACE_GET_NODE))
			{
				text = netInterfaceControlData.ReadString();
				AmTrace.Debug("GetNetInterfaceIdentifier: NetInterface '{0}' is owned by '{1}'.", new object[]
				{
					this.Name,
					text
				});
			}
			return text;
		}

		public string GetAddress()
		{
			return this.GetCommonProperty<string>("Address");
		}

		public string[] GetIPv6Addresses()
		{
			return this.GetCommonProperty<string[]>("IPv6Addresses");
		}

		public string GetNetworkName()
		{
			return this.GetCommonProperty<string>("Network");
		}

		public MyType GetCommonProperty<MyType>(string key)
		{
			MyType result = default(MyType);
			try
			{
				using (AmClusterRawData netInterfaceControlData = this.GetNetInterfaceControlData(AmClusterNetInterfaceControlCode.CLUSCTL_NETINTERFACE_GET_RO_COMMON_PROPERTIES))
				{
					AmClusterPropList amClusterPropList = new AmClusterPropList(netInterfaceControlData.Buffer, netInterfaceControlData.Size);
					result = amClusterPropList.Read<MyType>(key);
				}
			}
			catch (ClusterApiException arg)
			{
				AmClusterNetInterface.Tracer.TraceDebug<string, string, ClusterApiException>((long)this.GetHashCode(), "GetCommonProperty( {0} ) on '{1}' encountered an exception: {2}", key, this.Name, arg);
			}
			return result;
		}

		internal static bool IsNicUsable(AmNetInterfaceState state)
		{
			return state == AmNetInterfaceState.Up || state == AmNetInterfaceState.Unreachable;
		}

		internal AmNetInterfaceState GetState(bool isThrowIfUnknown)
		{
			AmNetInterfaceState clusterNetInterfaceState = ClusapiMethods.GetClusterNetInterfaceState(this.Handle);
			if (clusterNetInterfaceState == AmNetInterfaceState.Unknown)
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				Exception ex = new Win32Exception(lastWin32Error);
				AmTrace.Debug("GetClusterNetInterfaceState() returned error (rc={0}, message={1})", new object[]
				{
					lastWin32Error,
					ex
				});
				if (isThrowIfUnknown)
				{
					throw AmExceptionHelper.ConstructClusterApiException(lastWin32Error, "GetClusterNetInterfaceState()", new object[0]);
				}
			}
			return clusterNetInterfaceState;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<AmClusterNetInterface>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			lock (this)
			{
				if (disposing && !this.Handle.IsInvalid)
				{
					this.Handle.Dispose();
					this.Handle = null;
				}
			}
		}

		private AmClusterRawData GetNetInterfaceControlData(AmClusterNetInterfaceControlCode code)
		{
			uint num = 1024U;
			AmClusterRawData amClusterRawData = AmClusterRawData.Allocate(num);
			int num2 = ClusapiMethods.ClusterNetInterfaceControl(this.Handle, IntPtr.Zero, code, IntPtr.Zero, 0U, amClusterRawData.Buffer, num, out num);
			if (num2 == 234)
			{
				amClusterRawData.Dispose();
				amClusterRawData = AmClusterRawData.Allocate(num);
				num2 = ClusapiMethods.ClusterNetInterfaceControl(this.Handle, IntPtr.Zero, code, IntPtr.Zero, 0U, amClusterRawData.Buffer, num, out num);
			}
			if (num2 != 0)
			{
				amClusterRawData.Dispose();
				throw AmExceptionHelper.ConstructClusterApiException(num2, "ClusterNetInterfaceControl(controlcode={0})", new object[]
				{
					code
				});
			}
			return amClusterRawData;
		}

		public const string CLUSREG_NAME_NETIFACE_NAME = "Name";

		public const string CLUSREG_NAME_NETIFACE_NODE = "Node";

		public const string CLUSREG_NAME_NETIFACE_NETWORK = "Network";

		public const string CLUSREG_NAME_NETIFACE_ADAPTER_NAME = "Adapter";

		public const string CLUSREG_NAME_NETIFACE_ADAPTER_ID = "AdapterId";

		public const string CLUSREG_NAME_NETIFACE_DHCP_ENABLED = "DhcpEnabled";

		public const string CLUSREG_NAME_NETIFACE_IPV6_ADDRESSES = "IPv6Addresses";

		public const string CLUSREG_NAME_NETIFACE_IPV4_ADDRESSES = "IPv4Addresses";

		public const string CLUSREG_NAME_NETIFACE_ADDRESS = "Address";

		public const string CLUSREG_NAME_NETIFACE_DESC = "Description";
	}
}
