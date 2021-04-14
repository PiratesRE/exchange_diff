using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Exchange.Net.Protocols.DeltaSync.HMMail;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.ItemOperationsRequest
{
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[XmlType(TypeName = "ScanType", Namespace = "ItemOperations:")]
	[Serializable]
	public class ScanType : ItemOpsBaseType
	{
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

		[XmlElement(ElementName = "ResponseContentType", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMMAIL:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public ResponseContentTypeType internalResponseContentType;

		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool internalResponseContentTypeSpecified;
	}
}
