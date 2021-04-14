using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class DataAccessHelper
	{
		public static bool IsDataAccessKnownException(Exception exception)
		{
			if (exception == null)
			{
				throw new ArgumentNullException("exception");
			}
			for (int i = 0; i < DataAccessHelper.knownExceptionTypes.Length; i++)
			{
				if (DataAccessHelper.knownExceptionTypes[i].IsInstanceOfType(exception))
				{
					return true;
				}
			}
			return TaskHelper.IsTaskKnownException(exception);
		}

		public static ExchangeErrorCategory ResolveExceptionErrorCategory(Exception exception)
		{
			if (typeof(ManagementObjectNotFoundException).IsInstanceOfType(exception))
			{
				return ExchangeErrorCategory.Context;
			}
			if (typeof(TransientException).IsInstanceOfType(exception))
			{
				return ExchangeErrorCategory.ServerTransient;
			}
			if (typeof(DataSourceOperationException).IsInstanceOfType(exception))
			{
				if (typeof(ADFilterException).IsInstanceOfType(exception) || typeof(ADInvalidPasswordException).IsInstanceOfType(exception))
				{
					return ExchangeErrorCategory.Client;
				}
				if (typeof(ADObjectAlreadyExistsException).IsInstanceOfType(exception) || typeof(ADObjectEntryAlreadyExistsException).IsInstanceOfType(exception) || typeof(ADNoSuchObjectException).IsInstanceOfType(exception) || typeof(ADRemoveContainerException).IsInstanceOfType(exception))
				{
					return ExchangeErrorCategory.Context;
				}
				if (typeof(ADScopeException).IsInstanceOfType(exception) || typeof(ADInvalidCredentialException).IsInstanceOfType(exception))
				{
					return ExchangeErrorCategory.Authorization;
				}
				return ExchangeErrorCategory.ServerOperation;
			}
			else
			{
				if (typeof(DataValidationException).IsInstanceOfType(exception))
				{
					return ExchangeErrorCategory.Context;
				}
				if (typeof(ManagementObjectAmbiguousException).IsInstanceOfType(exception))
				{
					return ExchangeErrorCategory.Context;
				}
				return (ExchangeErrorCategory)0;
			}
		}

		private static Type[] knownExceptionTypes = new Type[]
		{
			typeof(ManagementObjectNotFoundException),
			typeof(ManagementObjectAmbiguousException)
		};

		internal delegate IConfigurable GetDataObjectDelegate(IIdentityParameter id, IConfigDataProvider session, ObjectId rootID, OptionalIdentityData optionalData, LocalizedString? notFoundError, LocalizedString? multipleFoundError);

		internal delegate IConfigurable CategorizedGetDataObjectDelegate(IIdentityParameter id, IConfigDataProvider session, ObjectId rootID, OptionalIdentityData optionalData, LocalizedString? notFoundError, LocalizedString? multipleFoundError, ExchangeErrorCategory category);
	}
}
