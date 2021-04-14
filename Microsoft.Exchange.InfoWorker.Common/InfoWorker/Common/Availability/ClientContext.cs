using System;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.Availability.Proxy;
using Microsoft.Exchange.InfoWorker.Common.Sharing;
using Microsoft.Exchange.Net.WSSecurity;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal abstract class ClientContext : IDisposable
	{
		public static ClientContext Create(ClientSecurityContext clientSecurityContext, IBudget budget, ExTimeZone timeZone, CultureInfo clientCulture)
		{
			return ClientContext.Create(clientSecurityContext, budget, timeZone, clientCulture, null, null);
		}

		public static ClientContext Create(ClientSecurityContext clientSecurityContext, IBudget budget, ExTimeZone timeZone, CultureInfo clientCulture, string messageId)
		{
			return ClientContext.Create(clientSecurityContext, budget, timeZone, clientCulture, messageId, null);
		}

		public static ClientContext Create(ClientSecurityContext clientSecurityContext, OrganizationId organizationId, IBudget budget, ExTimeZone timeZone, CultureInfo clientCulture, string messageId)
		{
			return new InternalClientContext(clientSecurityContext, organizationId, budget, timeZone, clientCulture, messageId);
		}

		public static ClientContext Create(ClientSecurityContext clientSecurityContext, IBudget budget, ExTimeZone timeZone, CultureInfo clientCulture, string messageId, ADUser adUser)
		{
			return new InternalClientContext(clientSecurityContext, budget, timeZone, clientCulture, messageId, adUser);
		}

		public static ClientContext Create(SmtpAddress emailAddress, SmtpAddress externalId, WSSecurityHeader wsSecurityHeader, SharingSecurityHeader sharingSecurityHeader, IBudget budget, ExTimeZone timeZone, CultureInfo clientCulture)
		{
			return ClientContext.Create(emailAddress, externalId, wsSecurityHeader, sharingSecurityHeader, budget, timeZone, clientCulture, null);
		}

		public static ClientContext Create(SmtpAddress emailAddress, SmtpAddress externalId, WSSecurityHeader wsSecurityHeader, SharingSecurityHeader sharingSecurityHeader, IBudget budget, ExTimeZone timeZone, CultureInfo clientCulture, string messageId)
		{
			if (externalId != SmtpAddress.Empty)
			{
				return new PersonalClientContext(emailAddress, externalId, wsSecurityHeader, sharingSecurityHeader, budget, timeZone, clientCulture, messageId);
			}
			return new OrganizationalClientContext(emailAddress, emailAddress.Domain, wsSecurityHeader, budget, timeZone, clientCulture, messageId);
		}

		public ExTimeZone TimeZone { get; private set; }

		public CultureInfo ClientCulture { get; private set; }

		public string MessageId { get; set; }

		public IBudget Budget { get; private set; }

		public string RequestId { get; set; }

		public void CheckOverBudget()
		{
			if (this.Budget != null)
			{
				this.Budget.CheckOverBudget();
			}
		}

		public abstract void Dispose();

		public abstract string IdentityForFilteredTracing { get; }

		public abstract OrganizationId OrganizationId { get; }

		public abstract ADObjectId QueryBaseDN { get; set; }

		public abstract ExchangeVersionType RequestSchemaVersion { get; set; }

		public abstract void ValidateContext();

		protected ClientContext(IBudget budget, ExTimeZone timeZone, CultureInfo clientCulture, string messageId)
		{
			this.Budget = budget;
			this.TimeZone = timeZone;
			this.ClientCulture = clientCulture;
			this.MessageId = messageId;
		}
	}
}
