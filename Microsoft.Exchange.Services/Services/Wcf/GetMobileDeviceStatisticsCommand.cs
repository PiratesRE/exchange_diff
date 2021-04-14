using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class GetMobileDeviceStatisticsCommand : SingleCmdletCommandBase<GetMobileDeviceStatisticsRequest, GetMobileDeviceStatisticsResponse, GetMobileDeviceStatistics, MobileDeviceConfiguration>
	{
		public GetMobileDeviceStatisticsCommand(CallContext callContext, GetMobileDeviceStatisticsRequest request) : base(callContext, request, "Get-MobileDeviceStatistics", ScopeLocation.RecipientRead)
		{
		}

		protected override void PopulateTaskParameters()
		{
			GetMobileDeviceStatistics task = this.cmdletRunner.TaskWrapper.Task;
			MobileDeviceStatisticsOptions options = this.request.Options;
			this.cmdletRunner.SetTaskParameter("Mailbox", task, new MailboxIdParameter(base.CallContext.AccessingPrincipal.ObjectId));
			this.cmdletRunner.SetTaskParameterIfModified("ActiveSync", this.request.Options, task, new SwitchParameter(options.ActiveSync));
			this.cmdletRunner.SetTaskParameterIfModified("GetMailboxLog", this.request.Options, task, new SwitchParameter(options.GetMailboxLog));
			this.cmdletRunner.SetTaskParameterIfModified("ShowRecoveryPassword", this.request.Options, task, new SwitchParameter(options.ShowRecoveryPassword));
		}

		protected override void PopulateResponseData(GetMobileDeviceStatisticsResponse response)
		{
			IEnumerable<MobileDeviceConfiguration> allResults = this.cmdletRunner.TaskWrapper.AllResults;
			if (allResults == null)
			{
				response.MobileDeviceStatisticsCollection = null;
				return;
			}
			IEnumerable<MobileDeviceStatistics> source = from result in allResults
			select new MobileDeviceStatistics
			{
				FirstSyncTime = this.GetFormattedDate(result.FirstSyncTime),
				DeviceType = result.DeviceType,
				DeviceId = result.DeviceID,
				DeviceUserAgent = result.DeviceUserAgent,
				DeviceModel = result.DeviceModel,
				DeviceImei = result.DeviceImei,
				DeviceOS = result.DeviceOS,
				DeviceOSLanguage = result.DeviceOSLanguage,
				DevicePhoneNumber = result.DevicePhoneNumber,
				DeviceAccessState = result.DeviceAccessState,
				DeviceAccessStateReason = result.DeviceAccessStateReason,
				DeviceAccessControlRule = result.DeviceAccessControlRule.ToIdentity((result.DeviceAccessControlRule == null) ? null : result.DeviceAccessControlRule.ToString()),
				ClientVersion = result.ClientVersion,
				ClientType = result.ClientType,
				DeviceMobileOperator = result.DeviceMobileOperator,
				DeviceFriendlyName = result.DeviceFriendlyName,
				LastPolicyUpdateTime = this.GetFormattedDate(result.LastPolicyUpdateTime),
				LastSyncAttemptTime = this.GetFormattedDate(result.LastSyncAttemptTime),
				LastSuccessSync = this.GetFormattedDate(result.LastSuccessSync),
				DeviceWipeSentTime = this.GetFormattedDate(result.DeviceWipeSentTime),
				DeviceWipeRequestTime = this.GetFormattedDate(result.DeviceWipeRequestTime),
				DeviceWipeAckTime = this.GetFormattedDate(result.DeviceWipeAckTime),
				LastPingHeartBeat = result.LastPingHeartbeat,
				RecoveryPassword = result.RecoveryPassword,
				Identity = new Identity((result.Identity != null) ? result.Identity.ToString() : null),
				IsRemoteWipeSupported = result.IsRemoteWipeSupported,
				Status = result.Status,
				StatusNote = result.StatusNote,
				DevicePolicyApplied = result.DevicePolicyApplied.ToIdentity((result.DevicePolicyApplied != null) ? result.DevicePolicyApplied.ToString() : null),
				NumberOfFoldersSynced = result.NumberOfFoldersSynced,
				DevicePolicyApplicationStatus = result.DevicePolicyApplicationStatus
			};
			response.MobileDeviceStatisticsCollection.MobileDevices = source.ToArray<MobileDeviceStatistics>();
		}

		protected override PSLocalTask<GetMobileDeviceStatistics, MobileDeviceConfiguration> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateGetMobileDeviceStatisticsTask(base.CallContext.AccessingPrincipal);
		}

		private string GetFormattedDate(DateTime? date)
		{
			if (date == null)
			{
				return null;
			}
			return ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime((ExDateTime)date.Value);
		}
	}
}
