using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class HierarchyManifestCallbackHelper : ManifestCallbackHelperBase<IMapiHierarchyManifestCallback>, IExchangeHierManifestCallback
	{
		internal HierarchyManifestCallbackHelper(HierarchyManifestCheckpoint checkpoint, bool isCallbackInfoExpected, bool isChangeNumberExpected)
		{
			if (ComponentTrace<MapiNetTags>.CheckEnabled(34))
			{
				ComponentTrace<MapiNetTags>.Trace<string>(51195, 34, (long)this.GetHashCode(), "HierarchyManifestCallbackHelper.HierarchyManifestCallbackHelper: this={0}", TraceUtils.MakeHash(this));
			}
			this.checkpoint = checkpoint;
			this.isCallbackInfoExpected = isCallbackInfoExpected;
			this.isChangeNumberExpected = isChangeNumberExpected;
		}

		unsafe int IExchangeHierManifestCallback.Change(int cpvalChanges, SPropValue* ppvalChanges)
		{
			PropValue[] props = new PropValue[cpvalChanges];
			long? fid = null;
			long? cn = null;
			int num = 0;
			for (int i = 0; i < cpvalChanges; i++)
			{
				props[i] = new PropValue(ppvalChanges + i);
				if (props[i].PropTag == PropTag.Fid)
				{
					fid = new long?(props[i].GetLong());
				}
				else if (props[i].PropTag == PropTag.Cn)
				{
					cn = new long?(props[i].GetLong());
					num++;
				}
			}
			if (num > 0 && !this.isChangeNumberExpected)
			{
				PropValue[] array = new PropValue[props.Length - num];
				int j = 0;
				int num2 = 0;
				while (j < props.Length)
				{
					if (props[j].PropTag != PropTag.Cn)
					{
						array[num2++] = props[j];
					}
					j++;
				}
				props = array;
			}
			base.Changes.Enqueue(delegate(IMapiHierarchyManifestCallback callback)
			{
				ManifestCallbackStatus result = callback.Change(props);
				if (fid != null)
				{
					this.checkpoint.IdGiven(fid.Value);
				}
				if (cn != null)
				{
					this.checkpoint.CnSeen(cn.Value);
				}
				return result;
			});
			return 0;
		}

		unsafe int IExchangeHierManifestCallback.Delete(int cbIdsetDeleted, IntPtr pbIdsetDeleted)
		{
			if (cbIdsetDeleted > 0)
			{
				if (this.isCallbackInfoExpected)
				{
					_CallbackInfo* ptr = (_CallbackInfo*)pbIdsetDeleted.ToPointer();
					int i = 0;
					while (i < cbIdsetDeleted)
					{
						byte[] entryId = new byte[ptr->cb];
						Marshal.Copy((IntPtr)((void*)ptr->pb), entryId, 0, ptr->cb);
						long fid = ptr->id;
						base.Deletes.Enqueue(delegate(IMapiHierarchyManifestCallback callback)
						{
							ManifestCallbackStatus result = callback.Delete(entryId);
							this.checkpoint.IdDeleted(fid);
							return result;
						});
						i++;
						ptr++;
					}
				}
				else
				{
					byte[] idsetDeleted = new byte[cbIdsetDeleted];
					Marshal.Copy(pbIdsetDeleted, idsetDeleted, 0, cbIdsetDeleted);
					base.Deletes.Enqueue((IMapiHierarchyManifestCallback callback) => callback.Delete(idsetDeleted));
				}
			}
			return 0;
		}

		private readonly HierarchyManifestCheckpoint checkpoint;

		private readonly bool isCallbackInfoExpected;

		private readonly bool isChangeNumberExpected;
	}
}
