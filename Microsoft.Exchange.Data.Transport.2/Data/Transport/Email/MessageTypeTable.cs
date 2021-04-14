using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Transport.Email
{
	internal class MessageTypeTable
	{
		internal static IEnumerable<MessageTypeEntry> Table
		{
			get
			{
				return MessageTypeTable.table;
			}
		}

		internal static MessageFlags GetMessageFlags(MessageType type)
		{
			MessageTypeEntry messageTypeEntry = MessageTypeTable.table[(int)type];
			return messageTypeEntry.MessageFlags;
		}

		internal static MessageSecurityType GetMessageSecurityType(MessageType type)
		{
			MessageTypeEntry messageTypeEntry = MessageTypeTable.table[(int)type];
			return messageTypeEntry.MessageSecurityType;
		}

		internal static MessageTypeEntry GetMessageTypeEntry(MessageType type)
		{
			return MessageTypeTable.table[(int)type];
		}

		private static MessageTypeEntry[] table = new MessageTypeEntry[]
		{
			new MessageTypeEntry(MessageType.Undefined, MessageFlags.None),
			new MessageTypeEntry(MessageType.Unknown, MessageFlags.KnownApplication),
			new MessageTypeEntry(MessageType.SingleAttachment, MessageFlags.Normal),
			new MessageTypeEntry(MessageType.MultipleAttachments, MessageFlags.Normal),
			new MessageTypeEntry(MessageType.Normal, MessageFlags.Normal),
			new MessageTypeEntry(MessageType.NormalWithRegularAttachments, MessageFlags.Normal),
			new MessageTypeEntry(MessageType.SummaryTnef, MessageFlags.Normal | MessageFlags.Tnef),
			new MessageTypeEntry(MessageType.LegacyTnef, MessageFlags.Normal | MessageFlags.Tnef),
			new MessageTypeEntry(MessageType.SuperLegacyTnef, MessageFlags.Normal | MessageFlags.Tnef),
			new MessageTypeEntry(MessageType.SuperLegacyTnefWithRegularAttachments, MessageFlags.Normal | MessageFlags.Tnef),
			new MessageTypeEntry(MessageType.Voice, MessageFlags.Normal | MessageFlags.KnownApplication),
			new MessageTypeEntry(MessageType.Fax, MessageFlags.KnownApplication),
			new MessageTypeEntry(MessageType.Journal, MessageFlags.System),
			new MessageTypeEntry(MessageType.Dsn, MessageFlags.System),
			new MessageTypeEntry(MessageType.Mdn, MessageFlags.System),
			new MessageTypeEntry(MessageType.MsRightsProtected, MessageFlags.KnownApplication, MessageSecurityType.Encrypted),
			new MessageTypeEntry(MessageType.Quota, MessageFlags.System),
			new MessageTypeEntry(MessageType.AdReplicationMessage, MessageFlags.System),
			new MessageTypeEntry(MessageType.PgpEncrypted, MessageFlags.KnownApplication, MessageSecurityType.Encrypted),
			new MessageTypeEntry(MessageType.SmimeSignedNormal, MessageFlags.KnownApplication, MessageSecurityType.ClearSigned),
			new MessageTypeEntry(MessageType.SmimeSignedUnknown, MessageFlags.KnownApplication, MessageSecurityType.ClearSigned),
			new MessageTypeEntry(MessageType.SmimeSignedEncrypted, MessageFlags.KnownApplication, MessageSecurityType.Encrypted),
			new MessageTypeEntry(MessageType.SmimeOpaqueSigned, MessageFlags.KnownApplication, MessageSecurityType.OpaqueSigned),
			new MessageTypeEntry(MessageType.SmimeEncrypted, MessageFlags.KnownApplication, MessageSecurityType.Encrypted),
			new MessageTypeEntry(MessageType.ApprovalInitiation, MessageFlags.System),
			new MessageTypeEntry(MessageType.UMPartner, MessageFlags.KnownApplication)
		};
	}
}
