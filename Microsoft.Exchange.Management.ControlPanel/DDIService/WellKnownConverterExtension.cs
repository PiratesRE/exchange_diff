using System;
using System.Windows.Markup;

namespace Microsoft.Exchange.Management.DDIService
{
	public class WellKnownConverterExtension : MarkupExtension
	{
		public WellKnownConverterExtension(string converterType)
		{
			this.ConverterType = (WellKnownConverterType)Enum.Parse(typeof(WellKnownConverterType), converterType);
		}

		public WellKnownConverterType ConverterType { get; set; }

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			switch (this.ConverterType)
			{
			case WellKnownConverterType.PasswordInput:
				return new SecureStringInputConverter();
			case WellKnownConverterType.ToString:
				return new ToStringConverter(ConvertMode.PerItemInEnumerable);
			default:
				throw new NotImplementedException();
			}
		}
	}
}
