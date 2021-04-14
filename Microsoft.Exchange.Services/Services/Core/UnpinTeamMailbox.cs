using System;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class UnpinTeamMailbox : SingleStepServiceCommand<UnpinTeamMailboxRequest, ServiceResultNone>
	{
		public UnpinTeamMailbox(CallContext callContext, UnpinTeamMailboxRequest request) : base(callContext, request)
		{
			this.request = request;
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly((base.CallContext.AccessingPrincipal == null) ? OrganizationId.ForestWideOrgId : base.CallContext.AccessingPrincipal.MailboxInfo.OrganizationId);
			this.readWriteSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, null, CultureInfo.InvariantCulture.LCID, false, ConsistencyMode.IgnoreInvalid, null, sessionSettings, 55, ".ctor", "f:\\15.00.1497\\sources\\dev\\services\\src\\Core\\servicecommands\\UnpinTeamMailbox.cs");
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new UnpinTeamMailboxResponseMessage(base.Result.Code, base.Result.Error);
		}

		internal override ServiceResult<ServiceResultNone> Execute()
		{
			ServiceResult<ServiceResultNone> result = new ServiceResult<ServiceResultNone>(new ServiceResultNone());
			ADUser aduser;
			Exception ex;
			if (!this.TryResolveUser(this.request.EmailAddress, out aduser, out ex) || (!TeamMailbox.IsLocalTeamMailbox(aduser) && !TeamMailbox.IsRemoteTeamMailbox(aduser)))
			{
				result = new ServiceResult<ServiceResultNone>(new ServiceError((ex == null) ? string.Empty : ex.Message, ResponseCodeType.ErrorTeamMailboxNotFound, 0, ExchangeVersion.Exchange2012));
			}
			else
			{
				Exception ex2 = null;
				ADUser aduser2 = (ADUser)this.readWriteSession.FindByProxyAddress(ProxyAddress.Parse(base.CallContext.AccessingPrincipal.MailboxInfo.PrimarySmtpAddress.ToString()));
				if (aduser2 == null)
				{
					ex2 = new Exception("executingUser cannot be found or resolved!");
				}
				else
				{
					try
					{
						bool flag = false;
						if (aduser2.TeamMailboxShowInClientList.Contains(aduser.Id))
						{
							aduser2.TeamMailboxShowInClientList.Remove(aduser.Id);
							flag = true;
						}
						for (int i = 0; i < aduser2.TeamMailboxShowInClientList.Count; i++)
						{
							ADObjectId adobjectId = aduser2.TeamMailboxShowInClientList[i];
							ADUser aduser3 = this.readWriteSession.FindADUserByObjectId(adobjectId);
							if (aduser3 == null || !TeamMailbox.FromDataObject(aduser3).Active)
							{
								aduser2.TeamMailboxShowInClientList.Remove(adobjectId);
								i--;
								flag = true;
							}
						}
						if (flag)
						{
							this.readWriteSession.Save(aduser2);
						}
					}
					catch (TransientException ex3)
					{
						ex2 = ex3;
					}
					catch (DataSourceOperationException ex4)
					{
						ex2 = ex4;
					}
				}
				if (ex2 != null)
				{
					result = new ServiceResult<ServiceResultNone>(new ServiceError(ex2.Message, ResponseCodeType.ErrorTeamMailboxErrorUnknown, 0, ExchangeVersion.Exchange2012));
				}
			}
			return result;
		}

		private bool TryResolveUser(EmailAddressWrapper emailAddress, out ADUser user, out Exception ex)
		{
			ex = null;
			user = null;
			if (emailAddress == null)
			{
				ex = new ArgumentNullException("emailAddress");
			}
			else if (string.Equals(emailAddress.RoutingType, "EX", StringComparison.OrdinalIgnoreCase))
			{
				user = (ADUser)this.readWriteSession.FindByLegacyExchangeDN(emailAddress.EmailAddress);
			}
			else if (SmtpAddress.IsValidSmtpAddress(emailAddress.EmailAddress))
			{
				user = (ADUser)this.readWriteSession.FindByProxyAddress(ProxyAddress.Parse(emailAddress.EmailAddress));
			}
			else
			{
				ex = new ArgumentException("Cannot get internal address for caller; identity: " + emailAddress.EmailAddress);
			}
			return user != null;
		}

		private readonly UnpinTeamMailboxRequest request;

		private readonly IRecipientSession readWriteSession;
	}
}
