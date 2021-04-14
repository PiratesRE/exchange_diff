using System;
using System.Net.Sockets;
using System.Runtime.Remoting;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration.Test
{
	internal class MigrationEmailHandlerProxy : IMigrationEmailHandler
	{
		private MigrationEmailHandlerProxy(string endpoint)
		{
			MigrationUtil.ThrowOnNullOrEmptyArgument(endpoint, "endpoint");
			this.endpoint = endpoint;
			this.implementation = (IMigrationEmailHandler)Activator.GetObject(typeof(IMigrationEmailHandler), endpoint);
			if (this.implementation == null)
			{
				throw new InvalidOperationException("couldn't create remote instance at endpoint " + endpoint);
			}
		}

		public static bool TryCreate(string reportMessageEndpoint, out IMigrationEmailHandler handler)
		{
			handler = null;
			if (string.IsNullOrWhiteSpace(reportMessageEndpoint))
			{
				return false;
			}
			bool result;
			try
			{
				handler = new MigrationEmailHandlerProxy(reportMessageEndpoint);
				result = true;
			}
			catch (RemotingException exception)
			{
				MigrationLogger.Log(MigrationEventType.Error, exception, "Failed to open connection to emulator even though one was set.", new object[0]);
				result = false;
			}
			return result;
		}

		public IMigrationEmailMessageItem CreateEmailMessage()
		{
			IMigrationEmailMessageItem result;
			try
			{
				result = this.implementation.CreateEmailMessage();
			}
			catch (RemotingException innerException)
			{
				throw new MigrationServerConnectionFailedException(this.endpoint, innerException);
			}
			catch (SocketException innerException2)
			{
				throw new MigrationServerConnectionFailedException(this.endpoint, innerException2);
			}
			return result;
		}

		private readonly IMigrationEmailHandler implementation;

		private readonly string endpoint;
	}
}
