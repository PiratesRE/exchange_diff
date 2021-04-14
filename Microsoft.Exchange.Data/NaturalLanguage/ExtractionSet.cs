using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.NaturalLanguage
{
	public abstract class ExtractionSet
	{
		public abstract XmlSerializer GetSerializer();

		public abstract bool IsEmpty { get; }
	}
}
