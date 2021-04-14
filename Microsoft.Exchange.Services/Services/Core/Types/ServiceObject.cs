using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.DataConverter;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType]
	[DataContract]
	public abstract class ServiceObject
	{
		[OnDeserializing]
		private void Init(StreamingContext context)
		{
			this.Init();
		}

		public ServiceObject()
		{
			this.Init();
		}

		internal ServiceObject(PropertyBag propertyBag)
		{
			this.propertyBag = propertyBag;
		}

		private void Init()
		{
			this.propertyBag = new PropertyBag();
		}

		[IgnoreDataMember]
		[XmlIgnore]
		internal PropertyBag PropertyBag
		{
			get
			{
				return this.propertyBag;
			}
			set
			{
				this.propertyBag = value;
			}
		}

		internal List<PropertyInformation> LoadedProperties
		{
			get
			{
				return this.PropertyBag.LoadedProperties;
			}
		}

		internal virtual object this[PropertyInformation property]
		{
			get
			{
				return this.PropertyBag[property];
			}
			set
			{
				this.PropertyBag[property] = value;
			}
		}

		internal bool IsSet(PropertyInformation propertyInfo)
		{
			return this.PropertyBag.Contains(propertyInfo);
		}

		internal void Clear()
		{
			this.PropertyBag.Clear();
		}

		internal bool Remove(PropertyInformation propertyInfo)
		{
			return this.PropertyBag.Remove(propertyInfo);
		}

		internal T GetValueOrDefault<T>(PropertyInformation propertyInfo)
		{
			return this.PropertyBag.GetValueOrDefault<T>(propertyInfo);
		}

		internal abstract StoreObjectType StoreObjectType { get; }

		internal abstract void AddExtendedPropertyValue(ExtendedPropertyType extendedProperty);

		private PropertyBag propertyBag;
	}
}
