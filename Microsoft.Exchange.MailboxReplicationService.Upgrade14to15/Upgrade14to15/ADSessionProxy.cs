using System;
using System.Collections.Generic;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ADSessionProxy : IADSession
	{
		public ADSessionProxy(IAnchorADProvider anchorAdProvider)
		{
			this.anchorAdProvider = anchorAdProvider;
		}

		public IEnumerable<RecipientWrapper> FindPagedMiniRecipient(UpgradeBatchCreatorScheduler.MailboxType mailboxType, ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize, IEnumerable<PropertyDefinition> properties)
		{
			ADPagedReader<MiniRecipient> pagedReader = this.anchorAdProvider.FindPagedMiniRecipient<MiniRecipient>(rootId, scope, filter, sortBy, pageSize, properties);
			foreach (MiniRecipient recipient in pagedReader)
			{
				RequestStatus moveStatus = (RequestStatus)recipient[SharedPropertyDefinitions.MailboxMoveStatus];
				string moveBatchName = (string)recipient[SharedPropertyDefinitions.MailboxMoveBatchName];
				RequestFlags requestFlags = (RequestFlags)recipient[SharedPropertyDefinitions.MailboxMoveFlags];
				RecipientWrapper wrappedRecipient = new RecipientWrapper(recipient.Id, moveStatus, moveBatchName, requestFlags, recipient.RecipientType, recipient.RecipientTypeDetails);
				yield return wrappedRecipient;
			}
			yield break;
		}

		public IEnumerable<RecipientWrapper> FindPilotUsersADRawEntry(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize, IEnumerable<PropertyDefinition> properties)
		{
			ADPagedReader<ADRawEntry> pagedReader = this.anchorAdProvider.FindPagedADRawEntry(rootId, scope, filter, sortBy, pageSize, properties);
			foreach (ADRawEntry adRawEntry in pagedReader)
			{
				RecipientWrapper wrappedRecipient = new RecipientWrapper(adRawEntry.Id, RequestStatus.None, null, RequestFlags.None, RecipientType.UserMailbox, RecipientTypeDetails.None);
				yield return wrappedRecipient;
			}
			yield break;
		}

		public QueryFilter BuildE14MailboxQueryFilter()
		{
			return CommonUtils.BuildMbxFilter(((AnchorADProvider)this.anchorAdProvider).ConfigurationSession, Server.E14MinVersion, Server.E15MinVersion);
		}

		private readonly IAnchorADProvider anchorAdProvider;
	}
}
