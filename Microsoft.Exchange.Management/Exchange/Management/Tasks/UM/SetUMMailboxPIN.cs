using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.UM;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("Set", "UMMailboxPIN", SupportsShouldProcess = true)]
	public class SetUMMailboxPIN : UMMailboxTask<MailboxIdParameter>
	{
		public SetUMMailboxPIN()
		{
			this.PinExpired = true;
			this.LockedOut = false;
		}

		[Parameter(Mandatory = false)]
		[LocDescription(Strings.IDs.Pin)]
		public string Pin
		{
			get
			{
				return (string)base.Fields["Pin"];
			}
			set
			{
				base.Fields["Pin"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[LocDescription(Strings.IDs.PinExpired)]
		public bool PinExpired
		{
			get
			{
				return (bool)base.Fields["PinExpired"];
			}
			set
			{
				base.Fields["PinExpired"] = value;
			}
		}

		[LocDescription(Strings.IDs.LockedOut)]
		[Parameter(Mandatory = false)]
		public bool LockedOut
		{
			get
			{
				return (bool)base.Fields["LockedOut"];
			}
			set
			{
				base.Fields["LockedOut"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[LocDescription(Strings.IDs.NotifyEmail)]
		public string NotifyEmail
		{
			get
			{
				return (string)base.Fields["NotifyEmail"];
			}
			set
			{
				base.Fields["NotifyEmail"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[LocDescription(Strings.IDs.SendEmail)]
		public bool SendEmail
		{
			get
			{
				return (bool)(base.Fields["SendEmail"] ?? true);
			}
			set
			{
				base.Fields["SendEmail"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetUMMailboxPIN(this.Identity.ToString());
			}
		}

		protected override void DoValidate()
		{
			LocalizedException ex = null;
			ADUser dataObject = this.DataObject;
			if (!UMSubscriber.IsValidSubscriber(dataObject))
			{
				ex = new UserNotUmEnabledException(this.DataObject.PrimarySmtpAddress.ToString());
			}
			else
			{
				base.PinInfo = base.ValidateOrGeneratePIN(this.Pin);
				base.PinInfo.PinExpired = this.PinExpired;
				base.PinInfo.LockedOut = this.LockedOut;
			}
			if (ex != null)
			{
				base.WriteError(ex, ErrorCategory.InvalidArgument, null);
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.SavePIN();
			base.InternalProcessRecord();
			if (!base.HasErrors)
			{
				if (this.SendEmail)
				{
					base.SubmitResetPINMessage(this.NotifyEmail);
				}
				if (!this.LockedOut)
				{
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_UMUserUnlocked, null, new object[]
					{
						this.DataObject.PrimarySmtpAddress
					});
				}
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_UMUserPasswordChanged, null, new object[]
				{
					this.DataObject.PrimarySmtpAddress
				});
				this.WriteResult();
			}
			TaskLogger.LogExit();
		}

		private void WriteResult()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject.Id
			});
			UMMailboxPin sendToPipeline = new UMMailboxPin(this.DataObject, base.PinInfo.PinExpired, base.PinInfo.LockedOut, base.PinInfo.FirstTimeUser, base.NeedSuppressingPiiData);
			base.WriteObject(sendToPipeline);
			TaskLogger.LogExit();
		}
	}
}
