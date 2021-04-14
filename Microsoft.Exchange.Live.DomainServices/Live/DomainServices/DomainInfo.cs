using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Live.DomainServices
{
	[DebuggerStepThrough]
	[XmlInclude(typeof(DomainInfoEx))]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://domains.live.com/Service/DomainServices/V1.0")]
	[Serializable]
	public class DomainInfo
	{
		public string DomainName
		{
			get
			{
				return this.domainNameField;
			}
			set
			{
				this.domainNameField = value;
			}
		}

		public int PartnerId
		{
			get
			{
				return this.partnerIdField;
			}
			set
			{
				this.partnerIdField = value;
			}
		}

		public string DomainConfigId
		{
			get
			{
				return this.domainConfigIdField;
			}
			set
			{
				this.domainConfigIdField = value;
			}
		}

		public PermissionFlags PermissionFlags
		{
			get
			{
				return this.permissionFlagsField;
			}
			set
			{
				this.permissionFlagsField = value;
			}
		}

		public bool IsPendingProcessing
		{
			get
			{
				return this.isPendingProcessingField;
			}
			set
			{
				this.isPendingProcessingField = value;
			}
		}

		public bool IsPendingRelease
		{
			get
			{
				return this.isPendingReleaseField;
			}
			set
			{
				this.isPendingReleaseField = value;
			}
		}

		public bool IsMembershipEditable
		{
			get
			{
				return this.isMembershipEditableField;
			}
			set
			{
				this.isMembershipEditableField = value;
			}
		}

		public bool IsEmailActive
		{
			get
			{
				return this.isEmailActiveField;
			}
			set
			{
				this.isEmailActiveField = value;
			}
		}

		private string domainNameField;

		private int partnerIdField;

		private string domainConfigIdField;

		private PermissionFlags permissionFlagsField;

		private bool isPendingProcessingField;

		private bool isPendingReleaseField;

		private bool isMembershipEditableField;

		private bool isEmailActiveField;
	}
}
