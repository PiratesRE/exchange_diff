using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DesignerCategory("code")]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class Device : DirectoryObject
	{
		internal override void ForEachProperty(IPropertyProcessor processor)
		{
		}

		[XmlElement(Order = 0)]
		public DirectoryPropertyBooleanSingle AccountEnabled
		{
			get
			{
				return this.accountEnabledField;
			}
			set
			{
				this.accountEnabledField = value;
			}
		}

		[XmlElement(Order = 1)]
		public DirectoryPropertyXmlAlternativeSecurityId AlternativeSecurityId
		{
			get
			{
				return this.alternativeSecurityIdField;
			}
			set
			{
				this.alternativeSecurityIdField = value;
			}
		}

		[XmlElement(Order = 2)]
		public DirectoryPropertyDateTimeSingle ApproximateLastLogonTimestamp
		{
			get
			{
				return this.approximateLastLogonTimestampField;
			}
			set
			{
				this.approximateLastLogonTimestampField = value;
			}
		}

		[XmlElement(Order = 3)]
		public DirectoryPropertyGuidSingle DeviceId
		{
			get
			{
				return this.deviceIdField;
			}
			set
			{
				this.deviceIdField = value;
			}
		}

		[XmlElement(Order = 4)]
		public DirectoryPropertyInt32Single DeviceObjectVersion
		{
			get
			{
				return this.deviceObjectVersionField;
			}
			set
			{
				this.deviceObjectVersionField = value;
			}
		}

		[XmlElement(Order = 5)]
		public DirectoryPropertyStringSingleLength1To1024 DeviceOSType
		{
			get
			{
				return this.deviceOSTypeField;
			}
			set
			{
				this.deviceOSTypeField = value;
			}
		}

		[XmlElement(Order = 6)]
		public DirectoryPropertyStringSingleLength1To512 DeviceOSVersion
		{
			get
			{
				return this.deviceOSVersionField;
			}
			set
			{
				this.deviceOSVersionField = value;
			}
		}

		[XmlElement(Order = 7)]
		public DirectoryPropertyStringLength1To1024 DevicePhysicalIds
		{
			get
			{
				return this.devicePhysicalIdsField;
			}
			set
			{
				this.devicePhysicalIdsField = value;
			}
		}

		[XmlElement(Order = 8)]
		public DirectoryPropertyBooleanSingle DirSyncEnabled
		{
			get
			{
				return this.dirSyncEnabledField;
			}
			set
			{
				this.dirSyncEnabledField = value;
			}
		}

		[XmlElement(Order = 9)]
		public DirectoryPropertyStringSingleLength1To256 DisplayName
		{
			get
			{
				return this.displayNameField;
			}
			set
			{
				this.displayNameField = value;
			}
		}

		[XmlElement(Order = 10)]
		public DirectoryPropertyStringLength1To100 ExchangeActiveSyncId
		{
			get
			{
				return this.exchangeActiveSyncIdField;
			}
			set
			{
				this.exchangeActiveSyncIdField = value;
			}
		}

		[XmlElement(Order = 11)]
		public DirectoryPropertyBooleanSingle IsCompliant
		{
			get
			{
				return this.isCompliantField;
			}
			set
			{
				this.isCompliantField = value;
			}
		}

		[XmlElement(Order = 12)]
		public DirectoryPropertyBooleanSingle IsManaged
		{
			get
			{
				return this.isManagedField;
			}
			set
			{
				this.isManagedField = value;
			}
		}

		[XmlElement(Order = 13)]
		public DirectoryPropertyStringSingleLength1To256 SourceAnchor
		{
			get
			{
				return this.sourceAnchorField;
			}
			set
			{
				this.sourceAnchorField = value;
			}
		}

		[XmlAnyAttribute]
		public XmlAttribute[] AnyAttr
		{
			get
			{
				return this.anyAttrField;
			}
			set
			{
				this.anyAttrField = value;
			}
		}

		private DirectoryPropertyBooleanSingle accountEnabledField;

		private DirectoryPropertyXmlAlternativeSecurityId alternativeSecurityIdField;

		private DirectoryPropertyDateTimeSingle approximateLastLogonTimestampField;

		private DirectoryPropertyGuidSingle deviceIdField;

		private DirectoryPropertyInt32Single deviceObjectVersionField;

		private DirectoryPropertyStringSingleLength1To1024 deviceOSTypeField;

		private DirectoryPropertyStringSingleLength1To512 deviceOSVersionField;

		private DirectoryPropertyStringLength1To1024 devicePhysicalIdsField;

		private DirectoryPropertyBooleanSingle dirSyncEnabledField;

		private DirectoryPropertyStringSingleLength1To256 displayNameField;

		private DirectoryPropertyStringLength1To100 exchangeActiveSyncIdField;

		private DirectoryPropertyBooleanSingle isCompliantField;

		private DirectoryPropertyBooleanSingle isManagedField;

		private DirectoryPropertyStringSingleLength1To256 sourceAnchorField;

		private XmlAttribute[] anyAttrField;
	}
}
