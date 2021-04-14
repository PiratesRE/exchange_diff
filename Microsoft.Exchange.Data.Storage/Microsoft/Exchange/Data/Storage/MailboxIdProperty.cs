using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class MailboxIdProperty : IdProperty
	{
		public MailboxIdProperty() : base("MailboxId", typeof(VersionedId), PropertyFlags.ReadOnly, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.EntryId, PropertyDependencyType.NeedForRead)
		})
		{
		}

		public override StorePropertyCapabilities Capabilities
		{
			get
			{
				return StorePropertyCapabilities.None;
			}
		}

		protected override byte[] GetChangeKey(PropertyBag.BasicPropertyStore propertyBag)
		{
			return Array<byte>.Empty;
		}

		protected override StoreObjectType GetStoreObjectType(PropertyBag.BasicPropertyStore propertyBag)
		{
			return StoreObjectType.Mailbox;
		}

		protected override bool IsCompatibleId(StoreId id, ICoreObject coreObject)
		{
			return coreObject is CoreMailboxObject;
		}

		internal override void RegisterFilterTranslation()
		{
		}
	}
}
