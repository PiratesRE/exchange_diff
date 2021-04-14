using System;
using System.Management.Automation;
using System.Reflection;
using System.Text;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.SnapIn;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class SingleTaskDataHandler : ExchangeDataHandler
	{
		public SingleTaskDataHandler() : this("")
		{
		}

		public SingleTaskDataHandler(string saveCommandText) : this(saveCommandText, new MonadConnection("timeout=30", new CommandInteractionHandler(), ADServerSettingsSingleton.GetInstance().CreateRunspaceServerSettingsObject(), PSConnectionInfoSingleton.GetInstance().GetMonadConnectionInfo()))
		{
		}

		internal SingleTaskDataHandler(string saveCommandText, MonadConnection connection) : this(new LoggableMonadCommand(saveCommandText, connection))
		{
		}

		internal SingleTaskDataHandler(MonadCommand saveCommand)
		{
			this.saveCommand = saveCommand;
			if (this.saveCommand == null)
			{
				throw new ArgumentNullException("saveCommand");
			}
			this.connection = saveCommand.Connection;
			if (this.connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			this.fields = new ADPropertyBag();
			base.DataSource = this;
		}

		internal WorkUnit WorkUnit
		{
			get
			{
				if (base.WorkUnits.Count == 0)
				{
					base.WorkUnits.Add(new WorkUnit());
				}
				return base.WorkUnits[0];
			}
		}

		internal MonadConnection Connection
		{
			get
			{
				return this.connection;
			}
		}

		internal MonadCommand Command
		{
			get
			{
				return this.saveCommand;
			}
		}

		internal MonadParameterCollection Parameters
		{
			get
			{
				return this.saveCommand.Parameters;
			}
		}

		public string CommandText
		{
			get
			{
				return this.saveCommand.CommandText;
			}
			set
			{
				this.saveCommand.CommandText = value;
			}
		}

		internal bool KeepInstanceParamerter { get; set; }

		protected virtual void AdjustParameters()
		{
		}

		protected virtual void HandleIdentityParameter()
		{
			IConfigurable configurable = this.Parameters["Instance"].Value as IConfigurable;
			if (configurable != null && !this.Parameters.Contains("Identity") && !(configurable is TransportConfigContainer) && !(configurable is PopImapAdConfiguration))
			{
				this.Parameters.AddWithValue("Identity", configurable.Identity);
			}
		}

		internal MonadParameter[] PrepareParameters()
		{
			MonadParameter[] array = new MonadParameter[this.Parameters.Count];
			this.Parameters.CopyTo(array, 0);
			this.AddParameters();
			if (!this.KeepInstanceParamerter && this.Parameters.Contains("Instance"))
			{
				object value = this.Parameters["Instance"].Value;
				BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;
				foreach (PropertyInfo propertyInfo in value.GetType().GetProperties(bindingAttr))
				{
					if ((propertyInfo.GetCustomAttributes(typeof(ParameterAttribute), true).Length != 0 || (propertyInfo.GetSetMethod(true) != null && !propertyInfo.GetSetMethod(true).IsPrivate)) && base.ParameterNames.Contains(propertyInfo.Name) && !this.Parameters.Contains(propertyInfo.Name))
					{
						this.Parameters.AddWithValue(propertyInfo.Name, propertyInfo.GetValue(value, null));
					}
				}
				this.HandleIdentityParameter();
				this.Parameters.Remove("Instance");
			}
			this.AdjustParameters();
			MonadParameter[] array2 = new MonadParameter[this.Parameters.Count];
			this.Parameters.CopyTo(array2, 0);
			this.Parameters.Clear();
			this.Parameters.AddRange(array);
			return array2;
		}

		internal sealed override void OnReadData(CommandInteractionHandler interactionHandler, string pageName)
		{
			this.Connection.InteractionHandler = interactionHandler;
			using (new OpenConnection(this.Connection))
			{
				this.OnReadData();
				object dataSource = base.DataSource;
				try
				{
					base.OnReadData(interactionHandler, pageName);
				}
				finally
				{
					base.DataSource = dataSource;
				}
			}
		}

		protected virtual void OnReadData()
		{
		}

		internal sealed override void OnSaveData(CommandInteractionHandler interactionHandler)
		{
			this.Connection.InteractionHandler = interactionHandler;
			using (new OpenConnection(this.Connection))
			{
				MonadParameter[] array = new MonadParameter[this.Parameters.Count];
				this.Parameters.CopyTo(array, 0);
				MonadParameter[] values = this.PrepareParameters();
				this.Parameters.Clear();
				this.Parameters.AddRange(values);
				this.OnSaveData();
				base.OnSaveData(interactionHandler);
				this.Parameters.Clear();
				this.Parameters.AddRange(array);
			}
		}

		protected virtual void OnSaveData()
		{
			if (!string.IsNullOrEmpty(this.saveCommand.CommandText))
			{
				base.SavedResults.Clear();
				this.saveCommand.ProgressReport += base.OnProgressReport;
				try
				{
					object[] array = this.saveCommand.Execute(base.WorkUnits.ToArray());
					if (array != null)
					{
						base.SavedResults.AddRange(array);
					}
				}
				finally
				{
					this.saveCommand.ProgressReport -= base.OnProgressReport;
				}
			}
		}

		protected virtual void AddParameters()
		{
			if (this.fields.Count > 0)
			{
				this.Parameters.Clear();
				foreach (object obj in this.Fields.Keys)
				{
					PropertyDefinition propertyDefinition = (PropertyDefinition)obj;
					this.Parameters.AddWithValue(propertyDefinition.Name, this.Fields[propertyDefinition]);
				}
			}
		}

		internal ADPropertyBag Fields
		{
			get
			{
				return this.fields;
			}
		}

		public override void UpdateWorkUnits()
		{
			if (base.WorkUnits.Count == 0 && !base.ReadOnly)
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (this.Fields.Count > 0)
				{
					stringBuilder.AppendLine(Strings.ParametersForTheTaskTitle);
				}
				foreach (object obj in this.Fields.Keys)
				{
					PropertyDefinition propertyDefinition = (PropertyDefinition)obj;
					object obj2 = this.Fields[propertyDefinition];
					stringBuilder.Append(Strings.NameValueFormat(propertyDefinition.Name, obj2.ToString()));
				}
				this.WorkUnit.Text = this.saveCommand.CommandText;
				this.WorkUnit.Description = stringBuilder.ToString();
			}
			base.UpdateWorkUnits();
		}

		public override void Cancel()
		{
			base.Cancel();
			this.saveCommand.Cancel();
		}

		public string GetCommandString()
		{
			if (this.saveCommand == null)
			{
				return null;
			}
			return this.saveCommand.ToString();
		}

		internal override string CommandToRun
		{
			get
			{
				return this.ClonePreparedCommand().ToString() + "\r\n";
			}
		}

		private MonadCommand ClonePreparedCommand()
		{
			MonadParameter[] array = new MonadParameter[this.Parameters.Count];
			this.Parameters.CopyTo(array, 0);
			MonadParameter[] values = this.PrepareParameters();
			this.Parameters.Clear();
			this.Parameters.AddRange(values);
			MonadCommand result = this.saveCommand.Clone();
			this.Parameters.Clear();
			this.Parameters.AddRange(array);
			return result;
		}

		public override string ModifiedParametersDescription
		{
			get
			{
				string result = string.Empty;
				if (this.IsModified())
				{
					StringBuilder stringBuilder = new StringBuilder();
					MonadCommand monadCommand = this.ClonePreparedCommand();
					foreach (object obj in monadCommand.Parameters)
					{
						MonadParameter monadParameter = (MonadParameter)obj;
						if (monadParameter.IsSwitch)
						{
							stringBuilder.Append(Strings.NameValueFormat(monadParameter.ParameterName, string.Empty));
						}
						else
						{
							stringBuilder.Append(Strings.NameValueFormat(monadParameter.ParameterName, MonadCommand.FormatParameterValue(monadParameter.Value)));
						}
					}
					result = stringBuilder.ToString();
				}
				return result;
			}
		}

		private MonadConnection connection;

		private MonadCommand saveCommand;

		private ADPropertyBag fields;
	}
}
