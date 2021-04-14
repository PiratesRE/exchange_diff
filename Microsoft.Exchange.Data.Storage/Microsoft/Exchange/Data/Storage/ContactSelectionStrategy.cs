using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class ContactSelectionStrategy : SelectionStrategy
	{
		public override StorePropertyDefinition[] RequiredProperties()
		{
			return ContactSelectionStrategy.DefaultRequiredProperties;
		}

		public static SelectionStrategy CreatePersonNameProperty(StorePropertyDefinition propertyDefinition)
		{
			Util.ThrowOnNullArgument(propertyDefinition, "propertyDefinition");
			return new ContactSelectionStrategy.PersonNamePropertySelection(propertyDefinition);
		}

		public override bool HasPriority(IStorePropertyBag contact1, IStorePropertyBag contact2)
		{
			return ContactSelectionStrategy.HasDefaultPriority(contact1, contact2);
		}

		public static bool HasDefaultPriority(IStorePropertyBag contact1, IStorePropertyBag contact2)
		{
			Util.ThrowOnNullArgument(contact1, "contact1");
			Util.ThrowOnNullArgument(contact2, "contact2");
			string valueOrDefault = contact1.GetValueOrDefault<string>(InternalSchema.PartnerNetworkId, string.Empty);
			string valueOrDefault2 = contact2.GetValueOrDefault<string>(InternalSchema.PartnerNetworkId, string.Empty);
			int num = ContactSelectionStrategy.NumericalRankingFromPartnerNetworkId(valueOrDefault);
			int num2 = ContactSelectionStrategy.NumericalRankingFromPartnerNetworkId(valueOrDefault2);
			if (num < num2)
			{
				return true;
			}
			if (num == num2)
			{
				ExDateTime valueOrDefault3 = contact1.GetValueOrDefault<ExDateTime>(InternalSchema.CreationTime, ExDateTime.MinValue);
				ExDateTime valueOrDefault4 = contact2.GetValueOrDefault<ExDateTime>(InternalSchema.CreationTime, ExDateTime.MinValue);
				return valueOrDefault3 > valueOrDefault4;
			}
			return false;
		}

		private static int NumericalRankingFromPartnerNetworkId(string partnerNetworkId)
		{
			if (string.IsNullOrWhiteSpace(partnerNetworkId))
			{
				return 0;
			}
			if (string.Equals(partnerNetworkId, WellKnownNetworkNames.GAL))
			{
				return 1;
			}
			if (string.Equals(partnerNetworkId, WellKnownNetworkNames.LinkedIn))
			{
				return 4;
			}
			if (string.Equals(partnerNetworkId, WellKnownNetworkNames.Facebook))
			{
				return 5;
			}
			return 9;
		}

		public static SelectionStrategy CreateSingleSourceProperty(StorePropertyDefinition propertyDefinition)
		{
			Util.ThrowOnNullArgument(propertyDefinition, "propertyDefinition");
			return new ContactSelectionStrategy.ContactSingleSourcePropertySelection(propertyDefinition);
		}

		protected ContactSelectionStrategy() : base(new StorePropertyDefinition[0])
		{
		}

		internal static readonly StorePropertyDefinition[] DefaultRequiredProperties = new StorePropertyDefinition[]
		{
			InternalSchema.CreationTime,
			InternalSchema.PartnerNetworkId
		};

		public static readonly ContactSelectionStrategy PhotoContactIdProperty = new ContactSelectionStrategy.PhotoContactIdSelection();

		public static readonly SelectionStrategy FileAsIdProperty = new ContactSelectionStrategy.FileAsIdPropertySelection();

		private sealed class PhotoContactIdSelection : ContactSelectionStrategy
		{
			public override StorePropertyDefinition[] RequiredProperties()
			{
				return PropertyDefinitionCollection.Merge<StorePropertyDefinition>(base.RequiredProperties(), ContactSelectionStrategy.PhotoContactIdSelection.RequiredPropertiesArray);
			}

			public override bool IsSelectable(IStorePropertyBag source)
			{
				Util.ThrowOnNullArgument(source, "source");
				return source.GetValueOrDefault<bool>(InternalSchema.HasPicture, false);
			}

			public override object GetValue(IStorePropertyBag contact)
			{
				Util.ThrowOnNullArgument(contact, "contact");
				byte[] valueOrDefault = contact.GetValueOrDefault<byte[]>(InternalSchema.EntryId, null);
				if (valueOrDefault == null)
				{
					return null;
				}
				return StoreObjectId.FromProviderSpecificId(valueOrDefault, StoreObjectType.Unknown);
			}

			private static readonly StorePropertyDefinition[] RequiredPropertiesArray = new StorePropertyDefinition[]
			{
				InternalSchema.EntryId,
				InternalSchema.HasPicture
			};
		}

		private abstract class PersonDisplayNameBasedPropertySelection : SelectionStrategy.SingleSourcePropertySelection
		{
			public override StorePropertyDefinition[] RequiredProperties()
			{
				return ContactSelectionStrategy.PersonDisplayNameBasedPropertySelection.RequiredPropertiesArray;
			}

			public PersonDisplayNameBasedPropertySelection(StorePropertyDefinition sourceProperty) : base(sourceProperty)
			{
			}

			public override bool IsSelectable(IStorePropertyBag source)
			{
				Util.ThrowOnNullArgument(source, "source");
				string valueOrDefault = source.GetValueOrDefault<string>(ContactBaseSchema.DisplayNameFirstLast, null);
				return !string.IsNullOrWhiteSpace(valueOrDefault);
			}

			public override object GetValue(IStorePropertyBag contact)
			{
				return contact.GetValueOrDefault<object>(base.SourceProperty, null);
			}

			public override bool HasPriority(IStorePropertyBag contact1, IStorePropertyBag contact2)
			{
				Util.ThrowOnNullArgument(contact1, "contact1");
				Util.ThrowOnNullArgument(contact2, "contact2");
				int valueOrDefault = contact1.GetValueOrDefault<int>(ContactBaseSchema.DisplayNamePriority, int.MaxValue);
				int valueOrDefault2 = contact2.GetValueOrDefault<int>(ContactBaseSchema.DisplayNamePriority, int.MaxValue);
				return valueOrDefault < valueOrDefault2 || (valueOrDefault <= valueOrDefault2 && ContactSelectionStrategy.HasDefaultPriority(contact1, contact2));
			}

			private static readonly StorePropertyDefinition[] RequiredPropertiesArray = new StorePropertyDefinition[]
			{
				InternalSchema.CreationTime,
				InternalSchema.DisplayNameFirstLast,
				InternalSchema.DisplayNamePriority
			};
		}

		private sealed class PersonNamePropertySelection : ContactSelectionStrategy.PersonDisplayNameBasedPropertySelection
		{
			public PersonNamePropertySelection(StorePropertyDefinition sourceProperty) : base(sourceProperty)
			{
			}
		}

		private sealed class FileAsIdPropertySelection : ContactSelectionStrategy.PersonDisplayNameBasedPropertySelection
		{
			public FileAsIdPropertySelection() : base(InternalSchema.FileAsId)
			{
			}

			public override object GetValue(IStorePropertyBag contact)
			{
				Util.ThrowOnNullArgument(contact, "contact");
				return contact.GetValueOrDefault<FileAsMapping>(InternalSchema.FileAsId, FileAsMapping.None).ToString();
			}
		}

		private class ContactSingleSourcePropertySelection : SelectionStrategy.SingleSourcePropertySelection
		{
			public ContactSingleSourcePropertySelection(StorePropertyDefinition sourceProperty) : base(sourceProperty)
			{
			}

			protected ContactSingleSourcePropertySelection(StorePropertyDefinition sourceProperty, params StorePropertyDefinition[] additionalDependencies) : base(sourceProperty, additionalDependencies)
			{
			}

			public override StorePropertyDefinition[] RequiredProperties()
			{
				return ContactSelectionStrategy.DefaultRequiredProperties;
			}

			public override bool HasPriority(IStorePropertyBag contact1, IStorePropertyBag contact2)
			{
				return ContactSelectionStrategy.HasDefaultPriority(contact1, contact2);
			}
		}
	}
}
