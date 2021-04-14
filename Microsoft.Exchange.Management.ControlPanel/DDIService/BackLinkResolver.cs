using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	public class BackLinkResolver : CmdletActivity
	{
		public BackLinkResolver()
		{
			this.EnableFilter = true;
			this.FilterOperator = "eq";
		}

		protected BackLinkResolver(BackLinkResolver activity) : base(activity)
		{
			this.LinkProperty = activity.LinkProperty;
			this.OutputVariable = activity.OutputVariable;
			this.EnableFilter = activity.EnableFilter;
			this.FilterOperator = activity.FilterOperator;
		}

		public override Activity Clone()
		{
			return new BackLinkResolver(this);
		}

		[DDIMandatoryValue]
		public string LinkProperty { get; set; }

		[DDIMandatoryValue]
		[DDIVariableNameExist]
		[DDIVaraibleWithoutDataObject]
		public string OutputVariable { get; set; }

		public bool EnableFilter { get; set; }

		public string FilterOperator { get; set; }

		public override RunResult Run(DataRow input, DataTable dataTable, DataObjectStore store, Type codeBehind, Workflow.UpdateTableDelegate updateTableDelegate)
		{
			DDIHelper.CheckDataTableForSingleObject(dataTable);
			DataRow dataRow = dataTable.Rows[0];
			if (!(dataRow[base.IdentityVariable] is ADObjectId))
			{
				throw new NotSupportedException("Currently we don't support Back-Link look up based on the UMC type Identity!");
			}
			if (this.EnableFilter)
			{
				base.Command.AddParameter("Filter", string.Format("{0} -{1} '{2}'", this.LinkProperty, this.FilterOperator, DDIHelper.ToQuotationEscapedString(((ADObjectId)dataRow[base.IdentityVariable]).DistinguishedName)));
			}
			RunResult runResult = new RunResult();
			PowerShellResults<PSObject> powerShellResults;
			base.ExecuteCmdlet(null, runResult, out powerShellResults, false);
			if (!runResult.ErrorOccur)
			{
				List<ADObjectId> list = new List<ADObjectId>();
				foreach (PSObject psobject in powerShellResults.Output)
				{
					if (!this.EnableFilter)
					{
						ADObjectId adobjectId = (ADObjectId)psobject.Properties[this.LinkProperty].Value;
						if (adobjectId != null && adobjectId.Equals(dataRow[base.IdentityVariable]))
						{
							list.Add(psobject.Properties["Identity"].Value as ADObjectId);
						}
					}
					else
					{
						list.Add(psobject.Properties["Identity"].Value as ADObjectId);
					}
				}
				dataRow[this.OutputVariable] = list;
			}
			return runResult;
		}

		protected override bool IsToExecuteCmdlet(DataRow input, DataTable dataTable, DataObjectStore store, List<Parameter> parameters)
		{
			return dataTable.Rows[0][base.IdentityVariable] is ADObjectId;
		}

		internal override bool HasPermission(DataRow input, DataTable dataTable, DataObjectStore store, Variable updatingVariable)
		{
			List<string> list = (from c in this.GetEffectiveParameters(input, dataTable, store)
			select c.Name).ToList<string>();
			if (this.EnableFilter && !list.Contains("Filter", StringComparer.OrdinalIgnoreCase))
			{
				list.Add("Filter");
			}
			return this.CheckPermission(store, list, updatingVariable);
		}

		internal override bool HasOutputVariable(string variable)
		{
			return this.OutputVariable == variable;
		}

		private const string Filter = "Filter";
	}
}
