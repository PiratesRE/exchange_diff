using System;
using System.Collections;
using System.Reflection;

namespace System.Runtime.Remoting.Activation
{
	internal class RemotingXmlConfigFileData
	{
		internal void AddInteropXmlElementEntry(string xmlElementName, string xmlElementNamespace, string urtTypeName, string urtAssemblyName)
		{
			this.TryToLoadTypeIfApplicable(urtTypeName, urtAssemblyName);
			RemotingXmlConfigFileData.InteropXmlElementEntry value = new RemotingXmlConfigFileData.InteropXmlElementEntry(xmlElementName, xmlElementNamespace, urtTypeName, urtAssemblyName);
			this.InteropXmlElementEntries.Add(value);
		}

		internal void AddInteropXmlTypeEntry(string xmlTypeName, string xmlTypeNamespace, string urtTypeName, string urtAssemblyName)
		{
			this.TryToLoadTypeIfApplicable(urtTypeName, urtAssemblyName);
			RemotingXmlConfigFileData.InteropXmlTypeEntry value = new RemotingXmlConfigFileData.InteropXmlTypeEntry(xmlTypeName, xmlTypeNamespace, urtTypeName, urtAssemblyName);
			this.InteropXmlTypeEntries.Add(value);
		}

		internal void AddPreLoadEntry(string typeName, string assemblyName)
		{
			this.TryToLoadTypeIfApplicable(typeName, assemblyName);
			RemotingXmlConfigFileData.PreLoadEntry value = new RemotingXmlConfigFileData.PreLoadEntry(typeName, assemblyName);
			this.PreLoadEntries.Add(value);
		}

		internal RemotingXmlConfigFileData.RemoteAppEntry AddRemoteAppEntry(string appUri)
		{
			RemotingXmlConfigFileData.RemoteAppEntry remoteAppEntry = new RemotingXmlConfigFileData.RemoteAppEntry(appUri);
			this.RemoteAppEntries.Add(remoteAppEntry);
			return remoteAppEntry;
		}

		internal void AddServerActivatedEntry(string typeName, string assemName, ArrayList contextAttributes)
		{
			this.TryToLoadTypeIfApplicable(typeName, assemName);
			RemotingXmlConfigFileData.TypeEntry value = new RemotingXmlConfigFileData.TypeEntry(typeName, assemName, contextAttributes);
			this.ServerActivatedEntries.Add(value);
		}

		internal RemotingXmlConfigFileData.ServerWellKnownEntry AddServerWellKnownEntry(string typeName, string assemName, ArrayList contextAttributes, string objURI, WellKnownObjectMode objMode)
		{
			this.TryToLoadTypeIfApplicable(typeName, assemName);
			RemotingXmlConfigFileData.ServerWellKnownEntry serverWellKnownEntry = new RemotingXmlConfigFileData.ServerWellKnownEntry(typeName, assemName, contextAttributes, objURI, objMode);
			this.ServerWellKnownEntries.Add(serverWellKnownEntry);
			return serverWellKnownEntry;
		}

		private void TryToLoadTypeIfApplicable(string typeName, string assemblyName)
		{
			if (!RemotingXmlConfigFileData.LoadTypes)
			{
				return;
			}
			Assembly assembly = Assembly.Load(assemblyName);
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
					typeName
				}));
			}
		}

		internal static volatile bool LoadTypes;

		internal string ApplicationName;

		internal RemotingXmlConfigFileData.LifetimeEntry Lifetime;

		internal bool UrlObjRefMode = RemotingConfigHandler.UrlObjRefMode;

		internal RemotingXmlConfigFileData.CustomErrorsEntry CustomErrors;

		internal ArrayList ChannelEntries = new ArrayList();

		internal ArrayList InteropXmlElementEntries = new ArrayList();

		internal ArrayList InteropXmlTypeEntries = new ArrayList();

		internal ArrayList PreLoadEntries = new ArrayList();

		internal ArrayList RemoteAppEntries = new ArrayList();

		internal ArrayList ServerActivatedEntries = new ArrayList();

		internal ArrayList ServerWellKnownEntries = new ArrayList();

		internal class ChannelEntry
		{
			internal ChannelEntry(string typeName, string assemblyName, Hashtable properties)
			{
				this.TypeName = typeName;
				this.AssemblyName = assemblyName;
				this.Properties = properties;
			}

			internal string TypeName;

			internal string AssemblyName;

			internal Hashtable Properties;

			internal bool DelayLoad;

			internal ArrayList ClientSinkProviders = new ArrayList();

			internal ArrayList ServerSinkProviders = new ArrayList();
		}

		internal class ClientWellKnownEntry
		{
			internal ClientWellKnownEntry(string typeName, string assemName, string url)
			{
				this.TypeName = typeName;
				this.AssemblyName = assemName;
				this.Url = url;
			}

			internal string TypeName;

			internal string AssemblyName;

			internal string Url;
		}

		internal class ContextAttributeEntry
		{
			internal ContextAttributeEntry(string typeName, string assemName, Hashtable properties)
			{
				this.TypeName = typeName;
				this.AssemblyName = assemName;
				this.Properties = properties;
			}

			internal string TypeName;

			internal string AssemblyName;

			internal Hashtable Properties;
		}

		internal class InteropXmlElementEntry
		{
			internal InteropXmlElementEntry(string xmlElementName, string xmlElementNamespace, string urtTypeName, string urtAssemblyName)
			{
				this.XmlElementName = xmlElementName;
				this.XmlElementNamespace = xmlElementNamespace;
				this.UrtTypeName = urtTypeName;
				this.UrtAssemblyName = urtAssemblyName;
			}

			internal string XmlElementName;

			internal string XmlElementNamespace;

			internal string UrtTypeName;

			internal string UrtAssemblyName;
		}

		internal class CustomErrorsEntry
		{
			internal CustomErrorsEntry(CustomErrorsModes mode)
			{
				this.Mode = mode;
			}

			internal CustomErrorsModes Mode;
		}

		internal class InteropXmlTypeEntry
		{
			internal InteropXmlTypeEntry(string xmlTypeName, string xmlTypeNamespace, string urtTypeName, string urtAssemblyName)
			{
				this.XmlTypeName = xmlTypeName;
				this.XmlTypeNamespace = xmlTypeNamespace;
				this.UrtTypeName = urtTypeName;
				this.UrtAssemblyName = urtAssemblyName;
			}

			internal string XmlTypeName;

			internal string XmlTypeNamespace;

			internal string UrtTypeName;

			internal string UrtAssemblyName;
		}

		internal class LifetimeEntry
		{
			internal TimeSpan LeaseTime
			{
				get
				{
					return this._leaseTime;
				}
				set
				{
					this._leaseTime = value;
					this.IsLeaseTimeSet = true;
				}
			}

			internal TimeSpan RenewOnCallTime
			{
				get
				{
					return this._renewOnCallTime;
				}
				set
				{
					this._renewOnCallTime = value;
					this.IsRenewOnCallTimeSet = true;
				}
			}

			internal TimeSpan SponsorshipTimeout
			{
				get
				{
					return this._sponsorshipTimeout;
				}
				set
				{
					this._sponsorshipTimeout = value;
					this.IsSponsorshipTimeoutSet = true;
				}
			}

			internal TimeSpan LeaseManagerPollTime
			{
				get
				{
					return this._leaseManagerPollTime;
				}
				set
				{
					this._leaseManagerPollTime = value;
					this.IsLeaseManagerPollTimeSet = true;
				}
			}

			internal bool IsLeaseTimeSet;

			internal bool IsRenewOnCallTimeSet;

			internal bool IsSponsorshipTimeoutSet;

			internal bool IsLeaseManagerPollTimeSet;

			private TimeSpan _leaseTime;

			private TimeSpan _renewOnCallTime;

			private TimeSpan _sponsorshipTimeout;

			private TimeSpan _leaseManagerPollTime;
		}

		internal class PreLoadEntry
		{
			public PreLoadEntry(string typeName, string assemblyName)
			{
				this.TypeName = typeName;
				this.AssemblyName = assemblyName;
			}

			internal string TypeName;

			internal string AssemblyName;
		}

		internal class RemoteAppEntry
		{
			internal RemoteAppEntry(string appUri)
			{
				this.AppUri = appUri;
			}

			internal void AddWellKnownEntry(string typeName, string assemName, string url)
			{
				RemotingXmlConfigFileData.ClientWellKnownEntry value = new RemotingXmlConfigFileData.ClientWellKnownEntry(typeName, assemName, url);
				this.WellKnownObjects.Add(value);
			}

			internal void AddActivatedEntry(string typeName, string assemName, ArrayList contextAttributes)
			{
				RemotingXmlConfigFileData.TypeEntry value = new RemotingXmlConfigFileData.TypeEntry(typeName, assemName, contextAttributes);
				this.ActivatedObjects.Add(value);
			}

			internal string AppUri;

			internal ArrayList WellKnownObjects = new ArrayList();

			internal ArrayList ActivatedObjects = new ArrayList();
		}

		internal class ServerWellKnownEntry : RemotingXmlConfigFileData.TypeEntry
		{
			internal ServerWellKnownEntry(string typeName, string assemName, ArrayList contextAttributes, string objURI, WellKnownObjectMode objMode) : base(typeName, assemName, contextAttributes)
			{
				this.ObjectURI = objURI;
				this.ObjectMode = objMode;
			}

			internal string ObjectURI;

			internal WellKnownObjectMode ObjectMode;
		}

		internal class SinkProviderEntry
		{
			internal SinkProviderEntry(string typeName, string assemName, Hashtable properties, bool isFormatter)
			{
				this.TypeName = typeName;
				this.AssemblyName = assemName;
				this.Properties = properties;
				this.IsFormatter = isFormatter;
			}

			internal string TypeName;

			internal string AssemblyName;

			internal Hashtable Properties;

			internal ArrayList ProviderData = new ArrayList();

			internal bool IsFormatter;
		}

		internal class TypeEntry
		{
			internal TypeEntry(string typeName, string assemName, ArrayList contextAttributes)
			{
				this.TypeName = typeName;
				this.AssemblyName = assemName;
				this.ContextAttributes = contextAttributes;
			}

			internal string TypeName;

			internal string AssemblyName;

			internal ArrayList ContextAttributes;
		}
	}
}
