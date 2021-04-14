using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[Serializable]
	public class LicenseUnitsDetailValue
	{
		public LicenseUnitsDetailValue()
		{
			this.lockedOutField = 0;
		}

		[XmlAttribute]
		public int Enabled
		{
			get
			{
				return this.enabledField;
			}
			set
			{
				this.enabledField = value;
			}
		}

		[XmlAttribute]
		public int Warning
		{
			get
			{
				return this.warningField;
			}
			set
			{
				this.warningField = value;
			}
		}

		[XmlAttribute]
		public int Suspended
		{
			get
			{
				return this.suspendedField;
			}
			set
			{
				this.suspendedField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(0)]
		public int LockedOut
		{
			get
			{
				return this.lockedOutField;
			}
			set
			{
				this.lockedOutField = value;
			}
		}

		private int enabledField;

		private int warningField;

		private int suspendedField;

		private int lockedOutField;
	}
}
