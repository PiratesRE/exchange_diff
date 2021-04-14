using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Markup;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics.Components.Management.SystemManager;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal abstract class AutomatedDataHandlerBase : DataHandler
	{
		public AutomatedDataHandlerBase(ITableCentricConfigurable profileBuilder)
		{
			this.profileBuilder = profileBuilder;
			this.Table = new DataTable();
			this.DataObjectStore = new DataObjectStore(this.profileBuilder.BuildDataObjectProfile());
			this.CreateColumn(this.table);
		}

		public AutomatedDataHandlerBase(Assembly assembly, string schema) : this(AutomatedDataHandlerBase.BuildProfile(assembly, schema))
		{
			this.Assembly = assembly;
			this.SchemaName = schema;
			this.Table.TableName = schema;
		}

		public Assembly Assembly { get; set; }

		public string SchemaName { get; set; }

		public DataTable Table
		{
			get
			{
				return this.table;
			}
			protected set
			{
				if (!object.ReferenceEquals(this.Table, value))
				{
					if (this.Table != null)
					{
						this.Table.ExtendedProperties["DataSourceStore"] = null;
						this.Table.ColumnChanged -= this.table_ColumnChanged;
					}
					this.table = value;
					if (this.Table != null)
					{
						this.Table.ExtendedProperties["DataSourceStore"] = this.DataObjectStore;
						this.Table.ColumnChanged += this.table_ColumnChanged;
					}
				}
			}
		}

		public bool EnableBulkEdit
		{
			get
			{
				return this.enableBulkEdit;
			}
			internal set
			{
				this.enableBulkEdit = value;
			}
		}

		protected DataRow Row
		{
			get
			{
				return this.Table.Rows[0];
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

		internal ITableCentricConfigurable ProfileBuilder
		{
			get
			{
				return this.profileBuilder;
			}
		}

		protected void CreateColumn(DataTable table)
		{
			IList<ColumnProfile> list = this.profileBuilder.BuildColumnProfile();
			foreach (ColumnProfile columnProfile in list)
			{
				DataColumn dataColumn = new DataColumn(columnProfile.Name);
				Type type = null;
				object defaultValue = null;
				if (columnProfile.PersistWholeObject)
				{
					type = this.dataObjectStore.GetDataObjectType(columnProfile.DataObjectName);
				}
				else
				{
					this.dataObjectStore.RetrievePropertyInfo(columnProfile.DataObjectName, columnProfile.MappingProperty, out type);
				}
				columnProfile.Retrieve(ref type, ref defaultValue);
				if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
				{
					type = type.GetGenericArguments()[0];
				}
				if (type.IsEnum)
				{
					dataColumn.DataType = typeof(object);
				}
				else
				{
					dataColumn.DataType = type;
				}
				dataColumn.DefaultValue = defaultValue;
				dataColumn.ExtendedProperties.Add("ColumnProfile", columnProfile);
				dataColumn.ExtendedProperties.Add("RealDataType", columnProfile.Type);
				if (!string.IsNullOrEmpty(columnProfile.LambdaExpression))
				{
					dataColumn.ExtendedProperties["LambdaExpression"] = columnProfile.LambdaExpression;
				}
				if (!string.IsNullOrEmpty(columnProfile.OnceLambdaExpression))
				{
					dataColumn.ExtendedProperties["OnceLambdaExpression"] = columnProfile.OnceLambdaExpression;
				}
				table.Columns.Add(dataColumn);
			}
		}

		public DataTable GetDataTableSchema()
		{
			return this.Table.Clone();
		}

		private void table_ColumnChanged(object sender, DataColumnChangeEventArgs e)
		{
			if (this.suppressColumnChanged)
			{
				return;
			}
			this.Row.EndEdit();
			this.FillColumnsBasedOnLambdaExpression(e.Column.ColumnName);
			try
			{
				this.UpdateObject(e.Column);
			}
			catch (TargetInvocationException ex)
			{
				throw (ex.InnerException != null) ? ex.InnerException : ex;
			}
		}

		private void FillColumnsBasedOnOnceLambdaExpression()
		{
			this.suppressColumnChanged = true;
			IList<KeyValuePair<string, object>> list = this.GetOnceExpressionCalculator().CalculateAll(this.Row, null);
			foreach (KeyValuePair<string, object> keyValuePair in list)
			{
				if (keyValuePair.Value == null)
				{
					this.Row[keyValuePair.Key] = DBNull.Value;
				}
				else
				{
					this.Row[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			this.suppressColumnChanged = false;
		}

		private void FillColumnsBasedOnLambdaExpression(string changedColumn)
		{
			this.suppressColumnChanged = true;
			IList<KeyValuePair<string, object>> list = string.IsNullOrEmpty(changedColumn) ? this.GetExpressionCalculator().CalculateAll(this.Row, null) : this.GetExpressionCalculator().CalculateAffectedColumns(changedColumn, this.Row, null);
			foreach (KeyValuePair<string, object> keyValuePair in list)
			{
				if (keyValuePair.Value == null)
				{
					this.Row[keyValuePair.Key] = DBNull.Value;
				}
				else
				{
					this.Row[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			this.suppressColumnChanged = false;
		}

		private ExpressionCalculator GetExpressionCalculator()
		{
			if (this.expressionCalculator == null)
			{
				this.expressionCalculator = ExpressionCalculator.Parse(this.Table);
			}
			return this.expressionCalculator;
		}

		private ExpressionCalculator GetOnceExpressionCalculator()
		{
			if (this.onceExpressionCalculator == null)
			{
				this.onceExpressionCalculator = ExpressionCalculator.Parse(this.Table, "OnceLambdaExpression");
			}
			return this.onceExpressionCalculator;
		}

		public void RefreshDataObjectStore()
		{
			foreach (string targetConfigObject in this.DataObjectStore.GetKeys())
			{
				this.UpdateTable(this.Row, targetConfigObject);
			}
		}

		public void RefreshDataObjectStoreWithNewTable()
		{
			this.Table = this.Table.Copy();
			this.RefreshDataObjectStore();
			base.DataSource = this.Table;
		}

		protected void UpdateObject(DataColumn column)
		{
			ColumnProfile columnProfile = column.ExtendedProperties["ColumnProfile"] as ColumnProfile;
			if (!string.IsNullOrEmpty(columnProfile.DataObjectName))
			{
				if (!columnProfile.PersistWholeObject)
				{
					this.dataObjectStore.SetValue(columnProfile.DataObjectName, columnProfile.MappingProperty, this.Row[column.ColumnName], columnProfile.PropertySetter);
				}
				else
				{
					this.dataObjectStore.UpdateDataObject(columnProfile.DataObjectName, this.Row[column.ColumnName]);
				}
				this.UpdateTable(this.Row, columnProfile.DataObjectName);
			}
		}

		internal void UpdateTable(DataRow row, string targetConfigObject)
		{
			this.UpdateTable(row, targetConfigObject, false);
		}

		internal void UpdateTable(DataRow row, string targetConfigObject, bool isOnReading)
		{
			if (this.DataObjectStore.GetDataObject(targetConfigObject) == null)
			{
				return;
			}
			this.suppressColumnChanged = true;
			try
			{
				foreach (object obj in this.Table.Columns)
				{
					DataColumn dataColumn = (DataColumn)obj;
					ColumnProfile columnProfile = dataColumn.ExtendedProperties["ColumnProfile"] as ColumnProfile;
					if (columnProfile != null && columnProfile.DataObjectName != null && targetConfigObject.Equals(columnProfile.DataObjectName, StringComparison.InvariantCultureIgnoreCase))
					{
						object obj2 = columnProfile.PersistWholeObject ? this.DataObjectStore.GetDataObject(columnProfile.DataObjectName) : this.DataObjectStore.GetValue(columnProfile.DataObjectName, columnProfile.MappingProperty);
						obj2 = (obj2 ?? DBNull.Value);
						this.Row[dataColumn] = obj2;
					}
				}
			}
			finally
			{
				this.suppressColumnChanged = false;
				if (isOnReading)
				{
					this.FillColumnsBasedOnOnceLambdaExpression();
				}
				this.FillColumnsBasedOnLambdaExpression(null);
			}
		}

		protected override void CheckObjectReadOnly()
		{
			bool isObjectReadOnly = false;
			ExchangeObjectVersion exchangeObjectVersion = ExchangeObjectVersion.Exchange2003;
			foreach (string name in this.dataObjectStore.GetKeys())
			{
				IVersionable versionable = this.dataObjectStore.GetDataObject(name) as IVersionable;
				if (versionable != null && versionable.IsReadOnly)
				{
					isObjectReadOnly = true;
				}
				if (versionable != null && exchangeObjectVersion.IsOlderThan(versionable.ExchangeVersion))
				{
					exchangeObjectVersion = versionable.ExchangeVersion;
				}
			}
			base.IsObjectReadOnly = isObjectReadOnly;
			if (base.IsObjectReadOnly)
			{
				base.ObjectReadOnlyReason = Strings.VersionMismatchWarning(exchangeObjectVersion.ExchangeBuild);
				return;
			}
			base.ObjectReadOnlyReason = string.Empty;
		}

		public override bool IsCorrupted
		{
			get
			{
				return !this.EnableBulkEdit && this.dataObjectStore.IsCorrupted;
			}
		}

		public override bool OverrideCorruptedValuesWithDefault()
		{
			return this.dataObjectStore.OverrideCorruptedValuesWithDefault();
		}

		public override ValidationError[] Validate()
		{
			if (this.EnableBulkEdit)
			{
				return new ValidationError[0];
			}
			return this.DataObjectStore.Validate(this.Table);
		}

		public override ValidationError[] ValidateOnly(object objectToBeValidated)
		{
			return this.Validate();
		}

		internal override void SpecifyParameterNames(Dictionary<object, List<string>> bindingMembers)
		{
			if (bindingMembers.Keys.Contains(this.Table))
			{
				this.DataObjectStore.SetModifiedColumns(bindingMembers[this.Table]);
			}
		}

		internal bool IsBulkEditingSupportedParameterName(object dataSource, string propertyName)
		{
			return (from DataColumn c in this.Table.Columns
			where c.ColumnName == propertyName && (c.ExtendedProperties["ColumnProfile"] as ColumnProfile).SupportBulkEdit
			select c).Count<DataColumn>() > 0;
		}

		internal bool IsBulkEditingModifiedParameterName(object dataSource, string propertyName)
		{
			return this.DataObjectStore.ModifiedColumnsAfterCreation.Contains(propertyName);
		}

		internal static ITableCentricConfigurable BuildProfile(Assembly resourceContainedAssembly, string resourceName)
		{
			string name = resourceName + ".xaml";
			ITableCentricConfigurable tableCentricConfigurable = null;
			Stopwatch stopwatch = new Stopwatch();
			ExTraceGlobals.DataFlowTracer.TracePerformance((long)Thread.CurrentThread.ManagedThreadId, "Start to parse the schema " + resourceName);
			stopwatch.Start();
			if (tableCentricConfigurable == null)
			{
				Stream manifestResourceStream = resourceContainedAssembly.GetManifestResourceStream(name);
				tableCentricConfigurable = (XamlReader.Load(manifestResourceStream) as ITableCentricConfigurable);
			}
			stopwatch.Stop();
			ExTraceGlobals.DataFlowTracer.TracePerformance<string, long>((long)Thread.CurrentThread.ManagedThreadId, "End to parse the schema {0} and it costs {1} ms.", resourceName, stopwatch.ElapsedMilliseconds);
			return tableCentricConfigurable;
		}

		internal abstract bool HasViewPermissionForPage(string pageName);

		internal abstract bool HasPermissionForProperty(string propertyName, bool canUpdate);

		private DataTable table;

		private ITableCentricConfigurable profileBuilder;

		private DataObjectStore dataObjectStore;

		private bool suppressColumnChanged;

		private bool enableBulkEdit;

		private ExpressionCalculator expressionCalculator;

		private ExpressionCalculator onceExpressionCalculator;
	}
}
