using System;
using System.ComponentModel;
using System.IO;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Exchange.Net.Protocols.DeltaSync.Xop;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.ItemOperationsResponse
{
	[XmlType(TypeName = "Message", Namespace = "ItemOperations:")]
	[Serializable]
	public class Message
	{
		internal Stream EmailMessage
		{
			get
			{
				return this.emailMessage;
			}
			set
			{
				this.emailMessage = value;
			}
		}

		[XmlIgnore]
		public Include Include
		{
			get
			{
				if (this.internalInclude == null)
				{
					this.internalInclude = new Include();
				}
				return this.internalInclude;
			}
			set
			{
				this.internalInclude = value;
			}
		}

		private Stream emailMessage;

		[XmlElement(Type = typeof(Include), ElementName = "Include", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/2004/08/xop/include")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public Include internalInclude;
	}
}
