using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.StatelessRequest
{
	[DebuggerStepThrough]
	[XmlType(AnonymousType = true, Namespace = "DeltaSyncV2:")]
	[GeneratedCode("xsd", "2.0.50727.3038")]
	[DesignerCategory("code")]
	[Serializable]
	public class StatelessCollectionGet
	{
		public StatelessCollectionGetFilter Filter
		{
			get
			{
				return this.filterField;
			}
			set
			{
				this.filterField = value;
			}
		}

		public StatelessCollectionGetSort Sort
		{
			get
			{
				return this.sortField;
			}
			set
			{
				this.sortField = value;
			}
		}

		private StatelessCollectionGetFilter filterField;

		private StatelessCollectionGetSort sortField;
	}
}
