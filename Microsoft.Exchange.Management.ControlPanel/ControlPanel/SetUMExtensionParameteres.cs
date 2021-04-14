using System;
using System.Collections.Generic;
using System.ServiceModel;
using Microsoft.Exchange.Configuration.ObjectModel;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class SetUMExtensionParameteres : SetObjectProperties
	{
		public IEnumerable<UMExtension> SecondaryExtensions { get; set; }

		public override string AssociatedCmdlet
		{
			get
			{
				return "Set-Mailbox";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@W:Organization";
			}
		}

		public void UpdateSecondaryExtensions(UMMailbox mailbox)
		{
			ProxyAddressCollection emailAddresses = mailbox.EmailAddresses;
			EumProxyAddress other = null;
			for (int i = emailAddresses.Count - 1; i >= 0; i--)
			{
				if (emailAddresses[i] is EumProxyAddress)
				{
					if (((EumProxyAddress)emailAddresses[i]).IsPrimaryAddress)
					{
						other = (EumProxyAddress)emailAddresses[i];
					}
					else
					{
						emailAddresses.RemoveAt(i);
					}
				}
			}
			if (this.SecondaryExtensions != null)
			{
				foreach (UMExtension umextension in this.SecondaryExtensions)
				{
					if (string.IsNullOrEmpty(umextension.PhoneContext) || string.IsNullOrEmpty(umextension.Extension))
					{
						throw new FaultException(Strings.InvalidSecondaryExtensionError);
					}
					ProxyAddress proxyAddress = UMMailbox.BuildProxyAddressFromExtensionAndPhoneContext(umextension.Extension, ProxyAddressPrefix.UM.SecondaryPrefix, umextension.PhoneContext);
					if (emailAddresses.Contains(proxyAddress))
					{
						if (proxyAddress.Equals(other))
						{
							throw new ProxyAddressExistsException(new LocalizedString(string.Format(Strings.DuplicateExtensionError, proxyAddress)));
						}
						throw new FaultException(string.Format(Strings.DuplicateSecondaryExtensionError, proxyAddress));
					}
					else
					{
						emailAddresses.Add(proxyAddress);
					}
				}
			}
			base[MailEnabledRecipientSchema.EmailAddresses] = emailAddresses;
		}
	}
}
