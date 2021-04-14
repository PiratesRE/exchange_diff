using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal abstract class FolderWrapper : WrapperBase<IFolder>, IFolder, IDisposable
	{
		public FolderWrapper(IFolder folder, CommonUtils.CreateContextDelegate createContext) : base(folder, createContext)
		{
		}

		FolderRec IFolder.GetFolderRec(PropTag[] additionalPtagsToLoad, GetFolderRecFlags flags)
		{
			FolderRec result = null;
			base.CreateContext("IFolder.GetFolderRec", new DataContext[]
			{
				new PropTagsDataContext(additionalPtagsToLoad),
				new SimpleValueDataContext("Flags", flags)
			}).Execute(delegate
			{
				result = this.WrappedObject.GetFolderRec(additionalPtagsToLoad, flags);
			}, true);
			return result;
		}

		byte[] IFolder.GetFolderId()
		{
			byte[] result = null;
			base.CreateContext("IFolder.GetFolderID", new DataContext[0]).Execute(delegate
			{
				result = this.WrappedObject.GetFolderId();
			}, true);
			return result;
		}

		List<MessageRec> IFolder.EnumerateMessages(EnumerateMessagesFlags emFlags, PropTag[] additionalPtagsToLoad)
		{
			List<MessageRec> result = null;
			base.CreateContext("IFolder.EnumerateMessages", new DataContext[]
			{
				new SimpleValueDataContext("Flags", emFlags),
				new PropTagsDataContext(additionalPtagsToLoad)
			}).Execute(delegate
			{
				result = this.WrappedObject.EnumerateMessages(emFlags, additionalPtagsToLoad);
			}, true);
			return result;
		}

		List<MessageRec> IFolder.LookupMessages(PropTag ptagToLookup, List<byte[]> keysToLookup, PropTag[] additionalPtagsToLoad)
		{
			List<MessageRec> result = null;
			base.CreateContext("IFolder.LookupMessages", new DataContext[]
			{
				new SimpleValueDataContext("PropTag", ptagToLookup),
				new EntryIDsDataContext(keysToLookup),
				new PropTagsDataContext(additionalPtagsToLoad)
			}).Execute(delegate
			{
				result = this.WrappedObject.LookupMessages(ptagToLookup, keysToLookup, additionalPtagsToLoad);
			}, true);
			return result;
		}

		RawSecurityDescriptor IFolder.GetSecurityDescriptor(SecurityProp secProp)
		{
			RawSecurityDescriptor result = null;
			base.CreateContext("IFolder.GetSecurityDescriptor", new DataContext[]
			{
				new SimpleValueDataContext("SecProp", secProp)
			}).Execute(delegate
			{
				result = this.WrappedObject.GetSecurityDescriptor(secProp);
			}, true);
			return result;
		}

		void IFolder.SetContentsRestriction(RestrictionData restriction)
		{
			base.CreateContext("IFolder.SetContentsRestriction", new DataContext[]
			{
				new RestrictionDataContext(restriction)
			}).Execute(delegate
			{
				this.WrappedObject.SetContentsRestriction(restriction);
			}, true);
		}

		void IFolder.DeleteMessages(byte[][] entryIds)
		{
			base.CreateContext("IFolder.DeleteMessages", new DataContext[]
			{
				new EntryIDsDataContext(entryIds)
			}).Execute(delegate
			{
				this.WrappedObject.DeleteMessages(entryIds);
			}, true);
		}

		PropValueData[] IFolder.GetProps(PropTag[] pta)
		{
			PropValueData[] result = null;
			base.CreateContext("IFolder.GetProps", new DataContext[]
			{
				new PropTagsDataContext(pta)
			}).Execute(delegate
			{
				result = this.WrappedObject.GetProps(pta);
			}, true);
			return result;
		}

		void IFolder.GetSearchCriteria(out RestrictionData restriction, out byte[][] entryIds, out SearchState state)
		{
			RestrictionData restrictionInt = null;
			byte[][] entryIdsInt = null;
			SearchState stateInt = SearchState.None;
			base.CreateContext("IFolder.GetSearchCriteria", new DataContext[0]).Execute(delegate
			{
				this.WrappedObject.GetSearchCriteria(out restrictionInt, out entryIdsInt, out stateInt);
			}, true);
			restriction = restrictionInt;
			entryIds = entryIdsInt;
			state = stateInt;
		}

		RuleData[] IFolder.GetRules(PropTag[] extraProps)
		{
			RuleData[] result = null;
			base.CreateContext("IFolder.GetRules", new DataContext[]
			{
				new PropTagsDataContext(extraProps)
			}).Execute(delegate
			{
				result = this.WrappedObject.GetRules(extraProps);
			}, true);
			return result;
		}

		PropValueData[][] IFolder.GetACL(SecurityProp secProp)
		{
			PropValueData[][] result = null;
			base.CreateContext("IFolder.GetACL", new DataContext[]
			{
				new SimpleValueDataContext("SecProp", secProp)
			}).Execute(delegate
			{
				result = this.WrappedObject.GetACL(secProp);
			}, true);
			return result;
		}

		PropValueData[][] IFolder.GetExtendedAcl(AclFlags aclFlags)
		{
			PropValueData[][] result = null;
			base.CreateContext("IFolder.GetExtendedAcl", new DataContext[]
			{
				new SimpleValueDataContext("AclFlags", aclFlags)
			}).Execute(delegate
			{
				result = this.WrappedObject.GetExtendedAcl(aclFlags);
			}, true);
			return result;
		}

		PropProblemData[] IFolder.SetProps(PropValueData[] pva)
		{
			PropProblemData[] result = null;
			base.CreateContext("IFolder.SetProps", new DataContext[]
			{
				new PropValuesDataContext(pva)
			}).Execute(delegate
			{
				result = this.WrappedObject.SetProps(pva);
			}, true);
			return result;
		}
	}
}
