using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Mserve.SettingsResponse
{
	[XmlType(Namespace = "HMSETTINGS:")]
	[GeneratedCode("xsd", "2.0.50727.1318")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class ServiceSettingsPropertiesType
	{
		public int ServiceTimeOut
		{
			get
			{
				return this.serviceTimeOutField;
			}
			set
			{
				this.serviceTimeOutField = value;
			}
		}

		public int MinSyncPollInterval
		{
			get
			{
				return this.minSyncPollIntervalField;
			}
			set
			{
				this.minSyncPollIntervalField = value;
			}
		}

		public int MinSettingsPollInterval
		{
			get
			{
				return this.minSettingsPollIntervalField;
			}
			set
			{
				this.minSettingsPollIntervalField = value;
			}
		}

		public double SyncMultiplier
		{
			get
			{
				return this.syncMultiplierField;
			}
			set
			{
				this.syncMultiplierField = value;
			}
		}

		public int MaxObjectsInSync
		{
			get
			{
				return this.maxObjectsInSyncField;
			}
			set
			{
				this.maxObjectsInSyncField = value;
			}
		}

		public int MaxNumberOfEmailAdds
		{
			get
			{
				return this.maxNumberOfEmailAddsField;
			}
			set
			{
				this.maxNumberOfEmailAddsField = value;
			}
		}

		public int MaxNumberOfFolderAdds
		{
			get
			{
				return this.maxNumberOfFolderAddsField;
			}
			set
			{
				this.maxNumberOfFolderAddsField = value;
			}
		}

		public int MaxNumberOfStatelessObjects
		{
			get
			{
				return this.maxNumberOfStatelessObjectsField;
			}
			set
			{
				this.maxNumberOfStatelessObjectsField = value;
			}
		}

		public int DefaultStatelessEmailWindowSize
		{
			get
			{
				return this.defaultStatelessEmailWindowSizeField;
			}
			set
			{
				this.defaultStatelessEmailWindowSizeField = value;
			}
		}

		public int MaxStatelessEmailWindowSize
		{
			get
			{
				return this.maxStatelessEmailWindowSizeField;
			}
			set
			{
				this.maxStatelessEmailWindowSizeField = value;
			}
		}

		public int MaxTotalLengthOfForwardingAddresses
		{
			get
			{
				return this.maxTotalLengthOfForwardingAddressesField;
			}
			set
			{
				this.maxTotalLengthOfForwardingAddressesField = value;
			}
		}

		public int MaxVacationResponseMessageLength
		{
			get
			{
				return this.maxVacationResponseMessageLengthField;
			}
			set
			{
				this.maxVacationResponseMessageLengthField = value;
			}
		}

		public string MinVacationResponseStartTime
		{
			get
			{
				return this.minVacationResponseStartTimeField;
			}
			set
			{
				this.minVacationResponseStartTimeField = value;
			}
		}

		public string MaxVacationResponseEndTime
		{
			get
			{
				return this.maxVacationResponseEndTimeField;
			}
			set
			{
				this.maxVacationResponseEndTimeField = value;
			}
		}

		public int MaxNumberOfProvisionCommands
		{
			get
			{
				return this.maxNumberOfProvisionCommandsField;
			}
			set
			{
				this.maxNumberOfProvisionCommandsField = value;
			}
		}

		private int serviceTimeOutField;

		private int minSyncPollIntervalField;

		private int minSettingsPollIntervalField;

		private double syncMultiplierField;

		private int maxObjectsInSyncField;

		private int maxNumberOfEmailAddsField;

		private int maxNumberOfFolderAddsField;

		private int maxNumberOfStatelessObjectsField;

		private int defaultStatelessEmailWindowSizeField;

		private int maxStatelessEmailWindowSizeField;

		private int maxTotalLengthOfForwardingAddressesField;

		private int maxVacationResponseMessageLengthField;

		private string minVacationResponseStartTimeField;

		private string maxVacationResponseEndTimeField;

		private int maxNumberOfProvisionCommandsField;
	}
}
