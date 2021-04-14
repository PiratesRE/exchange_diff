using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.GroupMailbox;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class FindMembersInUnifiedGroupCommand : ServiceCommand<FindMembersInUnifiedGroupResponse>
	{
		public FindMembersInUnifiedGroupCommand(CallContext callContext, FindMembersInUnifiedGroupRequest request) : base(callContext)
		{
			ArgumentValidator.ThrowIfNull("request", request);
			this.request = request;
			this.request.Validate();
		}

		protected override FindMembersInUnifiedGroupResponse InternalExecute()
		{
			FindMembersInUnifiedGroupResponse findMembersInUnifiedGroupResponse = new FindMembersInUnifiedGroupResponse();
			ADRecipient adrecipient = base.CallContext.ADRecipientSessionContext.GetADRecipientSession().FindByProxyAddress(this.request.ProxyAddress);
			ADUser aduser = adrecipient as ADUser;
			if (adrecipient == null)
			{
				throw new InvalidCastException("Expected a recipient of ADUser type");
			}
			UnifiedGroupADAccessLayer unifiedGroupADAccessLayer = new UnifiedGroupADAccessLayer(aduser, aduser.OriginatingServer);
			if (this.request.Filter != null)
			{
				IEnumerable<UnifiedGroupParticipant> membersByAnrMatch = unifiedGroupADAccessLayer.GetMembersByAnrMatch(this.request.Filter.Trim().ToLower(), true, 101);
				ModernGroupMemberType[] array = this.ConvertListToMemberTypeArray(membersByAnrMatch);
				findMembersInUnifiedGroupResponse.HasMoreMembers = (array.Length > 100);
				Array.Resize<ModernGroupMemberType>(ref array, Math.Min(array.Length, 100));
				findMembersInUnifiedGroupResponse.Members = array;
			}
			return findMembersInUnifiedGroupResponse;
		}

		private ModernGroupMemberType[] ConvertListToMemberTypeArray(IEnumerable<UnifiedGroupParticipant> memberList)
		{
			UnifiedGroupParticipant[] array = memberList.ToArray<UnifiedGroupParticipant>();
			ModernGroupMemberType[] array2 = new ModernGroupMemberType[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = new ModernGroupMemberType
				{
					Persona = UnifiedGroupsHelper.UnifiedGroupParticipantToPersona(array[i]),
					IsOwner = array[i].IsOwner
				};
			}
			return array2;
		}

		private const int maxResults = 100;

		private FindMembersInUnifiedGroupRequest request;
	}
}
