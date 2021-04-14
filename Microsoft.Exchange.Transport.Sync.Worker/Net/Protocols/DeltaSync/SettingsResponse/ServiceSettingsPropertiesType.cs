using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsResponse
{
	[XmlType(TypeName = "ServiceSettingsPropertiesType", Namespace = "HMSETTINGS:")]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Serializable]
	public class ServiceSettingsPropertiesType
	{
		[XmlIgnore]
		public int ServiceTimeOut
		{
			get
			{
				return this.internalServiceTimeOut;
			}
			set
			{
				this.internalServiceTimeOut = value;
				this.internalServiceTimeOutSpecified = true;
			}
		}

		[XmlIgnore]
		public int MinSyncPollInterval
		{
			get
			{
				return this.internalMinSyncPollInterval;
			}
			set
			{
				this.internalMinSyncPollInterval = value;
				this.internalMinSyncPollIntervalSpecified = true;
			}
		}

		[XmlIgnore]
		public int MinSettingsPollInterval
		{
			get
			{
				return this.internalMinSettingsPollInterval;
			}
			set
			{
				this.internalMinSettingsPollInterval = value;
				this.internalMinSettingsPollIntervalSpecified = true;
			}
		}

		[XmlIgnore]
		public double SyncMultiplier
		{
			get
			{
				return this.internalSyncMultiplier;
			}
			set
			{
				this.internalSyncMultiplier = value;
				this.internalSyncMultiplierSpecified = true;
			}
		}

		[XmlIgnore]
		public int MaxObjectsInSync
		{
			get
			{
				return this.internalMaxObjectsInSync;
			}
			set
			{
				this.internalMaxObjectsInSync = value;
				this.internalMaxObjectsInSyncSpecified = true;
			}
		}

		[XmlIgnore]
		public int MaxNumberOfEmailAdds
		{
			get
			{
				return this.internalMaxNumberOfEmailAdds;
			}
			set
			{
				this.internalMaxNumberOfEmailAdds = value;
				this.internalMaxNumberOfEmailAddsSpecified = true;
			}
		}

		[XmlIgnore]
		public int MaxNumberOfFolderAdds
		{
			get
			{
				return this.internalMaxNumberOfFolderAdds;
			}
			set
			{
				this.internalMaxNumberOfFolderAdds = value;
				this.internalMaxNumberOfFolderAddsSpecified = true;
			}
		}

		[XmlIgnore]
		public int MaxNumberOfStatelessObjects
		{
			get
			{
				return this.internalMaxNumberOfStatelessObjects;
			}
			set
			{
				this.internalMaxNumberOfStatelessObjects = value;
				this.internalMaxNumberOfStatelessObjectsSpecified = true;
			}
		}

		[XmlIgnore]
		public int DefaultStatelessEmailWindowSize
		{
			get
			{
				return this.internalDefaultStatelessEmailWindowSize;
			}
			set
			{
				this.internalDefaultStatelessEmailWindowSize = value;
				this.internalDefaultStatelessEmailWindowSizeSpecified = true;
			}
		}

		[XmlIgnore]
		public int MaxStatelessEmailWindowSize
		{
			get
			{
				return this.internalMaxStatelessEmailWindowSize;
			}
			set
			{
				this.internalMaxStatelessEmailWindowSize = value;
				this.internalMaxStatelessEmailWindowSizeSpecified = true;
			}
		}

		[XmlIgnore]
		public int MaxTotalLengthOfForwardingAddresses
		{
			get
			{
				return this.internalMaxTotalLengthOfForwardingAddresses;
			}
			set
			{
				this.internalMaxTotalLengthOfForwardingAddresses = value;
				this.internalMaxTotalLengthOfForwardingAddressesSpecified = true;
			}
		}

		[XmlIgnore]
		public int MaxVacationResponseMessageLength
		{
			get
			{
				return this.internalMaxVacationResponseMessageLength;
			}
			set
			{
				this.internalMaxVacationResponseMessageLength = value;
				this.internalMaxVacationResponseMessageLengthSpecified = true;
			}
		}

		[XmlIgnore]
		public string MinVacationResponseStartTime
		{
			get
			{
				return this.internalMinVacationResponseStartTime;
			}
			set
			{
				this.internalMinVacationResponseStartTime = value;
			}
		}

		[XmlIgnore]
		public string MaxVacationResponseEndTime
		{
			get
			{
				return this.internalMaxVacationResponseEndTime;
			}
			set
			{
				this.internalMaxVacationResponseEndTime = value;
			}
		}

		[XmlIgnore]
		public int MaxNumberOfProvisionCommands
		{
			get
			{
				return this.internalMaxNumberOfProvisionCommands;
			}
			set
			{
				this.internalMaxNumberOfProvisionCommands = value;
				this.internalMaxNumberOfProvisionCommandsSpecified = true;
			}
		}

		[XmlElement(ElementName = "ServiceTimeOut", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int", Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public int internalServiceTimeOut;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlIgnore]
		public bool internalServiceTimeOutSpecified;

		[XmlElement(ElementName = "MinSyncPollInterval", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int", Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public int internalMinSyncPollInterval;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlIgnore]
		public bool internalMinSyncPollIntervalSpecified;

		[XmlElement(ElementName = "MinSettingsPollInterval", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int", Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public int internalMinSettingsPollInterval;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlIgnore]
		public bool internalMinSettingsPollIntervalSpecified;

		[XmlElement(ElementName = "SyncMultiplier", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double", Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public double internalSyncMultiplier;

		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool internalSyncMultiplierSpecified;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "MaxObjectsInSync", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int", Namespace = "HMSETTINGS:")]
		public int internalMaxObjectsInSync;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlIgnore]
		public bool internalMaxObjectsInSyncSpecified;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "MaxNumberOfEmailAdds", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int", Namespace = "HMSETTINGS:")]
		public int internalMaxNumberOfEmailAdds;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlIgnore]
		public bool internalMaxNumberOfEmailAddsSpecified;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "MaxNumberOfFolderAdds", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int", Namespace = "HMSETTINGS:")]
		public int internalMaxNumberOfFolderAdds;

		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool internalMaxNumberOfFolderAddsSpecified;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "MaxNumberOfStatelessObjects", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int", Namespace = "HMSETTINGS:")]
		public int internalMaxNumberOfStatelessObjects;

		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool internalMaxNumberOfStatelessObjectsSpecified;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "DefaultStatelessEmailWindowSize", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int", Namespace = "HMSETTINGS:")]
		public int internalDefaultStatelessEmailWindowSize;

		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool internalDefaultStatelessEmailWindowSizeSpecified;

		[XmlElement(ElementName = "MaxStatelessEmailWindowSize", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int", Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public int internalMaxStatelessEmailWindowSize;

		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool internalMaxStatelessEmailWindowSizeSpecified;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "MaxTotalLengthOfForwardingAddresses", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int", Namespace = "HMSETTINGS:")]
		public int internalMaxTotalLengthOfForwardingAddresses;

		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool internalMaxTotalLengthOfForwardingAddressesSpecified;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "MaxVacationResponseMessageLength", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int", Namespace = "HMSETTINGS:")]
		public int internalMaxVacationResponseMessageLength;

		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool internalMaxVacationResponseMessageLengthSpecified;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "MinVacationResponseStartTime", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string", Namespace = "HMSETTINGS:")]
		public string internalMinVacationResponseStartTime;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "MaxVacationResponseEndTime", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string", Namespace = "HMSETTINGS:")]
		public string internalMaxVacationResponseEndTime;

		[XmlElement(ElementName = "MaxNumberOfProvisionCommands", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int", Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public int internalMaxNumberOfProvisionCommands;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlIgnore]
		public bool internalMaxNumberOfProvisionCommandsSpecified;
	}
}
