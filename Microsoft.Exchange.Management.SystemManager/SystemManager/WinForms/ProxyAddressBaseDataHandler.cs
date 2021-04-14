using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public abstract class ProxyAddressBaseDataHandler : ExchangeDataHandler
	{
		public string Prefix
		{
			get
			{
				return this.prefix;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentException(Strings.ProxyAddressTypeEmptyError);
				}
				ProxyAddressPrefix.GetPrefix(value);
				this.prefix = value;
			}
		}

		public string Address
		{
			get
			{
				return this.address;
			}
			set
			{
				ProxyAddressBase.ValidateAddressString(value);
				this.address = value;
			}
		}

		public virtual ProxyAddressBase ProxyAddressBase
		{
			get
			{
				return this.proxyAddressBase;
			}
			set
			{
				this.proxyAddressBase = value;
			}
		}

		public ProxyAddressBase OriginalProxyAddressBase
		{
			get
			{
				return this.originalProxyAddressBase;
			}
			set
			{
				this.originalProxyAddressBase = value;
			}
		}

		public List<ProxyAddressBase> ProxyAddresses
		{
			get
			{
				return this.proxyAddresses;
			}
		}

		public ProxyAddressBaseDataHandler()
		{
			base.DataSource = this;
		}

		public override ValidationError[] Validate()
		{
			ValidationError[] collection = base.Validate();
			List<ValidationError> list = new List<ValidationError>(collection);
			PropertyDefinition propertyDefinition = new AdminPropertyDefinition(this.BindingProperty, ExchangeObjectVersion.Exchange2003, typeof(string), null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
			try
			{
				this.ProxyAddressBase = this.GetValidatedProxyAddressBase(this.Prefix, this.Address);
				if (this.ProxyAddressBase != this.OriginalProxyAddressBase && this.ProxyAddresses.Contains(this.ProxyAddressBase))
				{
					list.Add(new PropertyValidationError(Strings.ProxyAddressExistedError, propertyDefinition, null));
				}
			}
			catch (ArgumentException ex)
			{
				list.Add(new PropertyValidationError(new LocalizedString(ex.Message), propertyDefinition, null));
			}
			return list.ToArray();
		}

		protected virtual string BindingProperty
		{
			get
			{
				return "Address";
			}
		}

		private ProxyAddressBase GetValidatedProxyAddressBase(string prefix, string address)
		{
			ProxyAddressBase proxyAddressBase = this.InternalGetProxyAddressBase(prefix, address);
			IInvalidProxy invalidProxy = proxyAddressBase as IInvalidProxy;
			if (invalidProxy != null)
			{
				throw invalidProxy.ParseException;
			}
			return proxyAddressBase;
		}

		protected abstract ProxyAddressBase InternalGetProxyAddressBase(string prefix, string address);

		private string prefix;

		private string address;

		private ProxyAddressBase proxyAddressBase;

		private ProxyAddressBase originalProxyAddressBase;

		private List<ProxyAddressBase> proxyAddresses = new List<ProxyAddressBase>();
	}
}
