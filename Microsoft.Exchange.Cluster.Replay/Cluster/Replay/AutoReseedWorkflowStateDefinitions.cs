using System;
using Microsoft.Exchange.Cluster.Common.ConfigurableParameters;
using Microsoft.Exchange.Cluster.Common.Extensions;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class AutoReseedWorkflowStateDefinitions : ConfigurableParameterDefinitionsBase
	{
		private AutoReseedWorkflowStateDefinitions() : base(10, Assert.Instance)
		{
		}

		protected override void DefineParameters()
		{
			base.DefineParameter(new StringConfigurableParameter("AssignedVolumeName", string.Empty, 0, 2));
			base.DefineParameter(new EnumConfigurableParameter<ReseedState>("LastReseedRecoveryAction", ReseedState.Unknown, 0, 2));
			base.DefineParameter(new BooleanConfigurableParameter("IgnoreInPlaceOverwriteDelay", false, 0, 2));
			base.DefineParameter(new Int32ConfigurableParameter("ReseedRecoveryActionRetryCount", 0, 0, 2, null, null));
			base.DefineParameter(new StringConfigurableParameter("WorkflowExecutionError", string.Empty, 0, 2));
			base.DefineParameter(new EnumConfigurableParameter<AutoReseedWorkflowExecutionResult>("WorkflowExecutionResult2", AutoReseedWorkflowExecutionResult.Unknown, 0, 2));
			base.DefineParameter(new DateTimeConfigurableParameter("WorkflowExecutionTime", DateTime.MinValue, 0, 2));
			base.DefineParameter(new DateTimeConfigurableParameter("WorkflowNextExecutionTime", DateTime.MinValue, 0, 2));
			base.DefineParameter(new BooleanConfigurableParameter("WorkflowInfoIsValid", false, 0, 2));
		}

		private const int ParameterCapacity = 10;

		public static AutoReseedWorkflowStateDefinitions Instance = new AutoReseedWorkflowStateDefinitions();
	}
}
