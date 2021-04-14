using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.AgentTasks
{
	internal static class AgentStrings
	{
		static AgentStrings()
		{
			AgentStrings.stringIDs.Add(3858184179U, "InvalidIdentity");
			AgentStrings.stringIDs.Add(2788510868U, "NoTransportPipelineData");
			AgentStrings.stringIDs.Add(1575515744U, "NoIdentityArgument");
			AgentStrings.stringIDs.Add(3657604156U, "AgentNameContainsInvalidCharacters");
			AgentStrings.stringIDs.Add(3268060226U, "TransportAgentTasksOnlyOnFewRoles");
			AgentStrings.stringIDs.Add(1173448710U, "AgentNameTooLargeArgument");
			AgentStrings.stringIDs.Add(2849847834U, "ReleaseAgentBinaryReference");
		}

		public static LocalizedString TransportServiceNotSupported(string service)
		{
			return new LocalizedString("TransportServiceNotSupported", "", false, false, AgentStrings.ResourceManager, new object[]
			{
				service
			});
		}

		public static LocalizedString AssemblyFilePathRelativeOnHub(string assemblyPath)
		{
			return new LocalizedString("AssemblyFilePathRelativeOnHub", "Ex447E83", false, true, AgentStrings.ResourceManager, new object[]
			{
				assemblyPath
			});
		}

		public static LocalizedString InvalidIdentity
		{
			get
			{
				return new LocalizedString("InvalidIdentity", "Ex1AA584", false, true, AgentStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoTransportPipelineData
		{
			get
			{
				return new LocalizedString("NoTransportPipelineData", "Ex5C457E", false, true, AgentStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmationMessageUninstallTransportAgent(string Identity)
		{
			return new LocalizedString("ConfirmationMessageUninstallTransportAgent", "ExE232D1", false, true, AgentStrings.ResourceManager, new object[]
			{
				Identity
			});
		}

		public static LocalizedString NoIdentityArgument
		{
			get
			{
				return new LocalizedString("NoIdentityArgument", "Ex1FA692", false, true, AgentStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmationMessageDisableTransportAgent(string Identity)
		{
			return new LocalizedString("ConfirmationMessageDisableTransportAgent", "Ex3574A2", false, true, AgentStrings.ResourceManager, new object[]
			{
				Identity
			});
		}

		public static LocalizedString ConfirmationMessageSetTransportAgent(string Identity)
		{
			return new LocalizedString("ConfirmationMessageSetTransportAgent", "ExC46ADA", false, true, AgentStrings.ResourceManager, new object[]
			{
				Identity
			});
		}

		public static LocalizedString DeliveryProtocolNotValid(string identity)
		{
			return new LocalizedString("DeliveryProtocolNotValid", "ExE646F6", false, true, AgentStrings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString InvalidTransportAgentFactory(string transportAgentFactory)
		{
			return new LocalizedString("InvalidTransportAgentFactory", "ExCF76E7", false, true, AgentStrings.ResourceManager, new object[]
			{
				transportAgentFactory
			});
		}

		public static LocalizedString AssemblyFileNotExist(string assemblyPath)
		{
			return new LocalizedString("AssemblyFileNotExist", "ExE78D21", false, true, AgentStrings.ResourceManager, new object[]
			{
				assemblyPath
			});
		}

		public static LocalizedString RestartServiceForChanges(string serviceName)
		{
			return new LocalizedString("RestartServiceForChanges", "ExF30C00", false, true, AgentStrings.ResourceManager, new object[]
			{
				serviceName
			});
		}

		public static LocalizedString AgentNameContainsInvalidCharacters
		{
			get
			{
				return new LocalizedString("AgentNameContainsInvalidCharacters", "Ex7A5D34", false, true, AgentStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PriorityOutOfRange(string maxPriority)
		{
			return new LocalizedString("PriorityOutOfRange", "ExFEB2E3", false, true, AgentStrings.ResourceManager, new object[]
			{
				maxPriority
			});
		}

		public static LocalizedString AgentAlreadyExist(string identity)
		{
			return new LocalizedString("AgentAlreadyExist", "Ex4D98F9", false, true, AgentStrings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString InvalidDeliveryAgentManager(string identity)
		{
			return new LocalizedString("InvalidDeliveryAgentManager", "ExC97BC5", false, true, AgentStrings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString AgentNotFound(string identity)
		{
			return new LocalizedString("AgentNotFound", "Ex23DBF5", false, true, AgentStrings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString MissingConfigurationFileCreate(string filePath)
		{
			return new LocalizedString("MissingConfigurationFileCreate", "Ex5C98CA", false, true, AgentStrings.ResourceManager, new object[]
			{
				filePath
			});
		}

		public static LocalizedString MustHaveUniqueDeliveryProtocol(string identity, string protocol)
		{
			return new LocalizedString("MustHaveUniqueDeliveryProtocol", "Ex2E8D12", false, true, AgentStrings.ResourceManager, new object[]
			{
				identity,
				protocol
			});
		}

		public static LocalizedString AgentFactoryTypeNotExist(string transportAgentFactory)
		{
			return new LocalizedString("AgentFactoryTypeNotExist", "Ex9BCFF1", false, true, AgentStrings.ResourceManager, new object[]
			{
				transportAgentFactory
			});
		}

		public static LocalizedString ConfirmationMessageEnableTransportAgent(string Identity)
		{
			return new LocalizedString("ConfirmationMessageEnableTransportAgent", "ExAD0323", false, true, AgentStrings.ResourceManager, new object[]
			{
				Identity
			});
		}

		public static LocalizedString TransportAgentTasksOnlyOnFewRoles
		{
			get
			{
				return new LocalizedString("TransportAgentTasksOnlyOnFewRoles", "", false, false, AgentStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AgentNameTooLargeArgument
		{
			get
			{
				return new LocalizedString("AgentNameTooLargeArgument", "ExA21F3D", false, true, AgentStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmationMessageInstallTransportAgent(string Name, string TransportAgentFactory, string AssemblyPath)
		{
			return new LocalizedString("ConfirmationMessageInstallTransportAgent", "Ex120410", false, true, AgentStrings.ResourceManager, new object[]
			{
				Name,
				TransportAgentFactory,
				AssemblyPath
			});
		}

		public static LocalizedString DeliveryProtocolNotSpecified(string identity)
		{
			return new LocalizedString("DeliveryProtocolNotSpecified", "Ex0138E2", false, true, AgentStrings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString AgentTypeNotSupportedOnFrontEnd(string agentType)
		{
			return new LocalizedString("AgentTypeNotSupportedOnFrontEnd", "", false, false, AgentStrings.ResourceManager, new object[]
			{
				agentType
			});
		}

		public static LocalizedString AssemblyFilePathCanNotBeUNC(string assemblyPath)
		{
			return new LocalizedString("AssemblyFilePathCanNotBeUNC", "Ex10CB51", false, true, AgentStrings.ResourceManager, new object[]
			{
				assemblyPath
			});
		}

		public static LocalizedString ReleaseAgentBinaryReference
		{
			get
			{
				return new LocalizedString("ReleaseAgentBinaryReference", "Ex7C8B43", false, true, AgentStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(AgentStrings.IDs key)
		{
			return new LocalizedString(AgentStrings.stringIDs[(uint)key], AgentStrings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(7);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Management.AgentStrings", typeof(AgentStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			InvalidIdentity = 3858184179U,
			NoTransportPipelineData = 2788510868U,
			NoIdentityArgument = 1575515744U,
			AgentNameContainsInvalidCharacters = 3657604156U,
			TransportAgentTasksOnlyOnFewRoles = 3268060226U,
			AgentNameTooLargeArgument = 1173448710U,
			ReleaseAgentBinaryReference = 2849847834U
		}

		private enum ParamIDs
		{
			TransportServiceNotSupported,
			AssemblyFilePathRelativeOnHub,
			ConfirmationMessageUninstallTransportAgent,
			ConfirmationMessageDisableTransportAgent,
			ConfirmationMessageSetTransportAgent,
			DeliveryProtocolNotValid,
			InvalidTransportAgentFactory,
			AssemblyFileNotExist,
			RestartServiceForChanges,
			PriorityOutOfRange,
			AgentAlreadyExist,
			InvalidDeliveryAgentManager,
			AgentNotFound,
			MissingConfigurationFileCreate,
			MustHaveUniqueDeliveryProtocol,
			AgentFactoryTypeNotExist,
			ConfirmationMessageEnableTransportAgent,
			ConfirmationMessageInstallTransportAgent,
			DeliveryProtocolNotSpecified,
			AgentTypeNotSupportedOnFrontEnd,
			AssemblyFilePathCanNotBeUNC
		}
	}
}
