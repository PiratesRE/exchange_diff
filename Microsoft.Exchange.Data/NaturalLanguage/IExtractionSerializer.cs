using System;
using System.Xml;

namespace Microsoft.Exchange.Data.NaturalLanguage
{
	public interface IExtractionSerializer<T>
	{
		T[] ReadXml(XmlReader reader, Version version);

		void WriteXml(XmlWriter writer, T[] t);
	}
}
