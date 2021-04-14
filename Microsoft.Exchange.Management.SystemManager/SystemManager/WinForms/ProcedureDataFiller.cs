using System;
using System.Data;
using Microsoft.Exchange.Configuration.MonadDataProvider;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class ProcedureDataFiller : MonadAdapterFiller
	{
		public ProcedureDataFiller(string commandText, ProcedureBuilder procedureBuilder) : base(commandText, procedureBuilder)
		{
		}

		internal ProcedureBuilder ProcedureBuilder
		{
			get
			{
				return this.CommandBuilder as ProcedureBuilder;
			}
		}

		protected override void OnFill(DataTable resultsTable)
		{
			base.Command.CommandType = CommandType.StoredProcedure;
			bool flag = this.RequireMatchFilter();
			bool flag2 = this.RequireMatchResolve();
			DataTable dataTable = resultsTable;
			if (flag || flag2)
			{
				dataTable = resultsTable.Clone();
			}
			using (MonadDataAdapter monadDataAdapter = new MonadDataAdapter(base.Command))
			{
				if (dataTable.Columns.Count != 0)
				{
					monadDataAdapter.MissingSchemaAction = MissingSchemaAction.Ignore;
					monadDataAdapter.EnforceDataSetSchema = true;
				}
				monadDataAdapter.Fill(dataTable);
			}
			if (flag || flag2)
			{
				resultsTable.BeginLoadData();
				foreach (object obj in dataTable.Rows)
				{
					DataRow dataRow = (DataRow)obj;
					bool flag3 = true;
					if (flag)
					{
						flag3 = this.MatchFilter(dataRow);
					}
					if (flag2 && flag3)
					{
						flag3 = this.MatchResolveProperty(dataRow);
					}
					if (flag3)
					{
						resultsTable.Rows.Add(dataRow.ItemArray);
					}
				}
				resultsTable.EndLoadData();
			}
		}

		private bool RequireMatchFilter()
		{
			return this.ProcedureBuilder.SearchType == 2 && !string.IsNullOrEmpty(this.searchText) && !string.IsNullOrEmpty(this.ProcedureBuilder.NamePropertyFilter);
		}

		private bool MatchFilter(DataRow row)
		{
			bool result = false;
			string text = string.Format("{0}", row[this.ProcedureBuilder.NamePropertyFilter]);
			if (text.IndexOf(this.searchText, 0, StringComparison.CurrentCultureIgnoreCase) >= 0)
			{
				result = true;
			}
			return result;
		}

		private bool RequireMatchResolve()
		{
			return base.IsResolving && !this.ProcedureBuilder.UseFilterToResolveNonId && !string.IsNullOrEmpty(this.ProcedureBuilder.ResolveProperty) && (this.pipeline != null || this.pipeline.Length > 0);
		}

		private bool MatchResolveProperty(DataRow row)
		{
			bool result = false;
			foreach (object objB in this.pipeline)
			{
				if (object.Equals(row[this.ProcedureBuilder.ResolveProperty], objB))
				{
					result = true;
					break;
				}
			}
			return result;
		}

		public override void BuildCommand(string searchText, object[] pipeline, DataRow row)
		{
			this.searchText = searchText;
			this.pipeline = pipeline;
			base.Command = this.ProcedureBuilder.BuildProcedure(base.GetExecutingCommandText(), searchText, pipeline, row);
		}

		public override void BuildCommandWithScope(string searchText, object[] pipeline, DataRow row, object scope)
		{
			this.searchText = searchText;
			this.pipeline = pipeline;
			base.Command = this.ProcedureBuilder.BuildProcedureWithScope(base.GetExecutingCommandText(), searchText, pipeline, row, scope);
		}

		public override object Clone()
		{
			return new ProcedureDataFiller(base.CommandText, this.ProcedureBuilder)
			{
				ResolveCommandText = base.ResolveCommandText,
				IsResolving = base.IsResolving
			};
		}

		private string searchText;

		private object[] pipeline;
	}
}
