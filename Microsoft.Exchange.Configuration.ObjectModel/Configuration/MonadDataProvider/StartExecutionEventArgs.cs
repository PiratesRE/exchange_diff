using System;
using System.Collections;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class StartExecutionEventArgs : RunGuidEventArgs
	{
		public IEnumerable Pipeline
		{
			get
			{
				return this.pipeline;
			}
		}

		public StartExecutionEventArgs(Guid guid, IEnumerable pipelineInput) : base(guid)
		{
			this.pipeline = pipelineInput;
		}

		private IEnumerable pipeline;
	}
}
