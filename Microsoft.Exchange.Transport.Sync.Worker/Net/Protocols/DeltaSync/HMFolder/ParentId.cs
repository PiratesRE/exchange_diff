using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.HMFolder
{
	[XmlRoot(ElementName = "ParentId", Namespace = "HMFOLDER:", IsNullable = false)]
	[Serializable]
	public class ParentId
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

		[XmlAttribute(AttributeName = "isClientId", Form = XmlSchemaForm.Unqualified, Namespace = "HMFOLDER:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bitType internalisClientId;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlIgnore]
		public bool internalisClientIdSpecified;

		[XmlText(DataType = "string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string internalValue;
	}
}
