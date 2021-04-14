using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsResponse
{
	[XmlType(TypeName = "Set", Namespace = "HMSETTINGS:")]
	[Serializable]
	public class Set
	{
		[XmlIgnore]
		public SettingsCategoryResponseType Filters
		{
			get
			{
				if (this.internalFilters == null)
				{
					this.internalFilters = new SettingsCategoryResponseType();
				}
				return this.internalFilters;
			}
			set
			{
				this.internalFilters = value;
			}
		}

		[XmlIgnore]
		public ListsSetResponseType Lists
		{
			get
			{
				if (this.internalLists == null)
				{
					this.internalLists = new ListsSetResponseType();
				}
				return this.internalLists;
			}
			set
			{
				this.internalLists = value;
			}
		}

		[XmlIgnore]
		public SettingsCategoryResponseType Options
		{
			get
			{
				if (this.internalOptions == null)
				{
					this.internalOptions = new SettingsCategoryResponseType();
				}
				return this.internalOptions;
			}
			set
			{
				this.internalOptions = value;
			}
		}

		[XmlIgnore]
		public SettingsCategoryResponseType UserSignature
		{
			get
			{
				if (this.internalUserSignature == null)
				{
					this.internalUserSignature = new SettingsCategoryResponseType();
				}
				return this.internalUserSignature;
			}
			set
			{
				this.internalUserSignature = value;
			}
		}

		[XmlElement(Type = typeof(SettingsCategoryResponseType), ElementName = "Filters", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public SettingsCategoryResponseType internalFilters;

		[XmlElement(Type = typeof(ListsSetResponseType), ElementName = "Lists", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public ListsSetResponseType internalLists;

		[XmlElement(Type = typeof(SettingsCategoryResponseType), ElementName = "Options", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public SettingsCategoryResponseType internalOptions;

		[XmlElement(Type = typeof(SettingsCategoryResponseType), ElementName = "UserSignature", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public SettingsCategoryResponseType internalUserSignature;
	}
}
