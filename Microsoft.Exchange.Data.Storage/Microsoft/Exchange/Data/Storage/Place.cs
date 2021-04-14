using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class Place : Contact, IPlace, IContact, IContactBase, IItem, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		internal Place(ICoreItem coreItem) : base(coreItem)
		{
			if (base.IsNew)
			{
				this.Initialize();
			}
		}

		public new static Place Create(StoreSession session, StoreId contactFolderId)
		{
			return ItemBuilder.CreateNewItem<Place>(session, contactFolderId, ItemCreateInfo.PlaceInfo);
		}

		public new static Place Bind(StoreSession session, StoreId storeId, params PropertyDefinition[] propsToReturn)
		{
			return Place.Bind(session, storeId, (ICollection<PropertyDefinition>)propsToReturn);
		}

		public new static Place Bind(StoreSession session, StoreId storeId, ICollection<PropertyDefinition> propsToReturn)
		{
			return ItemBuilder.ItemBind<Place>(session, storeId, PlaceSchema.Instance, propsToReturn);
		}

		public override Schema Schema
		{
			get
			{
				this.CheckDisposed("Schema::get");
				return PlaceSchema.Instance;
			}
		}

		public int RelevanceRank
		{
			get
			{
				this.CheckDisposed("RelevanceRank::get");
				return base.GetValueOrDefault<int>(PlaceSchema.LocationRelevanceRank, 0);
			}
			set
			{
				this.CheckDisposed("RelevanceRank::set");
				this[PlaceSchema.LocationRelevanceRank] = value;
			}
		}

		private void Initialize()
		{
			this[InternalSchema.ItemClass] = "IPM.Contact.Place";
		}
	}
}
