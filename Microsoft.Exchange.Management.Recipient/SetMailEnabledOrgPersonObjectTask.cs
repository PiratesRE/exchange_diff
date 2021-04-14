using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public abstract class SetMailEnabledOrgPersonObjectTask<TIdentity, TPublicObject, TDataObject> : SetMailEnabledRecipientObjectTask<TIdentity, TPublicObject, TDataObject> where TIdentity : IIdentityParameter, new() where TPublicObject : MailEnabledOrgPerson, new() where TDataObject : ADRecipient, new()
	{
		[Parameter(Mandatory = false)]
		public string SecondaryAddress
		{
			get
			{
				return (string)base.Fields["SecondaryAddress"];
			}
			set
			{
				base.Fields["SecondaryAddress"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public UMDialPlanIdParameter SecondaryDialPlan
		{
			get
			{
				return (UMDialPlanIdParameter)base.Fields["SecondaryDialPlan"];
			}
			set
			{
				base.Fields["SecondaryDialPlan"] = value;
			}
		}

		[Parameter]
		public SwitchParameter RemovePicture
		{
			get
			{
				return (SwitchParameter)(base.Fields["RemovePicture"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["RemovePicture"] = value;
			}
		}

		[Parameter]
		public SwitchParameter RemoveSpokenName
		{
			get
			{
				return (SwitchParameter)(base.Fields["RemoveSpokenName"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["RemoveSpokenName"] = value;
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			if (this.SecondaryAddress != null)
			{
				if (this.SecondaryDialPlan == null)
				{
					base.ThrowTerminatingError(new TaskArgumentException(Strings.ErrorUMInvalidParameters), ExchangeErrorCategory.Client, null);
				}
			}
			else if (this.SecondaryDialPlan != null)
			{
				base.WriteError(new TaskArgumentException(Strings.ErrorUMInvalidParameters), ExchangeErrorCategory.Client, null);
			}
			TaskLogger.LogExit();
		}

		protected override void ResolveLocalSecondaryIdentities()
		{
			base.ResolveLocalSecondaryIdentities();
			if (this.SecondaryAddress != null && this.SecondaryDialPlan != null)
			{
				this.secondaryDialPlan = (UMDialPlan)base.GetDataObject<UMDialPlan>(this.SecondaryDialPlan, this.ConfigurationSession, null, new LocalizedString?(Strings.NonExistantDialPlan(this.SecondaryDialPlan.ToString())), new LocalizedString?(Strings.MultipleDialplansWithSameId(this.SecondaryDialPlan.ToString())), ExchangeErrorCategory.Client);
				if (!Utils.IsUriValid(this.SecondaryAddress, this.secondaryDialPlan.URIType, this.secondaryDialPlan.NumberOfDigitsInExtension))
				{
					if (this.secondaryDialPlan.URIType == UMUriType.E164)
					{
						if (!Utils.IsUriValid(this.SecondaryAddress, UMUriType.TelExtn, this.secondaryDialPlan.NumberOfDigitsInExtension))
						{
							base.ThrowTerminatingError(new TaskArgumentException(Strings.ErrorUMInvalidE164OrExtensionAddressFormat(this.SecondaryAddress)), ExchangeErrorCategory.Client, null);
							return;
						}
					}
					else if (this.secondaryDialPlan.URIType == UMUriType.SipName)
					{
						if (!Utils.IsUriValid(this.SecondaryAddress, UMUriType.TelExtn, this.secondaryDialPlan.NumberOfDigitsInExtension))
						{
							base.ThrowTerminatingError(new TaskArgumentException(Strings.ErrorUMInvalidSipNameOrExtensionAddressFormat(this.SecondaryAddress)), ExchangeErrorCategory.Client, null);
							return;
						}
					}
					else if (this.secondaryDialPlan.URIType == UMUriType.TelExtn)
					{
						base.WriteError(new TaskArgumentException(Strings.ErrorUMInvalidAddressFormat(this.SecondaryAddress)), ExchangeErrorCategory.Client, null);
					}
				}
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			ADRecipient adrecipient = (ADRecipient)base.PrepareDataObject();
			if (this.secondaryDialPlan != null)
			{
				IADRecipientLookup iadrecipientLookup = ADRecipientLookupFactory.CreateUmProxyAddressLookup(this.secondaryDialPlan);
				ADRecipient adrecipient2 = iadrecipientLookup.LookupByExtensionAndEquivalentDialPlan(this.SecondaryAddress, this.secondaryDialPlan);
				if (adrecipient2 != null && adrecipient2.PrimarySmtpAddress != adrecipient.PrimarySmtpAddress)
				{
					base.WriteError(new TaskArgumentException(DirectoryStrings.ExtensionNotUnique(this.SecondaryAddress, this.secondaryDialPlan.Name)), ExchangeErrorCategory.Client, adrecipient.Identity);
				}
				try
				{
					adrecipient.AddEUMProxyAddress(this.SecondaryAddress, this.secondaryDialPlan);
				}
				catch (InvalidOperationException ex)
				{
					base.WriteError(new TaskException(Strings.ErrorStampSecondaryAddress(ex.Message), ex), ExchangeErrorCategory.Client, adrecipient);
				}
			}
			if (this.RemovePicture.IsPresent)
			{
				adrecipient.ThumbnailPhoto = null;
			}
			if (this.RemoveSpokenName.IsPresent)
			{
				adrecipient.UMSpokenName = null;
			}
			TaskLogger.LogExit();
			return adrecipient;
		}

		private UMDialPlan secondaryDialPlan;
	}
}
