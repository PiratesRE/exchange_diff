using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public static class DnsUtils
	{
		public static DnsUtils.DnsResponse GetARecord(string domain)
		{
			int returnCode;
			DnsUtils.ARecord arecord = (from r in DnsUtils.GetDNSRecords(domain, DnsUtils.DnsRecordType.DnsTypeA, null, out returnCode)
			where r.Type == DnsUtils.DnsRecordType.DnsTypeA
			select r).Cast<DnsUtils.ARecord>().FirstOrDefault<DnsUtils.ARecord>();
			IPAddress ipAddress = (arecord == null) ? IPAddress.None : arecord.IpAddress;
			return new DnsUtils.DnsResponse(returnCode, ipAddress);
		}

		public static DnsUtils.DnsResponse GetAAAARecord(string domain)
		{
			int returnCode;
			DnsUtils.AAAARecord aaaarecord = (from r in DnsUtils.GetDNSRecords(domain, DnsUtils.DnsRecordType.DnsTypeAAAA, null, out returnCode)
			where r.Type == DnsUtils.DnsRecordType.DnsTypeAAAA
			select r).Cast<DnsUtils.AAAARecord>().FirstOrDefault<DnsUtils.AAAARecord>();
			IPAddress ipAddress = (aaaarecord == null) ? IPAddress.None : aaaarecord.IpAddress;
			return new DnsUtils.DnsResponse(returnCode, ipAddress);
		}

		public static string[] GetMXRecords(string domain, out int returnCode)
		{
			return (from DnsUtils.MXRecord mx in DnsUtils.GetDNSRecords(domain, DnsUtils.DnsRecordType.DnsTypeMX, null, out returnCode)
			select mx.NameExchange).ToArray<string>();
		}

		public static DnsUtils.DnsResponse GetMXEndPointForDomain(string domain)
		{
			int returnCode;
			IEnumerable<DnsUtils.DNSRecord> dnsrecords = DnsUtils.GetDNSRecords(domain, DnsUtils.DnsRecordType.DnsTypeMX, new DnsUtils.DnsRecordType[]
			{
				DnsUtils.DnsRecordType.DnsTypeA
			}, out returnCode);
			IEnumerable<DnsUtils.ARecord> source = (from r in dnsrecords
			where r.Type == DnsUtils.DnsRecordType.DnsTypeA
			select r).Cast<DnsUtils.ARecord>();
			if (source.Count<DnsUtils.ARecord>() > 0)
			{
				return new DnsUtils.DnsResponse(returnCode, source.First<DnsUtils.ARecord>().IpAddress);
			}
			foreach (string domain2 in from DnsUtils.MXRecord mx in 
				from r in dnsrecords
				where r.Type == DnsUtils.DnsRecordType.DnsTypeMX
				select r
			select mx.NameExchange)
			{
				IEnumerable<DnsUtils.ARecord> source2 = DnsUtils.GetDNSRecords(domain2, DnsUtils.DnsRecordType.DnsTypeA, null, out returnCode).Cast<DnsUtils.ARecord>();
				if (source2.Count<DnsUtils.ARecord>() > 0)
				{
					return new DnsUtils.DnsResponse(returnCode, source2.Select((DnsUtils.ARecord a) => a.IpAddress).First<IPAddress>());
				}
			}
			source = DnsUtils.GetDNSRecords(domain, DnsUtils.DnsRecordType.DnsTypeA, null, out returnCode).Cast<DnsUtils.ARecord>();
			if (source.Count<DnsUtils.ARecord>() > 0)
			{
				return new DnsUtils.DnsResponse(returnCode, source.First<DnsUtils.ARecord>().IpAddress);
			}
			return new DnsUtils.DnsResponse(returnCode, IPAddress.None);
		}

		public static IEnumerable<IPAddress> GetNSIpEndPointsForDomain(string domain)
		{
			int num;
			IEnumerable<DnsUtils.DNSRecord> dnsrecords = DnsUtils.GetDNSRecords(domain, DnsUtils.DnsRecordType.DnsTypeNS, new DnsUtils.DnsRecordType[]
			{
				DnsUtils.DnsRecordType.DnsTypeA
			}, out num);
			IEnumerable<DnsUtils.ARecord> source = (from r in dnsrecords
			where r.Type == DnsUtils.DnsRecordType.DnsTypeA
			select r).Cast<DnsUtils.ARecord>();
			if (source.Count<DnsUtils.ARecord>() > 0)
			{
				return from a in source
				select a.IpAddress;
			}
			List<IPAddress> list = new List<IPAddress>();
			foreach (string domain2 in from DnsUtils.NSRecord ns in 
				from r in dnsrecords
				where r.Type == DnsUtils.DnsRecordType.DnsTypeNS
				select r
			select ns.NameHost)
			{
				IEnumerable<DnsUtils.ARecord> source2 = DnsUtils.GetDNSRecords(domain2, DnsUtils.DnsRecordType.DnsTypeA, null, out num).Cast<DnsUtils.ARecord>();
				list.AddRange(source2.Select((DnsUtils.ARecord a) => a.IpAddress));
			}
			return list;
		}

		private static IEnumerable<DnsUtils.DNSRecord> GetDNSRecords(string domain, DnsUtils.DnsRecordType queryType, IEnumerable<DnsUtils.DnsRecordType> optionalRecordTypes, out int returnCode)
		{
			IntPtr zero = IntPtr.Zero;
			HashSet<ushort> hashSet;
			if (optionalRecordTypes == null)
			{
				hashSet = new HashSet<ushort>();
			}
			else
			{
				hashSet = new HashSet<ushort>(optionalRecordTypes.Cast<ushort>());
			}
			IEnumerable<DnsUtils.DNSRecord> recordsFromResponse;
			try
			{
				returnCode = DnsUtils.DnsQuery(ref domain, queryType, DnsUtils.DnsQueryOption.DnsQueryBypassCache, 0, ref zero, 0);
				if (returnCode == 0)
				{
					hashSet.Add((ushort)queryType);
				}
				recordsFromResponse = DnsUtils.GetRecordsFromResponse(zero, hashSet);
			}
			finally
			{
				if (zero != IntPtr.Zero)
				{
					DnsUtils.DnsRecordListFree(zero, 0);
				}
			}
			return recordsFromResponse;
		}

		private static IEnumerable<DnsUtils.DNSRecord> GetRecordsFromResponse(IntPtr responsePtr, HashSet<ushort> recordTypes)
		{
			List<DnsUtils.DNSRecord> list = new List<DnsUtils.DNSRecord>();
			IntPtr intPtr = responsePtr;
			while (intPtr != IntPtr.Zero)
			{
				DnsUtils.DNSHeaderLayout dnsheaderLayout = (DnsUtils.DNSHeaderLayout)Marshal.PtrToStructure(intPtr, typeof(DnsUtils.DNSHeaderLayout));
				if (recordTypes.Contains(dnsheaderLayout.Type))
				{
					if (dnsheaderLayout.Type == 15)
					{
						DnsUtils.MXRecordLayout mxRecord = (DnsUtils.MXRecordLayout)Marshal.PtrToStructure(intPtr, typeof(DnsUtils.MXRecordLayout));
						list.Add(new DnsUtils.MXRecord(mxRecord));
					}
					else if (dnsheaderLayout.Type == 2)
					{
						DnsUtils.NSRecordLayout nsRecord = (DnsUtils.NSRecordLayout)Marshal.PtrToStructure(intPtr, typeof(DnsUtils.NSRecordLayout));
						list.Add(new DnsUtils.NSRecord(nsRecord));
					}
					else if (dnsheaderLayout.Type == 1)
					{
						DnsUtils.ARecordLayout aRecord = (DnsUtils.ARecordLayout)Marshal.PtrToStructure(intPtr, typeof(DnsUtils.ARecordLayout));
						list.Add(new DnsUtils.ARecord(aRecord));
					}
					else
					{
						if (dnsheaderLayout.Type != 28)
						{
							throw new NotImplementedException(string.Format("RecordType ={0} not implemented", dnsheaderLayout.Type));
						}
						DnsUtils.AAAARecordLayout aaaaRecord = (DnsUtils.AAAARecordLayout)Marshal.PtrToStructure(intPtr, typeof(DnsUtils.AAAARecordLayout));
						list.Add(new DnsUtils.AAAARecord(aaaaRecord));
					}
				}
				intPtr = dnsheaderLayout.Next;
			}
			return list;
		}

		[DllImport("dnsapi", CharSet = CharSet.Unicode, EntryPoint = "DnsQuery_W", ExactSpelling = true, SetLastError = true)]
		private static extern int DnsQuery([MarshalAs(UnmanagedType.VBByRefStr)] ref string pszName, DnsUtils.DnsRecordType wType, DnsUtils.DnsQueryOption options, int aipServers, ref IntPtr ppQueryResults, int pReserved);

		[DllImport("dnsapi", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern void DnsRecordListFree(IntPtr recordList, int freeType);

		public enum DnsRecordType : ushort
		{
			DnsTypeA = 1,
			DnsTypeNS,
			DnsTypeMX = 15,
			DnsTypeAAAA = 28
		}

		public enum DnsResponseCode : ushort
		{
			DNS_ERROR_RCODE_NO_ERROR,
			DNS_ERROR_RCODE_FORMAT_ERROR = 9001,
			DNS_ERROR_RCODE_SERVER_FAILURE,
			DNS_ERROR_RCODE_NAME_ERROR,
			DNS_ERROR_RCODE_NOT_IMPLEMENTED,
			DNS_ERROR_RCODE_REFUSED,
			DNS_ERROR_RCODE_YXDOMAIN,
			DNS_ERROR_RCODE_YXRRSET,
			DNS_ERROR_RCODE_NXRRSET,
			DNS_ERROR_RCODE_NOTAUTH,
			DNS_ERROR_RCODE_NOTZONE,
			DNS_ERROR_RCODE_BADSIG = 9016,
			DNS_ERROR_RCODE_BADKEY,
			DNS_ERROR_RCODE_BADTIME,
			ERROR_TIMEOUT = 1460
		}

		private enum DnsQueryOption
		{
			DnsQueryAcceptTruncatedResponse = 1,
			DnsQueryBypassCache = 8,
			DnsQueryDontResetTtlValues = 1048576,
			DnsQueryNoHostsFile = 64,
			DnsQueryNoLocalName = 32,
			DnsQueryNoNetBT = 128,
			DnsQueryNoRecursion = 4,
			DnsQueryNoWireQuery = 16,
			DnsQueryReserved = -16777216,
			DnsQueryReturnMessage = 512,
			DnsQueryStandard = 0,
			DnsQueryTreatAsFqdn = 4096,
			DnsQueryUseTcpOnly = 2,
			DnsQueryWireOnly = 256
		}

		private struct DNSHeaderLayout
		{
			public IntPtr Next;

			public string Name;

			public ushort Type;

			public ushort DataLength;

			public uint Flags;

			public uint Ttl;

			public uint Reserved;
		}

		private struct MXRecordLayout
		{
			public DnsUtils.DNSHeaderLayout Header;

			public IntPtr NameExchange;

			public ushort Preference;

			public ushort Pad;
		}

		private struct NSRecordLayout
		{
			public DnsUtils.DNSHeaderLayout Header;

			public IntPtr NameHost;
		}

		private struct ARecordLayout
		{
			public DnsUtils.DNSHeaderLayout Header;

			public uint IpAddress;
		}

		private struct AAAARecordLayout
		{
			public DnsUtils.DNSHeaderLayout Header;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
			public byte[] IpAddress;
		}

		public class DnsResponse
		{
			public DnsResponse(int returnCode, IPAddress ipAddress)
			{
				this.ReturnCode = (DnsUtils.DnsResponseCode)returnCode;
				this.IPAddress = ipAddress;
			}

			public DnsUtils.DnsResponseCode ReturnCode { get; private set; }

			public IPAddress IPAddress { get; private set; }

			public bool DnsResolvedSuccessfuly
			{
				get
				{
					return this.IPAddress != IPAddress.None;
				}
			}
		}

		private class DNSRecord
		{
			public DNSRecord(DnsUtils.DNSHeaderLayout recordHeader)
			{
				this.Name = recordHeader.Name;
				this.Type = (DnsUtils.DnsRecordType)recordHeader.Type;
				this.Flags = recordHeader.Flags;
				this.Ttl = recordHeader.Ttl;
			}

			public string Name { get; private set; }

			public DnsUtils.DnsRecordType Type { get; private set; }

			public uint Flags { get; private set; }

			public uint Ttl { get; private set; }
		}

		private class MXRecord : DnsUtils.DNSRecord
		{
			public MXRecord(DnsUtils.MXRecordLayout mxRecord) : base(mxRecord.Header)
			{
				this.NameExchange = Marshal.PtrToStringAuto(mxRecord.NameExchange);
			}

			public string NameExchange { get; private set; }
		}

		private class ARecord : DnsUtils.DNSRecord
		{
			public ARecord(DnsUtils.ARecordLayout aRecord) : base(aRecord.Header)
			{
				this.IpAddress = new IPAddress((long)((ulong)aRecord.IpAddress));
			}

			public IPAddress IpAddress { get; private set; }
		}

		private class AAAARecord : DnsUtils.DNSRecord
		{
			public AAAARecord(DnsUtils.AAAARecordLayout aaaaRecord) : base(aaaaRecord.Header)
			{
				this.IpAddress = new IPAddress(aaaaRecord.IpAddress);
			}

			public IPAddress IpAddress { get; private set; }
		}

		private class NSRecord : DnsUtils.DNSRecord
		{
			public NSRecord(DnsUtils.NSRecordLayout nsRecord) : base(nsRecord.Header)
			{
				this.NameHost = Marshal.PtrToStringAuto(nsRecord.NameHost);
			}

			public string NameHost { get; private set; }
		}
	}
}
