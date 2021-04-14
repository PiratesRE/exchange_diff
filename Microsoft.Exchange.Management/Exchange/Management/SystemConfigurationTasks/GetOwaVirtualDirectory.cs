using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "OwaVirtualDirectory", DefaultParameterSetName = "Identity")]
	public sealed class GetOwaVirtualDirectory : GetExchangeVirtualDirectory<ADOwaVirtualDirectory>
	{
		protected override void ProcessMetabaseProperties(ExchangeVirtualDirectory dataObject)
		{
			TaskLogger.LogEnter();
			base.ProcessMetabaseProperties(dataObject);
			ADOwaVirtualDirectory adowaVirtualDirectory = (ADOwaVirtualDirectory)dataObject;
			try
			{
				WebAppVirtualDirectoryHelper.UpdateFromMetabase(adowaVirtualDirectory);
				if (adowaVirtualDirectory.GzipLevel != GzipLevel.Off && !adowaVirtualDirectory.IsExchange2007OrLater && this.IsMailboxRoleInstalled(adowaVirtualDirectory))
				{
					this.WriteWarning(Strings.OwaGzipEnabledOnLegacyVirtualDirectoryWhenMailboxRoleInstalledWarning(adowaVirtualDirectory.Id.Name));
				}
			}
			catch (Exception ex)
			{
				TaskLogger.Trace("Exception occurred: {0}", new object[]
				{
					ex.Message
				});
				this.WriteError(new LocalizedException(Strings.OwaMetabaseGetPropertiesFailure), ErrorCategory.InvalidOperation, null, false);
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected override bool CanIgnoreMissingMetabaseEntry()
		{
			return true;
		}

		protected override LocalizedString GetMissingMetabaseEntryWarning(ExchangeVirtualDirectory vdir)
		{
			return Strings.OwaAdOrphanFound(vdir.Id.Name);
		}

		private bool IsMailboxRoleInstalled(ADOwaVirtualDirectory dataObject)
		{
			Server server = (Server)base.GetDataObject<Server>(new ServerIdParameter(dataObject.Server), base.DataSession, null, new LocalizedString?(Strings.ErrorServerNotFound(dataObject.Server.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(dataObject.Server.ToString())));
			return server.IsMailboxServer;
		}
	}
}
