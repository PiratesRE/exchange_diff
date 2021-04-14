using System;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class Pipeline
	{
		internal int Count
		{
			get
			{
				return this.MyStages.Length;
			}
		}

		protected abstract IPipelineStageFactory[] MyStages { get; }

		internal IPipelineStageFactory this[int index]
		{
			get
			{
				return this.MyStages[index];
			}
		}
	}
}
