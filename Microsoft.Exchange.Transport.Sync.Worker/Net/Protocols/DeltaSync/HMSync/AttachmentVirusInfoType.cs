using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.HMSync
{
	[XmlType(TypeName = "AttachmentVirusInfoType", Namespace = "HMSYNC:")]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Serializable]
	public class AttachmentVirusInfoType
	{
		[XmlIgnore]
		public int Index
		{
			get
			{
				return this.internalIndex;
			}
			set
			{
				this.internalIndex = value;
				this.internalIndexSpecified = true;
			}
		}

		[XmlIgnore]
		public string ContentId
		{
			get
			{
				return this.internalContentId;
			}
			set
			{
				this.internalContentId = value;
			}
		}

		[XmlIgnore]
		public VirusesFound VirusesFound
		{
			get
			{
				if (this.internalVirusesFound == null)
				{
					this.internalVirusesFound = new VirusesFound();
				}
				return this.internalVirusesFound;
			}
			set
			{
				this.internalVirusesFound = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "Index", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int", Namespace = "HMSYNC:")]
		public int internalIndex;

		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool internalIndexSpecified;

		[XmlElement(ElementName = "ContentId", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string", Namespace = "HMSYNC:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string internalContentId;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(VirusesFound), ElementName = "VirusesFound", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSYNC:")]
		public VirusesFound internalVirusesFound;
	}
}
