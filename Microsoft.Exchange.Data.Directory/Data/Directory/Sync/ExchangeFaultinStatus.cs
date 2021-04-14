using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11")]
	[DesignerCategory("code")]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class ExchangeFaultinStatus
	{
		public ExchangeFaultinStatus()
		{
			this.exchangeFaultinStatusCodeField = 0;
		}

		[XmlElement(IsNullable = true, Order = 0)]
		public string ErrorDescription
		{
			get
			{
				return this.errorDescriptionField;
			}
			set
			{
				this.errorDescriptionField = value;
			}
		}

		[XmlAttribute]
		public string ContextId
		{
			get
			{
				return this.contextIdField;
			}
			set
			{
				this.contextIdField = value;
			}
		}

		[DefaultValue(0)]
		[XmlAttribute]
		public int ExchangeFaultinStatusCode
		{
			get
			{
				return this.exchangeFaultinStatusCodeField;
			}
			set
			{
				this.exchangeFaultinStatusCodeField = value;
			}
		}

		private string errorDescriptionField;

		private string contextIdField;

		private int exchangeFaultinStatusCodeField;
	}
}
