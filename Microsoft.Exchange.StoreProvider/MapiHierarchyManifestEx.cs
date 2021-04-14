using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MapiHierarchyManifestEx : MapiUnk
	{
		internal unsafe MapiHierarchyManifestEx(SafeExExportHierManifestExHandle iExchangeExportHierManifestEx, MapiStore mapiStore, SyncConfigFlags flags, Restriction restriction, byte[] stateIdsetGiven, byte[] stateCnsetSeen, IMapiHierarchyManifestCallback iMapiHierarchyManifestCallback, ICollection<PropTag> tagsInclude, ICollection<PropTag> tagsExclude) : base(iExchangeExportHierManifestEx, null, mapiStore)
		{
			if (iMapiHierarchyManifestCallback == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("iMapiHierarchyManifestCallback");
			}
			if (stateIdsetGiven == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("stateIdsetGiven");
			}
			if (stateCnsetSeen == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("stateCnsetSeen");
			}
			base.LockStore();
			try
			{
				this.iExchangeExportHierManifestEx = iExchangeExportHierManifestEx;
				bool isChangeNumberExpected;
				if (tagsInclude != null && tagsInclude.Count > 0)
				{
					isChangeNumberExpected = false;
					using (IEnumerator<PropTag> enumerator = tagsInclude.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							PropTag propTag = enumerator.Current;
							if (propTag == PropTag.Cn)
							{
								isChangeNumberExpected = true;
								break;
							}
						}
						goto IL_D1;
					}
				}
				if (tagsExclude != null && tagsExclude.Count > 0)
				{
					isChangeNumberExpected = true;
					using (IEnumerator<PropTag> enumerator2 = tagsExclude.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							PropTag propTag2 = enumerator2.Current;
							if (propTag2 == PropTag.Cn)
							{
								isChangeNumberExpected = false;
								break;
							}
						}
						goto IL_D1;
					}
				}
				isChangeNumberExpected = false;
				IL_D1:
				this.manifestCheckpoint = new HierarchyManifestCheckpoint(base.MapiStore, stateIdsetGiven, stateCnsetSeen, this.iExchangeExportHierManifestEx, MapiHierarchyManifestEx.maxUncoalescedCount);
				this.manifestCallbackHelper = new HierarchyManifestCallbackHelper(this.manifestCheckpoint, (flags & SyncConfigFlags.ManifestHierReturnDeletedEntryIds) == SyncConfigFlags.ManifestHierReturnDeletedEntryIds, isChangeNumberExpected);
				int hr = 0;
				this.iMapiHierarchyManifestCallback = iMapiHierarchyManifestCallback;
				if (restriction != null)
				{
					int num = 4;
					num += restriction.GetBytesToMarshal();
					try
					{
						fixed (byte* ptr = new byte[num])
						{
							SRestriction* ptr2 = null;
							byte* ptr3 = ptr;
							ptr2 = (SRestriction*)ptr;
							ptr3 += (Marshal.SizeOf(typeof(SRestriction)) + 7 & -8);
							restriction.MarshalToNative(ptr2, ref ptr3);
							hr = this.iExchangeExportHierManifestEx.Config(stateIdsetGiven, stateIdsetGiven.Length, stateCnsetSeen, stateCnsetSeen.Length, flags, this.manifestCallbackHelper, ptr2, (tagsInclude != null && tagsInclude.Count > 0) ? PropTagHelper.SPropTagArray(tagsInclude) : null, (tagsExclude != null && tagsExclude.Count > 0) ? PropTagHelper.SPropTagArray(tagsExclude) : null);
							goto IL_22E;
						}
					}
					finally
					{
						byte* ptr = null;
					}
				}
				hr = this.iExchangeExportHierManifestEx.Config(stateIdsetGiven, stateIdsetGiven.Length, stateCnsetSeen, stateCnsetSeen.Length, flags, this.manifestCallbackHelper, null, (tagsInclude != null && tagsInclude.Count > 0) ? PropTagHelper.SPropTagArray(tagsInclude) : null, (tagsExclude != null && tagsExclude.Count > 0) ? PropTagHelper.SPropTagArray(tagsExclude) : null);
				IL_22E:
				base.ThrowIfError("Unable to configure MapiHierarchyManifestEx ICS synchronizer.", hr);
				this.done = false;
				this.synchronizationDone = false;
			}
			finally
			{
				base.UnlockStore();
			}
		}

		protected override void MapiInternalDispose()
		{
			this.iExchangeExportHierManifestEx = null;
			this.manifestCallbackHelper = null;
			this.manifestCheckpoint = null;
			base.MapiInternalDispose();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MapiHierarchyManifestEx>(this);
		}

		public ManifestStatus Synchronize()
		{
			base.CheckDisposed();
			ManifestStatus manifestStatus = ManifestStatus.Done;
			base.LockStore();
			ManifestStatus result;
			try
			{
				if (!this.done && !this.synchronizationDone)
				{
					int num = 264224;
					while (264224 == num)
					{
						manifestStatus = this.manifestCallbackHelper.DoCallbacks(this.iMapiHierarchyManifestCallback, new ManifestCallbackQueue<IMapiHierarchyManifestCallback>[]
						{
							this.manifestCallbackHelper.Changes,
							this.manifestCallbackHelper.Deletes
						});
						if (manifestStatus != ManifestStatus.Done)
						{
							break;
						}
						num = this.iExchangeExportHierManifestEx.Synchronize(0);
						if (num != 0 && num != 264224)
						{
							base.ThrowIfError("Unable to synchronize manifest.", num);
						}
						if (num == 0)
						{
							this.synchronizationDone = true;
						}
					}
				}
				if (manifestStatus == ManifestStatus.Done)
				{
					manifestStatus = this.manifestCallbackHelper.DoCallbacks(this.iMapiHierarchyManifestCallback, new ManifestCallbackQueue<IMapiHierarchyManifestCallback>[]
					{
						this.manifestCallbackHelper.Changes,
						this.manifestCallbackHelper.Deletes
					});
				}
				if (manifestStatus == ManifestStatus.Done)
				{
					this.done = true;
				}
				result = manifestStatus;
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public void GetState(out byte[] stateIdsetGiven, out byte[] stateCnsetSeen)
		{
			base.CheckDisposed();
			base.LockStore();
			try
			{
				if (this.done && this.manifestCallbackHelper.Changes.Count == 0 && this.manifestCallbackHelper.Deletes.Count == 0)
				{
					IntPtr zero = IntPtr.Zero;
					IntPtr zero2 = IntPtr.Zero;
					int num = 0;
					int num2 = 0;
					int state = this.iExchangeExportHierManifestEx.GetState(out zero, out num, out zero2, out num2);
					base.ThrowIfError("Unable to get final ICS state.", state);
					stateIdsetGiven = new byte[num];
					if (num > 0)
					{
						Marshal.Copy(zero, stateIdsetGiven, 0, num);
					}
					stateCnsetSeen = new byte[num2];
					if (num2 > 0)
					{
						Marshal.Copy(zero2, stateCnsetSeen, 0, num2);
					}
				}
				else
				{
					this.manifestCheckpoint.Checkpoint(out stateIdsetGiven, out stateCnsetSeen);
				}
			}
			finally
			{
				base.UnlockStore();
			}
		}

		private static readonly int maxUncoalescedCount = 250;

		private SafeExExportHierManifestExHandle iExchangeExportHierManifestEx;

		private IMapiHierarchyManifestCallback iMapiHierarchyManifestCallback;

		private HierarchyManifestCallbackHelper manifestCallbackHelper;

		private HierarchyManifestCheckpoint manifestCheckpoint;

		private bool done;

		private bool synchronizationDone;
	}
}
