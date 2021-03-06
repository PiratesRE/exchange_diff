using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("set", "ManagedFolderMailboxPolicy", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetManagedFolderMailboxPolicy : SetMailboxPolicyBase<ManagedFolderMailboxPolicy>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetManagedFolderMailboxPolicy(this.Identity.ToString());
			}
		}

		[Parameter(Mandatory = false)]
		public ELCFolderIdParameter[] ManagedFolderLinks
		{
			get
			{
				return (ELCFolderIdParameter[])base.Fields["ManagedFolderLinks"];
			}
			set
			{
				base.Fields["ManagedFolderLinks"] = value;
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (base.Fields.IsModified("ManagedFolderLinks"))
			{
				this.DataObject.ManagedFolderLinks.Clear();
				if (this.ManagedFolderLinks != null)
				{
					foreach (ELCFolderIdParameter elcfolderIdParameter in this.ManagedFolderLinks)
					{
						IConfigurable dataObject = base.GetDataObject<ELCFolder>(elcfolderIdParameter, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorElcFolderNotFound(elcfolderIdParameter.ToString())), new LocalizedString?(Strings.ErrorAmbiguousElcFolderId(elcfolderIdParameter.ToString())));
						IConfigurationSession session = base.DataSession as IConfigurationSession;
						ValidationError validationError = this.DataObject.AddManagedFolderToPolicy(session, (ELCFolder)dataObject);
						if (validationError != null)
						{
							base.WriteError(new InvalidOperationException(validationError.Description), ErrorCategory.InvalidOperation, null);
						}
					}
				}
			}
		}
	}
}
