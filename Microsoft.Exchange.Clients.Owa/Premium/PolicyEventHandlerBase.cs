using System;
using System.Globalization;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	internal abstract class PolicyEventHandlerBase : OwaEventHandlerBase
	{
		protected PolicyEventHandlerBase(PolicyProvider policyProvider)
		{
			this.policyProvider = policyProvider;
		}

		[OwaEventParameter("id", typeof(OwaStoreObjectId), false, true)]
		[OwaEvent("GetPolicyMenu")]
		[OwaEventVerb(OwaEventVerb.Get)]
		public void GetPolicyMenu()
		{
			if (!this.policyProvider.IsPolicyEnabled(base.UserContext.MailboxSession))
			{
				throw new OwaInvalidRequestException("The mailbox is not enabled for " + this.policyProvider.ToString());
			}
			OwaStoreObjectId owaStoreObjectId = base.GetParameter("id") as OwaStoreObjectId;
			PolicyContextMenuBase policyMenu = this.InternalGetPolicyMenu(ref owaStoreObjectId);
			if (owaStoreObjectId != null && !owaStoreObjectId.IsConversationId)
			{
				this.DoPolicy((MailboxSession)owaStoreObjectId.GetSession(base.UserContext), owaStoreObjectId.StoreObjectId, true, delegate(StoreObject storeObject)
				{
					bool isInherited;
					Guid? policyTag = this.policyProvider.GetPolicyTag(storeObject, out isInherited);
					policyMenu.SetStates(isInherited, policyTag);
				});
			}
			policyMenu.Render(this.Writer);
		}

		protected abstract PolicyContextMenuBase InternalGetPolicyMenu(ref OwaStoreObjectId itemId);

		[OwaEventParameter("tag", typeof(string), false, false)]
		[OwaEventParameter("id", typeof(OwaStoreObjectId), true)]
		[OwaEventParameter("fId", typeof(OwaStoreObjectId), false, true)]
		[OwaEvent("ApplyPolicy")]
		public void ApplyPolicy()
		{
			OwaStoreObjectId[] itemIdsFromParameter = this.GetItemIdsFromParameter();
			Guid tagGuid = new Guid((string)base.GetParameter("tag"));
			MailboxSession mailboxSession = (MailboxSession)itemIdsFromParameter[0].GetSession(base.UserContext);
			foreach (OwaStoreObjectId owaStoreObjectId in itemIdsFromParameter)
			{
				this.DoPolicy(mailboxSession, owaStoreObjectId.StoreObjectId, false, delegate(StoreObject storeObject)
				{
					this.policyProvider.ApplyPolicyTag(storeObject, tagGuid);
				});
			}
		}

		[OwaEventParameter("id", typeof(OwaStoreObjectId), true)]
		[OwaEventParameter("fId", typeof(OwaStoreObjectId), false, true)]
		[OwaEvent("InheritPolicy")]
		public void InheritPolicy()
		{
			OwaStoreObjectId[] itemIdsFromParameter = this.GetItemIdsFromParameter();
			MailboxSession mailboxSession = (MailboxSession)itemIdsFromParameter[0].GetSession(base.UserContext);
			foreach (OwaStoreObjectId owaStoreObjectId in itemIdsFromParameter)
			{
				this.DoPolicy(mailboxSession, owaStoreObjectId.StoreObjectId, false, delegate(StoreObject storeObject)
				{
					this.policyProvider.ClearPolicyTag(storeObject);
				});
			}
		}

		private OwaStoreObjectId[] GetItemIdsFromParameter()
		{
			OwaStoreObjectId[] array = (OwaStoreObjectId[])base.GetParameter("id");
			if (ConversationUtilities.ContainsConversationItem(base.UserContext, array))
			{
				OwaStoreObjectId localFolderId = (OwaStoreObjectId)base.GetParameter("fId");
				array = ConversationUtilities.GetLocalItemIds(base.UserContext, array, localFolderId);
			}
			if (array.Length > 500)
			{
				throw new OwaInvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Setting move policy on {0} item(s) in a single request is not supported", new object[]
				{
					array.Length
				}));
			}
			return array;
		}

		private void DoPolicy(MailboxSession mailboxSession, StoreObjectId storeObjectId, bool readOnly, PolicyEventHandlerBase.PolicyDelegate policyDelegate)
		{
			if (storeObjectId.IsFolderId)
			{
				this.DoPolicyOnFolder(mailboxSession, storeObjectId, readOnly, policyDelegate);
				return;
			}
			this.DoPolicyOnItem(mailboxSession, storeObjectId, readOnly, policyDelegate);
		}

		private void DoPolicyOnFolder(MailboxSession mailboxSession, StoreObjectId folderId, bool readOnly, PolicyEventHandlerBase.PolicyDelegate policyDelegate)
		{
			using (Folder folder = this.policyProvider.OpenFolderForPolicyTag(mailboxSession, folderId))
			{
				policyDelegate(folder);
				if (!readOnly)
				{
					folder.Save();
				}
			}
		}

		private void DoPolicyOnItem(MailboxSession mailboxSession, StoreObjectId messageId, bool readOnly, PolicyEventHandlerBase.PolicyDelegate policyDelegate)
		{
			using (Item item = this.policyProvider.OpenItemForPolicyTag(mailboxSession, messageId))
			{
				if (!readOnly)
				{
					item.OpenAsReadWrite();
				}
				policyDelegate(item);
				if (!readOnly)
				{
					item.Save(SaveMode.NoConflictResolution);
				}
			}
		}

		public const int MaxSelectionSize = 500;

		public const string MethodGetPolicyMenu = "GetPolicyMenu";

		public const string MethodApplyPolicy = "ApplyPolicy";

		public const string MethodInheritPolicy = "InheritPolicy";

		public const string Id = "id";

		public const string FolderId = "fId";

		public const string TagGuid = "tag";

		private PolicyProvider policyProvider;

		private delegate void PolicyDelegate(StoreObject storeObject);
	}
}
