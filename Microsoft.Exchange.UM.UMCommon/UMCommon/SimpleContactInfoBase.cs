using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.UM.UMCommon
{
	[Serializable]
	internal abstract class SimpleContactInfoBase : ContactInfo
	{
		internal override string DisplayName
		{
			get
			{
				return null;
			}
		}

		internal override string Title
		{
			get
			{
				return null;
			}
		}

		internal override string Company
		{
			get
			{
				return null;
			}
		}

		internal override string FirstName
		{
			get
			{
				return null;
			}
		}

		internal override string LastName
		{
			get
			{
				return null;
			}
		}

		internal override string BusinessPhone
		{
			get
			{
				return this.businessPhone;
			}
			set
			{
				this.businessPhone = value;
			}
		}

		internal override string MobilePhone
		{
			get
			{
				return this.mobilePhone;
			}
			set
			{
				this.mobilePhone = value;
			}
		}

		internal override string FaxNumber
		{
			get
			{
				return null;
			}
		}

		internal override string HomePhone
		{
			get
			{
				return this.homePhone;
			}
			set
			{
				this.homePhone = value;
			}
		}

		internal override string IMAddress
		{
			get
			{
				return null;
			}
		}

		internal override string EMailAddress
		{
			get
			{
				return null;
			}
		}

		internal override FoundByType FoundBy
		{
			get
			{
				return FoundByType.NotSpecified;
			}
		}

		internal override string Id
		{
			get
			{
				return null;
			}
		}

		internal override string EwsId
		{
			get
			{
				return null;
			}
		}

		internal override string EwsType
		{
			get
			{
				return null;
			}
		}

		internal override string City
		{
			get
			{
				return null;
			}
		}

		internal override string Country
		{
			get
			{
				return null;
			}
		}

		internal override ICollection<string> SanitizedPhoneNumbers
		{
			get
			{
				return new List<string>();
			}
		}

		private string businessPhone;

		private string mobilePhone;

		private string homePhone;
	}
}
