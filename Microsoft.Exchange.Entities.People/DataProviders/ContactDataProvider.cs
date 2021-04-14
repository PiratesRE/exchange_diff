using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Entities.DataModel.People;
using Microsoft.Exchange.Entities.DataProviders;
using Microsoft.Exchange.Entities.EntitySets;
using Microsoft.Exchange.Entities.TypeConversion.Translators;

namespace Microsoft.Exchange.Entities.People.DataProviders
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ContactDataProvider : StorageItemDataProvider<IMailboxSession, Contact, IContact>
	{
		internal ContactDataProvider(IStorageEntitySetScope<IMailboxSession> scope, ITracer tracer) : base(scope, null, tracer)
		{
			ArgumentValidator.ThrowIfNull("scope", scope);
			ArgumentValidator.ThrowIfNull("tracer", tracer);
			this.FolderInScope = DefaultFolderType.None;
		}

		public DefaultFolderType FolderInScope { get; set; }

		public PropertyDefinition[] ContactProperties { get; set; }

		protected override IStorageTranslator<IContact, Contact> Translator
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		internal IEnumerable<IStorePropertyBag> GetAllContacts(SortBy[] sortColumns)
		{
			this.ValidateContactProperties();
			this.ValidateFolderInScope();
			return ContactsEnumerator<IStorePropertyBag>.CreateContactsOnlyEnumerator(base.Session, this.FolderInScope, sortColumns, this.ContactProperties, (IStorePropertyBag propertyBag) => propertyBag, base.XsoFactory);
		}

		internal ConflictResolutionResult AddContact(IStorePropertyBag propertyBag)
		{
			ArgumentValidator.ThrowIfNull("propertyBag", propertyBag);
			this.ValidateContactProperties();
			ConflictResolutionResult result;
			using (IContact contact = this.CreateNewStoreObject())
			{
				foreach (PropertyDefinition propertyDefinition in this.ContactProperties)
				{
					object valueOrDefault = propertyBag.GetValueOrDefault<object>(propertyDefinition, null);
					if (valueOrDefault != null)
					{
						contact[propertyDefinition] = valueOrDefault;
					}
				}
				result = contact.Save(SaveMode.ResolveConflicts);
			}
			return result;
		}

		internal ConflictResolutionResult UpdateContact(VersionedId versionedId, ICollection<Tuple<PropertyDefinition, object>> changedPropertiesValue)
		{
			ArgumentValidator.ThrowIfNull("versionedId", versionedId);
			ArgumentValidator.ThrowIfNull("changedPropertiesValue", changedPropertiesValue);
			this.GetChangedProperties(changedPropertiesValue);
			ConflictResolutionResult result;
			using (IContact contact = this.Bind(versionedId))
			{
				foreach (Tuple<PropertyDefinition, object> tuple in changedPropertiesValue)
				{
					contact.SetOrDeleteProperty(tuple.Item1, tuple.Item2);
				}
				result = contact.Save(SaveMode.ResolveConflicts);
			}
			return result;
		}

		protected internal override IContact BindToStoreObject(StoreId id)
		{
			this.ValidateContactProperties();
			return base.XsoFactory.BindToContact(base.Session, id, this.ContactProperties);
		}

		protected override IContact CreateNewStoreObject()
		{
			this.ValidateFolderInScope();
			return base.XsoFactory.CreateContact(base.Session, base.Session.GetDefaultFolderId(this.FolderInScope));
		}

		private PropertyDefinition[] GetChangedProperties(IEnumerable<Tuple<PropertyDefinition, object>> list)
		{
			HashSet<PropertyDefinition> hashSet = new HashSet<PropertyDefinition>();
			foreach (Tuple<PropertyDefinition, object> tuple in list)
			{
				hashSet.Add(tuple.Item1);
			}
			return hashSet.ToArray<PropertyDefinition>();
		}

		private void ValidateContactProperties()
		{
			if (this.ContactProperties == null)
			{
				throw new InvalidOperationException("ContactPropertiesList property is not set.");
			}
		}

		private void ValidateFolderInScope()
		{
			if (this.FolderInScope == DefaultFolderType.None)
			{
				throw new InvalidOperationException("FolderInScope property is not set.");
			}
		}
	}
}
