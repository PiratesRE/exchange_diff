using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.ItemOperationsRequest
{
	[XmlType(TypeName = "MessageType", Namespace = "ItemOperations:")]
	[Serializable]
	public class MessageType
	{
		[XmlIgnore]
		public Junk Junk
		{
			get
			{
				if (this.internalJunk == null)
				{
					this.internalJunk = new Junk();
				}
				return this.internalJunk;
			}
			set
			{
				this.internalJunk = value;
			}
		}

		[XmlIgnore]
		public Phishing Phishing
		{
			get
			{
				if (this.internalPhishing == null)
				{
					this.internalPhishing = new Phishing();
				}
				return this.internalPhishing;
			}
			set
			{
				this.internalPhishing = value;
			}
		}

		[XmlElement(Type = typeof(Junk), ElementName = "Junk", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "ItemOperations:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public Junk internalJunk;

		[XmlElement(Type = typeof(Phishing), ElementName = "Phishing", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "ItemOperations:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public Phishing internalPhishing;
	}
}
