using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class ExtendedPropertyType
	{
		public ExtendedPropertyType()
		{
		}

		public ExtendedPropertyType(ExtendedPropertyUri propertyUri, string value)
		{
			this.ExtendedFieldURI = propertyUri;
			this.singleValue = value;
		}

		public ExtendedPropertyType(ExtendedPropertyUri propertyUri, string[] values)
		{
			this.ExtendedFieldURI = propertyUri;
			this.arrayValue = new NonEmptyArrayOfPropertyValues(values);
		}

		[DataMember(EmitDefaultValue = false)]
		public ExtendedPropertyUri ExtendedFieldURI { get; set; }

		[IgnoreDataMember]
		[XmlElement("Value", typeof(string))]
		[XmlElement("Values", typeof(NonEmptyArrayOfPropertyValues))]
		public object Item
		{
			get
			{
				if (this.singleValue != null)
				{
					return this.singleValue;
				}
				return this.arrayValue;
			}
			set
			{
				if (value is NonEmptyArrayOfPropertyValues)
				{
					this.arrayValue = (NonEmptyArrayOfPropertyValues)value;
					return;
				}
				this.singleValue = value;
			}
		}

		[XmlIgnore]
		[DataMember(Name = "Value", EmitDefaultValue = false)]
		public object Value
		{
			get
			{
				return this.singleValue;
			}
			set
			{
				this.singleValue = value;
				this.arrayValue = null;
			}
		}

		[XmlIgnore]
		[DataMember(Name = "Values", EmitDefaultValue = false)]
		public string[] Values
		{
			get
			{
				if (this.arrayValue == null)
				{
					return null;
				}
				return this.arrayValue.Values;
			}
			set
			{
				this.arrayValue = new NonEmptyArrayOfPropertyValues(value);
				this.singleValue = null;
			}
		}

		private object singleValue;

		private NonEmptyArrayOfPropertyValues arrayValue;
	}
}
