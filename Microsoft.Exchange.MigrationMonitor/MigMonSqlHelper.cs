using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor
{
	internal sealed class MigMonSqlHelper
	{
		public MigMonSqlHelper()
		{
			this.GetConnectionStringFromConfig(MigMonSqlHelper.MigMonDatabaseSelection.PrimaryMRSDatabase);
		}

		public void ClearConnectionPool()
		{
			if (!string.IsNullOrWhiteSpace(this.connectionString))
			{
				using (SqlConnection sqlConnection = new SqlConnection(this.connectionString))
				{
					SqlConnection.ClearPool(sqlConnection);
				}
			}
		}

		public void GetConnectionStringFromConfig(MigMonSqlHelper.MigMonDatabaseSelection db = MigMonSqlHelper.MigMonDatabaseSelection.PrimaryMRSDatabase)
		{
			string config;
			switch (db)
			{
			case MigMonSqlHelper.MigMonDatabaseSelection.PrimaryMRSDatabase:
				config = MigrationMonitor.MigrationMonitorContext.Config.GetConfig<string>("ConnectionStringPrimary");
				break;
			case MigMonSqlHelper.MigMonDatabaseSelection.DCInfoDatabase:
				config = MigrationMonitor.MigrationMonitorContext.Config.GetConfig<string>("ConnectionStringDCInfo");
				break;
			default:
				config = MigrationMonitor.MigrationMonitorContext.Config.GetConfig<string>("ConnectionStringPrimary");
				break;
			}
			SqlConnectionStringBuilder sqlConnectionStringBuilder;
			try
			{
				sqlConnectionStringBuilder = new SqlConnectionStringBuilder(config);
			}
			catch (ArgumentException ex)
			{
				MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, ex, "formatting issue with connection string {0}", new object[]
				{
					config
				});
				throw new SqlConnectionStringException(config, ex);
			}
			object obj;
			if (sqlConnectionStringBuilder.TryGetValue("password", out obj))
			{
				string password = MigMonUtilities.Decrypt((string)obj);
				sqlConnectionStringBuilder.Password = password;
			}
			this.connectionString = sqlConnectionStringBuilder.ToString();
		}

		public void SetHeartBeatTS()
		{
			List<SqlParameter> list = new List<SqlParameter>();
			this.AddSqlParameter(list, "@serverName", MigrationMonitor.ComputerName, false, false);
			this.ExecuteSprocNonQuery("MIGMON_SetHeartBeatTimestamp", list, 30);
		}

		public void RegisterLoggingServer(string serverName, string forest = null, string site = null, int? cpuCores = null, ByteQuantifiedSize? diskSize = null)
		{
			List<SqlParameter> list = new List<SqlParameter>();
			this.AddSqlParameter(list, "@loggingServerName", serverName, false, false);
			this.AddOptionalNullableSqlParameter(list, "@site", site, SqlDbType.NVarChar);
			this.AddOptionalNullableSqlParameter(list, "@forest", forest, SqlDbType.NVarChar);
			this.AddOptionalNullableSqlParameter(list, "@cpucores", cpuCores, SqlDbType.NVarChar);
			if (diskSize != null)
			{
				this.AddOptionalNullableSqlParameter(list, "@disksize", diskSize.Value.ToTB(), SqlDbType.NVarChar);
			}
			else
			{
				this.AddOptionalNullableSqlParameter(list, "@disksize", null, SqlDbType.NVarChar);
			}
			this.ExecuteSprocNonQuery("MIGMON_RegisterLoggingServer", list, 30);
		}

		public void AddOptionalNullableSqlParameter(List<SqlParameter> sqlParams, string name, object value, SqlDbType dbType = SqlDbType.NVarChar)
		{
			this.AddSqlParameter(sqlParams, name, value, dbType, true, true);
		}

		public void AddSqlParameter(List<SqlParameter> sqlParams, string name, string value, bool isOptional = false, bool isNullable = false)
		{
			this.AddSqlParameter(sqlParams, name, value, SqlDbType.NVarChar, isOptional, isNullable);
		}

		public void AddSqlParameter(List<SqlParameter> paramList, string name, object value, SqlDbType dbType, bool isOptional = false, bool isNullable = false)
		{
			SqlParameter sqlParameter = new SqlParameter
			{
				ParameterName = name,
				SqlDbType = dbType,
				IsNullable = isNullable
			};
			if (value == null && isOptional)
			{
				return;
			}
			if (value == null && isNullable)
			{
				sqlParameter.Value = DBNull.Value;
			}
			else
			{
				sqlParameter.Value = value;
			}
			paramList.Add(sqlParameter);
		}

		public T ExecuteSprocScalar<T>(string sprocName, List<SqlParameter> parameters = null)
		{
			object retVal = null;
			this.ExecuteSprocOperator(sprocName, parameters, delegate(SqlCommand cmd)
			{
				retVal = cmd.ExecuteScalar();
			}, 30);
			if (retVal == null)
			{
				Exception ex = new UnexpectedNullFromSprocException(sprocName);
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("Stored Procedure returned null unexptectedly.");
				stringBuilder.AppendLine(string.Format("Stored Procedure Name {0}", sprocName));
				stringBuilder.AppendLine(string.Format("ConnectionString: {0}", MigMonSqlHelper.GetSafeConnectionString(this.connectionString)));
				MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, stringBuilder.ToString(), new object[0]);
				ExWatson.SendReport(ex, ReportOptions.None, stringBuilder.ToString());
				throw ex;
			}
			return (T)((object)retVal);
		}

		public void ExecuteSprocNonQuery(string sprocName, List<SqlParameter> parameters = null, int commandTimeout = 30)
		{
			this.ExecuteSprocOperator(sprocName, parameters, delegate(SqlCommand cmd)
			{
				cmd.ExecuteNonQuery();
			}, commandTimeout);
		}

		public int GetLoggingServerId()
		{
			List<SqlParameter> list = new List<SqlParameter>();
			this.AddSqlParameter(list, "@loggingServerName", CommonUtils.LocalComputerName, false, false);
			SqlParameter sqlParameter = new SqlParameter("@loggingServerID", SqlDbType.Int);
			sqlParameter.Direction = ParameterDirection.Output;
			list.Add(sqlParameter);
			try
			{
				this.ExecuteSprocNonQuery("MIGMON_LookupLoggingServer", list, 30);
			}
			catch (SqlQueryFailedException ex)
			{
				MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, ex, "Error looking up server id, server name is {0}. Will attempt again next cycle.", new object[]
				{
					CommonUtils.LocalComputerName
				});
				throw new LookUpServerIdFailureException(ex.InnerException);
			}
			return (int)sqlParameter.Value;
		}

		public int? GetIdForKnownString(string value, string type)
		{
			List<SqlParameter> list = new List<SqlParameter>();
			this.AddSqlParameter(list, "@string", value, false, false);
			this.AddSqlParameter(list, "@stringType", type, false, false);
			SqlParameter sqlParameter = new SqlParameter("@stringID", SqlDbType.Int)
			{
				Direction = ParameterDirection.Output
			};
			list.Add(sqlParameter);
			try
			{
				this.ExecuteSprocNonQuery("MIGMON_LookupString", list, 30);
			}
			catch (SqlQueryFailedException ex)
			{
				MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, ex, "Error looking up string id for {0} {1}. Will attempt again next cycle.", new object[]
				{
					type,
					value
				});
				throw new LookUpStringIdFailureException(ex.InnerException);
			}
			if (!(sqlParameter.Value is DBNull))
			{
				return new int?((int)sqlParameter.Value);
			}
			return null;
		}

		public int GetIdForKnownTenantName(string value)
		{
			List<SqlParameter> list = new List<SqlParameter>();
			this.AddSqlParameter(list, "@tenant", value, false, false);
			SqlParameter sqlParameter = new SqlParameter("@tenantID", SqlDbType.Int)
			{
				Direction = ParameterDirection.Output
			};
			list.Add(sqlParameter);
			try
			{
				this.ExecuteSprocNonQuery("MIGMON_LookupTenant", list, 30);
			}
			catch (SqlQueryFailedException ex)
			{
				MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, ex, "Error looking up tenant name {0}. Will attempt again next cycle.", new object[]
				{
					value
				});
				throw new LookUpTenantIdFailureException(ex.InnerException);
			}
			return (int)sqlParameter.Value;
		}

		public int GetIdForKnownEndpoint(string value)
		{
			List<SqlParameter> list = new List<SqlParameter>();
			this.AddSqlParameter(list, "@Guid", MigMonUtilities.ConvertToGuid(value), SqlDbType.UniqueIdentifier, false, false);
			SqlParameter sqlParameter = new SqlParameter("@endpointID", SqlDbType.Int)
			{
				Direction = ParameterDirection.Output
			};
			list.Add(sqlParameter);
			try
			{
				this.ExecuteSprocNonQuery("MIGMON_LookupEndpoint", list, 30);
			}
			catch (SqlQueryFailedException ex)
			{
				MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, ex, "Error looking up Endpoint Guid {0}. Will attempt again next cycle.", new object[]
				{
					value
				});
				throw new LookUpEndpointIdFailureException(ex.InnerException);
			}
			return (int)sqlParameter.Value;
		}

		public int GetIdForKnownWatsonHash(string watsonHash, string stackTrace, string service)
		{
			int num = this.CheckIdForKnownWatsonHash(watsonHash);
			if (num == -1)
			{
				List<SqlParameter> list = new List<SqlParameter>();
				this.AddSqlParameter(list, "@watsonHash", watsonHash, false, false);
				this.AddSqlParameter(list, "@stackTrace", stackTrace, false, false);
				this.AddSqlParameter(list, "@service", service, false, false);
				SqlParameter sqlParameter = new SqlParameter("@watsonID", SqlDbType.Int)
				{
					Direction = ParameterDirection.Output
				};
				list.Add(sqlParameter);
				try
				{
					this.ExecuteSprocNonQuery("MIGMON_LookupWatsonV2", list, 30);
				}
				catch (SqlQueryFailedException ex)
				{
					MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, ex, "Error looking up watson hash {0}. Will attempt again next cycle.", new object[]
					{
						watsonHash
					});
					throw new LookUpWatsonIdFailureException(ex.InnerException);
				}
				num = (int)sqlParameter.Value;
			}
			return num;
		}

		public int CheckIdForKnownWatsonHash(string watsonHash)
		{
			List<SqlParameter> list = new List<SqlParameter>();
			this.AddSqlParameter(list, "@watsonHash", watsonHash, false, false);
			SqlParameter sqlParameter = new SqlParameter("@watsonID", SqlDbType.Int)
			{
				Direction = ParameterDirection.Output
			};
			list.Add(sqlParameter);
			try
			{
				this.ExecuteSprocNonQuery("MIGMON_CheckWatson", list, 30);
			}
			catch (SqlQueryFailedException ex)
			{
				MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, ex, "Error checking watson hash {0}. Will attempt again next cycle.", new object[]
				{
					watsonHash
				});
				throw new LookUpWatsonIdFailureException(ex.InnerException);
			}
			return (int)sqlParameter.Value;
		}

		private static string GetSafeConnectionString(string cs)
		{
			SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder(cs);
			if (sqlConnectionStringBuilder.ContainsKey("password"))
			{
				sqlConnectionStringBuilder.Password = "[REDACTED]";
			}
			return sqlConnectionStringBuilder.ToString();
		}

		private static bool IsTransientError(SqlException ex)
		{
			return ex.Number == -1 || ex.Number == -2 || ex.Number == 20 || ex.Number == 53 || ex.Number == 64 || ex.Number == 233 || ex.Number == 1204 || ex.Number == 1205 || ex.Number == 1222 || ex.Number == 10053 || ex.Number == 10054 || ex.Number == 10060 || ex.Number == 10061 || ex.Number == 10928 || ex.Number == 11004 || ex.Number == 40143 || ex.Number == 40197 || ex.Number == 40501 || ex.Number == 40540 || ex.Number == 40544 || ex.Number == 40545 || ex.Number == 40549 || ex.Number == 40550 || ex.Number == 40551 || ex.Number == 40552 || ex.Number == 40553 || ex.Number == 40613 || ex.Number == 40627;
		}

		private void ExecuteSprocOperator(string sprocName, List<SqlParameter> parameters, Action<SqlCommand> commandOperation, int commandTimeout = 30)
		{
			using (SqlConnection sqlConnection = new SqlConnection(this.connectionString))
			{
				using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
				{
					sqlCommand.CommandText = sprocName;
					sqlCommand.CommandType = CommandType.StoredProcedure;
					if (parameters != null)
					{
						sqlCommand.Parameters.AddRange(parameters.ToArray());
					}
					sqlCommand.CommandTimeout = commandTimeout;
					int num = 0;
					for (;;)
					{
						try
						{
							num++;
							sqlCommand.Connection.Open();
							commandOperation(sqlCommand);
							this.lastSuccessfulConnectionTime = DateTime.UtcNow;
							break;
						}
						catch (SqlException ex)
						{
							if (!MigMonSqlHelper.IsTransientError(ex))
							{
								MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, ex, "SQL Procedure {0} failed. Exception Number: {1}.", new object[]
								{
									sprocName,
									ex.Number
								});
								throw new SqlQueryFailedException(sprocName, ex);
							}
							if (num <= MigrationMonitor.MigrationMonitorContext.Config.GetConfig<int>("SqlMaxRetryAttempts"))
							{
								Thread.Sleep(MigrationMonitor.MigrationMonitorContext.Config.GetConfig<TimeSpan>("SqlSleepBetweenRetryDuration"));
							}
							else
							{
								this.HandleRetriesExhausted(ex, sprocName);
							}
						}
						finally
						{
							sqlCommand.Connection.Close();
						}
					}
				}
			}
		}

		private void HandleRetriesExhausted(SqlException ex, string sprocName)
		{
			if (ex.Number == -2)
			{
				MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, "SQl Procedure {0} failed after {1} attempts with Exception {2} ", new object[]
				{
					sprocName,
					MigrationMonitor.MigrationMonitorContext.Config.GetConfig<int>("SqlMaxRetryAttempts"),
					ex
				});
				throw new SqlServerTimeoutException(sprocName);
			}
			string safeConnectionString = MigMonSqlHelper.GetSafeConnectionString(this.connectionString);
			TimeSpan config = MigrationMonitor.MigrationMonitorContext.Config.GetConfig<TimeSpan>("TransientErrorAlertTreshold");
			if (DateTime.UtcNow - this.lastSuccessfulConnectionTime > config)
			{
				MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, ex, "SQL Server uncreachable for over {0}. Raising alert.", new object[]
				{
					config
				});
				this.lastSuccessfulConnectionTime = DateTime.UtcNow;
			}
			throw new SqlServerUnreachableException(safeConnectionString, ex);
		}

		internal const string KeyNameSqlMaxRetryAttempts = "SqlMaxRetryAttempts";

		internal const string KeyNameSqlSleepBetweenRetryDuration = "SqlSleepBetweenRetryDuration";

		internal const string KeyNameTransientErrorAlertTreshold = "TransientErrorAlertTreshold";

		internal const string KeyNameConnectionStringPrimary = "ConnectionStringPrimary";

		internal const string KeyNameConnectionStringDCInfo = "ConnectionStringDCInfo";

		internal const string KeyNameBulkInsertSqlCommandTimeout = "BulkInsertSqlCommandTimeout";

		private const string SprocNameRegisterLoggingServer = "MIGMON_RegisterLoggingServer";

		private const string SprocNameSetHeartBeatTS = "MIGMON_SetHeartBeatTimestamp";

		private const string SprocNameLookupLoggingServer = "MIGMON_LookupLoggingServer";

		private const string SprocNameLookupString = "MIGMON_LookupString";

		private const string SprocNameLookupTenant = "MIGMON_LookupTenant";

		private const string SprocNameLookupWatson = "MIGMON_LookupWatsonV2";

		private const string SprocNameCheckWatson = "MIGMON_CheckWatson";

		private const string SprocNameLookupEndpoint = "MIGMON_LookupEndpoint";

		private string connectionString;

		private DateTime lastSuccessfulConnectionTime;

		internal enum MigMonDatabaseSelection
		{
			PrimaryMRSDatabase,
			DCInfoDatabase
		}
	}
}
