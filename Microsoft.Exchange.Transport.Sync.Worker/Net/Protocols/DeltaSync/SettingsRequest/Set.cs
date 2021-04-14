using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Exchange.Net.Protocols.DeltaSync.HMTypes;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsRequest
{
	[XmlType(TypeName = "Set", Namespace = "HMSETTINGS:")]
	[Serializable]
	public class Set
	{
		[XmlIgnore]
		public FiltersRequestType Filters
		{
			get
			{
				if (this.internalFilters == null)
				{
					this.internalFilters = new FiltersRequestType();
				}
				return this.internalFilters;
			}
			set
			{
				this.internalFilters = value;
			}
		}

		[XmlIgnore]
		public ListsRequestType Lists
		{
			get
			{
				if (this.internalLists == null)
				{
					this.internalLists = new ListsRequestType();
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
		public StringWithCharSetType UserSignature
		{
			get
			{
				if (this.internalUserSignature == null)
				{
					this.internalUserSignature = new StringWithCharSetType();
				}
				return this.internalUserSignature;
			}
			set
			{
				this.internalUserSignature = value;
			}
		}

		[XmlElement(Type = typeof(FiltersRequestType), ElementName = "Filters", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public FiltersRequestType internalFilters;

		[XmlElement(Type = typeof(ListsRequestType), ElementName = "Lists", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public ListsRequestType internalLists;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(OptionsType), ElementName = "Options", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public OptionsType internalOptions;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(StringWithCharSetType), ElementName = "UserSignature", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public StringWithCharSetType internalUserSignature;
	}
}
