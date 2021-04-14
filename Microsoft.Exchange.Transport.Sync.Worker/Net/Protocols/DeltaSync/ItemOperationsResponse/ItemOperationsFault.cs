using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.ItemOperationsResponse
{
	[XmlType(TypeName = "ItemOperationsFault", Namespace = "ItemOperations:")]
	[Serializable]
	public class ItemOperationsFault
	{
		[XmlIgnore]
		public string Faultcode
		{
			get
			{
				return this.internalFaultcode;
			}
			set
			{
				this.internalFaultcode = value;
			}
		}

		[XmlIgnore]
		public string Faultstring
		{
			get
			{
				return this.internalFaultstring;
			}
			set
			{
				this.internalFaultstring = value;
			}
		}

		[XmlIgnore]
		public string Detail
		{
			get
			{
				return this.internalDetail;
			}
			set
			{
				this.internalDetail = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "Faultcode", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string", Namespace = "ItemOperations:")]
		public string internalFaultcode;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "Faultstring", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string", Namespace = "ItemOperations:")]
		public string internalFaultstring;

		[XmlElement(ElementName = "Detail", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string", Namespace = "ItemOperations:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string internalDetail;
	}
}
