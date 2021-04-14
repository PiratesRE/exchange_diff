using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class BccRecipientsProperty : RecipientsPropertyBase
	{
		public BccRecipientsProperty(CommandContext commandContext) : base(commandContext, RecipientItemType.Bcc)
		{
		}

		public static BccRecipientsProperty CreateCommand(CommandContext commandContext)
		{
			return new BccRecipientsProperty(commandContext);
		}
	}
}
