using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.StatelessRequest
{
	[GeneratedCode("xsd", "2.0.50727.3038")]
	[DebuggerStepThrough]
	[XmlType(AnonymousType = true, Namespace = "DeltaSyncV2:")]
	[DesignerCategory("code")]
	[Serializable]
	public class StatelessCollectionGetSort
	{
		public string SortBy
		{
			get
			{
				return this.sortByField;
			}
			set
			{
				this.sortByField = value;
			}
		}

		public SortOrderType SortOrder
		{
			get
			{
				return this.sortOrderField;
			}
			set
			{
				this.sortOrderField = value;
			}
		}

		private string sortByField;

		private SortOrderType sortOrderField;
	}
}
