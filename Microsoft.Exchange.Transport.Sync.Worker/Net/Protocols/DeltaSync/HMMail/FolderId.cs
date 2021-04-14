using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.HMMail
{
	[XmlRoot(ElementName = "FolderId", Namespace = "HMMAIL:", IsNullable = false)]
	[Serializable]
	public class FolderId
	{
		[XmlIgnore]
		public bitType isClientId
		{
			get
			{
				return this.internalisClientId;
			}
			set
			{
				this.internalisClientId = value;
				this.internalisClientIdSpecified = true;
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

		[XmlAttribute(AttributeName = "isClientId", Form = XmlSchemaForm.Unqualified, Namespace = "HMMAIL:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bitType internalisClientId;

		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool internalisClientIdSpecified;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlText(DataType = "string")]
		public string internalValue;
	}
}
