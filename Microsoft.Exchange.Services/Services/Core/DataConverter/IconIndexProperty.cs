using System;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class IconIndexProperty : SimpleProperty
	{
		private IconIndexProperty(CommandContext commandContext, BaseConverter converter) : base(commandContext, converter)
		{
		}

		public new static IconIndexProperty CreateCommand(CommandContext commandContext)
		{
			return new IconIndexProperty(commandContext, new IconIndexConverter());
		}
	}
}
