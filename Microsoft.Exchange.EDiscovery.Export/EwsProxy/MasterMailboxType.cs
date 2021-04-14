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
	public class MasterMailboxType
	{
		public string MailboxType
		{
			get
			{
				return this.mailboxTypeField;
			}
			set
			{
				this.mailboxTypeField = value;
			}
		}

		public string Alias
		{
			get
			{
				return this.aliasField;
			}
			set
			{
				this.aliasField = value;
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

		public string SmtpAddress
		{
			get
			{
				return this.smtpAddressField;
			}
			set
			{
				this.smtpAddressField = value;
			}
		}

		public ModernGroupTypeType GroupType
		{
			get
			{
				return this.groupTypeField;
			}
			set
			{
				this.groupTypeField = value;
			}
		}

		[XmlIgnore]
		public bool GroupTypeSpecified
		{
			get
			{
				return this.groupTypeFieldSpecified;
			}
			set
			{
				this.groupTypeFieldSpecified = value;
			}
		}

		public string Description
		{
			get
			{
				return this.descriptionField;
			}
			set
			{
				this.descriptionField = value;
			}
		}

		public string Photo
		{
			get
			{
				return this.photoField;
			}
			set
			{
				this.photoField = value;
			}
		}

		public string SharePointUrl
		{
			get
			{
				return this.sharePointUrlField;
			}
			set
			{
				this.sharePointUrlField = value;
			}
		}

		public string InboxUrl
		{
			get
			{
				return this.inboxUrlField;
			}
			set
			{
				this.inboxUrlField = value;
			}
		}

		public string CalendarUrl
		{
			get
			{
				return this.calendarUrlField;
			}
			set
			{
				this.calendarUrlField = value;
			}
		}

		public string DomainController
		{
			get
			{
				return this.domainControllerField;
			}
			set
			{
				this.domainControllerField = value;
			}
		}

		private string mailboxTypeField;

		private string aliasField;

		private string displayNameField;

		private string smtpAddressField;

		private ModernGroupTypeType groupTypeField;

		private bool groupTypeFieldSpecified;

		private string descriptionField;

		private string photoField;

		private string sharePointUrlField;

		private string inboxUrlField;

		private string calendarUrlField;

		private string domainControllerField;
	}
}
