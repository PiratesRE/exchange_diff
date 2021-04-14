using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Exchange.Management.DDIService
{
	[AttributeUsage(AttributeTargets.Class)]
	public class DDIIsValidPipelineInnerActivityAttribute : DDIValidateAttribute
	{
		public DDIIsValidPipelineInnerActivityAttribute() : base("DDIIsValidPipelineInnerActivityAttribute")
		{
		}

		public override List<string> Validate(object target, Service profile)
		{
			if (target != null && !(target is Pipeline))
			{
				throw new ArgumentException("DDIIsValidPipelineInnerActivityAttribute can only apply to Pipeline class.");
			}
			Pipeline pipeline = target as Pipeline;
			Collection<Activity> body = pipeline.Body;
			List<string> list = new List<string>();
			for (int i = 0; i < body.Count; i++)
			{
				Activity activity = body[i];
				if (i < body.Count - 1)
				{
					if (!(activity is OutputObjectCmdlet) && !(activity is GetListCmdlet))
					{
						list.Add("Activity inside Pipeline can only be OutputObjectCmdlet or GetListCmdlet.");
					}
					else if (!string.IsNullOrEmpty(((CmdletActivity)activity).PreAction) || !string.IsNullOrEmpty(((CmdletActivity)activity).PostAction))
					{
						list.Add("Activity inside Pipeline can not have code behind.");
					}
				}
				else if (!string.IsNullOrEmpty(((CmdletActivity)activity).PreAction) || !string.IsNullOrEmpty(((CmdletActivity)activity).PostAction))
				{
					list.Add("Activity inside Pipeline can not have code behind.");
				}
			}
			return list;
		}
	}
}
