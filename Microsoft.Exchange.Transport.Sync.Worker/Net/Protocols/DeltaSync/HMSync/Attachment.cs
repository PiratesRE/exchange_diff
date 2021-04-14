using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.HMSync
{
	[XmlType(TypeName = "Attachment", Namespace = "HMSYNC:")]
	[Serializable]
	public class Attachment : AttachmentVirusInfoType
	{
		[XmlIgnore]
		public Contents Contents
		{
			get
			{
				if (this.internalContents == null)
				{
					this.internalContents = new Contents();
				}
				return this.internalContents;
			}
			set
			{
				this.internalContents = value;
			}
		}

		[XmlElement(Type = typeof(Contents), ElementName = "Contents", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSYNC:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public Contents internalContents;
	}
}
