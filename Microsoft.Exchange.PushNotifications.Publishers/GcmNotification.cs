using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class GcmNotification : PushNotification
	{
		public GcmNotification(string appId, OrganizationId tenantId, string registrationId, GcmPayload payload, string collapseKey = "c", bool? delayWhileIdle = null, int? timeToLive = null) : base(appId, tenantId)
		{
			this.RegistrationId = registrationId;
			this.Payload = payload;
			this.CollapseKey = collapseKey;
			this.DelayWhileIdle = delayWhileIdle;
			this.TimeToLive = timeToLive;
			if (payload != null)
			{
				payload.NotificationId = base.Identifier;
				base.IsBackgroundSyncAvailable = (payload.BackgroundSyncType != BackgroundSyncType.None);
			}
		}

		public string RegistrationId { get; private set; }

		public override string RecipientId
		{
			get
			{
				return this.RegistrationId;
			}
		}

		public GcmPayload Payload { get; private set; }

		public string CollapseKey { get; private set; }

		public bool? DelayWhileIdle { get; private set; }

		public int? TimeToLive { get; private set; }

		public virtual bool DryRun
		{
			get
			{
				return false;
			}
		}

		public string ToGcmFormat()
		{
			base.Validate();
			return this.serializedPayload;
		}

		protected override string InternalToFullString()
		{
			return string.Format("{0}; payload:{1}; collapseKey:{2}; delayWhileIdle:{3}; timeToLive:{4}", new object[]
			{
				base.InternalToFullString(),
				this.Payload.ToNullableString(null),
				this.CollapseKey.ToNullableString(),
				this.DelayWhileIdle.ToNullableString<bool>(),
				this.TimeToLive.ToNullableString<int>()
			});
		}

		protected override void RunValidationCheck(List<LocalizedString> errors)
		{
			base.RunValidationCheck(errors);
			if (string.IsNullOrWhiteSpace(this.RegistrationId))
			{
				errors.Add(Strings.GcmInvalidRegistrationId(this.RegistrationId.ToNullableString()));
			}
			if (this.TimeToLive != null && (this.TimeToLive.Value < 0 || this.TimeToLive.Value > 2419200))
			{
				errors.Add(Strings.GcmInvalidTimeToLive(this.TimeToLive.Value));
			}
			if (this.Payload == null || (this.Payload.UnseenEmailCount == null && string.IsNullOrWhiteSpace(this.Payload.Message) && string.IsNullOrWhiteSpace(this.Payload.ExtraData) && this.Payload.BackgroundSyncType == BackgroundSyncType.None))
			{
				errors.Add(Strings.GcmInvalidPayload);
			}
			this.serializedPayload = this.InternalToGcmFormat();
			if (this.serializedPayload.Length > 4096)
			{
				errors.Add(Strings.GcmInvalidPayloadLength(this.serializedPayload.Length, this.serializedPayload.Substring(0, Math.Min(this.serializedPayload.Length, 5120))));
			}
		}

		private string InternalToGcmFormat()
		{
			GcmPayloadWriter gcmPayloadWriter = new GcmPayloadWriter();
			gcmPayloadWriter.WriteProperty("registration_id", this.RegistrationId);
			gcmPayloadWriter.WriteProperty("collapse_key", this.CollapseKey);
			gcmPayloadWriter.WriteProperty<bool>("delay_while_idle", this.DelayWhileIdle);
			gcmPayloadWriter.WriteProperty<int>("time_to_live", this.TimeToLive);
			gcmPayloadWriter.WriteProperty("restricted_package_name", base.AppId);
			gcmPayloadWriter.WriteProperty<bool>("dry_run", this.IsMonitoring);
			if (this.Payload != null)
			{
				this.Payload.WriteGcmPayload(gcmPayloadWriter);
			}
			return gcmPayloadWriter.ToString();
		}

		private const int MaxPayloadSize = 4096;

		private const int MinTimeToLive = 0;

		private const int MaxTimeToLive = 2419200;

		private const string DefaultCollapseKey = "c";

		private string serializedPayload;
	}
}
