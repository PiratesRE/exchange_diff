using System;
using System.Web.Compilation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class EcpResourceProviderFactory : ResourceProviderFactory
	{
		public override IResourceProvider CreateGlobalResourceProvider(string classKey)
		{
			return new EcpGlobalResourceProvider(classKey);
		}

		public override IResourceProvider CreateLocalResourceProvider(string virtualPath)
		{
			throw new NotSupportedException();
		}
	}
}
