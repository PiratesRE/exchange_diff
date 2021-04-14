using System;
using System.Net;
using System.Security;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class IisAuthentication : BaseTestStep
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
				return TestId.IisAuthentication;
			}
		}

		public IisAuthentication(Uri uri, string userName, string userDomain, SecureString password)
		{
			this.Uri = uri;
			this.UserName = userName;
			this.UserDomain = userDomain;
			this.Password = password;
		}

		protected override void StartTest()
		{
			this.session.AuthenticationData = new AuthenticationData?(new AuthenticationData
			{
				UseDefaultCredentials = false,
				Credentials = new NetworkCredential(this.UserName, this.Password)
			});
			base.ExecutionCompletedSuccessfully();
		}

		private const TestId ID = TestId.IisAuthentication;
	}
}
