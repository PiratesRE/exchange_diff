using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Security;
using System.Text;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Transport.Delivery;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensibility.EventLog;

namespace Microsoft.Exchange.Data.Transport.Internal.MExRuntime
{
	internal sealed class FactoryTable
	{
		public FactoryTable(IEnumerable agents, FactoryInitializer factoryInitializer)
		{
			this.factoriesByAgentId = new Dictionary<string, AgentFactory>();
			this.agentManagersByAgentId = new Dictionary<string, AgentManager>();
			Dictionary<string, AgentFactory> dictionary = new Dictionary<string, AgentFactory>();
			DateTime utcNow = DateTime.UtcNow;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (object obj in agents)
			{
				AgentInfo agentInfo = (AgentInfo)obj;
				if (this.factoriesByAgentId.ContainsKey(agentInfo.Id))
				{
					throw new ExchangeConfigurationException(MExRuntimeStrings.DuplicateAgentName(agentInfo.AgentName));
				}
				DateTime utcNow2 = DateTime.UtcNow;
				AgentFactory agentFactory;
				if (!dictionary.TryGetValue(agentInfo.FactoryTypeName, out agentFactory))
				{
					agentFactory = FactoryTable.CreateAgentFactory(agentInfo);
					if (factoryInitializer != null)
					{
						factoryInitializer(agentFactory);
					}
					dictionary.Add(agentInfo.FactoryTypeName, agentFactory);
				}
				this.factoriesByAgentId.Add(agentInfo.Id, agentFactory);
				AgentManager agentManagerInstance = FactoryTable.GetAgentManagerInstance(agentInfo);
				if (agentManagerInstance != null)
				{
					this.agentManagersByAgentId.Add(agentInfo.Id, agentManagerInstance);
				}
				TimeSpan timeSpan = DateTime.UtcNow - utcNow2;
				stringBuilder.AppendLine();
				stringBuilder.Append(agentInfo.AgentName);
				stringBuilder.Append(": ");
				stringBuilder.Append(timeSpan);
			}
			this.startupDiagnosticInfo = stringBuilder.ToString();
			TimeSpan timeSpan2 = DateTime.UtcNow - utcNow;
			if (timeSpan2 > FactoryTable.StartupThreshold)
			{
				MExDiagnostics.EventLog.LogEvent(EdgeExtensibilityEventLogConstants.Tuple_MExAgentFactoryStartupDelay, null, new object[]
				{
					timeSpan2,
					this.startupDiagnosticInfo
				});
			}
			this.factories = new AgentFactory[this.factoriesByAgentId.Count];
			this.factoriesByAgentId.Values.CopyTo(this.factories, 0);
		}

		public AgentFactory this[string agentId]
		{
			get
			{
				return this.factoriesByAgentId[agentId];
			}
		}

		internal string StartupDiagnosticInfo
		{
			get
			{
				return this.startupDiagnosticInfo;
			}
		}

		public static AgentManager GetAgentManagerInstance(AgentInfo agentInfo)
		{
			string text;
			Exception ex;
			return FactoryTable.LoadAssemblyAndCreateInstance<AgentManager>(agentInfo, delegate(Assembly assembly)
			{
				AgentManager agentManager = null;
				Type baseDeliveryAgentFactoryType = FactoryTable.GetBaseDeliveryAgentFactoryType(assembly.GetType(agentInfo.FactoryTypeName));
				if (baseDeliveryAgentFactoryType != null)
				{
					Type[] genericArguments = baseDeliveryAgentFactoryType.GetGenericArguments();
					if (genericArguments.Length == 1 && typeof(AgentManager).IsAssignableFrom(genericArguments[0]))
					{
						agentManager = (AgentManager)assembly.CreateInstance(genericArguments[0].FullName);
						agentManager.AgentName = agentInfo.AgentName;
					}
				}
				return agentManager;
			}, out text, out ex);
		}

		public AgentManager GetAgentManager(string agentId)
		{
			return this.agentManagersByAgentId[agentId];
		}

		public void Shutdown()
		{
			for (int i = 0; i < this.factories.Length; i++)
			{
				this.factories[i].Close();
			}
		}

		private static T LoadAssemblyAndCreateInstance<T>(AgentInfo agentInfo, FactoryTable.CreateInstance<T> createInstance, out string agentPath, out Exception exception)
		{
			agentPath = Path.Combine(Constants.MExRuntimeLocation, agentInfo.FactoryAssemblyPath);
			exception = null;
			T result = default(T);
			try
			{
				if (!File.Exists(agentPath))
				{
					exception = new ArgumentException(MExRuntimeStrings.InvalidAgentAssemblyPath);
				}
				else if (string.IsNullOrEmpty(agentInfo.FactoryTypeName))
				{
					exception = new ArgumentException(MExRuntimeStrings.InvalidAgentFactoryType);
				}
				else
				{
					Assembly assembly = Assembly.LoadFrom(agentPath);
					result = createInstance(assembly);
				}
			}
			catch (IOException ex)
			{
				exception = ex;
			}
			catch (BadImageFormatException ex2)
			{
				exception = ex2;
			}
			catch (SecurityException ex3)
			{
				exception = ex3;
			}
			catch (MissingMethodException ex4)
			{
				exception = ex4;
			}
			catch (TargetInvocationException ex5)
			{
				Exception innerException = ex5.InnerException;
				if (!FactoryTable.IsSafeToHandle(innerException))
				{
					throw;
				}
				exception = innerException;
			}
			catch (TypeInitializationException ex6)
			{
				Exception innerException2 = ex6.InnerException;
				if (!FactoryTable.IsSafeToHandle(innerException2))
				{
					throw;
				}
				exception = innerException2;
			}
			catch (InvalidCastException ex7)
			{
				exception = ex7;
			}
			return result;
		}

		private static AgentFactory CreateAgentFactory(AgentInfo agentInfo)
		{
			string assembly2;
			Exception ex;
			AgentFactory agentFactory = FactoryTable.LoadAssemblyAndCreateInstance<AgentFactory>(agentInfo, (Assembly assembly) => (AgentFactory)assembly.CreateInstance(agentInfo.FactoryTypeName), out assembly2, out ex);
			if (agentFactory == null)
			{
				ExEventLog.EventTuple tuple = EdgeExtensibilityEventLogConstants.Tuple_MExAgentFactoryCreationFailure;
				if (ex is InvalidCastException)
				{
					tuple = EdgeExtensibilityEventLogConstants.Tuple_MExAgentVersionMismatch;
				}
				ExchangeConfigurationException ex2 = new ExchangeConfigurationException(MExRuntimeStrings.InvalidTypeInConfiguration(agentInfo.FactoryTypeName, assembly2, (ex == null) ? "type not found" : ex.Message), ex);
				MExDiagnostics.EventLog.LogEvent(tuple, null, new object[]
				{
					agentInfo.AgentName,
					ex2.Message
				});
				throw ex2;
			}
			return agentFactory;
		}

		private static Type GetBaseDeliveryAgentFactoryType(Type factory)
		{
			Type type = null;
			if (factory != null)
			{
				type = factory.BaseType;
				while (type != null && (!type.IsGenericType || !(type.GetGenericTypeDefinition() == typeof(DeliveryAgentFactory<>))))
				{
					type = type.BaseType;
				}
			}
			return type;
		}

		private static bool IsSafeToHandle(Exception e)
		{
			return e is ConfigurationErrorsException || e is LocalizedException;
		}

		private static readonly TimeSpan StartupThreshold = TimeSpan.FromSeconds(15.0);

		private readonly AgentFactory[] factories;

		private readonly string startupDiagnosticInfo;

		private readonly Dictionary<string, AgentFactory> factoriesByAgentId;

		private readonly Dictionary<string, AgentManager> agentManagersByAgentId;

		private delegate T CreateInstance<T>(Assembly assembly);
	}
}
