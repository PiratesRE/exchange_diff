using System;
using System.Windows.Markup;

namespace Microsoft.Exchange.Management.DDIService
{
	public class TargetTypeExtension : MarkupExtension
	{
		public TargetTypeExtension(Type targetType)
		{
			this.targetType = targetType;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return new DDIStrongTypeConverter(this.targetType, this.ConvertMode);
		}

		public ConvertMode ConvertMode { get; set; }

		private Type targetType;
	}
}
