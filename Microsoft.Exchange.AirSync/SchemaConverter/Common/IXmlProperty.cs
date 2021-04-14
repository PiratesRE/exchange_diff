using System;
using System.Xml;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal interface IXmlProperty : IProperty
	{
		XmlNode XmlData { get; }
	}
}
