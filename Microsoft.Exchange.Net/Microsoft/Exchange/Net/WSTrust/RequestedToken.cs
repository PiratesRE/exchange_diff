using System;
using System.IdentityModel.Tokens;
using System.ServiceModel.Security.Tokens;
using System.Xml;

namespace Microsoft.Exchange.Net.WSTrust
{
	internal sealed class RequestedToken
	{
		internal RequestedToken(XmlElement securityToken, XmlElement securityTokenReference, XmlElement requestedUnattachedReference, SymmetricSecurityKey proofToken, Timestamp lifetime)
		{
			this.securityToken = securityToken;
			this.securityTokenReference = securityTokenReference;
			this.requestedUnattachedReference = requestedUnattachedReference;
			this.proofToken = proofToken;
			this.lifetime = lifetime;
		}

		public XmlElement SecurityToken
		{
			get
			{
				return this.securityToken;
			}
		}

		public XmlElement SecurityTokenReference
		{
			get
			{
				return this.securityTokenReference;
			}
		}

		public XmlElement RequestUnattachedReference
		{
			get
			{
				return this.requestedUnattachedReference;
			}
		}

		public SymmetricSecurityKey ProofToken
		{
			get
			{
				return this.proofToken;
			}
		}

		public Timestamp Lifetime
		{
			get
			{
				return this.lifetime;
			}
		}

		public GenericXmlSecurityToken GetSecurityToken()
		{
			return new GenericXmlSecurityToken(this.securityToken, new BinarySecretSecurityToken(this.proofToken.GetSymmetricKey()), this.lifetime.Created, this.lifetime.Expires, new SamlAssertionKeyIdentifierClause(this.securityTokenReference.InnerText), new SamlAssertionKeyIdentifierClause(this.requestedUnattachedReference.InnerText), null);
		}

		private XmlElement securityToken;

		private XmlElement securityTokenReference;

		private XmlElement requestedUnattachedReference;

		private SymmetricSecurityKey proofToken;

		private Timestamp lifetime;
	}
}
