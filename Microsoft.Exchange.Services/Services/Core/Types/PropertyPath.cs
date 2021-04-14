using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlInclude(typeof(DictionaryPropertyUri))]
	[XmlType(TypeName = "BasePathToElementType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[XmlInclude(typeof(ExtendedPropertyUri))]
	[XmlInclude(typeof(DictionaryPropertyUri))]
	[XmlInclude(typeof(ExceptionPropertyUri))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlInclude(typeof(ExtendedPropertyUri))]
	[XmlInclude(typeof(PropertyUri))]
	[KnownType(typeof(ExceptionPropertyUri))]
	[KnownType(typeof(ExtendedPropertyUri))]
	[KnownType(typeof(DictionaryPropertyUri))]
	[KnownType(typeof(PropertyUri))]
	[KnownType(typeof(ExtendedPropertyUri))]
	[KnownType(typeof(PropertyUri))]
	[XmlInclude(typeof(PropertyUri))]
	[KnownType(typeof(ExceptionPropertyUri))]
	[XmlInclude(typeof(ExceptionPropertyUri))]
	[KnownType(typeof(DictionaryPropertyUri))]
	[Serializable]
	public abstract class PropertyPath
	{
		internal static PropertyPath Parse(XmlElement element)
		{
			if (PropertyUri.IsPropertyUriXml(element))
			{
				return PropertyUri.Parse(element);
			}
			if (DictionaryPropertyUri.IsDictionaryPropertyUriXml(element))
			{
				return DictionaryPropertyUri.Parse(element);
			}
			if (ExtendedPropertyUri.IsExtendedPropertyUriXml(element))
			{
				return ExtendedPropertyUri.Parse(element);
			}
			throw new ArgumentException("[PropertyPath::Parse] Invalid xml element passed to parse: " + element.LocalName);
		}

		internal abstract XmlElement ToXml(XmlElement parentElement);
	}
}
