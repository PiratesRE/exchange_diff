using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Management.SnapIn;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class MonadAdapterFiller : AbstractDataTableFiller, IHasPermission
	{
		public MonadAdapterFiller(string commandText, ICommandBuilder builder)
		{
			this.commandText = commandText;
			this.commandBuilder = builder;
			this.FixedValues = new Dictionary<string, object>();
		}

		public Dictionary<string, object> FixedValues { get; private set; }

		public override ICommandBuilder CommandBuilder
		{
			get
			{
				return this.commandBuilder;
			}
		}

		public Dictionary<string, string> Parameters
		{
			get
			{
				return this.parameters;
			}
			set
			{
				this.parameters = value;
			}
		}

		public List<string> AddtionalParameters
		{
			get
			{
				return this.additionalParameters;
			}
			set
			{
				this.additionalParameters = value;
			}
		}

		public string ResolveCommandText
		{
			get
			{
				return this.resolveCommandText;
			}
			set
			{
				this.resolveCommandText = value;
			}
		}

		public bool IsResolving
		{
			get
			{
				return this.isResolving;
			}
			set
			{
				this.isResolving = value;
			}
		}

		public string CommandText
		{
			get
			{
				return this.commandText;
			}
		}

		internal MonadCommand Command { get; set; }

		public override void BuildCommand(string searchText, object[] pipeline, DataRow row)
		{
			this.Command = new LoggableMonadCommand();
			this.Command.CommandText = this.CommandBuilder.BuildCommand(this.GetExecutingCommandText(), searchText, pipeline, row);
		}

		public override void BuildCommandWithScope(string searchText, object[] pipeline, DataRow row, object scope)
		{
			this.Command = new LoggableMonadCommand();
			this.Command.CommandText = this.CommandBuilder.BuildCommandWithScope(this.GetExecutingCommandText(), searchText, pipeline, row, scope);
		}

		protected override void OnFill(DataTable table)
		{
			this.Command.CommandType = CommandType.Text;
			DataTable filledTable = table;
			if (this.FixedValues.Count != 0)
			{
				filledTable = table.Clone();
				filledTable.RowChanged += delegate(object sender, DataRowChangeEventArgs e)
				{
					if (e.Action == DataRowAction.Add)
					{
						foreach (string text in this.FixedValues.Keys)
						{
							e.Row[text] = this.FixedValues[text];
						}
						table.Rows.Add(e.Row.ItemArray);
						filledTable.Rows.Remove(e.Row);
					}
				};
			}
			using (MonadDataAdapter monadDataAdapter = new MonadDataAdapter(this.Command))
			{
				if (table.Columns.Count != 0)
				{
					monadDataAdapter.MissingSchemaAction = MissingSchemaAction.Ignore;
					monadDataAdapter.EnforceDataSetSchema = true;
				}
				monadDataAdapter.Fill(filledTable);
			}
		}

		protected string GetExecutingCommandText()
		{
			StringBuilder stringBuilder = new StringBuilder(this.commandText);
			foreach (string text in this.parameters.Keys)
			{
				stringBuilder.AppendFormat(" -{0} {1}", text, this.parameters[text]);
			}
			string result = stringBuilder.ToString();
			if (this.isResolving && !string.IsNullOrEmpty(this.resolveCommandText))
			{
				if (!this.resolveCommandText.Equals(this.commandText, StringComparison.OrdinalIgnoreCase))
				{
					throw new NotSupportedException();
				}
				result = this.resolveCommandText;
			}
			return result;
		}

		public override object Clone()
		{
			MonadAdapterFiller monadAdapterFiller = new MonadAdapterFiller(this.commandText, this.commandBuilder)
			{
				ResolveCommandText = this.resolveCommandText,
				IsResolving = this.isResolving,
				Parameters = this.parameters,
				AddtionalParameters = this.additionalParameters
			};
			foreach (string key in this.FixedValues.Keys)
			{
				monadAdapterFiller.FixedValues[key] = this.FixedValues[key];
			}
			return monadAdapterFiller;
		}

		public override void Cancel()
		{
			if (this.Command != null)
			{
				this.Command.Cancel();
			}
		}

		public bool HasPermission()
		{
			List<string> list = new List<string>(this.additionalParameters);
			list.AddRange(this.parameters.Keys);
			return EMCRunspaceConfigurationSingleton.GetInstance().IsCmdletAllowedInScope(this.CommandText, list.ToArray());
		}

		private ICommandBuilder commandBuilder;

		private string commandText;

		private string resolveCommandText;

		private bool isResolving;

		private Dictionary<string, string> parameters = new Dictionary<string, string>();

		private List<string> additionalParameters = new List<string>();
	}
}
