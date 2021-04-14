using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class RunspaceAccessorBase : SubscriptionAccessorBase
	{
		protected RunspaceAccessorBase(IMigrationDataProvider dataProvider)
		{
			this.DataProvider = dataProvider;
			this.OrganizationId = dataProvider.OrganizationId;
		}

		internal IMigrationDataProvider DataProvider { get; private set; }

		internal OrganizationId OrganizationId { get; private set; }

		internal T RunCommand<T>(PSCommand command) where T : class
		{
			return this.RunCommand<T>(command, null, null);
		}

		internal T RunCommand<T>(PSCommand command, ICollection<Type> ignoreExceptions, ICollection<Type> transientExceptions) where T : class
		{
			Type type;
			return this.RunCommand<T>(command, ignoreExceptions, transientExceptions, out type);
		}

		internal T RunCommand<T>(PSCommand command, ICollection<Type> ignoreExceptions, ICollection<Type> transientExceptions, out Type ignoredErrorType) where T : class
		{
			ErrorRecord errorRecord = null;
			ignoredErrorType = null;
			string commandString = MigrationRunspaceProxy.GetCommandString(command);
			try
			{
				MigrationLogger.Log(MigrationEventType.Verbose, "Running PS command {0}", new object[]
				{
					commandString
				});
				T result = this.DataProvider.RunspaceProxy.RunPSCommand<T>(command, out errorRecord);
				if (errorRecord == null)
				{
					return result;
				}
			}
			catch (ParameterBindingException ex)
			{
				return this.HandleException<T>(commandString, ex, ignoreExceptions, transientExceptions, out ignoredErrorType);
			}
			catch (CmdletInvocationException ex2)
			{
				return this.HandleException<T>(commandString, ex2.InnerException ?? ex2, ignoreExceptions, transientExceptions, out ignoredErrorType);
			}
			MigrationUtil.AssertOrThrow(errorRecord != null, "expect to have an error at this point", new object[0]);
			if (errorRecord.Exception != null)
			{
				return this.HandleException<T>(commandString, errorRecord.Exception, ignoreExceptions, transientExceptions, out ignoredErrorType);
			}
			throw new MigrationPermanentException(ServerStrings.MigrationRunspaceError(commandString, errorRecord.ToString()));
		}

		protected static bool ExceptionIsIn(Exception ex, ICollection<Type> typesToCheck, out Type exceptionType)
		{
			if (typesToCheck != null)
			{
				Type type = ex.GetType();
				foreach (Type type2 in typesToCheck)
				{
					if (type2.IsAssignableFrom(type))
					{
						exceptionType = type2;
						return true;
					}
				}
			}
			exceptionType = null;
			return false;
		}

		protected abstract T HandleException<T>(string commandString, Exception ex, ICollection<Type> transientExceptions);

		private T HandleException<T>(string commandString, Exception ex, ICollection<Type> ignoreExceptions, ICollection<Type> transientExceptions, out Type ignoredErrorType) where T : class
		{
			MigrationUtil.ThrowOnNullArgument(ex, "ex");
			Type type;
			if (RunspaceAccessorBase.ExceptionIsIn(ex, ignoreExceptions, out type))
			{
				MigrationLogger.Log(MigrationEventType.Verbose, "ignoring exception because it's on ignored list", new object[]
				{
					ex
				});
				ignoredErrorType = type;
				return default(T);
			}
			ignoredErrorType = null;
			return this.HandleException<T>(commandString, ex, transientExceptions);
		}
	}
}
