using System;
using System.Diagnostics;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.Authentication;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.RequestDispatch;
using Microsoft.Exchange.Net.WSTrust;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class ExternalAuthenticationRequest : AsyncTask
	{
		public ExternalAuthenticationRequest(RequestLogger requestLogger, ExternalAuthentication externalAuthentication, ADUser user, SmtpAddress emailAddress, TokenTarget target, Offer offer)
		{
			this.requestLogger = requestLogger;
			this.user = user;
			this.emailAddress = emailAddress;
			this.target = target;
			this.offer = offer;
			this.securityTokenService = externalAuthentication.GetSecurityTokenService(user.OrganizationId);
		}

		public LocalizedException Exception
		{
			get
			{
				return this.exception;
			}
		}

		public override void Abort()
		{
			base.Abort();
			if (this.asyncResult != null)
			{
				this.securityTokenService.AbortIssueToken(this.asyncResult);
			}
		}

		public RequestedToken RequestedToken
		{
			get
			{
				return this.requestedToken;
			}
		}

		public Offer Offer
		{
			get
			{
				return this.offer;
			}
		}

		public override void BeginInvoke(TaskCompleteCallback callback)
		{
			base.BeginInvoke(callback);
			this.stopwatch = Stopwatch.StartNew();
			try
			{
				DelegationTokenRequest request = new DelegationTokenRequest
				{
					FederatedIdentity = this.user.GetFederatedIdentity(),
					EmailAddress = this.GetFederatedSmtpAddress().ToString(),
					Target = this.target,
					Offer = this.offer
				};
				this.asyncResult = this.securityTokenService.BeginIssueToken(request, new AsyncCallback(this.Complete), null);
			}
			catch (LocalizedException ex)
			{
				this.exception = ex;
				base.Complete();
			}
		}

		private void Complete(IAsyncResult asyncResult)
		{
			try
			{
				this.requestedToken = this.securityTokenService.EndIssueToken(asyncResult);
			}
			catch (LocalizedException ex)
			{
				this.exception = ex;
			}
			finally
			{
				this.stopwatch.Stop();
				this.requestLogger.Add(RequestStatistics.Create(RequestStatisticsType.FederatedToken, this.stopwatch.ElapsedMilliseconds));
				base.Complete();
			}
		}

		public SmtpAddress GetFederatedSmtpAddress()
		{
			return this.user.GetFederatedSmtpAddress(this.emailAddress);
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"ExternalAuthenticationRequest as user ",
				this.user.Id,
				" to ",
				this.target
			});
		}

		private RequestLogger requestLogger;

		private SecurityTokenService securityTokenService;

		private ADUser user;

		private SmtpAddress emailAddress;

		private TokenTarget target;

		private Offer offer;

		private RequestedToken requestedToken;

		private IAsyncResult asyncResult;

		private Stopwatch stopwatch;

		private LocalizedException exception;

		private static readonly Microsoft.Exchange.Diagnostics.Trace RequestRoutingTracer = ExTraceGlobals.RequestRoutingTracer;
	}
}
