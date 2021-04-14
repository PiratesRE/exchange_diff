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
	public class MailTipsServiceConfiguration : ServiceConfiguration
	{
		public bool MailTipsEnabled
		{
			get
			{
				return this.mailTipsEnabledField;
			}
			set
			{
				this.mailTipsEnabledField = value;
			}
		}

		public int MaxRecipientsPerGetMailTipsRequest
		{
			get
			{
				return this.maxRecipientsPerGetMailTipsRequestField;
			}
			set
			{
				this.maxRecipientsPerGetMailTipsRequestField = value;
			}
		}

		public int MaxMessageSize
		{
			get
			{
				return this.maxMessageSizeField;
			}
			set
			{
				this.maxMessageSizeField = value;
			}
		}

		public int LargeAudienceThreshold
		{
			get
			{
				return this.largeAudienceThresholdField;
			}
			set
			{
				this.largeAudienceThresholdField = value;
			}
		}

		public bool ShowExternalRecipientCount
		{
			get
			{
				return this.showExternalRecipientCountField;
			}
			set
			{
				this.showExternalRecipientCountField = value;
			}
		}

		[XmlArrayItem("Domain", IsNullable = false)]
		public SmtpDomain[] InternalDomains
		{
			get
			{
				return this.internalDomainsField;
			}
			set
			{
				this.internalDomainsField = value;
			}
		}

		public bool PolicyTipsEnabled
		{
			get
			{
				return this.policyTipsEnabledField;
			}
			set
			{
				this.policyTipsEnabledField = value;
			}
		}

		public int LargeAudienceCap
		{
			get
			{
				return this.largeAudienceCapField;
			}
			set
			{
				this.largeAudienceCapField = value;
			}
		}

		private bool mailTipsEnabledField;

		private int maxRecipientsPerGetMailTipsRequestField;

		private int maxMessageSizeField;

		private int largeAudienceThresholdField;

		private bool showExternalRecipientCountField;

		private SmtpDomain[] internalDomainsField;

		private bool policyTipsEnabledField;

		private int largeAudienceCapField;
	}
}
