using System;
using System.ComponentModel;
using System.Data;
using Microsoft.Exchange.Management.SnapIn;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ParameterProfile
	{
		public ParameterProfile()
		{
		}

		public ParameterProfile(string name, string reference, ParameterType type, IRunnable runnableTester)
		{
			this.name = name;
			this.reference = reference;
			this.parameterType = type;
			this.runnableTester = runnableTester;
		}

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
		[DDIDataColumnExist]
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

		[DefaultValue(null)]
		[DDIValidLambdaExpression]
		public string LambdaExpression { get; set; }

		public ParameterType ParameterType
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

		[DefaultValue(null)]
		public string RunnableLambdaExpression { get; set; }

		[TypeConverter(typeof(OrganizationTypesConverter))]
		[DefaultValue(null)]
		public OrganizationType[] OrganizationTypes { get; set; }

		[DefaultValue(false)]
		public bool HideDisplay { get; set; }

		public bool IsRunnable(DataRow row)
		{
			if (this.ParameterType == ParameterType.ModifiedColumn)
			{
				DataObjectStore dataObjectStore = row.Table.ExtendedProperties["DataSourceStore"] as DataObjectStore;
				if (dataObjectStore == null || !dataObjectStore.ModifiedColumns.Contains(this.Reference))
				{
					return false;
				}
			}
			if (!string.IsNullOrEmpty(this.RunnableLambdaExpression))
			{
				return (bool)ExpressionCalculator.CalculateLambdaExpression(ExpressionCalculator.BuildColumnExpression(this.RunnableLambdaExpression), typeof(bool), row, null);
			}
			return this.runnableTester == null || this.runnableTester.IsRunnable(row);
		}

		private string name;

		private string reference;

		private ParameterType parameterType;

		private IRunnable runnableTester;
	}
}
