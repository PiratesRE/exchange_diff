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
	[Cmdlet("Get", "OabVirtualDirectory", DefaultParameterSetName = "Identity")]
	public sealed class GetOabVirtualDirectory : GetExchangeVirtualDirectory<ADOabVirtualDirectory>
	{
		protected override void ProcessMetabaseProperties(ExchangeVirtualDirectory dataObject)
		{
			TaskLogger.LogEnter();
			base.ProcessMetabaseProperties(dataObject);
			((ADOabVirtualDirectory)dataObject).OAuthAuthentication = ((ADOabVirtualDirectory)dataObject).InternalAuthenticationMethods.Contains(AuthenticationMethod.OAuth);
			((ADOabVirtualDirectory)dataObject).RequireSSL = IisUtility.SSLEnabled(dataObject.MetabasePath);
			using (DirectoryEntry directoryEntry = IisUtility.CreateIISDirectoryEntry(dataObject.MetabasePath))
			{
				((ADOabVirtualDirectory)dataObject).BasicAuthentication = IisUtility.CheckForAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.Basic);
				((ADOabVirtualDirectory)dataObject).WindowsAuthentication = IisUtility.CheckForAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.Ntlm);
			}
			dataObject.ResetChangeTracking();
			TaskLogger.LogExit();
		}

		protected override bool CanIgnoreMissingMetabaseEntry()
		{
			return true;
		}

		protected override LocalizedString GetMissingMetabaseEntryWarning(ExchangeVirtualDirectory vdir)
		{
			return Strings.OabVirtualDirectoryAdOrphanFound(vdir.Id.Name);
		}
	}
}
