using System;
using Microsoft.Exchange.SoapWebClient.AutoDiscover;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class DomainSoapAutoDiscoverRequest : SoapAutoDiscoverRequest
	{
		internal DomainSoapAutoDiscoverRequest(Application application, ClientContext clientContext, RequestLogger requestLogger, AutoDiscoverAuthenticator authenticator, Uri targetUri, EmailAddress[] emailAddresses, AutodiscoverType autodiscoverType) : base(application, clientContext, requestLogger, "DomainSoapAutoDiscoverRequest", authenticator, targetUri, emailAddresses, autodiscoverType)
		{
		}

		public override string ToString()
		{
			return "DomainSoapAutoDiscoverRequest to " + base.TargetUri.ToString();
		}

		protected override IAsyncResult BeginGetSettings(AsyncCallback callback)
		{
			string[] array = new string[base.EmailAddresses.Length];
			for (int i = 0; i < base.EmailAddresses.Length; i++)
			{
				array[i] = base.EmailAddresses[i].Domain;
			}
			GetDomainSettingsRequest request = new GetDomainSettingsRequest
			{
				Domains = array,
				RequestedSettings = DomainSoapAutoDiscoverRequest.RequestedSettings,
				RequestedVersion = base.Application.GetRequestedVersionForAutoDiscover(base.AutodiscoverType)
			};
			return this.client.BeginGetDomainSettings(request, callback, null);
		}

		protected override AutodiscoverResponse EndGetSettings(IAsyncResult asyncResult)
		{
			return this.client.EndGetDomainSettings(asyncResult);
		}

		protected override void HandleResponse(AutodiscoverResponse autodiscoverResponse)
		{
			GetDomainSettingsResponse getDomainSettingsResponse = (GetDomainSettingsResponse)autodiscoverResponse;
			if (getDomainSettingsResponse.DomainResponses == null || getDomainSettingsResponse.DomainResponses.Length != base.EmailAddresses.Length)
			{
				SoapAutoDiscoverRequest.AutoDiscoverTracer.TraceError<DomainSoapAutoDiscoverRequest>((long)this.GetHashCode(), "{0}: Response with no DomainResponses or unexpected number of DomainResponses", this);
				this.HandleException(new AutoDiscoverFailedException(Strings.descSoapAutoDiscoverInvalidResponseError(this.client.Url), 63804U));
				return;
			}
			SoapAutoDiscoverRequest.AutoDiscoverTracer.TraceDebug<DomainSoapAutoDiscoverRequest>((long)this.GetHashCode(), "{0}: Received valid response.", this);
			AutoDiscoverRequestResult[] array = new AutoDiscoverRequestResult[base.EmailAddresses.Length];
			for (int i = 0; i < base.EmailAddresses.Length; i++)
			{
				array[i] = this.GetAutodiscoverResultFromDomainResponse(base.EmailAddresses[i], getDomainSettingsResponse.DomainResponses[i]);
			}
			base.HandleResult(array);
		}

		private AutoDiscoverRequestResult GetAutodiscoverResultFromDomainResponse(EmailAddress emailAddress, DomainResponse domainResponse)
		{
			if (domainResponse != null)
			{
				if (domainResponse.DomainSettingErrors != null)
				{
					foreach (DomainSettingError domainSettingError in domainResponse.DomainSettingErrors)
					{
						if (domainSettingError != null && domainSettingError.ErrorCode != ErrorCode.NoError)
						{
							SoapAutoDiscoverRequest.AutoDiscoverTracer.TraceError((long)this.GetHashCode(), "{0}: Response for domain {1} has DomainSettings error: {2}:{3}:{4}", new object[]
							{
								this,
								emailAddress.Domain,
								domainSettingError.SettingName,
								domainSettingError.ErrorCode,
								domainSettingError.ErrorMessage
							});
							return new AutoDiscoverRequestResult(base.TargetUri, new AutoDiscoverFailedException(Strings.descSoapAutoDiscoverRequestUserSettingError(base.TargetUri.ToString(), domainSettingError.SettingName, domainSettingError.ErrorMessage), 39228U), null, null);
						}
					}
				}
				DomainStringSetting domainStringSetting = null;
				DomainStringSetting domainStringSetting2 = null;
				foreach (DomainSetting domainSetting in domainResponse.DomainSettings)
				{
					if (StringComparer.InvariantCulture.Equals(domainSetting.Name, "ExternalEwsUrl"))
					{
						domainStringSetting = (domainSetting as DomainStringSetting);
					}
					if (StringComparer.InvariantCulture.Equals(domainSetting.Name, "ExternalEwsVersion"))
					{
						domainStringSetting2 = (domainSetting as DomainStringSetting);
					}
					if (domainStringSetting != null && domainStringSetting2 != null)
					{
						break;
					}
				}
				if (domainStringSetting != null && !string.IsNullOrEmpty(domainStringSetting.Value))
				{
					string versionValue = (domainStringSetting2 != null) ? domainStringSetting2.Value : null;
					AutoDiscoverRequestResult autodiscoverResult = base.GetAutodiscoverResult(domainStringSetting.Value, versionValue, emailAddress);
					if (autodiscoverResult != null)
					{
						return autodiscoverResult;
					}
				}
				else
				{
					SoapAutoDiscoverRequest.AutoDiscoverTracer.TraceError<DomainSoapAutoDiscoverRequest, string>((long)this.GetHashCode(), "{0}: ExternalEwsUrl setting domain {1} has missing value", this, emailAddress.Domain);
				}
				SoapAutoDiscoverRequest.AutoDiscoverTracer.TraceError<DomainSoapAutoDiscoverRequest, string>((long)this.GetHashCode(), "{0}: Unable to find ExternalEwsUrl setting domain {1} in response", this, emailAddress.Domain);
			}
			else
			{
				SoapAutoDiscoverRequest.AutoDiscoverTracer.TraceError<DomainSoapAutoDiscoverRequest, string>((long)this.GetHashCode(), "{0}: Response for domain {1} is empty.", this, emailAddress.Domain);
			}
			return new AutoDiscoverRequestResult(base.TargetUri, new AutoDiscoverFailedException(Strings.descSoapAutoDiscoverRequestUserSettingInvalidError(base.TargetUri.ToString(), "ExternalEwsUrl"), 55612U), null, null);
		}

		private static readonly string[] RequestedSettings = new string[]
		{
			"ExternalEwsUrl",
			"ExternalEwsVersion"
		};
	}
}
