using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.Data.Services.Common;

namespace Microsoft.WindowsAzure.ActiveDirectoryV142
{
	[DataServiceKey("objectId")]
	public class Device : DirectoryObject
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public static Device CreateDevice(string objectId, Collection<AlternativeSecurityId> alternativeSecurityIds, Collection<string> devicePhysicalIds)
		{
			Device device = new Device();
			device.objectId = objectId;
			if (alternativeSecurityIds == null)
			{
				throw new ArgumentNullException("alternativeSecurityIds");
			}
			device.alternativeSecurityIds = alternativeSecurityIds;
			if (devicePhysicalIds == null)
			{
				throw new ArgumentNullException("devicePhysicalIds");
			}
			device.devicePhysicalIds = devicePhysicalIds;
			return device;
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public bool? accountEnabled
		{
			get
			{
				return this._accountEnabled;
			}
			set
			{
				this._accountEnabled = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<AlternativeSecurityId> alternativeSecurityIds
		{
			get
			{
				return this._alternativeSecurityIds;
			}
			set
			{
				this._alternativeSecurityIds = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public DateTime? approximateLastLogonTimestamp
		{
			get
			{
				return this._approximateLastLogonTimestamp;
			}
			set
			{
				this._approximateLastLogonTimestamp = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Guid? deviceId
		{
			get
			{
				return this._deviceId;
			}
			set
			{
				this._deviceId = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public int? deviceObjectVersion
		{
			get
			{
				return this._deviceObjectVersion;
			}
			set
			{
				this._deviceObjectVersion = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string deviceOSType
		{
			get
			{
				return this._deviceOSType;
			}
			set
			{
				this._deviceOSType = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string deviceOSVersion
		{
			get
			{
				return this._deviceOSVersion;
			}
			set
			{
				this._deviceOSVersion = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<string> devicePhysicalIds
		{
			get
			{
				return this._devicePhysicalIds;
			}
			set
			{
				this._devicePhysicalIds = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public bool? dirSyncEnabled
		{
			get
			{
				return this._dirSyncEnabled;
			}
			set
			{
				this._dirSyncEnabled = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string displayName
		{
			get
			{
				return this._displayName;
			}
			set
			{
				this._displayName = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public DateTime? lastDirSyncTime
		{
			get
			{
				return this._lastDirSyncTime;
			}
			set
			{
				this._lastDirSyncTime = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<DirectoryObject> registeredOwners
		{
			get
			{
				return this._registeredOwners;
			}
			set
			{
				if (value != null)
				{
					this._registeredOwners = value;
				}
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<DirectoryObject> registeredUsers
		{
			get
			{
				return this._registeredUsers;
			}
			set
			{
				if (value != null)
				{
					this._registeredUsers = value;
				}
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private bool? _accountEnabled;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<AlternativeSecurityId> _alternativeSecurityIds = new Collection<AlternativeSecurityId>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private DateTime? _approximateLastLogonTimestamp;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Guid? _deviceId;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private int? _deviceObjectVersion;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _deviceOSType;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _deviceOSVersion;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<string> _devicePhysicalIds = new Collection<string>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private bool? _dirSyncEnabled;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _displayName;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private DateTime? _lastDirSyncTime;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<DirectoryObject> _registeredOwners = new Collection<DirectoryObject>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<DirectoryObject> _registeredUsers = new Collection<DirectoryObject>();
	}
}
