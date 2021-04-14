using System;
using System.Collections.Generic;
using Microsoft.Exchange.AirSync.SchemaConverter;
using Microsoft.Exchange.Diagnostics.Components.AirSync;

namespace Microsoft.Exchange.AirSync
{
	internal abstract class SyncBase : Command
	{
		public SyncBase()
		{
			this.collections = new Dictionary<string, SyncCollection>();
		}

		public string DevicePhoneNumberForSms { get; set; }

		public bool DeviceEnableOutboundSMS { get; set; }

		internal IAirSyncVersionFactory VersionFactory
		{
			get
			{
				return this.versionFactory;
			}
			set
			{
				this.versionFactory = value;
			}
		}

		internal SyncBase.ErrorCodeStatus GlobalStatus
		{
			get
			{
				return this.globalStatus;
			}
			set
			{
				this.globalStatus = value;
			}
		}

		internal Exception LastException
		{
			get
			{
				return this.lastException;
			}
			set
			{
				this.lastException = value;
			}
		}

		internal uint GlobalWindowSize
		{
			get
			{
				return this.globalWindowSize;
			}
			set
			{
				if (value > (uint)GlobalSettings.MaxWindowSize)
				{
					this.globalWindowSize = (uint)GlobalSettings.MaxWindowSize;
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Window size is capped to: " + this.globalWindowSize);
					return;
				}
				this.globalWindowSize = value;
			}
		}

		internal Dictionary<string, SyncCollection> Collections
		{
			get
			{
				return this.collections;
			}
		}

		internal int DeviceSettingsHash
		{
			get
			{
				if (this.deviceSettingsHash == 0)
				{
					this.deviceSettingsHash = (this.DeviceEnableOutboundSMS.GetHashCode() ^ ((this.DevicePhoneNumberForSms == null) ? 0 : this.DevicePhoneNumberForSms.GetHashCode()));
				}
				return this.deviceSettingsHash;
			}
		}

		internal override Command.ExecutionState ExecuteCommand()
		{
			throw new InvalidOperationException("Microsoft.Exchange.AirSync.SyncBase is not an executable command class");
		}

		internal override void SetStateData(Command.StateData data)
		{
			this.DevicePhoneNumberForSms = data.DevicePhoneNumberForSms;
			this.DeviceEnableOutboundSMS = data.DeviceEnableOutboundSMS;
		}

		protected abstract string GetStatusString(SyncBase.ErrorCodeStatus error);

		protected void InitializeVersionFactory(int version)
		{
			if (this.versionFactory == null)
			{
				this.versionFactory = AirSyncProtocolVersionParserBuilder.FromVersion(version);
			}
		}

		private const int RootFolderHierarchyListMaxCapacity = 1000;

		private const int RootFolderHierarchyListInitialSize = 100;

		private IAirSyncVersionFactory versionFactory;

		private SyncBase.ErrorCodeStatus globalStatus = SyncBase.ErrorCodeStatus.Success;

		private Exception lastException;

		private uint globalWindowSize = uint.MaxValue;

		private Dictionary<string, SyncCollection> collections = new Dictionary<string, SyncCollection>();

		private int deviceSettingsHash;

		internal enum ErrorCodeStatus
		{
			Success = 1,
			ProtocolVersionMismatch,
			InvalidSyncKey,
			ProtocolError,
			ServerError,
			ClientServerConversion,
			Conflict,
			ObjectNotFound,
			OutOfDisk,
			NotificationGUID,
			NotificationsNotProvisioned,
			InvalidCollection,
			UnprimedSyncState
		}

		internal enum SyncCommandType
		{
			Invalid,
			Add,
			Change,
			Delete,
			SoftDelete,
			Fetch
		}
	}
}
