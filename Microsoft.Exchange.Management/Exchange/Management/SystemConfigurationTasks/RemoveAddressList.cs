using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "AddressList", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveAddressList : RemoveAddressBookBase<AddressListIdParameter>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if (this.Recursive)
				{
					return Strings.ConfirmationMessageRemoveAddressListRecursively(this.Identity.ToString());
				}
				return Strings.ConfirmationMessageRemoveAddressList(this.Identity.ToString());
			}
		}

		[Parameter]
		public SwitchParameter Recursive
		{
			get
			{
				return (SwitchParameter)(base.Fields["Recursive"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Recursive"] = value;
			}
		}

		private void MoveToNextPercent()
		{
			this.currentProcent = 5 + this.currentProcent % 95;
		}

		private void ReportDeleteTreeProgress(ADTreeDeleteNotFinishedException de)
		{
			this.MoveToNextPercent();
			if (de != null)
			{
				base.WriteVerbose(de.LocalizedString);
			}
			else
			{
				base.WriteVerbose(Strings.ProgressStatusRemovingAddressListTree);
			}
			base.WriteProgress(Strings.ProgressActivityRemovingAddressListTree(base.DataObject.Id.ToString()), Strings.ProgressStatusRemovingAddressListTree, this.currentProcent);
		}

		protected override bool HandleRemoveWithAssociatedAddressBookPolicies()
		{
			base.WriteError(new InvalidOperationException(Strings.ErrorRemoveAddressListWithAssociatedAddressBookPolicies(base.DataObject.Name)), ErrorCategory.InvalidOperation, base.DataObject.Identity);
			return false;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (this.Recursive)
			{
				IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
				List<ADObjectId> list = new List<ADObjectId>();
				try
				{
					this.currentProcent = 0;
					ADPagedReader<AddressBookBase> adpagedReader = configurationSession.FindPaged<AddressBookBase>(base.DataObject.Id, QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.NotEqual, ADObjectSchema.Id, base.DataObject.Id), null, 0);
					foreach (AddressBookBase addressBookBase in adpagedReader)
					{
						list.Add(addressBookBase.Id);
						if (list.Count % ADGenericPagedReader<AddressBookBase>.DefaultPageSize == 0)
						{
							this.MoveToNextPercent();
							base.WriteProgress(Strings.ProgressActivityRemovingAddressListTree(base.DataObject.Id.ToString()), Strings.ProgressStatusReadingAddressListTree(list.Count), this.currentProcent);
						}
					}
					list.Add(base.DataObject.Id);
					if (this.currentProcent != 0)
					{
						this.ReportDeleteTreeProgress(null);
					}
					configurationSession.DeleteTree(base.DataObject, new TreeDeleteNotFinishedHandler(this.ReportDeleteTreeProgress));
				}
				catch (DataSourceTransientException exception)
				{
					base.WriteError(exception, ErrorCategory.WriteError, base.DataObject.Identity);
					TaskLogger.LogExit();
					return;
				}
				try
				{
					UpdateAddressBookBase<AddressListIdParameter>.UpdateRecipients(base.DataObject, list.ToArray(), base.DomainController, base.TenantGlobalCatalogSession, new Task.TaskWarningLoggingDelegate(this.WriteWarning), new WriteProgress(base.WriteProgress), this);
					goto IL_206;
				}
				catch (DataSourceTransientException ex)
				{
					this.WriteWarning(Strings.ErrorReadMatchingRecipients(this.Identity.ToString(), base.DataObject.LdapRecipientFilter, ex.Message));
					TaskLogger.Trace("Exception is raised while reading recipients: {0}", new object[]
					{
						ex.ToString()
					});
					goto IL_206;
				}
				catch (DataSourceOperationException ex2)
				{
					this.WriteWarning(Strings.ErrorReadMatchingRecipients(this.Identity.ToString(), base.DataObject.LdapRecipientFilter, ex2.Message));
					TaskLogger.Trace("Exception is raised while reading recipients matching filter: {0}", new object[]
					{
						ex2.ToString()
					});
					goto IL_206;
				}
			}
			base.InternalProcessRecord();
			IL_206:
			TaskLogger.LogExit();
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return AddressList.FromDataObject((AddressBookBase)dataObject);
		}

		private int currentProcent;
	}
}
