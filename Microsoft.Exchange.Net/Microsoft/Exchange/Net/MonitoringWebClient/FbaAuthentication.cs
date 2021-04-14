using System;
using System.Collections.Generic;
using System.Net;
using System.Security;
using System.Threading;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class FbaAuthentication : BaseTestStep
	{
		public Uri Uri { get; private set; }

		public string UserName { get; private set; }

		public string UserDomain { get; private set; }

		public SecureString Password { get; private set; }

		public ITestFactory TestFactory { get; private set; }

		protected override TestId Id
		{
			get
			{
				return TestId.FbaAuthentication;
			}
		}

		public AuthenticationParameters AuthenticationParameters { get; set; }

		public override object Result
		{
			get
			{
				return this.Uri;
			}
		}

		public FbaAuthentication(Uri uri, string userName, string userDomain, SecureString password, AuthenticationParameters authenticationParameters, TestFactory testFactory)
		{
			this.Uri = uri;
			this.UserName = userName;
			this.UserDomain = userDomain;
			this.Password = password;
			this.AuthenticationParameters = authenticationParameters;
			this.TestFactory = testFactory;
		}

		protected override void StartTest()
		{
			this.session.BeginGetFollowingRedirections(this.Id, this.Uri.ToString(), delegate(IAsyncResult result)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.FbaRedirectToLogonPageReceived), result);
			}, null);
		}

		private void FbaRedirectToLogonPageReceived(IAsyncResult result)
		{
			FbaRedirectPage fbaRedirectPage = this.session.EndGetFollowingRedirections<FbaRedirectPage>(result, (HttpWebResponseWrapper response) => FbaRedirectPage.Parse(response));
			Uri uri = new Uri(this.Uri, fbaRedirectPage.FbaLogonPagePathAndQuery);
			this.session.BeginGet(this.Id, uri.OriginalString, delegate(IAsyncResult resultObj)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.FbaLogonPageReceived), resultObj);
			}, null);
		}

		private void FbaLogonPageReceived(IAsyncResult result)
		{
			FbaLogonPage fbaLogonPage = this.session.EndGet<FbaLogonPage>(result, (HttpWebResponseWrapper response) => FbaLogonPage.Parse(response));
			if (fbaLogonPage.StaticFileUri != null && this.AuthenticationParameters.ShouldDownloadStaticFileOnLogonPage)
			{
				this.simultaneousStepCount = 2;
				ITestStep testStep = this.TestFactory.CreateOwaDownloadStaticFileStep(fbaLogonPage.StaticFileUri);
				testStep.BeginExecute(this.session, delegate(IAsyncResult tempResult)
				{
					base.AsyncCallbackWrapper(new AsyncCallback(this.DownloadStaticFileStepCompleted), tempResult);
				}, testStep);
			}
			Cookie cookie = new Cookie("PBack", "0");
			this.session.CookieContainer.Add(cookie);
			RequestBody requestBody = RequestBody.Format("{0}&username={1}&password={2}", new object[]
			{
				fbaLogonPage.HiddenFieldsString,
				this.UserName,
				this.Password
			});
			this.PostCredentials(requestBody, fbaLogonPage, 0);
		}

		private void PostCredentials(RequestBody requestBody, FbaLogonPage fbaLogonPage, int credentialPostCount = 0)
		{
			Uri uri = new Uri(this.Uri, fbaLogonPage.PostTarget);
			this.session.BeginPost(this.Id, uri.ToString(), requestBody, "application/x-www-form-urlencoded", delegate(IAsyncResult resultObj)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.FbaCredentialPostResponseReceived), resultObj);
			}, new Dictionary<string, object>
			{
				{
					"FbaLogonPage",
					fbaLogonPage
				},
				{
					"CredentialPostCount",
					credentialPostCount
				}
			});
		}

		private void FbaCredentialPostResponseReceived(IAsyncResult result)
		{
			Dictionary<string, object> dictionary = result.AsyncState as Dictionary<string, object>;
			FbaLogonPage fbaLogonPage = dictionary["FbaLogonPage"] as FbaLogonPage;
			int credentialPostCount = (int)dictionary["CredentialPostCount"];
			FbaSilentRedirectPage fbaSilentRedirectPage = this.session.EndPost<FbaSilentRedirectPage>(result, delegate(HttpWebResponseWrapper response)
			{
				FbaSilentRedirectPage result2 = null;
				if (credentialPostCount < 5 && FbaSilentRedirectPage.TryParse(response, out result2))
				{
					return result2;
				}
				FbaLogonErrorPage fbaLogonErrorPage = null;
				if (FbaLogonErrorPage.TryParse(response, out fbaLogonErrorPage))
				{
					throw new LogonException(MonitoringWebClientStrings.LogonError(fbaLogonErrorPage.ErrorString), response.Request, response, fbaLogonErrorPage);
				}
				if (response.Headers["Set-Cookie"] == null || response.Headers["Set-Cookie"].IndexOf("cadata", StringComparison.OrdinalIgnoreCase) < 0 || response.Headers["Set-Cookie"].IndexOf("cadata=;", StringComparison.OrdinalIgnoreCase) >= 0)
				{
					throw new MissingKeywordException(MonitoringWebClientStrings.MissingFbaAuthCookies, response.Request, response, "CAData cookie");
				}
				return null;
			});
			if (fbaSilentRedirectPage != null)
			{
				this.Uri = fbaSilentRedirectPage.Destination;
				RequestBody requestBody = RequestBody.Format("{0}", new object[]
				{
					fbaSilentRedirectPage.HiddenFieldsString
				});
				this.PostCredentials(requestBody, fbaLogonPage, credentialPostCount + 1);
				return;
			}
			this.FinishTestStep();
		}

		private void DownloadStaticFileStepCompleted(IAsyncResult result)
		{
			ITestStep testStep = result.AsyncState as ITestStep;
			testStep.EndExecute(result);
			this.FinishTestStep();
		}

		private void FinishTestStep()
		{
			int num = Interlocked.Increment(ref this.currentFinishedStepCount);
			if (num >= this.simultaneousStepCount)
			{
				base.ExecutionCompletedSuccessfully();
			}
		}

		private const TestId ID = TestId.FbaAuthentication;

		private int simultaneousStepCount = 1;

		private int currentFinishedStepCount;
	}
}
