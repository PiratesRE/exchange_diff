using System;
using System.Data;
using System.Text;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class CommandLoggingSession : IDisposable
	{
		public void Dispose()
		{
			this.table.Dispose();
		}

		public int MaximumRecordCount
		{
			get
			{
				return this.maximumRecordCount;
			}
			set
			{
				if (!CommandLoggingSession.IsValidMaximumRecordCount(value))
				{
					throw new ArgumentOutOfRangeException(Strings.InvalidMaximumRecordNumber(CommandLoggingSession.MaximumRecordCountLimit), null);
				}
				lock (this.mutex)
				{
					while (this.table.Rows.Count > value)
					{
						this.RemoveOldestRow();
					}
					this.maximumRecordCount = value;
				}
			}
		}

		private void RemoveOldestRow()
		{
			if (this.table.Rows.Count > 0)
			{
				this.table.Rows.RemoveAt(0);
			}
		}

		public static bool IsValidMaximumRecordCount(int value)
		{
			return 1 <= value && value <= CommandLoggingSession.MaximumRecordCountLimit;
		}

		public void Clear()
		{
			lock (this.mutex)
			{
				this.table.Rows.Clear();
			}
		}

		internal CommandLoggingSession()
		{
			this.table = new DataTable();
			this.table.Columns.Add(CommandLoggingSession.startExecutionTime, typeof(DateTime));
			this.table.Columns.Add(CommandLoggingSession.endExecutionTime, typeof(DateTime));
			this.table.Columns.Add(CommandLoggingSession.command).DefaultValue = string.Empty;
			this.table.Columns.Add(CommandLoggingSession.executionStatus).DefaultValue = string.Empty;
			this.table.Columns.Add(CommandLoggingSession.error).DefaultValue = string.Empty;
			this.table.Columns.Add(CommandLoggingSession.warning).DefaultValue = string.Empty;
			this.table.PrimaryKey = new DataColumn[]
			{
				this.table.Columns.Add("Identity", typeof(Guid))
			};
		}

		public DataView LoggingData
		{
			get
			{
				return new DataView(this.table);
			}
		}

		public static CommandLoggingSession GetInstance()
		{
			if (CommandLoggingDialog.instance == null)
			{
				lock (CommandLoggingSession.entryLock)
				{
					if (CommandLoggingSession.instance == null)
					{
						CommandLoggingSession.instance = new CommandLoggingSession();
					}
				}
			}
			return CommandLoggingSession.instance;
		}

		public bool CommandLoggingEnabled
		{
			get
			{
				return this.commandLoggingEnabled;
			}
			set
			{
				lock (this.mutex)
				{
					this.commandLoggingEnabled = value;
				}
			}
		}

		public void LogStart(Guid guid, DateTime startTime, string commandText)
		{
			lock (this.mutex)
			{
				if (this.commandLoggingEnabled)
				{
					if (this.table.Rows.Count >= this.maximumRecordCount)
					{
						this.RemoveOldestRow();
					}
					DataRow dataRow = this.table.NewRow();
					dataRow["Identity"] = guid;
					dataRow[CommandLoggingSession.startExecutionTime] = startTime;
					dataRow[CommandLoggingSession.command] = commandText;
					this.table.Rows.Add(dataRow);
				}
			}
		}

		public void LogWarning(Guid guid, string warning)
		{
			lock (this.mutex)
			{
				if (this.commandLoggingEnabled)
				{
					DataRow dataRow = this.table.Rows.Find(guid);
					if (dataRow != null)
					{
						if (!string.IsNullOrEmpty((string)dataRow[CommandLoggingSession.warning]))
						{
							DataRow dataRow2;
							string columnName;
							(dataRow2 = dataRow)[columnName = CommandLoggingSession.warning] = dataRow2[columnName] + "\r\n";
						}
						DataRow dataRow3;
						string columnName2;
						(dataRow3 = dataRow)[columnName2 = CommandLoggingSession.warning] = dataRow3[columnName2] + Strings.WarningUpperCase(warning);
					}
				}
			}
		}

		public void LogError(Guid guid, string error)
		{
			lock (this.mutex)
			{
				if (this.commandLoggingEnabled)
				{
					DataRow dataRow = this.table.Rows.Find(guid);
					if (dataRow != null)
					{
						if (!string.IsNullOrEmpty((string)dataRow[CommandLoggingSession.error]))
						{
							DataRow dataRow2;
							string columnName;
							(dataRow2 = dataRow)[columnName = CommandLoggingSession.error] = dataRow2[columnName] + "\r\n";
						}
						DataRow dataRow3;
						string columnName2;
						(dataRow3 = dataRow)[columnName2 = CommandLoggingSession.error] = dataRow3[columnName2] + Strings.ErrorUpperCase(error);
					}
				}
			}
		}

		public void LogEnd(Guid guid, DateTime endTime)
		{
			lock (this.mutex)
			{
				if (this.commandLoggingEnabled)
				{
					DataRow dataRow = this.table.Rows.Find(guid);
					if (dataRow != null)
					{
						dataRow[CommandLoggingSession.endExecutionTime] = endTime;
						if (!string.IsNullOrEmpty((string)dataRow[CommandLoggingSession.error]))
						{
							dataRow[CommandLoggingSession.executionStatus] = Strings.ExecutionError;
						}
						else
						{
							dataRow[CommandLoggingSession.executionStatus] = Strings.ExecutionCompleted;
						}
					}
				}
			}
		}

		public static void ErrorReport(object sender, ErrorReportEventArgs e)
		{
			CommandLoggingDialog.LogError(e.Guid, e.ErrorRecord.Exception.Message);
		}

		public static void WarningReport(object sender, WarningReportEventArgs e)
		{
			CommandLoggingDialog.LogWarning(e.Guid, e.WarningMessage);
		}

		public static void StartExecution(object sender, StartExecutionEventArgs e)
		{
			MonadCommand monadCommand = sender as MonadCommand;
			StringBuilder stringBuilder = new StringBuilder();
			if (e.Pipeline != null)
			{
				int num = 0;
				foreach (object value in e.Pipeline)
				{
					if (num != 0)
					{
						stringBuilder.Append(",");
					}
					num++;
					stringBuilder.Append(MonadCommand.FormatParameterValue(value));
				}
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(" | ");
				}
			}
			stringBuilder.Append(monadCommand.ToString());
			CommandLoggingDialog.LogStart(e.Guid, (DateTime)ExDateTime.Now, stringBuilder.ToString());
		}

		public static void EndExecution(object sender, RunGuidEventArgs e)
		{
			CommandLoggingDialog.LogEnd(e.Guid, (DateTime)ExDateTime.Now);
		}

		internal static readonly string warning = "Warning";

		internal static readonly string error = "Error";

		internal static readonly string startExecutionTime = "StartExecutionTime";

		internal static readonly string endExecutionTime = "EndExecutionTime";

		internal static readonly string executionStatus = "ExecutionStatus";

		internal static readonly string command = "Command";

		private DataTable table;

		private object mutex = new object();

		private static object entryLock = new object();

		internal static readonly int MaximumRecordCountLimit = 32767;

		internal static readonly int DefaultMaximumRecordCount = 2048;

		private int maximumRecordCount;

		private static CommandLoggingSession instance;

		private bool commandLoggingEnabled;
	}
}
