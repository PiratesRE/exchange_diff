using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Net
{
	internal abstract class Request : IDisposable
	{
		protected Request(DnsServerList list, DnsQueryOptions flags, Dns dnsInstance, AsyncCallback requestCallback, object stateObject)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (dnsInstance == null)
			{
				throw new ArgumentNullException("dnsInstance");
			}
			this.resultCallback = new LazyAsyncResult(this, stateObject, requestCallback);
			this.dnsInstance = dnsInstance;
			this.serverList = list;
			this.queryOptions = flags;
			this.clients = new DnsClient[list.Count];
		}

		public int MaxWireDataSize
		{
			get
			{
				return this.maxWireDataSize;
			}
			set
			{
				this.maxWireDataSize = value;
			}
		}

		public DateTime RequestTimeout
		{
			get
			{
				return this.requestTimeout;
			}
			set
			{
				this.requestTimeout = value;
			}
		}

		protected DnsQueryOptions Options
		{
			get
			{
				return this.queryOptions;
			}
			set
			{
				this.queryOptions = value;
			}
		}

		protected bool BypassCache
		{
			get
			{
				return (this.queryOptions & DnsQueryOptions.BypassCache) != DnsQueryOptions.None;
			}
		}

		protected bool AcceptTruncatedResponse
		{
			get
			{
				return (this.queryOptions & DnsQueryOptions.AcceptTruncatedResponse) != DnsQueryOptions.None;
			}
		}

		protected bool FailureTolerant
		{
			get
			{
				return (this.queryOptions & DnsQueryOptions.FailureTolerant) != DnsQueryOptions.None;
			}
		}

		protected DnsServerList ServerList
		{
			get
			{
				return this.serverList;
			}
		}

		protected LazyAsyncResult Callback
		{
			get
			{
				return this.resultCallback;
			}
		}

		protected Dns DnsInstance
		{
			get
			{
				return this.dnsInstance;
			}
		}

		internal int ClientCount
		{
			get
			{
				return this.clients.Length;
			}
		}

		protected static bool NextQueryType(AddressFamily requestedAddressFamily, int ipv4AddressCount, int ipv6AddressCount, DnsRecordType lastQueryType, out DnsRecordType nextQueryType)
		{
			nextQueryType = DnsRecordType.Unknown;
			switch (requestedAddressFamily)
			{
			case AddressFamily.Unspecified:
				if (lastQueryType == DnsRecordType.Unknown)
				{
					if (ipv6AddressCount == 0)
					{
						nextQueryType = DnsRecordType.AAAA;
						return true;
					}
					if (ipv4AddressCount == 0)
					{
						nextQueryType = DnsRecordType.A;
						return true;
					}
				}
				else if (lastQueryType == DnsRecordType.AAAA && ipv4AddressCount == 0)
				{
					nextQueryType = DnsRecordType.A;
					return true;
				}
				return false;
			case AddressFamily.Unix:
				break;
			case AddressFamily.InterNetwork:
				if (lastQueryType == DnsRecordType.Unknown && ipv4AddressCount == 0)
				{
					nextQueryType = DnsRecordType.A;
					return true;
				}
				return false;
			default:
				if (requestedAddressFamily == AddressFamily.InterNetworkV6)
				{
					if (lastQueryType == DnsRecordType.Unknown && ipv6AddressCount == 0)
					{
						nextQueryType = DnsRecordType.AAAA;
						return true;
					}
					return false;
				}
				break;
			}
			throw new NotSupportedException("Invalid address family " + requestedAddressFamily);
		}

		protected static ExEventLog EventLogger
		{
			get
			{
				if (Request.eventLogger == null)
				{
					Request.eventLogger = new ExEventLog(ExTraceGlobals.DNSTracer.Category, "MSExchange Common");
				}
				return Request.eventLogger;
			}
		}

		protected abstract Request.Result InvalidDataResult { get; }

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal bool CloseClient(DnsClient clientToClose)
		{
			DnsClient dnsClient = Interlocked.CompareExchange<DnsClient>(ref this.clients[clientToClose.Id], null, clientToClose);
			if (clientToClose == dnsClient)
			{
				dnsClient.Dispose();
				return true;
			}
			return false;
		}

		internal void CloseSocketAndResendRequest(DnsClient clientToClose)
		{
			if (this.CloseClient(clientToClose))
			{
				this.SendToServer(clientToClose.Id);
			}
		}

		internal void CheckForTimeout()
		{
			ExTraceGlobals.DNSTracer.TraceDebug((long)this.GetHashCode(), "CheckForTimeout");
			DnsAsyncRequest dnsAsyncRequest = this.dnsAsyncRequest;
			if (dnsAsyncRequest == null)
			{
				ExTraceGlobals.DNSTracer.TraceDebug((long)this.GetHashCode(), "CheckForTimeout, no request");
				return;
			}
			if (dnsAsyncRequest.Query.Type == DnsRecordType.AAAA && this.serverList.Cache != null)
			{
				this.serverList.Cache.AddAaaaQueryTimeOut(dnsAsyncRequest.Query.Question);
			}
			if (DateTime.UtcNow > this.requestTimeout)
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.TimeOutRequest), dnsAsyncRequest);
				return;
			}
			if (!this.ShouldRetrySubQuery(dnsAsyncRequest))
			{
				DnsLog.Log(this, "Subquery retry not required. {0}", new object[]
				{
					dnsAsyncRequest
				});
				return;
			}
			if (this.nextTimeout > DateTime.UtcNow)
			{
				DnsTimer.RegisterTimer(this, this.nextTimeout);
				ExTraceGlobals.DNSTracer.TraceDebug((long)this.GetHashCode(), "Timeout in future");
				return;
			}
			DnsLog.Log(this, "Retrying request due to timeout {0}", new object[]
			{
				this.dnsAsyncRequest
			});
			this.SendToNextServer();
		}

		protected virtual bool ShouldRetrySubQuery(DnsAsyncRequest asyncRequest)
		{
			return true;
		}

		protected static int FollowCNameChain(List<DnsCNameRecord> names, string key, List<DnsCNameRecord> list)
		{
			int num = 0;
			for (;;)
			{
				DnsCNameRecord dnsCNameRecord = new DnsCNameRecord(key);
				int num2 = names.BinarySearch(dnsCNameRecord, DnsCNameRecord.Comparer);
				if (num2 >= 0)
				{
					dnsCNameRecord = names[num2];
					if (num > names.Count || list.Count == Dns.MaxCnameRecords)
					{
						break;
					}
					list.Add(dnsCNameRecord);
					num++;
					key = dnsCNameRecord.Host;
				}
				if (num2 < 0)
				{
					return num;
				}
			}
			return -1;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				for (int i = 0; i < this.clients.Length; i++)
				{
					DnsClient dnsClient = Interlocked.Exchange<DnsClient>(ref this.clients[i], null);
					if (dnsClient != null)
					{
						dnsClient.Dispose();
					}
				}
			}
		}

		protected void PostRequest(string question, DnsRecordType dnsRecordType, object previousState = null)
		{
			question = Dns.TrimTrailingDot(question);
			if (!Dns.IsValidQuestion(question))
			{
				this.Callback.InvokeCallback(this.InvalidDataResult);
				return;
			}
			this.BeginResolve(question, dnsRecordType, new AsyncCallback(this.ResolveComplete), this, previousState);
		}

		protected abstract bool ProcessData(DnsResult result, DnsAsyncRequest dnsAsyncRequest);

		protected IPAddress[] FindEntries(string key, AddressFamily type, bool staticOnly, out TimeSpan timeToLive)
		{
			List<IPAddress> list = new List<IPAddress>();
			timeToLive = TimeSpan.Zero;
			DnsRecordType type2;
			switch (type)
			{
			case AddressFamily.Unspecified:
			{
				TimeSpan timeSpan;
				IPAddress[] array = this.FindEntries(key, AddressFamily.InterNetwork, staticOnly, out timeSpan);
				if (array != null && timeSpan == TimeSpan.MaxValue)
				{
					staticOnly = true;
				}
				TimeSpan timeSpan2;
				IPAddress[] array2 = this.FindEntries(key, AddressFamily.InterNetworkV6, staticOnly, out timeSpan2);
				if (array != null && array2 != null)
				{
					if (timeSpan2 == TimeSpan.MaxValue && timeSpan != TimeSpan.MaxValue)
					{
						array = null;
						timeToLive = timeSpan2;
					}
					else
					{
						timeToLive = ((timeSpan < timeSpan2) ? timeSpan : timeSpan2);
					}
				}
				else if (array != null)
				{
					timeToLive = timeSpan;
				}
				else if (array2 != null)
				{
					timeToLive = timeSpan2;
				}
				if (array != null)
				{
					list.InsertRange(0, array);
				}
				if (array2 != null)
				{
					list.InsertRange(0, array2);
				}
				if (list.Count != 0)
				{
					return list.ToArray();
				}
				return null;
			}
			case AddressFamily.Unix:
				break;
			case AddressFamily.InterNetwork:
				type2 = DnsRecordType.A;
				goto IL_F8;
			default:
				if (type == AddressFamily.InterNetworkV6)
				{
					type2 = DnsRecordType.AAAA;
					goto IL_F8;
				}
				break;
			}
			return null;
			IL_F8:
			DnsResult dnsResult = this.FindInCache(new DnsQuery(type2, key));
			if (dnsResult == null || (staticOnly && !dnsResult.IsPermanentEntry) || dnsResult.HasExpired || dnsResult.Status != DnsStatus.Success)
			{
				return null;
			}
			DnsRecordList list2 = dnsResult.List;
			if (list2 == null)
			{
				return null;
			}
			if (dnsResult.IsPermanentEntry)
			{
				timeToLive = TimeSpan.MaxValue;
			}
			else
			{
				timeToLive = dnsResult.Expires - DateTime.UtcNow;
			}
			foreach (DnsRecord dnsRecord in list2.EnumerateAnswers(type2))
			{
				if (key.Equals(dnsRecord.Name, StringComparison.OrdinalIgnoreCase))
				{
					if (dnsRecord.RecordType == DnsRecordType.A)
					{
						list.Add(((DnsARecord)dnsRecord).IPAddress);
					}
					else
					{
						list.Add(((DnsAAAARecord)dnsRecord).IPAddress);
					}
				}
			}
			if (list.Count != 0)
			{
				return list.ToArray();
			}
			return null;
		}

		protected DnsResult FindInCache(DnsQuery query)
		{
			return this.ServerList.Cache.Find(query);
		}

		protected static IPAddress[] FindLocalHostEntries(string key, AddressFamily type, out TimeSpan timeToLive)
		{
			timeToLive = TimeSpan.MaxValue;
			List<IPAddress> list = null;
			if (string.Equals(key, "localhost", StringComparison.OrdinalIgnoreCase))
			{
				list = new List<IPAddress>();
				if (type == AddressFamily.Unspecified || type == AddressFamily.InterNetwork)
				{
					list.Add(IPAddress.Loopback);
				}
				if (type == AddressFamily.Unspecified || type == AddressFamily.InterNetworkV6)
				{
					list.Add(IPAddress.IPv6Loopback);
				}
			}
			if (list != null && list.Count != 0)
			{
				return list.ToArray();
			}
			return null;
		}

		private IAsyncResult BeginResolve(string question, DnsRecordType dnsRecordType, AsyncCallback callback, object state, object previousState)
		{
			if (string.IsNullOrEmpty(question))
			{
				throw new ArgumentNullException("question");
			}
			ushort queryIdentifier = this.GenerateRandomQueryIdentifier();
			DnsAsyncRequest dnsAsyncRequest = new DnsAsyncRequest(question, dnsRecordType, queryIdentifier, this.queryOptions, this, state, callback, previousState);
			ExTraceGlobals.DNSTracer.TraceDebug<DnsQuery, ushort>((long)dnsAsyncRequest.GetHashCode(), "BeginResolve, {0}, (query id:{1})", dnsAsyncRequest.Query, dnsAsyncRequest.QueryIdentifier);
			DnsAsyncRequest dnsAsyncRequest2 = Interlocked.CompareExchange<DnsAsyncRequest>(ref this.dnsAsyncRequest, dnsAsyncRequest, null);
			if (dnsAsyncRequest2 != null)
			{
				throw new InvalidOperationException(NetException.ResolveInProgress);
			}
			if (!dnsAsyncRequest.IsValid)
			{
				dnsAsyncRequest.InvokeCallback(new DnsResult(DnsStatus.ErrorInvalidData, IPAddress.None, DnsResult.ErrorTimeToLive));
				return dnsAsyncRequest;
			}
			if (!this.BypassCache)
			{
				this.cachedResult = this.FindInCache(dnsAsyncRequest.Query);
				if (this.cachedResult == null && dnsAsyncRequest.Query.Type != DnsRecordType.CNAME)
				{
					this.cachedResult = this.FindInCache(new DnsQuery(DnsRecordType.CNAME, dnsAsyncRequest.Query.Question));
				}
				if (this.cachedResult != null && !this.cachedResult.HasExpired)
				{
					DnsLog.Log(this, "Cachehit for {0}. Result = {1}", new object[]
					{
						question,
						this.cachedResult
					});
					dnsAsyncRequest.InvokeCallback(this.cachedResult);
					return dnsAsyncRequest;
				}
			}
			if (this.ServerList.Addresses.Count == 0)
			{
				dnsAsyncRequest.InvokeCallback(new DnsResult(DnsStatus.ErrorNoDns, IPAddress.None, DnsResult.DefaultTimeToLive));
				return dnsAsyncRequest;
			}
			this.queriesSent++;
			if (this.queriesSent == 1)
			{
				this.lastServerQueried = (int)queryIdentifier;
			}
			this.SendToNextServer();
			return dnsAsyncRequest;
		}

		private void ResolveComplete(IAsyncResult asyncResult)
		{
			DnsAsyncRequest dnsAsyncRequest = (DnsAsyncRequest)asyncResult;
			DnsResult dnsResult = this.EndResolve(asyncResult);
			DnsLog.Log(this, "ResolveComplete. Result={0}; Request={1}", new object[]
			{
				dnsResult,
				dnsAsyncRequest
			});
			if (dnsAsyncRequest.IsValid && !this.BypassCache && !dnsResult.HasExpired && dnsResult.Status != DnsStatus.ErrorRetry && dnsResult.Status != DnsStatus.ErrorTimeout && dnsResult.Status != DnsStatus.InfoTruncated && dnsResult.Status != DnsStatus.ErrorSubQueryTimeout && dnsResult != this.cachedResult)
			{
				this.ServerList.Cache.Add(dnsAsyncRequest.Query, dnsResult);
			}
			this.cachedResult = null;
			this.ProcessData(dnsResult, dnsAsyncRequest);
		}

		private DnsResult EndResolve(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			DnsAsyncRequest dnsAsyncRequest = (DnsAsyncRequest)asyncResult;
			DnsAsyncRequest dnsAsyncRequest2 = Interlocked.CompareExchange<DnsAsyncRequest>(ref this.dnsAsyncRequest, null, dnsAsyncRequest);
			if (dnsAsyncRequest != dnsAsyncRequest2)
			{
				throw new InvalidOperationException(NetException.IAsyncResultMismatch);
			}
			if (dnsAsyncRequest.EndCalled)
			{
				throw new InvalidOperationException(NetException.EndAlreadyCalled);
			}
			dnsAsyncRequest.EndCalled = true;
			DnsResult dnsResult = (DnsResult)dnsAsyncRequest.Result;
			ExTraceGlobals.DNSTracer.TraceDebug<DnsQuery, ushort, DnsStatus>((long)dnsAsyncRequest.GetHashCode(), "EndResolve, {0}, (query id:{1}), status {2}", dnsAsyncRequest.Query, dnsAsyncRequest.QueryIdentifier, dnsResult.Status);
			return dnsResult;
		}

		private ushort GenerateRandomQueryIdentifier()
		{
			ushort num;
			do
			{
				lock (Request.rand)
				{
					num = (ushort)Request.rand.Next(0, 65535);
				}
			}
			while (this.usedQueryIds.Contains(num));
			this.usedQueryIds.Add(num);
			return num;
		}

		private void SendToNextServer()
		{
			for (int i = 0; i < this.clients.Length; i++)
			{
				int server = Interlocked.Increment(ref this.lastServerQueried) % this.clients.Length;
				if (this.SendToServer(server))
				{
					return;
				}
			}
			ExTraceGlobals.DNSTracer.TraceDebug((long)this.GetHashCode(), "SendToNextServer did not send request.");
		}

		private bool SendToServer(int server)
		{
			this.SetNextTimeout();
			DnsAsyncRequest dnsAsyncRequest = this.dnsAsyncRequest;
			if (dnsAsyncRequest == null || !dnsAsyncRequest.IsValid)
			{
				return false;
			}
			DnsClient dnsClient = this.clients[server];
			if (dnsClient == null)
			{
				dnsClient = new DnsClient(server, this.serverList.Addresses[server]);
				DnsClient dnsClient2 = Interlocked.CompareExchange<DnsClient>(ref this.clients[server], dnsClient, null);
				if (dnsClient2 != null)
				{
					dnsClient.Dispose();
					return false;
				}
			}
			if (!dnsAsyncRequest.CanQueryClient(server))
			{
				return false;
			}
			ExTraceGlobals.DNSTracer.TraceDebug<int, DnsQuery, ushort>((long)dnsAsyncRequest.GetHashCode(), "SendToServer({0}), {1}, (query id:{2})", server, dnsAsyncRequest.Query, dnsAsyncRequest.QueryIdentifier);
			DnsLog.Log(this, "SendToServer {0}({1}), {2}, (query id:{3})", new object[]
			{
				this.serverList.Addresses[server],
				server,
				dnsAsyncRequest.Query,
				dnsAsyncRequest.QueryIdentifier
			});
			return dnsClient.Send(dnsAsyncRequest);
		}

		private void SetNextTimeout()
		{
			DateTime dateTime = DateTime.UtcNow + this.DnsInstance.QueryRetryInterval;
			bool flag = this.nextTimeout <= dateTime;
			this.nextTimeout = dateTime;
			DnsAsyncRequest dnsAsyncRequest = this.dnsAsyncRequest;
			if (dnsAsyncRequest != null)
			{
				ExTraceGlobals.DNSTracer.TraceDebug((long)dnsAsyncRequest.GetHashCode(), "SetNextTimeout, {0}, (query id:{1}), Next timeout {2} register:{3} valid:{4}", new object[]
				{
					dnsAsyncRequest.Query,
					dnsAsyncRequest.QueryIdentifier,
					dateTime,
					flag,
					dnsAsyncRequest.IsValid
				});
			}
			if (flag)
			{
				DnsTimer.RegisterTimer(this, dateTime);
			}
		}

		private void TimeOutRequest(object state)
		{
			DnsAsyncRequest dnsAsyncRequest = (DnsAsyncRequest)state;
			DnsLog.Log(this, "Query timedout. {0}", new object[]
			{
				dnsAsyncRequest
			});
			ExTraceGlobals.DNSTracer.TraceDebug<DnsQuery, ushort>((long)dnsAsyncRequest.GetHashCode(), "TimedOut, {0}, (query id:{1})", dnsAsyncRequest.Query, dnsAsyncRequest.QueryIdentifier);
			dnsAsyncRequest.InvokeCallback(new DnsResult(DnsStatus.ErrorTimeout, IPAddress.Any, DnsResult.ErrorTimeToLive));
		}

		private static Random rand = new Random((int)DateTime.UtcNow.Ticks);

		private int maxWireDataSize = 2048;

		private LazyAsyncResult resultCallback;

		private DnsResult cachedResult;

		private DnsServerList serverList;

		private DnsQueryOptions queryOptions;

		private DnsClient[] clients;

		private int lastServerQueried;

		private DnsAsyncRequest dnsAsyncRequest;

		private List<ushort> usedQueryIds = new List<ushort>(10);

		private DateTime requestTimeout;

		private DateTime nextTimeout;

		private int queriesSent;

		private Dns dnsInstance;

		private static ExEventLog eventLogger;

		internal class Result
		{
			internal Result(DnsStatus status, IPAddress server, object data)
			{
				this.status = status;
				this.server = server;
				this.data = data;
			}

			internal DnsStatus Status
			{
				get
				{
					return this.status;
				}
			}

			internal IPAddress Server
			{
				get
				{
					return this.server;
				}
			}

			internal object Data
			{
				get
				{
					return this.data;
				}
			}

			private DnsStatus status;

			private IPAddress server;

			private object data;
		}

		internal class HostAddressList : List<IPAddress>
		{
			public int Ipv4AddressCount
			{
				get
				{
					return this.ipv4AddressCount;
				}
			}

			public int Ipv6AddressCount
			{
				get
				{
					return this.ipv6AddressCount;
				}
			}

			private void ProcessAddAddress(IPAddress address)
			{
				if (address.AddressFamily == AddressFamily.InterNetwork)
				{
					this.ipv4AddressCount++;
					return;
				}
				if (address.AddressFamily == AddressFamily.InterNetworkV6)
				{
					this.ipv6AddressCount++;
					return;
				}
				throw new NotSupportedException("Unexpected address type");
			}

			public new void Insert(int index, IPAddress address)
			{
				base.Insert(index, address);
				this.ProcessAddAddress(address);
			}

			public new void InsertRange(int index, IEnumerable<IPAddress> collection)
			{
				base.InsertRange(index, collection);
				foreach (IPAddress address in collection)
				{
					this.ProcessAddAddress(address);
				}
			}

			public new void AddRange(IEnumerable<IPAddress> collection)
			{
				base.AddRange(collection);
				foreach (IPAddress address in collection)
				{
					this.ProcessAddAddress(address);
				}
			}

			public new void Add(IPAddress address)
			{
				base.Add(address);
				this.ProcessAddAddress(address);
			}

			private int ipv4AddressCount;

			private int ipv6AddressCount;
		}
	}
}
