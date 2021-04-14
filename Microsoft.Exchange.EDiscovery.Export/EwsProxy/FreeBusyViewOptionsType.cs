using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class FreeBusyViewOptionsType
	{
		public Duration TimeWindow
		{
			get
			{
				return this.timeWindowField;
			}
			set
			{
				this.timeWindowField = value;
			}
		}

		public int MergedFreeBusyIntervalInMinutes
		{
			get
			{
				return this.mergedFreeBusyIntervalInMinutesField;
			}
			set
			{
				this.mergedFreeBusyIntervalInMinutesField = value;
			}
		}

		[XmlIgnore]
		public bool MergedFreeBusyIntervalInMinutesSpecified
		{
			get
			{
				return this.mergedFreeBusyIntervalInMinutesFieldSpecified;
			}
			set
			{
				this.mergedFreeBusyIntervalInMinutesFieldSpecified = value;
			}
		}

		public FreeBusyViewType RequestedView
		{
			get
			{
				return this.requestedViewField;
			}
			set
			{
				this.requestedViewField = value;
			}
		}

		[XmlIgnore]
		public bool RequestedViewSpecified
		{
			get
			{
				return this.requestedViewFieldSpecified;
			}
			set
			{
				this.requestedViewFieldSpecified = value;
			}
		}

		private Duration timeWindowField;

		private int mergedFreeBusyIntervalInMinutesField;

		private bool mergedFreeBusyIntervalInMinutesFieldSpecified;

		private FreeBusyViewType requestedViewField;

		private bool requestedViewFieldSpecified;
	}
}
