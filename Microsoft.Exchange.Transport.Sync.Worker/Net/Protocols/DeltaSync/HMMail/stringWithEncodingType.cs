using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.HMMail
{
	[XmlType(TypeName = "stringWithEncodingType", Namespace = "HMMAIL:")]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Serializable]
	public class stringWithEncodingType
	{
		[XmlIgnore]
		public string encoding
		{
			get
			{
				return this.internalencoding;
			}
			set
			{
				this.internalencoding = value;
			}
		}

		[XmlIgnore]
		public string Value
		{
			get
			{
				return this.internalValue;
			}
			set
			{
				this.internalValue = value;
			}
		}

		[XmlAttribute(AttributeName = "encoding", Form = XmlSchemaForm.Unqualified, DataType = "string", Namespace = "HMMAIL:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string internalencoding;

		[XmlText(DataType = "string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string internalValue;
	}
}
