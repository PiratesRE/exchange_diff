using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Web;
using Microsoft.Exchange.Autodiscover.ConfigurationSettings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Autodiscover;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.OAuth;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[MessageContract]
	public class GetDomainSettingsRequestMessage : AutodiscoverRequestMessage
	{
		internal static Uri GetRandomCasExternalServiceUri<T>(ExchangeVersion? requestedVersion, out int serverVersion) where T : HttpService
		{
			Dictionary<Uri, int> uriMinimumVersions = null;
			serverVersion = 0;
			try
			{
				ServiceTopology currentServiceTopology = ServiceTopology.GetCurrentServiceTopology("f:\\15.00.1497\\sources\\dev\\autodisc\\src\\WCF\\Requests\\GetDomainSettingsRequestMessage.cs", "GetRandomCasExternalServiceUri", 101);
				int minimumVersionNeeded = GetDomainSettingsRequestMessage.GetMinimumServerVersionForExchangeVersion(requestedVersion);
				currentServiceTopology.ForEach<T>(delegate(T service)
				{
					if (service.ServerVersionNumber >= minimumVersionNeeded && service.ClientAccessType == ClientAccessType.External)
					{
						if (uriMinimumVersions == null)
						{
							uriMinimumVersions = new Dictionary<Uri, int>(10);
						}
						int num;
						if (!uriMinimumVersions.TryGetValue(service.Url, out num) || num > service.ServerVersionNumber)
						{
							uriMinimumVersions[service.Url] = service.ServerVersionNumber;
						}
					}
				}, "f:\\15.00.1497\\sources\\dev\\autodisc\\src\\WCF\\Requests\\GetDomainSettingsRequestMessage.cs", "GetRandomCasExternalServiceUri", 105);
			}
			catch (ServiceDiscoveryTransientException arg)
			{
				ExTraceGlobals.FrameworkTracer.TraceError<ServiceDiscoveryTransientException>(0L, "GetRandomCasExternalServiceUri encountered transient exception: {0}", arg);
			}
			catch (ServiceDiscoveryPermanentException arg2)
			{
				ExTraceGlobals.FrameworkTracer.TraceError<ServiceDiscoveryPermanentException>(0L, "GetRandomCasExternalServiceUri encountered permanent exception: {0}", arg2);
			}
			if (uriMinimumVersions == null)
			{
				return null;
			}
			int index = GetDomainSettingsRequestMessage.random.Next(uriMinimumVersions.Count);
			Uri uri = uriMinimumVersions.Keys.ElementAt(index);
			serverVersion = uriMinimumVersions[uri];
			return uri;
		}

		private static int GetMinimumServerVersionForExchangeVersion(ExchangeVersion? requestedVersion)
		{
			if (requestedVersion == null)
			{
				return Server.E14MinVersion;
			}
			switch (requestedVersion.Value)
			{
			case ExchangeVersion.Exchange2010:
				return Server.E14MinVersion;
			case ExchangeVersion.Exchange2010_SP1:
				return 1937866977;
			case ExchangeVersion.Exchange2010_SP2:
				return Server.E14SP1MinVersion;
			case ExchangeVersion.Exchange2012:
			case ExchangeVersion.Exchange2013:
			case ExchangeVersion.Exchange2013_SP1:
				return Server.E15MinVersion;
			default:
				ExTraceGlobals.FrameworkTracer.TraceError<ExchangeVersion>(0L, "Unknown requestedVersion {0}, using E14MinVersion", requestedVersion.Value);
				return Server.E14MinVersion;
			}
		}

		private static void AddSettingToResponse(DomainResponse domainResponse, DomainStringSetting setting, string settingName)
		{
			if (setting != null)
			{
				domainResponse.DomainSettings.Add(setting);
				return;
			}
			DomainSettingError domainSettingError = new DomainSettingError();
			domainSettingError.SettingName = settingName;
			domainSettingError.ErrorCode = ErrorCode.SettingIsNotAvailable;
			domainSettingError.ErrorMessage = string.Format(Strings.SettingIsNotAvailable, settingName);
			domainResponse.DomainSettingErrors.Add(domainSettingError);
		}

		internal override AutodiscoverResponseMessage Execute()
		{
			GetDomainSettingsResponseMessage getDomainSettingsResponseMessage = new GetDomainSettingsResponseMessage();
			GetDomainSettingsResponse response = getDomainSettingsResponseMessage.Response;
			ExchangeVersion? requestedVersion = this.Request.RequestedVersion;
			IPrincipal user = HttpContext.Current.User;
			string errorMessage;
			ExchangeVersion exchangeVersion;
			if (!AutodiscoverRequestMessage.ValidateRequest<string>(this.Request, this.Request.Domains, this.Request.RequestedSettings, null, GetDomainSettingsRequestMessage.maxDomainsPerGetDomainSettingsRequest, Strings.MaxDomainsPerGetDomainSettingsRequestExceeded, out errorMessage, out exchangeVersion))
			{
				response.ErrorCode = ErrorCode.InvalidRequest;
				response.ErrorMessage = errorMessage;
			}
			else
			{
				HashSet<DomainConfigurationSettingName> hashSet;
				DomainSettingErrorCollection settingErrors;
				this.TryParseRequestDomainSettings(out hashSet, out settingErrors);
				if (hashSet.Count == 0)
				{
					response.ErrorCode = ErrorCode.InvalidRequest;
					response.ErrorMessage = Strings.NoSettingsToReturn;
				}
				else
				{
					string userAgent = Common.SafeGetUserAgent(HttpContext.Current.Request);
					GetDomainSettingsCallContext callContext = new GetDomainSettingsCallContext(userAgent, this.Request.RequestedVersion, this.Request.Domains, hashSet, settingErrors, response);
					try
					{
						this.ExecuteGetDomainSettingsCommand(user, callContext);
					}
					catch (OverBudgetException arg)
					{
						ExTraceGlobals.FrameworkTracer.TraceError<OverBudgetException>(0L, "GetDomainSettingsRequestMessage.Execute()returning ServerBusy for exception: {0}.", arg);
						response.ErrorCode = ErrorCode.ServerBusy;
						response.ErrorMessage = Strings.ServerBusy;
						response.DomainResponses.Clear();
					}
					catch (LocalizedException ex)
					{
						ExTraceGlobals.FrameworkTracer.TraceError<LocalizedException>(0L, "GetDomainSettingsRequestMessage.Execute()returning InternalServerError for exception: {0}.", ex);
						Common.EventLog.LogEvent(AutodiscoverEventLogConstants.Tuple_ErrWebException, Common.PeriodicKey, new object[]
						{
							ex.Message,
							ex
						});
						response.ErrorCode = ErrorCode.InternalServerError;
						response.ErrorMessage = Strings.InternalServerError;
						response.DomainResponses.Clear();
					}
				}
			}
			return getDomainSettingsResponseMessage;
		}

		private void CheckIdentity(IPrincipal callingPrincipal)
		{
			IIdentity identity = callingPrincipal.Identity;
			if (identity is WindowsIdentity || identity is WindowsTokenIdentity || identity is ExternalIdentity || identity is OAuthIdentity)
			{
				return;
			}
			throw new InvalidOperationException(string.Format("Unexpected identity type: {0}", identity.GetType()));
		}

		private void ExecuteGetDomainSettingsCommand(IPrincipal callingPrincipal, GetDomainSettingsCallContext callContext)
		{
			this.CheckIdentity(callingPrincipal);
			if (callContext.RequestedSettings.Contains(DomainConfigurationSettingName.ExternalEwsUrl) && callContext.Domains != null && callContext.Domains.Count > 0)
			{
				Uri uri;
				int currentExchangeMajorVersion;
				if (Common.SkipServiceTopologyInDatacenter())
				{
					uri = FrontEndLocator.GetDatacenterFrontEndWebServicesUrl();
					currentExchangeMajorVersion = Server.CurrentExchangeMajorVersion;
				}
				else
				{
					uri = GetDomainSettingsRequestMessage.GetRandomCasExternalServiceUri<WebServicesService>(callContext.RequestedVersion, out currentExchangeMajorVersion);
				}
				DomainStringSetting setting = null;
				DomainStringSetting setting2 = null;
				if (null != uri)
				{
					setting = new DomainStringSetting
					{
						Name = GetDomainSettingsRequestMessage.externalEwsUrlSettingName,
						Value = uri.AbsoluteUri
					};
				}
				bool flag = callContext.RequestedSettings.Contains(DomainConfigurationSettingName.ExternalEwsVersion);
				if (flag)
				{
					ServerVersion serverVersion = new ServerVersion(currentExchangeMajorVersion);
					setting2 = new DomainStringSetting
					{
						Name = GetDomainSettingsRequestMessage.externalEwsVersionSettingName,
						Value = string.Format(CultureInfo.InvariantCulture, "{0:d}.{1:d2}.{2:d4}.{3:d3}", new object[]
						{
							serverVersion.Major,
							serverVersion.Minor,
							serverVersion.Build,
							serverVersion.Revision
						})
					};
				}
				using (IEnumerator<string> enumerator = callContext.Domains.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string text = enumerator.Current;
						DomainResponse domainResponse = new DomainResponse();
						domainResponse.ErrorCode = ErrorCode.NoError;
						domainResponse.ErrorMessage = Strings.NoError;
						domainResponse.DomainSettingErrors = new DomainSettingErrorCollection();
						domainResponse.DomainSettings = new DomainSettingCollection();
						GetDomainSettingsRequestMessage.AddSettingToResponse(domainResponse, setting, GetDomainSettingsRequestMessage.externalEwsUrlSettingName);
						if (flag)
						{
							GetDomainSettingsRequestMessage.AddSettingToResponse(domainResponse, setting2, GetDomainSettingsRequestMessage.externalEwsVersionSettingName);
						}
						callContext.Response.DomainResponses.Add(domainResponse);
					}
					return;
				}
			}
			callContext.Response.ErrorCode = ErrorCode.InvalidRequest;
			callContext.Response.ErrorMessage = Strings.InvalidRequest;
		}

		private bool TryParseRequestDomainSettings(out HashSet<DomainConfigurationSettingName> settingNames, out DomainSettingErrorCollection domainSettingErrors)
		{
			settingNames = new HashSet<DomainConfigurationSettingName>();
			domainSettingErrors = new DomainSettingErrorCollection();
			foreach (string text in this.Request.RequestedSettings)
			{
				try
				{
					DomainConfigurationSettingName item = (DomainConfigurationSettingName)Enum.Parse(typeof(DomainConfigurationSettingName), text);
					settingNames.Add(item);
				}
				catch (ArgumentException)
				{
					DomainSettingError domainSettingError = new DomainSettingError();
					domainSettingError.SettingName = text;
					domainSettingError.ErrorCode = ErrorCode.InvalidSetting;
					domainSettingError.ErrorMessage = string.Format(Strings.InvalidUserSetting, text);
					domainSettingErrors.Add(domainSettingError);
				}
			}
			return domainSettingErrors.Count == 0;
		}

		private const int DefaultMaxDomainsPerRequest = 20;

		private static readonly Random random = new Random();

		private static string externalEwsUrlSettingName = DomainConfigurationSettingName.ExternalEwsUrl.ToString();

		private static string externalEwsVersionSettingName = DomainConfigurationSettingName.ExternalEwsVersion.ToString();

		private static LazyMember<int> maxDomainsPerGetDomainSettingsRequest = new LazyMember<int>(delegate()
		{
			int result;
			if (int.TryParse(ConfigurationManager.AppSettings["MaxDomainsPerGetDomainSettingsRequest"], out result))
			{
				return result;
			}
			return 20;
		});

		[MessageBodyMember(Name = "Request", Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
		public GetDomainSettingsRequest Request;
	}
}
