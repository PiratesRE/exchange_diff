using System;
using System.Security;
using System.Web;
using Microsoft.Exchange.Configuration.Authorization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal abstract class EcpCmdletQueryProcessor : RbacQuery.RbacQueryProcessor
	{
		public sealed override bool? TryIsInRole(ExchangeRunspaceConfiguration rbacConfiguration)
		{
			bool? flag = null;
			if (HttpContext.Current.Items.Contains(base.GetType()))
			{
				flag = (bool?)HttpContext.Current.Items[base.GetType()];
			}
			else
			{
				try
				{
					flag = this.IsInRoleCmdlet(rbacConfiguration);
				}
				catch (SecurityException)
				{
					flag = null;
				}
				HttpContext.Current.Items[base.GetType()] = flag;
			}
			return flag;
		}

		internal abstract bool? IsInRoleCmdlet(ExchangeRunspaceConfiguration rbacConfiguration);

		internal void LogCmdletError(PowerShellResults results, string roleName)
		{
			string text = string.Empty;
			if (results.ErrorRecords.Length > 0)
			{
				text = results.ErrorRecords[0].ToString();
			}
			EcpEventLogConstants.Tuple_UnableToDetectRbacRoleViaCmdlet.LogEvent(new object[]
			{
				EcpEventLogExtensions.GetUserNameToLog(),
				roleName,
				text
			});
		}

		public sealed override bool CanCache
		{
			get
			{
				return false;
			}
		}
	}
}
