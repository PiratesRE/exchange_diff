using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.Nspi;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Nspi.Client
{
	internal class NspiClient : IDisposeTrackable, IDisposable
	{
		public NspiClient(string host) : this(host, null)
		{
		}

		public NspiClient(string host, NetworkCredential nc)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.disposeTracker = this.GetDisposeTracker();
				this.client = new NspiRpcClient(host, "ncacn_ip_tcp", nc);
				this.stat = new NspiState();
				this.stat.SortLocale = 1033;
				this.stat.TemplateLocale = 1033;
				this.stat.CodePage = 1252;
				this.statHandle = new SafeRpcMemoryHandle(this.stat.GetBytesToMarshal());
				disposeGuard.Success();
			}
		}

		public NspiClient(string machinename, string proxyserver, NetworkCredential nc) : this(machinename, proxyserver, nc, "ncacn_http:6004")
		{
		}

		public NspiClient(string machinename, string proxyserver, NetworkCredential nc, string protocolSequence) : this(machinename, proxyserver, nc, protocolSequence, HTTPAuthentication.Basic, AuthenticationService.Negotiate, machinename)
		{
		}

		public NspiClient(string machinename, string proxyserver, NetworkCredential nc, string protocolSequence, HTTPAuthentication httpAuth, AuthenticationService authService) : this(machinename, proxyserver, nc, protocolSequence, httpAuth, authService, machinename)
		{
		}

		public NspiClient(string machinename, string proxyserver, NetworkCredential nc, string protocolSequence, HTTPAuthentication httpAuth, AuthenticationService authService, string instanceName) : this(machinename, proxyserver, nc, protocolSequence, httpAuth, authService, instanceName, string.Empty, true)
		{
		}

		public NspiClient(string machinename, string proxyserver, NetworkCredential nc, string protocolSequence, HTTPAuthentication httpAuth, AuthenticationService authService, string instanceName, string certificateSubjectName, bool useEncryption = true)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.disposeTracker = this.GetDisposeTracker();
				this.client = new NspiRpcClient(machinename, proxyserver, protocolSequence, nc, (HttpAuthenticationScheme)httpAuth, authService, instanceName, true, certificateSubjectName, useEncryption);
				this.stat = new NspiState();
				this.stat.SortLocale = 1033;
				this.stat.TemplateLocale = 1033;
				this.stat.CodePage = 1252;
				this.statHandle = new SafeRpcMemoryHandle(this.stat.GetBytesToMarshal());
				disposeGuard.Success();
			}
		}

		internal NspiState Stat
		{
			get
			{
				return this.stat;
			}
		}

		internal Guid ServerGuid
		{
			get
			{
				return this.serverGuid;
			}
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<NspiClient>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public NspiStatus Bind(NspiBindFlags flags)
		{
			NspiStatus result;
			using (SafeRpcMemoryHandle safeRpcMemoryHandle = new SafeRpcMemoryHandle(16))
			{
				this.MarshalStatToNative();
				NspiStatus nspiStatus = this.client.Bind(flags, this.statHandle.DangerousGetHandle(), safeRpcMemoryHandle.DangerousGetHandle());
				this.MarshalNativeToStat();
				byte[] array = new byte[16];
				Marshal.Copy(safeRpcMemoryHandle.DangerousGetHandle(), array, 0, array.Length);
				this.serverGuid = new Guid(array);
				this.needToUnbind = (nspiStatus == NspiStatus.Success);
				result = nspiStatus;
			}
			return result;
		}

		public int Unbind()
		{
			this.needToUnbind = false;
			return this.client.Unbind();
		}

		public NspiStatus GetHierarchyInfo(NspiGetHierarchyInfoFlags flags, ref uint version, out PropRowSet rowset)
		{
			this.MarshalStatToNative();
			SafeRpcMemoryHandle rowsetHandle;
			NspiStatus hierarchyInfo = this.client.GetHierarchyInfo(flags, this.statHandle.DangerousGetHandle(), ref version, out rowsetHandle);
			this.MarshalNativeToStat();
			rowset = NspiClient.GetRowSetAndDisposeHandle(rowsetHandle);
			return hierarchyInfo;
		}

		public NspiStatus GetMatches(Restriction restriction, object propName, int maxRows, IList<PropTag> propTags, out int[] mids, out PropRowSet rowset)
		{
			NspiStatus result;
			using (SafeRpcMemoryHandle safeRpcMemoryHandle = new SafeRpcMemoryHandle())
			{
				if (restriction != null)
				{
					safeRpcMemoryHandle.Allocate(restriction.GetBytesToMarshalNspi());
					restriction.MarshalToNativeNspi(safeRpcMemoryHandle);
				}
				int[] intArrayFromPropTagArray = NspiClient.GetIntArrayFromPropTagArray(propTags);
				this.MarshalStatToNative();
				SafeRpcMemoryHandle rowsetHandle;
				NspiStatus matches = this.client.GetMatches(this.statHandle.DangerousGetHandle(), safeRpcMemoryHandle.DangerousGetHandle(), IntPtr.Zero, maxRows, intArrayFromPropTagArray, out mids, out rowsetHandle);
				this.MarshalNativeToStat();
				rowset = NspiClient.GetRowSetAndDisposeHandle(rowsetHandle);
				result = matches;
			}
			return result;
		}

		public NspiStatus QueryRows(NspiQueryRowsFlags flags, int[] mids, int count, IList<PropTag> propTags, out PropRowSet rowset)
		{
			int[] intArrayFromPropTagArray = NspiClient.GetIntArrayFromPropTagArray(propTags);
			this.MarshalStatToNative();
			SafeRpcMemoryHandle rowsetHandle;
			NspiStatus result = this.client.QueryRows(flags, this.statHandle.DangerousGetHandle(), mids, count, intArrayFromPropTagArray, out rowsetHandle);
			this.MarshalNativeToStat();
			rowset = NspiClient.GetRowSetAndDisposeHandle(rowsetHandle);
			return result;
		}

		public NspiStatus DnToEph(string[] distinguishedNames, out int[] mids)
		{
			return this.client.DNToEph(distinguishedNames, out mids);
		}

		public NspiStatus ResolveNames(string[] names, IList<PropTag> propTags, out ResolveResult[] results, out PropRowSet rowset)
		{
			int[] intArrayFromPropTagArray = NspiClient.GetIntArrayFromPropTagArray(propTags);
			this.MarshalStatToNative();
			int[] array;
			SafeRpcMemoryHandle rowsetHandle;
			NspiStatus result = this.client.ResolveNames(this.statHandle.DangerousGetHandle(), intArrayFromPropTagArray, names, out array, out rowsetHandle);
			this.MarshalNativeToStat();
			rowset = NspiClient.GetRowSetAndDisposeHandle(rowsetHandle);
			if (array != null)
			{
				results = new ResolveResult[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					results[i] = (ResolveResult)array[i];
				}
			}
			else
			{
				results = null;
			}
			return result;
		}

		public NspiStatus ResolveNames(byte[][] names, IList<PropTag> propTags, out ResolveResult[] results, out PropRowSet rowset)
		{
			int[] intArrayFromPropTagArray = NspiClient.GetIntArrayFromPropTagArray(propTags);
			this.MarshalStatToNative();
			int[] array;
			SafeRpcMemoryHandle rowsetHandle;
			NspiStatus result = this.client.ResolveNames(this.statHandle.DangerousGetHandle(), intArrayFromPropTagArray, names, out array, out rowsetHandle);
			this.MarshalNativeToStat();
			rowset = NspiClient.GetRowSetAndDisposeHandle(rowsetHandle);
			if (array != null)
			{
				results = new ResolveResult[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					results[i] = (ResolveResult)array[i];
				}
			}
			else
			{
				results = null;
			}
			return result;
		}

		public NspiStatus GetProps(NspiGetPropsFlags flags, IList<PropTag> propTags, out PropRow row)
		{
			int[] intArrayFromPropTagArray = NspiClient.GetIntArrayFromPropTagArray(propTags);
			this.MarshalStatToNative();
			SafeRpcMemoryHandle rowHandle;
			NspiStatus props = this.client.GetProps(flags, this.statHandle.DangerousGetHandle(), intArrayFromPropTagArray, out rowHandle);
			this.MarshalNativeToStat();
			row = NspiClient.GetRowAndDisposeHandle(rowHandle);
			return props;
		}

		public NspiStatus GetTemplateInfo(NspiGetTemplateInfoFlags flags, int type, string dn, int codePage, int localeId, out PropRow row)
		{
			SafeRpcMemoryHandle rowHandle;
			NspiStatus templateInfo = this.client.GetTemplateInfo(flags, type, dn, codePage, localeId, out rowHandle);
			row = NspiClient.GetRowAndDisposeHandle(rowHandle);
			return templateInfo;
		}

		public NspiStatus UpdateStat(out int delta)
		{
			this.MarshalStatToNative();
			NspiStatus result = this.client.UpdateStat(this.statHandle.DangerousGetHandle(), out delta);
			this.MarshalNativeToStat();
			return result;
		}

		public NspiStatus UpdateStat()
		{
			this.MarshalStatToNative();
			NspiStatus result = this.client.UpdateStat(this.statHandle.DangerousGetHandle());
			this.MarshalNativeToStat();
			return result;
		}

		public NspiStatus ResortRestriction(int[] mids, out int[] returnedMids)
		{
			this.MarshalStatToNative();
			NspiStatus result = this.client.ResortRestriction(this.statHandle.DangerousGetHandle(), mids, out returnedMids);
			this.MarshalNativeToStat();
			return result;
		}

		public NspiStatus GetPropList(NspiGetPropListFlags flags, int mid, int codePage, out PropTag[] props)
		{
			this.MarshalStatToNative();
			int[] propTagsAsInts;
			NspiStatus propList = this.client.GetPropList(flags, mid, codePage, out propTagsAsInts);
			this.MarshalNativeToStat();
			props = NspiClient.GetPropTagArrayFromIntArray(propTagsAsInts);
			return propList;
		}

		public NspiStatus CompareMids(int mid1, int mid2, out int result)
		{
			this.MarshalStatToNative();
			NspiStatus result2 = this.client.CompareMids(this.statHandle.DangerousGetHandle(), mid1, mid2, out result);
			this.MarshalNativeToStat();
			return result2;
		}

		public NspiStatus ModProps(IList<PropTag> propTags, PropRow row)
		{
			int[] intArrayFromPropTagArray = NspiClient.GetIntArrayFromPropTagArray(propTags);
			NspiStatus result;
			using (SafeRpcMemoryHandle safeRpcMemoryHandle = new SafeRpcMemoryHandle(row.GetBytesToMarshal()))
			{
				SafeRpcMemoryHandle safeRpcMemoryHandle2 = NspiMarshal.MarshalPropValueCollection(row.Properties);
				row.MarshalledPropertiesHandle = safeRpcMemoryHandle2;
				safeRpcMemoryHandle.AddAssociatedHandle(safeRpcMemoryHandle2);
				row.MarshalToNative(safeRpcMemoryHandle);
				this.MarshalStatToNative();
				NspiStatus nspiStatus = this.client.ModProps(this.statHandle.DangerousGetHandle(), intArrayFromPropTagArray, safeRpcMemoryHandle.DangerousGetHandle());
				this.MarshalNativeToStat();
				result = nspiStatus;
			}
			return result;
		}

		public NspiStatus QueryColumns(NspiQueryColumnsFlags flags, out PropTag[] propTags)
		{
			int[] propTagsAsInts;
			NspiStatus result = this.client.QueryColumns((int)flags, out propTagsAsInts);
			propTags = NspiClient.GetPropTagArrayFromIntArray(propTagsAsInts);
			return result;
		}

		public NspiStatus ModLinkAtt(NspiModLinkAttFlags flags, PropTag propTag, int mid, byte[][] entryIDs)
		{
			return this.client.ModLinkAtt(flags, (int)propTag, mid, entryIDs);
		}

		public NspiStatus SeekEntries(PropValue propValue, int[] mids, IList<PropTag> propTags, out PropRowSet rowset)
		{
			int[] intArrayFromPropTagArray = NspiClient.GetIntArrayFromPropTagArray(propTags);
			IList<PropValue> list = new List<PropValue>();
			list.Add(propValue);
			NspiStatus result;
			using (SafeRpcMemoryHandle safeRpcMemoryHandle = new SafeRpcMemoryHandle(propValue.GetBytesToMarshal()))
			{
				PropValue.MarshalToNative(list, safeRpcMemoryHandle);
				this.MarshalStatToNative();
				SafeRpcMemoryHandle rowsetHandle;
				NspiStatus nspiStatus = this.client.SeekEntries(this.statHandle.DangerousGetHandle(), safeRpcMemoryHandle.DangerousGetHandle(), mids, intArrayFromPropTagArray, out rowsetHandle);
				this.MarshalNativeToStat();
				rowset = NspiClient.GetRowSetAndDisposeHandle(rowsetHandle);
				result = nspiStatus;
			}
			return result;
		}

		public int Ping()
		{
			return this.client.Ping();
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.statHandle != null)
				{
					this.statHandle.Dispose();
					this.statHandle = null;
				}
				if (this.client != null)
				{
					if (this.needToUnbind)
					{
						try
						{
							this.client.Unbind();
						}
						catch (RpcException)
						{
						}
					}
					this.client.Dispose();
					this.client = null;
				}
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
					this.disposeTracker = null;
				}
			}
		}

		private static PropRow GetRowAndDisposeHandle(SafeRpcMemoryHandle rowHandle)
		{
			PropRow result;
			if (rowHandle != null)
			{
				result = new PropRow(rowHandle, true);
				rowHandle.Dispose();
			}
			else
			{
				result = null;
			}
			return result;
		}

		private static PropRowSet GetRowSetAndDisposeHandle(SafeRpcMemoryHandle rowsetHandle)
		{
			PropRowSet result;
			if (rowsetHandle != null)
			{
				result = new PropRowSet(rowsetHandle, true);
				rowsetHandle.Dispose();
			}
			else
			{
				result = null;
			}
			return result;
		}

		private static int[] GetIntArrayFromPropTagArray(IList<PropTag> propTags)
		{
			int[] array = null;
			if (propTags != null)
			{
				array = new int[propTags.Count];
				for (int i = 0; i < propTags.Count; i++)
				{
					array[i] = (int)propTags[i];
				}
			}
			return array;
		}

		private static PropTag[] GetPropTagArrayFromIntArray(int[] propTagsAsInts)
		{
			PropTag[] array = null;
			if (propTagsAsInts != null)
			{
				array = new PropTag[propTagsAsInts.Length];
				for (int i = 0; i < propTagsAsInts.Length; i++)
				{
					array[i] = (PropTag)propTagsAsInts[i];
				}
			}
			return array;
		}

		private void MarshalStatToNative()
		{
			this.stat.MarshalToNative(this.statHandle.DangerousGetHandle());
		}

		private void MarshalNativeToStat()
		{
			this.stat.MarshalFromNative(this.statHandle.DangerousGetHandle());
		}

		private const int DefaultLcid = 1033;

		private const int DefaultANSICodePage = 1252;

		private NspiRpcClient client;

		private Guid serverGuid;

		private NspiState stat;

		private SafeRpcMemoryHandle statHandle;

		private DisposeTracker disposeTracker;

		private bool needToUnbind;
	}
}
