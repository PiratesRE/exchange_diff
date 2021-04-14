using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ContactInfoForLinkingWithPropertyBagUpdater : ContactInfoForLinkingFromPropertyBag
	{
		private ContactInfoForLinkingWithPropertyBagUpdater(PropertyBagAdaptor propertyBagAdaptor, MailboxSession mailboxSession, IStorePropertyBag propertyBag, ICollection<PropertyDefinition> loadedProperties) : base(propertyBagAdaptor, mailboxSession, propertyBag)
		{
			this.originalPropertyBag = propertyBag;
			this.loadedProperties = loadedProperties;
		}

		public IStorePropertyBag PropertyBag
		{
			get
			{
				return this.writablePropertyBag ?? this.originalPropertyBag;
			}
		}

		private IStorePropertyBag WritablePropertyBag
		{
			get
			{
				if (this.writablePropertyBag == null)
				{
					this.writablePropertyBag = ContactInfoForLinkingWithPropertyBagUpdater.CloneToWritablePropertyBag(this.originalPropertyBag, this.loadedProperties);
				}
				return this.writablePropertyBag;
			}
		}

		public static ContactInfoForLinkingWithPropertyBagUpdater Create(MailboxSession mailboxSession, IStorePropertyBag propertyBag, ICollection<PropertyDefinition> loadedProperties)
		{
			Util.ThrowOnNullArgument(mailboxSession, "mailboxSession");
			Util.ThrowOnNullArgument(propertyBag, "propertyBag");
			Util.ThrowOnNullArgument(loadedProperties, "loadedProperties");
			PropertyBagAdaptor propertyBagAdaptor = PropertyBagAdaptor.Create(propertyBag);
			return new ContactInfoForLinkingWithPropertyBagUpdater(propertyBagAdaptor, mailboxSession, propertyBag, loadedProperties);
		}

		protected override void UpdateContact(IExtensibleLogger logger, IContactLinkingPerformanceTracker performanceTracker)
		{
			base.UpdateContact(logger, performanceTracker);
			base.SetLinkingProperties(PropertyBagAdaptor.Create(this.WritablePropertyBag));
		}

		private static IStorePropertyBag CloneToWritablePropertyBag(IStorePropertyBag propertyBag, ICollection<PropertyDefinition> loadedProperties)
		{
			MemoryPropertyBag memoryPropertyBag = new MemoryPropertyBag();
			memoryPropertyBag.LoadFromStorePropertyBag(propertyBag, loadedProperties);
			return memoryPropertyBag.AsIStorePropertyBag();
		}

		private readonly IStorePropertyBag originalPropertyBag;

		private readonly ICollection<PropertyDefinition> loadedProperties;

		private IStorePropertyBag writablePropertyBag;
	}
}
