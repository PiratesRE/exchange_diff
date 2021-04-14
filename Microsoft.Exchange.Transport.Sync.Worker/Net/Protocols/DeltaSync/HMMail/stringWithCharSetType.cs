using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.HMMail
{
	[XmlType(TypeName = "stringWithCharSetType", Namespace = "HMMAIL:")]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Serializable]
	public class stringWithCharSetType
	{
		[XmlIgnore]
		public string charset
		{
			get
			{
				return this.internalcharset;
			}
			set
			{
				this.internalcharset = value;
			}
		}

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

		[XmlAttribute(AttributeName = "charset", Form = XmlSchemaForm.Unqualified, DataType = "string", Namespace = "HMMAIL:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string internalcharset;

		[XmlAttribute(AttributeName = "encoding", Form = XmlSchemaForm.Unqualified, DataType = "string", Namespace = "HMMAIL:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string internalencoding;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlText(DataType = "string")]
		public string internalValue;
	}
}
