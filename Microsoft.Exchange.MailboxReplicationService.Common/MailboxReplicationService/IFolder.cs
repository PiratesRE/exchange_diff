using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal interface IFolder : IDisposable
	{
		FolderRec GetFolderRec(PropTag[] additionalPtagsToLoad, GetFolderRecFlags flags);

		byte[] GetFolderId();

		List<MessageRec> EnumerateMessages(EnumerateMessagesFlags emFlags, PropTag[] additionalPtagsToLoad);

		List<MessageRec> LookupMessages(PropTag ptagToLookup, List<byte[]> keysToLookup, PropTag[] additionalPtagsToLoad);

		RawSecurityDescriptor GetSecurityDescriptor(SecurityProp secProp);

		void SetContentsRestriction(RestrictionData restriction);

		void DeleteMessages(byte[][] entryIds);

		PropValueData[] GetProps(PropTag[] pta);

		void GetSearchCriteria(out RestrictionData restriction, out byte[][] entryIds, out SearchState state);

		RuleData[] GetRules(PropTag[] extraProps);

		PropValueData[][] GetACL(SecurityProp secProp);

		PropValueData[][] GetExtendedAcl(AclFlags aclFlags);

		PropProblemData[] SetProps(PropValueData[] pva);
	}
}
