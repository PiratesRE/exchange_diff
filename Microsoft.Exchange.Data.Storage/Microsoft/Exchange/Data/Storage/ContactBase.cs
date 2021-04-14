using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class ContactBase : Item, IContactBase, IItem, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		internal ContactBase(ICoreItem coreItem) : base(coreItem, false)
		{
		}

		public string DisplayName
		{
			get
			{
				this.CheckDisposed("DisplayName::get");
				return base.GetValueOrDefault<string>(StoreObjectSchema.DisplayName, string.Empty);
			}
			set
			{
				this.CheckDisposed("DisplayName::set");
				this[StoreObjectSchema.DisplayName] = value;
			}
		}
	}
}
