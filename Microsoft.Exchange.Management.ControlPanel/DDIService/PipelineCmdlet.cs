using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	public abstract class PipelineCmdlet : CmdletActivity
	{
		public PipelineCmdlet()
		{
		}

		protected PipelineCmdlet(PipelineCmdlet activity) : base(activity)
		{
			this.SingletonObject = activity.SingletonObject;
		}

		protected override void DoPreRunCore(DataRow input, DataTable dataTable, DataObjectStore store, Type codeBehind)
		{
			if (!this.SingletonObject && !(input[base.IdentityVariable] is Identity[]))
			{
				Parameter item = new Parameter
				{
					Name = base.IdentityName,
					Reference = base.IdentityVariable,
					Type = ParameterType.Mandatory
				};
				if (!base.Parameters.Contains(item))
				{
					base.Parameters.Add(item);
				}
			}
			base.DoPreRunCore(input, dataTable, store, codeBehind);
		}

		[DefaultValue(false)]
		public bool SingletonObject
		{
			get
			{
				return this.singletonObject;
			}
			set
			{
				if (value)
				{
					base.IdentityVariable = string.Empty;
				}
				this.singletonObject = value;
			}
		}

		public override RunResult Run(DataRow input, DataTable dataTable, DataObjectStore store, Type codeBehind, Workflow.UpdateTableDelegate updateTableDelegate)
		{
			Identity[] array = this.PreparePipelineExecution(input, dataTable);
			if (array == null || this.SingletonObject)
			{
				return base.Run(input, dataTable, store, codeBehind, updateTableDelegate);
			}
			if (array.Length == 0)
			{
				throw new InvalidOperationException();
			}
			RunResult runResult = new RunResult();
			PowerShellResults<PSObject> powerShellResults;
			base.ExecuteCmdlet(array, runResult, out powerShellResults, false);
			return runResult;
		}

		protected Identity[] PreparePipelineExecution(DataRow input, DataTable dataTable)
		{
			Identity[] array = null;
			if (!this.SingletonObject)
			{
				array = (input[base.IdentityVariable] as Identity[]);
				if (array != null)
				{
					IEnumerable<CommandParameter> source = from c in base.Command.Commands[0].Parameters
					where c.Name == base.IdentityName
					select c;
					if (source.Count<CommandParameter>() > 0)
					{
						base.Command.Commands[0].Parameters.Remove(source.First<CommandParameter>());
					}
				}
			}
			return array;
		}

		private bool singletonObject;
	}
}
