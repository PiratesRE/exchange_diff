using System;
using System.Windows.Markup;

namespace Microsoft.Exchange.Management.DDIService
{
	public class ToStringArrayExtension : MarkupExtension
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return new DDIToStringArrayConverter(this.Property);
		}

		public string Property { get; set; }
	}
}
