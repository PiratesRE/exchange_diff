using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("Remove", "UMMailboxPrompt", SupportsShouldProcess = true, DefaultParameterSetName = "CustomVoicemailGreeting", ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveUMMailboxPrompt : RecipientObjectActionTask<MailboxIdParameter, ADUser>
	{
		[Parameter(Mandatory = true, ParameterSetName = "CustomVoicemailGreeting", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ParameterSetName = "CustomAwayGreeting", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public override MailboxIdParameter Identity
		{
			get
			{
				return (MailboxIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "CustomVoicemailGreeting")]
		public SwitchParameter CustomVoicemailGreeting
		{
			get
			{
				return (SwitchParameter)(base.Fields["ClearVoicemailGreeting"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ClearVoicemailGreeting"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "CustomAwayGreeting")]
		public SwitchParameter CustomAwayGreeting
		{
			get
			{
				return (SwitchParameter)(base.Fields["ClearAwayGreeting"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ClearAwayGreeting"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveUMMailboxPrompt(this.Identity.ToString());
			}
		}

		protected override IConfigurable ResolveDataObject()
		{
			ADRecipient adrecipient = (ADRecipient)base.ResolveDataObject();
			if (MailboxTaskHelper.ExcludeMailboxPlan(adrecipient, false))
			{
				base.WriteError(new ManagementObjectNotFoundException(base.GetErrorMessageObjectNotFound(this.Identity.ToString(), typeof(ADUser).ToString(), (base.DataSession != null) ? base.DataSession.Source : null)), (ErrorCategory)1000, this.Identity);
			}
			return adrecipient;
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			if (this.CustomVoicemailGreeting || this.CustomAwayGreeting)
			{
				using (UMSubscriber umsubscriber = UMRecipient.Factory.FromADRecipient<UMSubscriber>(this.DataObject))
				{
					if (umsubscriber != null)
					{
						try
						{
							if (this.CustomVoicemailGreeting)
							{
								umsubscriber.RemoveCustomGreeting(MailboxGreetingEnum.Voicemail);
							}
							if (this.CustomAwayGreeting)
							{
								umsubscriber.RemoveCustomGreeting(MailboxGreetingEnum.Away);
							}
							goto IL_85;
						}
						catch (UserConfigurationException exception)
						{
							base.WriteError(exception, (ErrorCategory)1001, null);
							goto IL_85;
						}
					}
					base.WriteError(new UserNotUmEnabledException(this.Identity.ToString()), (ErrorCategory)1000, null);
					IL_85:;
				}
			}
		}

		internal abstract class ParameterSet
		{
			internal const string CustomVoicemailGreeting = "CustomVoicemailGreeting";

			internal const string CustomAwayGreeting = "CustomAwayGreeting";
		}
	}
}
