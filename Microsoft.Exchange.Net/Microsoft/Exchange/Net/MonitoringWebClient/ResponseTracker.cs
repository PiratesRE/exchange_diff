using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class ResponseTracker : IResponseTracker
	{
		internal ResponseTracker()
		{
		}

		public List<ResponseTrackerItem> Items
		{
			get
			{
				List<ResponseTrackerItem> list = this.sortedItems;
				if (list == null)
				{
					lock (this.items)
					{
						list = (from i in this.items
						orderby i.Index
						select i).ToList<ResponseTrackerItem>();
						this.sortedItems = list;
					}
				}
				return list;
			}
		}

		ResponseTrackerItem IResponseTracker.TrackRequest(TestId testId, RequestTarget requestTarget, HttpWebRequestWrapper request)
		{
			ResponseTrackerItem responseTrackerItem = new ResponseTrackerItem();
			responseTrackerItem.Index = this.items.Count;
			responseTrackerItem.StepId = testId.ToString();
			responseTrackerItem.TargetType = requestTarget.ToString();
			responseTrackerItem.TargetHost = request.RequestUri.Host;
			responseTrackerItem.PathAndQuery = request.RequestUri.PathAndQuery;
			lock (this.items)
			{
				this.items.Add(responseTrackerItem);
			}
			this.sortedItems = null;
			return responseTrackerItem;
		}

		void IResponseTracker.TrackSentRequest(ResponseTrackerItem item, HttpWebRequestWrapper request)
		{
			if (request.RequestIpAddressUri != null)
			{
				item.TargetIpAddress = request.RequestIpAddressUri.GetCanonicalHostAddress();
			}
		}

		void IResponseTracker.TrackResponse(ResponseTrackerItem item, HttpWebResponseWrapper response)
		{
			item.ResponseLatency = response.ResponseLatency;
			item.TotalLatency += response.TotalLatency;
			item.ContentLength = response.ContentLength;
			item.RespondingServer = response.RespondingFrontEndServer;
			item.MailboxServer = response.MailboxServer;
			item.DomainController = response.DomainController;
			item.ARRServer = response.ARRServer;
			item.LdapLatency = response.LdapLatency;
			item.MservLatency = response.MservLatency;
			item.RpcLatency = response.RpcLatency;
			item.CasLatency = response.CasLatency;
			item.IsE14CasServer = response.IsE14CasServer;
			item.Response = response;
		}

		void IResponseTracker.TrackFailedResponse(HttpWebResponseWrapper response, ScenarioException exception)
		{
			if (response == null)
			{
				return;
			}
			if (this.items == null)
			{
				return;
			}
			ResponseTrackerItem responseTrackerItem = null;
			lock (this.items)
			{
				if (this.items.Count <= 0)
				{
					return;
				}
				responseTrackerItem = this.GetPreviousMatchingItem(null, null, response.Request);
				if (responseTrackerItem == null)
				{
					responseTrackerItem = this.Items.Last<ResponseTrackerItem>();
				}
			}
			string text = null;
			if (exception.FailingComponent == FailingComponent.Mailbox)
			{
				text = response.MailboxServer;
			}
			else if (exception.FailingComponent == FailingComponent.ActiveDirectory)
			{
				text = response.DomainController;
			}
			if (text == null)
			{
				text = response.ProcessingServer;
			}
			responseTrackerItem.FailingTargetHostname = response.Request.RequestUri.Host;
			if (response.Request.RequestIpAddressUri != null)
			{
				responseTrackerItem.FailingTargetIPAddress = response.Request.RequestIpAddressUri.GetCanonicalHostAddress();
			}
			responseTrackerItem.FailingServer = text;
			responseTrackerItem.FailureHttpResponseCode = new int?((int)response.StatusCode);
		}

		void IResponseTracker.TrackFailedRequest(TestId testId, RequestTarget requestTarget, HttpWebRequestWrapper request, Exception exception)
		{
			if (request == null)
			{
				return;
			}
			ResponseTrackerItem responseTrackerItem = this.GetPreviousMatchingItem(new TestId?(testId), new RequestTarget?(requestTarget), request);
			if (responseTrackerItem == null)
			{
				responseTrackerItem = ((IResponseTracker)this).TrackRequest(testId, requestTarget, request);
			}
			responseTrackerItem.FailingTargetHostname = responseTrackerItem.TargetHost;
			responseTrackerItem.FailingTargetIPAddress = responseTrackerItem.TargetIpAddress;
			this.AppendFailedIpAddress(request, responseTrackerItem, exception);
		}

		void IResponseTracker.TrackResolvedRequest(HttpWebRequestWrapper request)
		{
			ResponseTrackerItem previousMatchingItem = this.GetPreviousMatchingItem(null, null, request);
			if (previousMatchingItem == null)
			{
				return;
			}
			if (request.RequestIpAddressUri != null)
			{
				previousMatchingItem.TargetIpAddress = request.RequestIpAddressUri.GetCanonicalHostAddress();
				previousMatchingItem.TargetVipName = request.TargetVipName;
				previousMatchingItem.TargetVipForestName = request.TargetVipForestName;
			}
			previousMatchingItem.DnsLatency = request.DnsLatency;
			previousMatchingItem.TotalLatency += request.DnsLatency;
		}

		void IResponseTracker.TrackItemCausingScenarioTimeout(ResponseTrackerItem item, Exception exception)
		{
			item.FailingTargetIPAddress = item.TargetIpAddress;
			item.FailingServer = item.RespondingServer;
			this.AppendFailedIpAddress(item.Response.Request, item, exception);
		}

		void IResponseTracker.TrackFailedTcpConnection(HttpWebRequestWrapper request, Exception exception)
		{
			ResponseTrackerItem previousMatchingItem = this.GetPreviousMatchingItem(null, null, request);
			if (previousMatchingItem == null)
			{
				return;
			}
			this.AppendFailedIpAddress(request, previousMatchingItem, exception);
		}

		public string GetActivitySummary(bool useCsvFormat)
		{
			SummaryHeader[] array = new SummaryHeader[6];
			array[0] = new SummaryHeader("Host", 30, (ResponseTrackerItem i) => new string[]
			{
				i.TargetHost
			});
			array[1] = new SummaryHeader("Path", 50, (ResponseTrackerItem i) => new string[]
			{
				i.PathAndQuery
			});
			array[2] = new SummaryHeader("Total Ltcy (msec)", 17, (ResponseTrackerItem i) => new string[]
			{
				i.TotalLatency.TotalMilliseconds.ToString("N0")
			});
			array[3] = new SummaryHeader("DNS Ltcy (msec)", 15, (ResponseTrackerItem i) => new string[]
			{
				i.DnsLatency.TotalMilliseconds.ToString("N0")
			});
			array[4] = new SummaryHeader("Server", 16, (ResponseTrackerItem i) => new string[]
			{
				i.RespondingServer
			});
			array[5] = new SummaryHeader("IP Address", 16, delegate(ResponseTrackerItem i)
			{
				if (i.FailedIpAddresses != null && i.FailedIpAddresses.Length > 0)
				{
					string[] array2 = i.FailedIpAddresses.Split(new char[]
					{
						','
					});
					int num = array2.Length;
					Array.Resize<string>(ref array2, num + 1);
					array2[num] = i.TargetIpAddress;
					return array2;
				}
				return new string[]
				{
					i.TargetIpAddress
				};
			});
			SummaryHeader[] headers = array;
			StringBuilder stringBuilder = new StringBuilder();
			lock (this.items)
			{
				foreach (ResponseTrackerItem responseTrackerItem in this.Items)
				{
					responseTrackerItem.AppendSummary(useCsvFormat, headers, stringBuilder);
				}
			}
			return stringBuilder.ToString();
		}

		private ResponseTrackerItem GetPreviousMatchingItem(TestId? testId, RequestTarget? requestTarget, HttpWebRequestWrapper request)
		{
			if (this.items == null)
			{
				return null;
			}
			ResponseTrackerItem result = null;
			lock (this.items)
			{
				if (this.items.Count <= 0)
				{
					return null;
				}
				int num = -1;
				foreach (ResponseTrackerItem responseTrackerItem in this.items)
				{
					if (responseTrackerItem.Matches(testId, requestTarget, request) && responseTrackerItem.Index > num)
					{
						num = responseTrackerItem.Index;
						result = responseTrackerItem;
					}
				}
			}
			return result;
		}

		private void AppendFailedIpAddress(HttpWebRequestWrapper request, ResponseTrackerItem item, Exception exception)
		{
			if (request.RequestIpAddressUri != null)
			{
				string canonicalHostAddress = request.RequestIpAddressUri.GetCanonicalHostAddress();
				if (item.FailedIpAddresses == null)
				{
					item.FailedIpAddresses = canonicalHostAddress;
				}
				else if (item.FailedIpAddresses.IndexOf(canonicalHostAddress, StringComparison.OrdinalIgnoreCase) < 0)
				{
					item.FailedIpAddresses = item.FailedIpAddresses + "," + canonicalHostAddress;
				}
				if (item.IpAddressListFailureList == null)
				{
					ConcurrentDictionary<NamedVip, Exception> value = new ConcurrentDictionary<NamedVip, Exception>();
					Interlocked.CompareExchange<ConcurrentDictionary<NamedVip, Exception>>(ref item.IpAddressListFailureList, value, null);
				}
				NamedVip key2 = new NamedVip
				{
					IPAddressString = canonicalHostAddress,
					Name = request.TargetVipName,
					ForestName = request.TargetVipForestName
				};
				item.IpAddressListFailureList.AddOrUpdate(key2, exception, (NamedVip key, Exception originalValue) => originalValue);
			}
		}

		private List<ResponseTrackerItem> items = new List<ResponseTrackerItem>();

		private List<ResponseTrackerItem> sortedItems;
	}
}
