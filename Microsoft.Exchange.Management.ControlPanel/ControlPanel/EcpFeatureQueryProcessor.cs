using System;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal sealed class EcpFeatureQueryProcessor : RbacQuery.RbacQueryProcessor, INamedQueryProcessor
	{
		public string Name { get; private set; }

		public EcpFeatureQueryProcessor(EcpFeature ecpFeature)
		{
			this.ecpFeature = ecpFeature;
			this.Name = ecpFeature.GetName();
		}

		public sealed override bool? TryIsInRole(ExchangeRunspaceConfiguration rbacConfiguration)
		{
			if (this.descriptor == null)
			{
				this.descriptor = this.ecpFeature.GetFeatureDescriptor();
			}
			bool value = LoginUtil.CheckUrlAccess(this.descriptor.ServerPath);
			return new bool?(value);
		}

		private readonly EcpFeature ecpFeature;

		private EcpFeatureDescriptor descriptor;
	}
}
