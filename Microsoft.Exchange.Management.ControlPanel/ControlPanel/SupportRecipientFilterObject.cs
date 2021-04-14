using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SupportRecipientFilterObject
	{
		public SupportRecipientFilterObject()
		{
			this.OnDeserializing(default(StreamingContext));
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext context)
		{
			this.DataObject = new EmailAddressPolicy();
		}

		internal ISupportRecipientFilter DataObject { get; set; }

		[DataMember]
		public ICollection<string> ConditionalCompany
		{
			get
			{
				return this.DataObject.ConditionalCompany;
			}
			set
			{
				this.DataObject.ConditionalCompany = ((value != null && value.Count > 0) ? new MultiValuedProperty<string>(value) : null);
			}
		}

		[DataMember]
		public ICollection<string> ConditionalCustomAttribute1
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute1;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute1 = ((value != null && value.Count > 0) ? new MultiValuedProperty<string>(value) : null);
			}
		}

		[DataMember]
		public ICollection<string> ConditionalCustomAttribute2
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute2;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute2 = ((value != null && value.Count > 0) ? new MultiValuedProperty<string>(value) : null);
			}
		}

		[DataMember]
		public ICollection<string> ConditionalCustomAttribute3
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute3;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute3 = ((value != null && value.Count > 0) ? new MultiValuedProperty<string>(value) : null);
			}
		}

		[DataMember]
		public ICollection<string> ConditionalCustomAttribute4
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute4;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute4 = ((value != null && value.Count > 0) ? new MultiValuedProperty<string>(value) : null);
			}
		}

		[DataMember]
		public ICollection<string> ConditionalCustomAttribute5
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute5;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute5 = ((value != null && value.Count > 0) ? new MultiValuedProperty<string>(value) : null);
			}
		}

		[DataMember]
		public ICollection<string> ConditionalCustomAttribute6
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute6;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute6 = ((value != null && value.Count > 0) ? new MultiValuedProperty<string>(value) : null);
			}
		}

		[DataMember]
		public ICollection<string> ConditionalCustomAttribute7
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute7;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute7 = ((value != null && value.Count > 0) ? new MultiValuedProperty<string>(value) : null);
			}
		}

		[DataMember]
		public ICollection<string> ConditionalCustomAttribute8
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute8;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute8 = ((value != null && value.Count > 0) ? new MultiValuedProperty<string>(value) : null);
			}
		}

		[DataMember]
		public ICollection<string> ConditionalCustomAttribute9
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute9;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute9 = ((value != null && value.Count > 0) ? new MultiValuedProperty<string>(value) : null);
			}
		}

		[DataMember]
		public ICollection<string> ConditionalCustomAttribute10
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute10;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute10 = ((value != null && value.Count > 0) ? new MultiValuedProperty<string>(value) : null);
			}
		}

		[DataMember]
		public ICollection<string> ConditionalCustomAttribute11
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute11;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute11 = ((value != null && value.Count > 0) ? new MultiValuedProperty<string>(value) : null);
			}
		}

		[DataMember]
		public ICollection<string> ConditionalCustomAttribute12
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute12;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute12 = ((value != null && value.Count > 0) ? new MultiValuedProperty<string>(value) : null);
			}
		}

		[DataMember]
		public ICollection<string> ConditionalCustomAttribute13
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute13;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute13 = ((value != null && value.Count > 0) ? new MultiValuedProperty<string>(value) : null);
			}
		}

		[DataMember]
		public ICollection<string> ConditionalCustomAttribute14
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute14;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute14 = ((value != null && value.Count > 0) ? new MultiValuedProperty<string>(value) : null);
			}
		}

		[DataMember]
		public ICollection<string> ConditionalCustomAttribute15
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute15;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute15 = ((value != null && value.Count > 0) ? new MultiValuedProperty<string>(value) : null);
			}
		}

		[DataMember]
		public ICollection<string> ConditionalDepartment
		{
			get
			{
				return this.DataObject.ConditionalDepartment;
			}
			set
			{
				this.DataObject.ConditionalDepartment = ((value != null && value.Count > 0) ? new MultiValuedProperty<string>(value) : null);
			}
		}

		[DataMember]
		public ICollection<string> ConditionalStateOrProvince
		{
			get
			{
				return this.DataObject.ConditionalStateOrProvince;
			}
			set
			{
				this.DataObject.ConditionalStateOrProvince = ((value != null && value.Count > 0) ? new MultiValuedProperty<string>(value) : null);
			}
		}

		[DataMember]
		public string IncludedRecipients
		{
			get
			{
				return this.DataObject.IncludedRecipients.ToString();
			}
			set
			{
				this.DataObject.IncludedRecipients = new WellKnownRecipientType?((WellKnownRecipientType)Enum.Parse(typeof(WellKnownRecipientType), value));
			}
		}

		[DataMember]
		public Identity RecipientContainer
		{
			get
			{
				if (this.DataObject.RecipientContainer == null)
				{
					return null;
				}
				return this.DataObject.RecipientContainer.ToIdentity();
			}
			set
			{
				this.DataObject.RecipientContainer = ((value != null) ? ADObjectId.ParseDnOrGuid(value.RawIdentity) : null);
			}
		}

		[DataMember]
		public string LdapRecipientFilter { get; set; }
	}
}
