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
	public class SearchableMailboxType
	{
		public string Guid
		{
			get
			{
				return this.guidField;
			}
			set
			{
				this.guidField = value;
			}
		}

		public string PrimarySmtpAddress
		{
			get
			{
				return this.primarySmtpAddressField;
			}
			set
			{
				this.primarySmtpAddressField = value;
			}
		}

		public bool IsExternalMailbox
		{
			get
			{
				return this.isExternalMailboxField;
			}
			set
			{
				this.isExternalMailboxField = value;
			}
		}

		public string ExternalEmailAddress
		{
			get
			{
				return this.externalEmailAddressField;
			}
			set
			{
				this.externalEmailAddressField = value;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.displayNameField;
			}
			set
			{
				this.displayNameField = value;
			}
		}

		public bool IsMembershipGroup
		{
			get
			{
				return this.isMembershipGroupField;
			}
			set
			{
				this.isMembershipGroupField = value;
			}
		}

		public string ReferenceId
		{
			get
			{
				return this.referenceIdField;
			}
			set
			{
				this.referenceIdField = value;
			}
		}

		private string guidField;

		private string primarySmtpAddressField;

		private bool isExternalMailboxField;

		private string externalEmailAddressField;

		private string displayNameField;

		private bool isMembershipGroupField;

		private string referenceIdField;
	}
}
