using System;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal static class SharedConfigurationTaskHelper
	{
		public static void Validate(Task task, SharedTenantConfigurationMode taskSharedTenantConfigurationMode, LazilyInitialized<SharedTenantConfigurationState> currentOrgState, string targetObject)
		{
			if (taskSharedTenantConfigurationMode == SharedTenantConfigurationMode.NotShared)
			{
				return;
			}
			if (SharedTenantConfigurationState.UnSupported == currentOrgState.Value || SharedTenantConfigurationState.NotShared == currentOrgState.Value)
			{
				return;
			}
			if (SharedTenantConfigurationMode.Static == taskSharedTenantConfigurationMode && (currentOrgState.Value & SharedTenantConfigurationState.Static) != SharedTenantConfigurationState.UnSupported)
			{
				task.WriteError(new InvalidOperationInDehydratedContextException(Strings.ErrorWriteOpOnDehydratedTenant), ExchangeErrorCategory.Context, targetObject);
			}
			if ((currentOrgState.Value & SharedTenantConfigurationState.Dehydrated) != SharedTenantConfigurationState.UnSupported)
			{
				task.WriteError(new InvalidOperationInDehydratedContextException(Strings.ErrorWriteOpOnDehydratedTenant), ExchangeErrorCategory.Context, targetObject);
			}
		}

		public static bool ShouldPrompt(Task task, SharedTenantConfigurationMode taskSharedTenantConfigurationMode, LazilyInitialized<SharedTenantConfigurationState> currentOrgState)
		{
			return taskSharedTenantConfigurationMode != SharedTenantConfigurationMode.NotShared && (currentOrgState.Value & SharedTenantConfigurationState.Shared) != SharedTenantConfigurationState.UnSupported;
		}

		internal static void VerifyIsNotTinyTenant(LazilyInitialized<SharedTenantConfigurationState> configurationState, Task.ErrorLoggerDelegate writeError)
		{
			if (null == configurationState)
			{
				throw new ArgumentNullException("configurationState");
			}
			if (writeError == null)
			{
				throw new ArgumentNullException("writeError");
			}
			if ((configurationState.Value & SharedTenantConfigurationState.Dehydrated) != SharedTenantConfigurationState.UnSupported)
			{
				SharedConfigurationTaskHelper.WriteTinyTenantError(writeError);
			}
		}

		internal static void VerifyIsNotTinyTenant(OrganizationId organizationId, Task.ErrorLoggerDelegate writeError)
		{
			if (null == organizationId)
			{
				throw new ArgumentNullException("organizationId");
			}
			if (writeError == null)
			{
				throw new ArgumentNullException("writeError");
			}
			if (SharedConfiguration.GetSharedConfiguration(organizationId) != null)
			{
				SharedConfigurationTaskHelper.WriteTinyTenantError(writeError);
			}
		}

		private static void WriteTinyTenantError(Task.ErrorLoggerDelegate writeError)
		{
			writeError(new InvalidOperationInDehydratedContextException(Strings.ErrorWriteOpOnDehydratedTenant), ExchangeErrorCategory.Context, null);
		}
	}
}
