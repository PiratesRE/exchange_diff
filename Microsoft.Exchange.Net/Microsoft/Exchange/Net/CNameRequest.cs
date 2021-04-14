using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Net
{
	internal sealed class CNameRequest : Request
	{
		public CNameRequest(DnsServerList list, DnsQueryOptions flags, Dns dnsInstance, AsyncCallback requestCallback, object stateObject) : base(list, flags, dnsInstance, requestCallback, stateObject)
		{
		}

		public int MaxCNameRecords
		{
			get
			{
				return this.maxCNameRecords;
			}
			set
			{
				this.maxCNameRecords = value;
			}
		}

		protected override Request.Result InvalidDataResult
		{
			get
			{
				return CNameRequest.InvalidDataRequestResult;
			}
		}

		public IAsyncResult Resolve(string domainName)
		{
			ExTraceGlobals.DNSTracer.Information<string>((long)this.GetHashCode(), "CNAME request for server {0}", domainName);
			this.queryFor = domainName;
			base.PostRequest(domainName, DnsRecordType.CNAME, null);
			return base.Callback;
		}

		protected override bool ProcessData(DnsResult dnsResult, DnsAsyncRequest dnsAsyncRequest)
		{
			this.resolutionContext = new List<DnsCNameRecord>();
			DnsStatus dnsStatus = dnsResult.Status;
			DnsRecordList list = dnsResult.List;
			switch (dnsStatus)
			{
			case DnsStatus.Success:
			case DnsStatus.InfoTruncated:
				using (IEnumerator<DnsRecord> enumerator = list.EnumerateAnswers(DnsRecordType.CNAME).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						DnsRecord dnsRecord = enumerator.Current;
						DnsCNameRecord dnsCNameRecord = (DnsCNameRecord)dnsRecord;
						if (string.IsNullOrEmpty(dnsCNameRecord.Name) || DnsNativeMethods.DnsNameCompare(dnsCNameRecord.Name, this.queryFor))
						{
							if (this.resolutionContext.Count < this.maxCNameRecords)
							{
								this.resolutionContext.Add(dnsCNameRecord);
							}
							else
							{
								this.truncation = true;
							}
						}
					}
					goto IL_BF;
				}
				break;
			case DnsStatus.InfoNoRecords:
			case DnsStatus.InfoDomainNonexistent:
			case DnsStatus.ErrorInvalidData:
			case DnsStatus.ErrorExcessiveData:
				goto IL_BF;
			}
			dnsStatus = DnsStatus.ErrorRetry;
			IL_BF:
			if (dnsStatus == DnsStatus.Success && this.resolutionContext.Count == 0)
			{
				dnsStatus = DnsStatus.InfoNoRecords;
			}
			ExTraceGlobals.DNSTracer.Information<string>((long)this.GetHashCode(), "CNAME request complete [{0}].", Dns.DnsStatusToString(dnsStatus));
			DnsCNameRecord[] array = CNameRequest.EmptyCNameArray;
			if (dnsStatus == DnsStatus.Success || dnsStatus == DnsStatus.InfoTruncated)
			{
				array = this.resolutionContext.ToArray();
				if (this.truncation)
				{
					dnsStatus = DnsStatus.InfoTruncated;
				}
				else if (array.Length != 0)
				{
					dnsStatus = DnsStatus.Success;
				}
			}
			base.Callback.InvokeCallback(new Request.Result(dnsStatus, dnsResult.Server, array));
			return true;
		}

		public const int DefaultMaxCNameRecords = 256;

		private const DnsRecordType RecordType = DnsRecordType.CNAME;

		private static readonly DnsCNameRecord[] EmptyCNameArray = new DnsCNameRecord[0];

		private static readonly Request.Result InvalidDataRequestResult = new Request.Result(DnsStatus.ErrorInvalidData, IPAddress.None, CNameRequest.EmptyCNameArray);

		private string queryFor;

		private List<DnsCNameRecord> resolutionContext;

		private int maxCNameRecords = 256;

		private bool truncation;
	}
}
