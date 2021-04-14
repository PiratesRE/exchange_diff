using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Web;
using Microsoft.Exchange.AddressBook.EventLog;
using Microsoft.Exchange.AddressBook.Nspi;
using Microsoft.Exchange.AddressBook.Service;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net.MapiHttp;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class NspiHttpHandler : MapiHttpHandler
	{
		internal static IRfriAsyncDispatch RfriAsyncDispatch
		{
			get
			{
				return NspiHttpHandler.rfriAsyncDispatch;
			}
		}

		internal static INspiAsyncDispatch NspiAsyncDispatch
		{
			get
			{
				return NspiHttpHandler.nspiAsyncDispatch;
			}
		}

		internal override string EndpointVdirPath
		{
			get
			{
				return MapiHttpEndpoints.VdirPathNspi;
			}
		}

		internal override IAsyncOperationFactory OperationFactory
		{
			get
			{
				return NspiHttpHandler.operationFactory;
			}
		}

		internal override bool TryEnsureHandlerIsInitialized()
		{
			if (NspiHttpHandler.shutdownTime != null)
			{
				return false;
			}
			if (!NspiHttpHandler.initialized)
			{
				lock (NspiHttpHandler.initializeLock)
				{
					if (NspiHttpHandler.shutdownTime != null)
					{
						return false;
					}
					if (!NspiHttpHandler.initialized)
					{
						MapiHttpHandler.IsValidContextHandleDelegate = new Func<object, bool>(NspiHttpHandler.InternalIsValidContextHandle);
						MapiHttpHandler.TryContextHandleRundownDelegate = new Func<object, bool>(NspiHttpHandler.InternalTryContextHandleRundown);
						MapiHttpHandler.ShutdownHandlerDelegate = new Action(NspiHttpHandler.InternalShutdownHandler);
						MapiHttpHandler.NeedTokenRehydrationDelegate = new Func<string, bool>(NspiHttpHandler.InternalNeedTokenRehydration);
						NspiHttpHandler.InitializeAddressBookService();
						NspiHttpHandler.initialized = true;
					}
				}
				return true;
			}
			return true;
		}

		internal override void LogFailure(IList<string> requestIds, IList<string> cookies, string message, string userName, string protocolSequence, string clientAddress, string organization, Exception exception, Microsoft.Exchange.Diagnostics.Trace trace)
		{
			ProtocolLog.LogProtocolFailure("MapiHttp: failure", requestIds, cookies, message, userName, protocolSequence, clientAddress, organization, exception);
		}

		private static void InternalShutdownHandler()
		{
			try
			{
				if (NspiHttpHandler.shutdownTime == null)
				{
					NspiHttpHandler.shutdownTime = new ExDateTime?(ExDateTime.Now);
					lock (NspiHttpHandler.initializeLock)
					{
						if (NspiHttpHandler.initialized)
						{
							NspiHttpHandler.ShutdownAddressBookService();
							NspiHttpHandler.initialized = false;
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}

		private static bool InternalIsValidContextHandle(object contextHandle)
		{
			if (contextHandle == null)
			{
				return false;
			}
			IntPtr? intPtr = contextHandle as IntPtr?;
			return intPtr != null && !(intPtr.Value == IntPtr.Zero);
		}

		private static bool InternalTryContextHandleRundown(object contextHandle)
		{
			IntPtr? localContextHandle = contextHandle as IntPtr?;
			if (localContextHandle == null)
			{
				return true;
			}
			if (localContextHandle.Value == IntPtr.Zero)
			{
				return true;
			}
			MapiHttpHandler.DispatchCallSync(delegate
			{
				NspiHttpHandler.NspiAsyncDispatch.ContextHandleRundown(localContextHandle.Value);
			});
			return true;
		}

		private static bool InternalNeedTokenRehydration(string requestType)
		{
			return !string.IsNullOrWhiteSpace(requestType) && (string.Compare(requestType, "Bind", true) == 0 || string.Compare(requestType, "GetMailboxUrl", true) == 0 || string.Compare(requestType, "GetAddressBookUrl", true) == 0 || string.Compare(requestType, "GetNspiUrl", true) == 0);
		}

		private static void InitializeAddressBookService()
		{
			Configuration.UseDefaultAppConfig = true;
			ProtocolLog.SetDefaults(string.Format("{0}Logging\\MAPI AddressBook Service\\", ExchangeSetupContext.InstallPath), "MAPI AddressBook Protocol Logs", "MAPIAB_", "MAPIAddressBookProtocolLogs");
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				NspiHttpHandler.processId = currentProcess.Id;
			}
			NspiHttpHandler.eventLogger = new ExEventLog(NspiHttpHandler.ComponentGuid, "MSExchangeMapiAddressBookAppPool");
			NspiHttpHandler.eventLogger.LogEvent(AddressBookEventLogConstants.Tuple_StartingMSExchangeMapiAddressBookAppPool, string.Empty, new object[]
			{
				NspiHttpHandler.processId,
				"Microsoft Exchange",
				"15.00.1497.015"
			});
			int num = Privileges.RemoveAllExcept(new string[]
			{
				"SeAuditPrivilege",
				"SeChangeNotifyPrivilege",
				"SeCreateGlobalPrivilege",
				"SeTcbPrivilege"
			}, "MSExchangeMapiAddressBookAppPool");
			if (num != 0)
			{
				NspiHttpHandler.eventLogger.LogEvent(AddressBookEventLogConstants.Tuple_MapiAddressBookRemovingPrivilegeErrorOnStart, string.Empty, new object[0]);
				string failureDescription = string.Format("Failed to remove privileges from {0}, error code = {1}", "MSExchangeMapiAddressBookAppPool", num);
				throw ProtocolException.FromResponseCode((LID)51100, failureDescription, ResponseCode.EndpointDisabled, null);
			}
			Configuration.Initialize(NspiHttpHandler.eventLogger, null);
			NspiPropMapper.Initialize();
			AddressBookService.InitializePerfCounters(new NspiPerformanceCountersWrapper());
			NspiHttpHandler.isProtocolLogEnabled = Configuration.ProtocolLoggingEnabled;
			if (NspiHttpHandler.isProtocolLogEnabled)
			{
				ProtocolLog.Initialize(ExDateTime.UtcNow, Configuration.LogFilePath, TimeSpan.FromHours((double)Configuration.MaxRetentionPeriod), Configuration.MaxDirectorySize, Configuration.PerFileMaxSize, Configuration.ApplyHourPrecision);
			}
			UserWorkloadManager.Initialize(100, 100, 100, TimeSpan.FromHours(1.0), null);
			ServerFqdnCache.InitializeCache();
			NspiHttpHandler.eventLogger.LogEvent(AddressBookEventLogConstants.Tuple_MSExchangeMapiAddressBookAppPoolStartSuccess, string.Empty, new object[]
			{
				NspiHttpHandler.processId,
				"Microsoft Exchange",
				"15.00.1497.015"
			});
		}

		private static void ShutdownAddressBookService()
		{
			NspiHttpHandler.eventLogger.LogEvent(AddressBookEventLogConstants.Tuple_StoppingMSExchangeMapiAddressBookAppPool, string.Empty, new object[]
			{
				NspiHttpHandler.processId,
				"Microsoft Exchange",
				"15.00.1497.015"
			});
			while (ExDateTime.Now - NspiHttpHandler.shutdownTime.Value < NspiHttpHandler.waitDrainOnShutdown)
			{
				Thread.Sleep(500);
				if (UserWorkloadManager.Singleton.TotalTasks == 0)
				{
					break;
				}
			}
			UserWorkloadManager.Singleton.Dispose();
			Thread.Sleep(500);
			if (NspiHttpHandler.isProtocolLogEnabled)
			{
				ProtocolLog.Shutdown();
			}
			Configuration.Terminate();
			NspiHttpHandler.eventLogger.LogEvent(AddressBookEventLogConstants.Tuple_MSExchangeMapiAddressBookAppPoolStopSuccess, string.Empty, new object[]
			{
				NspiHttpHandler.processId,
				"Microsoft Exchange",
				"15.00.1497.015"
			});
		}

		// Note: this type is marked as 'beforefieldinit'.
		static NspiHttpHandler()
		{
			Dictionary<string, Func<HttpContextBase, AsyncOperation>> dictionary = new Dictionary<string, Func<HttpContextBase, AsyncOperation>>();
			dictionary.Add("Bind", (HttpContextBase context) => new NspiBindAsyncOperation(context));
			dictionary.Add("Unbind", (HttpContextBase context) => new NspiUnbindAsyncOperation(context));
			dictionary.Add("GetMatches", (HttpContextBase context) => new NspiGetMatchesAsyncOperation(context));
			dictionary.Add("GetPropList", (HttpContextBase context) => new NspiGetPropListAsyncOperation(context));
			dictionary.Add("GetProps", (HttpContextBase context) => new NspiGetPropsAsyncOperation(context));
			dictionary.Add("ModProps", (HttpContextBase context) => new NspiModPropsAsyncOperation(context));
			dictionary.Add("DNToMId", (HttpContextBase context) => new NspiDNToEphAsyncOperation(context));
			dictionary.Add("CompareMIds", (HttpContextBase context) => new NspiCompareDNTsAsyncOperation(context));
			dictionary.Add("CompareDNTs", (HttpContextBase context) => new NspiCompareDNTsAsyncOperation(context));
			dictionary.Add("CompareMinIds", (HttpContextBase context) => new NspiCompareDNTsAsyncOperation(context));
			dictionary.Add("GetSpecialTable", (HttpContextBase context) => new NspiGetSpecialTableAsyncOperation(context));
			dictionary.Add("GetTemplateInfo", (HttpContextBase context) => new NspiGetTemplateInfoAsyncOperation(context));
			dictionary.Add("ModLinkAtt", (HttpContextBase context) => new NspiModLinkAttAsyncOperation(context));
			dictionary.Add("QueryColumns", (HttpContextBase context) => new NspiQueryColumnsAsyncOperation(context));
			dictionary.Add("QueryRows", (HttpContextBase context) => new NspiQueryRowsAsyncOperation(context));
			dictionary.Add("ResolveNames", (HttpContextBase context) => new NspiResolveNamesAsyncOperation(context));
			dictionary.Add("ResortRestriction", (HttpContextBase context) => new NspiResortRestrictionAsyncOperation(context));
			dictionary.Add("SeekEntries", (HttpContextBase context) => new NspiSeekEntriesAsyncOperation(context));
			dictionary.Add("UpdateStat", (HttpContextBase context) => new NspiUpdateStatAsyncOperation(context));
			dictionary.Add("GetMailboxUrl", (HttpContextBase context) => new RfriGetMailboxUrlAsyncOperation(context));
			dictionary.Add("GetAddressBookUrl", (HttpContextBase context) => new RfriGetAddressBookUrlAsyncOperation(context));
			dictionary.Add("GetNspiUrl", (HttpContextBase context) => new RfriGetAddressBookUrlAsyncOperation(context));
			NspiHttpHandler.operationFactory = new DictionaryBasedOperationFactory(dictionary);
			NspiHttpHandler.waitDrainOnShutdown = TimeSpan.FromSeconds(30.0);
			NspiHttpHandler.rfriAsyncDispatch = new RfriAsyncDispatch();
			NspiHttpHandler.nspiAsyncDispatch = new NspiAsyncDispatch();
			NspiHttpHandler.initializeLock = new object();
			NspiHttpHandler.initialized = false;
			NspiHttpHandler.shutdownTime = null;
			NspiHttpHandler.isProtocolLogEnabled = false;
		}

		public const string ApplicationPoolName = "MSExchangeMapiAddressBookAppPool";

		private const string LogTypeName = "MAPI AddressBook Protocol Logs";

		private const string LogFilePrefix = "MAPIAB_";

		private const string LogComponent = "MAPIAddressBookProtocolLogs";

		private static readonly Guid ComponentGuid = new Guid("ef013c5d-8aa2-402d-9c7d-227c8c6f0ad6");

		private static readonly IAsyncOperationFactory operationFactory;

		private static readonly TimeSpan waitDrainOnShutdown;

		private static readonly IRfriAsyncDispatch rfriAsyncDispatch;

		private static readonly INspiAsyncDispatch nspiAsyncDispatch;

		private static readonly object initializeLock;

		private static ExEventLog eventLogger;

		private static bool initialized;

		private static ExDateTime? shutdownTime;

		private static bool isProtocolLogEnabled;

		private static int processId;
	}
}
