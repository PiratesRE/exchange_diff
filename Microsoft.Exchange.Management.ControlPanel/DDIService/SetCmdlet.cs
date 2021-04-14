using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace Microsoft.Exchange.Management.DDIService
{
	public class SetCmdlet : PipelineCmdlet, IReadOnlyChecker
	{
		public SetCmdlet()
		{
		}

		protected SetCmdlet(SetCmdlet activity) : base(activity)
		{
			this.ForceExecution = activity.ForceExecution;
		}

		public override Activity Clone()
		{
			return new SetCmdlet(this);
		}

		[DefaultValue(false)]
		public bool ForceExecution { get; set; }

		protected override bool IsToExecuteCmdlet(DataRow input, DataTable dataTable, DataObjectStore store, List<Parameter> parameters)
		{
			return 1 != parameters.Count || !(parameters.First<Parameter>().Name == base.IdentityName) || this.ForceExecution;
		}

		protected override List<Parameter> GetParametersToInvoke(DataRow input, DataTable dataTable, DataObjectStore store)
		{
			List<Parameter> parametersToInvoke = base.GetParametersToInvoke(input, dataTable, store);
			this.BuildParameters(input, store, parametersToInvoke);
			return parametersToInvoke;
		}

		internal override bool HasPermission(DataRow input, DataTable dataTable, DataObjectStore store, Variable var)
		{
			string updatingVariable = (var == null) ? null : var.MappingProperty;
			List<string> list;
			if (!string.IsNullOrWhiteSpace(updatingVariable))
			{
				IEnumerable<string> source = from c in base.Parameters
				where c.IsAccessingVariable(updatingVariable)
				select c.Name;
				string text = (source.Count<string>() > 0) ? source.First<string>() : updatingVariable;
				Collection<Parameter> collection = new Collection<Parameter>((from c in this.GetEffectiveParameters(input, dataTable, store)
				where c.Type != ParameterType.RunOnModified
				select c).ToList<Parameter>());
				if (!base.SingletonObject)
				{
					Parameter item = new Parameter
					{
						Name = base.IdentityName,
						Reference = base.IdentityVariable,
						Type = ParameterType.Mandatory
					};
					if (!collection.Contains(item))
					{
						collection.Add(item);
					}
				}
				list = (from c in collection
				select c.Name).ToList<string>();
				if (!list.Contains(text, StringComparer.OrdinalIgnoreCase))
				{
					list.Add(text);
				}
			}
			else
			{
				Collection<Parameter> collection2 = new Collection<Parameter>((from c in this.GetEffectiveParameters(input, dataTable, store)
				where c.IsRunnable(input, dataTable)
				select c).ToList<Parameter>());
				this.BuildParameters(input, store, collection2);
				list = (from c in collection2
				select c.Name).ToList<string>();
			}
			return this.CheckPermission(store, list, var);
		}

		protected override string GetVerb()
		{
			return "Set-";
		}

		protected override void DoPreRunCore(DataRow input, DataTable dataTable, DataObjectStore store, Type codeBehind)
		{
			this.BuildParameters(input, store, base.Parameters);
			base.DoPreRunCore(input, dataTable, store, codeBehind);
		}

		private void BuildParameters(DataRow input, DataObjectStore store, ICollection<Parameter> parameters)
		{
			if (!base.SingletonObject)
			{
				Parameter item = new Parameter
				{
					Name = base.IdentityName,
					Reference = base.IdentityVariable,
					Type = ParameterType.Mandatory
				};
				if (!parameters.Contains(item))
				{
					parameters.Add(item);
				}
			}
			List<string> modifiedPropertiesBasedOnDataObject = store.GetModifiedPropertiesBasedOnDataObject(input, base.DataObjectName);
			foreach (string text in modifiedPropertiesBasedOnDataObject)
			{
				if (!string.Equals(base.IdentityName, text, StringComparison.OrdinalIgnoreCase))
				{
					Parameter item2 = new Parameter
					{
						Name = text,
						Reference = text,
						Type = ParameterType.Mandatory
					};
					if (!parameters.Contains(item2))
					{
						parameters.Add(item2);
					}
				}
			}
		}

		internal override bool? FindAndCheckPermission(Func<Activity, bool> predicate, DataRow input, DataTable dataTable, DataObjectStore store, Variable updatingVariable)
		{
			IEnumerable<Activity> enumerable = new List<Activity>
			{
				this
			}.Where(predicate);
			bool? result = null;
			bool flag = base.DataObjectName != null && base.DataObjectName == updatingVariable.DataObjectName;
			foreach (Activity activity in enumerable)
			{
				if (!flag)
				{
					if (!base.Parameters.Any((Parameter p) => p.IsAccessingVariable(updatingVariable.Name)))
					{
						continue;
					}
				}
				result = new bool?(activity.HasPermission(input, dataTable, store, updatingVariable));
			}
			return result;
		}
	}
}
