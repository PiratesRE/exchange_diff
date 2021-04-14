using System;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal abstract class ConverterInput : IDisposable
	{
		public bool EndOfFile
		{
			get
			{
				return this.endOfFile;
			}
		}

		public int MaxTokenSize
		{
			get
			{
				return this.maxTokenSize;
			}
		}

		protected ConverterInput(IProgressMonitor progressMonitor)
		{
			this.progressMonitor = progressMonitor;
		}

		public virtual void SetRestartConsumer(IRestartable restartConsumer)
		{
		}

		public abstract bool ReadMore(ref char[] buffer, ref int start, ref int current, ref int end);

		public abstract void ReportProcessed(int processedSize);

		public abstract int RemoveGap(int gapBegin, int gapEnd);

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		protected bool endOfFile;

		protected int maxTokenSize;

		protected IProgressMonitor progressMonitor;
	}
}
