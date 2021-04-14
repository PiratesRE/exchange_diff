using System;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Servicelets.AuthAdmin.Messages;

namespace Microsoft.Exchange.Servicelets.AuthAdmin
{
	internal class AuthAdminAuthConfig
	{
		internal AuthAdminAuthConfig(AuthAdminContext context, WaitHandle stopEvent)
		{
			this.Context = context;
			this.StopEvent = stopEvent;
		}

		private protected AuthAdminContext Context { protected get; private set; }

		private protected WaitHandle StopEvent { protected get; private set; }

		internal void DoScheduledWork(ITopologyConfigurationSession session)
		{
			if (this.StopEvent.WaitOne(0, false))
			{
				return;
			}
			this.Context.Logger.Log(MigrationEventType.Information, "Starting Authentication Configuration task", new object[0]);
			AuthConfig authConfig = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				authConfig = AuthConfig.Read(session);
			});
			if (adoperationResult != ADOperationResult.Success)
			{
				this.Context.Logger.Log(MigrationEventType.Warning, "Unable to read Auth Config, result = {0}", new object[]
				{
					adoperationResult.ErrorCode.ToString()
				});
				if (adoperationResult.Exception is TransientException)
				{
					throw adoperationResult.Exception;
				}
				return;
			}
			else
			{
				if (authConfig == null)
				{
					throw new InvalidAuthConfigurationException(DirectoryStrings.ErrorInvalidAuthSettings(string.Empty));
				}
				if (authConfig.NextCertificateEffectiveDate == null || authConfig.NextCertificateEffectiveDate == null)
				{
					this.Context.Logger.Log(MigrationEventType.Information, "Next effective date is not set. Task complete", new object[0]);
					return;
				}
				ExDateTime exDateTime = (ExDateTime)authConfig.NextCertificateEffectiveDate.Value;
				if (exDateTime.ToUtc() > ExDateTime.UtcNow)
				{
					this.Context.Logger.Log(MigrationEventType.Information, "Next effective date has not yet occurred: {0}.  Task complete", new object[]
					{
						exDateTime.ToUtc().ToString()
					});
					return;
				}
				this.Context.Logger.Log(MigrationEventType.Information, "Next effective date {0} has occured, performing automatic certificate publish", new object[]
				{
					exDateTime.ToUtc().ToString()
				});
				if (string.IsNullOrEmpty(authConfig.NextCertificateThumbprint) || string.Compare(authConfig.NextCertificateThumbprint, authConfig.CurrentCertificateThumbprint, StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.Context.Logger.Log(MigrationEventType.Warning, "Next effective certificate thumbprint not set or same as current thumbprint, ignoring", new object[0]);
					authConfig.NextCertificateThumbprint = null;
					authConfig.NextCertificateEffectiveDate = null;
				}
				else
				{
					authConfig.PreviousCertificateThumbprint = authConfig.CurrentCertificateThumbprint;
					authConfig.CurrentCertificateThumbprint = authConfig.NextCertificateThumbprint;
					authConfig.NextCertificateThumbprint = null;
					authConfig.NextCertificateEffectiveDate = null;
				}
				session.Save(authConfig);
				this.Context.Logger.LogTerseEvent(MigrationEventType.Information, MSExchangeAuthAdminEventLogConstants.Tuple_CurrentSigningUpdated, new string[]
				{
					authConfig.CurrentCertificateThumbprint
				});
				return;
			}
		}
	}
}
