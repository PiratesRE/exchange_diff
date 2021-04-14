using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.WSTrust
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class TokenValidationResults
	{
		private TokenValidationResults(TokenValidationResult result)
		{
			if (result == TokenValidationResult.Valid)
			{
				throw new ArgumentException("result");
			}
			this.result = result;
		}

		public TokenValidationResults(string externalId, string emailAddress, Offer offer, SecurityToken securityToken, SymmetricSecurityKey proofToken, List<string> emailAddresses)
		{
			this.result = TokenValidationResult.Valid;
			this.externalId = externalId;
			this.emailAddress = emailAddress;
			this.offer = offer;
			this.securityToken = securityToken;
			this.proofToken = proofToken;
			this.emailAddresses = emailAddresses;
		}

		public TokenValidationResult Result
		{
			get
			{
				return this.result;
			}
		}

		public string ExternalId
		{
			get
			{
				return this.externalId;
			}
		}

		public string EmailAddress
		{
			get
			{
				return this.emailAddress;
			}
		}

		public Offer Offer
		{
			get
			{
				return this.offer;
			}
		}

		public SecurityToken SecurityToken
		{
			get
			{
				return this.securityToken;
			}
		}

		public SymmetricSecurityKey ProofToken
		{
			get
			{
				return this.proofToken;
			}
		}

		public List<string> EmailAddresses
		{
			get
			{
				return this.emailAddresses;
			}
		}

		public override string ToString()
		{
			if (this.result == TokenValidationResult.Valid)
			{
				return string.Concat(new object[]
				{
					"TokenValidationResult(ExternalId=",
					this.externalId.ToString(),
					",EmailAddress=",
					this.emailAddress.ToString(),
					",Offer=",
					this.offer.ToString(),
					",Number of EmailAddresses=",
					this.emailAddresses.Count,
					")"
				});
			}
			return "TokenValidationResult(Result=" + this.result + ")";
		}

		public static readonly TokenValidationResults InvalidUnknownExternalIdentity = new TokenValidationResults(TokenValidationResult.InvalidUnknownExternalIdentity);

		public static readonly TokenValidationResults InvalidUnknownEncryption = new TokenValidationResults(TokenValidationResult.InvalidUnknownEncryption);

		public static readonly TokenValidationResults InvalidTokenFailedValidation = new TokenValidationResults(TokenValidationResult.InvalidTokenFailedValidation);

		public static readonly TokenValidationResults InvalidTokenFormat = new TokenValidationResults(TokenValidationResult.InvalidTokenFormat);

		public static readonly TokenValidationResults InvalidTrustBroker = new TokenValidationResults(TokenValidationResult.InvalidTrustBroker);

		public static readonly TokenValidationResults InvalidTarget = new TokenValidationResults(TokenValidationResult.InvalidTarget);

		public static readonly TokenValidationResults InvalidOffer = new TokenValidationResults(TokenValidationResult.InvalidOffer);

		public static readonly TokenValidationResults InvalidUnknownEmailAddress = new TokenValidationResults(TokenValidationResult.InvalidUnknownEmailAddress);

		public static readonly TokenValidationResults InvalidExpired = new TokenValidationResults(TokenValidationResult.InvalidExpired);

		private TokenValidationResult result;

		private string externalId;

		private string emailAddress;

		private Offer offer;

		private SecurityToken securityToken;

		private SymmetricSecurityKey proofToken;

		private List<string> emailAddresses;
	}
}
