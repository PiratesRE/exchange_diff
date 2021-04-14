using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MapiCollectorEx : MapiCollectorBase
	{
		internal MapiCollectorEx(SafeExImportContentsChanges4Handle iExchangeImportContentsChanges4, MapiStore mapiStore, byte[] stateIdsetGiven, byte[] stateCnsetSeen, byte[] stateCnsetSeenFAI, byte[] stateCnsetRead, CollectorConfigFlags flags) : base(iExchangeImportContentsChanges4, mapiStore)
		{
			if (stateIdsetGiven == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("stateIdsetGiven");
			}
			if (stateCnsetSeen == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("stateCnsetSeen");
			}
			if (stateCnsetSeenFAI == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("stateCnsetSeenFAI");
			}
			if (stateCnsetRead == null)
			{
				throw MapiExceptionHelper.ArgumentNullException("stateCnsetRead");
			}
			base.LockStore();
			try
			{
				this.iExchangeImportContentsChanges4 = iExchangeImportContentsChanges4;
				int num = this.iExchangeImportContentsChanges4.ConfigEx(stateIdsetGiven, stateIdsetGiven.Length, stateCnsetSeen, stateCnsetSeen.Length, stateCnsetSeenFAI, stateCnsetSeenFAI.Length, stateCnsetRead, stateCnsetRead.Length, (int)flags);
				if (num != 0)
				{
					base.ThrowIfErrorOrWarning("Unable to configure ICS collector.", num);
				}
			}
			finally
			{
				base.UnlockStore();
			}
		}

		protected override void MapiInternalDispose()
		{
			this.iExchangeImportContentsChanges4 = null;
			base.MapiInternalDispose();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MapiCollectorEx>(this);
		}

		public void GetState(out byte[] stateIdsetGiven, out byte[] stateCnsetSeen, out byte[] stateCnsetSeenFAI, out byte[] stateCnsetRead)
		{
			base.CheckDisposed();
			base.LockStore();
			try
			{
				IntPtr zero = IntPtr.Zero;
				IntPtr zero2 = IntPtr.Zero;
				IntPtr zero3 = IntPtr.Zero;
				IntPtr zero4 = IntPtr.Zero;
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				int num5 = this.iExchangeImportContentsChanges4.UpdateStateEx(out zero, out num, out zero2, out num2, out zero3, out num3, out zero4, out num4);
				if (num5 != 0)
				{
					base.ThrowIfErrorOrWarning("Unable to get collector state.", num5);
				}
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
				stateCnsetSeenFAI = new byte[num3];
				if (num3 > 0)
				{
					Marshal.Copy(zero3, stateCnsetSeenFAI, 0, num3);
				}
				stateCnsetRead = new byte[num4];
				if (num4 > 0)
				{
					Marshal.Copy(zero4, stateCnsetRead, 0, num4);
				}
			}
			finally
			{
				base.UnlockStore();
			}
		}

		public MapiCollectorStatus ImportMessageMove(PropValue sourceFolderKey, PropValue sourceMessageKey, PropValue changeListKey, PropValue destMessageKey, PropValue changeNumberKey)
		{
			base.CheckDisposed();
			base.LockStore();
			MapiCollectorStatus result;
			try
			{
				int hr = base.InternalImportMessageMove(sourceFolderKey, sourceMessageKey, changeListKey, destMessageKey, changeNumberKey);
				MapiCollectorStatus mapiCollectorStatus;
				if (!MapiCollectorEx.TryGetMapiCollectorStatus(hr, out mapiCollectorStatus))
				{
					base.ThrowIfErrorOrWarning("Unable to import message move.", hr);
				}
				result = mapiCollectorStatus;
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public MapiCollectorStatus ImportMessageChange(PropValue[] propValues, ImportMessageChangeFlags importMessageChangeFlags, out MapiMessage mapiMessage)
		{
			base.CheckDisposed();
			mapiMessage = null;
			base.LockStore();
			MapiCollectorStatus result;
			try
			{
				bool flag = false;
				try
				{
					int hr = base.InternalImportMessageChange(propValues, importMessageChangeFlags, out mapiMessage);
					MapiCollectorStatus mapiCollectorStatus;
					if (!MapiCollectorEx.TryGetMapiCollectorStatus(hr, out mapiCollectorStatus))
					{
						base.ThrowIfErrorOrWarning("Unable to import message change.", hr);
					}
					flag = true;
					result = mapiCollectorStatus;
				}
				finally
				{
					if (!flag && mapiMessage != null)
					{
						mapiMessage.Dispose();
						mapiMessage = null;
					}
				}
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		public void ThrowOnCollectorStatus(MapiCollectorStatus mapiCollectorStatus)
		{
			if (mapiCollectorStatus != MapiCollectorStatus.Success)
			{
				MapiExceptionHelper.ThrowIfErrorOrWarning("ThrowOnCollectorStatus", (int)mapiCollectorStatus, base.AllowWarnings);
			}
		}

		private static bool TryGetMapiCollectorStatus(int hr, out MapiCollectorStatus mapiCollectorStatus)
		{
			if (hr <= -2147221233)
			{
				switch (hr)
				{
				case -2147221239:
				case -2147221238:
					break;
				default:
					if (hr != -2147221233)
					{
						goto IL_51;
					}
					break;
				}
			}
			else
			{
				switch (hr)
				{
				case -2147219456:
				case -2147219455:
				case -2147219454:
					break;
				default:
					if (hr != 0 && hr != 264225)
					{
						goto IL_51;
					}
					break;
				}
			}
			mapiCollectorStatus = (MapiCollectorStatus)hr;
			return true;
			IL_51:
			mapiCollectorStatus = MapiCollectorStatus.Failed;
			return false;
		}

		private SafeExImportContentsChanges4Handle iExchangeImportContentsChanges4;
	}
}
