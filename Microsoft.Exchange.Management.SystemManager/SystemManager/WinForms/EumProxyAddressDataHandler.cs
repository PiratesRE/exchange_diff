using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class EumProxyAddressDataHandler : ProxyAddressDataHandler
	{
		public override ProxyAddressBase ProxyAddressBase
		{
			get
			{
				return base.ProxyAddressBase;
			}
			set
			{
				base.ProxyAddressBase = value;
				if (value != null)
				{
					string valueString = value.ValueString;
					EumAddress eumAddress = EumAddress.Parse(valueString);
					this.extension = eumAddress.Extension;
					this.phoneContext = eumAddress.PhoneContext;
				}
			}
		}

		public string Extension
		{
			get
			{
				return this.extension;
			}
			set
			{
				if (this.extension != value)
				{
					this.extension = value;
					this.UpdateAddress(this.extension, this.PhoneContext);
				}
			}
		}

		public string PhoneContext
		{
			get
			{
				return this.phoneContext;
			}
			set
			{
				if (this.phoneContext != value)
				{
					this.phoneContext = value;
					this.UpdateAddress(this.Extension, this.phoneContext);
				}
			}
		}

		private void UpdateAddress(string extension, string phoneContext)
		{
			base.Address = EumAddress.BuildAddressString(extension, phoneContext);
		}

		protected override ProxyAddressBase InternalGetProxyAddressBase(string prefix, string address)
		{
			return ProxyAddress.Parse(prefix, address);
		}

		protected override string BindingProperty
		{
			get
			{
				return "Extension";
			}
		}

		private string extension;

		private string phoneContext;
	}
}
