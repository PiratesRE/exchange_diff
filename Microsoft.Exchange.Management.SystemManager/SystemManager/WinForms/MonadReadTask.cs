using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Management.SnapIn;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class MonadReadTask : Reader
	{
		public MonadReadTask()
		{
		}

		public MonadReadTask(Type configObjectType) : this("get-" + configObjectType.Name.ToLowerInvariant())
		{
		}

		internal MonadReadTask(string commandText)
		{
			this.CommandText = commandText;
		}

		[DDIValidCommandText]
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

		public override void Run(object interactionHandler, DataRow row, DataObjectStore store)
		{
			object[] array = this.Command.Execute();
			switch (array.Length)
			{
			case 0:
				break;
			case 1:
				this.dataObject = array[0];
				break;
			default:
				return;
			}
		}

		public override void BuildParameters(DataRow row, DataObjectStore store, IList<ParameterProfile> paramInfos)
		{
			this.Command.Parameters.Clear();
			MonadSaveTask.BuildParametersCore(row, paramInfos, this.Command.Parameters);
		}

		public override bool HasPermission(IList<ParameterProfile> paramInfos)
		{
			return EMCRunspaceConfigurationSingleton.GetInstance().IsCmdletAllowedInScope(this.commandText, (from c in paramInfos
			select c.Name).ToArray<string>());
		}

		private object dataObject;

		private MonadCommand command;

		private string commandText;
	}
}
