using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MapiUnk : DisposeTrackableBase
	{
		protected MapiUnk()
		{
		}

		protected MapiUnk(IExInterface iUnknown, object externalIUnknown, MapiStore mapiStore)
		{
			bool flag = false;
			try
			{
				if (externalIUnknown == null)
				{
					if (iUnknown == null || iUnknown.IsInvalid)
					{
						throw MapiExceptionHelper.ArgumentException("iUnknown", "Unable to create MapiUnk object with null or invalid interface handle.");
					}
				}
				else if (iUnknown != null)
				{
					throw MapiExceptionHelper.ArgumentException("iUnknown", "Cannot create external interace object with also an iUnknown interface.");
				}
				this.iUnknown = iUnknown;
				this.externalIUnknown = externalIUnknown;
				this.childRef = null;
				this.mapiStore = mapiStore;
				if (this.mapiStore != null)
				{
					this.childRef = this.mapiStore.AddChildReference(this);
					this.allowWarnings = this.mapiStore.AllowWarnings;
				}
				else
				{
					this.allowWarnings = false;
				}
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					this.Dispose();
				}
			}
		}

		protected virtual void MapiInternalDispose()
		{
		}

		protected virtual void PostMapiInternalDispose()
		{
		}

		internal new void CheckDisposed()
		{
			if (base.IsDisposed)
			{
				throw MapiExceptionHelper.ObjectDisposedException(base.GetType().ToString());
			}
		}

		internal void LockStore()
		{
			if (this.mapiStore != null)
			{
				this.mapiStore.LockConnection();
			}
		}

		internal void UnlockStore()
		{
			if (this.mapiStore != null)
			{
				this.mapiStore.UnlockConnection();
			}
		}

		internal bool IsExternal
		{
			get
			{
				return this.externalIUnknown != null;
			}
		}

		public MapiStore MapiStore
		{
			get
			{
				this.CheckDisposed();
				return this.mapiStore;
			}
		}

		public override void SuppressDisposeTracker()
		{
			if (this.iUnknown != null)
			{
				this.iUnknown.SuppressDisposeTracker();
			}
			base.SuppressDisposeTracker();
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.LockStore();
				try
				{
					if (this.notificationCallbackIds != null)
					{
						foreach (ulong notificationCallbackId in this.notificationCallbackIds)
						{
							NotificationCallbackHelper.Instance.UnregisterNotificationHelper(notificationCallbackId);
						}
						this.notificationCallbackIds = null;
					}
					this.MapiInternalDispose();
					this.iUnknown.DisposeIfValid();
					this.iUnknown = null;
					MapiUnk.ReleaseObject(this.externalIUnknown);
					this.externalIUnknown = null;
					if (this.childRef != null)
					{
						DisposableRef.RemoveFromList(this.childRef);
						this.childRef.Dispose();
						this.childRef = null;
					}
					this.PostMapiInternalDispose();
				}
				finally
				{
					this.UnlockStore();
				}
				this.mapiStore = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MapiUnk>(this);
		}

		public bool AllowWarnings
		{
			get
			{
				return this.allowWarnings;
			}
			set
			{
				this.allowWarnings = value;
			}
		}

		protected void ThrowIfError(string message, int hr)
		{
			if (this.IsExternal)
			{
				MapiExceptionHelper.ThrowIfError(message, hr, this.externalIUnknown, this.LastLowLevelException);
				return;
			}
			MapiExceptionHelper.ThrowIfError(message, hr, this.iUnknown, this.LastLowLevelException);
		}

		protected void ThrowIfErrorOrWarning(string message, int hr)
		{
			if (this.IsExternal)
			{
				MapiExceptionHelper.ThrowIfErrorOrWarning(message, hr, this.AllowWarnings, this.externalIUnknown, this.LastLowLevelException);
				return;
			}
			MapiExceptionHelper.ThrowIfErrorOrWarning(message, hr, this.AllowWarnings, this.iUnknown, this.LastLowLevelException);
		}

		protected void BlockExternalObjectCheck()
		{
			if (this.IsExternal)
			{
				throw MapiExceptionHelper.NotSupportedException("Method is not supported with external object.");
			}
		}

		protected static void ReleaseObject(object obj)
		{
			if (obj != null && Marshal.IsComObject(obj))
			{
				try
				{
					Marshal.ReleaseComObject(obj);
				}
				catch (InvalidComObjectException)
				{
				}
			}
		}

		protected ulong RegisterNotificationHelper(NotificationHelper notificationHelper)
		{
			this.LockStore();
			ulong result;
			try
			{
				if (this.notificationCallbackIds == null)
				{
					this.notificationCallbackIds = new List<ulong>(4);
				}
				ulong num = NotificationCallbackHelper.Instance.RegisterNotificationHelper(notificationHelper);
				this.notificationCallbackIds.Add(num);
				result = num;
			}
			finally
			{
				this.UnlockStore();
			}
			return result;
		}

		protected void UnregisterNotificationHelper(ulong callbackId)
		{
			this.LockStore();
			if (this.notificationCallbackIds == null)
			{
				return;
			}
			try
			{
				this.notificationCallbackIds.Remove(callbackId);
				NotificationCallbackHelper.Instance.UnregisterNotificationHelper(callbackId);
			}
			finally
			{
				this.UnlockStore();
			}
		}

		private Exception LastLowLevelException
		{
			get
			{
				if (this.mapiStore != null)
				{
					return this.mapiStore.LastLowLevelException;
				}
				return null;
			}
		}

		private IExInterface iUnknown;

		private object externalIUnknown;

		private MapiStore mapiStore;

		private DisposableRef childRef;

		private bool allowWarnings;

		private List<ulong> notificationCallbackIds;
	}
}
