using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Net
{
	internal sealed class NsRequest : Request
	{
		public NsRequest(DnsServerList list, DnsQueryOptions flags, Dns dnsInstance, AsyncCallback requestCallback, object stateObject) : base(list, flags, dnsInstance, requestCallback, stateObject)
		{
		}

		protected override Request.Result InvalidDataResult
		{
			get
			{
				return NsRequest.InvalidDataRequestResult;
			}
		}

		public IAsyncResult Resolve(string domainName)
		{
			ExTraceGlobals.DNSTracer.Information<string>((long)this.GetHashCode(), "NS request for server {0}", domainName);
			this.queryFor = domainName;
			base.PostRequest(domainName, DnsRecordType.NS, null);
			return base.Callback;
		}

		protected override bool ProcessData(DnsResult dnsResult, DnsAsyncRequest dnsAsyncRequest)
		{
			List<DnsNsRecord> list = null;
			DnsStatus dnsStatus = dnsResult.Status;
			DnsRecordList list2 = dnsResult.List;
			switch (dnsStatus)
			{
			case DnsStatus.Success:
			case DnsStatus.InfoTruncated:
				list = new List<DnsNsRecord>(list2.Count);
				using (IEnumerator<DnsRecord> enumerator = list2.EnumerateAnswers(DnsRecordType.NS).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						DnsRecord dnsRecord = enumerator.Current;
						DnsNsRecord dnsNsRecord = (DnsNsRecord)dnsRecord;
						if (string.IsNullOrEmpty(dnsNsRecord.Name) || DnsNativeMethods.DnsNameCompare(dnsNsRecord.Name, this.queryFor))
						{
							list.Add(dnsNsRecord);
						}
					}
					goto IL_A1;
				}
				break;
			case DnsStatus.InfoNoRecords:
			case DnsStatus.InfoDomainNonexistent:
			case DnsStatus.ErrorInvalidData:
			case DnsStatus.ErrorExcessiveData:
				goto IL_A1;
			}
			dnsStatus = DnsStatus.ErrorRetry;
			IL_A1:
			if (dnsStatus == DnsStatus.Success && list.Count == 0)
			{
				dnsStatus = DnsStatus.InfoNoRecords;
			}
			ExTraceGlobals.DNSTracer.Information<string>((long)this.GetHashCode(), "NS request complete [{0}].", Dns.DnsStatusToString(dnsStatus));
			DnsNsRecord[] array = NsRequest.EmptyNsArray;
			if (dnsStatus == DnsStatus.Success || dnsStatus == DnsStatus.InfoTruncated)
			{
				array = list.ToArray();
				if (array.Length != 0)
				{
					dnsStatus = DnsStatus.Success;
				}
			}
			base.Callback.InvokeCallback(new Request.Result(dnsStatus, dnsResult.Server, array));
			return true;
		}

		private const DnsRecordType RecordType = DnsRecordType.NS;

		private static readonly DnsNsRecord[] EmptyNsArray = new DnsNsRecord[0];

		private static readonly Request.Result InvalidDataRequestResult = new Request.Result(DnsStatus.ErrorInvalidData, IPAddress.None, NsRequest.EmptyNsArray);

		private string queryFor;
	}
}
