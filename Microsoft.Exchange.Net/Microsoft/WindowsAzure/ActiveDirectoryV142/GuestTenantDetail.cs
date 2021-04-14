using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;

namespace Microsoft.WindowsAzure.ActiveDirectoryV142
{
	public class GuestTenantDetail
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public static GuestTenantDetail CreateGuestTenantDetail(Collection<string> domains)
		{
			GuestTenantDetail guestTenantDetail = new GuestTenantDetail();
			if (domains == null)
			{
				throw new ArgumentNullException("domains");
			}
			guestTenantDetail.domains = domains;
			return guestTenantDetail;
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string tenantId
		{
			get
			{
				return this._tenantId;
			}
			set
			{
				this._tenantId = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string country
		{
			get
			{
				return this._country;
			}
			set
			{
				this._country = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string countryCode
		{
			get
			{
				return this._countryCode;
			}
			set
			{
				this._countryCode = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string displayName
		{
			get
			{
				return this._displayName;
			}
			set
			{
				this._displayName = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<string> domains
		{
			get
			{
				return this._domains;
			}
			set
			{
				this._domains = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public bool? isHomeTenant
		{
			get
			{
				return this._isHomeTenant;
			}
			set
			{
				this._isHomeTenant = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _tenantId;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _country;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _countryCode;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _displayName;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<string> _domains = new Collection<string>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private bool? _isHomeTenant;
	}
}
