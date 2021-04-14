using System;
using System.DirectoryServices;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Metabase;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class GetExchangeServiceVirtualDirectory<T> : GetExchangeVirtualDirectory<T> where T : ADExchangeServiceVirtualDirectory, new()
	{
		protected virtual LocalizedString MetabaseGetPropertiesFailureMessage
		{
			get
			{
				return Strings.MetabaseGetPropertiesFailure;
			}
		}

		protected override void ProcessMetabaseProperties(ExchangeVirtualDirectory dataObject)
		{
			TaskLogger.LogEnter();
			base.ProcessMetabaseProperties(dataObject);
			ADExchangeServiceVirtualDirectory adexchangeServiceVirtualDirectory = (ADExchangeServiceVirtualDirectory)dataObject;
			try
			{
				DirectoryEntry directoryEntry2;
				DirectoryEntry directoryEntry = directoryEntry2 = IisUtility.CreateIISDirectoryEntry(adexchangeServiceVirtualDirectory.MetabasePath, new Task.TaskErrorLoggingReThrowDelegate(this.WriteError), dataObject.Identity, false);
				try
				{
					if (directoryEntry != null)
					{
						adexchangeServiceVirtualDirectory.BasicAuthentication = new bool?(IisUtility.CheckForAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.Basic));
						adexchangeServiceVirtualDirectory.DigestAuthentication = new bool?(IisUtility.CheckForAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.Digest));
						adexchangeServiceVirtualDirectory.WindowsAuthentication = new bool?(IisUtility.CheckForAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.Ntlm));
						adexchangeServiceVirtualDirectory.LiveIdNegotiateAuthentication = new bool?(IisUtility.CheckForAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.LiveIdNegotiate));
						adexchangeServiceVirtualDirectory.LiveIdBasicAuthentication = new bool?(adexchangeServiceVirtualDirectory.InternalAuthenticationMethods.Contains(AuthenticationMethod.LiveIdBasic));
						adexchangeServiceVirtualDirectory.OAuthAuthentication = new bool?(adexchangeServiceVirtualDirectory.InternalAuthenticationMethods.Contains(AuthenticationMethod.OAuth));
						adexchangeServiceVirtualDirectory.AdfsAuthentication = new bool?(adexchangeServiceVirtualDirectory.InternalAuthenticationMethods.Contains(AuthenticationMethod.Adfs));
						adexchangeServiceVirtualDirectory.WSSecurityAuthentication = new bool?(adexchangeServiceVirtualDirectory.InternalAuthenticationMethods.Contains(AuthenticationMethod.WSSecurity) && IisUtility.CheckForAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.WSSecurity));
					}
				}
				finally
				{
					if (directoryEntry2 != null)
					{
						((IDisposable)directoryEntry2).Dispose();
					}
				}
			}
			catch (Exception ex)
			{
				TaskLogger.Trace("Exception occurred: {0}", new object[]
				{
					ex.Message
				});
				base.WriteError(new LocalizedException(this.MetabaseGetPropertiesFailureMessage, ex), ErrorCategory.InvalidOperation, dataObject.Identity);
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected bool? GetCertificateAuthentication(ExchangeVirtualDirectory dataObject)
		{
			return this.GetCertificateAuthentication(dataObject, null);
		}

		protected bool? GetCertificateAuthentication(ExchangeVirtualDirectory dataObject, string subVDirName)
		{
			return this.GetAuthentication(dataObject, subVDirName, AuthenticationMethodFlags.Certificate);
		}

		protected bool? GetLiveIdNegotiateAuthentication(ExchangeVirtualDirectory dataObject, string subVDirName)
		{
			return this.GetAuthentication(dataObject, subVDirName, AuthenticationMethodFlags.LiveIdNegotiate);
		}

		private bool? GetAuthentication(ExchangeVirtualDirectory dataObject, string subVDirName, AuthenticationMethodFlags authFlags)
		{
			TaskLogger.LogEnter();
			try
			{
				string text = dataObject.MetabasePath;
				if (!string.IsNullOrEmpty(subVDirName))
				{
					text = string.Format("{0}/{1}", text, subVDirName);
				}
				if (IisUtility.Exists(text))
				{
					using (DirectoryEntry directoryEntry = IisUtility.CreateIISDirectoryEntry(text, new Task.TaskErrorLoggingReThrowDelegate(this.WriteError), dataObject.Identity))
					{
						bool ignoreAnonymousOnCert = dataObject is ADPowerShellCommonVirtualDirectory;
						return new bool?(IisUtility.CheckForAuthenticationMethod(directoryEntry, authFlags, ignoreAnonymousOnCert));
					}
				}
			}
			catch (Exception ex)
			{
				TaskLogger.Trace("Exception occurred: {0}", new object[]
				{
					ex.Message
				});
				base.WriteError(new LocalizedException(this.MetabaseGetPropertiesFailureMessage, ex), (ErrorCategory)1001, dataObject.Identity);
			}
			finally
			{
				TaskLogger.LogExit();
			}
			return null;
		}

		protected override bool CanIgnoreMissingMetabaseEntry()
		{
			return true;
		}
	}
}
