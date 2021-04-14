using System;
using System.Security.AccessControl;
using Microsoft.Exchange.PST;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class PstDestinationFolder : PstFolder, IDestinationFolder, IFolder, IDisposable
	{
		bool IDestinationFolder.SetSearchCriteria(RestrictionData restriction, byte[][] entryIds, SearchCriteriaFlags flags)
		{
			return true;
		}

		PropProblemData[] IDestinationFolder.SetSecurityDescriptor(SecurityProp secProp, RawSecurityDescriptor sd)
		{
			throw new NotImplementedException();
		}

		IFxProxy IDestinationFolder.GetFxProxy(FastTransferFlags flags)
		{
			MrsTracer.Provider.Function("PstDestinationFolder.IDestinationFolder.GetFxProxy", new object[0]);
			return new PSTFxProxy(base.Folder);
		}

		void IDestinationFolder.SetReadFlagsOnMessages(SetReadFlags flags, byte[][] entryIds)
		{
			throw new NotImplementedException();
		}

		void IDestinationFolder.SetMessageProps(byte[] entryId, PropValueData[] propValues)
		{
			throw new NotImplementedException();
		}

		void IDestinationFolder.SetRules(RuleData[] rules)
		{
			MrsTracer.Provider.Function("PstDestinationFolder.IDestinationFolder.SetRules", new object[0]);
		}

		void IDestinationFolder.SetACL(SecurityProp secProp, PropValueData[][] aclData)
		{
			throw new NotImplementedException();
		}

		void IDestinationFolder.SetExtendedAcl(AclFlags aclFlags, PropValueData[][] aclData)
		{
			throw new NotImplementedException();
		}

		void IDestinationFolder.Flush()
		{
			try
			{
				base.Folder.PstMailbox.IPst.Flush();
			}
			catch (PSTExceptionBase innerException)
			{
				throw new UnableToCreatePSTMessagePermanentException(base.Folder.PstMailbox.IPst.FileName, innerException);
			}
		}

		Guid IDestinationFolder.LinkMailPublicFolder(LinkMailPublicFolderFlags flags, byte[] objectId)
		{
			throw new NotImplementedException();
		}
	}
}
