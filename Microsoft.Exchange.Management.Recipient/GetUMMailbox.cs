using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Get", "UMMailbox", DefaultParameterSetName = "Identity")]
	public sealed class GetUMMailbox : GetUMMailboxBase<MailboxIdParameter>
	{
		protected override RecipientTypeDetails[] InternalRecipientTypeDetails
		{
			get
			{
				return GetUMMailbox.AllowedRecipientTypeDetails;
			}
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			ADUser aduser = (ADUser)dataObject;
			if (null != aduser.MasterAccountSid)
			{
				aduser.LinkedMasterAccount = SecurityPrincipalIdParameter.GetFriendlyUserName(aduser.MasterAccountSid, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
				aduser.ResetChangeTracking();
			}
			UMMailbox ummailbox = UMMailbox.FromDataObject(aduser);
			if (ummailbox.UMDialPlan != null)
			{
				UMDialPlan dialPlan = ummailbox.GetDialPlan();
				if (dialPlan != null && (dialPlan.URIType == UMUriType.E164 || dialPlan.URIType == UMUriType.SipName))
				{
					ummailbox.SIPResourceIdentifier = UMMailbox.GetPrimaryExtension(ummailbox.EmailAddresses, ProxyAddressPrefix.UM);
				}
				if (dialPlan != null)
				{
					ummailbox.PhoneNumber = ummailbox.GetEUMPhoneNumber(dialPlan);
				}
				ummailbox.AccessTelephoneNumbers = dialPlan.AccessTelephoneNumbers;
				ummailbox.CallAnsweringRulesExtensions = new MultiValuedProperty<string>(Utils.GetExtensionsInDialPlanValidForPAA(dialPlan, aduser));
			}
			return ummailbox;
		}

		private static readonly RecipientTypeDetails[] AllowedRecipientTypeDetails = UMMailbox.GetUMRecipientTypeDetails();
	}
}
