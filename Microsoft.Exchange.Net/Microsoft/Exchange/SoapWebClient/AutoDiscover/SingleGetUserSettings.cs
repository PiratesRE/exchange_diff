using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.SoapWebClient.AutoDiscover
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SingleGetUserSettings
	{
		public bool UseWSSecurityUrl { get; set; }

		public SingleGetUserSettings(params string[] requestedSettings)
		{
			this.requestedSettings = requestedSettings;
		}

		public UserSettings Discover(AutodiscoverClient client, string user)
		{
			return this.Discover(client, user, null);
		}

		public UserSettings Discover(AutodiscoverClient client, string user, Uri url)
		{
			InvokeDelegate invokeDelegate = this.GetInvokeDelegate(user);
			SingleGetUserSettings.DiscoveryResultData discoveryResultData = null;
			for (int i = 0; i < 3; i++)
			{
				if (url != null)
				{
					SingleGetUserSettings.Tracer.TraceDebug<SingleGetUserSettings, string, Uri>((long)this.GetHashCode(), "{0}: Discover user {1} at {2}", this, user, url);
					discoveryResultData = this.HandleResult(client.InvokeWithEndpoint(invokeDelegate, url));
					url = null;
				}
				else
				{
					SingleGetUserSettings.Tracer.TraceDebug<SingleGetUserSettings, string>((long)this.GetHashCode(), "{0}: Discover user {1}", this, user);
					discoveryResultData = this.HandleResults(client.InvokeWithDiscovery(invokeDelegate, SingleGetUserSettings.GetUserDomain(user)));
				}
				if (discoveryResultData.Result == SingleGetUserSettings.DiscoveryResult.UrlRedirect)
				{
					SingleGetUserSettings.Tracer.TraceDebug<SingleGetUserSettings, Uri>((long)this.GetHashCode(), "{0}: Following URL redirection to {1}", this, discoveryResultData.RedirectUrl);
					if (this.UseWSSecurityUrl)
					{
						url = EwsWsSecurityUrl.Fix(discoveryResultData.RedirectUrl);
					}
					else
					{
						url = discoveryResultData.RedirectUrl;
					}
				}
				else
				{
					if (discoveryResultData.Result != SingleGetUserSettings.DiscoveryResult.AddressRedirect)
					{
						break;
					}
					SingleGetUserSettings.Tracer.TraceDebug<SingleGetUserSettings, string>((long)this.GetHashCode(), "{0}: Following UserAddress redirection to {1}", this, discoveryResultData.RedirectUserAddress);
					user = discoveryResultData.RedirectUserAddress;
					invokeDelegate = this.GetInvokeDelegate(user);
				}
			}
			if (discoveryResultData.Result == SingleGetUserSettings.DiscoveryResult.Success)
			{
				UserSettings userSettings = new UserSettings(discoveryResultData.UserResponse);
				SingleGetUserSettings.Tracer.TraceDebug<SingleGetUserSettings, UserSettings>((long)this.GetHashCode(), "{0}: Received: {1}", this, userSettings);
				return userSettings;
			}
			if (discoveryResultData.Exception != null)
			{
				throw discoveryResultData.Exception;
			}
			throw new GetUserSettingsException(NetException.GetUserSettingsGeneralFailure);
		}

		private static string GetUserDomain(string user)
		{
			int num = user.IndexOf("@");
			if (num != -1)
			{
				string text = user.Substring(num + 1).Trim();
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
			}
			throw new GetUserSettingsException(NetException.InvalidUserForGetUserSettings(user));
		}

		private InvokeDelegate GetInvokeDelegate(string user)
		{
			GetUserSettingsRequest request = new GetUserSettingsRequest
			{
				RequestedSettings = this.requestedSettings,
				Users = new User[]
				{
					new User
					{
						Mailbox = user
					}
				}
			};
			return delegate(DefaultBinding_Autodiscover binding)
			{
				if (this.UseWSSecurityUrl)
				{
					binding.Url = EwsWsSecurityUrl.Fix(binding.Url);
				}
				return binding.GetUserSettings(request);
			};
		}

		private SingleGetUserSettings.DiscoveryResultData HandleResults(IEnumerable<AutodiscoverResultData> results)
		{
			SingleGetUserSettings.DiscoveryResultData discoveryResultData = null;
			foreach (AutodiscoverResultData result in results)
			{
				SingleGetUserSettings.DiscoveryResultData discoveryResultData2 = this.HandleResult(result);
				if (discoveryResultData2.Result != SingleGetUserSettings.DiscoveryResult.Failure)
				{
					return discoveryResultData2;
				}
				if (discoveryResultData == null)
				{
					discoveryResultData = discoveryResultData2;
				}
			}
			return discoveryResultData;
		}

		private SingleGetUserSettings.DiscoveryResultData HandleResult(AutodiscoverResultData result)
		{
			switch (result.Type)
			{
			case AutodiscoverResult.Failure:
				return new SingleGetUserSettings.DiscoveryResultData
				{
					Result = SingleGetUserSettings.DiscoveryResult.Failure,
					Exception = new GetUserSettingsException(NetException.GetUserSettingsGeneralFailure, result.Exception)
				};
			case AutodiscoverResult.UnsecuredRedirect:
			case AutodiscoverResult.InvalidSslHostname:
				return new SingleGetUserSettings.DiscoveryResultData
				{
					Result = SingleGetUserSettings.DiscoveryResult.Failure,
					Exception = new GetUserSettingsException(NetException.CannotHandleUnsecuredRedirection)
				};
			}
			GetUserSettingsResponse getUserSettingsResponse = result.Response as GetUserSettingsResponse;
			if (getUserSettingsResponse == null)
			{
				SingleGetUserSettings.Tracer.TraceError<SingleGetUserSettings, string>((long)this.GetHashCode(), "{0}: Unexpected response type {1}", this, result.Response.GetType().Name);
				return new SingleGetUserSettings.DiscoveryResultData
				{
					Result = SingleGetUserSettings.DiscoveryResult.Failure,
					Exception = new GetUserSettingsException(NetException.UnexpectedAutodiscoverResult(result.Response.GetType().Name))
				};
			}
			if (getUserSettingsResponse.UserResponses == null)
			{
				SingleGetUserSettings.Tracer.TraceError<SingleGetUserSettings>((long)this.GetHashCode(), "{0}: Response with no UserResponses", this);
				return new SingleGetUserSettings.DiscoveryResultData
				{
					Result = SingleGetUserSettings.DiscoveryResult.Failure,
					Exception = new GetUserSettingsException(NetException.UnexpectedUserResponses)
				};
			}
			if (getUserSettingsResponse.UserResponses.Length != 1)
			{
				SingleGetUserSettings.Tracer.TraceError<SingleGetUserSettings, int>((long)this.GetHashCode(), "{0}: Response with unexpected number of UserResponses: {1}", this, getUserSettingsResponse.UserResponses.Length);
				return new SingleGetUserSettings.DiscoveryResultData
				{
					Result = SingleGetUserSettings.DiscoveryResult.Failure,
					Exception = new GetUserSettingsException(NetException.UnexpectedUserResponses)
				};
			}
			UserResponse userResponse = getUserSettingsResponse.UserResponses[0];
			if (userResponse == null)
			{
				SingleGetUserSettings.Tracer.TraceError<SingleGetUserSettings>((long)this.GetHashCode(), "{0}: No response", this);
				return new SingleGetUserSettings.DiscoveryResultData
				{
					Result = SingleGetUserSettings.DiscoveryResult.Failure,
					Exception = new GetUserSettingsException(NetException.UnexpectedUserResponses)
				};
			}
			if (userResponse.ErrorCodeSpecified && userResponse.ErrorCode != ErrorCode.NoError)
			{
				return this.HandleErrorResponse(userResponse);
			}
			return new SingleGetUserSettings.DiscoveryResultData
			{
				Result = SingleGetUserSettings.DiscoveryResult.Success,
				UserResponse = userResponse
			};
		}

		private SingleGetUserSettings.DiscoveryResultData HandleErrorResponse(UserResponse userResponse)
		{
			if (userResponse.ErrorCode == ErrorCode.RedirectUrl)
			{
				if (!string.IsNullOrEmpty(userResponse.RedirectTarget))
				{
					if (Uri.IsWellFormedUriString(userResponse.RedirectTarget, UriKind.Absolute))
					{
						Uri uri = new Uri(userResponse.RedirectTarget);
						if (uri.Scheme == Uri.UriSchemeHttps)
						{
							SingleGetUserSettings.Tracer.TraceDebug<SingleGetUserSettings, string>((long)this.GetHashCode(), "{0}: Response has RedirectTarget: {1}", this, userResponse.RedirectTarget);
							return new SingleGetUserSettings.DiscoveryResultData
							{
								Result = SingleGetUserSettings.DiscoveryResult.UrlRedirect,
								RedirectUrl = new Uri(userResponse.RedirectTarget)
							};
						}
						SingleGetUserSettings.Tracer.TraceError<SingleGetUserSettings, string>((long)this.GetHashCode(), "{0}: Response has non-HTTPS RedirectTarget: {1}", this, userResponse.RedirectTarget);
					}
					else
					{
						SingleGetUserSettings.Tracer.TraceError<SingleGetUserSettings, string>((long)this.GetHashCode(), "{0}: Response has bad RedirectTarget: {1}", this, userResponse.RedirectTarget);
					}
				}
				else
				{
					SingleGetUserSettings.Tracer.TraceError<SingleGetUserSettings>((long)this.GetHashCode(), "{0}: Response missing RedirectTarget", this);
				}
			}
			if (userResponse.ErrorCode == ErrorCode.RedirectAddress)
			{
				if (!string.IsNullOrEmpty(userResponse.RedirectTarget))
				{
					return new SingleGetUserSettings.DiscoveryResultData
					{
						Result = SingleGetUserSettings.DiscoveryResult.AddressRedirect,
						RedirectUserAddress = userResponse.RedirectTarget
					};
				}
				SingleGetUserSettings.Tracer.TraceError<SingleGetUserSettings>((long)this.GetHashCode(), "{0}: Response missing RedirectTarget", this);
			}
			SingleGetUserSettings.Tracer.TraceError<SingleGetUserSettings, ErrorCode>((long)this.GetHashCode(), "{0}: Response has error: {1}", this, userResponse.ErrorCode);
			return new SingleGetUserSettings.DiscoveryResultData
			{
				Result = SingleGetUserSettings.DiscoveryResult.Failure,
				Exception = new GetUserSettingsException(NetException.UnexpectedUserResponses)
			};
		}

		private const int MaximumRedirections = 3;

		private static readonly Trace Tracer = ExTraceGlobals.EwsClientTracer;

		private string[] requestedSettings;

		private enum DiscoveryResult
		{
			Success,
			Failure,
			UrlRedirect,
			AddressRedirect
		}

		private sealed class DiscoveryResultData
		{
			public SingleGetUserSettings.DiscoveryResult Result { get; set; }

			public LocalizedException Exception { get; set; }

			public string RedirectUserAddress { get; set; }

			public Uri RedirectUrl { get; set; }

			public UserResponse UserResponse { get; set; }

			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder(400);
				stringBuilder.Append("Result=" + this.Result + ";");
				if (this.Exception != null)
				{
					stringBuilder.Append("Exception=" + this.Exception.Message + ";");
				}
				if (this.RedirectUserAddress != null)
				{
					stringBuilder.Append("UserAddress=" + this.RedirectUserAddress + ";");
				}
				if (this.RedirectUrl != null)
				{
					stringBuilder.Append("RedirectUrl=" + this.RedirectUrl.ToString() + ";");
				}
				return stringBuilder.ToString();
			}
		}
	}
}
