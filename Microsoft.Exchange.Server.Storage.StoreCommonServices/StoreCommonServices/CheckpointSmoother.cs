using System;
using System.Data.SqlClient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PhysicalAccessSql;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class CheckpointSmoother
	{
		public CheckpointSmoother(StoreDatabase database, Func<bool> shouldCallbackContinue)
		{
			this.database = database;
		}

		public void SmoothCheckpoint()
		{
			using (Connection connection = Factory.CreateConnection(null, this.database.PhysicalDatabase, "CheckpointSmoother"))
			{
				using (Microsoft.Exchange.Server.Storage.PhysicalAccessSql.SqlCommand sqlCommand = new Microsoft.Exchange.Server.Storage.PhysicalAccessSql.SqlCommand(connection, "CHECKPOINT 300", Connection.OperationType.Other))
				{
					sqlCommand.ExecuteScalar(Connection.TransactionOption.NoTransaction);
				}
			}
		}

		internal static void SmoothCheckpoint(Context context, StoreDatabase database, Func<bool> shouldCallbackContinue)
		{
			if (shouldCallbackContinue())
			{
				database.GetSharedLock(context.Diagnostics);
				try
				{
					if (database.IsOnlineActive)
					{
						CheckpointSmoother checkpointSmoother = new CheckpointSmoother(database, shouldCallbackContinue);
						checkpointSmoother.SmoothCheckpoint();
					}
					else if (ExTraceGlobals.BadPlanDetectionTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.BadPlanDetectionTracer.TraceError<Guid>(0L, "Could not connect to database {0}.  Skipping checkpoint smoothing.", database.MdbGuid);
					}
				}
				catch (SqlException ex)
				{
					context.OnExceptionCatch(ex);
					if (ExTraceGlobals.BadPlanDetectionTracer.IsTraceEnabled(TraceType.ErrorTrace))
					{
						ExTraceGlobals.BadPlanDetectionTracer.TraceError<Type, string, string>(0L, "SqlException in checkpoint smoothing. Type:[{0}] Message:[{1}] StackTrace:[{2}]", ex.GetType(), ex.Message, ex.StackTrace);
					}
				}
				catch (StoreException ex2)
				{
					context.OnExceptionCatch(ex2);
					if (ExTraceGlobals.BadPlanDetectionTracer.IsTraceEnabled(TraceType.ErrorTrace))
					{
						ExTraceGlobals.BadPlanDetectionTracer.TraceError<Type, string, string>(0L, "Exception in checkpoint smoothing. Type:[{0}] Message:[{1}] StackTrace:[{2}]", ex2.GetType(), ex2.Message, ex2.StackTrace);
					}
				}
				finally
				{
					database.ReleaseSharedLock();
				}
			}
		}

		internal static void MountEventHandler(StoreDatabase database)
		{
			RecurringTask<StoreDatabase> task = new RecurringTask<StoreDatabase>(TaskExecutionWrapper<StoreDatabase>.WrapExecute(new TaskDiagnosticInformation(TaskTypeId.CheckpointSmoother, ClientType.System, database.MdbGuid), new TaskExecutionWrapper<StoreDatabase>.TaskCallback<Context>(CheckpointSmoother.SmoothCheckpoint)), database, CheckpointSmoother.checkpointInitialDelay, CheckpointSmoother.checkpointInterval, false);
			database.TaskList.Add(task, true);
		}

		private static TimeSpan checkpointInitialDelay = TimeSpan.FromMinutes(5.0);

		private static TimeSpan checkpointInterval = TimeSpan.FromMinutes(15.0);

		private readonly StoreDatabase database;
	}
}
