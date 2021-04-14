using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.CompliancePolicy.ComplianceTask;
using Microsoft.Office.CompliancePolicy.Dar;

namespace Microsoft.Exchange.Data.Storage.UnifiedPolicy
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ExPolicyConfigChangeHandler : PolicyConfigChangeEventHandler
	{
		private ExPolicyConfigChangeHandler() : base(Activator.CreateInstance(ExPolicyConfigChangeHandler.serviceProviderType) as DarServiceProvider)
		{
		}

		public static ExPolicyConfigChangeHandler Current
		{
			get
			{
				return ExPolicyConfigChangeHandler.current;
			}
		}

		private static Type serviceProviderType = Type.GetType("Microsoft.Office.CompliancePolicy.Exchange.Dar.ExDarServiceProvider, Microsoft.Office.CompliancePolicy.Exchange.Dar");

		private static ExPolicyConfigChangeHandler current = new ExPolicyConfigChangeHandler();
	}
}
