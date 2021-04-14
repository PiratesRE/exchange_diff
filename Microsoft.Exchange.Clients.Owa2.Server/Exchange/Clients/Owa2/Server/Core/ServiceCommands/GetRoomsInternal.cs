using System;
using System.Globalization;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.ServiceCommands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class GetRoomsInternal : ServiceCommand<GetRoomsResponse>
	{
		public GetRoomsInternal(CallContext callContext, string roomList) : base(callContext)
		{
			this.roomList = roomList;
		}

		protected override GetRoomsResponse InternalExecute()
		{
			ServiceResult<EwsRoomType[]> serviceResult = (this.roomList != null) ? this.GetSpecifiedRooms() : this.GetAllAvailableRooms();
			return new GetRoomsResponse(serviceResult.Code, serviceResult.Error, serviceResult.Value);
		}

		private ServiceResult<EwsRoomType[]> GetSpecifiedRooms()
		{
			GetRoomsRequest getRoomsRequest = new GetRoomsRequest();
			getRoomsRequest.RoomList = new EmailAddressWrapper
			{
				EmailAddress = this.roomList
			};
			GetRooms getRooms = new GetRooms(base.CallContext, getRoomsRequest);
			getRooms.PreExecute();
			ServiceResult<EwsRoomType[]> serviceResult = getRooms.Execute();
			getRooms.SetCurrentStepResult(serviceResult);
			getRooms.PostExecute();
			return serviceResult;
		}

		private ServiceResult<EwsRoomType[]> GetAllAvailableRooms()
		{
			UserContext userContext = UserContextManager.GetUserContext(base.CallContext.HttpContext, base.CallContext.EffectiveCaller, true);
			QueryFilter filter = null;
			AddressLists addressLists = new AddressLists(base.CallContext.EffectiveCaller.ClientSecurityContext, base.MailboxIdentityMailboxSession.MailboxOwner, userContext);
			ADSessionSettings sessionSettings;
			if (addressLists.AllRoomsAddressList == null)
			{
				filter = new OrFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientDisplayType, RecipientDisplayType.ConferenceRoomMailbox),
					new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientDisplayType, RecipientDisplayType.SyncedConferenceRoomMailbox)
				});
				sessionSettings = ADSessionSettings.FromOrganizationIdWithAddressListScopeServiceOnly(addressLists.GlobalAddressList.OrganizationId, addressLists.GlobalAddressList.Id);
			}
			else
			{
				sessionSettings = ADSessionSettings.FromOrganizationIdWithAddressListScopeServiceOnly(addressLists.AllRoomsAddressList.OrganizationId, addressLists.AllRoomsAddressList.Id);
			}
			int lcid = 0;
			CultureInfo cultureInfo = base.CallContext.AccessingPrincipal.PreferredCultures.FirstOrDefault<CultureInfo>();
			if (cultureInfo != null)
			{
				lcid = cultureInfo.LCID;
			}
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, null, lcid, true, ConsistencyMode.IgnoreInvalid, null, sessionSettings, 123, "GetAllAvailableRooms", "f:\\15.00.1497\\sources\\dev\\clients\\src\\Owa2\\Server\\Core\\ServiceCommands\\GetRoomsInternal.cs");
			ADRecipient[] rooms = tenantOrRootOrgRecipientSession.Find(null, QueryScope.SubTree, filter, new SortBy(ADRecipientSchema.DisplayName, SortOrder.Ascending), 100);
			return new ServiceResult<EwsRoomType[]>(GetRooms.GetRoomTypes(rooms, this.GetHashCode()).ToArray());
		}

		private const int MaxNumberOfRoomsToReturn = 100;

		private readonly string roomList;
	}
}
