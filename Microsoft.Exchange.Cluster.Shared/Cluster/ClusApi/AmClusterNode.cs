using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Win32;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	internal class AmClusterNode : EqualityComparer<AmClusterNode>, IDisposeTrackable, IEquatable<AmClusterNode>, IEqualityComparer<AmClusterNode>, IAmClusterNode, IDisposable
	{
		internal static bool IsNodeUp(AmNodeState nodeState)
		{
			return nodeState == AmNodeState.Up || nodeState == AmNodeState.Paused;
		}

		internal static AmServerName GetNameById(int nodeId)
		{
			AmServerName result = null;
			string name = string.Format("Cluster\\Nodes\\{0}", nodeId);
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(name))
			{
				if (registryKey != null)
				{
					result = new AmServerName((string)registryKey.GetValue("NodeName"));
				}
			}
			return result;
		}

		internal static IEnumerable<int> GetNodeIdsFromNodeMask(long nodeMask)
		{
			int nodeId = 1;
			while (nodeMask != 0L)
			{
				nodeMask >>= 1;
				if ((nodeMask & 1L) == 1L)
				{
					yield return nodeId;
				}
				nodeId++;
			}
			yield break;
		}

		internal AmClusterNode(AmServerName nodeName, IAmCluster owningCluster, AmClusterNodeHandle nodeHandle)
		{
			this.m_disposeTracker = this.GetDisposeTracker();
			this.Name = nodeName;
			this.OwningCluster = owningCluster;
			this.Handle = nodeHandle;
		}

		public AmServerName Name { get; private set; }

		internal IAmCluster OwningCluster { get; private set; }

		public AmClusterNodeHandle Handle { get; private set; }

		public AmNodeState State
		{
			get
			{
				return this.GetState(false);
			}
		}

		public string GetNodeIdentifier()
		{
			string text = string.Empty;
			using (AmClusterRawData nodeControlData = this.GetNodeControlData(AmClusterNodeControlCode.CLUSCTL_NODE_GET_ID, 1024U))
			{
				text = nodeControlData.ReadString();
				AmTrace.Debug("GetNodeIdentifier: Node '{0}' is identified by '{1}'.", new object[]
				{
					this.Name,
					text
				});
			}
			return text;
		}

		public bool IsNetworkVisible(string networkName)
		{
			AmClusterNetInterface amClusterNetInterface;
			bool result = this.IsNetworkVisible(networkName, out amClusterNetInterface);
			using (amClusterNetInterface)
			{
			}
			return result;
		}

		public bool IsNetworkVisible(string networkStr1, out AmClusterNetInterface networkInterface)
		{
			networkInterface = null;
			foreach (AmClusterNetInterface amClusterNetInterface in this.EnumerateNetInterfaces())
			{
				string networkName = amClusterNetInterface.GetNetworkName();
				if (SharedHelper.StringIEquals(networkStr1, networkName))
				{
					networkInterface = amClusterNetInterface;
					return true;
				}
				amClusterNetInterface.Dispose();
			}
			return false;
		}

		public override string ToString()
		{
			return string.Format("node:{0}", this.Name.NetbiosName);
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<AmClusterNode>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.m_disposeTracker != null)
			{
				this.m_disposeTracker.Suppress();
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public override bool Equals(AmClusterNode x, AmClusterNode y)
		{
			return x.Equals(y);
		}

		public override int GetHashCode(AmClusterNode obj)
		{
			return obj.GetHashCode();
		}

		public override int GetHashCode()
		{
			if (this.Name == null)
			{
				return 0;
			}
			return this.Name.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public bool Equals(AmClusterNode other)
		{
			return this.Name.Equals(other.Name);
		}

		public AmNodeState GetState(bool isThrowIfUnknown)
		{
			AmNodeState clusterNodeState = ClusapiMethods.GetClusterNodeState(this.Handle);
			if (clusterNodeState == AmNodeState.Unknown)
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				Exception ex = new Win32Exception(lastWin32Error);
				AmTrace.Debug("GetClusterNodeState() returned error (rc={0}, message={1})", new object[]
				{
					lastWin32Error,
					ex
				});
				if (isThrowIfUnknown)
				{
					throw AmExceptionHelper.ConstructClusterApiException(lastWin32Error, "GetClusterNodeState()", new object[0]);
				}
			}
			return clusterNodeState;
		}

		public IEnumerable<AmClusterNetInterface> EnumerateNetInterfaces()
		{
			return from nicName in this.EnumerateNetInterfaceNames()
			select this.OwningCluster.OpenNetInterface(nicName);
		}

		internal IEnumerable<string> EnumerateNetInterfaceNames()
		{
			return AmClusterNode.EnumerateObjects(this.Handle, AmClusterNodeEnum.CLUSTER_NODE_ENUM_NETINTERFACES);
		}

		public long GetHungNodesMask(out int currentGumId)
		{
			currentGumId = 0;
			long result = 0L;
			using (AmClusterRawData nodeControlData = this.GetNodeControlData(AmClusterNodeControlCode.CLUSCTL_NODE_GET_STUCK_NODES, 1024U))
			{
				IntPtr intPtr = nodeControlData.Buffer;
				currentGumId = Marshal.ReadInt32(intPtr);
				intPtr += Marshal.SizeOf(typeof(int));
				result = Marshal.ReadInt64(intPtr);
			}
			return result;
		}

		protected void Dispose(bool disposing)
		{
			lock (this)
			{
				if (!this.m_isDisposed)
				{
					if (disposing)
					{
						if (!this.Handle.IsInvalid)
						{
							this.Handle.Dispose();
							this.Handle = null;
						}
						if (this.m_disposeTracker != null)
						{
							this.m_disposeTracker.Dispose();
							this.m_disposeTracker = null;
						}
					}
					this.m_isDisposed = true;
				}
			}
		}

		private static IEnumerable<string> EnumerateObjects(AmClusterNodeHandle handle, AmClusterNodeEnum objectType)
		{
			new List<string>(16);
			using (AmClusNodeEnumHandle enumHandle = ClusapiMethods.ClusterNodeOpenEnum(handle, objectType))
			{
				if (enumHandle.IsInvalid)
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					throw AmExceptionHelper.ConstructClusterApiException(lastWin32Error, "ClusterOpenNodeEnum(objecttype={0})", new object[]
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
					AmClusterNodeEnum objectTypeRetrived;
					errorCode = ClusapiMethods.ClusterNodeEnum(enumHandle, entryIndex, out objectTypeRetrived, objectNameBuffer, ref objectNameLen);
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
				throw AmExceptionHelper.ConstructClusterApiException(errorCode, "ClusterNodeEnum()", new object[0]);
				IL_171:;
			}
			yield break;
		}

		private AmClusterRawData GetNodeControlData(AmClusterNodeControlCode code, uint initialDataSize = 1024U)
		{
			uint num = initialDataSize;
			AmClusterRawData amClusterRawData = AmClusterRawData.Allocate(num);
			int num2 = ClusapiMethods.ClusterNodeControl(this.Handle, IntPtr.Zero, code, IntPtr.Zero, 0U, amClusterRawData.Buffer, num, out num);
			if (num2 == 234)
			{
				amClusterRawData.Dispose();
				amClusterRawData = AmClusterRawData.Allocate(num);
				num2 = ClusapiMethods.ClusterNodeControl(this.Handle, IntPtr.Zero, code, IntPtr.Zero, 0U, amClusterRawData.Buffer, num, out num);
			}
			if (num2 != 0)
			{
				amClusterRawData.Dispose();
				throw AmExceptionHelper.ConstructClusterApiException(num2, "ClusterNodeControl(controlcode={0})", new object[]
				{
					code
				});
			}
			return amClusterRawData;
		}

		private DisposeTracker m_disposeTracker;

		private bool m_isDisposed;
	}
}
