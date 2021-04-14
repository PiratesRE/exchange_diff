using System;
using System.Net;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Connections.Eas.Commands;
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
	public sealed class EasConnection : IEasConnection, IConnection<IEasConnection>
	{
		private EasConnection(EasConnectionParameters connectionParameters, EasAuthenticationParameters authenticationParameters, EasDeviceParameters deviceParameters)
		{
			this.ConnectionParameters = connectionParameters;
			this.AuthenticationParameters = authenticationParameters;
			this.DeviceParameters = deviceParameters;
		}

		public string ServerName
		{
			get
			{
				return this.EasEndpointSettings.Domain;
			}
		}

		public UserSmtpAddress UserSmtpAddress
		{
			get
			{
				return this.AuthenticationParameters.UserSmtpAddress;
			}
		}

		internal EasEndpointSettings EasEndpointSettings { get; set; }

		private EasConnectionParameters ConnectionParameters { get; set; }

		private EasDeviceParameters DeviceParameters { get; set; }

		private EasAuthenticationParameters AuthenticationParameters { get; set; }

		private EasConnectionSettings EasConnectionSettings { get; set; }

		public static IEasConnection CreateInstance(EasConnectionParameters connectionParameters, EasAuthenticationParameters authenticationParameters, EasDeviceParameters deviceParameters)
		{
			return EasConnection.hookableFactory.Value(connectionParameters, authenticationParameters, deviceParameters);
		}

		public IEasConnection Initialize()
		{
			this.EasEndpointSettings = new EasEndpointSettings(this.AuthenticationParameters);
			this.EasConnectionSettings = new EasConnectionSettings(this.EasEndpointSettings, this.ConnectionParameters, this.AuthenticationParameters, this.DeviceParameters);
			return this;
		}

		ConnectResponse IEasConnection.Connect(ConnectRequest connectRequest, IServerCapabilities capabilities)
		{
			ConnectCommand connectCommand = new ConnectCommand(this.EasConnectionSettings);
			return connectCommand.Execute(connectRequest, capabilities ?? EasConnection.DefaultCapabilities);
		}

		OperationStatusCode IEasConnection.TestLogon()
		{
			ConnectStatus connectStatus;
			HttpStatus httpStatus;
			try
			{
				ConnectResponse connectResponse = ((IEasConnection)this).Connect(ConnectRequest.Default, null);
				connectStatus = connectResponse.ConnectStatus;
				httpStatus = connectResponse.HttpStatus;
			}
			catch (WebException ex)
			{
				HttpWebResponse httpWebResponse = ex.Response as HttpWebResponse;
				connectStatus = ConnectStatus.IsPermanent;
				httpStatus = (HttpStatus)httpWebResponse.StatusCode;
			}
			((IEasConnection)this).Disconnect(DisconnectRequest.Default);
			if (connectStatus == ConnectStatus.Success)
			{
				return OperationStatusCode.Success;
			}
			if (connectStatus == ConnectStatus.AutodiscoverFailed)
			{
				return OperationStatusCode.ErrorInvalidRemoteServer;
			}
			HttpStatus httpStatus2 = httpStatus;
			switch (httpStatus2)
			{
			case HttpStatus.Unauthorized:
			case HttpStatus.Forbidden:
			case HttpStatus.NotFound:
				break;
			case (HttpStatus)402:
				return OperationStatusCode.ErrorCannotCommunicateWithRemoteServer;
			default:
				if (httpStatus2 != HttpStatus.NeedProvisioning)
				{
					switch (httpStatus2)
					{
					case HttpStatus.ProxyError:
					case HttpStatus.ServiceUnavailable:
						return OperationStatusCode.ErrorInvalidRemoteServer;
					case (HttpStatus)504:
						return OperationStatusCode.ErrorCannotCommunicateWithRemoteServer;
					case HttpStatus.VersionNotSupported:
						return OperationStatusCode.ErrorUnsupportedProtocolVersion;
					default:
						return OperationStatusCode.ErrorCannotCommunicateWithRemoteServer;
					}
				}
				break;
			}
			return OperationStatusCode.ErrorInvalidCredentials;
		}

		DisconnectResponse IEasConnection.Disconnect(DisconnectRequest disconnectRequest)
		{
			DisconnectCommand disconnectCommand = new DisconnectCommand(this.EasConnectionSettings);
			return disconnectCommand.Execute(disconnectRequest);
		}

		FolderCreateResponse IEasConnection.FolderCreate(FolderCreateRequest folderCreateRequest)
		{
			FolderCreateCommand folderCreateCommand = new FolderCreateCommand(this.EasConnectionSettings);
			return folderCreateCommand.Execute(folderCreateRequest);
		}

		FolderDeleteResponse IEasConnection.FolderDelete(FolderDeleteRequest folderDeleteRequest)
		{
			FolderDeleteCommand folderDeleteCommand = new FolderDeleteCommand(this.EasConnectionSettings);
			return folderDeleteCommand.Execute(folderDeleteRequest);
		}

		FolderSyncResponse IEasConnection.FolderSync(FolderSyncRequest folderSyncRequest)
		{
			FolderSyncCommand folderSyncCommand = new FolderSyncCommand(this.EasConnectionSettings);
			return folderSyncCommand.Execute(folderSyncRequest);
		}

		FolderUpdateResponse IEasConnection.FolderUpdate(FolderUpdateRequest folderUpdateRequest)
		{
			FolderUpdateCommand folderUpdateCommand = new FolderUpdateCommand(this.EasConnectionSettings);
			return folderUpdateCommand.Execute(folderUpdateRequest);
		}

		GetItemEstimateResponse IEasConnection.GetItemEstimate(GetItemEstimateRequest getItemEstimateRequest)
		{
			GetItemEstimateCommand getItemEstimateCommand = new GetItemEstimateCommand(this.EasConnectionSettings);
			return getItemEstimateCommand.Execute(getItemEstimateRequest);
		}

		ItemOperationsResponse IEasConnection.ItemOperations(ItemOperationsRequest itemOperationsRequest)
		{
			ItemOperationsCommand itemOperationsCommand = new ItemOperationsCommand(this.EasConnectionSettings);
			return itemOperationsCommand.Execute(itemOperationsRequest);
		}

		MoveItemsResponse IEasConnection.MoveItems(MoveItemsRequest moveItemsRequest)
		{
			MoveItemsCommand moveItemsCommand = new MoveItemsCommand(this.EasConnectionSettings);
			return moveItemsCommand.Execute(moveItemsRequest);
		}

		OptionsResponse IEasConnection.Options(OptionsRequest optionsRequest)
		{
			OptionsCommand optionsCommand = new OptionsCommand(this.EasConnectionSettings);
			return optionsCommand.Execute(optionsRequest);
		}

		SendMailResponse IEasConnection.SendMail(SendMailRequest sendMailRequest)
		{
			SendMailCommand sendMailCommand = new SendMailCommand(this.EasConnectionSettings);
			return sendMailCommand.Execute(sendMailRequest);
		}

		SettingsResponse IEasConnection.Settings(SettingsRequest settingsRequest)
		{
			SettingsCommand settingsCommand = new SettingsCommand(this.EasConnectionSettings);
			return settingsCommand.Execute(settingsRequest);
		}

		SyncResponse IEasConnection.Sync(SyncRequest syncRequest)
		{
			SyncCommand syncCommand = new SyncCommand(this.EasConnectionSettings);
			return syncCommand.Execute(syncRequest);
		}

		internal static IDisposable SetTestHook(Func<EasConnectionParameters, EasAuthenticationParameters, EasDeviceParameters, IEasConnection> newFactory)
		{
			return EasConnection.hookableFactory.SetTestHook(newFactory);
		}

		private static IEasConnection Factory(EasConnectionParameters connectionParameters, EasAuthenticationParameters authenticationParameters, EasDeviceParameters deviceParameters)
		{
			return new EasConnection(connectionParameters, authenticationParameters, deviceParameters).Initialize();
		}

		private static readonly EasServerCapabilities DefaultCapabilities = new EasServerCapabilities(new string[]
		{
			"FolderCreate",
			"FolderDelete",
			"FolderSync",
			"FolderUpdate",
			"GetItemEstimate",
			"ItemOperations",
			"MoveItems",
			"SendMail",
			"Settings",
			"Sync"
		});

		private static Hookable<Func<EasConnectionParameters, EasAuthenticationParameters, EasDeviceParameters, IEasConnection>> hookableFactory = Hookable<Func<EasConnectionParameters, EasAuthenticationParameters, EasDeviceParameters, IEasConnection>>.Create(true, new Func<EasConnectionParameters, EasAuthenticationParameters, EasDeviceParameters, IEasConnection>(EasConnection.Factory));
	}
}
