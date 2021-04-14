using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class MoveRequestUserSchema : MailEnabledOrgPersonSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<ADUserSchema>();
		}

		public static readonly ADPropertyDefinition ExchangeGuid = ADMailboxRecipientSchema.ExchangeGuid;

		public static readonly ADPropertyDefinition SourceDatabase = ADUserSchema.MailboxMoveSourceMDB;

		public static readonly ADPropertyDefinition TargetDatabase = ADUserSchema.MailboxMoveTargetMDB;

		public static readonly ADPropertyDefinition SourceArchiveDatabase = ADUserSchema.MailboxMoveSourceArchiveMDB;

		public static readonly ADPropertyDefinition TargetArchiveDatabase = ADUserSchema.MailboxMoveTargetArchiveMDB;

		public static readonly ADPropertyDefinition Flags = ADUserSchema.MailboxMoveFlags;

		public static readonly ADPropertyDefinition RemoteHostName = ADUserSchema.MailboxMoveRemoteHostName;

		public static readonly ADPropertyDefinition BatchName = ADUserSchema.MailboxMoveBatchName;

		public static readonly ADPropertyDefinition Status = ADUserSchema.MailboxMoveStatus;
	}
}
