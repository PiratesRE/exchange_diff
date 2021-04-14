using System;
using System.Management.Automation;
using System.Reflection;
using System.Text;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SetupSingleTaskDataHandler : ExchangeDataHandler
	{
		public SetupSingleTaskDataHandler(ISetupContext context, MonadCommand command) : base(command)
		{
			this.SetupContext = context;
			this.WorkUnit.CanShowExecutedCommand = false;
			this.ImplementsDatacenterMode = false;
			this.ImplementsDatacenterDedicatedMode = false;
			this.ImplementsPartnerHostedMode = false;
		}

		public SetupSingleTaskDataHandler(ISetupContext context, string commandText, MonadConnection connection)
		{
			this.Command = new LoggableMonadCommand(commandText, connection);
			this.Connection = this.Command.Connection;
			if (this.Connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			this.Fields = new ADPropertyBag();
			base.DataSource = this;
			this.SetupContext = context;
			this.WorkUnit.CanShowExecutedCommand = false;
			this.ImplementsDatacenterMode = false;
			this.ImplementsDatacenterDedicatedMode = false;
			this.ImplementsPartnerHostedMode = false;
		}

		protected ISetupContext SetupContext { get; set; }

		public bool ImplementsDatacenterMode { get; protected set; }

		public bool ImplementsDatacenterDedicatedMode { get; protected set; }

		public bool ImplementsPartnerHostedMode { get; protected set; }

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

		internal MonadConnection Connection { get; private set; }

		internal MonadCommand Command { get; private set; }

		internal MonadParameterCollection Parameters
		{
			get
			{
				return this.Command.Parameters;
			}
		}

		public string CommandText
		{
			get
			{
				return this.Command.CommandText;
			}
			set
			{
				this.Command.CommandText = value;
			}
		}

		internal bool KeepInstanceParamerter { get; set; }

		internal ADPropertyBag Fields { get; private set; }

		protected virtual void OnSaveData()
		{
			if (!string.IsNullOrEmpty(this.CommandText))
			{
				try
				{
					SetupLogger.Log(SetupLogger.HalfAsterixLine);
					SetupLogger.Log(Strings.WillExecuteHighLevelTask(this.CommandText));
					this.startTime = DateTime.UtcNow;
					SetupLogger.IncreaseIndentation(Strings.HighLevelTaskStarted(this.GetCommandString()));
					base.SavedResults.Clear();
					this.Command.ProgressReport += base.OnProgressReport;
					try
					{
						object[] array = this.Command.Execute(base.WorkUnits.ToArray());
						if (array != null)
						{
							base.SavedResults.AddRange(array);
						}
					}
					finally
					{
						this.Command.ProgressReport -= base.OnProgressReport;
					}
				}
				finally
				{
					SetupLogger.DecreaseIndentation();
					SetupLogger.LogForDataMining(this.GetCommandString(), this.startTime);
				}
			}
		}

		protected virtual void AddParameters()
		{
			SetupLogger.TraceEnter(new object[0]);
			this.Parameters.Clear();
			if (this.ImplementsDatacenterMode && this.SetupContext.IsDatacenter)
			{
				this.Parameters.AddWithValue("IsDatacenter", true);
			}
			if (this.ImplementsDatacenterDedicatedMode && this.SetupContext.IsDatacenterDedicated)
			{
				this.Parameters.AddWithValue("IsDatacenterDedicated", true);
			}
			if (this.ImplementsPartnerHostedMode && this.SetupContext.IsPartnerHosted)
			{
				this.Parameters.AddWithValue("IsPartnerHosted", true);
			}
			SetupLogger.TraceExit();
		}

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
				this.WorkUnit.Text = this.Command.CommandText;
				this.WorkUnit.Description = stringBuilder.ToString();
			}
			base.UpdateWorkUnits();
		}

		public override void Cancel()
		{
			base.Cancel();
			this.Command.Cancel();
		}

		public string GetCommandString()
		{
			if (this.Command == null)
			{
				return null;
			}
			return this.Command.ToString();
		}

		internal override string CommandToRun
		{
			get
			{
				return this.ClonePreparedCommand() + "\r\n";
			}
		}

		private string ClonePreparedCommand()
		{
			MonadParameter[] array = new MonadParameter[this.Parameters.Count];
			this.Parameters.CopyTo(array, 0);
			MonadCommand monadCommand = null;
			string result;
			try
			{
				MonadParameter[] values = this.PrepareParameters();
				this.Parameters.Clear();
				this.Parameters.AddRange(values);
				monadCommand = this.Command.Clone();
				result = monadCommand + "\r\n";
				this.modifiedParametersDescription = string.Empty;
				if (this.IsModified())
				{
					StringBuilder stringBuilder = new StringBuilder();
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
					this.modifiedParametersDescription = stringBuilder.ToString();
				}
			}
			finally
			{
				this.Parameters.Clear();
				this.Parameters.AddRange(array);
				if (monadCommand != null)
				{
					monadCommand.Dispose();
				}
			}
			return result;
		}

		public override string ModifiedParametersDescription
		{
			get
			{
				this.ClonePreparedCommand();
				return this.modifiedParametersDescription;
			}
		}

		private string modifiedParametersDescription;

		private DateTime startTime;
	}
}
