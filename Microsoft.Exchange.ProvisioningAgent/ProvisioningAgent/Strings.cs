using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.ProvisioningAgent
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(3378717027U, "LdapFilterErrorInvalidBooleanValue");
			Strings.stringIDs.Add(1860494422U, "LdapFilterErrorNotSupportSingleComp");
			Strings.stringIDs.Add(1386447651U, "InvokedProvisionDefaultProperties");
			Strings.stringIDs.Add(4236118860U, "ProxyDLLError");
			Strings.stringIDs.Add(1091171211U, "ExitedValidate");
			Strings.stringIDs.Add(4230107429U, "LdapFilterErrorQueryTooLong");
			Strings.stringIDs.Add(1688256845U, "LdapFilterErrorBracketMismatch");
			Strings.stringIDs.Add(3060439136U, "ErrorUpdateAffectedIConfigurableBadRetObject");
			Strings.stringIDs.Add(1755960558U, "ErrorNoMailboxPlan");
			Strings.stringIDs.Add(449876541U, "ProxyDLLDiskSpace");
			Strings.stringIDs.Add(56024811U, "LdapFilterErrorInvalidToken");
			Strings.stringIDs.Add(1880382981U, "ProxyDLLContention");
			Strings.stringIDs.Add(2164854509U, "ProxyDLLSyntax");
			Strings.stringIDs.Add(2800661133U, "EnteredOnComplete");
			Strings.stringIDs.Add(4152138657U, "InvokedUpdateAffectedIConfigurable");
			Strings.stringIDs.Add(1989031583U, "ProxyNotImplemented");
			Strings.stringIDs.Add(3580054472U, "RunningAsSystem");
			Strings.stringIDs.Add(3269299477U, "ProxyDLLOOM");
			Strings.stringIDs.Add(3410851087U, "ProxyDLLNotFound");
			Strings.stringIDs.Add(2781326009U, "ErrorNoDefaultMailboxPlan");
			Strings.stringIDs.Add(3226694139U, "ExitedInitializeConfig");
			Strings.stringIDs.Add(1998083787U, "EnteredInitializeConfig");
			Strings.stringIDs.Add(1775082576U, "InvokedValidate");
			Strings.stringIDs.Add(1859635364U, "ErrorUnexpectedReturnTypeInValidate");
			Strings.stringIDs.Add(2735291928U, "ProxyDLLConfig");
			Strings.stringIDs.Add(3936814429U, "LdapFilterErrorAnrIsNotSupported");
			Strings.stringIDs.Add(3897335713U, "ExitedOnComplete");
			Strings.stringIDs.Add(3269299668U, "ProxyDLLEOF");
			Strings.stringIDs.Add(1703828310U, "ProxyDLLProtocol");
			Strings.stringIDs.Add(4045631128U, "LdapFilterErrorInvalidGtLtOperand");
			Strings.stringIDs.Add(2520898135U, "WarningNoDefaultMailboxPlan");
			Strings.stringIDs.Add(1236062515U, "EnteredValidate");
			Strings.stringIDs.Add(1767506191U, "VerboseZeroProvisioningPolicyFound");
			Strings.stringIDs.Add(1005127777U, "NoError");
			Strings.stringIDs.Add(3641189505U, "WarningNoDefaultMailboxPlanUsingNonDefault");
			Strings.stringIDs.Add(367606263U, "ProxyDLLDefault");
			Strings.stringIDs.Add(156106839U, "ProxyDLLSoftware");
			Strings.stringIDs.Add(135248896U, "LdapFilterErrorInvalidBitwiseOperand");
			Strings.stringIDs.Add(531667494U, "FailedToEnableEwsMailboxLogger");
			Strings.stringIDs.Add(2753705114U, "ErrorTooManyDefaultMailboxPlans");
			Strings.stringIDs.Add(3355805493U, "ErrorLoadBalancingFailedToFindDatabase");
			Strings.stringIDs.Add(2370329957U, "NotRunningAsSystem");
			Strings.stringIDs.Add(1324265457U, "LdapFilterErrorInvalidWildCardGtLt");
		}

		public static LocalizedString LdapFilterErrorInvalidBooleanValue
		{
			get
			{
				return new LocalizedString("LdapFilterErrorInvalidBooleanValue", "ExF79F20", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LdapFilterErrorNotSupportSingleComp
		{
			get
			{
				return new LocalizedString("LdapFilterErrorNotSupportSingleComp", "ExEDC679", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorProxyDllNotFound(string dll)
		{
			return new LocalizedString("ErrorProxyDllNotFound", "ExE0C9D5", false, true, Strings.ResourceManager, new object[]
			{
				dll
			});
		}

		public static LocalizedString InvokedProvisionDefaultProperties
		{
			get
			{
				return new LocalizedString("InvokedProvisionDefaultProperties", "Ex025540", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProxyDLLError
		{
			get
			{
				return new LocalizedString("ProxyDLLError", "ExCE9170", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseAddSecondaryEmailAddress(string address)
		{
			return new LocalizedString("VerboseAddSecondaryEmailAddress", "Ex8CF8B9", false, true, Strings.ResourceManager, new object[]
			{
				address
			});
		}

		public static LocalizedString LdapFilterErrorUnsupportedAttrbuteType(string type)
		{
			return new LocalizedString("LdapFilterErrorUnsupportedAttrbuteType", "Ex375F26", false, true, Strings.ResourceManager, new object[]
			{
				type
			});
		}

		public static LocalizedString VerboseSetWindowsEmailAddress(string addres)
		{
			return new LocalizedString("VerboseSetWindowsEmailAddress", "ExA71AE7", false, true, Strings.ResourceManager, new object[]
			{
				addres
			});
		}

		public static LocalizedString VerboseAddAddressListMemberShip(string id)
		{
			return new LocalizedString("VerboseAddAddressListMemberShip", "Ex86C423", false, true, Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString ExitedValidate
		{
			get
			{
				return new LocalizedString("ExitedValidate", "Ex7C45B4", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseNoDefaultForTask(string task)
		{
			return new LocalizedString("VerboseNoDefaultForTask", "Ex16E409", false, true, Strings.ResourceManager, new object[]
			{
				task
			});
		}

		public static LocalizedString ErrorInvalidBaseAddress(string baseAddress)
		{
			return new LocalizedString("ErrorInvalidBaseAddress", "Ex7D174C", false, true, Strings.ResourceManager, new object[]
			{
				baseAddress
			});
		}

		public static LocalizedString ScriptReturned(string output)
		{
			return new LocalizedString("ScriptReturned", "Ex44DA9E", false, true, Strings.ResourceManager, new object[]
			{
				output
			});
		}

		public static LocalizedString XmlErrorMissingInnerText(string file)
		{
			return new LocalizedString("XmlErrorMissingInnerText", "Ex1C0E71", false, true, Strings.ResourceManager, new object[]
			{
				file
			});
		}

		public static LocalizedString ErrorProxyGenErrorEntryPoint(string module, string addressType, string methodName)
		{
			return new LocalizedString("ErrorProxyGenErrorEntryPoint", "Ex45A6FB", false, true, Strings.ResourceManager, new object[]
			{
				module,
				addressType,
				methodName
			});
		}

		public static LocalizedString ErrorInProvisionDefaultProperties(string inner)
		{
			return new LocalizedString("ErrorInProvisionDefaultProperties", "ExDF2BDD", false, true, Strings.ResourceManager, new object[]
			{
				inner
			});
		}

		public static LocalizedString LdapFilterErrorQueryTooLong
		{
			get
			{
				return new LocalizedString("LdapFilterErrorQueryTooLong", "Ex7DB526", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseEmailAddressPoliciesForOganizationFromDC(string org, string dc)
		{
			return new LocalizedString("VerboseEmailAddressPoliciesForOganizationFromDC", "Ex2E6460", false, true, Strings.ResourceManager, new object[]
			{
				org,
				dc
			});
		}

		public static LocalizedString VerboseProvisionDefaultProperties(string org)
		{
			return new LocalizedString("VerboseProvisionDefaultProperties", "ExBB2146", false, true, Strings.ResourceManager, new object[]
			{
				org
			});
		}

		public static LocalizedString LdapFilterErrorBracketMismatch
		{
			get
			{
				return new LocalizedString("LdapFilterErrorBracketMismatch", "Ex78886A", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseSettingDisplayName(string name)
		{
			return new LocalizedString("VerboseSettingDisplayName", "Ex216486", false, true, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ErrorUpdateAffectedIConfigurableBadRetObject
		{
			get
			{
				return new LocalizedString("ErrorUpdateAffectedIConfigurableBadRetObject", "Ex666611", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNoMailboxPlan
		{
			get
			{
				return new LocalizedString("ErrorNoMailboxPlan", "ExFAD707", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProxyDLLDiskSpace
		{
			get
			{
				return new LocalizedString("ProxyDLLDiskSpace", "ExB64B84", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LdapFilterErrorInvalidToken
		{
			get
			{
				return new LocalizedString("LdapFilterErrorInvalidToken", "Ex9FB888", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProxyDLLContention
		{
			get
			{
				return new LocalizedString("ProxyDLLContention", "ExEABD38", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LdapFilterErrorNoAttributeType(string filter)
		{
			return new LocalizedString("LdapFilterErrorNoAttributeType", "Ex4E7238", false, true, Strings.ResourceManager, new object[]
			{
				filter
			});
		}

		public static LocalizedString ProxyDLLSyntax
		{
			get
			{
				return new LocalizedString("ProxyDLLSyntax", "ExD0AF2A", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EnteredOnComplete
		{
			get
			{
				return new LocalizedString("EnteredOnComplete", "ExC8CC66", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvokedUpdateAffectedIConfigurable
		{
			get
			{
				return new LocalizedString("InvokedUpdateAffectedIConfigurable", "ExCC1F76", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProxyNotImplemented
		{
			get
			{
				return new LocalizedString("ProxyNotImplemented", "ExF3E01F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidLdapFilter(string reason)
		{
			return new LocalizedString("ErrorInvalidLdapFilter", "Ex84358D", false, true, Strings.ResourceManager, new object[]
			{
				reason
			});
		}

		public static LocalizedString RunningAsSystem
		{
			get
			{
				return new LocalizedString("RunningAsSystem", "ExC0B449", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvokedOnComplete(string first, string second)
		{
			return new LocalizedString("InvokedOnComplete", "Ex817DB0", false, true, Strings.ResourceManager, new object[]
			{
				first,
				second
			});
		}

		public static LocalizedString MultiTemplatePolicyFound(string tag)
		{
			return new LocalizedString("MultiTemplatePolicyFound", "ExEFE4F4", false, true, Strings.ResourceManager, new object[]
			{
				tag
			});
		}

		public static LocalizedString LdapFilterErrorNoAttributeValue(string filter)
		{
			return new LocalizedString("LdapFilterErrorNoAttributeValue", "ExFDFEA3", false, true, Strings.ResourceManager, new object[]
			{
				filter
			});
		}

		public static LocalizedString ProxyDLLOOM
		{
			get
			{
				return new LocalizedString("ProxyDLLOOM", "Ex024420", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProxyDLLNotFound
		{
			get
			{
				return new LocalizedString("ProxyDLLNotFound", "Ex3E8843", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorFailedToGenerateUniqueProxy(string baseAddress, string recipient)
		{
			return new LocalizedString("ErrorFailedToGenerateUniqueProxy", "Ex1D6500", false, true, Strings.ResourceManager, new object[]
			{
				baseAddress,
				recipient
			});
		}

		public static LocalizedString ErrorInValidate(string inner)
		{
			return new LocalizedString("ErrorInValidate", "ExA96987", false, true, Strings.ResourceManager, new object[]
			{
				inner
			});
		}

		public static LocalizedString ErrorNoDefaultMailboxPlan
		{
			get
			{
				return new LocalizedString("ErrorNoDefaultMailboxPlan", "Ex76EB50", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExitedInitializeConfig
		{
			get
			{
				return new LocalizedString("ExitedInitializeConfig", "Ex46F1E9", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorFailedToFindAddressTypeObject(string type)
		{
			return new LocalizedString("ErrorFailedToFindAddressTypeObject", "Ex0553B4", false, true, Strings.ResourceManager, new object[]
			{
				type
			});
		}

		public static LocalizedString VerboseTaskUseTypeAsDefault(string task, string type)
		{
			return new LocalizedString("VerboseTaskUseTypeAsDefault", "Ex637177", false, true, Strings.ResourceManager, new object[]
			{
				task,
				type
			});
		}

		public static LocalizedString VerboseSettingLegacyExchangeDN(string legacyDN)
		{
			return new LocalizedString("VerboseSettingLegacyExchangeDN", "Ex81C24E", false, true, Strings.ResourceManager, new object[]
			{
				legacyDN
			});
		}

		public static LocalizedString ErrorProvisioningPolicyCorrupt(string identity)
		{
			return new LocalizedString("ErrorProvisioningPolicyCorrupt", "Ex4B23DC", false, true, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString VerbosePolicyProviderUseDomainController(string dc)
		{
			return new LocalizedString("VerbosePolicyProviderUseDomainController", "ExC1187D", false, true, Strings.ResourceManager, new object[]
			{
				dc
			});
		}

		public static LocalizedString ErrorFailedToReadRegistryKey(string key, string msg)
		{
			return new LocalizedString("ErrorFailedToReadRegistryKey", "ExA70D00", false, true, Strings.ResourceManager, new object[]
			{
				key,
				msg
			});
		}

		public static LocalizedString EnteredInitializeConfig
		{
			get
			{
				return new LocalizedString("EnteredInitializeConfig", "ExC318A7", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LdapFilterErrorInvalidDecimal(string value)
		{
			return new LocalizedString("LdapFilterErrorInvalidDecimal", "Ex16EF43", false, true, Strings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString InvokedValidate
		{
			get
			{
				return new LocalizedString("InvokedValidate", "ExA995B0", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LdapFilterErrorObjectCategoryNotFound(string name)
		{
			return new LocalizedString("LdapFilterErrorObjectCategoryNotFound", "Ex4B0417", false, true, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ErrorUnexpectedReturnTypeInValidate
		{
			get
			{
				return new LocalizedString("ErrorUnexpectedReturnTypeInValidate", "ExB328F1", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProxyDLLConfig
		{
			get
			{
				return new LocalizedString("ProxyDLLConfig", "Ex381B58", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LdapFilterErrorAnrIsNotSupported
		{
			get
			{
				return new LocalizedString("LdapFilterErrorAnrIsNotSupported", "Ex476BF1", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInUpdateAffectedIConfigurable(string inner)
		{
			return new LocalizedString("ErrorInUpdateAffectedIConfigurable", "Ex052563", false, true, Strings.ResourceManager, new object[]
			{
				inner
			});
		}

		public static LocalizedString ExitedOnComplete
		{
			get
			{
				return new LocalizedString("ExitedOnComplete", "ExCB425E", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseNoNeedToValidate(string type)
		{
			return new LocalizedString("VerboseNoNeedToValidate", "ExA207E6", false, true, Strings.ResourceManager, new object[]
			{
				type
			});
		}

		public static LocalizedString ProxyDLLEOF
		{
			get
			{
				return new LocalizedString("ProxyDLLEOF", "Ex7CEC81", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LdapFilterErrorSpaceMiddleType(string attributeType)
		{
			return new LocalizedString("LdapFilterErrorSpaceMiddleType", "Ex978866", false, true, Strings.ResourceManager, new object[]
			{
				attributeType
			});
		}

		public static LocalizedString FailedToFindEwsEndpoint(string mailbox)
		{
			return new LocalizedString("FailedToFindEwsEndpoint", "ExE1F5ED", false, true, Strings.ResourceManager, new object[]
			{
				mailbox
			});
		}

		public static LocalizedString ProxyDLLProtocol
		{
			get
			{
				return new LocalizedString("ProxyDLLProtocol", "Ex7D557D", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LdapFilterErrorInvalidWildCard(string type)
		{
			return new LocalizedString("LdapFilterErrorInvalidWildCard", "Ex6F59E7", false, true, Strings.ResourceManager, new object[]
			{
				type
			});
		}

		public static LocalizedString VerboseFoundAddressList(string id)
		{
			return new LocalizedString("VerboseFoundAddressList", "Ex8817D1", false, true, Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString LdapFilterErrorUnsupportedOperand(string op)
		{
			return new LocalizedString("LdapFilterErrorUnsupportedOperand", "Ex883BC4", false, true, Strings.ResourceManager, new object[]
			{
				op
			});
		}

		public static LocalizedString ErrorProxyGeneration(string message)
		{
			return new LocalizedString("ErrorProxyGeneration", "ExCE3FA1", false, true, Strings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString XmlErrorWrongAPI(string file, string api)
		{
			return new LocalizedString("XmlErrorWrongAPI", "Ex76549C", false, true, Strings.ResourceManager, new object[]
			{
				file,
				api
			});
		}

		public static LocalizedString VerboseUpdateRecipientObject(string id, string cdc, string dc, string gc)
		{
			return new LocalizedString("VerboseUpdateRecipientObject", "Ex32E49A", false, true, Strings.ResourceManager, new object[]
			{
				id,
				cdc,
				dc,
				gc
			});
		}

		public static LocalizedString LdapFilterErrorInvalidGtLtOperand
		{
			get
			{
				return new LocalizedString("LdapFilterErrorInvalidGtLtOperand", "Ex9A634A", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WarningNoDefaultMailboxPlan
		{
			get
			{
				return new LocalizedString("WarningNoDefaultMailboxPlan", "ExCD6408", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToCreateAdminAuditLogFolder(string responseclass, string code, string error)
		{
			return new LocalizedString("FailedToCreateAdminAuditLogFolder", "", false, false, Strings.ResourceManager, new object[]
			{
				responseclass,
				code,
				error
			});
		}

		public static LocalizedString AdminAuditLogFolderNotFound(string reason)
		{
			return new LocalizedString("AdminAuditLogFolderNotFound", "Ex913D13", false, true, Strings.ResourceManager, new object[]
			{
				reason
			});
		}

		public static LocalizedString XmlErrorMissingNode(string node, string file)
		{
			return new LocalizedString("XmlErrorMissingNode", "ExE024CE", false, true, Strings.ResourceManager, new object[]
			{
				node,
				file
			});
		}

		public static LocalizedString EnteredValidate
		{
			get
			{
				return new LocalizedString("EnteredValidate", "ExC593C0", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseZeroProvisioningPolicyFound
		{
			get
			{
				return new LocalizedString("VerboseZeroProvisioningPolicyFound", "ExE1585A", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoError
		{
			get
			{
				return new LocalizedString("NoError", "Ex2025DD", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExecutingScript(string script)
		{
			return new LocalizedString("ExecutingScript", "ExAAA631", false, true, Strings.ResourceManager, new object[]
			{
				script
			});
		}

		public static LocalizedString LdapFilterErrorTypeOnlySpaces(string filter)
		{
			return new LocalizedString("LdapFilterErrorTypeOnlySpaces", "Ex2B210D", false, true, Strings.ResourceManager, new object[]
			{
				filter
			});
		}

		public static LocalizedString VerboseToBeValidateObject(string id, string type)
		{
			return new LocalizedString("VerboseToBeValidateObject", "Ex2A1953", false, true, Strings.ResourceManager, new object[]
			{
				id,
				type
			});
		}

		public static LocalizedString WarningNoDefaultMailboxPlanUsingNonDefault
		{
			get
			{
				return new LocalizedString("WarningNoDefaultMailboxPlanUsingNonDefault", "Ex98C64F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProxyDLLDefault
		{
			get
			{
				return new LocalizedString("ProxyDLLDefault", "Ex572958", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorAddressListInvalidLdapFilter(string filter, string id, string message)
		{
			return new LocalizedString("ErrorAddressListInvalidLdapFilter", "Ex87D3D4", false, true, Strings.ResourceManager, new object[]
			{
				filter,
				id,
				message
			});
		}

		public static LocalizedString VerboseSettingHomeMTA(string id)
		{
			return new LocalizedString("VerboseSettingHomeMTA", "Ex4AC437", false, true, Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString ProxyDLLSoftware
		{
			get
			{
				return new LocalizedString("ProxyDLLSoftware", "ExAA9657", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LdapFilterErrorInvalidBitwiseOperand
		{
			get
			{
				return new LocalizedString("LdapFilterErrorInvalidBitwiseOperand", "Ex3C76E0", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseRemoveAddressListMemberShip(string id)
		{
			return new LocalizedString("VerboseRemoveAddressListMemberShip", "Ex10A84F", false, true, Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString VerboseMakeEmailAddressToPrimary(string address)
		{
			return new LocalizedString("VerboseMakeEmailAddressToPrimary", "ExC1589E", false, true, Strings.ResourceManager, new object[]
			{
				address
			});
		}

		public static LocalizedString LdapFilterErrorUndefinedAttributeType(string type)
		{
			return new LocalizedString("LdapFilterErrorUndefinedAttributeType", "ExB8987F", false, true, Strings.ResourceManager, new object[]
			{
				type
			});
		}

		public static LocalizedString VerboseSkipRemoteForestEmailAddressUpdate(string id, string forestName)
		{
			return new LocalizedString("VerboseSkipRemoteForestEmailAddressUpdate", "ExD038DE", false, true, Strings.ResourceManager, new object[]
			{
				id,
				forestName
			});
		}

		public static LocalizedString ErrorFailedToValidBaseAddress(string baseAddress, string msg)
		{
			return new LocalizedString("ErrorFailedToValidBaseAddress", "ExD050D0", false, true, Strings.ResourceManager, new object[]
			{
				baseAddress,
				msg
			});
		}

		public static LocalizedString FailedToEnableEwsMailboxLogger
		{
			get
			{
				return new LocalizedString("FailedToEnableEwsMailboxLogger", "ExC7850C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerboseAddPrimaryEmailAddress(string address)
		{
			return new LocalizedString("VerboseAddPrimaryEmailAddress", "Ex8C9BE8", false, true, Strings.ResourceManager, new object[]
			{
				address
			});
		}

		public static LocalizedString VerboseHandlingProvisioningPolicyFound(string id)
		{
			return new LocalizedString("VerboseHandlingProvisioningPolicyFound", "ExFBB9B9", false, true, Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString ErrorFileIsNotFound(string file)
		{
			return new LocalizedString("ErrorFileIsNotFound", "ExAFB7E9", false, true, Strings.ResourceManager, new object[]
			{
				file
			});
		}

		public static LocalizedString VerboseSettingExchangeGuid(string id)
		{
			return new LocalizedString("VerboseSettingExchangeGuid", "ExA89C29", false, true, Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString VerboseEmailAddressPolicy(string id)
		{
			return new LocalizedString("VerboseEmailAddressPolicy", "Ex907BFF", false, true, Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString VerboseAddressListsForOganizationFromDC(string org, string dc)
		{
			return new LocalizedString("VerboseAddressListsForOganizationFromDC", "ExF0FA60", false, true, Strings.ResourceManager, new object[]
			{
				org,
				dc
			});
		}

		public static LocalizedString LdapFilterErrorEscCharWithoutEscapable(string value)
		{
			return new LocalizedString("LdapFilterErrorEscCharWithoutEscapable", "ExE67B50", false, true, Strings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString ScriptingAgentInitializationFailed(string msg)
		{
			return new LocalizedString("ScriptingAgentInitializationFailed", "Ex19AD41", false, true, Strings.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString VerboseWorkingOrganizationForPolicy(string org)
		{
			return new LocalizedString("VerboseWorkingOrganizationForPolicy", "ExE5D77C", false, true, Strings.ResourceManager, new object[]
			{
				org
			});
		}

		public static LocalizedString LdapFilterErrorInvalidEscaping(string value)
		{
			return new LocalizedString("LdapFilterErrorInvalidEscaping", "Ex4F5C08", false, true, Strings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString FailedToCreateAdminAuditLogItem(string responseclass, string code, string error)
		{
			return new LocalizedString("FailedToCreateAdminAuditLogItem", "", false, false, Strings.ResourceManager, new object[]
			{
				responseclass,
				code,
				error
			});
		}

		public static LocalizedString ErrorProxyLoadDll(string module, string addressType, string message)
		{
			return new LocalizedString("ErrorProxyLoadDll", "Ex8BD879", false, true, Strings.ResourceManager, new object[]
			{
				module,
				addressType,
				message
			});
		}

		public static LocalizedString ErrorTooManyDefaultMailboxPlans
		{
			get
			{
				return new LocalizedString("ErrorTooManyDefaultMailboxPlans", "Ex0E03C9", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MultipleAdminAuditLogConfig(string organization)
		{
			return new LocalizedString("MultipleAdminAuditLogConfig", "Ex7B3DFB", false, true, Strings.ResourceManager, new object[]
			{
				organization
			});
		}

		public static LocalizedString XmlErrorMissingAttribute(string attrib, string file)
		{
			return new LocalizedString("XmlErrorMissingAttribute", "ExE31801", false, true, Strings.ResourceManager, new object[]
			{
				attrib,
				file
			});
		}

		public static LocalizedString ErrorsDuringAdminLogProvisioningHandlerValidate(string error)
		{
			return new LocalizedString("ErrorsDuringAdminLogProvisioningHandlerValidate", "Ex2796D3", false, true, Strings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString LdapFilterErrorValueOnlySpaces(string filter)
		{
			return new LocalizedString("LdapFilterErrorValueOnlySpaces", "Ex0E8168", false, true, Strings.ResourceManager, new object[]
			{
				filter
			});
		}

		public static LocalizedString ErrorInOnComplete(string inner)
		{
			return new LocalizedString("ErrorInOnComplete", "Ex0398C5", false, true, Strings.ResourceManager, new object[]
			{
				inner
			});
		}

		public static LocalizedString ErrorLoadBalancingFailedToFindDatabase
		{
			get
			{
				return new LocalizedString("ErrorLoadBalancingFailedToFindDatabase", "Ex02FD47", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LdapFilterErrorNoValidComparison(string filter)
		{
			return new LocalizedString("LdapFilterErrorNoValidComparison", "Ex056AEF", false, true, Strings.ResourceManager, new object[]
			{
				filter
			});
		}

		public static LocalizedString NotRunningAsSystem
		{
			get
			{
				return new LocalizedString("NotRunningAsSystem", "Ex350B38", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LdapFilterErrorInvalidWildCardGtLt
		{
			get
			{
				return new LocalizedString("LdapFilterErrorInvalidWildCardGtLt", "ExCDC904", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(43);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.ProvisioningAgent.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			LdapFilterErrorInvalidBooleanValue = 3378717027U,
			LdapFilterErrorNotSupportSingleComp = 1860494422U,
			InvokedProvisionDefaultProperties = 1386447651U,
			ProxyDLLError = 4236118860U,
			ExitedValidate = 1091171211U,
			LdapFilterErrorQueryTooLong = 4230107429U,
			LdapFilterErrorBracketMismatch = 1688256845U,
			ErrorUpdateAffectedIConfigurableBadRetObject = 3060439136U,
			ErrorNoMailboxPlan = 1755960558U,
			ProxyDLLDiskSpace = 449876541U,
			LdapFilterErrorInvalidToken = 56024811U,
			ProxyDLLContention = 1880382981U,
			ProxyDLLSyntax = 2164854509U,
			EnteredOnComplete = 2800661133U,
			InvokedUpdateAffectedIConfigurable = 4152138657U,
			ProxyNotImplemented = 1989031583U,
			RunningAsSystem = 3580054472U,
			ProxyDLLOOM = 3269299477U,
			ProxyDLLNotFound = 3410851087U,
			ErrorNoDefaultMailboxPlan = 2781326009U,
			ExitedInitializeConfig = 3226694139U,
			EnteredInitializeConfig = 1998083787U,
			InvokedValidate = 1775082576U,
			ErrorUnexpectedReturnTypeInValidate = 1859635364U,
			ProxyDLLConfig = 2735291928U,
			LdapFilterErrorAnrIsNotSupported = 3936814429U,
			ExitedOnComplete = 3897335713U,
			ProxyDLLEOF = 3269299668U,
			ProxyDLLProtocol = 1703828310U,
			LdapFilterErrorInvalidGtLtOperand = 4045631128U,
			WarningNoDefaultMailboxPlan = 2520898135U,
			EnteredValidate = 1236062515U,
			VerboseZeroProvisioningPolicyFound = 1767506191U,
			NoError = 1005127777U,
			WarningNoDefaultMailboxPlanUsingNonDefault = 3641189505U,
			ProxyDLLDefault = 367606263U,
			ProxyDLLSoftware = 156106839U,
			LdapFilterErrorInvalidBitwiseOperand = 135248896U,
			FailedToEnableEwsMailboxLogger = 531667494U,
			ErrorTooManyDefaultMailboxPlans = 2753705114U,
			ErrorLoadBalancingFailedToFindDatabase = 3355805493U,
			NotRunningAsSystem = 2370329957U,
			LdapFilterErrorInvalidWildCardGtLt = 1324265457U
		}

		private enum ParamIDs
		{
			ErrorProxyDllNotFound,
			VerboseAddSecondaryEmailAddress,
			LdapFilterErrorUnsupportedAttrbuteType,
			VerboseSetWindowsEmailAddress,
			VerboseAddAddressListMemberShip,
			VerboseNoDefaultForTask,
			ErrorInvalidBaseAddress,
			ScriptReturned,
			XmlErrorMissingInnerText,
			ErrorProxyGenErrorEntryPoint,
			ErrorInProvisionDefaultProperties,
			VerboseEmailAddressPoliciesForOganizationFromDC,
			VerboseProvisionDefaultProperties,
			VerboseSettingDisplayName,
			LdapFilterErrorNoAttributeType,
			ErrorInvalidLdapFilter,
			InvokedOnComplete,
			MultiTemplatePolicyFound,
			LdapFilterErrorNoAttributeValue,
			ErrorFailedToGenerateUniqueProxy,
			ErrorInValidate,
			ErrorFailedToFindAddressTypeObject,
			VerboseTaskUseTypeAsDefault,
			VerboseSettingLegacyExchangeDN,
			ErrorProvisioningPolicyCorrupt,
			VerbosePolicyProviderUseDomainController,
			ErrorFailedToReadRegistryKey,
			LdapFilterErrorInvalidDecimal,
			LdapFilterErrorObjectCategoryNotFound,
			ErrorInUpdateAffectedIConfigurable,
			VerboseNoNeedToValidate,
			LdapFilterErrorSpaceMiddleType,
			FailedToFindEwsEndpoint,
			LdapFilterErrorInvalidWildCard,
			VerboseFoundAddressList,
			LdapFilterErrorUnsupportedOperand,
			ErrorProxyGeneration,
			XmlErrorWrongAPI,
			VerboseUpdateRecipientObject,
			FailedToCreateAdminAuditLogFolder,
			AdminAuditLogFolderNotFound,
			XmlErrorMissingNode,
			ExecutingScript,
			LdapFilterErrorTypeOnlySpaces,
			VerboseToBeValidateObject,
			ErrorAddressListInvalidLdapFilter,
			VerboseSettingHomeMTA,
			VerboseRemoveAddressListMemberShip,
			VerboseMakeEmailAddressToPrimary,
			LdapFilterErrorUndefinedAttributeType,
			VerboseSkipRemoteForestEmailAddressUpdate,
			ErrorFailedToValidBaseAddress,
			VerboseAddPrimaryEmailAddress,
			VerboseHandlingProvisioningPolicyFound,
			ErrorFileIsNotFound,
			VerboseSettingExchangeGuid,
			VerboseEmailAddressPolicy,
			VerboseAddressListsForOganizationFromDC,
			LdapFilterErrorEscCharWithoutEscapable,
			ScriptingAgentInitializationFailed,
			VerboseWorkingOrganizationForPolicy,
			LdapFilterErrorInvalidEscaping,
			FailedToCreateAdminAuditLogItem,
			ErrorProxyLoadDll,
			MultipleAdminAuditLogConfig,
			XmlErrorMissingAttribute,
			ErrorsDuringAdminLogProvisioningHandlerValidate,
			LdapFilterErrorValueOnlySpaces,
			ErrorInOnComplete,
			LdapFilterErrorNoValidComparison
		}
	}
}
