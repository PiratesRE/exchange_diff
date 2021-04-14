using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public sealed class ManagedFolderMailboxPolicy : MailboxPolicy
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ManagedFolderMailboxPolicy.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ManagedFolderMailboxPolicy.mostDerivedClass;
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return ManagedFolderMailboxPolicy.parentPath;
			}
		}

		internal override bool CheckForAssociatedUsers()
		{
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.DistinguishedName, base.Id.DistinguishedName),
				new ExistsFilter(ManagedFolderMailboxPolicySchema.AssociatedUsers)
			});
			ManagedFolderMailboxPolicy[] array = base.Session.Find<ManagedFolderMailboxPolicy>(null, QueryScope.SubTree, filter, null, 1);
			return array != null && array.Length > 0;
		}

		public MultiValuedProperty<ADObjectId> ManagedFolderLinks
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ManagedFolderMailboxPolicySchema.ManagedFolderLinks];
			}
			set
			{
				this[ManagedFolderMailboxPolicySchema.ManagedFolderLinks] = value;
			}
		}

		internal ValidationError AddManagedFolderToPolicy(IConfigurationSession session, ELCFolder elcFolderToAdd)
		{
			ValidationError result;
			if (elcFolderToAdd.FolderType != ElcFolderType.ManagedCustomFolder)
			{
				ADObjectId adobjectId = this.ManagedFolderLinks.Find(delegate(ADObjectId id)
				{
					ELCFolder elcfolder = session.Read<ELCFolder>(id);
					return elcfolder != null && elcfolder.FolderType == elcFolderToAdd.FolderType;
				});
				if (adobjectId == null)
				{
					try
					{
						this.ManagedFolderLinks.Add(elcFolderToAdd.Id);
						return null;
					}
					catch (InvalidOperationException ex)
					{
						return new PropertyValidationError(DirectoryStrings.ErrorInvalidFolderLinksAddition(elcFolderToAdd.Name, ex.Message), ManagedFolderMailboxPolicySchema.ManagedFolderLinks, this);
					}
				}
				if (adobjectId == elcFolderToAdd.Id)
				{
					return new PropertyValidationError(DirectoryStrings.ErrorDuplicateManagedFolderAddition(elcFolderToAdd.Name), ManagedFolderMailboxPolicySchema.ManagedFolderLinks, this);
				}
				return new PropertyValidationError(DirectoryStrings.ErrorDefaultElcFolderTypeExists(elcFolderToAdd.Name, elcFolderToAdd.FolderType.ToString()), ManagedFolderMailboxPolicySchema.ManagedFolderLinks, this);
			}
			else
			{
				try
				{
					this.ManagedFolderLinks.Add(elcFolderToAdd.Id);
					result = null;
				}
				catch (InvalidOperationException ex2)
				{
					result = new PropertyValidationError(new LocalizedString(ex2.Message), ManagedFolderMailboxPolicySchema.ManagedFolderLinks, this);
				}
			}
			return result;
		}

		internal bool AreDefaultManagedFolderLinksUnique(IConfigurationSession session)
		{
			List<ELCFolder> list = new List<ELCFolder>();
			foreach (ADObjectId entryId in this.ManagedFolderLinks)
			{
				ELCFolder elcFolderToCheck = session.Read<ELCFolder>(entryId);
				if (elcFolderToCheck != null && elcFolderToCheck.FolderType != ElcFolderType.ManagedCustomFolder)
				{
					ELCFolder elcfolder = list.Find((ELCFolder folder) => folder.FolderType == elcFolderToCheck.FolderType);
					if (elcfolder == null)
					{
						list.Add(elcFolderToCheck);
					}
					else if (elcfolder.Id.ObjectGuid != elcFolderToCheck.Id.ObjectGuid)
					{
						return false;
					}
				}
			}
			return true;
		}

		private static ManagedFolderMailboxPolicySchema schema = ObjectSchema.GetInstance<ManagedFolderMailboxPolicySchema>();

		private static string mostDerivedClass = "msExchMailboxRecipientTemplate";

		private static ADObjectId parentPath = new ADObjectId("CN=ELC Mailbox Policies");
	}
}
