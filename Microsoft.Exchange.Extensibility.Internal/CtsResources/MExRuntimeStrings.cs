using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;

namespace Microsoft.Exchange.CtsResources
{
	internal static class MExRuntimeStrings
	{
		static MExRuntimeStrings()
		{
			MExRuntimeStrings.stringIDs.Add(3834762168U, "InvalidConcurrentInvoke");
			MExRuntimeStrings.stringIDs.Add(496466864U, "FailedToReadDataCenterMode");
			MExRuntimeStrings.stringIDs.Add(137784932U, "InvalidAgentFactoryType");
			MExRuntimeStrings.stringIDs.Add(1310430585U, "InvalidConfiguration");
			MExRuntimeStrings.stringIDs.Add(3602164712U, "TooManyInvokes");
			MExRuntimeStrings.stringIDs.Add(716036182U, "InvalidState");
			MExRuntimeStrings.stringIDs.Add(1872988198U, "InvalidEndInvoke");
			MExRuntimeStrings.stringIDs.Add(3581140767U, "InvalidAgentAssemblyPath");
		}

		public static string InvalidConcurrentInvoke
		{
			get
			{
				return MExRuntimeStrings.ResourceManager.GetString("InvalidConcurrentInvoke");
			}
		}

		public static string DuplicateAgentName(string agentName)
		{
			return string.Format(MExRuntimeStrings.ResourceManager.GetString("DuplicateAgentName"), agentName);
		}

		public static string FailedToReadDataCenterMode
		{
			get
			{
				return MExRuntimeStrings.ResourceManager.GetString("FailedToReadDataCenterMode");
			}
		}

		public static string InvalidAgentFactoryType
		{
			get
			{
				return MExRuntimeStrings.ResourceManager.GetString("InvalidAgentFactoryType");
			}
		}

		public static string AgentFault(string agent, string topic)
		{
			return string.Format(MExRuntimeStrings.ResourceManager.GetString("AgentFault"), agent, topic);
		}

		public static string InvalidConfiguration
		{
			get
			{
				return MExRuntimeStrings.ResourceManager.GetString("InvalidConfiguration");
			}
		}

		public static string TooManyInvokes
		{
			get
			{
				return MExRuntimeStrings.ResourceManager.GetString("TooManyInvokes");
			}
		}

		public static string InvalidConfigurationFile(string filePath)
		{
			return string.Format(MExRuntimeStrings.ResourceManager.GetString("InvalidConfigurationFile"), filePath);
		}

		public static string InvalidState
		{
			get
			{
				return MExRuntimeStrings.ResourceManager.GetString("InvalidState");
			}
		}

		public static string InvalidTypeInConfiguration(string type, string assembly, string error)
		{
			return string.Format(MExRuntimeStrings.ResourceManager.GetString("InvalidTypeInConfiguration"), type, assembly, error);
		}

		public static string MissingConfigurationFile(string filePath)
		{
			return string.Format(MExRuntimeStrings.ResourceManager.GetString("MissingConfigurationFile"), filePath);
		}

		public static string InvalidEndInvoke
		{
			get
			{
				return MExRuntimeStrings.ResourceManager.GetString("InvalidEndInvoke");
			}
		}

		public static string AgentCreationFailure(string agent, string error)
		{
			return string.Format(MExRuntimeStrings.ResourceManager.GetString("AgentCreationFailure"), agent, error);
		}

		public static string InvalidAgentAssemblyPath
		{
			get
			{
				return MExRuntimeStrings.ResourceManager.GetString("InvalidAgentAssemblyPath");
			}
		}

		public static string GetLocalizedString(MExRuntimeStrings.IDs key)
		{
			return MExRuntimeStrings.ResourceManager.GetString(MExRuntimeStrings.stringIDs[(uint)key]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(8);

		private static ResourceManager ResourceManager = new ResourceManager("Microsoft.Exchange.CtsResources.MExRuntimeStrings", typeof(MExRuntimeStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			InvalidConcurrentInvoke = 3834762168U,
			FailedToReadDataCenterMode = 496466864U,
			InvalidAgentFactoryType = 137784932U,
			InvalidConfiguration = 1310430585U,
			TooManyInvokes = 3602164712U,
			InvalidState = 716036182U,
			InvalidEndInvoke = 1872988198U,
			InvalidAgentAssemblyPath = 3581140767U
		}

		private enum ParamIDs
		{
			DuplicateAgentName,
			AgentFault,
			InvalidConfigurationFile,
			InvalidTypeInConfiguration,
			MissingConfigurationFile,
			AgentCreationFailure
		}
	}
}
