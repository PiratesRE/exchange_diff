using System;
using Microsoft.Exchange.Connections.Common;
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

namespace Microsoft.Exchange.Connections.Eas
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	public interface IEasConnection : IConnection<IEasConnection>
	{
		string ServerName { get; }

		UserSmtpAddress UserSmtpAddress { get; }

		ConnectResponse Connect(ConnectRequest connectRequest, IServerCapabilities capabilities = null);

		OperationStatusCode TestLogon();

		DisconnectResponse Disconnect(DisconnectRequest disconnectRequest);

		FolderCreateResponse FolderCreate(FolderCreateRequest folderCreateRequest);

		FolderDeleteResponse FolderDelete(FolderDeleteRequest folderDeleteRequest);

		FolderSyncResponse FolderSync(FolderSyncRequest folderSyncRequest);

		FolderUpdateResponse FolderUpdate(FolderUpdateRequest folderUpdateRequest);

		GetItemEstimateResponse GetItemEstimate(GetItemEstimateRequest getItemEstimateRequest);

		ItemOperationsResponse ItemOperations(ItemOperationsRequest itemOperationsRequest);

		MoveItemsResponse MoveItems(MoveItemsRequest moveItemsRequest);

		OptionsResponse Options(OptionsRequest optionsRequest);

		SendMailResponse SendMail(SendMailRequest sendMailRequest);

		SettingsResponse Settings(SettingsRequest settingsRequest);

		SyncResponse Sync(SyncRequest syncRequest);
	}
}
