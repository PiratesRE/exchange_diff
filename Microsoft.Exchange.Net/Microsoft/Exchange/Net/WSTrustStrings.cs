using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Net
{
	internal static class WSTrustStrings
	{
		static WSTrustStrings()
		{
			WSTrustStrings.stringIDs.Add(1535677007U, "CannotDecryptToken");
			WSTrustStrings.stringIDs.Add(3292530159U, "ProofTokenNotFoundException");
			WSTrustStrings.stringIDs.Add(767822664U, "MalformedRequestSecurityTokenResponse");
			WSTrustStrings.stringIDs.Add(1285681424U, "SoapXmlMalformedException");
			WSTrustStrings.stringIDs.Add(4152981396U, "SoapFaultException");
		}

		public static LocalizedString CannotDecryptToken
		{
			get
			{
				return new LocalizedString("CannotDecryptToken", "Ex2FCEEA", false, true, WSTrustStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProofTokenNotFoundException
		{
			get
			{
				return new LocalizedString("ProofTokenNotFoundException", "ExB3FB21", false, true, WSTrustStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HttpClientFailedToCommunicate(string endpoint)
		{
			return new LocalizedString("HttpClientFailedToCommunicate", "Ex44EF69", false, true, WSTrustStrings.ResourceManager, new object[]
			{
				endpoint
			});
		}

		public static LocalizedString MalformedRequestSecurityTokenResponse
		{
			get
			{
				return new LocalizedString("MalformedRequestSecurityTokenResponse", "Ex7DF18B", false, true, WSTrustStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SoapXmlMalformedException
		{
			get
			{
				return new LocalizedString("SoapXmlMalformedException", "ExC3152E", false, true, WSTrustStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnknownTokenIssuerException(string federatedTokenIssuerUri, string targetTokenIssuerUri)
		{
			return new LocalizedString("UnknownTokenIssuerException", "Ex831278", false, true, WSTrustStrings.ResourceManager, new object[]
			{
				federatedTokenIssuerUri,
				targetTokenIssuerUri
			});
		}

		public static LocalizedString FailedToSerializeToken(Exception e)
		{
			return new LocalizedString("FailedToSerializeToken", "", false, false, WSTrustStrings.ResourceManager, new object[]
			{
				e
			});
		}

		public static LocalizedString SoapFaultException
		{
			get
			{
				return new LocalizedString("SoapFaultException", "Ex45AD59", false, true, WSTrustStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToIssueToken(Exception e)
		{
			return new LocalizedString("FailedToIssueToken", "", false, false, WSTrustStrings.ResourceManager, new object[]
			{
				e
			});
		}

		public static LocalizedString GetLocalizedString(WSTrustStrings.IDs key)
		{
			return new LocalizedString(WSTrustStrings.stringIDs[(uint)key], WSTrustStrings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(5);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Net.WSTrustStrings", typeof(WSTrustStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			CannotDecryptToken = 1535677007U,
			ProofTokenNotFoundException = 3292530159U,
			MalformedRequestSecurityTokenResponse = 767822664U,
			SoapXmlMalformedException = 1285681424U,
			SoapFaultException = 4152981396U
		}

		private enum ParamIDs
		{
			HttpClientFailedToCommunicate,
			UnknownTokenIssuerException,
			FailedToSerializeToken,
			FailedToIssueToken
		}
	}
}
