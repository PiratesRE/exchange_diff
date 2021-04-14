using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.CompliancePolicy;
using Microsoft.Office.CompliancePolicy.PolicySync;

namespace Microsoft.Exchange.Data.Storage.UnifiedPolicy
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class TenantInfoProvider : ITenantInfoProvider, IDisposable
	{
		public ExchangePrincipal SyncMailboxPrincipal { get; private set; }

		public void Save(TenantInfo tenantInfo)
		{
			try
			{
				using (MailboxSession mailboxSession = MailboxSession.OpenAsAdmin(this.SyncMailboxPrincipal, CultureInfo.InvariantCulture, "Client=UnifiedPolicy;Action=CommitChanges;Interactive=False"))
				{
					using (UserConfiguration mailboxConfiguration = UserConfigurationHelper.GetMailboxConfiguration(mailboxSession, "TenantInfoConfigurations", UserConfigurationTypes.Stream, true))
					{
						using (Stream stream = mailboxConfiguration.GetStream())
						{
							BinaryFormatter binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null);
							binaryFormatter.Serialize(stream, tenantInfo);
						}
						mailboxConfiguration.Save();
					}
				}
			}
			catch (StoragePermanentException innerException)
			{
				throw new SyncAgentPermanentException("TenantInfoProvider.Save failed with StoragePermanentException", innerException);
			}
			catch (StorageTransientException innerException2)
			{
				throw new SyncAgentTransientException("TenantInfoProvider.Save failed with StorageTransientException", innerException2);
			}
			catch (IOException innerException3)
			{
				throw new SyncAgentTransientException("TenantInfoProvider.Save failed with IOException", innerException3);
			}
		}

		public TenantInfo Load()
		{
			TenantInfo result;
			try
			{
				using (MailboxSession mailboxSession = MailboxSession.OpenAsAdmin(this.SyncMailboxPrincipal, CultureInfo.InvariantCulture, "Client=UnifiedPolicy;Action=CommitChanges;Interactive=False"))
				{
					using (UserConfiguration mailboxConfiguration = UserConfigurationHelper.GetMailboxConfiguration(mailboxSession, "TenantInfoConfigurations", UserConfigurationTypes.Stream, false))
					{
						if (mailboxConfiguration == null)
						{
							result = null;
						}
						else
						{
							using (Stream stream = mailboxConfiguration.GetStream())
							{
								if (stream == null || stream.Length == 0L)
								{
									result = null;
								}
								else
								{
									BinaryFormatter binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null);
									TenantInfo tenantInfo = (TenantInfo)binaryFormatter.Deserialize(stream);
									result = tenantInfo;
								}
							}
						}
					}
				}
			}
			catch (StoragePermanentException innerException)
			{
				throw new SyncAgentPermanentException("TenantInfoProvider.Load failed with StoragePermanentException", innerException);
			}
			catch (StorageTransientException innerException2)
			{
				throw new SyncAgentTransientException("TenantInfoProvider.Load failed with StorageTransientException", innerException2);
			}
			catch (IOException innerException3)
			{
				throw new SyncAgentTransientException("TenantInfoProvider.Load failed with IOException", innerException3);
			}
			return result;
		}

		public TenantInfoProvider(ExchangePrincipal syncMailboxPrincipal)
		{
			this.SyncMailboxPrincipal = syncMailboxPrincipal;
		}

		public void Dispose()
		{
		}
	}
}
