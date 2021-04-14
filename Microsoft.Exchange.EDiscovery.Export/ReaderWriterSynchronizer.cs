using System;
using System.Threading.Tasks;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal class ReaderWriterSynchronizer<T>
	{
		public ReaderWriterSynchronizer(IBatchDataReader<T> batchDataReader, IBatchDataWriter<T> batchDataWriter)
		{
			this.writingTask = null;
			this.batchDataReader = batchDataReader;
			this.batchDataWriter = batchDataWriter;
			this.batchDataReader.DataBatchRead += this.WriteDataBatch;
			this.batchDataReader.AbortingOnError += this.ReaderAborting;
		}

		public void Execute()
		{
			ExportException ex = null;
			try
			{
				this.batchDataReader.StartReading();
			}
			finally
			{
				ex = AsynchronousTaskHandler.WaitForAsynchronousTask(this.writingTask);
				this.writingTask = null;
			}
			if (ex != null)
			{
				throw ex;
			}
		}

		private void WriteDataBatch(object sender, DataBatchEventArgs<T> args)
		{
			ScenarioData scenarioData = ScenarioData.Current;
			ExportException ex = AsynchronousTaskHandler.WaitForAsynchronousTask(this.writingTask);
			this.writingTask = ((ex != null || args.DataBatch == null) ? null : Task.Factory.StartNew(delegate(object dataBatch)
			{
				ScenarioData scenarioData;
				using (new ScenarioData(scenarioData))
				{
					this.batchDataWriter.WriteDataBatch((T)((object)dataBatch));
				}
			}, args.DataBatch));
			if (ex != null)
			{
				throw ex;
			}
		}

		private void ReaderAborting(object sender, EventArgs args)
		{
			AsynchronousTaskHandler.WaitForAsynchronousTask(this.writingTask);
			this.writingTask = null;
		}

		private readonly IBatchDataReader<T> batchDataReader;

		private readonly IBatchDataWriter<T> batchDataWriter;

		private Task writingTask;
	}
}
