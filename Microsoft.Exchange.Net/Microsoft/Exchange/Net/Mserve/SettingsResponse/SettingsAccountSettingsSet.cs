using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Mserve.SettingsResponse
{
	[GeneratedCode("xsd", "2.0.50727.1318")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[XmlType(AnonymousType = true, Namespace = "HMSETTINGS:")]
	[Serializable]
	public class SettingsAccountSettingsSet
	{
		public SettingsCategoryResponseType Filters
		{
			get
			{
				return this.filtersField;
			}
			set
			{
				this.filtersField = value;
			}
		}

		public ListsSetResponseType Lists
		{
			get
			{
				return this.listsField;
			}
			set
			{
				this.listsField = value;
			}
		}

		public SettingsCategoryResponseType Options
		{
			get
			{
				return this.optionsField;
			}
			set
			{
				this.optionsField = value;
			}
		}

		public SettingsCategoryResponseType UserSignature
		{
			get
			{
				return this.userSignatureField;
			}
			set
			{
				this.userSignatureField = value;
			}
		}

		private SettingsCategoryResponseType filtersField;

		private ListsSetResponseType listsField;

		private SettingsCategoryResponseType optionsField;

		private SettingsCategoryResponseType userSignatureField;
	}
}
