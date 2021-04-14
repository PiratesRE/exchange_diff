using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ProxyAddressTemplateDataHandler : ProxyAddressBaseDataHandler
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
					base.Prefix = value.PrefixString;
					base.Address = ((ProxyAddressTemplate)value).AddressTemplateString;
				}
			}
		}

		protected override ProxyAddressBase InternalGetProxyAddressBase(string prefix, string address)
		{
			return ProxyAddressTemplate.Parse(prefix, address);
		}
	}
}
