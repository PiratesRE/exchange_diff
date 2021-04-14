using System;
using System.Globalization;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.SchemaConverter;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	internal abstract class XsoProperty : PropertyBase
	{
		public XsoProperty(StorePropertyDefinition propertyDef)
		{
			this.PropertyDef = propertyDef;
			if (this.PropertyDef != null)
			{
				this.prefetchPropertyDef = new PropertyDefinition[1];
				this.prefetchPropertyDef[0] = this.PropertyDef;
			}
		}

		public XsoProperty(StorePropertyDefinition propertyDef, PropertyType type)
		{
			this.PropertyDef = propertyDef;
			this.type = type;
			if (this.PropertyDef != null)
			{
				this.prefetchPropertyDef = new PropertyDefinition[1];
				this.prefetchPropertyDef[0] = this.PropertyDef;
			}
		}

		public XsoProperty(StorePropertyDefinition propertyDef, PropertyDefinition[] prefetchPropDef)
		{
			this.PropertyDef = propertyDef;
			this.prefetchPropertyDef = prefetchPropDef;
		}

		public XsoProperty(StorePropertyDefinition propertyDef, PropertyType type, PropertyDefinition[] prefetchPropDef)
		{
			this.PropertyDef = propertyDef;
			this.type = type;
			this.prefetchPropertyDef = prefetchPropDef;
		}

		public StorePropertyDefinition StorePropertyDefinition
		{
			get
			{
				return this.PropertyDef;
			}
		}

		public PropertyType Type
		{
			get
			{
				if (this.type != PropertyType.ReadOnlyForNonDraft)
				{
					return this.type;
				}
				bool flag = false;
				if (this.item != null)
				{
					object obj;
					if ((obj = this.item.TryGetProperty(MessageItemSchema.IsDraft)) is PropertyError)
					{
						AirSyncDiagnostics.TraceError<PropertyErrorCode>(ExTraceGlobals.XsoTracer, this, "Error retrieving IsDraft property for item. ErrorCode:{0}", ((PropertyError)obj).PropertyErrorCode);
					}
					else
					{
						flag = (bool)obj;
					}
				}
				if (!flag)
				{
					return PropertyType.ReadOnly;
				}
				return PropertyType.ReadWrite;
			}
			set
			{
				this.type = value;
			}
		}

		public string[] SupportedItemClasses
		{
			get
			{
				return this.supportedItemClasses;
			}
			set
			{
				this.supportedItemClasses = value;
			}
		}

		protected StorePropertyDefinition PropertyDef
		{
			get
			{
				return this.propertyDef;
			}
			set
			{
				this.propertyDef = value;
			}
		}

		protected StoreObject XsoItem
		{
			get
			{
				return this.item;
			}
			set
			{
				this.item = value;
			}
		}

		public virtual void Bind(StoreObject item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			this.XsoItem = item;
		}

		public void ComputePropertyState()
		{
			if (this.supportedItemClasses != null)
			{
				bool flag = false;
				foreach (string value in this.supportedItemClasses)
				{
					if (this.item.ClassName.StartsWith(value, StringComparison.OrdinalIgnoreCase))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					base.State = PropertyState.NotSupported;
					return;
				}
			}
			base.State = PropertyState.Modified;
			if (this.PropertyDef != null)
			{
				PropertyError propertyError = this.XsoItem.TryGetProperty(this.PropertyDef) as PropertyError;
				CalendarItemOccurrence calendarItemOccurrence;
				if (propertyError != null)
				{
					if (propertyError.PropertyErrorCode == PropertyErrorCode.NotFound || propertyError.PropertyErrorCode == PropertyErrorCode.GetCalculatedPropertyError)
					{
						base.State = PropertyState.SetToDefault;
						return;
					}
					if (propertyError.PropertyErrorCode != PropertyErrorCode.NotEnoughMemory)
					{
						throw new ConversionException(string.Format(CultureInfo.InvariantCulture, "Property = {0} could not be fetched, Property Error code = {1}, Description = {2}", new object[]
						{
							this.PropertyDef,
							propertyError.PropertyErrorCode,
							propertyError.PropertyErrorDescription
						}));
					}
					if (this is XsoStringProperty)
					{
						base.State = PropertyState.Stream;
						return;
					}
					base.State = PropertyState.SetToDefault;
					return;
				}
				else if ((calendarItemOccurrence = (this.XsoItem as CalendarItemOccurrence)) != null)
				{
					if (calendarItemOccurrence.IsModifiedProperty(this.PropertyDef))
					{
						return;
					}
					foreach (PropertyDefinition propertyDefinition in this.prefetchPropertyDef)
					{
						if (calendarItemOccurrence.IsModifiedProperty(propertyDefinition))
						{
							return;
						}
					}
					base.State = PropertyState.Unmodified;
				}
			}
		}

		public override void CopyFrom(IProperty srcProperty)
		{
			if (srcProperty == null)
			{
				throw new ArgumentNullException("srcProperty");
			}
			if (srcProperty.SchemaLinkId != base.SchemaLinkId)
			{
				throw new ConversionException("Schema link id's must match in CopyFrom()");
			}
			if (this.XsoItem == null)
			{
				throw new ConversionException("Cannot copy property into null XSO Item");
			}
			switch (srcProperty.State)
			{
			case PropertyState.Uninitialized:
				throw new ConversionException("Can't CopyFrom uninitialized property");
			case PropertyState.SetToDefault:
				if (this.type == PropertyType.ReadAndRequiredForWrite)
				{
					throw new ConversionException(4, string.Format(CultureInfo.InvariantCulture, "Property {0} is required to be present in client side changes", new object[]
					{
						this
					}));
				}
				this.InternalSetToDefault(srcProperty);
				break;
			case PropertyState.Modified:
				this.InternalCopyFromModified(srcProperty);
				return;
			case PropertyState.Unmodified:
				return;
			case PropertyState.Stream:
				break;
			default:
				return;
			}
		}

		public PropertyDefinition[] GetPrefetchProperties()
		{
			return this.prefetchPropertyDef;
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"( Base: ",
				base.ToString(),
				", propertyDefinition: ",
				this.PropertyDef,
				", type: ",
				this.type,
				", item: ",
				this.XsoItem,
				", state: ",
				base.State,
				")"
			});
		}

		public override void Unbind()
		{
			try
			{
				base.State = PropertyState.Uninitialized;
				this.XsoItem = null;
			}
			finally
			{
				base.Unbind();
			}
		}

		protected virtual void InternalCopyFromModified(IProperty srcProperty)
		{
			if (this.PropertyDef == null)
			{
				throw new ConversionException("this.propertyDef is null");
			}
		}

		protected virtual bool IsItemDelegated()
		{
			if (this.XsoItem is MeetingRequest || this.XsoItem is MeetingCancellation)
			{
				MeetingMessage meetingMessage = this.XsoItem as MeetingMessage;
				if (meetingMessage != null && meetingMessage.IsDelegated())
				{
					return true;
				}
			}
			return false;
		}

		protected virtual void InternalSetToDefault(IProperty srcProperty)
		{
			if (this.PropertyDef is NativeStorePropertyDefinition)
			{
				try
				{
					this.XsoItem.DeleteProperties(new PropertyDefinition[]
					{
						this.PropertyDef
					});
				}
				catch (NotSupportedException ex)
				{
					AirSyncDiagnostics.TraceError(ExTraceGlobals.XsoTracer, this, "Error trying to delete properties in XsoProperty.InternalSetToDefault\r\nType of XsoProperty: {0}\r\nType of XsoItem: {1}\r\nClass of XsoItem: {2}\r\nPropertyDef: {3}\r\nError: {4}", new object[]
					{
						base.GetType(),
						this.XsoItem.GetType(),
						this.XsoItem.ClassName,
						this.PropertyDef,
						ex
					});
					throw;
				}
			}
		}

		private StoreObject item;

		private PropertyDefinition[] prefetchPropertyDef;

		private StorePropertyDefinition propertyDef;

		private PropertyType type = PropertyType.ReadWrite;

		private string[] supportedItemClasses;
	}
}
