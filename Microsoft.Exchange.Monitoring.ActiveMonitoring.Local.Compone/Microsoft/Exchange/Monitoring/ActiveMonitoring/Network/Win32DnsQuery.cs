using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Network
{
	internal static class Win32DnsQuery
	{
		internal static Win32DnsQueryResult<IPAddress> ResolveRecordsA(string domainName, IPAddress server)
		{
			return Win32DnsQuery.ResolveRecords<IPAddress>(domainName, server, Win32DnsQuery.RecordType.DNS_TYPE_A, (Win32DnsQuery.DnsRecord dnsRecord) => new IPAddress((long)((ulong)((uint)Marshal.PtrToStructure(dnsRecord.Data, typeof(uint))))));
		}

		internal static Win32DnsQueryResult<IPAddress> ResolveRecordsAaaa(string domainName, IPAddress server)
		{
			return Win32DnsQuery.ResolveRecords<IPAddress>(domainName, server, Win32DnsQuery.RecordType.DNS_TYPE_AAAA, delegate(Win32DnsQuery.DnsRecord dnsRecord)
			{
				byte[] array = new byte[16];
				Marshal.Copy(dnsRecord.Data, array, 0, 16);
				return new IPAddress(array);
			});
		}

		internal static Win32DnsQueryResult<string> ResolveRecordsAny(string domainName, IPAddress server)
		{
			return Win32DnsQuery.ResolveRecords<string>(domainName, server, Win32DnsQuery.RecordType.DNS_TYPE_ANY, (Win32DnsQuery.DnsRecord dnsRecord) => string.Format("{0}  {1}", dnsRecord.Name, (Win32DnsQuery.RecordType)dnsRecord.Type));
		}

		internal static Win32DnsQueryResult<string> ResolveRecordsCname(string domainName, IPAddress server)
		{
			return Win32DnsQuery.ResolveRecords<string>(domainName, server, Win32DnsQuery.RecordType.DNS_TYPE_CNAME, (Win32DnsQuery.DnsRecord dnsRecord) => Marshal.PtrToStringUni(Marshal.ReadIntPtr(dnsRecord.Data)));
		}

		internal static Win32DnsQueryResult<string> ResolveRecordsTxt(string domainName, IPAddress server)
		{
			return Win32DnsQuery.ResolveRecords<string>(domainName, server, Win32DnsQuery.RecordType.DNS_TYPE_TEXT, delegate(Win32DnsQuery.DnsRecord dnsRecord)
			{
				Win32DnsQuery.DNS_TXT_DATA dns_TXT_DATA = (Win32DnsQuery.DNS_TXT_DATA)Marshal.PtrToStructure(dnsRecord.Data, typeof(Win32DnsQuery.DNS_TXT_DATA));
				if (dns_TXT_DATA.StringCount == 1U)
				{
					return Marshal.PtrToStringUni(dns_TXT_DATA.StringArray);
				}
				StringBuilder stringBuilder = new StringBuilder();
				IntPtr intPtr = dns_TXT_DATA.StringArray;
				int num = 0;
				while ((long)num < (long)((ulong)dns_TXT_DATA.StringCount))
				{
					string text = Marshal.PtrToStringUni(intPtr);
					stringBuilder.Append(text);
					intPtr += 2 * (text.Length + 1);
					num++;
				}
				return stringBuilder.ToString();
			});
		}

		[DllImport("dnsapi", CharSet = CharSet.Unicode, EntryPoint = "DnsQuery_W", ExactSpelling = true)]
		private static extern int DnsQuery([MarshalAs(UnmanagedType.LPWStr)] string lpstrName, Win32DnsQuery.RecordType wType, Win32DnsQuery.QueryOptions fOptions, ref Win32DnsQuery.IP4_ARRAY pServerIpArray, out IntPtr ppQueryResultsSet, uint pReserved);

		[DllImport("dnsapi")]
		private static extern void DnsRecordListFree(IntPtr pRecordList, int fFreeType);

		private static Win32DnsQuery.IP4_ARRAY GetServerIp4Array(IPAddress server)
		{
			Win32DnsQuery.IP4_ARRAY result = default(Win32DnsQuery.IP4_ARRAY);
			if (server == null || server == IPAddress.None)
			{
				result.AddrCount = 0U;
				result.AddrArray = null;
			}
			else
			{
				if (server.AddressFamily != AddressFamily.InterNetwork)
				{
					throw new ArgumentException(string.Format("The IP address must be an IPv4 address ({0}).", server), "server");
				}
				result.AddrCount = 1U;
				result.AddrArray = new uint[]
				{
					BitConverter.ToUInt32(server.GetAddressBytes(), 0)
				};
			}
			return result;
		}

		private static Win32DnsQueryResult<T> ResolveRecords<T>(string domainName, IPAddress server, Win32DnsQuery.RecordType type, Func<Win32DnsQuery.DnsRecord, T> interpreter)
		{
			Win32DnsQuery.ValidateArgumentNotNull(domainName, "domainName");
			Win32DnsQuery.ValidateArgumentNotNull(interpreter, "interpreter");
			Win32DnsQuery.QueryOptions fOptions;
			if (server == null)
			{
				fOptions = Win32DnsQuery.QueryOptions.DNS_QUERY_STANDARD;
			}
			else
			{
				fOptions = Win32DnsQuery.QueryOptions.DNS_QUERY_BYPASS_CACHE;
			}
			Win32DnsQuery.IP4_ARRAY serverIp4Array = Win32DnsQuery.GetServerIp4Array(server);
			IntPtr zero = IntPtr.Zero;
			Win32DnsQueryResult<T> result;
			try
			{
				Stopwatch stopwatch = Stopwatch.StartNew();
				long num = (long)Win32DnsQuery.DnsQuery(domainName, type, fOptions, ref serverIp4Array, out zero, 0U);
				stopwatch.Stop();
				if (num != 0L)
				{
					result = new Win32DnsQueryResult<T>(stopwatch.Elapsed, num, null);
				}
				else
				{
					List<T> list = new List<T>();
					IntPtr intPtr = zero;
					while (intPtr != IntPtr.Zero)
					{
						Win32DnsQuery.DnsRecord arg = (Win32DnsQuery.DnsRecord)Marshal.PtrToStructure(intPtr, typeof(Win32DnsQuery.DnsRecord));
						if (arg.Type == (ushort)type || type == Win32DnsQuery.RecordType.DNS_TYPE_ANY)
						{
							arg.Data = intPtr + Marshal.OffsetOf(typeof(Win32DnsQuery.DnsRecord), "Data").ToInt32();
							T item = interpreter(arg);
							list.Add(item);
						}
						intPtr = arg.Next;
					}
					result = new Win32DnsQueryResult<T>(stopwatch.Elapsed, num, list.ToArray());
				}
			}
			finally
			{
				if (zero != IntPtr.Zero)
				{
					Win32DnsQuery.DnsRecordListFree(zero, 1);
				}
			}
			return result;
		}

		private static void ValidateArgumentNotNull(object argument, string name)
		{
			if (argument == null)
			{
				throw new ArgumentNullException(name);
			}
		}

		[Flags]
		private enum QueryOptions : uint
		{
			DNS_QUERY_STANDARD = 0U,
			DNS_QUERY_ACCEPT_TRUNCATED_RESPONSE = 1U,
			DNS_QUERY_USE_TCP_ONLY = 2U,
			DNS_QUERY_NO_RECURSION = 4U,
			DNS_QUERY_BYPASS_CACHE = 8U,
			DNS_QUERY_NO_WIRE_QUERY = 16U,
			DNS_QUERY_NO_LOCAL_NAME = 32U,
			DNS_QUERY_NO_HOSTS_FILE = 64U,
			DNS_QUERY_NO_NETBT = 128U,
			DNS_QUERY_WIRE_ONLY = 256U,
			DNS_QUERY_RETURN_MESSAGE = 512U,
			DNS_QUERY_MULTICAST_ONLY = 1024U,
			DNS_QUERY_NO_MULTICAST = 2048U,
			DNS_QUERY_TREAT_AS_FQDN = 4096U,
			DNS_QUERY_ADDRCONFIG = 8192U,
			DNS_QUERY_DUAL_ADDR = 16384U,
			DNS_QUERY_MULTICAST_WAIT = 131072U,
			DNS_QUERY_MULTICAST_VERIFY = 262144U,
			DNS_QUERY_DONT_RESET_TTL_VALUES = 1048576U,
			DNS_QUERY_DISABLE_IDN_ENCODING = 2097152U,
			DNS_QUERY_APPEND_MULTILABEL = 8388608U,
			DNS_QUERY_RESERVED = 4026531840U
		}

		private enum RecordType : ushort
		{
			DNS_TYPE_A = 1,
			DNS_TYPE_CNAME = 5,
			DNS_TYPE_TEXT = 16,
			DNS_TYPE_AAAA = 28,
			DNS_TYPE_ANY = 255
		}

		private struct DnsRecord
		{
			public IntPtr Next;

			[MarshalAs(UnmanagedType.LPWStr)]
			public string Name;

			public ushort Type;

			public ushort DataLength;

			public uint Flags;

			public uint Ttl;

			public uint Reserved;

			public IntPtr Data;
		}

		private struct DNS_TXT_DATA
		{
			public uint StringCount;

			public IntPtr StringArray;
		}

		private struct IP4_ARRAY
		{
			public uint AddrCount;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1, ArraySubType = UnmanagedType.U4)]
			public uint[] AddrArray;
		}
	}
}
