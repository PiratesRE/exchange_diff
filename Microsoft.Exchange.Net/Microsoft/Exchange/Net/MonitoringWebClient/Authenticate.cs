using System;
using System.Collections.Generic;
using System.Net;
using System.Security;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class Authenticate : BaseTestStep
	{
		public Uri Uri { get; private set; }

		public string UserName { get; private set; }

		public string UserDomain { get; private set; }

		public SecureString Password { get; private set; }

		public ITestFactory TestFactory { get; private set; }

		public AuthenticationParameters AuthenticationParameters { get; private set; }

		protected override TestId Id
		{
			get
			{
				return TestId.Authentication;
			}
		}

		public override object Result
		{
			get
			{
				return this.authenticationResult;
			}
		}

		public Authenticate(Uri uri, string userName, string userDomain, SecureString password, AuthenticationParameters authenticationParameters, ITestFactory factory)
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
			if (this.Uri.Port == 444)
			{
				ITestStep testStep = this.TestFactory.CreateBrickAuthenticateStep(this.Uri, this.UserName, this.UserDomain, this.AuthenticationParameters);
				testStep.BeginExecute(this.session, delegate(IAsyncResult tempResult)
				{
					base.AsyncCallbackWrapper(new AsyncCallback(this.AuthenticationStepFinished), tempResult);
				}, testStep);
				return;
			}
			this.session.BeginGetFollowingRedirections(this.Id, this.Uri.ToString(), RedirectionOptions.StopOnFirstCrossDomainRedirect, delegate(IAsyncResult tempResult)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.AuthenticationResponseReceived), tempResult);
			}, new Dictionary<string, object>
			{
				{
					"CafeErrorPageValidationRules",
					CafeErrorPageValidationRules.Accept401Response
				}
			});
		}

		private void AuthenticationResponseReceived(IAsyncResult result)
		{
			var <>f__AnonymousType = this.session.EndGet(result, new HttpStatusCode[]
			{
				HttpStatusCode.OK,
				HttpStatusCode.Found,
				HttpStatusCode.Unauthorized
			}, (HttpWebResponseWrapper response) => new
			{
				StatusCode = response.StatusCode,
				RedirectUrl = response.Headers["Location"],
				LastUri = response.Request.RequestUri
			});
			ITestStep testStep;
			if (this.IsFbaAuthentication(<>f__AnonymousType.StatusCode, <>f__AnonymousType.LastUri))
			{
				testStep = this.TestFactory.CreateFbaAuthenticateStep(this.Uri, this.UserName, this.UserDomain, this.Password, this.AuthenticationParameters);
			}
			else if (this.IsLiveIdAuthentication(<>f__AnonymousType.StatusCode, <>f__AnonymousType.RedirectUrl))
			{
				testStep = this.TestFactory.CreateLiveIDAuthenticateStep(this.Uri, this.UserName, this.UserDomain, this.Password, this.AuthenticationParameters, this.TestFactory);
			}
			else
			{
				if (!this.IsIisAuthentication(<>f__AnonymousType.StatusCode))
				{
					throw new NotImplementedException("Authentication method was not recognized.");
				}
				testStep = this.TestFactory.CreateIisAuthenticateStep(this.Uri, this.UserName, this.UserDomain, this.Password);
			}
			testStep.BeginExecute(this.session, delegate(IAsyncResult tempResult)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.AuthenticationStepFinished), tempResult);
			}, testStep);
		}

		private void AuthenticationStepFinished(IAsyncResult result)
		{
			ITestStep testStep = result.AsyncState as ITestStep;
			testStep.EndExecute(result);
			this.authenticationResult = testStep.Result;
			base.ExecutionCompletedSuccessfully();
		}

		private bool IsFbaAuthentication(HttpStatusCode httpStatusCode, Uri lastResponseUri)
		{
			return httpStatusCode == HttpStatusCode.OK && lastResponseUri.Host == this.Uri.Host;
		}

		private bool IsLiveIdAuthentication(HttpStatusCode httpStatusCode, string redirectUrl)
		{
			return httpStatusCode == HttpStatusCode.Found;
		}

		private bool IsIisAuthentication(HttpStatusCode httpStatusCode)
		{
			return httpStatusCode == HttpStatusCode.Unauthorized;
		}

		private const TestId ID = TestId.Authentication;

		private const int BrickPort = 444;

		private object authenticationResult;
	}
}
