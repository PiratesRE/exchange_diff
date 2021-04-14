using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MapiManifestCollector : DisposeTrackableBase, IExchangeImportContentsChanges
	{
		public MapiManifestCollector(MapiFolder sourceFolder, IMapiManifestCallback manifestCallback)
		{
			this.manifestCallback = manifestCallback;
			this.iUnknown = Marshal.GetIUnknownForObject(this);
			this.sourceFolder = sourceFolder;
			byte[] bytes = this.sourceFolder.GetProp(PropTag.EntryId).GetBytes();
			this.sourceFID = this.sourceFolder.MapiStore.GetFidFromEntryId(bytes);
		}

		internal IntPtr ExchangeCollector
		{
			get
			{
				return this.iUnknown;
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.iUnknown != IntPtr.Zero)
			{
				Marshal.Release(this.iUnknown);
				this.iUnknown = IntPtr.Zero;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MapiManifestCollector>(this);
		}

		unsafe int IExchangeImportContentsChanges.GetLastError(int hResult, int ulFlags, out MAPIERROR* lpMapiError)
		{
			lpMapiError = (IntPtr)((UIntPtr)0);
			return 0;
		}

		int IExchangeImportContentsChanges.Config(IStream pIStream, int ulFlags)
		{
			return 0;
		}

		int IExchangeImportContentsChanges.UpdateState(IStream pIStream)
		{
			return 0;
		}

		unsafe int IExchangeImportContentsChanges.ImportMessageChange(int cpvalChanges, SPropValue* ppvalChanges, int ulFlags, out IMessage message)
		{
			ManifestChangeType changeType = ((ulFlags & 2048) != 0) ? ManifestChangeType.Add : ManifestChangeType.Change;
			byte[] array = null;
			byte[] array2 = null;
			byte[] changeKey = null;
			byte[] changeList = null;
			DateTime lastModificationTime = DateTime.MinValue;
			bool associated = false;
			List<PropValue> list = new List<PropValue>();
			int i = 0;
			while (i < cpvalChanges)
			{
				PropValue item = new PropValue(ppvalChanges + i);
				PropTag propTag = item.PropTag;
				if (propTag <= PropTag.SourceKey)
				{
					if (propTag != PropTag.EntryId)
					{
						if (propTag != PropTag.LastModificationTime)
						{
							if (propTag != PropTag.SourceKey)
							{
								goto IL_D2;
							}
							array2 = item.GetBytes();
						}
						else
						{
							lastModificationTime = item.GetDateTime();
						}
					}
					else
					{
						array = item.GetBytes();
					}
				}
				else if (propTag != PropTag.ChangeKey)
				{
					if (propTag != PropTag.PredecessorChangeList)
					{
						if (propTag != PropTag.Associated)
						{
							goto IL_D2;
						}
						associated = item.GetBoolean();
					}
					else
					{
						changeList = item.GetBytes();
					}
				}
				else
				{
					changeKey = item.GetBytes();
				}
				IL_DB:
				i++;
				continue;
				IL_D2:
				list.Add(item);
				goto IL_DB;
			}
			if (array == null && array2 != null)
			{
				array = this.EntryIdFromSourceKey(array2);
			}
			if (array != null)
			{
				this.manifestCallback.Change(array, array2, changeKey, changeList, lastModificationTime, changeType, associated, (list.Count > 0) ? list.ToArray() : null);
			}
			message = null;
			return -2147219455;
		}

		unsafe int IExchangeImportContentsChanges.ImportMessageDeletion(int ulFlags, _SBinaryArray* lpSrcEntryList)
		{
			if (lpSrcEntryList == null || lpSrcEntryList->cValues == 0)
			{
				return 0;
			}
			_SBinary* ptr = lpSrcEntryList->lpbin;
			int i = 0;
			while (i < lpSrcEntryList->cValues)
			{
				byte[] array = new byte[ptr->cb];
				Marshal.Copy((IntPtr)((void*)ptr->lpb), array, 0, ptr->cb);
				byte[] array2 = this.EntryIdFromSourceKey(array);
				if (array2 != null)
				{
					this.manifestCallback.Delete(array2, (ulFlags & 4) == 0, false);
				}
				i++;
				ptr++;
			}
			return 0;
		}

		unsafe int IExchangeImportContentsChanges.ImportPerUserReadStateChange(int cElements, _ReadState* lpReadState)
		{
			int i = 0;
			while (i < cElements)
			{
				byte[] array = new byte[lpReadState->cbSourceKey];
				Marshal.Copy((IntPtr)((void*)lpReadState->pbSourceKey), array, 0, lpReadState->cbSourceKey);
				ExchangeManifestCallbackReadFlags ulFlags = (ExchangeManifestCallbackReadFlags)lpReadState->ulFlags;
				bool read = (ulFlags & ExchangeManifestCallbackReadFlags.Read) == ExchangeManifestCallbackReadFlags.Read;
				byte[] array2 = this.EntryIdFromSourceKey(array);
				if (array2 != null)
				{
					this.manifestCallback.ReadUnread(array2, read);
				}
				i++;
				lpReadState++;
			}
			return 0;
		}

		int IExchangeImportContentsChanges.ImportMessageMove(int cbSourceKeySrcFolder, byte[] pbSourceKeySrcFolder, int cbSourceKeySrcMessage, byte[] pbSourceKeySrcMessage, int cbPCLMessage, byte[] pbPCLMessage, int cbSourceKeyDestMessage, byte[] pbSourceKeyDestMessage, int cbChangeNumDestMessage, byte[] pbChangeNumDestMessage)
		{
			return 0;
		}

		private byte[] EntryIdFromSourceKey(byte[] sourceKey)
		{
			if (sourceKey == null)
			{
				return null;
			}
			long mid = 0L;
			try
			{
				mid = this.sourceFolder.MapiStore.IdFromGlobalId(sourceKey);
			}
			catch (ArgumentException)
			{
				return null;
			}
			return this.sourceFolder.MapiStore.CreateEntryId(this.sourceFID, mid);
		}

		private IMapiManifestCallback manifestCallback;

		private IntPtr iUnknown;

		private MapiFolder sourceFolder;

		private long sourceFID;
	}
}
