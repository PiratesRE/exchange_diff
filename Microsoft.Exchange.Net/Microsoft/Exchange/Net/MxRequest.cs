using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Net
{
	internal class MxRequest : Request
	{
		public MxRequest(DnsServerList list, DnsQueryOptions flags, IEnumerable<IPAddress> addresses, int retryCount, Dns dnsInstance, AsyncCallback requestCallback, object stateObject) : base(list, flags, dnsInstance, requestCallback, stateObject)
		{
			this.localAddresses = addresses;
			this.dataProcessor = new MxRequest.DataProcessor(this.ProcessMxResponse);
			this.requestedAddressFamily = dnsInstance.DefaultAddressFamily;
		}

		public MxRequest(DnsServerList list, DnsQueryOptions flags, IEnumerable<IPAddress> addresses, int retryCount, AddressFamily requestedAddressFamily, bool implicitSearch, Dns dnsInstance, AsyncCallback requestCallback, object stateObject) : this(list, flags, addresses, retryCount, dnsInstance, requestCallback, stateObject)
		{
			if (requestedAddressFamily != AddressFamily.Unspecified && requestedAddressFamily != AddressFamily.InterNetwork && requestedAddressFamily != AddressFamily.InterNetworkV6)
			{
				throw new ArgumentException(NetException.UnsupportedFilter, "requestedAddressFamily");
			}
			this.implicitSearch = implicitSearch;
			this.requestedAddressFamily = requestedAddressFamily;
		}

		protected override Request.Result InvalidDataResult
		{
			get
			{
				return MxRequest.InvalidDataRequestResult;
			}
		}

		public IAsyncResult Resolve(string domainName)
		{
			ExTraceGlobals.DNSTracer.Information<string>((long)this.GetHashCode(), "MX request for server {0}", domainName);
			if (domainName[domainName.Length - 1] == '.')
			{
				domainName = domainName.Substring(0, domainName.Length - 1);
			}
			this.queryFor = (this.domain = domainName);
			base.PostRequest(domainName, this.currentType, null);
			return base.Callback;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		protected override bool ProcessData(DnsResult dnsResult, DnsAsyncRequest dnsAsyncRequest)
		{
			return this.dataProcessor(dnsResult, dnsAsyncRequest);
		}

		protected override bool ShouldRetrySubQuery(DnsAsyncRequest asyncRequest)
		{
			if (asyncRequest.Query.Type != DnsRecordType.MX)
			{
				bool flag = false;
				if (base.FailureTolerant)
				{
					asyncRequest.RetryCount++;
					if (asyncRequest.RetryCount > this.maxRetriesPerAddressRequest)
					{
						flag = true;
					}
				}
				if (flag || this.IsFrequentQuadAQueryTimeOut(asyncRequest.Query))
				{
					ThreadPool.QueueUserWorkItem(new WaitCallback(this.TimeOutAddressRequest), asyncRequest);
					return false;
				}
			}
			return true;
		}

		private bool IsFrequentQuadAQueryTimeOut(DnsQuery query)
		{
			return query.Type == DnsRecordType.AAAA && base.ServerList.Cache != null && base.ServerList.Cache.GetAaaaTimeOutEntry(query.Question) > 20;
		}

		private void TimeOutAddressRequest(object state)
		{
			DnsAsyncRequest dnsAsyncRequest = (DnsAsyncRequest)state;
			ExTraceGlobals.DNSTracer.TraceDebug<DnsQuery, ushort>((long)dnsAsyncRequest.GetHashCode(), "TimedOut, {0}, (query id:{1})", dnsAsyncRequest.Query, dnsAsyncRequest.QueryIdentifier);
			DnsLog.Log(this, "Timing out subquery {0}", new object[]
			{
				dnsAsyncRequest
			});
			dnsAsyncRequest.InvokeCallback(new DnsResult(DnsStatus.ErrorSubQueryTimeout, IPAddress.Any, DnsResult.ErrorTimeToLive));
		}

		private static int GenerateRandomNumber()
		{
			int result;
			lock (MxRequest.random)
			{
				result = MxRequest.random.Next();
			}
			return result;
		}

		private void AnalyzeAResponse(DnsRecordList records)
		{
			MxRequest.MxRecord mxRecord = this.mxRecords[this.mxIndex];
			int num = this.FindAliasesFromList(mxRecord, records);
			if (num < 0)
			{
				this.seenLocal = true;
				this.mxRecords.Prune(mxRecord.Preference);
				return;
			}
			if (mxRecord.Addresses.Count > 0)
			{
				return;
			}
			if (this.IsSecondAddressQuery)
			{
				return;
			}
			List<DnsCNameRecord> list = records.ExtractRecords<DnsCNameRecord>(DnsRecordType.CNAME, DnsCNameRecord.Comparer);
			if (list.Count == 0)
			{
				return;
			}
			int num2 = mxRecord.BuildChain(list);
			if (num2 <= 0)
			{
				return;
			}
			num = this.FindAliases(mxRecord, records);
			if (num < 0)
			{
				this.seenLocal = true;
				this.mxRecords.Prune(mxRecord.Preference);
				return;
			}
			this.currentType = DnsRecordType.Unknown;
		}

		private bool IsSecondAddressQuery
		{
			get
			{
				return this.requestedAddressFamily == AddressFamily.Unspecified && this.currentType == DnsRecordType.A;
			}
		}

		private bool ProcessAddressResponse(DnsResult results, DnsAsyncRequest originalRequest)
		{
			DnsStatus dnsStatus = results.Status;
			DnsRecordList list = results.List;
			bool flag = false;
			MxRequest.MxRecord mxRecord = this.mxRecords[this.mxIndex];
			MxRequest.FaultTolerantQueryState previousState = null;
			switch (dnsStatus)
			{
			case DnsStatus.Success:
				break;
			case DnsStatus.InfoNoRecords:
			case DnsStatus.ErrorInvalidData:
				flag = this.SetNextQueryType(mxRecord, this.currentType);
				if (!flag && (mxRecord.Addresses.Count > 0 || !this.ephemeralMx))
				{
					dnsStatus = DnsStatus.Success;
					goto IL_1BE;
				}
				goto IL_1BE;
			case DnsStatus.InfoDomainNonexistent:
				dnsStatus = DnsStatus.Success;
				goto IL_1BE;
			case DnsStatus.InfoMxLoopback:
			case DnsStatus.ErrorExcessiveData:
			case DnsStatus.ErrorRetry:
			case DnsStatus.ErrorDisconnectException:
			case DnsStatus.Pending:
			case DnsStatus.ConfigChanged:
				goto IL_1B0;
			case DnsStatus.InfoTruncated:
				if (!base.FailureTolerant)
				{
					goto IL_1B0;
				}
				break;
			case DnsStatus.ErrorTimeout:
				base.Callback.ErrorCode = (int)dnsStatus;
				if (!base.FailureTolerant)
				{
					this.mxRecords.Prune(mxRecord.Preference);
					goto IL_1BE;
				}
				goto IL_1BE;
			case DnsStatus.ServerFailure:
				base.Callback.ErrorCode = (int)dnsStatus;
				if (this.HandleServerFailure(originalRequest))
				{
					DnsLog.Log(this, "Ignoring ServerFailure response for request [{0}] and querying A record", new object[]
					{
						originalRequest
					});
					DnsRecordType dnsRecordType = this.currentType;
					flag = this.SetNextQueryType(mxRecord, this.currentType);
					if (flag && !base.FailureTolerant && dnsRecordType == DnsRecordType.AAAA && this.currentType == DnsRecordType.A)
					{
						previousState = new MxRequest.FaultTolerantQueryState(true);
					}
				}
				else
				{
					this.mxRecords.Prune(mxRecord.Preference);
				}
				dnsStatus = DnsStatus.Success;
				goto IL_1BE;
			case DnsStatus.ErrorSubQueryTimeout:
				flag = this.SetNextQueryType(mxRecord, this.currentType);
				dnsStatus = DnsStatus.Success;
				goto IL_1BE;
			default:
				goto IL_1B0;
			}
			if (this.currentType == DnsRecordType.AAAA && base.ServerList.Cache != null)
			{
				base.ServerList.Cache.RemoveAaaaTimeOutEntry(this.queryFor);
			}
			this.AnalyzeAResponse(list);
			flag = this.SetNextQueryType(mxRecord, this.currentType);
			goto IL_1BE;
			IL_1B0:
			base.Callback.ErrorCode = (int)dnsStatus;
			dnsStatus = DnsStatus.ErrorRetry;
			IL_1BE:
			if (flag)
			{
				if (++this.subQueryCount < Dns.MaxSubQueries)
				{
					this.queryFor = mxRecord.NextLink;
					ExTraceGlobals.DNSTracer.Information<int, string>((long)this.GetHashCode(), "{0} Posting a query to find Alias for {1}", this.subQueryCount, this.queryFor);
					base.PostRequest(this.queryFor, this.currentType, previousState);
					return false;
				}
				dnsStatus = DnsStatus.Success;
				this.tooManyQueries = true;
			}
			if (dnsStatus == DnsStatus.Success || this.IsStatusInfoTruncatedAndModeFailureTolerant(dnsStatus))
			{
				dnsStatus = this.ValidateMxData();
				if (dnsStatus == DnsStatus.Pending)
				{
					return false;
				}
			}
			this.InterpretResults(dnsStatus, results);
			return true;
		}

		private bool HandleServerFailure(DnsAsyncRequest originalRequest)
		{
			if (base.FailureTolerant)
			{
				return true;
			}
			if (originalRequest != null && originalRequest.PreviousRequestState != null)
			{
				MxRequest.FaultTolerantQueryState faultTolerantQueryState = originalRequest.PreviousRequestState as MxRequest.FaultTolerantQueryState;
				if (faultTolerantQueryState != null && faultTolerantQueryState.IsFaultTolerantQuery)
				{
					DnsLog.Log(this, "A and AAAA records failed for {0}. Failing the query.", new object[]
					{
						originalRequest
					});
					return false;
				}
			}
			return this.currentType == DnsRecordType.AAAA || this.currentType == DnsRecordType.A;
		}

		private void ProcessMxRecords(DnsRecordList records)
		{
			foreach (DnsRecord dnsRecord in records.EnumerateAnswers(DnsRecordType.MX))
			{
				DnsMxRecord dnsMxRecord = (DnsMxRecord)dnsRecord;
				ExTraceGlobals.DNSTracer.Information<string, int, string>((long)this.GetHashCode(), "    MX record {0} {1} : {2}", dnsMxRecord.Name, dnsMxRecord.Preference, dnsMxRecord.NameExchange);
				if (string.IsNullOrEmpty(dnsMxRecord.Name) || DnsNativeMethods.DnsNameCompare(dnsMxRecord.Name, this.queryFor))
				{
					MxRequest.MxRecord mxRecord = MxRequest.MxRecord.Create(dnsMxRecord, MxRequest.GenerateRandomNumber());
					if (mxRecord == null)
					{
						ExTraceGlobals.DNSTracer.Information((long)this.GetHashCode(), "Empty nameExchange on MX record.. Skipping");
					}
					else
					{
						if (this.canonicalNames.Count > 0)
						{
							foreach (DnsCNameRecord dnsCNameRecord in this.canonicalNames)
							{
								if (mxRecord.TimeToLive > dnsCNameRecord.TimeToLive)
								{
									mxRecord.TimeToLive = dnsCNameRecord.TimeToLive;
								}
							}
						}
						this.mxRecords.Add(mxRecord);
					}
				}
			}
		}

		private bool ProcessMxResponse(DnsResult results, DnsAsyncRequest asyncRequest)
		{
			DnsStatus dnsStatus = results.Status;
			DnsRecordList list = results.List;
			this.mxRecords = new MxRequest.MxRecordList();
			DnsStatus dnsStatus2 = dnsStatus;
			switch (dnsStatus2)
			{
			case DnsStatus.Success:
				break;
			case DnsStatus.InfoNoRecords:
			case DnsStatus.InfoDomainNonexistent:
				goto IL_1F2;
			case DnsStatus.InfoMxLoopback:
			case DnsStatus.ErrorInvalidData:
			case DnsStatus.ErrorExcessiveData:
				goto IL_2B5;
			case DnsStatus.InfoTruncated:
				if (!base.FailureTolerant)
				{
					goto IL_2B5;
				}
				break;
			default:
				if (dnsStatus2 != DnsStatus.ErrorNoDns)
				{
					goto IL_2B5;
				}
				goto IL_1F2;
			}
			this.ProcessMxRecords(list);
			List<DnsCNameRecord> names = list.ExtractRecords<DnsCNameRecord>(DnsRecordType.CNAME, DnsCNameRecord.Comparer);
			if (this.mxRecords.Count == 0)
			{
				int num = this.BuildChain(names);
				if (num < 0)
				{
					dnsStatus = DnsStatus.ErrorInvalidData;
					goto IL_2B5;
				}
				if (num == 0)
				{
					dnsStatus = DnsStatus.InfoNoRecords;
					goto IL_2B5;
				}
				this.queryFor = this.canonicalNames[this.canonicalNames.Count - 1].Host;
				this.ProcessMxRecords(list);
			}
			if (this.mxRecords.Count > 0)
			{
				this.mxRecords.Truncate(100);
				using (List<MxRequest.MxRecord>.Enumerator enumerator = this.mxRecords.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MxRequest.MxRecord mxRecord = enumerator.Current;
						int num2 = this.FindAliases(mxRecord, list);
						if (num2 == -1)
						{
							this.seenLocal = true;
							this.mxRecords.Prune(mxRecord.Preference);
							break;
						}
						if (mxRecord.Addresses.Count <= 0)
						{
							num2 = mxRecord.BuildChain(names);
							if (num2 >= 0 && num2 != 0)
							{
								num2 = this.FindAliases(mxRecord, list);
								if (num2 == -1)
								{
									this.seenLocal = true;
									this.mxRecords.Prune(mxRecord.Preference);
									break;
								}
							}
						}
					}
					goto IL_2B5;
				}
			}
			if (++this.subQueryCount < Dns.MaxSubQueries)
			{
				ExTraceGlobals.DNSTracer.Information<int, string>((long)this.GetHashCode(), "{0} Posting a query to find Mx records for {1}", this.subQueryCount, this.queryFor);
				base.PostRequest(this.queryFor, this.currentType, null);
				dnsStatus = DnsStatus.Pending;
				return false;
			}
			dnsStatus = DnsStatus.Success;
			this.tooManyQueries = true;
			goto IL_2B5;
			IL_1F2:
			if (this.implicitSearch)
			{
				this.ephemeralMx = true;
				MxRequest.MxRecord mxRecord2 = new MxRequest.MxRecord((this.canonicalNames.Count == 0) ? this.domain : this.canonicalNames[this.canonicalNames.Count - 1].Host, MxRequest.DefaultTTL, 1, 0);
				ExTraceGlobals.DNSTracer.Information<string, int, string>((long)this.GetHashCode(), "    MX record {0} {1} : {2} (ephemeral)", mxRecord2.DnsName, mxRecord2.Preference, mxRecord2.DnsName);
				this.mxRecords.Add(mxRecord2);
				if (dnsStatus == DnsStatus.InfoDomainNonexistent || dnsStatus == DnsStatus.ErrorNoDns)
				{
					if (this.CacheOrInternalLookup(mxRecord2, mxRecord2.NextLink, true) != 0)
					{
						dnsStatus = DnsStatus.Success;
					}
				}
				else
				{
					this.CacheOrInternalLookup(mxRecord2, mxRecord2.NextLink, false);
					dnsStatus = DnsStatus.Success;
				}
			}
			if (dnsStatus == DnsStatus.ErrorNoDns)
			{
				dnsStatus = DnsStatus.ErrorRetry;
			}
			IL_2B5:
			if (dnsStatus == DnsStatus.Success || this.IsStatusInfoTruncatedAndModeFailureTolerant(dnsStatus))
			{
				this.dataProcessor = new MxRequest.DataProcessor(this.ProcessAddressResponse);
				dnsStatus = this.ValidateMxData();
				if (dnsStatus == DnsStatus.Pending)
				{
					return false;
				}
			}
			this.InterpretResults(dnsStatus, results);
			return true;
		}

		private int BuildChain(List<DnsCNameRecord> names)
		{
			string key = (this.canonicalNames.Count == 0) ? this.domain : this.canonicalNames[this.canonicalNames.Count - 1].Host;
			return Request.FollowCNameChain(names, key, this.canonicalNames);
		}

		private int FindAliases(MxRequest.MxRecord current, DnsRecordList records)
		{
			int num = this.CacheOrInternalLookup(current, current.NextLink, true);
			if (num < 0)
			{
				return num;
			}
			if (current.Addresses.Count == 0)
			{
				num = this.FindAliasesFromList(current, records);
				if (num < 0)
				{
					return num;
				}
			}
			if (current.Addresses.Count == 0)
			{
				num = this.CacheOrInternalLookup(current, current.NextLink, false);
				if (num < 0)
				{
					return num;
				}
			}
			return current.Addresses.Count;
		}

		private int FindAliasesFromList(MxRequest.MxRecord current, DnsRecordList records)
		{
			int num = current.ExtractAliases(records);
			if (num > 0)
			{
				for (int i = current.Addresses.Count - 1; i >= 0; i--)
				{
					IPAddress ipaddress = current.Addresses[i];
					if (IPAddress.IsLoopback(ipaddress) || this.IsIpLocal(ipaddress))
					{
						ExTraceGlobals.DNSTracer.Information<int, IPAddress>((long)this.GetHashCode(), "Alias {1} matches a loopback address. Pruning records with preference >= {0}", current.Preference, ipaddress);
						return -1;
					}
					if ((this.requestedAddressFamily == AddressFamily.InterNetworkV6 && ipaddress.AddressFamily != AddressFamily.InterNetworkV6) || (this.requestedAddressFamily == AddressFamily.InterNetwork && ipaddress.AddressFamily != AddressFamily.InterNetwork))
					{
						current.Addresses.RemoveAt(i);
					}
				}
			}
			return current.Addresses.Count;
		}

		private bool SetNextQueryType(MxRequest.MxRecord item, DnsRecordType lastQueryType)
		{
			if (this.mxIndex >= this.mxRecords.Count || item.StaticRecords)
			{
				return false;
			}
			DnsRecordType dnsRecordType;
			bool flag = Request.NextQueryType(this.requestedAddressFamily, item.Addresses.Ipv4AddressCount, item.Addresses.Ipv6AddressCount, lastQueryType, out dnsRecordType);
			if (flag)
			{
				this.currentType = dnsRecordType;
			}
			return flag;
		}

		private DnsStatus ValidateMxData()
		{
			if (this.maxRetriesPerAddressRequest == 0)
			{
				int num = (this.requestedAddressFamily == AddressFamily.Unspecified) ? 2 : 1;
				int num2 = (int)((base.RequestTimeout.Ticks - DateTime.UtcNow.Ticks) / base.DnsInstance.QueryRetryInterval.Ticks);
				if (this.mxRecords.Count > 0)
				{
					this.maxRetriesPerAddressRequest = Math.Max(3, num2 / (num * this.mxRecords.Count));
				}
			}
			if (this.tooManyQueries)
			{
				this.mxIndex = this.mxRecords.Count;
			}
			this.mxIndex++;
			while (this.mxIndex < this.mxRecords.Count)
			{
				MxRequest.MxRecord mxRecord = this.mxRecords[this.mxIndex];
				if (mxRecord.Addresses.Count != 0)
				{
					goto IL_1A7;
				}
				if (mxRecord.CanonicalNames.Count == 0)
				{
					IPAddress ipaddress;
					if (!IPAddress.TryParse(mxRecord.DnsName, out ipaddress))
					{
						goto IL_1A7;
					}
					ExTraceGlobals.DNSTracer.Information<string, IPAddress>((long)this.GetHashCode(), "    Alias {0} : {1} (ephemeral)", mxRecord.DnsName, ipaddress);
					if (!this.IsIpLocal(ipaddress) && !IPAddress.IsLoopback(ipaddress))
					{
						if (this.requestedAddressFamily == AddressFamily.Unspecified || this.requestedAddressFamily == ipaddress.AddressFamily)
						{
							mxRecord.Addresses.Add(ipaddress);
						}
					}
					else
					{
						ExTraceGlobals.DNSTracer.Information<int>((long)this.GetHashCode(), "Ephemeral alias matches local list. Pruning records with preference >= {0}", mxRecord.Preference);
						this.seenLocal = true;
						this.mxRecords.Prune(mxRecord.Preference);
					}
				}
				else
				{
					if (mxRecord.CanonicalNames.Count != Dns.MaxCnameRecords)
					{
						goto IL_1A7;
					}
					ExTraceGlobals.DNSTracer.Information<string>((long)this.GetHashCode(), "The chain of canonical names on {0} is too long.", mxRecord.DnsName);
				}
				IL_240:
				this.mxIndex++;
				continue;
				IL_1A7:
				if (!this.SetNextQueryType(mxRecord, DnsRecordType.Unknown))
				{
					DnsLog.Log(this, "No more queries needed for this record, move to the next item at index {0}", new object[]
					{
						this.mxIndex + 1
					});
					goto IL_240;
				}
				this.queryFor = mxRecord.NextLink;
				if (++this.subQueryCount >= Dns.MaxSubQueries)
				{
					this.tooManyQueries = true;
					break;
				}
				ExTraceGlobals.DNSTracer.Information<int, string>((long)this.GetHashCode(), "{0} Posting another query to find Alias for {1}", this.subQueryCount, mxRecord.DnsName);
				base.PostRequest(this.queryFor, this.currentType, null);
				return DnsStatus.Pending;
			}
			return DnsStatus.Success;
		}

		private void InterpretResults(DnsStatus status, DnsResult dnsResult)
		{
			TargetHost[] array = MxRequest.EmptyTargetHostArray;
			if (status == DnsStatus.Success || status == DnsStatus.InfoTruncated || status == DnsStatus.ErrorTimeout)
			{
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				foreach (MxRequest.MxRecord mxRecord in this.mxRecords)
				{
					if (mxRecord.Addresses.Count != 0)
					{
						num3++;
					}
					else if (mxRecord.CanonicalNames.Count == Dns.MaxCnameRecords)
					{
						num2++;
					}
					else
					{
						num++;
					}
				}
				array = new TargetHost[num3];
				int num4 = 0;
				foreach (MxRequest.MxRecord mxRecord2 in this.mxRecords)
				{
					if (mxRecord2.Addresses.Count != 0)
					{
						array[num4++] = new TargetHost(mxRecord2.NextLink, mxRecord2.Addresses.ToArray(), mxRecord2.TimeToLive);
						if (num4 == num3)
						{
							break;
						}
					}
				}
				if (this.tooManyQueries && !base.FailureTolerant)
				{
					status = DnsStatus.InfoTruncated;
				}
				else if (array.Length == 0)
				{
					if (num2 > 0)
					{
						status = DnsStatus.ErrorInvalidData;
					}
					else if (this.seenLocal)
					{
						status = DnsStatus.InfoMxLoopback;
					}
					else if (num > 0 || this.mxRecords.Count == 0)
					{
						status = DnsStatus.ErrorRetry;
					}
				}
				else
				{
					if (base.Callback.ErrorCode != 0)
					{
						DnsStatus errorCode = (DnsStatus)base.Callback.ErrorCode;
						Request.EventLogger.LogEvent(CommonEventLogConstants.Tuple_DnsQueryReturnedPartialResults, this.domain, new object[]
						{
							this.domain,
							errorCode
						});
						base.Callback.ErrorCode = 0;
					}
					if (status != DnsStatus.Success && base.FailureTolerant)
					{
						status = DnsStatus.Success;
					}
				}
			}
			DnsLog.Log(this, "MX request complete. Status={0}. {1}", new object[]
			{
				status,
				(array == null) ? string.Empty : string.Join<TargetHost>(",", (IEnumerable<TargetHost>)array)
			});
			ExTraceGlobals.DNSTracer.Information<string>((long)this.GetHashCode(), "MX request complete [{0}].", Dns.DnsStatusToString(status));
			base.Callback.InvokeCallback(new Request.Result(status, dnsResult.Server, array));
		}

		private bool IsIpLocal(IPAddress routingAddress)
		{
			if (this.localAddresses != null)
			{
				foreach (IPAddress obj in this.localAddresses)
				{
					if (routingAddress.Equals(obj))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		private int CacheOrInternalLookup(MxRequest.MxRecord record, string name, bool staticOnly)
		{
			TimeSpan timeSpan = TimeSpan.Zero;
			IPAddress[] array = null;
			if (!base.BypassCache)
			{
				array = base.FindEntries(name, this.requestedAddressFamily, staticOnly, out timeSpan);
			}
			if (array == null)
			{
				array = Request.FindLocalHostEntries(name, this.requestedAddressFamily, out timeSpan);
			}
			if (array == null)
			{
				return 0;
			}
			bool flag = false;
			for (int i = 0; i < array.Length; i++)
			{
				if (this.IsIpLocal(array[i]))
				{
					flag = true;
					break;
				}
				if (timeSpan == TimeSpan.MaxValue)
				{
					record.Addresses.Add(array[i]);
				}
				else
				{
					if (IPAddress.IsLoopback(array[i]))
					{
						flag = true;
						break;
					}
					record.Addresses.Add(array[i]);
				}
			}
			if (flag)
			{
				ExTraceGlobals.DNSTracer.Information<int>((long)this.GetHashCode(), "Cached Alias matches local list. Pruning records with preference >= {0}", record.Preference);
				return -1;
			}
			if (timeSpan == TimeSpan.MaxValue)
			{
				timeSpan = MxRequest.DefaultTTL;
				record.StaticRecords = true;
			}
			if (record.TimeToLive > timeSpan)
			{
				record.TimeToLive = timeSpan;
			}
			return record.Addresses.Count;
		}

		private bool IsStatusInfoTruncatedAndModeFailureTolerant(DnsStatus status)
		{
			return status == DnsStatus.InfoTruncated && base.FailureTolerant;
		}

		private const DnsRecordType RecordType = DnsRecordType.MX;

		private const int ConsecutiveTimeOutThreshold = 20;

		private static readonly TimeSpan DefaultTTL = TimeSpan.FromHours(1.0);

		private static readonly TargetHost[] EmptyTargetHostArray = new TargetHost[0];

		private static readonly Request.Result InvalidDataRequestResult = new Request.Result(DnsStatus.ErrorInvalidData, IPAddress.None, MxRequest.EmptyTargetHostArray);

		private static Random random = new Random((int)DateTime.UtcNow.Ticks);

		private string domain;

		private IEnumerable<IPAddress> localAddresses;

		private AddressFamily requestedAddressFamily;

		private bool implicitSearch = true;

		private MxRequest.MxRecordList mxRecords;

		private List<DnsCNameRecord> canonicalNames = new List<DnsCNameRecord>();

		private bool seenLocal;

		private bool tooManyQueries;

		private int subQueryCount;

		private MxRequest.DataProcessor dataProcessor;

		private int maxRetriesPerAddressRequest;

		private string queryFor;

		private DnsRecordType currentType = DnsRecordType.MX;

		private int mxIndex = -1;

		private bool ephemeralMx;

		private delegate bool DataProcessor(DnsResult dnsResult, DnsAsyncRequest dnsAsyncRequest);

		private class MxRecord
		{
			public MxRecord(string name, TimeSpan ttl, int preference, int weight)
			{
				this.DnsName = name;
				this.TimeToLive = ttl;
				this.Preference = preference;
				this.Weight = weight;
			}

			private MxRecord(DnsMxRecord record, int weight)
			{
				string nameExchange = record.NameExchange;
				if (string.IsNullOrEmpty(nameExchange) || (nameExchange.Length == 1 && nameExchange[0] == '.'))
				{
					throw new ArgumentException("Name on MxRecord was null, empty or a simple .", "record");
				}
				int length = nameExchange.Length;
				this.DnsName = ((nameExchange[length - 1] != '.') ? nameExchange : nameExchange.Substring(0, length));
				this.TimeToLive = record.TimeToLive;
				this.Preference = record.Preference;
				this.Weight = weight;
			}

			public string DnsName
			{
				get
				{
					return this.dnsName;
				}
				set
				{
					this.dnsName = value;
				}
			}

			public Request.HostAddressList Addresses
			{
				get
				{
					return this.addresses;
				}
			}

			public List<DnsCNameRecord> CanonicalNames
			{
				get
				{
					return this.canonicalNames;
				}
			}

			public TimeSpan TimeToLive
			{
				get
				{
					return this.timeToLive;
				}
				set
				{
					this.timeToLive = value;
				}
			}

			public int Weight
			{
				get
				{
					return this.weight;
				}
				set
				{
					this.weight = value;
				}
			}

			public int Preference
			{
				get
				{
					return this.preference;
				}
				set
				{
					this.preference = value;
				}
			}

			public bool StaticRecords
			{
				get
				{
					return this.staticRecords;
				}
				set
				{
					this.staticRecords = value;
				}
			}

			public string NextLink
			{
				get
				{
					if (this.CanonicalNames.Count != 0)
					{
						return this.CanonicalNames[this.CanonicalNames.Count - 1].Host;
					}
					return this.DnsName;
				}
			}

			public static MxRequest.MxRecord Create(DnsMxRecord record, int weight)
			{
				if (string.IsNullOrEmpty(record.NameExchange))
				{
					return null;
				}
				string nameExchange = record.NameExchange;
				int length = nameExchange.Length;
				if (length == 1 && nameExchange[0] == '.')
				{
					return null;
				}
				return new MxRequest.MxRecord(record, weight);
			}

			public static int Compare(MxRequest.MxRecord a, MxRequest.MxRecord b)
			{
				int num = a.Preference.CompareTo(b.Preference);
				if (num == 0)
				{
					return a.Weight.CompareTo(b.Weight);
				}
				return num;
			}

			public int BuildChain(List<DnsCNameRecord> names)
			{
				return Request.FollowCNameChain(names, this.NextLink, this.canonicalNames);
			}

			public int ExtractAliases(DnsRecordList records)
			{
				string nextLink = this.NextLink;
				foreach (DnsRecord dnsRecord in DnsRecordList.EnumerateAddresses(records))
				{
					IPAddress ipaddress = IPAddress.None;
					TimeSpan zero = TimeSpan.Zero;
					DnsRecordType recordType = dnsRecord.RecordType;
					if (recordType != DnsRecordType.A)
					{
						if (recordType != DnsRecordType.AAAA)
						{
							throw new InvalidOperationException("Internal DNS Record type filtering failure.");
						}
						if (!nextLink.Equals(dnsRecord.Name, StringComparison.OrdinalIgnoreCase))
						{
							continue;
						}
						DnsAAAARecord dnsAAAARecord = dnsRecord as DnsAAAARecord;
						ExTraceGlobals.DNSTracer.Information<string, IPAddress>((long)this.GetHashCode(), "    Alias (IPv6) {0} : {1}", dnsAAAARecord.Name, dnsAAAARecord.IPAddress);
						ipaddress = dnsAAAARecord.IPAddress;
						zero = dnsAAAARecord.TimeToLive;
					}
					else
					{
						if (!nextLink.Equals(dnsRecord.Name, StringComparison.OrdinalIgnoreCase))
						{
							continue;
						}
						DnsARecord dnsARecord = dnsRecord as DnsARecord;
						ExTraceGlobals.DNSTracer.Information<string, IPAddress>((long)this.GetHashCode(), "    Alias (IPv4) {0} : {1}", dnsARecord.Name, dnsARecord.IPAddress);
						ipaddress = dnsARecord.IPAddress;
						zero = dnsARecord.TimeToLive;
					}
					if (ipaddress != IPAddress.None)
					{
						if (this.TimeToLive > zero)
						{
							this.TimeToLive = zero;
						}
						this.Addresses.Add(ipaddress);
					}
				}
				return this.Addresses.Count;
			}

			private string dnsName;

			private Request.HostAddressList addresses = new Request.HostAddressList();

			private List<DnsCNameRecord> canonicalNames = new List<DnsCNameRecord>();

			private TimeSpan timeToLive;

			private int weight;

			private int preference;

			private bool staticRecords;
		}

		private class MxRecordList : List<MxRequest.MxRecord>
		{
			public void Truncate(int maxLength)
			{
				if (base.Count == 0)
				{
					return;
				}
				base.Sort(new Comparison<MxRequest.MxRecord>(MxRequest.MxRecord.Compare));
				if (base.Count > maxLength)
				{
					base.RemoveRange(maxLength, base.Count - maxLength);
				}
			}

			public void Prune(int preference)
			{
				int i = 0;
				while (i < base.Count)
				{
					if (preference == base[i].Preference)
					{
						if (i == 0)
						{
							base.Clear();
							return;
						}
						base.RemoveRange(i, base.Count - i);
						return;
					}
					else
					{
						i++;
					}
				}
			}
		}

		private class FaultTolerantQueryState
		{
			public FaultTolerantQueryState(bool isFaultTolerantQuery)
			{
				this.IsFaultTolerantQuery = isFaultTolerantQuery;
			}

			public readonly bool IsFaultTolerantQuery;
		}
	}
}
