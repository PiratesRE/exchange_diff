using System;

namespace Microsoft.Exchange.Diagnostics.Components.AddressBook.Service
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

		public static Trace NspiTracer
		{
			get
			{
				if (ExTraceGlobals.nspiTracer == null)
				{
					ExTraceGlobals.nspiTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.nspiTracer;
			}
		}

		public static Trace ReferralTracer
		{
			get
			{
				if (ExTraceGlobals.referralTracer == null)
				{
					ExTraceGlobals.referralTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.referralTracer;
			}
		}

		public static Trace PropertyMapperTracer
		{
			get
			{
				if (ExTraceGlobals.propertyMapperTracer == null)
				{
					ExTraceGlobals.propertyMapperTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.propertyMapperTracer;
			}
		}

		public static Trace ModCacheTracer
		{
			get
			{
				if (ExTraceGlobals.modCacheTracer == null)
				{
					ExTraceGlobals.modCacheTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.modCacheTracer;
			}
		}

		public static Trace NspiConnectionTracer
		{
			get
			{
				if (ExTraceGlobals.nspiConnectionTracer == null)
				{
					ExTraceGlobals.nspiConnectionTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.nspiConnectionTracer;
			}
		}

		private static Guid componentGuid = new Guid("583dfb2d-4ab4-4416-848b-88cc74d39e1f");

		private static Trace generalTracer = null;

		private static Trace nspiTracer = null;

		private static Trace referralTracer = null;

		private static Trace propertyMapperTracer = null;

		private static Trace modCacheTracer = null;

		private static Trace nspiConnectionTracer = null;
	}
}
