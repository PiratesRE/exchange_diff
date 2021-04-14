using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.Rpc.NspiClient
{
	internal class NspiAsyncRpcClient : RpcClientBase, INspiAsyncDispatch
	{
		public NspiAsyncRpcClient(RpcBindingInfo bindingInfo) : base(bindingInfo.UseKerberosSpn("exchangeAB", null))
		{
		}

		public virtual ICancelableAsyncResult BeginBind(ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, NspiBindFlags flags, NspiState state, Guid? guid, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			ClientAsyncCallState_Bind clientAsyncCallState_Bind = null;
			bool flag = false;
			ICancelableAsyncResult result;
			try
			{
				IntPtr bindingHandle = (IntPtr)base.BindingHandle;
				clientAsyncCallState_Bind = new ClientAsyncCallState_Bind(asyncCallback, asyncState, bindingHandle, flags, state, guid);
				clientAsyncCallState_Bind.Begin();
				flag = true;
				result = clientAsyncCallState_Bind;
			}
			finally
			{
				if (!flag && clientAsyncCallState_Bind != null)
				{
					((IDisposable)clientAsyncCallState_Bind).Dispose();
				}
			}
			return result;
		}

		public virtual NspiStatus EndBind(ICancelableAsyncResult asyncResult, out Guid? guid, out IntPtr contextHandle)
		{
			NspiStatus result;
			using (ClientAsyncCallState_Bind clientAsyncCallState_Bind = (ClientAsyncCallState_Bind)asyncResult)
			{
				result = clientAsyncCallState_Bind.End(out guid, out contextHandle);
			}
			return result;
		}

		public virtual ICancelableAsyncResult BeginUnbind(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiUnbindFlags flags, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			ClientAsyncCallState_Unbind clientAsyncCallState_Unbind = null;
			bool flag = false;
			ICancelableAsyncResult result;
			try
			{
				clientAsyncCallState_Unbind = new ClientAsyncCallState_Unbind(asyncCallback, asyncState, contextHandle, flags);
				clientAsyncCallState_Unbind.Begin();
				flag = true;
				result = clientAsyncCallState_Unbind;
			}
			finally
			{
				if (!flag && clientAsyncCallState_Unbind != null)
				{
					((IDisposable)clientAsyncCallState_Unbind).Dispose();
				}
			}
			return result;
		}

		public virtual NspiStatus EndUnbind(ICancelableAsyncResult asyncResult, out IntPtr contextHandle)
		{
			NspiStatus result;
			using (ClientAsyncCallState_Unbind clientAsyncCallState_Unbind = (ClientAsyncCallState_Unbind)asyncResult)
			{
				result = clientAsyncCallState_Unbind.End(out contextHandle);
			}
			return result;
		}

		public virtual ICancelableAsyncResult BeginUpdateStat(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiUpdateStatFlags flags, NspiState state, [MarshalAs(UnmanagedType.U1)] bool deltaRequested, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException("BeginUpdateStat not implemented");
		}

		public virtual NspiStatus EndUpdateStat(ICancelableAsyncResult asyncResult, out NspiState state, out int? delta)
		{
			throw new NotImplementedException("EndUpdateStat not implemented");
		}

		public virtual ICancelableAsyncResult BeginQueryRows(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiQueryRowsFlags flags, NspiState state, int[] mids, int count, PropertyTag[] propTags, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			ClientAsyncCallState_QueryRows clientAsyncCallState_QueryRows = null;
			bool flag = false;
			ICancelableAsyncResult result;
			try
			{
				clientAsyncCallState_QueryRows = new ClientAsyncCallState_QueryRows(asyncCallback, asyncState, contextHandle, flags, state, mids, count, propTags);
				clientAsyncCallState_QueryRows.Begin();
				flag = true;
				result = clientAsyncCallState_QueryRows;
			}
			finally
			{
				if (!flag && clientAsyncCallState_QueryRows != null)
				{
					((IDisposable)clientAsyncCallState_QueryRows).Dispose();
				}
			}
			return result;
		}

		public virtual NspiStatus EndQueryRows(ICancelableAsyncResult asyncResult, out NspiState state, out PropertyValue[][] rowset)
		{
			NspiStatus result;
			using (ClientAsyncCallState_QueryRows clientAsyncCallState_QueryRows = (ClientAsyncCallState_QueryRows)asyncResult)
			{
				result = clientAsyncCallState_QueryRows.End(out state, out rowset);
			}
			return result;
		}

		public virtual ICancelableAsyncResult BeginSeekEntries(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiSeekEntriesFlags flags, NspiState state, PropertyValue? target, int[] restriction, PropertyTag[] propTags, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException("BeginSeekEntries not implemented");
		}

		public virtual NspiStatus EndSeekEntries(ICancelableAsyncResult asyncResult, out NspiState state, out PropertyValue[][] rowset)
		{
			throw new NotImplementedException("EndSeekEntries not implemented");
		}

		public virtual ICancelableAsyncResult BeginGetMatches(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiGetMatchesFlags flags, NspiState state, int[] mids, int interfaceOptions, Restriction restriction, IntPtr propName, int maxRows, PropertyTag[] propTags, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			ClientAsyncCallState_GetMatches clientAsyncCallState_GetMatches = null;
			bool flag = false;
			ICancelableAsyncResult result;
			try
			{
				clientAsyncCallState_GetMatches = new ClientAsyncCallState_GetMatches(asyncCallback, asyncState, contextHandle, flags, state, mids, interfaceOptions, restriction, propName, maxRows, propTags);
				clientAsyncCallState_GetMatches.Begin();
				flag = true;
				result = clientAsyncCallState_GetMatches;
			}
			finally
			{
				if (!flag && clientAsyncCallState_GetMatches != null)
				{
					((IDisposable)clientAsyncCallState_GetMatches).Dispose();
				}
			}
			return result;
		}

		public virtual NspiStatus EndGetMatches(ICancelableAsyncResult asyncResult, out NspiState state, out int[] mids, out PropertyValue[][] rowset)
		{
			NspiStatus result;
			using (ClientAsyncCallState_GetMatches clientAsyncCallState_GetMatches = (ClientAsyncCallState_GetMatches)asyncResult)
			{
				result = clientAsyncCallState_GetMatches.End(out state, out mids, out rowset);
			}
			return result;
		}

		public virtual ICancelableAsyncResult BeginResortRestriction(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiResortRestrictionFlags flags, NspiState state, int[] mids, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException("BeginResortRestriction not implemented");
		}

		public virtual NspiStatus EndResortRestriction(ICancelableAsyncResult asyncResult, out NspiState state, out int[] mids)
		{
			throw new NotImplementedException("EndResortRestriction not implemented");
		}

		public virtual ICancelableAsyncResult BeginDNToEph(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiDNToEphFlags flags, string[] DNs, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			ClientAsyncCallState_DNToEph clientAsyncCallState_DNToEph = null;
			bool flag = false;
			ICancelableAsyncResult result;
			try
			{
				clientAsyncCallState_DNToEph = new ClientAsyncCallState_DNToEph(asyncCallback, asyncState, contextHandle, flags, DNs);
				clientAsyncCallState_DNToEph.Begin();
				flag = true;
				result = clientAsyncCallState_DNToEph;
			}
			finally
			{
				if (!flag && clientAsyncCallState_DNToEph != null)
				{
					((IDisposable)clientAsyncCallState_DNToEph).Dispose();
				}
			}
			return result;
		}

		public virtual NspiStatus EndDNToEph(ICancelableAsyncResult asyncResult, out int[] mids)
		{
			NspiStatus result;
			using (ClientAsyncCallState_DNToEph clientAsyncCallState_DNToEph = (ClientAsyncCallState_DNToEph)asyncResult)
			{
				result = clientAsyncCallState_DNToEph.End(out mids);
			}
			return result;
		}

		public virtual ICancelableAsyncResult BeginGetPropList(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiGetPropListFlags flags, int mid, int codePage, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException("BeginGetPropList not implemented");
		}

		public virtual NspiStatus EndGetPropList(ICancelableAsyncResult asyncResult, out PropertyTag[] propTags)
		{
			throw new NotImplementedException("EndGetPropList not implemented");
		}

		public virtual ICancelableAsyncResult BeginGetProps(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiGetPropsFlags flags, NspiState state, PropertyTag[] propTags, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			ClientAsyncCallState_GetProps clientAsyncCallState_GetProps = null;
			bool flag = false;
			ICancelableAsyncResult result;
			try
			{
				clientAsyncCallState_GetProps = new ClientAsyncCallState_GetProps(asyncCallback, asyncState, contextHandle, flags, state, propTags);
				clientAsyncCallState_GetProps.Begin();
				flag = true;
				result = clientAsyncCallState_GetProps;
			}
			finally
			{
				if (!flag && clientAsyncCallState_GetProps != null)
				{
					((IDisposable)clientAsyncCallState_GetProps).Dispose();
				}
			}
			return result;
		}

		public virtual NspiStatus EndGetProps(ICancelableAsyncResult asyncResult, out int codePage, out PropertyValue[] row)
		{
			NspiStatus result;
			using (ClientAsyncCallState_GetProps clientAsyncCallState_GetProps = (ClientAsyncCallState_GetProps)asyncResult)
			{
				result = clientAsyncCallState_GetProps.End(out codePage, row);
			}
			return result;
		}

		public virtual ICancelableAsyncResult BeginCompareDNTs(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiCompareDNTsFlags flags, NspiState state, int mid1, int mid2, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException("BeginCompareDNTs not implemented");
		}

		public virtual NspiStatus EndCompareDNTs(ICancelableAsyncResult asyncResult, out int result)
		{
			throw new NotImplementedException("EndCompareDNTs not implemented");
		}

		public virtual ICancelableAsyncResult BeginModProps(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiModPropsFlags flags, NspiState state, PropertyTag[] propTags, PropertyValue[] row, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException("BeginModProps not implemented");
		}

		public virtual NspiStatus EndModProps(ICancelableAsyncResult asyncResult)
		{
			throw new NotImplementedException("EndModProps not implemented");
		}

		public virtual ICancelableAsyncResult BeginGetHierarchyInfo(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiGetHierarchyInfoFlags flags, NspiState state, int version, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			ClientAsyncCallState_GetHierarchyInfo clientAsyncCallState_GetHierarchyInfo = null;
			bool flag = false;
			ICancelableAsyncResult result;
			try
			{
				clientAsyncCallState_GetHierarchyInfo = new ClientAsyncCallState_GetHierarchyInfo(asyncCallback, asyncState, contextHandle, flags, state, version);
				clientAsyncCallState_GetHierarchyInfo.Begin();
				flag = true;
				result = clientAsyncCallState_GetHierarchyInfo;
			}
			finally
			{
				if (!flag && clientAsyncCallState_GetHierarchyInfo != null)
				{
					((IDisposable)clientAsyncCallState_GetHierarchyInfo).Dispose();
				}
			}
			return result;
		}

		public virtual NspiStatus EndGetHierarchyInfo(ICancelableAsyncResult asyncResult, out int codePage, out int returnedVersion, out PropertyValue[][] rowset)
		{
			NspiStatus result;
			using (ClientAsyncCallState_GetHierarchyInfo clientAsyncCallState_GetHierarchyInfo = (ClientAsyncCallState_GetHierarchyInfo)asyncResult)
			{
				result = clientAsyncCallState_GetHierarchyInfo.End(out codePage, out returnedVersion, out rowset);
			}
			return result;
		}

		public virtual ICancelableAsyncResult BeginGetTemplateInfo(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiGetTemplateInfoFlags flags, int type, string dn, int codePage, int locale, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException("BeginGetTemplateInfo not implemented");
		}

		public virtual NspiStatus EndGetTemplateInfo(ICancelableAsyncResult asyncResult, out int codePage, out PropertyValue[] row)
		{
			throw new NotImplementedException("EndGetTemplateInfo not implemented");
		}

		public virtual ICancelableAsyncResult BeginModLinkAtt(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiModLinkAttFlags flags, PropertyTag propTag, int mid, byte[][] entryIds, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException("BeginModLinkAtt not implemented");
		}

		public virtual NspiStatus EndModLinkAtt(ICancelableAsyncResult asyncResult)
		{
			throw new NotImplementedException("EndModLinkAtt not implemented");
		}

		public virtual ICancelableAsyncResult BeginDeleteEntries(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiDeleteEntriesFlags flags, int mid, byte[][] entryIds, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException("BeginDeleteEntries not implemented");
		}

		public virtual NspiStatus EndDeleteEntries(ICancelableAsyncResult asyncResult)
		{
			throw new NotImplementedException("EndDeleteEntries not implemented");
		}

		public virtual ICancelableAsyncResult BeginQueryColumns(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiQueryColumnsFlags flags, NspiQueryColumnsMapiFlags mapiFlags, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException("BeginQueryColumns not implemented");
		}

		public virtual NspiStatus EndQueryColumns(ICancelableAsyncResult asyncResult, out PropertyTag[] columns)
		{
			throw new NotImplementedException("EndQueryColumns not implemented");
		}

		public virtual ICancelableAsyncResult BeginGetNamesFromIDs(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiGetNamesFromIDsFlags flags, Guid? guid, PropertyTag[] propTags, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException("BeginGetNamesFromIDs not implemented");
		}

		public virtual NspiStatus EndGetNamesFromIDs(ICancelableAsyncResult asyncResult, out PropertyTag[] propTags, out SafeRpcMemoryHandle namesHandle)
		{
			throw new NotImplementedException("EndGetNamesFromIDs not implemented");
		}

		public virtual ICancelableAsyncResult BeginGetIDsFromNames(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiGetIDsFromNamesFlags flags, int mapiFlags, int nameCount, IntPtr names, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException("BeginGetIDsFromNames not implemented");
		}

		public virtual NspiStatus EndGetIDsFromNames(ICancelableAsyncResult asyncResult, out PropertyTag[] propTags)
		{
			throw new NotImplementedException("EndGetIDsFromNames not implemented");
		}

		public virtual ICancelableAsyncResult BeginResolveNames(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiResolveNamesFlags flags, NspiState state, PropertyTag[] propTags, byte[][] names, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException("BeginResolveNames not implemented");
		}

		public virtual NspiStatus EndResolveNames(ICancelableAsyncResult asyncResult, out int codePage, out int[] mids, out PropertyValue[][] rowset)
		{
			throw new NotImplementedException("EndResolveNames not implemented");
		}

		public virtual ICancelableAsyncResult BeginResolveNamesW(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiResolveNamesFlags flags, NspiState state, PropertyTag[] propTags, string[] names, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException("BeginResolveNamesW not implemented");
		}

		public virtual NspiStatus EndResolveNamesW(ICancelableAsyncResult asyncResult, out int codePage, out int[] mids, out PropertyValue[][] rowset)
		{
			throw new NotImplementedException("EndResolveNamesW not implemented");
		}

		public virtual void ContextHandleRundown(IntPtr contextHandle)
		{
			throw new NotImplementedException("ContextHandleRundown not implemented");
		}
	}
}
