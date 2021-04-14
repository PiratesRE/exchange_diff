using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[Serializable]
	public class GetServerTimeZonesType : BaseRequestType
	{
		[XmlArrayItem("Id", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public string[] Ids
		{
			get
			{
				return this.idsField;
			}
			set
			{
				this.idsField = value;
			}
		}

		[XmlAttribute]
		public bool ReturnFullTimeZoneData
		{
			get
			{
				return this.returnFullTimeZoneDataField;
			}
			set
			{
				this.returnFullTimeZoneDataField = value;
			}
		}

		[XmlIgnore]
		public bool ReturnFullTimeZoneDataSpecified
		{
			get
			{
				return this.returnFullTimeZoneDataFieldSpecified;
			}
			set
			{
				this.returnFullTimeZoneDataFieldSpecified = value;
			}
		}

		private string[] idsField;

		private bool returnFullTimeZoneDataField;

		private bool returnFullTimeZoneDataFieldSpecified;
	}
}
