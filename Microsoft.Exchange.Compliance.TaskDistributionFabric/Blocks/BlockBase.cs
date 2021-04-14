using System;
using System.Diagnostics;
using System.Threading.Tasks.Dataflow;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Instrumentation;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;

namespace Microsoft.Exchange.Compliance.TaskDistributionFabric.Blocks
{
	internal abstract class BlockBase<TIn, TOut>
	{
		static BlockBase()
		{
			BlockBase<TIn, TOut>.watch.Start();
		}

		public static ExecutionDataflowBlockOptions DefaultExecutionOptions
		{
			get
			{
				return BlockBase<TIn, TOut>.defaultExecutionOptions;
			}
		}

		public static DataflowLinkOptions DefaultLinkOptions
		{
			get
			{
				return BlockBase<TIn, TOut>.defaultLinkOptions;
			}
		}

		public virtual string BlockType
		{
			get
			{
				if (string.IsNullOrEmpty(this.blockType))
				{
					this.blockType = base.GetType().Name;
				}
				return this.blockType;
			}
		}

		public abstract TOut Process(TIn input);

		public virtual TransformBlock<TIn, TOut> GetDataflowBlock(ExecutionDataflowBlockOptions options = null)
		{
			if (options != null)
			{
				return new TransformBlock<TIn, TOut>((TIn input) => this.MeterBlockProcessing(input), options);
			}
			return new TransformBlock<TIn, TOut>((TIn input) => this.MeterBlockProcessing(input), BlockBase<TIn, TOut>.DefaultExecutionOptions);
		}

		protected virtual TOut MeterBlockProcessing(TIn input)
		{
			long elapsedMilliseconds = BlockBase<TIn, TOut>.watch.ElapsedMilliseconds;
			ComplianceMessage complianceMessage = input as ComplianceMessage;
			TOut result;
			try
			{
				if (complianceMessage != null)
				{
					MessageLogger.Instance.LogMessageBlockProcessing(complianceMessage, this.BlockType);
				}
				if (input != null)
				{
					result = this.Process(input);
				}
				else
				{
					result = default(TOut);
				}
			}
			finally
			{
				long elapsedMilliseconds2 = BlockBase<TIn, TOut>.watch.ElapsedMilliseconds;
				if (complianceMessage != null)
				{
					MessageLogger.Instance.LogMessageBlockProcessed(complianceMessage, this.BlockType, elapsedMilliseconds2 - elapsedMilliseconds);
				}
			}
			return result;
		}

		private static Stopwatch watch = new Stopwatch();

		private static DataflowLinkOptions defaultLinkOptions = new DataflowLinkOptions
		{
			PropagateCompletion = true
		};

		private static ExecutionDataflowBlockOptions defaultExecutionOptions = new ExecutionDataflowBlockOptions
		{
			MaxDegreeOfParallelism = -1,
			BoundedCapacity = TaskDistributionSettings.MaxQueuePerBlock
		};

		private string blockType;
	}
}
