using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct ADProviderTags
	{
		public const int TopologyProvider = 0;

		public const int ADTopology = 1;

		public const int Connection = 2;

		public const int ConnectionDetails = 3;

		public const int GetConnection = 4;

		public const int ADFind = 5;

		public const int ADRead = 6;

		public const int ADReadDetails = 7;

		public const int ADSave = 8;

		public const int ADSaveDetails = 9;

		public const int ADDelete = 10;

		public const int Validation = 11;

		public const int ADNotifications = 12;

		public const int DirectoryException = 13;

		public const int LdapFilterBuilder = 14;

		public const int ADPropertyRequest = 15;

		public const int ADObject = 17;

		public const int ContentTypeMapping = 19;

		public const int LcidMapper = 20;

		public const int RecipientUpdateService = 21;

		public const int UMAutoAttendant = 22;

		public const int ExchangeTopology = 23;

		public const int PerfCounters = 24;

		public const int ClientThrottling = 25;

		public const int ServerSettingsProvider = 27;

		public const int RetryManager = 29;

		public const int SystemConfigurationCache = 30;

		public const int FederatedIdentity = 31;

		public const int FaultInjection = 32;

		public const int AddressList = 33;

		public const int NspiRpcClientConnection = 34;

		public const int ScopeVerification = 35;

		public const int SchemaInitialization = 36;

		public const int IsMemberOfResolver = 37;

		public const int OwaSegmentation = 39;

		public const int ADPerformance = 40;

		public const int ResourceHealthManager = 41;

		public const int BudgetDelay = 42;

		public const int GLS = 43;

		public const int MServ = 44;

		public const int TenantRelocation = 45;

		public const int StateManagement = 46;

		public const int ServerComponentStateManager = 48;

		public const int SessionSettings = 49;

		public const int ADConfigLoader = 50;

		public const int SlimTenant = 51;

		public const int TenantUpgradeServicelet = 52;

		public const int DirectoryTasks = 53;

		public const int Compliance = 54;

		public static Guid guid = new Guid("0c6a4049-bb65-4ea6-9f0c-12808260c2f1");
	}
}
