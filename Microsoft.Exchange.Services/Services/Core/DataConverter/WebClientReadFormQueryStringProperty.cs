using System;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class WebClientReadFormQueryStringProperty : WebClientQueryStringPropertyBase, IToXmlCommand, IToXmlForPropertyBagCommand, IPropertyCommand
	{
		private WebClientReadFormQueryStringProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static WebClientReadFormQueryStringProperty CreateCommand(CommandContext commandContext)
		{
			return new WebClientReadFormQueryStringProperty(commandContext);
		}
	}
}
