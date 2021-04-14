using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class GetUMCallSummaryType : BaseRequestType
	{
		public string DailPlanGuid
		{
			get
			{
				return this.dailPlanGuidField;
			}
			set
			{
				this.dailPlanGuidField = value;
			}
		}

		public string GatewayGuid
		{
			get
			{
				return this.gatewayGuidField;
			}
			set
			{
				this.gatewayGuidField = value;
			}
		}

		public UMCDRGroupByType GroupRecordsBy
		{
			get
			{
				return this.groupRecordsByField;
			}
			set
			{
				this.groupRecordsByField = value;
			}
		}

		private string dailPlanGuidField;

		private string gatewayGuidField;

		private UMCDRGroupByType groupRecordsByField;
	}
}
