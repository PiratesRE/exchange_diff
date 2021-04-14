using System;
using System.IO;
using System.Management.Automation;
using System.Reflection;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport.Delivery;
using Microsoft.Exchange.Data.Transport.Internal.MExRuntime;
using Microsoft.Exchange.Data.Transport.Routing;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Management.AgentTasks
{
	[Cmdlet("Install", "TransportAgent", SupportsShouldProcess = true)]
	public class InstallTransportAgent : AgentBaseTask
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return AgentStrings.ConfirmationMessageInstallTransportAgent(this.Name.ToString(), this.TransportAgentFactory.ToString(), this.AssemblyPath.ToString());
			}
		}

		private Type ReflectAgentType(string assemblyPath, string transportAgentFactory)
		{
			Assembly assembly;
			try
			{
				assembly = Assembly.LoadFrom(assemblyPath);
			}
			catch (FileNotFoundException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidArgument, null);
				return null;
			}
			catch (FileLoadException exception2)
			{
				base.WriteError(exception2, ErrorCategory.InvalidArgument, null);
				return null;
			}
			catch (BadImageFormatException exception3)
			{
				base.WriteError(exception3, ErrorCategory.InvalidArgument, null);
				return null;
			}
			catch (ArgumentException exception4)
			{
				base.WriteError(exception4, ErrorCategory.InvalidArgument, null);
				return null;
			}
			Type type = assembly.GetType(transportAgentFactory, false);
			if (null == type)
			{
				base.WriteError(new ArgumentException(AgentStrings.AgentFactoryTypeNotExist(transportAgentFactory), "TransportAgentFactory"), ErrorCategory.InvalidArgument, null);
				return null;
			}
			while (type != null)
			{
				if (type == typeof(SmtpReceiveAgentFactory))
				{
					return typeof(SmtpReceiveAgent);
				}
				if (type == typeof(RoutingAgentFactory))
				{
					return typeof(RoutingAgent);
				}
				if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(DeliveryAgentFactory<>))
				{
					return typeof(DeliveryAgent);
				}
				type = type.BaseType;
			}
			base.WriteError(new ArgumentException(AgentStrings.InvalidTransportAgentFactory(transportAgentFactory), "TransportAgentFactory"), ErrorCategory.InvalidArgument, null);
			return null;
		}

		protected override void InternalValidate()
		{
			if (this.Name.Length > 64)
			{
				base.WriteError(new ArgumentException(AgentStrings.AgentNameTooLargeArgument, "Name"), ErrorCategory.InvalidArgument, null);
			}
			if (!InstallTransportAgent.IsValidAgentName(this.Name))
			{
				base.WriteError(new ArgumentException(AgentStrings.AgentNameContainsInvalidCharacters, "Name"), ErrorCategory.InvalidArgument, null);
			}
			this.absoluteAssemblyPath = this.AssemblyPath;
			Server serverObject = base.GetServerObject();
			bool flag = serverObject != null && serverObject.IsEdgeServer;
			try
			{
				if (flag)
				{
					string currentPathProviderName = base.SessionState.CurrentPathProviderName;
					if (string.Compare(currentPathProviderName, "FileSystem", StringComparison.OrdinalIgnoreCase) == 0)
					{
						this.absoluteAssemblyPath = Path.Combine(base.SessionState.CurrentPath, this.AssemblyPath);
					}
				}
				else if (!Path.IsPathRooted(this.AssemblyPath))
				{
					base.WriteError(new ArgumentException(AgentStrings.AssemblyFilePathRelativeOnHub(this.AssemblyPath), "AssemblyPath"), ErrorCategory.InvalidArgument, null);
				}
			}
			catch (ArgumentException)
			{
				base.WriteError(new ArgumentException(AgentStrings.AssemblyFileNotExist(this.AssemblyPath), "AssemblyPath"), ErrorCategory.InvalidArgument, this.AssemblyPath);
			}
			if (new Uri(this.absoluteAssemblyPath).IsUnc)
			{
				base.WriteError(new ArgumentException(AgentStrings.AssemblyFilePathCanNotBeUNC(this.AssemblyPath), "AssemblyPath"), ErrorCategory.InvalidArgument, this.AssemblyPath);
			}
			if (!File.Exists(this.absoluteAssemblyPath))
			{
				base.WriteError(new ArgumentException(AgentStrings.AssemblyFileNotExist(this.AssemblyPath), "AssemblyPath"), ErrorCategory.InvalidArgument, null);
			}
			base.InternalValidate();
		}

		private static bool IsValidAgentName(string stringToValidate)
		{
			int length = stringToValidate.Length;
			bool result = true;
			for (int i = 0; i < length; i++)
			{
				char c = stringToValidate[i];
				if (!char.IsLetterOrDigit(c) && c != ' ' && c != '.' && c != '_' && c != '-')
				{
					result = false;
					break;
				}
			}
			return result;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			string text = base.ValidateAndNormalizeAgentIdentity(this.Name);
			if (base.AgentExists(text))
			{
				base.WriteError(new ArgumentException(AgentStrings.AgentAlreadyExist(this.Name), "Name"), ErrorCategory.InvalidArgument, null);
			}
			Type type = this.ReflectAgentType(this.absoluteAssemblyPath, this.TransportAgentFactory);
			AgentInfo agentInfo = new AgentInfo(text, type.ToString(), this.TransportAgentFactory, this.absoluteAssemblyPath, false, false);
			if (base.TransportService == TransportService.FrontEnd && type != typeof(SmtpReceiveAgent))
			{
				base.WriteError(new InvalidOperationException(AgentStrings.AgentTypeNotSupportedOnFrontEnd(type.ToString())), ErrorCategory.InvalidOperation, null);
			}
			if (type == typeof(DeliveryAgent))
			{
				this.ValidateDeliveryAgent(agentInfo);
			}
			TransportAgent sendToPipeline = new TransportAgent(text, false, base.MExConfiguration.GetPublicAgentList().Count + 1, this.TransportAgentFactory, this.absoluteAssemblyPath);
			int index = base.MExConfiguration.GetPublicAgentList().Count + base.MExConfiguration.GetPreExecutionInternalAgents().Count;
			base.MExConfiguration.AgentList.Insert(index, agentInfo);
			base.Save();
			base.WriteObject(sendToPipeline);
			if (base.MissingConfigFile)
			{
				this.WriteWarning(AgentStrings.MissingConfigurationFileCreate(base.MExConfigPath));
			}
			this.WriteWarning(AgentStrings.ReleaseAgentBinaryReference);
			this.WriteWarning(AgentStrings.RestartServiceForChanges(base.GetTransportServiceName()));
			TaskLogger.LogExit();
		}

		private void ValidateDeliveryAgent(AgentInfo agentInfo)
		{
			HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (AgentInfo agentInfo2 in base.MExConfiguration.AgentList)
			{
				DeliveryAgentManager deliveryAgentManager = FactoryTable.GetAgentManagerInstance(agentInfo2) as DeliveryAgentManager;
				if (deliveryAgentManager != null)
				{
					if (string.IsNullOrEmpty(deliveryAgentManager.SupportedDeliveryProtocol))
					{
						this.WriteWarning(AgentStrings.DeliveryProtocolNotValid(agentInfo2.AgentName));
					}
					else
					{
						hashSet.Add(deliveryAgentManager.SupportedDeliveryProtocol);
					}
				}
			}
			DeliveryAgentManager deliveryAgentManager2 = FactoryTable.GetAgentManagerInstance(agentInfo) as DeliveryAgentManager;
			if (deliveryAgentManager2 == null)
			{
				base.WriteError(new ArgumentException(AgentStrings.InvalidDeliveryAgentManager(this.Name)), ErrorCategory.InvalidArgument, null);
			}
			if (string.IsNullOrEmpty(deliveryAgentManager2.SupportedDeliveryProtocol))
			{
				base.WriteError(new ArgumentException(AgentStrings.DeliveryProtocolNotSpecified(this.Name)), ErrorCategory.InvalidArgument, null);
			}
			if (hashSet.Contains(deliveryAgentManager2.SupportedDeliveryProtocol))
			{
				base.WriteError(new ArgumentException(AgentStrings.MustHaveUniqueDeliveryProtocol(this.Name, deliveryAgentManager2.SupportedDeliveryProtocol)), ErrorCategory.InvalidArgument, null);
			}
		}

		[Parameter(Mandatory = true, Position = 0)]
		public string Name
		{
			get
			{
				return (string)base.Fields["Name"];
			}
			set
			{
				base.Fields["Name"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string TransportAgentFactory
		{
			get
			{
				return (string)base.Fields["TransportAgentFactory"];
			}
			set
			{
				base.Fields["TransportAgentFactory"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string AssemblyPath
		{
			get
			{
				return (string)base.Fields["AssemblyPath"];
			}
			set
			{
				base.Fields["AssemblyPath"] = value;
			}
		}

		private string absoluteAssemblyPath;
	}
}
