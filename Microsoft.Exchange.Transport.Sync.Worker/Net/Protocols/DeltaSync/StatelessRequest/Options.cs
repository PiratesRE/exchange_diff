using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.StatelessRequest
{
	[GeneratedCode("xsd", "2.0.50727.3038")]
	[DesignerCategory("code")]
	[XmlRoot(Namespace = "HMSYNC:", IsNullable = false)]
	[DebuggerStepThrough]
	[XmlType(AnonymousType = true, Namespace = "HMSYNC:")]
	[Serializable]
	public class Options
	{
		public byte Conflict
		{
			get
			{
				return this.conflictField;
			}
			set
			{
				this.conflictField = value;
			}
		}

		[XmlIgnore]
		public bool ConflictSpecified
		{
			get
			{
				return this.conflictFieldSpecified;
			}
			set
			{
				this.conflictFieldSpecified = value;
			}
		}

		private byte conflictField;

		private bool conflictFieldSpecified;
	}
}
