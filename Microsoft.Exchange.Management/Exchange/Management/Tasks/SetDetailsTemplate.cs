using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Set", "DetailsTemplate", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetDetailsTemplate : SetSystemConfigurationObjectTask<DetailsTemplateIdParameter, DetailsTemplate>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetDetailsTemplate(this.Identity.ToString());
			}
		}

		protected override void InternalValidate()
		{
			DetailsTemplate instance = this.Instance;
			instance[DetailsTemplateSchema.Pages] = instance.Pages;
			if (instance.TemplateType.Equals("Mailbox Agent"))
			{
				base.WriteError(new InvalidOperationException(Strings.DetailsTemplateMailboxAgent), ErrorCategory.InvalidData, null);
			}
			base.InternalValidate();
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
			base.StampChangesOn(dataObject);
			DetailsTemplate detailsTemplate = (DetailsTemplate)dataObject;
			if (detailsTemplate.IsModified(DetailsTemplateSchema.Pages))
			{
				detailsTemplate.PagesToBlob();
			}
		}

		protected override IConfigurable ResolveDataObject()
		{
			DetailsTemplate detailsTemplate = (DetailsTemplate)base.ResolveDataObject();
			detailsTemplate.MAPIPropertiesDictionary = MAPIPropertiesDictionaryFactory.GetPropertiesDictionary();
			detailsTemplate.BlobToPages();
			return detailsTemplate;
		}
	}
}
