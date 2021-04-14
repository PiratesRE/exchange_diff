using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks.ManagementScopeExtensions
{
	public static class ManagementScopeTasksExtensions
	{
		internal static bool ValidateAndSetFilterOnDataObject(this SetTaskBase<ManagementScope> task, string filterPropertyName, ManagementScope dataObject, Task.TaskErrorLoggingDelegate writeError)
		{
			string text = (string)task.Fields[filterPropertyName];
			LocalizedString? localizedString = null;
			switch (dataObject.ScopeRestrictionType)
			{
			case ScopeRestrictionType.RecipientScope:
				if (!filterPropertyName.Equals("RecipientRestrictionFilter"))
				{
					localizedString = new LocalizedString?(Strings.InvalidParameterForFilter(filterPropertyName, ScopeRestrictionType.RecipientScope.ToString(), "RecipientRestrictionFilter"));
				}
				break;
			case ScopeRestrictionType.ServerScope:
				if (!filterPropertyName.Equals("ServerRestrictionFilter"))
				{
					localizedString = new LocalizedString?(Strings.InvalidParameterForFilter(filterPropertyName, ScopeRestrictionType.ServerScope.ToString(), "ServerRestrictionFilter"));
				}
				break;
			case ScopeRestrictionType.PartnerDelegatedTenantScope:
				if (!filterPropertyName.Equals("PartnerDelegatedTenantRestrictionFilter"))
				{
					localizedString = new LocalizedString?(Strings.InvalidParameterForFilter(filterPropertyName, ScopeRestrictionType.PartnerDelegatedTenantScope.ToString(), "PartnerDelegatedTenantRestrictionFilter"));
				}
				break;
			case ScopeRestrictionType.DatabaseScope:
				if (!filterPropertyName.Equals("DatabaseRestrictionFilter"))
				{
					localizedString = new LocalizedString?(Strings.InvalidParameterForFilter(filterPropertyName, ScopeRestrictionType.DatabaseScope.ToString(), "DatabaseRestrictionFilter"));
				}
				break;
			}
			if (localizedString != null)
			{
				writeError(new ArgumentException(localizedString.Value, filterPropertyName), ErrorCategory.InvalidArgument, null);
			}
			if (!string.IsNullOrEmpty(text))
			{
				QueryFilter queryFilter;
				string m;
				if (!RBACHelper.TryConvertPowershellFilterIntoQueryFilter(text, dataObject.ScopeRestrictionType, task, out queryFilter, out m))
				{
					switch (dataObject.ScopeRestrictionType)
					{
					case ScopeRestrictionType.RecipientScope:
						localizedString = new LocalizedString?(Strings.RecipientFilterMustBeValid(m));
						break;
					case ScopeRestrictionType.ServerScope:
						localizedString = new LocalizedString?(Strings.ServerFilterMustBeValid(m));
						break;
					case ScopeRestrictionType.PartnerDelegatedTenantScope:
						localizedString = new LocalizedString?(Strings.PartnerFilterMustBeValid(m));
						break;
					case ScopeRestrictionType.DatabaseScope:
						localizedString = new LocalizedString?(Strings.DatabaseFilterMustBeValid(m));
						break;
					default:
						localizedString = new LocalizedString?(Strings.ErrorUnsupportedConfigScopeType(dataObject.Id.ToString(), dataObject.ScopeRestrictionType.ToString()));
						break;
					}
					writeError(new ArgumentException(localizedString.Value, filterPropertyName), ErrorCategory.InvalidArgument, null);
				}
				dataObject.Filter = queryFilter.GenerateInfixString(FilterLanguage.Monad);
				return true;
			}
			dataObject.Filter = null;
			return false;
		}
	}
}
