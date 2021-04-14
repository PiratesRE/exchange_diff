using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using Microsoft.Exchange.Connections.Eas.Commands;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class EasSyntheticFolder : EasFolderBase, ISourceFolder, IDestinationFolder, IFolder, IDisposable
	{
		private EasSyntheticFolder(string serverId, string parentId, EasFolderType folderType, Func<EasFolderBase, FolderRec> createFolderRec) : base(serverId, parentId, folderType.ToString(), folderType)
		{
			this.FolderRec = createFolderRec(this);
		}

		internal FolderRec FolderRec { get; private set; }

		void ISourceFolder.CopyTo(IFxProxy fxFolderProxy, CopyPropertiesFlags flags, PropTag[] propTagsToExclude)
		{
		}

		void ISourceFolder.ExportMessages(IFxProxy destFolderProxy, CopyMessagesFlags flags, byte[][] entryIds)
		{
			throw new NotImplementedException();
		}

		FolderChangesManifest ISourceFolder.EnumerateChanges(EnumerateContentChangesFlags flags, int maxChanges)
		{
			return base.CreateInitializedChangesManifest();
		}

		List<MessageRec> ISourceFolder.EnumerateMessagesPaged(int maxPageSize)
		{
			return null;
		}

		int ISourceFolder.GetEstimatedItemCount()
		{
			return 0;
		}

		bool IDestinationFolder.SetSearchCriteria(RestrictionData restriction, byte[][] entryIds, SearchCriteriaFlags flags)
		{
			throw new NotImplementedException();
		}

		PropProblemData[] IDestinationFolder.SetSecurityDescriptor(SecurityProp secProp, RawSecurityDescriptor sd)
		{
			throw new NotImplementedException();
		}

		IFxProxy IDestinationFolder.GetFxProxy(FastTransferFlags flags)
		{
			throw new NotImplementedException();
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
			throw new NotImplementedException();
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
			throw new NotImplementedException();
		}

		Guid IDestinationFolder.LinkMailPublicFolder(LinkMailPublicFolderFlags flags, byte[] objectId)
		{
			throw new NotImplementedException();
		}

		protected override FolderRec InternalGetFolderRec(PropTag[] additionalPtagsToLoad, GetFolderRecFlags flags)
		{
			return this.FolderRec;
		}

		protected override List<MessageRec> InternalLookupMessages(PropTag ptagToLookup, List<byte[]> keysToLookup, PropTag[] additionalPtagsToLoad)
		{
			return EasSyntheticFolder.EmptyMessageRec;
		}

		private static FolderRec CreateRootFolderRec(EasFolderBase folder)
		{
			return new FolderRec(folder.EntryId, null, FolderType.Root, folder.DisplayName, DateTime.MinValue, null);
		}

		private const string RootServerId = "BBAA51E4-3863-42D1-9CE2-817B0DEEE67E";

		private const string IpmSubtreeServerId = "0";

		internal static readonly EasSyntheticFolder RootFolder = new EasSyntheticFolder("BBAA51E4-3863-42D1-9CE2-817B0DEEE67E", "BBAA51E4-3863-42D1-9CE2-817B0DEEE67E", EasFolderType.SyntheticRoot, new Func<EasFolderBase, FolderRec>(EasSyntheticFolder.CreateRootFolderRec));

		internal static readonly EasSyntheticFolder IpmSubtreeFolder = new EasSyntheticFolder("0", "BBAA51E4-3863-42D1-9CE2-817B0DEEE67E", EasFolderType.SyntheticIpmSubtree, new Func<EasFolderBase, FolderRec>(EasFolderBase.CreateGenericFolderRec));

		private static readonly List<MessageRec> EmptyMessageRec = new List<MessageRec>(0);
	}
}
