using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics.Components.Monad;
using Microsoft.PowerShell.HostingTools;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class MonadCommand : DbCommand, ICloneable
	{
		public MonadCommand()
		{
			ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "new MonadCommand()");
			this.CommandType = CommandType.StoredProcedure;
		}

		public MonadCommand(string cmdText) : this()
		{
			this.CommandText = cmdText;
		}

		public MonadCommand(string cmdText, MonadConnection connection) : this(cmdText)
		{
			this.Connection = connection;
		}

		private MonadCommand(MonadCommand from)
		{
			this.commandText = from.commandText;
			this.commandType = from.commandType;
			this.commandTimeout = from.commandTimeout;
			this.connection = from.connection;
			MonadParameterCollection parameters = this.Parameters;
			foreach (object obj in from.Parameters)
			{
				ICloneable cloneable = (ICloneable)obj;
				parameters.Add(cloneable.Clone());
			}
			this.ErrorReport += from.ErrorReport;
			this.WarningReport += from.WarningReport;
			this.StartExecution += from.StartExecution;
			this.EndExecution += from.EndExecution;
		}

		public event EventHandler<ProgressReportEventArgs> ProgressReport;

		public event EventHandler<ErrorReportEventArgs> ErrorReport;

		public event EventHandler<WarningReportEventArgs> WarningReport;

		public event EventHandler<StartExecutionEventArgs> StartExecution;

		public event EventHandler<RunGuidEventArgs> EndExecution;

		[DefaultValue(null)]
		public new MonadConnection Connection
		{
			get
			{
				return this.connection;
			}
			set
			{
				ExTraceGlobals.IntegrationTracer.Information<string>((long)this.GetHashCode(), "MonadCommand.set_Connection({0})", (value == null) ? null : value.ConnectionString);
				if (this.pipelineProxy != null)
				{
					throw new InvalidOperationException("Cannot change the connection while a command is executing.");
				}
				this.connection = value;
			}
		}

		[DefaultValue(null)]
		public override string CommandText
		{
			get
			{
				return this.commandText;
			}
			set
			{
				ExTraceGlobals.IntegrationTracer.Information<string>((long)this.GetHashCode(), "MonadCommand.set_CommandText({0})", value);
				if (this.pipelineProxy != null)
				{
					throw new InvalidOperationException("Cannot change the command text while a command is executing.");
				}
				this.commandText = value;
				ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "<--MonadCommand.set_CommandText()");
			}
		}

		[DefaultValue(30)]
		public override int CommandTimeout
		{
			get
			{
				return this.commandTimeout;
			}
			set
			{
				ExTraceGlobals.IntegrationTracer.Information<int>((long)this.GetHashCode(), "-->MonadCommand.set_CommandTimeout({0})", value);
				if (value < 0)
				{
					throw new ArgumentException();
				}
				if (this.pipelineProxy != null)
				{
					throw new InvalidOperationException("Cannot change the command type while a command is executing.");
				}
				this.commandTimeout = value;
				ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "<--MonadCommand.set_CommandTimeout()");
			}
		}

		[DefaultValue(CommandType.StoredProcedure)]
		public override CommandType CommandType
		{
			get
			{
				return this.commandType;
			}
			set
			{
				ExTraceGlobals.IntegrationTracer.Information<CommandType>((long)this.GetHashCode(), "-->MonadCommand.set_CommandType({0})", value);
				if (value != CommandType.StoredProcedure && value != CommandType.Text)
				{
					throw new ArgumentException("Only StoredProcedure and Text modes are supported.");
				}
				if (value == CommandType.Text && this.Connection.IsPooled && !this.connection.IsRemote)
				{
					throw new ArgumentException("Scripts can only be executed on non-pooled connections.");
				}
				if (this.pipelineProxy != null)
				{
					throw new InvalidOperationException("Cannot change the command type while a command is executing.");
				}
				this.commandType = value;
				ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "<--MonadCommand.set_CommandType()");
			}
		}

		public override UpdateRowSource UpdatedRowSource
		{
			get
			{
				return this.updatedRowSource;
			}
			set
			{
				this.updatedRowSource = value;
			}
		}

		[DefaultValue(false)]
		public override bool DesignTimeVisible
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public new MonadParameterCollection Parameters
		{
			get
			{
				return this.parameterCollection;
			}
		}

		internal Guid CommandGuid
		{
			get
			{
				return this.guid;
			}
		}

		internal string PreservedObjectProperty
		{
			get
			{
				return this.preservedObjectProperty;
			}
			set
			{
				this.preservedObjectProperty = value;
			}
		}

		internal PowerShell ActivePipeline
		{
			get
			{
				if (this.pipelineProxy == null)
				{
					return null;
				}
				return this.pipelineProxy.PowerShell;
			}
		}

		protected override DbConnection DbConnection
		{
			get
			{
				return this.Connection;
			}
			set
			{
				this.Connection = (MonadConnection)value;
			}
		}

		protected override DbTransaction DbTransaction
		{
			get
			{
				return null;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		protected override DbParameterCollection DbParameterCollection
		{
			get
			{
				return this.Parameters;
			}
		}

		public override int ExecuteNonQuery()
		{
			object[] array = this.Execute();
			int num = array.Length;
			if (num == 0)
			{
				num = -1;
			}
			return num;
		}

		public override object ExecuteScalar()
		{
			object result = null;
			using (IDataReader dataReader = base.ExecuteReader())
			{
				if (0 < dataReader.FieldCount && dataReader.Read())
				{
					result = dataReader.GetValue(0);
				}
			}
			return result;
		}

		public MonadAsyncResult BeginExecute(IEnumerable input)
		{
			IEnumerable enumerable;
			this.CreatePipeline(input, this.GetPipelineCommand(out enumerable), enumerable);
			return this.BeginExecute(enumerable != null);
		}

		public MonadAsyncResult BeginExecute(WorkUnit[] workUnits)
		{
			IEnumerable enumerable;
			this.CreatePipeline(workUnits, this.GetPipelineCommand(out enumerable));
			return this.BeginExecute(false);
		}

		public object[] EndExecute(MonadAsyncResult asyncResult)
		{
			ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "-->MonadCommand.EndExecute()");
			if (asyncResult.RunningCommand != this)
			{
				throw new ArgumentException("Parameter does not correspond to this command.", "asyncResult");
			}
			List<object> list = null;
			try
			{
				Collection<PSObject> collection = this.pipelineProxy.EndInvoke(asyncResult);
				bool flag = false;
				if (this.Connection.IsRemote && collection.Count > 0 && collection[0] != null && collection[0].BaseObject != null && collection[0].BaseObject is PSCustomObject)
				{
					flag = MonadCommand.CanDeserialize(collection[0]);
				}
				list = new List<object>(collection.Count);
				ExTraceGlobals.IntegrationTracer.Information<int>((long)this.GetHashCode(), "\tPipeline contains {0} results", collection.Count);
				for (int i = 0; i < collection.Count; i++)
				{
					if (collection[i] == null)
					{
						ExTraceGlobals.VerboseTracer.Information<int>((long)this.GetHashCode(), "\tPipeline contains a null result at position {0}", i);
					}
					else
					{
						if (collection[i].BaseObject == null)
						{
							throw new InvalidOperationException("Pure PSObjects are not supported.");
						}
						if (!this.Connection.IsRemote)
						{
							list.Add(collection[i].BaseObject);
						}
						else if (flag)
						{
							list.Add(MonadCommand.Deserialize(collection[i]));
						}
						else
						{
							list.Add(collection[i]);
						}
					}
				}
			}
			finally
			{
				this.Connection.NotifyExecutionFinished();
				if (this.EndExecution != null)
				{
					this.EndExecution(this, new RunGuidEventArgs(this.guid));
				}
				this.Connection.CurrentCommand = null;
				this.pipelineProxy = null;
			}
			ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "<--MonadCommand.EndExecute()");
			return list.ToArray();
		}

		public object[] Execute()
		{
			return this.Execute(null);
		}

		public object[] Execute(object[] pipelineInput)
		{
			ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "-->MonadCommand.Execute(pipelineInput)");
			MonadAsyncResult asyncResult = this.BeginExecute(pipelineInput);
			object[] result = this.EndExecute(asyncResult);
			ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "<--MonadCommand.Execute()");
			return result;
		}

		public object[] Execute(WorkUnit[] workUnits)
		{
			ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "-->MonadCommand.Execute(workUnits)");
			MonadAsyncResult asyncResult = this.BeginExecute(workUnits);
			object[] result = this.EndExecute(asyncResult);
			ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "<--MonadCommand.Execute()");
			return result;
		}

		public override void Cancel()
		{
			ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "-->MonadCommand.Cancel()");
			PowerShell activePipeline = this.ActivePipeline;
			if (activePipeline != null && activePipeline.InvocationStateInfo.State != PSInvocationState.NotStarted)
			{
				ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "\tStopping the pipeline.");
				activePipeline.Stop();
			}
			ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "<--MonadCommand.Cancel()");
		}

		public override void Prepare()
		{
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(this.CommandText);
			foreach (object obj in this.Parameters)
			{
				MonadParameter monadParameter = (MonadParameter)obj;
				if (monadParameter.IsSwitch)
				{
					stringBuilder.Append(" -" + monadParameter.ParameterName);
				}
				else
				{
					string text = MonadCommand.FormatParameterValue(monadParameter.Value);
					if (!string.IsNullOrEmpty(text))
					{
						stringBuilder.Append(" -" + monadParameter.ParameterName + " " + text.ToString());
					}
					else
					{
						stringBuilder.Append(" -" + monadParameter.ParameterName + " ''");
					}
				}
			}
			return stringBuilder.ToString();
		}

		public MonadCommand Clone()
		{
			return new MonadCommand(this);
		}

		object ICloneable.Clone()
		{
			return this.Clone();
		}

		internal static string FormatParameterValue(object value)
		{
			if (value != null && value is IList)
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (value is byte[])
				{
					stringBuilder.AppendFormat("'{0}'", Strings.BinaryDataStakeHodler);
				}
				else
				{
					IList list = (IList)value;
					for (int i = 0; i < list.Count - 1; i++)
					{
						stringBuilder.Append(MonadCommand.FormatNonListParameterValue(list[i]) + ",");
					}
					if (list.Count > 0)
					{
						stringBuilder.Append(MonadCommand.FormatNonListParameterValue(list[list.Count - 1]));
					}
					else
					{
						stringBuilder.Append("@()");
					}
				}
				return stringBuilder.ToString();
			}
			return MonadCommand.FormatNonListParameterValue(value);
		}

		internal static PSDataCollection<object> Serialize(IEnumerable collection)
		{
			PSDataCollection<object> psdataCollection = new PSDataCollection<object>();
			if (collection != null)
			{
				foreach (object obj in collection)
				{
					if (MonadCommand.CanSerialize(obj))
					{
						psdataCollection.Add(MonadCommand.Serialize(obj));
					}
					else if (obj is Enum)
					{
						psdataCollection.Add(obj.ToString());
					}
					else
					{
						psdataCollection.Add(obj);
					}
				}
			}
			psdataCollection.Complete();
			return psdataCollection;
		}

		internal static PSObject Serialize(object obj)
		{
			if (obj is MapiFolderPath)
			{
				return new PSObject(obj.ToString());
			}
			PSObject psobject = new PSObject(obj);
			PSNoteProperty member = new PSNoteProperty("SerializationData", SerializationTypeConverter.GetSerializationData(psobject));
			psobject.Properties.Add(member);
			psobject.TypeNames.Insert(0, "Deserialized." + obj.GetType().FullName);
			return psobject;
		}

		internal static bool CanSerialize(object obj)
		{
			return SerializationTypeConverter.CanSerialize(obj);
		}

		internal static Exception DeserializeException(Exception ex)
		{
			RemoteException ex2 = ex as RemoteException;
			if (ex2 != null && MonadCommand.CanDeserialize(ex2.SerializedRemoteException))
			{
				return (MonadCommand.Deserialize(ex2.SerializedRemoteException) as Exception) ?? ex;
			}
			return ex;
		}

		internal static bool CanDeserialize(PSObject psObject)
		{
			if (psObject == null || psObject.Members["SerializationData"] == null || psObject.Members["SerializationData"].Value == null)
			{
				return false;
			}
			Type destinationType = MonadCommand.ResolveType(psObject);
			return MonadCommand.TypeConverter.CanConvertFrom(psObject, destinationType);
		}

		internal static Type ResolveType(PSObject psObject)
		{
			if (psObject == null)
			{
				throw new ArgumentNullException("psObject");
			}
			lock (MonadCommand.syncInstance)
			{
				if (MonadCommand.typeDictionary.ContainsKey(psObject.TypeNames[0]))
				{
					return MonadCommand.typeDictionary[psObject.TypeNames[0]];
				}
			}
			string text = psObject.TypeNames[0].Substring("Deserialized.".Length);
			Type type = null;
			try
			{
				type = (Type)LanguagePrimitives.ConvertTo(text, typeof(Type));
			}
			catch (PSInvalidCastException)
			{
				type = MonadCommand.ResolvePSType(psObject, text);
				if (type == null)
				{
					throw;
				}
			}
			lock (MonadCommand.syncInstance)
			{
				if (!MonadCommand.typeDictionary.ContainsKey(psObject.TypeNames[0]))
				{
					MonadCommand.typeDictionary.Add(psObject.TypeNames[0], type);
				}
			}
			return type;
		}

		internal static Type ResolvePSType(PSObject psObject, string typeName)
		{
			if (!typeName.StartsWith("System.Management.Automation", StringComparison.OrdinalIgnoreCase))
			{
				return null;
			}
			Assembly assembly = Assembly.GetAssembly(psObject.GetType());
			if (assembly != null)
			{
				return assembly.GetType(typeName);
			}
			return null;
		}

		internal static object Deserialize(PSObject psObject)
		{
			if (psObject == null)
			{
				throw new ArgumentNullException("psObject");
			}
			if (psObject.Members["SerializationData"] != null && psObject.Members["SerializationData"].Value == null)
			{
				throw new ArgumentException("Cannot deserialize PSObject, SerializationData is missing.");
			}
			Type type = MonadCommand.ResolveType(psObject);
			if (psObject.Members["EMCMockEngineEnabled"] != null)
			{
				return MockObjectInformation.TranslateToMockObject(type, psObject);
			}
			return MonadCommand.TypeConverter.ConvertFrom(psObject, type, null, true);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.pipelineProxy != null)
			{
				this.pipelineProxy.Dispose();
				this.pipelineProxy = null;
			}
			base.Dispose(disposing);
		}

		protected override DbParameter CreateDbParameter()
		{
			return new MonadParameter();
		}

		protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
		{
			ExTraceGlobals.IntegrationTracer.Information<CommandBehavior>((long)this.GetHashCode(), "-->MonadCommand.ExecuteReader({0})", behavior);
			MonadAsyncResult asyncResult = this.BeginExecute(null);
			MonadDataReader result = new MonadDataReader(this, behavior, asyncResult, this.preservedObjectProperty);
			ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "<--MonadCommand.ExecuteReader()");
			return result;
		}

		private static string FormatNonListParameterValue(object value)
		{
			string result = string.Empty;
			if (value == null)
			{
				result = MonadCommand.nullString;
			}
			else
			{
				Type type = value.GetType();
				if (type == typeof(bool))
				{
					result = (((bool)value) ? MonadCommand.trueString : MonadCommand.falseString);
				}
				else
				{
					result = string.Format("'{0}'", (value.ToString() != null) ? value.ToString().Replace("'", "''") : value.ToString());
				}
			}
			return result;
		}

		private MonadAsyncResult BeginExecute(bool armedPipelineInputFromScript)
		{
			ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "-->MonadCommand.BeginExecute()");
			if (this.Connection == null)
			{
				throw new InvalidOperationException("Connection must be set before executing the command.");
			}
			this.Connection.NotifyExecutionStarting();
			this.Connection.CurrentCommand = this;
			IAsyncResult asyncResult = this.pipelineProxy.BeginInvoke(null, null);
			this.Connection.NotifyExecutionStarted();
			this.guid = Guid.NewGuid();
			if (this.StartExecution != null)
			{
				this.StartExecution(this, new StartExecutionEventArgs(this.CommandGuid, armedPipelineInputFromScript ? null : this.pipelineProxy.Input));
			}
			ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "<--MonadCommand.BeginExecute()");
			return (MonadAsyncResult)asyncResult;
		}

		private PSCommand GetPipelineCommand(out IEnumerable pipelineInputFromScript)
		{
			ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "-->MonadCommand.GetPipelineCommand()");
			if (this.CommandText.Contains(" ") && this.CommandType == CommandType.StoredProcedure)
			{
				throw new InvalidOperationException("CommandType.StoredProcedure cannot be used to run scripts. Add any parameters to the Parameters collection.");
			}
			ExTraceGlobals.IntegrationTracer.Information<string>((long)this.GetHashCode(), "\tCreating command {0}", this.CommandText);
			bool flag = this.CommandType == CommandType.Text;
			PSCommand pscommand = null;
			pipelineInputFromScript = null;
			if (this.Connection != null && this.Connection.IsRemote && this.CommandType == CommandType.Text && MonadCommand.scriptRegex.IsMatch(this.CommandText))
			{
				this.ConvertScriptToStoreProcedure(this.CommandText, out pscommand, out pipelineInputFromScript);
			}
			else
			{
				pscommand = new PSCommand().AddCommand(new Command(this.CommandText, flag, !flag));
				foreach (object obj in this.Parameters)
				{
					MonadParameter monadParameter = (MonadParameter)obj;
					if (ParameterDirection.Input != monadParameter.Direction)
					{
						throw new InvalidOperationException("ParameterDirection.Input is the only supported parameter type.");
					}
					ExTraceGlobals.IntegrationTracer.Information<string, object>((long)this.GetHashCode(), "\tAdding parameter {0} = {1}", monadParameter.ParameterName, monadParameter.Value);
					if (this.connection.IsRemote && MonadCommand.CanSerialize(monadParameter.Value))
					{
						if (monadParameter.Value is ICollection)
						{
							pscommand.AddParameter(monadParameter.ParameterName, MonadCommand.Serialize(monadParameter.Value as IEnumerable));
						}
						else
						{
							pscommand.AddParameter(monadParameter.ParameterName, MonadCommand.Serialize(monadParameter.Value));
						}
					}
					else if (monadParameter.IsSwitch)
					{
						pscommand.AddParameter(monadParameter.ParameterName, true);
					}
					else
					{
						pscommand.AddParameter(monadParameter.ParameterName, monadParameter.Value);
					}
				}
			}
			ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "<--MonadCommand.GetPipelineCommand()");
			return pscommand;
		}

		private void CreatePipeline(WorkUnit[] workUnits, PSCommand commands)
		{
			ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "-->MonadCommand.CreatePipeline(workUnits)");
			if (this.pipelineProxy != null)
			{
				throw new InvalidOperationException("The command is already executing.");
			}
			if (this.Connection == null || (this.Connection.State & ConnectionState.Open) == ConnectionState.Closed)
			{
				throw new InvalidOperationException("The command requires an open connection.");
			}
			if (this.connection.IsRemote)
			{
				using (PSDataCollection<object> powerShellInput = WorkUnitBase.GetPowerShellInput<object>(workUnits))
				{
					this.pipelineProxy = new MonadPipelineProxy(this.Connection.RunspaceProxy, MonadCommand.Serialize(powerShellInput), commands, workUnits);
					goto IL_A7;
				}
			}
			this.pipelineProxy = new MonadPipelineProxy(this.Connection.RunspaceProxy, WorkUnitBase.GetPowerShellInput<object>(workUnits), commands, workUnits);
			IL_A7:
			this.InitializePipelineProxy();
			ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "<--MonadCommand.CreatePipeline(workUnits)");
		}

		private void CreatePipeline(IEnumerable pipelineInput, PSCommand commands, IEnumerable pipelineInputFromScript)
		{
			ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "-->MonadCommand.CreatePipeline(input)");
			if (this.pipelineProxy != null)
			{
				throw new InvalidOperationException("The command is already executing.");
			}
			if (this.Connection == null || (this.Connection.State & ConnectionState.Open) == ConnectionState.Closed)
			{
				throw new InvalidOperationException("The command requires an open connection.");
			}
			if (pipelineInputFromScript != null)
			{
				this.pipelineProxy = new MonadPipelineProxy(this.Connection.RunspaceProxy, pipelineInputFromScript, commands);
			}
			else if (pipelineInput != null)
			{
				if (this.connection.IsRemote)
				{
					this.pipelineProxy = new MonadPipelineProxy(this.Connection.RunspaceProxy, MonadCommand.Serialize(pipelineInput), commands);
				}
				else
				{
					this.pipelineProxy = new MonadPipelineProxy(this.Connection.RunspaceProxy, pipelineInput, commands);
				}
			}
			else
			{
				this.pipelineProxy = new MonadPipelineProxy(this.Connection.RunspaceProxy, commands);
			}
			this.InitializePipelineProxy();
			ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "<--MonadCommand.CreatePipeline(input)");
		}

		private void InitializePipelineProxy()
		{
			if (this.pipelineProxy == null)
			{
				throw new InvalidOperationException("The PipelineProxy must be created before calling Initialize.");
			}
			this.pipelineProxy.InteractionHandler = this.Connection.InteractionHandler;
			this.pipelineProxy.Command = this;
			this.pipelineProxy.ErrorReport += this.ErrorReport;
			this.pipelineProxy.WarningReport += this.WarningReport;
			this.pipelineProxy.ProgressReport += this.ProgressReport;
		}

		private void ConvertScriptToStoreProcedure(string commandText, out PSCommand cmdlet, out IEnumerable pipelineInput)
		{
			Match match = MonadCommand.scriptRegex.Match(commandText);
			cmdlet = new PSCommand();
			CaptureCollection captures = match.Groups["pipelineInput"].Captures;
			object[] array = new object[captures.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = this.Unescape(captures[i].Value);
			}
			pipelineInput = array;
			this.RegisterCommand(match, cmdlet, "firstCmdletName", "firstParameterSet");
			if (!string.IsNullOrEmpty(match.Groups["secondCmdletName"].Value))
			{
				this.RegisterCommand(match, cmdlet, "secondCmdletName", "secondParameterSet");
			}
			if (!string.IsNullOrEmpty(match.Groups["thirdCmdletName"].Value))
			{
				this.RegisterCommand(match, cmdlet, "thirdCmdletName", "thirdParameterSet");
			}
		}

		private PSCommand RegisterCommand(Match match, PSCommand cmdlet, string cmdletNameGroup, string parameterSetGroup)
		{
			cmdlet.AddCommand(new Command(match.Groups[cmdletNameGroup].Value, false, true));
			CaptureCollection captures = match.Groups[parameterSetGroup].Captures;
			string[] array = new string[captures.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = captures[i].Value;
			}
			int j = 0;
			while (j < array.Length)
			{
				if (j == 0 && !array[j].StartsWith("-"))
				{
					cmdlet.AddArgument(array[j]);
					j++;
				}
				else if (j + 1 == array.Length || (j + 1 < array.Length && array[j + 1].StartsWith("-")))
				{
					cmdlet.AddParameter(array[j].Substring(1), true);
					j++;
				}
				else
				{
					cmdlet.AddParameter(array[j].Substring(1), this.UnwrapValue(array[j + 1]));
					j += 2;
				}
			}
			return cmdlet;
		}

		private object UnwrapValue(string value)
		{
			Match match = MonadCommand.valueRegex.Match(value);
			CaptureCollection captures = match.Groups["value"].Captures;
			if (captures.Count == 1)
			{
				return this.Unescape(captures[0].Value);
			}
			IList<object> list = new List<object>();
			for (int i = 0; i < captures.Count; i++)
			{
				list.Add(this.Unescape(captures[i].Value));
			}
			return list;
		}

		private object Unescape(string value)
		{
			string text = value.Replace("''", "'");
			if (string.Equals("$true", text, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			if (string.Equals("$false", text, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			if (string.Equals("$null", text, StringComparison.OrdinalIgnoreCase))
			{
				return null;
			}
			return text;
		}

		private const int DefaultCommandTimeout = 30;

		private const CommandType DefaultCommandType = CommandType.StoredProcedure;

		internal static Regex scriptRegex = new Regex("^\\s*\t\t\t\t\t\t\t\t\t\t\t\t# match the begining of the line and any blank character\r\n                                    # Section for pipelineInput\r\n                                    (\r\n                                        (\r\n                                            '\t\t\t\t\t\t\t\t\t\t\t# any pipeline input object must begin with the character '\r\n                                                (?<pipelineInput>\t\t\t\t\t# put the value to the group pipelineInput, and we will do escape later\r\n                                                    (''|[^'])+\t\t\t\t\t\t# at least one character must be specific, and ' is used to escapte\r\n                                                )\r\n                                            '\t\t\t\t\t\t\t\t\t\t\t# any pipeline input object must end with the character '\r\n                                            \\s*\\,\\s*\t\t\t\t\t\t\t\t\t# if there are more than one pipelineinput, the character , is used to separate them\r\n                                        )*\r\n                                        (\r\n                                            '\t\t\t\t\t\t\t\t\t\t\t# any pipeline input object must begin with the character '\r\n                                                (?<pipelineInput>\t\t\t\t\t# put the value to the group pipelineInput, and we will do escape later\r\n                                                    (''|[^'])+\t\t\t\t\t\t# at least one character must be specific, and ' is used to escapte\r\n                                                )\r\n                                            '\t\t\t\t\t\t\t\t\t\t\t# any pipeline input object must end with the character '\r\n                                        )\t\t\t\t\t\t\t\t\t\t\t\t# Must have one and only one\r\n                                        \\s*\\|\\s*\t\t\t\t\t\t\t\t\t\t# | is the pipeline symbol\r\n                                    ){0,1}\r\n\r\n                                    # First Cmdlet Section\r\n                                    (?<firstCmdletName>\t\t\t\t\t\t# put the first cmdlet into the group firstCmdletName\r\n                                        [\\w,-]+\t\t\t\t\t\t\t\t\t\t# only digits, alphabets, _ and - are allowed for cmdlet name\r\n                                    )\t\t\t\t\t\t\t\t\t\t\t\t\t# match the end of the cmdlet name\r\n                                    \\s*\r\n\r\n                                    # [Optional] Argument\r\n                                    (?<firstParameterSet>\t\t\t\t\t\t# the first position is reserved for an argument, like \\;\r\n                                        '\t\t\t\t\t\t\t\t\t\t\t\t# A value can begin with ', which is used to escape!\r\n                                            (''|[^'])+\t\t\t\t\t\t\t\t# '' will be escaped to a single character '\r\n                                        '?\t\t\t\t\t\t\t\t\t\t\t\t# Another ' is needed to end a value\r\n                                        |\t\t\t\t\t\t\t\t\t\t\t\t# or\r\n                                            [\\w\\\\$]+\t\t\t\t\t\t\t\t\t# another format of a value\r\n                                    ){0,1}\r\n\r\n                                    (\\s+\t\t\t\t\t\t\t\t\t\t\t\t# match any blank character\r\n                                        (\r\n                                            (?<firstParameterSet>\t\t\t\t# put both parameterName and value to the group firstParameterSet\r\n                                                -\\w+\t\t\t\t\t\t\t\t\t# parameterName must begin with the character -\r\n                                            )\r\n\r\n                                            (?<firstParameterSet>\r\n                                                \\s+\r\n                                                (\r\n                                                    (\r\n                                                        '\t\t\t\t\t\t\t\t# A value can begin with ', which is used to escape!\r\n                                                            (''|[^'])+\t\t\t\t# '' will be escaped to a single character '\r\n                                                        '{1}?\t\t\t\t\t\t\t# Another ' is needed to end a value\r\n                                                        |\t\t\t\t\t\t\t\t# or\r\n                                                            [\\w\\\\$]+\t\t\t\t\t# another format of a value\r\n                                                    ),\t\t\t\t\t\t\t\t\t# we also support collection\r\n                                                )*\r\n                                                (\r\n                                                    '\t\t\t\t\t\t\t\t\t# A value can begin with ', which is used to escape!\r\n                                                        (''|[^'])+\t\t\t\t\t# '' will be escaped to a single character '\r\n                                                    '{1}?\t\t\t\t\t\t\t\t# Another ' is needed to end a value\r\n                                                    |\t\t\t\t\t\t\t\t\t# or\r\n                                                        [\\w\\\\$]+\t\t\t\t\t\t# another format of a value\r\n                                                )\t\t\t\t\t\t\t\t\t\t# the end of value\r\n                                            ){0,1}\r\n                                        )\r\n                                    )*\r\n\r\n                                    # Second Cmdlet Section\r\n                                    (\\s*\\|\\s*\\b\r\n                                    (?<secondCmdletName>\t\t\t\t\t# put the first cmdlet into the group firstCmdletName\r\n                                        [\\w,-]+\t\t\t\t\t\t\t\t\t\t# only digits, alphabets, _ and - are allowed for cmdlet name\r\n                                    )\t\t\t\t\t\t\t\t\t\t\t\t\t# match the end of the cmdlet name\r\n                                    \\s*\r\n\r\n                                    # [Optional] Argument\r\n                                    (?<secondParameterSet>\t\t\t\t\t# the first position is reserved for an argument, like \\;\r\n                                        '\t\t\t\t\t\t\t\t\t\t\t\t# A value can begin with ', which is used to escape!\r\n                                            (''|[^'])+\t\t\t\t\t\t\t\t# '' will be escaped to a single character '\r\n                                        '?\t\t\t\t\t\t\t\t\t\t\t\t# Another ' is needed to end a value\r\n                                        |\t\t\t\t\t\t\t\t\t\t\t\t# or\r\n                                            [\\w\\\\$]+\t\t\t\t\t\t\t\t\t# another format of a value\r\n                                    ){0,1}\r\n\r\n                                    (\\s+\t\t\t\t\t\t\t\t\t\t\t\t# match any blank character\r\n                                        (\r\n                                            (?<secondParameterSet>\t\t\t# put both parameterName and value to the group firstParameterSet\r\n                                                -\\w+\t\t\t\t\t\t\t\t\t# parameterName must begin with the character -\r\n                                            )\r\n\r\n                                            (?<secondParameterSet>\r\n                                                \\s+\r\n                                                (\r\n                                                    (\r\n                                                        '\t\t\t\t\t\t\t\t# A value can begin with ', which is used to escape!\r\n                                                            (''|[^'])+\t\t\t\t# '' will be escaped to a single character '\r\n                                                        '{1}?\t\t\t\t\t\t\t# Another ' is needed to end a value\r\n                                                        |\t\t\t\t\t\t\t\t# or\r\n                                                            [\\w\\\\$]+\t\t\t\t\t# another format of a value\r\n                                                    ),\t\t\t\t\t\t\t\t\t# we also support collection\r\n                                                )*\r\n                                                (\r\n                                                    '\t\t\t\t\t\t\t\t\t# A value can begin with ', which is used to escape!\r\n                                                        (''|[^'])+\t\t\t\t\t# '' will be escaped to a single character '\r\n                                                    '{1}?\t\t\t\t\t\t\t\t# Another ' is needed to end a value\r\n                                                    |\t\t\t\t\t\t\t\t\t# or\r\n                                                        [\\w\\\\$]+\t\t\t\t\t\t# another format of a value\r\n                                                )\t\t\t\t\t\t\t\t\t\t# the end of value\r\n                                            ){0,1}\r\n                                        )\r\n                                    )*\r\n                                    )?\r\n\r\n                                    # Third Cmdlet Section\r\n                                    (\\s*\\|\\s*\\b\r\n                                    (?<thirdCmdletName>\t\t\t\t\t\t# put the first cmdlet into the group firstCmdletName\r\n                                        [\\w,-]+\t\t\t\t\t\t\t\t\t\t# only digits, alphabets, _ and - are allowed for cmdlet name\r\n                                    )\t\t\t\t\t\t\t\t\t\t\t\t\t# match the end of the cmdlet name\r\n                                    \\s*\r\n\r\n                                    # [Optional] Argument\r\n                                    (?<thirdParameterSet>\t\t\t\t\t\t# the first position is reserved for an argument, like \\;\r\n                                        '\t\t\t\t\t\t\t\t\t\t\t\t# A value can begin with ', which is used to escape!\r\n                                            (''|[^'])+\t\t\t\t\t\t\t\t# '' will be escaped to a single character '\r\n                                        '?\t\t\t\t\t\t\t\t\t\t\t\t# Another ' is needed to end a value\r\n                                        |\t\t\t\t\t\t\t\t\t\t\t\t# or\r\n                                            [\\w\\\\$]+\t\t\t\t\t\t\t\t\t# another format of a value\r\n                                    ){0,1}\r\n\r\n                                    (\\s+\t\t\t\t\t\t\t\t\t\t\t\t# match any blank character\r\n                                        (\r\n                                            (?<thirdParameterSet>\t\t\t\t# put both parameterName and value to the group firstParameterSet\r\n                                                -\\w+\t\t\t\t\t\t\t\t\t# parameterName must begin with the character -\r\n                                            )\r\n\r\n                                            (?<thirdParameterSet>\r\n                                                \\s+\r\n                                                (\r\n                                                    (\r\n                                                        '\t\t\t\t\t\t\t\t# A value can begin with ', which is used to escape!\r\n                                                            (''|[^'])+\t\t\t\t# '' will be escaped to a single character '\r\n                                                        '{1}?\t\t\t\t\t\t\t# Another ' is needed to end a value\r\n                                                        |\t\t\t\t\t\t\t\t# or\r\n                                                            [\\w\\\\$]+\t\t\t\t\t# another format of a value\r\n                                                    ),\t\t\t\t\t\t\t\t\t# we also support collection\r\n                                                )*\r\n                                                (\r\n                                                    '\t\t\t\t\t\t\t\t\t# A value can begin with ', which is used to escape!\r\n                                                        (''|[^'])+\t\t\t\t\t# '' will be escaped to a single character '\r\n                                                    '{1}?\t\t\t\t\t\t\t\t# Another ' is needed to end a value\r\n                                                    |\t\t\t\t\t\t\t\t\t# or\r\n                                                        [\\w\\\\$]+\t\t\t\t\t\t# another format of a value\r\n                                                )\t\t\t\t\t\t\t\t\t\t# the end of value\r\n                                            ){0,1}\r\n                                        )\r\n                                    )*\r\n                                    )?\r\n                                  \\s*$", RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);

		private static string nullString = "$null";

		private static string trueString = "$true";

		private static string falseString = "$false";

		private static SerializationTypeConverter TypeConverter = new SerializationTypeConverter();

		private static object syncInstance = new object();

		private static Dictionary<string, Type> typeDictionary = new Dictionary<string, Type>();

		private static Regex valueRegex = new Regex("^\\s*\r\n                                        (\r\n                                            (\r\n                                                '\t\t\t\t\t\t\t\t\t\t\t\t# A value can begin with ', which is used to escape!\r\n                                                    (?<value>\r\n                                                        (''|[^'])+\t\t\t\t\t\t\t# '' will be escaped to a single character '\r\n                                                    )\r\n                                                '{1}?\t\t\t\t\t\t\t\t\t\t\t# Another ' is needed to end a value\r\n                                                |\t\t\t\t\t\t\t\t\t\t\t\t# or\r\n                                                (?<value>\r\n                                                    [\\w\\\\$]+\t\t\t\t\t\t\t\t\t# another format of a value\r\n                                                )\r\n                                            ),\t\t\t\t\t\t\t\t\t\t\t\t\t# we also support collection\r\n                                        )*\r\n                                        (\r\n                                            '\t\t\t\t\t\t\t\t\t\t\t\t\t# A value can begin with ', which is used to escape!\r\n                                                (?<value>\r\n                                                    (''|[^'])+\t\t\t    \t\t\t\t# '' will be escaped to a single character '\r\n                                                )\r\n                                            '{1}?\t\t\t\t\t\t\t\t\t\t\t\t# Another ' is needed to end a value\r\n                                            |\t\t\t\t\t\t\t\t\t\t\t\t\t# or\r\n                                            (?<value>\r\n                                                [\\w\\\\$]+\t\t\t\t\t\t\t\t\t\t# another format of a value\r\n                                            )\r\n                                        )\t\t\t\t\t\t\t\t\t\t\t\t\t\t# the end of value\r\n                                   $", RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);

		private Guid guid;

		private string commandText;

		private CommandType commandType;

		private int commandTimeout = 30;

		private UpdateRowSource updatedRowSource = UpdateRowSource.Both;

		private string preservedObjectProperty;

		private MonadPipelineProxy pipelineProxy;

		private MonadConnection connection;

		private MonadParameterCollection parameterCollection = new MonadParameterCollection();
	}
}
