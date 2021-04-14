using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsResponse
{
	[XmlType(TypeName = "SettingsFault", Namespace = "HMSETTINGS:")]
	[Serializable]
	public class SettingsFault
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

		[XmlElement(ElementName = "Faultcode", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string", Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string internalFaultcode;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "Faultstring", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string", Namespace = "HMSETTINGS:")]
		public string internalFaultstring;

		[XmlElement(ElementName = "Detail", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string", Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string internalDetail;
	}
}
