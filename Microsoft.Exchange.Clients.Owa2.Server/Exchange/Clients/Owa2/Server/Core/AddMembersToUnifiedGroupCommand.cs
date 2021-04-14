using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.FederatedDirectory;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class AddMembersToUnifiedGroupCommand : ServiceCommand<AddMembersToUnifiedGroupResponse>
	{
		public AddMembersToUnifiedGroupCommand(CallContext callContext, AddMembersToUnifiedGroupRequest request) : base(callContext)
		{
			WcfServiceCommandBase.ThrowIfNull(request, "AddMembersToUnifiedGroupRequest", "AddMembersToUnifiedGroupCommand::AddMembersToUnifiedGroupCommand");
			this.request = request;
			this.request.Validate();
			OwsLogRegistry.Register(OwaApplication.GetRequestDetailsLogger.Get(ExtensibleLoggerMetadata.EventId), typeof(CreateModernGroupCommandMetadata), new Type[0]);
		}

		protected override AddMembersToUnifiedGroupResponse InternalExecute()
		{
			AddMembersToUnifiedGroupResponse addMembersToUnifiedGroupResponse = new AddMembersToUnifiedGroupResponse();
			Stopwatch stopwatch = new Stopwatch();
			try
			{
				stopwatch.Start();
				UpdateUnifiedGroupTask updateUnifiedGroupTask = new UpdateUnifiedGroupTask(base.CallContext.AccessingADUser, base.CallContext.AccessingPrincipal, base.CallContext.ADRecipientSessionContext.GetADRecipientSession());
				updateUnifiedGroupTask.ExternalDirectoryObjectId = this.request.ExternalDirectoryObjectId.ToString();
				updateUnifiedGroupTask.AddedMembers = this.request.AddedMembers;
				base.CallContext.ProtocolLog.Set(CreateModernGroupCommandMetadata.MemberCount, this.request.AddedMembers.Length);
				if (!updateUnifiedGroupTask.Run())
				{
					base.CallContext.ProtocolLog.Set(CreateModernGroupCommandMetadata.Exception, updateUnifiedGroupTask.ErrorException);
					base.CallContext.ProtocolLog.Set(CreateModernGroupCommandMetadata.ExceptionType, updateUnifiedGroupTask.ErrorException.GetType());
					base.CallContext.ProtocolLog.Set(CreateModernGroupCommandMetadata.ErrorAction, updateUnifiedGroupTask.ErrorAction);
					if (!string.IsNullOrEmpty(updateUnifiedGroupTask.ErrorCode))
					{
						base.CallContext.ProtocolLog.Set(CreateModernGroupCommandMetadata.ErrorCode, updateUnifiedGroupTask.ErrorCode);
					}
					UnifiedGroupsTask.UnifiedGroupsAction errorAction = updateUnifiedGroupTask.ErrorAction;
					if (errorAction == UnifiedGroupsTask.UnifiedGroupsAction.AADUpdate)
					{
						throw new InternalServerErrorException(updateUnifiedGroupTask.ErrorException);
					}
					if (errorAction == UnifiedGroupsTask.UnifiedGroupsAction.ExchangeUpdate)
					{
						addMembersToUnifiedGroupResponse.ErrorState = UnifiedGroupResponseErrorState.FailedMailbox;
						addMembersToUnifiedGroupResponse.Error = updateUnifiedGroupTask.ErrorException.ToString();
						base.CallContext.ProtocolLog.Set(ServiceCommonMetadata.GenericErrors, updateUnifiedGroupTask.ErrorException);
					}
				}
			}
			finally
			{
				base.CallContext.ProtocolLog.Set(CreateModernGroupCommandMetadata.TotalProcessingTime, stopwatch.Elapsed.TotalSeconds.ToString("n2"));
			}
			return addMembersToUnifiedGroupResponse;
		}

		private AddMembersToUnifiedGroupRequest request;
	}
}
