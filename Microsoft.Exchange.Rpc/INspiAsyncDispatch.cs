using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.Rpc
{
	internal interface INspiAsyncDispatch
	{
		ICancelableAsyncResult BeginBind(ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, NspiBindFlags flags, NspiState state, Guid? guid, CancelableAsyncCallback asyncCallback, object asyncState);

		NspiStatus EndBind(ICancelableAsyncResult asyncResult, out Guid? guid, out IntPtr contextHandle);

		ICancelableAsyncResult BeginUnbind(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiUnbindFlags flags, CancelableAsyncCallback asyncCallback, object asyncState);

		NspiStatus EndUnbind(ICancelableAsyncResult asyncResult, out IntPtr contextHandle);

		ICancelableAsyncResult BeginUpdateStat(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiUpdateStatFlags flags, NspiState state, [MarshalAs(UnmanagedType.U1)] bool deltaRequested, CancelableAsyncCallback asyncCallback, object asyncState);

		NspiStatus EndUpdateStat(ICancelableAsyncResult asyncResult, out NspiState state, out int? delta);

		ICancelableAsyncResult BeginQueryRows(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiQueryRowsFlags flags, NspiState state, int[] mids, int count, PropertyTag[] propertyTags, CancelableAsyncCallback asyncCallback, object asyncState);

		NspiStatus EndQueryRows(ICancelableAsyncResult asyncResult, out NspiState state, out PropertyValue[][] rowset);

		ICancelableAsyncResult BeginSeekEntries(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiSeekEntriesFlags flags, NspiState state, PropertyValue? target, int[] restriction, PropertyTag[] propertyTags, CancelableAsyncCallback asyncCallback, object asyncState);

		NspiStatus EndSeekEntries(ICancelableAsyncResult asyncResult, out NspiState state, out PropertyValue[][] rowset);

		ICancelableAsyncResult BeginGetMatches(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiGetMatchesFlags flags, NspiState state, int[] mids, int interfaceOptions, Restriction restriction, IntPtr propName, int maxRows, PropertyTag[] propertyTags, CancelableAsyncCallback asyncCallback, object asyncState);

		NspiStatus EndGetMatches(ICancelableAsyncResult asyncResult, out NspiState state, out int[] mids, out PropertyValue[][] rowset);

		ICancelableAsyncResult BeginResortRestriction(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiResortRestrictionFlags flags, NspiState state, int[] mids, CancelableAsyncCallback asyncCallback, object asyncState);

		NspiStatus EndResortRestriction(ICancelableAsyncResult asyncResult, out NspiState state, out int[] mids);

		ICancelableAsyncResult BeginDNToEph(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiDNToEphFlags flags, string[] DNs, CancelableAsyncCallback asyncCallback, object asyncState);

		NspiStatus EndDNToEph(ICancelableAsyncResult asyncResult, out int[] mids);

		ICancelableAsyncResult BeginGetPropList(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiGetPropListFlags flags, int mid, int codePage, CancelableAsyncCallback asyncCallback, object asyncState);

		NspiStatus EndGetPropList(ICancelableAsyncResult asyncResult, out PropertyTag[] propertyTags);

		ICancelableAsyncResult BeginGetProps(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiGetPropsFlags flags, NspiState state, PropertyTag[] propertyTags, CancelableAsyncCallback asyncCallback, object asyncState);

		NspiStatus EndGetProps(ICancelableAsyncResult asyncResult, out int codePage, out PropertyValue[] row);

		ICancelableAsyncResult BeginCompareDNTs(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiCompareDNTsFlags flags, NspiState state, int mid1, int mid2, CancelableAsyncCallback asyncCallback, object asyncState);

		NspiStatus EndCompareDNTs(ICancelableAsyncResult asyncResult, out int result);

		ICancelableAsyncResult BeginModProps(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiModPropsFlags flags, NspiState state, PropertyTag[] propertyTags, PropertyValue[] row, CancelableAsyncCallback asyncCallback, object asyncState);

		NspiStatus EndModProps(ICancelableAsyncResult asyncResult);

		ICancelableAsyncResult BeginGetHierarchyInfo(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiGetHierarchyInfoFlags flags, NspiState state, int version, CancelableAsyncCallback asyncCallback, object asyncState);

		NspiStatus EndGetHierarchyInfo(ICancelableAsyncResult asyncResult, out int codePage, out int returnedVersion, out PropertyValue[][] rowset);

		ICancelableAsyncResult BeginGetTemplateInfo(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiGetTemplateInfoFlags flags, int type, string dn, int codePage, int locale, CancelableAsyncCallback asyncCallback, object asyncState);

		NspiStatus EndGetTemplateInfo(ICancelableAsyncResult asyncResult, out int codePage, out PropertyValue[] row);

		ICancelableAsyncResult BeginModLinkAtt(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiModLinkAttFlags flags, PropertyTag propertyTag, int mid, byte[][] entryIds, CancelableAsyncCallback asyncCallback, object asyncState);

		NspiStatus EndModLinkAtt(ICancelableAsyncResult asyncResult);

		ICancelableAsyncResult BeginDeleteEntries(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiDeleteEntriesFlags flags, int mid, byte[][] entryIds, CancelableAsyncCallback asyncCallback, object asyncState);

		NspiStatus EndDeleteEntries(ICancelableAsyncResult asyncResult);

		ICancelableAsyncResult BeginQueryColumns(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiQueryColumnsFlags flags, NspiQueryColumnsMapiFlags mapiFlags, CancelableAsyncCallback asyncCallback, object asyncState);

		NspiStatus EndQueryColumns(ICancelableAsyncResult asyncResult, out PropertyTag[] columns);

		ICancelableAsyncResult BeginGetNamesFromIDs(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiGetNamesFromIDsFlags flags, Guid? guid, PropertyTag[] propertyTags, CancelableAsyncCallback asyncCallback, object asyncState);

		NspiStatus EndGetNamesFromIDs(ICancelableAsyncResult asyncResult, out PropertyTag[] propertyTags, out SafeRpcMemoryHandle namesHandle);

		ICancelableAsyncResult BeginGetIDsFromNames(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiGetIDsFromNamesFlags flags, int mapiFlags, int nameCount, IntPtr names, CancelableAsyncCallback asyncCallback, object asyncState);

		NspiStatus EndGetIDsFromNames(ICancelableAsyncResult asyncResult, out PropertyTag[] propertyTags);

		ICancelableAsyncResult BeginResolveNames(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiResolveNamesFlags flags, NspiState state, PropertyTag[] propertyTags, byte[][] names, CancelableAsyncCallback asyncCallback, object asyncState);

		NspiStatus EndResolveNames(ICancelableAsyncResult asyncResult, out int codePage, out int[] mids, out PropertyValue[][] rowset);

		ICancelableAsyncResult BeginResolveNamesW(ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, NspiResolveNamesFlags flags, NspiState state, PropertyTag[] propertyTags, string[] names, CancelableAsyncCallback asyncCallback, object asyncState);

		NspiStatus EndResolveNamesW(ICancelableAsyncResult asyncResult, out int codePage, out int[] mids, out PropertyValue[][] rowset);

		void ContextHandleRundown(IntPtr contextHandle);
	}
}
