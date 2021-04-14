using System;
using Microsoft.Exchange.Data.Storage.Optics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ConversationAggregationLogConfiguration : LogConfigurationBase
	{
		public static ConversationAggregationLogConfiguration Default
		{
			get
			{
				if (ConversationAggregationLogConfiguration.defaultInstance == null)
				{
					ConversationAggregationLogConfiguration.defaultInstance = new ConversationAggregationLogConfiguration();
				}
				return ConversationAggregationLogConfiguration.defaultInstance;
			}
		}

		protected override string Component
		{
			get
			{
				return "ConversationAggregationLog";
			}
		}

		protected override string Type
		{
			get
			{
				return "Conversation Aggregation Log";
			}
		}

		protected override Trace Tracer
		{
			get
			{
				return ExTraceGlobals.ConversationAggregationTracer;
			}
		}

		private static ConversationAggregationLogConfiguration defaultInstance;
	}
}
