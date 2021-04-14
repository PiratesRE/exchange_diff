using System;

namespace Microsoft.Exchange.Management.Analysis.Features
{
	internal class HelpTopicFeature : Feature
	{
		public HelpTopicFeature(Guid topicGuid) : base(false, false)
		{
			this.TopicGuid = topicGuid;
		}

		public Guid TopicGuid { get; private set; }

		public override string ToString()
		{
			return string.Format("{0}({1})", base.ToString(), this.TopicGuid.ToString());
		}
	}
}
