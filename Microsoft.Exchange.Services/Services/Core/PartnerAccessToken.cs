using System;
using System.Text;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Security.PartnerToken;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class PartnerAccessToken
	{
		public PartnerAccessToken(PartnerIdentity partnerIdentity) : this(partnerIdentity.DelegatedOrganizationId.OrganizationalUnit, partnerIdentity.DelegatedOrganizationId.ConfigurationUnit, partnerIdentity.DelegatedPrincipal.DelegatedOrganization, partnerIdentity.DelegatedPrincipal.UserId)
		{
			if (partnerIdentity.DelegatedOrganizationId == OrganizationId.ForestWideOrgId)
			{
				ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug(0L, "[PartnerAccessToken::ctor] the first org id is not expected.");
				throw FaultExceptionUtilities.CreateFault(new InvalidSerializedAccessTokenException(), FaultParty.Sender);
			}
			if (string.IsNullOrEmpty(partnerIdentity.DelegatedPrincipal.DelegatedOrganization))
			{
				ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug(0L, "[PartnerAccessToken::ctor] empty tenant name is not expected.");
				throw FaultExceptionUtilities.CreateFault(new InvalidSerializedAccessTokenException(), FaultParty.Sender);
			}
		}

		private PartnerAccessToken(ADObjectId organizationalUnit, ADObjectId configurationUnit, string tenantName, string partnerUser)
		{
			this.organizationalUnit = organizationalUnit;
			this.configurationUnit = configurationUnit;
			this.tenantName = tenantName;
			this.partnerUser = partnerUser;
		}

		public static PartnerAccessToken FromBytes(byte[] serializedTokenBytes)
		{
			int num = 0;
			if (serializedTokenBytes.Length < PartnerAccessToken.partnerAccessTokenCookie.Length + 1)
			{
				throw FaultExceptionUtilities.CreateFault(new InvalidSerializedAccessTokenException(), FaultParty.Sender);
			}
			for (int i = 0; i < PartnerAccessToken.partnerAccessTokenCookie.Length; i++)
			{
				if (serializedTokenBytes[num++] != PartnerAccessToken.partnerAccessTokenCookie[i])
				{
					throw FaultExceptionUtilities.CreateFault(new InvalidSerializedAccessTokenException(), FaultParty.Sender);
				}
			}
			if (serializedTokenBytes[num++] != 1)
			{
				throw FaultExceptionUtilities.CreateFault(new InvalidSerializedAccessTokenException(), FaultParty.Sender);
			}
			string text = SerializedSecurityAccessToken.DeserializeStringFromByteArray(serializedTokenBytes, ref num);
			string text2 = SerializedSecurityAccessToken.DeserializeStringFromByteArray(serializedTokenBytes, ref num);
			ADObjectId adobjectId = PartnerAccessToken.DeserializeADObjectIdFromByteArray(serializedTokenBytes, ref num);
			ADObjectId adobjectId2 = PartnerAccessToken.DeserializeADObjectIdFromByteArray(serializedTokenBytes, ref num);
			return new PartnerAccessToken(adobjectId, adobjectId2, text2, text);
		}

		public OrganizationId OrganizationId
		{
			get
			{
				return this.OrganizationIdGetter();
			}
		}

		public string OrganizationName
		{
			get
			{
				return this.tenantName;
			}
		}

		public string PartnerUser
		{
			get
			{
				return this.partnerUser;
			}
		}

		public byte[] GetBytes()
		{
			byte[] array = new byte[this.GetByteCountToSerializeToken()];
			int num = 0;
			PartnerAccessToken.partnerAccessTokenCookie.CopyTo(array, num);
			num += PartnerAccessToken.partnerAccessTokenCookie.Length;
			array[num++] = 1;
			SerializedSecurityAccessToken.SerializeStringToByteArray(this.partnerUser, array, ref num);
			SerializedSecurityAccessToken.SerializeStringToByteArray(this.tenantName, array, ref num);
			PartnerAccessToken.SerializeADObjectIdToByteArray(this.organizationalUnit, array, ref num);
			PartnerAccessToken.SerializeADObjectIdToByteArray(this.configurationUnit, array, ref num);
			return array;
		}

		private static void SerializeADObjectIdToByteArray(ADObjectId adObjectId, byte[] byteArray, ref int byteIndex)
		{
			byte[] bytes = adObjectId.GetBytes(PartnerAccessToken.DefaultEncoding);
			int num = bytes.Length;
			byteIndex += ExBitConverter.Write(num, byteArray, byteIndex);
			Array.Copy(bytes, 0, byteArray, byteIndex, num);
			byteIndex += num;
		}

		private static ADObjectId DeserializeADObjectIdFromByteArray(byte[] byteArray, ref int byteIndex)
		{
			int num = SerializedSecurityAccessToken.ReadInt32(byteArray, ref byteIndex);
			if (byteArray.Length < byteIndex + num)
			{
				throw FaultExceptionUtilities.CreateFault(new InvalidSerializedAccessTokenException(), FaultParty.Sender);
			}
			byte[] array = new byte[num];
			Array.Copy(byteArray, byteIndex, array, 0, num);
			byteIndex += num;
			ADObjectId result;
			try
			{
				result = new ADObjectId(array, PartnerAccessToken.DefaultEncoding);
			}
			catch (FormatException arg)
			{
				ExTraceGlobals.ProxyEvaluatorTracer.TraceWarning<FormatException>(0L, "[PartnerAccessToken::DeserializeADObjectIdFromByteArray] FormatException hit while creating a new ADObjectId. Exception: {0}", arg);
				throw FaultExceptionUtilities.CreateFault(new InvalidSerializedAccessTokenException(), FaultParty.Sender);
			}
			return result;
		}

		private int GetByteCountToSerializeToken()
		{
			int num = 0;
			num += PartnerAccessToken.partnerAccessTokenCookie.Length;
			num++;
			num += 4;
			num += Encoding.UTF8.GetByteCount(this.partnerUser);
			num += 4;
			num += Encoding.UTF8.GetByteCount(this.tenantName);
			num += 4;
			num += this.organizationalUnit.GetByteCount(PartnerAccessToken.DefaultEncoding);
			num += 4;
			return num + this.configurationUnit.GetByteCount(PartnerAccessToken.DefaultEncoding);
		}

		private OrganizationId OrganizationIdGetter()
		{
			return new OrganizationId(this.organizationalUnit, this.configurationUnit);
		}

		private const byte PartnerAccessTokenVersion1 = 1;

		private static readonly Encoding DefaultEncoding = Encoding.Unicode;

		private static readonly byte[] partnerAccessTokenCookie = new byte[]
		{
			80,
			65,
			84
		};

		private readonly ADObjectId organizationalUnit;

		private readonly ADObjectId configurationUnit;

		private readonly string tenantName;

		private readonly string partnerUser;
	}
}
