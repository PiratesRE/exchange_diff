using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage.Clutter.Notifications;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Storage.Clutter
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class NotificationManager : IDisposable
	{
		public NotificationManager(MailboxSession session, VariantConfigurationSnapshot snapshot, IFrontEndLocator frontEndLocator) : this(session, snapshot, frontEndLocator, null)
		{
		}

		public NotificationManager(MailboxSession session, VariantConfigurationSnapshot snapshot, IFrontEndLocator frontEndLocator, UserConfiguration inferenceSettings)
		{
			this.session = session;
			this.snapshot = snapshot;
			this.frontEndLocator = frontEndLocator;
			if (inferenceSettings == null || inferenceSettings.GetDictionary() == null)
			{
				this.inferenceSettings = ClutterUtilities.GetInferenceSettingsConfiguration(session);
				this.ownsInferenceSettings = true;
			}
			else
			{
				this.inferenceSettings = inferenceSettings;
				this.ownsInferenceSettings = false;
			}
			this.localTimeZone = DateTimeHelper.GetUserTimeZoneOrUtc(this.session);
			this.MigrateNotificationType("ClutterReady", ClutterNotificationType.Invitation);
		}

		public static void SendNotification(ClutterNotificationType notificationType, DefaultFolderType folder, MailboxSession session, VariantConfigurationSnapshot snapshot, IFrontEndLocator frontEndLocator)
		{
			NotificationManager.SendNotification(notificationType, folder, session, snapshot, frontEndLocator, null);
		}

		public static void SendNotification(ClutterNotificationType notificationType, DefaultFolderType folder, MailboxSession session, VariantConfigurationSnapshot snapshot, IFrontEndLocator frontEndLocator, UserConfiguration inferenceSettings)
		{
			using (NotificationManager notificationManager = new NotificationManager(session, snapshot, frontEndLocator, inferenceSettings))
			{
				notificationManager.SendNotification(notificationType, folder);
				notificationManager.Save();
			}
		}

		public void SendNotification(ClutterNotificationType notificationType, DefaultFolderType folder)
		{
			this.ThrowIfNone(notificationType);
			ClutterNotification notification = this.GetNotification(notificationType);
			using (MessageItem messageItem = notification.Compose(folder))
			{
				messageItem.Load(new PropertyDefinition[]
				{
					MessageItemSchema.InferenceMessageIdentifier
				});
				Guid? valueAsNullable = messageItem.GetValueAsNullable<Guid>(MessageItemSchema.InferenceMessageIdentifier);
				this.SetUserConfigurationProperty(this.GetScheduledTimePropertyKey(notificationType), null);
				this.SetUserConfigurationProperty(this.GetSentTimePropertyKey(notificationType), ExDateTime.UtcNow);
				this.SetUserConfigurationProperty(this.GetInternetMessageIdPropertyKey(notificationType), messageItem.InternetMessageId);
				this.SetUserConfigurationProperty(this.GetMessageGuidPropertyKey(notificationType), (valueAsNullable != null) ? valueAsNullable.Value.ToString() : null);
				this.SetUserConfigurationProperty(this.GetServerVersionPropertyKey(notificationType), "15.00.1497.012");
				NotificationManager.LogNotificationSentActivity(this.session, notificationType, messageItem, valueAsNullable, folder);
			}
		}

		public void ScheduleNotification(ClutterNotificationType notificationType, int afterMinimumDays, DayOfWeek onDayOfWeek)
		{
			DayOfWeek onDayOfWeek2 = (onDayOfWeek - DayOfWeek.Monday < 0) ? (onDayOfWeek - 1 + 7) : (onDayOfWeek - 1);
			this.ScheduleNotification(notificationType, afterMinimumDays, onDayOfWeek2, TimeSpan.FromHours(12.0));
		}

		public void ScheduleNotification(ClutterNotificationType notificationType, int afterMinimumDays, DayOfWeek onDayOfWeek, TimeSpan atTimeOfDay)
		{
			this.ScheduleNotification(notificationType, DateTimeHelper.GetFutureTimestamp(ExDateTime.UtcNow, afterMinimumDays, onDayOfWeek, atTimeOfDay, this.localTimeZone));
		}

		public void ScheduleNotification(ClutterNotificationType notificationType, ExDateTime scheduledTime)
		{
			this.ThrowIfNone(notificationType);
			this.SetUserConfigurationProperty(this.GetScheduledTimePropertyKey(notificationType), scheduledTime);
		}

		public void CancelScheduledNotification(ClutterNotificationType notificationType)
		{
			this.ThrowIfNone(notificationType);
			this.SetUserConfigurationProperty(this.GetScheduledTimePropertyKey(notificationType), null);
		}

		public void CancelScheduledNotifications()
		{
			foreach (ClutterNotificationType clutterNotificationType in (ClutterNotificationType[])Enum.GetValues(typeof(ClutterNotificationType)))
			{
				if (clutterNotificationType != ClutterNotificationType.None)
				{
					this.CancelScheduledNotification(clutterNotificationType);
				}
			}
		}

		public ClutterNotificationType GetNextScheduledNotification(out ExDateTime scheduledTime)
		{
			ClutterNotificationType result = ClutterNotificationType.None;
			scheduledTime = ExDateTime.MaxValue;
			foreach (ClutterNotificationType clutterNotificationType in (ClutterNotificationType[])Enum.GetValues(typeof(ClutterNotificationType)))
			{
				if (clutterNotificationType != ClutterNotificationType.None)
				{
					ExDateTime? notificationScheduledTime = this.GetNotificationScheduledTime(clutterNotificationType);
					if (notificationScheduledTime != null && notificationScheduledTime.Value < scheduledTime)
					{
						result = clutterNotificationType;
						scheduledTime = notificationScheduledTime.Value;
					}
				}
			}
			return result;
		}

		public ExDateTime? GetNotificationScheduledTime(ClutterNotificationType notificationType)
		{
			this.ThrowIfNone(notificationType);
			return this.GetUserConfigurationProperty<ExDateTime?>(this.GetScheduledTimePropertyKey(notificationType), null);
		}

		public bool IsNotificationSent(ClutterNotificationType notificationType)
		{
			this.ThrowIfNone(notificationType);
			return this.GetNotificationSentTime(notificationType) != null;
		}

		public ExDateTime? GetNotificationSentTime(ClutterNotificationType notificationType)
		{
			this.ThrowIfNone(notificationType);
			return this.GetUserConfigurationProperty<ExDateTime?>(this.GetSentTimePropertyKey(notificationType), null);
		}

		public string GetNotificationInternetMessageId(ClutterNotificationType notificationType)
		{
			this.ThrowIfNone(notificationType);
			return this.GetUserConfigurationProperty<string>(this.GetInternetMessageIdPropertyKey(notificationType), null);
		}

		public string GetNotificationMessageGuid(ClutterNotificationType notificationType)
		{
			this.ThrowIfNone(notificationType);
			return this.GetUserConfigurationProperty<string>(this.GetMessageGuidPropertyKey(notificationType), null);
		}

		public string GetNotificationServerVersion(ClutterNotificationType notificationType)
		{
			this.ThrowIfNone(notificationType);
			return this.GetUserConfigurationProperty<string>(this.GetServerVersionPropertyKey(notificationType), null);
		}

		public void Save()
		{
			this.inferenceSettings.Save();
		}

		public void Dispose()
		{
			if (this.ownsInferenceSettings && this.inferenceSettings != null)
			{
				this.inferenceSettings.Dispose();
			}
		}

		private ClutterNotification GetNotification(ClutterNotificationType notificationType)
		{
			this.ThrowIfNone(notificationType);
			switch (notificationType)
			{
			case ClutterNotificationType.Invitation:
				return new InvitationNotification(this.session, this.snapshot, this.frontEndLocator);
			case ClutterNotificationType.OptedIn:
				return new OptInNotification(this.session, this.snapshot, this.frontEndLocator);
			case ClutterNotificationType.FirstReminder:
			case ClutterNotificationType.SecondReminder:
			case ClutterNotificationType.ThirdReminder:
				return new PeriodicNotification(this.session, this.snapshot, this.frontEndLocator);
			case ClutterNotificationType.AutoEnablementNotice:
				return new AutoEnablementNotice(this.session, this.snapshot, this.frontEndLocator);
			default:
				throw new ArgumentException(string.Format("Unknown clutter notification type: {0}", notificationType), "notificationType");
			}
		}

		private static void LogNotificationSentActivity(MailboxSession session, ClutterNotificationType notificationType, MessageItem message, Guid? messageGuid, DefaultFolderType folder)
		{
			if (session.ActivitySession != null)
			{
				Dictionary<string, string> messageProperties = new Dictionary<string, string>
				{
					{
						"NotificationType",
						notificationType.ToString()
					},
					{
						"InternetMessageId",
						message.InternetMessageId
					},
					{
						"MessageGuid",
						(messageGuid != null) ? messageGuid.Value.ToString() : string.Empty
					},
					{
						"CreationFolder",
						folder.ToString()
					}
				};
				session.ActivitySession.CaptureClutterNotificationSent(message.InternalObjectId, messageProperties);
			}
		}

		private string GetScheduledTimePropertyKey(ClutterNotificationType notificationType)
		{
			return this.GetPropertyKey(notificationType, "ScheduledTime");
		}

		private string GetSentTimePropertyKey(ClutterNotificationType notificationType)
		{
			return this.GetPropertyKey(notificationType, "SentTime");
		}

		private string GetInternetMessageIdPropertyKey(ClutterNotificationType notificationType)
		{
			return this.GetPropertyKey(notificationType, "InternetMessageId");
		}

		private string GetMessageGuidPropertyKey(ClutterNotificationType notificationType)
		{
			return this.GetPropertyKey(notificationType, "MessageGuid");
		}

		private string GetServerVersionPropertyKey(ClutterNotificationType notificationType)
		{
			return this.GetPropertyKey(notificationType, "ServerVersion");
		}

		private string GetPropertyKey(ClutterNotificationType notificationType, string propertySuffix)
		{
			this.ThrowIfNone(notificationType);
			return string.Format("{0}.{1}", notificationType.ToString(), propertySuffix);
		}

		private void MigrateNotificationType(string oldNotificationName, ClutterNotificationType notificationType)
		{
			this.MigrateProperty(oldNotificationName, notificationType.ToString(), "SentTime");
			this.MigrateProperty(oldNotificationName, notificationType.ToString(), "InternetMessageId");
			this.MigrateProperty(oldNotificationName, notificationType.ToString(), "MessageGuid");
			this.MigrateProperty(oldNotificationName, notificationType.ToString(), "ServerVersion");
		}

		private void MigrateProperty(string oldPrefix, string newPrefix, string suffix)
		{
			string property = string.Format("{0}.{1}", oldPrefix, suffix);
			string property2 = string.Format("{0}.{1}", newPrefix, suffix);
			object userConfigurationProperty = this.GetUserConfigurationProperty(property);
			object userConfigurationProperty2 = this.GetUserConfigurationProperty(property2);
			if (userConfigurationProperty != null && userConfigurationProperty2 == null)
			{
				this.SetUserConfigurationProperty(property, null);
				this.SetUserConfigurationProperty(property2, userConfigurationProperty);
			}
		}

		private T GetUserConfigurationProperty<T>(string property, T defaultValue)
		{
			object userConfigurationProperty = this.GetUserConfigurationProperty(property);
			if (userConfigurationProperty != null && userConfigurationProperty is T)
			{
				return (T)((object)userConfigurationProperty);
			}
			return defaultValue;
		}

		private object GetUserConfigurationProperty(string property)
		{
			if (!this.inferenceSettings.GetDictionary().Contains(property))
			{
				return null;
			}
			return this.inferenceSettings.GetDictionary()[property];
		}

		private void SetUserConfigurationProperty(string property, object value)
		{
			if (value == null)
			{
				this.inferenceSettings.GetDictionary().Remove(property);
				return;
			}
			this.inferenceSettings.GetDictionary()[property] = value;
		}

		private void ThrowIfNone(ClutterNotificationType notificationType)
		{
			if (notificationType == ClutterNotificationType.None)
			{
				throw new ArgumentException("ClutterNotificationType.None cannot have properties", "notificationType");
			}
		}

		public const string ScheduledTimeSuffix = "ScheduledTime";

		public const string SentTimeSuffix = "SentTime";

		public const string InternetMessageIdSuffix = "InternetMessageId";

		public const string MessageGuidSuffix = "MessageGuid";

		public const string ServerVersionSuffix = "ServerVersion";

		private readonly MailboxSession session;

		private readonly VariantConfigurationSnapshot snapshot;

		private readonly IFrontEndLocator frontEndLocator;

		private readonly ExTimeZone localTimeZone;

		private readonly UserConfiguration inferenceSettings;

		private readonly bool ownsInferenceSettings;
	}
}
