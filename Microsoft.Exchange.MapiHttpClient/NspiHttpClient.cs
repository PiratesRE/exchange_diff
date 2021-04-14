using System;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.MapiHttp;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class NspiHttpClient : MapiHttpClient, INspiAsyncDispatch
	{
		public NspiHttpClient(MapiHttpBindingInfo bindingInfo) : base(bindingInfo)
		{
		}

		internal override string VdirPath
		{
			get
			{
				return MapiHttpEndpoints.VdirPathNspi;
			}
		}

		public ICancelableAsyncResult BeginBind(ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, NspiBindFlags flags, NspiState state, Guid? guid, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			IntPtr contextHandle = base.CreateNewContextHandle((IntPtr handle) => handle);
			return base.BeginWrapper<NspiBindClientAsyncOperation>(contextHandle, true, delegate(ClientSessionContext context)
			{
				NspiBindClientAsyncOperation nspiBindClientAsyncOperation = new NspiBindClientAsyncOperation(context, asyncCallback, asyncState);
				nspiBindClientAsyncOperation.Begin(new NspiBindRequest(flags, state, Array<byte>.EmptySegment));
				return nspiBindClientAsyncOperation;
			});
		}

		public NspiStatus EndBind(ICancelableAsyncResult asyncResult, out Guid? guid, out IntPtr contextHandle)
		{
			NspiBindResponse response = null;
			NspiStatus result = (NspiStatus)base.EndWrapper<NspiBindClientAsyncOperation>(asyncResult, false, true, out contextHandle, (NspiBindClientAsyncOperation operation) => operation.End(out response));
			guid = new Guid?(response.ServerGuid);
			return result;
		}

		public ICancelableAsyncResult BeginUnbind(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiUnbindFlags flags, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			ICancelableAsyncResult result;
			try
			{
				result = base.BeginWrapper<NspiUnbindClientAsyncOperation>(contextHandle, false, delegate(ClientSessionContext context)
				{
					NspiUnbindClientAsyncOperation nspiUnbindClientAsyncOperation = new NspiUnbindClientAsyncOperation(context, asyncCallback, asyncState);
					nspiUnbindClientAsyncOperation.Begin(new NspiUnbindRequest(flags, Array<byte>.EmptySegment));
					return nspiUnbindClientAsyncOperation;
				});
			}
			catch (ContextNotFoundException exception)
			{
				FailureAsyncResult<int> failureAsyncResult = new FailureAsyncResult<int>(0, IntPtr.Zero, exception, asyncCallback, asyncState);
				if (!ThreadPool.QueueUserWorkItem(delegate(object state)
				{
					FailureAsyncResult<int> failureAsyncResult2 = (FailureAsyncResult<int>)state;
					failureAsyncResult2.InvokeCallback();
				}, failureAsyncResult))
				{
					failureAsyncResult.InvokeCallback();
				}
				result = failureAsyncResult;
			}
			return result;
		}

		public NspiStatus EndUnbind(ICancelableAsyncResult asyncResult, out IntPtr contextHandle)
		{
			NspiStatus result;
			try
			{
				FailureAsyncResult<int> failureAsyncResult = asyncResult as FailureAsyncResult<int>;
				if (failureAsyncResult != null)
				{
					throw new AggregateException(new Exception[]
					{
						failureAsyncResult.Exception
					});
				}
				NspiUnbindResponse response = null;
				NspiStatus nspiStatus = (NspiStatus)base.EndWrapper<NspiUnbindClientAsyncOperation>(asyncResult, true, true, out contextHandle, (NspiUnbindClientAsyncOperation operation) => operation.End(out response));
				result = nspiStatus;
			}
			catch (AggregateException exception)
			{
				if (exception.FindException<ContextNotFoundException>() == null && exception.FindException<InvalidSequenceException>() == null)
				{
					throw;
				}
				contextHandle = IntPtr.Zero;
				result = NspiStatus.UnbindSuccess;
			}
			catch (ContextNotFoundException)
			{
				contextHandle = IntPtr.Zero;
				result = NspiStatus.UnbindSuccess;
			}
			catch (InvalidSequenceException)
			{
				contextHandle = IntPtr.Zero;
				result = NspiStatus.UnbindSuccess;
			}
			return result;
		}

		public ICancelableAsyncResult BeginUpdateStat(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiUpdateStatFlags flags, NspiState state, bool deltaRequested, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return base.BeginWrapper<NspiUpdateStatClientAsyncOperation>(contextHandle, false, delegate(ClientSessionContext context)
			{
				NspiUpdateStatClientAsyncOperation nspiUpdateStatClientAsyncOperation = new NspiUpdateStatClientAsyncOperation(context, asyncCallback, asyncState);
				nspiUpdateStatClientAsyncOperation.Begin(new NspiUpdateStatRequest(flags, state, deltaRequested, Array<byte>.EmptySegment));
				return nspiUpdateStatClientAsyncOperation;
			});
		}

		public NspiStatus EndUpdateStat(ICancelableAsyncResult asyncResult, out NspiState state, out int? delta)
		{
			NspiUpdateStatResponse response = null;
			IntPtr zero = IntPtr.Zero;
			NspiStatus result = (NspiStatus)base.EndWrapper<NspiUpdateStatClientAsyncOperation>(asyncResult, false, false, out zero, (NspiUpdateStatClientAsyncOperation operation) => operation.End(out response));
			state = response.State;
			delta = response.Delta;
			return result;
		}

		public ICancelableAsyncResult BeginQueryRows(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiQueryRowsFlags flags, NspiState state, int[] mids, int count, PropertyTag[] propertyTags, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return base.BeginWrapper<NspiQueryRowsClientAsyncOperation>(contextHandle, false, delegate(ClientSessionContext context)
			{
				NspiQueryRowsClientAsyncOperation nspiQueryRowsClientAsyncOperation = new NspiQueryRowsClientAsyncOperation(context, asyncCallback, asyncState);
				nspiQueryRowsClientAsyncOperation.Begin(new NspiQueryRowsRequest(flags, state, mids, (uint)count, propertyTags, Array<byte>.EmptySegment));
				return nspiQueryRowsClientAsyncOperation;
			});
		}

		public NspiStatus EndQueryRows(ICancelableAsyncResult asyncResult, out NspiState state, out PropertyValue[][] rowset)
		{
			NspiQueryRowsResponse response = null;
			IntPtr zero = IntPtr.Zero;
			NspiStatus result = (NspiStatus)base.EndWrapper<NspiQueryRowsClientAsyncOperation>(asyncResult, false, false, out zero, (NspiQueryRowsClientAsyncOperation operation) => operation.End(out response));
			state = response.State;
			rowset = response.PropertyValues;
			return result;
		}

		public ICancelableAsyncResult BeginSeekEntries(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiSeekEntriesFlags flags, NspiState state, PropertyValue? target, int[] restriction, PropertyTag[] propertyTags, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return base.BeginWrapper<NspiSeekEntriesClientAsyncOperation>(contextHandle, false, delegate(ClientSessionContext context)
			{
				NspiSeekEntriesClientAsyncOperation nspiSeekEntriesClientAsyncOperation = new NspiSeekEntriesClientAsyncOperation(context, asyncCallback, asyncState);
				nspiSeekEntriesClientAsyncOperation.Begin(new NspiSeekEntriesRequest(flags, state, target, restriction, propertyTags, Array<byte>.EmptySegment));
				return nspiSeekEntriesClientAsyncOperation;
			});
		}

		public NspiStatus EndSeekEntries(ICancelableAsyncResult asyncResult, out NspiState state, out PropertyValue[][] rowset)
		{
			NspiSeekEntriesResponse response = null;
			IntPtr zero = IntPtr.Zero;
			NspiStatus result = (NspiStatus)base.EndWrapper<NspiSeekEntriesClientAsyncOperation>(asyncResult, false, false, out zero, (NspiSeekEntriesClientAsyncOperation operation) => operation.End(out response));
			state = response.State;
			rowset = response.PropertyValues;
			return result;
		}

		public ICancelableAsyncResult BeginGetMatches(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiGetMatchesFlags flags, NspiState state, int[] mids, int interfaceOptions, Restriction restriction, IntPtr propName, int maxRows, PropertyTag[] propertyTags, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return base.BeginWrapper<NspiGetMatchesClientAsyncOperation>(contextHandle, false, delegate(ClientSessionContext context)
			{
				NspiGetMatchesClientAsyncOperation nspiGetMatchesClientAsyncOperation = new NspiGetMatchesClientAsyncOperation(context, asyncCallback, asyncState);
				nspiGetMatchesClientAsyncOperation.Begin(new NspiGetMatchesRequest(flags, state, mids, (uint)interfaceOptions, restriction, null, (uint)maxRows, propertyTags, Array<byte>.EmptySegment));
				return nspiGetMatchesClientAsyncOperation;
			});
		}

		public NspiStatus EndGetMatches(ICancelableAsyncResult asyncResult, out NspiState state, out int[] mIds, out PropertyValue[][] rowSet)
		{
			NspiGetMatchesResponse getMatchesResponse = null;
			IntPtr intPtr;
			ErrorCode result = base.EndWrapper<NspiGetMatchesClientAsyncOperation>(asyncResult, false, false, out intPtr, (NspiGetMatchesClientAsyncOperation operation) => operation.End(out getMatchesResponse));
			state = getMatchesResponse.State;
			mIds = getMatchesResponse.Matches;
			rowSet = getMatchesResponse.PropertyValues;
			return (NspiStatus)result;
		}

		public ICancelableAsyncResult BeginResortRestriction(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiResortRestrictionFlags flags, NspiState state, int[] mids, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return base.BeginWrapper<NspiResortRestrictionClientAsyncOperation>(contextHandle, false, delegate(ClientSessionContext context)
			{
				NspiResortRestrictionClientAsyncOperation nspiResortRestrictionClientAsyncOperation = new NspiResortRestrictionClientAsyncOperation(context, asyncCallback, asyncState);
				nspiResortRestrictionClientAsyncOperation.Begin(new NspiResortRestrictionRequest(flags, state, mids, Array<byte>.EmptySegment));
				return nspiResortRestrictionClientAsyncOperation;
			});
		}

		public NspiStatus EndResortRestriction(ICancelableAsyncResult asyncResult, out NspiState state, out int[] mids)
		{
			NspiResortRestrictionResponse response = null;
			IntPtr zero = IntPtr.Zero;
			NspiStatus result = (NspiStatus)base.EndWrapper<NspiResortRestrictionClientAsyncOperation>(asyncResult, false, false, out zero, (NspiResortRestrictionClientAsyncOperation operation) => operation.End(out response));
			state = response.State;
			mids = response.EphemeralIds;
			return result;
		}

		public ICancelableAsyncResult BeginDNToEph(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiDNToEphFlags flags, string[] dnValues, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return base.BeginWrapper<NspiDNToEphClientAsyncOperation>(contextHandle, false, delegate(ClientSessionContext context)
			{
				NspiDNToEphClientAsyncOperation nspiDNToEphClientAsyncOperation = new NspiDNToEphClientAsyncOperation(context, asyncCallback, asyncState);
				nspiDNToEphClientAsyncOperation.Begin(new NspiDnToEphRequest(flags, dnValues, Array<byte>.EmptySegment));
				return nspiDNToEphClientAsyncOperation;
			});
		}

		public NspiStatus EndDNToEph(ICancelableAsyncResult asyncResult, out int[] mids)
		{
			NspiDnToEphResponse response = null;
			IntPtr zero = IntPtr.Zero;
			NspiStatus result = (NspiStatus)base.EndWrapper<NspiDNToEphClientAsyncOperation>(asyncResult, false, false, out zero, (NspiDNToEphClientAsyncOperation operation) => operation.End(out response));
			mids = response.EphemeralIds;
			return result;
		}

		public ICancelableAsyncResult BeginGetPropList(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiGetPropListFlags flags, int mid, int codePage, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return base.BeginWrapper<NspiGetPropListClientAsyncOperation>(contextHandle, false, delegate(ClientSessionContext context)
			{
				NspiGetPropListClientAsyncOperation nspiGetPropListClientAsyncOperation = new NspiGetPropListClientAsyncOperation(context, asyncCallback, asyncState);
				nspiGetPropListClientAsyncOperation.Begin(new NspiGetPropListRequest(flags, mid, (uint)codePage, Array<byte>.EmptySegment));
				return nspiGetPropListClientAsyncOperation;
			});
		}

		public NspiStatus EndGetPropList(ICancelableAsyncResult asyncResult, out PropertyTag[] propertyTags)
		{
			NspiGetPropListResponse getPropListResponse = null;
			IntPtr intPtr;
			ErrorCode result = base.EndWrapper<NspiGetPropListClientAsyncOperation>(asyncResult, false, false, out intPtr, (NspiGetPropListClientAsyncOperation operation) => operation.End(out getPropListResponse));
			propertyTags = getPropListResponse.PropertyTags;
			return (NspiStatus)result;
		}

		public ICancelableAsyncResult BeginGetProps(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiGetPropsFlags flags, NspiState state, PropertyTag[] propertyTags, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return base.BeginWrapper<NspiGetPropsClientAsyncOperation>(contextHandle, false, delegate(ClientSessionContext context)
			{
				NspiGetPropsClientAsyncOperation nspiGetPropsClientAsyncOperation = new NspiGetPropsClientAsyncOperation(context, asyncCallback, asyncState);
				nspiGetPropsClientAsyncOperation.Begin(new NspiGetPropsRequest(flags, state, propertyTags, Array<byte>.EmptySegment));
				return nspiGetPropsClientAsyncOperation;
			});
		}

		public NspiStatus EndGetProps(ICancelableAsyncResult asyncResult, out int codePage, out PropertyValue[] row)
		{
			NspiGetPropsResponse getPropsResponse = null;
			IntPtr intPtr;
			ErrorCode result = base.EndWrapper<NspiGetPropsClientAsyncOperation>(asyncResult, false, false, out intPtr, (NspiGetPropsClientAsyncOperation operation) => operation.End(out getPropsResponse));
			codePage = (int)getPropsResponse.CodePage;
			row = getPropsResponse.PropertyValues;
			return (NspiStatus)result;
		}

		public ICancelableAsyncResult BeginCompareDNTs(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiCompareDNTsFlags flags, NspiState state, int mid1, int mid2, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return base.BeginWrapper<NspiCompareDNTsClientAsyncOperation>(contextHandle, false, delegate(ClientSessionContext context)
			{
				NspiCompareDNTsClientAsyncOperation nspiCompareDNTsClientAsyncOperation = new NspiCompareDNTsClientAsyncOperation(context, asyncCallback, asyncState);
				nspiCompareDNTsClientAsyncOperation.Begin(new NspiCompareDntsRequest(flags, state, mid1, mid2, Array<byte>.EmptySegment));
				return nspiCompareDNTsClientAsyncOperation;
			});
		}

		public NspiStatus EndCompareDNTs(ICancelableAsyncResult asyncResult, out int result)
		{
			NspiCompareDntsResponse response = null;
			IntPtr zero = IntPtr.Zero;
			NspiStatus result2 = (NspiStatus)base.EndWrapper<NspiCompareDNTsClientAsyncOperation>(asyncResult, false, false, out zero, (NspiCompareDNTsClientAsyncOperation operation) => operation.End(out response));
			result = response.Result;
			return result2;
		}

		public ICancelableAsyncResult BeginModProps(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiModPropsFlags flags, NspiState state, PropertyTag[] propertyTags, PropertyValue[] row, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return base.BeginWrapper<NspiModPropsClientAsyncOperation>(contextHandle, false, delegate(ClientSessionContext context)
			{
				NspiModPropsClientAsyncOperation nspiModPropsClientAsyncOperation = new NspiModPropsClientAsyncOperation(context, asyncCallback, asyncState);
				nspiModPropsClientAsyncOperation.Begin(new NspiModPropsRequest(flags, state, propertyTags, row, Array<byte>.EmptySegment));
				return nspiModPropsClientAsyncOperation;
			});
		}

		public NspiStatus EndModProps(ICancelableAsyncResult asyncResult)
		{
			NspiModPropsResponse modPropsResponse = null;
			IntPtr intPtr;
			return (NspiStatus)base.EndWrapper<NspiModPropsClientAsyncOperation>(asyncResult, false, false, out intPtr, (NspiModPropsClientAsyncOperation operation) => operation.End(out modPropsResponse));
		}

		public ICancelableAsyncResult BeginGetHierarchyInfo(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiGetHierarchyInfoFlags flags, NspiState state, int version, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return base.BeginWrapper<NspiGetSpecialTableClientAsyncOperation>(contextHandle, false, delegate(ClientSessionContext context)
			{
				NspiGetSpecialTableClientAsyncOperation nspiGetSpecialTableClientAsyncOperation = new NspiGetSpecialTableClientAsyncOperation(context, asyncCallback, asyncState);
				nspiGetSpecialTableClientAsyncOperation.Begin(new NspiGetSpecialTableRequest(flags, state, new uint?((uint)version), Array<byte>.EmptySegment));
				return nspiGetSpecialTableClientAsyncOperation;
			});
		}

		public NspiStatus EndGetHierarchyInfo(ICancelableAsyncResult asyncResult, out int codePage, out int returnedVersion, out PropertyValue[][] rowset)
		{
			NspiGetSpecialTableResponse response = null;
			IntPtr zero = IntPtr.Zero;
			NspiStatus result = (NspiStatus)base.EndWrapper<NspiGetSpecialTableClientAsyncOperation>(asyncResult, false, false, out zero, (NspiGetSpecialTableClientAsyncOperation operation) => operation.End(out response));
			codePage = (int)response.CodePage;
			returnedVersion = (int)response.Version.Value;
			rowset = response.PropertyValues;
			return result;
		}

		public ICancelableAsyncResult BeginGetTemplateInfo(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiGetTemplateInfoFlags flags, int type, string dn, int codePage, int locale, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return base.BeginWrapper<NspiGetTemplateInfoClientAsyncOperation>(contextHandle, false, delegate(ClientSessionContext context)
			{
				NspiGetTemplateInfoClientAsyncOperation nspiGetTemplateInfoClientAsyncOperation = new NspiGetTemplateInfoClientAsyncOperation(context, asyncCallback, asyncState);
				nspiGetTemplateInfoClientAsyncOperation.Begin(new NspiGetTemplateInfoRequest(flags, (uint)type, dn, (uint)codePage, (uint)locale, Array<byte>.EmptySegment));
				return nspiGetTemplateInfoClientAsyncOperation;
			});
		}

		public NspiStatus EndGetTemplateInfo(ICancelableAsyncResult asyncResult, out int codePage, out PropertyValue[] row)
		{
			NspiGetTemplateInfoResponse response = null;
			IntPtr zero = IntPtr.Zero;
			NspiStatus result = (NspiStatus)base.EndWrapper<NspiGetTemplateInfoClientAsyncOperation>(asyncResult, false, false, out zero, (NspiGetTemplateInfoClientAsyncOperation operation) => operation.End(out response));
			codePage = (int)response.CodePage;
			row = response.PropertyValues;
			return result;
		}

		public ICancelableAsyncResult BeginModLinkAtt(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiModLinkAttFlags flags, PropertyTag propertyTag, int mid, byte[][] entryIds, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return base.BeginWrapper<NspiModLinkAttClientAsyncOperation>(contextHandle, false, delegate(ClientSessionContext context)
			{
				NspiModLinkAttClientAsyncOperation nspiModLinkAttClientAsyncOperation = new NspiModLinkAttClientAsyncOperation(context, asyncCallback, asyncState);
				nspiModLinkAttClientAsyncOperation.Begin(new NspiModLinkAttRequest(flags, propertyTag, mid, entryIds, Array<byte>.EmptySegment));
				return nspiModLinkAttClientAsyncOperation;
			});
		}

		public NspiStatus EndModLinkAtt(ICancelableAsyncResult asyncResult)
		{
			NspiModLinkAttResponse response = null;
			IntPtr zero = IntPtr.Zero;
			return (NspiStatus)base.EndWrapper<NspiModLinkAttClientAsyncOperation>(asyncResult, false, false, out zero, (NspiModLinkAttClientAsyncOperation operation) => operation.End(out response));
		}

		public ICancelableAsyncResult BeginDeleteEntries(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiDeleteEntriesFlags flags, int mid, byte[][] entryIds, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			throw new NotSupportedException();
		}

		public NspiStatus EndDeleteEntries(ICancelableAsyncResult asyncResult)
		{
			throw new NotSupportedException();
		}

		public ICancelableAsyncResult BeginQueryColumns(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiQueryColumnsFlags flags, NspiQueryColumnsMapiFlags mapiFlags, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return base.BeginWrapper<NspiQueryColumnsClientAsyncOperation>(contextHandle, false, delegate(ClientSessionContext context)
			{
				NspiQueryColumnsClientAsyncOperation nspiQueryColumnsClientAsyncOperation = new NspiQueryColumnsClientAsyncOperation(context, asyncCallback, asyncState);
				nspiQueryColumnsClientAsyncOperation.Begin(new NspiQueryColumnsRequest(flags, mapiFlags, Array<byte>.EmptySegment));
				return nspiQueryColumnsClientAsyncOperation;
			});
		}

		public NspiStatus EndQueryColumns(ICancelableAsyncResult asyncResult, out PropertyTag[] columns)
		{
			NspiQueryColumnsResponse response = null;
			IntPtr zero = IntPtr.Zero;
			NspiStatus result = (NspiStatus)base.EndWrapper<NspiQueryColumnsClientAsyncOperation>(asyncResult, false, false, out zero, (NspiQueryColumnsClientAsyncOperation operation) => operation.End(out response));
			columns = response.Columns;
			return result;
		}

		public ICancelableAsyncResult BeginGetNamesFromIDs(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiGetNamesFromIDsFlags flags, Guid? guid, PropertyTag[] propertyTags, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public NspiStatus EndGetNamesFromIDs(ICancelableAsyncResult asyncResult, out PropertyTag[] propertyTags, out SafeRpcMemoryHandle namesHandle)
		{
			throw new NotImplementedException();
		}

		public ICancelableAsyncResult BeginGetIDsFromNames(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiGetIDsFromNamesFlags flags, int mapiFlags, int nameCount, IntPtr names, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public NspiStatus EndGetIDsFromNames(ICancelableAsyncResult asyncResult, out PropertyTag[] propertyTags)
		{
			throw new NotImplementedException();
		}

		public ICancelableAsyncResult BeginResolveNames(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiResolveNamesFlags flags, NspiState state, PropertyTag[] propertyTags, byte[][] rawNames, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			string[] names = null;
			if (rawNames != null)
			{
				Encoding asciiEncoding;
				if (!String8Encodings.TryGetEncoding(state.CodePage, out asciiEncoding))
				{
					asciiEncoding = CTSGlobals.AsciiEncoding;
				}
				names = new string[rawNames.Length];
				for (int i = 0; i < rawNames.Length; i++)
				{
					String8 @string = new String8(new ArraySegment<byte>(rawNames[i]));
					@string.ResolveString8Values(asciiEncoding);
					names[i] = @string.StringValue;
				}
			}
			return base.BeginWrapper<NspiResolveNamesClientAsyncOperation>(contextHandle, false, delegate(ClientSessionContext context)
			{
				NspiResolveNamesClientAsyncOperation nspiResolveNamesClientAsyncOperation = new NspiResolveNamesClientAsyncOperation(context, asyncCallback, asyncState);
				nspiResolveNamesClientAsyncOperation.Begin(new NspiResolveNamesRequest(flags, state, propertyTags, names, Array<byte>.EmptySegment));
				return nspiResolveNamesClientAsyncOperation;
			});
		}

		public NspiStatus EndResolveNames(ICancelableAsyncResult asyncResult, out int codePage, out int[] mids, out PropertyValue[][] rowset)
		{
			NspiResolveNamesResponse response = null;
			IntPtr zero = IntPtr.Zero;
			NspiStatus result = (NspiStatus)base.EndWrapper<NspiResolveNamesClientAsyncOperation>(asyncResult, false, false, out zero, (NspiResolveNamesClientAsyncOperation operation) => operation.End(out response));
			codePage = (int)response.CodePage;
			mids = response.EphemeralIds;
			rowset = response.ResolvedValues;
			return result;
		}

		public ICancelableAsyncResult BeginResolveNamesW(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiResolveNamesFlags flags, NspiState state, PropertyTag[] propertyTags, string[] names, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			return base.BeginWrapper<NspiResolveNamesClientAsyncOperation>(contextHandle, false, delegate(ClientSessionContext context)
			{
				NspiResolveNamesClientAsyncOperation nspiResolveNamesClientAsyncOperation = new NspiResolveNamesClientAsyncOperation(context, asyncCallback, asyncState);
				nspiResolveNamesClientAsyncOperation.Begin(new NspiResolveNamesRequest(flags, state, propertyTags, names, Array<byte>.EmptySegment));
				return nspiResolveNamesClientAsyncOperation;
			});
		}

		public NspiStatus EndResolveNamesW(ICancelableAsyncResult asyncResult, out int codePage, out int[] mids, out PropertyValue[][] rowset)
		{
			NspiResolveNamesResponse response = null;
			IntPtr zero = IntPtr.Zero;
			NspiStatus result = (NspiStatus)base.EndWrapper<NspiResolveNamesClientAsyncOperation>(asyncResult, false, false, out zero, (NspiResolveNamesClientAsyncOperation operation) => operation.End(out response));
			codePage = (int)response.CodePage;
			mids = response.EphemeralIds;
			rowset = response.ResolvedValues;
			return result;
		}

		public void ContextHandleRundown(IntPtr contextHandle)
		{
			throw new InvalidOperationException("ContextHandleRundown");
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<NspiHttpClient>(this);
		}
	}
}
