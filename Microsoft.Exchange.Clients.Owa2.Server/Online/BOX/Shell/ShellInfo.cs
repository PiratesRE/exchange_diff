using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.BOX.Shell
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	[DataContract(Name = "ShellInfo", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.Shell")]
	public class ShellInfo : NavBarInfo
	{
		[DataMember]
		public string SharedCSSTouchDeviceUrl
		{
			get
			{
				return this.SharedCSSTouchDeviceUrlField;
			}
			set
			{
				this.SharedCSSTouchDeviceUrlField = value;
			}
		}

		[DataMember]
		public string SharedCSSTouchNarrowUrl
		{
			get
			{
				return this.SharedCSSTouchNarrowUrlField;
			}
			set
			{
				this.SharedCSSTouchNarrowUrlField = value;
			}
		}

		[DataMember]
		public string SharedCSSTouchWideUrl
		{
			get
			{
				return this.SharedCSSTouchWideUrlField;
			}
			set
			{
				this.SharedCSSTouchWideUrlField = value;
			}
		}

		[DataMember]
		public string SharedJSTouchDeviceUrl
		{
			get
			{
				return this.SharedJSTouchDeviceUrlField;
			}
			set
			{
				this.SharedJSTouchDeviceUrlField = value;
			}
		}

		[DataMember]
		public string SharedJSTouchNarrowUrl
		{
			get
			{
				return this.SharedJSTouchNarrowUrlField;
			}
			set
			{
				this.SharedJSTouchNarrowUrlField = value;
			}
		}

		[DataMember]
		public string SharedJSTouchWideUrl
		{
			get
			{
				return this.SharedJSTouchWideUrlField;
			}
			set
			{
				this.SharedJSTouchWideUrlField = value;
			}
		}

		[DataMember]
		public string SuiteServiceProxyOriginAllowedList
		{
			get
			{
				return this.SuiteServiceProxyOriginAllowedListField;
			}
			set
			{
				this.SuiteServiceProxyOriginAllowedListField = value;
			}
		}

		[DataMember]
		public string SuiteServiceProxyScriptUrl
		{
			get
			{
				return this.SuiteServiceProxyScriptUrlField;
			}
			set
			{
				this.SuiteServiceProxyScriptUrlField = value;
			}
		}

		[DataMember]
		public string ThemeCSSUrl
		{
			get
			{
				return this.ThemeCSSUrlField;
			}
			set
			{
				this.ThemeCSSUrlField = value;
			}
		}

		[DataMember]
		public ThemeData ThemeData
		{
			get
			{
				return this.ThemeDataField;
			}
			set
			{
				this.ThemeDataField = value;
			}
		}

		private string SharedCSSTouchDeviceUrlField;

		private string SharedCSSTouchNarrowUrlField;

		private string SharedCSSTouchWideUrlField;

		private string SharedJSTouchDeviceUrlField;

		private string SharedJSTouchNarrowUrlField;

		private string SharedJSTouchWideUrlField;

		private string SuiteServiceProxyOriginAllowedListField;

		private string SuiteServiceProxyScriptUrlField;

		private string ThemeCSSUrlField;

		private ThemeData ThemeDataField;
	}
}
