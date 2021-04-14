using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class AsrDirectorySearchResult : AsrSearchResult
	{
		internal AsrDirectorySearchResult(IUMRecognitionPhrase recognitionPhrase, Guid tenantGuid)
		{
			string text = (string)recognitionPhrase["ObjectGuid"];
			if (!string.IsNullOrEmpty(text))
			{
				IADRecipientLookup iadrecipientLookup = ADRecipientLookupFactory.CreateFromTenantGuid(tenantGuid);
				this.selectedRecipient = iadrecipientLookup.LookupByObjectId(new ADObjectId(new Guid(text)));
			}
			if (this.selectedRecipient == null)
			{
				throw new InvalidObjectGuidException(text);
			}
			IADOrgPerson iadorgPerson = this.selectedRecipient as IADOrgPerson;
			if (iadorgPerson != null && !string.IsNullOrEmpty(iadorgPerson.Phone))
			{
				this.selectedPhoneNumber = PhoneNumber.Parse(iadorgPerson.Phone);
				return;
			}
		}

		public ADRecipient Recipient
		{
			get
			{
				return this.selectedRecipient;
			}
		}

		internal override void SetManagerVariables(ActivityManager manager, BaseUMCallSession vo)
		{
			ContactSearchItem varValue = ContactSearchItem.CreateFromRecipient(this.selectedRecipient);
			manager.WriteVariable("resultType", ResultType.DirectoryContact);
			manager.WriteVariable("resultTypeString", ResultType.DirectoryContact.ToString());
			manager.WriteVariable("selectedUser", varValue);
			manager.WriteVariable("directorySearchResult", varValue);
			manager.WriteVariable("selectedPhoneNumber", this.selectedPhoneNumber);
		}

		private ADRecipient selectedRecipient;

		private PhoneNumber selectedPhoneNumber;
	}
}
