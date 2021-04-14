using System;
using Microsoft.Exchange.Data.ApplicationLogic.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.Services;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class ContactIdentity : RecipientIdentity
	{
		private ContactIdentity(ADContact adContact)
		{
			this.adRecipient = adContact;
			this.masterAccountSid = adContact.MasterAccountSid;
			this.sid = this.masterAccountSid;
		}

		public static bool TryCreate(ADRecipient adRecipient, out ContactIdentity contactIdentity)
		{
			contactIdentity = null;
			ADContact adcontact = adRecipient as ADContact;
			if (adcontact != null)
			{
				if (RecipientHelper.TryGetMasterAccountSid(adRecipient) == null)
				{
					ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<ADContact>(0L, "adContact {0} does not have a valid MasterAccountSid", adcontact);
				}
				else
				{
					contactIdentity = new ContactIdentity(adcontact);
				}
			}
			else
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<ADRecipient>(0L, "adRecipient {0} is not ADContact", adRecipient);
			}
			return contactIdentity != null;
		}
	}
}
