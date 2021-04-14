using System;
using System.Windows.Markup;

namespace Microsoft.Exchange.Management.DDIService
{
	public class ResolverExtension : MarkupExtension
	{
		public ResolverExtension(string resolverType)
		{
			this.resolverType = (ResolverType)Enum.Parse(typeof(ResolverType), resolverType);
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return new DDIResolver(this.resolverType);
		}

		private ResolverType resolverType;
	}
}
