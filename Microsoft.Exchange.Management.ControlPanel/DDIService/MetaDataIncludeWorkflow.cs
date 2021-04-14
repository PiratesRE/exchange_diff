using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.ControlPanel;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.DDIService
{
	public abstract class MetaDataIncludeWorkflow : Workflow
	{
		public MetaDataIncludeWorkflow()
		{
			this.IncludeNotAccessProperty = true;
			this.IncludeReadOnlyProperty = true;
			this.IncludeValidator = true;
		}

		protected MetaDataIncludeWorkflow(MetaDataIncludeWorkflow workflow) : base(workflow)
		{
			this.IncludeNotAccessProperty = workflow.IncludeNotAccessProperty;
			this.IncludeReadOnlyProperty = workflow.IncludeReadOnlyProperty;
			this.IncludeValidator = workflow.IncludeValidator;
		}

		[DefaultValue(true)]
		protected bool IncludeValidator { get; set; }

		internal IIsInRole RbacChecker
		{
			get
			{
				return this.rbacPrincipal ?? RbacCheckerWrapper.RbacChecker;
			}
			set
			{
				this.rbacPrincipal = value;
			}
		}

		[DefaultValue(true)]
		protected bool IncludeReadOnlyProperty { get; set; }

		[DefaultValue(true)]
		protected bool IncludeNotAccessProperty { get; set; }

		internal override void LoadMetaData(DataRow input, DataTable dataTable, DataObjectStore store, IList<string> outputVariables, out Dictionary<string, ValidatorInfo[]> validators, out IList<string> readOnlyProperties, out IList<string> notAccessProperties, Service service)
		{
			base.LoadMetaData(input, dataTable, store, outputVariables, out validators, out readOnlyProperties, out notAccessProperties, service);
			bool flag = true.Equals(dataTable.Rows[0]["IsReadOnly"]);
			foreach (object obj in dataTable.Columns)
			{
				DataColumn dataColumn = (DataColumn)obj;
				Variable variable = dataColumn.ExtendedProperties["Variable"] as Variable;
				if (variable != null)
				{
					if (outputVariables == null || outputVariables.Contains(dataColumn.ColumnName, StringComparer.OrdinalIgnoreCase))
					{
						if (this.IncludeValidator)
						{
							ValidatorInfo[] array = ValidatorHelper.ValidatorsFromPropertyDefinition(dataColumn.ExtendedProperties["PropertyDefinition"] as ProviderPropertyDefinition);
							if (array.Length != 0)
							{
								validators[dataColumn.ColumnName] = array;
							}
						}
						if (this.IncludeNotAccessProperty)
						{
							bool? flag2 = this.IsVariableAccessible(input, dataTable, store, variable, DDIHelper.GetOutputDepVariables(dataColumn));
							if (flag2.IsFalse())
							{
								notAccessProperties.Add(variable.Name);
							}
						}
					}
					if (this.IncludeReadOnlyProperty)
					{
						bool? flag3 = this.IsVariableSettable(input, dataTable, store, variable, service, DDIHelper.GetCodeBehindRegisteredDepVariables(dataColumn));
						if (flag3.IsFalse() || flag)
						{
							readOnlyProperties.Add(variable.Name);
						}
					}
				}
			}
		}

		private bool CheckPredefinedPermission(string role, DataObjectStore store, Variable variable)
		{
			return role.Equals("NA", StringComparison.OrdinalIgnoreCase) || this.RbacChecker.IsInRole(role, (variable.RbacDataObjectName != null) ? (store.GetDataObject(variable.RbacDataObjectName) as ADRawEntry) : null);
		}

		private bool? IsVariableAccessible(DataRow input, DataTable dataTable, DataObjectStore store, Variable variable, List<string> dependencies)
		{
			bool? flag = null;
			return this.CheckPermission(null, input, dataTable, store, variable, dependencies, new MetaDataIncludeWorkflow.CheckPermissionDelegate(this.CheckRbacForNotAccessPermission));
		}

		private bool? IsVariableSettable(DataRow input, DataTable dataTable, DataObjectStore store, Variable variable, Service service, List<string> dependencies)
		{
			bool flag = this is GetObjectWorkflow;
			bool flag2 = this is GetObjectForNewWorkflow;
			bool? result = null;
			if (flag || flag2)
			{
				if (flag && variable.SetRoles != null)
				{
					result = new bool?(this.CheckPredefinedPermission(variable.SetRoles, store, variable));
				}
				else if (flag2 && variable.NewRoles != null)
				{
					result = new bool?(this.CheckPredefinedPermission(variable.NewRoles, store, variable));
				}
				else
				{
					IEnumerable<Workflow> enumerable = null;
					if (flag)
					{
						enumerable = from c in service.Workflows
						where c is SetObjectWorkflow
						select c;
						if (variable.RbacDependenciesForSet != null)
						{
							dependencies.AddRange(variable.RbacDependenciesForSet);
						}
					}
					else if (flag2)
					{
						enumerable = from c in service.Workflows
						where c is NewObjectWorkflow
						select c;
						if (variable.RbacDependenciesForNew != null)
						{
							dependencies.AddRange(variable.RbacDependenciesForNew);
						}
					}
					if (enumerable.Count<Workflow>() > 0)
					{
						result = this.CheckPermission(enumerable, input, dataTable, store, variable, dependencies, new MetaDataIncludeWorkflow.CheckPermissionDelegate(this.CheckRbacForReadOnlyPermission));
					}
				}
			}
			return result;
		}

		private bool? CheckPermission(IEnumerable<Workflow> sets, DataRow input, DataTable dataTable, DataObjectStore store, Variable variable, IList<string> dependencies, MetaDataIncludeWorkflow.CheckPermissionDelegate checkPermissionDelegate)
		{
			bool? flag = null;
			if (dependencies != null && dependencies.Count > 0)
			{
				using (IEnumerator<string> enumerator = (from c in dependencies
				where !string.IsNullOrWhiteSpace(c)
				select c).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string name = enumerator.Current;
						Variable variable2 = dataTable.Columns[name].ExtendedProperties["Variable"] as Variable;
						if (variable2 != null)
						{
							flag = flag.And(checkPermissionDelegate(input, dataTable, store, variable2, sets));
							if (flag.IsFalse())
							{
								break;
							}
						}
					}
					return flag;
				}
			}
			flag = checkPermissionDelegate(input, dataTable, store, variable, sets);
			return flag;
		}

		private bool? CheckRbacForReadOnlyPermission(DataRow input, DataTable dataTable, DataObjectStore store, Variable variable, IEnumerable<Workflow> sets)
		{
			bool? flag = null;
			Collection<Activity> activities = sets.First<Workflow>().Activities;
			bool isSetCmdletDynamicParameter = !string.IsNullOrWhiteSpace(variable.DataObjectName) && !variable.IgnoreChangeTracking;
			foreach (Activity activity in activities)
			{
				flag = flag.Or(activity.FindAndCheckPermission((Activity a) => (isSetCmdletDynamicParameter && a is SetCmdlet && (a as SetCmdlet).DataObjectName == variable.DataObjectName) || (a is IReadOnlyChecker && a is CmdletActivity), input, dataTable, store, variable));
				if (flag.IsTrue())
				{
					break;
				}
			}
			return flag;
		}

		private bool? CheckRbacForNotAccessPermission(DataRow input, DataTable dataTable, DataObjectStore store, Variable variable, IEnumerable<Workflow> sets)
		{
			bool? flag = null;
			Collection<Activity> activities = base.Activities;
			bool hasDataObject = !string.IsNullOrWhiteSpace(variable.DataObjectName);
			foreach (Activity activity in activities)
			{
				flag = flag.Or(activity.FindAndCheckPermission((Activity a) => (hasDataObject && a is GetCmdlet && (a as GetCmdlet).DataObjectName == variable.DataObjectName) || a.HasOutputVariable(variable.Name), input, dataTable, store, variable));
				if (flag.IsTrue())
				{
					break;
				}
			}
			if (hasDataObject)
			{
				IVersionable versionable = store.GetDataObject(variable.DataObjectName) as IVersionable;
				if (versionable != null && versionable.ExchangeVersion != null)
				{
					PropertyDefinition propertyDefinition = versionable.ObjectSchema[variable.MappingProperty];
					if (propertyDefinition != null && !versionable.IsPropertyAccessible(propertyDefinition))
					{
						flag = new bool?(false);
					}
				}
			}
			return flag;
		}

		internal const string RBAC_NotApplicable = "NA";

		private IIsInRole rbacPrincipal;

		internal delegate bool? CheckPermissionDelegate(DataRow input, DataTable dataTable, DataObjectStore store, Variable variable, IEnumerable<Workflow> sets);
	}
}
