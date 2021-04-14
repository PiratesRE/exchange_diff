using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Services.Core.Types
{
	public class OutlookServiceNotificationSubscription : ICloneable
	{
		public OutlookServiceNotificationSubscription()
		{
		}

		internal OutlookServiceNotificationSubscription(IOutlookServiceSubscriptionItem item)
		{
			this.SubscriptionId = item.SubscriptionId;
			this.LastUpdateTime = new ExDateTime?(item.LastUpdateTimeUTC);
			this.AppId = item.AppId;
			this.PackageId = item.PackageId;
			this.DeviceNotificationId = item.DeviceNotificationId;
			this.ExpirationTime = new ExDateTime?(item.ExpirationTime);
			this.LockScreen = item.LockScreen;
		}

		public object Clone()
		{
			return base.MemberwiseClone();
		}

		public string SubscriptionId { get; set; }

		public ExDateTime? LastUpdateTime { get; set; }

		public string AppId { get; set; }

		public string PackageId { get; set; }

		public string DeviceNotificationId { get; set; }

		public ExDateTime? ExpirationTime { get; set; }

		public bool LockScreen { get; set; }

		public virtual string ToFullString()
		{
			if (this.toFullStringCache == null)
			{
				this.toFullStringCache = string.Format("{{SubscriptionId:{0}; LastUpdateTime {1}; PackageId:{2}; AppId:{3}; DeviceNotificationId:{4}; ExpirationTime:{5}; LockScreen:{6}}}", new object[]
				{
					this.SubscriptionId ?? "null",
					(this.LastUpdateTime != null) ? this.LastUpdateTime.Value.ToString() : "null",
					this.PackageId ?? "null",
					this.AppId ?? "null",
					this.DeviceNotificationId ?? "null",
					(this.ExpirationTime != null) ? this.ExpirationTime.Value.ToString() : "null",
					this.LockScreen
				});
			}
			return this.toFullStringCache;
		}

		public static string GenerateSubscriptionId(string appId, string deviceId)
		{
			if (string.IsNullOrWhiteSpace(appId))
			{
				throw new ArgumentException("Must have non-null and non-whitespace appid", "appId");
			}
			if (string.IsNullOrWhiteSpace(deviceId))
			{
				throw new ArgumentException("Must have non-null and non-whitespace deviceId", "deviceId");
			}
			string text = string.Format(CultureInfo.InvariantCulture, "{0}-{1}", new object[]
			{
				appId,
				deviceId
			});
			text = Regex.Replace(text, "[^a-zA-Z0-9-_@#.:]+", string.Empty);
			if ((long)text.Length > 120L)
			{
				throw new InvalidOperationException("subscriptionId is too long : " + text);
			}
			return text;
		}

		public const uint DefaultSubscriptionDeactivationInHours = 72U;

		private const uint MaxSubscriptionIdLength = 120U;

		public static readonly string AppId_HxMail = PushNotificationCannedApp.WnsOutlookMailOfficialWindowsImmersive.Name;

		public static readonly string AppId_HxCalendar = PushNotificationCannedApp.WnsOutlookCalendarOfficialWindowsImmersive.Name;

		private string toFullStringCache;
	}
}
