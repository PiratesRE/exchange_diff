using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Exchange.Net.Protocols.DeltaSync.Xop;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.HMMail
{
	[XmlRoot(ElementName = "Message", Namespace = "HMMAIL:", IsNullable = false)]
	[Serializable]
	public class Message
	{
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

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(Include), ElementName = "Include", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/2004/08/xop/include")]
		public Include internalInclude;
	}
}
