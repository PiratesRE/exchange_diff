using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Exchange.Net.Protocols.DeltaSync.HMMail;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.ItemOperationsRequest
{
	[XmlType(TypeName = "FetchType", Namespace = "ItemOperations:")]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Serializable]
	public class FetchType : ItemOpsBaseType
	{
		[XmlIgnore]
		public string Compression
		{
			get
			{
				return this.internalCompression;
			}
			set
			{
				this.internalCompression = value;
			}
		}

		[XmlIgnore]
		public ResponseContentTypeType ResponseContentType
		{
			get
			{
				return this.internalResponseContentType;
			}
			set
			{
				this.internalResponseContentType = value;
				this.internalResponseContentTypeSpecified = true;
			}
		}

		[XmlElement(ElementName = "Compression", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string", Namespace = "HMMAIL:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string internalCompression;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "ResponseContentType", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMMAIL:")]
		public ResponseContentTypeType internalResponseContentType;

		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool internalResponseContentTypeSpecified;
	}
}
