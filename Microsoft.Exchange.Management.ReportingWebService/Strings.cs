using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.ReportingWebService
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(957461802U, "InvalidFormatQuery");
			Strings.stringIDs.Add(819773363U, "ErrorInvalidVersion");
			Strings.stringIDs.Add(3351215994U, "UnknownError");
			Strings.stringIDs.Add(3512077190U, "ADOperationError");
			Strings.stringIDs.Add(3594659945U, "ADTransientError");
			Strings.stringIDs.Add(136560791U, "ErrorOverBudget");
			Strings.stringIDs.Add(2772872006U, "DataMartTimeoutException");
			Strings.stringIDs.Add(4183935500U, "InvalidQueryException");
			Strings.stringIDs.Add(238685540U, "ErrorMissingTenantDomainInRequest");
			Strings.stringIDs.Add(3447632727U, "ErrorSchemaInitializationFail");
			Strings.stringIDs.Add(267521403U, "UserUnauthenticated");
			Strings.stringIDs.Add(3837723474U, "CreateRunspaceConfigTimeoutError");
			Strings.stringIDs.Add(2878617116U, "ErrorVersionAmbiguous");
			Strings.stringIDs.Add(3760355046U, "UserNotSet");
			Strings.stringIDs.Add(3646759096U, "ConnectionFailedException");
		}

		public static LocalizedString InvalidFormatQuery
		{
			get
			{
				return new LocalizedString("InvalidFormatQuery", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorTenantNotInOrgScope(string tenant)
		{
			return new LocalizedString("ErrorTenantNotInOrgScope", Strings.ResourceManager, new object[]
			{
				tenant
			});
		}

		public static LocalizedString ErrorInvalidVersion
		{
			get
			{
				return new LocalizedString("ErrorInvalidVersion", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnknownError
		{
			get
			{
				return new LocalizedString("UnknownError", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADOperationError
		{
			get
			{
				return new LocalizedString("ADOperationError", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADTransientError
		{
			get
			{
				return new LocalizedString("ADTransientError", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorOverBudget
		{
			get
			{
				return new LocalizedString("ErrorOverBudget", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSchemaConfigurationFileMissing(string path)
		{
			return new LocalizedString("ErrorSchemaConfigurationFileMissing", Strings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString ErrorTenantNotResolved(string tenant)
		{
			return new LocalizedString("ErrorTenantNotResolved", Strings.ResourceManager, new object[]
			{
				tenant
			});
		}

		public static LocalizedString DataMartTimeoutException
		{
			get
			{
				return new LocalizedString("DataMartTimeoutException", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidQueryException
		{
			get
			{
				return new LocalizedString("InvalidQueryException", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorOAuthAuthorizationNoAccount(string oauthIdentity)
		{
			return new LocalizedString("ErrorOAuthAuthorizationNoAccount", Strings.ResourceManager, new object[]
			{
				oauthIdentity
			});
		}

		public static LocalizedString ErrorMissingTenantDomainInRequest
		{
			get
			{
				return new LocalizedString("ErrorMissingTenantDomainInRequest", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSchemaInitializationFail
		{
			get
			{
				return new LocalizedString("ErrorSchemaInitializationFail", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UserUnauthenticated
		{
			get
			{
				return new LocalizedString("UserUnauthenticated", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CreateRunspaceConfigTimeoutError
		{
			get
			{
				return new LocalizedString("CreateRunspaceConfigTimeoutError", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorVersionAmbiguous
		{
			get
			{
				return new LocalizedString("ErrorVersionAmbiguous", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UserNotSet
		{
			get
			{
				return new LocalizedString("UserNotSet", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConnectionFailedException
		{
			get
			{
				return new LocalizedString("ConnectionFailedException", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(15);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Management.ReportingWebService.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			InvalidFormatQuery = 957461802U,
			ErrorInvalidVersion = 819773363U,
			UnknownError = 3351215994U,
			ADOperationError = 3512077190U,
			ADTransientError = 3594659945U,
			ErrorOverBudget = 136560791U,
			DataMartTimeoutException = 2772872006U,
			InvalidQueryException = 4183935500U,
			ErrorMissingTenantDomainInRequest = 238685540U,
			ErrorSchemaInitializationFail = 3447632727U,
			UserUnauthenticated = 267521403U,
			CreateRunspaceConfigTimeoutError = 3837723474U,
			ErrorVersionAmbiguous = 2878617116U,
			UserNotSet = 3760355046U,
			ConnectionFailedException = 3646759096U
		}

		private enum ParamIDs
		{
			ErrorTenantNotInOrgScope,
			ErrorSchemaConfigurationFileMissing,
			ErrorTenantNotResolved,
			ErrorOAuthAuthorizationNoAccount
		}
	}
}
