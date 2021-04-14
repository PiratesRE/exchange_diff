using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsRequest
{
	[XmlType(TypeName = "AccountSettingsTypeGet", Namespace = "HMSETTINGS:")]
	[Serializable]
	public class AccountSettingsTypeGet
	{
		[XmlIgnore]
		public Filters Filters
		{
			get
			{
				if (this.internalFilters == null)
				{
					this.internalFilters = new Filters();
				}
				return this.internalFilters;
			}
			set
			{
				this.internalFilters = value;
			}
		}

		[XmlIgnore]
		public AccountSettingsTypeGetLists Lists
		{
			get
			{
				if (this.internalLists == null)
				{
					this.internalLists = new AccountSettingsTypeGetLists();
				}
				return this.internalLists;
			}
			set
			{
				this.internalLists = value;
			}
		}

		[XmlIgnore]
		public AccountSettingsTypeGetOptions Options
		{
			get
			{
				if (this.internalOptions == null)
				{
					this.internalOptions = new AccountSettingsTypeGetOptions();
				}
				return this.internalOptions;
			}
			set
			{
				this.internalOptions = value;
			}
		}

		[XmlIgnore]
		public AccountSettingsTypeGetProperties Properties
		{
			get
			{
				if (this.internalProperties == null)
				{
					this.internalProperties = new AccountSettingsTypeGetProperties();
				}
				return this.internalProperties;
			}
			set
			{
				this.internalProperties = value;
			}
		}

		[XmlIgnore]
		public UserSignature UserSignature
		{
			get
			{
				if (this.internalUserSignature == null)
				{
					this.internalUserSignature = new UserSignature();
				}
				return this.internalUserSignature;
			}
			set
			{
				this.internalUserSignature = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(Filters), ElementName = "Filters", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public Filters internalFilters;

		[XmlElement(Type = typeof(AccountSettingsTypeGetLists), ElementName = "Lists", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public AccountSettingsTypeGetLists internalLists;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(AccountSettingsTypeGetOptions), ElementName = "Options", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public AccountSettingsTypeGetOptions internalOptions;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(AccountSettingsTypeGetProperties), ElementName = "Properties", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public AccountSettingsTypeGetProperties internalProperties;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(UserSignature), ElementName = "UserSignature", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public UserSignature internalUserSignature;
	}
}
