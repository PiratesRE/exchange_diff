using System;
using System.Net;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.RightsManagement;
using Microsoft.Exchange.Transport.RightsManagement;

namespace Microsoft.Exchange.MessagingPolicies.RmSvcAgent
{
	internal static class RmsEventLogHandler
	{
		public static void LogException(ExEventLog logger, MailItem mailItem, RmsComponent component, Exception exception, bool permanentFailure)
		{
			if (logger == null)
			{
				throw new ArgumentNullException("logger");
			}
			if (mailItem == null)
			{
				throw new ArgumentNullException("mailItem");
			}
			if (exception == null)
			{
				throw new ArgumentNullException("exception");
			}
			RmsEventLogHandler.EventParam param = new RmsEventLogHandler.EventParam(mailItem, component, exception, permanentFailure);
			if (!RmsEventLogHandler.LogRightsManagementException(logger, param))
			{
				RmsEventLogHandler.LogGeneralException(logger, param);
			}
		}

		private static bool LogRightsManagementException(ExEventLog logger, RmsEventLogHandler.EventParam param)
		{
			if (string.IsNullOrEmpty(param.RmsObjectName) || !(param.Exception is RightsManagementException))
			{
				return false;
			}
			RightsManagementException ex = (RightsManagementException)param.Exception;
			RightsManagementFailureCode failureCode = ex.FailureCode;
			if (ex.InnerException is WebException && ex.IsPermanent)
			{
				RmsEventLogHandler.LogPermanentWebException(logger, param, failureCode);
				return true;
			}
			if (failureCode == RightsManagementFailureCode.ServerRightNotGranted && param.RmsComponent == RmsComponent.JournalReportDecryptionAgent)
			{
				RmsEventLogHandler.LogNoRightSoapException(logger, param);
				return true;
			}
			return false;
		}

		private static void LogPermanentWebException(ExEventLog logger, RmsEventLogHandler.EventParam param, RightsManagementFailureCode failureCode)
		{
			WebException ex = param.Exception.InnerException as WebException;
			string text = string.Format("WebExceptionStatus = {0}", ex.Status);
			string text2 = param.IsEnterprise ? ex.Status.ToString() : string.Format("{0}_{1}", param.TenantId, ex.Status);
			if (ex.Status == WebExceptionStatus.ProtocolError)
			{
				HttpWebResponse httpWebResponse = ex.Response as HttpWebResponse;
				if (httpWebResponse != null)
				{
					text2 = string.Format("{0}_{1}", text2, httpWebResponse.StatusCode);
					text = string.Format("{0} | HttpStatusCode = {1}", text, httpWebResponse.StatusCode);
				}
			}
			switch (failureCode)
			{
			case RightsManagementFailureCode.Http3xxFailure:
			case RightsManagementFailureCode.ConnectFailure:
			{
				LocalizedString localizedString = RMSvcAgentStrings.RmsErrorTextForConnectFailure(param.TenantName, param.RmsComponentName, param.RmsObjectName, param.RmsServerName, text);
				logger.LogEvent(MessagingPoliciesEventLogConstants.Tuple_RmsConnectFailure, text2, new object[]
				{
					localizedString,
					param.EffectText,
					param.Exception
				});
				return;
			}
			case RightsManagementFailureCode.TrustFailure:
			{
				LocalizedString localizedString = RMSvcAgentStrings.RmsErrorTextForTrustFailure(param.TenantName, param.RmsComponentName, param.RmsObjectName, param.RmsServerName, text);
				logger.LogEvent(MessagingPoliciesEventLogConstants.Tuple_RmsTrustFailure, text2, new object[]
				{
					localizedString,
					param.EffectText,
					param.Exception
				});
				return;
			}
			case RightsManagementFailureCode.HttpUnauthorizedFailure:
			{
				LocalizedString localizedString = RMSvcAgentStrings.RmsErrorTextFor401(param.TenantName, param.RmsComponentName, param.RmsObjectName, param.RmsServerName, text);
				logger.LogEvent(MessagingPoliciesEventLogConstants.Tuple_Rms401Failure, text2, new object[]
				{
					localizedString,
					param.EffectText,
					param.Exception
				});
				return;
			}
			case RightsManagementFailureCode.HttpForbiddenFailure:
			{
				LocalizedString localizedString = RMSvcAgentStrings.RmsErrorTextFor403(param.TenantName, param.RmsComponentName, param.RmsObjectName, param.RmsServerName, text);
				logger.LogEvent(MessagingPoliciesEventLogConstants.Tuple_Rms403Failure, text2, new object[]
				{
					localizedString,
					param.EffectText,
					param.Exception
				});
				return;
			}
			case RightsManagementFailureCode.HttpNotFoundFailure:
			{
				LocalizedString localizedString = RMSvcAgentStrings.RmsErrorTextFor404(param.TenantName, param.RmsComponentName, param.RmsObjectName, param.RmsServerName, text);
				logger.LogEvent(MessagingPoliciesEventLogConstants.Tuple_Rms404Failure, text2, new object[]
				{
					localizedString,
					param.EffectText,
					param.Exception
				});
				return;
			}
			default:
			{
				LocalizedString localizedString = RMSvcAgentStrings.RmsErrorTextForSpecialException(param.TenantName, param.RmsComponentName, param.RmsObjectName, param.RmsServerName, string.Format("WebException ({0})", text));
				logger.LogEvent(MessagingPoliciesEventLogConstants.Tuple_RmsSpecialFailure, text2, new object[]
				{
					localizedString,
					param.EffectText,
					param.Exception
				});
				return;
			}
			}
		}

		private static void LogNoRightSoapException(ExEventLog logger, RmsEventLogHandler.EventParam param)
		{
			string periodicKey = param.IsEnterprise ? null : param.TenantId.ToString();
			LocalizedString localizedString = RMSvcAgentStrings.RmsErrorTextForNoRightException(param.TenantName, param.RmsComponentName, param.RmsObjectName, param.RmsServerName);
			logger.LogEvent(MessagingPoliciesEventLogConstants.Tuple_RmsNoRightFailure, periodicKey, new object[]
			{
				localizedString,
				param.EffectText,
				param.Exception
			});
		}

		private static void LogSpecialException(ExEventLog logger, RmsEventLogHandler.EventParam param)
		{
			string name = param.Exception.GetType().Name;
			string periodicKey = param.IsEnterprise ? name : string.Format("{0}_{1}", param.TenantId, name);
			LocalizedString localizedString = RMSvcAgentStrings.RmsErrorTextForSpecialException(param.TenantName, param.RmsComponentName, param.RmsObjectName, param.RmsServerName, name);
			logger.LogEvent(MessagingPoliciesEventLogConstants.Tuple_RmsSpecialFailure, periodicKey, new object[]
			{
				localizedString,
				param.EffectText,
				param.Exception
			});
		}

		private static void LogGeneralException(ExEventLog logger, RmsEventLogHandler.EventParam param)
		{
			string name = param.Exception.GetType().Name;
			string periodicKey = param.IsEnterprise ? name : string.Format("{0}_{1}", param.TenantId, name);
			LocalizedString localizedString = RMSvcAgentStrings.RmsErrorTextForGeneralException(param.TenantName, param.RmsComponentName, name);
			logger.LogEvent(MessagingPoliciesEventLogConstants.Tuple_RmsGeneralFailure, periodicKey, new object[]
			{
				localizedString,
				param.EffectText,
				param.Exception
			});
		}

		internal struct EventParam
		{
			public EventParam(MailItem mailItem, RmsComponent component, Exception exception, bool permanentError)
			{
				this.RmsComponent = component;
				this.Exception = exception;
				this.TenantId = mailItem.TenantId;
				OrganizationId a = Utils.OrgIdFromMailItem(mailItem);
				this.IsEnterprise = (a == OrganizationId.ForestWideOrgId);
				this.RmsComponentName = RmsEventLogHandler.EventParam.GetRmsComponentName(component);
				this.RmsServerName = RmsEventLogHandler.EventParam.GetRmsServerName(exception);
				this.TenantName = (this.IsEnterprise ? null : string.Format("(Tenant Id: {0}) ", this.TenantId));
				this.RmsObjectName = RmsEventLogHandler.EventParam.GetRmsObjectName(exception);
				this.EffectText = RmsEventLogHandler.EventParam.GetRmsErrorEffectText(component, permanentError, mailItem, this.RmsObjectName);
			}

			private static string GetRmsComponentName(RmsComponent component)
			{
				string result = null;
				switch (component)
				{
				case RmsComponent.EncryptionAgent:
					result = "Encryption Agent";
					break;
				case RmsComponent.PrelicensingAgent:
					result = "Prelicensing Agent";
					break;
				case RmsComponent.JournalReportDecryptionAgent:
					result = "Journal Report Decryption Agent";
					break;
				}
				return result;
			}

			private static string GetRmsServerName(Exception exception)
			{
				RightsManagementException ex = exception as RightsManagementException;
				string result;
				if (ex != null && !string.IsNullOrEmpty(ex.RmsUrl))
				{
					result = RMSvcAgentStrings.RmsAdNameWithUrl(ex.RmsUrl);
				}
				else
				{
					result = RMSvcAgentStrings.RmsAdName;
				}
				return result;
			}

			private static LocalizedString GetRmsErrorEffectText(RmsComponent component, bool isPermanentFailure, MailItem mailItem, string rmsObjectName)
			{
				string messageId = mailItem.Message.MessageId;
				switch (component)
				{
				case RmsComponent.EncryptionAgent:
					return isPermanentFailure ? RMSvcAgentStrings.RmsErrorTextForNDR(messageId) : RMSvcAgentStrings.RmsErrorTextForDefer(messageId);
				case RmsComponent.PrelicensingAgent:
					if (!isPermanentFailure)
					{
						return RMSvcAgentStrings.RmsErrorTextForDefer(messageId);
					}
					if (string.Equals(rmsObjectName, "License", StringComparison.OrdinalIgnoreCase))
					{
						return RMSvcAgentStrings.RmsErrorTextForNoServerPL(messageId);
					}
					return RMSvcAgentStrings.RmsErrorTextForNoPL(messageId);
				case RmsComponent.JournalReportDecryptionAgent:
					return isPermanentFailure ? RMSvcAgentStrings.RmsErrorTextForNoJR(messageId) : RMSvcAgentStrings.RmsErrorTextForDeferJR(messageId);
				}
				return LocalizedString.Empty;
			}

			private static string GetRmsObjectName(Exception exception)
			{
				string result = null;
				RightsManagementException ex = exception as RightsManagementException;
				if (ex != null)
				{
					switch (ex.FailureCode)
					{
					case RightsManagementFailureCode.PreLicenseAcquisitionFailed:
						result = "PreLicense";
						break;
					case RightsManagementFailureCode.ClcAcquisitionFailed:
						result = "CLC";
						break;
					case RightsManagementFailureCode.RacAcquisitionFailed:
						result = "GIC";
						break;
					case RightsManagementFailureCode.TemplateAcquisitionFailed:
						result = "Templates";
						break;
					case RightsManagementFailureCode.UseLicenseAcquisitionFailed:
						result = "License";
						break;
					case RightsManagementFailureCode.FindServiceLocationFailed:
						result = "FindServiceLocation";
						break;
					}
				}
				return result;
			}

			private const string RmsEntityRAC = "GIC";

			private const string RmsEntityCLC = "CLC";

			private const string RmsEntityLicense = "License";

			private const string RmsEntityPreLicense = "PreLicense";

			private const string RmsEntityTemplates = "Templates";

			private const string RmsEntityFSL = "FindServiceLocation";

			public bool IsEnterprise;

			public Guid TenantId;

			public RmsComponent RmsComponent;

			public string RmsComponentName;

			public string RmsServerName;

			public string RmsObjectName;

			public string TenantName;

			public LocalizedString EffectText;

			public Exception Exception;
		}
	}
}
