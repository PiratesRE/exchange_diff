using System;
using System.Security;
using Microsoft.Exchange.Net.MonitoringWebClient.Ecp;
using Microsoft.Exchange.Net.MonitoringWebClient.Owa;
using Microsoft.Exchange.Net.MonitoringWebClient.Rws;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class TestFactory : ITestFactory
	{
		public ITestStep CreateOwaLoginScenario(Uri uri, string userName, string userDomain, SecureString password, OwaLoginParameters owaLoginParameters, ITestFactory testFactory)
		{
			return new OwaLogin(uri, userName, userDomain, password, owaLoginParameters, testFactory);
		}

		public ITestStep CreateOwaExternalLoginAgainstSpecificServerScenario(Uri uri, string userName, string userDomain, SecureString password, string serverToHit, ITestFactory testFactory)
		{
			return new OwaExternalLoginAgainstSpecificServer(uri, userName, userDomain, password, testFactory, serverToHit);
		}

		public ITestStep CreateOwaHealthCheckScenario(Uri uri, ITestFactory testFactory)
		{
			return new OwaHealthCheckScenario(uri, testFactory);
		}

		public ITestStep CreateOwaStaticPageScenario(Uri uri, ITestFactory testFactory)
		{
			return new OwaStaticPage(uri, testFactory);
		}

		public ITestStep CreateOwaFindPlacesScenario(Uri uri, string userName, string userDomain, SecureString password, ITestFactory testFactory)
		{
			return new FindPlacesScenario(uri, userName, userDomain, password, testFactory);
		}

		public ITestStep CreateOwaCertificateRevocationCheckScenario(Uri uri, ITestFactory testFactory)
		{
			return new OwaCertificateRevocationCheckScenario(uri, testFactory);
		}

		public ITestStep CreateAuthenticateStep(Uri uri, string userName, string userDomain, SecureString password, AuthenticationParameters authenticationParameters, ITestFactory testFactory)
		{
			return new Authenticate(uri, userName, userDomain, password, authenticationParameters, testFactory);
		}

		public ITestStep CreateLiveIDAuthenticateStep(Uri uri, string userName, string userDomain, SecureString password, AuthenticationParameters authenticationParameters, ITestFactory testFactory)
		{
			return new LiveIdAuthentication(uri, userName, userDomain, password, authenticationParameters, testFactory);
		}

		public ITestStep CreateIisAuthenticateStep(Uri uri, string userName, string userDomain, SecureString password)
		{
			return new IisAuthentication(uri, userName, userDomain, password);
		}

		public ITestStep CreateFbaAuthenticateStep(Uri uri, string userName, string userDomain, SecureString password, AuthenticationParameters authenticationParameters)
		{
			return new FbaAuthentication(uri, userName, userDomain, password, authenticationParameters, this);
		}

		public ITestStep CreateBrickAuthenticateStep(Uri uri, string userName, string userDomain, AuthenticationParameters authenticationParameters)
		{
			return new BrickAuthentication(uri, userName, userDomain, authenticationParameters);
		}

		public ITestStep CreateAdfsAuthenticateStep(Uri uri, string userName, string userDomain, SecureString password, AdfsLogonPage adfsLogonPage)
		{
			return new AdfsAuthentication(uri, userName, userDomain, password, adfsLogonPage);
		}

		public ITestStep CreateMeasureClientLatencyStep()
		{
			return new MeasureClientLantency();
		}

		public ITestStep CreateOwaStartPageStep(Uri uri)
		{
			return new OwaStartPage(uri);
		}

		public ITestStep CreateOwaDownloadStaticFileStep(Uri uri)
		{
			return new OwaDownloadStaticFile(uri);
		}

		public ITestStep CreateLogoffStep(Uri uri, string logoffPath)
		{
			return new Logoff(uri, logoffPath);
		}

		public ITestStep CreateOwaPingStep(Uri uri)
		{
			return new OwaPing(uri);
		}

		public ITestStep CreateOwaSessionDataStep(Uri uri)
		{
			return new OwaSessionData(uri);
		}

		public ITestStep CreateOwaWebServiceStep(Uri uri, string action)
		{
			return new OwaWebService(uri, action);
		}

		public ITestStep CreateOwaWebServiceStep(Uri uri, string action, RequestBody requestBody)
		{
			return new OwaWebService(uri, action, requestBody);
		}

		public ITestStep CreateOwaHealthCheckStep(Uri uri)
		{
			return new OwaHealthCheck(uri);
		}

		public ITestStep CreateEstablishAffinityStep(Uri uri, string serverToHit)
		{
			return new EstablishAffinity(uri, serverToHit);
		}

		public ITestStep CreateEcpLoginScenario(Uri uri, string userName, string userDomain, SecureString password, ITestFactory testFactory)
		{
			return new EcpLogin(uri, userName, userDomain, password, testFactory);
		}

		public ITestStep CreateEcpExternalLoginAgainstSpecificServerScenario(Uri uri, string userName, string userDomain, SecureString password, string serverToHit, ITestFactory testFactory)
		{
			return new EcpExternalLoginAgainstSpecificServer(uri, userName, userDomain, password, testFactory, serverToHit);
		}

		public ITestStep CreateEcpActiveMonitoringLocalScenario(Uri uri, string userName, string userDomain, AuthenticationParameters authenticationParameters, ITestFactory testFactory, Func<EcpStartPage, ITestStep> getFeatureTestStep)
		{
			return new EcpActiveMonitoringLocal(uri, userName, userDomain, authenticationParameters, testFactory, getFeatureTestStep);
		}

		public ITestStep CreateEcpActiveMonitoringOutsideInScenario(Uri uri, string userName, SecureString password, ITestFactory testFactory, Func<EcpStartPage, ITestStep> getFeatureTestStep)
		{
			return new EcpActiveMonitoringOutsideIn(uri, userName, password, testFactory, getFeatureTestStep);
		}

		public ITestStep CreateEcpStartPageStep(Uri uri)
		{
			return new EcpStartPage(uri);
		}

		public ITestStep CreateEcpWebServiceCallStep(Uri uri)
		{
			return new EcpWebServiceCall(uri);
		}

		public ITestStep CreateRwsCallScenario(Uri uri, RwsAuthenticationInfo authenticationInfo, ITestFactory testFactory)
		{
			return new RwsCallScenario(uri, authenticationInfo, testFactory);
		}

		public ITestStep CreateRwsAuthenticateStep(Uri uri, RwsAuthenticationInfo authenticationInfo, ITestFactory testFactory)
		{
			return new RwsAuthentication(uri, authenticationInfo, testFactory);
		}

		public ITestStep CreateRwsBrickAuthenticateStep(CommonAccessToken token, Uri uri)
		{
			return new RwsBrickAuthentication(token, uri);
		}

		public ITestStep CreateRwsCallStep(Uri uri)
		{
			return new RwsCall(uri);
		}
	}
}
