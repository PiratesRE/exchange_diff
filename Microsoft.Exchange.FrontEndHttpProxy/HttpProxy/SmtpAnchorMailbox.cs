using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.HttpProxy.Routing;
using Microsoft.Exchange.HttpProxy.Routing.RoutingKeys;

namespace Microsoft.Exchange.HttpProxy
{
	internal class SmtpAnchorMailbox : ArchiveSupportedAnchorMailbox
	{
		public SmtpAnchorMailbox(string smtp, IRequestContext requestContext) : base(AnchorSource.Smtp, smtp, requestContext)
		{
			this.FailOnDomainNotFound = true;
		}

		public string Smtp
		{
			get
			{
				return (string)base.SourceObject;
			}
		}

		public bool FailOnDomainNotFound { get; set; }

		public override string GetOrganizationNameForLogging()
		{
			string text = base.GetOrganizationNameForLogging();
			if (string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(this.Smtp) && SmtpAddress.IsValidSmtpAddress(this.Smtp))
			{
				text = SmtpAddress.Parse(this.Smtp).Domain;
			}
			return text;
		}

		protected override ADRawEntry LoadADRawEntry()
		{
			IRecipientSession session = DirectoryHelper.GetRecipientSessionFromSmtpOrLiveId(base.RequestContext.LatencyTracker, this.Smtp, !this.FailOnDomainNotFound);
			ADRawEntry ret = null;
			if (session != null)
			{
				ret = DirectoryHelper.InvokeAccountForest(base.RequestContext.LatencyTracker, delegate
				{
					bool flag = FaultInjection.TraceTest<bool>(FaultInjection.LIDs.ShouldFailSmtpAnchorMailboxADLookup);
					if (flag)
					{
						return null;
					}
					return session.FindByProxyAddress(new SmtpProxyAddress(this.Smtp, true), this.PropertySet);
				});
			}
			return base.CheckForNullAndThrowIfApplicable<ADRawEntry>(ret);
		}

		protected override IRoutingKey GetRoutingKey()
		{
			return new SmtpRoutingKey(new SmtpAddress(this.Smtp));
		}
	}
}
