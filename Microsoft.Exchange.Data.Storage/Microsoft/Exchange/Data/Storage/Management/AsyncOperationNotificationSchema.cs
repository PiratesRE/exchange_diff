using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AsyncOperationNotificationSchema : EwsStoreObjectSchema
	{
		public static readonly EwsStoreObjectPropertyDefinition ExtendedAttributes = new EwsStoreObjectPropertyDefinition("ExtendedAttributes", ExchangeObjectVersion.Exchange2007, typeof(KeyValuePair<string, LocalizedString>[]), PropertyDefinitionFlags.ReturnOnBind, null, null, ExtendedEwsStoreObjectSchema.ExtendedAttributes);

		public static readonly EwsStoreObjectPropertyDefinition LastModified = new EwsStoreObjectPropertyDefinition("LastModified", ExchangeObjectVersion.Exchange2007, typeof(DateTime?), PropertyDefinitionFlags.ReadOnly, null, null, ItemSchema.LastModifiedTime);

		public static readonly EwsStoreObjectPropertyDefinition Message = new EwsStoreObjectPropertyDefinition("Message", ExchangeObjectVersion.Exchange2007, typeof(LocalizedString), PropertyDefinitionFlags.ReturnOnBind, LocalizedString.Empty, LocalizedString.Empty, ExtendedEwsStoreObjectSchema.Message);

		public static readonly EwsStoreObjectPropertyDefinition PercentComplete = new EwsStoreObjectPropertyDefinition("PercentComplete", ExchangeObjectVersion.Exchange2007, typeof(int?), PropertyDefinitionFlags.None, null, null, ExtendedEwsStoreObjectSchema.PercentComplete);

		public static readonly EwsStoreObjectPropertyDefinition StartedByValue = new EwsStoreObjectPropertyDefinition("StartedByValue", ExchangeObjectVersion.Exchange2007, typeof(ADRecipientOrAddress), PropertyDefinitionFlags.None, null, null, EmailMessageSchema.From, delegate(Item item, object value)
		{
			((EmailMessage)item).From = (EmailAddress)value;
		});

		public static readonly SimplePropertyDefinition StartedBy = new SimplePropertyDefinition("StartedBy", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.ReadOnly | PropertyDefinitionFlags.Calculated, string.Empty, string.Empty, Array<PropertyDefinitionConstraint>.Empty, Array<PropertyDefinitionConstraint>.Empty, new ProviderPropertyDefinition[]
		{
			AsyncOperationNotificationSchema.StartedByValue
		}, null, new GetterDelegate(AsyncOperationNotification.GetStartedBy), null);

		public static readonly EwsStoreObjectPropertyDefinition StartTime = new EwsStoreObjectPropertyDefinition("StartTime", ExchangeObjectVersion.Exchange2007, typeof(DateTime?), PropertyDefinitionFlags.ReadOnly, null, null, ItemSchema.DateTimeCreated);

		public static readonly EwsStoreObjectPropertyDefinition Status = new EwsStoreObjectPropertyDefinition("Status", ExchangeObjectVersion.Exchange2007, typeof(AsyncOperationStatus), PropertyDefinitionFlags.PersistDefaultValue, AsyncOperationStatus.Queued, AsyncOperationStatus.Queued, ExtendedEwsStoreObjectSchema.Status);

		public static readonly EwsStoreObjectPropertyDefinition DisplayName = new EwsStoreObjectPropertyDefinition("DisplayName", ExchangeObjectVersion.Exchange2007, typeof(LocalizedString), PropertyDefinitionFlags.ReturnOnBind, LocalizedString.Empty, LocalizedString.Empty, ExtendedEwsStoreObjectSchema.DisplayName);

		public static readonly EwsStoreObjectPropertyDefinition NotificationEmails = new EwsStoreObjectPropertyDefinition("NotificationEmails", ExchangeObjectVersion.Exchange2007, typeof(ADRecipientOrAddress), PropertyDefinitionFlags.MultiValued, null, null, EmailMessageSchema.ToRecipients, new Action<Item, object>(AsyncOperationNotification.SetNotificationEmails));

		public static readonly EwsStoreObjectPropertyDefinition IsNotificationEmailFromTaskSent = new EwsStoreObjectPropertyDefinition("IsNotificationEmailFromTaskSent", ExchangeObjectVersion.Exchange2007, typeof(bool), PropertyDefinitionFlags.None, false, false, ExtendedEwsStoreObjectSchema.IsNotificationEmailFromTaskSent);

		public static readonly SimplePropertyDefinition Type = new SimplePropertyDefinition("Type", ExchangeObjectVersion.Exchange2007, typeof(AsyncOperationType), PropertyDefinitionFlags.Calculated | PropertyDefinitionFlags.Mandatory, AsyncOperationType.Unknown, AsyncOperationType.Unknown, Array<PropertyDefinitionConstraint>.Empty, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(AsyncOperationType))
		}, new ProviderPropertyDefinition[]
		{
			EwsStoreObjectSchema.ItemClass
		}, null, new GetterDelegate(AsyncOperationNotification.GetAsyncOperationType), new SetterDelegate(AsyncOperationNotification.SetAsyncOperationType));
	}
}
