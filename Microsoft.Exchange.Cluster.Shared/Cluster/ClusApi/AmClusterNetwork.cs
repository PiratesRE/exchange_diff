using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	internal class AmClusterNetwork : DisposeTrackableBase, IAmClusterNetwork
	{
		internal AmClusterNetwork(string networkName, IAmCluster owningCluster, AmClusterNetworkHandle networkHandle)
		{
			this.Name = networkName;
			this.OwningCluster = owningCluster;
			this.Handle = networkHandle;
		}

		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.ClusterEventsTracer;
			}
		}

		internal string Name { get; private set; }

		internal AmClusterNetworkHandle Handle { get; private set; }

		internal AmNetworkState State
		{
			get
			{
				return this.GetState(false);
			}
		}

		private IAmCluster OwningCluster { get; set; }

		public static bool IsIPInNetwork(IPAddress addrCandidate, string addrNetwork, string netMask)
		{
			char[] separator = new char[]
			{
				'.'
			};
			string[] array = addrNetwork.Split(separator);
			string[] array2 = netMask.Split(separator);
			byte[] addressBytes = addrCandidate.GetAddressBytes();
			if (array.Length != 4 || array2.Length != 4 || addressBytes.Length != 4)
			{
				return false;
			}
			for (int i = 0; i < 4; i++)
			{
				byte b = byte.Parse(array2[i]);
				if ((addressBytes[i] & b) != (byte.Parse(array[i]) & b))
				{
					ExTraceGlobals.ClusterTracer.TraceDebug(0L, "IsIPInNetwork(): IP {0} is NOT in network '{1}', netmask={2}. ( bytesCandidate[{3}]={4} & mask={5} ) != ( rgNet[{3}]={6} & mask={5}).", new object[]
					{
						addrCandidate.ToString(),
						addrNetwork,
						netMask,
						i,
						addressBytes[i],
						b,
						array[i]
					});
					return false;
				}
			}
			ExTraceGlobals.ClusterTracer.TraceDebug<string, string, string>(0L, "IsIPInNetwork(): IP {0} is in network '{1}', netmask={2}.", addrCandidate.ToString(), addrNetwork, netMask);
			return true;
		}

		public IEnumerable<string> EnumeratePureAlternateIPv6Names()
		{
			IEnumerable<string> source = this.EnumerateAlternateIPv6Names();
			ExTraceGlobals.ClusterTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Network.GetPureAlternateIPv6Names(): all ipv6 networks on '{0}' are '{1}'.", this.Name, string.Join(",", source.ToArray<string>()));
			return from ipString in source
			where ipString.EndsWith("/64")
			select ipString;
		}

		public string GetAddress()
		{
			return this.GetCommonROProperty<string>("Address");
		}

		public string GetAddressMask()
		{
			return this.GetCommonROProperty<string>("AddressMask");
		}

		public AmNetworkRole GetNativeRole()
		{
			AmNetworkRole result;
			try
			{
				result = (AmNetworkRole)this.GetCommonProperty<int>("Role");
			}
			catch (ClusterApiException arg)
			{
				ExTraceGlobals.ClusterTracer.TraceDebug<string, ClusterApiException>((long)this.GetHashCode(), "GetNativeRole({0}) encountered an exception, defaulting to ClusterNetworkRoleNone: {1}", this.Name, arg);
				result = AmNetworkRole.ClusterNetworkRoleNone;
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
					this.SetNetworkControlData(AmClusterNetworkControlCode.CLUSCTL_NETWORK_SET_PRIVATE_PROPERTIES, amClusterPropListDisposable.RawBuffer, amClusterPropListDisposable.BufferSize);
					return;
				}
			}
			if (typeof(int) == typeof(MyType))
			{
				int value3 = (int)((object)value);
				using (AmClusterPropListDisposable amClusterPropListDisposable2 = AmClusPropListMaker.CreatePropListInt(key, value3, out num))
				{
					this.SetNetworkControlData(AmClusterNetworkControlCode.CLUSCTL_NETWORK_SET_PRIVATE_PROPERTIES, amClusterPropListDisposable2.RawBuffer, amClusterPropListDisposable2.BufferSize);
					return;
				}
			}
			if (typeof(string[]) == typeof(MyType))
			{
				string[] value4 = (string[])((object)value);
				using (AmClusterPropListDisposable amClusterPropListDisposable3 = AmClusPropListMaker.CreatePropListMultiString(key, value4, out num))
				{
					this.SetNetworkControlData(AmClusterNetworkControlCode.CLUSCTL_NETWORK_SET_PRIVATE_PROPERTIES, amClusterPropListDisposable3.RawBuffer, amClusterPropListDisposable3.BufferSize);
				}
			}
		}

		public void SetNativeRole(AmNetworkRole newValue)
		{
			this.SetCommonProperty<int>("Role", (int)newValue);
		}

		public MyType GetCommonProperty<MyType>(string key)
		{
			MyType result = default(MyType);
			try
			{
				using (AmClusterRawData networkControlData = this.GetNetworkControlData(AmClusterNetworkControlCode.CLUSCTL_NETWORK_GET_COMMON_PROPERTIES))
				{
					AmClusterPropList amClusterPropList = new AmClusterPropList(networkControlData.Buffer, networkControlData.Size);
					result = amClusterPropList.Read<MyType>(key);
				}
			}
			catch (ClusterApiException arg)
			{
				AmClusterNetwork.Tracer.TraceDebug<string, string, ClusterApiException>((long)this.GetHashCode(), "GetCommonProperty( {0} ) on '{1}' encountered an exception: {2}", key, this.Name, arg);
			}
			return result;
		}

		public MyType GetCommonROProperty<MyType>(string key)
		{
			MyType result = default(MyType);
			try
			{
				using (AmClusterRawData networkControlData = this.GetNetworkControlData(AmClusterNetworkControlCode.CLUSCTL_NETWORK_GET_RO_COMMON_PROPERTIES))
				{
					AmClusterPropList amClusterPropList = new AmClusterPropList(networkControlData.Buffer, networkControlData.Size);
					result = amClusterPropList.Read<MyType>(key);
				}
			}
			catch (ClusterApiException arg)
			{
				AmClusterNetwork.Tracer.TraceDebug<string, string, ClusterApiException>((long)this.GetHashCode(), "GetCommonROProperty( {0} ) on '{1}' encountered an exception: {2}", key, this.Name, arg);
			}
			return result;
		}

		public MyType GetPrivateProperty<MyType>(string key)
		{
			MyType result = default(MyType);
			try
			{
				using (AmClusterRawData networkControlData = this.GetNetworkControlData(AmClusterNetworkControlCode.CLUSCTL_NETWORK_GET_PRIVATE_PROPERTIES))
				{
					AmClusterPropList amClusterPropList = new AmClusterPropList(networkControlData.Buffer, networkControlData.Size);
					result = amClusterPropList.Read<MyType>(key);
				}
			}
			catch (ClusterApiException arg)
			{
				AmClusterNetwork.Tracer.TraceDebug<string, string, ClusterApiException>((long)this.GetHashCode(), "GetPrivateProperty( {0} ) on '{1}' encountered an exception: {2}", key, this.Name, arg);
			}
			return result;
		}

		public void SetCommonProperty<MyType>(string key, MyType value)
		{
			int num = 0;
			if (typeof(string) == typeof(MyType))
			{
				string value2 = (string)((object)value);
				using (AmClusterPropListDisposable amClusterPropListDisposable = AmClusPropListMaker.CreatePropListString(key, value2, out num))
				{
					this.SetNetworkControlData(AmClusterNetworkControlCode.CLUSCTL_NETWORK_SET_COMMON_PROPERTIES, amClusterPropListDisposable.RawBuffer, amClusterPropListDisposable.BufferSize);
					return;
				}
			}
			if (typeof(int) == typeof(MyType))
			{
				int value3 = (int)((object)value);
				using (AmClusterPropListDisposable amClusterPropListDisposable2 = AmClusPropListMaker.CreatePropListInt(key, value3, out num))
				{
					this.SetNetworkControlData(AmClusterNetworkControlCode.CLUSCTL_NETWORK_SET_COMMON_PROPERTIES, amClusterPropListDisposable2.RawBuffer, amClusterPropListDisposable2.BufferSize);
					return;
				}
			}
			if (typeof(string[]) == typeof(MyType))
			{
				string[] value4 = (string[])((object)value);
				using (AmClusterPropListDisposable amClusterPropListDisposable3 = AmClusPropListMaker.CreatePropListMultiString(key, value4, out num))
				{
					this.SetNetworkControlData(AmClusterNetworkControlCode.CLUSCTL_NETWORK_SET_COMMON_PROPERTIES, amClusterPropListDisposable3.RawBuffer, amClusterPropListDisposable3.BufferSize);
				}
			}
		}

		public IEnumerable<IAmClusterNode> GetVisibleNodes(IEnumerable<IAmClusterNode> nodesToCheck)
		{
			IEnumerable<IAmClusterNode> nodes = null;
			if (nodesToCheck != null)
			{
				nodes = nodesToCheck;
			}
			else
			{
				nodes = this.OwningCluster.EnumerateNodes();
			}
			foreach (IAmClusterNode node in nodes)
			{
				if (node.IsNetworkVisible(this.Name))
				{
					yield return node;
				}
			}
			yield break;
		}

		public bool SupportsIPv4Dhcp()
		{
			IEnumerable<string> source = this.EnumerateAlternateIPv4Names();
			return source.Count<string>() > 0;
		}

		public bool SupportsIPv6AutoConfiguration()
		{
			IEnumerable<string> source = this.EnumerateAlternateIPv6Names();
			return source.Count<string>() > 0;
		}

		public bool IsIPInNetwork(IPAddress ip)
		{
			return AmClusterNetwork.IsIPInNetwork(ip, this.GetAddress(), this.GetAddressMask());
		}

		internal AmNetworkState GetState(bool isThrowIfUnknown)
		{
			AmNetworkState clusterNetworkState = ClusapiMethods.GetClusterNetworkState(this.Handle);
			if (clusterNetworkState == AmNetworkState.Unavailable)
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				Exception ex = new Win32Exception(lastWin32Error);
				AmTrace.Debug("GetClusterNetworkState() returned error (rc={0}, message={1})", new object[]
				{
					lastWin32Error,
					ex
				});
				if (isThrowIfUnknown)
				{
					throw AmExceptionHelper.ConstructClusterApiException(lastWin32Error, "GetClusterNetworkState()", new object[0]);
				}
			}
			return clusterNetworkState;
		}

		internal IEnumerable<string> EnumerateAlternateIPv4Names()
		{
			string[] commonROProperty = this.GetCommonROProperty<string[]>("IPv4Addresses");
			string[] commonROProperty2 = this.GetCommonROProperty<string[]>("IPv4PrefixLengths");
			return this.FormSubnetIds(commonROProperty, commonROProperty2, "EnumerateAlternateIPv4Names");
		}

		internal IEnumerable<string> EnumerateAlternateIPv6Names()
		{
			string[] commonROProperty = this.GetCommonROProperty<string[]>("IPv6Addresses");
			string[] commonROProperty2 = this.GetCommonROProperty<string[]>("IPv6PrefixLengths");
			return this.FormSubnetIds(commonROProperty, commonROProperty2, "EnumerateAlternateIPv6Names");
		}

		private IEnumerable<string> FormSubnetIds(string[] addressParts, string[] prefixParts, string callerName)
		{
			List<string> list = new List<string>(16);
			if (addressParts == null || prefixParts == null)
			{
				string text = string.Format("{0} retrieved a null RO property.", callerName);
				AmClusterNetwork.Tracer.TraceError((long)this.GetHashCode(), text);
				ReplayCrimsonEvents.NetworkDiscoveryInconsistent.LogPeriodic<string>(callerName, SharedDiag.DefaultEventSuppressionInterval, text);
				return list;
			}
			if (addressParts.Length != prefixParts.Length)
			{
				string text2 = string.Format("{0} found addressParts.Length = {1} and prefixParts.Length = {2}.", callerName, addressParts.Length, prefixParts.Length);
				AmClusterNetwork.Tracer.TraceError((long)this.GetHashCode(), text2);
				ReplayCrimsonEvents.NetworkDiscoveryInconsistent.LogPeriodic<string>(callerName, SharedDiag.DefaultEventSuppressionInterval, text2);
				return new List<string>(0);
			}
			int num = 0;
			while (num < addressParts.Length && num < prefixParts.Length)
			{
				if (!string.IsNullOrEmpty(addressParts[num]) && !string.IsNullOrEmpty(prefixParts[num]))
				{
					list.Add(addressParts[num] + "/" + prefixParts[num]);
				}
				num++;
			}
			return list;
		}

		internal IEnumerable<string> EnumerateNetworkInterfaceNames()
		{
			return AmClusterNetwork.EnumerateObjects(this.Handle, AmClusterNetworkEnum.CLUSTER_NETWORK_ENUM_NETINTERFACES);
		}

		internal IEnumerable<AmClusterNetInterface> EnumerateNetworkInterfaces()
		{
			return AmCluster.EvaluateAllElements<AmClusterNetInterface>(this.LazyEnumerateNetworkInterfaces());
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<AmClusterNetwork>(this);
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

		private static IEnumerable<string> EnumerateObjects(AmClusterNetworkHandle handle, AmClusterNetworkEnum objectType)
		{
			new List<string>(16);
			using (AmClusNetworkEnumHandle enumHandle = ClusapiMethods.ClusterNetworkOpenEnum(handle, objectType))
			{
				if (enumHandle.IsInvalid)
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					throw AmExceptionHelper.ConstructClusterApiException(lastWin32Error, "ClusterOpenNetworkEnum(objecttype={0})", new object[]
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
					AmClusterNetworkEnum objectTypeRetrived;
					errorCode = ClusapiMethods.ClusterNetworkEnum(enumHandle, entryIndex, out objectTypeRetrived, objectNameBuffer, ref objectNameLen);
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
				throw AmExceptionHelper.ConstructClusterApiException(errorCode, "ClusterNetworkEnum()", new object[0]);
				IL_171:;
			}
			yield break;
		}

		private AmClusterRawData GetNetworkControlData(AmClusterNetworkControlCode code)
		{
			uint num = 1024U;
			AmClusterRawData amClusterRawData = AmClusterRawData.Allocate(num);
			int num2 = ClusapiMethods.ClusterNetworkControl(this.Handle, IntPtr.Zero, code, IntPtr.Zero, 0U, amClusterRawData.Buffer, num, out num);
			if (num2 == 234)
			{
				amClusterRawData.Dispose();
				amClusterRawData = AmClusterRawData.Allocate(num);
				num2 = ClusapiMethods.ClusterNetworkControl(this.Handle, IntPtr.Zero, code, IntPtr.Zero, 0U, amClusterRawData.Buffer, num, out num);
			}
			if (num2 != 0)
			{
				amClusterRawData.Dispose();
				throw AmExceptionHelper.ConstructClusterApiException(num2, "ClusterNetworkControl({0},controlcode={1})", new object[]
				{
					this.Name,
					code
				});
			}
			return amClusterRawData;
		}

		private void SetNetworkControlData(AmClusterNetworkControlCode code, IntPtr buffer, uint bufferSize)
		{
			uint num = 0U;
			int num2 = ClusapiMethods.ClusterNetworkControl(this.Handle, IntPtr.Zero, code, buffer, bufferSize, IntPtr.Zero, 0U, out num);
			if (num2 != 0)
			{
				ClusterApiException ex = AmExceptionHelper.ConstructClusterApiException(num2, "ClusterNetworkControl(controlcode={0})", new object[]
				{
					code
				});
				AmClusterNetwork.Tracer.TraceDebug((long)this.GetHashCode(), ex.Message);
				throw ex;
			}
		}

		private IEnumerable<AmClusterNetInterface> LazyEnumerateNetworkInterfaces()
		{
			return from nicName in this.EnumerateNetworkInterfaceNames()
			select this.OwningCluster.OpenNetInterface(nicName);
		}

		public const string CLUSREG_NAME_NET_NAME = "Name";

		public const string CLUSREG_NAME_NET_IPV6_ADDRESSES = "IPv6Addresses";

		public const string CLUSREG_NAME_NET_IPV6_PREFIXLENGTHS = "IPv6PrefixLengths";

		public const string CLUSREG_NAME_NET_IPV4_ADDRESSES = "IPv4Addresses";

		public const string CLUSREG_NAME_NET_IPV4_PREFIXLENGTHS = "IPv4PrefixLengths";

		public const string CLUSREG_NAME_NET_ADDRESS = "Address";

		public const string CLUSREG_NAME_NET_ADDRESS_MASK = "AddressMask";

		public const string CLUSREG_NAME_NET_DESC = "Description";

		public const string CLUSREG_NAME_NET_ROLE = "Role";
	}
}
