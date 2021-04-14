using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "ThrottlingPolicyAssociation", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetThrottlingPolicyAssociation : SetRecipientObjectTask<ThrottlingPolicyAssociationIdParameter, ThrottlingPolicyAssociation, ADRecipient>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if (this.ThrottlingPolicy == null)
				{
					return Strings.ConfirmationMessageSetThrottlingPolicyAssociationToNull(this.Identity.ToString());
				}
				return Strings.ConfirmationMessageSetThrottlingPolicyAssociation(this.Identity.ToString(), this.ThrottlingPolicy.ToString());
			}
		}

		[Parameter]
		public ThrottlingPolicyIdParameter ThrottlingPolicy
		{
			get
			{
				return (ThrottlingPolicyIdParameter)base.Fields["ThrottlingPolicy"];
			}
			set
			{
				base.Fields["ThrottlingPolicy"] = value;
			}
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
			TaskLogger.LogEnter();
			ADRecipient adrecipient = (ADRecipient)dataObject;
			if (this.ThrottlingPolicy != null)
			{
				if (SharedConfiguration.IsDehydratedConfiguration(base.CurrentOrganizationId))
				{
					base.WriteError(new ArgumentException(Strings.ErrorLinkOpOnDehydratedTenant("ThrottlingPolicy")), ErrorCategory.InvalidArgument, (this.DataObject != null) ? this.DataObject.Identity : null);
				}
				ThrottlingPolicy throttlingPolicy = (ThrottlingPolicy)base.GetDataObject<ThrottlingPolicy>(this.ThrottlingPolicy, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorThrottlingPolicyNotFound(this.ThrottlingPolicy.ToString())), new LocalizedString?(Strings.ErrorThrottlingPolicyNotUnique(this.ThrottlingPolicy.ToString())));
				adrecipient.ThrottlingPolicy = (ADObjectId)throttlingPolicy.Identity;
			}
			else
			{
				adrecipient.ThrottlingPolicy = null;
			}
			base.StampChangesOn(adrecipient);
			TaskLogger.LogExit();
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			if (!base.Fields.IsModified("ThrottlingPolicy"))
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorThrottlingPolicyMustBeSpecified), ExchangeErrorCategory.Client, null);
			}
			TaskLogger.LogExit();
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			ADRecipient dataObject2 = (ADRecipient)dataObject;
			return ThrottlingPolicyAssociation.FromDataObject(dataObject2);
		}
	}
}
