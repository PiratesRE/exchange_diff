using System;
using System.Threading.Tasks.Dataflow;

namespace Microsoft.Exchange.Compliance.TaskDistributionFabric.Blocks
{
	internal abstract class FinalBlockBase<TIn> : BlockBase<TIn, object>
	{
		public override object Process(TIn input)
		{
			this.ProcessFinal(input);
			return null;
		}

		public override TransformBlock<TIn, object> GetDataflowBlock(ExecutionDataflowBlockOptions options = null)
		{
			TransformBlock<TIn, object> dataflowBlock = base.GetDataflowBlock(options);
			ITargetBlock<object> targetBlock = new ActionBlock<object>(delegate(object o)
			{
			});
			DataflowBlock.LinkTo<object>(dataflowBlock, targetBlock);
			return dataflowBlock;
		}

		protected abstract void ProcessFinal(TIn input);
	}
}
