using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Get", "MailboxPlan", DefaultParameterSetName = "Identity")]
	public sealed class GetMailboxPlan : GetMailboxBase<MailboxPlanIdParameter>
	{
		[Parameter(Mandatory = false)]
		public SwitchParameter AllMailboxPlanReleases
		{
			get
			{
				return (SwitchParameter)(base.Fields["AllMailboxPlanReleases"] ?? false);
			}
			set
			{
				base.Fields["AllMailboxPlanReleases"] = value;
			}
		}

		protected override RecipientTypeDetails[] InternalRecipientTypeDetails
		{
			get
			{
				return GetMailboxPlan.AllowedRecipientTypeDetails;
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				if (this.Identity != null || this.AllMailboxPlanReleases)
				{
					return base.InternalFilter;
				}
				return QueryFilter.AndTogether(new QueryFilter[]
				{
					base.InternalFilter,
					new NotFilter(new BitMaskAndFilter(ADRecipientSchema.ProvisioningFlags, 32UL))
				});
			}
		}

		internal override ObjectSchema FilterableObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<MailboxPlanSchema>();
			}
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			ADUser aduser = (ADUser)dataObject;
			if (null != aduser.MasterAccountSid)
			{
				aduser.LinkedMasterAccount = SecurityPrincipalIdParameter.GetFriendlyUserName(aduser.MasterAccountSid, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
				aduser.ResetChangeTracking();
			}
			MailboxPlan mailboxPlan = new MailboxPlan(aduser);
			mailboxPlan.Database = ADObjectIdResolutionHelper.ResolveDN(mailboxPlan.Database);
			return mailboxPlan;
		}

		internal new string Anr
		{
			get
			{
				return null;
			}
		}

		private static readonly RecipientTypeDetails[] AllowedRecipientTypeDetails = new RecipientTypeDetails[]
		{
			RecipientTypeDetails.MailboxPlan
		};
	}
}
