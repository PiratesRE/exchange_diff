using System;
using System.Collections.Generic;
using System.Net;
using System.Security;
using System.Web;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class AdfsAuthentication : BaseTestStep
	{
		public Uri Uri { get; private set; }

		public string UserName { get; private set; }

		public string UserDomain { get; private set; }

		public SecureString Password { get; private set; }

		public AdfsLogonPage AdfsLogonPage { get; private set; }

		public ITestFactory TestFactory { get; private set; }

		protected override TestId Id
		{
			get
			{
				return TestId.AdfsAuthentication;
			}
		}

		private protected LiveIdBasePage TicketPostPage { protected get; private set; }

		public override object Result
		{
			get
			{
				return this.TicketPostPage;
			}
		}

		public AdfsAuthentication(Uri uri, string userName, string userDomain, SecureString password, AdfsLogonPage adfsLogonPage)
		{
			this.Uri = uri;
			this.UserName = userName;
			this.UserDomain = userDomain;
			this.Password = password;
			this.AdfsLogonPage = adfsLogonPage;
		}

		protected override void StartTest()
		{
			if (this.AdfsLogonPage.IsIntegratedAuthChallenge)
			{
				this.session.AuthenticationData = new AuthenticationData?(new AuthenticationData
				{
					UseDefaultCredentials = false,
					Credentials = new NetworkCredential(this.UserName, this.Password)
				});
				this.session.BeginGetFollowingRedirections(this.Id, this.AdfsLogonPage.PostUri, RedirectionOptions.FollowUntilNo302, delegate(IAsyncResult resultTemp)
				{
					base.AsyncCallbackWrapper(new AsyncCallback(this.AdfsCredentialPostResponseReceived), resultTemp);
				}, new Dictionary<string, object>
				{
					{
						"CredentialPostCount",
						10
					}
				});
				return;
			}
			this.AdfsLogonPage.HiddenFields.Remove(this.AdfsLogonPage.UserNameFieldName);
			this.AdfsLogonPage.HiddenFields.Remove(this.AdfsLogonPage.PasswordFieldName);
			this.PostCredentials(new int?(1));
		}

		private void PostCredentials(int? credentialPostCount)
		{
			RequestBody body = RequestBody.Format("{4}&{0}={1}&{2}={3}", new object[]
			{
				HttpUtility.UrlEncode(this.AdfsLogonPage.UserNameFieldName),
				HttpUtility.UrlEncode(this.UserName),
				HttpUtility.UrlEncode(this.AdfsLogonPage.PasswordFieldName),
				this.Password,
				this.AdfsLogonPage.HiddenFieldsString
			});
			Cookie cookie = new Cookie("CkTst", "G" + ExDateTime.Now.UtcTicks, "/", this.Uri.Host);
			this.session.CookieContainer.Add(cookie);
			this.session.BeginPostFollowingRedirections(this.Id, this.AdfsLogonPage.PostUri, body, "application/x-www-form-urlencoded", null, RedirectionOptions.FollowUntilNo302, delegate(IAsyncResult resultTemp)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.AdfsCredentialPostResponseReceived), resultTemp);
			}, new Dictionary<string, object>
			{
				{
					"CredentialPostCount",
					credentialPostCount
				}
			});
		}

		private void AdfsCredentialPostResponseReceived(IAsyncResult result)
		{
			Dictionary<string, object> dictionary = result.AsyncState as Dictionary<string, object>;
			int? credentialPostCount = dictionary["CredentialPostCount"] as int?;
			this.TicketPostPage = this.session.EndPostFollowingRedirections<LiveIdBasePage>(result, delegate(HttpWebResponseWrapper response)
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
			if (this.TicketPostPage == null)
			{
				this.PostCredentials(credentialPostCount + 1);
				return;
			}
			base.ExecutionCompletedSuccessfully();
		}

		private const TestId ID = TestId.AdfsAuthentication;
	}
}
