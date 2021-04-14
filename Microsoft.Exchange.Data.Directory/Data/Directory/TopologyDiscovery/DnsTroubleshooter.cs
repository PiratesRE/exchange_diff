using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Directory.TopologyService;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Data.Directory.TopologyDiscovery
{
	internal sealed class DnsTroubleshooter
	{
		private static Dns Client
		{
			get
			{
				if (-1 == DnsTroubleshooter.lastRenewal || Globals.GetTickDifference(DnsTroubleshooter.lastRenewal, Environment.TickCount) > 60000UL)
				{
					ExTraceGlobals.DnsTroubleshooterTracer.TraceInformation<int>(0, 0L, "Updating server list {0}", Environment.TickCount);
					DnsTroubleshooter.sClient.InitializeServerList(DnsServerList.GetAdapterDnsServerList(Guid.Empty, false, false));
					Interlocked.Exchange(ref DnsTroubleshooter.lastRenewal, Environment.TickCount);
				}
				return DnsTroubleshooter.sClient;
			}
		}

		public static Task DiagnoseDnsProblemForDomainController(string serverFqdn)
		{
			if (string.IsNullOrEmpty(serverFqdn))
			{
				throw new ArgumentNullException("serverFqdn");
			}
			string addomainNameForServer = DnsTroubleshooter.GetADDomainNameForServer(serverFqdn);
			DnsTroubleshooter.DnsQueryContext context = new DnsTroubleshooter.DnsQueryContext(addomainNameForServer, serverFqdn);
			return DnsTroubleshooter.QuerySrvRecords(context).ContinueWith(new Action<Task<DnsTroubleshooter.SrvQueryResult>>(DnsTroubleshooter.CheckDnsResult), TaskContinuationOptions.AttachedToParent);
		}

		public static Task DiagnoseDnsProblemForDomain(string domainFqdn)
		{
			if (string.IsNullOrEmpty(domainFqdn))
			{
				throw new ArgumentNullException("domainFqdn");
			}
			DnsTroubleshooter.DnsQueryContext context = new DnsTroubleshooter.DnsQueryContext(domainFqdn, null);
			return DnsTroubleshooter.QuerySrvRecords(context).ContinueWith(new Action<Task<DnsTroubleshooter.SrvQueryResult>>(DnsTroubleshooter.CheckDnsResult), TaskContinuationOptions.AttachedToParent);
		}

		private static string GetADDomainNameForServer(string serverFqdn)
		{
			if (string.IsNullOrEmpty(serverFqdn))
			{
				throw new ArgumentNullException("serverFqdn");
			}
			string text = null;
			try
			{
				text = NativeHelpers.GetPrimaryDomainInformationFromServer(serverFqdn);
			}
			catch (CannotGetDomainInfoException arg)
			{
				ExTraceGlobals.DnsTroubleshooterTracer.TraceError<string, CannotGetDomainInfoException>(0L, "{0} CannotGetDomainInfoException Error {1}", serverFqdn, arg);
			}
			catch (ADTransientException arg2)
			{
				ExTraceGlobals.DnsTroubleshooterTracer.TraceError<string, ADTransientException>(0L, "{0} ADTransientException Error {1}", serverFqdn, arg2);
			}
			if (string.IsNullOrEmpty(text))
			{
				text = serverFqdn.Substring(serverFqdn.IndexOf(".") + 1);
			}
			return text;
		}

		private static Task<DnsTroubleshooter.SrvQueryResult> QuerySrvRecords(DnsTroubleshooter.DnsQueryContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("state");
			}
			context.SrvQuery = string.Format("_ldap._tcp.dc._msdcs.{0}", context.DomainFqdn);
			Dns client = DnsTroubleshooter.Client;
			return Task.Factory.FromAsync<string, DnsQueryOptions, DnsTroubleshooter.SrvQueryResult>(new Func<string, DnsQueryOptions, AsyncCallback, object, IAsyncResult>(client.BeginRetrieveSrvRecords), delegate(IAsyncResult x)
			{
				ExTraceGlobals.FaultInjectionTracer.TraceTest(2244357437U);
				SrvRecord[] srvRecords;
				DnsStatus status = Dns.EndResolveToSrvRecords(x, out srvRecords);
				return new DnsTroubleshooter.SrvQueryResult(status, srvRecords);
			}, context.SrvQuery, DnsQueryOptions.UseTcpOnly | DnsQueryOptions.BypassCache, context, TaskCreationOptions.AttachedToParent);
		}

		private static void CheckDnsResult(Task<DnsTroubleshooter.SrvQueryResult> task)
		{
			if (task == null)
			{
				throw new ArgumentException("task");
			}
			if (task.AsyncState == null)
			{
				throw new ArgumentException("Task.AsyncContext sholdn't be null");
			}
			DnsTroubleshooter.DnsQueryContext dnsQueryContext = task.AsyncState as DnsTroubleshooter.DnsQueryContext;
			if (dnsQueryContext == null)
			{
				throw new ArgumentException("Invalid async context");
			}
			ExTraceGlobals.DnsTroubleshooterTracer.TraceFunction<string, TaskStatus>(0L, "DiagnoseError - {0} Task Result {1}", dnsQueryContext.DomainFqdn, task.Status);
			if (task.Status != TaskStatus.RanToCompletion)
			{
				ExTraceGlobals.DnsTroubleshooterTracer.TraceError<string>(0L, "{0} Error Dns Query", dnsQueryContext.DomainFqdn);
				if (task.Exception != null)
				{
					Exception innerException = task.Exception.Flatten().InnerException;
					ExTraceGlobals.DnsTroubleshooterTracer.TraceError<string, Exception>(0L, "{0} Error Dns Query {1}", dnsQueryContext.DomainFqdn, innerException);
					Globals.LogEvent(DirectoryEventLogConstants.Tuple_DnsThroubleshooterError, dnsQueryContext.DomainFqdn, new object[]
					{
						dnsQueryContext.DomainFqdn,
						innerException.Message
					});
				}
				return;
			}
			DnsTroubleshooter.SrvQueryResult result = task.Result;
			DnsTroubleshooter.AnalyzeDnsResultAndLogEvent(dnsQueryContext, task.Result);
		}

		private static void AnalyzeDnsResultAndLogEvent(DnsTroubleshooter.DnsQueryContext context, DnsTroubleshooter.SrvQueryResult result)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			if (result == null)
			{
				throw new ArgumentNullException("result");
			}
			if (ExTraceGlobals.DnsTroubleshooterTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (!result.SrvRecords.IsNullOrEmpty<SrvRecord>())
				{
					foreach (SrvRecord srvRecord in result.SrvRecords)
					{
						stringBuilder.Append(srvRecord.TargetHost);
						stringBuilder.Append(",");
					}
				}
				ExTraceGlobals.DnsTroubleshooterTracer.TraceDebug(0L, "Domain Fqdn {0}. Server Fqdn {1}. Query {2}. ResultStatus {3}. Result Records {4}", new object[]
				{
					context.DomainFqdn,
					context.ServerFqdn ?? "<NULL>",
					context.SrvQuery,
					result.Status,
					stringBuilder.ToString()
				});
			}
			DnsStatus status = result.Status;
			switch (status)
			{
			case DnsStatus.Success:
			{
				if (result.SrvRecords.IsNullOrEmpty<SrvRecord>())
				{
					ExTraceGlobals.DnsTroubleshooterTracer.TraceDebug<string, DnsStatus>(0L, "{0} - Status {1}. No records found", context.DomainFqdn, result.Status);
					Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_DNS_OTHER, context.DomainFqdn, new object[]
					{
						result.Status,
						context.DomainFqdn,
						context.SrvQuery
					});
					return;
				}
				StringBuilder stringBuilder2 = new StringBuilder();
				bool flag = false;
				foreach (SrvRecord srvRecord2 in result.SrvRecords)
				{
					stringBuilder2.AppendLine(srvRecord2.TargetHost);
					if (context.ServerFqdn != null && context.ServerFqdn.Equals(srvRecord2.TargetHost, StringComparison.OrdinalIgnoreCase))
					{
						flag = true;
					}
				}
				ExTraceGlobals.DnsTroubleshooterTracer.TraceDebug<string, DnsStatus, bool>(0L, "{0} - Status {1}. isHostAdvertisedInDns {2}", context.DomainFqdn, result.Status, flag);
				if (string.IsNullOrEmpty(context.ServerFqdn))
				{
					Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_DNS_NO_ERROR, context.DomainFqdn, new object[]
					{
						context.DomainFqdn,
						context.SrvQuery,
						stringBuilder2.ToString()
					});
					return;
				}
				if (flag)
				{
					Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_DNS_NO_ERROR_DC_FOUND, context.ServerFqdn, new object[]
					{
						context.ServerFqdn,
						context.SrvQuery,
						stringBuilder2.ToString()
					});
					return;
				}
				string listOfZones = DnsTroubleshooter.GetListOfZones(context.DomainFqdn);
				string listOfClientDnsServerAddresses = DnsTroubleshooter.GetListOfClientDnsServerAddresses();
				ExTraceGlobals.DnsTroubleshooterTracer.TraceDebug(0L, "{0} - Status {1}. Zones {2}. Dns Servers {3}", new object[]
				{
					context.DomainFqdn,
					result.Status,
					listOfZones,
					listOfClientDnsServerAddresses
				});
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_DNS_NO_ERROR_DC_NOT_FOUND, context.ServerFqdn, new object[]
				{
					context.ServerFqdn,
					context.DomainFqdn,
					context.SrvQuery,
					stringBuilder2.ToString(),
					listOfClientDnsServerAddresses,
					listOfZones
				});
				return;
			}
			case DnsStatus.InfoNoRecords:
				break;
			case DnsStatus.InfoDomainNonexistent:
			{
				string listOfZones = DnsTroubleshooter.GetListOfZones(context.DomainFqdn);
				string listOfClientDnsServerAddresses = DnsTroubleshooter.GetListOfClientDnsServerAddresses();
				ExTraceGlobals.DnsTroubleshooterTracer.TraceDebug(0L, "{0} - Status {1}. Zones {2}. Dns Servers {3}", new object[]
				{
					context.DomainFqdn,
					result.Status,
					listOfZones,
					listOfClientDnsServerAddresses
				});
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_DNS_NAME_ERROR, context.DomainFqdn, new object[]
				{
					NativeMethods.HRESULT_FROM_WIN32(9003U).ToString("X"),
					context.DomainFqdn,
					context.SrvQuery,
					listOfClientDnsServerAddresses,
					listOfZones
				});
				return;
			}
			default:
				switch (status)
				{
				case DnsStatus.ErrorRetry:
				case DnsStatus.ErrorTimeout:
				{
					string listOfClientDnsServerAddresses = DnsTroubleshooter.GetListOfClientDnsServerAddresses();
					ExTraceGlobals.DnsTroubleshooterTracer.TraceDebug<string, DnsStatus, string>(0L, "{0} - Status {1}. Dns Servers {2}", context.DomainFqdn, result.Status, listOfClientDnsServerAddresses);
					Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_DNS_TIMEOUT, context.DomainFqdn, new object[]
					{
						NativeMethods.HRESULT_FROM_WIN32(1460U).ToString("X"),
						context.DomainFqdn,
						context.SrvQuery,
						listOfClientDnsServerAddresses
					});
					return;
				}
				case DnsStatus.ServerFailure:
				{
					string listOfZones = DnsTroubleshooter.GetListOfZones(context.DomainFqdn);
					string listOfClientDnsServerAddresses = DnsTroubleshooter.GetListOfClientDnsServerAddresses();
					ExTraceGlobals.DnsTroubleshooterTracer.TraceDebug(0L, "{0} - Status {1}. Zones {2}. Dns Servers {3}", new object[]
					{
						context.DomainFqdn,
						result.Status,
						listOfZones,
						listOfClientDnsServerAddresses
					});
					Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_DNS_DIAG_SERVER_FAILURE, context.DomainFqdn, new object[]
					{
						NativeMethods.HRESULT_FROM_WIN32(9002U).ToString("X"),
						context.DomainFqdn,
						context.SrvQuery,
						listOfClientDnsServerAddresses,
						listOfZones
					});
					return;
				}
				}
				break;
			}
			Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_DNS_OTHER, context.DomainFqdn, new object[]
			{
				result.Status,
				context.DomainFqdn,
				context.SrvQuery
			});
		}

		private static string GetListOfZones(string dnsDomainName)
		{
			if (string.IsNullOrEmpty(dnsDomainName))
			{
				throw new ArgumentNullException("dnsDomainName");
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(dnsDomainName);
			for (int i = dnsDomainName.IndexOf("."); i > 0; i = dnsDomainName.IndexOf("."))
			{
				dnsDomainName = dnsDomainName.Substring(i + 1);
				stringBuilder.AppendLine(dnsDomainName);
			}
			stringBuilder.AppendLine(DirectoryStrings.RootZone);
			return stringBuilder.ToString();
		}

		private static string GetListOfClientDnsServerAddresses()
		{
			IPAddress[] adapterDnsServerList = DnsServerList.GetAdapterDnsServerList(Guid.Empty, false, false);
			if (!adapterDnsServerList.IsNullOrEmpty<IPAddress>())
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (IPAddress ipaddress in adapterDnsServerList)
				{
					stringBuilder.AppendLine(ipaddress.ToString());
				}
				return stringBuilder.ToString();
			}
			return DirectoryStrings.NoAddresses;
		}

		private const uint UnhandledDnsQueryException = 2244357437U;

		private const string SrvQueryFormat = "_ldap._tcp.dc._msdcs.{0}";

		private const uint DnsCacheRefreshEveryNMilliseconds = 60000U;

		private static Dns sClient = new Dns();

		private static int lastRenewal = -1;

		internal class SrvQueryResult
		{
			public SrvQueryResult(DnsStatus status, SrvRecord[] srvRecords)
			{
				this.SrvRecords = srvRecords;
				this.Status = status;
			}

			public SrvRecord[] SrvRecords { get; private set; }

			public DnsStatus Status { get; private set; }
		}

		internal class DnsQueryContext
		{
			public DnsQueryContext(string domainFqdn, string serverFqdn = null)
			{
				if (string.IsNullOrEmpty(domainFqdn))
				{
					throw new ArgumentNullException("domainFqdn");
				}
				this.DomainFqdn = domainFqdn;
				this.ServerFqdn = serverFqdn;
			}

			public string DomainFqdn { get; private set; }

			public string ServerFqdn { get; private set; }

			public string SrvQuery { get; set; }
		}
	}
}
