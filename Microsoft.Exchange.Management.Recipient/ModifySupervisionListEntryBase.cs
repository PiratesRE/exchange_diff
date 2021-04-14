using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public abstract class ModifySupervisionListEntryBase : RecipientObjectActionTask<RecipientIdParameter, ADRecipient>
	{
		protected PropertyValidationError FindADRecipientEntry(ADRecipient adRecipient, MultiValuedProperty<ADObjectIdWithString> supervisionList, out ADObjectIdWithString foundEntry, out string[] tags)
		{
			if (adRecipient == null)
			{
				base.WriteError(new ArgumentNullException("adRecipient"), (ErrorCategory)1000, null);
			}
			if (supervisionList == null)
			{
				base.WriteError(new ArgumentNullException("supervisionList"), (ErrorCategory)1000, null);
			}
			foundEntry = null;
			tags = null;
			foreach (ADObjectIdWithString adobjectIdWithString in supervisionList)
			{
				if (adobjectIdWithString.ObjectIdValue.Equals(adRecipient.Id))
				{
					foundEntry = adobjectIdWithString;
					break;
				}
			}
			PropertyValidationError propertyValidationError = null;
			if (foundEntry != null)
			{
				SupervisionListEntryConstraint supervisionListEntryConstraint = new SupervisionListEntryConstraint(false);
				propertyValidationError = supervisionListEntryConstraint.Validate(foundEntry, null, null);
				if (propertyValidationError != null)
				{
					return propertyValidationError;
				}
				tags = foundEntry.StringValue.Split(new char[]
				{
					SupervisionListEntryConstraint.Delimiter
				});
			}
			return propertyValidationError;
		}

		protected PropertyValidationError FindExternalAddressEntry(SmtpAddress externalAddress, MultiValuedProperty<ADObjectIdWithString> supervisionList, out ADObjectIdWithString foundEntry, out string[] tags)
		{
			if (supervisionList == null)
			{
				base.WriteError(new ArgumentNullException("supervisionList"), (ErrorCategory)1000, null);
			}
			foundEntry = null;
			tags = null;
			SupervisionListEntryConstraint supervisionListEntryConstraint = new SupervisionListEntryConstraint(true);
			PropertyValidationError propertyValidationError = null;
			string[] array = null;
			foreach (ADObjectIdWithString adobjectIdWithString in supervisionList)
			{
				propertyValidationError = supervisionListEntryConstraint.Validate(adobjectIdWithString, null, null);
				if (propertyValidationError != null)
				{
					return propertyValidationError;
				}
				array = adobjectIdWithString.StringValue.Split(new char[]
				{
					SupervisionListEntryConstraint.Delimiter
				});
				SmtpAddress smtpAddress = new SmtpAddress(array[array.Length - 1]);
				if (smtpAddress.Equals(externalAddress))
				{
					foundEntry = adobjectIdWithString;
					break;
				}
			}
			if (foundEntry != null)
			{
				Array.Resize<string>(ref array, array.Length - 1);
				tags = array;
			}
			return propertyValidationError;
		}

		protected override IConfigurable PrepareDataObject()
		{
			return (ADRecipient)base.PrepareDataObject();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			this.SupervisionListAction();
			TaskLogger.LogExit();
		}

		protected abstract void SupervisionListAction();

		protected void ResolveEntry(RecipientIdParameter entry, out ADRecipient adRecipient, out SmtpAddress? externalAddress)
		{
			if (entry == null)
			{
				base.WriteError(new ArgumentNullException("entry"), (ErrorCategory)1000, null);
			}
			adRecipient = null;
			externalAddress = null;
			try
			{
				adRecipient = (ADRecipient)base.GetDataObject<ADRecipient>(entry, base.TenantGlobalCatalogSession, this.DataObject.OrganizationId.OrganizationalUnit, new LocalizedString?(Strings.ErrorRecipientNotFound((string)entry)), new LocalizedString?(Strings.ErrorRecipientNotUnique((string)entry)));
			}
			catch (ManagementObjectNotFoundException)
			{
				try
				{
					externalAddress = new SmtpAddress?(SmtpAddress.Parse(entry.ToString()));
				}
				catch (FormatException exception)
				{
					base.WriteError(exception, ErrorCategory.InvalidData, null);
				}
				adRecipient = null;
				return;
			}
			OrganizationId organizationId = this.DataObject.OrganizationId;
			OrganizationId organizationId2 = adRecipient.OrganizationId;
			if (!organizationId.Equals(organizationId2))
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorEntryNotInRecipientOrg(entry.ToString())), (ErrorCategory)1003, null);
			}
		}

		protected MultiValuedProperty<ADObjectIdWithString> GetSupervisionListForADRecipient(bool isGroup)
		{
			if (isGroup)
			{
				return (MultiValuedProperty<ADObjectIdWithString>)this.DataObject[ADRecipientSchema.DLSupervisionList];
			}
			return (MultiValuedProperty<ADObjectIdWithString>)this.DataObject[ADRecipientSchema.InternalRecipientSupervisionList];
		}

		protected MultiValuedProperty<ADObjectIdWithString> GetSupervisionListForExternalAddress()
		{
			return (MultiValuedProperty<ADObjectIdWithString>)this.DataObject[ADRecipientSchema.OneOffSupervisionList];
		}
	}
}
