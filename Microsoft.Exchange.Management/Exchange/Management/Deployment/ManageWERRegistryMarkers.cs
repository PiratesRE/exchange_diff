using System;
using System.ComponentModel;
using System.IO;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	public abstract class ManageWERRegistryMarkers : Task
	{
		protected void WriteRegistryMarkers()
		{
			try
			{
				using (RegistryKey werconsentRegistryKey = this.GetWERConsentRegistryKey())
				{
					if (werconsentRegistryKey != null)
					{
						foreach (string name in this.exchangeEventTypes)
						{
							werconsentRegistryKey.SetValue(name, 4, RegistryValueKind.DWord);
						}
					}
				}
			}
			catch (SecurityException exception)
			{
				base.WriteError(exception, ErrorCategory.SecurityError, null);
			}
			catch (UnauthorizedAccessException exception2)
			{
				base.WriteError(exception2, ErrorCategory.PermissionDenied, null);
			}
			catch (IOException exception3)
			{
				base.WriteError(exception3, ErrorCategory.ResourceUnavailable, null);
			}
			catch (Win32Exception exception4)
			{
				base.WriteError(exception4, ErrorCategory.InvalidOperation, null);
			}
		}

		protected void DeleteRegistryMarkers()
		{
			try
			{
				using (RegistryKey werconsentRegistryKey = this.GetWERConsentRegistryKey())
				{
					if (werconsentRegistryKey != null)
					{
						foreach (string name in this.exchangeEventTypes)
						{
							werconsentRegistryKey.DeleteValue(name, false);
						}
					}
				}
			}
			catch (SecurityException exception)
			{
				base.WriteError(exception, ErrorCategory.SecurityError, null);
			}
			catch (UnauthorizedAccessException exception2)
			{
				base.WriteError(exception2, ErrorCategory.PermissionDenied, null);
			}
			catch (IOException exception3)
			{
				base.WriteError(exception3, ErrorCategory.ResourceUnavailable, null);
			}
			catch (Win32Exception exception4)
			{
				base.WriteError(exception4, ErrorCategory.InvalidOperation, null);
			}
		}

		private RegistryKey GetWERConsentRegistryKey()
		{
			RegistryKey registryKey = null;
			if (ConfigurationContext.Setup.IsLonghornServer)
			{
				registryKey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\Windows Error Reporting\\Consent", RegistryKeyPermissionCheck.ReadWriteSubTree);
				if (registryKey == null)
				{
					throw new IOException(Strings.ExceptionRegistryKeyNotFound("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\Windows Error Reporting\\Consent"));
				}
			}
			return registryKey;
		}

		private const string WERConsentKey = "SOFTWARE\\Microsoft\\Windows\\Windows Error Reporting\\Consent";

		private string[] exchangeEventTypes = new string[]
		{
			"E12",
			"E12N",
			"E12IE",
			"E12IIS"
		};
	}
}
