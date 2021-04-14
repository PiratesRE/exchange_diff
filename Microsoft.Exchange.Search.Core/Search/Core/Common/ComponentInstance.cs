using System;

namespace Microsoft.Exchange.Search.Core.Common
{
	internal sealed class ComponentInstance
	{
		private ComponentInstance(Guid id, string name, string serviceName)
		{
			this.Id = id;
			this.NameX = name;
			this.ServiceName = serviceName;
		}

		internal Guid Id { get; private set; }

		internal string NameX { get; private set; }

		internal string ServiceName { get; private set; }

		internal static class Globals
		{
			internal static readonly ComponentInstance Search = new ComponentInstance(new Guid("decafbad-0001-40eb-9233-00219b403a32"), "Search", "MSExchangeFastSearch");
		}
	}
}
