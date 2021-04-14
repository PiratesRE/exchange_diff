using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Security
{
	internal static class SecurityStrings
	{
		static SecurityStrings()
		{
			SecurityStrings.stringIDs.Add(3271480525U, "ResultThumbprintNotSet");
			SecurityStrings.stringIDs.Add(1735881142U, "ReadCertificates");
			SecurityStrings.stringIDs.Add(3167281586U, "LoadPartnerApplication");
			SecurityStrings.stringIDs.Add(3026893530U, "CheckCurrentCertificate");
			SecurityStrings.stringIDs.Add(1008692634U, "CheckNextCertificate");
			SecurityStrings.stringIDs.Add(3117434263U, "CheckAuthConfigRealm");
			SecurityStrings.stringIDs.Add(227590970U, "ResultAuthServerDisabled");
			SecurityStrings.stringIDs.Add(2753441348U, "ResultAuthServerOK");
			SecurityStrings.stringIDs.Add(140719347U, "LoadAuthServer");
			SecurityStrings.stringIDs.Add(3171288429U, "ResultAuthConfigFound");
			SecurityStrings.stringIDs.Add(1849315756U, "LoadConfiguration");
			SecurityStrings.stringIDs.Add(1673247710U, "CheckServiceName");
			SecurityStrings.stringIDs.Add(1391827654U, "MissingWindowsAccessToken");
			SecurityStrings.stringIDs.Add(92707853U, "ResultPartnerApplicationDisabled");
			SecurityStrings.stringIDs.Add(3173209329U, "ResultPartnerApplicationOK");
			SecurityStrings.stringIDs.Add(2239003722U, "LoadAuthConfig");
			SecurityStrings.stringIDs.Add(664393098U, "CheckPreviousCertificate");
		}

		public static LocalizedString CannotResolveOrganization(string orgName)
		{
			return new LocalizedString("CannotResolveOrganization", SecurityStrings.ResourceManager, new object[]
			{
				orgName
			});
		}

		public static LocalizedString LowPasswordConfidence(string upn)
		{
			return new LocalizedString("LowPasswordConfidence", SecurityStrings.ResourceManager, new object[]
			{
				upn
			});
		}

		public static LocalizedString UserIsDisabled(string upn)
		{
			return new LocalizedString("UserIsDisabled", SecurityStrings.ResourceManager, new object[]
			{
				upn
			});
		}

		public static LocalizedString ResultThumbprintNotSet
		{
			get
			{
				return new LocalizedString("ResultThumbprintNotSet", SecurityStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SourceServerNoTokenSerializationPermission(string serverIdentity)
		{
			return new LocalizedString("SourceServerNoTokenSerializationPermission", SecurityStrings.ResourceManager, new object[]
			{
				serverIdentity
			});
		}

		public static LocalizedString ReadCertificates
		{
			get
			{
				return new LocalizedString("ReadCertificates", SecurityStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LoadPartnerApplication
		{
			get
			{
				return new LocalizedString("LoadPartnerApplication", SecurityStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ResultCertHasNoPrivateKey(string thumbprint)
		{
			return new LocalizedString("ResultCertHasNoPrivateKey", SecurityStrings.ResourceManager, new object[]
			{
				thumbprint
			});
		}

		public static LocalizedString CheckCurrentCertificate
		{
			get
			{
				return new LocalizedString("CheckCurrentCertificate", SecurityStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CheckNextCertificate
		{
			get
			{
				return new LocalizedString("CheckNextCertificate", SecurityStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ResultADOperationFailure(string code, string message)
		{
			return new LocalizedString("ResultADOperationFailure", SecurityStrings.ResourceManager, new object[]
			{
				code,
				message
			});
		}

		public static LocalizedString FailedToLogon(string upn)
		{
			return new LocalizedString("FailedToLogon", SecurityStrings.ResourceManager, new object[]
			{
				upn
			});
		}

		public static LocalizedString CheckAuthServer(string name)
		{
			return new LocalizedString("CheckAuthServer", SecurityStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString CheckAuthConfigRealm
		{
			get
			{
				return new LocalizedString("CheckAuthConfigRealm", SecurityStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ResultCertInvalid(string thumbprint, DateTime notBefore, DateTime notAfer)
		{
			return new LocalizedString("ResultCertInvalid", SecurityStrings.ResourceManager, new object[]
			{
				thumbprint,
				notBefore,
				notAfer
			});
		}

		public static LocalizedString MissingExtensionDataKey(string key)
		{
			return new LocalizedString("MissingExtensionDataKey", SecurityStrings.ResourceManager, new object[]
			{
				key
			});
		}

		public static LocalizedString ResultAuthServerDisabled
		{
			get
			{
				return new LocalizedString("ResultAuthServerDisabled", SecurityStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ResultAuthServerOK
		{
			get
			{
				return new LocalizedString("ResultAuthServerOK", SecurityStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidOAuthLinkedAccountException(string partnerApplication, string linkedAccount)
		{
			return new LocalizedString("InvalidOAuthLinkedAccountException", SecurityStrings.ResourceManager, new object[]
			{
				partnerApplication,
				linkedAccount
			});
		}

		public static LocalizedString ResultMatchServiceNameDefaultValue(string actual)
		{
			return new LocalizedString("ResultMatchServiceNameDefaultValue", SecurityStrings.ResourceManager, new object[]
			{
				actual
			});
		}

		public static LocalizedString LoadAuthServer
		{
			get
			{
				return new LocalizedString("LoadAuthServer", SecurityStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotLookupUser(string partitionId, string sid, string reason)
		{
			return new LocalizedString("CannotLookupUser", SecurityStrings.ResourceManager, new object[]
			{
				partitionId,
				sid,
				reason
			});
		}

		public static LocalizedString ResultAuthConfigFound
		{
			get
			{
				return new LocalizedString("ResultAuthConfigFound", SecurityStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LoadConfiguration
		{
			get
			{
				return new LocalizedString("LoadConfiguration", SecurityStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ResultDidNotMatchServiceNameDefaultValue(string actual, string expected)
		{
			return new LocalizedString("ResultDidNotMatchServiceNameDefaultValue", SecurityStrings.ResourceManager, new object[]
			{
				actual,
				expected
			});
		}

		public static LocalizedString ResultCertNotFound(string thumbprint)
		{
			return new LocalizedString("ResultCertNotFound", SecurityStrings.ResourceManager, new object[]
			{
				thumbprint
			});
		}

		public static LocalizedString ResultAuthConfigRealmOverwrote(string overwrite)
		{
			return new LocalizedString("ResultAuthConfigRealmOverwrote", SecurityStrings.ResourceManager, new object[]
			{
				overwrite
			});
		}

		public static LocalizedString ResultLoadConfigurationException(object exception)
		{
			return new LocalizedString("ResultLoadConfigurationException", SecurityStrings.ResourceManager, new object[]
			{
				exception
			});
		}

		public static LocalizedString CheckServiceName
		{
			get
			{
				return new LocalizedString("CheckServiceName", SecurityStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ResultReadCertificatesException(object exception)
		{
			return new LocalizedString("ResultReadCertificatesException", SecurityStrings.ResourceManager, new object[]
			{
				exception
			});
		}

		public static LocalizedString LowPasswordConfidenceWithException(string upn, Exception e)
		{
			return new LocalizedString("LowPasswordConfidenceWithException", SecurityStrings.ResourceManager, new object[]
			{
				upn,
				e
			});
		}

		public static LocalizedString MissingWindowsAccessToken
		{
			get
			{
				return new LocalizedString("MissingWindowsAccessToken", SecurityStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ResultPartnerApplicationDisabled
		{
			get
			{
				return new LocalizedString("ResultPartnerApplicationDisabled", SecurityStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ResultPartnerApplicationOK
		{
			get
			{
				return new LocalizedString("ResultPartnerApplicationOK", SecurityStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AccessTokenTypeNotSupported(string type)
		{
			return new LocalizedString("AccessTokenTypeNotSupported", SecurityStrings.ResourceManager, new object[]
			{
				type
			});
		}

		public static LocalizedString InvalidExtensionDataKey(string key)
		{
			return new LocalizedString("InvalidExtensionDataKey", SecurityStrings.ResourceManager, new object[]
			{
				key
			});
		}

		public static LocalizedString BackendRehydrationException(LocalizedString reason)
		{
			return new LocalizedString("BackendRehydrationException", SecurityStrings.ResourceManager, new object[]
			{
				reason
			});
		}

		public static LocalizedString ResultPartnerApplicationsFound(int count)
		{
			return new LocalizedString("ResultPartnerApplicationsFound", SecurityStrings.ResourceManager, new object[]
			{
				count
			});
		}

		public static LocalizedString ResultAuthServersFound(int count)
		{
			return new LocalizedString("ResultAuthServersFound", SecurityStrings.ResourceManager, new object[]
			{
				count
			});
		}

		public static LocalizedString CheckPartnerApplication(string name)
		{
			return new LocalizedString("CheckPartnerApplication", SecurityStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString LoadAuthConfig
		{
			get
			{
				return new LocalizedString("LoadAuthConfig", SecurityStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotLookupUserEx(string puid, string domainName)
		{
			return new LocalizedString("CannotLookupUserEx", SecurityStrings.ResourceManager, new object[]
			{
				puid,
				domainName
			});
		}

		public static LocalizedString ResultCertValid(string thumbprint)
		{
			return new LocalizedString("ResultCertValid", SecurityStrings.ResourceManager, new object[]
			{
				thumbprint
			});
		}

		public static LocalizedString CheckPreviousCertificate
		{
			get
			{
				return new LocalizedString("CheckPreviousCertificate", SecurityStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(SecurityStrings.IDs key)
		{
			return new LocalizedString(SecurityStrings.stringIDs[(uint)key], SecurityStrings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(17);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Security.Strings", typeof(SecurityStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			ResultThumbprintNotSet = 3271480525U,
			ReadCertificates = 1735881142U,
			LoadPartnerApplication = 3167281586U,
			CheckCurrentCertificate = 3026893530U,
			CheckNextCertificate = 1008692634U,
			CheckAuthConfigRealm = 3117434263U,
			ResultAuthServerDisabled = 227590970U,
			ResultAuthServerOK = 2753441348U,
			LoadAuthServer = 140719347U,
			ResultAuthConfigFound = 3171288429U,
			LoadConfiguration = 1849315756U,
			CheckServiceName = 1673247710U,
			MissingWindowsAccessToken = 1391827654U,
			ResultPartnerApplicationDisabled = 92707853U,
			ResultPartnerApplicationOK = 3173209329U,
			LoadAuthConfig = 2239003722U,
			CheckPreviousCertificate = 664393098U
		}

		private enum ParamIDs
		{
			CannotResolveOrganization,
			LowPasswordConfidence,
			UserIsDisabled,
			SourceServerNoTokenSerializationPermission,
			ResultCertHasNoPrivateKey,
			ResultADOperationFailure,
			FailedToLogon,
			CheckAuthServer,
			ResultCertInvalid,
			MissingExtensionDataKey,
			InvalidOAuthLinkedAccountException,
			ResultMatchServiceNameDefaultValue,
			CannotLookupUser,
			ResultDidNotMatchServiceNameDefaultValue,
			ResultCertNotFound,
			ResultAuthConfigRealmOverwrote,
			ResultLoadConfigurationException,
			ResultReadCertificatesException,
			LowPasswordConfidenceWithException,
			AccessTokenTypeNotSupported,
			InvalidExtensionDataKey,
			BackendRehydrationException,
			ResultPartnerApplicationsFound,
			ResultAuthServersFound,
			CheckPartnerApplication,
			CannotLookupUserEx,
			ResultCertValid
		}
	}
}
