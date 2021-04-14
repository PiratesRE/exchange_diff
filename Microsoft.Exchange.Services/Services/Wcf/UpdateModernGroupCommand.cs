using System;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.GroupMailbox;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.FederatedDirectory;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;
using Microsoft.Office.Server.Directory;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class UpdateModernGroupCommand : ServiceCommand<UpdateModernGroupResponse>
	{
		public UpdateModernGroupCommand(CallContext callContext, UpdateModernGroupRequest request) : base(callContext)
		{
			WcfServiceCommandBase.ThrowIfNull(request, "UpdateModernGroupRequest", "UpdateModernGroupCommand::UpdateModernGroupCommand");
			request.Validate();
			this.request = request;
		}

		protected override UpdateModernGroupResponse InternalExecute()
		{
			UpdateModernGroupResponse updateModernGroupResponse = new UpdateModernGroupResponse
			{
				ErrorState = UnifiedGroupResponseErrorState.NoError
			};
			ExTraceGlobals.ModernGroupsTracer.TraceDebug<string, SmtpAddress>((long)this.GetHashCode(), "UpdateModernGroupCommand.InternalExecute: Group: {0}, User: {1}.", this.request.SmtpAddress, base.CallContext.AccessingPrincipal.MailboxInfo.PrimarySmtpAddress);
			if (base.CallContext.FeaturesManager != null && base.CallContext.FeaturesManager.IsFeatureSupported("ModernGroupsNewArchitecture"))
			{
				UpdateUnifiedGroupTask updateUnifiedGroupTask = new UpdateUnifiedGroupTask(base.CallContext.AccessingADUser, base.CallContext.AccessingPrincipal, base.CallContext.ADRecipientSessionContext.GetADRecipientSession());
				updateUnifiedGroupTask.SmtpAddress = this.request.SmtpAddress;
				updateUnifiedGroupTask.DisplayName = this.request.Name;
				updateUnifiedGroupTask.Description = this.request.Description;
				updateUnifiedGroupTask.AddedOwners = this.request.AddedOwners;
				updateUnifiedGroupTask.RemovedOwners = this.request.DeletedOwners;
				updateUnifiedGroupTask.AddedMembers = this.request.AddedMembers;
				updateUnifiedGroupTask.RemovedMembers = this.request.DeletedMembers;
				updateUnifiedGroupTask.RequireSenderAuthenticationEnabled = this.request.RequireSenderAuthenticationEnabled;
				updateUnifiedGroupTask.Language = this.request.Language;
				if (this.request.AutoSubscribeNewGroupMembers != null)
				{
					IRecipientSession adrecipientSession = base.CallContext.ADRecipientSessionContext.GetADRecipientSession();
					GroupMailboxLocator groupMailboxLocator = GroupMailboxLocator.Instantiate(adrecipientSession, this.request.ProxyAddress);
					ADUser aduser = groupMailboxLocator.FindAdUser();
					bool value = this.request.AutoSubscribeNewGroupMembers.Value;
					if (value != aduser.AutoSubscribeNewGroupMembers)
					{
						updateUnifiedGroupTask.AutoSubscribeNewGroupMembers = new bool?(value && this.CanSetTheAutoSubscribeBit(adrecipientSession, groupMailboxLocator));
					}
				}
				if (!updateUnifiedGroupTask.Run())
				{
					ExTraceGlobals.ModernGroupsTracer.TraceError<UnifiedGroupsTask.UnifiedGroupsAction, Exception>((long)this.GetHashCode(), "UpdateModernGroupCommand.InternalExecute: UpdateUnifiedGroupTask.Run failed. ErrorAction: {0}, ErrorException: {1}", updateUnifiedGroupTask.ErrorAction, updateUnifiedGroupTask.ErrorException);
					if (updateUnifiedGroupTask.ErrorAction == UnifiedGroupsTask.UnifiedGroupsAction.AADUpdate)
					{
						throw new InternalServerErrorException(updateUnifiedGroupTask.ErrorException);
					}
					if (updateUnifiedGroupTask.ErrorAction == UnifiedGroupsTask.UnifiedGroupsAction.ExchangeUpdate)
					{
						updateModernGroupResponse.ErrorState = UnifiedGroupResponseErrorState.FailedMailbox;
						updateModernGroupResponse.Error = updateUnifiedGroupTask.ErrorException.ToString();
						base.CallContext.ProtocolLog.Set(ServiceCommonMetadata.GenericErrors, updateUnifiedGroupTask.ErrorException);
					}
				}
			}
			else
			{
				using (new CorrelationContext())
				{
					IdentityMapping identityMapping = new IdentityMapping(base.CallContext.ADRecipientSessionContext.GetADRecipientSession());
					identityMapping.Prefetch(new string[]
					{
						this.request.SmtpAddress
					});
					identityMapping.Prefetch(this.request.AddedMembers);
					identityMapping.Prefetch(this.request.DeletedMembers);
					identityMapping.Prefetch(this.request.AddedOwners);
					identityMapping.Prefetch(this.request.DeletedOwners);
					DirectorySession directorySession = FederatedDirectorySessionFactory.Create(base.CallContext.AccessingADUser, base.CallContext.AccessingPrincipal);
					RequestSchema requestSchema = new RequestSchema();
					Guid identityFromSmtpAddress = identityMapping.GetIdentityFromSmtpAddress(this.request.SmtpAddress);
					if (identityFromSmtpAddress == Guid.Empty)
					{
						ExTraceGlobals.ModernGroupsTracer.TraceError<string>((long)this.GetHashCode(), "UpdateModernGroupCommand.InternalExecute: no group found with SMTP address: {0}.", this.request.SmtpAddress);
						throw new ObjectNotFoundException(ServerStrings.ExItemNotFound);
					}
					Group group = directorySession.GetGroup(identityFromSmtpAddress, requestSchema);
					if (group == null)
					{
						ExTraceGlobals.ModernGroupsTracer.TraceError<Guid>((long)this.GetHashCode(), "UpdateModernGroupCommand.InternalExecute: no group found with ExternalDirectoryObjectId {0}", identityFromSmtpAddress);
						throw new ObjectNotFoundException(ServerStrings.ExItemNotFound);
					}
					if (this.request.Name != null)
					{
						group.DisplayName = this.request.Name;
					}
					if (this.request.Description != null)
					{
						group.Description = this.request.Description;
					}
					identityMapping.AddToRelation(this.request.AddedOwners, group.Owners);
					identityMapping.RemoveFromRelation(this.request.DeletedOwners, group.Owners);
					identityMapping.AddToRelation(this.request.AddedMembers, group.Members);
					identityMapping.RemoveFromRelation(this.request.DeletedMembers, group.Members);
					group.Commit();
				}
			}
			return updateModernGroupResponse;
		}

		private bool CanSetTheAutoSubscribeBit(IRecipientSession adSession, GroupMailboxLocator groupMailboxLocator)
		{
			bool canUpdateAutoSubscribe = false;
			GroupMailboxAccessLayer.Execute("GetPermissionToSetAutoSubscribe", adSession, groupMailboxLocator.MailboxGuid, base.CallContext.AccessingADUser.OrganizationId, StoreSessionCacheBase.BuildMapiApplicationId(base.CallContext, null), delegate(GroupMailboxAccessLayer accessLayer)
			{
				int num = accessLayer.GetEscalatedMembers(groupMailboxLocator, false).ToArray<UserMailbox>().Count<UserMailbox>();
				canUpdateAutoSubscribe = (num < 100);
			});
			return canUpdateAutoSubscribe;
		}

		private readonly UpdateModernGroupRequest request;
	}
}
