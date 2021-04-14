using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.MessagingPolicies.AttachFilter
{
	[Cmdlet("remove", "attachmentfilterentry", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public class RemoveAttachmentFilterEntry : SingletonSystemConfigurationObjectActionTask<AttachmentFilteringConfig>
	{
		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		public string Identity
		{
			get
			{
				return (string)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				return ((IConfigurationSession)base.DataSession).GetOrgContainerId();
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveAttachmentfilterentry(this.Identity.ToString());
			}
		}

		protected override void InternalProcessRecord()
		{
			AttachmentFilteringConfig attachmentFilteringConfig = null;
			try
			{
				attachmentFilteringConfig = AFilterUtils.GetAFilterConfig(base.DataSession);
			}
			catch (AttachmentFilterADEntryNotFoundException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidArgument, null);
				return;
			}
			string identity = this.Identity;
			string[] array = attachmentFilteringConfig.AttachmentNames.ToArray();
			foreach (string text in array)
			{
				if (text.Equals(identity, StringComparison.InvariantCultureIgnoreCase))
				{
					attachmentFilteringConfig.AttachmentNames.Remove(text);
					base.DataSession.Save(attachmentFilteringConfig);
					return;
				}
			}
			base.WriteError(new ArgumentException(Strings.AttachmentFilterEntryNotFound, "AttachmentFilterEntry"), ErrorCategory.InvalidArgument, null);
		}
	}
}
