using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Lifetime;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Metadata;
using System.Security;
using System.Security.Permissions;
using System.Security.Util;

namespace System.Runtime.Remoting
{
	internal static class RemotingConfigHandler
	{
		internal static string ApplicationName
		{
			get
			{
				if (RemotingConfigHandler._applicationName == null)
				{
					throw new RemotingException(Environment.GetResourceString("Remoting_Config_NoAppName"));
				}
				return RemotingConfigHandler._applicationName;
			}
			set
			{
				if (RemotingConfigHandler._applicationName != null)
				{
					throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_Config_AppNameSet"), RemotingConfigHandler._applicationName));
				}
				RemotingConfigHandler._applicationName = value;
				char[] trimChars = new char[]
				{
					'/'
				};
				if (RemotingConfigHandler._applicationName.StartsWith("/", StringComparison.Ordinal))
				{
					RemotingConfigHandler._applicationName = RemotingConfigHandler._applicationName.TrimStart(trimChars);
				}
				if (RemotingConfigHandler._applicationName.EndsWith("/", StringComparison.Ordinal))
				{
					RemotingConfigHandler._applicationName = RemotingConfigHandler._applicationName.TrimEnd(trimChars);
				}
			}
		}

		internal static bool HasApplicationNameBeenSet()
		{
			return RemotingConfigHandler._applicationName != null;
		}

		internal static bool UrlObjRefMode
		{
			get
			{
				return RemotingConfigHandler._bUrlObjRefMode;
			}
		}

		internal static CustomErrorsModes CustomErrorsMode
		{
			get
			{
				return RemotingConfigHandler._errorMode;
			}
			set
			{
				if (RemotingConfigHandler._errorsModeSet)
				{
					throw new RemotingException(Environment.GetResourceString("Remoting_Config_ErrorsModeSet"));
				}
				RemotingConfigHandler._errorMode = value;
				RemotingConfigHandler._errorsModeSet = true;
			}
		}

		[SecurityCritical]
		internal static IMessageSink FindDelayLoadChannelForCreateMessageSink(string url, object data, out string objectURI)
		{
			RemotingConfigHandler.LoadMachineConfigIfNecessary();
			objectURI = null;
			foreach (object obj in RemotingConfigHandler._delayLoadChannelConfigQueue)
			{
				DelayLoadClientChannelEntry delayLoadClientChannelEntry = (DelayLoadClientChannelEntry)obj;
				IChannelSender channel = delayLoadClientChannelEntry.Channel;
				if (channel != null)
				{
					IMessageSink messageSink = channel.CreateMessageSink(url, data, out objectURI);
					if (messageSink != null)
					{
						delayLoadClientChannelEntry.RegisterChannel();
						return messageSink;
					}
				}
			}
			return null;
		}

		[SecurityCritical]
		private static void LoadMachineConfigIfNecessary()
		{
			if (!RemotingConfigHandler._bMachineConfigLoaded)
			{
				RemotingConfigHandler.RemotingConfigInfo info = RemotingConfigHandler.Info;
				lock (info)
				{
					if (!RemotingConfigHandler._bMachineConfigLoaded)
					{
						RemotingXmlConfigFileData remotingXmlConfigFileData = RemotingXmlConfigFileParser.ParseDefaultConfiguration();
						if (remotingXmlConfigFileData != null)
						{
							RemotingConfigHandler.ConfigureRemoting(remotingXmlConfigFileData, false);
						}
						string machineDirectory = Config.MachineDirectory;
						string text = machineDirectory + "machine.config";
						new FileIOPermission(FileIOPermissionAccess.Read, text).Assert();
						remotingXmlConfigFileData = RemotingConfigHandler.LoadConfigurationFromXmlFile(text);
						if (remotingXmlConfigFileData != null)
						{
							RemotingConfigHandler.ConfigureRemoting(remotingXmlConfigFileData, false);
						}
						RemotingConfigHandler._bMachineConfigLoaded = true;
					}
				}
			}
		}

		[SecurityCritical]
		internal static void DoConfiguration(string filename, bool ensureSecurity)
		{
			RemotingConfigHandler.LoadMachineConfigIfNecessary();
			RemotingXmlConfigFileData remotingXmlConfigFileData = RemotingConfigHandler.LoadConfigurationFromXmlFile(filename);
			if (remotingXmlConfigFileData != null)
			{
				RemotingConfigHandler.ConfigureRemoting(remotingXmlConfigFileData, ensureSecurity);
			}
		}

		private static RemotingXmlConfigFileData LoadConfigurationFromXmlFile(string filename)
		{
			RemotingXmlConfigFileData result;
			try
			{
				if (filename != null)
				{
					result = RemotingXmlConfigFileParser.ParseConfigFile(filename);
				}
				else
				{
					result = null;
				}
			}
			catch (Exception ex)
			{
				Exception ex2 = ex.InnerException as FileNotFoundException;
				if (ex2 != null)
				{
					ex = ex2;
				}
				throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_Config_ReadFailure"), filename, ex));
			}
			return result;
		}

		[SecurityCritical]
		private static void ConfigureRemoting(RemotingXmlConfigFileData configData, bool ensureSecurity)
		{
			try
			{
				string applicationName = configData.ApplicationName;
				if (applicationName != null)
				{
					RemotingConfigHandler.ApplicationName = applicationName;
				}
				if (configData.CustomErrors != null)
				{
					RemotingConfigHandler._errorMode = configData.CustomErrors.Mode;
				}
				RemotingConfigHandler.ConfigureChannels(configData, ensureSecurity);
				if (configData.Lifetime != null)
				{
					if (configData.Lifetime.IsLeaseTimeSet)
					{
						LifetimeServices.LeaseTime = configData.Lifetime.LeaseTime;
					}
					if (configData.Lifetime.IsRenewOnCallTimeSet)
					{
						LifetimeServices.RenewOnCallTime = configData.Lifetime.RenewOnCallTime;
					}
					if (configData.Lifetime.IsSponsorshipTimeoutSet)
					{
						LifetimeServices.SponsorshipTimeout = configData.Lifetime.SponsorshipTimeout;
					}
					if (configData.Lifetime.IsLeaseManagerPollTimeSet)
					{
						LifetimeServices.LeaseManagerPollTime = configData.Lifetime.LeaseManagerPollTime;
					}
				}
				RemotingConfigHandler._bUrlObjRefMode = configData.UrlObjRefMode;
				RemotingConfigHandler.Info.StoreRemoteAppEntries(configData);
				RemotingConfigHandler.Info.StoreActivatedExports(configData);
				RemotingConfigHandler.Info.StoreInteropEntries(configData);
				RemotingConfigHandler.Info.StoreWellKnownExports(configData);
				if (configData.ServerActivatedEntries.Count > 0)
				{
					ActivationServices.StartListeningForRemoteRequests();
				}
			}
			catch (Exception arg)
			{
				throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_Config_ConfigurationFailure"), arg));
			}
		}

		[SecurityCritical]
		private static void ConfigureChannels(RemotingXmlConfigFileData configData, bool ensureSecurity)
		{
			RemotingServices.RegisterWellKnownChannels();
			foreach (object obj in configData.ChannelEntries)
			{
				RemotingXmlConfigFileData.ChannelEntry channelEntry = (RemotingXmlConfigFileData.ChannelEntry)obj;
				if (!channelEntry.DelayLoad)
				{
					IChannel chnl = RemotingConfigHandler.CreateChannelFromConfigEntry(channelEntry);
					ChannelServices.RegisterChannel(chnl, ensureSecurity);
				}
				else
				{
					RemotingConfigHandler._delayLoadChannelConfigQueue.Enqueue(new DelayLoadClientChannelEntry(channelEntry, ensureSecurity));
				}
			}
		}

		[SecurityCritical]
		internal static IChannel CreateChannelFromConfigEntry(RemotingXmlConfigFileData.ChannelEntry entry)
		{
			Type type = RemotingConfigHandler.RemotingConfigInfo.LoadType(entry.TypeName, entry.AssemblyName);
			bool flag = typeof(IChannelReceiver).IsAssignableFrom(type);
			bool flag2 = typeof(IChannelSender).IsAssignableFrom(type);
			IClientChannelSinkProvider clientChannelSinkProvider = null;
			IServerChannelSinkProvider serverChannelSinkProvider = null;
			if (entry.ClientSinkProviders.Count > 0)
			{
				clientChannelSinkProvider = RemotingConfigHandler.CreateClientChannelSinkProviderChain(entry.ClientSinkProviders);
			}
			if (entry.ServerSinkProviders.Count > 0)
			{
				serverChannelSinkProvider = RemotingConfigHandler.CreateServerChannelSinkProviderChain(entry.ServerSinkProviders);
			}
			object[] args;
			if (flag && flag2)
			{
				args = new object[]
				{
					entry.Properties,
					clientChannelSinkProvider,
					serverChannelSinkProvider
				};
			}
			else if (flag)
			{
				args = new object[]
				{
					entry.Properties,
					serverChannelSinkProvider
				};
			}
			else
			{
				if (!flag2)
				{
					throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_Config_InvalidChannelType"), type.FullName));
				}
				args = new object[]
				{
					entry.Properties,
					clientChannelSinkProvider
				};
			}
			IChannel result = null;
			try
			{
				result = (IChannel)Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance, null, args, null, null);
			}
			catch (MissingMethodException)
			{
				string arg = null;
				if (flag && flag2)
				{
					arg = "MyChannel(IDictionary properties, IClientChannelSinkProvider clientSinkProvider, IServerChannelSinkProvider serverSinkProvider)";
				}
				else if (flag)
				{
					arg = "MyChannel(IDictionary properties, IServerChannelSinkProvider serverSinkProvider)";
				}
				else if (flag2)
				{
					arg = "MyChannel(IDictionary properties, IClientChannelSinkProvider clientSinkProvider)";
				}
				throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_Config_ChannelMissingCtor"), type.FullName, arg));
			}
			return result;
		}

		[SecurityCritical]
		private static IClientChannelSinkProvider CreateClientChannelSinkProviderChain(ArrayList entries)
		{
			IClientChannelSinkProvider clientChannelSinkProvider = null;
			IClientChannelSinkProvider clientChannelSinkProvider2 = null;
			foreach (object obj in entries)
			{
				RemotingXmlConfigFileData.SinkProviderEntry entry = (RemotingXmlConfigFileData.SinkProviderEntry)obj;
				if (clientChannelSinkProvider == null)
				{
					clientChannelSinkProvider = (IClientChannelSinkProvider)RemotingConfigHandler.CreateChannelSinkProvider(entry, false);
					clientChannelSinkProvider2 = clientChannelSinkProvider;
				}
				else
				{
					clientChannelSinkProvider2.Next = (IClientChannelSinkProvider)RemotingConfigHandler.CreateChannelSinkProvider(entry, false);
					clientChannelSinkProvider2 = clientChannelSinkProvider2.Next;
				}
			}
			return clientChannelSinkProvider;
		}

		[SecurityCritical]
		private static IServerChannelSinkProvider CreateServerChannelSinkProviderChain(ArrayList entries)
		{
			IServerChannelSinkProvider serverChannelSinkProvider = null;
			IServerChannelSinkProvider serverChannelSinkProvider2 = null;
			foreach (object obj in entries)
			{
				RemotingXmlConfigFileData.SinkProviderEntry entry = (RemotingXmlConfigFileData.SinkProviderEntry)obj;
				if (serverChannelSinkProvider == null)
				{
					serverChannelSinkProvider = (IServerChannelSinkProvider)RemotingConfigHandler.CreateChannelSinkProvider(entry, true);
					serverChannelSinkProvider2 = serverChannelSinkProvider;
				}
				else
				{
					serverChannelSinkProvider2.Next = (IServerChannelSinkProvider)RemotingConfigHandler.CreateChannelSinkProvider(entry, true);
					serverChannelSinkProvider2 = serverChannelSinkProvider2.Next;
				}
			}
			return serverChannelSinkProvider;
		}

		[SecurityCritical]
		private static object CreateChannelSinkProvider(RemotingXmlConfigFileData.SinkProviderEntry entry, bool bServer)
		{
			object result = null;
			Type type = RemotingConfigHandler.RemotingConfigInfo.LoadType(entry.TypeName, entry.AssemblyName);
			if (bServer)
			{
				if (!typeof(IServerChannelSinkProvider).IsAssignableFrom(type))
				{
					throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_Config_InvalidSinkProviderType"), type.FullName, "IServerChannelSinkProvider"));
				}
			}
			else if (!typeof(IClientChannelSinkProvider).IsAssignableFrom(type))
			{
				throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_Config_InvalidSinkProviderType"), type.FullName, "IClientChannelSinkProvider"));
			}
			if (entry.IsFormatter && ((bServer && !typeof(IServerFormatterSinkProvider).IsAssignableFrom(type)) || (!bServer && !typeof(IClientFormatterSinkProvider).IsAssignableFrom(type))))
			{
				throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_Config_SinkProviderNotFormatter"), type.FullName));
			}
			object[] args = new object[]
			{
				entry.Properties,
				entry.ProviderData
			};
			try
			{
				result = Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance, null, args, null, null);
			}
			catch (MissingMethodException)
			{
				throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_Config_SinkProviderMissingCtor"), type.FullName, "MySinkProvider(IDictionary properties, ICollection providerData)"));
			}
			return result;
		}

		[SecurityCritical]
		internal static ActivatedClientTypeEntry IsRemotelyActivatedClientType(RuntimeType svrType)
		{
			RemotingTypeCachedData reflectionCachedData = InternalRemotingServices.GetReflectionCachedData(svrType);
			string simpleAssemblyName = reflectionCachedData.SimpleAssemblyName;
			ActivatedClientTypeEntry activatedClientTypeEntry = RemotingConfigHandler.Info.QueryRemoteActivate(svrType.FullName, simpleAssemblyName);
			if (activatedClientTypeEntry == null)
			{
				string assemblyName = reflectionCachedData.AssemblyName;
				activatedClientTypeEntry = RemotingConfigHandler.Info.QueryRemoteActivate(svrType.FullName, assemblyName);
				if (activatedClientTypeEntry == null)
				{
					activatedClientTypeEntry = RemotingConfigHandler.Info.QueryRemoteActivate(svrType.Name, simpleAssemblyName);
				}
			}
			return activatedClientTypeEntry;
		}

		internal static ActivatedClientTypeEntry IsRemotelyActivatedClientType(string typeName, string assemblyName)
		{
			return RemotingConfigHandler.Info.QueryRemoteActivate(typeName, assemblyName);
		}

		[SecurityCritical]
		internal static WellKnownClientTypeEntry IsWellKnownClientType(RuntimeType svrType)
		{
			RemotingTypeCachedData reflectionCachedData = InternalRemotingServices.GetReflectionCachedData(svrType);
			string simpleAssemblyName = reflectionCachedData.SimpleAssemblyName;
			WellKnownClientTypeEntry wellKnownClientTypeEntry = RemotingConfigHandler.Info.QueryConnect(svrType.FullName, simpleAssemblyName);
			if (wellKnownClientTypeEntry == null)
			{
				wellKnownClientTypeEntry = RemotingConfigHandler.Info.QueryConnect(svrType.Name, simpleAssemblyName);
			}
			return wellKnownClientTypeEntry;
		}

		internal static WellKnownClientTypeEntry IsWellKnownClientType(string typeName, string assemblyName)
		{
			return RemotingConfigHandler.Info.QueryConnect(typeName, assemblyName);
		}

		private static void ParseGenericType(string typeAssem, int indexStart, out string typeName, out string assemName)
		{
			int length = typeAssem.Length;
			int num = 1;
			int num2 = indexStart;
			while (num > 0 && ++num2 < length - 1)
			{
				if (typeAssem[num2] == '[')
				{
					num++;
				}
				else if (typeAssem[num2] == ']')
				{
					num--;
				}
			}
			if (num > 0 || num2 >= length)
			{
				typeName = null;
				assemName = null;
				return;
			}
			num2 = typeAssem.IndexOf(',', num2);
			if (num2 >= 0 && num2 < length - 1)
			{
				typeName = typeAssem.Substring(0, num2).Trim();
				assemName = typeAssem.Substring(num2 + 1).Trim();
				return;
			}
			typeName = null;
			assemName = null;
		}

		internal static void ParseType(string typeAssem, out string typeName, out string assemName)
		{
			int num = typeAssem.IndexOf("[");
			if (num >= 0 && num < typeAssem.Length - 1)
			{
				RemotingConfigHandler.ParseGenericType(typeAssem, num, out typeName, out assemName);
				return;
			}
			int num2 = typeAssem.IndexOf(",");
			if (num2 >= 0 && num2 < typeAssem.Length - 1)
			{
				typeName = typeAssem.Substring(0, num2).Trim();
				assemName = typeAssem.Substring(num2 + 1).Trim();
				return;
			}
			typeName = null;
			assemName = null;
		}

		[SecurityCritical]
		internal static bool IsActivationAllowed(RuntimeType svrType)
		{
			if (svrType == null)
			{
				return false;
			}
			RemotingTypeCachedData reflectionCachedData = InternalRemotingServices.GetReflectionCachedData(svrType);
			string simpleAssemblyName = reflectionCachedData.SimpleAssemblyName;
			return RemotingConfigHandler.Info.ActivationAllowed(svrType.FullName, simpleAssemblyName);
		}

		[SecurityCritical]
		internal static bool IsActivationAllowed(string TypeName)
		{
			string text = RemotingServices.InternalGetTypeNameFromQualifiedTypeName(TypeName);
			if (text == null)
			{
				return false;
			}
			string typeName;
			string text2;
			RemotingConfigHandler.ParseType(text, out typeName, out text2);
			if (text2 == null)
			{
				return false;
			}
			int num = text2.IndexOf(',');
			if (num != -1)
			{
				text2 = text2.Substring(0, num);
			}
			return RemotingConfigHandler.Info.ActivationAllowed(typeName, text2);
		}

		internal static void RegisterActivatedServiceType(ActivatedServiceTypeEntry entry)
		{
			RemotingConfigHandler.Info.AddActivatedType(entry.TypeName, entry.AssemblyName, entry.ContextAttributes);
		}

		[SecurityCritical]
		internal static void RegisterWellKnownServiceType(WellKnownServiceTypeEntry entry)
		{
			string typeName = entry.TypeName;
			string assemblyName = entry.AssemblyName;
			string objectUri = entry.ObjectUri;
			WellKnownObjectMode mode = entry.Mode;
			RemotingConfigHandler.RemotingConfigInfo info = RemotingConfigHandler.Info;
			lock (info)
			{
				RemotingConfigHandler.Info.AddWellKnownEntry(entry);
			}
		}

		internal static void RegisterActivatedClientType(ActivatedClientTypeEntry entry)
		{
			RemotingConfigHandler.Info.AddActivatedClientType(entry);
		}

		internal static void RegisterWellKnownClientType(WellKnownClientTypeEntry entry)
		{
			RemotingConfigHandler.Info.AddWellKnownClientType(entry);
		}

		[SecurityCritical]
		internal static Type GetServerTypeForUri(string URI)
		{
			URI = Identity.RemoveAppNameOrAppGuidIfNecessary(URI);
			return RemotingConfigHandler.Info.GetServerTypeForUri(URI);
		}

		internal static ActivatedServiceTypeEntry[] GetRegisteredActivatedServiceTypes()
		{
			return RemotingConfigHandler.Info.GetRegisteredActivatedServiceTypes();
		}

		internal static WellKnownServiceTypeEntry[] GetRegisteredWellKnownServiceTypes()
		{
			return RemotingConfigHandler.Info.GetRegisteredWellKnownServiceTypes();
		}

		internal static ActivatedClientTypeEntry[] GetRegisteredActivatedClientTypes()
		{
			return RemotingConfigHandler.Info.GetRegisteredActivatedClientTypes();
		}

		internal static WellKnownClientTypeEntry[] GetRegisteredWellKnownClientTypes()
		{
			return RemotingConfigHandler.Info.GetRegisteredWellKnownClientTypes();
		}

		[SecurityCritical]
		internal static ServerIdentity CreateWellKnownObject(string uri)
		{
			uri = Identity.RemoveAppNameOrAppGuidIfNecessary(uri);
			return RemotingConfigHandler.Info.StartupWellKnownObject(uri);
		}

		private static volatile string _applicationName;

		private static volatile CustomErrorsModes _errorMode = CustomErrorsModes.RemoteOnly;

		private static volatile bool _errorsModeSet = false;

		private static volatile bool _bMachineConfigLoaded = false;

		private static volatile bool _bUrlObjRefMode = false;

		private static Queue _delayLoadChannelConfigQueue = new Queue();

		public static RemotingConfigHandler.RemotingConfigInfo Info = new RemotingConfigHandler.RemotingConfigInfo();

		private const string _machineConfigFilename = "machine.config";

		internal class RemotingConfigInfo
		{
			internal RemotingConfigInfo()
			{
				this._remoteTypeInfo = Hashtable.Synchronized(new Hashtable());
				this._exportableClasses = Hashtable.Synchronized(new Hashtable());
				this._remoteAppInfo = Hashtable.Synchronized(new Hashtable());
				this._wellKnownExportInfo = Hashtable.Synchronized(new Hashtable());
			}

			private string EncodeTypeAndAssemblyNames(string typeName, string assemblyName)
			{
				return typeName + ", " + assemblyName.ToLower(CultureInfo.InvariantCulture);
			}

			internal void StoreActivatedExports(RemotingXmlConfigFileData configData)
			{
				foreach (object obj in configData.ServerActivatedEntries)
				{
					RemotingXmlConfigFileData.TypeEntry typeEntry = (RemotingXmlConfigFileData.TypeEntry)obj;
					RemotingConfiguration.RegisterActivatedServiceType(new ActivatedServiceTypeEntry(typeEntry.TypeName, typeEntry.AssemblyName)
					{
						ContextAttributes = RemotingConfigHandler.RemotingConfigInfo.CreateContextAttributesFromConfigEntries(typeEntry.ContextAttributes)
					});
				}
			}

			[SecurityCritical]
			internal void StoreInteropEntries(RemotingXmlConfigFileData configData)
			{
				foreach (object obj in configData.InteropXmlElementEntries)
				{
					RemotingXmlConfigFileData.InteropXmlElementEntry interopXmlElementEntry = (RemotingXmlConfigFileData.InteropXmlElementEntry)obj;
					Assembly assembly = Assembly.Load(interopXmlElementEntry.UrtAssemblyName);
					Type type = assembly.GetType(interopXmlElementEntry.UrtTypeName);
					SoapServices.RegisterInteropXmlElement(interopXmlElementEntry.XmlElementName, interopXmlElementEntry.XmlElementNamespace, type);
				}
				foreach (object obj2 in configData.InteropXmlTypeEntries)
				{
					RemotingXmlConfigFileData.InteropXmlTypeEntry interopXmlTypeEntry = (RemotingXmlConfigFileData.InteropXmlTypeEntry)obj2;
					Assembly assembly2 = Assembly.Load(interopXmlTypeEntry.UrtAssemblyName);
					Type type2 = assembly2.GetType(interopXmlTypeEntry.UrtTypeName);
					SoapServices.RegisterInteropXmlType(interopXmlTypeEntry.XmlTypeName, interopXmlTypeEntry.XmlTypeNamespace, type2);
				}
				foreach (object obj3 in configData.PreLoadEntries)
				{
					RemotingXmlConfigFileData.PreLoadEntry preLoadEntry = (RemotingXmlConfigFileData.PreLoadEntry)obj3;
					Assembly assembly3 = Assembly.Load(preLoadEntry.AssemblyName);
					if (preLoadEntry.TypeName != null)
					{
						Type type3 = assembly3.GetType(preLoadEntry.TypeName);
						SoapServices.PreLoad(type3);
					}
					else
					{
						SoapServices.PreLoad(assembly3);
					}
				}
			}

			internal void StoreRemoteAppEntries(RemotingXmlConfigFileData configData)
			{
				char[] trimChars = new char[]
				{
					'/'
				};
				foreach (object obj in configData.RemoteAppEntries)
				{
					RemotingXmlConfigFileData.RemoteAppEntry remoteAppEntry = (RemotingXmlConfigFileData.RemoteAppEntry)obj;
					string text = remoteAppEntry.AppUri;
					if (text != null && !text.EndsWith("/", StringComparison.Ordinal))
					{
						text = text.TrimEnd(trimChars);
					}
					foreach (object obj2 in remoteAppEntry.ActivatedObjects)
					{
						RemotingXmlConfigFileData.TypeEntry typeEntry = (RemotingXmlConfigFileData.TypeEntry)obj2;
						RemotingConfiguration.RegisterActivatedClientType(new ActivatedClientTypeEntry(typeEntry.TypeName, typeEntry.AssemblyName, text)
						{
							ContextAttributes = RemotingConfigHandler.RemotingConfigInfo.CreateContextAttributesFromConfigEntries(typeEntry.ContextAttributes)
						});
					}
					foreach (object obj3 in remoteAppEntry.WellKnownObjects)
					{
						RemotingXmlConfigFileData.ClientWellKnownEntry clientWellKnownEntry = (RemotingXmlConfigFileData.ClientWellKnownEntry)obj3;
						RemotingConfiguration.RegisterWellKnownClientType(new WellKnownClientTypeEntry(clientWellKnownEntry.TypeName, clientWellKnownEntry.AssemblyName, clientWellKnownEntry.Url)
						{
							ApplicationUrl = text
						});
					}
				}
			}

			[SecurityCritical]
			internal void StoreWellKnownExports(RemotingXmlConfigFileData configData)
			{
				foreach (object obj in configData.ServerWellKnownEntries)
				{
					RemotingXmlConfigFileData.ServerWellKnownEntry serverWellKnownEntry = (RemotingXmlConfigFileData.ServerWellKnownEntry)obj;
					RemotingConfigHandler.RegisterWellKnownServiceType(new WellKnownServiceTypeEntry(serverWellKnownEntry.TypeName, serverWellKnownEntry.AssemblyName, serverWellKnownEntry.ObjectURI, serverWellKnownEntry.ObjectMode)
					{
						ContextAttributes = null
					});
				}
			}

			private static IContextAttribute[] CreateContextAttributesFromConfigEntries(ArrayList contextAttributes)
			{
				int count = contextAttributes.Count;
				if (count == 0)
				{
					return null;
				}
				IContextAttribute[] array = new IContextAttribute[count];
				int num = 0;
				foreach (object obj in contextAttributes)
				{
					RemotingXmlConfigFileData.ContextAttributeEntry contextAttributeEntry = (RemotingXmlConfigFileData.ContextAttributeEntry)obj;
					Assembly assembly = Assembly.Load(contextAttributeEntry.AssemblyName);
					Hashtable properties = contextAttributeEntry.Properties;
					IContextAttribute contextAttribute;
					if (properties != null && properties.Count > 0)
					{
						object[] args = new object[]
						{
							properties
						};
						contextAttribute = (IContextAttribute)Activator.CreateInstance(assembly.GetType(contextAttributeEntry.TypeName, false, false), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, args, null, null);
					}
					else
					{
						contextAttribute = (IContextAttribute)Activator.CreateInstance(assembly.GetType(contextAttributeEntry.TypeName, false, false), true);
					}
					array[num++] = contextAttribute;
				}
				return array;
			}

			internal bool ActivationAllowed(string typeName, string assemblyName)
			{
				return this._exportableClasses.ContainsKey(this.EncodeTypeAndAssemblyNames(typeName, assemblyName));
			}

			internal ActivatedClientTypeEntry QueryRemoteActivate(string typeName, string assemblyName)
			{
				string key = this.EncodeTypeAndAssemblyNames(typeName, assemblyName);
				ActivatedClientTypeEntry activatedClientTypeEntry = this._remoteTypeInfo[key] as ActivatedClientTypeEntry;
				if (activatedClientTypeEntry == null)
				{
					return null;
				}
				if (activatedClientTypeEntry.GetRemoteAppEntry() == null)
				{
					RemoteAppEntry remoteAppEntry = (RemoteAppEntry)this._remoteAppInfo[activatedClientTypeEntry.ApplicationUrl];
					if (remoteAppEntry == null)
					{
						throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_Activation_MissingRemoteAppEntry"), activatedClientTypeEntry.ApplicationUrl));
					}
					activatedClientTypeEntry.CacheRemoteAppEntry(remoteAppEntry);
				}
				return activatedClientTypeEntry;
			}

			internal WellKnownClientTypeEntry QueryConnect(string typeName, string assemblyName)
			{
				string key = this.EncodeTypeAndAssemblyNames(typeName, assemblyName);
				WellKnownClientTypeEntry wellKnownClientTypeEntry = this._remoteTypeInfo[key] as WellKnownClientTypeEntry;
				if (wellKnownClientTypeEntry == null)
				{
					return null;
				}
				return wellKnownClientTypeEntry;
			}

			internal ActivatedServiceTypeEntry[] GetRegisteredActivatedServiceTypes()
			{
				ActivatedServiceTypeEntry[] array = new ActivatedServiceTypeEntry[this._exportableClasses.Count];
				int num = 0;
				foreach (object obj in this._exportableClasses)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
					array[num++] = (ActivatedServiceTypeEntry)dictionaryEntry.Value;
				}
				return array;
			}

			internal WellKnownServiceTypeEntry[] GetRegisteredWellKnownServiceTypes()
			{
				WellKnownServiceTypeEntry[] array = new WellKnownServiceTypeEntry[this._wellKnownExportInfo.Count];
				int num = 0;
				foreach (object obj in this._wellKnownExportInfo)
				{
					WellKnownServiceTypeEntry wellKnownServiceTypeEntry = (WellKnownServiceTypeEntry)((DictionaryEntry)obj).Value;
					WellKnownServiceTypeEntry wellKnownServiceTypeEntry2 = new WellKnownServiceTypeEntry(wellKnownServiceTypeEntry.TypeName, wellKnownServiceTypeEntry.AssemblyName, wellKnownServiceTypeEntry.ObjectUri, wellKnownServiceTypeEntry.Mode);
					wellKnownServiceTypeEntry2.ContextAttributes = wellKnownServiceTypeEntry.ContextAttributes;
					array[num++] = wellKnownServiceTypeEntry2;
				}
				return array;
			}

			internal ActivatedClientTypeEntry[] GetRegisteredActivatedClientTypes()
			{
				int num = 0;
				foreach (object obj in this._remoteTypeInfo)
				{
					ActivatedClientTypeEntry activatedClientTypeEntry = ((DictionaryEntry)obj).Value as ActivatedClientTypeEntry;
					if (activatedClientTypeEntry != null)
					{
						num++;
					}
				}
				ActivatedClientTypeEntry[] array = new ActivatedClientTypeEntry[num];
				int num2 = 0;
				foreach (object obj2 in this._remoteTypeInfo)
				{
					ActivatedClientTypeEntry activatedClientTypeEntry2 = ((DictionaryEntry)obj2).Value as ActivatedClientTypeEntry;
					if (activatedClientTypeEntry2 != null)
					{
						string appUrl = null;
						RemoteAppEntry remoteAppEntry = activatedClientTypeEntry2.GetRemoteAppEntry();
						if (remoteAppEntry != null)
						{
							appUrl = remoteAppEntry.GetAppURI();
						}
						ActivatedClientTypeEntry activatedClientTypeEntry3 = new ActivatedClientTypeEntry(activatedClientTypeEntry2.TypeName, activatedClientTypeEntry2.AssemblyName, appUrl);
						activatedClientTypeEntry3.ContextAttributes = activatedClientTypeEntry2.ContextAttributes;
						array[num2++] = activatedClientTypeEntry3;
					}
				}
				return array;
			}

			internal WellKnownClientTypeEntry[] GetRegisteredWellKnownClientTypes()
			{
				int num = 0;
				foreach (object obj in this._remoteTypeInfo)
				{
					WellKnownClientTypeEntry wellKnownClientTypeEntry = ((DictionaryEntry)obj).Value as WellKnownClientTypeEntry;
					if (wellKnownClientTypeEntry != null)
					{
						num++;
					}
				}
				WellKnownClientTypeEntry[] array = new WellKnownClientTypeEntry[num];
				int num2 = 0;
				foreach (object obj2 in this._remoteTypeInfo)
				{
					WellKnownClientTypeEntry wellKnownClientTypeEntry2 = ((DictionaryEntry)obj2).Value as WellKnownClientTypeEntry;
					if (wellKnownClientTypeEntry2 != null)
					{
						WellKnownClientTypeEntry wellKnownClientTypeEntry3 = new WellKnownClientTypeEntry(wellKnownClientTypeEntry2.TypeName, wellKnownClientTypeEntry2.AssemblyName, wellKnownClientTypeEntry2.ObjectUrl);
						RemoteAppEntry remoteAppEntry = wellKnownClientTypeEntry2.GetRemoteAppEntry();
						if (remoteAppEntry != null)
						{
							wellKnownClientTypeEntry3.ApplicationUrl = remoteAppEntry.GetAppURI();
						}
						array[num2++] = wellKnownClientTypeEntry3;
					}
				}
				return array;
			}

			internal void AddActivatedType(string typeName, string assemblyName, IContextAttribute[] contextAttributes)
			{
				if (typeName == null)
				{
					throw new ArgumentNullException("typeName");
				}
				if (assemblyName == null)
				{
					throw new ArgumentNullException("assemblyName");
				}
				if (this.CheckForRedirectedClientType(typeName, assemblyName))
				{
					throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_Config_CantUseRedirectedTypeForWellKnownService"), typeName, assemblyName));
				}
				ActivatedServiceTypeEntry activatedServiceTypeEntry = new ActivatedServiceTypeEntry(typeName, assemblyName);
				activatedServiceTypeEntry.ContextAttributes = contextAttributes;
				string key = this.EncodeTypeAndAssemblyNames(typeName, assemblyName);
				this._exportableClasses.Add(key, activatedServiceTypeEntry);
			}

			private bool CheckForServiceEntryWithType(string typeName, string asmName)
			{
				return this.CheckForWellKnownServiceEntryWithType(typeName, asmName) || this.ActivationAllowed(typeName, asmName);
			}

			private bool CheckForWellKnownServiceEntryWithType(string typeName, string asmName)
			{
				foreach (object obj in this._wellKnownExportInfo)
				{
					WellKnownServiceTypeEntry wellKnownServiceTypeEntry = (WellKnownServiceTypeEntry)((DictionaryEntry)obj).Value;
					if (typeName == wellKnownServiceTypeEntry.TypeName)
					{
						bool flag = false;
						if (asmName == wellKnownServiceTypeEntry.AssemblyName)
						{
							flag = true;
						}
						else if (string.Compare(wellKnownServiceTypeEntry.AssemblyName, 0, asmName, 0, asmName.Length, StringComparison.OrdinalIgnoreCase) == 0 && wellKnownServiceTypeEntry.AssemblyName[asmName.Length] == ',')
						{
							flag = true;
						}
						if (flag)
						{
							return true;
						}
					}
				}
				return false;
			}

			private bool CheckForRedirectedClientType(string typeName, string asmName)
			{
				int num = asmName.IndexOf(",");
				if (num != -1)
				{
					asmName = asmName.Substring(0, num);
				}
				return this.QueryRemoteActivate(typeName, asmName) != null || this.QueryConnect(typeName, asmName) != null;
			}

			internal void AddActivatedClientType(ActivatedClientTypeEntry entry)
			{
				if (this.CheckForRedirectedClientType(entry.TypeName, entry.AssemblyName))
				{
					throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_Config_TypeAlreadyRedirected"), entry.TypeName, entry.AssemblyName));
				}
				if (this.CheckForServiceEntryWithType(entry.TypeName, entry.AssemblyName))
				{
					throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_Config_CantRedirectActivationOfWellKnownService"), entry.TypeName, entry.AssemblyName));
				}
				string applicationUrl = entry.ApplicationUrl;
				RemoteAppEntry remoteAppEntry = (RemoteAppEntry)this._remoteAppInfo[applicationUrl];
				if (remoteAppEntry == null)
				{
					remoteAppEntry = new RemoteAppEntry(applicationUrl, applicationUrl);
					this._remoteAppInfo.Add(applicationUrl, remoteAppEntry);
				}
				if (remoteAppEntry != null)
				{
					entry.CacheRemoteAppEntry(remoteAppEntry);
				}
				string key = this.EncodeTypeAndAssemblyNames(entry.TypeName, entry.AssemblyName);
				this._remoteTypeInfo.Add(key, entry);
			}

			internal void AddWellKnownClientType(WellKnownClientTypeEntry entry)
			{
				if (this.CheckForRedirectedClientType(entry.TypeName, entry.AssemblyName))
				{
					throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_Config_TypeAlreadyRedirected"), entry.TypeName, entry.AssemblyName));
				}
				if (this.CheckForServiceEntryWithType(entry.TypeName, entry.AssemblyName))
				{
					throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_Config_CantRedirectActivationOfWellKnownService"), entry.TypeName, entry.AssemblyName));
				}
				string applicationUrl = entry.ApplicationUrl;
				RemoteAppEntry remoteAppEntry = null;
				if (applicationUrl != null)
				{
					remoteAppEntry = (RemoteAppEntry)this._remoteAppInfo[applicationUrl];
					if (remoteAppEntry == null)
					{
						remoteAppEntry = new RemoteAppEntry(applicationUrl, applicationUrl);
						this._remoteAppInfo.Add(applicationUrl, remoteAppEntry);
					}
				}
				if (remoteAppEntry != null)
				{
					entry.CacheRemoteAppEntry(remoteAppEntry);
				}
				string key = this.EncodeTypeAndAssemblyNames(entry.TypeName, entry.AssemblyName);
				this._remoteTypeInfo.Add(key, entry);
			}

			[SecurityCritical]
			internal void AddWellKnownEntry(WellKnownServiceTypeEntry entry)
			{
				this.AddWellKnownEntry(entry, true);
			}

			[SecurityCritical]
			internal void AddWellKnownEntry(WellKnownServiceTypeEntry entry, bool fReplace)
			{
				if (this.CheckForRedirectedClientType(entry.TypeName, entry.AssemblyName))
				{
					throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_Config_CantUseRedirectedTypeForWellKnownService"), entry.TypeName, entry.AssemblyName));
				}
				string key = entry.ObjectUri.ToLower(CultureInfo.InvariantCulture);
				if (fReplace)
				{
					this._wellKnownExportInfo[key] = entry;
					IdentityHolder.RemoveIdentity(entry.ObjectUri);
					return;
				}
				this._wellKnownExportInfo.Add(key, entry);
			}

			[SecurityCritical]
			internal Type GetServerTypeForUri(string URI)
			{
				Type result = null;
				string key = URI.ToLower(CultureInfo.InvariantCulture);
				WellKnownServiceTypeEntry wellKnownServiceTypeEntry = (WellKnownServiceTypeEntry)this._wellKnownExportInfo[key];
				if (wellKnownServiceTypeEntry != null)
				{
					result = RemotingConfigHandler.RemotingConfigInfo.LoadType(wellKnownServiceTypeEntry.TypeName, wellKnownServiceTypeEntry.AssemblyName);
				}
				return result;
			}

			[SecurityCritical]
			internal ServerIdentity StartupWellKnownObject(string URI)
			{
				string key = URI.ToLower(CultureInfo.InvariantCulture);
				ServerIdentity result = null;
				WellKnownServiceTypeEntry wellKnownServiceTypeEntry = (WellKnownServiceTypeEntry)this._wellKnownExportInfo[key];
				if (wellKnownServiceTypeEntry != null)
				{
					result = this.StartupWellKnownObject(wellKnownServiceTypeEntry.AssemblyName, wellKnownServiceTypeEntry.TypeName, wellKnownServiceTypeEntry.ObjectUri, wellKnownServiceTypeEntry.Mode);
				}
				return result;
			}

			[SecurityCritical]
			internal ServerIdentity StartupWellKnownObject(string asmName, string svrTypeName, string URI, WellKnownObjectMode mode)
			{
				return this.StartupWellKnownObject(asmName, svrTypeName, URI, mode, false);
			}

			[SecurityCritical]
			internal ServerIdentity StartupWellKnownObject(string asmName, string svrTypeName, string URI, WellKnownObjectMode mode, bool fReplace)
			{
				object obj = RemotingConfigHandler.RemotingConfigInfo.s_wkoStartLock;
				ServerIdentity result;
				lock (obj)
				{
					ServerIdentity serverIdentity = null;
					Type type = RemotingConfigHandler.RemotingConfigInfo.LoadType(svrTypeName, asmName);
					if (!type.IsMarshalByRef)
					{
						throw new RemotingException(Environment.GetResourceString("Remoting_WellKnown_MustBeMBR", new object[]
						{
							svrTypeName
						}));
					}
					serverIdentity = (ServerIdentity)IdentityHolder.ResolveIdentity(URI);
					if (serverIdentity != null && serverIdentity.IsRemoteDisconnected())
					{
						IdentityHolder.RemoveIdentity(URI);
						serverIdentity = null;
					}
					if (serverIdentity == null)
					{
						RemotingConfigHandler.RemotingConfigInfo.s_fullTrust.Assert();
						try
						{
							MarshalByRefObject marshalByRefObject = (MarshalByRefObject)Activator.CreateInstance(type, true);
							if (RemotingServices.IsClientProxy(marshalByRefObject))
							{
								RemotingServices.MarshalInternal(new RedirectionProxy(marshalByRefObject, type)
								{
									ObjectMode = mode
								}, URI, type, true, true);
								serverIdentity = (ServerIdentity)IdentityHolder.ResolveIdentity(URI);
								serverIdentity.SetSingletonObjectMode();
							}
							else if (type.IsCOMObject && mode == WellKnownObjectMode.Singleton)
							{
								ComRedirectionProxy obj2 = new ComRedirectionProxy(marshalByRefObject, type);
								RemotingServices.MarshalInternal(obj2, URI, type, true, true);
								serverIdentity = (ServerIdentity)IdentityHolder.ResolveIdentity(URI);
								serverIdentity.SetSingletonObjectMode();
							}
							else
							{
								string objectUri = RemotingServices.GetObjectUri(marshalByRefObject);
								if (objectUri != null)
								{
									throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_WellKnown_CtorCantMarshal"), URI));
								}
								RemotingServices.MarshalInternal(marshalByRefObject, URI, type, true, true);
								serverIdentity = (ServerIdentity)IdentityHolder.ResolveIdentity(URI);
								if (mode == WellKnownObjectMode.SingleCall)
								{
									serverIdentity.SetSingleCallObjectMode();
								}
								else
								{
									serverIdentity.SetSingletonObjectMode();
								}
							}
						}
						catch
						{
							throw;
						}
						finally
						{
							if (serverIdentity != null)
							{
								serverIdentity.IsInitializing = false;
							}
							CodeAccessPermission.RevertAssert();
						}
					}
					result = serverIdentity;
				}
				return result;
			}

			[SecurityCritical]
			internal static Type LoadType(string typeName, string assemblyName)
			{
				Assembly assembly = null;
				new FileIOPermission(PermissionState.Unrestricted).Assert();
				try
				{
					assembly = Assembly.Load(assemblyName);
				}
				finally
				{
					CodeAccessPermission.RevertAssert();
				}
				if (assembly == null)
				{
					throw new RemotingException(Environment.GetResourceString("Remoting_AssemblyLoadFailed", new object[]
					{
						assemblyName
					}));
				}
				Type type = assembly.GetType(typeName, false, false);
				if (type == null)
				{
					throw new RemotingException(Environment.GetResourceString("Remoting_BadType", new object[]
					{
						typeName + ", " + assemblyName
					}));
				}
				return type;
			}

			private Hashtable _exportableClasses;

			private Hashtable _remoteTypeInfo;

			private Hashtable _remoteAppInfo;

			private Hashtable _wellKnownExportInfo;

			private static char[] SepSpace = new char[]
			{
				' '
			};

			private static char[] SepPound = new char[]
			{
				'#'
			};

			private static char[] SepSemiColon = new char[]
			{
				';'
			};

			private static char[] SepEquals = new char[]
			{
				'='
			};

			private static object s_wkoStartLock = new object();

			private static PermissionSet s_fullTrust = new PermissionSet(PermissionState.Unrestricted);
		}
	}
}
