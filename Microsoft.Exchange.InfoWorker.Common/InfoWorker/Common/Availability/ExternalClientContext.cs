using System;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.Availability.Proxy;
using Microsoft.Exchange.Net.WSSecurity;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal abstract class ExternalClientContext : ClientContext
	{
		internal ExternalClientContext(SmtpAddress emailAddress, WSSecurityHeader wsSecurityHeader, IBudget budget, ExTimeZone timeZone, CultureInfo clientCulture, string messageId) : base(budget, timeZone, clientCulture, messageId)
		{
			this.emailAddress = emailAddress;
			this.wsSecurityHeader = wsSecurityHeader;
		}

		public abstract ProxyAuthenticator CreateInternalProxyAuthenticator();

		public SmtpAddress EmailAddress
		{
			get
			{
				return this.emailAddress;
			}
		}

		public WSSecurityHeader WSSecurityHeader
		{
			get
			{
				return this.wsSecurityHeader;
			}
		}

		public override OrganizationId OrganizationId
		{
			get
			{
				return OrganizationId.ForestWideOrgId;
			}
		}

		public override ADObjectId QueryBaseDN
		{
			get
			{
				return null;
			}
			set
			{
				throw new InvalidOperationException("Cannot set QueryBaseDN on ExternalClientContext.");
			}
		}

		public override ExchangeVersionType RequestSchemaVersion
		{
			get
			{
				return this.requestSchemaVersion;
			}
			set
			{
				this.requestSchemaVersion = value;
			}
		}

		public override string IdentityForFilteredTracing
		{
			get
			{
				return this.emailAddress.ToString();
			}
		}

		public override void ValidateContext()
		{
		}

		public override void Dispose()
		{
		}

		private SmtpAddress emailAddress;

		private WSSecurityHeader wsSecurityHeader;

		private ExchangeVersionType requestSchemaVersion = ExchangeVersionType.Exchange2010;
	}
}
