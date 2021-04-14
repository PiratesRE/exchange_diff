using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Net.MonitoringWebClient;
using Microsoft.Exchange.Net.MonitoringWebClient.Ecp;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Eac.Probes
{
	internal class EacBackEndLogonProbe : EacWebClientProbeBase
	{
		internal override Task ExecuteScenario(IHttpSession session)
		{
			session.PersistentHeaders.Add("X-IsFromCafe", "1");
			string text = base.Definition.Attributes["DatabaseGuid"];
			if (!DirectoryAccessor.Instance.IsDatabaseCopyActiveOnLocalServer(new Guid(text)))
			{
				base.CancelReason = string.Format("Skip probe '{0}' because database with guid '{1}' is not active on local server", base.Definition.Name, text);
				this.TraceInformation(base.CancelReason, new object[0]);
				CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
				Task result = new Task(delegate()
				{
				}, cancellationTokenSource.Token);
				cancellationTokenSource.Cancel();
				if (cancellationTokenSource != null)
				{
					cancellationTokenSource.Dispose();
				}
				return result;
			}
			if (!base.Definition.Attributes.ContainsKey("CommonAccessToken"))
			{
				throw new ArgumentException(Strings.AttributeMissingFromProbeDefinition("CommonAccessToken"));
			}
			string text2 = base.Definition.Attributes["CommonAccessToken"];
			if (string.IsNullOrWhiteSpace(text2))
			{
				throw new ArgumentException(Strings.InvalidAccessToken(text2));
			}
			Uri uri = new Uri(base.Definition.Endpoint);
			string account = base.Definition.Account;
			if (string.IsNullOrEmpty(account) || account.IndexOf('@') < 0)
			{
				throw new ArgumentException(Strings.InvalidUserName(account));
			}
			string userDomain = account.Substring(account.IndexOf('@') + 1);
			AuthenticationParameters authenticationParameters = OwaUtils.ReadBackendAuthenticationParameters(base.Definition);
			ITestFactory testFactory = new TestFactory();
			ITestStep testStep = testFactory.CreateEcpActiveMonitoringLocalScenario(uri, account, userDomain, authenticationParameters, testFactory, (EcpStartPage startPage) => testFactory.CreateEcpWebServiceCallStep(startPage.Uri));
			return testStep.CreateTask(session);
		}
	}
}
