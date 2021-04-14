using System;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Remove", "SupervisionListEntry", DefaultParameterSetName = "RemoveOne", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveSupervisionListEntry : ModifySupervisionListEntryBase
	{
		[Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
		public override RecipientIdParameter Identity
		{
			get
			{
				return (RecipientIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[ValidateNotNull]
		[Parameter(ParameterSetName = "RemoveOne", Mandatory = true)]
		public string Tag
		{
			get
			{
				return (string)base.Fields["Tag"];
			}
			set
			{
				base.Fields["Tag"] = value;
			}
		}

		[Parameter(ParameterSetName = "RemoveOne", Mandatory = true)]
		[ValidateNotNull]
		public RecipientIdParameter Entry
		{
			get
			{
				return (RecipientIdParameter)base.Fields["Entry"];
			}
			set
			{
				base.Fields["Entry"] = value;
			}
		}

		[Parameter(ParameterSetName = "RemoveAll", Mandatory = true)]
		public SwitchParameter RemoveAll
		{
			get
			{
				return (SwitchParameter)(base.Fields["RemoveAll"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["RemoveAll"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if (this.RemoveAll)
				{
					return Strings.ConfirmationRemoveAllSupervisionListEntries;
				}
				return Strings.ConfirmationRemoveSupervisionListEntry(this.Entry.ToString(), this.Tag);
			}
		}

		protected override void SupervisionListAction()
		{
			TaskLogger.LogEnter();
			if (this.RemoveAll)
			{
				this.DataObject[ADRecipientSchema.DLSupervisionList] = null;
				this.DataObject[ADRecipientSchema.InternalRecipientSupervisionList] = null;
				this.DataObject[ADRecipientSchema.OneOffSupervisionList] = null;
			}
			else
			{
				ADRecipient adrecipient = null;
				SmtpAddress? smtpAddress = null;
				base.ResolveEntry(this.Entry, out adrecipient, out smtpAddress);
				if (adrecipient != null)
				{
					bool isGroup = ADRecipient.IsAllowedDeliveryRestrictionGroup(adrecipient.RecipientType);
					this.RemoveADRecipientSupervisionListEntry(isGroup, adrecipient);
				}
				else if (smtpAddress != null)
				{
					this.RemoveExternalRecipientSupervisionListEntry(smtpAddress.Value);
				}
				else
				{
					base.WriteError(new ArgumentNullException("adRecipientToRemove, externalAddressToRemove"), (ErrorCategory)1000, null);
				}
			}
			TaskLogger.LogExit();
		}

		private void RemoveExternalRecipientSupervisionListEntry(SmtpAddress externalRecipientToRemove)
		{
			MultiValuedProperty<ADObjectIdWithString> supervisionListForExternalAddress = base.GetSupervisionListForExternalAddress();
			ADObjectIdWithString adobjectIdWithString = null;
			string[] array = null;
			PropertyValidationError propertyValidationError = base.FindExternalAddressEntry(externalRecipientToRemove, supervisionListForExternalAddress, out adobjectIdWithString, out array);
			if (propertyValidationError != null)
			{
				return;
			}
			if (adobjectIdWithString != null)
			{
				foreach (string text in array)
				{
					if (text.Equals(this.Tag, StringComparison.OrdinalIgnoreCase))
					{
						this.RemoveTagFromExternalAddressEntry(adobjectIdWithString, array, externalRecipientToRemove, supervisionListForExternalAddress);
						return;
					}
				}
				base.WriteError(new RecipientTaskException(Strings.ErrorSupervisionEntryNotPresent(this.Entry.ToString(), this.Tag.ToLower())), (ErrorCategory)1003, null);
				return;
			}
			base.WriteError(new RecipientTaskException(Strings.ErrorSupervisionEntryNotPresent(this.Entry.ToString(), this.Tag.ToLower())), (ErrorCategory)1003, null);
		}

		private void RemoveADRecipientSupervisionListEntry(bool isGroup, ADRecipient adRecipientToRemove)
		{
			MultiValuedProperty<ADObjectIdWithString> supervisionListForADRecipient = base.GetSupervisionListForADRecipient(isGroup);
			ADObjectIdWithString adobjectIdWithString = null;
			string[] array = null;
			PropertyValidationError propertyValidationError = base.FindADRecipientEntry(adRecipientToRemove, supervisionListForADRecipient, out adobjectIdWithString, out array);
			if (propertyValidationError != null)
			{
				return;
			}
			if (adobjectIdWithString != null)
			{
				foreach (string text in array)
				{
					if (text.Equals(this.Tag, StringComparison.OrdinalIgnoreCase))
					{
						this.RemoveTagFromADRecipientEntry(adobjectIdWithString, array, supervisionListForADRecipient);
						return;
					}
				}
				base.WriteError(new RecipientTaskException(Strings.ErrorSupervisionEntryNotPresent(this.Entry.ToString(), this.Tag.ToLower())), (ErrorCategory)1003, null);
				return;
			}
			base.WriteError(new RecipientTaskException(Strings.ErrorSupervisionEntryNotPresent(this.Entry.ToString(), this.Tag.ToLower())), (ErrorCategory)1003, null);
		}

		private void RemoveTagFromADRecipientEntry(ADObjectIdWithString entry, string[] tags, MultiValuedProperty<ADObjectIdWithString> supervisionList)
		{
			if (tags.Length == 1)
			{
				supervisionList.Remove(entry);
				return;
			}
			StringBuilder stringBuilder = new StringBuilder(entry.StringValue.Length - this.Tag.Length - 1);
			foreach (string text in tags)
			{
				if (!text.Equals(this.Tag, StringComparison.OrdinalIgnoreCase))
				{
					if (stringBuilder.Length != 0)
					{
						stringBuilder.Append(SupervisionListEntryConstraint.Delimiter);
					}
					stringBuilder.Append(text);
				}
			}
			ADObjectIdWithString item = new ADObjectIdWithString(stringBuilder.ToString(), entry.ObjectIdValue);
			supervisionList.Remove(entry);
			supervisionList.Add(item);
		}

		private void RemoveTagFromExternalAddressEntry(ADObjectIdWithString entry, string[] tags, SmtpAddress externalRecipientToRemove, MultiValuedProperty<ADObjectIdWithString> supervisionList)
		{
			if (tags.Length == 1)
			{
				supervisionList.Remove(entry);
				return;
			}
			StringBuilder stringBuilder = new StringBuilder(entry.StringValue.Length - this.Tag.Length - 1);
			foreach (string text in tags)
			{
				if (!text.Equals(this.Tag, StringComparison.OrdinalIgnoreCase))
				{
					if (stringBuilder.Length != 0)
					{
						stringBuilder.Append(SupervisionListEntryConstraint.Delimiter);
					}
					stringBuilder.Append(text);
				}
			}
			stringBuilder.Append(SupervisionListEntryConstraint.Delimiter);
			stringBuilder.Append(externalRecipientToRemove.ToString());
			ADObjectIdWithString item = new ADObjectIdWithString(stringBuilder.ToString(), entry.ObjectIdValue);
			supervisionList.Remove(entry);
			supervisionList.Add(item);
		}

		private const string RemoveOneParameterSetName = "RemoveOne";

		private const string RemoveAllParameterSetName = "RemoveAll";
	}
}
