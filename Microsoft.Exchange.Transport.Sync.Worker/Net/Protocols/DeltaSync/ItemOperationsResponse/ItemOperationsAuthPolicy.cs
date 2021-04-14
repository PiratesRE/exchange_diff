using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.ItemOperationsResponse
{
	[XmlType(TypeName = "ItemOperationsAuthPolicy", Namespace = "ItemOperations:")]
	[Serializable]
	public class ItemOperationsAuthPolicy
	{
		[XmlIgnore]
		public string SAP
		{
			get
			{
				return this.internalSAP;
			}
			set
			{
				this.internalSAP = value;
			}
		}

		[XmlIgnore]
		public string Version
		{
			get
			{
				return this.internalVersion;
			}
			set
			{
				this.internalVersion = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "SAP", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string", Namespace = "ItemOperations:")]
		public string internalSAP;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "Version", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string", Namespace = "ItemOperations:")]
		public string internalVersion;
	}
}
