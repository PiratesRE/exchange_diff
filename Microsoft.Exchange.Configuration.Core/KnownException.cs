using System;
using System.DirectoryServices.Protocols;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Core
{
	internal static class KnownException
	{
		internal static bool IsKnownException(Exception ex)
		{
			if (ex == null)
			{
				return false;
			}
			string fullName = ex.GetType().FullName;
			return KnownException.KnownExceptionInPlainStringList.Contains(fullName) || ex is TransientException || ex is OverBudgetException || ex is CannotResolveTenantNameException || ex is DataSourceOperationException || ex is LdapException;
		}

		internal static bool IsUnhandledException(Exception ex)
		{
			return !KnownException.IsKnownException(ex);
		}

		private static readonly HashSet<string> KnownExceptionInPlainStringList = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"Microsoft.Exchange.Configuration.Authorization.AuthorizationException",
			"Microsoft.Exchange.Configuration.Authorization.CmdletAccessDeniedException",
			"Microsoft.Exchange.Configuration.Authorization.ImpersonationDeniedException",
			"Microsoft.Exchange.Configuration.Authorization.AppPasswordLoginException",
			"Microsoft.Exchange.Configuration.Authorization.FilteringOnlyUserForFfoLoginException",
			"Microsoft.Exchange.Configuration.Authorization.NonMigratedUserDeniedException",
			"Microsoft.Exchange.Configuration.Authorization.RBACContextParserException"
		};
	}
}
