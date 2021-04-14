using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class MessageFlagsProperty : SimpleProperty
	{
		public MessageFlagsProperty(CommandContext commandContext, BaseConverter baseConverter) : base(commandContext, baseConverter)
		{
		}

		public static MessageFlagsProperty CreateIsUnmodifiedCommand(CommandContext commandContext)
		{
			return MessageFlagsProperty.CreateCommand(commandContext, MessageFlags.IsUnmodified);
		}

		public static MessageFlagsProperty CreateIsSubmittedCommand(CommandContext commandContext)
		{
			return MessageFlagsProperty.CreateCommand(commandContext, MessageFlags.HasBeenSubmitted);
		}

		public static MessageFlagsProperty CreateIsAssociatedCommand(CommandContext commandContext)
		{
			return MessageFlagsProperty.CreateCommand(commandContext, MessageFlags.IsAssociated);
		}

		public static MessageFlagsProperty CreateIsDraftCommand(CommandContext commandContext)
		{
			return MessageFlagsProperty.CreateCommand(commandContext, MessageFlags.IsDraft);
		}

		public static MessageFlagsProperty CreateIsFromMeCommand(CommandContext commandContext)
		{
			return MessageFlagsProperty.CreateCommand(commandContext, MessageFlags.IsFromMe);
		}

		public static MessageFlagsProperty CreateIsResendCommand(CommandContext commandContext)
		{
			return MessageFlagsProperty.CreateCommand(commandContext, MessageFlags.IsResend);
		}

		private static MessageFlagsProperty CreateCommand(CommandContext commandContext, MessageFlags flag)
		{
			MessageFlagsConverter baseConverter = new MessageFlagsConverter(flag);
			return new MessageFlagsProperty(commandContext, baseConverter);
		}
	}
}
