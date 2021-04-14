using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.ApplicationLogic.ConnectionSettingsDiscovery.Connections;
using Microsoft.Exchange.Data.ApplicationLogic.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.ConnectionSettingsDiscovery
{
	internal class ConnectionSettingsManager
	{
		protected ConnectionSettingsManager(IConnectionSettingsWriteProvider[] writableProviders, IConnectionSettingsReadProvider[] readOnlyProviders)
		{
			this.readOnlyProviders = readOnlyProviders;
			this.writableProviders = writableProviders;
		}

		public static ConnectionSettingsManager GetInstanceForModernOutlook(ILogAdapter logAdapter)
		{
			if (logAdapter == null)
			{
				throw new ArgumentNullException("logAdapter", "The logAdapter parameter cannot be null.");
			}
			logAdapter.RegisterLogMetaData("ConnectionSettingsDiscovery", typeof(ConnectionSettingsDiscoveryMetadata));
			GlobalConnectionSettingsProvider globalConnectionSettingsProvider = new GlobalConnectionSettingsProvider(logAdapter);
			return new ConnectionSettingsManager(new IConnectionSettingsWriteProvider[]
			{
				globalConnectionSettingsProvider
			}, new IConnectionSettingsReadProvider[]
			{
				new O365ConnectionSettingsProvider(logAdapter),
				globalConnectionSettingsProvider
			});
		}

		public IEnumerable<ConnectionSettings> GetConnectionSettingsMatchingEmail(SmtpAddress email)
		{
			foreach (IConnectionSettingsReadProvider provider in this.readOnlyProviders)
			{
				foreach (ConnectionSettings settings in provider.GetConnectionSettingsMatchingEmail(email))
				{
					yield return settings;
				}
			}
			yield break;
		}

		public void SetConnectionSettings(SmtpAddress email, ConnectionSettings connectionSettings)
		{
			foreach (IConnectionSettingsWriteProvider connectionSettingsWriteProvider in this.writableProviders)
			{
				if (connectionSettingsWriteProvider.SourceId != connectionSettings.SourceId)
				{
					connectionSettingsWriteProvider.SetConnectionSettingsMatchingEmail(email, connectionSettings);
				}
			}
		}

		private const string ConnectionSettingsDiscoveryLogName = "ConnectionSettingsDiscovery";

		private readonly IConnectionSettingsWriteProvider[] writableProviders;

		private readonly IConnectionSettingsReadProvider[] readOnlyProviders;
	}
}
