using System;
using System.Data;
using System.Data.Common;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Diagnostics.Components.Monad;
using Microsoft.PowerShell.HostingTools;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class MonadConnection : DbConnection
	{
		public MonadConnection() : this("timeout=30")
		{
		}

		public MonadConnection(string connectionString) : this(connectionString, MonadConnection.defaultInteractionHandler)
		{
		}

		public MonadConnection(string connectionString, CommandInteractionHandler uiHandler) : this(connectionString, uiHandler, null)
		{
		}

		public MonadConnection(string connectionString, CommandInteractionHandler uiHandler, RunspaceServerSettingsPresentationObject serverSettings) : this(connectionString, uiHandler, serverSettings, null)
		{
		}

		public MonadConnection(string connectionString, CommandInteractionHandler uiHandler, RunspaceServerSettingsPresentationObject serverSettings, MonadConnectionInfo connectionInfo)
		{
			this.SyncRoot = new object();
			base..ctor();
			ExTraceGlobals.IntegrationTracer.Information<string>((long)this.GetHashCode(), "new MonadConnection({0})", connectionString);
			if (uiHandler == null)
			{
				throw new ArgumentNullException("uiHandler");
			}
			if (string.IsNullOrEmpty(connectionString))
			{
				throw new ArgumentException("Argument 'connectionString' was null or emtpy.");
			}
			this.pooled = true;
			this.timeout = 0;
			this.ConnectionString = connectionString;
			this.InteractionHandler = uiHandler;
			this.state = ConnectionState.Closed;
			if (connectionInfo != null)
			{
				this.remote = true;
				if (this.pooled)
				{
					this.mediator = MonadConnection.mediatorPool.GetRunspacePooledMediatorInstance(new MonadMediatorPoolKey(connectionInfo, serverSettings));
				}
				else
				{
					this.mediator = new RunspaceMediator(new MonadRemoteRunspaceFactory(connectionInfo, serverSettings), new EmptyRunspaceCache());
				}
			}
			else if (this.pooled)
			{
				this.mediator = MonadConnection.mediatorPool.GetRunspacePooledMediatorInstance();
			}
			else
			{
				this.mediator = MonadConnection.mediatorPool.GetRunspaceNonPooledMediatorInstance();
			}
			this.serverSettings = serverSettings;
		}

		internal MonadConnection(CommandInteractionHandler uiHandler, RunspaceMediator mediator) : this(uiHandler, mediator, null)
		{
		}

		internal MonadConnection(CommandInteractionHandler uiHandler, RunspaceMediator mediator, RunspaceServerSettingsPresentationObject serverSettings)
		{
			this.SyncRoot = new object();
			base..ctor();
			ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "new MonadConnection(RunspaceMediator)");
			if (uiHandler == null)
			{
				throw new ArgumentNullException("uiHandler");
			}
			if (mediator == null)
			{
				throw new ArgumentNullException("mediator");
			}
			this.pooled = true;
			this.timeout = 0;
			this.InteractionHandler = uiHandler;
			this.mediator = mediator;
			this.state = ConnectionState.Closed;
			this.serverSettings = serverSettings;
		}

		public override event StateChangeEventHandler StateChange;

		internal static event EventHandler Test_StateChanged;

		public CommandInteractionHandler InteractionHandler
		{
			get
			{
				return this.commandInteractionHandler;
			}
			set
			{
				if (value == null)
				{
					value = MonadConnection.defaultInteractionHandler;
				}
				this.commandInteractionHandler = value;
			}
		}

		public override string ConnectionString
		{
			get
			{
				return this.connectionString;
			}
			set
			{
				ExTraceGlobals.IntegrationTracer.Information<string>((long)this.GetHashCode(), "-->MonadConnection.ConnectionString={0}", value);
				string[] array = value.Split(new char[]
				{
					'='
				});
				if (array.Length != 2)
				{
					throw new ArgumentException(Strings.ExceptionMDAInvalidConnectionString(value), value);
				}
				if (array[0].Trim().ToLowerInvariant() == "timeout")
				{
					int num = int.Parse(array[1].Trim());
					if (num < 0)
					{
						throw new ArgumentOutOfRangeException("timeout", num, Strings.InvalidNegativeValue("timeout"));
					}
					this.timeout = num;
				}
				else
				{
					if (!(array[0].Trim().ToLowerInvariant() == "pooled"))
					{
						throw new ArgumentException(Strings.ExceptionMDAInvalidConnectionString(value), value);
					}
					if (bool.Parse(array[1].Trim()))
					{
						throw new ArgumentException(Strings.ExceptionMDAInvalidConnectionString(value), value);
					}
					this.pooled = false;
				}
				this.connectionString = value;
				ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "<--MonadConnection.ConnectionString");
			}
		}

		public override int ConnectionTimeout
		{
			get
			{
				return this.timeout;
			}
		}

		public bool IsPooled
		{
			get
			{
				return this.pooled;
			}
		}

		public override string ServerVersion
		{
			get
			{
				return MonadHost.ServerVersion;
			}
		}

		public override ConnectionState State
		{
			get
			{
				if (this.state == ConnectionState.Open)
				{
					if (this.RunspaceProxy.State == RunspaceState.Broken)
					{
						this.state = ConnectionState.Broken;
					}
					else if (this.RunspaceProxy.State == RunspaceState.Closed || this.RunspaceProxy.State == RunspaceState.Closing)
					{
						this.state = ConnectionState.Closed;
					}
				}
				return this.state;
			}
		}

		public override string Database
		{
			get
			{
				return string.Empty;
			}
		}

		public override string DataSource
		{
			get
			{
				return string.Empty;
			}
		}

		internal bool IsRemote
		{
			get
			{
				return this.remote;
			}
		}

		internal RunspaceProxy RunspaceProxy
		{
			get
			{
				return this.proxy;
			}
		}

		internal MonadCommand CurrentCommand
		{
			get
			{
				return this.currentCommand;
			}
			set
			{
				this.currentCommand = value;
			}
		}

		public override void Open()
		{
			ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "-->MonadConnection.Open");
			lock (this.SyncRoot)
			{
				if (this.state != ConnectionState.Closed)
				{
					throw new InvalidOperationException(Strings.ExceptionMDAConnectionAlreadyOpened);
				}
				this.proxy = new RunspaceProxy(this.mediator);
				if (this.serverSettings != null && !this.IsRemote)
				{
					this.InitializeLocalServerSettings(this.serverSettings);
				}
				this.SetState(ConnectionState.Open);
			}
			ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "<--MonadConnection.Open");
		}

		public override void Close()
		{
			this.Close(false);
		}

		public override void ChangeDatabase(string databaseName)
		{
			throw new NotImplementedException();
		}

		internal void NotifyExecutionStarting()
		{
			ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "-->MonadConnection.NotifyExecutionStarting");
			lock (this.SyncRoot)
			{
				if (this.state == ConnectionState.Closed)
				{
					throw new InvalidOperationException(Strings.ExceptionMDAConnectionMustBeOpened);
				}
				if (ConnectionState.Open != this.state)
				{
					throw new InvalidOperationException(Strings.ExceptionMDACommandAlreadyExecuting);
				}
				this.SetState(ConnectionState.Open | ConnectionState.Connecting);
			}
			ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "<--MonadConnection.NotifyExecutionStarting");
		}

		internal void NotifyExecutionStarted()
		{
			ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "-->MonadConnection.NotifyExecutionStarted");
			lock (this.SyncRoot)
			{
				if (this.state == ConnectionState.Closed)
				{
					throw new InvalidOperationException(Strings.ExceptionMDAConnectionMustBeOpened);
				}
				if ((ConnectionState.Executing & this.state) != ConnectionState.Closed)
				{
					throw new InvalidOperationException(Strings.ExceptionMDACommandAlreadyExecuting);
				}
				this.SetState(ConnectionState.Open | ConnectionState.Executing);
			}
			ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "<--MonadConnection.NotifyExecutionStarted");
		}

		internal void NotifyExecutionFinished()
		{
			ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "-->MonadConnection.NotifyExecutionFinished");
			lock (this.SyncRoot)
			{
				if (this.state == ConnectionState.Closed)
				{
					throw new InvalidOperationException(Strings.ExceptionMDAConnectionMustBeOpened);
				}
				if ((ConnectionState.Executing & this.state) == ConnectionState.Closed)
				{
					throw new InvalidOperationException(Strings.ExceptionMDACommandNotExecuting);
				}
				this.SetState(ConnectionState.Open);
			}
			ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "<--MonadConnection.NotifyExecutionFinished");
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.Close(true);
			}
			base.Dispose(disposing);
		}

		protected override DbCommand CreateDbCommand()
		{
			return new MonadCommand();
		}

		protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
		{
			throw new NotSupportedException();
		}

		private bool ShouldSerializeInteractionHandler()
		{
			return this.InteractionHandler != MonadConnection.defaultInteractionHandler;
		}

		private void ResetInteractionHandler()
		{
			this.InteractionHandler = MonadConnection.defaultInteractionHandler;
		}

		private void Close(bool force)
		{
			ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "-->MonadConnection.Close");
			lock (this.SyncRoot)
			{
				if (this.state != ConnectionState.Closed)
				{
					if (!force && (ConnectionState.Executing & this.state) != ConnectionState.Closed)
					{
						throw new InvalidOperationException(Strings.ExceptionMDACommandStillExecuting);
					}
					if (this.serverSettings != null && !this.IsRemote)
					{
						this.CleanUpLocalServerSettings();
					}
					this.proxy.Dispose();
					this.proxy = null;
					this.SetState(ConnectionState.Closed);
				}
			}
			ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "<--MonadConnection.Close");
		}

		private void SetState(ConnectionState state)
		{
			ExTraceGlobals.VerboseTracer.Information<ConnectionState>((long)this.GetHashCode(), "-->MonadConnection.SetState({0})", state);
			ConnectionState originalState = this.state;
			this.state = state;
			if (this.StateChange != null)
			{
				ExTraceGlobals.VerboseTracer.Information((long)this.GetHashCode(), "\tInvoking event subscribers.");
				this.StateChange(this, new StateChangeEventArgs(originalState, state));
			}
			ExTraceGlobals.VerboseTracer.Information((long)this.GetHashCode(), "<--MonadConnection.SetState()");
			if (MonadConnection.Test_StateChanged != null)
			{
				MonadConnection.Test_StateChanged(this, EventArgs.Empty);
			}
		}

		private void InitializeLocalServerSettings(RunspaceServerSettingsPresentationObject serverSettings)
		{
			PSCommand pscommand = new PSCommand().AddCommand("Set-ADServerSettings");
			pscommand.AddParameter("RunspaceServerSettings", serverSettings);
			using (PowerShell powerShell = this.proxy.CreatePowerShell(pscommand))
			{
				try
				{
					powerShell.Invoke();
				}
				catch (CmdletInvocationException ex)
				{
					if (ex.InnerException != null)
					{
						throw ex.InnerException;
					}
					throw;
				}
				if (powerShell.Streams.Error.Count > 0)
				{
					ErrorRecord errorRecord = powerShell.Streams.Error[0];
					throw new CmdletInvocationException(errorRecord.Exception.Message, errorRecord.Exception);
				}
			}
		}

		private void CleanUpLocalServerSettings()
		{
			EngineIntrinsics engineIntrinsics = this.RunspaceProxy.GetVariable("ExecutionContext") as EngineIntrinsics;
			if (engineIntrinsics != null)
			{
				ISessionState sessionState = new PSSessionState(engineIntrinsics.SessionState);
				if (ExchangePropertyContainer.IsContainerInitialized(sessionState))
				{
					ExchangePropertyContainer.SetServerSettings(sessionState, null);
				}
			}
			this.RunspaceProxy.SetVariable(ExchangePropertyContainer.ADServerSettingsVarName, null);
		}

		public const string DefaultConnectionString = "timeout=30";

		public const string NonpooledConnectionString = "pooled=false";

		private const string TimeoutKey = "timeout";

		private const string PooledKey = "pooled";

		private static readonly CommandInteractionHandler defaultInteractionHandler = new CommandInteractionHandler();

		private static MonadMediatorPool mediatorPool = new MonadMediatorPool(3, TimeSpan.FromMinutes(5.0));

		private ConnectionState state;

		private RunspaceProxy proxy;

		private object SyncRoot;

		private string connectionString;

		private int timeout;

		private bool pooled;

		private bool remote;

		private CommandInteractionHandler commandInteractionHandler;

		private RunspaceMediator mediator;

		private RunspaceServerSettingsPresentationObject serverSettings;

		private MonadCommand currentCommand;
	}
}
