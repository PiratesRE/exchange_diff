using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Transport.Agent.InterceptorAgent;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "InterceptorRule", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class GetInterceptorRule : GetSystemConfigurationObjectTask<InterceptorRuleIdParameter, InterceptorRule>
	{
		public GetInterceptorRule()
		{
			base.Fields.ResetChangeTracking();
		}

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				return base.RootOrgContainerId.GetDescendantId(InterceptorRule.InterceptorRulesContainer);
			}
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			InterceptorRule interceptorRule = dataObject as InterceptorRule;
			InterceptorAgentRule interceptorAgentRule = null;
			try
			{
				interceptorAgentRule = InterceptorAgentRule.CreateRuleFromXml(interceptorRule.Xml);
			}
			catch (InvalidOperationException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidOperation, this.Identity);
				TaskLogger.LogExit();
				return;
			}
			catch (FormatException exception2)
			{
				base.WriteError(exception2, ErrorCategory.InvalidOperation, this.Identity);
				TaskLogger.LogExit();
				return;
			}
			interceptorAgentRule.SetPropertiesFromAdObjet(interceptorRule);
			base.WriteResult(interceptorAgentRule);
		}
	}
}
