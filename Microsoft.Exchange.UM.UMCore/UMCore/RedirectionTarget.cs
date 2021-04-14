using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class RedirectionTarget
	{
		public static RedirectionTarget Instance
		{
			get
			{
				return RedirectionTarget.instance;
			}
		}

		public abstract RedirectionTarget.ResultSet GetForCallAnsweringCall(UMRecipient user, IRoutingContext context);

		public abstract RedirectionTarget.ResultSet GetForSubscriberAccessCall(UMRecipient user, IRoutingContext context);

		public abstract RedirectionTarget.ResultSet GetForNonUserSpecificCall(OrganizationId orgId, IRoutingContext context);

		private static readonly RedirectionTarget instance = CommonConstants.UseDataCenterCallRouting ? (UMRecyclerConfig.UseDataCenterActiveManagerRouting ? new DataCenterActiveManagerRedirectionTarget() : new DataCenterLegacySupportRedirectionTarget()) : new EnterpriseRedirectionTarget();

		internal class ResultSet
		{
			public PlatformSipUri Uri { get; private set; }

			public string Fqdn { get; private set; }

			public int Port { get; private set; }

			public ResultSet(PlatformSipUri uri, string fqdn, int port)
			{
				this.Uri = uri;
				this.Fqdn = fqdn;
				this.Port = port;
			}
		}
	}
}
