using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Net
{
	internal sealed class TxtRequest : Request
	{
		public TxtRequest(DnsServerList list, DnsQueryOptions flags, Dns dnsInstance, AsyncCallback requestCallback, object stateObject) : base(list, flags, dnsInstance, requestCallback, stateObject)
		{
		}

		public int MaxTextRecords
		{
			get
			{
				return this.maxTextRecords;
			}
			set
			{
				this.maxTextRecords = value;
			}
		}

		private string NextLink
		{
			get
			{
				if (this.canonicalNames.Count != 0)
				{
					return this.canonicalNames[this.canonicalNames.Count - 1].Host;
				}
				return this.domain;
			}
		}

		protected override Request.Result InvalidDataResult
		{
			get
			{
				return TxtRequest.InvalidDataRequestResult;
			}
		}

		public IAsyncResult Resolve(string domainName)
		{
			ExTraceGlobals.DNSTracer.Information<string>((long)this.GetHashCode(), "TXT request for server {0}", domainName);
			this.domain = domainName;
			this.queryFor = domainName;
			base.PostRequest(domainName, DnsRecordType.TXT, null);
			return base.Callback;
		}

		protected override bool ProcessData(DnsResult dnsResult, DnsAsyncRequest dnsAsyncRequest)
		{
			this.resolutionContext = new List<string>();
			DnsStatus dnsStatus = dnsResult.Status;
			DnsRecordList list = dnsResult.List;
			switch (dnsStatus)
			{
			case DnsStatus.Success:
			case DnsStatus.InfoTruncated:
			{
				foreach (DnsRecord dnsRecord in list.EnumerateAnswers(DnsRecordType.TXT))
				{
					DnsTxtRecord dnsTxtRecord = (DnsTxtRecord)dnsRecord;
					if (string.IsNullOrEmpty(dnsTxtRecord.Name) || DnsNativeMethods.DnsNameCompare(dnsTxtRecord.Name, this.queryFor))
					{
						DnsStatus status = dnsTxtRecord.Status;
						if (status != DnsStatus.Success)
						{
							dnsStatus = status;
							break;
						}
						if (this.resolutionContext.Count < this.maxTextRecords)
						{
							string text = dnsTxtRecord.Text;
							this.resolutionContext.Add(text);
						}
						else
						{
							this.truncation = true;
						}
					}
				}
				if (this.resolutionContext.Count != 0)
				{
					goto IL_FF;
				}
				dnsStatus = this.FollowChain(list);
				DnsStatus dnsStatus2 = dnsStatus;
				if (dnsStatus2 != DnsStatus.Success && dnsStatus2 == DnsStatus.Pending)
				{
					return false;
				}
				goto IL_FF;
			}
			case DnsStatus.InfoNoRecords:
			case DnsStatus.InfoDomainNonexistent:
			case DnsStatus.ErrorInvalidData:
			case DnsStatus.ErrorExcessiveData:
				goto IL_FF;
			}
			dnsStatus = DnsStatus.ErrorRetry;
			IL_FF:
			if (dnsStatus == DnsStatus.Success && this.resolutionContext.Count == 0)
			{
				dnsStatus = DnsStatus.InfoNoRecords;
			}
			ExTraceGlobals.DNSTracer.Information<string>((long)this.GetHashCode(), "TXT request complete [{0}].", Dns.DnsStatusToString(dnsStatus));
			string[] array = TxtRequest.EmptyStringArray;
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

		private DnsStatus FollowChain(DnsRecordList records)
		{
			List<DnsCNameRecord> list = records.ExtractRecords<DnsCNameRecord>(DnsRecordType.CNAME, DnsCNameRecord.Comparer);
			if (list.Count == 0)
			{
				return DnsStatus.InfoNoRecords;
			}
			if (this.canonicalNames == null)
			{
				this.canonicalNames = new List<DnsCNameRecord>();
			}
			int num = Request.FollowCNameChain(list, this.NextLink, this.canonicalNames);
			if (num == -1)
			{
				return DnsStatus.InfoNoRecords;
			}
			if (num == 0)
			{
				return DnsStatus.InfoNoRecords;
			}
			string nextLink = this.NextLink;
			foreach (DnsRecord dnsRecord in records.EnumerateAnswers(DnsRecordType.TXT))
			{
				DnsTxtRecord dnsTxtRecord = (DnsTxtRecord)dnsRecord;
				if (nextLink.Equals(dnsTxtRecord.Name, StringComparison.OrdinalIgnoreCase))
				{
					if (this.resolutionContext.Count < this.maxTextRecords)
					{
						this.resolutionContext.Add(dnsTxtRecord.Text);
					}
					else
					{
						this.truncation = true;
					}
				}
			}
			if (this.resolutionContext.Count > 0)
			{
				return DnsStatus.Success;
			}
			if (++this.subQueryCount >= Dns.MaxSubQueries)
			{
				return DnsStatus.InfoTruncated;
			}
			this.queryFor = nextLink;
			ExTraceGlobals.DNSTracer.Information<int, string>((long)this.GetHashCode(), "{0} Posting a query to find TXT records for {1}", this.subQueryCount, this.queryFor);
			base.PostRequest(this.queryFor, DnsRecordType.TXT, null);
			return DnsStatus.Pending;
		}

		public const int DefaultMaxTextRecords = 256;

		private const DnsRecordType RecordType = DnsRecordType.TXT;

		private static readonly string[] EmptyStringArray = new string[0];

		private static readonly Request.Result InvalidDataRequestResult = new Request.Result(DnsStatus.ErrorInvalidData, IPAddress.None, TxtRequest.EmptyStringArray);

		private string domain;

		private string queryFor;

		private List<string> resolutionContext;

		private int maxTextRecords = 256;

		private bool truncation;

		private List<DnsCNameRecord> canonicalNames;

		private int subQueryCount;
	}
}
