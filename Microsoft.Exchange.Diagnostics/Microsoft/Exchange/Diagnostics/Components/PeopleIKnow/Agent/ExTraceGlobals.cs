using System;

namespace Microsoft.Exchange.Diagnostics.Components.PeopleIKnow.Agent
{
	public static class ExTraceGlobals
	{
		public static Trace GeneralTracer
		{
			get
			{
				if (ExTraceGlobals.generalTracer == null)
				{
					ExTraceGlobals.generalTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.generalTracer;
			}
		}

		private static Guid componentGuid = new Guid("fd252cbc-f22c-4a4f-8eb5-30ce53b9915d");

		private static Trace generalTracer = null;
	}
}
