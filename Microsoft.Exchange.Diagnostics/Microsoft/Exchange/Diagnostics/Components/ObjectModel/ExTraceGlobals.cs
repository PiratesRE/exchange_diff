using System;

namespace Microsoft.Exchange.Diagnostics.Components.ObjectModel
{
	public static class ExTraceGlobals
	{
		public static Trace DataSourceInfoTracer
		{
			get
			{
				if (ExTraceGlobals.dataSourceInfoTracer == null)
				{
					ExTraceGlobals.dataSourceInfoTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.dataSourceInfoTracer;
			}
		}

		public static Trace DataSourceManagerTracer
		{
			get
			{
				if (ExTraceGlobals.dataSourceManagerTracer == null)
				{
					ExTraceGlobals.dataSourceManagerTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.dataSourceManagerTracer;
			}
		}

		public static Trace DataSourceSessionTracer
		{
			get
			{
				if (ExTraceGlobals.dataSourceSessionTracer == null)
				{
					ExTraceGlobals.dataSourceSessionTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.dataSourceSessionTracer;
			}
		}

		public static Trace FieldTracer
		{
			get
			{
				if (ExTraceGlobals.fieldTracer == null)
				{
					ExTraceGlobals.fieldTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.fieldTracer;
			}
		}

		public static Trace ConfigObjectTracer
		{
			get
			{
				if (ExTraceGlobals.configObjectTracer == null)
				{
					ExTraceGlobals.configObjectTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.configObjectTracer;
			}
		}

		public static Trace PropertyBagTracer
		{
			get
			{
				if (ExTraceGlobals.propertyBagTracer == null)
				{
					ExTraceGlobals.propertyBagTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.propertyBagTracer;
			}
		}

		public static Trace QueryParserTracer
		{
			get
			{
				if (ExTraceGlobals.queryParserTracer == null)
				{
					ExTraceGlobals.queryParserTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.queryParserTracer;
			}
		}

		public static Trace SchemaManagerTracer
		{
			get
			{
				if (ExTraceGlobals.schemaManagerTracer == null)
				{
					ExTraceGlobals.schemaManagerTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.schemaManagerTracer;
			}
		}

		public static Trace SecurityMangerTracer
		{
			get
			{
				if (ExTraceGlobals.securityMangerTracer == null)
				{
					ExTraceGlobals.securityMangerTracer = new Trace(ExTraceGlobals.componentGuid, 8);
				}
				return ExTraceGlobals.securityMangerTracer;
			}
		}

		public static Trace ConfigObjectReaderTracer
		{
			get
			{
				if (ExTraceGlobals.configObjectReaderTracer == null)
				{
					ExTraceGlobals.configObjectReaderTracer = new Trace(ExTraceGlobals.componentGuid, 9);
				}
				return ExTraceGlobals.configObjectReaderTracer;
			}
		}

		public static Trace IdentityTracer
		{
			get
			{
				if (ExTraceGlobals.identityTracer == null)
				{
					ExTraceGlobals.identityTracer = new Trace(ExTraceGlobals.componentGuid, 10);
				}
				return ExTraceGlobals.identityTracer;
			}
		}

		public static Trace RoleBasedStringMappingTracer
		{
			get
			{
				if (ExTraceGlobals.roleBasedStringMappingTracer == null)
				{
					ExTraceGlobals.roleBasedStringMappingTracer = new Trace(ExTraceGlobals.componentGuid, 11);
				}
				return ExTraceGlobals.roleBasedStringMappingTracer;
			}
		}

		private static Guid componentGuid = new Guid("b643f45b-9d4a-4186-ba92-05d5c229d692");

		private static Trace dataSourceInfoTracer = null;

		private static Trace dataSourceManagerTracer = null;

		private static Trace dataSourceSessionTracer = null;

		private static Trace fieldTracer = null;

		private static Trace configObjectTracer = null;

		private static Trace propertyBagTracer = null;

		private static Trace queryParserTracer = null;

		private static Trace schemaManagerTracer = null;

		private static Trace securityMangerTracer = null;

		private static Trace configObjectReaderTracer = null;

		private static Trace identityTracer = null;

		private static Trace roleBasedStringMappingTracer = null;
	}
}
