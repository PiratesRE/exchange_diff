using System;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public abstract class XsoMailboxConfigurationObject : ConfigurableObject
	{
		public XsoMailboxConfigurationObject() : base(new SimplePropertyBag(XsoMailboxConfigurationObjectSchema.MailboxOwnerId, UserConfigurationObjectSchema.ObjectState, UserConfigurationObjectSchema.ExchangeVersion))
		{
		}

		internal abstract XsoMailboxConfigurationObjectSchema Schema { get; }

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

		public ADObjectId MailboxOwnerId
		{
			get
			{
				return (ADObjectId)this[XsoMailboxConfigurationObjectSchema.MailboxOwnerId];
			}
			internal set
			{
				this[XsoMailboxConfigurationObjectSchema.MailboxOwnerId] = value;
			}
		}

		internal void LoadDataFromXsoRows(ADObjectId mailboxOwnerId, object[] objectRow, PropertyDefinition[] xsoPropertyDefinitions)
		{
			if (mailboxOwnerId == null)
			{
				throw new ArgumentNullException("mailboxOwnerId");
			}
			if (objectRow == null)
			{
				throw new ArgumentNullException("objectRow");
			}
			if (xsoPropertyDefinitions == null)
			{
				throw new ArgumentNullException("xsoPropertyDefinitions");
			}
			if (objectRow.Length != xsoPropertyDefinitions.Length)
			{
				throw new ArgumentException("xsoPropertyDefinitions and objectRow length mismatch");
			}
			base.InstantiationErrors.Clear();
			this.MailboxOwnerId = mailboxOwnerId;
			for (int i = 0; i < xsoPropertyDefinitions.Length; i++)
			{
				XsoDriverPropertyDefinition relatedWrapperProperty = this.Schema.GetRelatedWrapperProperty(xsoPropertyDefinitions[i]);
				try
				{
					object obj = objectRow[i];
					StorePropertyDefinition propertyDefinition = InternalSchema.ToStorePropertyDefinition(xsoPropertyDefinitions[i]);
					if (obj != null)
					{
						this.propertyBag.SetField(relatedWrapperProperty, PropertyBag.CheckPropertyValue<object>(propertyDefinition, obj, null));
					}
				}
				catch (StoragePermanentException ex)
				{
					base.InstantiationErrors.Add(new PropertyValidationError(ex.LocalizedString, relatedWrapperProperty, null));
				}
			}
		}

		internal void LoadDataFromXso(ADObjectId mailboxOwnerId, StoreObject storeObject)
		{
			if (storeObject == null)
			{
				throw new ArgumentNullException("storeObject");
			}
			PropertyDefinition[] allDependentXsoProperties = this.Schema.AllDependentXsoProperties;
			this.LoadDataFromXsoRows(mailboxOwnerId, storeObject.GetProperties(allDependentXsoProperties), allDependentXsoProperties);
		}

		internal void SaveDataToXso(StoreObject storeObject, ReadOnlyCollection<XsoDriverPropertyDefinition> ignoredProperties)
		{
			if (storeObject == null)
			{
				throw new ArgumentNullException("storeObject");
			}
			foreach (PropertyDefinition propertyDefinition in this.Schema.AllProperties)
			{
				if (propertyDefinition is XsoDriverPropertyDefinition)
				{
					XsoDriverPropertyDefinition xsoDriverPropertyDefinition = (XsoDriverPropertyDefinition)propertyDefinition;
					if (!xsoDriverPropertyDefinition.IsReadOnly && (ignoredProperties == null || !ignoredProperties.Contains(xsoDriverPropertyDefinition)) && (PropertyFlags.ReadOnly & xsoDriverPropertyDefinition.StorePropertyDefinition.PropertyFlags) == PropertyFlags.None && base.IsChanged(xsoDriverPropertyDefinition))
					{
						object obj = null;
						this.propertyBag.TryGetField((ProviderPropertyDefinition)propertyDefinition, ref obj);
						if (obj != null)
						{
							storeObject[xsoDriverPropertyDefinition.StorePropertyDefinition] = obj;
						}
						else
						{
							storeObject.Delete(xsoDriverPropertyDefinition.StorePropertyDefinition);
						}
					}
				}
			}
		}
	}
}
