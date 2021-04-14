using System;

namespace System.Runtime.Serialization.Formatters.Binary
{
	[Serializable]
	internal enum SoapAttributeType
	{
		None,
		SchemaType,
		Embedded,
		XmlElement = 4,
		XmlAttribute = 8
	}
}
