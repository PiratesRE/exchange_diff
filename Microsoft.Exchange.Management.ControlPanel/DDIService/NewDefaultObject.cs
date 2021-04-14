using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows.Markup;
using Microsoft.Exchange.Management.SystemManager.WinForms;

namespace Microsoft.Exchange.Management.DDIService
{
	[ContentProperty("DefaultValues")]
	public class NewDefaultObject : Activity
	{
		public NewDefaultObject()
		{
		}

		protected NewDefaultObject(NewDefaultObject activity) : base(activity)
		{
			this.DataObjectNames = activity.DataObjectNames;
			this.DefaultValues = new Collection<Set>((from c in activity.DefaultValues
			select c.Clone() as Set).ToList<Set>());
		}

		public override Activity Clone()
		{
			return new NewDefaultObject(this);
		}

		[DDICollectionDecorator(AttributeType = typeof(DDIVariableNameExistAttribute), ObjectConverter = typeof(DDISetToVariableConverter))]
		public Collection<Set> DefaultValues
		{
			get
			{
				return this.sets;
			}
			set
			{
				this.sets = value;
			}
		}

		public string DataObjectNames { get; set; }

		public override RunResult Run(DataRow input, DataTable dataTable, DataObjectStore store, Type codeBehind, Workflow.UpdateTableDelegate updateTableDelegate)
		{
			RunResult runResult = new RunResult();
			foreach (string text in this.GetDataObjectKeys(store))
			{
				object obj = null;
				IDataObjectCreator dataObjectCreator = store.GetDataObjectCreator(text);
				if (dataObjectCreator != null)
				{
					obj = dataObjectCreator.Create(dataTable);
				}
				else
				{
					Type dataObjectType = store.GetDataObjectType(text);
					if (null != dataObjectType)
					{
						obj = dataObjectType.GetConstructor(new Type[0]).Invoke(new object[0]);
					}
				}
				if (obj != null)
				{
					store.UpdateDataObject(text, obj, true);
					updateTableDelegate(text, false);
				}
			}
			runResult.DataObjectes.AddRange(store.GetKeys());
			return runResult;
		}

		private IEnumerable<string> GetDataObjectKeys(DataObjectStore store)
		{
			if (string.IsNullOrEmpty(this.DataObjectNames))
			{
				return store.GetKeys();
			}
			IList<string> list = new List<string>();
			foreach (string text in this.DataObjectNames.Split(new char[]
			{
				','
			}))
			{
				list.Add(text.Trim());
			}
			return list;
		}

		protected override void DoPostRunCore(DataRow input, DataTable dataTable, DataObjectStore store, Type codeBehind)
		{
			Collection<Set> defaultValues = this.DefaultValues;
			NewDefaultObject.SetDefaultValues(input, dataTable.Rows[0], dataTable, defaultValues);
		}

		internal static void SetDefaultValues(DataRow input, DataRow row, DataTable dataTable, Collection<Set> defaultValues)
		{
			if (defaultValues != null)
			{
				foreach (Set set in defaultValues)
				{
					object obj = set.Value;
					string text = obj as string;
					if (DDIHelper.IsLambdaExpression(text))
					{
						obj = ExpressionCalculator.CalculateLambdaExpression(ExpressionCalculator.BuildColumnExpression(text), typeof(object), DDIHelper.GetLambdaExpressionDataRow(dataTable), input);
					}
					else
					{
						VariableReference variableReference = obj as VariableReference;
						if (variableReference != null)
						{
							obj = DDIHelper.GetVariableValue(variableReference, input, dataTable);
						}
					}
					row[set.Variable] = obj;
				}
			}
		}

		private Collection<Set> sets = new Collection<Set>();
	}
}
