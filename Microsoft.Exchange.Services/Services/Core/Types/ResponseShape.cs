using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[KnownType(typeof(ItemResponseShape))]
	[XmlInclude(typeof(FolderResponseShape))]
	[XmlInclude(typeof(ItemResponseShape))]
	[KnownType(typeof(FolderResponseShape))]
	[Serializable]
	public class ResponseShape
	{
		public ResponseShape()
		{
		}

		internal ResponseShape(ShapeEnum baseShape, PropertyPath[] additionalProperties)
		{
			this.BaseShape = baseShape;
			this.AdditionalProperties = additionalProperties;
		}

		[IgnoreDataMember]
		[XmlElement]
		public ShapeEnum BaseShape { get; set; }

		[DataMember(Name = "BaseShape", IsRequired = true)]
		[XmlIgnore]
		public string BaseShapeString
		{
			get
			{
				return EnumUtilities.ToString<ShapeEnum>(this.BaseShape);
			}
			set
			{
				this.BaseShape = EnumUtilities.Parse<ShapeEnum>(value);
			}
		}

		[XmlArrayItem("IndexedFieldURI", typeof(DictionaryPropertyUri), IsNullable = false)]
		[DataMember(IsRequired = false)]
		[XmlArrayItem("ExtendedFieldURI", typeof(ExtendedPropertyUri), IsNullable = false)]
		[XmlArrayItem("FieldURI", typeof(PropertyUri), IsNullable = false)]
		public PropertyPath[] AdditionalProperties { get; set; }

		[IgnoreDataMember]
		[XmlIgnore]
		public Dictionary<string, PropertyPath[]> FlightedProperties { get; set; }
	}
}
