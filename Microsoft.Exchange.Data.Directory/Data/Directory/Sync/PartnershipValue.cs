using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[Serializable]
	public class PartnershipValue
	{
		[XmlAttribute]
		public string PartnerContextId
		{
			get
			{
				return this.partnerContextIdField;
			}
			set
			{
				this.partnerContextIdField = value;
			}
		}

		[XmlAttribute]
		public int PartnerType
		{
			get
			{
				return this.partnerTypeField;
			}
			set
			{
				this.partnerTypeField = value;
			}
		}

		[XmlAttribute]
		public bool LoggingEnabled
		{
			get
			{
				return this.loggingEnabledField;
			}
			set
			{
				this.loggingEnabledField = value;
			}
		}

		[XmlAttribute]
		public bool SupportPartner
		{
			get
			{
				return this.supportPartnerField;
			}
			set
			{
				this.supportPartnerField = value;
			}
		}

		private string partnerContextIdField;

		private int partnerTypeField;

		private bool loggingEnabledField;

		private bool supportPartnerField;
	}
}
