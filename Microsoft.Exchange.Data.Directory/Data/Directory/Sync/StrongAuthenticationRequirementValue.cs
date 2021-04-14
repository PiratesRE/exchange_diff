using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DebuggerStepThrough]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DesignerCategory("code")]
	[Serializable]
	public class StrongAuthenticationRequirementValue
	{
		[XmlAttribute(DataType = "token")]
		public string RelyingParty
		{
			get
			{
				return this.relyingPartyField;
			}
			set
			{
				this.relyingPartyField = value;
			}
		}

		[XmlAttribute]
		public int State
		{
			get
			{
				return this.stateField;
			}
			set
			{
				this.stateField = value;
			}
		}

		[XmlIgnore]
		public bool StateSpecified
		{
			get
			{
				return this.stateFieldSpecified;
			}
			set
			{
				this.stateFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public DateTime RememberDevicesNotIssuedBefore
		{
			get
			{
				return this.rememberDevicesNotIssuedBeforeField;
			}
			set
			{
				this.rememberDevicesNotIssuedBeforeField = value;
			}
		}

		[XmlIgnore]
		public bool RememberDevicesNotIssuedBeforeSpecified
		{
			get
			{
				return this.rememberDevicesNotIssuedBeforeFieldSpecified;
			}
			set
			{
				this.rememberDevicesNotIssuedBeforeFieldSpecified = value;
			}
		}

		private string relyingPartyField;

		private int stateField;

		private bool stateFieldSpecified;

		private DateTime rememberDevicesNotIssuedBeforeField;

		private bool rememberDevicesNotIssuedBeforeFieldSpecified;
	}
}
