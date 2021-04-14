using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class TextBodyProperty : BodyProperty
	{
		public TextBodyProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public new static TextBodyProperty CreateCommand(CommandContext commandContext)
		{
			return new TextBodyProperty(commandContext);
		}

		protected override BodyFormat ComputeBodyFormat(BodyResponseType bodyType, Item item)
		{
			return BodyFormat.TextPlain;
		}
	}
}
