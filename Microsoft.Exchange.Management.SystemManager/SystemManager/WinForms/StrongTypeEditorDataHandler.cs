using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class StrongTypeEditorDataHandler<T> : IValidator
	{
		public StrongTypeEditorDataHandler(StrongTypeEditor<T> strongTypeEditor, string schema)
		{
			this.strongTypeEditor = strongTypeEditor;
			this.schemaLoader = new StrongTypeSchemaLoader(schema);
			this.InitializeDataTable();
		}

		public StrongTypeEditorDataHandler(StrongTypeEditor<T> strongTypeEditor) : this(strongTypeEditor, null)
		{
		}

		private void InitializeDataTable()
		{
			this.table = new DataTable();
			this.CreateColumn(this.table);
			this.table.Rows.Add(this.table.NewRow());
			this.strongTypeEditor.BindingSource.DataSource = this.table;
			this.table.ColumnChanged += this.table_ColumnChanged;
		}

		protected StrongTypeEditor<T> StrongTypeEditor
		{
			get
			{
				return this.strongTypeEditor;
			}
		}

		public DataTable Table
		{
			get
			{
				return this.table;
			}
		}

		public T StrongType
		{
			get
			{
				return this.strongType;
			}
			set
			{
				try
				{
					this.isLoadingData = true;
					if (!object.Equals(this.strongType, value))
					{
						this.strongType = value;
						if (this.AllowUpdateGUI)
						{
							if (value == null)
							{
								this.SetAsDefaultValue(this.Table);
							}
							else
							{
								this.UpdateTable();
							}
						}
						this.StrongTypeEditor.StrongType = value;
					}
				}
				finally
				{
					this.isLoadingData = false;
				}
			}
		}

		public bool AllowUpdateGUI
		{
			get
			{
				return !this.suppressUpdateGUI;
			}
		}

		public bool IsOpenedAsEdit
		{
			get
			{
				return (bool)this.table.Rows[0]["IsEditMode"];
			}
			set
			{
				this.table.Rows[0]["IsEditMode"] = value;
			}
		}

		private void table_ColumnChanged(object sender, DataColumnChangeEventArgs e)
		{
			this.table.Rows[0].EndEdit();
			if (this.isLoadingData)
			{
				return;
			}
			if (this.strongTypeEditor.IsHandleCreated && !e.Column.ColumnName.Equals("IsEditMode"))
			{
				this.CheckError(e.Column.ColumnName);
			}
		}

		private void CheckError(string propertyName)
		{
			ValidationError[] array = this.Validate();
			if (array.Length == 1)
			{
				StrongTypeValidationError strongTypeValidationError = array[0] as StrongTypeValidationError;
				bool isTargetProperty = strongTypeValidationError != null && strongTypeValidationError.PropertyName.Equals(propertyName);
				throw new StrongTypeException(strongTypeValidationError.Description, isTargetProperty);
			}
		}

		public ValidationError[] Validate()
		{
			List<ValidationError> list = new List<ValidationError>();
			try
			{
				this.suppressUpdateGUI = true;
				this.UpdateStrongType();
			}
			catch (ArgumentException ex)
			{
				list.Add(new StrongTypeValidationError(new LocalizedString(ex.Message), ex.ParamName));
			}
			catch (StrongTypeFormatException ex2)
			{
				list.Add(new StrongTypeValidationError(new LocalizedString(ex2.Message), ex2.ParamName));
			}
			catch (Exception ex3)
			{
				list.Add(new StrongTypeValidationError(new LocalizedString(ex3.Message), string.Empty));
			}
			finally
			{
				this.suppressUpdateGUI = false;
			}
			return list.ToArray();
		}

		private void SetAsDefaultValue(DataTable table)
		{
			foreach (object obj in table.Columns)
			{
				DataColumn dataColumn = (DataColumn)obj;
				table.Rows[0][dataColumn] = dataColumn.DefaultValue;
			}
		}

		protected virtual void CreateColumn(DataTable table)
		{
			DataColumn dataColumn = new DataColumn("IsEditMode", typeof(bool));
			dataColumn.DefaultValue = false;
			table.Columns.Add(dataColumn);
			table.Columns.AddRange(this.schemaLoader.LoadDataColumns(this.bindingMapping).ToArray());
		}

		protected virtual void UpdateStrongType()
		{
			int count = this.schemaLoader.ArgumentList.Count;
			Type[] array = new Type[count];
			object[] array2 = new object[count];
			int num = 0;
			foreach (string text in this.schemaLoader.ArgumentList)
			{
				array[num] = this.table.Columns[text].DataType;
				array2[num++] = (DBNull.Value.Equals(this.table.Rows[0][text]) ? null : this.table.Rows[0][text]);
			}
			ConstructorInfo constructor = typeof(T).GetConstructor(array);
			try
			{
				this.StrongType = (T)((object)constructor.Invoke(array2));
			}
			catch (TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
		}

		protected virtual void UpdateTable()
		{
			foreach (string text in this.bindingMapping)
			{
				PropertyInfo property = typeof(T).GetProperty(text);
				this.Table.Rows[0][text] = (property.GetValue(this.StrongType, null) ?? DBNull.Value);
			}
		}

		private const string IsEditMode = "IsEditMode";

		private StrongTypeEditor<T> strongTypeEditor;

		private DataTable table;

		private StrongTypeSchemaLoader schemaLoader;

		private T strongType;

		private bool suppressUpdateGUI;

		private bool isLoadingData;

		private List<string> bindingMapping = new List<string>();
	}
}
