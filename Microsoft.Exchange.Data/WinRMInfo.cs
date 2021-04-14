using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.Security;

namespace Microsoft.Exchange.Data
{
	internal class WinRMInfo
	{
		public string Action { get; set; }

		public string RawAction { get; set; }

		public string SessionId { get; set; }

		public string FomattedSessionId
		{
			get
			{
				if (string.IsNullOrWhiteSpace(this.SessionId))
				{
					return null;
				}
				string text = this.SessionId;
				if (text.StartsWith("uuid:"))
				{
					text = text.Substring("uuid:".Length);
				}
				Guid guid;
				if (Guid.TryParse(text, out guid))
				{
					return text;
				}
				return null;
			}
		}

		public string ShellId { get; set; }

		public string CommandId { get; set; }

		public string CommandName { get; set; }

		public string SignalCode { get; set; }

		public string SessionUniqueId
		{
			get
			{
				return this.SessionId ?? this.ShellId;
			}
		}

		public static void StampToHttpHeaders(WinRMInfo winRMInfo, NameValueCollection httpHeaders)
		{
			httpHeaders["X-RemotePs-Action"] = winRMInfo.Action;
			httpHeaders["X-RemotePs-RawAction"] = winRMInfo.RawAction;
			httpHeaders["X-RemotePs-SessionId"] = winRMInfo.SessionId;
			httpHeaders["X-RemotePs-ShellId"] = winRMInfo.ShellId;
			httpHeaders["X-RemotePs-CommandId"] = winRMInfo.CommandId;
			httpHeaders["X-RemotePs-CommandName"] = winRMInfo.CommandName;
			httpHeaders["X-RemotePs-SignalCode"] = winRMInfo.SignalCode;
		}

		public static WinRMInfo GetWinRMInfoFromHttpHeaders(NameValueCollection httpHeaders)
		{
			WinRMInfo winRMInfo = null;
			if (httpHeaders != null)
			{
				winRMInfo = new WinRMInfo();
				winRMInfo.Action = httpHeaders["X-RemotePs-Action"];
				winRMInfo.RawAction = httpHeaders["X-RemotePs-RawAction"];
				winRMInfo.SessionId = httpHeaders["X-RemotePs-SessionId"];
				winRMInfo.ShellId = httpHeaders["X-RemotePs-ShellId"];
				winRMInfo.CommandId = httpHeaders["X-RemotePs-CommandId"];
				winRMInfo.CommandName = httpHeaders["X-RemotePs-CommandName"];
				winRMInfo.SignalCode = httpHeaders["X-RemotePs-SignalCode"];
			}
			return winRMInfo;
		}

		public static bool IsHeaderReserverd(string headerName)
		{
			return "X-RemotePs-Action".Equals(headerName, StringComparison.OrdinalIgnoreCase) || "X-RemotePs-RawAction".Equals(headerName, StringComparison.OrdinalIgnoreCase) || "X-RemotePs-SessionId".Equals(headerName, StringComparison.OrdinalIgnoreCase) || "X-RemotePs-ShellId".Equals(headerName, StringComparison.OrdinalIgnoreCase) || "X-RemotePs-CommandId".Equals(headerName, StringComparison.OrdinalIgnoreCase) || "X-RemotePs-CommandName".Equals(headerName, StringComparison.OrdinalIgnoreCase) || "X-RemotePs-SignalCode".Equals(headerName, StringComparison.OrdinalIgnoreCase);
		}

		public static void SetFailureCategoryInfo(NameValueCollection httpHeaders, FailureCategory fc, string fcSubInfo)
		{
			if (string.IsNullOrEmpty(httpHeaders["X-RemotePS-FailureCategory"]))
			{
				httpHeaders["X-RemotePS-FailureCategory"] = string.Format("{0}-{1}", fc, fcSubInfo);
			}
		}

		public static void ClearFailureCategoryInfo(NameValueCollection httpHeaders)
		{
			httpHeaders.Remove("X-RemotePS-FailureCategory");
		}

		public static string GetFailureCategoryInfo(HttpContext context)
		{
			if (!string.IsNullOrEmpty(context.Response.Headers["X-RemotePS-FailureCategory"]))
			{
				return context.Response.Headers["X-RemotePS-FailureCategory"];
			}
			if (context.Items["LiveIdBasicAuthResult"] != null && string.Compare(context.Items["LiveIdBasicAuthResult"].ToString(), LiveIdAuthResult.Success.ToString()) != 0)
			{
				return string.Format("{0}-{1}", FailureCategory.LiveID, context.Items["LiveIdBasicAuthResult"]);
			}
			return null;
		}

		public static void SetWSManFailureCategory(NameValueCollection httpHeaders, string wsmanFaultMessage)
		{
			if (string.IsNullOrEmpty(wsmanFaultMessage) || wsmanFaultMessage.Contains("FailureCategory"))
			{
				return;
			}
			string fcSubInfo = "Others";
			string text = WinRMInfo.knownWSManError.Keys.FirstOrDefault((string key) => wsmanFaultMessage.Contains(key));
			if (!string.IsNullOrEmpty(text))
			{
				fcSubInfo = WinRMInfo.knownWSManError[text];
			}
			WinRMInfo.SetFailureCategoryInfo(httpHeaders, FailureCategory.WSMan, fcSubInfo);
		}

		private const string ActionHeaderKey = "X-RemotePs-Action";

		private const string RawActionHeaderKey = "X-RemotePs-RawAction";

		private const string SessionIdHeaderKey = "X-RemotePs-SessionId";

		private const string ShellIdHeaderKey = "X-RemotePs-ShellId";

		private const string CommandIdHeaderKey = "X-RemotePs-CommandId";

		private const string CommandNameHeaderKey = "X-RemotePs-CommandName";

		private const string SignalCodeHeaderKey = "X-RemotePs-SignalCode";

		private const string UuidPrefix = "uuid:";

		internal const string PingHeaderKey = "X-RemotePS-Ping";

		internal const string RevisedActionHeaderKey = "X-RemotePS-RevisedAction";

		internal const string WinRMInfoSessionItemKey = "X-RemotePS-WinRMInfo";

		internal const string FailureCategoryItemKey = "X-RemotePS-FailureCategory";

		internal const string NewPSSessionAction = "http://schemas.xmlsoap.org/ws/2004/09/transfer/Create";

		internal const string CommandAction = "http://schemas.microsoft.com/wbem/wsman/1/windows/shell/Command";

		internal const string SignalAction = "http://schemas.microsoft.com/wbem/wsman/1/windows/shell/Signal";

		internal const string TerminateSignalCode = "http://schemas.microsoft.com/wbem/wsman/1/windows/shell/signal/terminate";

		internal const string KnownPingActionName = "Ping";

		internal const string KnownNonPingActionName = "Non-Ping";

		internal const string KnownPossiblePingActionName = "Possible-Ping";

		internal const string KnownTerminateActionName = "Terminate";

		internal const string KnownReceiveActionName = "Receive";

		internal const string KnownCommandReceiveActionName = "Command:Receive";

		internal const string KnownNewPSSessionActionName = "New-PSSession";

		internal const string KnownRemovePSSessionActionName = "Remove-PSSession";

		internal const string DiagnosticsInfoFmt = "[Server={0},RequestId={1},TimeStamp={2}] ";

		internal const string CafeDiagnosticsInfoFmt = "[ClientAccessServer={0},BackEndServer={1},RequestId={2},TimeStamp={3}] ";

		internal const string FailureCategoryInfoFmt = "[FailureCategory={0}] ";

		internal const string LiveIdBasicAuthResultKey = "LiveIdBasicAuthResult";

		internal static Dictionary<string, string> KnownActions = new Dictionary<string, string>
		{
			{
				"http://schemas.xmlsoap.org/ws/2004/09/transfer/Create",
				"New-PSSession"
			},
			{
				"http://schemas.xmlsoap.org/ws/2004/09/transfer/Delete",
				"Remove-PSSession"
			},
			{
				"http://schemas.microsoft.com/wbem/wsman/1/windows/shell/Receive",
				"Receive"
			},
			{
				"http://schemas.microsoft.com/wbem/wsman/1/windows/shell/Send",
				"Send"
			},
			{
				"http://schemas.microsoft.com/wbem/wsman/1/windows/shell/Command",
				"Command"
			},
			{
				"http://schemas.microsoft.com/wbem/wsman/1/windows/shell/Signal",
				"Signal"
			}
		};

		private static Dictionary<string, string> knownWSManError = new Dictionary<string, string>
		{
			{
				"because the shell was not found on the server.",
				"InvalidShellID"
			},
			{
				"Access is denied.",
				"AccessIsDenied"
			},
			{
				"Unable to load assembly",
				"UnableToLoadAssembly"
			},
			{
				"The supplied command context is not valid.",
				"InvalidCommandContext"
			},
			{
				"The WS-Management service cannot complete the operation within the time specified in OperationTimeout.",
				"OperationTimeout"
			},
			{
				"identifier does not give a valid initial session state on the remote computer.",
				"InvalidInitialSessionState"
			},
			{
				"The supplied shell context is not valid",
				"InvalidShellContext"
			},
			{
				"The Windows Remote Shell received a request to perform an operation on a command identifier that does not exist.",
				"NonExistCommandID"
			},
			{
				"The I/O operation has been aborted because of either a thread exit or an application request.",
				"IOOperationAborted"
			},
			{
				"PowerShell plugin operation is shutting down. This may happen if the hosting service or application is shutting down.",
				"PluginOperationShutDown"
			},
			{
				"A device attached to the system is not functioning.",
				"DeviceNotFunctioning"
			},
			{
				"The operation was canceled by the user.",
				"OperationCancelled"
			},
			{
				"WinRM cannot process the request because the input XML contains a syntax error.",
				"XMLSyntaxError"
			},
			{
				"The WS-Management service cannot process the request. Operations involving multiple messages from the client are not supported.",
				"MultipleClientMessages"
			}
		};
	}
}
