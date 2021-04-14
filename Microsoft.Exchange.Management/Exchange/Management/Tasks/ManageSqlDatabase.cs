using System;
using System.Data.SqlClient;
using System.IO;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	public abstract class ManageSqlDatabase : Task
	{
		[Parameter(Mandatory = true)]
		public string DatabaseName
		{
			get
			{
				return (string)base.Fields["DatabaseName"];
			}
			set
			{
				base.Fields["DatabaseName"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string ServerName
		{
			get
			{
				return (string)base.Fields["ServerName"];
			}
			set
			{
				base.Fields["ServerName"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string MirrorServerName
		{
			get
			{
				return (string)base.Fields["MirrorServerName"];
			}
			set
			{
				base.Fields["MirrorServerName"] = value;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			if (!base.Fields.IsModified("ServerName"))
			{
				this.ServerName = "localhost";
			}
			base.InternalValidate();
			TaskLogger.LogExit();
		}

		protected void Install(string databasePath, string logPath)
		{
			TaskLogger.LogEnter();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("create database {0}", this.DatabaseName);
			if (!string.IsNullOrEmpty(databasePath))
			{
				string arg = Path.Combine(databasePath, string.Format("{0}.mdf", this.DatabaseName));
				stringBuilder.AppendFormat(" ON (NAME = {0}, FILENAME = '{1}')", this.DatabaseName, arg);
			}
			if (!string.IsNullOrEmpty(logPath))
			{
				string arg2 = Path.Combine(logPath, string.Format("{0}_log.ldf", this.DatabaseName));
				stringBuilder.AppendFormat(" LOG ON (NAME = {0}_log, FILENAME = '{1}')", this.DatabaseName, arg2);
			}
			this.ExecuteCommand(stringBuilder.ToString(), "Master", false, 0);
			this.ExecuteCommand("exec sp_changedbowner 'sa'", this.DatabaseName, false, 0);
			TaskLogger.LogExit();
		}

		protected void Uninstall()
		{
			TaskLogger.LogEnter();
			this.ExecuteCommand(string.Format("drop database {0}", this.DatabaseName), "Master", false, 0);
			TaskLogger.LogExit();
		}

		protected void ExecuteCommand(string commandToExecute, bool executeScalar, int timeout)
		{
			TaskLogger.LogEnter();
			this.ExecuteCommand(commandToExecute, this.DatabaseName, executeScalar, timeout);
			TaskLogger.LogExit();
		}

		protected void ExecuteCommand(string commandToExecute, string databaseName, bool executeScalar, int timeout)
		{
			TaskLogger.LogEnter();
			try
			{
				using (SqlConnection sqlConnection = new SqlConnection(this.GetConnectionString(databaseName)))
				{
					sqlConnection.Open();
					this.ExecuteCommand(commandToExecute, databaseName, sqlConnection, executeScalar, timeout);
					sqlConnection.Close();
				}
			}
			catch (SqlException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidOperation, null);
			}
			TaskLogger.LogExit();
		}

		private void ExecuteCommand(string commandToExecute, string databaseName, SqlConnection connection, bool executeScalar, int timeout)
		{
			TaskLogger.LogEnter();
			if (!string.IsNullOrEmpty(commandToExecute))
			{
				try
				{
					SqlCommand sqlCommand = new SqlCommand(commandToExecute, connection);
					if (timeout > 0)
					{
						sqlCommand.CommandTimeout = timeout;
					}
					object sendToPipeline;
					if (executeScalar)
					{
						sendToPipeline = sqlCommand.ExecuteScalar();
					}
					else
					{
						sendToPipeline = sqlCommand.ExecuteNonQuery();
					}
					base.WriteObject(sendToPipeline);
				}
				catch (SqlException exception)
				{
					base.WriteError(exception, ErrorCategory.InvalidOperation, null);
				}
			}
			TaskLogger.LogExit();
		}

		private string GetConnectionString(string databaseName)
		{
			return string.Format("Data Source={0};Initial Catalog={1};Integrated Security=SSPI;Failover Partner={2}", this.ServerName, databaseName, this.MirrorServerName);
		}
	}
}
