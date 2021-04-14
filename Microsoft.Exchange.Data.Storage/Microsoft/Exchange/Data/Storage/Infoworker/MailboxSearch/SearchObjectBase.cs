using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch
{
	[Serializable]
	public abstract class SearchObjectBase : ConfigurableObject
	{
		internal abstract SearchObjectBaseSchema Schema { get; }

		internal sealed override ObjectSchema ObjectSchema
		{
			get
			{
				return this.Schema;
			}
		}

		internal SearchObjectId Id
		{
			get
			{
				return (SearchObjectId)this.propertyBag[SearchObjectBaseSchema.Id];
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter]
		public string Name
		{
			get
			{
				return (string)this.propertyBag[SearchObjectBaseSchema.Name];
			}
			set
			{
				this.propertyBag[SearchObjectBaseSchema.Name] = value;
			}
		}

		internal abstract ObjectType ObjectType { get; }

		internal void SetId(SearchObjectId identity)
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			if (identity.ObjectType != this.ObjectType)
			{
				throw new ArgumentException("identity.ObjectType " + identity.ObjectType.ToString());
			}
			this[SearchObjectBaseSchema.Id] = identity;
		}

		internal void SetId(ADObjectId mailboxOwner, Guid guid)
		{
			this.SetId(new SearchObjectId(mailboxOwner, this.ObjectType, guid));
		}

		internal void SetId(ADObjectId mailboxOwner)
		{
			this.SetId(new SearchObjectId(mailboxOwner, this.ObjectType, Guid.Empty));
		}

		internal virtual void OnSaving()
		{
		}

		internal SearchObjectBase(SearchObjectPropertyBag propertyBag) : base(propertyBag)
		{
			this.propertyBag.SetField(SearchObjectBaseSchema.ExchangeVersion, this.MaximumSupportedExchangeObjectVersion);
			this.propertyBag.ResetChangeTracking();
		}

		public SearchObjectBase() : this(new SearchObjectPropertyBag())
		{
		}
	}
}
