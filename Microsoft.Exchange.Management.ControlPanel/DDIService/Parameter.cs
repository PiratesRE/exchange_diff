using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using Microsoft.Exchange.Management.SnapIn;
using Microsoft.Exchange.Management.SystemManager.WinForms;

namespace Microsoft.Exchange.Management.DDIService
{
	[DDIParameter]
	public class Parameter : ICloneable
	{
		public Parameter()
		{
		}

		public Parameter(string name, string reference, ParameterType type, IRunnable runnableTester)
		{
			this.name = name;
			this.reference = reference;
			this.parameterType = type;
			this.runnableTester = runnableTester;
		}

		[DDIMandatoryValue]
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		[DefaultValue(null)]
		[DDIVariableNameExist]
		public string Reference
		{
			get
			{
				return this.reference;
			}
			set
			{
				this.reference = value;
			}
		}

		[DDIValidLambdaExpression]
		[DefaultValue(null)]
		public object Value { get; set; }

		[DefaultValue(ParameterType.Mandatory)]
		public ParameterType Type
		{
			get
			{
				return this.parameterType;
			}
			set
			{
				this.parameterType = value;
			}
		}

		[DefaultValue(null)]
		public IRunnable RunnableTester
		{
			get
			{
				return this.runnableTester;
			}
			set
			{
				this.runnableTester = value;
			}
		}

		[DDIValidLambdaExpression]
		[DefaultValue(null)]
		public string Condition { get; set; }

		[DefaultValue(null)]
		[TypeConverter(typeof(OrganizationTypesConverter))]
		public OrganizationType[] OrganizationTypes { get; set; }

		public static object ConvertToParameterValue(DataRow input, DataTable dataTable, Parameter paramInfo, DataObjectStore store)
		{
			string variableName = paramInfo.Reference ?? paramInfo.Name;
			object obj;
			if (paramInfo.Value == null)
			{
				obj = DDIHelper.GetVariableValue(store.ModifiedColumns, variableName, input, dataTable, store.IsGetListWorkflow);
			}
			else
			{
				string text = paramInfo.Value as string;
				if (DDIHelper.IsLambdaExpression(text))
				{
					obj = ExpressionCalculator.CalculateLambdaExpression(ExpressionCalculator.BuildColumnExpression(text), typeof(object), DDIHelper.GetLambdaExpressionDataRow(dataTable), input);
				}
				else
				{
					VariableReference variableReference = paramInfo.Value as VariableReference;
					if (variableReference != null)
					{
						obj = DDIHelper.GetVariableValue(variableReference, input, dataTable);
					}
					else
					{
						obj = paramInfo.Value;
					}
				}
			}
			if (obj == DBNull.Value)
			{
				return null;
			}
			return obj;
		}

		public object Clone()
		{
			return new Parameter(this.name, this.reference, this.parameterType, this.runnableTester)
			{
				Value = this.Value,
				Condition = this.Condition,
				OrganizationTypes = this.OrganizationTypes
			};
		}

		public override bool Equals(object obj)
		{
			Parameter parameter = obj as Parameter;
			return parameter != null && parameter.Name == this.Name;
		}

		public override int GetHashCode()
		{
			if (this.Name == null)
			{
				return 0;
			}
			return this.Name.GetHashCode();
		}

		public bool IsRunnable(DataRow input, DataTable dataTable)
		{
			DataObjectStore dataObjectStore = dataTable.ExtendedProperties["DataSourceStore"] as DataObjectStore;
			if (this.Type == ParameterType.RunOnModified)
			{
				if (this.Value != null)
				{
					VariableReference variableReference = this.Value as VariableReference;
					if (variableReference != null && !dataObjectStore.ModifiedColumns.Contains(variableReference.Variable))
					{
						return false;
					}
					string text = this.Value as string;
					if (DDIHelper.IsLambdaExpression(text))
					{
						List<string> dependentColumns = ExpressionCalculator.BuildColumnExpression(text).DependentColumns;
						bool flag = false;
						foreach (string item in dependentColumns)
						{
							if (dataObjectStore != null && dataObjectStore.ModifiedColumns.Contains(item))
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							return false;
						}
					}
				}
				else if (!dataObjectStore.ModifiedColumns.Contains(this.Reference ?? this.Name))
				{
					return false;
				}
			}
			if (!string.IsNullOrEmpty(this.Condition))
			{
				return (bool)ExpressionCalculator.CalculateLambdaExpression(ExpressionCalculator.BuildColumnExpression(this.Condition), typeof(bool), DDIHelper.GetLambdaExpressionDataRow(dataTable), input);
			}
			return this.runnableTester == null || this.runnableTester.IsRunnable(input, dataTable);
		}

		internal bool IsAccessingVariable(string accessingVariable)
		{
			List<string> list = new List<string>();
			if (this.Value != null)
			{
				VariableReference variableReference = this.Value as VariableReference;
				if (variableReference != null)
				{
					list.Add(variableReference.Variable);
				}
				else
				{
					string text = this.Value as string;
					if (DDIHelper.IsLambdaExpression(text))
					{
						list.AddRange(ExpressionCalculator.BuildColumnExpression(text).DependentColumns);
					}
				}
			}
			else
			{
				list.Add(this.Reference ?? this.Name);
			}
			return list.Any((string c) => string.Equals(c, accessingVariable, StringComparison.OrdinalIgnoreCase));
		}

		private string name;

		private string reference;

		private ParameterType parameterType = ParameterType.Mandatory;

		private IRunnable runnableTester;
	}
}
