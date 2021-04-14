using System;
using Microsoft.Exchange.AddressBook.EventLog;
using Microsoft.Exchange.AddressBook.Nspi;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.AddressBook.Service;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal sealed class AddressBookService : BaseObject, IRpcService, IDisposable
	{
		public AddressBookService(IRpcServiceManager serviceManager)
		{
			this.eventLog = new ExEventLog(AddressBookService.ComponentGuid, "MSExchangeAB");
			this.serviceManager = serviceManager;
			string userName = Environment.UserName;
			AddressBookService.GeneralTracer.TraceDebug<string>(0L, "Running as {0}", userName);
		}

		public string Name
		{
			get
			{
				return "MSExchangeAB";
			}
		}

		internal static MovingAveragePerfCounter NspiRpcRequestsAverageLatency
		{
			get
			{
				return AddressBookService.nspiRpcRequestsAverageLatency;
			}
		}

		internal static MovingAveragePerfCounter NspiRpcBrowseRequestsAverageLatency
		{
			get
			{
				return AddressBookService.nspiRpcBrowseRequestsAverageLatency;
			}
		}

		internal static MovingAveragePerfCounter RfrRpcRequestsAverageLatency
		{
			get
			{
				return AddressBookService.rfrRpcRequestsAverageLatency;
			}
		}

		internal ExEventLog ExEventLog
		{
			get
			{
				return this.eventLog;
			}
		}

		internal bool IsStarted
		{
			get
			{
				return this.isStarted;
			}
		}

		public bool IsEnabled()
		{
			return NspiServer.Instance != null || RfriServer.Instance != null;
		}

		public void OnStartBegin()
		{
			this.isStarted = true;
			Configuration.Initialize(this.eventLog, new Action(this.serviceManager.StopService));
			if (!Configuration.ServiceEnabled)
			{
				AddressBookService.GeneralTracer.TraceDebug(0L, "The service is not enabled.");
				return;
			}
			AddressBookService.InitializePerfCounters(new AddressBookPerformanceCounters());
			NspiPropMapper.Initialize();
			this.RegisterServicePrincipalNames();
			ADSession.DisableAdminTopologyMode();
			this.serviceManager.AddHttpPort(6001.ToString());
			NspiServer.Initialize(this.serviceManager, this.eventLog);
			RfriServer.Initialize(this.serviceManager, this.eventLog);
			if (Configuration.ProtocolLoggingEnabled)
			{
				if (string.IsNullOrEmpty(Configuration.LogFilePath))
				{
					this.eventLog.LogEvent(AddressBookEventLogConstants.Tuple_BadConfigParameter, "LogFilePath", new object[]
					{
						Configuration.LogFilePath
					});
					return;
				}
				ProtocolLog.Initialize(ExDateTime.UtcNow, Configuration.LogFilePath, TimeSpan.FromHours((double)Configuration.MaxRetentionPeriod), Configuration.MaxDirectorySize, Configuration.PerFileMaxSize, Configuration.ApplyHourPrecision);
			}
		}

		internal static void InitializePerfCounters(IAddressBookPerformanceCounters addressBookPerformanceCounters)
		{
			Util.ThrowOnNullArgument(addressBookPerformanceCounters, "addressBookPerformanceCounters");
			AddressBookPerformanceCountersWrapper.Initialize(addressBookPerformanceCounters);
			AddressBookPerformanceCountersWrapper.AddressBookPerformanceCounters.PID.RawValue = (long)Globals.ProcessId;
			AddressBookService.nspiRpcRequestsAverageLatency = new MovingAveragePerfCounter(AddressBookPerformanceCountersWrapper.AddressBookPerformanceCounters.NspiRequestsAverageLatency, Configuration.AverageLatencySamples);
			AddressBookService.nspiRpcBrowseRequestsAverageLatency = new MovingAveragePerfCounter(AddressBookPerformanceCountersWrapper.AddressBookPerformanceCounters.NspiBrowseRequestsAverageLatency, Configuration.AverageLatencySamples);
			AddressBookService.rfrRpcRequestsAverageLatency = new MovingAveragePerfCounter(AddressBookPerformanceCountersWrapper.AddressBookPerformanceCounters.RfrRequestsAverageLatency, Configuration.AverageLatencySamples);
		}

		public void OnStartEnd()
		{
			if (NspiServer.Instance == null && RfriServer.Instance == null)
			{
				this.eventLog.LogEvent(AddressBookEventLogConstants.Tuple_NoEndpointsConfigured, string.Empty, new object[0]);
				return;
			}
			this.eventLog.LogEvent(AddressBookEventLogConstants.Tuple_AddressBookServiceStartSuccess, string.Empty, new object[0]);
		}

		public void OnStopBegin()
		{
			NspiServer.ShuttingDown();
			RfriServer.ShuttingDown();
		}

		public void OnStopEnd()
		{
			if (this.isStarted)
			{
				if (Configuration.ProtocolLoggingEnabled)
				{
					ProtocolLog.Shutdown();
				}
				Configuration.Terminate();
				this.eventLog.LogEvent(AddressBookEventLogConstants.Tuple_AddressBookServiceStopSuccess, string.Empty, new object[0]);
			}
			this.isStarted = false;
		}

		public void HandleUnexpectedExceptionOnStart(Exception ex)
		{
			if (ex is DuplicateRpcEndpointException)
			{
				DuplicateRpcEndpointException ex2 = (DuplicateRpcEndpointException)ex;
				AddressBookService.GeneralTracer.TraceError<int, string>(0L, "Error {0} starting the RPC server: {1}", ex2.ErrorCode, ex2.Message);
				this.eventLog.LogEvent(AddressBookEventLogConstants.Tuple_RpcRegisterInterfaceFailure, string.Empty, new object[]
				{
					"MSExchangeAB",
					ServiceHelper.FormatWin32ErrorString(ex2.ErrorCode)
				});
				return;
			}
			this.eventLog.LogEvent(AddressBookEventLogConstants.Tuple_UnexpectedExceptionOnStart, string.Empty, new object[]
			{
				ex.Message
			});
		}

		public void HandleUnexpectedExceptionOnStop(Exception ex)
		{
			AddressBookService.GeneralTracer.TraceError<Exception>(0L, "Unexpected exception while stopping: {0}", ex);
			this.eventLog.LogEvent(AddressBookEventLogConstants.Tuple_UnexpectedExceptionOnStop, string.Empty, new object[]
			{
				ex
			});
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<AddressBookService>(this);
		}

		private void RegisterServicePrincipalNames()
		{
			ServiceHelper.RegisterSPN("exchangeAB", this.eventLog, AddressBookEventLogConstants.Tuple_SpnRegisterFailure);
			ServiceHelper.RegisterSPN("exchangeRFR", this.eventLog, AddressBookEventLogConstants.Tuple_SpnRegisterFailure);
		}

		private const string AddressBookServiceName = "MSExchangeAB";

		private const string NspiServiceClass = "exchangeAB";

		private const string RfrServiceClass = "exchangeRFR";

		internal static readonly Trace GeneralTracer = ExTraceGlobals.GeneralTracer;

		private static readonly Guid ComponentGuid = new Guid("10193997-6273-4e05-b423-2ffb1d96e1aa");

		private static MovingAveragePerfCounter nspiRpcRequestsAverageLatency;

		private static MovingAveragePerfCounter nspiRpcBrowseRequestsAverageLatency;

		private static MovingAveragePerfCounter rfrRpcRequestsAverageLatency;

		private readonly ExEventLog eventLog;

		private readonly IRpcServiceManager serviceManager;

		private bool isStarted;
	}
}
