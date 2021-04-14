using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Management.Deployment
{
	internal class DnsApi
	{
		[DllImport("dnsapi.dll", CharSet = CharSet.Unicode, EntryPoint = "DnsQuery_W")]
		public static extern int DnsQuery(string name, DnsApi.DnsRecordType type, DnsApi.DnsQueryOptions options, IntPtr dnsServers, ref IntPtr QueryResultSet, IntPtr reserved);

		[DllImport("dnsapi.dll", CharSet = CharSet.Unicode)]
		public static extern void DnsRecordListFree(IntPtr ptrRecords, DnsApi.DnsFreeType freeType);

		public enum DnsFreeType
		{
			DnsFreeFlat,
			DnsFreeRecordList,
			DnsFreeParsedMessageFields
		}

		public enum DnsStatus
		{
			Success
		}

		public enum DnsRecordType : ushort
		{
			A = 1,
			NS,
			CNAME = 5,
			SOA,
			PTR = 12,
			MX = 15,
			TXT,
			AAAA = 28,
			SRV = 33
		}

		[Flags]
		public enum DnsQueryOptions
		{
			Standard = 0,
			AcceptTruncatedResponse = 1,
			UseTcpOnly = 2,
			NoRecursion = 4,
			ByPassCache = 8,
			TreatAsFqdn = 4096
		}

		public enum DNSErrorCodes
		{
			DNS_ERROR_RESPONSE_CODES_BASE = 9000,
			DNS_ERROR_RCODE_FORMAT_ERROR,
			DNS_ERROR_RCODE_SERVER_FAILURE,
			DNS_ERROR_RCODE_NAME_ERROR
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct DNS_MX_DATA
		{
			public string PNameExchange;

			public ushort WPreference;

			public ushort Pad;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct DNS_SRV_DATA
		{
			public string PNameTarget;

			public ushort WPriority;

			public ushort WWeight;

			public ushort WPort;

			public ushort Pad;
		}

		public struct IP6_ADDRESS
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
			public byte[] Address;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct DNS_RECORD_BASE
		{
			public readonly IntPtr PNext;

			public string PName;

			public ushort WType;

			public ushort WDataLength;

			public uint Flags;

			public uint DWTtl;

			public uint DWReserved;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct DNS_RECORD_A
		{
			public readonly IntPtr PNext;

			public string PName;

			public ushort WType;

			public ushort WDataLength;

			public uint Flags;

			public uint DWTtl;

			public uint DWReserved;

			public uint A;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct DNS_RECORD_AAAA
		{
			public readonly IntPtr PNext;

			public string PName;

			public ushort WType;

			public ushort WDataLength;

			public uint Flags;

			public uint DWTtl;

			public uint DWReserved;

			public DnsApi.IP6_ADDRESS A;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct DNS_RECORD_PTR
		{
			public readonly IntPtr PNext;

			public string PName;

			public ushort WType;

			public ushort WDataLength;

			public uint Flags;

			public uint DWTtl;

			public uint DWReserved;

			public string PTR;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct DNS_RECORD_MX
		{
			public readonly IntPtr PNext;

			public string PName;

			public ushort WType;

			public ushort WDataLength;

			public uint Flags;

			public uint DWTtl;

			public uint DWReserved;

			public DnsApi.DNS_MX_DATA MX;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct DNS_RECORD_SRV
		{
			public readonly IntPtr PNext;

			public string PName;

			public ushort WType;

			public ushort WDataLength;

			public uint Flags;

			public uint DWTtl;

			public uint DWReserved;

			public DnsApi.DNS_SRV_DATA SRV;
		}
	}
}
