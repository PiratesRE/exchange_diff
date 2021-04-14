using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessageSecurity.MessageClassifications;

namespace Microsoft.Exchange.Management.SecureMail
{
	[Cmdlet("Install", "SystemMessageClassification", SupportsShouldProcess = true)]
	public sealed class InstallSystemMessageClassification : InstallContainerTaskBase<MessageClassification>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageInstallSystemMessageClassification;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				foreach (SystemClassificationSummary classificationSummary in SystemClassificationSummary.DefaultClassifications)
				{
					MessageClassification messageClassification = new MessageClassification();
					messageClassification.Name = classificationSummary.Name;
					messageClassification.ClassificationID = classificationSummary.ClassificationID;
					messageClassification.DisplayName = classificationSummary.DisplayName;
					messageClassification.SenderDescription = classificationSummary.SenderDescription;
					messageClassification.RecipientDescription = classificationSummary.RecipientDescription;
					messageClassification.DisplayPrecedence = classificationSummary.DisplayPrecedence;
					messageClassification.PermissionMenuVisible = classificationSummary.PermissionMenuVisible;
					messageClassification.RetainClassificationEnabled = classificationSummary.RetainClassificationEnabled;
					messageClassification.OrganizationId = OrganizationId.ForestWideOrgId;
					messageClassification.SetId((IConfigurationSession)base.DataSession, InstallSystemMessageClassification.DefaultContainer, classificationSummary.Name);
					try
					{
						base.DataSession.Save(messageClassification);
					}
					catch (ADObjectAlreadyExistsException)
					{
					}
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		private static readonly ADObjectId DefaultContainer = new ADObjectId("CN=Default");
	}
}
