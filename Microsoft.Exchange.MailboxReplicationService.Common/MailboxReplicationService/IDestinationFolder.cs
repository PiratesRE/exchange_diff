using System;
using System.Security.AccessControl;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal interface IDestinationFolder : IFolder, IDisposable
	{
		PropProblemData[] SetSecurityDescriptor(SecurityProp secProp, RawSecurityDescriptor sd);

		bool SetSearchCriteria(RestrictionData restriction, byte[][] entryIds, SearchCriteriaFlags flags);

		IFxProxy GetFxProxy(FastTransferFlags flags);

		void SetReadFlagsOnMessages(SetReadFlags flags, byte[][] entryIds);

		void SetRules(RuleData[] rules);

		void SetACL(SecurityProp secProp, PropValueData[][] aclData);

		void SetExtendedAcl(AclFlags aclFlags, PropValueData[][] aclData);

		void Flush();

		Guid LinkMailPublicFolder(LinkMailPublicFolderFlags flags, byte[] objectId);

		void SetMessageProps(byte[] entryId, PropValueData[] propValues);
	}
}
