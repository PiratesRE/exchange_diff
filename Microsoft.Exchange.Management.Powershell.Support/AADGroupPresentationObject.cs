using System;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.WindowsAzure.ActiveDirectory;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Serializable]
	public sealed class AADGroupPresentationObject : AADDirectoryObjectPresentationObject
	{
		internal AADGroupPresentationObject(Group group) : base(group)
		{
			AADDirectoryObjectPresentationObject[] allowAccessTo;
			if (group.allowAccessTo == null)
			{
				allowAccessTo = null;
			}
			else
			{
				allowAccessTo = (from directoryObject in @group.allowAccessTo
				select AADPresentationObjectFactory.Create(directoryObject)).ToArray<AADDirectoryObjectPresentationObject>();
			}
			this.AllowAccessTo = allowAccessTo;
			this.Description = group.description;
			this.DisplayName = group.displayName;
			this.ExchangeResources = ((group.exchangeResources != null) ? group.exchangeResources.ToArray<string>() : null);
			this.GroupType = group.groupType;
			this.IsPublic = group.isPublic;
			this.Mail = group.mail;
			this.MailEnabled = group.mailEnabled;
			AADDirectoryObjectPresentationObject[] pendingMembers;
			if (group.pendingMembers == null)
			{
				pendingMembers = null;
			}
			else
			{
				pendingMembers = (from directoryObject in @group.pendingMembers
				select AADPresentationObjectFactory.Create(directoryObject)).ToArray<AADDirectoryObjectPresentationObject>();
			}
			this.PendingMembers = pendingMembers;
			this.ProxyAddresses = ((group.proxyAddresses != null) ? group.proxyAddresses.ToArray<string>() : null);
			this.SecurityEnabled = group.securityEnabled;
			this.SharePointResources = ((group.sharepointResources != null) ? group.sharepointResources.ToArray<string>() : null);
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<AADGroupPresentationObjectSchema>();
			}
		}

		public AADDirectoryObjectPresentationObject[] AllowAccessTo
		{
			get
			{
				return (AADDirectoryObjectPresentationObject[])this[AADGroupPresentationObjectSchema.AllowAccessTo];
			}
			set
			{
				this[AADGroupPresentationObjectSchema.AllowAccessTo] = value;
			}
		}

		public string Description
		{
			get
			{
				return (string)this[AADGroupPresentationObjectSchema.Description];
			}
			set
			{
				this[AADGroupPresentationObjectSchema.Description] = value;
			}
		}

		public string DisplayName
		{
			get
			{
				return (string)this[AADGroupPresentationObjectSchema.DisplayName];
			}
			set
			{
				this[AADGroupPresentationObjectSchema.DisplayName] = value;
			}
		}

		public string[] ExchangeResources
		{
			get
			{
				return (string[])this[AADGroupPresentationObjectSchema.ExchangeResources];
			}
			set
			{
				this[AADGroupPresentationObjectSchema.ExchangeResources] = value;
			}
		}

		public string GroupType
		{
			get
			{
				return (string)this[AADGroupPresentationObjectSchema.GroupType];
			}
			set
			{
				this[AADGroupPresentationObjectSchema.GroupType] = value;
			}
		}

		public bool? IsPublic
		{
			get
			{
				return (bool?)this[AADGroupPresentationObjectSchema.IsPublic];
			}
			set
			{
				this[AADGroupPresentationObjectSchema.IsPublic] = value;
			}
		}

		public string Mail
		{
			get
			{
				return (string)this[AADGroupPresentationObjectSchema.Mail];
			}
			set
			{
				this[AADGroupPresentationObjectSchema.Mail] = value;
			}
		}

		public bool? MailEnabled
		{
			get
			{
				return (bool?)this[AADGroupPresentationObjectSchema.MailEnabled];
			}
			set
			{
				this[AADGroupPresentationObjectSchema.MailEnabled] = value;
			}
		}

		public AADDirectoryObjectPresentationObject[] PendingMembers
		{
			get
			{
				return (AADDirectoryObjectPresentationObject[])this[AADGroupPresentationObjectSchema.PendingMembers];
			}
			set
			{
				this[AADGroupPresentationObjectSchema.PendingMembers] = value;
			}
		}

		public string[] ProxyAddresses
		{
			get
			{
				return (string[])this[AADGroupPresentationObjectSchema.ProxyAddresses];
			}
			set
			{
				this[AADGroupPresentationObjectSchema.ProxyAddresses] = value;
			}
		}

		public bool? SecurityEnabled
		{
			get
			{
				return (bool?)this[AADGroupPresentationObjectSchema.SecurityEnabled];
			}
			set
			{
				this[AADGroupPresentationObjectSchema.SecurityEnabled] = value;
			}
		}

		public string[] SharePointResources
		{
			get
			{
				return (string[])this[AADGroupPresentationObjectSchema.SharePointResources];
			}
			set
			{
				this[AADGroupPresentationObjectSchema.SharePointResources] = value;
			}
		}
	}
}
