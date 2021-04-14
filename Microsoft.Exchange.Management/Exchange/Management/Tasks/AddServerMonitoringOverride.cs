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
	[Cmdlet("Add", "ServerMonitoringOverride", SupportsShouldProcess = true, DefaultParameterSetName = "Duration")]
	public sealed class AddServerMonitoringOverride : ServerMonitoringOverrideBase
	{
		[Parameter(Mandatory = true, Position = 0)]
		[ValidateNotNullOrEmpty]
		public string Identity
		{
			get
			{
				return (string)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public MonitoringItemTypeEnum ItemType
		{
			get
			{
				return (MonitoringItemTypeEnum)base.Fields["ItemType"];
			}
			set
			{
				base.Fields["ItemType"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		[ValidateNotNullOrEmpty]
		public string PropertyName
		{
			get
			{
				return (string)base.Fields["PropertyName"];
			}
			set
			{
				base.Fields["PropertyName"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		[ValidateNotNullOrEmpty]
		public string PropertyValue
		{
			get
			{
				return (string)base.Fields["PropertyValue"];
			}
			set
			{
				base.Fields["PropertyValue"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Duration")]
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

		[Parameter(Mandatory = true, ParameterSetName = "ApplyVersion")]
		public Version ApplyVersion
		{
			get
			{
				return (Version)base.Fields["ApplyVersion"];
			}
			set
			{
				base.Fields["ApplyVersion"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageAddMonitoringOverride(this.PropertyName, this.helper.MonitoringItemIdentity, this.ItemType.ToString());
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			this.helper.ParseAndValidateIdentity(this.Identity, false);
			if (base.Fields.IsModified("ApplyVersion"))
			{
				MonitoringOverrideHelpers.ValidateApplyVersion(this.ApplyVersion);
			}
			if (base.Fields.IsModified("Duration"))
			{
				MonitoringOverrideHelpers.ValidateOverrideDuration(this.Duration);
				return;
			}
			this.Duration = new EnhancedTimeSpan?(EnhancedTimeSpan.FromDays(365.0));
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (!base.ValidateMonitoringItemExist(this.helper.HealthSet, this.ItemType) && !base.ShouldProcess(Strings.ConfirmationMonitoringItemNotFound(this.helper.MonitoringItemIdentity, this.ItemType.ToString())))
			{
				TaskLogger.LogExit();
				return;
			}
			try
			{
				using (RegistryKey registryKey = base.RegistryKeyHive.CreateSubKey(ServerMonitoringOverrideBase.OverridesBaseRegistryKey, RegistryKeyPermissionCheck.ReadWriteSubTree))
				{
					using (RegistryKey registryKey2 = registryKey.CreateSubKey(this.ItemType.ToString(), RegistryKeyPermissionCheck.ReadWriteSubTree))
					{
						string[] subKeyNames = registryKey2.GetSubKeyNames();
						string text = base.GenerateOverrideString(this.helper.MonitoringItemIdentity, this.PropertyName);
						base.ValidateGlobalLocalConflict(text, subKeyNames, this.PropertyName, this.ItemType);
						using (RegistryKey registryKey3 = registryKey2.CreateSubKey(text))
						{
							registryKey3.SetValue("PropertyValue", this.PropertyValue, RegistryValueKind.String);
							registryKey3.SetValue("TimeUpdated", DateTime.UtcNow.ToString("u"), RegistryValueKind.String);
							registryKey3.SetValue("CreatedBy", base.ExecutingUserIdentityName, RegistryValueKind.String);
							if (base.Fields.IsModified("Duration"))
							{
								registryKey3.SetValue("ExpirationTime", DateTime.UtcNow.AddSeconds(this.Duration.Value.TotalSeconds).ToString("u"), RegistryValueKind.String);
							}
							if (base.Fields.IsModified("ApplyVersion"))
							{
								registryKey3.SetValue("ApplyVersion", this.ApplyVersion.ToString(true), RegistryValueKind.String);
							}
							registryKey.SetValue("Watermark", Guid.NewGuid().ToString(), RegistryValueKind.String);
							registryKey.SetValue("TimeUpdated", DateTime.UtcNow, RegistryValueKind.String);
						}
					}
				}
				if (base.IsVerboseOn)
				{
					base.WriteVerbose(Strings.SuccessAddServerMonitoringOverride(this.helper.MonitoringItemName, base.ServerName));
				}
			}
			catch (IOException ex)
			{
				base.WriteError(new FailedToRunServerMonitoringOverrideException(base.ServerName, ex.ToString()), ErrorCategory.ObjectNotFound, null);
			}
			catch (SecurityException ex2)
			{
				base.WriteError(new FailedToRunServerMonitoringOverrideException(base.ServerName, ex2.ToString()), ExchangeErrorCategory.Authorization, null);
			}
			catch (UnauthorizedAccessException ex3)
			{
				base.WriteError(new FailedToRunServerMonitoringOverrideException(base.ServerName, ex3.ToString()), ExchangeErrorCategory.Authorization, null);
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}
	}
}
