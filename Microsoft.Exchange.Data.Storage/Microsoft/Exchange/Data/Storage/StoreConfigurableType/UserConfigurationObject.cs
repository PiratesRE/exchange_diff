using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.Principal;

namespace Microsoft.Exchange.Data.Storage.StoreConfigurableType
{
	[Serializable]
	public abstract class UserConfigurationObject : ConfigurableObject, IMailboxStoreType, IConfigurable
	{
		public UserConfigurationObject() : base(new SimplePropertyBag(UserConfigurationObjectSchema.ExchangePrincipal, UserConfigurationObjectSchema.ObjectState, UserConfigurationObjectSchema.ExchangeVersion))
		{
		}

		internal UserConfigurationObject(IExchangePrincipal principal) : this()
		{
			this.Principal = principal;
		}

		public override ObjectId Identity
		{
			get
			{
				ObjectId objectId = this.identity;
				if (SuppressingPiiContext.NeedPiiSuppression && objectId is ADObjectId)
				{
					objectId = (ObjectId)SuppressingPiiProperty.TryRedact(ADObjectSchema.Id, objectId);
				}
				return objectId;
			}
		}

		internal virtual UserConfigurationObjectSchema Schema
		{
			get
			{
				return null;
			}
		}

		internal sealed override ObjectSchema ObjectSchema
		{
			get
			{
				return this.Schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal IExchangePrincipal Principal
		{
			get
			{
				return this.principal;
			}
			set
			{
				this.principal = value;
				if (value != null)
				{
					this.identity = value.ObjectId;
					return;
				}
				this.identity = null;
			}
		}

		public new object Clone()
		{
			UserConfigurationObject userConfigurationObject = (UserConfigurationObject)base.Clone();
			userConfigurationObject.Principal = this.Principal;
			return userConfigurationObject;
		}

		public abstract void Save(MailboxStoreTypeProvider session);

		public virtual void Delete(MailboxStoreTypeProvider session)
		{
			throw new NotImplementedException();
		}

		public abstract IConfigurable Read(MailboxStoreTypeProvider session, ObjectId identity);

		public IConfigurable[] Find(MailboxStoreTypeProvider session, QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<T> FindPaged<T>(MailboxStoreTypeProvider session, QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize)
		{
			T t = (T)((object)this.Read(session, session.ADUser.Identity));
			List<T> list = new List<T>();
			if (t != null)
			{
				list.Add(t);
			}
			return list;
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			base.ValidateRead(errors);
			foreach (PropertyDefinition propertyDefinition in this.Schema.AllProperties)
			{
				ProviderPropertyDefinition providerPropertyDefinition = propertyDefinition as ProviderPropertyDefinition;
				if (providerPropertyDefinition != null)
				{
					providerPropertyDefinition.ValidateValue(this[providerPropertyDefinition], false);
				}
			}
		}

		[NonSerialized]
		private IExchangePrincipal principal;

		private ObjectId identity;
	}
}
