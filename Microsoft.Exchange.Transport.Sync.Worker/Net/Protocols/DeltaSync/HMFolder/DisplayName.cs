using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.HMFolder
{
	[XmlRoot(ElementName = "DisplayName", Namespace = "HMFOLDER:", IsNullable = false)]
	[Serializable]
	public class DisplayName
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

		[XmlAttribute(AttributeName = "charset", Form = XmlSchemaForm.Unqualified, DataType = "string", Namespace = "HMFOLDER:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string internalcharset;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlAttribute(AttributeName = "encoding", Form = XmlSchemaForm.Unqualified, DataType = "string", Namespace = "HMFOLDER:")]
		public string internalencoding;

		[XmlText(DataType = "string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string internalValue;
	}
}
