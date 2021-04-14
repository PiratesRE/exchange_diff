using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Install", "HostedOutboundSpamFilterPolicy")]
	public sealed class InstallHostedOutboundSpamFilterPolicy : NewMultitenancySystemConfigurationObjectTask<HostedOutboundSpamFilterPolicy>
	{
		[Parameter]
		public override SwitchParameter IgnoreDehydratedFlag { get; set; }

		[Parameter(Mandatory = true, Position = 0)]
		public new string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<SmtpAddress> NotifyOutboundSpamRecipients
		{
			get
			{
				return this.DataObject.NotifyOutboundSpamRecipients;
			}
			set
			{
				this.DataObject.NotifyOutboundSpamRecipients = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<SmtpAddress> BccSuspiciousOutboundAdditionalRecipients
		{
			get
			{
				return this.DataObject.BccSuspiciousOutboundAdditionalRecipients;
			}
			set
			{
				this.DataObject.BccSuspiciousOutboundAdditionalRecipients = value;
			}
		}

		[Parameter]
		public bool BccSuspiciousOutboundMail
		{
			get
			{
				return this.DataObject.BccSuspiciousOutboundMail;
			}
			set
			{
				this.DataObject.BccSuspiciousOutboundMail = value;
			}
		}

		[Parameter]
		public bool NotifyOutboundSpam
		{
			get
			{
				return this.DataObject.NotifyOutboundSpam;
			}
			set
			{
				this.DataObject.NotifyOutboundSpam = value;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			HostedOutboundSpamFilterPolicy hostedOutboundSpamFilterPolicy = (HostedOutboundSpamFilterPolicy)base.PrepareDataObject();
			hostedOutboundSpamFilterPolicy.SetId((IConfigurationSession)base.DataSession, this.Name);
			TaskLogger.LogExit();
			return hostedOutboundSpamFilterPolicy;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (SharedConfiguration.IsSharedConfiguration(this.DataObject.OrganizationId) && !base.ShouldContinue(Strings.ConfirmSharedConfiguration(this.DataObject.OrganizationId.OrganizationalUnit.Name)))
			{
				TaskLogger.LogExit();
				return;
			}
			base.CreateParentContainerIfNeeded(this.DataObject);
			base.InternalProcessRecord();
			FfoDualWriter.SaveToFfo<HostedOutboundSpamFilterPolicy>(this, this.DataObject, null);
			TaskLogger.LogExit();
		}
	}
}
