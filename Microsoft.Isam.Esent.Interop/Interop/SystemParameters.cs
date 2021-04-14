using System;

namespace Microsoft.Isam.Esent.Interop
{
	public static class SystemParameters
	{
		public static int CacheSizeMax
		{
			get
			{
				return SystemParameters.GetIntegerParameter(JET_param.CacheSizeMax);
			}
			set
			{
				SystemParameters.SetIntegerParameter(JET_param.CacheSizeMax, value);
			}
		}

		public static int CacheSize
		{
			get
			{
				return SystemParameters.GetIntegerParameter(JET_param.CacheSize);
			}
			set
			{
				SystemParameters.SetIntegerParameter(JET_param.CacheSize, value);
			}
		}

		public static int DatabasePageSize
		{
			get
			{
				return SystemParameters.GetIntegerParameter(JET_param.DatabasePageSize);
			}
			set
			{
				SystemParameters.SetIntegerParameter(JET_param.DatabasePageSize, value);
			}
		}

		public static int CacheSizeMin
		{
			get
			{
				return SystemParameters.GetIntegerParameter(JET_param.CacheSizeMin);
			}
			set
			{
				SystemParameters.SetIntegerParameter(JET_param.CacheSizeMin, value);
			}
		}

		public static int OutstandingIOMax
		{
			get
			{
				return SystemParameters.GetIntegerParameter(JET_param.OutstandingIOMax);
			}
			set
			{
				SystemParameters.SetIntegerParameter(JET_param.OutstandingIOMax, value);
			}
		}

		public static int StartFlushThreshold
		{
			get
			{
				return SystemParameters.GetIntegerParameter(JET_param.StartFlushThreshold);
			}
			set
			{
				SystemParameters.SetIntegerParameter(JET_param.StartFlushThreshold, value);
			}
		}

		public static int StopFlushThreshold
		{
			get
			{
				return SystemParameters.GetIntegerParameter(JET_param.StopFlushThreshold);
			}
			set
			{
				SystemParameters.SetIntegerParameter(JET_param.StopFlushThreshold, value);
			}
		}

		public static int MaxInstances
		{
			get
			{
				return SystemParameters.GetIntegerParameter(JET_param.MaxInstances);
			}
			set
			{
				SystemParameters.SetIntegerParameter(JET_param.MaxInstances, value);
			}
		}

		public static int EventLoggingLevel
		{
			get
			{
				return SystemParameters.GetIntegerParameter(JET_param.EventLoggingLevel);
			}
			set
			{
				SystemParameters.SetIntegerParameter(JET_param.EventLoggingLevel, value);
			}
		}

		public static int KeyMost
		{
			get
			{
				if (EsentVersion.SupportsVistaFeatures)
				{
					return SystemParameters.GetIntegerParameter((JET_param)134);
				}
				return 255;
			}
		}

		public static int ColumnsKeyMost
		{
			get
			{
				return Api.Impl.Capabilities.ColumnsKeyMost;
			}
		}

		public static int BookmarkMost
		{
			get
			{
				return SystemParameters.KeyMost + 1;
			}
		}

		public static int LVChunkSizeMost
		{
			get
			{
				if (EsentVersion.SupportsWindows7Features)
				{
					return SystemParameters.GetIntegerParameter((JET_param)163);
				}
				return SystemParameters.GetIntegerParameter(JET_param.DatabasePageSize) - 82;
			}
		}

		public static int Configuration
		{
			get
			{
				if (EsentVersion.SupportsVistaFeatures)
				{
					return SystemParameters.GetIntegerParameter((JET_param)129);
				}
				return 1;
			}
			set
			{
				if (EsentVersion.SupportsVistaFeatures)
				{
					SystemParameters.SetIntegerParameter((JET_param)129, value);
				}
			}
		}

		public static bool EnableAdvanced
		{
			get
			{
				return !EsentVersion.SupportsVistaFeatures || SystemParameters.GetBoolParameter((JET_param)130);
			}
			set
			{
				if (EsentVersion.SupportsVistaFeatures)
				{
					SystemParameters.SetBoolParameter((JET_param)130, value);
				}
			}
		}

		public static int LegacyFileNames
		{
			get
			{
				if (EsentVersion.SupportsVistaFeatures)
				{
					return SystemParameters.GetIntegerParameter((JET_param)136);
				}
				return 1;
			}
			set
			{
				if (EsentVersion.SupportsVistaFeatures)
				{
					SystemParameters.SetIntegerParameter((JET_param)136, value);
				}
			}
		}

		public static JET_ExceptionAction ExceptionAction
		{
			get
			{
				return (JET_ExceptionAction)SystemParameters.GetIntegerParameter(JET_param.ExceptionAction);
			}
			set
			{
				SystemParameters.SetIntegerParameter(JET_param.ExceptionAction, (int)value);
			}
		}

		public static bool EnableFileCache
		{
			get
			{
				return EsentVersion.SupportsVistaFeatures && SystemParameters.GetBoolParameter((JET_param)126);
			}
			set
			{
				if (EsentVersion.SupportsVistaFeatures)
				{
					SystemParameters.SetBoolParameter((JET_param)126, value);
				}
			}
		}

		public static bool EnableViewCache
		{
			get
			{
				return EsentVersion.SupportsVistaFeatures && SystemParameters.GetBoolParameter((JET_param)127);
			}
			set
			{
				if (EsentVersion.SupportsVistaFeatures)
				{
					SystemParameters.SetBoolParameter((JET_param)127, value);
				}
			}
		}

		private static void SetStringParameter(JET_param param, string value)
		{
			Api.JetSetSystemParameter(JET_INSTANCE.Nil, JET_SESID.Nil, param, 0, value);
		}

		private static string GetStringParameter(JET_param param)
		{
			int num = 0;
			string result;
			Api.JetGetSystemParameter(JET_INSTANCE.Nil, JET_SESID.Nil, param, ref num, out result, 1024);
			return result;
		}

		private static void SetIntegerParameter(JET_param param, int value)
		{
			Api.JetSetSystemParameter(JET_INSTANCE.Nil, JET_SESID.Nil, param, value, null);
		}

		private static int GetIntegerParameter(JET_param param)
		{
			int result = 0;
			string text;
			Api.JetGetSystemParameter(JET_INSTANCE.Nil, JET_SESID.Nil, param, ref result, out text, 0);
			return result;
		}

		private static void SetBoolParameter(JET_param param, bool value)
		{
			int paramValue = value ? 1 : 0;
			Api.JetSetSystemParameter(JET_INSTANCE.Nil, JET_SESID.Nil, param, paramValue, null);
		}

		private static bool GetBoolParameter(JET_param param)
		{
			int num = 0;
			string text;
			Api.JetGetSystemParameter(JET_INSTANCE.Nil, JET_SESID.Nil, param, ref num, out text, 0);
			return num != 0;
		}

		public static int MinDataForXpress
		{
			get
			{
				return SystemParameters.GetIntegerParameter((JET_param)183);
			}
			set
			{
				SystemParameters.SetIntegerParameter((JET_param)183, value);
			}
		}

		public static int HungIOThreshold
		{
			get
			{
				return SystemParameters.GetIntegerParameter((JET_param)181);
			}
			set
			{
				SystemParameters.SetIntegerParameter((JET_param)181, value);
			}
		}

		public static int HungIOActions
		{
			get
			{
				return SystemParameters.GetIntegerParameter((JET_param)182);
			}
			set
			{
				SystemParameters.SetIntegerParameter((JET_param)182, value);
			}
		}

		public static string ProcessFriendlyName
		{
			get
			{
				return SystemParameters.GetStringParameter((JET_param)186);
			}
			set
			{
				SystemParameters.SetStringParameter((JET_param)186, value);
			}
		}

		public const int BaseNameLength = 3;

		public const int NameMost = 64;

		public const int ColumnMost = 255;

		public const int ColumnsMost = 65248;

		public const int ColumnsFixedMost = 127;

		public const int ColumnsVarMost = 128;

		public const int ColumnsTaggedMost = 64993;

		public const int PageTempDBSmallest = 14;

		public const int LocaleNameMaxLength = 85;
	}
}
