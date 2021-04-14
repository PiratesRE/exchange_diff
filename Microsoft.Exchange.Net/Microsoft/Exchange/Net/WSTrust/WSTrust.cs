using System;

namespace Microsoft.Exchange.Net.WSTrust
{
	internal static class WSTrust
	{
		public const string NamespaceUri = "http://schemas.xmlsoap.org/ws/2005/02/trust";

		public static readonly XmlNamespaceDefinition Namespace = new XmlNamespaceDefinition("t", "http://schemas.xmlsoap.org/ws/2005/02/trust");

		public static readonly XmlElementDefinition RequestedSecurityToken = new XmlElementDefinition("RequestedSecurityToken", WSTrust.Namespace);

		public static readonly XmlElementDefinition RequestedProofToken = new XmlElementDefinition("RequestedProofToken", WSTrust.Namespace);

		public static readonly XmlElementDefinition BinarySecret = new XmlElementDefinition("BinarySecret", WSTrust.Namespace);

		public static readonly XmlElementDefinition TokenType = new XmlElementDefinition("TokenType", WSTrust.Namespace);

		public static readonly XmlElementDefinition KeyType = new XmlElementDefinition("KeyType", WSTrust.Namespace);

		public static readonly XmlElementDefinition RequestSecurityTokenResponse = new XmlElementDefinition("RequestSecurityTokenResponse", WSTrust.Namespace);

		public static readonly XmlElementDefinition RequestSecurityTokenResponseCollection = new XmlElementDefinition("RequestSecurityTokenResponseCollection", WSTrust.Namespace);

		public static readonly XmlElementDefinition RequestType = new XmlElementDefinition("RequestType", WSTrust.Namespace);

		public static readonly XmlElementDefinition KeySize = new XmlElementDefinition("KeySize", WSTrust.Namespace);

		public static readonly XmlElementDefinition CanonicalizationAlgorithm = new XmlElementDefinition("CanonicalizationAlgorithm", WSTrust.Namespace);

		public static readonly XmlElementDefinition EncryptionAlgorithm = new XmlElementDefinition("EncryptionAlgorithm", WSTrust.Namespace);

		public static readonly XmlElementDefinition EncryptWith = new XmlElementDefinition("EncryptWith", WSTrust.Namespace);

		public static readonly XmlElementDefinition SignWith = new XmlElementDefinition("SignWith", WSTrust.Namespace);

		public static readonly XmlElementDefinition OnBehalfOf = new XmlElementDefinition("OnBehalfOf", WSTrust.Namespace);

		public static readonly XmlElementDefinition Claims = new XmlElementDefinition("Claims", WSTrust.Namespace);

		public static readonly XmlElementDefinition ComputedKeyAlgorithm = new XmlElementDefinition("ComputedKeyAlgorithm", WSTrust.Namespace);

		public static readonly XmlElementDefinition RequestSecurityToken = new XmlElementDefinition("RequestSecurityToken", WSTrust.Namespace);

		public static readonly XmlElementDefinition Entropy = new XmlElementDefinition("Entropy", WSTrust.Namespace);

		public static readonly XmlElementDefinition RequestedAttachedReference = new XmlElementDefinition("RequestedAttachedReference", WSTrust.Namespace);

		public static readonly XmlElementDefinition RequestedUnattachedReference = new XmlElementDefinition("RequestedUnattachedReference", WSTrust.Namespace);

		public static readonly XmlElementDefinition Lifetime = new XmlElementDefinition("Lifetime", WSTrust.Namespace);
	}
}
