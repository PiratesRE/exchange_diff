using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DesignerCategory("code")]
	[Serializable]
	public class SyncFolderHierarchyResponseMessageType : ResponseMessageType
	{
		public string SyncState
		{
			get
			{
				return this.syncStateField;
			}
			set
			{
				this.syncStateField = value;
			}
		}

		public bool IncludesLastFolderInRange
		{
			get
			{
				return this.includesLastFolderInRangeField;
			}
			set
			{
				this.includesLastFolderInRangeField = value;
			}
		}

		[XmlIgnore]
		public bool IncludesLastFolderInRangeSpecified
		{
			get
			{
				return this.includesLastFolderInRangeFieldSpecified;
			}
			set
			{
				this.includesLastFolderInRangeFieldSpecified = value;
			}
		}

		public SyncFolderHierarchyChangesType Changes
		{
			get
			{
				return this.changesField;
			}
			set
			{
				this.changesField = value;
			}
		}

		private string syncStateField;

		private bool includesLastFolderInRangeField;

		private bool includesLastFolderInRangeFieldSpecified;

		private SyncFolderHierarchyChangesType changesField;
	}
}
