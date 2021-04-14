using System;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Diagnostics
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class WatsonHelper
	{
		public static WatsonReportAction RegisterWatsonReportDataProvider(this Activity activity, WatsonReportActionType reportActionType, WatsonHelper.IProvideWatsonReportData evaluationDelegate)
		{
			WatsonReportAction watsonReportAction = WatsonHelper.CreateWatsonReportDataProvider(activity, reportActionType, evaluationDelegate);
			activity.RegisterWatsonReportAction(watsonReportAction);
			return watsonReportAction;
		}

		public static IDisposable RegisterWatsonReportDataProviderAndGetGuard(this Activity activity, WatsonReportActionType reportActionType, WatsonHelper.IProvideWatsonReportData evaluationDelegate)
		{
			return WatsonHelper.ReportActionGuard.Create(activity, WatsonHelper.CreateWatsonReportDataProvider(activity, reportActionType, evaluationDelegate));
		}

		private static string AppName
		{
			get
			{
				if (WatsonHelper.appName == null)
				{
					WatsonHelper.appName = ExWatson.AppName;
				}
				return WatsonHelper.appName;
			}
		}

		private static string AppVersion
		{
			get
			{
				if (WatsonHelper.appVersion == null)
				{
					using (Process currentProcess = Process.GetCurrentProcess())
					{
						Version version;
						if (ExWatson.TryGetRealApplicationVersion(currentProcess, out version))
						{
							WatsonHelper.appVersion = version.ToString();
						}
						else
						{
							WatsonHelper.appVersion = "0";
						}
					}
				}
				return WatsonHelper.appVersion;
			}
		}

		private static bool IsTestEnvironment
		{
			get
			{
				if (WatsonHelper.isTestEnvironment == null)
				{
					string environmentVariable = Environment.GetEnvironmentVariable("USERDNSDOMAIN");
					WatsonHelper.isTestEnvironment = new bool?(environmentVariable != null && environmentVariable.EndsWith(".EXTEST.MICROSOFT.COM", StringComparison.OrdinalIgnoreCase));
				}
				return WatsonHelper.isTestEnvironment.Value;
			}
		}

		public static void SendGenericWatsonReport(string exceptionType, string exceptionDetails)
		{
			Util.ThrowOnNullOrEmptyArgument(exceptionType, "exceptionType");
			if (WatsonHelper.IsTestEnvironment)
			{
				return;
			}
			StackTrace stackTrace = new StackTrace(1);
			MethodBase method = stackTrace.GetFrame(0).GetMethod();
			AssemblyName name = method.DeclaringType.Assembly.GetName();
			int hashCode = (method.Name + exceptionType).GetHashCode();
			ExWatson.SendGenericWatsonReport("E12", WatsonHelper.AppVersion, WatsonHelper.AppName, name.Version.ToString(), name.Name, exceptionType, stackTrace.ToString(), hashCode.ToString("x"), method.Name, exceptionDetails);
		}

		private static bool IsActionEnabled(string actionName)
		{
			return true;
		}

		private static WatsonReportAction CreateWatsonReportDataProvider(Activity activity, WatsonReportActionType reportActionType, WatsonHelper.IProvideWatsonReportData evaluationDelegate)
		{
			string actionName = WatsonHelper.ReportActionTypeToActionName(reportActionType);
			if (!WatsonHelper.IsActionEnabled(actionName))
			{
				return null;
			}
			return new WatsonHelper.DelegateWatsonReportAction(actionName, evaluationDelegate);
		}

		private static string ReportActionTypeToActionName(WatsonReportActionType reportActionType)
		{
			switch (reportActionType)
			{
			case WatsonReportActionType.Connection:
				return "Connection";
			case WatsonReportActionType.IcsDownload:
				return "ICS Download";
			case WatsonReportActionType.MessageAdaptor:
				return "ICS/FX Message";
			case WatsonReportActionType.FolderAdaptor:
				return "ICS/FX Folder";
			case WatsonReportActionType.FastTransferState:
				return "ICS/FX client/processing state";
			default:
				throw new ArgumentOutOfRangeException("reportActionType");
			}
		}

		private static string appName;

		private static string appVersion;

		private static bool? isTestEnvironment;

		public interface IProvideWatsonReportData
		{
			string GetWatsonReportString();
		}

		private sealed class ReportActionGuard : BaseObject
		{
			private ReportActionGuard(Activity activity, WatsonReportAction reportAction)
			{
				using (DisposeGuard disposeGuard = this.Guard())
				{
					Util.ThrowOnNullArgument(reportAction, "reportAction");
					this.activity = activity;
					this.reportAction = reportAction;
					this.activity.RegisterWatsonReportAction(this.reportAction);
					disposeGuard.Success();
				}
			}

			public static IDisposable Create(Activity activity, WatsonReportAction reportAction)
			{
				if (reportAction == null || !WatsonHelper.IsActionEnabled(reportAction.ActionName))
				{
					return null;
				}
				return new WatsonHelper.ReportActionGuard(activity, reportAction);
			}

			protected override void InternalDispose()
			{
				this.activity.UnregisterWatsonReportAction(this.reportAction);
				base.InternalDispose();
			}

			protected override DisposeTracker GetDisposeTracker()
			{
				return DisposeTracker.Get<WatsonHelper.ReportActionGuard>(this);
			}

			private readonly Activity activity;

			private readonly WatsonReportAction reportAction;
		}

		private sealed class DelegateWatsonReportAction : WatsonReportAction
		{
			public DelegateWatsonReportAction(string actionName, WatsonHelper.IProvideWatsonReportData dataProvider) : base(null, true)
			{
				Util.ThrowOnNullArgument(dataProvider, "dataProvider");
				this.actionName = actionName;
				this.dataProvider = dataProvider;
			}

			public override string ActionName
			{
				get
				{
					return this.actionName;
				}
			}

			public override string Evaluate(WatsonReport watsonReport)
			{
				string result;
				try
				{
					result = (this.dataProvider.GetWatsonReportString() ?? string.Empty);
				}
				catch (Exception ex)
				{
					result = ex.ToString();
				}
				return result;
			}

			public override bool Equals(object obj)
			{
				WatsonHelper.DelegateWatsonReportAction delegateWatsonReportAction = obj as WatsonHelper.DelegateWatsonReportAction;
				return delegateWatsonReportAction != null && this.dataProvider.Equals(delegateWatsonReportAction.dataProvider);
			}

			public override int GetHashCode()
			{
				return this.dataProvider.GetHashCode();
			}

			private readonly string actionName;

			private readonly WatsonHelper.IProvideWatsonReportData dataProvider;
		}
	}
}
