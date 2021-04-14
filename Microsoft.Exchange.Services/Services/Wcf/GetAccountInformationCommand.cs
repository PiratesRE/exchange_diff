using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Mapi;
using Microsoft.Exchange.Management.MapiTasks;
using Microsoft.Exchange.Management.MapiTasks.Presentation;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Management.RecipientTasks;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class GetAccountInformationCommand : OptionServiceCommandBase<GetAccountInformationRequest, GetAccountInformationResponse>
	{
		public GetAccountInformationCommand(CallContext callContext, GetAccountInformationRequest request) : base(callContext, request)
		{
		}

		protected override GetAccountInformationResponse CreateTaskAndExecute()
		{
			this.mailbox = base.CallContext.AccessingADUser;
			this.GetUserInfo();
			this.GetMailboxStatisticsInfo();
			return this.MergeResultsAndGenerateSuccessResponse();
		}

		private void GetUserInfo()
		{
			PSLocalTask<GetUser, User> pslocalTask = CmdletTaskFactory.Instance.CreateGetUserTask(base.CallContext.AccessingPrincipal);
			CmdletRunner<GetUser, User> cmdletRunner = new CmdletRunner<GetUser, User>(base.CallContext, "Get-User", ScopeLocation.RecipientRead, pslocalTask);
			cmdletRunner.SetTaskParameter("Identity", pslocalTask.Task, new UserIdParameter(base.CallContext.AccessingPrincipal.ObjectId));
			cmdletRunner.Execute();
			this.userInfo = cmdletRunner.TaskResult;
		}

		private void GetMailboxStatisticsInfo()
		{
			PSLocalTask<GetMailboxStatistics, Microsoft.Exchange.Management.MapiTasks.Presentation.MailboxStatistics> pslocalTask = CmdletTaskFactory.Instance.CreateGetMailboxStatisticsTask(base.CallContext.AccessingPrincipal);
			CmdletRunner<GetMailboxStatistics, Microsoft.Exchange.Management.MapiTasks.Presentation.MailboxStatistics> cmdletRunner = new CmdletRunner<GetMailboxStatistics, Microsoft.Exchange.Management.MapiTasks.Presentation.MailboxStatistics>(base.CallContext, "Get-MailboxStatistics", ScopeLocation.RecipientRead, pslocalTask);
			cmdletRunner.SetTaskParameter("Identity", pslocalTask.Task, new GeneralMailboxOrMailUserIdParameter(base.CallContext.AccessingPrincipal.ObjectId));
			cmdletRunner.Execute();
			this.statistics = cmdletRunner.TaskResult;
		}

		private GetAccountInformationResponse MergeResultsAndGenerateSuccessResponse()
		{
			GetAccountInformationResponse getAccountInformationResponse = new GetAccountInformationResponse();
			getAccountInformationResponse.AccountInfo.City = this.userInfo.City;
			getAccountInformationResponse.AccountInfo.CountryOrRegion = ((this.userInfo.CountryOrRegion == null) ? null : this.userInfo.CountryOrRegion.ToString());
			getAccountInformationResponse.AccountInfo.DisplayName = this.userInfo.DisplayName;
			getAccountInformationResponse.AccountInfo.Fax = this.userInfo.Fax;
			getAccountInformationResponse.AccountInfo.FirstName = this.userInfo.FirstName;
			getAccountInformationResponse.AccountInfo.HomePhone = this.userInfo.HomePhone;
			getAccountInformationResponse.AccountInfo.Initials = this.userInfo.Initials;
			getAccountInformationResponse.AccountInfo.LastName = this.userInfo.LastName;
			getAccountInformationResponse.AccountInfo.MobilePhone = this.userInfo.MobilePhone;
			getAccountInformationResponse.AccountInfo.Office = this.userInfo.Office;
			getAccountInformationResponse.AccountInfo.Phone = this.userInfo.Phone;
			getAccountInformationResponse.AccountInfo.PostalCode = this.userInfo.PostalCode;
			getAccountInformationResponse.AccountInfo.StateOrProvince = this.userInfo.StateOrProvince;
			getAccountInformationResponse.AccountInfo.StreetAddress = this.userInfo.StreetAddress;
			if (this.statistics == null)
			{
				getAccountInformationResponse.AccountInfo.Statistics = null;
			}
			else
			{
				getAccountInformationResponse.AccountInfo.Statistics = new MyAccountStatisticsData
				{
					DatabaseIssueWarningQuota = new UnlimitedUnsignedInteger(this.statistics.DatabaseIssueWarningQuota),
					DatabaseProhibitSendQuota = new UnlimitedUnsignedInteger(this.statistics.DatabaseProhibitSendQuota),
					DatabaseProhibitSendReceiveQuota = new UnlimitedUnsignedInteger(this.statistics.DatabaseProhibitSendReceiveQuota),
					StorageLimitStatus = (NullableStorageLimitStatus)((this.statistics.StorageLimitStatus == null) ? ((StorageLimitStatus)(-1)) : this.statistics.StorageLimitStatus.Value),
					TotalItemSize = new UnlimitedUnsignedInteger(this.statistics.TotalItemSize)
				};
			}
			if (this.mailbox == null)
			{
				getAccountInformationResponse.AccountInfo.Mailbox = null;
			}
			else
			{
				getAccountInformationResponse.AccountInfo.Mailbox = new MyAccountMailboxData
				{
					IssueWarningQuota = new UnlimitedUnsignedInteger(this.mailbox.IssueWarningQuota),
					ProhibitSendQuota = new UnlimitedUnsignedInteger(this.mailbox.ProhibitSendQuota),
					ProhibitSendReceiveQuota = new UnlimitedUnsignedInteger(this.mailbox.ProhibitSendReceiveQuota),
					UseDatabaseQuotaDefaults = (this.mailbox.UseDatabaseQuotaDefaults != null && this.mailbox.UseDatabaseQuotaDefaults.Value)
				};
			}
			getAccountInformationResponse.AccountInfo.CountryList = new CountryData[CountryInfo.AllCountryInfos.Count];
			int i = 0;
			int count = CountryInfo.AllCountryInfos.Count;
			while (i < count)
			{
				getAccountInformationResponse.AccountInfo.CountryList[i] = new CountryData
				{
					LocalizedDisplayName = CountryInfo.AllCountryInfos[i].LocalizedDisplayName,
					Name = CountryInfo.AllCountryInfos[i].Name
				};
				i++;
			}
			getAccountInformationResponse.WasSuccessful = true;
			return getAccountInformationResponse;
		}

		private User userInfo;

		private ADUser mailbox;

		private Microsoft.Exchange.Management.MapiTasks.Presentation.MailboxStatistics statistics;
	}
}
