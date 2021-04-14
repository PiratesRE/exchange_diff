using System;
using System.Security;
using Microsoft.Exchange.Net.MonitoringWebClient.Ecp;
using Microsoft.Exchange.Net.MonitoringWebClient.Owa;
using Microsoft.Exchange.Net.MonitoringWebClient.Rws;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal interface ITestFactory
	{
		ITestStep CreateOwaLoginScenario(Uri uri, string userName, string userDomain, SecureString password, OwaLoginParameters owaLoginParameters, ITestFactory testFactory);

		ITestStep CreateOwaExternalLoginAgainstSpecificServerScenario(Uri uri, string userName, string userDomain, SecureString password, string serverToHit, ITestFactory testFactory);

		ITestStep CreateOwaHealthCheckScenario(Uri uri, ITestFactory testFactory);

		ITestStep CreateOwaStaticPageScenario(Uri uri, ITestFactory testFactory);

		ITestStep CreateOwaFindPlacesScenario(Uri uri, string userName, string userDomain, SecureString password, ITestFactory testFactory);

		ITestStep CreateOwaCertificateRevocationCheckScenario(Uri uri, ITestFactory testFactory);

		ITestStep CreateEcpLoginScenario(Uri uri, string userName, string userDomain, SecureString password, ITestFactory testFactory);

		ITestStep CreateEcpExternalLoginAgainstSpecificServerScenario(Uri uri, string userName, string userDomain, SecureString password, string serverToHit, ITestFactory testFactory);

		ITestStep CreateEcpActiveMonitoringLocalScenario(Uri uri, string userName, string userDomain, AuthenticationParameters authenticationParameters, ITestFactory testFactory, Func<EcpStartPage, ITestStep> getFeatureTestStep);

		ITestStep CreateEcpActiveMonitoringOutsideInScenario(Uri uri, string userName, SecureString password, ITestFactory testFactory, Func<EcpStartPage, ITestStep> getFeatureTestStep);

		ITestStep CreateRwsCallScenario(Uri uri, RwsAuthenticationInfo authenticationInfo, ITestFactory testFactory);

		ITestStep CreateAuthenticateStep(Uri uri, string userName, string userDomain, SecureString password, AuthenticationParameters authenticationParameters, ITestFactory testFactory);

		ITestStep CreateLiveIDAuthenticateStep(Uri uri, string userName, string userDomain, SecureString password, AuthenticationParameters authenticationParameters, ITestFactory testFactory);

		ITestStep CreateIisAuthenticateStep(Uri uri, string userName, string userDomain, SecureString password);

		ITestStep CreateFbaAuthenticateStep(Uri uri, string userName, string userDomain, SecureString password, AuthenticationParameters authenticationParameters);

		ITestStep CreateBrickAuthenticateStep(Uri uri, string userName, string userDomain, AuthenticationParameters authenticationParameters);

		ITestStep CreateAdfsAuthenticateStep(Uri uri, string userName, string userDomain, SecureString password, AdfsLogonPage adfsLogonPage);

		ITestStep CreateMeasureClientLatencyStep();

		ITestStep CreateOwaStartPageStep(Uri uri);

		ITestStep CreateLogoffStep(Uri uri, string logoffPath);

		ITestStep CreateOwaSessionDataStep(Uri uri);

		ITestStep CreateOwaWebServiceStep(Uri uri, string action);

		ITestStep CreateOwaWebServiceStep(Uri uri, string action, RequestBody requestBody);

		ITestStep CreateOwaPingStep(Uri uri);

		ITestStep CreateOwaHealthCheckStep(Uri uri);

		ITestStep CreateOwaDownloadStaticFileStep(Uri uri);

		ITestStep CreateEstablishAffinityStep(Uri uri, string serverToHit);

		ITestStep CreateEcpStartPageStep(Uri uri);

		ITestStep CreateEcpWebServiceCallStep(Uri uri);

		ITestStep CreateRwsAuthenticateStep(Uri uri, RwsAuthenticationInfo authenticationInfo, ITestFactory testFactory);

		ITestStep CreateRwsBrickAuthenticateStep(CommonAccessToken token, Uri uri);

		ITestStep CreateRwsCallStep(Uri uri);
	}
}
