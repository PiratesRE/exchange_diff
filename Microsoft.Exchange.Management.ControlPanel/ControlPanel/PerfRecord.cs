using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.LatencyDetection;
using Microsoft.Exchange.PowerShell.RbacHostingTools;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class PerfRecord
	{
		private static IPerformanceDataProvider[] PerformanceDataProviders
		{
			get
			{
				return new IPerformanceDataProvider[]
				{
					PerformanceContext.Current,
					RpcDataProvider.Instance,
					TaskPerformanceData.CmdletInvoked,
					TaskPerformanceData.BeginProcessingInvoked,
					TaskPerformanceData.ProcessRecordInvoked,
					TaskPerformanceData.EndProcessingInvoked,
					EcpPerformanceData.CreateRbacSession,
					EcpPerformanceData.ActiveRunspace,
					EcpPerformanceData.CreateRunspace,
					EcpPerformanceData.PowerShellInvoke,
					EcpPerformanceData.WcfSerialization,
					AspPerformanceData.GetStepData(RequestNotification.AuthenticateRequest),
					AspPerformanceData.GetStepData(RequestNotification.AuthorizeRequest),
					AspPerformanceData.GetStepData(RequestNotification.ResolveRequestCache),
					AspPerformanceData.GetStepData(RequestNotification.MapRequestHandler),
					AspPerformanceData.GetStepData(RequestNotification.AcquireRequestState),
					AspPerformanceData.GetStepData(RequestNotification.ExecuteRequestHandler),
					AspPerformanceData.GetStepData(RequestNotification.ReleaseRequestState),
					AspPerformanceData.GetStepData(RequestNotification.UpdateRequestCache),
					AspPerformanceData.GetStepData(RequestNotification.LogRequest),
					EcpPerformanceData.XamlParsed,
					EcpPerformanceData.DDIServiceExecution,
					EcpPerformanceData.DDITypeConversion
				};
			}
		}

		public PerfRecord(string requestPath)
		{
			PerfRecord.activeRequestsCounter.Increment();
			this.aspThreadPerfRecord = new TaskPerformanceRecord(requestPath, PerfRecord.aspLatencyDetectionContextFactory, EcpEventLogConstants.Tuple_EcpApplicationRequestStarted, EcpEventLogConstants.Tuple_EcpApplicationRequestEnded, EcpEventLogExtensions.EventLog);
			this.wcfThreadPerfRecord = new TaskPerformanceRecord(requestPath, PerfRecord.wcfLatencyDetectionContextFactory, EcpEventLogConstants.Tuple_EcpWebServiceRequestStarted, EcpEventLogConstants.Tuple_EcpWebServiceRequestCompleted, EcpEventLogExtensions.EventLog);
			this.aspThreadPerfRecord.Start(PerfRecord.PerformanceDataProviders);
		}

		public static PerfRecord Current
		{
			get
			{
				return (PerfRecord)HttpContext.Current.Items[PerfRecord.ecpRequestContextKey];
			}
			set
			{
				HttpContext.Current.Items[PerfRecord.ecpRequestContextKey] = value;
			}
		}

		public void StepStarted(RequestNotification notification)
		{
			this.aspPerformanceData.StepStarted(notification);
		}

		public void StepCompleted()
		{
			this.aspPerformanceData.StepCompleted();
		}

		public void EndRequest()
		{
			if (this.aspThreadPerfRecord.IsCollecting)
			{
				this.averageRequestTime.Stop();
				PerfRecord.activeRequestsCounter.Decrement();
				this.ServerRequestTime = Math.Round(this.aspThreadPerfRecord.Stop().TotalMilliseconds, 4);
				EcpEventLogConstants.Tuple_EcpPerformanceRecord.LogEvent(new object[]
				{
					this.aspThreadPerfRecord.TaskName,
					this
				});
			}
		}

		public void WebServiceCallStarted()
		{
			this.wcfThreadPerfRecord.Start(PerfRecord.PerformanceDataProviders);
		}

		public void WebServiceCallCompleted()
		{
			this.wcfThreadPerfRecord.Stop();
		}

		private double GetLatency(PerfRecord.PerfProvider providerIndex)
		{
			return Math.Round((this.aspThreadPerfRecord[(int)providerIndex].Latency + this.wcfThreadPerfRecord[(int)providerIndex].Latency).TotalMilliseconds, 4);
		}

		private uint GetCount(PerfRecord.PerfProvider providerIndex)
		{
			return this.aspThreadPerfRecord[(int)providerIndex].Count + this.wcfThreadPerfRecord[(int)providerIndex].Count;
		}

		[DataMember]
		public double ServerRequestTime { get; set; }

		[DataMember]
		public double Authentication
		{
			get
			{
				return this.GetLatency(PerfRecord.PerfProvider.AuthenticateRequest);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public double Authorization
		{
			get
			{
				return this.GetLatency(PerfRecord.PerfProvider.AuthorizeRequest);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public double ResolveCache
		{
			get
			{
				return this.GetLatency(PerfRecord.PerfProvider.ResolveRequestCache);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public double MapRequest
		{
			get
			{
				return this.GetLatency(PerfRecord.PerfProvider.MapRequestHandler);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public double AcquireState
		{
			get
			{
				return this.GetLatency(PerfRecord.PerfProvider.AcquireRequestState);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public double ExecuteRequest
		{
			get
			{
				return this.GetLatency(PerfRecord.PerfProvider.ExecuteRequestHandler);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public double ReleaseState
		{
			get
			{
				return this.GetLatency(PerfRecord.PerfProvider.ReleaseRequestState);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public double UpdateCache
		{
			get
			{
				return this.GetLatency(PerfRecord.PerfProvider.UpdateRequestCache);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public double LogRequest
		{
			get
			{
				return this.GetLatency(PerfRecord.PerfProvider.LogRequest);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public uint Rpc
		{
			get
			{
				return this.GetCount(PerfRecord.PerfProvider.Rpc);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public double RpcLatency
		{
			get
			{
				return this.GetLatency(PerfRecord.PerfProvider.Rpc);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public uint Ldap
		{
			get
			{
				return this.GetCount(PerfRecord.PerfProvider.Ldap);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public double LdapLatency
		{
			get
			{
				return this.GetLatency(PerfRecord.PerfProvider.Ldap);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public uint Rbac
		{
			get
			{
				return this.GetCount(PerfRecord.PerfProvider.CreateRbacSession);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public double RbacLatency
		{
			get
			{
				return this.GetLatency(PerfRecord.PerfProvider.CreateRbacSession);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public double CreateRunspace
		{
			get
			{
				return this.GetCount(PerfRecord.PerfProvider.CreateRunspace);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public double CreateRunspaceLatency
		{
			get
			{
				return this.GetLatency(PerfRecord.PerfProvider.CreateRunspace);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public double ActiveRunspace
		{
			get
			{
				return this.GetCount(PerfRecord.PerfProvider.ActiveRunspace);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public double ActiveRunspaceLatency
		{
			get
			{
				return this.GetLatency(PerfRecord.PerfProvider.ActiveRunspace);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public uint PowerShellInvoke
		{
			get
			{
				return this.GetCount(PerfRecord.PerfProvider.PowerShellInvoke);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public double PowerShellInvokeLatency
		{
			get
			{
				return this.GetLatency(PerfRecord.PerfProvider.PowerShellInvoke);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public uint Cmdlet
		{
			get
			{
				return this.GetCount(PerfRecord.PerfProvider.Cmdlet);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public double CmdletLatency
		{
			get
			{
				return this.GetLatency(PerfRecord.PerfProvider.Cmdlet);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public double BeginProcessing
		{
			get
			{
				return this.GetCount(PerfRecord.PerfProvider.BeginProcessing);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public double BeginProcessingLatency
		{
			get
			{
				return this.GetLatency(PerfRecord.PerfProvider.BeginProcessing);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public uint ProcessRecord
		{
			get
			{
				return this.GetCount(PerfRecord.PerfProvider.ProcessRecord);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public double ProcessRecordLatency
		{
			get
			{
				return this.GetLatency(PerfRecord.PerfProvider.ProcessRecord);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public double EndProcessingLatency
		{
			get
			{
				return this.GetLatency(PerfRecord.PerfProvider.EndProcessing);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public uint WcfSerialization
		{
			get
			{
				return this.GetCount(PerfRecord.PerfProvider.WcfSerialization);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public double WcfSerializationLatency
		{
			get
			{
				return this.GetLatency(PerfRecord.PerfProvider.WcfSerialization);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public double XamlParser
		{
			get
			{
				return this.GetLatency(PerfRecord.PerfProvider.XamlParser);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public double DDIServiceExecution
		{
			get
			{
				return this.GetLatency(PerfRecord.PerfProvider.DDIServiceExecution);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public double DDITypeConversion
		{
			get
			{
				return this.GetLatency(PerfRecord.PerfProvider.DDITypeConversion);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		public void AppendToIisLog()
		{
			if (!string.IsNullOrEmpty(HttpContext.Current.GetSessionID()))
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("&perfRecord=");
				stringBuilder.Append(this.ToString().Replace("&", "%26"));
				stringBuilder.Append("&sessionId=");
				using (StringWriter stringWriter = new StringWriter(stringBuilder))
				{
					HttpContext.Current.Server.UrlEncode(HttpContext.Current.GetSessionID(), stringWriter);
				}
				LocalSession localSession = RbacPrincipal.GetCurrent(false) as LocalSession;
				if (localSession != null)
				{
					stringBuilder.Append("&logonType=");
					stringBuilder.Append(localSession.LogonTypeFlag);
				}
				HttpContext.Current.Response.AppendToLog(stringBuilder.ToString());
			}
		}

		public override string ToString()
		{
			if (this.json == null && this.ServerRequestTime != 0.0)
			{
				this.json = this.ToJsonString(null);
			}
			if (this.json != null)
			{
				return this.json;
			}
			return this.ToJsonString(null);
		}

		private const int Precision = 4;

		private static LatencyDetectionContextFactory aspLatencyDetectionContextFactory = LatencyDetectionContextFactory.CreateFactory("ECP.AspRequest");

		private static LatencyDetectionContextFactory wcfLatencyDetectionContextFactory = LatencyDetectionContextFactory.CreateFactory("ECP.WcfRequest");

		private TaskPerformanceRecord aspThreadPerfRecord;

		private TaskPerformanceRecord wcfThreadPerfRecord;

		private AspPerformanceData aspPerformanceData = new AspPerformanceData();

		private AverageTimePerfCounter averageRequestTime = new AverageTimePerfCounter(EcpPerfCounters.AverageResponseTime, EcpPerfCounters.AverageResponseTimeBase, true);

		private static PerfCounterGroup activeRequestsCounter = new PerfCounterGroup(EcpPerfCounters.ActiveRequests, EcpPerfCounters.ActiveRequestsPeak, EcpPerfCounters.ActiveRequestsTotal);

		private static object ecpRequestContextKey = new object();

		private string json;

		internal enum PerfProvider
		{
			Ldap,
			Rpc,
			Cmdlet,
			BeginProcessing,
			ProcessRecord,
			EndProcessing,
			CreateRbacSession,
			ActiveRunspace,
			CreateRunspace,
			PowerShellInvoke,
			WcfSerialization,
			AuthenticateRequest,
			AuthorizeRequest,
			ResolveRequestCache,
			MapRequestHandler,
			AcquireRequestState,
			ExecuteRequestHandler,
			ReleaseRequestState,
			UpdateRequestCache,
			LogRequest,
			XamlParser,
			DDIServiceExecution,
			DDITypeConversion
		}
	}
}
