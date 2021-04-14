using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Net
{
	internal sealed class AddressRequest : Request
	{
		public AddressRequest(DnsServerList list, DnsQueryOptions flags, AddressFamily requestedAddressFamily, Dns dnsInstance, AsyncCallback requestCallback, object stateObject) : base(list, flags, dnsInstance, requestCallback, stateObject)
		{
			this.requestedAddressFamily = requestedAddressFamily;
		}

		public AddressRequest(DnsServerList list, DnsQueryOptions flags, Dns dnsInstance, AsyncCallback requestCallback, object stateObject) : base(list, flags, dnsInstance, requestCallback, stateObject)
		{
			this.requestedAddressFamily = dnsInstance.DefaultAddressFamily;
		}

		private bool SetNextQueryType()
		{
			if (this.staticRecords)
			{
				return false;
			}
			DnsRecordType dnsRecordType;
			bool flag = Request.NextQueryType(this.requestedAddressFamily, this.addressList.Ipv4AddressCount, this.addressList.Ipv6AddressCount, this.currentType, out dnsRecordType);
			if (flag)
			{
				this.currentType = dnsRecordType;
			}
			return flag;
		}

		private string NextLink
		{
			get
			{
				if (this.canonicalNames != null && this.canonicalNames.Count != 0)
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
				return AddressRequest.InvalidDataRequestResult;
			}
		}

		private int CacheOrInternalLookup(string name)
		{
			TimeSpan zero = TimeSpan.Zero;
			IPAddress[] array = null;
			if (!base.BypassCache)
			{
				array = base.FindEntries(name, this.requestedAddressFamily, false, out zero);
			}
			if (array == null)
			{
				array = Request.FindLocalHostEntries(name, this.requestedAddressFamily, out zero);
			}
			if (array != null)
			{
				this.addressList.AddRange(array);
				if (zero == TimeSpan.MaxValue)
				{
					this.staticRecords = true;
				}
				return array.Length;
			}
			return 0;
		}

		public IAsyncResult Resolve(string domainName)
		{
			ExTraceGlobals.DNSTracer.Information<string, DnsRecordType>((long)this.GetHashCode(), "Address request for server {0}, record type {1}", domainName, this.currentType);
			this.domain = domainName;
			this.queryFor = domainName;
			if (this.CacheOrInternalLookup(this.queryFor) > 0)
			{
				ExTraceGlobals.DNSTracer.Information<string>((long)this.GetHashCode(), "Found cached IPAddresses for {0}", this.queryFor);
			}
			if (!this.SetNextQueryType())
			{
				base.Callback.InvokeCallback(new Request.Result(DnsStatus.Success, IPAddress.None, this.addressList.ToArray()));
			}
			else
			{
				base.PostRequest(domainName, this.currentType, null);
			}
			return base.Callback;
		}

		protected override bool ProcessData(DnsResult dnsResult, DnsAsyncRequest dnsAsyncRequest)
		{
			DnsStatus dnsStatus = dnsResult.Status;
			DnsRecordList list = dnsResult.List;
			bool flag = false;
			switch (dnsStatus)
			{
			case DnsStatus.Success:
			case DnsStatus.InfoTruncated:
				foreach (DnsRecord dnsRecord in list.EnumerateAnswers(this.currentType))
				{
					DnsAddressRecord dnsAddressRecord = (DnsAddressRecord)dnsRecord;
					if (this.queryFor.Equals(dnsAddressRecord.Name, StringComparison.OrdinalIgnoreCase))
					{
						this.addressList.Add(dnsAddressRecord.IPAddress);
					}
				}
				if (this.addressList.Count == 0)
				{
					dnsStatus = this.FollowChain(list);
					DnsStatus dnsStatus2 = dnsStatus;
					if (dnsStatus2 != DnsStatus.Success && dnsStatus2 == DnsStatus.Pending)
					{
						return false;
					}
				}
				if (dnsStatus == DnsStatus.Success)
				{
					flag = this.SetNextQueryType();
					goto IL_E1;
				}
				goto IL_E1;
			case DnsStatus.InfoNoRecords:
			case DnsStatus.ErrorInvalidData:
			case DnsStatus.ErrorExcessiveData:
				flag = this.SetNextQueryType();
				goto IL_E1;
			case DnsStatus.InfoDomainNonexistent:
				goto IL_E1;
			}
			base.Callback.ErrorCode = (int)dnsStatus;
			dnsStatus = DnsStatus.ErrorRetry;
			IL_E1:
			if (flag)
			{
				if (++this.subQueryCount < Dns.MaxSubQueries)
				{
					ExTraceGlobals.DNSTracer.Information<int, string>((long)this.GetHashCode(), "{0} Posting a query to find Alias for {1}", this.subQueryCount, this.queryFor);
					base.PostRequest(this.queryFor, this.currentType, null);
					return false;
				}
				if (this.addressList.Count > 0)
				{
					dnsStatus = DnsStatus.Success;
				}
				else
				{
					dnsStatus = DnsStatus.InfoTruncated;
				}
			}
			if (dnsStatus != DnsStatus.Success && this.addressList.Count > 0)
			{
				ExTraceGlobals.DNSTracer.Information((long)this.GetHashCode(), "Overwrite status with DnsStatus.Success because we have valid address in the list already.");
				dnsStatus = DnsStatus.Success;
			}
			ExTraceGlobals.DNSTracer.Information<string>((long)this.GetHashCode(), "Address request complete [{0}].", Dns.DnsStatusToString(dnsStatus));
			base.Callback.InvokeCallback(new Request.Result(dnsStatus, dnsResult.Server, (dnsStatus == DnsStatus.Success) ? this.addressList.ToArray() : AddressRequest.EmptyIPAddressArray));
			return true;
		}

		private DnsStatus FollowChain(DnsRecordList records)
		{
			List<DnsCNameRecord> list = records.ExtractRecords<DnsCNameRecord>(DnsRecordType.CNAME, DnsCNameRecord.Comparer);
			if (list.Count == 0)
			{
				ExTraceGlobals.DNSTracer.Information((long)this.GetHashCode(), "No CNAME records found in response");
				return DnsStatus.InfoNoRecords;
			}
			if (this.canonicalNames == null)
			{
				this.canonicalNames = new List<DnsCNameRecord>();
			}
			int num = Request.FollowCNameChain(list, this.NextLink, this.canonicalNames);
			if (num == -1)
			{
				ExTraceGlobals.DNSTracer.Information<string>((long)this.GetHashCode(), "CNAME chain too long. End of chain is {0}", this.NextLink);
				return DnsStatus.InfoNoRecords;
			}
			if (num == 0)
			{
				ExTraceGlobals.DNSTracer.Information<string>((long)this.GetHashCode(), "No CNAME link found for {0}", this.NextLink);
				return DnsStatus.InfoNoRecords;
			}
			string nextLink = this.NextLink;
			ExTraceGlobals.DNSTracer.Information<string, string>((long)this.GetHashCode(), "CNAME chain for {0} leads to {1}", this.queryFor, nextLink);
			if (this.CacheOrInternalLookup(nextLink) > 0)
			{
				ExTraceGlobals.DNSTracer.Information<string>((long)this.GetHashCode(), "Found cached IPAddresses for {0}", nextLink);
			}
			if (this.addressList.Count > 0)
			{
				return DnsStatus.Success;
			}
			foreach (DnsRecord dnsRecord in records.EnumerateAnswers(this.currentType))
			{
				DnsAddressRecord dnsAddressRecord = (DnsAddressRecord)dnsRecord;
				if (nextLink.Equals(dnsAddressRecord.Name, StringComparison.OrdinalIgnoreCase))
				{
					this.addressList.Add(dnsAddressRecord.IPAddress);
				}
			}
			if (this.addressList.Count > 0)
			{
				ExTraceGlobals.DNSTracer.Information<string>((long)this.GetHashCode(), "Found IPAddresses for {0}", nextLink);
				return DnsStatus.Success;
			}
			if (++this.subQueryCount >= Dns.MaxSubQueries)
			{
				return DnsStatus.InfoTruncated;
			}
			this.queryFor = nextLink;
			ExTraceGlobals.DNSTracer.Information<int, string, DnsRecordType>((long)this.GetHashCode(), "{0} Posting a query to find Address records for {1}, record type {2}", this.subQueryCount, this.queryFor, this.currentType);
			base.PostRequest(this.queryFor, this.currentType, null);
			return DnsStatus.Pending;
		}

		private static readonly IPAddress[] EmptyIPAddressArray = new IPAddress[0];

		private static readonly Request.Result InvalidDataRequestResult = new Request.Result(DnsStatus.ErrorInvalidData, IPAddress.None, AddressRequest.EmptyIPAddressArray);

		private string domain;

		private string queryFor;

		private List<DnsCNameRecord> canonicalNames;

		private int subQueryCount;

		private AddressFamily requestedAddressFamily;

		private DnsRecordType currentType;

		private Request.HostAddressList addressList = new Request.HostAddressList();

		private bool staticRecords;
	}
}
