using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class PersonIdPropertyRule
	{
		public bool UpdateProperties(ICorePropertyBag propertyBag)
		{
			string valueOrDefault = propertyBag.GetValueOrDefault<string>(StoreObjectSchema.ItemClass, null);
			if (string.IsNullOrEmpty(valueOrDefault))
			{
				return false;
			}
			if (!ObjectClass.IsContact(valueOrDefault) && !ObjectClass.IsDistributionList(valueOrDefault))
			{
				return false;
			}
			bool valueOrDefault2 = propertyBag.GetValueOrDefault<bool>(InternalSchema.ConversationIndexTracking, false);
			if (valueOrDefault2)
			{
				return false;
			}
			propertyBag[InternalSchema.ConversationIndexTracking] = true;
			return true;
		}

		private static readonly Trace Tracer = ExTraceGlobals.ContactLinkingTracer;

		public static readonly PropertyReference[] PropertyReferences = new PropertyReference[]
		{
			new PropertyReference(InternalSchema.ItemClass, PropertyAccess.Read),
			new PropertyReference(InternalSchema.ConversationIndexTracking, PropertyAccess.ReadWrite)
		};
	}
}
