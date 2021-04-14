using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;
using System.Web;
using Microsoft.Exchange.Configuration.Core;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Diagnostics.Components.Authorization;

namespace Microsoft.Exchange.Configuration.ObjectModel
{
	internal class PswsErrorHandling : TaskIOPipelineBase, ITaskModule, ICriticalFeature
	{
		private List<LocalizedString> Warnings { get; set; }

		public PswsErrorHandling(TaskContext context)
		{
			this.context = context;
		}

		bool ICriticalFeature.IsCriticalException(Exception ex)
		{
			return false;
		}

		public void Init(ITaskEvent task)
		{
			task.Error += this.Task_Error;
			task.Stop += this.OnStop;
			task.Release += this.OnRelease;
			this.context.CommandShell.PrependTaskIOPipelineHandler(this);
		}

		public void Dispose()
		{
		}

		public override bool ShouldContinue(string query, string caption, ref bool yesToAll, ref bool noToAll, out bool? output)
		{
			output = null;
			PswsErrorHandling.SendErrorToClient(PswsErrorCode.CmdletShouldContinue, null, query);
			return true;
		}

		public override bool ShouldProcess(string verboseDescription, string verboseWarning, string caption, out bool? output)
		{
			output = new bool?(true);
			return false;
		}

		internal static void SendErrorToClient(PswsErrorCode errorCode, Exception exception, string additionalInfo)
		{
			ExTraceGlobals.PublicPluginAPITracer.TraceDebug<PswsErrorCode, Exception, string>(0L, "[PswsErrorHandling.SendErrorToClient] Error Code = {0}, Exception = \"{1}\", additionalInfo = \"{2}\".", errorCode, exception, additionalInfo);
			try
			{
				HttpContext httpContext = HttpContext.Current;
				if (httpContext != null)
				{
					HttpResponse response = httpContext.Response;
					if (response != null)
					{
						if (response.Headers["X-Psws-ErrorCode"] == null)
						{
							response.AddHeader("X-Psws-ErrorCode", ((int)PswsErrorHandling.TranslateErrorCode(errorCode, exception, additionalInfo)).ToString());
							HttpLogger.SafeSetLogger(ServiceCommonMetadata.ErrorCode, errorCode);
							if (exception != null)
							{
								string text = exception.GetType() + "," + exception.Message;
								if (text.Length > 500)
								{
									text = text.Substring(0, 500 - "...(truncated)".Length);
									text += "...(truncated)";
								}
								response.AddHeader("X-Psws-Exception", text);
								HttpLogger.SafeAppendGenericInfo("PswsExceptionInfo", text);
							}
							if (!string.IsNullOrWhiteSpace(additionalInfo))
							{
								response.AddHeader("X-Psws-ErrorInfo", additionalInfo);
								HttpLogger.SafeAppendGenericInfo("PswsErrorAdditionalInfo", additionalInfo);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				ExTraceGlobals.PublicPluginAPITracer.TraceDebug<Exception>(0L, "[PswsErrorHandling.SendErrorToClient] Exception {0}.", ex);
				HttpLogger.SafeAppendGenericError("SendErrorToClientError", ex, new Func<Exception, bool>(KnownException.IsUnhandledException));
			}
		}

		private static PswsErrorCode TranslateErrorCode(PswsErrorCode orginalErrorCode, Exception exception, string additionalInfo)
		{
			if (orginalErrorCode == PswsErrorCode.CmdletExecutionFailure)
			{
				foreach (KeyValuePair<PswsErrorCode, Type[]> keyValuePair in PswsErrorHandling.ExceptionErrorCodeMapping)
				{
					foreach (Type type in keyValuePair.Value)
					{
						if (type.IsInstanceOfType(exception))
						{
							return keyValuePair.Key;
						}
					}
				}
				return orginalErrorCode;
			}
			return orginalErrorCode;
		}

		private void Task_Error(object sender, GenericEventArg<TaskErrorEventArg> e)
		{
			if (e.Data.ExceptionHandled)
			{
				return;
			}
			PswsErrorHandling.SendErrorToClient(PswsErrorCode.CmdletExecutionFailure, e.Data.Exception, null);
		}

		public override bool WriteWarning(LocalizedString input, string helperUrl, out LocalizedString output)
		{
			if (this.Warnings == null)
			{
				this.Warnings = new List<LocalizedString>();
			}
			this.Warnings.Add(input);
			return base.WriteWarning(input, helperUrl, out output);
		}

		private void OnStop(object sender, EventArgs eventArgs)
		{
			this.SendWarningToClientIfNeeded();
		}

		private void OnRelease(object sender, EventArgs eventArgs)
		{
			this.SendWarningToClientIfNeeded();
		}

		private void SendWarningToClientIfNeeded()
		{
			if (this.Warnings != null && this.Warnings.Count > 0)
			{
				HttpContext httpContext = HttpContext.Current;
				if (httpContext == null)
				{
					return;
				}
				HttpResponse response = httpContext.Response;
				if (response == null)
				{
					return;
				}
				StringBuilder stringBuilder = new StringBuilder(response.Headers["X-Psws-Warning"]);
				int num = 0;
				while (num < this.Warnings.Count && stringBuilder.Length < 500)
				{
					stringBuilder.Append(this.Warnings[num].ToString());
					num++;
				}
				if (stringBuilder.Length > 500)
				{
					stringBuilder.Length = 500 - "...(truncated)".Length;
					stringBuilder.Append("...(truncated)");
				}
				HttpLogger.SafeAppendGenericInfo("PswsWarningInfo", stringBuilder.ToString());
				try
				{
					response.Headers["X-Psws-Warning"] = stringBuilder.ToString();
				}
				catch (Exception ex)
				{
					ExTraceGlobals.PublicPluginAPITracer.TraceDebug<Exception>(0L, "[PswsErrorHandling.SendErrorToClient] Exception {0}.", ex);
					HttpLogger.SafeAppendGenericError("SendErrorToClientError", ex, new Func<Exception, bool>(KnownException.IsUnhandledException));
				}
				this.Warnings.Clear();
			}
		}

		private const string ErrorCodeHttpHeaderKey = "X-Psws-ErrorCode";

		private const string ExceptionHttpHeaderKey = "X-Psws-Exception";

		private const string AdditionalInfoHttpHeaderKey = "X-Psws-ErrorInfo";

		private const string WarningHttpHeaderKey = "X-Psws-Warning";

		private const int HeaderLenghtLimitation = 500;

		private const string SuffixTruncatedHeader = "...(truncated)";

		private readonly TaskContext context;

		private static readonly Dictionary<PswsErrorCode, Type[]> ExceptionErrorCodeMapping = new Dictionary<PswsErrorCode, Type[]>
		{
			{
				PswsErrorCode.RetriableTransientException,
				new Type[]
				{
					typeof(TransientException)
				}
			},
			{
				PswsErrorCode.ParameterValidationFailed,
				new Type[]
				{
					typeof(DataValidationException),
					typeof(ParameterBindingException)
				}
			},
			{
				PswsErrorCode.DuplicateObjectCreation,
				new Type[]
				{
					typeof(ProxyAddressExistsException)
				}
			},
			{
				PswsErrorCode.ScopePermissionDenied,
				new Type[]
				{
					typeof(OperationRequiresGroupManagerException)
				}
			}
		};
	}
}
