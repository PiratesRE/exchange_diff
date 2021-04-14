using System;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;

namespace Microsoft.Exchange.Search.Core.Pipeline
{
	internal sealed class PipelineConfigAdapter : IConfigAdapter
	{
		internal PipelineConfigAdapter(IPipelineComponentConfig config)
		{
			Util.ThrowOnNullArgument(config, "config");
			this.config = config;
		}

		public string GetSetting(string key)
		{
			return this.config[key];
		}

		private readonly IPipelineComponentConfig config;
	}
}
