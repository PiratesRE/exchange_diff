using System;
using System.IO;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Enable", "MailboxQuarantine", DefaultParameterSetName = "Identity", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class EnableMailboxQuarantine : MailboxQuarantineTaskBase
	{
		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? Duration
		{
			get
			{
				if (!base.Fields.Contains("Duration"))
				{
					return null;
				}
				return (EnhancedTimeSpan?)base.Fields["Duration"];
			}
			set
			{
				base.Fields["Duration"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter AllowMigration
		{
			get
			{
				return (SwitchParameter)(base.Fields["AllowMigration"] ?? false);
			}
			set
			{
				base.Fields["AllowMigration"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageEnableMailboxQuarantine(base.Identity.ToString());
			}
		}

		protected override void InternalValidate()
		{
			if (this.Duration != null)
			{
				EnhancedTimeSpan zero = EnhancedTimeSpan.Zero;
				if (this.Duration.Value <= zero)
				{
					base.WriteError(new ArgumentException(Strings.DurationShouldBeGreaterThanZero(zero.ToString()), "Duration"), ErrorCategory.InvalidData, null);
				}
				EnhancedTimeSpan t = EnhancedTimeSpan.FromDays(365.0);
				if (this.Duration.Value > t)
				{
					base.WriteError(new ArgumentException(Strings.DurationShouldBeLessThan1Year(t.ToString()), "Duration"), ErrorCategory.InvalidData, null);
				}
			}
			base.InternalValidate();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				if (base.RegistryKeyHive != null)
				{
					string text = string.Format("{0}\\{1}\\Private-{2}", MailboxQuarantineTaskBase.QuarantineBaseRegistryKey, base.Server, base.Database.Guid);
					int num;
					using (RegistryKey registryKey = base.RegistryKeyHive.CreateSubKey(text))
					{
						num = (int)registryKey.GetValue("MailboxQuarantineCrashThreshold", 3);
					}
					string subkey = string.Format("{0}\\QuarantinedMailboxes\\{1}", text, base.ExchangeGuid);
					using (RegistryKey registryKey2 = base.RegistryKeyHive.CreateSubKey(subkey, RegistryKeyPermissionCheck.ReadWriteSubTree))
					{
						registryKey2.SetValue("CrashCount", num);
						registryKey2.SetValue("LastCrashTime", DateTime.UtcNow.ToFileTime(), RegistryValueKind.QWord);
						registryKey2.SetValue("MailboxQuarantineDescription", "Enable-MailboxQuarantine - " + base.ExecutingUserIdentityName, RegistryValueKind.String);
						if (this.Duration != null)
						{
							registryKey2.SetValue("MailboxQuarantineDurationInSeconds", (int)this.Duration.Value.TotalSeconds);
						}
						if (this.AllowMigration)
						{
							registryKey2.SetValue("AllowMigrationOfQuarantinedMailbox", 1);
						}
					}
				}
				if (base.IsVerboseOn)
				{
					base.WriteVerbose(Strings.SuccessEnableMailboxQuarantine(base.Identity.ToString()));
				}
			}
			catch (IOException ex)
			{
				base.WriteError(new FailedMailboxQuarantineException(base.Identity.ToString(), ex.ToString()), ErrorCategory.ObjectNotFound, null);
			}
			catch (SecurityException ex2)
			{
				base.WriteError(new FailedMailboxQuarantineException(base.Identity.ToString(), ex2.ToString()), ErrorCategory.SecurityError, null);
			}
			catch (UnauthorizedAccessException ex3)
			{
				base.WriteError(new FailedMailboxQuarantineException(base.Identity.ToString(), ex3.ToString()), ErrorCategory.PermissionDenied, null);
			}
		}

		private const string ParameterDuration = "Duration";

		private const string ParameterAllowMigration = "AllowMigration";

		private const int DefaultCrashCount = 3;

		private const int AllowMigrationValue = 1;
	}
}
