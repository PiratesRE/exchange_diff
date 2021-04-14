using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Net.WSTrust;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class SharingInformation
	{
		public TokenTarget TokenTarget { get; private set; }

		public WebServiceUri TargetSharingEpr { get; private set; }

		public Uri TargetAutodiscoverEpr { get; private set; }

		public SmtpAddress RequestorSmtpAddress { get; private set; }

		public SmtpAddress SharingKey { get; private set; }

		public bool IsFromIntraOrgConnector { get; private set; }

		public Exception Exception { get; set; }

		public SharingInformation(SmtpAddress requestorSmtpAddress, SmtpAddress sharingKey, TokenTarget tokenTarget, WebServiceUri targetSharingEpr, Uri targetAutodiscoverEpr)
		{
			this.RequestorSmtpAddress = requestorSmtpAddress;
			this.SharingKey = sharingKey;
			this.TokenTarget = tokenTarget;
			this.TargetSharingEpr = targetSharingEpr;
			this.TargetAutodiscoverEpr = targetAutodiscoverEpr;
			this.IsFromIntraOrgConnector = false;
		}

		public SharingInformation(SmtpAddress requestorSmtpAddress, WebServiceUri targetSharingEpr, Uri targetAutodiscoverEpr)
		{
			this.RequestorSmtpAddress = requestorSmtpAddress;
			this.TargetSharingEpr = targetSharingEpr;
			this.TargetAutodiscoverEpr = targetAutodiscoverEpr;
			this.IsFromIntraOrgConnector = true;
		}

		public SharingInformation(Exception exception)
		{
			this.Exception = exception;
		}
	}
}
