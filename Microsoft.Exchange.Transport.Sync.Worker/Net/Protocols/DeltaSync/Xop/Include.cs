using System;
using System.ComponentModel;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.Xop
{
	[XmlRoot(ElementName = "Include", Namespace = "http://www.w3.org/2004/08/xop/include", IsNullable = false)]
	[XmlType(TypeName = "Include", Namespace = "http://www.w3.org/2004/08/xop/include")]
	[Serializable]
	public class Include
	{
		[XmlIgnore]
		public string href
		{
			get
			{
				return this.internalhref;
			}
			set
			{
				this.internalhref = value;
			}
		}

		[XmlAttribute(AttributeName = "href", Form = XmlSchemaForm.Unqualified, DataType = "anyURI", Namespace = "http://www.w3.org/2004/08/xop/include")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string internalhref;

		[XmlAnyAttribute]
		public XmlAttribute[] AnyAttr;

		[XmlAnyElement]
		public XmlElement[] Any;
	}
}
