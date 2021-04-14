using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("New", "Mailbox", SupportsShouldProcess = true, DefaultParameterSetName = "User")]
	public sealed class NewMailbox : NewMailboxOrSyncMailbox
	{
		protected override void WriteResult(ADObject result)
		{
			TaskLogger.LogEnter(new object[]
			{
				result.Identity
			});
			ADUser aduser = (ADUser)result;
			if (null != aduser.MasterAccountSid)
			{
				aduser.LinkedMasterAccount = SecurityPrincipalIdParameter.GetFriendlyUserName(aduser.MasterAccountSid, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
				aduser.ResetChangeTracking();
			}
			Mailbox result2 = new Mailbox(aduser);
			base.WriteResult(result2);
			TaskLogger.LogExit();
		}

		public NewMailbox()
		{
			base.NumberofCalls = ProvisioningCounters.NumberOfNewMailboxCalls;
			base.NumberofSuccessfulCalls = ProvisioningCounters.NumberOfSuccessfulNewMailboxCalls;
			base.AverageTimeTaken = ProvisioningCounters.AverageNewMailboxResponseTime;
			base.AverageBaseTimeTaken = ProvisioningCounters.AverageNewMailboxResponseTimeBase;
			base.AverageTimeTakenWithCache = ProvisioningCounters.AverageNewMailboxResponseTimeWithCache;
			base.AverageBaseTimeTakenWithCache = ProvisioningCounters.AverageNewMailboxResponseTimeBaseWithCache;
			base.AverageTimeTakenWithoutCache = ProvisioningCounters.AverageNewMailboxResponseTimeWithoutCache;
			base.AverageBaseTimeTakenWithoutCache = ProvisioningCounters.AverageNewMailboxResponseTimeBaseWithoutCache;
			base.TotalResponseTime = ProvisioningCounters.TotalNewMailboxResponseTime;
			base.CacheActivePercentage = ProvisioningCounters.NewMailboxCacheActivePercentage;
			base.CacheActiveBasePercentage = ProvisioningCounters.NewMailboxCacheActivePercentageBase;
		}

		[Parameter(Mandatory = false, ParameterSetName = "Room")]
		[Parameter(Mandatory = false, ParameterSetName = "LinkedRoomMailbox")]
		public string Office
		{
			get
			{
				return this.DataObject.Office;
			}
			set
			{
				this.DataObject.Office = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Room")]
		[Parameter(Mandatory = false, ParameterSetName = "LinkedRoomMailbox")]
		public string Phone
		{
			get
			{
				return this.DataObject.Phone;
			}
			set
			{
				this.DataObject.Phone = value;
			}
		}

		[Parameter(Mandatory = false)]
		public NetID OriginalNetID
		{
			get
			{
				return this.DataObject.OriginalNetID;
			}
			set
			{
				this.DataObject.OriginalNetID = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Room")]
		[Parameter(Mandatory = false, ParameterSetName = "LinkedRoomMailbox")]
		public int? ResourceCapacity
		{
			get
			{
				return this.DataObject.ResourceCapacity;
			}
			set
			{
				this.DataObject.ResourceCapacity = value;
			}
		}
	}
}
