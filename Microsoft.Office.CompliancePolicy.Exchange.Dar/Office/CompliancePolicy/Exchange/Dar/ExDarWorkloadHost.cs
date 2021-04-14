using System;
using Microsoft.Office.CompliancePolicy.Dar;

namespace Microsoft.Office.CompliancePolicy.Exchange.Dar
{
	internal class ExDarWorkloadHost : DarWorkloadHost
	{
		public ExDarWorkloadHost(DarServiceProvider provider)
		{
			this.provider = provider;
		}

		private DarServiceProvider provider;
	}
}
