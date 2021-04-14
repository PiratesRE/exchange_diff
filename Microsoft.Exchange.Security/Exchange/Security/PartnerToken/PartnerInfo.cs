using System;
using System.Collections.Generic;
using System.IdentityModel.Claims;
using System.IdentityModel.Tokens;
using System.Security.Principal;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Authentication;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.Net.WSTrust;

namespace Microsoft.Exchange.Security.PartnerToken
{
	internal class PartnerInfo
	{
		private PartnerInfo()
		{
		}

		private PartnerInfo(string userPrincipalName, OrganizationId organizationId, string externalOrganizationId, string[] roleGroupExternalObjectIds)
		{
			this.UPN = userPrincipalName;
			this.OrganizationId = organizationId;
			this.ExternalOrganizationId = externalOrganizationId;
			this.RoleGroupExternalObjectIds = roleGroupExternalObjectIds;
			this.knownPartnership = new Dictionary<OrganizationId, bool>(1024);
		}

		public string UPN { get; private set; }

		public OrganizationId OrganizationId { get; private set; }

		public string ExternalOrganizationId { get; private set; }

		public string[] RoleGroupExternalObjectIds { get; private set; }

		public static PartnerInfo Invalid
		{
			get
			{
				return PartnerInfo.invalidPartnerInfo;
			}
		}

		public static PartnerInfo CreateFromADObjectId(ADObjectId objectId, OrganizationId organizationId)
		{
			if (objectId == null)
			{
				throw new ArgumentNullException("objectId");
			}
			if (organizationId == null)
			{
				throw new ArgumentNullException("organizationId");
			}
			if (OrganizationId.ForestWideOrgId.Equals(organizationId))
			{
				ExTraceGlobals.PartnerTokenTracer.TraceDebug(0L, "[PartnerInfo::CreateFromADObjectId] we do not create partner info for the first org user");
				return PartnerInfo.Invalid;
			}
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId);
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, sessionSettings, 159, "CreateFromADObjectId", "f:\\15.00.1497\\sources\\dev\\Security\\src\\Authentication\\PartnerToken\\PartnerInfo.cs");
			ADUser aduser = tenantOrRootOrgRecipientSession.FindADUserByObjectId(objectId);
			if (aduser == null)
			{
				ExTraceGlobals.PartnerTokenTracer.TraceDebug<ADObjectId>(0L, "[PartnerInfo::CreateFromADObjectId] fail to find ADUser for objectId {0}", objectId);
				return PartnerInfo.Invalid;
			}
			string userPrincipalName = aduser.UserPrincipalName;
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, sessionSettings, 177, "CreateFromADObjectId", "f:\\15.00.1497\\sources\\dev\\Security\\src\\Authentication\\PartnerToken\\PartnerInfo.cs");
			ExchangeConfigurationUnit exchangeConfigurationUnit = tenantOrTopologyConfigurationSession.Read<ExchangeConfigurationUnit>(organizationId.ConfigurationUnit);
			if (exchangeConfigurationUnit == null)
			{
				ExTraceGlobals.PartnerTokenTracer.TraceWarning<OrganizationId>(0L, "[PartnerInfo::CreateFromADObjectId] failed to read the configuration unit for organization {0}", organizationId);
				return PartnerInfo.Invalid;
			}
			string[] roleGroupExternalObjectIds = null;
			if (!PartnerInfo.TryGetRoleGroupExternalDirectoryObjectId(tenantOrRootOrgRecipientSession, aduser, out roleGroupExternalObjectIds))
			{
				ExTraceGlobals.PartnerTokenTracer.TraceDebug<string, ADObjectId>(0L, "[PartnerInfo::CreateFromADObjectId] user {0} (objectId {1}) has no partner managed groups.", userPrincipalName, objectId);
				return PartnerInfo.Invalid;
			}
			return new PartnerInfo(userPrincipalName, organizationId, exchangeConfigurationUnit.ExternalDirectoryOrganizationId, roleGroupExternalObjectIds);
		}

		public bool HasPartnerRelationship(OrganizationId delegatedOrganizationId)
		{
			if (delegatedOrganizationId == null)
			{
				return true;
			}
			bool flag2;
			lock (this.knownPartnership)
			{
				if (!this.knownPartnership.TryGetValue(delegatedOrganizationId, out flag2))
				{
					flag2 = PartnerToken.FindLinkedRoleGroupInDelegatedOrganization(delegatedOrganizationId, this.ExternalOrganizationId, this.RoleGroupExternalObjectIds);
					this.knownPartnership.Add(delegatedOrganizationId, flag2);
				}
			}
			return flag2;
		}

		public XmlElement CreateSamlToken(string assertionId, string targetTenant, byte[] binarySecret, TimeSpan tokenLifetime)
		{
			ExternalAuthentication current = ExternalAuthentication.GetCurrent();
			SecurityTokenService securityTokenService = current.GetSecurityTokenService(this.OrganizationId);
			XmlElement result;
			using (X509SecurityToken x509SecurityToken = new X509SecurityToken(securityTokenService.Certificate))
			{
				SecurityKeyIdentifier securityKeyIdentifier = new SecurityKeyIdentifier(new SecurityKeyIdentifierClause[]
				{
					x509SecurityToken.CreateKeyIdentifierClause<X509SubjectKeyIdentifierClause>()
				});
				SecurityKey securityKey = x509SecurityToken.SecurityKeys[0];
				SecurityKeyIdentifier proofKeyIdentifier = new SecurityKeyIdentifier(new SecurityKeyIdentifierClause[]
				{
					new EncryptedKeyIdentifierClause(securityKey.EncryptKey("http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p", binarySecret), "http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p", securityKeyIdentifier, PartnerInfo.KeyName)
				});
				SamlAssertion samlAssertion = this.CreateSamlAssertion(assertionId, targetTenant, current.TokenValidator.TargetUri, tokenLifetime, proofKeyIdentifier);
				samlAssertion.SigningCredentials = new SigningCredentials(securityKey, "http://www.w3.org/2000/09/xmldsig#rsa-sha1", "http://www.w3.org/2000/09/xmldsig#sha1", securityKeyIdentifier);
				SamlSecurityToken samlSecurityToken = new SamlSecurityToken(samlAssertion);
				XmlElement xmlFromSecurityToken = SecurityTokenService.GetXmlFromSecurityToken(samlSecurityToken);
				if (ExTraceGlobals.PartnerTokenTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.PartnerTokenTracer.TraceDebug<string, string>(0L, "[PartnerInfo.CreateSamlToken] partner token was generated for {0} as {1}", this.UPN, xmlFromSecurityToken.OuterXml);
				}
				result = xmlFromSecurityToken;
			}
			return result;
		}

		private SamlAssertion CreateSamlAssertion(string assertionId, string targetTenant, Uri targetUri, TimeSpan tokenLifetime, SecurityKeyIdentifier proofKeyIdentifier)
		{
			DateTime utcNow = DateTime.UtcNow;
			SamlConditions samlConditions = new SamlConditions(utcNow, utcNow.Add(tokenLifetime));
			samlConditions.Conditions.Add(new SamlAudienceRestrictionCondition(new Uri[]
			{
				targetUri
			}));
			SamlSubject samlSubject = new SamlSubject("http://schemas.xmlsoap.org/claims/UPN", null, this.UPN, new string[]
			{
				SamlConstants.HolderOfKey
			}, null, proofKeyIdentifier);
			SamlAttributeStatement samlAttributeStatement = new SamlAttributeStatement(samlSubject, new SamlAttribute[]
			{
				new SamlAttribute(new Claim("http://schemas.microsoft.com/exchange/services/2006/partnertoken/targettenant", targetTenant, Rights.PossessProperty)),
				new SamlAttribute(new Claim("http://schemas.microsoft.com/exchange/services/2006/partnertoken/linkedpartnerorganizationid", this.ExternalOrganizationId, Rights.PossessProperty)),
				new SamlAttribute("http://schemas.microsoft.com/exchange/services/2006/partnertoken", "linkedpartnergroupid", this.RoleGroupExternalObjectIds)
			});
			return new SamlAssertion(assertionId, "http://schemas.microsoft.com/exchange/2010/autodiscover/getusersettings", utcNow, samlConditions, null, new SamlStatement[]
			{
				samlAttributeStatement
			});
		}

		public static XmlElement GetTokenReference(string assertionId)
		{
			return SecurityTokenService.CreateSamlAssertionSecurityTokenReference(SecurityTokenService.CreateXmlDocument(), assertionId);
		}

		private static bool TryGetRoleGroupExternalDirectoryObjectId(IRecipientSession recipientSession, ADRawEntry userEntry, out string[] roleGroupExternalObjectIds)
		{
			roleGroupExternalObjectIds = null;
			string[] array = recipientSession.GetTokenSids(userEntry, AssignmentMethod.SecurityGroup).ToArray();
			string arg = (string)userEntry[ADUserSchema.UserPrincipalName];
			if (array == null || array.Length == 0)
			{
				ExTraceGlobals.PartnerTokenTracer.TraceWarning<string>(0L, "[PartnerInfo::TryGetRoleGroupExternalDirectoryObjectId] user {0} has no security group assigned.", arg);
				return false;
			}
			if (ExTraceGlobals.PartnerTokenTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.PartnerTokenTracer.TraceDebug<string, string>(0L, "[PartnerInfo::TryGetRoleGroupExternalDirectoryObjectId] user {0} has group sids {1}.", arg, string.Join(",", array));
			}
			ADObjectId[] array2 = recipientSession.ResolveSidsToADObjectIds(array);
			Result<ADGroup>[] array3 = recipientSession.ReadMultipleADGroups(array2);
			List<string> list = new List<string>();
			for (int i = 0; i < array3.Length; i++)
			{
				if (array3[i].Error != null)
				{
					ExTraceGlobals.PartnerTokenTracer.TraceWarning<ADObjectId, ProviderError>(0L, "[PartnerInfo::TryGetRoleGroupExternalDirectoryObjectId] failed to read the group information for sid {0}, error: {1}", array2[i], array3[i].Error);
				}
				else
				{
					ADGroup data = array3[i].Data;
					ExTraceGlobals.PartnerTokenTracer.TraceDebug<SecurityIdentifier, ADGroup>(0L, "[PartnerInfo::TryGetRoleGroupExternalDirectoryObjectId] group information for sid {0} is read as {1}", data.Sid, data);
					if (!string.IsNullOrEmpty(data.ExternalDirectoryObjectId) && data.RawCapabilities != null && data.RawCapabilities.Contains(Capability.Partner_Managed))
					{
						ExTraceGlobals.PartnerTokenTracer.TraceDebug<string, SecurityIdentifier>(0L, "[PartnerInfo::TryGetRoleGroupExternalDirectoryObjectId] add group {0} with sid {1}", data.Name, data.Sid);
						list.Add(data.ExternalDirectoryObjectId);
					}
					else
					{
						ExTraceGlobals.PartnerTokenTracer.TraceDebug<string, SecurityIdentifier>(0L, "[PartnerInfo::TryGetRoleGroupExternalDirectoryObjectId] skip group {0} with sid {1} which is not partner_managed", data.Name, data.Sid);
					}
				}
			}
			if (list.Count > 0)
			{
				roleGroupExternalObjectIds = list.ToArray();
				return true;
			}
			return false;
		}

		internal static readonly string KeyName = "#exch";

		private Dictionary<OrganizationId, bool> knownPartnership;

		private static PartnerInfo invalidPartnerInfo = new PartnerInfo();

		private static PropertyDefinition[] userProperties = new PropertyDefinition[]
		{
			ADObjectSchema.OrganizationId,
			ADObjectSchema.Id,
			ADUserSchema.UserPrincipalName
		};

		internal static PropertyDefinition[] groupProperties = new PropertyDefinition[]
		{
			ADObjectSchema.RawName,
			ADObjectSchema.Name,
			ADGroupSchema.LinkedPartnerGroupAndOrganizationId,
			ADObjectSchema.Id,
			ADObjectSchema.ExchangeVersion
		};
	}
}
