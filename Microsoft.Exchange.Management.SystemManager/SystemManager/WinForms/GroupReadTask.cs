using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Management.SnapIn;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class GroupReadTask : Reader
	{
		public GroupReadTask()
		{
		}

		public GroupReadTask(string commandText)
		{
			this.CommandText = commandText;
		}

		public override object DataObject
		{
			get
			{
				return this.dataObject;
			}
		}

		internal MonadCommand Command
		{
			get
			{
				if (this.command == null)
				{
					this.command = new LoggableMonadCommand(this.commandText);
				}
				return this.command;
			}
		}

		public string CommandText
		{
			get
			{
				return this.commandText;
			}
			set
			{
				this.commandText = value;
			}
		}

		public override void Run(object interactionHandler, DataRow row, DataObjectStore store)
		{
			this.dataObject = new List<object>(this.Command.Execute());
		}

		public override bool HasPermission(IList<ParameterProfile> paramInfos)
		{
			return EMCRunspaceConfigurationSingleton.GetInstance().IsCmdletAllowedInScope(this.commandText, (from c in paramInfos
			select c.Name).ToArray<string>());
		}

		public override void BuildParameters(DataRow row, DataObjectStore store, IList<ParameterProfile> paramInfos)
		{
			this.Command.Parameters.Clear();
			MonadSaveTask.BuildParametersCore(row, paramInfos, this.Command.Parameters);
		}

		private object dataObject;

		private LoggableMonadCommand command;

		private string commandText;
	}
}
