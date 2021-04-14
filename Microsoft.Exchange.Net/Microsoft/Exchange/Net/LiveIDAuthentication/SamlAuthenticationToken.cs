using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.LiveIDAuthentication
{
	internal sealed class SamlAuthenticationToken : BaseAuthenticationToken
	{
		public SamlAuthenticationToken(string assertionXml)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("assertionXml", assertionXml);
			this.assertionXml = assertionXml;
		}

		public string AssertionXml
		{
			get
			{
				return this.assertionXml;
			}
		}

		public override string ToString()
		{
			return this.assertionXml;
		}

		private readonly string assertionXml;
	}
}
