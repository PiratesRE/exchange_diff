using System;

namespace Microsoft.Exchange.Diagnostics.Components.OrgIdAuthentication
{
	public static class ExTraceGlobals
	{
		public static Trace OrgIdAuthenticationTracer
		{
			get
			{
				if (ExTraceGlobals.orgIdAuthenticationTracer == null)
				{
					ExTraceGlobals.orgIdAuthenticationTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.orgIdAuthenticationTracer;
			}
		}

		public static Trace OrgIdBasicAuthTracer
		{
			get
			{
				if (ExTraceGlobals.orgIdBasicAuthTracer == null)
				{
					ExTraceGlobals.orgIdBasicAuthTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.orgIdBasicAuthTracer;
			}
		}

		public static Trace OrgIdConfigurationTracer
		{
			get
			{
				if (ExTraceGlobals.orgIdConfigurationTracer == null)
				{
					ExTraceGlobals.orgIdConfigurationTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.orgIdConfigurationTracer;
			}
		}

		public static Trace OrgIdUserValidationTracer
		{
			get
			{
				if (ExTraceGlobals.orgIdUserValidationTracer == null)
				{
					ExTraceGlobals.orgIdUserValidationTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.orgIdUserValidationTracer;
			}
		}

		private static Guid componentGuid = new Guid("BD7A7CA1-6659-4EB0-A477-8F89F9A7D983");

		private static Trace orgIdAuthenticationTracer = null;

		private static Trace orgIdBasicAuthTracer = null;

		private static Trace orgIdConfigurationTracer = null;

		private static Trace orgIdUserValidationTracer = null;
	}
}
