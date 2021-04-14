using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.EDiscovery.Export.AutoDiscoverProxy;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal sealed class AutoDiscoverClient : BaseServiceClient<DefaultBinding_Autodiscover, IAutoDiscoverClient>, IAutoDiscoverClient
	{
		public AutoDiscoverClient(Uri serviceEndpoint, IServiceCallingContext<DefaultBinding_Autodiscover> serviceCallingContext, CancellationToken abortTokenForTasks) : base(serviceEndpoint, serviceCallingContext, abortTokenForTasks)
		{
			this.AutoDiscoverInternalUrlOnly = false;
		}

		public AutoDiscoverClient(Uri serviceEndpoint, IServiceCallingContext<DefaultBinding_Autodiscover> serviceCallingContext) : base(serviceEndpoint, serviceCallingContext, CancellationToken.None)
		{
			this.AutoDiscoverInternalUrlOnly = false;
		}

		public override IAutoDiscoverClient FunctionalInterface
		{
			get
			{
				return this;
			}
		}

		public bool AutoDiscoverInternalUrlOnly { get; set; }

		public List<AutoDiscoverResult> GetUserEwsEndpoints(IEnumerable<string> mailboxes)
		{
			GetUserSettingsRequest getUserSettingsRequest = new GetUserSettingsRequest();
			getUserSettingsRequest.Users = (from mailbox in mailboxes
			select new User
			{
				Mailbox = mailbox
			}).ToArray<User>();
			getUserSettingsRequest.RequestedSettings = new string[]
			{
				"ExternalEwsUrl",
				"InternalEwsUrl"
			};
			if (this.AutoDiscoverInternalUrlOnly)
			{
				getUserSettingsRequest.RequestedSettings = new string[]
				{
					"InternalEwsUrl"
				};
			}
			List<AutoDiscoverResult> results = new List<AutoDiscoverResult>(getUserSettingsRequest.Users.Length);
			if (this.Connect())
			{
				Tracer.TraceInformation("AutoDiscoverClient.GetUserEwsEndpoints: URL:{0}, number of mailboxes:{1}", new object[]
				{
					base.ServiceBinding.Url,
					getUserSettingsRequest.Users.Length
				});
				base.ServiceBinding.RequestedServerVersionValue = new RequestedServerVersion
				{
					Text = new string[]
					{
						"Exchange2010"
					}
				};
				base.ServiceBinding.Action = new Microsoft.Exchange.EDiscovery.Export.AutoDiscoverProxy.Action
				{
					Text = "http://schemas.microsoft.com/exchange/2010/Autodiscover/Autodiscover/GetUserSettings"
				};
				base.ServiceBinding.To = new To
				{
					Text = base.ServiceBinding.Url
				};
				base.ServiceCallingContext.SetServiceApiContext(base.ServiceBinding, getUserSettingsRequest.Users[0].Mailbox);
				base.InternalCallService<GetUserSettingsResponse>(() => this.ServiceBinding.GetUserSettings(getUserSettingsRequest), delegate(GetUserSettingsResponse response)
				{
					if (response.UserResponses != null)
					{
						UserResponse[] userResponses = response.UserResponses;
						int i = 0;
						while (i < userResponses.Length)
						{
							UserResponse userResponse = userResponses[i];
							switch (userResponse.ErrorCode)
							{
							case ErrorCode.NoError:
							{
								string text = null;
								string text2 = null;
								if (userResponse.UserSettings != null && userResponse.UserSettings.Length > 0)
								{
									foreach (UserSetting userSetting in userResponse.UserSettings)
									{
										StringSetting stringSetting = userSetting as StringSetting;
										if (stringSetting != null)
										{
											if (stringSetting.Name == "ExternalEwsUrl" && Uri.IsWellFormedUriString(stringSetting.Value, UriKind.Absolute))
											{
												text2 = stringSetting.Value;
												break;
											}
											if (stringSetting.Name == "InternalEwsUrl" && Uri.IsWellFormedUriString(stringSetting.Value, UriKind.Absolute))
											{
												text = stringSetting.Value;
											}
										}
									}
								}
								if (!string.IsNullOrEmpty(text2))
								{
									Tracer.TraceInformation("AutoDiscoverClient.GetUserEwsEndpoints: ExternalEwsUrl found: {0}", new object[]
									{
										text2
									});
									results.Add(new AutoDiscoverResult(AutoDiscoverResultCode.Success, text2));
									this.ServiceCallingContext.SetServiceUrlAffinity(this.ServiceBinding, new Uri(text2));
								}
								else if (!string.IsNullOrEmpty(text))
								{
									Tracer.TraceInformation("AutoDiscoverClient.GetUserEwsEndpoints: InternalEwsUrl found: {0}", new object[]
									{
										text
									});
									results.Add(new AutoDiscoverResult(AutoDiscoverResultCode.Success, text));
									this.ServiceCallingContext.SetServiceUrlAffinity(this.ServiceBinding, new Uri(text));
								}
								else
								{
									Tracer.TraceInformation("AutoDiscoverClient.GetUserEwsEndpoints: Ews endpoint not found.", new object[0]);
									results.Add(new AutoDiscoverResult(AutoDiscoverResultCode.UrlConfigurationNotFound, null));
								}
								break;
							}
							case ErrorCode.RedirectAddress:
								Tracer.TraceInformation("AutoDiscoverClient.GetUserEwsEndpoints: EmailAddressRedirected: {0}", new object[]
								{
									userResponse.RedirectTarget
								});
								results.Add(new AutoDiscoverResult(AutoDiscoverResultCode.EmailAddressRedirected, userResponse.RedirectTarget));
								break;
							case ErrorCode.RedirectUrl:
								Tracer.TraceInformation("AutoDiscoverClient.GetUserEwsEndpoints: UrlRedirected: {0}", new object[]
								{
									userResponse.RedirectTarget
								});
								results.Add(new AutoDiscoverResult(AutoDiscoverResultCode.UrlRedirected, userResponse.RedirectTarget));
								break;
							case ErrorCode.InvalidUser:
								Tracer.TraceInformation("AutoDiscoverClient.GetUserEwsEndpoints: InvalidUser: {0}", new object[]
								{
									userResponse.ErrorMessage
								});
								results.Add(new AutoDiscoverResult(AutoDiscoverResultCode.InvalidUser, null));
								break;
							case ErrorCode.InvalidRequest:
							case ErrorCode.InvalidSetting:
							case ErrorCode.SettingIsNotAvailable:
							case ErrorCode.InvalidDomain:
							case ErrorCode.NotFederated:
								goto IL_2B2;
							case ErrorCode.ServerBusy:
							case ErrorCode.InternalServerError:
								Tracer.TraceInformation("AutoDiscoverClient.GetUserEwsEndpoints: TransientError: {0}", new object[]
								{
									userResponse.ErrorMessage
								});
								results.Add(new AutoDiscoverResult(AutoDiscoverResultCode.TransientError, null));
								break;
							default:
								goto IL_2B2;
							}
							IL_2E7:
							i++;
							continue;
							IL_2B2:
							Tracer.TraceInformation("AutoDiscoverClient.GetUserEwsEndpoints: Unknown error: {0}", new object[]
							{
								userResponse.ErrorMessage
							});
							results.Add(new AutoDiscoverResult(AutoDiscoverResultCode.Error, userResponse.ErrorMessage));
							goto IL_2E7;
						}
					}
				}, null, () => base.ServiceCallingContext.AuthorizeServiceBinding(base.ServiceBinding), delegate(Uri redirectedUrl)
				{
					base.ServiceCallingContext.SetServiceUrlAffinity(base.ServiceBinding, redirectedUrl);
					base.ServiceCallingContext.SetServiceUrl(base.ServiceBinding, redirectedUrl);
					base.ServiceBinding.To = new To
					{
						Text = base.ServiceBinding.Url
					};
				});
				return results;
			}
			throw new ExportException(ExportErrorType.ExchangeWebServiceCallFailed, "Unable to connect to auto discover service at: " + base.ServiceEndpoint.ToString());
		}

		private const string ExternalEwsUrl = "ExternalEwsUrl";

		private const string InternalEwsUrl = "InternalEwsUrl";
	}
}
