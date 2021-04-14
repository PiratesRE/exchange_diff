using System;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Management.Supervision;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class ClosedCampusQueryProcessor : RbacQuery.RbacQueryProcessor
	{
		public override bool? TryIsInRole(ExchangeRunspaceConfiguration rbacConfiguration)
		{
			bool? result;
			try
			{
				PSLocalTask<GetSupervisionPolicy, SupervisionPolicy> taskWrapper = CmdletTaskFactory.Instance.CreateGetSupervisionPolicyTask(CallContext.Current.AccessingPrincipal);
				CmdletRunner<GetSupervisionPolicy, SupervisionPolicy> cmdletRunner = new CmdletRunner<GetSupervisionPolicy, SupervisionPolicy>(CallContext.Current, "Get-SupervisionPolicy", ScopeLocation.RecipientRead, taskWrapper);
				cmdletRunner.Execute();
				foreach (SupervisionPolicy supervisionPolicy in cmdletRunner.TaskAllResults)
				{
					if (supervisionPolicy.ClosedCampusInboundPolicyEnabled && supervisionPolicy.ClosedCampusOutboundPolicyEnabled)
					{
						return new bool?(true);
					}
				}
				result = new bool?(false);
			}
			catch (CmdletException ex)
			{
				if (ex.ErrorCode == OptionsActionError.PermissionDenied)
				{
					result = new bool?(false);
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		internal const string RoleName = "ClosedCampus";
	}
}
