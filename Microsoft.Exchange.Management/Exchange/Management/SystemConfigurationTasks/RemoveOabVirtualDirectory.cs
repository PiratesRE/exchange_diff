using System;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "OabVirtualDirectory", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveOabVirtualDirectory : RemoveExchangeVirtualDirectory<ADOabVirtualDirectory>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveOabVirtualDirectory(this.Identity.ToString());
			}
		}

		[Parameter]
		public SwitchParameter Force
		{
			get
			{
				return (SwitchParameter)(base.Fields["Force"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Force"] = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				if (!this.Force)
				{
					OfflineAddressBook[] array = this.ConfigurationSession.FindOABsForWebDistributionPoint(base.DataObject);
					if (array != null && array.Length > 0)
					{
						StringBuilder stringBuilder = new StringBuilder();
						stringBuilder.Append(array[0].Name);
						for (int i = 1; i < array.Length; i++)
						{
							stringBuilder.Append(", ");
							stringBuilder.Append(array[i].Name);
						}
						if (!base.ShouldContinue(Strings.RemoveNonEmptyOabVirtualDirectory(this.Identity.ToString(), stringBuilder.ToString())))
						{
							return;
						}
					}
				}
			}
			catch (DataSourceTransientException ex)
			{
				TaskLogger.Trace("The action of quary offline address books associcated with this virtual directory raised exception {0}", new object[]
				{
					ex.ToString()
				});
			}
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}
	}
}
