using System;
using System.Linq;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Diagnostics.Components.GroupMailbox;
using Microsoft.Exchange.Diagnostics.Performance;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class UpdateGroupMailboxViaEWS : UpdateGroupMailboxBase
	{
		public UpdateGroupMailboxViaEWS(ADUser group, ADUser executingUser, Uri endpointUrl, GroupMailboxConfigurationActionType forceActionMask, ADUser[] addedMembers, ADUser[] removedMembers, int? permissionsVersion) : base(group, executingUser, forceActionMask, permissionsVersion)
		{
			this.endpointUrl = endpointUrl;
			this.addedMembers = addedMembers;
			this.removedMembers = removedMembers;
		}

		private string ExecutingUserSmtpAddress
		{
			get
			{
				if (this.executingUser == null)
				{
					return null;
				}
				return this.executingUser.PrimarySmtpAddress.ToString();
			}
		}

		public override void Execute()
		{
			using (UpdateGroupMailboxEwsBinding updateGroupMailboxEwsBinding = this.CreateUpdateGroupMailboxEwsBinding())
			{
				UpdateGroupMailboxType request = this.CreateUpdateGroupMailboxType();
				UpdateGroupMailboxResponseType response = updateGroupMailboxEwsBinding.ExecuteUpdateGroupMailboxWithRetry(request);
				this.ProcessResponse(response);
			}
		}

		private static string[] GetUserSmtpAddresses(ADUser[] users)
		{
			if (users == null)
			{
				return null;
			}
			return (from user in users
			select user.PrimarySmtpAddress.ToString()).ToArray<string>();
		}

		private UpdateGroupMailboxEwsBinding CreateUpdateGroupMailboxEwsBinding()
		{
			UpdateGroupMailboxEwsBinding result;
			using (new StopwatchPerformanceTracker("UpdateGroupMailboxViaEWS.CreateUpdateGroupMailboxEwsBinding", GenericCmdletInfoDataLogger.Instance))
			{
				result = new UpdateGroupMailboxEwsBinding(this.group, this.endpointUrl);
			}
			return result;
		}

		private UpdateGroupMailboxType CreateUpdateGroupMailboxType()
		{
			return new UpdateGroupMailboxType
			{
				GroupSmtpAddress = this.group.PrimarySmtpAddress.ToString(),
				DomainController = this.group.OriginatingServer,
				ExecutingUserSmtpAddress = this.ExecutingUserSmtpAddress,
				ForceConfigurationAction = this.forceActionMask,
				MemberIdentifierType = GroupMemberIdentifierType.SmtpAddress,
				MemberIdentifierTypeSpecified = true,
				AddedMembers = UpdateGroupMailboxViaEWS.GetUserSmtpAddresses(this.addedMembers),
				RemovedMembers = UpdateGroupMailboxViaEWS.GetUserSmtpAddresses(this.removedMembers),
				PermissionsVersionSpecified = (this.permissionsVersion != null),
				PermissionsVersion = ((this.permissionsVersion != null) ? this.permissionsVersion.Value : 0)
			};
		}

		private void ProcessResponse(UpdateGroupMailboxResponseType response)
		{
			using (new StopwatchPerformanceTracker("UpdateGroupMailboxViaEWS.ProcessResponse", GenericCmdletInfoDataLogger.Instance))
			{
				if (response == null || response.ResponseMessages == null || response.ResponseMessages.Items == null || response.ResponseMessages.Items.Length == 0)
				{
					UpdateGroupMailboxViaEWS.Tracer.TraceError((long)this.GetHashCode(), "Empty Response");
					base.Error = "Empty Response";
				}
				else
				{
					ResponseMessageType responseMessageType = response.ResponseMessages.Items[0];
					if (responseMessageType.ResponseClass == ResponseClassType.Success)
					{
						UpdateGroupMailboxViaEWS.Tracer.TraceDebug<string, string>((long)this.GetHashCode(), "UpdateGroupMailbox succeeded. Group={0}, MessageText={1}", this.group.ExternalDirectoryObjectId, responseMessageType.MessageText);
					}
					else
					{
						UpdateGroupMailboxViaEWS.Tracer.TraceError<ResponseClassType, ResponseCodeType, string>((long)this.GetHashCode(), "UpdateGroupMailbox failed. ResponseClass={0}, ResponseCode={1}, MessageText={2}", responseMessageType.ResponseClass, responseMessageType.ResponseCode, responseMessageType.MessageText);
						base.Error = responseMessageType.MessageText;
						base.ResponseCode = new ResponseCodeType?(responseMessageType.ResponseCode);
					}
				}
			}
		}

		private static readonly Trace Tracer = ExTraceGlobals.CmdletsTracer;

		private readonly Uri endpointUrl;

		private readonly ADUser[] addedMembers;

		private readonly ADUser[] removedMembers;
	}
}
