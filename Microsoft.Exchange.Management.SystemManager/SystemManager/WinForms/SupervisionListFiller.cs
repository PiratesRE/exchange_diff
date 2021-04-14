using System;
using System.Data;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class SupervisionListFiller : AbstractDataTableFiller
	{
		public SupervisionListFiller(string commandText)
		{
			this.command = new LoggableMonadCommand(commandText);
		}

		internal MonadCommand Command
		{
			get
			{
				return this.command;
			}
		}

		protected override void OnFill(DataTable table)
		{
			this.Command.CommandType = CommandType.Text;
			object[] array = this.Command.Execute();
			TransportConfigContainer transportConfigContainer = null;
			int num = array.Length;
			if (num == 1)
			{
				transportConfigContainer = (array[0] as TransportConfigContainer);
			}
			if (transportConfigContainer != null && transportConfigContainer.SupervisionTags != null)
			{
				table.BeginLoadData();
				foreach (string text in transportConfigContainer.SupervisionTags)
				{
					if (this.MatchFilter(text))
					{
						table.Rows.Add(new object[]
						{
							text,
							new Word(text)
						});
					}
				}
				table.EndLoadData();
			}
		}

		private bool MatchFilter(string displayString)
		{
			bool result = true;
			if (!string.IsNullOrEmpty(this.searchText) && (string.IsNullOrEmpty(displayString) || -1 == displayString.IndexOf(this.searchText, 0, StringComparison.InvariantCultureIgnoreCase)))
			{
				result = false;
			}
			return result;
		}

		public override object Clone()
		{
			return new SupervisionListFiller(this.Command.CommandText);
		}

		public override ICommandBuilder CommandBuilder
		{
			get
			{
				return NullableCommandBuilder.Value;
			}
		}

		public override void BuildCommand(string searchText, object[] pipeline, DataRow row)
		{
			this.searchText = searchText;
		}

		public override void BuildCommandWithScope(string searchText, object[] pipeline, DataRow row, object scope)
		{
			this.searchText = searchText;
		}

		private string searchText;

		private MonadCommand command;
	}
}
