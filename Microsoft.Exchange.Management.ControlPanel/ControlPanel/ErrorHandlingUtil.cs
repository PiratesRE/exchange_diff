using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.ObjectModel;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mapi.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Management.Tasks.UM;
using Microsoft.Exchange.PowerShell.RbacHostingTools;
using Microsoft.Exchange.UM.Prompts.Provisioning;
using Microsoft.Exchange.UM.Rpc;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class ErrorHandlingUtil
	{
		static ErrorHandlingUtil()
		{
			string value = WebConfigurationManager.AppSettings["ShowDebugInformation"];
			ErrorHandlingUtil.ShowDebugInformation = (string.IsNullOrEmpty(value) ? ShowDebugInforamtion.None : ((ShowDebugInforamtion)Enum.Parse(typeof(ShowDebugInforamtion), value, true)));
			ErrorHandlingUtil.AddSourceToErrorMessages = ConfigUtil.ReadBool("AddSourceToErrorMessages", false);
		}

		public static ShowDebugInforamtion ShowDebugInformation { get; private set; }

		public static bool AddSourceToErrorMessages { get; private set; }

		public static bool CanShowDebugInfo(Exception exception)
		{
			return ErrorHandlingUtil.ShowDebugInformation == ShowDebugInforamtion.All || (ErrorHandlingUtil.ShowDebugInformation == ShowDebugInforamtion.Unknown && !ErrorHandlingUtil.KnownExceptionList.Contains(exception.GetType()) && !ErrorHandlingUtil.KnownReflectedExceptions.Value.ContainsValue(exception.GetType()));
		}

		public static void TransferToErrorPage(string cause)
		{
			ErrorHandlingModule.TransferToErrorPage(cause, false);
		}

		public static void ShowServerError(string errorString, string detailString, System.Web.UI.Page page)
		{
			ModalDialog current = ModalDialog.GetCurrent(page);
			current.ShowDialog(ClientStrings.Error, errorString, string.Empty, ModalDialogType.Error);
		}

		public static void ShowServerErrors(ErrorInformationBase[] errorInfos, System.Web.UI.Page page)
		{
			if (errorInfos != null && errorInfos.Length > 0)
			{
				ErrorHandlingUtil.ShowServerErrors(errorInfos.ToInfos(), page);
			}
		}

		public static void ShowServerError(InfoCore errorInfo, System.Web.UI.Page page)
		{
			if (errorInfo != null)
			{
				ErrorHandlingUtil.ShowServerErrors(new InfoCore[]
				{
					errorInfo
				}, page);
			}
		}

		public static void ShowServerErrors(InfoCore[] errorInfos, System.Web.UI.Page page)
		{
			if (errorInfos != null && errorInfos.Length > 0)
			{
				Dictionary<string, InfoCore> dictionary = new Dictionary<string, InfoCore>();
				Array.ForEach<InfoCore>(errorInfos, delegate(InfoCore x)
				{
					if (!dictionary.ContainsKey(x.Message))
					{
						dictionary.Add(x.Message, x);
					}
				});
				ModalDialog current = ModalDialog.GetCurrent(page);
				current.ShowDialog(dictionary.Values.ToArray<InfoCore>());
			}
		}

		public static void SendReportForCriticalException(HttpContext context, Exception exception)
		{
			if (exception.IsUICriticalException() && exception.IsControlPanelException())
			{
				ExWatson.AddExtraData(ErrorHandlingUtil.GetEcpWatsonExtraData(context, exception));
				EcpPerfCounters.SendWatson.Increment();
				RbacPrincipal rbacPrincipal = context.User as RbacPrincipal;
				if (rbacPrincipal != null && ExTraceConfiguration.Instance.InMemoryTracingEnabled)
				{
					TroubleshootingContext troubleshootingContext = rbacPrincipal.RbacConfiguration.TroubleshootingContext;
					troubleshootingContext.SendTroubleshootingReportWithTraces(exception, ErrorHandlingUtil.GetEcpWatsonTitle(exception, context));
					return;
				}
				ExWatson.SendReport(exception, ReportOptions.ReportTerminateAfterSend, exception.GetCustomMessage());
			}
		}

		public static string GetEcpWatsonExtraData(HttpContext context, Exception ex)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (context != null && context.Request != null)
			{
				stringBuilder.Append("context.Request.Url = ");
				stringBuilder.AppendLine(context.GetRequestUrlForLog());
				if (context.HasTargetTenant() || context.IsExplicitSignOn())
				{
					stringBuilder.Append("RawUrl = ");
					stringBuilder.AppendLine(context.Request.RawUrl);
				}
			}
			if (context != null && context.Request.Cookies != null)
			{
				HttpCookie httpCookie = context.Request.Cookies["TCMID"];
				if (httpCookie != null)
				{
					stringBuilder.Append("TestCaseID: ");
					stringBuilder.AppendLine(httpCookie.Value);
				}
			}
			foreach (object obj in ex.Data.Keys)
			{
				object obj2 = ex.Data[obj];
				stringBuilder.Append(obj);
				stringBuilder.Append(": ");
				stringBuilder.AppendLine((obj2 == null) ? "null" : obj2.ToString());
			}
			if (ex is OutOfMemoryException)
			{
				stringBuilder.Append("Managed Memory: ").AppendLine(GC.GetTotalMemory(false).ToString());
				using (Process currentProcess = Process.GetCurrentProcess())
				{
					stringBuilder.Append("Private Bytes: ").AppendLine(currentProcess.PrivateMemorySize64.ToString());
					stringBuilder.Append("Working Set: ").AppendLine(currentProcess.WorkingSet64.ToString());
				}
				NativeMethods.MEMORYSTATUSEX memorystatusex = default(NativeMethods.MEMORYSTATUSEX);
				memorystatusex.dwLength = (uint)Marshal.SizeOf(memorystatusex);
				if (NativeMethods.GlobalMemoryStatusEx(ref memorystatusex))
				{
					stringBuilder.Append("Available Physical Memory: ").Append(memorystatusex.ullAvailPhys.ToString()).Append("/").Append(memorystatusex.ullTotalPhys.ToString()).Append("(").Append((memorystatusex.ullAvailPhys * 100UL / memorystatusex.ullTotalPhys).ToString()).AppendLine("%)");
					stringBuilder.Append("Available Virtual Memory: ").Append(memorystatusex.ullAvailVirtual.ToString()).Append("/").Append(memorystatusex.ullTotalPhys.ToString()).Append("(").Append((memorystatusex.ullAvailVirtual * 100UL / memorystatusex.ullTotalVirtual).ToString()).AppendLine("%)");
				}
				stringBuilder.AppendLine("**Top 5 Apps:**");
				foreach (Process process in (from x in Process.GetProcesses()
				orderby x.PrivateMemorySize64 descending
				select x).Take(5))
				{
					using (process)
					{
						stringBuilder.Append(process.ProcessName).Append("#").Append(process.Id.ToString()).Append(": ").AppendLine(process.PrivateMemorySize64.ToString());
					}
				}
			}
			if (stringBuilder.Length == 0)
			{
				stringBuilder.Append("Couldn't extract extra watson data from the context or exception.");
			}
			return stringBuilder.ToString();
		}

		public static string GetEcpWatsonTitle(Exception exception, HttpContext context)
		{
			string str;
			if (exception.TargetSite != null && exception.TargetSite.ReflectedType != null && exception.TargetSite.ReflectedType.FullName != null && exception.TargetSite.Name != null)
			{
				str = exception.TargetSite.ReflectedType.FullName + "." + exception.TargetSite.Name;
			}
			else
			{
				str = "unknown function";
			}
			return str + " " + context.GetRequestUrlAbsolutePath();
		}

		private static Dictionary<string, Type> InitializeKnownReflectedExceptions()
		{
			Dictionary<string, Type> dictionary = new Dictionary<string, Type>();
			foreach (string text in ErrorHandlingUtil.ReflectedExceptionDefinitions)
			{
				try
				{
					Type type = Type.GetType(text);
					if (null != type)
					{
						dictionary.Add(text, type);
					}
				}
				catch (ArgumentException)
				{
				}
				catch (TypeLoadException)
				{
				}
				catch (FileLoadException)
				{
				}
				catch (BadImageFormatException)
				{
				}
			}
			return dictionary;
		}

		public const string EcpErrorHeaderName = "X-ECP-ERROR";

		private static readonly Type[] KnownExceptionList = new Type[]
		{
			typeof(ParameterBindingException),
			typeof(DataValidationException),
			typeof(WLCDDomainException),
			typeof(ManagementObjectNotFoundException),
			typeof(ADNoSuchObjectException),
			typeof(ShouldContinueException),
			typeof(MapiObjectNotFoundException),
			typeof(ADInvalidPasswordException),
			typeof(ADObjectAlreadyExistsException),
			typeof(MapiObjectAlreadyExistsException),
			typeof(TrackingSearchException),
			typeof(ADScopeException),
			typeof(SecurityException),
			typeof(SelfMemberAlreadyExistsException),
			typeof(SelfMemberNotFoundException),
			typeof(DefaultPinGenerationException),
			typeof(UMRecipient),
			typeof(UMRpcException),
			typeof(ExtensionNotUniqueException),
			typeof(RpcUMServerNotFoundException),
			typeof(GlobalRoutingEntryNotFoundException),
			typeof(InvalidOperationForGetUMMailboxPinException),
			typeof(WeakPinException),
			typeof(InvalidExtensionException),
			typeof(ExtensionNotUniqueException),
			typeof(UserAlreadyUmEnabledException),
			typeof(InvalidSipNameResourceIdException),
			typeof(InvalidE164ResourceIdException),
			typeof(CannotDeleteAssociatedMailboxPolicyException),
			typeof(InvalidUMFaxServerURIValue),
			typeof(UMMailboxPolicyValidationException),
			typeof(UnsupportedCustomGreetingWaveFormatException),
			typeof(UnsupportedCustomGreetingWmaFormatException),
			typeof(UnsupportedCustomGreetingWmaFormatException),
			typeof(UnsupportedCustomGreetingSizeFormatException),
			typeof(UnsupportedCustomGreetingLegacyFormatException),
			typeof(DialPlanAssociatedWithPoliciesException),
			typeof(UnableToCreateGatewayObjectException),
			typeof(AutoAttendantExistsException),
			typeof(SIPResouceIdConflictWithExistingValue),
			typeof(InvalidDtmfFallbackAutoAttendantException),
			typeof(InvalidAutoAttendantException),
			typeof(InvalidCustomMenuException),
			typeof(UserAlreadyUmDisabledException),
			typeof(ProxyAddressExistsException),
			typeof(OverBudgetException),
			typeof(AdUserNotFoundException),
			typeof(NonUniqueRecipientException),
			typeof(ServerNotInSiteException),
			typeof(LowVersionUserDeniedException),
			typeof(CmdletAccessDeniedException),
			typeof(UrlNotFoundOrNoAccessException),
			typeof(BadRequestException),
			typeof(BadQueryParameterException),
			typeof(ProxyCantFindCasServerException),
			typeof(CasServerNotSupportEsoException)
		};

		internal static Lazy<Dictionary<string, Type>> KnownReflectedExceptions = new Lazy<Dictionary<string, Type>>(new Func<Dictionary<string, Type>>(ErrorHandlingUtil.InitializeKnownReflectedExceptions));

		private static readonly string[] ReflectedExceptionDefinitions = new string[]
		{
			"Microsoft.Exchange.Hygiene.Security.Authorization.NoValidRolesAssociatedToUserException, Microsoft.Exchange.Hygiene.Security.Authorization"
		};

		public static readonly bool ShowIisNativeErrorPage = ConfigUtil.ReadBool("ShowIisNativeErrorPage", false);
	}
}
