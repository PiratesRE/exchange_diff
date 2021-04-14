using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Microsoft.Exchange.Net.Claim
{
	internal static class TracerHelper
	{
		public static string GetTraceString(this EvaluationContext evaluationContext)
		{
			StringBuilder stringBuilder = new StringBuilder(2000);
			TracerHelper.Append(stringBuilder, evaluationContext);
			return stringBuilder.ToString();
		}

		public static string GetTraceString(this ReadOnlyCollection<ClaimSet> claimSets)
		{
			StringBuilder stringBuilder = new StringBuilder(2000);
			TracerHelper.Append(stringBuilder, string.Empty, claimSets);
			return stringBuilder.ToString();
		}

		private static void Append(StringBuilder text, EvaluationContext evaluationContext)
		{
			TracerHelper.AppendNameValueLine(text, string.Empty, "Generation", evaluationContext.Generation.ToString());
			TracerHelper.AppendGroupLine(text, string.Empty, "ClaimSets");
			TracerHelper.Append(text, "  ", evaluationContext.ClaimSets);
			TracerHelper.AppendGroupLine(text, string.Empty, "Properties");
			TracerHelper.Append(text, "  ", evaluationContext.Properties);
		}

		private static void Append(StringBuilder text, string indent, IDictionary<string, object> collection)
		{
			if (collection == null)
			{
				TracerHelper.AppendNullLine(text, indent);
				return;
			}
			if (collection.Count == 0)
			{
				TracerHelper.AppendGroupLine(text, indent, "<empty>");
				return;
			}
			foreach (KeyValuePair<string, object> keyValuePair in collection)
			{
				TracerHelper.AppendNameValueLine(text, indent, keyValuePair.Key, keyValuePair.Value.ToString());
			}
		}

		internal static void Append(StringBuilder text, string indent, ReadOnlyCollection<ClaimSet> claimSets)
		{
			if (claimSets == null)
			{
				TracerHelper.AppendNullLine(text, indent);
				return;
			}
			if (claimSets.Count == 0)
			{
				TracerHelper.AppendGroupLine(text, indent, "<empty>");
				return;
			}
			for (int i = 0; i < claimSets.Count; i++)
			{
				TracerHelper.AppendGroupLine(text, indent, "ClaimSet[" + i + "]");
				TracerHelper.Append(text, indent + "  ", claimSets[i], true);
			}
		}

		private static void Append(StringBuilder text, string indent, ClaimSet claimSet, bool appendIssuer)
		{
			if (claimSet == null)
			{
				TracerHelper.AppendNullLine(text, indent);
				return;
			}
			if (claimSet.Count == 0)
			{
				TracerHelper.AppendGroupLine(text, indent, "<empty>");
			}
			else
			{
				for (int i = 0; i < claimSet.Count; i++)
				{
					TracerHelper.AppendGroupLine(text, indent, "Claim[" + i + "]");
					TracerHelper.Append(text, indent + "  ", claimSet[i]);
				}
			}
			if (appendIssuer)
			{
				TracerHelper.AppendGroupLine(text, indent, "Issuer");
				TracerHelper.Append(text, indent + "  ", claimSet.Issuer, false);
			}
		}

		private static void Append(StringBuilder text, string indent, Claim claim)
		{
			if (claim == null)
			{
				TracerHelper.AppendNullLine(text, indent);
				return;
			}
			TracerHelper.AppendNameValueLine(text, indent, "ClaimType", claim.ClaimType);
			TracerHelper.AppendNameValueLine(text, indent, "Right", claim.Right);
			TracerHelper.AppendGroupLine(text, indent, "Resource");
			TracerHelper.Append(text, indent + "  ", claim.Resource);
		}

		private static void Append(StringBuilder text, string indent, object resource)
		{
			byte[] array = resource as byte[];
			if (array != null)
			{
				TracerHelper.AppendNameValueLine(text, indent, "byte[]", BitConverter.ToString(array));
				return;
			}
			string text2 = resource as string;
			if (text2 != null)
			{
				TracerHelper.AppendNameValueLine(text, indent, "string", text2);
				return;
			}
			SamlNameIdentifierClaimResource samlNameIdentifierClaimResource = resource as SamlNameIdentifierClaimResource;
			if (samlNameIdentifierClaimResource != null)
			{
				TracerHelper.AppendGroupLine(text, indent, "SamlNameIdentifierClaimResource");
				TracerHelper.Append(text, indent + "  ", samlNameIdentifierClaimResource);
				return;
			}
			SamlAuthenticationClaimResource samlAuthenticationClaimResource = resource as SamlAuthenticationClaimResource;
			if (samlAuthenticationClaimResource != null)
			{
				TracerHelper.AppendGroupLine(text, indent, "SamlAuthenticationClaimResource");
				TracerHelper.Append(text, indent + "  ", samlAuthenticationClaimResource);
				return;
			}
			X500DistinguishedName x500DistinguishedName = resource as X500DistinguishedName;
			if (x500DistinguishedName != null)
			{
				TracerHelper.AppendGroupLine(text, indent, "X500DistinguishedName");
				TracerHelper.Append(text, indent + "  ", x500DistinguishedName);
				return;
			}
			RSACryptoServiceProvider rsacryptoServiceProvider = resource as RSACryptoServiceProvider;
			if (rsacryptoServiceProvider != null)
			{
				TracerHelper.AppendGroupLine(text, indent, "RSACryptoServiceProvider");
				TracerHelper.Append(text, indent + "  ", rsacryptoServiceProvider);
				return;
			}
			object[] array2 = resource as object[];
			if (array2 != null)
			{
				TracerHelper.AppendGroupLine(text, indent, "object[]");
				foreach (object obj in array2)
				{
					TracerHelper.Append(text, indent + "  ", obj.ToString());
				}
				return;
			}
			TracerHelper.AppendNameValueLine(text, indent, "Resource", resource.ToString());
		}

		private static void Append(StringBuilder text, string indent, SamlNameIdentifierClaimResource samlNameIdentifierClaimResource)
		{
			TracerHelper.AppendNameValueLine(text, indent, "Format", samlNameIdentifierClaimResource.Format);
			TracerHelper.AppendNameValueLine(text, indent, "Name ", samlNameIdentifierClaimResource.Name);
			TracerHelper.AppendNameValueLine(text, indent, "NameQualifier", samlNameIdentifierClaimResource.NameQualifier);
		}

		private static void Append(StringBuilder text, string indent, SamlAuthenticationClaimResource samlAuthenticationClaimResource)
		{
			TracerHelper.AppendNameValueLine(text, indent, "AuthenticationInstant", samlAuthenticationClaimResource.AuthenticationInstant.ToString());
			TracerHelper.AppendNameValueLine(text, indent, "AuthenticationMethod ", samlAuthenticationClaimResource.AuthenticationMethod);
			TracerHelper.AppendNameValueLine(text, indent, "DnsAddress", samlAuthenticationClaimResource.DnsAddress);
			TracerHelper.AppendNameValueLine(text, indent, "IPAddress", samlAuthenticationClaimResource.IPAddress);
			TracerHelper.AppendGroupLine(text, indent, "AuthorityBindings");
			TracerHelper.Append(text, indent + "  ", samlAuthenticationClaimResource.AuthorityBindings);
		}

		private static void Append(StringBuilder text, string indent, X500DistinguishedName x500DistinguishedName)
		{
			TracerHelper.AppendNameValueLine(text, indent, "KeyExchangeAlgorithm", x500DistinguishedName.Name);
		}

		private static void Append(StringBuilder text, string indent, RSACryptoServiceProvider cryptoServiceProvider)
		{
			TracerHelper.AppendNameValueLine(text, indent, "KeyExchangeAlgorithm", cryptoServiceProvider.KeyExchangeAlgorithm);
			TracerHelper.AppendNameValueLine(text, indent, "SignatureAlgorithm ", cryptoServiceProvider.SignatureAlgorithm);
			TracerHelper.AppendNameValueLine(text, indent, "KeySize", cryptoServiceProvider.KeySize.ToString());
		}

		private static void Append(StringBuilder text, string indent, ReadOnlyCollection<SamlAuthorityBinding> authorityBindings)
		{
			if (authorityBindings == null)
			{
				TracerHelper.AppendNullLine(text, indent);
				return;
			}
			if (authorityBindings.Count == 0)
			{
				TracerHelper.AppendGroupLine(text, indent, "<empty>");
				return;
			}
			for (int i = 0; i < authorityBindings.Count; i++)
			{
				TracerHelper.AppendGroupLine(text, indent, "Item[" + i + "]");
				TracerHelper.Append(text, indent + "  ", authorityBindings[i]);
			}
		}

		private static void Append(StringBuilder text, string indent, SamlAuthorityBinding authorityBinding)
		{
			if (authorityBinding == null)
			{
				TracerHelper.AppendNullLine(text, indent);
				return;
			}
			TracerHelper.AppendNameValueLine(text, indent, "AuthorityKind ", authorityBinding.AuthorityKind.ToString());
			TracerHelper.AppendNameValueLine(text, indent, "Binding  ", authorityBinding.Binding);
			TracerHelper.AppendNameValueLine(text, indent, "IsReadOnly", authorityBinding.IsReadOnly.ToString());
			TracerHelper.AppendNameValueLine(text, indent, "Location", authorityBinding.Location);
		}

		private static void AppendNameValueLine(StringBuilder text, string indent, string name, string value)
		{
			text.Append(indent);
			text.Append(name);
			text.Append(": '");
			text.Append((value == null) ? "<null>" : value);
			text.AppendLine("'");
		}

		private static void AppendGroupLine(StringBuilder text, string indent, string groupName)
		{
			text.Append(indent);
			text.AppendLine(groupName);
		}

		private static void AppendNullLine(StringBuilder text, string indent)
		{
			text.Append(indent);
			text.AppendLine("<null>");
		}

		private const string IndentStep = "  ";
	}
}
