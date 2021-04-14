using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Win32;
using Microsoft.Win32;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	internal static class ClusapiMethods
	{
		[DllImport("clusapi.dll", SetLastError = true)]
		public static extern AmClusterRegkeyHandle GetClusterResourceKey(AmClusterResourceHandle clusterResourceHandle, RegSAM samDesired);

		[DllImport("clusapi.dll", SetLastError = true)]
		public static extern AmClusterRegkeyHandle GetClusterNetworkKey(AmClusterNetworkHandle clusterNetworkHandle, RegSAM samDesired);

		[DllImport("clusapi.dll", CharSet = CharSet.Unicode)]
		public static extern int ClusterRegEnumKey(AmClusterRegkeyHandle RegKeyHandle, int dwIndex, StringBuilder lpszName, ref int lpcchName, IntPtr lpftLastWriteTime);

		[DllImport("clusapi.dll", CharSet = CharSet.Unicode)]
		public static extern int ClusterRegEnumValue(AmClusterRegkeyHandle RegKeyHandle, int dwIndex, StringBuilder lpszName, ref int lpcchName, ref int lpdwType, IntPtr lpData, ref int lpcbData);

		[DllImport("clusapi.dll", CharSet = CharSet.Unicode, EntryPoint = "ClusterRegDeleteKey")]
		public static extern int ClusterRegDeleteKeyUnsafe(AmClusterRegkeyHandle RegKeyHandle, string lpszSubKey);

		public static int ClusterRegDeleteKey(AmClusterRegkeyHandle RegKeyHandle, string lpszSubKey)
		{
			AmClusterRegkeyHandle amClusterRegkeyHandle = null;
			int retCode = ClusapiMethods.ClusterRegOpenKey(RegKeyHandle, lpszSubKey, RegSAM.Read, out amClusterRegkeyHandle);
			if (retCode == 0)
			{
				amClusterRegkeyHandle.Dispose();
				retCode = ClusApiHook.CallBackDriver(ClusApiHooks.ClusterRegDeleteKey, string.Format("RootKey = {0} Subkey = {1}", RegKeyHandle.Name, lpszSubKey), delegate
				{
					retCode = ClusapiMethods.ClusterRegDeleteKeyUnsafe(RegKeyHandle, lpszSubKey);
					return retCode;
				});
			}
			return retCode;
		}

		[DllImport("resutils.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern uint ResUtilFindDwordProperty([In] IntPtr pPropertyList, [In] uint cbPropertyListSize, [In] string pszPropertyName, out uint pdwPropertyValue);

		[DllImport("resutils.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern uint ResUtilFindSzProperty([In] IntPtr pPropertyList, [In] uint cbPropertyListSize, [In] string pszPropertyName, [MarshalAs(UnmanagedType.LPWStr)] out string propertyValue);

		[DllImport("resutils.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern uint ResUtilFindMultiSzProperty([In] IntPtr pPropertyList, [In] uint cbPropertyListSize, [In] string pszPropertyName, out SafeHGlobalHandle pbPropertyValue, out uint pcbPropertyValueSize);

		[DllImport("resutils.dll")]
		public static extern uint ResUtilVerifyPrivatePropertyList([In] IntPtr pInPropertyList, [In] int cbInPropertyListSize);

		[DllImport("msvcrt.dll")]
		public static extern IntPtr memcpy(IntPtr dest, IntPtr src, int count);

		[DllImport("clusapi.dll", CharSet = CharSet.Unicode, EntryPoint = "OpenCluster", SetLastError = true)]
		private static extern AmClusterHandle OpenClusterInternal([In] string clusterName);

		internal static AmClusterHandle OpenCluster([In] string clusterName)
		{
			AmClusterHandle handle = null;
			ClusApiHook.CallBackDriver(ClusApiHooks.OpenCluster, string.Format("clusterName = {0}", clusterName), delegate
			{
				int result = 0;
				handle = ClusapiMethods.OpenClusterInternal(clusterName);
				if (handle == null || handle.IsInvalid)
				{
					result = Marshal.GetLastWin32Error();
				}
				return result;
			});
			return handle;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[DllImport("clusapi.dll", EntryPoint = "CloseCluster")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CloseClusterInternal([In] IntPtr hCluster);

		internal static bool CloseCluster([In] IntPtr hCluster)
		{
			bool isSuccess = false;
			ClusApiHook.CallBackDriver(ClusApiHooks.CloseCluster, string.Format("hCluster = {0}", hCluster), delegate
			{
				int result = 0;
				isSuccess = ClusapiMethods.CloseClusterInternal(hCluster);
				return result;
			});
			return isSuccess;
		}

		[DllImport("clusapi.dll", CharSet = CharSet.Unicode)]
		internal static extern int GetNodeClusterState([In] string nodeName, [In] [Out] ref AmNodeClusterState dwClusterState);

		[DllImport("clusapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern AmClusterHandle CreateCluster([In] SafeHGlobalHandle pconfig, [In] [Optional] ClusapiMethods.PCLUSTER_SETUP_PROGRESS_CALLBACK pfnProgressCallback, [In] [Optional] IntPtr pvCallbackArg);

		[DllImport("clusapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern AmClusterHandle CreateCluster([In] IntPtr pconfig, [In] [Optional] IntPtr pfnProgressCallback, [In] [Optional] IntPtr pvCallbackArg);

		[DllImport("clusapi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint DestroyCluster([In] AmClusterHandle hCluster, [In] [Optional] IntPtr pfnProgressCallback, [In] [Optional] IntPtr pvCallbackArg, [In] uint fdeleteVirtualComputerObjects);

		[DllImport("clusapi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint DestroyCluster([In] AmClusterHandle hCluster, [In] [Optional] ClusapiMethods.PCLUSTER_SETUP_PROGRESS_CALLBACK pfnProgressCallback, [In] [Optional] IntPtr pvCallbackArg, [In] uint fdeleteVirtualComputerObjects);

		[DllImport("clusapi.dll", CharSet = CharSet.Unicode)]
		internal static extern uint GetClusterInformation([In] AmClusterHandle hCluster, [MarshalAs(UnmanagedType.LPWStr)] [Out] StringBuilder clusterName, ref int clusterNameLength, IntPtr clusterInformation);

		internal static IntPtr ClusterIpEntryArrayToIntPtr(ClusapiMethods.CLUSTER_IP_ENTRY[] ipEntryArray)
		{
			int num = ipEntryArray.Length;
			int num2 = Marshal.SizeOf(typeof(ClusapiMethods.CLUSTER_IP_ENTRY));
			IntPtr result = Marshal.AllocHGlobal(num2 * num);
			for (int i = 0; i < num; i++)
			{
				IntPtr ptr = (IntPtr)(result.ToInt64() + (long)(i * num2));
				Marshal.StructureToPtr(ipEntryArray[i], ptr, false);
			}
			return result;
		}

		internal static IntPtr StringArrayToIntPtr(string[] stringArray)
		{
			int num = stringArray.Length;
			IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr)) * num);
			for (int i = 0; i < num; i++)
			{
				Marshal.WriteIntPtr(intPtr, Marshal.SizeOf(typeof(IntPtr)) * i, Marshal.StringToHGlobalUni(stringArray[i]));
			}
			return intPtr;
		}

		internal static void FreeIntPtrOfMarshalledObjectsArray(IntPtr MarshalledArrayPtr, int numStrings)
		{
			for (int i = 0; i < numStrings; i++)
			{
				IntPtr hglobal = Marshal.ReadIntPtr(MarshalledArrayPtr, Marshal.SizeOf(typeof(IntPtr)) * i);
				Marshal.FreeHGlobal(hglobal);
			}
			Marshal.FreeHGlobal(MarshalledArrayPtr);
		}

		[DllImport("clusapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern AmClusterResourceHandle OpenClusterResource([In] AmClusterHandle hCluster, [In] string resourceName);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[DllImport("clusapi.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool CloseClusterResource([In] IntPtr hResource);

		[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
		internal static extern AmResourceState GetClusterResourceState([In] AmClusterResourceHandle hresource, [Out] StringBuilder owningNodeName, [In] [Out] ref uint nodeNameLength, [Out] StringBuilder owningGroupName, [In] [Out] ref uint groupNameLength);

		[DllImport("clusapi.dll")]
		internal static extern uint OnlineClusterResource([In] AmClusterResourceHandle hresource);

		[DllImport("clusapi.dll")]
		internal static extern uint OfflineClusterResource([In] AmClusterResourceHandle hresource);

		[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
		internal static extern AmClusterResourceHandle CreateClusterResource([In] AmClusterGroupHandle hGroup, [In] string lpszResourceName, [In] string lpszResourceType, [In] ClusterResourceCreateFlags dwFlags);

		[DllImport("clusapi.dll")]
		internal static extern uint DeleteClusterResource([In] AmClusterResourceHandle hResource);

		[DllImport("clusapi.dll")]
		internal static extern uint RemoveClusterResourceDependency([In] AmClusterResourceHandle hResource, [In] AmClusterResourceHandle hDependsOn);

		[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		internal static extern uint SetClusterResourceDependencyExpression([In] AmClusterResourceHandle clusterResourceHandle, [MarshalAs(UnmanagedType.LPWStr)] [In] string dependencyExpression);

		[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		internal static extern uint GetClusterQuorumResource([In] AmClusterHandle hCluster, StringBuilder lpszResourceName, [In] [Out] ref uint lpcchResourceName, StringBuilder lpszDeviceName, [In] [Out] ref uint lpcchDeviceName, out uint lpdwMaxQuorumLogSize);

		[DllImport("clusapi.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, ExactSpelling = true)]
		internal static extern uint SetClusterQuorumResource([In] AmClusterResourceHandle hResource, [MarshalAs(UnmanagedType.LPWStr)] [In] [Optional] string deviceName, [In] uint maxQuorumLogSize);

		[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		internal static extern int ClusterResourceControl([In] AmClusterResourceHandle hResource, [In] IntPtr hNode, [In] AmClusterResourceControlCode controlCode, [In] IntPtr inBuffer, [In] uint inBufferSize, [Out] IntPtr outBuffer, [In] uint outBufferSize, out uint bytesReturned);

		[DllImport("clusapi.dll")]
		internal static extern uint AddClusterResourceNode([In] AmClusterResourceHandle hResource, [In] AmClusterNodeHandle hNode);

		[DllImport("clusapi.dll")]
		internal static extern uint RemoveClusterResourceNode([In] AmClusterResourceHandle hResource, [In] AmClusterNodeHandle hNode);

		[DllImport("clusapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern AmClusterGroupHandle OpenClusterGroup([In] AmClusterHandle hCluster, [In] string groupName);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[DllImport("clusapi.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool CloseClusterGroup([In] IntPtr hGroup);

		[DllImport("clusapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern AmGroupState GetClusterGroupState([In] AmClusterGroupHandle hGroup, StringBuilder nodeName, ref int nodeNameLenth);

		[DllImport("clusapi.dll", CharSet = CharSet.Unicode)]
		internal static extern int ClusterGroupControl([In] AmClusterGroupHandle hGroup, [In] IntPtr hNode, [In] AmClusterGroupControlCode controlCode, [In] IntPtr inBuffer, [In] uint inBufferSize, [Out] IntPtr outBuffer, [In] uint outBufferSize, out uint bytesReturned);

		[DllImport("clusapi.dll")]
		internal static extern uint MoveClusterGroup(AmClusterGroupHandle hGroup, AmClusterNodeHandle hDestinationNode);

		[DllImport("clusapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern AmClusterNodeHandle OpenClusterNode([In] AmClusterHandle hCluster, [In] string nodeName);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[DllImport("clusapi.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool CloseClusterNode([In] IntPtr hNode);

		[DllImport("clusapi.dll", SetLastError = true)]
		internal static extern AmNodeState GetClusterNodeState([In] AmClusterNodeHandle hNode);

		[DllImport("clusapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern AmClusterNodeHandle AddClusterNode([In] AmClusterHandle hCluster, [In] string lpszNodeName, [In] [Optional] ClusapiMethods.PCLUSTER_SETUP_PROGRESS_CALLBACK pfnProgressCallback, [In] [Optional] IntPtr pvCallbackArg);

		[DllImport("clusapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern uint EvictClusterNodeEx([In] AmClusterNodeHandle hNode, [In] uint dwTimeOut, out int phrCleanupStatus);

		[DllImport("clusapi.dll", CharSet = CharSet.Unicode)]
		internal static extern int ClusterNodeControl([In] AmClusterNodeHandle hNode, [In] IntPtr hHostNode, [In] AmClusterNodeControlCode controlCode, [In] IntPtr inBuffer, [In] uint inBufferSize, [Out] IntPtr outBuffer, [In] uint outBufferSize, out uint bytesReturned);

		[DllImport("clusapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern AmClusterNetworkHandle OpenClusterNetwork([In] AmClusterHandle hCluster, [In] string networkName);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[DllImport("clusapi.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool CloseClusterNetwork([In] IntPtr hNetwork);

		[DllImport("clusapi.dll", SetLastError = true)]
		internal static extern AmNetworkState GetClusterNetworkState([In] AmClusterNetworkHandle hNetwork);

		[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		internal static extern int ClusterNetworkControl([In] AmClusterNetworkHandle hCluster, [In] IntPtr hHostNode, [In] AmClusterNetworkControlCode controlCode, [In] IntPtr inBuffer, [In] uint inBufferSize, [Out] IntPtr outBuffer, [In] uint outBufferSize, out uint bytesReturned);

		[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
		internal static extern AmClusterNetInterfaceHandle OpenClusterNetInterface([In] AmClusterHandle hCluster, [In] string networkName);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[DllImport("clusapi.dll", ExactSpelling = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool CloseClusterNetInterface([In] IntPtr hNetInterface);

		[DllImport("clusapi.dll", ExactSpelling = true, SetLastError = true)]
		internal static extern AmNetInterfaceState GetClusterNetInterfaceState([In] AmClusterNetInterfaceHandle hNetInterface);

		[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		internal static extern int ClusterNetInterfaceControl([In] AmClusterNetInterfaceHandle hCluster, [In] IntPtr hHostNode, [In] AmClusterNetInterfaceControlCode controlCode, [In] IntPtr inBuffer, [In] uint inBufferSize, [Out] IntPtr outBuffer, [In] uint outBufferSize, out uint bytesReturned);

		[DllImport("clusapi.dll", SetLastError = true)]
		internal static extern AmClusEnumHandle ClusterOpenEnum([In] AmClusterHandle hCluster, [In] AmClusterEnum dwType);

		[DllImport("clusapi.dll", CharSet = CharSet.Unicode)]
		internal static extern int ClusterEnum([In] AmClusEnumHandle hEnum, [In] int dwIndex, out AmClusterEnum lpdwType, [Out] StringBuilder pName, [In] [Out] ref int count);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[DllImport("clusapi.dll")]
		internal static extern int ClusterCloseEnum([In] IntPtr hEnum);

		[DllImport("clusapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		internal static extern int ClusterControl([In] AmClusterHandle hCluster, [In] IntPtr hHostNode, [In] AmClusterClusterControlCode controlCode, [In] IntPtr inBuffer, [In] uint inBufferSize, [Out] IntPtr outBuffer, [In] uint outBufferSize, out uint bytesReturned);

		[DllImport("clusapi.dll", SetLastError = true)]
		internal static extern AmClusGroupEnumHandle ClusterGroupOpenEnum([In] AmClusterGroupHandle hCluster, [In] AmClusterGroupEnum dwType);

		[DllImport("clusapi.dll", CharSet = CharSet.Unicode)]
		internal static extern int ClusterGroupEnum([In] AmClusGroupEnumHandle hEnum, [In] int dwIndex, out AmClusterGroupEnum lpdwType, [Out] StringBuilder pName, [In] [Out] ref int count);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[DllImport("clusapi.dll")]
		internal static extern int ClusterGroupCloseEnum([In] IntPtr hEnum);

		[DllImport("clusapi.dll", SetLastError = true)]
		internal static extern AmClusResourceEnumHandle ClusterResourceOpenEnum([In] AmClusterResourceHandle hCluster, [In] AmClusterResourceEnum dwType);

		[DllImport("clusapi.dll", CharSet = CharSet.Unicode)]
		internal static extern int ClusterResourceEnum([In] AmClusResourceEnumHandle hEnum, [In] int dwIndex, out AmClusterResourceEnum lpdwType, [Out] StringBuilder pName, [In] [Out] ref int count);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[DllImport("clusapi.dll")]
		internal static extern int ClusterResourceCloseEnum([In] IntPtr hEnum);

		[DllImport("clusapi.dll", SetLastError = true)]
		internal static extern AmClusNodeEnumHandle ClusterNodeOpenEnum([In] AmClusterNodeHandle hCluster, [In] AmClusterNodeEnum dwType);

		[DllImport("clusapi.dll", CharSet = CharSet.Unicode)]
		internal static extern int ClusterNodeEnum([In] AmClusNodeEnumHandle hEnum, [In] int dwIndex, out AmClusterNodeEnum lpdwType, [Out] StringBuilder pName, [In] [Out] ref int count);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[DllImport("clusapi.dll")]
		internal static extern int ClusterNodeCloseEnum([In] IntPtr hEnum);

		[DllImport("clusapi.dll", SetLastError = true)]
		internal static extern AmClusNetworkEnumHandle ClusterNetworkOpenEnum([In] AmClusterNetworkHandle hCluster, [In] AmClusterNetworkEnum dwType);

		[DllImport("clusapi.dll", CharSet = CharSet.Unicode)]
		internal static extern int ClusterNetworkEnum([In] AmClusNetworkEnumHandle hEnum, [In] int dwIndex, out AmClusterNetworkEnum lpdwType, [Out] StringBuilder pName, [In] [Out] ref int count);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[DllImport("clusapi.dll")]
		internal static extern int ClusterNetworkCloseEnum([In] IntPtr hEnum);

		[DllImport("clusapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern AmClusterNotifyHandle CreateClusterNotifyPort([In] AmClusterNotifyHandle hChange, [In] AmClusterHandle hCluster, [In] ClusterNotifyFlags dwFilter, [In] IntPtr dwNotifyKey);

		[DllImport("clusapi.dll", CharSet = CharSet.Unicode)]
		internal static extern int GetClusterNotify([In] AmClusterNotifyHandle hChange, out IntPtr dwNotifyKey, out ClusterNotifyFlags dwFilter, [MarshalAs(UnmanagedType.LPWStr)] [In] [Out] StringBuilder lpszName, [In] [Out] ref uint cbNameSize, [In] uint dwMillisecTimeout);

		[DllImport("clusapi.dll", CharSet = CharSet.Unicode)]
		internal static extern int RegisterClusterNotify([In] AmClusterNotifyHandle hChange, [In] ClusterNotifyFlags dwFilter, [In] SafeHandle hObject, [In] IntPtr dwNotifyKey);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[DllImport("clusapi.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool CloseClusterNotifyPort([In] IntPtr hChange);

		[DllImport("clusapi.dll", EntryPoint = "GetClusterKey", SetLastError = true)]
		private static extern AmClusterRegkeyHandle GetClusterKeyInternal(AmClusterHandle clusterHandle, RegSAM samDesired);

		internal static AmClusterRegkeyHandle GetClusterKey(AmClusterHandle clusterHandle, RegSAM samDesired)
		{
			AmClusterRegkeyHandle handle = null;
			ClusApiHook.CallBackDriver(ClusApiHooks.GetClusterKey, string.Format("clusterHandle = {0} samDesired = {1}", clusterHandle, samDesired), delegate
			{
				int result = 0;
				handle = ClusapiMethods.GetClusterKeyInternal(clusterHandle, samDesired);
				if (handle == null || handle.IsInvalid)
				{
					result = Marshal.GetLastWin32Error();
				}
				else
				{
					handle.Name = "HKLM:\\Cluster";
				}
				return result;
			});
			return handle;
		}

		[DllImport("clusapi.dll", CharSet = CharSet.Unicode, EntryPoint = "ClusterRegOpenKey")]
		private static extern int ClusterRegOpenKeyInternal(AmClusterRegkeyHandle RegKeyHandle, string lpszSubKey, RegSAM samDesired, out AmClusterRegkeyHandle phkResult);

		internal static int ClusterRegOpenKey(AmClusterRegkeyHandle RegKeyHandle, string lpszSubKey, RegSAM samDesired, out AmClusterRegkeyHandle phkResult)
		{
			AmClusterRegkeyHandle phkResultTmp = null;
			int retCode = ClusApiHook.CallBackDriver(ClusApiHooks.ClusterRegOpenKey, string.Format("RootKeyName = {0} SubKey = {1}", RegKeyHandle.Name, lpszSubKey), delegate
			{
				retCode = ClusapiMethods.ClusterRegOpenKeyInternal(RegKeyHandle, lpszSubKey, samDesired, out phkResultTmp);
				if (retCode == 0 && phkResultTmp != null)
				{
					phkResultTmp.Name = RegKeyHandle.Name + "\\" + lpszSubKey;
				}
				return retCode;
			});
			phkResult = phkResultTmp;
			return retCode;
		}

		[DllImport("clusapi.dll", CharSet = CharSet.Unicode, EntryPoint = "ClusterRegCreateKey")]
		private static extern int ClusterRegCreateKeyInternal(AmClusterRegkeyHandle RegKeyHandle, string lpszSubKey, uint options, RegSAM samDesired, IntPtr securityAttributes, out AmClusterRegkeyHandle phkResult, out uint disposition);

		internal static int ClusterRegCreateKey(AmClusterRegkeyHandle RegKeyHandle, string lpszSubKey, uint options, RegSAM samDesired, IntPtr securityAttributes, out AmClusterRegkeyHandle phkResult, out uint disposition)
		{
			AmClusterRegkeyHandle phkResultTmp = null;
			uint dispositionTmp = 0U;
			int retCode = ClusApiHook.CallBackDriver(ClusApiHooks.ClusterRegCreateKey, string.Format("RootKeyName = {0} SubKey = {1}", RegKeyHandle.Name, lpszSubKey), delegate
			{
				retCode = ClusapiMethods.ClusterRegCreateKeyInternal(RegKeyHandle, lpszSubKey, options, samDesired, securityAttributes, out phkResultTmp, out dispositionTmp);
				if (retCode == 0 && phkResultTmp != null)
				{
					phkResultTmp.Name = RegKeyHandle.Name + "\\" + lpszSubKey;
				}
				return retCode;
			});
			phkResult = phkResultTmp;
			disposition = dispositionTmp;
			return retCode;
		}

		[DllImport("clusapi.dll", CharSet = CharSet.Unicode, EntryPoint = "ClusterRegQueryValue")]
		private static extern int ClusterRegQueryValueInternal(AmClusterRegkeyHandle RegKeyHandle, string lpszValueName, out int lpdwValueType, IntPtr lpbData, ref int lpcbData);

		internal static int ClusterRegQueryValue(AmClusterRegkeyHandle RegKeyHandle, string lpszValueName, out RegistryValueKind valueType, IntPtr lpbData, ref int lpcbData)
		{
			int lpcbDataTmp = lpcbData;
			int valueTypeInt = 0;
			int retCode = ClusApiHook.CallBackDriver(ClusApiHooks.ClusterRegQueryValue, string.Format("KeyName = {0} lpszValueName = {1}", RegKeyHandle.Name, lpszValueName), delegate
			{
				retCode = ClusapiMethods.ClusterRegQueryValueInternal(RegKeyHandle, lpszValueName, out valueTypeInt, lpbData, ref lpcbDataTmp);
				return retCode;
			});
			valueType = (RegistryValueKind)valueTypeInt;
			lpcbData = lpcbDataTmp;
			return retCode;
		}

		[DllImport("clusapi.dll", CharSet = CharSet.Unicode, EntryPoint = "ClusterRegSetValue")]
		private static extern int ClusterRegSetValueInternal(AmClusterRegkeyHandle RegKeyHandle, string lpszValueName, RegistryValueKind dwType, IntPtr lpbData, int cbData);

		internal static int ClusterRegSetValue(AmClusterRegkeyHandle RegKeyHandle, string lpszValueName, RegistryValueKind dwType, IntPtr lpbData, int cbData)
		{
			string text = "ClusterRegSetValue";
			string text2 = string.Format("KeyName = {0} lpszValueName = {1}", RegKeyHandle.Name, lpszValueName);
			int retCode = ClusApiHook.CallBackDriver(ClusApiHooks.ClusterRegSetValue, text2, delegate
			{
				retCode = ClusapiMethods.ClusterRegSetValueInternal(RegKeyHandle, lpszValueName, dwType, lpbData, cbData);
				return retCode;
			});
			if (retCode != 0)
			{
				ReplayCrimsonEvents.CriticalClusterApiFailed.Log<string, string, int>(text, text2, retCode);
			}
			return retCode;
		}

		[DllImport("clusapi.dll", CharSet = CharSet.Unicode, EntryPoint = "ClusterRegDeleteValue")]
		private static extern int ClusterRegDeleteValueUnsafe(AmClusterRegkeyHandle RegKeyHandle, string lpszValueName);

		internal static int ClusterRegDeleteValue(AmClusterRegkeyHandle RegKeyHandle, string lpszValueName)
		{
			int num = 0;
			RegistryValueKind registryValueKind;
			int retCode = ClusapiMethods.ClusterRegQueryValue(RegKeyHandle, lpszValueName, out registryValueKind, IntPtr.Zero, ref num);
			if (retCode == 0)
			{
				retCode = ClusApiHook.CallBackDriver(ClusApiHooks.ClusterRegDeleteValue, string.Format("KeyName = {0} lpszValueName = {1}", RegKeyHandle.Name, lpszValueName), delegate
				{
					retCode = ClusapiMethods.ClusterRegDeleteValueUnsafe(RegKeyHandle, lpszValueName);
					return retCode;
				});
			}
			return retCode;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[DllImport("clusapi.dll", EntryPoint = "ClusterRegCloseKey")]
		private static extern int ClusterRegCloseKeyInternal(IntPtr RegKeyHandle);

		internal static int ClusterRegCloseKey(IntPtr RegKeyHandle, string name)
		{
			int retCode = ClusApiHook.CallBackDriver(ClusApiHooks.ClusterRegCloseKey, string.Format("KeyName = {0}, Handle = {1}", name, RegKeyHandle), delegate
			{
				retCode = ClusapiMethods.ClusterRegCloseKeyInternal(RegKeyHandle);
				return retCode;
			});
			return retCode;
		}

		[DllImport("clusapi.dll", EntryPoint = "ClusterRegCreateBatch")]
		private static extern int ClusterRegCreateBatchInternal([In] AmClusterRegkeyHandle hKey, out AmClusterBatchHandle hBatch);

		internal static int ClusterRegCreateBatch([In] AmClusterRegkeyHandle hKey, out AmClusterBatchHandle hBatch)
		{
			AmClusterBatchHandle hBatchTmp = null;
			int retCode = ClusApiHook.CallBackDriver(ClusApiHooks.ClusterRegCreateBatch, string.Format("KeyName = {0}", hKey.Name), delegate
			{
				retCode = ClusapiMethods.ClusterRegCreateBatchInternal(hKey, out hBatchTmp);
				return retCode;
			});
			hBatch = hBatchTmp;
			return retCode;
		}

		[DllImport("clusapi.dll", CharSet = CharSet.Unicode, EntryPoint = "ClusterRegBatchAddCommand")]
		private static extern int ClusterRegBatchAddCommandInternal([In] AmClusterBatchHandle hBatch, [In] CLUSTER_REG_COMMAND dwCommand, [MarshalAs(UnmanagedType.LPWStr)] [In] string wzName, [In] RegistryValueKind dwOptions, [In] IntPtr lpData, [In] int cbData);

		internal static int ClusterRegBatchAddCommand([In] AmClusterBatchHandle hBatch, [In] CLUSTER_REG_COMMAND dwCommand, [MarshalAs(UnmanagedType.LPWStr)] [In] string wzName, [In] RegistryValueKind dwOptions, [In] IntPtr lpData, [In] int cbData)
		{
			int retCode = ClusApiHook.CallBackDriver(ClusApiHooks.ClusterRegBatchAddCommand, string.Format("hBatch = {0} dwCommand = {1} wsName = {2}", hBatch, dwCommand, wzName), delegate
			{
				retCode = ClusapiMethods.ClusterRegBatchAddCommandInternal(hBatch, dwCommand, wzName, dwOptions, lpData, cbData);
				return retCode;
			});
			return retCode;
		}

		[DllImport("clusapi.dll", EntryPoint = "ClusterRegCloseBatch")]
		private static extern int ClusterRegCloseBatchInternal([In] IntPtr hBatch, [MarshalAs(UnmanagedType.Bool)] [In] bool bCommit, [Optional] out int failedCommandNumber);

		internal static int ClusterRegCloseBatch([In] IntPtr hBatch, [MarshalAs(UnmanagedType.Bool)] [In] bool bCommit, [Optional] out int failedCommandNumber)
		{
			int failedCommandNumberTmp = 0;
			int retCode = ClusApiHook.CallBackDriver(ClusApiHooks.ClusterRegCloseBatch, string.Format("hBatch = {0} bCommit = {1}", hBatch, bCommit), delegate
			{
				retCode = ClusapiMethods.ClusterRegCloseBatchInternal(hBatch, bCommit, out failedCommandNumberTmp);
				return retCode;
			});
			failedCommandNumber = failedCommandNumberTmp;
			return retCode;
		}

		internal const int MAX_PATH = 260;

		internal const int INVALID_HANDLE_VALUE = -1;

		internal const uint CREATE_CLUSTER_VERSION = 1536U;

		internal const uint CLUSAPI_VERSION_WINDOWSBLUE = 1794U;

		private const string CLUSAPI = "clusapi.dll";

		private const string ResUtilsDll = "resutils.dll";

		private const string CrtDll = "msvcrt.dll";

		public enum CLUSTER_SETUP_PHASE
		{
			ClusterSetupPhaseInitialize = 1,
			ClusterSetupPhaseValidateNodeState = 100,
			ClusterSetupPhaseValidateClusterNameAccount_Obsolete,
			ClusterSetupPhaseValidateNetft,
			ClusterSetupPhaseValidateClusDisk,
			ClusterSetupPhaseConfigureClusSvc,
			ClusterSetupPhaseStartingClusSvc,
			ClusterSetupPhaseQueryClusterNameAccount,
			ClusterSetupPhaseValidateClusterNameAccount,
			ClusterSetupPhaseCreateClusterAccount,
			ClusterSetupPhaseConfigureClusterAccount,
			ClusterSetupPhaseFormingCluster = 200,
			ClusterSetupPhaseAddClusterProperties,
			ClusterSetupPhaseCreateResourceTypes,
			ClusterSetupPhaseCreateGroups,
			ClusterSetupPhaseCreateIPAddressResources,
			ClusterSetupPhaseCreateNetworkName,
			ClusterSetupPhaseClusterGroupOnline,
			ClusterSetupPhaseGettingCurrentMembership = 300,
			ClusterSetupPhaseAddNodeToCluster,
			ClusterSetupPhaseNodeUp,
			ClusterSetupPhaseMoveGroup = 400,
			ClusterSetupPhaseDeleteGroup,
			ClusterSetupPhaseCleanupCOs,
			ClusterSetupPhaseOfflineGroup,
			ClusterSetupPhaseEvictNode,
			ClusterSetupPhaseCleanupNode,
			ClusterSetupPhaseCoreGroupCleanup,
			ClusterSetupPhaseFailureCleanup = 999
		}

		public enum CLUSTER_SETUP_PHASE_TYPE
		{
			ClusterSetupPhaseStart = 1,
			ClusterSetupPhaseContinue,
			ClusterSetupPhaseEnd
		}

		public enum CLUSTER_SETUP_PHASE_SEVERITY
		{
			ClusterSetupPhaseInformational = 1,
			ClusterSetupPhaseWarning,
			ClusterSetupPhaseFatal
		}

		public delegate int PCLUSTER_SETUP_PROGRESS_CALLBACK(IntPtr pvCallbackArg, ClusapiMethods.CLUSTER_SETUP_PHASE eSetupPhase, ClusapiMethods.CLUSTER_SETUP_PHASE_TYPE ePhaseType, ClusapiMethods.CLUSTER_SETUP_PHASE_SEVERITY ePhaseSeverity, uint dwPercentComplete, [MarshalAs(UnmanagedType.LPWStr)] [In] string lpszObjectName, uint dwStatus);

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal class CLUSTER_IP_ENTRY
		{
			public CLUSTER_IP_ENTRY(string ipaddr, uint prefixLength)
			{
				this.szIpAddress = ipaddr;
				this.dwPrefixLength = prefixLength;
			}

			[MarshalAs(UnmanagedType.LPWStr)]
			internal string szIpAddress;

			internal uint dwPrefixLength;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal class CREATE_CLUSTER_CONFIG
		{
			internal uint dwVersion;

			[MarshalAs(UnmanagedType.LPWStr)]
			internal string lpszClusterName;

			internal uint cNodes;

			internal IntPtr ppszNodeNames;

			internal uint cIpEntries;

			internal IntPtr pIpEntries;

			internal uint fEmptyCluster;
		}
	}
}
