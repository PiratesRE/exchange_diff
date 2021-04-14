using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.BOX.Shell
{
	[DataContract(Name = "ThemeData", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.Shell")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	public class ThemeData : IExtensibleDataObject
	{
		public ExtensionDataObject ExtensionData
		{
			get
			{
				return this.extensionDataField;
			}
			set
			{
				this.extensionDataField = value;
			}
		}

		[DataMember]
		public bool IsDarkTheme
		{
			get
			{
				return this.IsDarkThemeField;
			}
			set
			{
				this.IsDarkThemeField = value;
			}
		}

		[DataMember]
		public string[] NeutralColors
		{
			get
			{
				return this.NeutralColorsField;
			}
			set
			{
				this.NeutralColorsField = value;
			}
		}

		[DataMember]
		public string TenantPrimaryColor
		{
			get
			{
				return this.TenantPrimaryColorField;
			}
			set
			{
				this.TenantPrimaryColorField = value;
			}
		}

		[DataMember]
		public string[] TenantPrimaryColorShades
		{
			get
			{
				return this.TenantPrimaryColorShadesField;
			}
			set
			{
				this.TenantPrimaryColorShadesField = value;
			}
		}

		[DataMember]
		public string[] TenantThemeColors
		{
			get
			{
				return this.TenantThemeColorsField;
			}
			set
			{
				this.TenantThemeColorsField = value;
			}
		}

		[DataMember]
		public string ThemeVersion
		{
			get
			{
				return this.ThemeVersionField;
			}
			set
			{
				this.ThemeVersionField = value;
			}
		}

		[DataMember]
		public bool UserPersonalizationAllowed
		{
			get
			{
				return this.UserPersonalizationAllowedField;
			}
			set
			{
				this.UserPersonalizationAllowedField = value;
			}
		}

		[DataMember]
		public string[] UserThemeColors
		{
			get
			{
				return this.UserThemeColorsField;
			}
			set
			{
				this.UserThemeColorsField = value;
			}
		}

		[DataMember]
		public string UserThemeId
		{
			get
			{
				return this.UserThemeIdField;
			}
			set
			{
				this.UserThemeIdField = value;
			}
		}

		[DataMember]
		public string[] UserThemePrimaryColorShades
		{
			get
			{
				return this.UserThemePrimaryColorShadesField;
			}
			set
			{
				this.UserThemePrimaryColorShadesField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private bool IsDarkThemeField;

		private string[] NeutralColorsField;

		private string TenantPrimaryColorField;

		private string[] TenantPrimaryColorShadesField;

		private string[] TenantThemeColorsField;

		private string ThemeVersionField;

		private bool UserPersonalizationAllowedField;

		private string[] UserThemeColorsField;

		private string UserThemeIdField;

		private string[] UserThemePrimaryColorShadesField;
	}
}
