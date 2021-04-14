using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Net
{
	internal sealed class SoaRequest : Request
	{
		public SoaRequest(DnsServerList list, DnsQueryOptions flags, Dns dnsInstance, AsyncCallback requestCallback, object stateObject) : base(list, flags, dnsInstance, requestCallback, stateObject)
		{
		}

		protected override Request.Result InvalidDataResult
		{
			get
			{
				return SoaRequest.InvalidDataRequestResult;
			}
		}

		public IAsyncResult Resolve(string query)
		{
			ExTraceGlobals.DNSTracer.Information<string>((long)this.GetHashCode(), "Soa request for zone {0}", query);
			this.queryFor = query;
			base.PostRequest(this.queryFor, DnsRecordType.SOA, null);
			return base.Callback;
		}

		protected override bool ProcessData(DnsResult dnsResult, DnsAsyncRequest dnsAsyncRequest)
		{
			DnsStatus dnsStatus = dnsResult.Status;
			DnsRecordList list = dnsResult.List;
			List<SoaRecord> list2 = null;
			switch (dnsStatus)
			{
			case DnsStatus.Success:
			case DnsStatus.InfoTruncated:
				list2 = new List<SoaRecord>(list.Count);
				foreach (DnsRecord dnsRecord in list.EnumerateAnswers(DnsRecordType.SOA))
				{
					DnsSoaRecord dnsSoaRecord = (DnsSoaRecord)dnsRecord;
					if (!string.IsNullOrEmpty(dnsSoaRecord.PrimaryServer))
					{
						SoaRecord item = new SoaRecord(dnsSoaRecord.PrimaryServer, dnsSoaRecord.Administrator, dnsSoaRecord.SerialNumber, dnsSoaRecord.Refresh, dnsSoaRecord.Retry, dnsSoaRecord.Expire, dnsSoaRecord.DefaultTimeToLive);
						list2.Add(item);
					}
				}
				if (list2.Count == 0 && dnsStatus != DnsStatus.InfoTruncated)
				{
					dnsStatus = DnsStatus.InfoNoRecords;
					goto IL_E0;
				}
				if (list2.Count > 0)
				{
					dnsStatus = DnsStatus.Success;
					goto IL_E0;
				}
				goto IL_E0;
			case DnsStatus.InfoNoRecords:
			case DnsStatus.InfoDomainNonexistent:
			case DnsStatus.ErrorInvalidData:
			case DnsStatus.ErrorExcessiveData:
				goto IL_E0;
			}
			dnsStatus = DnsStatus.ErrorRetry;
			IL_E0:
			ExTraceGlobals.DNSTracer.Information<string>((long)this.GetHashCode(), "Soa request complete [{0}].", Dns.DnsStatusToString(dnsStatus));
			base.Callback.InvokeCallback(new Request.Result(dnsStatus, dnsResult.Server, (dnsStatus == DnsStatus.Success) ? list2.ToArray() : SoaRequest.EmptySoaRecordArray));
			return true;
		}

		private const DnsRecordType RecordType = DnsRecordType.SOA;

		private static readonly SoaRecord[] EmptySoaRecordArray = new SoaRecord[0];

		private static readonly Request.Result InvalidDataRequestResult = new Request.Result(DnsStatus.ErrorInvalidData, IPAddress.None, SoaRequest.EmptySoaRecordArray);

		private string queryFor;
	}
}
