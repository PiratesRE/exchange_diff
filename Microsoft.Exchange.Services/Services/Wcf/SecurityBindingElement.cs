using System;
using System.Net.Security;
using System.ServiceModel.Channels;
using System.Xml;

namespace Microsoft.Exchange.Services.Wcf
{
	public class SecurityBindingElement : BindingElement, ITransportTokenAssertionProvider
	{
		public override BindingElement Clone()
		{
			return new SecurityBindingElement();
		}

		public override T GetProperty<T>(BindingContext context)
		{
			if (typeof(T) == typeof(ISecurityCapabilities))
			{
				return (T)((object)new SecurityBindingElement.SecurityCapabilities());
			}
			return context.GetInnerProperty<T>();
		}

		public XmlElement GetTransportTokenAssertion()
		{
			return null;
		}

		internal class SecurityCapabilities : ISecurityCapabilities
		{
			public ProtectionLevel SupportedRequestProtectionLevel
			{
				get
				{
					return ProtectionLevel.EncryptAndSign;
				}
			}

			public ProtectionLevel SupportedResponseProtectionLevel
			{
				get
				{
					return ProtectionLevel.EncryptAndSign;
				}
			}

			public bool SupportsClientAuthentication
			{
				get
				{
					return false;
				}
			}

			public bool SupportsClientWindowsIdentity
			{
				get
				{
					return false;
				}
			}

			public bool SupportsServerAuthentication
			{
				get
				{
					return true;
				}
			}
		}
	}
}
