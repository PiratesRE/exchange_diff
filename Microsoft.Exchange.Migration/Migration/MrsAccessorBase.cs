using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class MrsAccessorBase : RunspaceAccessorBase
	{
		protected MrsAccessorBase(IMigrationDataProvider dataProvider, string batchName) : base(dataProvider)
		{
			this.BatchName = string.Format("MigrationService:{0}", batchName);
		}

		private protected string BatchName { protected get; private set; }

		internal T Run<T>(MrsAccessorCommand command) where T : class
		{
			Type type;
			return this.Run<T>(command, out type);
		}

		internal T Run<T>(MrsAccessorCommand command, out Type ignoredErrorType) where T : class
		{
			return base.RunCommand<T>(command.Command, command.IgnoreExceptions, command.TransientExceptions, out ignoredErrorType);
		}

		protected static long HandleLongOverflow(ulong value, RequestStatisticsBase subscription)
		{
			if (value > 9223372036854775807UL)
			{
				MigrationLogger.Log(MigrationEventType.Warning, "Subscription {0}: Enumerated more items {1} than a signed long can store.", new object[]
				{
					subscription,
					value
				});
				return long.MaxValue;
			}
			return (long)value;
		}

		protected MRSSubscriptionId GetMRSIdentity(ISubscriptionId subscriptionId, bool required = false)
		{
			if (required)
			{
				MigrationUtil.ThrowOnNullArgument(subscriptionId, "subscriptionId");
			}
			else if (subscriptionId == null)
			{
				return null;
			}
			MRSSubscriptionId mrssubscriptionId = subscriptionId as MRSSubscriptionId;
			MigrationUtil.AssertOrThrow(mrssubscriptionId != null, "SubscriptionId needs to be a MRSSubscriptionID", new object[0]);
			return mrssubscriptionId;
		}

		protected override T HandleException<T>(string commandString, Exception ex, ICollection<Type> transientExceptions)
		{
			MigrationUtil.ThrowOnNullArgument(ex, "ex");
			this.HandleTransientException(ex as LocalizedException, transientExceptions);
			LocalizedException ex2 = ex as LocalizedException;
			if (ex2 != null)
			{
				throw new MigrationPermanentException(ex2.LocalizedString, commandString, ex);
			}
			throw new MigrationPermanentException(ServerStrings.MigrationRunspaceError(commandString, ex.Message), ex);
		}

		private void HandleTransientException(LocalizedException ex, ICollection<Type> transientExceptions)
		{
			if (ex == null)
			{
				return;
			}
			if (ex is ManagementObjectNotFoundException)
			{
				throw new MigrationTransientException(ex.LocalizedString, ex);
			}
			if (CommonUtils.IsTransientException(ex))
			{
				throw new MigrationTransientException(ex.LocalizedString, ex);
			}
			Type type;
			if (RunspaceAccessorBase.ExceptionIsIn(ex, transientExceptions, out type))
			{
				throw new MigrationTransientException(ex.LocalizedString, ex);
			}
		}

		private const string BatchNameFormat = "MigrationService:{0}";
	}
}
