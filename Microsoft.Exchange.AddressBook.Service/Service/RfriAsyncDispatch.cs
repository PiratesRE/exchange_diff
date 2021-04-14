using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.AddressBook.Service;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi.Rfri;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Server;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal sealed class RfriAsyncDispatch : IRfriAsyncDispatch
	{
		public ICancelableAsyncResult BeginGetNewDSA(ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, RfriGetNewDSAFlags flags, string userDn, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return this.BeginWrapper("BeginGetNewDSA", asyncCallback, asyncState, clientBinding, userDn, (RfriContext context) => new RfriGetNewDSADispatchTask(asyncCallback, asyncState, protocolRequestInfo, clientBinding, context, flags, userDn));
		}

		public RfriStatus EndGetNewDSA(ICancelableAsyncResult asyncResult, out string serverDn)
		{
			string localServerDn = null;
			RfriStatus result;
			try
			{
				result = this.EndWrapper("EndGetNewDSA", asyncResult, (RfriDispatchTask task) => ((RfriGetNewDSADispatchTask)task).End(out localServerDn));
			}
			finally
			{
				serverDn = localServerDn;
			}
			return result;
		}

		public ICancelableAsyncResult BeginGetFQDNFromLegacyDN(ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, RfriGetFQDNFromLegacyDNFlags flags, string serverDn, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return this.BeginWrapper("BeginGetFQDNFromLegacyDN", asyncCallback, asyncState, clientBinding, serverDn, (RfriContext context) => new RfriGetFQDNFromLegacyDNDispatchTask(asyncCallback, asyncState, protocolRequestInfo, clientBinding, context, flags, serverDn));
		}

		public RfriStatus EndGetFQDNFromLegacyDN(ICancelableAsyncResult asyncResult, out string serverFqdn)
		{
			string localServerFqdn = null;
			RfriStatus result;
			try
			{
				result = this.EndWrapper("EndGetFQDNFromLegacyDN", asyncResult, (RfriDispatchTask task) => ((RfriGetFQDNFromLegacyDNDispatchTask)task).End(out localServerFqdn));
			}
			finally
			{
				serverFqdn = localServerFqdn;
			}
			return result;
		}

		public ICancelableAsyncResult BeginGetAddressBookUrl(ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, RfriGetAddressBookUrlFlags flags, string hostname, string userDn, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return this.BeginWrapper("BeginGetAddressBookUrl", asyncCallback, asyncState, clientBinding, userDn, (RfriContext context) => new RfriGetAddressBookUrlDispatchTask(asyncCallback, asyncState, protocolRequestInfo, clientBinding, context, flags, hostname, userDn));
		}

		public RfriStatus EndGetAddressBookUrl(ICancelableAsyncResult asyncResult, out string serverUrl)
		{
			string localServerUrl = null;
			RfriStatus result;
			try
			{
				result = this.EndWrapper("EndGetAddressBookUrl", asyncResult, (RfriDispatchTask task) => ((RfriGetAddressBookUrlDispatchTask)task).End(out localServerUrl));
			}
			finally
			{
				serverUrl = localServerUrl;
			}
			return result;
		}

		public ICancelableAsyncResult BeginGetMailboxUrl(ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, RfriGetMailboxUrlFlags flags, string hostname, string serverDn, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return this.BeginWrapper("BeginGetMailboxUrl", asyncCallback, asyncState, clientBinding, serverDn, (RfriContext context) => new RfriGetMailboxUrlDispatchTask(asyncCallback, asyncState, protocolRequestInfo, clientBinding, context, flags, hostname, serverDn));
		}

		public RfriStatus EndGetMailboxUrl(ICancelableAsyncResult asyncResult, out string serverUrl)
		{
			string localServerUrl = null;
			RfriStatus result;
			try
			{
				result = this.EndWrapper("EndGetMailboxUrl", asyncResult, (RfriDispatchTask task) => ((RfriGetMailboxUrlDispatchTask)task).End(out localServerUrl));
			}
			finally
			{
				serverUrl = localServerUrl;
			}
			return result;
		}

		internal void ShuttingDown()
		{
			this.isShuttingDown = true;
		}

		private static void FailureCallback(object state)
		{
			FailureAsyncResult<RfriStatus> failureAsyncResult = (FailureAsyncResult<RfriStatus>)state;
			failureAsyncResult.InvokeCallback();
		}

		private static void ConditionalExceptionWrapper(bool wrapException, Action wrappedAction, Action<Exception> exceptionDelegate)
		{
			if (wrapException)
			{
				try
				{
					wrappedAction();
					return;
				}
				catch (Exception obj)
				{
					if (exceptionDelegate != null)
					{
						exceptionDelegate(obj);
					}
					throw;
				}
			}
			wrappedAction();
		}

		private static RfriContext CreateRfriContext(ClientBinding clientBinding)
		{
			RfriContext rfriContext = null;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				ClientSecurityContext clientSecurityContext = null;
				bool isAnonymous = false;
				string text = null;
				string userDomain = null;
				RpcHttpConnectionProperties rpcHttpConnectionProperties = null;
				if (!RpcDispatch.TryGetAuthContextInfo(clientBinding, out clientSecurityContext, out isAnonymous, out text, out userDomain, out rpcHttpConnectionProperties))
				{
					ExTraceGlobals.ReferralTracer.TraceError<Guid>(0L, "Could not resolve anonymous user for session id: {0}", clientBinding.AssociationGuid);
					throw new RfriException(RfriStatus.LogonFailed, "Could not resolve anonymous user.");
				}
				disposeGuard.Add<ClientSecurityContext>(clientSecurityContext);
				Guid empty = Guid.Empty;
				if (rpcHttpConnectionProperties != null && rpcHttpConnectionProperties.RequestIds.Length > 0)
				{
					Guid.TryParse(rpcHttpConnectionProperties.RequestIds[rpcHttpConnectionProperties.RequestIds.Length - 1], out empty);
				}
				rfriContext = new RfriContext(clientSecurityContext, userDomain, clientBinding.ClientAddress, clientBinding.ServerAddress, clientBinding.ProtocolSequence, clientBinding.AuthenticationType.ToString(), clientBinding.IsEncrypted, isAnonymous, empty);
				disposeGuard.Add<RfriContext>(rfriContext);
				if (!rfriContext.TryAcquireBudget())
				{
					ExTraceGlobals.ReferralTracer.TraceError((long)rfriContext.ContextHandle, "Could not acquire budget");
					throw new RfriException(RfriStatus.GeneralFailure, "Failed to acquire budget.");
				}
				disposeGuard.Success();
			}
			return rfriContext;
		}

		private void SubmitTask(RfriDispatchTask task)
		{
			this.CheckShuttingDown();
			if (!UserWorkloadManager.Singleton.TrySubmitNewTask(task))
			{
				ExTraceGlobals.ReferralTracer.TraceError((long)task.ContextHandle, "Could not submit task");
				throw new ServerTooBusyException("Unable to submit task; queue full");
			}
		}

		private void CheckShuttingDown()
		{
			if (this.isShuttingDown)
			{
				throw new ServerUnavailableException("Shutting down");
			}
		}

		private ICancelableAsyncResult BeginWrapper(string methodName, CancelableAsyncCallback asyncCallback, object asyncState, ClientBinding clientBinding, string legacyDn, Func<RfriContext, RfriDispatchTask> beginDelegate)
		{
			ICancelableAsyncResult asyncResult = null;
			RfriAsyncDispatch.ConditionalExceptionWrapper(ExTraceGlobals.ReferralTracer.IsTraceEnabled(TraceType.DebugTrace), delegate
			{
				if (ExTraceGlobals.ReferralTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					using (ClientSecurityContext clientSecurityContext = clientBinding.GetClientSecurityContext())
					{
						ExTraceGlobals.ReferralTracer.TraceDebug(0, 0L, "{0} started. LegacyDn={1}. ClientAddress={2}. ServerAddress={3}. ProtocolSequence={4}. EndPoint={5}. IsEncrypted={6}. ClientSecurityContext={7}.", new object[]
						{
							methodName,
							legacyDn,
							clientBinding.ClientAddress,
							clientBinding.ServerAddress,
							clientBinding.ProtocolSequence,
							clientBinding.ClientEndpoint,
							clientBinding.IsEncrypted,
							clientSecurityContext
						});
					}
				}
				FailureAsyncResult<RfriStatus> failureAsyncResult = null;
				this.CheckShuttingDown();
				try
				{
					using (DisposeGuard disposeGuard = default(DisposeGuard))
					{
						RfriContext rfriContext = RfriAsyncDispatch.CreateRfriContext(clientBinding);
						disposeGuard.Add<RfriContext>(rfriContext);
						RfriDispatchTask rfriDispatchTask = beginDelegate(rfriContext);
						disposeGuard.Add<RfriDispatchTask>(rfriDispatchTask);
						asyncResult = rfriDispatchTask.AsyncResult;
						this.SubmitTask(rfriDispatchTask);
						disposeGuard.Success();
					}
				}
				catch (FailRpcException ex)
				{
					failureAsyncResult = new FailureAsyncResult<RfriStatus>((RfriStatus)ex.ErrorCode, IntPtr.Zero, ex, asyncCallback, asyncState);
					asyncResult = failureAsyncResult;
				}
				catch (RfriException ex2)
				{
					failureAsyncResult = new FailureAsyncResult<RfriStatus>(ex2.Status, IntPtr.Zero, ex2, asyncCallback, asyncState);
					asyncResult = failureAsyncResult;
				}
				if (failureAsyncResult != null && !ThreadPool.QueueUserWorkItem(RfriAsyncDispatch.FailureWaitCallback, failureAsyncResult))
				{
					failureAsyncResult.InvokeCallback();
				}
				ExTraceGlobals.ReferralTracer.TraceDebug<string>(0, 0L, "{0} succeeded.", methodName);
			}, delegate(Exception exception)
			{
				ExTraceGlobals.ReferralTracer.TraceDebug<string, Exception>(0, 0L, "{0} failed. Exception={1}.", methodName, exception);
			});
			return asyncResult;
		}

		private RfriStatus EndWrapper(string methodName, ICancelableAsyncResult asyncResult, Func<RfriDispatchTask, RfriStatus> endDelegate)
		{
			RfriStatus rfriStatus = RfriStatus.Success;
			RfriAsyncDispatch.ConditionalExceptionWrapper(ExTraceGlobals.ReferralTracer.IsTraceEnabled(TraceType.DebugTrace), delegate
			{
				DispatchTaskAsyncResult dispatchTaskAsyncResult = asyncResult as DispatchTaskAsyncResult;
				if (dispatchTaskAsyncResult != null)
				{
					RfriDispatchTask rfriDispatchTask = (RfriDispatchTask)dispatchTaskAsyncResult.DispatchTask;
					using (DisposeGuard disposeGuard = default(DisposeGuard))
					{
						disposeGuard.Add<RfriDispatchTask>(rfriDispatchTask);
						rfriStatus = endDelegate(rfriDispatchTask);
					}
					ExTraceGlobals.ReferralTracer.TraceDebug<string, RfriStatus>(0, 0L, "{0} succeeded. RfriStatus={1}.", methodName, rfriStatus);
					return;
				}
				FailureAsyncResult<RfriStatus> failureAsyncResult = asyncResult as FailureAsyncResult<RfriStatus>;
				if (failureAsyncResult != null)
				{
					rfriStatus = failureAsyncResult.ErrorCode;
					ExTraceGlobals.ReferralTracer.TraceDebug<string, RfriStatus, Exception>(0, 0L, "{0} failed. RfriStatus={1}. Exception={2}.", methodName, rfriStatus, failureAsyncResult.Exception);
					return;
				}
				throw new InvalidOperationException(string.Format("Invalid IAsyncResult encountered; {0}", asyncResult));
			}, delegate(Exception exception)
			{
				ExTraceGlobals.ReferralTracer.TraceDebug<string, Exception>(0, 0L, "{0} failed. Exception={1}.", methodName, exception);
			});
			return rfriStatus;
		}

		private static readonly WaitCallback FailureWaitCallback = new WaitCallback(RfriAsyncDispatch.FailureCallback);

		private bool isShuttingDown;
	}
}
