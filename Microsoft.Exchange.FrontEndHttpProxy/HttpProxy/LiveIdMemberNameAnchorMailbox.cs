using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.HttpProxy.Routing;
using Microsoft.Exchange.HttpProxy.Routing.RoutingKeys;

namespace Microsoft.Exchange.HttpProxy
{
	internal class LiveIdMemberNameAnchorMailbox : UserBasedAnchorMailbox
	{
		public LiveIdMemberNameAnchorMailbox(string liveIdMemberName, string organizationContext, IRequestContext requestContext) : base(AnchorSource.LiveIdMemberName, liveIdMemberName, requestContext)
		{
			if (string.IsNullOrEmpty(liveIdMemberName))
			{
				throw new ArgumentNullException("liveIdMemberName");
			}
			this.OrganizationContext = organizationContext;
		}

		public string LiveIdMemberName
		{
			get
			{
				return (string)base.SourceObject;
			}
		}

		public string OrganizationContext { get; private set; }

		protected override ADRawEntry LoadADRawEntry()
		{
			string text = this.OrganizationContext;
			if (string.IsNullOrEmpty(text))
			{
				SmtpAddress smtpAddress = new SmtpAddress(this.LiveIdMemberName);
				text = smtpAddress.Domain;
			}
			bool flag = PuidAnchorMailbox.AllowMissingDirectoryObject.Value || AnchorMailbox.AllowMissingTenant.Value;
			ITenantRecipientSession session = (ITenantRecipientSession)DirectoryHelper.GetRecipientSessionFromDomain(base.RequestContext.LatencyTracker, text, flag);
			if (flag && session == null)
			{
				return null;
			}
			ExTraceGlobals.VerboseTracer.Information<string, string, string>((long)this.GetHashCode(), "Searching GC {0} for LiveIdMemberName {1}, OrganizationContext {2}", session.DomainController ?? "<null>", this.LiveIdMemberName, this.OrganizationContext ?? "<null>");
			ADRawEntry adrawEntry = DirectoryHelper.InvokeAccountForest(base.RequestContext.LatencyTracker, () => session.FindByLiveIdMemberName(this.LiveIdMemberName, this.PropertySet));
			if (adrawEntry != null && base.RequestContext.HttpContext.User.Identity.Name.Equals(this.LiveIdMemberName, StringComparison.OrdinalIgnoreCase))
			{
				base.RequestContext.HttpContext.Items[Constants.CallerADRawEntryKeyName] = adrawEntry;
			}
			return adrawEntry;
		}

		protected override IRoutingKey GetRoutingKey()
		{
			return new LiveIdMemberNameRoutingKey(new SmtpAddress(this.LiveIdMemberName), this.OrganizationContext);
		}
	}
}
