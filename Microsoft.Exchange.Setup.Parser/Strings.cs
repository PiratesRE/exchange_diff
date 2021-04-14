using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Setup.Parser
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(1914581610U, "ParameterShouldStartWith");
			Strings.stringIDs.Add(1564022379U, "CurrentOperationNotSet");
			Strings.stringIDs.Add(2509045854U, "InstallRequiresRoles");
			Strings.stringIDs.Add(3247175721U, "MBXRoleIsInstalled");
			Strings.stringIDs.Add(2607811679U, "CASRoleIsInstalled");
		}

		public static LocalizedString NotAValidFqdn(string fqdn)
		{
			return new LocalizedString("NotAValidFqdn", Strings.ResourceManager, new object[]
			{
				fqdn
			});
		}

		public static LocalizedString UnknownParameter(string parameter)
		{
			return new LocalizedString("UnknownParameter", Strings.ResourceManager, new object[]
			{
				parameter
			});
		}

		public static LocalizedString ParameterShouldStartWith
		{
			get
			{
				return new LocalizedString("ParameterShouldStartWith", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AnswerFileCouldNotBeOpened(string answerfile)
		{
			return new LocalizedString("AnswerFileCouldNotBeOpened", Strings.ResourceManager, new object[]
			{
				answerfile
			});
		}

		public static LocalizedString NotInTheRange(string s)
		{
			return new LocalizedString("NotInTheRange", Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString InvalidRole(string role)
		{
			return new LocalizedString("InvalidRole", Strings.ResourceManager, new object[]
			{
				role
			});
		}

		public static LocalizedString InvalidEdbFilePath(string path)
		{
			return new LocalizedString("InvalidEdbFilePath", Strings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString CurrentOperationNotSet
		{
			get
			{
				return new LocalizedString("CurrentOperationNotSet", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DirectoryNotExist(string dir)
		{
			return new LocalizedString("DirectoryNotExist", Strings.ResourceManager, new object[]
			{
				dir
			});
		}

		public static LocalizedString AnswerFileSetupParameterEntry(string name, object value)
		{
			return new LocalizedString("AnswerFileSetupParameterEntry", Strings.ResourceManager, new object[]
			{
				name,
				value
			});
		}

		public static LocalizedString NotValidIndustryType(string industry, string validValue)
		{
			return new LocalizedString("NotValidIndustryType", Strings.ResourceManager, new object[]
			{
				industry,
				validValue
			});
		}

		public static LocalizedString ParameterNotValidForCurrentRoles(string parameter)
		{
			return new LocalizedString("ParameterNotValidForCurrentRoles", Strings.ResourceManager, new object[]
			{
				parameter
			});
		}

		public static LocalizedString ParameterMustHaveValue(string parameter)
		{
			return new LocalizedString("ParameterMustHaveValue", Strings.ResourceManager, new object[]
			{
				parameter
			});
		}

		public static LocalizedString AnswerFileNameNotValid(string name)
		{
			return new LocalizedString("AnswerFileNameNotValid", Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString InvalidUIMode(string mode)
		{
			return new LocalizedString("InvalidUIMode", Strings.ResourceManager, new object[]
			{
				mode
			});
		}

		public static LocalizedString ParameterNotValidForCurrentOperation(string parameter, string operation)
		{
			return new LocalizedString("ParameterNotValidForCurrentOperation", Strings.ResourceManager, new object[]
			{
				parameter,
				operation
			});
		}

		public static LocalizedString InvalidMode(string mode)
		{
			return new LocalizedString("InvalidMode", Strings.ResourceManager, new object[]
			{
				mode
			});
		}

		public static LocalizedString NotAValidNumber(string s)
		{
			return new LocalizedString("NotAValidNumber", Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString InvalidIPv4Address(string ipaddress)
		{
			return new LocalizedString("InvalidIPv4Address", Strings.ResourceManager, new object[]
			{
				ipaddress
			});
		}

		public static LocalizedString InstallRequiresRoles
		{
			get
			{
				return new LocalizedString("InstallRequiresRoles", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ParameterSpecifiedMultipleTimes(string parameter)
		{
			return new LocalizedString("ParameterSpecifiedMultipleTimes", Strings.ResourceManager, new object[]
			{
				parameter
			});
		}

		public static LocalizedString InvalidServerIdParameter(string server)
		{
			return new LocalizedString("InvalidServerIdParameter", Strings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString NotAValidNonRootLocalLongFullPath(string path)
		{
			return new LocalizedString("NotAValidNonRootLocalLongFullPath", Strings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString FileNotExist(string file)
		{
			return new LocalizedString("FileNotExist", Strings.ResourceManager, new object[]
			{
				file
			});
		}

		public static LocalizedString NotAValidCultureString(string culture)
		{
			return new LocalizedString("NotAValidCultureString", Strings.ResourceManager, new object[]
			{
				culture
			});
		}

		public static LocalizedString MBXRoleIsInstalled
		{
			get
			{
				return new LocalizedString("MBXRoleIsInstalled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CASRoleIsInstalled
		{
			get
			{
				return new LocalizedString("CASRoleIsInstalled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleSpecifiedMultipleTimes(string role)
		{
			return new LocalizedString("RoleSpecifiedMultipleTimes", Strings.ResourceManager, new object[]
			{
				role
			});
		}

		public static LocalizedString EmptyValueSpecified(string parameter)
		{
			return new LocalizedString("EmptyValueSpecified", Strings.ResourceManager, new object[]
			{
				parameter
			});
		}

		public static LocalizedString CommandLineSetupParameterEntry(string name, object value)
		{
			return new LocalizedString("CommandLineSetupParameterEntry", Strings.ResourceManager, new object[]
			{
				name,
				value
			});
		}

		public static LocalizedString NotBooleanValue(string value)
		{
			return new LocalizedString("NotBooleanValue", Strings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString ParameterCannotHaveValue(string parameter)
		{
			return new LocalizedString("ParameterCannotHaveValue", Strings.ResourceManager, new object[]
			{
				parameter
			});
		}

		public static LocalizedString PrepareFlagConstraint(string name)
		{
			return new LocalizedString("PrepareFlagConstraint", Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString InvalidCommonName(string name)
		{
			return new LocalizedString("InvalidCommonName", Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(5);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Setup.Parser.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			ParameterShouldStartWith = 1914581610U,
			CurrentOperationNotSet = 1564022379U,
			InstallRequiresRoles = 2509045854U,
			MBXRoleIsInstalled = 3247175721U,
			CASRoleIsInstalled = 2607811679U
		}

		private enum ParamIDs
		{
			NotAValidFqdn,
			UnknownParameter,
			AnswerFileCouldNotBeOpened,
			NotInTheRange,
			InvalidRole,
			InvalidEdbFilePath,
			DirectoryNotExist,
			AnswerFileSetupParameterEntry,
			NotValidIndustryType,
			ParameterNotValidForCurrentRoles,
			ParameterMustHaveValue,
			AnswerFileNameNotValid,
			InvalidUIMode,
			ParameterNotValidForCurrentOperation,
			InvalidMode,
			NotAValidNumber,
			InvalidIPv4Address,
			ParameterSpecifiedMultipleTimes,
			InvalidServerIdParameter,
			NotAValidNonRootLocalLongFullPath,
			FileNotExist,
			NotAValidCultureString,
			RoleSpecifiedMultipleTimes,
			EmptyValueSpecified,
			CommandLineSetupParameterEntry,
			NotBooleanValue,
			ParameterCannotHaveValue,
			PrepareFlagConstraint,
			InvalidCommonName
		}
	}
}
