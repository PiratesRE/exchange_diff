using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.AutoDiscoverProxy
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class OrganizationRelationshipSettings
	{
		public bool DeliveryReportEnabled
		{
			get
			{
				return this.deliveryReportEnabledField;
			}
			set
			{
				this.deliveryReportEnabledField = value;
			}
		}

		[XmlArrayItem("Domain")]
		[XmlArray(IsNullable = true)]
		public string[] DomainNames
		{
			get
			{
				return this.domainNamesField;
			}
			set
			{
				this.domainNamesField = value;
			}
		}

		public bool FreeBusyAccessEnabled
		{
			get
			{
				return this.freeBusyAccessEnabledField;
			}
			set
			{
				this.freeBusyAccessEnabledField = value;
			}
		}

		[XmlElement(IsNullable = true)]
		public string FreeBusyAccessLevel
		{
			get
			{
				return this.freeBusyAccessLevelField;
			}
			set
			{
				this.freeBusyAccessLevelField = value;
			}
		}

		public bool MailTipsAccessEnabled
		{
			get
			{
				return this.mailTipsAccessEnabledField;
			}
			set
			{
				this.mailTipsAccessEnabledField = value;
			}
		}

		[XmlElement(IsNullable = true)]
		public string MailTipsAccessLevel
		{
			get
			{
				return this.mailTipsAccessLevelField;
			}
			set
			{
				this.mailTipsAccessLevelField = value;
			}
		}

		public bool MailboxMoveEnabled
		{
			get
			{
				return this.mailboxMoveEnabledField;
			}
			set
			{
				this.mailboxMoveEnabledField = value;
			}
		}

		[XmlElement(IsNullable = true)]
		public string Name
		{
			get
			{
				return this.nameField;
			}
			set
			{
				this.nameField = value;
			}
		}

		[XmlElement(DataType = "anyURI", IsNullable = true)]
		public string TargetApplicationUri
		{
			get
			{
				return this.targetApplicationUriField;
			}
			set
			{
				this.targetApplicationUriField = value;
			}
		}

		[XmlElement(DataType = "anyURI", IsNullable = true)]
		public string TargetAutodiscoverEpr
		{
			get
			{
				return this.targetAutodiscoverEprField;
			}
			set
			{
				this.targetAutodiscoverEprField = value;
			}
		}

		[XmlElement(DataType = "anyURI", IsNullable = true)]
		public string TargetSharingEpr
		{
			get
			{
				return this.targetSharingEprField;
			}
			set
			{
				this.targetSharingEprField = value;
			}
		}

		private bool deliveryReportEnabledField;

		private string[] domainNamesField;

		private bool freeBusyAccessEnabledField;

		private string freeBusyAccessLevelField;

		private bool mailTipsAccessEnabledField;

		private string mailTipsAccessLevelField;

		private bool mailboxMoveEnabledField;

		private string nameField;

		private string targetApplicationUriField;

		private string targetAutodiscoverEprField;

		private string targetSharingEprField;
	}
}
