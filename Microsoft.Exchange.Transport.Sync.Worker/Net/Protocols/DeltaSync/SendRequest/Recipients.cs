using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SendRequest
{
	[XmlType(TypeName = "Recipients", Namespace = "Send:")]
	[Serializable]
	public class Recipients
	{
		[XmlIgnore]
		public RecipientsType To
		{
			get
			{
				if (this.internalTo == null)
				{
					this.internalTo = new RecipientsType();
				}
				return this.internalTo;
			}
			set
			{
				this.internalTo = value;
			}
		}

		[XmlIgnore]
		public RecipientsType Cc
		{
			get
			{
				if (this.internalCc == null)
				{
					this.internalCc = new RecipientsType();
				}
				return this.internalCc;
			}
			set
			{
				this.internalCc = value;
			}
		}

		[XmlIgnore]
		public RecipientsType Bcc
		{
			get
			{
				if (this.internalBcc == null)
				{
					this.internalBcc = new RecipientsType();
				}
				return this.internalBcc;
			}
			set
			{
				this.internalBcc = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(RecipientsType), ElementName = "To", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "Send:")]
		public RecipientsType internalTo;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(RecipientsType), ElementName = "Cc", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "Send:")]
		public RecipientsType internalCc;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(RecipientsType), ElementName = "Bcc", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "Send:")]
		public RecipientsType internalBcc;
	}
}
