using System;
using System.IO;
using System.Xml;

namespace DataContractSerializerProject
{
	internal class EntityXmlTextWriter : XmlTextWriter
	{
		public EntityXmlTextWriter(StringWriter textWriter) : base(textWriter)
		{
		}

		public override void WriteStartElement(string prefix, string localName, string ns)
		{
			base.WriteStartElement(string.Empty, localName, string.Empty);
		}

		public override void WriteStartAttribute(string prefix, string localName, string ns)
		{
			base.WriteStartAttribute(string.Empty, localName, string.Empty);
		}

		public override void WriteQualifiedName(string localName, string ns)
		{
			base.WriteQualifiedName(localName, null);
		}

		public override string LookupPrefix(string ns)
		{
			return string.Empty;
		}
	}
}
