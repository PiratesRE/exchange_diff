using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ContactInfoForLinkingFromCoreObject : ContactInfoForLinking
	{
		public static ContactInfoForLinkingFromCoreObject Create(ICoreItem coreObject)
		{
			Util.ThrowOnNullArgument(coreObject, "coreObject");
			coreObject.PropertyBag.Load(ContactInfoForLinking.Properties);
			PropertyBagAdaptor propertyBagAdaptor = PropertyBagAdaptor.Create(coreObject);
			return new ContactInfoForLinkingFromCoreObject(propertyBagAdaptor, coreObject);
		}

		private ContactInfoForLinkingFromCoreObject(PropertyBagAdaptor propertyBagAdaptor, ICoreItem coreObject) : base(propertyBagAdaptor)
		{
			this.coreObject = coreObject;
		}

		protected override void UpdateContact(IExtensibleLogger logger, IContactLinkingPerformanceTracker performanceTracker)
		{
			base.SetLinkingProperties(PropertyBagAdaptor.Create(this.coreObject));
			ContactInfoForLinking.Tracer.TraceDebug<VersionedId, string>((long)this.GetHashCode(), "ContactInfoForLinkingFromCoreObject.UpdateContact: setting link properties but not saving contact with id = {0}; given-name: {1}", base.ItemId, base.GivenName);
		}

		private readonly ICoreItem coreObject;
	}
}
