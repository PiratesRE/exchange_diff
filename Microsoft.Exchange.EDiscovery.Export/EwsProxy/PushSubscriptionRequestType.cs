using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class PushSubscriptionRequestType : BaseSubscriptionRequestType
	{
		public int StatusFrequency
		{
			get
			{
				return this.statusFrequencyField;
			}
			set
			{
				this.statusFrequencyField = value;
			}
		}

		public string URL
		{
			get
			{
				return this.uRLField;
			}
			set
			{
				this.uRLField = value;
			}
		}

		public string CallerData
		{
			get
			{
				return this.callerDataField;
			}
			set
			{
				this.callerDataField = value;
			}
		}

		private int statusFrequencyField;

		private string uRLField;

		private string callerDataField;
	}
}
