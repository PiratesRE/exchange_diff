using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	internal sealed class PolicyProvider
	{
		private PolicyProvider.GetPoliciesDelegate GetPolicies { get; set; }

		private PolicyProvider.GetPolicyTagFromFolderDelegate GetPolicyTagFromFolder { get; set; }

		private PolicyProvider.GetPolicyTagFromItemDelegate GetPolicyTagFromItem { get; set; }

		private PolicyProvider.SetPolicyTagOnFolderDelegate SetPolicyTagOnFolder { get; set; }

		private PolicyProvider.SetPolicyTagOnItemDelegate SetPolicyTagOnItem { get; set; }

		private PolicyProvider.ClearPolicyTagOnFolderDelegate ClearPolicyTagOnFolder { get; set; }

		private PolicyProvider.ClearPolicyTagOnItemDelegate ClearPolicyTagOnItem { get; set; }

		private PropertyDefinition[] PolicyProperties { get; set; }

		private string PolicyType { get; set; }

		private PolicyProvider()
		{
		}

		internal bool IsPolicyEnabled(MailboxSession mailboxSession)
		{
			if (mailboxSession == null)
			{
				throw new ArgumentNullException("mailboxSession");
			}
			PolicyTagList allPolicies = this.GetAllPolicies(mailboxSession);
			return allPolicies != null && allPolicies.Count > 0;
		}

		internal PolicyTagList GetAllPolicies(MailboxSession mailboxSession)
		{
			if (mailboxSession == null)
			{
				throw new ArgumentNullException("mailboxSession");
			}
			return this.GetPolicies(mailboxSession);
		}

		internal void ClearPolicyTag(StoreObject itemOrFolder)
		{
			if (itemOrFolder == null)
			{
				throw new ArgumentNullException("itemOrFolder");
			}
			this.SetPolicyTag(itemOrFolder, true, null);
		}

		internal void ApplyPolicyTag(StoreObject itemOrFolder, Guid policyGuid)
		{
			if (itemOrFolder == null)
			{
				throw new ArgumentNullException("itemOrFolder");
			}
			this.SetPolicyTag(itemOrFolder, false, new Guid?(policyGuid));
		}

		private void SetPolicyTag(StoreObject itemOrFolder, bool isInherited, Guid? policyGuid)
		{
			MailboxSession mailboxSession = itemOrFolder.Session as MailboxSession;
			if (mailboxSession == null)
			{
				throw new NotSupportedException("Only support item or folder in mailbox.");
			}
			if (!this.IsPolicyEnabled(mailboxSession))
			{
				throw new InvalidOperationException("The mailbox is not enabled for " + this.PolicyType);
			}
			if (isInherited)
			{
				if (itemOrFolder is Folder)
				{
					this.ClearPolicyTagOnFolder((Folder)itemOrFolder);
					return;
				}
				this.ClearPolicyTagOnItem(itemOrFolder);
				return;
			}
			else
			{
				PolicyTagList allPolicies = this.GetAllPolicies(mailboxSession);
				PolicyTag policyTag;
				if (!allPolicies.TryGetValue(policyGuid.Value, out policyTag))
				{
					throw new ArgumentException("policyGuid is invalid");
				}
				if (itemOrFolder is Folder)
				{
					this.SetPolicyTagOnFolder(policyTag, (Folder)itemOrFolder);
					return;
				}
				this.SetPolicyTagOnItem(policyTag, itemOrFolder);
				return;
			}
		}

		internal Guid? GetPolicyTag(StoreObject itemOrFolder, out bool isInherited)
		{
			if (itemOrFolder == null)
			{
				throw new ArgumentNullException("itemOrFolder");
			}
			bool flag;
			Guid? result;
			if (itemOrFolder is Folder)
			{
				result = this.GetPolicyTagFromFolder((Folder)itemOrFolder, out flag);
			}
			else
			{
				DateTime? dateTime;
				result = this.GetPolicyTagFromItem(itemOrFolder, out flag, out dateTime);
			}
			isInherited = !flag;
			return result;
		}

		internal Item OpenItemForPolicyTag(MailboxSession mailboxSession, StoreObjectId messageId)
		{
			return Item.Bind(mailboxSession, messageId, this.PolicyProperties);
		}

		internal Folder OpenFolderForPolicyTag(MailboxSession mailboxSession, StoreObjectId folderId)
		{
			return Folder.Bind(mailboxSession, folderId, this.PolicyProperties);
		}

		public override string ToString()
		{
			return this.PolicyType;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static PolicyProvider()
		{
			PolicyProvider policyProvider = new PolicyProvider();
			policyProvider.PolicyType = "DeletePolicy";
			policyProvider.GetPolicies = ((MailboxSession mailboxSession) => mailboxSession.GetPolicyTagList(RetentionActionType.DeleteAndAllowRecovery));
			policyProvider.GetPolicyTagFromFolder = new PolicyProvider.GetPolicyTagFromFolderDelegate(PolicyTagHelper.GetPolicyTagForDeleteFromFolder);
			policyProvider.GetPolicyTagFromItem = new PolicyProvider.GetPolicyTagFromItemDelegate(PolicyTagHelper.GetPolicyTagForDeleteFromItem);
			policyProvider.SetPolicyTagOnFolder = new PolicyProvider.SetPolicyTagOnFolderDelegate(PolicyTagHelper.SetPolicyTagForDeleteOnFolder);
			policyProvider.SetPolicyTagOnItem = new PolicyProvider.SetPolicyTagOnItemDelegate(PolicyTagHelper.SetPolicyTagForDeleteOnItem);
			policyProvider.ClearPolicyTagOnFolder = new PolicyProvider.ClearPolicyTagOnFolderDelegate(PolicyTagHelper.ClearPolicyTagForDeleteOnFolder);
			policyProvider.ClearPolicyTagOnItem = new PolicyProvider.ClearPolicyTagOnItemDelegate(PolicyTagHelper.ClearPolicyTagForDeleteOnItem);
			policyProvider.PolicyProperties = PolicyTagHelper.RetentionProperties;
			PolicyProvider.DeletePolicyProvider = policyProvider;
			PolicyProvider policyProvider2 = new PolicyProvider();
			policyProvider2.PolicyType = "MovePolicy";
			policyProvider2.GetPolicies = ((MailboxSession mailboxSession) => mailboxSession.GetPolicyTagList(RetentionActionType.MoveToArchive));
			policyProvider2.GetPolicyTagFromFolder = new PolicyProvider.GetPolicyTagFromFolderDelegate(PolicyTagHelper.GetPolicyTagForArchiveFromFolder);
			policyProvider2.GetPolicyTagFromItem = delegate(StoreObject item, out bool isExplicit, out DateTime? moveToArchive)
			{
				bool flag;
				return PolicyTagHelper.GetPolicyTagForArchiveFromItem(item, out isExplicit, out flag, out moveToArchive);
			};
			policyProvider2.SetPolicyTagOnFolder = new PolicyProvider.SetPolicyTagOnFolderDelegate(PolicyTagHelper.SetPolicyTagForArchiveOnFolder);
			policyProvider2.SetPolicyTagOnItem = new PolicyProvider.SetPolicyTagOnItemDelegate(PolicyTagHelper.SetPolicyTagForArchiveOnItem);
			policyProvider2.ClearPolicyTagOnFolder = new PolicyProvider.ClearPolicyTagOnFolderDelegate(PolicyTagHelper.ClearPolicyTagForArchiveOnFolder);
			policyProvider2.ClearPolicyTagOnItem = new PolicyProvider.ClearPolicyTagOnItemDelegate(PolicyTagHelper.ClearPolicyTagForArchiveOnItem);
			policyProvider2.PolicyProperties = PolicyTagHelper.ArchiveProperties;
			PolicyProvider.MovePolicyProvider = policyProvider2;
		}

		internal static readonly PolicyProvider DeletePolicyProvider;

		internal static readonly PolicyProvider MovePolicyProvider;

		private delegate PolicyTagList GetPoliciesDelegate(MailboxSession mailboxSession);

		private delegate Guid? GetPolicyTagFromFolderDelegate(Folder folder, out bool isExplicit);

		private delegate Guid? GetPolicyTagFromItemDelegate(StoreObject item, out bool isExplicit, out DateTime? actionDate);

		private delegate void SetPolicyTagOnFolderDelegate(PolicyTag policyTag, Folder folder);

		private delegate void SetPolicyTagOnItemDelegate(PolicyTag policyTag, StoreObject item);

		private delegate void ClearPolicyTagOnFolderDelegate(Folder folder);

		private delegate void ClearPolicyTagOnItemDelegate(StoreObject item);
	}
}
