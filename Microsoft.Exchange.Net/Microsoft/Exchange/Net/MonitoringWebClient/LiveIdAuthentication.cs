using System;
using System.Collections.Generic;
using System.Net;
using System.Security;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class LiveIdAuthentication : BaseTestStep
	{
		public Uri Uri { get; private set; }

		public string UserName { get; private set; }

		public string UserDomain { get; private set; }

		public SecureString Password { get; private set; }

		public ITestFactory TestFactory { get; private set; }

		private string UserNameNoDomain { get; set; }

		public AuthenticationParameters AuthenticationParameters { get; private set; }

		protected override TestId Id
		{
			get
			{
				return TestId.LiveIdAuthentication;
			}
		}

		public override object Result
		{
			get
			{
				return this.ticketPostUri;
			}
		}

		public LiveIdAuthentication(Uri uri, string userName, string userDomain, SecureString password, AuthenticationParameters authenticationParameters, ITestFactory factory)
		{
			this.Uri = uri;
			this.UserName = userName;
			this.UserDomain = userDomain;
			this.Password = password;
			this.AuthenticationParameters = authenticationParameters;
			this.TestFactory = factory;
		}

		protected override void StartTest()
		{
			string[] array = this.UserName.Split(new char[]
			{
				'@'
			});
			if (array.Length != 2)
			{
				throw new ArgumentException("Invalid user name: " + this.UserName);
			}
			this.UserNameNoDomain = array[0];
			this.UserDomain = array[1];
			this.BeginGetLiveIdLogonPage();
		}

		private void BeginGetLiveIdLogonPage()
		{
			Uri targetUri = this.GetTargetUri(this.UserDomain);
			this.session.BeginGetFollowingRedirections(this.Id, targetUri.ToString(), delegate(IAsyncResult result)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.LiveIdLogonPageReceived), result);
			}, null);
		}

		private Uri GetTargetUri(string domainName)
		{
			bool flag = this.Uri.ToString().IndexOf("sdfpilot.outlook.com", StringComparison.OrdinalIgnoreCase) >= 0 || this.Uri.ToString().IndexOf("outlook.office365.com", StringComparison.OrdinalIgnoreCase) >= 0;
			if (this.AuthenticationParameters != null && !this.AuthenticationParameters.ShouldUseTenantHintOnLiveIdLogon && !flag)
			{
				return this.Uri;
			}
			if (this.Uri.Segments.Length > 1 && this.Uri.Segments[1].TrimEnd(new char[]
			{
				'/'
			}).Equals("ecp", StringComparison.OrdinalIgnoreCase))
			{
				return new Uri(this.Uri, string.Format("?realm={0}", domainName));
			}
			Uri uri = new Uri(this.Uri, domainName);
			if (flag && uri.ToString().IndexOf("/owa/", StringComparison.OrdinalIgnoreCase) < 0)
			{
				return new Uri(this.Uri, string.Format("owa/{0}", domainName));
			}
			return uri;
		}

		private void LiveIdLogonPageReceived(IAsyncResult result)
		{
			LiveIdLogonPage liveIdLogonPage = this.session.EndGetFollowingRedirections<LiveIdLogonPage>(result, delegate(HttpWebResponseWrapper response)
			{
				AdfsLogonPage result2;
				if (AdfsLogonPage.TryParse(response, out result2))
				{
					return result2;
				}
				LiveIdLogonPage result3;
				Exception ex;
				if (LiveIdLogonPage.TryParse(response, out result3, out ex))
				{
					return result3;
				}
				if (ex != null && this.retryCount >= 1)
				{
					throw ex;
				}
				return null;
			});
			if (liveIdLogonPage == null)
			{
				this.retryCount++;
				this.BeginGetLiveIdLogonPage();
				return;
			}
			if (liveIdLogonPage is AdfsLogonPage)
			{
				ITestStep testStep = this.TestFactory.CreateAdfsAuthenticateStep(liveIdLogonPage.PostUri, this.UserName, this.UserDomain, this.Password, liveIdLogonPage as AdfsLogonPage);
				testStep.BeginExecute(this.session, delegate(IAsyncResult tempResult)
				{
					base.AsyncCallbackWrapper(new AsyncCallback(this.AdfsAuthenticationStepFinished), tempResult);
				}, testStep);
				return;
			}
			this.PostCredentials(liveIdLogonPage, new int?(1));
		}

		private void AdfsAuthenticationStepFinished(IAsyncResult result)
		{
			ITestStep testStep = result.AsyncState as ITestStep;
			testStep.EndExecute(result);
			LiveIdBasePage ticketPostPage = (LiveIdBasePage)testStep.Result;
			this.PostTicket(ticketPostPage);
		}

		private void PostCredentials(LiveIdLogonPage liveIdLogonPage, int? credentialPostCount)
		{
			RequestBody body = RequestBody.Format("login={0}&passwd={1}&{2}&user_id={3}&password={4}", new object[]
			{
				this.UserName,
				this.Password,
				liveIdLogonPage.HiddenFieldsString,
				this.UserName,
				this.Password
			});
			Cookie cookie = new Cookie("CkTst", "G" + ExDateTime.Now.UtcTicks, "/", liveIdLogonPage.PostUri.Host);
			this.session.CookieContainer.Add(cookie);
			this.session.BeginPostFollowingRedirections(this.Id, liveIdLogonPage.PostUrl, body, "application/x-www-form-urlencoded", null, RedirectionOptions.FollowUntilNo302ExpectCrossDomainOnFirstRedirect, delegate(IAsyncResult resultTemp)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.LiveIdCredentialPostResponseReceived), resultTemp);
			}, new Dictionary<string, object>
			{
				{
					"CredentialPostCount",
					credentialPostCount
				},
				{
					"LiveIdPage",
					liveIdLogonPage
				}
			});
		}

		private void LiveIdCredentialPostResponseReceived(IAsyncResult result)
		{
			Dictionary<string, object> dictionary = result.AsyncState as Dictionary<string, object>;
			int? credentialPostCount = dictionary["CredentialPostCount"] as int?;
			LiveIdLogonPage liveIdLogonPage = dictionary["LiveIdPage"] as LiveIdLogonPage;
			LiveIdBasePage liveIdBasePage = this.session.EndPostFollowingRedirections<LiveIdBasePage>(result, delegate(HttpWebResponseWrapper response)
			{
				if (credentialPostCount < 5 && response.StatusCode == HttpStatusCode.Found)
				{
					return null;
				}
				LiveIdSamlTokenPage result2;
				if (LiveIdSamlTokenPage.TryParse(response, out result2))
				{
					return result2;
				}
				return LiveIdCompactTokenPage.Parse(response);
			});
			if (liveIdBasePage == null)
			{
				this.PostCredentials(liveIdLogonPage, credentialPostCount + 1);
				return;
			}
			this.PostTicket(liveIdBasePage);
		}

		private void PostTicket(LiveIdBasePage ticketPostPage)
		{
			if (ticketPostPage is LiveIdSamlTokenPage)
			{
				this.session.BeginPost(this.Id, ticketPostPage.PostUrl, RequestBody.Format(ticketPostPage.HiddenFieldsString, new object[0]), "application/x-www-form-urlencoded", delegate(IAsyncResult resultTemp)
				{
					base.AsyncCallbackWrapper(new AsyncCallback(this.LiveIdSamlPostResponseReceived), resultTemp);
				}, null);
				return;
			}
			this.SendCompactTicketRequest(ticketPostPage);
		}

		private void LiveIdSamlPostResponseReceived(IAsyncResult result)
		{
			LiveIdBasePage ticketPostPage = this.session.EndPost<LiveIdCompactTokenPage>(result, (HttpWebResponseWrapper response) => LiveIdCompactTokenPage.Parse(response));
			this.SendCompactTicketRequest(ticketPostPage);
		}

		private void SendCompactTicketRequest(LiveIdBasePage ticketPostPage)
		{
			this.session.BeginPost(this.Id, ticketPostPage.PostUrl, RequestBody.Format(ticketPostPage.HiddenFieldsString, new object[0]), "application/x-www-form-urlencoded", delegate(IAsyncResult resultTemp)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.CompactTicketPostResponseRecived), resultTemp);
			}, null);
		}

		private void CompactTicketPostResponseRecived(IAsyncResult result)
		{
			this.session.EndPost<HttpStatusCode>(result, delegate(HttpWebResponseWrapper response)
			{
				if (response.Headers["Set-Cookie"] == null || response.Headers["Set-Cookie"].IndexOf("RPSAuth", StringComparison.OrdinalIgnoreCase) < 0)
				{
					throw new MissingKeywordException(MonitoringWebClientStrings.MissingLiveIdAuthCookies, response.Request, response, "RPSAuth cookie");
				}
				if (response.StatusCode == HttpStatusCode.Found)
				{
					this.ticketPostUri = new Uri(response.Headers["Location"]);
				}
				else
				{
					this.ticketPostUri = response.Request.RequestUri;
				}
				return response.StatusCode;
			});
			base.ExecutionCompletedSuccessfully();
		}

		private const TestId ID = TestId.LiveIdAuthentication;

		private const int MaxRetryNumber = 1;

		private Uri ticketPostUri;

		private int retryCount;
	}
}
