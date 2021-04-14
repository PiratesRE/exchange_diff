using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class GetRoomLists : SingleStepServiceCommand<GetRoomListsRequest, EmailAddressWrapper[]>
	{
		public GetRoomLists(CallContext callContext, GetRoomListsRequest request) : base(callContext, request)
		{
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new GetRoomListsResponse(base.Result.Code, base.Result.Error, base.Result.Value);
		}

		internal override ServiceResult<EmailAddressWrapper[]> Execute()
		{
			if (base.CallContext.AccessingPrincipal == null)
			{
				throw new NameResolutionNoMailboxException();
			}
			int lcid = 0;
			if (base.CallContext.AccessingPrincipal.PreferredCultures.Any<CultureInfo>())
			{
				lcid = base.CallContext.AccessingPrincipal.PreferredCultures.First<CultureInfo>().LCID;
			}
			ExchangePrincipal accessingPrincipal = base.CallContext.AccessingPrincipal;
			IRecipientSession recipientSession = Directory.CreateAddressListScopedADRecipientSessionForOrganization(DirectoryHelper.GetGlobalAddressListFromAddressBookPolicy(accessingPrincipal.MailboxInfo.Configuration.AddressBookPolicy, base.MailboxIdentityMailboxSession.GetADConfigurationSession(true, ConsistencyMode.IgnoreInvalid)), base.CallContext.AccessingADUser.QueryBaseDN, lcid, accessingPrincipal.MailboxInfo.OrganizationId, base.CallContext.EffectiveCaller.ClientSecurityContext);
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetails, RecipientTypeDetails.RoomList);
			ADPagedReader<ADRecipient> roomLists = recipientSession.FindPaged(null, QueryScope.SubTree, filter, new SortBy(ADRecipientSchema.DisplayName, SortOrder.Ascending), 0);
			return new ServiceResult<EmailAddressWrapper[]>(this.GenerateXmlForRoomList(roomLists).ToArray<EmailAddressWrapper>());
		}

		private IEnumerable<EmailAddressWrapper> GenerateXmlForRoomList(IEnumerable<ADRecipient> roomLists)
		{
			List<EmailAddressWrapper> list = new List<EmailAddressWrapper>();
			foreach (ADRecipient adrecipient in roomLists)
			{
				MailboxHelper.MailboxTypeType mailboxTypeType = MailboxHelper.ConvertToMailboxType(adrecipient.RecipientType, adrecipient.RecipientTypeDetails);
				if (mailboxTypeType == MailboxHelper.MailboxTypeType.PublicDL)
				{
					SmtpAddress primarySmtpAddress = adrecipient.PrimarySmtpAddress;
					list.Add(new EmailAddressWrapper
					{
						Name = (string)adrecipient[ADRecipientSchema.DisplayName],
						EmailAddress = primarySmtpAddress.ToString(),
						RoutingType = "SMTP",
						MailboxType = mailboxTypeType.ToString()
					});
				}
				else
				{
					ExTraceGlobals.GetRoomsCallTracer.TraceDebug((long)this.GetHashCode(), "MailboxType is not a PublicDL");
				}
			}
			list = list.OrderBy((EmailAddressWrapper roomList) => roomList.Name, StringComparer.CurrentCulture).ToList<EmailAddressWrapper>();
			ExTraceGlobals.GetRoomsCallTracer.TraceDebug((long)this.GetHashCode(), "RoomLists length is:" + list.Count);
			return list;
		}
	}
}
