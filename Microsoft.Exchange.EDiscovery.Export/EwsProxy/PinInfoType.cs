using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class PinInfoType
	{
		public string PIN
		{
			get
			{
				return this.pINField;
			}
			set
			{
				this.pINField = value;
			}
		}

		public bool IsValid
		{
			get
			{
				return this.isValidField;
			}
			set
			{
				this.isValidField = value;
			}
		}

		public bool PinExpired
		{
			get
			{
				return this.pinExpiredField;
			}
			set
			{
				this.pinExpiredField = value;
			}
		}

		public bool LockedOut
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

		public bool FirstTimeUser
		{
			get
			{
				return this.firstTimeUserField;
			}
			set
			{
				this.firstTimeUserField = value;
			}
		}

		private string pINField;

		private bool isValidField;

		private bool pinExpiredField;

		private bool lockedOutField;

		private bool firstTimeUserField;
	}
}
