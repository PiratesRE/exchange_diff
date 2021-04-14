using System;
using System.Diagnostics;
using System.Web;
using Microsoft.Exchange.Autodiscover;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.AutodiscoverV2;

namespace Microsoft.Exchange.AutoDiscoverV2
{
	public class AutoDiscoverV2
	{
		internal AutoDiscoverV2(RequestDetailsLogger logger, IFlightSettingRepository flightSettings = null, ITenantRepository tenantRepository = null)
		{
			this.logger = logger;
			this.flightSettings = flightSettings;
			this.tenantRepository = tenantRepository;
		}

		internal static TResult TrackLatency<TResult>(IFlightSettingRepository flightSettingRepository, RequestDetailsLogger logger, Func<TResult> method, string keyToLog)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			TResult result;
			try
			{
				result = method();
			}
			finally
			{
				stopwatch.Stop();
				logger.AppendGenericInfo(keyToLog, stopwatch.ElapsedMilliseconds);
				ExTraceGlobals.LatencyTracer.TraceDebug<string>((long)keyToLog.GetHashCode(), string.Format("{0} - {1}", keyToLog, stopwatch.ElapsedMilliseconds), null);
			}
			return result;
		}

		internal AutoDiscoverV2Response ProcessRequest(AutoDiscoverV2Request request, IFlightSettingRepository flightSettings)
		{
			return this.ProcessRequest(request, flightSettings, new TenantRepository(this.logger));
		}

		internal AutoDiscoverV2Response ProcessRequest(AutoDiscoverV2Request request, IFlightSettingRepository flightSettings, ITenantRepository tenantRepository)
		{
			this.flightSettings = flightSettings;
			this.tenantRepository = tenantRepository;
			return this.Execute(request, tenantRepository);
		}

		internal AutoDiscoverV2Request CreateRequestFromContext(HttpContextBase context, string emailAddressFromUrl)
		{
			AutoDiscoverV2Request autoDiscoverV2Request = new AutoDiscoverV2Request();
			string protocol = context.Request.Params["Protocol"];
			uint redirectCount = 0U;
			uint.TryParse(context.Request.Params["RedirectCount"], out redirectCount);
			autoDiscoverV2Request.ValidateRequest(emailAddressFromUrl, protocol, redirectCount, this.logger);
			autoDiscoverV2Request.EmailAddress = SmtpAddress.Parse(emailAddressFromUrl);
			autoDiscoverV2Request.Protocol = protocol;
			autoDiscoverV2Request.HostNameHint = context.Request.Params["HostNameHint"];
			autoDiscoverV2Request.RedirectCount = redirectCount;
			return autoDiscoverV2Request;
		}

		private AutoDiscoverV2Response Execute(AutoDiscoverV2Request request, ITenantRepository tenantRepository)
		{
			this.logger.AppendGenericInfo("IsOnPrem", "true");
			return this.ExecuteOnPremEndFlow(request);
		}

		private AutoDiscoverV2Response GetResourceUrlResponse(string hostName, string protocol)
		{
			string resourceUrl = ResourceUrlBuilder.GetResourceUrl(protocol, hostName);
			this.logger.AppendGenericInfo("GetResourceUrlResponse", resourceUrl);
			return new AutoDiscoverV2Response
			{
				ProtocolName = protocol,
				Url = resourceUrl
			};
		}

		private AutoDiscoverV2Response ExecuteOnPremEndFlow(AutoDiscoverV2Request request)
		{
			this.logger.AppendGenericInfo("ExecuteOnPremEndFlow", "OnPrem");
			ADRecipient onPremUser = this.tenantRepository.GetOnPremUser(request.EmailAddress);
			if (onPremUser == null)
			{
				this.logger.AppendGenericInfo("TryGetEmailRedirectResponse", "UserNotFound");
				IAutodMiniRecipient nextUserFromSortedList = this.tenantRepository.GetNextUserFromSortedList(request.EmailAddress);
				if (nextUserFromSortedList != null)
				{
					this.logger.AppendGenericInfo("TryGetEmailRedirectResponse", "FoundRandomUser");
					AutoDiscoverV2Response result;
					if (this.TryGetEmailRedirectResponse(request, nextUserFromSortedList, out result))
					{
						return result;
					}
				}
				return this.GetResourceUrlResponse(this.flightSettings.GetHostNameFromVdir(null, request.Protocol), request.Protocol);
			}
			AutoDiscoverV2Response result2;
			if (this.TryGetEmailRedirectResponse(request, onPremUser, out result2))
			{
				return result2;
			}
			return this.GetResourceUrlResponse(this.flightSettings.GetHostNameFromVdir(null, request.Protocol), request.Protocol);
		}

		private bool TryGetEmailRedirectResponse(AutoDiscoverV2Request request, IAutodMiniRecipient recipient, out AutoDiscoverV2Response redirectResponse)
		{
			redirectResponse = null;
			if (recipient == null)
			{
				return false;
			}
			this.logger.AppendGenericInfo("TryGetEmailRedirectResponseUserFound", recipient.RecipientType);
			if (recipient.ExternalEmailAddress != null && recipient.ExternalEmailAddress.AddressString != null && recipient.ExternalEmailAddress.PrefixString == "SMTP")
			{
				this.logger.AppendGenericInfo("TryGetEmailRedirectResponse", string.Format("ExternalEmailAddressFound - {0}", recipient.ExternalEmailAddress.AddressString));
				redirectResponse = ResourceUrlBuilder.GetRedirectResponse(this.logger, "outlook.office365.com", recipient.ExternalEmailAddress.AddressString, request.Protocol, request.RedirectCount, null);
				return true;
			}
			return false;
		}

		private bool TryGetEmailRedirectResponse(AutoDiscoverV2Request request, ADRecipient recipient, out AutoDiscoverV2Response redirectResponse)
		{
			redirectResponse = null;
			if (recipient == null)
			{
				return false;
			}
			this.logger.AppendGenericInfo("ADUserFound", recipient.RecipientType);
			if (recipient.ExternalEmailAddress != null && recipient.ExternalEmailAddress.AddressString != null && recipient.ExternalEmailAddress.PrefixString == "SMTP")
			{
				this.logger.AppendGenericInfo("TryGetEmailRedirectResponse", string.Format("ExternalEmailAddressFound - {0}", recipient.ExternalEmailAddress.AddressString + " " + request.EmailAddress.Address));
				redirectResponse = ResourceUrlBuilder.GetRedirectResponse(this.logger, "outlook.office365.com", recipient.ExternalEmailAddress.AddressString, request.Protocol, request.RedirectCount, null);
				return true;
			}
			return false;
		}

		private readonly RequestDetailsLogger logger;

		private IFlightSettingRepository flightSettings;

		private ITenantRepository tenantRepository;
	}
}
