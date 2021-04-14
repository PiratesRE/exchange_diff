using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Net
{
	internal sealed class SrvRequest : Request
	{
		public SrvRequest(DnsServerList list, DnsQueryOptions flags, Dns dnsInstance, AsyncCallback requestCallback, object stateObject) : base(list, flags, dnsInstance, requestCallback, stateObject)
		{
		}

		protected override Request.Result InvalidDataResult
		{
			get
			{
				return SrvRequest.InvalidDataRequestResult;
			}
		}

		public IAsyncResult Resolve(string query)
		{
			ExTraceGlobals.DNSTracer.Information<string>((long)this.GetHashCode(), "SRV request for service {0}", query);
			this.queryFor = query;
			base.PostRequest(this.queryFor, DnsRecordType.SRV, null);
			return base.Callback;
		}

		protected override bool ProcessData(DnsResult dnsResult, DnsAsyncRequest dnsAsyncRequest)
		{
			DnsStatus dnsStatus = dnsResult.Status;
			DnsRecordList list = dnsResult.List;
			List<SrvRecord> list2 = null;
			switch (dnsStatus)
			{
			case DnsStatus.Success:
			case DnsStatus.InfoTruncated:
				list2 = new List<SrvRecord>(list.Count);
				foreach (DnsRecord dnsRecord in list.EnumerateAnswers(DnsRecordType.SRV))
				{
					DnsSrvRecord dnsSrvRecord = (DnsSrvRecord)dnsRecord;
					if (!string.IsNullOrEmpty(dnsSrvRecord.NameTarget) && (string.IsNullOrEmpty(dnsSrvRecord.Name) || DnsNativeMethods.DnsNameCompare(dnsSrvRecord.Name, this.queryFor)))
					{
						SrvRecord item = new SrvRecord(dnsSrvRecord.Name, dnsSrvRecord.NameTarget, dnsSrvRecord.Priority, dnsSrvRecord.Weight, dnsSrvRecord.Port);
						list2.Add(item);
					}
				}
				if (list2.Count == 0 && dnsStatus != DnsStatus.InfoTruncated)
				{
					dnsStatus = DnsStatus.InfoNoRecords;
					goto IL_F5;
				}
				if (list2.Count > 0)
				{
					dnsStatus = DnsStatus.Success;
					goto IL_F5;
				}
				goto IL_F5;
			case DnsStatus.InfoNoRecords:
			case DnsStatus.InfoDomainNonexistent:
			case DnsStatus.ErrorInvalidData:
			case DnsStatus.ErrorExcessiveData:
				goto IL_F5;
			}
			dnsStatus = DnsStatus.ErrorRetry;
			IL_F5:
			ExTraceGlobals.DNSTracer.Information<string>((long)this.GetHashCode(), "SRV request complete [{0}].", Dns.DnsStatusToString(dnsStatus));
			base.Callback.InvokeCallback(new Request.Result(dnsStatus, dnsResult.Server, (dnsStatus == DnsStatus.Success) ? list2.ToArray() : SrvRequest.EmptySrvRecordArray));
			return true;
		}

		private const DnsRecordType RecordType = DnsRecordType.SRV;

		private static readonly SrvRecord[] EmptySrvRecordArray = new SrvRecord[0];

		private static readonly Request.Result InvalidDataRequestResult = new Request.Result(DnsStatus.ErrorInvalidData, IPAddress.None, SrvRequest.EmptySrvRecordArray);

		private string queryFor;
	}
}
