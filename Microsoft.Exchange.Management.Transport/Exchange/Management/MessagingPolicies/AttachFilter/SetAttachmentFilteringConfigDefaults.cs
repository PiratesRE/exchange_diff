using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.MessagingPolicies.AttachFilter
{
	[Cmdlet("set", "attachmentfilteringconfigdefaults")]
	public class SetAttachmentFilteringConfigDefaults : SetSingletonSystemConfigurationObjectTask<AttachmentFilteringConfig>
	{
		protected override ObjectId RootId
		{
			get
			{
				return ((IConfigurationSession)base.DataSession).GetOrgContainerId();
			}
		}

		protected override void InternalProcessRecord()
		{
			this.DataObject.AdminMessage = DirectoryStrings.AttachmentsWereRemovedMessage;
			this.DataObject.RejectResponse = "Message rejected due to unacceptable attachments";
			base.InternalProcessRecord();
		}
	}
}
