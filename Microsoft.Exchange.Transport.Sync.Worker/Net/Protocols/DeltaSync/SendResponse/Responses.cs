using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SendResponse
{
	[XmlType(TypeName = "Responses", Namespace = "Send:")]
	[Serializable]
	public class Responses
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

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(SendItem), ElementName = "SendItem", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "Send:")]
		public SendItem internalSendItem;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(SaveItem), ElementName = "SaveItem", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "Send:")]
		public SaveItem internalSaveItem;
	}
}
