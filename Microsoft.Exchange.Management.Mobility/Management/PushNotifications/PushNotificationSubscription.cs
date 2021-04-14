using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Mobility;

namespace Microsoft.Exchange.Management.PushNotifications
{
	[Serializable]
	public class PushNotificationSubscription : ConfigurableObject
	{
		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return PushNotificationSubscription.schema;
			}
		}

		public PushNotificationSubscription() : base(new SimpleProviderPropertyBag())
		{
		}

		internal PushNotificationSubscription(ADObjectId userId, VersionedId versionId, string subscriptionId, string serializedSubscription) : base(new SimpleProviderPropertyBag())
		{
			this.propertyBag.SetField(PushNotificationSubscriptionSchema.SubscriptionId, subscriptionId);
			if (versionId != null && userId != null)
			{
				this.propertyBag.SetField(PushNotificationSubscriptionSchema.SubscriptionStoreId, new PushNotificationStoreId(userId, versionId.ObjectId, this.SubscriptionId));
			}
			try
			{
				this.propertyBag.SetField(PushNotificationSubscriptionSchema.DeserializedSubscription, PushNotificationServerSubscription.FromJson(serializedSubscription));
			}
			catch (SerializationException ex)
			{
				this.SerializationError = new PropertyValidationError(Strings.ErrorDeserializingSubscription(serializedSubscription, ex.Message), PushNotificationSubscriptionSchema.DeserializedSubscription, serializedSubscription);
			}
			this.propertyBag.ResetChangeTracking();
		}

		private ValidationError SerializationError { get; set; }

		public string AppId
		{
			get
			{
				return (string)this[PushNotificationSubscriptionSchema.AppId];
			}
		}

		public string DeviceNotificationId
		{
			get
			{
				return (string)this[PushNotificationSubscriptionSchema.DeviceNotificationId];
			}
		}

		public string DeviceNotificationType
		{
			get
			{
				return (string)this[PushNotificationSubscriptionSchema.DeviceNotificationType];
			}
		}

		public long? InboxUnreadCount
		{
			get
			{
				return new long?((long)this[PushNotificationSubscriptionSchema.InboxUnreadCount]);
			}
		}

		public PushNotificationStoreId SubscriptionStoreId
		{
			get
			{
				return (PushNotificationStoreId)this[PushNotificationSubscriptionSchema.SubscriptionStoreId];
			}
		}

		public DateTime? LastSubscriptionUpdate
		{
			get
			{
				return (DateTime?)this[PushNotificationSubscriptionSchema.LastSubscriptionUpdate];
			}
		}

		public string SubscriptionId
		{
			get
			{
				return (string)this[PushNotificationSubscriptionSchema.SubscriptionId];
			}
		}

		internal static object LastSubscriptionUpdateGetter(IPropertyBag propertyBag)
		{
			PushNotificationServerSubscription pushNotificationServerSubscription = PushNotificationSubscription.DeserializedSubscriptionGetter(propertyBag);
			return (pushNotificationServerSubscription == null) ? null : new DateTime?(pushNotificationServerSubscription.LastSubscriptionUpdate);
		}

		internal static object AppIdGetter(IPropertyBag propertyBag)
		{
			PushNotificationServerSubscription pushNotificationServerSubscription = PushNotificationSubscription.DeserializedSubscriptionGetter(propertyBag);
			if (pushNotificationServerSubscription != null)
			{
				return pushNotificationServerSubscription.AppId;
			}
			return null;
		}

		internal static object DeviceNotificationIdGetter(IPropertyBag propertyBag)
		{
			PushNotificationServerSubscription pushNotificationServerSubscription = PushNotificationSubscription.DeserializedSubscriptionGetter(propertyBag);
			if (pushNotificationServerSubscription != null)
			{
				return pushNotificationServerSubscription.DeviceNotificationId;
			}
			return null;
		}

		internal static object DeviceNotificationTypeGetter(IPropertyBag propertyBag)
		{
			PushNotificationServerSubscription pushNotificationServerSubscription = PushNotificationSubscription.DeserializedSubscriptionGetter(propertyBag);
			if (pushNotificationServerSubscription != null)
			{
				return pushNotificationServerSubscription.DeviceNotificationType;
			}
			return null;
		}

		internal static object InboxUnreadCountGetter(IPropertyBag propertyBag)
		{
			PushNotificationServerSubscription pushNotificationServerSubscription = PushNotificationSubscription.DeserializedSubscriptionGetter(propertyBag);
			return (pushNotificationServerSubscription == null) ? null : pushNotificationServerSubscription.InboxUnreadCount;
		}

		private static PushNotificationServerSubscription DeserializedSubscriptionGetter(IPropertyBag propertyBag)
		{
			return (PushNotificationServerSubscription)propertyBag[PushNotificationSubscriptionSchema.DeserializedSubscription];
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			if (this.SubscriptionStoreId == null)
			{
				errors.Add(new PropertyValidationError(Strings.NullSubscriptionStoreId, PushNotificationSubscriptionSchema.SubscriptionStoreId, this.SubscriptionStoreId));
			}
			if (this.SerializationError != null)
			{
				errors.Add(this.SerializationError);
			}
			base.ValidateRead(errors);
		}

		private static PushNotificationSubscriptionSchema schema = ObjectSchema.GetInstance<PushNotificationSubscriptionSchema>();
	}
}
