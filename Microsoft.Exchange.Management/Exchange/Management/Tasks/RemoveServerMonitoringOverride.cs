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
	[Cmdlet("Remove", "ServerMonitoringOverride", SupportsShouldProcess = true)]
	public sealed class RemoveServerMonitoringOverride : ServerMonitoringOverrideBase
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

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveMonitoringOverride(this.PropertyName, this.helper.MonitoringItemIdentity, this.ItemType.ToString());
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			this.helper.ParseAndValidateIdentity(this.Identity, false);
		}

		protected override void InternalProcessRecord()
		{
			bool flag = false;
			TaskLogger.LogEnter();
			try
			{
				using (RegistryKey registryKey = base.RegistryKeyHive.OpenSubKey(ServerMonitoringOverrideBase.OverridesBaseRegistryKey, true))
				{
					if (registryKey != null)
					{
						using (RegistryKey registryKey2 = registryKey.OpenSubKey(this.ItemType.ToString(), true))
						{
							if (registryKey2 != null)
							{
								string subkey = base.GenerateOverrideString(this.helper.MonitoringItemIdentity, this.PropertyName);
								try
								{
									registryKey2.DeleteSubKey(subkey, true);
									registryKey.SetValue("Watermark", Guid.NewGuid().ToString(), RegistryValueKind.String);
									registryKey.SetValue("TimeUpdated", DateTime.UtcNow, RegistryValueKind.String);
									flag = true;
								}
								catch (ArgumentException)
								{
								}
							}
						}
					}
				}
				if (!flag)
				{
					base.WriteError(new OverrideNotFoundException(this.helper.MonitoringItemIdentity, this.ItemType.ToString(), this.PropertyName), ErrorCategory.ObjectNotFound, null);
				}
				if (base.IsVerboseOn)
				{
					base.WriteVerbose(Strings.SuccessRemoveServerMonitoringOverride(this.helper.MonitoringItemName, base.ServerName));
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
