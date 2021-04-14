using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class ProxyNotification : PushNotification
	{
		public ProxyNotification(string appId, string tenantId, MailboxNotificationBatch batch) : base(appId, OrganizationId.ForestWideOrgId)
		{
			this.NotificationBatch = batch;
			this.recipientId = tenantId;
		}

		public ProxyNotification(string appId, IEnumerable<AzurePublisherSettings> azureSettings) : base(appId, OrganizationId.ForestWideOrgId)
		{
			this.AzureSettings = azureSettings;
		}

		public MailboxNotificationBatch NotificationBatch { get; private set; }

		public IEnumerable<AzurePublisherSettings> AzureSettings { get; private set; }

		public override string RecipientId
		{
			get
			{
				return this.recipientId;
			}
		}

		protected override void RunValidationCheck(List<LocalizedString> errors)
		{
			base.RunValidationCheck(errors);
			if (this.NotificationBatch == null && this.AzureSettings == null)
			{
				errors.Add(Strings.InvalidProxyNotificationBatch);
			}
		}

		protected override string InternalToFullString()
		{
			string format = "{0}; recipientId:{1}; batch:{2}; configuration:{3}";
			object[] array = new object[4];
			array[0] = base.InternalToFullString();
			array[1] = this.RecipientId;
			array[2] = this.NotificationBatch.ToNullableString((MailboxNotificationBatch x) => x.ToFullString());
			array[3] = this.AzureSettings.ToNullableString((AzurePublisherSettings x) => x.ToNullableString(null));
			return string.Format(format, array);
		}

		private readonly string recipientId;
	}
}
