using System;
using System.Security.AccessControl;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class DestinationFolderWrapper : FolderWrapper, IDestinationFolder, IFolder, IDisposable
	{
		public DestinationFolderWrapper(IDestinationFolder folder, CommonUtils.CreateContextDelegate createContext) : base(folder, createContext)
		{
		}

		PropProblemData[] IDestinationFolder.SetSecurityDescriptor(SecurityProp secProp, RawSecurityDescriptor sd)
		{
			PropProblemData[] result = null;
			base.CreateContext("IDestinationFolder.SetSecurityDescriptor", new DataContext[]
			{
				new SimpleValueDataContext("SecProp", secProp),
				new SimpleValueDataContext("SD", CommonUtils.GetSDDLString(sd))
			}).Execute(delegate
			{
				result = ((IDestinationFolder)this.WrappedObject).SetSecurityDescriptor(secProp, sd);
			}, true);
			return result;
		}

		bool IDestinationFolder.SetSearchCriteria(RestrictionData restriction, byte[][] entryIds, SearchCriteriaFlags flags)
		{
			bool result = false;
			base.CreateContext("IDestinationFolder.SetSearchCriteria", new DataContext[]
			{
				new RestrictionDataContext(restriction),
				new EntryIDsDataContext(entryIds),
				new SimpleValueDataContext("Flags", flags)
			}).Execute(delegate
			{
				result = ((IDestinationFolder)this.WrappedObject).SetSearchCriteria(restriction, entryIds, flags);
			}, true);
			return result;
		}

		IFxProxy IDestinationFolder.GetFxProxy(FastTransferFlags flags)
		{
			IFxProxy result = null;
			base.CreateContext("IDestinationFolder.GetFxProxy", new DataContext[0]).Execute(delegate
			{
				result = ((IDestinationFolder)this.WrappedObject).GetFxProxy(flags);
			}, true);
			if (result == null)
			{
				return null;
			}
			return new FxProxyWrapper(result, base.CreateContext);
		}

		void IDestinationFolder.SetReadFlagsOnMessages(SetReadFlags flags, byte[][] entryIds)
		{
			base.CreateContext("IDestinationFolder.SetReadFlagsOnMessages", new DataContext[]
			{
				new SimpleValueDataContext("Flags", flags),
				new EntryIDsDataContext(entryIds)
			}).Execute(delegate
			{
				((IDestinationFolder)this.WrappedObject).SetReadFlagsOnMessages(flags, entryIds);
			}, true);
		}

		void IDestinationFolder.SetMessageProps(byte[] entryId, PropValueData[] propValues)
		{
			base.CreateContext("IDestinationFolder.SetMessageProps", new DataContext[]
			{
				new EntryIDsDataContext(entryId)
			}).Execute(delegate
			{
				((IDestinationFolder)this.WrappedObject).SetMessageProps(entryId, propValues);
			}, true);
		}

		void IDestinationFolder.SetRules(RuleData[] rules)
		{
			base.CreateContext("IDestinationFolder.SetRules", new DataContext[]
			{
				new RulesDataContext(rules)
			}).Execute(delegate
			{
				((IDestinationFolder)this.WrappedObject).SetRules(rules);
			}, true);
		}

		void IDestinationFolder.SetACL(SecurityProp secProp, PropValueData[][] aclData)
		{
			base.CreateContext("IDestinationFolder.SetACL", new DataContext[]
			{
				new SimpleValueDataContext("SecProp", secProp),
				new PropValuesDataContext(aclData)
			}).Execute(delegate
			{
				((IDestinationFolder)this.WrappedObject).SetACL(secProp, aclData);
			}, true);
		}

		void IDestinationFolder.SetExtendedAcl(AclFlags aclFlags, PropValueData[][] aclData)
		{
			base.CreateContext("IDestinationFolder.SetExtendedAcl", new DataContext[]
			{
				new SimpleValueDataContext("AclFlags", aclFlags),
				new PropValuesDataContext(aclData)
			}).Execute(delegate
			{
				((IDestinationFolder)this.WrappedObject).SetExtendedAcl(aclFlags, aclData);
			}, true);
		}

		void IDestinationFolder.Flush()
		{
			base.CreateContext("IDestinationFolder.Flush", new DataContext[0]).Execute(delegate
			{
				((IDestinationFolder)base.WrappedObject).Flush();
			}, true);
		}

		Guid IDestinationFolder.LinkMailPublicFolder(LinkMailPublicFolderFlags flags, byte[] objectId)
		{
			Guid result = Guid.Empty;
			base.CreateContext("IDestinationFolder.LinkMailPublicFolder", new DataContext[]
			{
				new SimpleValueDataContext("Flags", flags),
				new EntryIDsDataContext(objectId)
			}).Execute(delegate
			{
				result = ((IDestinationFolder)this.WrappedObject).LinkMailPublicFolder(flags, objectId);
			}, true);
			return result;
		}
	}
}
