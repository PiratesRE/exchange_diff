using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class CcRecipientsProperty : RecipientsPropertyBase
	{
		public CcRecipientsProperty(CommandContext commandContext) : base(commandContext, RecipientItemType.Cc)
		{
		}

		public static CcRecipientsProperty CreateCommand(CommandContext commandContext)
		{
			return new CcRecipientsProperty(commandContext);
		}
	}
}
