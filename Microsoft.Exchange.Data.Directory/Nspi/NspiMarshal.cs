using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Rpc;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Nspi
{
	internal static class NspiMarshal
	{
		internal static void GuidToNative(Guid guid, IntPtr ptr)
		{
			if (ptr != IntPtr.Zero)
			{
				Marshal.StructureToPtr(guid, ptr, false);
			}
		}

		internal static SafeRpcMemoryHandle MarshalIntList(IList<int> list)
		{
			if (list == null)
			{
				return null;
			}
			int size = (list.Count + 1) * 4;
			SafeRpcMemoryHandle safeRpcMemoryHandle = new SafeRpcMemoryHandle();
			safeRpcMemoryHandle.Allocate(size);
			int num = 0;
			Marshal.WriteInt32(safeRpcMemoryHandle.DangerousGetHandle(), num, list.Count);
			foreach (int val in list)
			{
				num += 4;
				Marshal.WriteInt32(safeRpcMemoryHandle.DangerousGetHandle(), num, val);
			}
			return safeRpcMemoryHandle;
		}

		internal static SafeRpcMemoryHandle MarshalRowSet(PropRowSet rowset)
		{
			if (rowset == null)
			{
				return null;
			}
			SafeRpcMemoryHandle safeRpcMemoryHandle = new SafeRpcMemoryHandle();
			foreach (PropRow propRow in rowset.Rows)
			{
				SafeRpcMemoryHandle safeRpcMemoryHandle2 = NspiMarshal.MarshalPropValueCollection(propRow.Properties);
				propRow.MarshalledPropertiesHandle = safeRpcMemoryHandle2;
				safeRpcMemoryHandle.AddAssociatedHandle(safeRpcMemoryHandle2);
			}
			safeRpcMemoryHandle.Allocate(rowset.GetBytesToMarshal());
			rowset.MarshalToNative(safeRpcMemoryHandle);
			return safeRpcMemoryHandle;
		}

		internal static SafeRpcMemoryHandle MarshalRow(PropRow row)
		{
			if (row == null)
			{
				return null;
			}
			SafeRpcMemoryHandle safeRpcMemoryHandle = new SafeRpcMemoryHandle();
			SafeRpcMemoryHandle safeRpcMemoryHandle2 = NspiMarshal.MarshalPropValueCollection(row.Properties);
			row.MarshalledPropertiesHandle = safeRpcMemoryHandle2;
			safeRpcMemoryHandle.AddAssociatedHandle(safeRpcMemoryHandle2);
			safeRpcMemoryHandle.Allocate(row.GetBytesToMarshal());
			row.MarshalToNative(safeRpcMemoryHandle);
			return safeRpcMemoryHandle;
		}

		internal static SafeRpcMemoryHandle MarshalPropValueCollection(ICollection<PropValue> properties)
		{
			SafeRpcMemoryHandle safeRpcMemoryHandle = new SafeRpcMemoryHandle();
			safeRpcMemoryHandle.Allocate(PropValue.GetBytesToMarshal(properties));
			PropValue.MarshalToNative(properties, safeRpcMemoryHandle);
			return safeRpcMemoryHandle;
		}
	}
}
