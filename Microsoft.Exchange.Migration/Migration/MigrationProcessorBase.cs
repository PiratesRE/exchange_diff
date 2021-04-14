using System;
using System.Diagnostics;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration
{
	internal abstract class MigrationProcessorBase<T, TResponse> where TResponse : MigrationProcessorResponse
	{
		protected MigrationProcessorBase(T migrationObject, IMigrationDataProvider dataProvider)
		{
			this.MigrationObject = migrationObject;
			this.DataProvider = dataProvider;
		}

		internal TResponse Process()
		{
			MigrationEventType eventType = MigrationEventType.Information;
			string format = "Processor {0} - Starting to process {1} {2}";
			object[] array = new object[3];
			array[0] = base.GetType().Name;
			object[] array2 = array;
			int num = 1;
			T migrationObject = this.MigrationObject;
			array2[num] = migrationObject.GetType().Name;
			array[2] = this.MigrationObject;
			MigrationLogger.Log(eventType, format, array);
			MigrationLogger.Flush();
			Stopwatch stopwatch = Stopwatch.StartNew();
			TResponse tresponse = this.PerformPoisonDetection();
			if (tresponse.Result != MigrationProcessorResult.Working)
			{
				return this.ApplyResponse(tresponse);
			}
			try
			{
				this.SetContext();
				try
				{
					tresponse = this.InternalProcess();
				}
				catch (LocalizedException ex)
				{
					if (CommonUtils.IsTransientException(ex))
					{
						tresponse = this.HandleTransientException(ex);
					}
					else
					{
						tresponse = this.HandlePermanentException(ex);
					}
				}
				MigrationEventType eventType2 = MigrationEventType.Information;
				string format2 = "Processor {0} - Applying result {1} to {2} {3}.";
				object[] array3 = new object[4];
				array3[0] = base.GetType().Name;
				array3[1] = tresponse;
				object[] array4 = array3;
				int num2 = 2;
				T migrationObject2 = this.MigrationObject;
				array4[num2] = migrationObject2.GetType().Name;
				array3[3] = this.MigrationObject;
				MigrationLogger.Log(eventType2, format2, array3);
				tresponse.ClearPoison = true;
				tresponse.ProcessingDuration = new TimeSpan?(stopwatch.Elapsed);
				tresponse = this.ApplyResponse(tresponse);
			}
			finally
			{
				this.RestoreContext();
			}
			stopwatch.Stop();
			MigrationEventType eventType3 = MigrationEventType.Information;
			string format3 = "Processor {0} - Finished result {1} processing {2} {3}. Duration {4}.";
			object[] array5 = new object[5];
			array5[0] = base.GetType().Name;
			array5[1] = tresponse;
			object[] array6 = array5;
			int num3 = 2;
			T migrationObject3 = this.MigrationObject;
			array6[num3] = migrationObject3.GetType().Name;
			array5[3] = this.MigrationObject;
			array5[4] = stopwatch.Elapsed;
			MigrationLogger.Log(eventType3, format3, array5);
			return tresponse;
		}

		protected abstract TResponse InternalProcess();

		protected abstract TResponse PerformPoisonDetection();

		protected abstract void SetContext();

		protected abstract void RestoreContext();

		protected abstract TResponse ApplyResponse(TResponse response);

		protected abstract TResponse HandleTransientException(LocalizedException ex);

		protected abstract TResponse HandlePermanentException(LocalizedException ex);

		protected readonly T MigrationObject;

		protected readonly IMigrationDataProvider DataProvider;
	}
}
