using System;

namespace Microsoft.Exchange.Diagnostics.Components.Inference.GroupingModel
{
	public static class ExTraceGlobals
	{
		public static Trace RecommendedGroupsInfoWriterTracer
		{
			get
			{
				if (ExTraceGlobals.recommendedGroupsInfoWriterTracer == null)
				{
					ExTraceGlobals.recommendedGroupsInfoWriterTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.recommendedGroupsInfoWriterTracer;
			}
		}

		private static Guid componentGuid = new Guid("C7C1EF44-5F48-42B9-868D-E25A72067991");

		private static Trace recommendedGroupsInfoWriterTracer = null;
	}
}
