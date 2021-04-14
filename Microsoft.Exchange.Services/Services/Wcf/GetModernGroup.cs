using System;
using System.Linq;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.GroupMailbox;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.FederatedDirectory;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Wcf.Types;
using Microsoft.Exchange.UnifiedGroups;

namespace Microsoft.Exchange.Services.Wcf
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class GetModernGroup : ServiceCommand<GetModernGroupResponse>
	{
		public GetModernGroup(CallContext context, GetModernGroupRequest request) : base(context)
		{
			this.request = request;
			OwsLogRegistry.Register(GetModernGroup.GetModernGroupActionName, typeof(GetModernGroupMetadata), new Type[0]);
			request.ValidateRequest();
			WarmupGroupManagementDependency.WarmUpAsyncIfRequired(base.CallContext.AccessingPrincipal);
		}

		private IRecipientSession ADSession
		{
			get
			{
				return base.CallContext.ADRecipientSessionContext.GetADRecipientSession();
			}
		}

		private UserMailboxLocator UserMailboxLocator
		{
			get
			{
				if (this.userMailboxLocator == null)
				{
					this.userMailboxLocator = UserMailboxLocator.Instantiate(this.ADSession, base.CallContext.AccessingADUser);
				}
				return this.userMailboxLocator;
			}
		}

		private GroupMailboxLocator GroupMailboxLocator
		{
			get
			{
				if (this.groupMailboxLocator == null)
				{
					this.groupMailboxLocator = GroupMailboxLocator.Instantiate(this.ADSession, this.request.ProxyAddress);
				}
				return this.groupMailboxLocator;
			}
		}

		private GroupMailbox GroupMailbox
		{
			get
			{
				if (this.groupMailbox == null)
				{
					this.groupMailbox = this.GetGroup();
				}
				return this.groupMailbox;
			}
		}

		protected override GetModernGroupResponse InternalExecute()
		{
			GetModernGroupResponse getModernGroupResponse = new GetModernGroupResponse();
			if (this.request.IsGeneralInfoRequested)
			{
				base.CallContext.ProtocolLog.Set(GetModernGroupMetadata.GeneralInfo, ExtensibleLogger.FormatPIIValue(this.request.SmtpAddress));
				getModernGroupResponse.GeneralInfo = this.GetGeneralInfo();
			}
			if (this.request.IsMemberRequested || this.request.IsOwnerListRequested)
			{
				GetGroupMembers getGroupMembers = new GetGroupMembers(this.ADSession, base.CallContext.AccessingADUser.OrganizationId, this.GroupMailboxLocator, StoreSessionCacheBase.BuildMapiApplicationId(base.CallContext, null), this.request.MemberSortOrder, (this.request.MembersPageRequest != null) ? this.request.MembersPageRequest.MaxRows : 100, this.request.SerializedPeopleIKnowGraph, base.CallContext.ProtocolLog);
				if (this.request.IsOwnerListRequested)
				{
					base.CallContext.ProtocolLog.Set(GetModernGroupMetadata.OwnerList, ExtensibleLogger.FormatPIIValue(this.request.SmtpAddress));
					getModernGroupResponse.OwnerList = getGroupMembers.GetOwners();
				}
				if (this.request.IsMemberRequested)
				{
					getModernGroupResponse.MembersInfo = getGroupMembers.GetMembers();
				}
			}
			if (this.request.IsExternalResourcesRequested)
			{
				getModernGroupResponse.ExternalResources = this.GetExternalResources();
			}
			if (this.request.IsMailboxInfoRequested)
			{
				getModernGroupResponse.MailboxProperties = this.GetGroupProperties();
			}
			return getModernGroupResponse;
		}

		private ModernGroupExternalResources GetExternalResources()
		{
			SharePointUrlResolver sharePointUrlResolver = new SharePointUrlResolver(this.GroupMailboxLocator.FindAdUser());
			return new ModernGroupExternalResources
			{
				SharePointUrl = sharePointUrlResolver.GetSiteUrl(),
				DocumentsUrl = sharePointUrlResolver.GetDocumentsUrl()
			};
		}

		private GroupMailboxProperties GetGroupProperties()
		{
			int subscribersCount = 100;
			this.ExecuteGroupMailboxAction("GetEscalatedMembers", delegate(GroupMailboxAccessLayer accessLayer)
			{
				subscribersCount = accessLayer.GetEscalatedMembers(this.GroupMailboxLocator, false).ToArray<UserMailbox>().Count<UserMailbox>();
			});
			return new GroupMailboxProperties
			{
				SubscribersCount = subscribersCount,
				CanUpdateAutoSubscribeFlag = (subscribersCount < 100),
				LanguageLCID = ((this.GroupMailbox.Language != null) ? this.GroupMailbox.Language.LCID : -1)
			};
		}

		private ModernGroupGeneralInfoResponse GetGeneralInfo()
		{
			string description = this.GroupMailbox.Description;
			UserMailbox member = this.GetMember();
			bool flag = member != null && member.IsMember;
			return new ModernGroupGeneralInfoResponse
			{
				Description = description,
				IsMember = flag,
				IsOwner = (flag && member.IsOwner),
				ModernGroupType = this.GroupMailbox.Type,
				Name = this.GroupMailbox.DisplayName,
				OwnersCount = this.GroupMailbox.Owners.Count,
				ShouldEscalate = (flag && member.ShouldEscalate),
				SmtpAddress = this.GroupMailbox.SmtpAddress.ToString(),
				RequireSenderAuthenticationEnabled = this.GroupMailbox.RequireSenderAuthenticationEnabled,
				AutoSubscribeNewGroupMembers = this.GroupMailbox.AutoSubscribeNewGroupMembers
			};
		}

		private UserMailbox GetMember()
		{
			UserMailbox user = null;
			this.ExecuteGroupMailboxAction("GetModernGroupMember", delegate(GroupMailboxAccessLayer accessLayer)
			{
				user = accessLayer.GetMember(this.GroupMailboxLocator, this.UserMailboxLocator, true);
			});
			return user;
		}

		private GroupMailbox GetGroup()
		{
			GroupMailbox group = null;
			this.ExecuteGroupMailboxAction("GetModernGroupDetails", delegate(GroupMailboxAccessLayer accessLayer)
			{
				group = accessLayer.GetGroupMailbox(this.GroupMailboxLocator, this.UserMailboxLocator, true);
			});
			return group;
		}

		private void ExecuteGroupMailboxAction(string operationDescription, Action<GroupMailboxAccessLayer> action)
		{
			GroupMailboxAccessLayer.Execute(operationDescription, this.ADSession, this.GroupMailboxLocator.MailboxGuid, base.CallContext.AccessingADUser.OrganizationId, StoreSessionCacheBase.BuildMapiApplicationId(base.CallContext, null), action);
		}

		private const int MaxMembers = 100;

		private static readonly string GetModernGroupActionName = typeof(GetModernGroup).Name;

		private readonly GetModernGroupRequest request;

		private GroupMailbox groupMailbox;

		private UserMailboxLocator userMailboxLocator;

		private GroupMailboxLocator groupMailboxLocator;
	}
}
