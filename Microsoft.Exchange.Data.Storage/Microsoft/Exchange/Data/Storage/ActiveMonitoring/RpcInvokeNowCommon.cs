using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.ActiveMonitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class RpcInvokeNowCommon
	{
		[Serializable]
		public class Request
		{
			public Request(string identity, string propertyBag, string extensionAttributes)
			{
				this.MonitorIdentity = identity;
				this.PropertyBag = propertyBag;
				this.ExtensionAttributes = extensionAttributes;
			}

			public string MonitorIdentity { get; set; }

			public string PropertyBag { get; set; }

			public string ExtensionAttributes { get; set; }
		}
	}
}
