using System;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Connections.Eas;
using Microsoft.Exchange.Connections.Eas.Commands.Connect;
using Microsoft.Exchange.Connections.Eas.Commands.Disconnect;
using Microsoft.Exchange.Connections.Eas.Commands.FolderCreate;
using Microsoft.Exchange.Connections.Eas.Commands.FolderDelete;
using Microsoft.Exchange.Connections.Eas.Commands.FolderSync;
using Microsoft.Exchange.Connections.Eas.Commands.FolderUpdate;
using Microsoft.Exchange.Connections.Eas.Commands.GetItemEstimate;
using Microsoft.Exchange.Connections.Eas.Commands.ItemOperations;
using Microsoft.Exchange.Connections.Eas.Commands.MoveItems;
using Microsoft.Exchange.Connections.Eas.Commands.Options;
using Microsoft.Exchange.Connections.Eas.Commands.SendMail;
using Microsoft.Exchange.Connections.Eas.Commands.Settings;
using Microsoft.Exchange.Connections.Eas.Commands.Sync;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class EasCrawlerConnection : IEasConnection, IConnection<IEasConnection>
	{
		internal EasCrawlerConnection(EasConnectionParameters connectionParameters, EasAuthenticationParameters authenticationParameters, EasDeviceParameters deviceParameters)
		{
			EasDeviceParameters deviceParameters2 = new EasDeviceParameters("FEDCBA9876543210", deviceParameters);
			this.innerConnection = EasConnection.CreateInstance(connectionParameters, authenticationParameters, deviceParameters2);
		}

		string IEasConnection.ServerName
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		UserSmtpAddress IEasConnection.UserSmtpAddress
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		IEasConnection IConnection<IEasConnection>.Initialize()
		{
			throw new NotImplementedException();
		}

		ConnectResponse IEasConnection.Connect(ConnectRequest connectRequest, IServerCapabilities capabilities)
		{
			return this.innerConnection.Connect(connectRequest, null);
		}

		DisconnectResponse IEasConnection.Disconnect(DisconnectRequest disconnectRequest)
		{
			return this.innerConnection.Disconnect(disconnectRequest);
		}

		FolderCreateResponse IEasConnection.FolderCreate(FolderCreateRequest folderCreateRequest)
		{
			throw new NotImplementedException();
		}

		FolderDeleteResponse IEasConnection.FolderDelete(FolderDeleteRequest folderDeleteRequest)
		{
			throw new NotImplementedException();
		}

		FolderSyncResponse IEasConnection.FolderSync(FolderSyncRequest folderSyncRequest)
		{
			throw new NotImplementedException();
		}

		FolderUpdateResponse IEasConnection.FolderUpdate(FolderUpdateRequest folderUpdateRequest)
		{
			throw new NotImplementedException();
		}

		GetItemEstimateResponse IEasConnection.GetItemEstimate(GetItemEstimateRequest getItemEstimateRequest)
		{
			return this.innerConnection.GetItemEstimate(getItemEstimateRequest);
		}

		ItemOperationsResponse IEasConnection.ItemOperations(ItemOperationsRequest itemOperationsRequest)
		{
			throw new NotImplementedException();
		}

		MoveItemsResponse IEasConnection.MoveItems(MoveItemsRequest moveItemsRequest)
		{
			throw new NotImplementedException();
		}

		OptionsResponse IEasConnection.Options(OptionsRequest optionsRequest)
		{
			throw new NotImplementedException();
		}

		SendMailResponse IEasConnection.SendMail(SendMailRequest sendMailRequest)
		{
			throw new NotImplementedException();
		}

		SettingsResponse IEasConnection.Settings(SettingsRequest settingsRequest)
		{
			throw new NotImplementedException();
		}

		SyncResponse IEasConnection.Sync(SyncRequest syncRequest)
		{
			SyncResponse result;
			try
			{
				result = this.innerConnection.Sync(syncRequest);
			}
			catch (EasRequiresFolderSyncException)
			{
				this.innerConnection.FolderSync(FolderSyncRequest.InitialSyncRequest);
				result = this.innerConnection.Sync(syncRequest);
			}
			return result;
		}

		OperationStatusCode IEasConnection.TestLogon()
		{
			throw new NotImplementedException("This kind of connection does not support TestLogon functionality. Please use EasConnection if you need this functionality.");
		}

		private const string CrawlerDeviceId = "FEDCBA9876543210";

		private readonly IEasConnection innerConnection;
	}
}
