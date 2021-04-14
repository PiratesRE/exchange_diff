using System;
using System.Net;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common.SmartCategorization
{
	internal class CafeFailureAnalyser : FailureAnalyserBase
	{
		public static IFailureAnalyser Instance
		{
			get
			{
				return CafeFailureAnalyser.instance;
			}
		}

		private CafeFailureAnalyser()
		{
		}

		protected override FailureDetails AnalyseCafeFailure(RequestFailureContext requestFailureContext)
		{
			FailureDetails result;
			if (requestFailureContext.LiveIdAuthResult != null && this.TryClassifyLiveIdBasicAuthResult(requestFailureContext.LiveIdAuthResult.Value, out result))
			{
				return result;
			}
			if (requestFailureContext.HttpProxySubErrorCode != null && this.TryClassifyHttpProxySubErrorCode(requestFailureContext.HttpProxySubErrorCode.Value, out result))
			{
				return result;
			}
			if (requestFailureContext.HttpStatusCode == 401)
			{
				return new FailureDetails(FailureType.Recognized, ExchangeComponent.Monitoring, SCStrings.FailureMonitoringAccount, HttpStatusCode.Unauthorized.ToString());
			}
			return new FailureDetails(FailureType.Unrecognized, ExchangeComponent.Cafe);
		}

		protected override FailureDetails AnalyseBackendFailure(RequestFailureContext requestFailureContext)
		{
			if (requestFailureContext.HttpStatusCode == 401)
			{
				return new FailureDetails(FailureType.Recognized, ExchangeComponent.AD, SCStrings.FailureFrontendBackendAuthN, HttpStatusCode.Unauthorized.ToString());
			}
			return base.AnalyseBackendFailure(requestFailureContext);
		}

		private bool TryClassifyHttpProxySubErrorCode(HttpProxySubErrorCode errorCode, out FailureDetails failureDetails)
		{
			failureDetails = null;
			switch (errorCode)
			{
			case HttpProxySubErrorCode.DirectoryOperationError:
				failureDetails = new FailureDetails(FailureType.Recognized, ExchangeComponent.AD, SCStrings.FailureActiveDirectory, errorCode.ToString());
				return true;
			case HttpProxySubErrorCode.MServOperationError:
			case HttpProxySubErrorCode.ServerDiscoveryError:
			case HttpProxySubErrorCode.ServerLocatorError:
				failureDetails = new FailureDetails(FailureType.Recognized, ExchangeComponent.Gls, SCStrings.FailureServerLocator, errorCode.ToString());
				return true;
			default:
				return false;
			}
		}

		private bool TryClassifyLiveIdBasicAuthResult(LiveIdAuthResult authResult, out FailureDetails failureDetails)
		{
			failureDetails = null;
			switch (authResult)
			{
			case LiveIdAuthResult.UserNotFoundInAD:
			case LiveIdAuthResult.AuthFailure:
			case LiveIdAuthResult.ExpiredCreds:
			case LiveIdAuthResult.InvalidCreds:
			case LiveIdAuthResult.RecoverableAuthFailure:
			case LiveIdAuthResult.AmbigiousMailboxFoundFailure:
				failureDetails = new FailureDetails(FailureType.Recognized, ExchangeComponent.Monitoring, string.Format(SCStrings.FailureMonitoringAccount, authResult.ToString()), authResult.ToString());
				return true;
			case LiveIdAuthResult.LiveServerUnreachable:
			case LiveIdAuthResult.FederatedStsUnreachable:
			case LiveIdAuthResult.OperationTimedOut:
			case LiveIdAuthResult.CommunicationFailure:
				failureDetails = new FailureDetails(FailureType.Recognized, ExchangeComponent.LiveId, string.Format(SCStrings.FailureLiveId, authResult.ToString()), authResult.ToString());
				return true;
			}
			return false;
		}

		private static IFailureAnalyser instance = new CafeFailureAnalyser();
	}
}
