using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Live.DomainServices
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://domains.live.com/Service/DomainServices/V1.0")]
	[DebuggerStepThrough]
	[Serializable]
	public class DomainInfoEx : DomainInfo
	{
		public NamespaceState NamespaceState
		{
			get
			{
				return this.namespaceStateField;
			}
			set
			{
				this.namespaceStateField = value;
			}
		}

		public OpenState OpenState
		{
			get
			{
				return this.openStateField;
			}
			set
			{
				this.openStateField = value;
			}
		}

		public EmailState EmailState
		{
			get
			{
				return this.emailStateField;
			}
			set
			{
				this.emailStateField = value;
			}
		}

		public bool IsEmailSuspended
		{
			get
			{
				return this.isEmailSuspendedField;
			}
			set
			{
				this.isEmailSuspendedField = value;
			}
		}

		public bool IsMxValid
		{
			get
			{
				return this.isMxValidField;
			}
			set
			{
				this.isMxValidField = value;
			}
		}

		public string[] MxRecords
		{
			get
			{
				return this.mxRecordsField;
			}
			set
			{
				this.mxRecordsField = value;
			}
		}

		public EmailType EmailType
		{
			get
			{
				return this.emailTypeField;
			}
			set
			{
				this.emailTypeField = value;
			}
		}

		private NamespaceState namespaceStateField;

		private OpenState openStateField;

		private EmailState emailStateField;

		private bool isEmailSuspendedField;

		private bool isMxValidField;

		private string[] mxRecordsField;

		private EmailType emailTypeField;
	}
}
