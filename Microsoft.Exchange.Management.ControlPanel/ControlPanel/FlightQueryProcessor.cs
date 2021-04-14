using System;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Office.CsmSdk;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class FlightQueryProcessor : RbacQuery.RbacQueryProcessor
	{
		public FlightQueryProcessor(string featureName)
		{
			this.featureName = featureName;
		}

		public override bool CanCache
		{
			get
			{
				return false;
			}
		}

		public override bool? TryIsInRole(ExchangeRunspaceConfiguration rbacConfiguration)
		{
			return new bool?(FlightProvider.Instance.IsFeatureEnabled(this.featureName));
		}

		private readonly string featureName;
	}
}
