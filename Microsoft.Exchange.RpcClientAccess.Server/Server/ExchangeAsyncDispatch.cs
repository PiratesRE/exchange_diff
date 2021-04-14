using System;
using System.Security.Principal;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.RpcClientAccess;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal sealed class ExchangeAsyncDispatch : IExchangeAsyncDispatch
	{
		public ExchangeAsyncDispatch(IExchangeDispatch exchangeDispatch, DispatchPool dispatchPool)
		{
			this.exchangeDispatch = exchangeDispatch;
			this.dispatchPool = dispatchPool;
		}

		public ICancelableAsyncResult BeginConnect(ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, string userDn, int flags, int conMod, int cpid, int lcidString, int lcidSort, short[] clientVersion, ArraySegment<byte> segmentExtendedAuxIn, ArraySegment<byte> segmentExtendedAuxOut, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return this.BeginWrapper("BeginConnect", IntPtr.Zero, asyncCallback, asyncState, delegate(IExchangeDispatch exchangeDispatch)
			{
				bool flag = false;
				IStandardBudget standardBudget = null;
				DispatchTask result;
				try
				{
					using (ClientSecurityContext clientSecurityContext = clientBinding.GetClientSecurityContext())
					{
						if (ExTraceGlobals.ExchangeAsyncDispatchTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							ExTraceGlobals.ExchangeAsyncDispatchTracer.TraceDebug(0, 0L, "BeginConnect started. UserDn={0}. Flags={1}. ConMod={2}. Cpid={3}. LcidString={4}. LcidSort={5}. ClientVersion={6}. SegmentExtendedAuxIn={7}. SegmentExtendedAuxOut={8}. ClientAddress={9}. ServerAddress={10}. ProtocolSequence={11}. EndPoint={12}. IsEncrypted={13}. ClientSecurityContext={14}.", new object[]
							{
								userDn,
								flags,
								conMod,
								cpid,
								lcidString,
								lcidSort,
								clientVersion,
								segmentExtendedAuxIn.Count,
								segmentExtendedAuxOut.Count,
								clientBinding.ClientAddress,
								clientBinding.ServerAddress,
								clientBinding.ProtocolSequence,
								clientBinding.ClientEndpoint,
								clientBinding.IsEncrypted,
								clientSecurityContext
							});
						}
						if (clientBinding.ProtocolSequence != null && clientBinding.ProtocolSequence.StartsWith(RpcDispatch.WebServiceProtocolSequencePrefix, StringComparison.OrdinalIgnoreCase))
						{
							string domain = clientBinding.ProtocolSequence.Substring(RpcDispatch.WebServiceProtocolSequencePrefix.Length);
							if (!ExchangeAsyncDispatch.TryAcquireBudget(userDn, clientSecurityContext, BudgetType.Cpa, domain, out standardBudget))
							{
								throw new FailRpcException("Failed to acquire budget.", 2415);
							}
						}
					}
					ConnectDispatchTask connectDispatchTask = new ConnectDispatchTask(exchangeDispatch, asyncCallback, asyncState, protocolRequestInfo, clientBinding, userDn, flags, conMod, cpid, lcidString, lcidSort, clientVersion, segmentExtendedAuxIn, segmentExtendedAuxOut, standardBudget);
					flag = true;
					result = connectDispatchTask;
				}
				finally
				{
					if (!flag)
					{
						Util.DisposeIfPresent(standardBudget);
					}
				}
				return result;
			});
		}

		public int EndConnect(ICancelableAsyncResult asyncResult, out IntPtr contextHandle, out TimeSpan pollsMax, out int retryCount, out TimeSpan retryDelay, out string dnPrefix, out string displayName, out short[] serverVersion, out ArraySegment<byte> segmentExtendedAuxOut)
		{
			IntPtr localContextHandle = IntPtr.Zero;
			TimeSpan localPollsMax = TimeSpan.Zero;
			int localRetryCount = 0;
			TimeSpan localRetryDelay = TimeSpan.Zero;
			string localDnPrefix = string.Empty;
			string localDisplayName = string.Empty;
			short[] localServerVersion = ExchangeDispatch.ExchangeServerVersion;
			ArraySegment<byte> localSegmentExtendedAuxOut = Array<byte>.EmptySegment;
			int result;
			try
			{
				result = this.EndWrapper("EndConnect", asyncResult, delegate(DispatchTask task)
				{
					int num = ((ConnectDispatchTask)task).End(out localContextHandle, out localPollsMax, out localRetryCount, out localRetryDelay, out localDnPrefix, out localDisplayName, out localServerVersion, out localSegmentExtendedAuxOut);
					if (ExTraceGlobals.ExchangeAsyncDispatchTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.ExchangeAsyncDispatchTracer.TraceDebug(0, 0L, "EndConnect succeeded. ContextHandle={0:X}. PollsMax={1}. RetryCount={2}. RetryDelay={3}. DnPrefix={4}. DisplayName={5}. ServerVersion={6}. SegmentExtendedAuxOut={7}. ErrorCode={8}. {9}", new object[]
						{
							localContextHandle.ToInt64(),
							localPollsMax,
							localRetryCount,
							localRetryDelay,
							localDnPrefix,
							localDisplayName,
							localServerVersion,
							localSegmentExtendedAuxOut.Count,
							num,
							(localContextHandle != IntPtr.Zero) ? "Context Created" : string.Empty
						});
					}
					return num;
				});
			}
			finally
			{
				contextHandle = localContextHandle;
				pollsMax = localPollsMax;
				retryCount = localRetryCount;
				retryDelay = localRetryDelay;
				dnPrefix = localDnPrefix;
				displayName = localDisplayName;
				serverVersion = localServerVersion;
				segmentExtendedAuxOut = localSegmentExtendedAuxOut;
			}
			return result;
		}

		public ICancelableAsyncResult BeginDisconnect(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return this.BeginWrapper("BeginDisconnect", contextHandle, asyncCallback, asyncState, delegate(IExchangeDispatch exchangeDispatch)
			{
				if (ExTraceGlobals.ExchangeAsyncDispatchTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.ExchangeAsyncDispatchTracer.TraceDebug<long>(0, 0L, "BeginDisconnect started. ContextHandle={0:X}.", contextHandle.ToInt64());
				}
				return new DisconnectDispatchTask(exchangeDispatch, asyncCallback, asyncState, protocolRequestInfo, contextHandle);
			});
		}

		public int EndDisconnect(ICancelableAsyncResult asyncResult, out IntPtr contextHandle)
		{
			IntPtr localContextHandle = IntPtr.Zero;
			int result;
			try
			{
				result = this.EndWrapper("EndDisconnect", asyncResult, delegate(DispatchTask task)
				{
					int num = ((DisconnectDispatchTask)task).End(out localContextHandle);
					if (ExTraceGlobals.ExchangeAsyncDispatchTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.ExchangeAsyncDispatchTracer.TraceDebug<long, int, string>(0, 0L, "EndDisconnect succeeded. ContextHandle={0:X}. ErrorCode={1}. {2}", task.ContextHandle.ToInt64(), num, (localContextHandle == IntPtr.Zero) ? "Context Dropped" : string.Empty);
					}
					return num;
				});
			}
			finally
			{
				contextHandle = localContextHandle;
			}
			return result;
		}

		public ICancelableAsyncResult BeginExecute(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, int flags, ArraySegment<byte> segmentExtendedRopIn, ArraySegment<byte> segmentExtendedRopOut, ArraySegment<byte> segmentExtendedAuxIn, ArraySegment<byte> segmentExtendedAuxOut, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return this.BeginWrapper("BeginExecute", contextHandle, asyncCallback, asyncState, delegate(IExchangeDispatch exchangeDispatch)
			{
				if (ExTraceGlobals.ExchangeAsyncDispatchTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.ExchangeAsyncDispatchTracer.TraceDebug(0, 0L, "BeginExecute started. ContextHandle={0:X}. Flags={1}. SegmentExtendedRopIn={2}. SegmentExtendedRopOut={3}. SegmentExtendedAuxIn={4}. SegmentExtendedAuxOut={5}.", new object[]
					{
						contextHandle.ToInt64(),
						flags,
						segmentExtendedRopIn.Count,
						segmentExtendedRopOut.Count,
						segmentExtendedAuxIn.Count,
						segmentExtendedAuxOut.Count
					});
				}
				return new ExecuteDispatchTask(exchangeDispatch, asyncCallback, asyncState, protocolRequestInfo, contextHandle, flags, segmentExtendedRopIn, segmentExtendedRopOut, segmentExtendedAuxIn, segmentExtendedAuxOut);
			});
		}

		public int EndExecute(ICancelableAsyncResult asyncResult, out IntPtr contextHandle, out ArraySegment<byte> segmentExtendedRopOut, out ArraySegment<byte> segmentExtendedAuxOut)
		{
			IntPtr localContextHandle = IntPtr.Zero;
			ArraySegment<byte> localSegmentExtendedRopOut = Array<byte>.EmptySegment;
			ArraySegment<byte> localSegmentExtendedAuxOut = Array<byte>.EmptySegment;
			int result;
			try
			{
				result = this.EndWrapper("EndExecute", asyncResult, delegate(DispatchTask task)
				{
					int num = ((ExecuteDispatchTask)task).End(out localContextHandle, out localSegmentExtendedRopOut, out localSegmentExtendedAuxOut);
					if (ExTraceGlobals.ExchangeAsyncDispatchTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.ExchangeAsyncDispatchTracer.TraceDebug(0, 0L, "EndExecute succeeded. ContextHandle={0:X}. SegmentExtendedRopOut={1}. SegmentExtendedAuxOut={2}. ErrorCode={3}. {4}", new object[]
						{
							task.ContextHandle.ToInt64(),
							localSegmentExtendedRopOut.Count,
							localSegmentExtendedAuxOut.Count,
							num,
							(localContextHandle == IntPtr.Zero) ? "Context Dropped" : string.Empty
						});
					}
					return num;
				});
			}
			finally
			{
				contextHandle = localContextHandle;
				segmentExtendedRopOut = localSegmentExtendedRopOut;
				segmentExtendedAuxOut = localSegmentExtendedAuxOut;
			}
			return result;
		}

		public ICancelableAsyncResult BeginNotificationConnect(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return this.BeginWrapper("BeginNotificationConnect", contextHandle, asyncCallback, asyncState, delegate(IExchangeDispatch exchangeDispatch)
			{
				if (ExTraceGlobals.ExchangeAsyncDispatchTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.ExchangeAsyncDispatchTracer.TraceDebug<long>(0, 0L, "BeginNotificationConnect started. ContextHandle={0:X}.", contextHandle.ToInt64());
				}
				return new NotificationConnectDispatchTask(exchangeDispatch, asyncCallback, asyncState, protocolRequestInfo, contextHandle);
			});
		}

		public int EndNotificationConnect(ICancelableAsyncResult asyncResult, out IntPtr notificationContextHandle)
		{
			IntPtr localNotificationContextHandle = IntPtr.Zero;
			int result;
			try
			{
				result = this.EndWrapper("EndNotificationConnect", asyncResult, delegate(DispatchTask task)
				{
					int num = ((NotificationConnectDispatchTask)task).End(out localNotificationContextHandle);
					if (ExTraceGlobals.ExchangeAsyncDispatchTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.ExchangeAsyncDispatchTracer.TraceDebug<long, long, int>(0, 0L, "EndNotificationConnect succeeded. ContextHandle={0:X}. NotificationContextHandle={1:X}. ErrorCode={2}.", task.ContextHandle.ToInt64(), localNotificationContextHandle.ToInt64(), num);
					}
					return num;
				});
			}
			finally
			{
				notificationContextHandle = localNotificationContextHandle;
			}
			return result;
		}

		public ICancelableAsyncResult BeginNotificationWait(ProtocolRequestInfo protocolRequestInfo, IntPtr notificationContextHandle, int flagsIn, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return this.BeginWrapper("BeginNotificationWait", notificationContextHandle, asyncCallback, asyncState, delegate(IExchangeDispatch exchangeDispatch)
			{
				if (ExTraceGlobals.ExchangeAsyncDispatchTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.ExchangeAsyncDispatchTracer.TraceDebug<long>(0, 0L, "BeginNotificationWait started. NotificationContextHandle={0:X}.", notificationContextHandle.ToInt64());
				}
				return new NotificationWaitDispatchTask(exchangeDispatch, asyncCallback, asyncState, protocolRequestInfo, notificationContextHandle, flagsIn);
			});
		}

		public int EndNotificationWait(ICancelableAsyncResult asyncResult, out int flagsOut)
		{
			int localFlagsOut = 0;
			int result;
			try
			{
				result = this.EndWrapper("EndNotificationWait", asyncResult, delegate(DispatchTask task)
				{
					int num = ((NotificationWaitDispatchTask)task).End(out localFlagsOut);
					if (ExTraceGlobals.ExchangeAsyncDispatchTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.ExchangeAsyncDispatchTracer.TraceDebug<long, int, int>(0, 0L, "EndNotificationWait succeeded. NotificationContextHandle={0:X}. FlagsOut={1}. ErrorCode={2}.", task.ContextHandle.ToInt64(), localFlagsOut, num);
					}
					return num;
				});
			}
			finally
			{
				flagsOut = localFlagsOut;
			}
			return result;
		}

		public ICancelableAsyncResult BeginRegisterPushNotification(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, ArraySegment<byte> segmentContext, int adviseBits, ArraySegment<byte> segmentClientBlob, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return this.BeginWrapper("BeginRegistrationPushNotification", contextHandle, asyncCallback, asyncState, delegate(IExchangeDispatch exchangeDispatch)
			{
				if (ExTraceGlobals.ExchangeAsyncDispatchTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.ExchangeAsyncDispatchTracer.TraceDebug<long>(0, 0L, "BeginRegisterPushNotification started. ContextHandle={0:X}.", contextHandle.ToInt64());
				}
				return new RegisterPushNotificationDispatchTask(exchangeDispatch, asyncCallback, asyncState, protocolRequestInfo, contextHandle, segmentContext, adviseBits, segmentClientBlob);
			});
		}

		public int EndRegisterPushNotification(ICancelableAsyncResult asyncResult, out IntPtr contextHandle, out int notificationHandle)
		{
			IntPtr localContextHandle = IntPtr.Zero;
			int localNotificationHandle = 0;
			int result;
			try
			{
				result = this.EndWrapper("EndRegisterPushNotification", asyncResult, delegate(DispatchTask task)
				{
					int num = ((RegisterPushNotificationDispatchTask)task).End(out localContextHandle, out localNotificationHandle);
					if (ExTraceGlobals.ExchangeAsyncDispatchTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.ExchangeAsyncDispatchTracer.TraceDebug<long, int, string>(0, 0L, "EndRegisterPushNotification succeeded. ContextHandle={0:X}. ErrorCode={1}. {2}", task.ContextHandle.ToInt64(), num, (localContextHandle == IntPtr.Zero) ? "Context Dropped" : string.Empty);
					}
					return num;
				});
			}
			finally
			{
				contextHandle = localContextHandle;
				notificationHandle = localNotificationHandle;
			}
			return result;
		}

		public ICancelableAsyncResult BeginUnregisterPushNotification(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, int notificationHandle, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return this.BeginWrapper("BeginUnregistrationPushNotification", contextHandle, asyncCallback, asyncState, delegate(IExchangeDispatch exchangeDispatch)
			{
				if (ExTraceGlobals.ExchangeAsyncDispatchTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.ExchangeAsyncDispatchTracer.TraceDebug<long>(0, 0L, "BeginUnregisterPushNotification started. ContextHandle={0:X}.", contextHandle.ToInt64());
				}
				return new UnregisterPushNotificationDispatchTask(exchangeDispatch, asyncCallback, asyncState, protocolRequestInfo, contextHandle, notificationHandle);
			});
		}

		public int EndUnregisterPushNotification(ICancelableAsyncResult asyncResult, out IntPtr contextHandle)
		{
			IntPtr localContextHandle = IntPtr.Zero;
			int result;
			try
			{
				result = this.EndWrapper("EndUnregisterPushNotification", asyncResult, delegate(DispatchTask task)
				{
					int num = ((UnregisterPushNotificationDispatchTask)task).End(out localContextHandle);
					if (ExTraceGlobals.ExchangeAsyncDispatchTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.ExchangeAsyncDispatchTracer.TraceDebug<long, int, string>(0, 0L, "EndUnregisterPushNotification succeeded. ContextHandle={0:X}. ErrorCode={1}. {2}", task.ContextHandle.ToInt64(), num, (localContextHandle == IntPtr.Zero) ? "Context Dropped" : string.Empty);
					}
					return num;
				});
			}
			finally
			{
				contextHandle = localContextHandle;
			}
			return result;
		}

		public ICancelableAsyncResult BeginDummy(ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return this.BeginWrapper("BeginDummy", IntPtr.Zero, asyncCallback, asyncState, delegate(IExchangeDispatch exchangeDispatch)
			{
				if (ExTraceGlobals.ExchangeAsyncDispatchTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.ExchangeAsyncDispatchTracer.TraceDebug(0, 0L, "BeginDummy started.");
				}
				return new DummyDispatchTask(exchangeDispatch, asyncCallback, asyncState, protocolRequestInfo, clientBinding);
			});
		}

		public int EndDummy(ICancelableAsyncResult asyncResult)
		{
			return this.EndWrapper("EndDummy", asyncResult, delegate(DispatchTask task)
			{
				int result = ((DummyDispatchTask)task).End();
				if (ExTraceGlobals.ExchangeAsyncDispatchTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.ExchangeAsyncDispatchTracer.TraceDebug(0, 0L, "EndDummy succeeded.");
				}
				return result;
			});
		}

		public void ContextHandleRundown(IntPtr contextHandle)
		{
			if (ExTraceGlobals.ExchangeAsyncDispatchTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.ExchangeAsyncDispatchTracer.TraceDebug<long>(0, 0L, "ContextHandleRundown called. ContextHandle={0:X}.", contextHandle.ToInt64());
			}
			IntPtr contextHandle2 = contextHandle;
			bool flag = false;
			try
			{
				ExchangeAsyncDispatch.ExecuteAndIgnore("ContextHandleRundown", delegate
				{
					if (!this.IsShuttingDown)
					{
						IntPtr contextHandle3 = contextHandle;
						this.exchangeDispatch.Disconnect(null, ref contextHandle3, true);
					}
				});
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					ExTraceGlobals.ExchangeAsyncDispatchTracer.TraceDebug<long>(0, 0L, "ContextHandleRundown failed. ContextHandle={0:X}.", contextHandle2.ToInt64());
				}
			}
		}

		public void NotificationContextHandleRundown(IntPtr notificationContextHandle)
		{
			if (ExTraceGlobals.ExchangeAsyncDispatchTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.ExchangeAsyncDispatchTracer.TraceDebug<long>(0, 0L, "NotificationContextHandleRundown called. NotificationContextHandle={0:X}.", notificationContextHandle.ToInt64());
			}
		}

		public void DroppedConnection(IntPtr notificationContextHandle)
		{
			if (ExTraceGlobals.ExchangeAsyncDispatchTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.ExchangeAsyncDispatchTracer.TraceDebug<long>(0, 0L, "DroppedConnection called. NotificationContextHandle={0:X}.", notificationContextHandle.ToInt64());
			}
			IntPtr notificationContextHandle2 = notificationContextHandle;
			bool flag = false;
			try
			{
				ExchangeAsyncDispatch.ExecuteAndIgnore("DroppedConnection", delegate
				{
					if (!this.IsShuttingDown)
					{
						this.exchangeDispatch.DroppedConnection(notificationContextHandle);
					}
				});
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					ExTraceGlobals.ExchangeAsyncDispatchTracer.TraceDebug<long>(0, 0L, "DroppedConnection failed. NotificationContextHandle={0:X}.", notificationContextHandle2.ToInt64());
				}
			}
		}

		internal bool IsShuttingDown
		{
			get
			{
				return RpcClientAccessService.IsShuttingDown;
			}
		}

		private static void FailureCallback(object state)
		{
			FailureAsyncResult<int> failureAsyncResult = (FailureAsyncResult<int>)state;
			failureAsyncResult.InvokeCallback();
		}

		private static bool TryAcquireBudget(string userDn, ClientSecurityContext clientSecurityContext, BudgetType budgetType, string domain, out IStandardBudget budget)
		{
			bool flag = false;
			Exception arg = null;
			budget = null;
			ADSessionSettings settings;
			if (!string.IsNullOrEmpty(domain))
			{
				settings = ADSessionSettings.RootOrgOrSingleTenantFromAcceptedDomainAutoDetect(domain);
			}
			else
			{
				settings = ADSessionSettings.FromRootOrgScopeSet();
			}
			try
			{
				budget = StandardBudget.Acquire(clientSecurityContext.UserSid, budgetType, settings);
				flag = true;
			}
			catch (DataValidationException ex)
			{
				arg = ex;
			}
			catch (ADTransientException ex2)
			{
				arg = ex2;
			}
			catch (ADOperationException ex3)
			{
				arg = ex3;
			}
			if (!flag)
			{
				ExTraceGlobals.ExchangeAsyncDispatchTracer.TraceError<string, SecurityIdentifier, Exception>(0, 0L, "Unable to acquire budget for user '{0}'. SID: {1}. {2}.", userDn, clientSecurityContext.UserSid, arg);
			}
			return flag;
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

		private static void ExecuteAndIgnore(string methodName, Action executeDelegate)
		{
			try
			{
				ExchangeAsyncDispatch.ConditionalExceptionWrapper(ExTraceGlobals.ExchangeAsyncDispatchTracer.IsTraceEnabled(TraceType.DebugTrace), delegate
				{
					executeDelegate();
				}, delegate(Exception exception)
				{
					ExTraceGlobals.ExchangeAsyncDispatchTracer.TraceDebug<string, Exception>(0, 0L, "{0} failed. Exception={1}.", methodName, exception);
				});
			}
			catch (FailRpcException)
			{
			}
			catch (RpcException)
			{
			}
			catch (RpcServiceException)
			{
			}
			catch (RpcServerException)
			{
			}
			catch (OutOfMemoryException)
			{
			}
		}

		private void SubmitTask(DispatchTask task)
		{
			if (!this.IsShuttingDown)
			{
				if (!this.dispatchPool.SubmitTask(task))
				{
					task.Completion(new ServerTooBusyException("Unable to submit task; queue full"), 0);
					return;
				}
			}
			else
			{
				task.Completion(new ServerUnavailableException("Shutting down"), 0);
			}
		}

		private void CheckShuttingDown()
		{
			if (this.IsShuttingDown)
			{
				throw new ServerUnavailableException("Shutting down");
			}
		}

		private ICancelableAsyncResult BeginWrapper(string methodName, IntPtr contextHandle, CancelableAsyncCallback asyncCallback, object asyncState, Func<IExchangeDispatch, DispatchTask> beginDelegate)
		{
			ICancelableAsyncResult asyncResult = null;
			ExchangeAsyncDispatch.ConditionalExceptionWrapper(ExTraceGlobals.ExchangeAsyncDispatchTracer.IsTraceEnabled(TraceType.DebugTrace), delegate
			{
				this.CheckShuttingDown();
				try
				{
					DispatchTask dispatchTask = beginDelegate(this.exchangeDispatch);
					asyncResult = dispatchTask;
					this.SubmitTask(dispatchTask);
				}
				catch (FailRpcException ex)
				{
					FailureAsyncResult<int> failureAsyncResult = new FailureAsyncResult<int>(ex.ErrorCode, contextHandle, ex, asyncCallback, asyncState);
					asyncResult = failureAsyncResult;
					if (!ThreadPool.QueueUserWorkItem(ExchangeAsyncDispatch.FailureWaitCallback, failureAsyncResult))
					{
						failureAsyncResult.InvokeCallback();
					}
				}
				if (ExTraceGlobals.ExchangeAsyncDispatchTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.ExchangeAsyncDispatchTracer.TraceDebug<string, long>(0, 0L, "{0} succeeded. ContextHandle={1:X}.", methodName, contextHandle.ToInt64());
				}
			}, delegate(Exception exception)
			{
				ExTraceGlobals.ExchangeAsyncDispatchTracer.TraceDebug<string, long, Exception>(0, 0L, "{0} failed. ContextHandle={1:X}. Exception={2}.", methodName, contextHandle.ToInt64(), exception);
			});
			return asyncResult;
		}

		private int EndWrapper(string methodName, ICancelableAsyncResult asyncResult, Func<DispatchTask, int> endDelegate)
		{
			IntPtr contextHandle = IntPtr.Zero;
			int errorCode = 0;
			ExchangeAsyncDispatch.ConditionalExceptionWrapper(ExTraceGlobals.ExchangeAsyncDispatchTracer.IsTraceEnabled(TraceType.DebugTrace), delegate
			{
				DispatchTask dispatchTask = asyncResult as DispatchTask;
				if (dispatchTask != null)
				{
					contextHandle = dispatchTask.ContextHandle;
					errorCode = endDelegate(dispatchTask);
					return;
				}
				FailureAsyncResult<int> failureAsyncResult = asyncResult as FailureAsyncResult<int>;
				if (failureAsyncResult == null)
				{
					throw new InvalidOperationException(string.Format("Invalid IAsyncResult encountered; {0}", asyncResult));
				}
				errorCode = failureAsyncResult.ErrorCode;
				contextHandle = failureAsyncResult.ContextHandle;
				if (ExTraceGlobals.ExchangeAsyncDispatchTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.ExchangeAsyncDispatchTracer.TraceDebug(0, 0L, "{0} failed. ContextHandle={1:X}. ErrorCode={2}. Exception={3}.", new object[]
					{
						methodName,
						contextHandle.ToInt64(),
						errorCode,
						failureAsyncResult.Exception
					});
					return;
				}
			}, delegate(Exception exception)
			{
				ExTraceGlobals.ExchangeAsyncDispatchTracer.TraceDebug<string, long, Exception>(0, 0L, "{0} failed. ContextHandle={1:X}. Exception={2}.", methodName, contextHandle.ToInt64(), exception);
			});
			return errorCode;
		}

		private static readonly WaitCallback FailureWaitCallback = new WaitCallback(ExchangeAsyncDispatch.FailureCallback);

		private readonly IExchangeDispatch exchangeDispatch;

		private readonly DispatchPool dispatchPool;
	}
}
