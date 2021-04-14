using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SendRequest
{
	[XmlType(TypeName = "SendItem", Namespace = "Send:")]
	[Serializable]
	public class SendItem
	{
		[XmlIgnore]
		public string Class
		{
			get
			{
				return this.internalClass;
			}
			set
			{
				this.internalClass = value;
			}
		}

		[XmlIgnore]
		public Recipients Recipients
		{
			get
			{
				if (this.internalRecipients == null)
				{
					this.internalRecipients = new Recipients();
				}
				return this.internalRecipients;
			}
			set
			{
				this.internalRecipients = value;
			}
		}

		[XmlIgnore]
		public Item Item
		{
			get
			{
				if (this.internalItem == null)
				{
					this.internalItem = new Item();
				}
				return this.internalItem;
			}
			set
			{
				this.internalItem = value;
			}
		}

		[XmlElement(ElementName = "Class", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string", Namespace = "Send:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string internalClass;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(Recipients), ElementName = "Recipients", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "Send:")]
		public Recipients internalRecipients;

		[XmlElement(Type = typeof(Item), ElementName = "Item", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "Send:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public Item internalItem;
	}
}
