using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class AsyncOperationNotification : EwsStoreObject
	{
		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return AsyncOperationNotification.schema;
			}
		}

		internal override SearchFilter ItemClassFilter
		{
			get
			{
				return new SearchFilter.ContainsSubstring(ItemSchema.ItemClass, "IPM.Notification.", 1, 0);
			}
		}

		public KeyValuePair<string, LocalizedString>[] ExtendedAttributes
		{
			get
			{
				return (KeyValuePair<string, LocalizedString>[])this[AsyncOperationNotificationSchema.ExtendedAttributes];
			}
			set
			{
				this[AsyncOperationNotificationSchema.ExtendedAttributes] = value;
			}
		}

		public DateTime? LastModified
		{
			get
			{
				return (DateTime?)this[AsyncOperationNotificationSchema.LastModified];
			}
		}

		public LocalizedString Message
		{
			get
			{
				return (LocalizedString)this[AsyncOperationNotificationSchema.Message];
			}
			internal set
			{
				this[AsyncOperationNotificationSchema.Message] = value;
			}
		}

		public int? PercentComplete
		{
			get
			{
				return (int?)this[AsyncOperationNotificationSchema.PercentComplete];
			}
			internal set
			{
				this[AsyncOperationNotificationSchema.PercentComplete] = value;
			}
		}

		public string StartedBy
		{
			get
			{
				return (string)this[AsyncOperationNotificationSchema.StartedBy];
			}
		}

		internal ADRecipientOrAddress StartedByValue
		{
			get
			{
				return (ADRecipientOrAddress)this[AsyncOperationNotificationSchema.StartedByValue];
			}
			set
			{
				this[AsyncOperationNotificationSchema.StartedByValue] = value;
			}
		}

		public DateTime? StartTime
		{
			get
			{
				return (DateTime?)this[AsyncOperationNotificationSchema.StartTime];
			}
		}

		public AsyncOperationStatus Status
		{
			get
			{
				return (AsyncOperationStatus)this[AsyncOperationNotificationSchema.Status];
			}
			internal set
			{
				this[AsyncOperationNotificationSchema.Status] = value;
			}
		}

		public LocalizedString DisplayName
		{
			get
			{
				return (LocalizedString)this[AsyncOperationNotificationSchema.DisplayName];
			}
			internal set
			{
				this[AsyncOperationNotificationSchema.DisplayName] = value;
			}
		}

		public AsyncOperationType Type
		{
			get
			{
				return (AsyncOperationType)this[AsyncOperationNotificationSchema.Type];
			}
			internal set
			{
				this[AsyncOperationNotificationSchema.Type] = value;
			}
		}

		public MultiValuedProperty<ADRecipientOrAddress> NotificationEmails
		{
			get
			{
				return (MultiValuedProperty<ADRecipientOrAddress>)this[AsyncOperationNotificationSchema.NotificationEmails];
			}
			internal set
			{
				this[AsyncOperationNotificationSchema.NotificationEmails] = value;
			}
		}

		internal bool IsSettingsObject
		{
			get
			{
				return AsyncOperationNotification.IsSettingsObjectId(base.AlternativeId);
			}
		}

		internal bool IsNotificationEmailFromTaskSent
		{
			get
			{
				return (bool)this[AsyncOperationNotificationSchema.IsNotificationEmailFromTaskSent];
			}
			set
			{
				this[AsyncOperationNotificationSchema.IsNotificationEmailFromTaskSent] = value;
			}
		}

		public static bool IsSettingsObjectId(string id)
		{
			return AsyncOperationNotificationDataProvider.SettingsObjectIdentityMap.ContainsValue(id);
		}

		public LocalizedString GetExtendedAttributeValue(string key)
		{
			LocalizedString result;
			if (this.TryGetExtendedAttributeValue(key, out result))
			{
				return result;
			}
			throw new KeyNotFoundException(key);
		}

		public bool TryGetExtendedAttributeValue(string key, out LocalizedString result)
		{
			result = LocalizedString.Empty;
			bool result2 = false;
			if (this.ExtendedAttributes != null)
			{
				foreach (KeyValuePair<string, LocalizedString> keyValuePair in this.ExtendedAttributes)
				{
					if (key == keyValuePair.Key)
					{
						result = keyValuePair.Value;
						result2 = true;
					}
				}
			}
			return result2;
		}

		internal static object GetAsyncOperationType(IPropertyBag propertyPag)
		{
			AsyncOperationType asyncOperationType = AsyncOperationType.Unknown;
			string text = (string)propertyPag[EwsStoreObjectSchema.ItemClass];
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			if (text.StartsWith("IPM.Notification."))
			{
				Enum.TryParse<AsyncOperationType>(text.Substring("IPM.Notification.".Length), true, out asyncOperationType);
			}
			return asyncOperationType;
		}

		internal static void SetAsyncOperationType(object value, IPropertyBag propertyPag)
		{
			propertyPag[EwsStoreObjectSchema.ItemClass] = "IPM.Notification." + ((AsyncOperationType)value).ToString();
		}

		internal static object GetStartedBy(IPropertyBag propertyPag)
		{
			ADRecipientOrAddress adrecipientOrAddress = (ADRecipientOrAddress)propertyPag[AsyncOperationNotificationSchema.StartedByValue];
			string result;
			if (adrecipientOrAddress != null)
			{
				if ((result = adrecipientOrAddress.DisplayName) == null)
				{
					return adrecipientOrAddress.Address;
				}
			}
			else
			{
				result = null;
			}
			return result;
		}

		internal static void SetNotificationEmails(Item item, object value)
		{
			if (value != null)
			{
				EmailAddressCollection toRecipients = ((EmailMessage)item).ToRecipients;
				IEnumerable<string> source = (IEnumerable<string>)value;
				toRecipients.Clear();
				toRecipients.AddRange((from x in source
				select new EmailAddress(x)).ToArray<EmailAddress>());
			}
		}

		internal const string ItemClassPrefix = "IPM.Notification.";

		private static ObjectSchema schema = new AsyncOperationNotificationSchema();
	}
}
