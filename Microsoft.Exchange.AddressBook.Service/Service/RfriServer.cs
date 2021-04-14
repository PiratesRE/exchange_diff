using System;
using Microsoft.Exchange.AddressBook.EventLog;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.AddressBook.Service;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.RfriServer;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal sealed class RfriServer : RfriAsyncRpcServer
	{
		internal static void Initialize(IRpcServiceManager serviceManager, ExEventLog eventLog)
		{
			Util.ThrowOnNullArgument(serviceManager, "serviceManager");
			Util.ThrowOnNullArgument(eventLog, "eventLog");
			RfriServer.rfriAsyncDispatch = new RfriAsyncDispatch();
			RfriServer.eventLog = eventLog;
			serviceManager.AddServer(new Action(RfriServer.Start), new Action(RfriServer.Stop));
		}

		internal static RfriServer Instance
		{
			get
			{
				return RfriServer.instance;
			}
		}

		internal static void Start()
		{
			ServerFqdnCache.InitializeCache();
			if (RfriServer.instance == null)
			{
				bool flag = false;
				try
				{
					RfriServer.instance = (RfriServer)RpcServerBase.RegisterAutoListenInterfaceSupportingAnonymous(typeof(RfriServer), RpcServerBase.DefaultMaxRpcCalls, "Microsoft Exchange RFR Interface", true);
					flag = true;
				}
				catch (RpcException ex)
				{
					RfriServer.ReferralTracer.TraceError<string>(0L, "Error registering the RFR RPC interface: {0}", ex.Message);
					RfriServer.eventLog.LogEvent(AddressBookEventLogConstants.Tuple_RpcRegisterInterfaceFailure, string.Empty, new object[]
					{
						"RFR",
						ServiceHelper.FormatWin32ErrorString(ex.ErrorCode)
					});
				}
				finally
				{
					if (!flag)
					{
						RfriServer.rfriAsyncDispatch = null;
						RfriServer.Stop();
						RfriServer.instance = null;
					}
				}
			}
		}

		internal static void Stop()
		{
			if (RfriServer.instance != null)
			{
				RpcServerBase.UnregisterInterface(RfriAsyncRpcServer.RpcIntfHandle, true);
				RfriServer.instance = null;
			}
			ServerFqdnCache.TerminateCache();
		}

		internal static void ShuttingDown()
		{
			if (RfriServer.instance != null && RfriServer.rfriAsyncDispatch != null)
			{
				RfriServer.rfriAsyncDispatch.ShuttingDown();
			}
		}

		public override IRfriAsyncDispatch GetAsyncDispatch()
		{
			return RfriServer.rfriAsyncDispatch;
		}

		private static readonly Trace ReferralTracer = ExTraceGlobals.ReferralTracer;

		private static RfriServer instance = null;

		private static RfriAsyncDispatch rfriAsyncDispatch = null;

		private static ExEventLog eventLog;
	}
}
