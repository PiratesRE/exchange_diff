using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class EwsStoreObjectPropertyDefinition : SimplePropertyDefinition
	{
		public EwsStoreObjectPropertyDefinition(string name, ExchangeObjectVersion versionAdded, Type type, PropertyDefinitionFlags flags, object defaultValue, object initialValue, PropertyDefinitionBase storePropertyDefinition) : this(name, versionAdded, type, flags, defaultValue, initialValue, storePropertyDefinition, null)
		{
		}

		public EwsStoreObjectPropertyDefinition(string name, ExchangeObjectVersion versionAdded, Type type, PropertyDefinitionFlags flags, object defaultValue, object initialValue, PropertyDefinitionBase storePropertyDefinition, Action<Item, object> itemPropertySetter) : base(name, versionAdded ?? EwsHelper.EwsVersionToExchangeObjectVersion(storePropertyDefinition.Version), type, flags, defaultValue, initialValue)
		{
			if (storePropertyDefinition == null)
			{
				throw new ArgumentNullException("storePropertyDefinition");
			}
			if (storePropertyDefinition is PropertyDefinition)
			{
				if (itemPropertySetter == null && !this.IsReadOnly && storePropertyDefinition != ItemSchema.Attachments)
				{
					throw new ArgumentException("ItemPropertySetter must be provided for writable first class property.");
				}
			}
			else if (storePropertyDefinition is ExtendedPropertyDefinition && itemPropertySetter != null)
			{
				throw new ArgumentException("ItemPropertySetter shouldn't be provided for extended property.");
			}
			this.storePropertyDefinition = storePropertyDefinition;
			this.ItemPropertySetter = itemPropertySetter;
		}

		public PropertyDefinitionBase StorePropertyDefinition
		{
			get
			{
				return this.storePropertyDefinition;
			}
		}

		public Action<Item, object> ItemPropertySetter { get; private set; }

		public bool ReturnOnBind
		{
			get
			{
				return (base.PropertyDefinitionFlags & PropertyDefinitionFlags.ReturnOnBind) != PropertyDefinitionFlags.None;
			}
		}

		public bool GetItemProperty(Item item, out object result)
		{
			if (this.storePropertyDefinition == ItemSchema.Attachments)
			{
				return this.TryGetAttachmentItemProperty(item, out result);
			}
			return item.TryGetProperty(this.storePropertyDefinition, ref result);
		}

		public void SetItemProperty(Item item, object value)
		{
			if (value != null && this.StorePropertyDefinition.Type != value.GetType())
			{
				value = Convert.ChangeType(value, EwsStoreValueConverter.GetStorePropertyDefinitionActualType(this));
				if (value is Array && ((Array)value).Length == 0)
				{
					value = null;
				}
			}
			if (this.ItemPropertySetter != null)
			{
				this.ItemPropertySetter(item, value);
				return;
			}
			if (this.storePropertyDefinition == ItemSchema.Attachments)
			{
				this.SetAttachmentItemProperty(item, value);
				return;
			}
			EwsStoreObjectPropertyDefinition.SetExtendedPropertyItemProperty(item, (ExtendedPropertyDefinition)this.storePropertyDefinition, value);
		}

		private static void SetExtendedPropertyItemProperty(Item item, ExtendedPropertyDefinition storeProperty, object value)
		{
			if (value == null)
			{
				item.RemoveExtendedProperty(storeProperty);
				return;
			}
			item.SetExtendedProperty(storeProperty, value);
		}

		private static FileAttachment GetPropertyAttachment(Item item, string propertyName)
		{
			FileAttachment fileAttachment = null;
			if (item.Attachments != null)
			{
				for (int i = 0; i < item.Attachments.Count; i++)
				{
					if (item.Attachments[i].Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
					{
						fileAttachment = (item.Attachments[i] as FileAttachment);
						if (fileAttachment != null)
						{
							break;
						}
					}
				}
			}
			return fileAttachment;
		}

		private bool TryGetAttachmentItemProperty(Item item, out object result)
		{
			FileAttachment propertyAttachment = EwsStoreObjectPropertyDefinition.GetPropertyAttachment(item, base.Name);
			result = null;
			if (propertyAttachment != null)
			{
				int num = 2;
				Exception ex;
				do
				{
					ex = null;
					try
					{
						propertyAttachment.Load();
						goto IL_64;
					}
					catch (ServiceRemoteException ex2)
					{
						if (ex2 is ServiceResponseException && ((ServiceResponseException)ex2).ErrorCode == 131)
						{
							goto IL_64;
						}
						ex = ex2;
					}
					catch (ServiceLocalException ex3)
					{
						ex = ex3;
					}
				}
				while (num-- > 0);
				throw new DataSourceOperationException(new LocalizedString(ex.Message), ex);
				IL_64:
				if (base.Type != typeof(byte[]))
				{
					if (propertyAttachment.Content != null && propertyAttachment.Content.Length > 0)
					{
						result = EwsStoreValueConverter.DeserializeFromBinary(propertyAttachment.Content);
					}
				}
				else
				{
					result = propertyAttachment.Content;
				}
				return true;
			}
			return false;
		}

		private void SetAttachmentItemProperty(Item item, object value)
		{
			FileAttachment propertyAttachment = EwsStoreObjectPropertyDefinition.GetPropertyAttachment(item, base.Name);
			if (value == null)
			{
				if (propertyAttachment != null)
				{
					item.Attachments.Remove(propertyAttachment);
					return;
				}
			}
			else
			{
				byte[] array = value as byte[];
				if (array == null)
				{
					array = EwsStoreValueConverter.SerializeToBinary(value);
				}
				if (propertyAttachment != null)
				{
					item.Attachments.Remove(propertyAttachment);
				}
				item.Attachments.AddFileAttachment(base.Name, array);
			}
		}

		[NonSerialized]
		private PropertyDefinitionBase storePropertyDefinition;
	}
}
