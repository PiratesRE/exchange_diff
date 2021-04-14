using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.Management.DDIService
{
	[AttributeUsage(AttributeTargets.Class)]
	public class DDIWorkflowAttribute : DDIValidateAttribute
	{
		public DDIWorkflowAttribute() : base("DDIWorkflowAttribute")
		{
		}

		public override List<string> Validate(object target, Service profile)
		{
			List<string> list = new List<string>();
			if (target != null)
			{
				Workflow workflow = target as Workflow;
				if (workflow == null)
				{
					throw new ArgumentException("DDIWorkflowAttribute can only be applied to Workflow object");
				}
				if (target is GetObjectWorkflow || target is GetSDOWorkflow || target is GetObjectForNewWorkflow)
				{
					if ((from c in workflow.Activities
					where c is CmdletActivity
					select c).Any((Activity c) => !(c is GetCmdlet) && !(c is BackLinkResolver)))
					{
						list.Add(string.Format("Only the activity GetCmdlet and BackLinkResolver can be added to GetObjectForNewWorkflow.", target));
					}
				}
				if (target is GetListWorkflow)
				{
					if ((from c in workflow.Activities
					where c is CmdletActivity
					select c).Any((Activity c) => !(c is GetListCmdlet) && (!(c is GetCmdlet) || !((GetCmdlet)c).SingletonObject)))
					{
						list.Add(string.Format("Only the activity GetListCmdlet can be added to GetListWorkflow.", target));
					}
					foreach (Activity activity in workflow.Activities)
					{
						GetListCmdlet getListCmdlet = activity as GetListCmdlet;
						if (getListCmdlet != null && getListCmdlet.Parameters.Contains(new Parameter
						{
							Name = "ResultSize"
						}))
						{
							list.Add(string.Format("You should set the ResultSize property to the GetListWorkflow instead of setting it to activity!", target));
							break;
						}
					}
				}
				if (target is BulkEditWorkflow)
				{
					BulkEditWorkflow workflow2 = target as BulkEditWorkflow;
					if (!this.IsBulkEditWorkflowRequiringNonPipelineActivity(workflow2) && (workflow.Activities.Count<Activity>() != 1 || !(workflow.Activities[0] is PipelineCmdlet)))
					{
						list.Add(string.Format("Only the single PipelineCmdlet can be added to BulkEditWorkflow.", target));
					}
					if (!string.IsNullOrEmpty(workflow.Output))
					{
						list.Add(string.Format("You cannot specify Output value to BulkEditWorkflow.", target));
					}
				}
				if (workflow.AsyncMode != AsyncMode.SynchronousOnly && !typeof(ProgressCalculatorBase).IsAssignableFrom(workflow.ProgressCalculator))
				{
					list.Add(string.Format("A valid ProgressCalculator type must be specified for async running workflow.", target));
				}
			}
			return list;
		}

		private bool IsBulkEditWorkflowRequiringNonPipelineActivity(BulkEditWorkflow workflow)
		{
			string name = workflow.Name;
			return string.Equals(name, "BulkRemoveDelegationWorkflow", StringComparison.OrdinalIgnoreCase) || string.Equals(name, "BulkAddDelegationWorkflow", StringComparison.OrdinalIgnoreCase);
		}
	}
}
