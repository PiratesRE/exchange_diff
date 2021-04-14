using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Mserve.SettingsResponse
{
	[XmlType(AnonymousType = true, Namespace = "HMSETTINGS:")]
	[DesignerCategory("code")]
	[GeneratedCode("xsd", "2.0.50727.1318")]
	[DebuggerStepThrough]
	[Serializable]
	public class SettingsAccountSettingsGet
	{
		[XmlArrayItem("Filter", IsNullable = false)]
		public FiltersResponseTypeFilter[] Filters
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

		[XmlArrayItem("List", IsNullable = false)]
		public ListsGetResponseTypeList[] Lists
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

		public OptionsType Options
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

		public PropertiesType Properties
		{
			get
			{
				return this.propertiesField;
			}
			set
			{
				this.propertiesField = value;
			}
		}

		public StringWithVersionType UserSignature
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

		private FiltersResponseTypeFilter[] filtersField;

		private ListsGetResponseTypeList[] listsField;

		private OptionsType optionsField;

		private PropertiesType propertiesField;

		private StringWithVersionType userSignatureField;
	}
}
