using System;
using Microsoft.Exchange.Cluster.Common.ConfigurableParameters;
using Microsoft.Exchange.Cluster.Common.Extensions;

namespace Microsoft.Exchange.Cluster.Shared
{
	internal class RegistryParameterValues : ConfigurableParameterAccessorBase
	{
		public RegistryParameterValues() : base(RegistryParameterDefinitions.Instance, Assert.Instance)
		{
			base.LoadInitialValues();
		}

		protected override StateAccessor GetStateAccessor()
		{
			return new RegistryStateAccess("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\Parameters");
		}
	}
}
