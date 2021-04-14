using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[XmlInclude(typeof(DirectoryLinkInvitedBy))]
	[XmlInclude(typeof(UnauthOrig))]
	[XmlInclude(typeof(MSExchModeratedByLink))]
	[XmlInclude(typeof(DirectoryLinkAddressListObjectToGroup))]
	[XmlInclude(typeof(DirectoryLinkOwnerObjectToUserAndServicePrincipal))]
	[XmlInclude(typeof(Owner))]
	[XmlInclude(typeof(DirectoryLinkGroupBasedLicenseErrorOccuredUserToGroup))]
	[XmlInclude(typeof(DirectoryLinkServicePrincipalToServicePrincipal))]
	[XmlInclude(typeof(DelegationEntry))]
	[XmlInclude(typeof(DirectoryLinkUserToUser))]
	[XmlInclude(typeof(CloudMSExchDelegateListLink))]
	[XmlInclude(typeof(MSExchDelegateListLink))]
	[XmlInclude(typeof(DirectoryLinkAddressListObjectToGroupAndUser))]
	[XmlInclude(typeof(PublicDelegates))]
	[XmlInclude(typeof(DirectoryLinkGroupToPerson))]
	[XmlInclude(typeof(MSExchCoManagedByLink))]
	[XmlInclude(typeof(ManagedBy))]
	[XmlInclude(typeof(DirectoryLinkUserToServicePrincipal))]
	[XmlInclude(typeof(InvitedBy))]
	[XmlInclude(typeof(DirectoryLinkAllowAccessTo))]
	[XmlInclude(typeof(AllowAccessTo))]
	[XmlInclude(typeof(DirectoryLinkPendingMember))]
	[XmlInclude(typeof(PendingMember))]
	[XmlInclude(typeof(DirectoryLinkMemberObjectToAddressListObject))]
	[XmlInclude(typeof(Member))]
	[XmlInclude(typeof(DirectoryLinkPersonToPerson))]
	[XmlInclude(typeof(Manager))]
	[XmlInclude(typeof(DirectoryLinkDeviceToUser))]
	[XmlInclude(typeof(RegisteredOwner))]
	[XmlInclude(typeof(RegisteredUsers))]
	[XmlInclude(typeof(DirectoryLinkAddressListObjectToAddressListObject))]
	[XmlInclude(typeof(CloudPublicDelegates))]
	[XmlInclude(typeof(DirectoryLinkAddressListObjectToPerson))]
	[XmlInclude(typeof(MSExchBypassModerationLink))]
	[XmlInclude(typeof(AuthOrig))]
	[XmlInclude(typeof(MSExchBypassModerationFromDLMembersLink))]
	[XmlInclude(typeof(DLMemSubmitPerms))]
	[XmlInclude(typeof(DLMemRejectPerms))]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[Serializable]
	public abstract class DirectoryLink
	{
		public abstract DirectoryObjectClass GetSourceClass();

		public abstract void SetSourceClass(DirectoryObjectClass objectClass);

		public abstract DirectoryObjectClass GetTargetClass();

		public abstract void SetTargetClass(DirectoryObjectClass objectClass);

		protected static object ConvertEnums(Type targetType, string name)
		{
			return Enum.Parse(targetType, name);
		}

		public DirectoryLink()
		{
			this.deletedField = false;
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

		[XmlAttribute]
		public string SourceId
		{
			get
			{
				return this.sourceIdField;
			}
			set
			{
				this.sourceIdField = value;
			}
		}

		[XmlAttribute]
		public string TargetId
		{
			get
			{
				return this.targetIdField;
			}
			set
			{
				this.targetIdField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(false)]
		public bool Deleted
		{
			get
			{
				return this.deletedField;
			}
			set
			{
				this.deletedField = value;
			}
		}

		[XmlAnyAttribute]
		public XmlAttribute[] AnyAttr
		{
			get
			{
				return this.anyAttrField;
			}
			set
			{
				this.anyAttrField = value;
			}
		}

		private string contextIdField;

		private string sourceIdField;

		private string targetIdField;

		private bool deletedField;

		private XmlAttribute[] anyAttrField;
	}
}
