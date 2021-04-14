using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.DDIService
{
	public abstract class AutomatedDataHandlerBase
	{
		public AutomatedDataHandlerBase(Service profileBuilder)
		{
			this.profileBuilder = profileBuilder;
			this.Table = new DataTable();
			this.DataObjectStore = new DataObjectStore(this.profileBuilder.DataObjects, profileBuilder.PredefinedTypes.ToArray());
			Dictionary<string, List<string>> rbacMetaData = null;
			if (typeof(DDICodeBehind).IsAssignableFrom(this.profileBuilder.Class))
			{
				object obj = Activator.CreateInstance(this.profileBuilder.Class);
				this.profileBuilder.Class.GetMethod("ApplyMetaData").Invoke(obj, new object[0]);
				rbacMetaData = (this.profileBuilder.Class.GetProperty("RbacMetaData").GetValue(obj, null) as Dictionary<string, List<string>>);
			}
			lock (AutomatedDataHandlerBase.syncObject)
			{
				this.CreateColumn(this.table, rbacMetaData);
			}
			this.InputTable = this.Table.Clone();
			this.InputTable.Rows.Add(this.InputTable.NewRow());
		}

		public AutomatedDataHandlerBase(string schemaFilesInstallPath, string schema) : this(AutomatedDataHandlerBase.BuildProfile(schemaFilesInstallPath, schema))
		{
			DDIHelper.Trace("Schema: " + schema);
			this.Table.TableName = schema;
		}

		public Service ProfileBuilder
		{
			get
			{
				return this.profileBuilder;
			}
		}

		public DataTable InputTable { get; private set; }

		public DataRow Input
		{
			get
			{
				return this.InputTable.Rows[0];
			}
		}

		public DataTable Table
		{
			get
			{
				return this.table;
			}
			private set
			{
				this.table = value;
				this.Table.ExtendedProperties["DataSourceStore"] = this.DataObjectStore;
			}
		}

		public DataObjectStore DataObjectStore
		{
			get
			{
				return this.dataObjectStore;
			}
			set
			{
				if (value != this.DataObjectStore)
				{
					this.dataObjectStore = value;
					this.Table.ExtendedProperties["DataSourceStore"] = this.dataObjectStore;
				}
			}
		}

		protected DataRow Row
		{
			get
			{
				return this.Table.Rows[0];
			}
		}

		internal static Service BuildProfile(string schemaFilesInstallPath, string resourceName)
		{
			return ServiceManager.GetInstance().GetService(schemaFilesInstallPath, resourceName);
		}

		protected void CreateColumn(DataTable table, Dictionary<string, List<string>> rbacMetaData)
		{
			IList<Variable> variables = this.profileBuilder.Variables;
			foreach (Variable profile in variables)
			{
				table.Columns.Add(AutomatedDataHandlerBase.CreateColumn(profile, rbacMetaData, this.dataObjectStore));
			}
			if ((from c in variables
			where string.Equals("IsReadOnly", c.Name, StringComparison.OrdinalIgnoreCase)
			select c).Count<Variable>() == 0)
			{
				table.Columns.Add(AutomatedDataHandlerBase.CreateColumn(new Variable
				{
					Name = "IsReadOnly",
					Type = typeof(bool)
				}, rbacMetaData, this.dataObjectStore));
			}
		}

		internal static DataColumn CreateColumn(Variable profile, Dictionary<string, List<string>> rbacMetaData, DataObjectStore store)
		{
			DataColumn dataColumn = new DataColumn(profile.Name);
			Type type = null;
			PropertyDefinition value = null;
			if (profile.PersistWholeObject)
			{
				type = store.GetDataObjectType(profile.DataObjectName);
			}
			else
			{
				store.RetrievePropertyInfo(profile.DataObjectName, profile.MappingProperty, out type, out value);
			}
			dataColumn.DataType = (profile.Type ?? typeof(object));
			Type type2;
			if ((type2 = profile.Type) == null)
			{
				type2 = (type ?? typeof(object));
			}
			profile.Type = type2;
			dataColumn.ExtendedProperties.Add("Variable", profile);
			dataColumn.ExtendedProperties.Add("RealDataType", profile.Type);
			dataColumn.ExtendedProperties.Add("PropertyDefinition", value);
			if (rbacMetaData != null && rbacMetaData.ContainsKey(profile.Name))
			{
				dataColumn.ExtendedProperties.Add("RbacMetaData", rbacMetaData[profile.Name]);
			}
			if (profile.Value != null)
			{
				string value2 = profile.Value as string;
				if (DDIHelper.IsLambdaExpression(value2))
				{
					dataColumn.ExtendedProperties["LambdaExpression"] = profile.Value;
				}
				else
				{
					dataColumn.DefaultValue = profile.Value;
				}
			}
			return dataColumn;
		}

		protected void FillColumnsBasedOnLambdaExpression(DataRow row, Variable variable)
		{
			if (DDIHelper.IsLambdaExpression(variable.Value as string))
			{
				this.FillColumns(row, this.GetExpressionCalculator().CalculateSpecifiedColumn(variable.Name, row, this.Input));
			}
		}

		private void FillColumns(DataRow row, IList<KeyValuePair<string, object>> proposedValues)
		{
			foreach (KeyValuePair<string, object> keyValuePair in proposedValues)
			{
				if (keyValuePair.Value == null)
				{
					row[keyValuePair.Key] = DBNull.Value;
				}
				else
				{
					row[keyValuePair.Key] = keyValuePair.Value;
				}
			}
		}

		private ExpressionCalculator GetExpressionCalculator()
		{
			if (this.expressionCalculator == null)
			{
				this.expressionCalculator = ExpressionCalculator.Parse(this.Table);
			}
			return this.expressionCalculator;
		}

		private DataTable table;

		private Service profileBuilder;

		private DataObjectStore dataObjectStore;

		private ExpressionCalculator expressionCalculator;

		private static readonly object syncObject = new object();
	}
}
