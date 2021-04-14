using System;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.AddressBook.Service;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal sealed class NspiAsyncDispatch : INspiAsyncDispatch
	{
		public ICancelableAsyncResult BeginBind(ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, NspiBindFlags flags, NspiState state, Guid? guid, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return this.BeginWrapper("BeginBind", asyncCallback, asyncState, true, (NspiContext context) => new NspiBindDispatchTask(asyncCallback, asyncState, protocolRequestInfo, clientBinding, context, flags, state, guid), () => ClientContextCache.CreateContext(clientBinding));
		}

		public NspiStatus EndBind(ICancelableAsyncResult asyncResult, out Guid? guid, out IntPtr contextHandle)
		{
			Guid? localGuid = new Guid?(default(Guid));
			int localContextHandle = 0;
			NspiStatus result;
			try
			{
				result = this.EndWrapper("EndBind", asyncResult, true, (NspiDispatchTask task) => ((NspiBindDispatchTask)task).End(out localGuid, out localContextHandle));
			}
			finally
			{
				guid = localGuid;
				contextHandle = new IntPtr(localContextHandle);
			}
			return result;
		}

		public ICancelableAsyncResult BeginUnbind(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiUnbindFlags flags, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return this.BeginWrapper("BeginUnbind", asyncCallback, asyncState, true, (NspiContext context) => new NspiUnbindDispatchTask(asyncCallback, asyncState, protocolRequestInfo, context, flags), () => ClientContextCache.GetContext(contextHandle.ToInt32()));
		}

		public NspiStatus EndUnbind(ICancelableAsyncResult asyncResult, out IntPtr contextHandle)
		{
			int localContextHandle = 0;
			NspiStatus result;
			try
			{
				result = this.EndWrapper("EndUnbind", asyncResult, true, (NspiDispatchTask task) => ((NspiUnbindDispatchTask)task).End(out localContextHandle));
			}
			finally
			{
				contextHandle = new IntPtr(localContextHandle);
			}
			return result;
		}

		public ICancelableAsyncResult BeginUpdateStat(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiUpdateStatFlags flags, NspiState state, bool deltaRequested, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return this.BeginContextWrapper("BeginUpdateStat", asyncCallback, asyncState, contextHandle, (NspiContext context) => new NspiUpdateStatDispatchTask(asyncCallback, asyncState, protocolRequestInfo, context, flags, state, deltaRequested));
		}

		public NspiStatus EndUpdateStat(ICancelableAsyncResult asyncResult, out NspiState state, out int? delta)
		{
			NspiState localState = null;
			int? localDelta = null;
			NspiStatus result;
			try
			{
				result = this.EndContextWrapper("EndUpdateStat", asyncResult, (NspiDispatchTask task) => ((NspiUpdateStatDispatchTask)task).End(out localState, out localDelta));
			}
			finally
			{
				state = localState;
				delta = localDelta;
			}
			return result;
		}

		public ICancelableAsyncResult BeginQueryRows(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiQueryRowsFlags flags, NspiState state, int[] mids, int rowCount, PropertyTag[] propTags, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return this.BeginContextWrapper("BeginQueryRows", asyncCallback, asyncState, contextHandle, (NspiContext context) => new NspiQueryRowsDispatchTask(asyncCallback, asyncState, protocolRequestInfo, context, flags, state, mids, rowCount, propTags));
		}

		public NspiStatus EndQueryRows(ICancelableAsyncResult asyncResult, out NspiState state, out PropertyValue[][] rowset)
		{
			NspiState localState = null;
			PropertyValue[][] localRowset = null;
			NspiStatus result;
			try
			{
				result = this.EndContextWrapper("EndQueryRows", asyncResult, (NspiDispatchTask task) => ((NspiQueryRowsDispatchTask)task).End(out localState, out localRowset));
			}
			finally
			{
				state = localState;
				rowset = localRowset;
			}
			return result;
		}

		public ICancelableAsyncResult BeginSeekEntries(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiSeekEntriesFlags flags, NspiState state, PropertyValue? target, int[] restriction, PropertyTag[] propTags, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return this.BeginContextWrapper("BeginSeekEntries", asyncCallback, asyncState, contextHandle, (NspiContext context) => new NspiSeekEntriesDispatchTask(asyncCallback, asyncState, protocolRequestInfo, context, flags, state, target, restriction, propTags));
		}

		public NspiStatus EndSeekEntries(ICancelableAsyncResult asyncResult, out NspiState state, out PropertyValue[][] rowset)
		{
			NspiState localState = null;
			PropertyValue[][] localRowset = null;
			NspiStatus result;
			try
			{
				result = this.EndContextWrapper("EndSeekEntries", asyncResult, (NspiDispatchTask task) => ((NspiSeekEntriesDispatchTask)task).End(out localState, out localRowset));
			}
			finally
			{
				state = localState;
				rowset = localRowset;
			}
			return result;
		}

		public ICancelableAsyncResult BeginGetMatches(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiGetMatchesFlags flags, NspiState state, int[] mids, int interfaceOptions, Restriction restriction, IntPtr propName, int maxRows, PropertyTag[] propTags, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return this.BeginContextWrapper("BeginGetMatches", asyncCallback, asyncState, contextHandle, (NspiContext context) => new NspiGetMatchesDispatchTask(asyncCallback, asyncState, protocolRequestInfo, context, flags, state, mids, interfaceOptions, restriction, propName, maxRows, propTags));
		}

		public NspiStatus EndGetMatches(ICancelableAsyncResult asyncResult, out NspiState state, out int[] mids, out PropertyValue[][] rowset)
		{
			NspiState localState = null;
			int[] localMids = null;
			PropertyValue[][] localRowset = null;
			NspiStatus result;
			try
			{
				result = this.EndContextWrapper("EndGetMatches", asyncResult, (NspiDispatchTask task) => ((NspiGetMatchesDispatchTask)task).End(out localState, out localMids, out localRowset));
			}
			finally
			{
				state = localState;
				mids = localMids;
				rowset = localRowset;
			}
			return result;
		}

		public ICancelableAsyncResult BeginResortRestriction(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiResortRestrictionFlags flags, NspiState state, int[] mids, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return this.BeginContextWrapper("BeginResortRestriction", asyncCallback, asyncState, contextHandle, (NspiContext context) => new NspiResortRestrictionDispatchTask(asyncCallback, asyncState, protocolRequestInfo, context, flags, state, mids));
		}

		public NspiStatus EndResortRestriction(ICancelableAsyncResult asyncResult, out NspiState state, out int[] mids)
		{
			NspiState localState = null;
			int[] localMids = null;
			NspiStatus result;
			try
			{
				result = this.EndContextWrapper("EndResortRestriction", asyncResult, (NspiDispatchTask task) => ((NspiResortRestrictionDispatchTask)task).End(out localState, out localMids));
			}
			finally
			{
				state = localState;
				mids = localMids;
			}
			return result;
		}

		public ICancelableAsyncResult BeginDNToEph(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiDNToEphFlags flags, string[] legacyDNs, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return this.BeginContextWrapper("BeginDNToEph", asyncCallback, asyncState, contextHandle, (NspiContext context) => new NspiDNToEphDispatchTask(asyncCallback, asyncState, protocolRequestInfo, context, flags, legacyDNs));
		}

		public NspiStatus EndDNToEph(ICancelableAsyncResult asyncResult, out int[] mids)
		{
			int[] localMids = null;
			NspiStatus result;
			try
			{
				result = this.EndContextWrapper("EndDNToEph", asyncResult, (NspiDispatchTask task) => ((NspiDNToEphDispatchTask)task).End(out localMids));
			}
			finally
			{
				mids = localMids;
			}
			return result;
		}

		public ICancelableAsyncResult BeginGetPropList(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiGetPropListFlags flags, int mid, int codePage, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return this.BeginContextWrapper("BeginGetPropList", asyncCallback, asyncState, contextHandle, (NspiContext context) => new NspiGetPropListDispatchTask(asyncCallback, asyncState, protocolRequestInfo, context, flags, mid, codePage));
		}

		public NspiStatus EndGetPropList(ICancelableAsyncResult asyncResult, out PropertyTag[] propTags)
		{
			PropertyTag[] localPropTags = null;
			NspiStatus result;
			try
			{
				result = this.EndContextWrapper("EndGetPropList", asyncResult, (NspiDispatchTask task) => ((NspiGetPropListDispatchTask)task).End(out localPropTags));
			}
			finally
			{
				propTags = localPropTags;
			}
			return result;
		}

		public ICancelableAsyncResult BeginGetProps(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiGetPropsFlags flags, NspiState state, PropertyTag[] propTags, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return this.BeginContextWrapper("BeginGetProps", asyncCallback, asyncState, contextHandle, (NspiContext context) => new NspiGetPropsDispatchTask(asyncCallback, asyncState, protocolRequestInfo, context, flags, state, propTags));
		}

		public NspiStatus EndGetProps(ICancelableAsyncResult asyncResult, out int codePage, out PropertyValue[] row)
		{
			int localCodePage = 0;
			PropertyValue[] localRow = null;
			NspiStatus result;
			try
			{
				result = this.EndContextWrapper("EndGetProps", asyncResult, (NspiDispatchTask task) => ((NspiGetPropsDispatchTask)task).End(out localCodePage, out localRow));
			}
			finally
			{
				codePage = localCodePage;
				row = localRow;
			}
			return result;
		}

		public ICancelableAsyncResult BeginCompareDNTs(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiCompareDNTsFlags flags, NspiState state, int mid1, int mid2, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return this.BeginContextWrapper("BeginCompareDNTs", asyncCallback, asyncState, contextHandle, (NspiContext context) => new NspiCompareDNTsDispatchTask(asyncCallback, asyncState, protocolRequestInfo, context, flags, state, mid1, mid2));
		}

		public NspiStatus EndCompareDNTs(ICancelableAsyncResult asyncResult, out int result)
		{
			int localResult = 0;
			NspiStatus result2;
			try
			{
				result2 = this.EndContextWrapper("EndCompareDNTs", asyncResult, (NspiDispatchTask task) => ((NspiCompareDNTsDispatchTask)task).End(out localResult));
			}
			finally
			{
				result = localResult;
			}
			return result2;
		}

		public ICancelableAsyncResult BeginModProps(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiModPropsFlags flags, NspiState state, PropertyTag[] propTags, PropertyValue[] row, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return this.BeginContextWrapper("BeginModProps", asyncCallback, asyncState, contextHandle, (NspiContext context) => new NspiModPropsDispatchTask(asyncCallback, asyncState, protocolRequestInfo, context, flags, state, propTags, row));
		}

		public NspiStatus EndModProps(ICancelableAsyncResult asyncResult)
		{
			return this.EndContextWrapper("EndModProps", asyncResult, (NspiDispatchTask task) => ((NspiModPropsDispatchTask)task).End());
		}

		public ICancelableAsyncResult BeginGetHierarchyInfo(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiGetHierarchyInfoFlags flags, NspiState state, int version, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return this.BeginContextWrapper("BeginGetHierarchyInfo", asyncCallback, asyncState, contextHandle, (NspiContext context) => new NspiGetHierarchyInfoDispatchTask(asyncCallback, asyncState, protocolRequestInfo, context, flags, state, version));
		}

		public NspiStatus EndGetHierarchyInfo(ICancelableAsyncResult asyncResult, out int codePage, out int returnedVersion, out PropertyValue[][] rowset)
		{
			int localCodePage = 0;
			int localReturnedVersion = 0;
			PropertyValue[][] localRowset = null;
			NspiStatus result;
			try
			{
				result = this.EndContextWrapper("EndGetHierarchyInfo", asyncResult, (NspiDispatchTask task) => ((NspiGetHierarchyInfoDispatchTask)task).End(out localCodePage, out localReturnedVersion, out localRowset));
			}
			finally
			{
				codePage = localCodePage;
				returnedVersion = localReturnedVersion;
				rowset = localRowset;
			}
			return result;
		}

		public ICancelableAsyncResult BeginGetTemplateInfo(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiGetTemplateInfoFlags flags, int type, string dn, int codePage, int locale, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return this.BeginContextWrapper("BeginGetTemplateInfo", asyncCallback, asyncState, contextHandle, (NspiContext context) => new NspiGetTemplateInfoDispatchTask(asyncCallback, asyncState, protocolRequestInfo, context, flags, type, dn, codePage, locale));
		}

		public NspiStatus EndGetTemplateInfo(ICancelableAsyncResult asyncResult, out int codePage, out PropertyValue[] row)
		{
			int localCodePage = 0;
			PropertyValue[] localRow = null;
			NspiStatus result;
			try
			{
				result = this.EndContextWrapper("EndGetTemplateInfo", asyncResult, (NspiDispatchTask task) => ((NspiGetTemplateInfoDispatchTask)task).End(out localCodePage, out localRow));
			}
			finally
			{
				codePage = localCodePage;
				row = localRow;
			}
			return result;
		}

		public ICancelableAsyncResult BeginModLinkAtt(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiModLinkAttFlags flags, PropertyTag propTag, int mid, byte[][] entryIds, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return this.BeginContextWrapper("BeginModLinkAtt", asyncCallback, asyncState, contextHandle, (NspiContext context) => new NspiModLinkAttDispatchTask(asyncCallback, asyncState, protocolRequestInfo, context, flags, propTag, mid, entryIds));
		}

		public NspiStatus EndModLinkAtt(ICancelableAsyncResult asyncResult)
		{
			return this.EndContextWrapper("EndModLinkAtt", asyncResult, (NspiDispatchTask task) => ((NspiModLinkAttDispatchTask)task).End());
		}

		public ICancelableAsyncResult BeginDeleteEntries(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiDeleteEntriesFlags flags, int mid, byte[][] entryIds, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return this.BeginContextWrapper("BeginDeleteEntries", asyncCallback, asyncState, contextHandle, (NspiContext context) => new NspiDeleteEntriesDispatchTask(asyncCallback, asyncState, protocolRequestInfo, context, flags, mid, entryIds));
		}

		public NspiStatus EndDeleteEntries(ICancelableAsyncResult asyncResult)
		{
			return this.EndContextWrapper("EndDeleteEntries", asyncResult, (NspiDispatchTask task) => ((NspiDeleteEntriesDispatchTask)task).End());
		}

		public ICancelableAsyncResult BeginQueryColumns(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiQueryColumnsFlags flags, NspiQueryColumnsMapiFlags mapiFlags, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return this.BeginContextWrapper("BeginQueryColumns", asyncCallback, asyncState, contextHandle, (NspiContext context) => new NspiQueryColumnsDispatchTask(asyncCallback, asyncState, protocolRequestInfo, context, flags, mapiFlags));
		}

		public NspiStatus EndQueryColumns(ICancelableAsyncResult asyncResult, out PropertyTag[] columns)
		{
			PropertyTag[] localColumns = null;
			NspiStatus result;
			try
			{
				result = this.EndContextWrapper("EndQueryColumns", asyncResult, (NspiDispatchTask task) => ((NspiQueryColumnsDispatchTask)task).End(out localColumns));
			}
			finally
			{
				columns = localColumns;
			}
			return result;
		}

		public ICancelableAsyncResult BeginGetNamesFromIDs(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiGetNamesFromIDsFlags flags, Guid? guid, PropertyTag[] propTags, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return this.BeginContextWrapper("BeginGetNamesFromIDs", asyncCallback, asyncState, contextHandle, (NspiContext context) => new NspiGetNamesFromIDsDispatchTask(asyncCallback, asyncState, protocolRequestInfo, context, flags, guid, propTags));
		}

		public NspiStatus EndGetNamesFromIDs(ICancelableAsyncResult asyncResult, out PropertyTag[] propTags, out SafeRpcMemoryHandle namesHandle)
		{
			PropertyTag[] localPropTags = null;
			SafeRpcMemoryHandle localNamesHandle = null;
			NspiStatus result;
			try
			{
				result = this.EndContextWrapper("EndGetNamesFromIDs", asyncResult, (NspiDispatchTask task) => ((NspiGetNamesFromIDsDispatchTask)task).End(out localPropTags, out localNamesHandle));
			}
			finally
			{
				propTags = localPropTags;
				namesHandle = localNamesHandle;
			}
			return result;
		}

		public ICancelableAsyncResult BeginGetIDsFromNames(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiGetIDsFromNamesFlags flags, int mapiFlags, int nameCount, IntPtr names, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return this.BeginContextWrapper("BeginGetIDsFromNames", asyncCallback, asyncState, contextHandle, (NspiContext context) => new NspiGetIDsFromNamesDispatchTask(asyncCallback, asyncState, protocolRequestInfo, context, flags, mapiFlags, nameCount, names));
		}

		public NspiStatus EndGetIDsFromNames(ICancelableAsyncResult asyncResult, out PropertyTag[] propTags)
		{
			PropertyTag[] localPropTags = null;
			NspiStatus result;
			try
			{
				result = this.EndContextWrapper("EndGetIDsFromNames", asyncResult, (NspiDispatchTask task) => ((NspiGetIDsFromNamesDispatchTask)task).End(out localPropTags));
			}
			finally
			{
				propTags = localPropTags;
			}
			return result;
		}

		public ICancelableAsyncResult BeginResolveNames(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiResolveNamesFlags flags, NspiState state, PropertyTag[] propTags, byte[][] names, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return this.BeginContextWrapper("BeginResolveNames", asyncCallback, asyncState, contextHandle, (NspiContext context) => new NspiResolveNamesDispatchTask(asyncCallback, asyncState, protocolRequestInfo, context, flags, state, propTags, names));
		}

		public NspiStatus EndResolveNames(ICancelableAsyncResult asyncResult, out int codePage, out int[] mids, out PropertyValue[][] rowset)
		{
			int localCodePage = 0;
			int[] localMids = null;
			PropertyValue[][] localRowset = null;
			NspiStatus result;
			try
			{
				result = this.EndContextWrapper("EndResolveNames", asyncResult, (NspiDispatchTask task) => ((NspiResolveNamesDispatchTask)task).End(out localCodePage, out localMids, out localRowset));
			}
			finally
			{
				codePage = localCodePage;
				mids = localMids;
				rowset = localRowset;
			}
			return result;
		}

		public ICancelableAsyncResult BeginResolveNamesW(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiResolveNamesFlags flags, NspiState state, PropertyTag[] propTags, string[] names, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return this.BeginContextWrapper("BeginResolveNamesW", asyncCallback, asyncState, contextHandle, (NspiContext context) => new NspiResolveNamesWDispatchTask(asyncCallback, asyncState, protocolRequestInfo, context, flags, state, propTags, names));
		}

		public NspiStatus EndResolveNamesW(ICancelableAsyncResult asyncResult, out int codePage, out int[] mids, out PropertyValue[][] rowset)
		{
			int localCodePage = 0;
			int[] localMids = null;
			PropertyValue[][] localRowset = null;
			NspiStatus result;
			try
			{
				result = this.EndContextWrapper("EndResolveNamesW", asyncResult, (NspiDispatchTask task) => ((NspiResolveNamesWDispatchTask)task).End(out localCodePage, out localMids, out localRowset));
			}
			finally
			{
				codePage = localCodePage;
				mids = localMids;
				rowset = localRowset;
			}
			return result;
		}

		public void ContextHandleRundown(IntPtr contextHandle)
		{
			if (contextHandle == IntPtr.Zero)
			{
				return;
			}
			if (!this.isShuttingDown)
			{
				NspiAsyncDispatch.ExecuteAndIgnore("ContextHandleRundown", delegate
				{
					NspiContext nspiContext = null;
					using (DisposeGuard disposeGuard = default(DisposeGuard))
					{
						if (ClientContextCache.TryGetContext(contextHandle.ToInt32(), out nspiContext))
						{
							disposeGuard.Add<NspiContext>(nspiContext);
							ClientContextCache.DeleteContext(contextHandle.ToInt32());
							nspiContext.Unbind(true);
						}
					}
				});
			}
		}

		internal void ShuttingDown()
		{
			this.isShuttingDown = true;
		}

		private static void FailureCallback(object state)
		{
			FailureAsyncResult<NspiStatus> failureAsyncResult = (FailureAsyncResult<NspiStatus>)state;
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

		private static void ExecuteAndIgnore(string methodName, Action executeDelegate)
		{
			try
			{
				NspiAsyncDispatch.ConditionalExceptionWrapper(ExTraceGlobals.NspiTracer.IsTraceEnabled(TraceType.DebugTrace), delegate
				{
					executeDelegate();
				}, delegate(Exception exception)
				{
					ExTraceGlobals.NspiTracer.TraceDebug<string, Exception>(0, 0L, "{0} failed. Exception={1}.", methodName, exception);
				});
			}
			catch (RpcException)
			{
			}
			catch (ADTransientException)
			{
			}
			catch (ADOperationException)
			{
			}
			catch (DataValidationException)
			{
			}
			catch (StoragePermanentException)
			{
			}
			catch (StorageTransientException)
			{
			}
			catch (NspiException)
			{
			}
			catch (OutOfMemoryException)
			{
			}
		}

		private void SubmitTask(NspiDispatchTask task)
		{
			this.CheckShuttingDown();
			if (!UserWorkloadManager.Singleton.TrySubmitNewTask(task))
			{
				ExTraceGlobals.NspiTracer.TraceError((long)task.ContextHandle, "Could not submit task");
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

		private ICancelableAsyncResult BeginContextWrapper(string methodName, CancelableAsyncCallback asyncCallback, object asyncState, IntPtr contextHandle, Func<NspiContext, NspiDispatchTask> beginDelegate)
		{
			return this.BeginWrapper(methodName, asyncCallback, asyncState, false, beginDelegate, () => ClientContextCache.GetContext(contextHandle.ToInt32()));
		}

		private ICancelableAsyncResult BeginWrapper(string methodName, CancelableAsyncCallback asyncCallback, object asyncState, bool rundownContextOnFailure, Func<NspiContext, NspiDispatchTask> beginDelegate, Func<NspiContext> contextFactory)
		{
			int contextHandle = 0;
			ICancelableAsyncResult asyncResult = null;
			NspiAsyncDispatch.ConditionalExceptionWrapper(ExTraceGlobals.NspiTracer.IsTraceEnabled(TraceType.DebugTrace), delegate
			{
				bool flag = false;
				FailureAsyncResult<NspiStatus> failureAsyncResult = null;
				try
				{
					this.CheckShuttingDown();
					try
					{
						using (DisposeGuard disposeGuard = default(DisposeGuard))
						{
							NspiContext nspiContext = contextFactory();
							contextHandle = ((nspiContext != null) ? nspiContext.ContextHandle : 0);
							NspiDispatchTask nspiDispatchTask = beginDelegate(nspiContext);
							disposeGuard.Add<NspiDispatchTask>(nspiDispatchTask);
							asyncResult = nspiDispatchTask.AsyncResult;
							this.SubmitTask(nspiDispatchTask);
							disposeGuard.Success();
						}
						flag = true;
					}
					catch (FailRpcException ex)
					{
						failureAsyncResult = new FailureAsyncResult<NspiStatus>((NspiStatus)ex.ErrorCode, new IntPtr(contextHandle), ex, asyncCallback, asyncState);
						asyncResult = failureAsyncResult;
					}
					catch (NspiException ex2)
					{
						failureAsyncResult = new FailureAsyncResult<NspiStatus>(ex2.Status, new IntPtr(contextHandle), ex2, asyncCallback, asyncState);
						asyncResult = failureAsyncResult;
					}
					if (failureAsyncResult != null && !ThreadPool.QueueUserWorkItem(NspiAsyncDispatch.FailureWaitCallback, failureAsyncResult))
					{
						failureAsyncResult.InvokeCallback();
					}
					ExTraceGlobals.NspiTracer.TraceDebug<string, int>(0, 0L, "{0} succeeded. ContextHandle={1}", methodName, contextHandle);
				}
				finally
				{
					if (!flag && rundownContextOnFailure && contextHandle != 0)
					{
						this.ContextHandleRundown(new IntPtr(contextHandle));
					}
				}
			}, delegate(Exception exception)
			{
				ExTraceGlobals.NspiTracer.TraceDebug<string, int, Exception>(0, 0L, "{0} failed. ContextHandle={1}, Exception={2}.", methodName, contextHandle, exception);
			});
			return asyncResult;
		}

		private NspiStatus EndContextWrapper(string methodName, ICancelableAsyncResult asyncResult, Func<NspiDispatchTask, NspiStatus> endDelegate)
		{
			return this.EndWrapper(methodName, asyncResult, false, endDelegate);
		}

		private NspiStatus EndWrapper(string methodName, ICancelableAsyncResult asyncResult, bool rundownContextOnFailure, Func<NspiDispatchTask, NspiStatus> endDelegate)
		{
			int contextHandle = 0;
			NspiStatus nspiStatus = NspiStatus.Success;
			NspiAsyncDispatch.ConditionalExceptionWrapper(ExTraceGlobals.NspiTracer.IsTraceEnabled(TraceType.DebugTrace), delegate
			{
				bool flag = false;
				try
				{
					DispatchTaskAsyncResult dispatchTaskAsyncResult = asyncResult as DispatchTaskAsyncResult;
					if (dispatchTaskAsyncResult != null)
					{
						NspiDispatchTask nspiDispatchTask = (NspiDispatchTask)dispatchTaskAsyncResult.DispatchTask;
						contextHandle = nspiDispatchTask.ContextHandle;
						using (DisposeGuard disposeGuard = default(DisposeGuard))
						{
							disposeGuard.Add<NspiDispatchTask>(nspiDispatchTask);
							try
							{
								nspiStatus = endDelegate(nspiDispatchTask);
							}
							finally
							{
								if (nspiDispatchTask.IsContextRundown && contextHandle != 0)
								{
									this.ContextHandleRundown(new IntPtr(contextHandle));
									contextHandle = 0;
								}
							}
						}
						ExTraceGlobals.NspiTracer.TraceDebug<string, int, NspiStatus>(0, 0L, "{0} succeeded. ContextHandle={1} NspiStatus={2}.", methodName, contextHandle, nspiStatus);
						flag = true;
					}
					else
					{
						FailureAsyncResult<NspiStatus> failureAsyncResult = asyncResult as FailureAsyncResult<NspiStatus>;
						if (failureAsyncResult == null)
						{
							throw new InvalidOperationException(string.Format("Invalid IAsyncResult encountered; {0}", asyncResult));
						}
						nspiStatus = failureAsyncResult.ErrorCode;
						ExTraceGlobals.NspiTracer.TraceDebug(0, 0L, "{0} failed. ContextHandle={1} NspiStatus={2}. Exception={3}.", new object[]
						{
							methodName,
							failureAsyncResult.ContextHandle.ToInt32(),
							nspiStatus,
							failureAsyncResult.Exception
						});
					}
				}
				finally
				{
					if (!flag && rundownContextOnFailure && contextHandle != 0)
					{
						this.ContextHandleRundown(new IntPtr(contextHandle));
					}
				}
			}, delegate(Exception exception)
			{
				ExTraceGlobals.NspiTracer.TraceDebug<string, int, Exception>(0, 0L, "{0} failed. ContextHandle={1}, Exception={2}.", methodName, contextHandle, exception);
			});
			return nspiStatus;
		}

		private static readonly WaitCallback FailureWaitCallback = new WaitCallback(NspiAsyncDispatch.FailureCallback);

		private bool isShuttingDown;
	}
}
