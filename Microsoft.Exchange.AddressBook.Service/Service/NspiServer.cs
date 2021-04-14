using System;
using Microsoft.Exchange.AddressBook.EventLog;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.AddressBook.Service;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.NspiServer;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal sealed class NspiServer : NspiAsyncRpcServer
	{
		internal static void Initialize(IRpcServiceManager serviceManager, ExEventLog eventLog)
		{
			Util.ThrowOnNullArgument(serviceManager, "serviceManager");
			Util.ThrowOnNullArgument(eventLog, "eventLog");
			NspiServer.nspiAsyncDispatch = new NspiAsyncDispatch();
			NspiServer.eventLog = eventLog;
			serviceManager.AddServer(new Action(NspiServer.Start), new Action(NspiServer.Stop));
		}

		internal static NspiServer Instance
		{
			get
			{
				return NspiServer.instance;
			}
		}

		internal static void Start()
		{
			if (NspiServer.instance == null)
			{
				bool flag = false;
				try
				{
					NspiServer.instance = (NspiServer)RpcServerBase.RegisterAutoListenInterfaceSupportingAnonymous(typeof(NspiServer), RpcServerBase.DefaultMaxRpcCalls, "Microsoft Exchange NSPI Interface", false);
					NspiServer.instance.StartRundownQueue();
					flag = true;
				}
				catch (RpcException ex)
				{
					NspiServer.NspiTracer.TraceError<string>(0L, "Error registering the NSPI RPC interface: {0}", ex.Message);
					NspiServer.eventLog.LogEvent(AddressBookEventLogConstants.Tuple_RpcRegisterInterfaceFailure, string.Empty, new object[]
					{
						"NSPI",
						ServiceHelper.FormatWin32ErrorString(ex.ErrorCode)
					});
				}
				finally
				{
					if (!flag)
					{
						NspiServer.nspiAsyncDispatch = null;
						NspiServer.Stop();
						NspiServer.instance = null;
					}
				}
			}
		}

		internal static void Stop()
		{
			if (NspiServer.instance != null)
			{
				RpcServerBase.UnregisterInterface(NspiAsyncRpcServer.RpcIntfHandle);
				NspiServer.instance.StopRundownQueue();
				NspiServer.instance = null;
			}
		}

		internal static void ShuttingDown()
		{
			if (NspiServer.instance != null && NspiServer.nspiAsyncDispatch != null)
			{
				NspiServer.nspiAsyncDispatch.ShuttingDown();
			}
		}

		public override INspiAsyncDispatch GetAsyncDispatch()
		{
			return NspiServer.nspiAsyncDispatch;
		}

		private static readonly Trace NspiTracer = ExTraceGlobals.NspiTracer;

		private static NspiServer instance = null;

		private static NspiAsyncDispatch nspiAsyncDispatch = null;

		private static ExEventLog eventLog;
	}
}
