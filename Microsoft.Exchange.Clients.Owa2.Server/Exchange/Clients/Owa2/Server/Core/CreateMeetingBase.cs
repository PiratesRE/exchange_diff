using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.OnlineMeetings;
using Microsoft.Exchange.Services.OnlineMeetings.Autodiscover;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal abstract class CreateMeetingBase : ServiceCommand<OnlineMeetingType>
	{
		public CreateMeetingBase(CallContext callContext, string sipUri, bool isPrivate) : base(callContext)
		{
			if (string.IsNullOrEmpty(sipUri))
			{
				throw new OwaInvalidRequestException("No sipUri specified");
			}
			this.sipUri = sipUri;
			this.isPrivate = isPrivate;
		}

		protected abstract OnlineMeetingType ProcessOnlineMeetingResult(UserContext userContext, OnlineMeetingResult result);

		protected abstract OnlineMeetingSettings ConstructOnlineMeetingSettings();

		protected abstract void SetDefaultValuesForOptics();

		protected abstract bool ShoudlMeetingBeCreated();

		protected abstract void UpdateOpticsLog(OnlineMeetingResult createMeeting);

		protected override OnlineMeetingType InternalExecute()
		{
			this.SetDefaultValuesForOptics();
			UserContext userContext = UserContextManager.GetUserContext(base.CallContext.HttpContext, base.CallContext.EffectiveCaller, true);
			if (string.Compare(userContext.SipUri, this.sipUri, StringComparison.OrdinalIgnoreCase) != 0)
			{
				base.CallContext.ProtocolLog.Set(CreateOnlineMeetingMetadata.ManagerSipUri, ExtensibleLogger.FormatPIIValue(this.sipUri));
			}
			if (userContext.ExchangePrincipal.MailboxInfo.OrganizationId != null && userContext.ExchangePrincipal.MailboxInfo.OrganizationId.OrganizationalUnit != null)
			{
				base.CallContext.ProtocolLog.Set(CreateOnlineMeetingMetadata.Organization, userContext.ExchangePrincipal.MailboxInfo.OrganizationId.OrganizationalUnit.Name);
			}
			UcwaUserConfiguration ucwaUserConfiguration = UcwaConfigurationUtilities.GetUcwaUserConfiguration(this.sipUri, base.CallContext);
			if (!ucwaUserConfiguration.IsUcwaSupported)
			{
				base.CallContext.ProtocolLog.Set(CreateOnlineMeetingMetadata.IsUcwaSupported, bool.FalseString);
				string errorString = string.Format("[InternalExecute] Attempted to create an online meeting for a non-UCWA supported user; autodiscover returned UcwaUrl: '{0}', DiagnosticInfo: '{1}'", ucwaUserConfiguration.UcwaUrl, ucwaUserConfiguration.DiagnosticInfo);
				this.LogAndTraceError(errorString);
				return OnlineMeetingType.CreateFailedOnlineMeetingType("User is not UCWA enabled");
			}
			if (!this.ShoudlMeetingBeCreated())
			{
				return OnlineMeetingType.CreateFailedOnlineMeetingType("Item already has an online meeting associated to it.");
			}
			OAuthCredentials oauthCredential;
			try
			{
				oauthCredential = UcwaConfigurationUtilities.GetOAuthCredential(this.sipUri);
				oauthCredential.ClientRequestId = new Guid?(Guid.NewGuid());
				base.CallContext.ProtocolLog.Set(CreateOnlineMeetingMetadata.OAuthCorrelationId, oauthCredential.ClientRequestId.Value.ToString());
			}
			catch (OwaException ex)
			{
				this.LogAndTraceError("An error occurred while obtaining OAuth Credential: " + UcwaConfigurationUtilities.BuildFailureLogString(ex));
				return OnlineMeetingType.CreateFailedOnlineMeetingType("An error occurred while obtaining OAuth credential");
			}
			Task<OnlineMeetingResult> task = this.CreateLyncOnlineMeeting(ucwaUserConfiguration.UcwaUrl, oauthCredential, userContext.UserCulture);
			base.CallContext.ProtocolLog.Set(CreateOnlineMeetingMetadata.IsTaskCompleted, task.IsCompleted.ToString());
			if (task.IsFaulted)
			{
				task.Exception.Flatten().Handle(new Func<Exception, bool>(this.HandleCreateOnlineMeetingTaskException));
				return OnlineMeetingType.CreateFailedOnlineMeetingType("An error occured while calling UCWA to create the meeting");
			}
			if (task.Result == null)
			{
				this.LogAndTraceError("Result from call to UCWA is null");
				return OnlineMeetingType.CreateFailedOnlineMeetingType("Unable to create the meeting");
			}
			if (task.Result.OnlineMeeting.WebUrl == "")
			{
				return OnlineMeetingType.CreateFailedOnlineMeetingType("Unable to create the meeting");
			}
			this.UpdateOpticsLog(task.Result);
			OnlineMeetingType result = this.ProcessOnlineMeetingResult(userContext, task.Result);
			this.DiposeObjectsIfNeeded();
			return result;
		}

		protected virtual void DiposeObjectsIfNeeded()
		{
		}

		protected void LogAndTraceError(string errorString)
		{
			ExTraceGlobals.OnlineMeetingTracer.TraceError(0L, "[CreateMeetingBase]" + errorString);
			base.CallContext.ProtocolLog.Set(CreateOnlineMeetingMetadata.Exceptions, errorString);
		}

		protected void LogException(Exception ex, string userSipUri, string description)
		{
			if (!string.IsNullOrEmpty(userSipUri))
			{
				AutodiscoverCache.IncrementFailureCount(userSipUri);
				base.CallContext.ProtocolLog.Set(CreateOnlineMeetingMetadata.CacheOperation, AutodiscoverCacheOperation.IncrementFailureCounter.ToString());
			}
			StringBuilder stringBuilder = new StringBuilder(description);
			if (ex != null)
			{
				if (ex is HttpOperationException)
				{
					HttpOperationException ex2 = ex as HttpOperationException;
					if (ex2.HttpResponse != null)
					{
						base.CallContext.ProtocolLog.Set(CreateOnlineMeetingMetadata.ResponseHeaders, ex2.HttpResponse.GetResponseHeadersAsString());
						base.CallContext.ProtocolLog.Set(CreateOnlineMeetingMetadata.ResponseBody, ex2.HttpResponse.GetResponseBodyAsString());
					}
					stringBuilder.Append(((HttpOperationException)ex).ToLogString());
				}
				else if (ex is AggregateException)
				{
					stringBuilder.Append(((AggregateException)ex).ToLogString());
				}
				else
				{
					stringBuilder.Append(UcwaConfigurationUtilities.BuildFailureLogString(ex));
				}
			}
			this.LogAndTraceError(stringBuilder.ToString());
		}

		protected void LogException(Exception ex, string description)
		{
			this.LogException(ex, string.Empty, description);
		}

		protected bool HandleCreateOnlineMeetingTaskException(Exception ex)
		{
			HttpOperationException ex2 = ex as HttpOperationException;
			if (ex2 != null)
			{
				this.LogException(ex2, "An HttpOperationException occurred while attempting to create online meeting:");
				if (ex2.ErrorInformation == null)
				{
					AutodiscoverCache.IncrementFailureCount(this.sipUri);
					base.CallContext.ProtocolLog.Set(CreateOnlineMeetingMetadata.CacheOperation, AutodiscoverCacheOperation.IncrementFailureCounter.ToString());
				}
				else
				{
					ErrorCode code = ex2.ErrorInformation.Code;
					if (code != ErrorCode.Forbidden)
					{
						if (code != ErrorCode.BadGateway)
						{
							AutodiscoverCache.IncrementFailureCount(this.sipUri);
							base.CallContext.ProtocolLog.Set(CreateOnlineMeetingMetadata.CacheOperation, AutodiscoverCacheOperation.IncrementFailureCounter.ToString());
						}
						else
						{
							AutodiscoverCache.InvalidateUser(this.sipUri);
							base.CallContext.ProtocolLog.Set(CreateOnlineMeetingMetadata.CacheOperation, AutodiscoverCacheOperation.InvalidateUser.ToString());
						}
					}
					else
					{
						AutodiscoverCache.InvalidateDomain(OnlineMeetingHelper.GetSipDomain(this.sipUri));
						base.CallContext.ProtocolLog.Set(CreateOnlineMeetingMetadata.CacheOperation, AutodiscoverCacheOperation.InvalidateDomain.ToString());
					}
				}
				return true;
			}
			if (ex is OperationFailureException)
			{
				this.LogException(ex, this.sipUri, "A network error occured while attempting to create online meeting:");
				return true;
			}
			if (ex is OnlineMeetingSchedulerException)
			{
				this.LogException(ex, this.sipUri, "Unable to create an online meeting:");
				return true;
			}
			this.LogException(ex, this.sipUri, "An unknown exception occurred while creating an online meeting:");
			return false;
		}

		private async Task<OnlineMeetingResult> CreateLyncOnlineMeeting(string ucwaUrl, OAuthCredentials credentials, CultureInfo culture)
		{
			base.CallContext.ProtocolLog.Set(CreateOnlineMeetingMetadata.UcwaUrl, ucwaUrl);
			OnlineMeetingSettings settings = this.ConstructOnlineMeetingSettings();
			OnlineMeetingResult result;
			if (this.isPrivate)
			{
				Uri uri = new Uri(ucwaUrl);
				UcwaNewOnlineMeetingWorker ucwaOnlineMeetingWorker = new UcwaNewOnlineMeetingWorker(uri, credentials, culture);
				result = await ucwaOnlineMeetingWorker.CreatePrivateMeetingAsync(settings);
			}
			else
			{
				UcwaOnlineMeetingScheduler scheduler = new UcwaOnlineMeetingScheduler(ucwaUrl, credentials, culture);
				result = await scheduler.CreateMeetingAsync(settings);
			}
			return result;
		}

		public readonly string sipUri;

		public readonly bool isPrivate;
	}
}
