using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class ToRecipientsProperty : RecipientsPropertyBase
	{
		public ToRecipientsProperty(CommandContext commandContext) : base(commandContext, RecipientItemType.To)
		{
		}

		public static ToRecipientsProperty CreateCommand(CommandContext commandContext)
		{
			return new ToRecipientsProperty(commandContext);
		}
	}
}
