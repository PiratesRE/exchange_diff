using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class EwsStoreObject : ConfigurableObject
	{
		public EwsStoreObject() : base(new SimplePropertyBag(EwsStoreObjectSchema.Identity, EwsStoreObjectSchema.ObjectState, EwsStoreObjectSchema.ExchangeVersion))
		{
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return EwsStoreObject.schema;
			}
		}

		internal virtual string ItemClass
		{
			get
			{
				return null;
			}
		}

		internal Guid? PolicyTag
		{
			get
			{
				return (Guid?)this[EwsStoreObjectSchema.PolicyTag];
			}
			set
			{
				this[EwsStoreObjectSchema.PolicyTag] = value;
			}
		}

		public string AlternativeId
		{
			get
			{
				return (string)this[EwsStoreObjectSchema.AlternativeId];
			}
			set
			{
				this[EwsStoreObjectSchema.AlternativeId] = value;
			}
		}

		public new EwsStoreObjectId Identity
		{
			get
			{
				return (EwsStoreObjectId)base.Identity;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		internal virtual SearchFilter ItemClassFilter
		{
			get
			{
				if (!string.IsNullOrEmpty(this.ItemClass))
				{
					return new SearchFilter.IsEqualTo(ItemSchema.ItemClass, this.ItemClass);
				}
				return null;
			}
		}

		internal virtual SearchFilter VersioningFilter
		{
			get
			{
				return new SearchFilter.Not(new SearchFilter.SearchFilterCollection(0, new SearchFilter[]
				{
					new SearchFilter.IsGreaterThanOrEqualTo(ExtendedEwsStoreObjectSchema.ExchangeVersion, this.MaximumSupportedExchangeObjectVersion.NextMajorVersion.ToInt64()),
					new SearchFilter.Exists(ExtendedEwsStoreObjectSchema.ExchangeVersion)
				}));
			}
		}

		internal void CopyFromItemObject(Item item, ExchangeVersion ewsVersion)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			base.InstantiationErrors.Clear();
			foreach (PropertyDefinition propertyDefinition in this.ObjectSchema.AllProperties)
			{
				if (propertyDefinition != EwsStoreObjectSchema.ExchangeVersion)
				{
					EwsStoreObjectPropertyDefinition ewsStoreObjectPropertyDefinition = propertyDefinition as EwsStoreObjectPropertyDefinition;
					if (ewsStoreObjectPropertyDefinition != null)
					{
						object obj = null;
						if (ewsStoreObjectPropertyDefinition.GetItemProperty(item, out obj))
						{
							if (ewsStoreObjectPropertyDefinition.StorePropertyDefinition.Version > ewsVersion)
							{
								ExTraceGlobals.StorageTracer.TraceDebug(0L, "Skip loading property '{0}.{1}' because the current EWS version '{2}' is lower than '{3}'.", new object[]
								{
									base.GetType().FullName,
									ewsStoreObjectPropertyDefinition.Name,
									ewsVersion,
									ewsStoreObjectPropertyDefinition.StorePropertyDefinition.Version
								});
							}
							else
							{
								Exception ex = null;
								try
								{
									obj = EwsStoreValueConverter.ConvertValueFromStore(ewsStoreObjectPropertyDefinition, obj);
									this.propertyBag.SetField(ewsStoreObjectPropertyDefinition, obj);
									base.InstantiationErrors.AddRange(ewsStoreObjectPropertyDefinition.ValidateProperty(obj, this.propertyBag, true));
								}
								catch (LocalizedException ex2)
								{
									base.InstantiationErrors.Add(new PropertyValidationError(ex2.LocalizedString, ewsStoreObjectPropertyDefinition, obj));
								}
								catch (InvalidCastException ex3)
								{
									ex = ex3;
								}
								catch (FormatException ex4)
								{
									ex = ex4;
								}
								catch (SerializationException ex5)
								{
									ex = ex5;
								}
								if (ex != null)
								{
									base.InstantiationErrors.Add(new PropertyValidationError(new LocalizedString(ex.Message), ewsStoreObjectPropertyDefinition, obj));
								}
							}
						}
					}
				}
			}
		}

		internal void CopyChangeToItemObject(Item item, ExchangeVersion ewsVersion)
		{
			if (base.ObjectState == ObjectState.Unchanged)
			{
				return;
			}
			if (item.IsNew && !string.IsNullOrEmpty(this.ItemClass))
			{
				EwsStoreObjectSchema.ItemClass.SetItemProperty(item, this.ItemClass);
			}
			foreach (PropertyDefinition propertyDefinition in this.ObjectSchema.AllProperties)
			{
				EwsStoreObjectPropertyDefinition ewsStoreObjectPropertyDefinition = propertyDefinition as EwsStoreObjectPropertyDefinition;
				if (ewsStoreObjectPropertyDefinition != null && !ewsStoreObjectPropertyDefinition.IsReadOnly && !ewsStoreObjectPropertyDefinition.IsCalculated && (base.ObjectState == ObjectState.New || base.IsChanged(ewsStoreObjectPropertyDefinition)))
				{
					if (ewsStoreObjectPropertyDefinition.StorePropertyDefinition.Version > ewsVersion && !ewsStoreObjectPropertyDefinition.IsMandatory)
					{
						ExTraceGlobals.StorageTracer.TraceDebug(0L, "Skip saving property '{0}.{1}' because the current EWS version '{2}' is lower than '{3}'.", new object[]
						{
							base.GetType().FullName,
							ewsStoreObjectPropertyDefinition.Name,
							ewsVersion,
							ewsStoreObjectPropertyDefinition.StorePropertyDefinition.Version
						});
					}
					else
					{
						object obj = this[ewsStoreObjectPropertyDefinition];
						if (obj == ewsStoreObjectPropertyDefinition.DefaultValue && !ewsStoreObjectPropertyDefinition.PersistDefaultValue)
						{
							if (base.ObjectState == ObjectState.New)
							{
								continue;
							}
							obj = null;
						}
						if (obj != null)
						{
							obj = EwsStoreValueConverter.ConvertValueToStore(ewsStoreObjectPropertyDefinition, obj);
						}
						ewsStoreObjectPropertyDefinition.SetItemProperty(item, obj);
					}
				}
			}
		}

		private static ObjectSchema schema = new EwsStoreObjectSchema();
	}
}
