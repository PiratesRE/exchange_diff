using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Exchange.Net.Protocols.DeltaSync.HMTypes;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsResponse
{
	[XmlType(TypeName = "AccountSettingsGet", Namespace = "HMSETTINGS:")]
	[Serializable]
	public class AccountSettingsGet
	{
		[XmlIgnore]
		public FiltersResponseType Filters
		{
			get
			{
				if (this.internalFilters == null)
				{
					this.internalFilters = new FiltersResponseType();
				}
				return this.internalFilters;
			}
			set
			{
				this.internalFilters = value;
			}
		}

		[XmlIgnore]
		public ListsGetResponseType Lists
		{
			get
			{
				if (this.internalLists == null)
				{
					this.internalLists = new ListsGetResponseType();
				}
				return this.internalLists;
			}
			set
			{
				this.internalLists = value;
			}
		}

		[XmlIgnore]
		public OptionsType Options
		{
			get
			{
				if (this.internalOptions == null)
				{
					this.internalOptions = new OptionsType();
				}
				return this.internalOptions;
			}
			set
			{
				this.internalOptions = value;
			}
		}

		[XmlIgnore]
		public PropertiesType Properties
		{
			get
			{
				if (this.internalProperties == null)
				{
					this.internalProperties = new PropertiesType();
				}
				return this.internalProperties;
			}
			set
			{
				this.internalProperties = value;
			}
		}

		[XmlIgnore]
		public StringWithVersionType UserSignature
		{
			get
			{
				if (this.internalUserSignature == null)
				{
					this.internalUserSignature = new StringWithVersionType();
				}
				return this.internalUserSignature;
			}
			set
			{
				this.internalUserSignature = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(FiltersResponseType), ElementName = "Filters", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public FiltersResponseType internalFilters;

		[XmlElement(Type = typeof(ListsGetResponseType), ElementName = "Lists", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public ListsGetResponseType internalLists;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(OptionsType), ElementName = "Options", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public OptionsType internalOptions;

		[XmlElement(Type = typeof(PropertiesType), ElementName = "Properties", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public PropertiesType internalProperties;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(StringWithVersionType), ElementName = "UserSignature", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public StringWithVersionType internalUserSignature;
	}
}
