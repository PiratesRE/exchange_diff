using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.ItemOperationsRequest
{
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[XmlType(TypeName = "ReportAsSpamType", Namespace = "ItemOperations:")]
	[Serializable]
	public class ReportAsSpamType : ItemOpsBaseType
	{
		[XmlIgnore]
		public MessageType MessageType
		{
			get
			{
				if (this.internalMessageType == null)
				{
					this.internalMessageType = new MessageType();
				}
				return this.internalMessageType;
			}
			set
			{
				this.internalMessageType = value;
			}
		}

		[XmlIgnore]
		public ReportAsSpamTypeOptions Options
		{
			get
			{
				if (this.internalOptions == null)
				{
					this.internalOptions = new ReportAsSpamTypeOptions();
				}
				return this.internalOptions;
			}
			set
			{
				this.internalOptions = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(MessageType), ElementName = "MessageType", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "ItemOperations:")]
		public MessageType internalMessageType;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(ReportAsSpamTypeOptions), ElementName = "Options", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "ItemOperations:")]
		public ReportAsSpamTypeOptions internalOptions;
	}
}
