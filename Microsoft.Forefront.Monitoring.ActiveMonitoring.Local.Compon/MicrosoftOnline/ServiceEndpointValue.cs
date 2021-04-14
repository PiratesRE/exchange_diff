using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[Serializable]
	public class ServiceEndpointValue
	{
		[XmlAnyElement]
		public XmlElement[] Any
		{
			get
			{
				return this.anyField;
			}
			set
			{
				this.anyField = value;
			}
		}

		[XmlAttribute]
		public string Name
		{
			get
			{
				return this.nameField;
			}
			set
			{
				this.nameField = value;
			}
		}

		[XmlAttribute]
		public string Address
		{
			get
			{
				return this.addressField;
			}
			set
			{
				this.addressField = value;
			}
		}

		[XmlAttribute]
		public int[] PartitionSubset
		{
			get
			{
				return this.partitionSubsetField;
			}
			set
			{
				this.partitionSubsetField = value;
			}
		}

		[XmlAttribute]
		public DateTime LastUpdatedTime
		{
			get
			{
				return this.lastUpdatedTimeField;
			}
			set
			{
				this.lastUpdatedTimeField = value;
			}
		}

		[XmlIgnore]
		public bool LastUpdatedTimeSpecified
		{
			get
			{
				return this.lastUpdatedTimeFieldSpecified;
			}
			set
			{
				this.lastUpdatedTimeFieldSpecified = value;
			}
		}

		private XmlElement[] anyField;

		private string nameField;

		private string addressField;

		private int[] partitionSubsetField;

		private DateTime lastUpdatedTimeField;

		private bool lastUpdatedTimeFieldSpecified;
	}
}
