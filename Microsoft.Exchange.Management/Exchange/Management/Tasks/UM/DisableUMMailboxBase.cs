using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.UM;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	public class DisableUMMailboxBase<TIdentity> : UMMailboxTask<TIdentity> where TIdentity : RecipientIdParameter, new()
	{
		public DisableUMMailboxBase()
		{
			this.KeepProperties = true;
		}

		[LocDescription(Strings.IDs.KeepProperties)]
		[Parameter(Mandatory = false)]
		public bool KeepProperties
		{
			get
			{
				return (bool)base.Fields["KeepProperties"];
			}
			set
			{
				base.Fields["KeepProperties"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				TIdentity identity = this.Identity;
				return Strings.ConfirmationMessageDisableUMMailbox(identity.ToString());
			}
		}

		protected override void DoValidate()
		{
			LocalizedException ex = null;
			if (!UMSubscriber.IsValidSubscriber(this.DataObject))
			{
				ex = new UserAlreadyUmDisabledException(this.DataObject.PrimarySmtpAddress.ToString());
			}
			else
			{
				Utility.ResetUMADProperties(this.DataObject, this.KeepProperties);
			}
			if (ex != null)
			{
				base.WriteError(ex, ErrorCategory.InvalidArgument, null);
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.ResetUMMailbox(this.KeepProperties);
			base.InternalProcessRecord();
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_UMUserDisabled, null, new object[]
			{
				this.DataObject.PrimarySmtpAddress
			});
			this.WriteResult();
			TaskLogger.LogExit();
		}

		private void WriteResult()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject.Id
			});
			UMMailbox sendToPipeline = new UMMailbox(this.DataObject);
			base.WriteObject(sendToPipeline);
			TaskLogger.LogExit();
		}

		internal static readonly PropertyDefinition[] PropertiesToReset = new PropertyDefinition[]
		{
			ADUserSchema.UMEnabledFlags,
			ADUserSchema.UMEnabledFlags2,
			ADUserSchema.UMMailboxPolicy,
			ADUserSchema.OperatorNumber,
			ADUserSchema.PhoneProviderId,
			ADUserSchema.UMPinChecksum,
			ADUserSchema.UMServerWritableFlags,
			ADUserSchema.CallAnsweringAudioCodecLegacy,
			ADUserSchema.CallAnsweringAudioCodec2
		};
	}
}
