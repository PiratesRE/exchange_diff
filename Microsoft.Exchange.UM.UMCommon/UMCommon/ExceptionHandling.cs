using System;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Common.IL;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class ExceptionHandling
	{
		public static void MapAndReportGrayExceptions(GrayException.UserCodeDelegate tryCode)
		{
			ExceptionHandling.MapAndReportGrayExceptions(tryCode, null);
		}

		public static void MapAndReportGrayExceptions(GrayException.UserCodeDelegate tryCode, GrayException.IsGrayExceptionDelegate isGrayExceptionDelegate)
		{
			ExceptionHandling.<>c__DisplayClass4 CS$<>8__locals1 = new ExceptionHandling.<>c__DisplayClass4();
			CS$<>8__locals1.tryCode = tryCode;
			CS$<>8__locals1.isGrayExceptionDelegate = isGrayExceptionDelegate;
			ILUtil.DoTryFilterCatch(new TryDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<MapAndReportGrayExceptions>b__0)), new FilterDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<MapAndReportGrayExceptions>b__1)), new CatchDelegate(null, (UIntPtr)ldftn(ExceptionCatcher)));
		}

		internal static void SendWatsonWithExtraData(Exception e, bool fatal)
		{
			ExceptionHandling.SendWatsonWithExtraData(e, null, fatal);
		}

		internal static void SendWatsonWithExtraData(Exception e, string fileName, bool fatal)
		{
			using (ITempFile tempFile = Breadcrumbs.GenerateDump())
			{
				using (ITempFile tempFile2 = ProcessLog.GenerateDump())
				{
					if (fileName != null)
					{
						ExWatson.TryAddExtraFile(fileName);
					}
					ExWatson.TryAddExtraFile(tempFile.FilePath);
					ExWatson.TryAddExtraFile(tempFile2.FilePath);
					ExWatson.SendReport(e, fatal ? ReportOptions.ReportTerminateAfterSend : ReportOptions.None, null);
				}
			}
		}

		internal static void SendWatsonWithoutDump(Exception e)
		{
			ExWatson.SendReport(e, ReportOptions.DoNotCollectDumps, null);
		}

		internal static void SendWatsonWithoutDumpWithExtraData(Exception e)
		{
			using (ITempFile tempFile = Breadcrumbs.GenerateDump())
			{
				using (ITempFile tempFile2 = ProcessLog.GenerateDump())
				{
					ExWatson.TryAddExtraFile(tempFile.FilePath);
					ExWatson.TryAddExtraFile(tempFile2.FilePath);
					ExWatson.SendReport(e, ReportOptions.DoNotCollectDumps, null);
				}
			}
		}

		private static bool ExceptionFilter(object exception, GrayException.IsGrayExceptionDelegate isGrayExceptionDelegate)
		{
			Exception ex = exception as Exception;
			if (ex == null)
			{
				return false;
			}
			if (!isGrayExceptionDelegate(ex))
			{
				return false;
			}
			ExceptionHandling.SendWatsonWithExtraData(ex, false);
			return true;
		}

		private static void ExceptionCatcher(object exception)
		{
			throw new UMGrayException((Exception)exception);
		}
	}
}
