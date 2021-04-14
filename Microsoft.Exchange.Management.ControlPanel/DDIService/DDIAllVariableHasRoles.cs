using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Microsoft.Exchange.Management.DDIService
{
	[AttributeUsage(AttributeTargets.Class)]
	public class DDIAllVariableHasRoles : DDIValidateAttribute, IDDIHasArgumentValidator
	{
		public DDIAllVariableHasRoles() : base("DDIAllVariableHasRoles")
		{
		}

		public override List<string> Validate(object target, Service profile)
		{
			throw new NotImplementedException();
		}

		public List<string> ValidateWithArg(object target, Service profile, Dictionary<string, string> arguments)
		{
			List<string> list = new List<string>();
			Variable variable = target as Variable;
			arguments.TryGetValue("CodeBehind", out this.codeBehind);
			arguments.TryGetValue("Xaml", out this.xaml);
			string xamlName = arguments["SchemaName"];
			using (new DDIVMockRbacPrincipal())
			{
				Dictionary<string, List<string>> rbacMetaData = DDIVUtil.GetRbacMetaData(xamlName, profile);
				DataObjectStore store = DDIVUtil.GetStore(xamlName, profile);
				DataTable table = DDIVUtil.GetTable(xamlName, profile, rbacMetaData);
				DataRow dataRow = table.NewRow();
				DataColumn dataColumn = table.Columns[variable.Name];
				bool? flag = null;
				foreach (Workflow workflow in from x in profile.Workflows
				where x is GetObjectWorkflow || x is GetObjectForNewWorkflow
				select x)
				{
					List<string> list2 = new List<string>();
					if (rbacMetaData != null && rbacMetaData.ContainsKey(variable.Name))
					{
						list2.AddRange(rbacMetaData[variable.Name]);
					}
					MethodInfo method = typeof(MetaDataIncludeWorkflow).GetMethod("IsVariableSettable", BindingFlags.Instance | BindingFlags.NonPublic);
					if ((workflow is GetObjectWorkflow || workflow is GetObjectForNewWorkflow) && !(method.Invoke(workflow, new object[]
					{
						dataRow,
						table,
						store,
						variable,
						profile,
						list2
					}) is bool?))
					{
						string workflowName = (workflow is GetObjectWorkflow) ? "SetObjectWorkflow" : "NewObjectWorkflow";
						string code = DDIVUtil.RetrieveCodesInPostAndPreActions(workflowName, this.xaml, this.codeBehind);
						if (DDIVUtil.IsVariableUsedInCode(code, variable.Name))
						{
							list.Add(string.Format("Variable '{0}', Workflow {1}. Please register dependency via RbacDependenciesForNew/RbacDependenciesForSet or set the SetRoles to NA", variable.Name + " " + variable.DataObjectName, workflow.Name));
						}
					}
				}
			}
			return list;
		}

		private string codeBehind;

		private string xaml;
	}
}
