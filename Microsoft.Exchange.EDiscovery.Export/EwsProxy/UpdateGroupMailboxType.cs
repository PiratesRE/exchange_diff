using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class UpdateGroupMailboxType : BaseRequestType
	{
		public string GroupSmtpAddress
		{
			get
			{
				return this.groupSmtpAddressField;
			}
			set
			{
				this.groupSmtpAddressField = value;
			}
		}

		public string ExecutingUserSmtpAddress
		{
			get
			{
				return this.executingUserSmtpAddressField;
			}
			set
			{
				this.executingUserSmtpAddressField = value;
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

		public GroupMailboxConfigurationActionType ForceConfigurationAction
		{
			get
			{
				return this.forceConfigurationActionField;
			}
			set
			{
				this.forceConfigurationActionField = value;
			}
		}

		public GroupMemberIdentifierType MemberIdentifierType
		{
			get
			{
				return this.memberIdentifierTypeField;
			}
			set
			{
				this.memberIdentifierTypeField = value;
			}
		}

		[XmlIgnore]
		public bool MemberIdentifierTypeSpecified
		{
			get
			{
				return this.memberIdentifierTypeFieldSpecified;
			}
			set
			{
				this.memberIdentifierTypeFieldSpecified = value;
			}
		}

		[XmlArrayItem("String", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public string[] AddedMembers
		{
			get
			{
				return this.addedMembersField;
			}
			set
			{
				this.addedMembersField = value;
			}
		}

		[XmlArrayItem("String", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public string[] RemovedMembers
		{
			get
			{
				return this.removedMembersField;
			}
			set
			{
				this.removedMembersField = value;
			}
		}

		[XmlArrayItem("String", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public string[] AddedPendingMembers
		{
			get
			{
				return this.addedPendingMembersField;
			}
			set
			{
				this.addedPendingMembersField = value;
			}
		}

		[XmlArrayItem("String", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public string[] RemovedPendingMembers
		{
			get
			{
				return this.removedPendingMembersField;
			}
			set
			{
				this.removedPendingMembersField = value;
			}
		}

		public int PermissionsVersion
		{
			get
			{
				return this.permissionsVersionField;
			}
			set
			{
				this.permissionsVersionField = value;
			}
		}

		[XmlIgnore]
		public bool PermissionsVersionSpecified
		{
			get
			{
				return this.permissionsVersionFieldSpecified;
			}
			set
			{
				this.permissionsVersionFieldSpecified = value;
			}
		}

		private string groupSmtpAddressField;

		private string executingUserSmtpAddressField;

		private string domainControllerField;

		private GroupMailboxConfigurationActionType forceConfigurationActionField;

		private GroupMemberIdentifierType memberIdentifierTypeField;

		private bool memberIdentifierTypeFieldSpecified;

		private string[] addedMembersField;

		private string[] removedMembersField;

		private string[] addedPendingMembersField;

		private string[] removedPendingMembersField;

		private int permissionsVersionField;

		private bool permissionsVersionFieldSpecified;
	}
}
