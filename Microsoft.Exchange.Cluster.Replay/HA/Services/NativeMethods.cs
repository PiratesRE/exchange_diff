using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.HA.Services
{
	public static class NativeMethods
	{
		[DllImport("iphlpapi.dll")]
		private static extern int GetExtendedTcpTable(IntPtr pTcpTable, ref int pdwSize, bool bOrder, uint ulAf, NativeMethods.TCP_TABLE_CLASS TableClass, uint reserved);

		[DllImport("iphlpapi.dll")]
		private static extern int SetTcpEntry(IntPtr pTcprow);

		[DllImport("wsock32.dll")]
		private static extern int ntohs(int netshort);

		[DllImport("wsock32.dll")]
		private static extern int htons(int netshort);

		public static NativeMethods.SocketData GetOpenSocketByPort(int port)
		{
			List<NativeMethods.SocketData> list = (from socket in NativeMethods.GetOpenedSockets()
			where socket.LocalPort == port
			select socket).ToList<NativeMethods.SocketData>();
			if (list.Count > 0)
			{
				return list.First<NativeMethods.SocketData>();
			}
			return null;
		}

		public static List<NativeMethods.SocketData> GetOpenedSockets()
		{
			List<NativeMethods.SocketData> list = new List<NativeMethods.SocketData>();
			list.AddRange(NativeMethods.GetIPv4OpenedSockets());
			list.AddRange(NativeMethods.GetIPv6OpenedSockets());
			return list;
		}

		private static List<NativeMethods.SocketData> GetIPv4OpenedSockets()
		{
			return NativeMethods.ListActiveSockets<NativeMethods.MIB_TCPROW_OWNER_PID>(default(NativeMethods.MIB_TCPROW_OWNER_PID));
		}

		private static List<NativeMethods.SocketData> GetIPv6OpenedSockets()
		{
			return NativeMethods.ListActiveSockets<NativeMethods.MIB_TCP6ROW_OWNER_PID>(default(NativeMethods.MIB_TCP6ROW_OWNER_PID));
		}

		private static List<NativeMethods.SocketData> ListActiveSockets<T>(T tcpStructure)
		{
			if (typeof(T) != typeof(NativeMethods.MIB_TCPROW_OWNER_PID) && typeof(T) != typeof(NativeMethods.MIB_TCP6ROW_OWNER_PID))
			{
				throw new ArgumentException(typeof(T).FullName + " not supported for method GetSocketsOpenedByProcess<T>");
			}
			IntPtr intPtr = IntPtr.Zero;
			List<NativeMethods.SocketData> list = new List<NativeMethods.SocketData>();
			List<NativeMethods.SocketData> result;
			try
			{
				int cb = 0;
				uint ulAf;
				if (typeof(T) == typeof(NativeMethods.MIB_TCPROW_OWNER_PID))
				{
					ulAf = 2U;
				}
				else
				{
					ulAf = 23U;
				}
				NativeMethods.GetExtendedTcpTable(IntPtr.Zero, ref cb, true, ulAf, NativeMethods.TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL, 0U);
				intPtr = Marshal.AllocCoTaskMem(cb);
				NativeMethods.GetExtendedTcpTable(intPtr, ref cb, true, ulAf, NativeMethods.TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL, 0U);
				int num = Marshal.ReadInt32(intPtr);
				int offset = Marshal.SizeOf(tcpStructure);
				IntPtr intPtr2 = IntPtr.Add(intPtr, 4);
				for (int i = 0; i < num; i++)
				{
					tcpStructure = (T)((object)Marshal.PtrToStructure(intPtr2, tcpStructure.GetType()));
					NativeMethods.SocketData item = NativeMethods.CreateSocketData(tcpStructure);
					list.Add(item);
					intPtr2 = IntPtr.Add(intPtr2, offset);
				}
				result = list;
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.FreeCoTaskMem(intPtr);
				}
			}
			return result;
		}

		private static NativeMethods.SocketData CreateSocketData(object tcpStructure)
		{
			NativeMethods.SocketData socketData = new NativeMethods.SocketData();
			if (tcpStructure.GetType() == typeof(NativeMethods.MIB_TCPROW_OWNER_PID))
			{
				NativeMethods.MIB_TCPROW_OWNER_PID mib_TCPROW_OWNER_PID = (NativeMethods.MIB_TCPROW_OWNER_PID)tcpStructure;
				socketData.LocalAddress = new IPAddress((long)((ulong)mib_TCPROW_OWNER_PID.dwLocalAddr));
				socketData.LocalPort = NativeMethods.ntohs(mib_TCPROW_OWNER_PID.dwLocalPort);
				socketData.RemoteAddress = new IPAddress((long)((ulong)mib_TCPROW_OWNER_PID.dwRemoteAddr));
				socketData.RemotePort = NativeMethods.ntohs(mib_TCPROW_OWNER_PID.dwRemotePort);
				socketData.OwnerPid = mib_TCPROW_OWNER_PID.dwOwningPid;
				socketData.State = mib_TCPROW_OWNER_PID.dwState;
			}
			else
			{
				if (!(tcpStructure.GetType() == typeof(NativeMethods.MIB_TCP6ROW_OWNER_PID)))
				{
					throw new ArgumentException("Type not supported: " + tcpStructure.GetType().FullName);
				}
				NativeMethods.MIB_TCP6ROW_OWNER_PID mib_TCP6ROW_OWNER_PID = (NativeMethods.MIB_TCP6ROW_OWNER_PID)tcpStructure;
				socketData.LocalAddress = new IPAddress(mib_TCP6ROW_OWNER_PID.ucLocalAddr, (long)((ulong)mib_TCP6ROW_OWNER_PID.dwLocalScopeId));
				socketData.LocalPort = NativeMethods.ntohs(mib_TCP6ROW_OWNER_PID.dwLocalPort);
				socketData.RemoteAddress = new IPAddress(mib_TCP6ROW_OWNER_PID.ucRemoteAddr, (long)((ulong)mib_TCP6ROW_OWNER_PID.dwRemoteScopeId));
				socketData.RemotePort = NativeMethods.ntohs(mib_TCP6ROW_OWNER_PID.dwRemotePort);
				socketData.OwnerPid = mib_TCP6ROW_OWNER_PID.dwOwningPid;
				socketData.State = mib_TCP6ROW_OWNER_PID.dwState;
			}
			return socketData;
		}

		private const uint AF_INET = 2U;

		private const uint AF_INET6 = 23U;

		public struct MIB_TCPROW_OWNER_PID
		{
			public NativeMethods.State dwState;

			public uint dwLocalAddr;

			public int dwLocalPort;

			public uint dwRemoteAddr;

			public int dwRemotePort;

			public int dwOwningPid;
		}

		public struct MIB_TCP6ROW_OWNER_PID
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
			public byte[] ucLocalAddr;

			public uint dwLocalScopeId;

			public int dwLocalPort;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
			public byte[] ucRemoteAddr;

			public uint dwRemoteScopeId;

			public int dwRemotePort;

			public NativeMethods.State dwState;

			public int dwOwningPid;
		}

		private enum TCP_TABLE_CLASS
		{
			TCP_TABLE_BASIC_LISTENER,
			TCP_TABLE_BASIC_CONNECTIONS,
			TCP_TABLE_BASIC_ALL,
			TCP_TABLE_OWNER_PID_LISTENER,
			TCP_TABLE_OWNER_PID_CONNECTIONS,
			TCP_TABLE_OWNER_PID_ALL,
			TCP_TABLE_OWNER_MODULE_LISTENER,
			TCP_TABLE_OWNER_MODULE_CONNECTIONS,
			TCP_TABLE_OWNER_MODULE_ALL
		}

		public enum State : uint
		{
			All,
			Closed,
			Listen,
			Syn_Sent,
			Syn_Rcvd,
			Established,
			Fin_Wait1,
			Fin_Wait2,
			Close_Wait,
			Closing,
			Last_Ack,
			Time_Wait,
			Delete_TCB
		}

		public class SocketData
		{
			public uint LocalAddressInt
			{
				get
				{
					return NativeMethods.SocketData.GetIntRepresentationOfByteArray(this.LocalAddress.GetAddressBytes());
				}
			}

			public uint RemoteAddressInt
			{
				get
				{
					return NativeMethods.SocketData.GetIntRepresentationOfByteArray(this.RemoteAddress.GetAddressBytes());
				}
			}

			private static uint GetIntRepresentationOfByteArray(byte[] bytes)
			{
				uint num = 0U;
				for (int i = bytes.Length - 1; i >= 0; i--)
				{
					num = (num << 8) + (uint)bytes[i];
				}
				return num;
			}

			public IPAddress LocalAddress;

			public int LocalPort;

			public IPAddress RemoteAddress;

			public int RemotePort;

			public int OwnerPid;

			public NativeMethods.State State;
		}
	}
}
