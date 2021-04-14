using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SendRequest
{
	[XmlRoot(ElementName = "Send", Namespace = "Send:", IsNullable = false)]
	[Serializable]
	public class Send
	{
		[XmlIgnore]
		public SendItem SendItem
		{
			get
			{
				if (this.internalSendItem == null)
				{
					this.internalSendItem = new SendItem();
				}
				return this.internalSendItem;
			}
			set
			{
				this.internalSendItem = value;
			}
		}

		[XmlIgnore]
		public SaveItem SaveItem
		{
			get
			{
				if (this.internalSaveItem == null)
				{
					this.internalSaveItem = new SaveItem();
				}
				return this.internalSaveItem;
			}
			set
			{
				this.internalSaveItem = value;
			}
		}

		[XmlElement(Type = typeof(SendItem), ElementName = "SendItem", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "Send:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public SendItem internalSendItem;

		[XmlElement(Type = typeof(SaveItem), ElementName = "SaveItem", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "Send:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public SaveItem internalSaveItem;
	}
}
