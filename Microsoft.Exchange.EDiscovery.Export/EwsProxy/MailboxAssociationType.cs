using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class MailboxAssociationType
	{
		public GroupLocatorType Group
		{
			get
			{
				return this.groupField;
			}
			set
			{
				this.groupField = value;
			}
		}

		public UserLocatorType User
		{
			get
			{
				return this.userField;
			}
			set
			{
				this.userField = value;
			}
		}

		public bool IsMember
		{
			get
			{
				return this.isMemberField;
			}
			set
			{
				this.isMemberField = value;
			}
		}

		[XmlIgnore]
		public bool IsMemberSpecified
		{
			get
			{
				return this.isMemberFieldSpecified;
			}
			set
			{
				this.isMemberFieldSpecified = value;
			}
		}

		public DateTime JoinDate
		{
			get
			{
				return this.joinDateField;
			}
			set
			{
				this.joinDateField = value;
			}
		}

		[XmlIgnore]
		public bool JoinDateSpecified
		{
			get
			{
				return this.joinDateFieldSpecified;
			}
			set
			{
				this.joinDateFieldSpecified = value;
			}
		}

		public bool IsPin
		{
			get
			{
				return this.isPinField;
			}
			set
			{
				this.isPinField = value;
			}
		}

		[XmlIgnore]
		public bool IsPinSpecified
		{
			get
			{
				return this.isPinFieldSpecified;
			}
			set
			{
				this.isPinFieldSpecified = value;
			}
		}

		public string JoinedBy
		{
			get
			{
				return this.joinedByField;
			}
			set
			{
				this.joinedByField = value;
			}
		}

		private GroupLocatorType groupField;

		private UserLocatorType userField;

		private bool isMemberField;

		private bool isMemberFieldSpecified;

		private DateTime joinDateField;

		private bool joinDateFieldSpecified;

		private bool isPinField;

		private bool isPinFieldSpecified;

		private string joinedByField;
	}
}
