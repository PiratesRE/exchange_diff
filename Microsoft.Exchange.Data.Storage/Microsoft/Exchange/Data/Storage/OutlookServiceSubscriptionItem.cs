using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.OutlookService.Service;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class OutlookServiceSubscriptionItem : Item, IOutlookServiceSubscriptionItem, IItem, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		internal OutlookServiceSubscriptionItem(ICoreItem coreItem) : base(coreItem, false)
		{
			if (base.IsNew)
			{
				this.Initialize();
			}
		}

		public string SubscriptionId
		{
			get
			{
				this.CheckDisposed("SubscriptionId::get");
				return base.GetValueOrDefault<string>(OutlookServiceSubscriptionItemSchema.SubscriptionId, null);
			}
			set
			{
				this.CheckDisposed("SubscriptionId::set");
				this[OutlookServiceSubscriptionItemSchema.SubscriptionId] = value;
			}
		}

		public ExDateTime LastUpdateTimeUTC
		{
			get
			{
				this.CheckDisposed("LastUpdateTimeUTC::get");
				return base.GetValueOrDefault<ExDateTime>(OutlookServiceSubscriptionItemSchema.LastUpdateTimeUTC, ExDateTime.UtcNow);
			}
			set
			{
				this.CheckDisposed("LastUpdateTimeUTC::set");
				this[OutlookServiceSubscriptionItemSchema.LastUpdateTimeUTC] = value;
			}
		}

		public string PackageId
		{
			get
			{
				this.CheckDisposed("PackageId::get");
				return base.GetValueOrDefault<string>(OutlookServiceSubscriptionItemSchema.PackageId, this.AppId);
			}
			set
			{
				this.CheckDisposed("PackageId::set");
				this[OutlookServiceSubscriptionItemSchema.PackageId] = value;
			}
		}

		public string AppId
		{
			get
			{
				this.CheckDisposed("AppId::get");
				return base.GetValueOrDefault<string>(OutlookServiceSubscriptionItemSchema.AppId, null);
			}
			set
			{
				this.CheckDisposed("AppId::set");
				this[OutlookServiceSubscriptionItemSchema.AppId] = value;
			}
		}

		public string DeviceNotificationId
		{
			get
			{
				this.CheckDisposed("DeviceNotificationId::get");
				return base.GetValueOrDefault<string>(OutlookServiceSubscriptionItemSchema.DeviceNotificationId, null);
			}
			set
			{
				this.CheckDisposed("DeviceNotificationId::set");
				this[OutlookServiceSubscriptionItemSchema.DeviceNotificationId] = value;
			}
		}

		public ExDateTime ExpirationTime
		{
			get
			{
				this.CheckDisposed("ExpirationTime::get");
				return base.GetValueOrDefault<ExDateTime>(OutlookServiceSubscriptionItemSchema.ExpirationTime, ExDateTime.UtcNow);
			}
			set
			{
				this.CheckDisposed("ExpirationTime::set");
				this[OutlookServiceSubscriptionItemSchema.ExpirationTime] = value;
			}
		}

		public bool LockScreen
		{
			get
			{
				this.CheckDisposed("LockScreen::get");
				return base.GetValueOrDefault<bool>(OutlookServiceSubscriptionItemSchema.LockScreen, true);
			}
			set
			{
				this.CheckDisposed("LockScreen::set");
				this[OutlookServiceSubscriptionItemSchema.LockScreen] = value;
			}
		}

		public override Schema Schema
		{
			get
			{
				this.CheckDisposed("Schema::get");
				return OutlookServiceSubscriptionItemSchema.Instance;
			}
		}

		private void Initialize()
		{
			this[InternalSchema.ItemClass] = "OutlookService.Notification.Subscription";
			this[OutlookServiceSubscriptionItemSchema.LastUpdateTimeUTC] = ExDateTime.UtcNow;
			if (ExTraceGlobals.StorageNotificationSubscriptionTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug<string>((long)this.GetHashCode(), "OutlookServiceSubscriptionItem.Initialize: Initialized new SubscriptionItem, RefTm:{1}", this[OutlookServiceSubscriptionItemSchema.LastUpdateTimeUTC].ToString());
			}
		}
	}
}
