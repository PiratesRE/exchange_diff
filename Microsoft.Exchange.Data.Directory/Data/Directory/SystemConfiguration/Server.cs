using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Server)]
	[Serializable]
	public class Server : ADLegacyVersionableObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				if (this.schema == null)
				{
					if (TopologyProvider.IsAdamTopology())
					{
						this.schema = ObjectSchema.GetInstance<ServerSchema>();
					}
					else
					{
						this.schema = ObjectSchema.GetInstance<ActiveDirectoryServerSchema>();
					}
				}
				return this.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return Server.MostDerivedClass;
			}
		}

		internal static GetterDelegate AssistantConfigurationGetterDelegate(TimeBasedAssistantIndex assistantIndex, Server.GetConfigurationDelegate getConfiguration)
		{
			return delegate(IPropertyBag bag)
			{
				MultiValuedProperty<string> allConfigurations = (MultiValuedProperty<string>)bag[ActiveDirectoryServerSchema.AssistantsThrottleWorkcycle];
				Server.AssistantConfigurationEntry configurationForAssistant = Server.AssistantConfigurationEntry.GetConfigurationForAssistant(allConfigurations, assistantIndex);
				return getConfiguration(configurationForAssistant);
			};
		}

		internal static GetterDelegate AssistantWorkCycleGetterDelegate(TimeBasedAssistantIndex assistantIndex)
		{
			return Server.AssistantConfigurationGetterDelegate(assistantIndex, delegate(Server.AssistantConfigurationEntry entry)
			{
				if (entry != null && entry.WorkCycle != EnhancedTimeSpan.Zero)
				{
					return new EnhancedTimeSpan?(entry.WorkCycle);
				}
				return null;
			});
		}

		internal static GetterDelegate AssistantCheckpointGetterDelegate(TimeBasedAssistantIndex assistantIndex)
		{
			return Server.AssistantConfigurationGetterDelegate(assistantIndex, delegate(Server.AssistantConfigurationEntry entry)
			{
				if (entry != null && entry.WorkCycleCheckpoint != EnhancedTimeSpan.Zero)
				{
					return new EnhancedTimeSpan?(entry.WorkCycleCheckpoint);
				}
				return null;
			});
		}

		internal static SetterDelegate AssistantConfigurationSetterDelegate(TimeBasedAssistantIndex assistantIndex, Server.UpdateEntryDelegate updateEntry)
		{
			return delegate(object value, IPropertyBag bag)
			{
				MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)bag[ActiveDirectoryServerSchema.AssistantsThrottleWorkcycle];
				Server.AssistantConfigurationEntry assistantConfigurationEntry = Server.AssistantConfigurationEntry.GetConfigurationForAssistant(multiValuedProperty, assistantIndex);
				for (int i = 0; i < multiValuedProperty.Count; i++)
				{
					if (Server.AssistantConfigurationEntry.IsAssistantConfiguration(multiValuedProperty[i], assistantIndex))
					{
						multiValuedProperty.RemoveAt(i);
						break;
					}
				}
				EnhancedTimeSpan? enhancedTimeSpan = (EnhancedTimeSpan?)value;
				assistantConfigurationEntry = updateEntry(assistantConfigurationEntry, (enhancedTimeSpan != null) ? enhancedTimeSpan.Value : EnhancedTimeSpan.Zero);
				multiValuedProperty.Add(assistantConfigurationEntry.ToString());
				bag[ActiveDirectoryServerSchema.AssistantsThrottleWorkcycle] = multiValuedProperty;
			};
		}

		internal static SetterDelegate AssistantWorkCycleSetterDelegate(TimeBasedAssistantIndex assistantIndex)
		{
			return Server.AssistantConfigurationSetterDelegate(assistantIndex, delegate(Server.AssistantConfigurationEntry entry, EnhancedTimeSpan workCycle)
			{
				if (entry != null)
				{
					entry.WorkCycle = workCycle;
				}
				else
				{
					entry = new Server.AssistantConfigurationEntry(assistantIndex, workCycle, EnhancedTimeSpan.Zero);
				}
				return entry;
			});
		}

		internal static SetterDelegate AssistantCheckpointSetterDelegate(TimeBasedAssistantIndex assistantIndex)
		{
			return Server.AssistantConfigurationSetterDelegate(assistantIndex, delegate(Server.AssistantConfigurationEntry entry, EnhancedTimeSpan checkpoint)
			{
				if (entry != null)
				{
					entry.WorkCycleCheckpoint = checkpoint;
				}
				else
				{
					entry = new Server.AssistantConfigurationEntry(assistantIndex, EnhancedTimeSpan.Zero, checkpoint);
				}
				return entry;
			});
		}

		internal static GetterDelegate AssistantMaintenanceScheduleGetterDelegate(ProviderPropertyDefinition propertyDefinition, ScheduledAssistant assistant)
		{
			return delegate(IPropertyBag bag)
			{
				MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)bag[propertyDefinition];
				foreach (string adString in multiValuedProperty)
				{
					Server.MaintenanceScheduleEntry fromADString = Server.MaintenanceScheduleEntry.GetFromADString(adString, assistant);
					if (fromADString != null)
					{
						return fromADString.MaintenanceSchedule;
					}
				}
				return new ScheduleInterval[0];
			};
		}

		internal static SetterDelegate AssistantMaintenanceScheduleSetterDelegate(ProviderPropertyDefinition propertyDefinition, ScheduledAssistant assistant)
		{
			return delegate(object value, IPropertyBag bag)
			{
				ScheduleInterval[] array = (ScheduleInterval[])value;
				string text = (array == null) ? null : new Server.MaintenanceScheduleEntry(assistant, array).ToADString();
				MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)bag[propertyDefinition];
				for (int i = multiValuedProperty.Count - 1; i >= 0; i--)
				{
					Server.MaintenanceScheduleEntry fromADString = Server.MaintenanceScheduleEntry.GetFromADString(multiValuedProperty[i], assistant);
					if (fromADString != null)
					{
						multiValuedProperty.RemoveAt(i);
					}
				}
				if (text != null)
				{
					multiValuedProperty.Add(text);
				}
				bag[propertyDefinition] = multiValuedProperty;
			};
		}

		internal static object InternalTransportCertificateThumbprintGetter(IPropertyBag propertyBag)
		{
			object result = null;
			try
			{
				byte[] array = (byte[])propertyBag[ServerSchema.InternalTransportCertificate];
				if (array != null)
				{
					X509Certificate2 x509Certificate = new X509Certificate2(array);
					result = x509Certificate.Thumbprint;
				}
			}
			catch (CryptographicException)
			{
			}
			return result;
		}

		internal static object IsPreE12FrontEndGetter(IPropertyBag propertyBag)
		{
			return (bool)Server.IsExchange2003Sp1OrLaterGetter(propertyBag) && !(bool)Server.IsE12OrLaterGetter(propertyBag) && 1 == (int)propertyBag[ServerSchema.ServerRole];
		}

		internal static object IsPreE12RPCHTTPEnabledGetter(IPropertyBag propertyBag)
		{
			return (bool)Server.IsExchange2003Sp1OrLaterGetter(propertyBag) && !(bool)Server.IsE12OrLaterGetter(propertyBag) && 0 != (536870912 & (int)propertyBag[ServerSchema.Heuristics]);
		}

		internal static object IsExchange2003OrLaterGetter(IPropertyBag propertyBag)
		{
			int num = (int)propertyBag[ServerSchema.VersionNumber];
			return num >= Server.E2k3MinVersion;
		}

		internal static object IsExchange2003Sp1OrLaterGetter(IPropertyBag propertyBag)
		{
			int num = (int)propertyBag[ServerSchema.VersionNumber];
			return num >= Server.E2k3SP1MinVersion;
		}

		internal static object IsExchange2003Sp2OrLaterGetter(IPropertyBag propertyBag)
		{
			int num = (int)propertyBag[ServerSchema.VersionNumber];
			return num >= Server.E2k3SP2MinVersion;
		}

		internal static object IsExchange2003Sp3OrLaterGetter(IPropertyBag propertyBag)
		{
			int num = (int)propertyBag[ServerSchema.VersionNumber];
			return num >= Server.E2k3SP3MinVersion;
		}

		internal static object IsE12OrLaterGetter(IPropertyBag propertyBag)
		{
			int num = (int)propertyBag[ServerSchema.VersionNumber];
			return num >= Server.E2007MinVersion;
		}

		internal static object IsE14OrLaterGetter(IPropertyBag propertyBag)
		{
			int num = (int)propertyBag[ServerSchema.VersionNumber];
			return num >= Server.E14MinVersion;
		}

		internal static object IsE14Sp1OrLaterGetter(IPropertyBag propertyBag)
		{
			int num = (int)propertyBag[ServerSchema.VersionNumber];
			return num >= Server.E14SP1MinVersion;
		}

		internal static object IsE15OrLaterGetter(IPropertyBag propertyBag)
		{
			int num = (int)propertyBag[ServerSchema.VersionNumber];
			return num >= Server.E15MinVersion;
		}

		internal static object MajorVersionGetter(IPropertyBag propertyBag)
		{
			int num = (int)propertyBag[ServerSchema.VersionNumber];
			return num >> 22 & 63;
		}

		internal static object FqdnGetter(IPropertyBag propertyBag)
		{
			NetworkAddressCollection networkAddressCollection = (NetworkAddressCollection)propertyBag[ServerSchema.NetworkAddress];
			NetworkAddress networkAddress = networkAddressCollection[NetworkProtocol.TcpIP];
			if (!(networkAddress != null))
			{
				return string.Empty;
			}
			return networkAddress.AddressString;
		}

		internal static QueryFilter FqdnFilterBuilder(SinglePropertyFilter filter)
		{
			string name = ServerSchema.Fqdn.Name;
			ComparisonFilter comparisonFilter = filter as ComparisonFilter;
			if (comparisonFilter == null)
			{
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedFilterForProperty(name, filter.GetType(), typeof(ComparisonFilter)));
			}
			return new ComparisonFilter(comparisonFilter.ComparisonOperator, ServerSchema.NetworkAddress, NetworkProtocol.TcpIP.ProtocolName + ':' + (comparisonFilter.PropertyValue ?? string.Empty));
		}

		internal static object DomainGetter(IPropertyBag propertyBag)
		{
			string result = string.Empty;
			string text = (string)Server.FqdnGetter(propertyBag);
			if (!string.IsNullOrEmpty(text))
			{
				int num = text.IndexOf('.') + 1;
				if (num > 0 && num < text.Length)
				{
					result = text.Substring(num);
				}
			}
			return result;
		}

		internal static object OuGetter(IPropertyBag propertyBag)
		{
			string arg = (string)Server.DomainGetter(propertyBag);
			return string.Format("{0}/{1}", arg, (string)propertyBag[ADObjectSchema.RawName]);
		}

		internal static object EditionGetter(IPropertyBag propertyBag)
		{
			string serverTypeInAD = (string)propertyBag[ServerSchema.ServerType];
			return ServerEdition.DecryptServerEdition(serverTypeInAD);
		}

		internal static void EditionSetter(object value, IPropertyBag propertyBag)
		{
			ServerEditionType edition = (ServerEditionType)value;
			propertyBag[ServerSchema.ServerType] = ServerEdition.EncryptServerEdition(edition);
		}

		internal static object AdminDisplayVersionGetter(IPropertyBag propertyBag)
		{
			string text = (string)propertyBag[ServerSchema.SerialNumber];
			if (string.IsNullOrEmpty(text))
			{
				InvalidOperationException ex = new InvalidOperationException(DirectoryStrings.SerialNumberMissing);
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("AdminDisplayVersion", ex.Message), ServerSchema.AdminDisplayVersion, string.Empty), ex);
			}
			object result;
			try
			{
				result = ServerVersion.ParseFromSerialNumber(text);
			}
			catch (FormatException ex2)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("AdminDisplayVersion", ex2.Message), ServerSchema.AdminDisplayVersion, propertyBag[ServerSchema.SerialNumber]), ex2);
			}
			return result;
		}

		internal static void AdminDisplayVersionSetter(object value, IPropertyBag propertyBag)
		{
			ServerVersion serverVersion = (ServerVersion)value;
			propertyBag[ServerSchema.SerialNumber] = serverVersion.ToString(true);
		}

		internal static object IsMailboxServerGetter(IPropertyBag propertyBag)
		{
			ServerRole serverRole = (ServerRole)propertyBag[ServerSchema.CurrentServerRole];
			return (serverRole & ServerRole.Mailbox) == ServerRole.Mailbox;
		}

		internal static void IsMailboxServerSetter(object value, IPropertyBag propertyBag)
		{
			if ((bool)value)
			{
				propertyBag[ServerSchema.CurrentServerRole] = ((ServerRole)propertyBag[ServerSchema.CurrentServerRole] & ~ServerRole.ProvisionedServer);
				propertyBag[ServerSchema.CurrentServerRole] = ((ServerRole)propertyBag[ServerSchema.CurrentServerRole] | ServerRole.Mailbox);
				return;
			}
			propertyBag[ServerSchema.CurrentServerRole] = ((ServerRole)propertyBag[ServerSchema.CurrentServerRole] & ~ServerRole.Mailbox);
		}

		internal static object IsClientAccessServerGetter(IPropertyBag propertyBag)
		{
			ServerRole serverRole = (ServerRole)propertyBag[ServerSchema.CurrentServerRole];
			return (serverRole & ServerRole.ClientAccess) == ServerRole.ClientAccess;
		}

		internal static void IsClientAccessServerSetter(object value, IPropertyBag propertyBag)
		{
			if ((bool)value)
			{
				propertyBag[ServerSchema.CurrentServerRole] = ((ServerRole)propertyBag[ServerSchema.CurrentServerRole] & ~ServerRole.ProvisionedServer);
				propertyBag[ServerSchema.CurrentServerRole] = ((ServerRole)propertyBag[ServerSchema.CurrentServerRole] | ServerRole.ClientAccess);
				return;
			}
			propertyBag[ServerSchema.CurrentServerRole] = ((ServerRole)propertyBag[ServerSchema.CurrentServerRole] & ~ServerRole.ClientAccess);
		}

		internal static object IsHubTransportServerGetter(IPropertyBag propertyBag)
		{
			ServerRole serverRole = (ServerRole)propertyBag[ServerSchema.CurrentServerRole];
			return (serverRole & ServerRole.HubTransport) == ServerRole.HubTransport;
		}

		internal static void IsHubTransportServerSetter(object value, IPropertyBag propertyBag)
		{
			if ((bool)value)
			{
				propertyBag[ServerSchema.CurrentServerRole] = ((ServerRole)propertyBag[ServerSchema.CurrentServerRole] & ~ServerRole.ProvisionedServer);
				propertyBag[ServerSchema.CurrentServerRole] = ((ServerRole)propertyBag[ServerSchema.CurrentServerRole] | ServerRole.HubTransport);
				return;
			}
			propertyBag[ServerSchema.CurrentServerRole] = ((ServerRole)propertyBag[ServerSchema.CurrentServerRole] & ~ServerRole.HubTransport);
		}

		internal static object IsUnifiedMessagingServerGetter(IPropertyBag propertyBag)
		{
			ServerRole serverRole = (ServerRole)propertyBag[ServerSchema.CurrentServerRole];
			return (serverRole & ServerRole.UnifiedMessaging) == ServerRole.UnifiedMessaging;
		}

		internal static void IsUnifiedMessagingServerSetter(object value, IPropertyBag propertyBag)
		{
			if ((bool)value)
			{
				propertyBag[ServerSchema.CurrentServerRole] = ((ServerRole)propertyBag[ServerSchema.CurrentServerRole] & ~ServerRole.ProvisionedServer);
				propertyBag[ServerSchema.CurrentServerRole] = ((ServerRole)propertyBag[ServerSchema.CurrentServerRole] | ServerRole.UnifiedMessaging);
				return;
			}
			propertyBag[ServerSchema.CurrentServerRole] = ((ServerRole)propertyBag[ServerSchema.CurrentServerRole] & ~ServerRole.UnifiedMessaging);
		}

		internal static object IsEdgeServerGetter(IPropertyBag propertyBag)
		{
			ServerRole serverRole = (ServerRole)propertyBag[ServerSchema.CurrentServerRole];
			return (serverRole & ServerRole.Edge) == ServerRole.Edge;
		}

		internal static void IsEdgeServerSetter(object value, IPropertyBag propertyBag)
		{
			if ((bool)value)
			{
				propertyBag[ServerSchema.CurrentServerRole] = ((ServerRole)propertyBag[ServerSchema.CurrentServerRole] & ~ServerRole.ProvisionedServer);
				propertyBag[ServerSchema.CurrentServerRole] = ((ServerRole)propertyBag[ServerSchema.CurrentServerRole] | ServerRole.Edge);
				return;
			}
			propertyBag[ServerSchema.CurrentServerRole] = ((ServerRole)propertyBag[ServerSchema.CurrentServerRole] & ~ServerRole.Edge);
		}

		internal static object IsProvisionedServerGetter(IPropertyBag propertyBag)
		{
			ServerRole serverRole = (ServerRole)propertyBag[ServerSchema.CurrentServerRole];
			return (serverRole & ServerRole.ProvisionedServer) == ServerRole.ProvisionedServer;
		}

		internal static void IsProvisionedServerSetter(object value, IPropertyBag propertyBag)
		{
			if ((bool)value)
			{
				propertyBag[ServerSchema.CurrentServerRole] = ((ServerRole)propertyBag[ServerSchema.CurrentServerRole] | ServerRole.ProvisionedServer);
				return;
			}
			propertyBag[ServerSchema.CurrentServerRole] = ((ServerRole)propertyBag[ServerSchema.CurrentServerRole] & ~ServerRole.ProvisionedServer);
		}

		internal static QueryFilter CafeServerRoleFlagFilterBuilder(SinglePropertyFilter filter)
		{
			return ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(ServerSchema.CurrentServerRole, 1UL));
		}

		internal static QueryFilter MailboxServerRoleFlagFilterBuilder(SinglePropertyFilter filter)
		{
			return ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(ServerSchema.CurrentServerRole, 2UL));
		}

		internal static object IsCafeServerGetter(IPropertyBag propertyBag)
		{
			ServerRole serverRole = (ServerRole)propertyBag[ServerSchema.CurrentServerRole];
			return (serverRole & ServerRole.Cafe) == ServerRole.Cafe;
		}

		internal static void IsCafeServerSetter(object value, IPropertyBag propertyBag)
		{
			if ((bool)value)
			{
				propertyBag[ServerSchema.CurrentServerRole] = ((ServerRole)propertyBag[ServerSchema.CurrentServerRole] & ~ServerRole.ProvisionedServer);
				propertyBag[ServerSchema.CurrentServerRole] = ((ServerRole)propertyBag[ServerSchema.CurrentServerRole] | ServerRole.Cafe);
				return;
			}
			propertyBag[ServerSchema.CurrentServerRole] = ((ServerRole)propertyBag[ServerSchema.CurrentServerRole] & ~ServerRole.Cafe);
		}

		internal static object IsFrontendTransportServerGetter(IPropertyBag propertyBag)
		{
			ServerRole serverRole = (ServerRole)propertyBag[ServerSchema.CurrentServerRole];
			return (serverRole & ServerRole.FrontendTransport) == ServerRole.FrontendTransport;
		}

		internal static void IsFrontendTransportServerSetter(object value, IPropertyBag propertyBag)
		{
			if ((bool)value)
			{
				propertyBag[ServerSchema.CurrentServerRole] = ((ServerRole)propertyBag[ServerSchema.CurrentServerRole] & ~ServerRole.ProvisionedServer);
				propertyBag[ServerSchema.CurrentServerRole] = ((ServerRole)propertyBag[ServerSchema.CurrentServerRole] | ServerRole.FrontendTransport);
				return;
			}
			propertyBag[ServerSchema.CurrentServerRole] = ((ServerRole)propertyBag[ServerSchema.CurrentServerRole] & ~ServerRole.FrontendTransport);
		}

		internal static object EmptyDomainAllowedGetter(IPropertyBag propertyBag)
		{
			return !(bool)Server.IsMailboxServerGetter(propertyBag) && !(bool)Server.IsClientAccessServerGetter(propertyBag) && !(bool)Server.IsHubTransportServerGetter(propertyBag) && !(bool)Server.IsUnifiedMessagingServerGetter(propertyBag) && !(bool)Server.IsCafeServerGetter(propertyBag) && !(bool)Server.IsFrontendTransportServerGetter(propertyBag);
		}

		internal static object IsExchangeTrialEditionGetter(IPropertyBag propertyBag)
		{
			if (!(bool)propertyBag[ServerSchema.IsExchange2007OrLater])
			{
				return false;
			}
			string value = (string)propertyBag[ServerSchema.ProductID];
			if (string.IsNullOrEmpty(value))
			{
				return true;
			}
			switch ((ServerEditionType)propertyBag[ServerSchema.Edition])
			{
			case ServerEditionType.Standard:
			case ServerEditionType.Enterprise:
			case ServerEditionType.Coexistence:
				return false;
			}
			return true;
		}

		internal static object IsExpiredExchangeTrialEditionGetter(IPropertyBag propertyBag)
		{
			if (!(bool)propertyBag[ServerSchema.IsExchangeTrialEdition])
			{
				return false;
			}
			EnhancedTimeSpan t = (EnhancedTimeSpan)propertyBag[ServerSchema.RemainingTrialPeriod];
			return t <= EnhancedTimeSpan.Zero;
		}

		internal static object RemainingTrialPeriodGetter(IPropertyBag propertyBag)
		{
			EnhancedTimeSpan enhancedTimeSpan = EnhancedTimeSpan.Zero;
			bool flag = (bool)propertyBag[ServerSchema.IsExchangeTrialEdition];
			if (flag)
			{
				DateTime? dateTime = (DateTime?)ADObject.WhenCreatedUTCGetter(propertyBag);
				if (dateTime != null)
				{
					bool flag2 = (bool)propertyBag[ServerSchema.IsE15OrLater];
					if (flag2)
					{
						enhancedTimeSpan = dateTime.Value.Add(Server.E15TrialEditionExpirationPeriod) - DateTime.UtcNow;
					}
					else
					{
						enhancedTimeSpan = dateTime.Value.Add(Server.Exchange2007TrialEditionExpirationPeriod) - DateTime.UtcNow;
					}
					if (enhancedTimeSpan < EnhancedTimeSpan.Zero)
					{
						enhancedTimeSpan = EnhancedTimeSpan.Zero;
					}
				}
			}
			return enhancedTimeSpan;
		}

		internal static object ExternalDNSServersGetter(IPropertyBag propertyBag)
		{
			List<IPAddress> list = Server.ParseStringForAddresses((string)propertyBag[ServerSchema.ExternalDNSServersStr]);
			if (list.Count > 0)
			{
				return new MultiValuedProperty<IPAddress>(false, null, list);
			}
			return new MultiValuedProperty<IPAddress>();
		}

		internal static List<IPAddress> ParseStringForAddresses(string addressString)
		{
			List<IPAddress> list = new List<IPAddress>();
			if (!string.IsNullOrEmpty(addressString))
			{
				char[] separator = new char[]
				{
					',',
					';'
				};
				string[] array = addressString.Split(separator, StringSplitOptions.RemoveEmptyEntries);
				if (array != null && array.Length > 0)
				{
					foreach (string ipString in array)
					{
						IPAddress item;
						if (IPAddress.TryParse(ipString, out item))
						{
							list.Add(item);
						}
					}
				}
			}
			return list;
		}

		internal static void ExternalDNSServersSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[ServerSchema.ExternalDNSServersStr] = Server.FormatAddressesToString((MultiValuedProperty<IPAddress>)value);
		}

		internal static string FormatAddressesToString(MultiValuedProperty<IPAddress> addresses)
		{
			if (addresses == null || addresses.Count == 0)
			{
				return null;
			}
			int num = 0;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (IPAddress ipaddress in addresses)
			{
				num++;
				stringBuilder.Append(ipaddress.ToString());
				if (num < addresses.Count)
				{
					stringBuilder.Append(',');
				}
				else
				{
					stringBuilder.Append(';');
				}
			}
			return stringBuilder.ToString();
		}

		private static string GetDomainOrComputerName(IPropertyBag propertyBag)
		{
			string text = (string)Server.DomainGetter(propertyBag);
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			text = (string)Server.FqdnGetter(propertyBag);
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			return "unknowndomain";
		}

		internal static GetterDelegate MailboxRoleFlagsGetter(ProviderPropertyDefinition propertyDefinition, MailboxServerRoleFlags mask)
		{
			return delegate(IPropertyBag bag)
			{
				MailboxServerRoleFlags mailboxServerRoleFlags = (MailboxServerRoleFlags)bag[propertyDefinition];
				return (mailboxServerRoleFlags & mask) == mask;
			};
		}

		internal static SetterDelegate MailboxRoleFlagsSetter(ProviderPropertyDefinition propertyDefinition, MailboxServerRoleFlags mask)
		{
			return delegate(object value, IPropertyBag bag)
			{
				MailboxServerRoleFlags mailboxServerRoleFlags = (MailboxServerRoleFlags)bag[propertyDefinition];
				bag[propertyDefinition] = (((bool)value) ? (mailboxServerRoleFlags | mask) : (mailboxServerRoleFlags & ~mask));
			};
		}

		internal static object IPAddressFamilyGetter(IPropertyBag propertyBag)
		{
			UMServerSetFlags umserverSetFlags = (UMServerSetFlags)propertyBag[ActiveDirectoryServerSchema.UMServerSet];
			bool flag = (umserverSetFlags & UMServerSetFlags.IPv4Enabled) == UMServerSetFlags.IPv4Enabled;
			bool flag2 = (umserverSetFlags & UMServerSetFlags.IPv6Enabled) == UMServerSetFlags.IPv6Enabled;
			if (flag && flag2)
			{
				return IPAddressFamily.Any;
			}
			if (flag2)
			{
				return IPAddressFamily.IPv6Only;
			}
			if (flag)
			{
				return IPAddressFamily.IPv4Only;
			}
			ExAssert.RetailAssert(false, "At least one of UMServerSChema IPv4Enabled and IPv6Enabled must be set");
			return (IPAddressFamily)(-1);
		}

		internal static void IPAddressFamilySetter(object value, IPropertyBag propertyBag)
		{
			UMServerSetFlags umserverSetFlags = (UMServerSetFlags)propertyBag[ActiveDirectoryServerSchema.UMServerSet];
			IPAddressFamily ipaddressFamily = (IPAddressFamily)value;
			if (ipaddressFamily == IPAddressFamily.Any)
			{
				umserverSetFlags |= UMServerSetFlags.IPv4Enabled;
				umserverSetFlags |= UMServerSetFlags.IPv6Enabled;
			}
			else if (ipaddressFamily == IPAddressFamily.IPv6Only)
			{
				umserverSetFlags &= ~UMServerSetFlags.IPv4Enabled;
				umserverSetFlags |= UMServerSetFlags.IPv6Enabled;
			}
			else if (ipaddressFamily == IPAddressFamily.IPv4Only)
			{
				umserverSetFlags |= UMServerSetFlags.IPv4Enabled;
				umserverSetFlags &= ~UMServerSetFlags.IPv6Enabled;
			}
			else
			{
				ExAssert.RetailAssert(false, "IPAddressFamily set value must be Any, IPv6Only, or IPv4Only");
			}
			propertyBag[ActiveDirectoryServerSchema.UMServerSet] = (int)umserverSetFlags;
		}

		public string ExchangeLegacyDN
		{
			get
			{
				return (string)this[ServerSchema.ExchangeLegacyDN];
			}
			internal set
			{
				this[ServerSchema.ExchangeLegacyDN] = value;
			}
		}

		public ADObjectId ResponsibleMTA
		{
			get
			{
				return (ADObjectId)this[ServerSchema.ResponsibleMTA];
			}
		}

		public int Heuristics
		{
			get
			{
				return (int)this[ServerSchema.Heuristics];
			}
			internal set
			{
				this[ServerSchema.Heuristics] = value;
			}
		}

		public ADObjectId HomeRoutingGroup
		{
			get
			{
				return (ADObjectId)this[ServerSchema.HomeRoutingGroup];
			}
			internal set
			{
				this[ServerSchema.HomeRoutingGroup] = value;
			}
		}

		public NetworkAddressCollection NetworkAddress
		{
			get
			{
				return (NetworkAddressCollection)this[ServerSchema.NetworkAddress];
			}
			internal set
			{
				this[ServerSchema.NetworkAddress] = value;
			}
		}

		public MultiValuedProperty<byte[]> EdgeSyncCredentials
		{
			get
			{
				return (MultiValuedProperty<byte[]>)this[ServerSchema.EdgeSyncCredentials];
			}
			internal set
			{
				this[ServerSchema.EdgeSyncCredentials] = value;
			}
		}

		public MultiValuedProperty<string> EdgeSyncStatus
		{
			get
			{
				return (MultiValuedProperty<string>)this[ServerSchema.EdgeSyncStatus];
			}
			internal set
			{
				this[ServerSchema.EdgeSyncStatus] = value;
			}
		}

		public MultiValuedProperty<byte[]> EdgeSyncCookies
		{
			get
			{
				return (MultiValuedProperty<byte[]>)this[ServerSchema.EdgeSyncCookies];
			}
			internal set
			{
				this[ServerSchema.EdgeSyncCookies] = value;
			}
		}

		public int EdgeSyncAdamSslPort
		{
			get
			{
				return (int)this[ActiveDirectoryServerSchema.EdgeSyncAdamSslPort];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.EdgeSyncAdamSslPort] = value;
			}
		}

		public byte[] InternalTransportCertificate
		{
			get
			{
				return (byte[])this[ServerSchema.InternalTransportCertificate];
			}
			internal set
			{
				this[ServerSchema.InternalTransportCertificate] = value;
			}
		}

		public byte[] EdgeSyncSourceGuid
		{
			get
			{
				return (byte[])this[ServerSchema.EdgeSyncSourceGuid];
			}
			internal set
			{
				this[ServerSchema.EdgeSyncSourceGuid] = value;
			}
		}

		public string InternalTransportCertificateThumbprint
		{
			get
			{
				return (string)this[ServerSchema.InternalTransportCertificateThumbprint];
			}
		}

		public MultiValuedProperty<string> ComponentStates
		{
			get
			{
				return (MultiValuedProperty<string>)this[ServerSchema.ComponentStates];
			}
			internal set
			{
				this[ServerSchema.ComponentStates] = value;
			}
		}

		public string MonitoringGroup
		{
			get
			{
				return (string)this[ServerSchema.MonitoringGroup];
			}
			set
			{
				this[ServerSchema.MonitoringGroup] = value;
			}
		}

		public byte[] EdgeSyncLease
		{
			get
			{
				return (byte[])this[ServerSchema.EdgeSyncLease];
			}
			internal set
			{
				this[ServerSchema.EdgeSyncLease] = value;
			}
		}

		public string ServerType
		{
			get
			{
				return (string)this[ServerSchema.ServerType];
			}
			internal set
			{
				this[ServerSchema.ServerType] = value;
			}
		}

		public bool IsMailboxServer
		{
			get
			{
				return (bool)this[ServerSchema.IsMailboxServer];
			}
			internal set
			{
				this[ServerSchema.IsMailboxServer] = value;
			}
		}

		public bool? CustomerFeedbackEnabled
		{
			get
			{
				return (bool?)this[ServerSchema.CustomerFeedbackEnabled];
			}
			set
			{
				this[ServerSchema.CustomerFeedbackEnabled] = value;
			}
		}

		public Uri InternetWebProxy
		{
			get
			{
				return (Uri)this[ServerSchema.InternetWebProxy];
			}
			set
			{
				this[ServerSchema.InternetWebProxy] = value;
			}
		}

		public bool IsClientAccessServer
		{
			get
			{
				return (bool)this[ServerSchema.IsClientAccessServer];
			}
			internal set
			{
				this[ServerSchema.IsClientAccessServer] = value;
			}
		}

		public bool IsUnifiedMessagingServer
		{
			get
			{
				return (bool)this[ServerSchema.IsUnifiedMessagingServer];
			}
			internal set
			{
				this[ServerSchema.IsUnifiedMessagingServer] = value;
			}
		}

		public bool IsHubTransportServer
		{
			get
			{
				return (bool)this[ServerSchema.IsHubTransportServer];
			}
			internal set
			{
				this[ServerSchema.IsHubTransportServer] = value;
			}
		}

		public bool IsEdgeServer
		{
			get
			{
				return (bool)this[ServerSchema.IsEdgeServer];
			}
			internal set
			{
				this[ServerSchema.IsEdgeServer] = value;
			}
		}

		public bool IsCafeServer
		{
			get
			{
				return (bool)this[ServerSchema.IsCafeServer];
			}
			internal set
			{
				this[ServerSchema.IsCafeServer] = value;
			}
		}

		public bool IsFrontendTransportServer
		{
			get
			{
				return (bool)this[ServerSchema.IsFrontendTransportServer];
			}
			internal set
			{
				this[ServerSchema.IsFrontendTransportServer] = value;
			}
		}

		public bool IsPhoneticSupportEnabled
		{
			get
			{
				return (bool)this[ServerSchema.IsPhoneticSupportEnabled];
			}
			internal set
			{
				this[ServerSchema.IsPhoneticSupportEnabled] = value;
			}
		}

		public bool EmptyDomainAllowed
		{
			get
			{
				return (bool)this[ServerSchema.EmptyDomainAllowed];
			}
			internal set
			{
				this[ServerSchema.EmptyDomainAllowed] = value;
			}
		}

		public ServerVersion AdminDisplayVersion
		{
			get
			{
				return (ServerVersion)this[ServerSchema.AdminDisplayVersion];
			}
			internal set
			{
				this[ServerSchema.AdminDisplayVersion] = value;
			}
		}

		public string Domain
		{
			get
			{
				return (string)this[ServerSchema.Domain];
			}
		}

		public string OrganizationalUnit
		{
			get
			{
				return (string)this[ServerSchema.OrganizationalUnit];
			}
		}

		public ServerRole CurrentServerRole
		{
			get
			{
				return (ServerRole)this[ServerSchema.CurrentServerRole];
			}
			internal set
			{
				this[ServerSchema.CurrentServerRole] = value;
			}
		}

		public string SerialNumber
		{
			get
			{
				return (string)this.propertyBag[ServerSchema.SerialNumber];
			}
		}

		public int VersionNumber
		{
			get
			{
				return (int)this[ServerSchema.VersionNumber];
			}
			internal set
			{
				this[ServerSchema.VersionNumber] = value;
			}
		}

		public int MajorVersion
		{
			get
			{
				return (int)this[ServerSchema.MajorVersion];
			}
		}

		public LocalLongFullPath DataPath
		{
			get
			{
				return (LocalLongFullPath)this[ServerSchema.DataPath];
			}
			internal set
			{
				this[ServerSchema.DataPath] = value;
			}
		}

		public LocalLongFullPath InstallPath
		{
			get
			{
				return (LocalLongFullPath)this[ServerSchema.InstallPath];
			}
		}

		public ServerEditionType Edition
		{
			get
			{
				return (ServerEditionType)this[ServerSchema.Edition];
			}
			internal set
			{
				this[ServerSchema.Edition] = value;
			}
		}

		public bool IsPreE12FrontEnd
		{
			get
			{
				return (bool)this[ServerSchema.IsPreE12FrontEnd];
			}
		}

		public bool IsPreE12RPCHTTPEnabled
		{
			get
			{
				return (bool)this[ServerSchema.IsPreE12RPCHTTPEnabled];
			}
		}

		public bool IsProvisionedServer
		{
			get
			{
				return (bool)this[ServerSchema.IsProvisionedServer];
			}
			internal set
			{
				this[ServerSchema.IsProvisionedServer] = value;
			}
		}

		public bool IsExchange2003OrLater
		{
			get
			{
				return (bool)this[ServerSchema.IsExchange2003OrLater];
			}
		}

		public bool IsExchange2003Sp1OrLater
		{
			get
			{
				return (bool)this[ServerSchema.IsExchange2003Sp1OrLater];
			}
		}

		public bool IsExchange2003Sp2OrLater
		{
			get
			{
				return (bool)this[ServerSchema.IsExchange2003Sp2OrLater];
			}
		}

		public bool IsExchange2003Sp3OrLater
		{
			get
			{
				return (bool)this[ServerSchema.IsExchange2003Sp3OrLater];
			}
		}

		public bool IsExchange2007OrLater
		{
			get
			{
				return (bool)this[ServerSchema.IsExchange2007OrLater];
			}
		}

		public bool IsE14OrLater
		{
			get
			{
				return (bool)this[ServerSchema.IsE14OrLater];
			}
		}

		public bool IsE14Sp1OrLater
		{
			get
			{
				return (bool)this[ServerSchema.IsE14Sp1OrLater];
			}
		}

		public bool IsE15OrLater
		{
			get
			{
				return (bool)this[ServerSchema.IsE15OrLater];
			}
		}

		public string Fqdn
		{
			get
			{
				return (string)this[ServerSchema.Fqdn];
			}
		}

		public int IntraOrgConnectorSmtpMaxMessagesPerConnection
		{
			get
			{
				return (int)this[ServerSchema.IntraOrgConnectorSmtpMaxMessagesPerConnection];
			}
			internal set
			{
				this[ServerSchema.IntraOrgConnectorSmtpMaxMessagesPerConnection] = value;
			}
		}

		public ScheduleInterval[] ManagedFolderAssistantSchedule
		{
			get
			{
				return (ScheduleInterval[])this[ServerSchema.ElcSchedule];
			}
			internal set
			{
				this[ServerSchema.ElcSchedule] = value;
			}
		}

		public LocalLongFullPath LogPathForManagedFolders
		{
			get
			{
				return (LocalLongFullPath)this[ActiveDirectoryServerSchema.ElcAuditLogPath];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.ElcAuditLogPath] = value;
			}
		}

		public EnhancedTimeSpan LogFileAgeLimitForManagedFolders
		{
			get
			{
				return (EnhancedTimeSpan)this[ActiveDirectoryServerSchema.ElcAuditLogFileAgeLimit];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.ElcAuditLogFileAgeLimit] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> LogDirectorySizeLimitForManagedFolders
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ActiveDirectoryServerSchema.ElcAuditLogDirectorySizeLimit];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.ElcAuditLogDirectorySizeLimit] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> LogFileSizeLimitForManagedFolders
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ActiveDirectoryServerSchema.ElcAuditLogFileSizeLimit];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.ElcAuditLogFileSizeLimit] = value;
			}
		}

		public bool MAPIEncryptionRequired
		{
			get
			{
				return (bool)this[ActiveDirectoryServerSchema.MAPIEncryptionRequired];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.MAPIEncryptionRequired] = value;
			}
		}

		public bool RetentionLogForManagedFoldersEnabled
		{
			get
			{
				return (bool)this[ActiveDirectoryServerSchema.ExpirationAuditLogEnabled];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.ExpirationAuditLogEnabled] = value;
			}
		}

		public bool JournalingLogForManagedFoldersEnabled
		{
			get
			{
				return (bool)this[ActiveDirectoryServerSchema.AutocopyAuditLogEnabled];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.AutocopyAuditLogEnabled] = value;
			}
		}

		public bool FolderLogForManagedFoldersEnabled
		{
			get
			{
				return (bool)this[ActiveDirectoryServerSchema.FolderAuditLogEnabled];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.FolderAuditLogEnabled] = value;
			}
		}

		public bool SubjectLogForManagedFoldersEnabled
		{
			get
			{
				return (bool)this[ActiveDirectoryServerSchema.ElcSubjectLoggingEnabled];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.ElcSubjectLoggingEnabled] = value;
			}
		}

		public ScheduleInterval[] SharingPolicySchedule
		{
			get
			{
				return (ScheduleInterval[])this[ActiveDirectoryServerSchema.SharingPolicySchedule];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.SharingPolicySchedule] = value;
			}
		}

		public bool CalendarRepairMissingItemFixDisabled
		{
			get
			{
				return (bool)this[ActiveDirectoryServerSchema.CalendarRepairMissingItemFixDisabled];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.CalendarRepairMissingItemFixDisabled] = value;
			}
		}

		public bool CalendarRepairLogEnabled
		{
			get
			{
				return (bool)this[ActiveDirectoryServerSchema.CalendarRepairLogEnabled];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.CalendarRepairLogEnabled] = value;
			}
		}

		public bool CalendarRepairLogSubjectLoggingEnabled
		{
			get
			{
				return (bool)this[ActiveDirectoryServerSchema.CalendarRepairLogSubjectLoggingEnabled];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.CalendarRepairLogSubjectLoggingEnabled] = value;
			}
		}

		public LocalLongFullPath CalendarRepairLogPath
		{
			get
			{
				return (LocalLongFullPath)this[ActiveDirectoryServerSchema.CalendarRepairLogPath];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.CalendarRepairLogPath] = value;
			}
		}

		public int CalendarRepairIntervalEndWindow
		{
			get
			{
				return (int)this[ActiveDirectoryServerSchema.CalendarRepairIntervalEndWindow];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.CalendarRepairIntervalEndWindow] = value;
			}
		}

		public EnhancedTimeSpan CalendarRepairLogFileAgeLimit
		{
			get
			{
				return (EnhancedTimeSpan)this[ActiveDirectoryServerSchema.CalendarRepairLogFileAgeLimit];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.CalendarRepairLogFileAgeLimit] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> CalendarRepairLogDirectorySizeLimit
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ActiveDirectoryServerSchema.CalendarRepairLogDirectorySizeLimit];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.CalendarRepairLogDirectorySizeLimit] = value;
			}
		}

		public CalendarRepairType CalendarRepairMode
		{
			get
			{
				return (CalendarRepairType)this[ActiveDirectoryServerSchema.CalendarRepairMode];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.CalendarRepairMode] = value;
			}
		}

		public EnhancedTimeSpan? CalendarRepairWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.CalendarRepairWorkCycle];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.CalendarRepairWorkCycle] = value;
			}
		}

		public EnhancedTimeSpan? CalendarRepairWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.CalendarRepairWorkCycleCheckpoint];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.CalendarRepairWorkCycleCheckpoint] = value;
			}
		}

		public EnhancedTimeSpan? SharingPolicyWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.SharingPolicyWorkCycle];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.SharingPolicyWorkCycle] = value;
			}
		}

		public EnhancedTimeSpan? SharingPolicyWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.SharingPolicyWorkCycleCheckpoint];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.SharingPolicyWorkCycleCheckpoint] = value;
			}
		}

		public EnhancedTimeSpan? PublicFolderWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.PublicFolderWorkCycle];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.PublicFolderWorkCycle] = value;
			}
		}

		public EnhancedTimeSpan? PublicFolderWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.PublicFolderWorkCycleCheckpoint];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.PublicFolderWorkCycleCheckpoint] = value;
			}
		}

		public EnhancedTimeSpan? SiteMailboxWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.SiteMailboxWorkCycle];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.SiteMailboxWorkCycle] = value;
			}
		}

		public EnhancedTimeSpan? SiteMailboxWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.SiteMailboxWorkCycleCheckpoint];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.SiteMailboxWorkCycleCheckpoint] = value;
			}
		}

		public EnhancedTimeSpan? SharingSyncWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.SharingSyncWorkCycle];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.SharingSyncWorkCycle] = value;
			}
		}

		public EnhancedTimeSpan? SharingSyncWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.SharingSyncWorkCycleCheckpoint];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.SharingSyncWorkCycleCheckpoint] = value;
			}
		}

		public EnhancedTimeSpan? ManagedFolderWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.ManagedFolderWorkCycle];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.ManagedFolderWorkCycle] = value;
			}
		}

		public EnhancedTimeSpan? ManagedFolderWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.ManagedFolderWorkCycleCheckpoint];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.ManagedFolderWorkCycleCheckpoint] = value;
			}
		}

		public EnhancedTimeSpan? MailboxAssociationReplicationWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.MailboxAssociationReplicationWorkCycle];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.MailboxAssociationReplicationWorkCycle] = value;
			}
		}

		public EnhancedTimeSpan? MailboxAssociationReplicationWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.MailboxAssociationReplicationWorkCycleCheckpoint];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.MailboxAssociationReplicationWorkCycleCheckpoint] = value;
			}
		}

		public EnhancedTimeSpan? GroupMailboxWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.GroupMailboxWorkCycle];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.GroupMailboxWorkCycle] = value;
			}
		}

		public EnhancedTimeSpan? GroupMailboxWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.GroupMailboxWorkCycleCheckpoint];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.GroupMailboxWorkCycleCheckpoint] = value;
			}
		}

		public EnhancedTimeSpan? TopNWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.TopNWorkCycle];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.TopNWorkCycle] = value;
			}
		}

		public EnhancedTimeSpan? TopNWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.TopNWorkCycleCheckpoint];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.TopNWorkCycleCheckpoint] = value;
			}
		}

		public EnhancedTimeSpan? UMReportingWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.UMReportingWorkCycle];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.UMReportingWorkCycle] = value;
			}
		}

		public EnhancedTimeSpan? UMReportingWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.UMReportingWorkCycleCheckpoint];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.UMReportingWorkCycleCheckpoint] = value;
			}
		}

		public EnhancedTimeSpan? InferenceTrainingWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.InferenceTrainingWorkCycle];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.InferenceTrainingWorkCycle] = value;
			}
		}

		public EnhancedTimeSpan? InferenceTrainingWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.InferenceTrainingWorkCycleCheckpoint];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.InferenceTrainingWorkCycleCheckpoint] = value;
			}
		}

		public EnhancedTimeSpan? DirectoryProcessorWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.DirectoryProcessorWorkCycle];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.DirectoryProcessorWorkCycle] = value;
			}
		}

		public EnhancedTimeSpan? DirectoryProcessorWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.DirectoryProcessorWorkCycleCheckpoint];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.DirectoryProcessorWorkCycleCheckpoint] = value;
			}
		}

		public EnhancedTimeSpan? OABGeneratorWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.OABGeneratorWorkCycle];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.OABGeneratorWorkCycle] = value;
			}
		}

		public EnhancedTimeSpan? OABGeneratorWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.OABGeneratorWorkCycleCheckpoint];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.OABGeneratorWorkCycleCheckpoint] = value;
			}
		}

		public EnhancedTimeSpan? InferenceDataCollectionWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.InferenceDataCollectionWorkCycle];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.InferenceDataCollectionWorkCycle] = value;
			}
		}

		public EnhancedTimeSpan? InferenceDataCollectionWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.InferenceDataCollectionWorkCycleCheckpoint];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.InferenceDataCollectionWorkCycleCheckpoint] = value;
			}
		}

		public EnhancedTimeSpan? PeopleRelevanceWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.PeopleRelevanceWorkCycle];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.PeopleRelevanceWorkCycle] = value;
			}
		}

		public EnhancedTimeSpan? PeopleRelevanceWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.PeopleRelevanceWorkCycleCheckpoint];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.PeopleRelevanceWorkCycleCheckpoint] = value;
			}
		}

		public EnhancedTimeSpan? SharePointSignalStoreWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.SharePointSignalStoreWorkCycle];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.SharePointSignalStoreWorkCycle] = value;
			}
		}

		public EnhancedTimeSpan? SharePointSignalStoreWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.SharePointSignalStoreWorkCycleCheckpoint];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.SharePointSignalStoreWorkCycleCheckpoint] = value;
			}
		}

		public EnhancedTimeSpan? PeopleCentricTriageWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.PeopleCentricTriageWorkCycle];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.PeopleCentricTriageWorkCycle] = value;
			}
		}

		public EnhancedTimeSpan? PeopleCentricTriageWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.PeopleCentricTriageWorkCycleCheckpoint];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.PeopleCentricTriageWorkCycleCheckpoint] = value;
			}
		}

		public EnhancedTimeSpan? MailboxProcessorWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.MailboxProcessorWorkCycle];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.MailboxProcessorWorkCycle] = value;
			}
		}

		public EnhancedTimeSpan? StoreDsMaintenanceWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.StoreDsMaintenanceWorkCycle];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.StoreDsMaintenanceWorkCycle] = value;
			}
		}

		public EnhancedTimeSpan? StoreDsMaintenanceWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.StoreDsMaintenanceWorkCycleCheckpoint];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.StoreDsMaintenanceWorkCycleCheckpoint] = value;
			}
		}

		public EnhancedTimeSpan? StoreIntegrityCheckWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.StoreIntegrityCheckWorkCycle];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.StoreIntegrityCheckWorkCycle] = value;
			}
		}

		public EnhancedTimeSpan? StoreIntegrityCheckWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.StoreIntegrityCheckWorkCycleCheckpoint];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.StoreIntegrityCheckWorkCycleCheckpoint] = value;
			}
		}

		public EnhancedTimeSpan? StoreMaintenanceWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.StoreMaintenanceWorkCycle];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.StoreMaintenanceWorkCycle] = value;
			}
		}

		public EnhancedTimeSpan? StoreMaintenanceWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.StoreMaintenanceWorkCycleCheckpoint];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.StoreMaintenanceWorkCycleCheckpoint] = value;
			}
		}

		public EnhancedTimeSpan? StoreScheduledIntegrityCheckWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.StoreScheduledIntegrityCheckWorkCycle];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.StoreScheduledIntegrityCheckWorkCycle] = value;
			}
		}

		public EnhancedTimeSpan? StoreScheduledIntegrityCheckWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.StoreScheduledIntegrityCheckWorkCycleCheckpoint];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.StoreScheduledIntegrityCheckWorkCycleCheckpoint] = value;
			}
		}

		public EnhancedTimeSpan? StoreUrgentMaintenanceWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.StoreUrgentMaintenanceWorkCycle];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.StoreUrgentMaintenanceWorkCycle] = value;
			}
		}

		public EnhancedTimeSpan? StoreUrgentMaintenanceWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.StoreUrgentMaintenanceWorkCycleCheckpoint];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.StoreUrgentMaintenanceWorkCycleCheckpoint] = value;
			}
		}

		public EnhancedTimeSpan? JunkEmailOptionsCommitterWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.JunkEmailOptionsCommitterWorkCycle];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.JunkEmailOptionsCommitterWorkCycle] = value;
			}
		}

		public EnhancedTimeSpan? ProbeTimeBasedAssistantWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.ProbeTimeBasedAssistantWorkCycle];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.ProbeTimeBasedAssistantWorkCycle] = value;
			}
		}

		public EnhancedTimeSpan? ProbeTimeBasedAssistantWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.ProbeTimeBasedAssistantWorkCycleCheckpoint];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.ProbeTimeBasedAssistantWorkCycleCheckpoint] = value;
			}
		}

		public EnhancedTimeSpan? SearchIndexRepairTimeBasedAssistantWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.SearchIndexRepairTimeBasedAssistantWorkCycle];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.SearchIndexRepairTimeBasedAssistantWorkCycle] = value;
			}
		}

		public EnhancedTimeSpan? SearchIndexRepairTimeBasedAssistantWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.SearchIndexRepairTimeBasedAssistantWorkCycleCheckpoint];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.SearchIndexRepairTimeBasedAssistantWorkCycleCheckpoint] = value;
			}
		}

		public EnhancedTimeSpan? DarTaskStoreTimeBasedAssistantWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.DarTaskStoreTimeBasedAssistantWorkCycle];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.DarTaskStoreTimeBasedAssistantWorkCycle] = value;
			}
		}

		public EnhancedTimeSpan? DarTaskStoreTimeBasedAssistantWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[ActiveDirectoryServerSchema.DarTaskStoreTimeBasedAssistantWorkCycleCheckpoint];
			}
			internal set
			{
				this[ActiveDirectoryServerSchema.DarTaskStoreTimeBasedAssistantWorkCycleCheckpoint] = value;
			}
		}

		internal MailboxServerRoleFlags MailboxRoleFlags
		{
			get
			{
				return (MailboxServerRoleFlags)this[ActiveDirectoryServerSchema.MailboxRoleFlags];
			}
			set
			{
				this[ActiveDirectoryServerSchema.MailboxRoleFlags] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan DelayNotificationTimeout
		{
			get
			{
				return (EnhancedTimeSpan)this[ServerSchema.DelayNotificationTimeout];
			}
			set
			{
				this[ServerSchema.DelayNotificationTimeout] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan MessageExpirationTimeout
		{
			get
			{
				return (EnhancedTimeSpan)this[ServerSchema.MessageExpirationTimeout];
			}
			set
			{
				this[ServerSchema.MessageExpirationTimeout] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan QueueMaxIdleTime
		{
			get
			{
				return (EnhancedTimeSpan)this[ServerSchema.QueueMaxIdleTime];
			}
			set
			{
				this[ServerSchema.QueueMaxIdleTime] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan MessageRetryInterval
		{
			get
			{
				return (EnhancedTimeSpan)this[ServerSchema.MessageRetryInterval];
			}
			set
			{
				this[ServerSchema.MessageRetryInterval] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan TransientFailureRetryInterval
		{
			get
			{
				return (EnhancedTimeSpan)this[ServerSchema.TransientFailureRetryInterval];
			}
			set
			{
				this[ServerSchema.TransientFailureRetryInterval] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int TransientFailureRetryCount
		{
			get
			{
				return (int)this[ServerSchema.TransientFailureRetryCount];
			}
			set
			{
				this[ServerSchema.TransientFailureRetryCount] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> MaxOutboundConnections
		{
			get
			{
				return (Unlimited<int>)this[ServerSchema.MaxOutboundConnections];
			}
			set
			{
				this[ServerSchema.MaxOutboundConnections] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> MaxPerDomainOutboundConnections
		{
			get
			{
				return (Unlimited<int>)this[ServerSchema.MaxPerDomainOutboundConnections];
			}
			set
			{
				this[ServerSchema.MaxPerDomainOutboundConnections] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MaxConnectionRatePerMinute
		{
			get
			{
				return (int)this[ServerSchema.MaxConnectionRatePerMinute];
			}
			set
			{
				this[ServerSchema.MaxConnectionRatePerMinute] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath ReceiveProtocolLogPath
		{
			get
			{
				return (LocalLongFullPath)this[ServerSchema.ReceiveProtocolLogPath];
			}
			set
			{
				this[ServerSchema.ReceiveProtocolLogPath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath SendProtocolLogPath
		{
			get
			{
				return (LocalLongFullPath)this[ServerSchema.SendProtocolLogPath];
			}
			set
			{
				this[ServerSchema.SendProtocolLogPath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan OutboundConnectionFailureRetryInterval
		{
			get
			{
				return (EnhancedTimeSpan)this[ServerSchema.OutboundConnectionFailureRetryInterval];
			}
			set
			{
				this[ServerSchema.OutboundConnectionFailureRetryInterval] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan ReceiveProtocolLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[ServerSchema.ReceiveProtocolLogMaxAge];
			}
			set
			{
				this[ServerSchema.ReceiveProtocolLogMaxAge] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> ReceiveProtocolLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.ReceiveProtocolLogMaxDirectorySize];
			}
			set
			{
				this[ServerSchema.ReceiveProtocolLogMaxDirectorySize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> ReceiveProtocolLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.ReceiveProtocolLogMaxFileSize];
			}
			set
			{
				this[ServerSchema.ReceiveProtocolLogMaxFileSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan SendProtocolLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[ServerSchema.SendProtocolLogMaxAge];
			}
			set
			{
				this[ServerSchema.SendProtocolLogMaxAge] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> SendProtocolLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.SendProtocolLogMaxDirectorySize];
			}
			set
			{
				this[ServerSchema.SendProtocolLogMaxDirectorySize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> SendProtocolLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.SendProtocolLogMaxFileSize];
			}
			set
			{
				this[ServerSchema.SendProtocolLogMaxFileSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool InternalDNSAdapterEnabled
		{
			get
			{
				return !(bool)this[ServerSchema.InternalDNSAdapterDisabled];
			}
			set
			{
				this[ServerSchema.InternalDNSAdapterDisabled] = !value;
			}
		}

		[Parameter(Mandatory = false)]
		public Guid InternalDNSAdapterGuid
		{
			get
			{
				return (Guid)this[ServerSchema.InternalDNSAdapterGuid];
			}
			set
			{
				this[ServerSchema.InternalDNSAdapterGuid] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<IPAddress> InternalDNSServers
		{
			get
			{
				return (MultiValuedProperty<IPAddress>)this[ServerSchema.InternalDNSServers];
			}
			set
			{
				this[ServerSchema.InternalDNSServers] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ProtocolOption InternalDNSProtocolOption
		{
			get
			{
				return (ProtocolOption)this[ServerSchema.InternalDNSProtocolOption];
			}
			set
			{
				this[ServerSchema.InternalDNSProtocolOption] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ExternalDNSAdapterEnabled
		{
			get
			{
				return !(bool)this[ServerSchema.ExternalDNSAdapterDisabled];
			}
			set
			{
				this[ServerSchema.ExternalDNSAdapterDisabled] = !value;
			}
		}

		[Parameter(Mandatory = false)]
		public Guid ExternalDNSAdapterGuid
		{
			get
			{
				return (Guid)this[ServerSchema.ExternalDNSAdapterGuid];
			}
			set
			{
				this[ServerSchema.ExternalDNSAdapterGuid] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<IPAddress> ExternalDNSServers
		{
			get
			{
				return (MultiValuedProperty<IPAddress>)this[ServerSchema.ExternalDNSServers];
			}
			set
			{
				this[ServerSchema.ExternalDNSServers] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public IPAddress ExternalIPAddress
		{
			get
			{
				return (IPAddress)this[ServerSchema.ExternalIPAddress];
			}
			set
			{
				this[ServerSchema.ExternalIPAddress] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ProtocolOption ExternalDNSProtocolOption
		{
			get
			{
				return (ProtocolOption)this[ServerSchema.ExternalDNSProtocolOption];
			}
			set
			{
				this[ServerSchema.ExternalDNSProtocolOption] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MaxConcurrentMailboxDeliveries
		{
			get
			{
				return (int)this[ServerSchema.MaxConcurrentMailboxDeliveries];
			}
			set
			{
				this[ServerSchema.MaxConcurrentMailboxDeliveries] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MaxConcurrentMailboxSubmissions
		{
			get
			{
				return (int)this[ActiveDirectoryServerSchema.MaxConcurrentMailboxSubmissions];
			}
			set
			{
				this[ActiveDirectoryServerSchema.MaxConcurrentMailboxSubmissions] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int PoisonThreshold
		{
			get
			{
				return (int)this[ServerSchema.PoisonThreshold];
			}
			set
			{
				this[ServerSchema.PoisonThreshold] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath MessageTrackingLogPath
		{
			get
			{
				return (LocalLongFullPath)this[ServerSchema.MessageTrackingLogPath];
			}
			set
			{
				this[ServerSchema.MessageTrackingLogPath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan MessageTrackingLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[ServerSchema.MessageTrackingLogMaxAge];
			}
			set
			{
				this[ServerSchema.MessageTrackingLogMaxAge] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> MessageTrackingLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.MessageTrackingLogMaxDirectorySize];
			}
			set
			{
				this[ServerSchema.MessageTrackingLogMaxDirectorySize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize MessageTrackingLogMaxFileSize
		{
			get
			{
				return (ByteQuantifiedSize)this[ServerSchema.MessageTrackingLogMaxFileSize];
			}
			set
			{
				this[ServerSchema.MessageTrackingLogMaxFileSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string MigrationLogExtensionData
		{
			get
			{
				return (string)this[ActiveDirectoryServerSchema.MigrationLogExtensionData];
			}
			set
			{
				this[ActiveDirectoryServerSchema.MigrationLogExtensionData] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MigrationEventType MigrationLogLoggingLevel
		{
			get
			{
				return (MigrationEventType)this[ActiveDirectoryServerSchema.MigrationLogLoggingLevel];
			}
			set
			{
				this[ActiveDirectoryServerSchema.MigrationLogLoggingLevel] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath MigrationLogFilePath
		{
			get
			{
				return (LocalLongFullPath)this[ActiveDirectoryServerSchema.MigrationLogFilePath];
			}
			set
			{
				this[ActiveDirectoryServerSchema.MigrationLogFilePath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan MigrationLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[ActiveDirectoryServerSchema.MigrationLogMaxAge];
			}
			set
			{
				this[ActiveDirectoryServerSchema.MigrationLogMaxAge] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize MigrationLogMaxDirectorySize
		{
			get
			{
				return (ByteQuantifiedSize)this[ActiveDirectoryServerSchema.MigrationLogMaxDirectorySize];
			}
			set
			{
				this[ActiveDirectoryServerSchema.MigrationLogMaxDirectorySize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize MigrationLogMaxFileSize
		{
			get
			{
				return (ByteQuantifiedSize)this[ActiveDirectoryServerSchema.MigrationLogMaxFileSize];
			}
			set
			{
				this[ActiveDirectoryServerSchema.MigrationLogMaxFileSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath IrmLogPath
		{
			get
			{
				return (LocalLongFullPath)this[ServerSchema.IrmLogPath];
			}
			set
			{
				this[ServerSchema.IrmLogPath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan IrmLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[ServerSchema.IrmLogMaxAge];
			}
			set
			{
				this[ServerSchema.IrmLogMaxAge] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> IrmLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.IrmLogMaxDirectorySize];
			}
			set
			{
				this[ServerSchema.IrmLogMaxDirectorySize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize IrmLogMaxFileSize
		{
			get
			{
				return (ByteQuantifiedSize)this[ServerSchema.IrmLogMaxFileSize];
			}
			set
			{
				this[ServerSchema.IrmLogMaxFileSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan ActiveUserStatisticsLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[ServerSchema.ActiveUserStatisticsLogMaxAge];
			}
			set
			{
				this[ServerSchema.ActiveUserStatisticsLogMaxAge] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize ActiveUserStatisticsLogMaxDirectorySize
		{
			get
			{
				return (ByteQuantifiedSize)this[ServerSchema.ActiveUserStatisticsLogMaxDirectorySize];
			}
			set
			{
				this[ServerSchema.ActiveUserStatisticsLogMaxDirectorySize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize ActiveUserStatisticsLogMaxFileSize
		{
			get
			{
				return (ByteQuantifiedSize)this[ServerSchema.ActiveUserStatisticsLogMaxFileSize];
			}
			set
			{
				this[ServerSchema.ActiveUserStatisticsLogMaxFileSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath ActiveUserStatisticsLogPath
		{
			get
			{
				return (LocalLongFullPath)this[ServerSchema.ActiveUserStatisticsLogPath];
			}
			set
			{
				this[ServerSchema.ActiveUserStatisticsLogPath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan ServerStatisticsLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[ServerSchema.ServerStatisticsLogMaxAge];
			}
			set
			{
				this[ServerSchema.ServerStatisticsLogMaxAge] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize ServerStatisticsLogMaxDirectorySize
		{
			get
			{
				return (ByteQuantifiedSize)this[ServerSchema.ServerStatisticsLogMaxDirectorySize];
			}
			set
			{
				this[ServerSchema.ServerStatisticsLogMaxDirectorySize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize ServerStatisticsLogMaxFileSize
		{
			get
			{
				return (ByteQuantifiedSize)this[ServerSchema.ServerStatisticsLogMaxFileSize];
			}
			set
			{
				this[ServerSchema.ServerStatisticsLogMaxFileSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath ServerStatisticsLogPath
		{
			get
			{
				return (LocalLongFullPath)this[ServerSchema.ServerStatisticsLogPath];
			}
			set
			{
				this[ServerSchema.ServerStatisticsLogPath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ConnectivityLogEnabled
		{
			get
			{
				return (bool)this[ServerSchema.ConnectivityLogEnabled];
			}
			set
			{
				this[ServerSchema.ConnectivityLogEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath ConnectivityLogPath
		{
			get
			{
				return (LocalLongFullPath)this[ServerSchema.ConnectivityLogPath];
			}
			set
			{
				this[ServerSchema.ConnectivityLogPath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan ConnectivityLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[ServerSchema.ConnectivityLogMaxAge];
			}
			set
			{
				this[ServerSchema.ConnectivityLogMaxAge] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> ConnectivityLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.ConnectivityLogMaxDirectorySize];
			}
			set
			{
				this[ServerSchema.ConnectivityLogMaxDirectorySize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> ConnectivityLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.ConnectivityLogMaxFileSize];
			}
			set
			{
				this[ServerSchema.ConnectivityLogMaxFileSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath PickupDirectoryPath
		{
			get
			{
				return (LocalLongFullPath)this[ServerSchema.PickupDirectoryPath];
			}
			set
			{
				this[ServerSchema.PickupDirectoryPath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath ReplayDirectoryPath
		{
			get
			{
				return (LocalLongFullPath)this[ServerSchema.ReplayDirectoryPath];
			}
			set
			{
				this[ServerSchema.ReplayDirectoryPath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int PickupDirectoryMaxMessagesPerMinute
		{
			get
			{
				return (int)this[ServerSchema.PickupDirectoryMaxMessagesPerMinute];
			}
			set
			{
				this[ServerSchema.PickupDirectoryMaxMessagesPerMinute] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize PickupDirectoryMaxHeaderSize
		{
			get
			{
				return (ByteQuantifiedSize)this[ServerSchema.PickupDirectoryMaxHeaderSize];
			}
			set
			{
				this[ServerSchema.PickupDirectoryMaxHeaderSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int PickupDirectoryMaxRecipientsPerMessage
		{
			get
			{
				return (int)this[ServerSchema.PickupDirectoryMaxRecipientsPerMessage];
			}
			set
			{
				this[ServerSchema.PickupDirectoryMaxRecipientsPerMessage] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan RoutingTableLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[ServerSchema.RoutingTableLogMaxAge];
			}
			set
			{
				this[ServerSchema.RoutingTableLogMaxAge] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> RoutingTableLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.RoutingTableLogMaxDirectorySize];
			}
			set
			{
				this[ServerSchema.RoutingTableLogMaxDirectorySize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath RoutingTableLogPath
		{
			get
			{
				return (LocalLongFullPath)this[ServerSchema.RoutingTableLogPath];
			}
			set
			{
				this[ServerSchema.RoutingTableLogPath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ProtocolLoggingLevel IntraOrgConnectorProtocolLoggingLevel
		{
			get
			{
				return (ProtocolLoggingLevel)this[ServerSchema.IntraOrgConnectorProtocolLoggingLevel];
			}
			set
			{
				this[ServerSchema.IntraOrgConnectorProtocolLoggingLevel] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ProtocolLoggingLevel InMemoryReceiveConnectorProtocolLoggingLevel
		{
			get
			{
				return (ProtocolLoggingLevel)this[ServerSchema.InMemoryReceiveConnectorProtocolLoggingLevel];
			}
			set
			{
				this[ServerSchema.InMemoryReceiveConnectorProtocolLoggingLevel] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool InMemoryReceiveConnectorSmtpUtf8Enabled
		{
			get
			{
				return (bool)this[ServerSchema.InMemoryReceiveConnectorSmtpUtf8Enabled];
			}
			set
			{
				this[ServerSchema.InMemoryReceiveConnectorSmtpUtf8Enabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool MessageTrackingLogEnabled
		{
			get
			{
				return (bool)this[ServerSchema.MessageTrackingLogEnabled];
			}
			set
			{
				this[ServerSchema.MessageTrackingLogEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool MessageTrackingLogSubjectLoggingEnabled
		{
			get
			{
				return (bool)this[ServerSchema.MessageTrackingLogSubjectLoggingEnabled];
			}
			set
			{
				this[ServerSchema.MessageTrackingLogSubjectLoggingEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IrmLogEnabled
		{
			get
			{
				return (bool)this[ServerSchema.IrmLogEnabled];
			}
			set
			{
				this[ServerSchema.IrmLogEnabled] = value;
			}
		}

		public bool PipelineTracingEnabled
		{
			get
			{
				return (bool)this[ServerSchema.PipelineTracingEnabled];
			}
			internal set
			{
				this[ServerSchema.PipelineTracingEnabled] = value;
			}
		}

		public bool ContentConversionTracingEnabled
		{
			get
			{
				return (bool)this[ServerSchema.ContentConversionTracingEnabled];
			}
			internal set
			{
				this[ServerSchema.ContentConversionTracingEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool GatewayEdgeSyncSubscribed
		{
			get
			{
				return (bool)this[ServerSchema.GatewayEdgeSyncSubscribed];
			}
			set
			{
				this[ServerSchema.GatewayEdgeSyncSubscribed] = value;
			}
		}

		public bool AntispamUpdatesEnabled
		{
			get
			{
				return (bool)this[ServerSchema.AntispamUpdatesEnabled];
			}
			internal set
			{
				this[ServerSchema.AntispamUpdatesEnabled] = value;
			}
		}

		public LocalLongFullPath PipelineTracingPath
		{
			get
			{
				return (LocalLongFullPath)this[ServerSchema.PipelineTracingPath];
			}
			internal set
			{
				this[ServerSchema.PipelineTracingPath] = value;
			}
		}

		public SmtpAddress? PipelineTracingSenderAddress
		{
			get
			{
				return (SmtpAddress?)this[ServerSchema.PipelineTracingSenderAddress];
			}
			internal set
			{
				this[ServerSchema.PipelineTracingSenderAddress] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool PoisonMessageDetectionEnabled
		{
			get
			{
				return (bool)this[ServerSchema.PoisonMessageDetectionEnabled];
			}
			set
			{
				this[ServerSchema.PoisonMessageDetectionEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AntispamAgentsEnabled
		{
			get
			{
				return (bool)this[ServerSchema.AntispamAgentsEnabled];
			}
			set
			{
				this[ServerSchema.AntispamAgentsEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RecipientValidationCacheEnabled
		{
			get
			{
				return (bool)this[ServerSchema.RecipientValidationCacheEnabled];
			}
			set
			{
				this[ServerSchema.RecipientValidationCacheEnabled] = value;
			}
		}

		internal override SystemFlagsEnum SystemFlags
		{
			get
			{
				return (SystemFlagsEnum)this[ServerSchema.SystemFlags];
			}
		}

		[Parameter(Mandatory = false)]
		public string RootDropDirectoryPath
		{
			get
			{
				return (string)this[ServerSchema.RootDropDirectoryPath];
			}
			set
			{
				this[ServerSchema.RootDropDirectoryPath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? MaxCallsAllowed
		{
			get
			{
				return (int?)this[ServerSchema.MaxCallsAllowed];
			}
			set
			{
				this[ServerSchema.MaxCallsAllowed] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ServerStatus Status
		{
			get
			{
				return (ServerStatus)this[ServerSchema.Status];
			}
			set
			{
				this[ServerSchema.Status] = value;
			}
		}

		public MultiValuedProperty<UMLanguage> Languages
		{
			get
			{
				return (MultiValuedProperty<UMLanguage>)this[ServerSchema.Languages];
			}
			internal set
			{
				this[ServerSchema.Languages] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> DialPlans
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ServerSchema.DialPlans];
			}
			set
			{
				this[ServerSchema.DialPlans] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ScheduleInterval[] GrammarGenerationSchedule
		{
			get
			{
				return (ScheduleInterval[])this[ServerSchema.GrammarGenerationSchedule];
			}
			set
			{
				this[ServerSchema.GrammarGenerationSchedule] = value;
			}
		}

		public UMSmartHost ExternalHostFqdn
		{
			get
			{
				return (UMSmartHost)this[ServerSchema.ExternalHostFqdn];
			}
			internal set
			{
				this[ServerSchema.ExternalHostFqdn] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> SubmissionServerOverrideList
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ServerSchema.SubmissionServerOverrideList];
			}
			set
			{
				this[ServerSchema.SubmissionServerOverrideList] = value;
			}
		}

		public bool UseCustomReferralServerList
		{
			get
			{
				return (bool)this[ServerSchema.FolderAffinityCustom];
			}
			internal set
			{
				this[ServerSchema.FolderAffinityCustom] = value;
			}
		}

		public MultiValuedProperty<ServerCostPair> CustomReferralServerList
		{
			get
			{
				return (MultiValuedProperty<ServerCostPair>)this[ServerSchema.FolderAffinityList];
			}
			internal set
			{
				this[ServerSchema.FolderAffinityList] = value;
			}
		}

		public MultiValuedProperty<CultureInfo> Locale
		{
			get
			{
				return (MultiValuedProperty<CultureInfo>)this[ServerSchema.Locale];
			}
			internal set
			{
				this[ServerSchema.Locale] = value;
			}
		}

		public bool? ErrorReportingEnabled
		{
			get
			{
				return (bool?)this[ServerSchema.ErrorReportingEnabled];
			}
			internal set
			{
				this[ServerSchema.ErrorReportingEnabled] = value;
			}
		}

		public MultiValuedProperty<string> StaticDomainControllers
		{
			get
			{
				return (MultiValuedProperty<string>)this[ServerSchema.StaticDomainControllers];
			}
			internal set
			{
				this[ServerSchema.StaticDomainControllers] = value;
			}
		}

		public MultiValuedProperty<string> StaticGlobalCatalogs
		{
			get
			{
				return (MultiValuedProperty<string>)this[ServerSchema.StaticGlobalCatalogs];
			}
			internal set
			{
				this[ServerSchema.StaticGlobalCatalogs] = value;
			}
		}

		public string StaticConfigDomainController
		{
			get
			{
				return (string)this[ServerSchema.StaticConfigDomainController];
			}
			internal set
			{
				this[ServerSchema.StaticConfigDomainController] = value;
			}
		}

		public MultiValuedProperty<string> StaticExcludedDomainControllers
		{
			get
			{
				return (MultiValuedProperty<string>)this[ServerSchema.StaticExcludedDomainControllers];
			}
			internal set
			{
				this[ServerSchema.StaticExcludedDomainControllers] = value;
			}
		}

		public MultiValuedProperty<string> CurrentDomainControllers
		{
			get
			{
				return (MultiValuedProperty<string>)this[ServerSchema.CurrentDomainControllers];
			}
			internal set
			{
				this[ServerSchema.CurrentDomainControllers] = value;
			}
		}

		public MultiValuedProperty<string> CurrentGlobalCatalogs
		{
			get
			{
				return (MultiValuedProperty<string>)this[ServerSchema.CurrentGlobalCatalogs];
			}
			internal set
			{
				this[ServerSchema.CurrentGlobalCatalogs] = value;
			}
		}

		public string CurrentConfigDomainController
		{
			get
			{
				return (string)this[ServerSchema.CurrentConfigDomainController];
			}
			internal set
			{
				this[ServerSchema.CurrentConfigDomainController] = value;
			}
		}

		public ADObjectId ServerSite
		{
			get
			{
				return (ADObjectId)this[ServerSchema.ServerSite];
			}
			internal set
			{
				this[ServerSchema.ServerSite] = value;
			}
		}

		public ADObjectId[] Databases
		{
			get
			{
				lock (this)
				{
					if (this.databases == null)
					{
						Database[] array = this.GetDatabases();
						if (array != null)
						{
							List<ADObjectId> list = new List<ADObjectId>(array.Length);
							foreach (Database database in array)
							{
								if (database != null)
								{
									list.Add(database.Id);
								}
							}
							this.databases = list.ToArray();
						}
					}
				}
				return this.databases;
			}
		}

		public int? MaximumActiveDatabases
		{
			get
			{
				return (int?)this[ServerSchema.MaxActiveMailboxDatabases];
			}
			internal set
			{
				this[ServerSchema.MaxActiveMailboxDatabases] = value;
			}
		}

		public int? MaximumPreferredActiveDatabases
		{
			get
			{
				return (int?)this[ServerSchema.MaxPreferredActiveDatabases];
			}
			internal set
			{
				this[ServerSchema.MaxPreferredActiveDatabases] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public AutoDatabaseMountDial AutoDatabaseMountDial
		{
			get
			{
				return (AutoDatabaseMountDial)this[ActiveDirectoryServerSchema.AutoDatabaseMountDialType];
			}
			set
			{
				this[ActiveDirectoryServerSchema.AutoDatabaseMountDialType] = value;
			}
		}

		public ADObjectId DatabaseAvailabilityGroup
		{
			get
			{
				return (ADObjectId)this[ServerSchema.DatabaseAvailabilityGroup];
			}
			internal set
			{
				this[ServerSchema.DatabaseAvailabilityGroup] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DatabaseCopyAutoActivationPolicyType DatabaseCopyAutoActivationPolicy
		{
			get
			{
				return (DatabaseCopyAutoActivationPolicyType)this[ActiveDirectoryServerSchema.DatabaseCopyAutoActivationPolicy];
			}
			set
			{
				this[ActiveDirectoryServerSchema.DatabaseCopyAutoActivationPolicy] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool DatabaseCopyActivationDisabledAndMoveNow
		{
			get
			{
				return (bool)this[ActiveDirectoryServerSchema.DatabaseCopyActivationDisabledAndMoveNow];
			}
			set
			{
				this[ActiveDirectoryServerSchema.DatabaseCopyActivationDisabledAndMoveNow] = value;
			}
		}

		internal ServerAutoDagFlags AutoDagFlags
		{
			get
			{
				return (ServerAutoDagFlags)this[ActiveDirectoryServerSchema.AutoDagFlags];
			}
			set
			{
				this[ActiveDirectoryServerSchema.AutoDagFlags] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string FaultZone
		{
			get
			{
				return (string)this[ActiveDirectoryServerSchema.FaultZone];
			}
			set
			{
				this[ActiveDirectoryServerSchema.FaultZone] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AutoDagServerConfigured
		{
			get
			{
				return (bool)this[ActiveDirectoryServerSchema.AutoDagServerConfigured];
			}
			set
			{
				this[ActiveDirectoryServerSchema.AutoDagServerConfigured] = value;
			}
		}

		public string ProductID
		{
			get
			{
				return (string)this[ServerSchema.ProductID];
			}
			internal set
			{
				this[ServerSchema.ProductID] = value;
			}
		}

		public bool IsExchangeTrialEdition
		{
			get
			{
				return (bool)this[ServerSchema.IsExchangeTrialEdition];
			}
		}

		public bool IsExpiredExchangeTrialEdition
		{
			get
			{
				return (bool)this[ServerSchema.IsExpiredExchangeTrialEdition];
			}
		}

		public EnhancedTimeSpan RemainingTrialPeriod
		{
			get
			{
				return (EnhancedTimeSpan)this[ServerSchema.RemainingTrialPeriod];
			}
		}

		[Parameter(Mandatory = false)]
		public bool TransportSyncEnabled
		{
			get
			{
				return (bool)this[ServerSchema.TransportSyncEnabled];
			}
			set
			{
				this[ServerSchema.TransportSyncEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool TransportSyncPopEnabled
		{
			get
			{
				return (bool)this[ServerSchema.TransportSyncPopEnabled];
			}
			set
			{
				this[ServerSchema.TransportSyncPopEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool WindowsLiveHotmailTransportSyncEnabled
		{
			get
			{
				return (bool)this[ServerSchema.WindowsLiveHotmailTransportSyncEnabled];
			}
			set
			{
				this[ServerSchema.WindowsLiveHotmailTransportSyncEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool TransportSyncExchangeEnabled
		{
			get
			{
				return (bool)this[ServerSchema.TransportSyncExchangeEnabled];
			}
			set
			{
				this[ServerSchema.TransportSyncExchangeEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool TransportSyncImapEnabled
		{
			get
			{
				return (bool)this[ServerSchema.TransportSyncImapEnabled];
			}
			set
			{
				this[ServerSchema.TransportSyncImapEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool TransportSyncFacebookEnabled
		{
			get
			{
				return (bool)this[ServerSchema.TransportSyncFacebookEnabled];
			}
			set
			{
				this[ServerSchema.TransportSyncFacebookEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool TransportSyncDispatchEnabled
		{
			get
			{
				return (bool)this[ServerSchema.TransportSyncDispatchEnabled];
			}
			set
			{
				this[ServerSchema.TransportSyncDispatchEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool TransportSyncLinkedInEnabled
		{
			get
			{
				return (bool)this[ServerSchema.TransportSyncLinkedInEnabled];
			}
			set
			{
				this[ServerSchema.TransportSyncLinkedInEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MaxNumberOfTransportSyncAttempts
		{
			get
			{
				return (int)this[ServerSchema.MaxNumberOfTransportSyncAttempts];
			}
			set
			{
				this[ServerSchema.MaxNumberOfTransportSyncAttempts] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MaxAcceptedTransportSyncJobsPerProcessor
		{
			get
			{
				return (int)this[ServerSchema.MaxAcceptedTransportSyncJobsPerProcessor];
			}
			set
			{
				this[ServerSchema.MaxAcceptedTransportSyncJobsPerProcessor] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MaxActiveTransportSyncJobsPerProcessor
		{
			get
			{
				return (int)this[ServerSchema.MaxActiveTransportSyncJobsPerProcessor];
			}
			set
			{
				this[ServerSchema.MaxActiveTransportSyncJobsPerProcessor] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string HttpTransportSyncProxyServer
		{
			get
			{
				return (string)this[ServerSchema.HttpTransportSyncProxyServer];
			}
			set
			{
				this[ServerSchema.HttpTransportSyncProxyServer] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool HttpProtocolLogEnabled
		{
			get
			{
				return (bool)this[ServerSchema.HttpProtocolLogEnabled];
			}
			set
			{
				this[ServerSchema.HttpProtocolLogEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath HttpProtocolLogFilePath
		{
			get
			{
				return (LocalLongFullPath)this[ServerSchema.HttpProtocolLogFilePath];
			}
			set
			{
				this[ServerSchema.HttpProtocolLogFilePath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan HttpProtocolLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[ServerSchema.HttpProtocolLogMaxAge];
			}
			set
			{
				this[ServerSchema.HttpProtocolLogMaxAge] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize HttpProtocolLogMaxDirectorySize
		{
			get
			{
				return (ByteQuantifiedSize)this[ServerSchema.HttpProtocolLogMaxDirectorySize];
			}
			set
			{
				this[ServerSchema.HttpProtocolLogMaxDirectorySize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize HttpProtocolLogMaxFileSize
		{
			get
			{
				return (ByteQuantifiedSize)this[ServerSchema.HttpProtocolLogMaxFileSize];
			}
			set
			{
				this[ServerSchema.HttpProtocolLogMaxFileSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ProtocolLoggingLevel HttpProtocolLogLoggingLevel
		{
			get
			{
				return (ProtocolLoggingLevel)this[ServerSchema.HttpProtocolLogLoggingLevel];
			}
			set
			{
				this[ServerSchema.HttpProtocolLogLoggingLevel] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool TransportSyncLogEnabled
		{
			get
			{
				return (bool)this[ServerSchema.TransportSyncLogEnabled];
			}
			set
			{
				this[ServerSchema.TransportSyncLogEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SyncLoggingLevel TransportSyncLogLoggingLevel
		{
			get
			{
				return (SyncLoggingLevel)this[ServerSchema.TransportSyncLogLoggingLevel];
			}
			set
			{
				this[ServerSchema.TransportSyncLogLoggingLevel] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath TransportSyncLogFilePath
		{
			get
			{
				return (LocalLongFullPath)this[ServerSchema.TransportSyncLogFilePath];
			}
			set
			{
				this[ServerSchema.TransportSyncLogFilePath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan TransportSyncLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[ServerSchema.TransportSyncLogMaxAge];
			}
			set
			{
				this[ServerSchema.TransportSyncLogMaxAge] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize TransportSyncLogMaxDirectorySize
		{
			get
			{
				return (ByteQuantifiedSize)this[ServerSchema.TransportSyncLogMaxDirectorySize];
			}
			set
			{
				this[ServerSchema.TransportSyncLogMaxDirectorySize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize TransportSyncLogMaxFileSize
		{
			get
			{
				return (ByteQuantifiedSize)this[ServerSchema.TransportSyncLogMaxFileSize];
			}
			set
			{
				this[ServerSchema.TransportSyncLogMaxFileSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool TransportSyncHubHealthLogEnabled
		{
			get
			{
				return (bool)this[ActiveDirectoryServerSchema.TransportSyncHubHealthLogEnabled];
			}
			set
			{
				this[ActiveDirectoryServerSchema.TransportSyncHubHealthLogEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath TransportSyncHubHealthLogFilePath
		{
			get
			{
				return (LocalLongFullPath)this[ActiveDirectoryServerSchema.TransportSyncHubHealthLogFilePath];
			}
			set
			{
				this[ActiveDirectoryServerSchema.TransportSyncHubHealthLogFilePath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan TransportSyncHubHealthLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[ActiveDirectoryServerSchema.TransportSyncHubHealthLogMaxAge];
			}
			set
			{
				this[ActiveDirectoryServerSchema.TransportSyncHubHealthLogMaxAge] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize TransportSyncHubHealthLogMaxDirectorySize
		{
			get
			{
				return (ByteQuantifiedSize)this[ActiveDirectoryServerSchema.TransportSyncHubHealthLogMaxDirectorySize];
			}
			set
			{
				this[ActiveDirectoryServerSchema.TransportSyncHubHealthLogMaxDirectorySize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize TransportSyncHubHealthLogMaxFileSize
		{
			get
			{
				return (ByteQuantifiedSize)this[ActiveDirectoryServerSchema.TransportSyncHubHealthLogMaxFileSize];
			}
			set
			{
				this[ActiveDirectoryServerSchema.TransportSyncHubHealthLogMaxFileSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool TransportSyncAccountsPoisonDetectionEnabled
		{
			get
			{
				return (bool)this[ServerSchema.TransportSyncAccountsPoisonDetectionEnabled];
			}
			set
			{
				this[ServerSchema.TransportSyncAccountsPoisonDetectionEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int TransportSyncAccountsPoisonAccountThreshold
		{
			get
			{
				return (int)this[ServerSchema.TransportSyncAccountsPoisonAccountThreshold];
			}
			set
			{
				this[ServerSchema.TransportSyncAccountsPoisonAccountThreshold] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int TransportSyncAccountsPoisonItemThreshold
		{
			get
			{
				return (int)this[ServerSchema.TransportSyncAccountsPoisonItemThreshold];
			}
			set
			{
				this[ServerSchema.TransportSyncAccountsPoisonItemThreshold] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int TransportSyncAccountsSuccessivePoisonItemThreshold
		{
			get
			{
				return (int)this[ActiveDirectoryServerSchema.TransportSyncAccountsSuccessivePoisonItemThreshold];
			}
			set
			{
				this[ActiveDirectoryServerSchema.TransportSyncAccountsSuccessivePoisonItemThreshold] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan TransportSyncRemoteConnectionTimeout
		{
			get
			{
				return (EnhancedTimeSpan)this[ServerSchema.TransportSyncRemoteConnectionTimeout];
			}
			set
			{
				this[ServerSchema.TransportSyncRemoteConnectionTimeout] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize TransportSyncMaxDownloadSizePerItem
		{
			get
			{
				return (ByteQuantifiedSize)this[ServerSchema.TransportSyncMaxDownloadSizePerItem];
			}
			set
			{
				this[ServerSchema.TransportSyncMaxDownloadSizePerItem] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize TransportSyncMaxDownloadSizePerConnection
		{
			get
			{
				return (ByteQuantifiedSize)this[ServerSchema.TransportSyncMaxDownloadSizePerConnection];
			}
			set
			{
				this[ServerSchema.TransportSyncMaxDownloadSizePerConnection] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int TransportSyncMaxDownloadItemsPerConnection
		{
			get
			{
				return (int)this[ServerSchema.TransportSyncMaxDownloadItemsPerConnection];
			}
			set
			{
				this[ServerSchema.TransportSyncMaxDownloadItemsPerConnection] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string DeltaSyncClientCertificateThumbprint
		{
			get
			{
				return (string)this[ServerSchema.DeltaSyncClientCertificateThumbprint];
			}
			set
			{
				this[ServerSchema.DeltaSyncClientCertificateThumbprint] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MaxTransportSyncDispatchers
		{
			get
			{
				return (int)this[ServerSchema.MaxTransportSyncDispatchers];
			}
			set
			{
				this[ServerSchema.MaxTransportSyncDispatchers] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool TransportSyncMailboxLogEnabled
		{
			get
			{
				return (bool)this[ServerSchema.TransportSyncMailboxLogEnabled];
			}
			set
			{
				this[ServerSchema.TransportSyncMailboxLogEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SyncLoggingLevel TransportSyncMailboxLogLoggingLevel
		{
			get
			{
				return (SyncLoggingLevel)this[ServerSchema.TransportSyncMailboxLogLoggingLevel];
			}
			set
			{
				this[ServerSchema.TransportSyncMailboxLogLoggingLevel] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath TransportSyncMailboxLogFilePath
		{
			get
			{
				return (LocalLongFullPath)this[ServerSchema.TransportSyncMailboxLogFilePath];
			}
			set
			{
				this[ServerSchema.TransportSyncMailboxLogFilePath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan TransportSyncMailboxLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[ServerSchema.TransportSyncMailboxLogMaxAge];
			}
			set
			{
				this[ServerSchema.TransportSyncMailboxLogMaxAge] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize TransportSyncMailboxLogMaxDirectorySize
		{
			get
			{
				return (ByteQuantifiedSize)this[ServerSchema.TransportSyncMailboxLogMaxDirectorySize];
			}
			set
			{
				this[ServerSchema.TransportSyncMailboxLogMaxDirectorySize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize TransportSyncMailboxLogMaxFileSize
		{
			get
			{
				return (ByteQuantifiedSize)this[ServerSchema.TransportSyncMailboxLogMaxFileSize];
			}
			set
			{
				this[ServerSchema.TransportSyncMailboxLogMaxFileSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool TransportSyncMailboxHealthLogEnabled
		{
			get
			{
				return (bool)this[ActiveDirectoryServerSchema.TransportSyncMailboxHealthLogEnabled];
			}
			set
			{
				this[ActiveDirectoryServerSchema.TransportSyncMailboxHealthLogEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath TransportSyncMailboxHealthLogFilePath
		{
			get
			{
				return (LocalLongFullPath)this[ActiveDirectoryServerSchema.TransportSyncMailboxHealthLogFilePath];
			}
			set
			{
				this[ActiveDirectoryServerSchema.TransportSyncMailboxHealthLogFilePath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan TransportSyncMailboxHealthLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[ActiveDirectoryServerSchema.TransportSyncMailboxHealthLogMaxAge];
			}
			set
			{
				this[ActiveDirectoryServerSchema.TransportSyncMailboxHealthLogMaxAge] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize TransportSyncMailboxHealthLogMaxDirectorySize
		{
			get
			{
				return (ByteQuantifiedSize)this[ActiveDirectoryServerSchema.TransportSyncMailboxHealthLogMaxDirectorySize];
			}
			set
			{
				this[ActiveDirectoryServerSchema.TransportSyncMailboxHealthLogMaxDirectorySize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize TransportSyncMailboxHealthLogMaxFileSize
		{
			get
			{
				return (ByteQuantifiedSize)this[ActiveDirectoryServerSchema.TransportSyncMailboxHealthLogMaxFileSize];
			}
			set
			{
				this[ActiveDirectoryServerSchema.TransportSyncMailboxHealthLogMaxFileSize] = value;
			}
		}

		public MailboxRelease MailboxRelease
		{
			get
			{
				MailboxRelease result;
				if (!Enum.TryParse<MailboxRelease>((string)this[ActiveDirectoryServerSchema.MailboxRelease], true, out result))
				{
					return MailboxRelease.None;
				}
				return result;
			}
			set
			{
				this[ActiveDirectoryServerSchema.MailboxRelease] = value.ToString();
			}
		}

		public MailboxProvisioningAttributes MailboxProvisioningAttributes
		{
			get
			{
				return this[ServerSchema.MailboxProvisioningAttributes] as MailboxProvisioningAttributes;
			}
			set
			{
				this[ServerSchema.MailboxProvisioningAttributes] = value;
			}
		}

		internal long? ContinuousReplicationMaxMemoryPerDatabase
		{
			get
			{
				return (long?)this[ActiveDirectoryServerSchema.ContinuousReplicationMaxMemoryPerDatabase];
			}
		}

		internal MailboxDatabase[] GetMailboxDatabases()
		{
			return this.GetDatabases<MailboxDatabase>();
		}

		public bool UseDowngradedExchangeServerAuth
		{
			get
			{
				return (bool)this[ActiveDirectoryServerSchema.UseDowngradedExchangeServerAuth];
			}
		}

		internal bool IsFfoWebServiceRole
		{
			get
			{
				return (this.CurrentServerRole & ServerRole.FfoWebService) == ServerRole.FfoWebService;
			}
		}

		internal bool IsOSPRole
		{
			get
			{
				bool result;
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\ExchangeServer\\v15\\OspServerRole"))
				{
					result = (registryKey != null);
				}
				return result;
			}
		}

		public EnhancedTimeSpan QueueLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[ServerSchema.QueueLogMaxAge];
			}
			set
			{
				this[ServerSchema.QueueLogMaxAge] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> QueueLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.QueueLogMaxDirectorySize];
			}
			set
			{
				this[ServerSchema.QueueLogMaxDirectorySize] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> QueueLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.QueueLogMaxFileSize];
			}
			set
			{
				this[ServerSchema.QueueLogMaxFileSize] = value;
			}
		}

		public LocalLongFullPath QueueLogPath
		{
			get
			{
				return (LocalLongFullPath)this[ServerSchema.QueueLogPath];
			}
			set
			{
				this[ServerSchema.QueueLogPath] = value;
			}
		}

		public EnhancedTimeSpan WlmLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[ServerSchema.WlmLogMaxAge];
			}
			set
			{
				this[ServerSchema.WlmLogMaxAge] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> WlmLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.WlmLogMaxDirectorySize];
			}
			set
			{
				this[ServerSchema.WlmLogMaxDirectorySize] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> WlmLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.WlmLogMaxFileSize];
			}
			set
			{
				this[ServerSchema.WlmLogMaxFileSize] = value;
			}
		}

		public LocalLongFullPath WlmLogPath
		{
			get
			{
				return (LocalLongFullPath)this[ServerSchema.WlmLogPath];
			}
			set
			{
				this[ServerSchema.WlmLogPath] = value;
			}
		}

		public EnhancedTimeSpan AgentLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[ServerSchema.AgentLogMaxAge];
			}
			set
			{
				this[ServerSchema.AgentLogMaxAge] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> AgentLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.AgentLogMaxDirectorySize];
			}
			set
			{
				this[ServerSchema.AgentLogMaxDirectorySize] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> AgentLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.AgentLogMaxFileSize];
			}
			set
			{
				this[ServerSchema.AgentLogMaxFileSize] = value;
			}
		}

		public LocalLongFullPath AgentLogPath
		{
			get
			{
				return (LocalLongFullPath)this[ServerSchema.AgentLogPath];
			}
			set
			{
				this[ServerSchema.AgentLogPath] = value;
			}
		}

		public bool AgentLogEnabled
		{
			get
			{
				return (bool)this[ServerSchema.AgentLogEnabled];
			}
			set
			{
				this[ServerSchema.AgentLogEnabled] = value;
			}
		}

		public EnhancedTimeSpan AttributionLogMaxAge { get; set; }

		public Unlimited<ByteQuantifiedSize> AttributionLogMaxDirectorySize { get; set; }

		public Unlimited<ByteQuantifiedSize> AttributionLogMaxFileSize { get; set; }

		public LocalLongFullPath AttributionLogPath { get; set; }

		public bool AttributionLogEnabled { get; set; }

		public EnhancedTimeSpan FlowControlLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[ServerSchema.FlowControlLogMaxAge];
			}
			set
			{
				this[ServerSchema.FlowControlLogMaxAge] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> FlowControlLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.FlowControlLogMaxDirectorySize];
			}
			set
			{
				this[ServerSchema.FlowControlLogMaxDirectorySize] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> FlowControlLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.FlowControlLogMaxFileSize];
			}
			set
			{
				this[ServerSchema.FlowControlLogMaxFileSize] = value;
			}
		}

		public LocalLongFullPath FlowControlLogPath
		{
			get
			{
				return (LocalLongFullPath)this[ServerSchema.FlowControlLogPath];
			}
			set
			{
				this[ServerSchema.FlowControlLogPath] = value;
			}
		}

		public bool FlowControlLogEnabled
		{
			get
			{
				return (bool)this[ServerSchema.FlowControlLogEnabled];
			}
			set
			{
				this[ServerSchema.FlowControlLogEnabled] = value;
			}
		}

		public EnhancedTimeSpan ProcessingSchedulerLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[ServerSchema.ProcessingSchedulerLogMaxAge];
			}
			set
			{
				this[ServerSchema.ProcessingSchedulerLogMaxAge] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> ProcessingSchedulerLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.ProcessingSchedulerLogMaxDirectorySize];
			}
			set
			{
				this[ServerSchema.ProcessingSchedulerLogMaxDirectorySize] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> ProcessingSchedulerLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.ProcessingSchedulerLogMaxFileSize];
			}
			set
			{
				this[ServerSchema.ProcessingSchedulerLogMaxFileSize] = value;
			}
		}

		public LocalLongFullPath ProcessingSchedulerLogPath
		{
			get
			{
				return (LocalLongFullPath)this[ServerSchema.ProcessingSchedulerLogPath];
			}
			set
			{
				this[ServerSchema.ProcessingSchedulerLogPath] = value;
			}
		}

		public bool ProcessingSchedulerLogEnabled
		{
			get
			{
				return (bool)this[ServerSchema.ProcessingSchedulerLogEnabled];
			}
			set
			{
				this[ServerSchema.ProcessingSchedulerLogEnabled] = value;
			}
		}

		public EnhancedTimeSpan ResourceLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[ServerSchema.ResourceLogMaxAge];
			}
			set
			{
				this[ServerSchema.ResourceLogMaxAge] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> ResourceLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.ResourceLogMaxDirectorySize];
			}
			set
			{
				this[ServerSchema.ResourceLogMaxDirectorySize] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> ResourceLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.ResourceLogMaxFileSize];
			}
			set
			{
				this[ServerSchema.ResourceLogMaxFileSize] = value;
			}
		}

		public LocalLongFullPath ResourceLogPath
		{
			get
			{
				return (LocalLongFullPath)this[ServerSchema.ResourceLogPath];
			}
			set
			{
				this[ServerSchema.ResourceLogPath] = value;
			}
		}

		public bool ResourceLogEnabled
		{
			get
			{
				return (bool)this[ServerSchema.ResourceLogEnabled];
			}
			set
			{
				this[ServerSchema.ResourceLogEnabled] = value;
			}
		}

		public EnhancedTimeSpan DnsLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[ServerSchema.DnsLogMaxAge];
			}
			set
			{
				this[ServerSchema.DnsLogMaxAge] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> DnsLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.DnsLogMaxDirectorySize];
			}
			set
			{
				this[ServerSchema.DnsLogMaxDirectorySize] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> DnsLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.DnsLogMaxFileSize];
			}
			set
			{
				this[ServerSchema.DnsLogMaxFileSize] = value;
			}
		}

		public LocalLongFullPath DnsLogPath
		{
			get
			{
				return (LocalLongFullPath)this[ServerSchema.DnsLogPath];
			}
			set
			{
				this[ServerSchema.DnsLogPath] = value;
			}
		}

		public bool DnsLogEnabled
		{
			get
			{
				return (bool)this[ServerSchema.DnsLogEnabled];
			}
			set
			{
				this[ServerSchema.DnsLogEnabled] = value;
			}
		}

		public EnhancedTimeSpan JournalLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[ServerSchema.JournalLogMaxAge];
			}
			set
			{
				this[ServerSchema.JournalLogMaxAge] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> JournalLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.JournalLogMaxDirectorySize];
			}
			set
			{
				this[ServerSchema.JournalLogMaxDirectorySize] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> JournalLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.JournalLogMaxFileSize];
			}
			set
			{
				this[ServerSchema.JournalLogMaxFileSize] = value;
			}
		}

		public LocalLongFullPath JournalLogPath
		{
			get
			{
				return (LocalLongFullPath)this[ServerSchema.JournalLogPath];
			}
			set
			{
				this[ServerSchema.JournalLogPath] = value;
			}
		}

		public bool JournalLogEnabled
		{
			get
			{
				return (bool)this[ServerSchema.JournalLogEnabled];
			}
			set
			{
				this[ServerSchema.JournalLogEnabled] = value;
			}
		}

		public EnhancedTimeSpan TransportMaintenanceLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[ServerSchema.TransportMaintenanceLogMaxAge];
			}
			set
			{
				this[ServerSchema.TransportMaintenanceLogMaxAge] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> TransportMaintenanceLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.TransportMaintenanceLogMaxDirectorySize];
			}
			set
			{
				this[ServerSchema.TransportMaintenanceLogMaxDirectorySize] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> TransportMaintenanceLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.TransportMaintenanceLogMaxFileSize];
			}
			set
			{
				this[ServerSchema.TransportMaintenanceLogMaxFileSize] = value;
			}
		}

		public LocalLongFullPath TransportMaintenanceLogPath
		{
			get
			{
				return (LocalLongFullPath)this[ServerSchema.TransportMaintenanceLogPath];
			}
			set
			{
				this[ServerSchema.TransportMaintenanceLogPath] = value;
			}
		}

		public bool TransportMaintenanceLogEnabled
		{
			get
			{
				return (bool)this[ServerSchema.TransportMaintenanceLogEnabled];
			}
			set
			{
				this[ServerSchema.TransportMaintenanceLogEnabled] = value;
			}
		}

		public int MaxReceiveTlsRatePerMinute { get; set; }

		public EnhancedTimeSpan MailboxDeliveryAgentLogMaxAge { get; set; }

		public Unlimited<ByteQuantifiedSize> MailboxDeliveryAgentLogMaxDirectorySize { get; set; }

		public Unlimited<ByteQuantifiedSize> MailboxDeliveryAgentLogMaxFileSize { get; set; }

		public LocalLongFullPath MailboxDeliveryAgentLogPath { get; set; }

		public bool MailboxDeliveryAgentLogEnabled { get; set; }

		public EnhancedTimeSpan MailboxSubmissionAgentLogMaxAge { get; set; }

		public Unlimited<ByteQuantifiedSize> MailboxSubmissionAgentLogMaxDirectorySize { get; set; }

		public Unlimited<ByteQuantifiedSize> MailboxSubmissionAgentLogMaxFileSize { get; set; }

		public LocalLongFullPath MailboxSubmissionAgentLogPath { get; set; }

		public bool MailboxSubmissionAgentLogEnabled { get; set; }

		public bool MailboxDeliveryThrottlingLogEnabled { get; set; }

		public EnhancedTimeSpan MailboxDeliveryThrottlingLogMaxAge { get; set; }

		public Unlimited<ByteQuantifiedSize> MailboxDeliveryThrottlingLogMaxDirectorySize { get; set; }

		public Unlimited<ByteQuantifiedSize> MailboxDeliveryThrottlingLogMaxFileSize { get; set; }

		public LocalLongFullPath MailboxDeliveryThrottlingLogPath { get; set; }

		internal PublicFolderDatabase[] GetPublicFolderDatabases()
		{
			return this.GetPublicFolderDatabases(null);
		}

		internal PublicFolderDatabase[] GetMapiPublicFolderDatabases()
		{
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, PublicFolderTreeSchema.PublicFolderTreeType, PublicFolderTreeType.Mapi);
			return this.GetPublicFolderDatabases(filter);
		}

		private PublicFolderDatabase[] GetPublicFolderDatabases(QueryFilter filter)
		{
			List<PublicFolderDatabase> list = new List<PublicFolderDatabase>();
			PublicFolderDatabase[] array = this.GetDatabases<PublicFolderDatabase>();
			foreach (PublicFolderDatabase publicFolderDatabase in array)
			{
				PublicFolderTree[] array3 = base.Session.Find<PublicFolderTree>(publicFolderDatabase.PublicFolderHierarchy, QueryScope.SubTree, filter, null, 0);
				if (array3.Length > 0)
				{
					list.Add(publicFolderDatabase);
				}
			}
			return list.ToArray();
		}

		internal TDatabase[] GetDatabases<TDatabase>() where TDatabase : IConfigurable, new()
		{
			return this.GetDatabases<TDatabase>(false);
		}

		internal TDatabase[] GetDatabases<TDatabase>(bool allowInvalidCopies) where TDatabase : IConfigurable, new()
		{
			if (base.Session == null)
			{
				throw new InvalidOperationException("Server object does not have a session reference, so cannot get databases.");
			}
			List<TDatabase> list = new List<TDatabase>();
			if (this.IsE14OrLater)
			{
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, base.Name);
				DatabaseCopy[] array = base.Session.Find<DatabaseCopy>(null, QueryScope.SubTree, filter, null, 0);
				foreach (DatabaseCopy databaseCopy in array)
				{
					if (databaseCopy.IsValidDatabaseCopy(allowInvalidCopies))
					{
						TDatabase database = databaseCopy.GetDatabase<TDatabase>();
						if (database != null)
						{
							list.Add(database);
						}
					}
				}
			}
			else
			{
				list.AddRange(base.Session.FindPaged<TDatabase>(null, base.Id, true, null, 0));
			}
			return list.ToArray();
		}

		internal Database[] GetDatabases()
		{
			return this.GetDatabases(false);
		}

		internal Database[] GetDatabases(bool allowInvalidCopies)
		{
			return this.GetDatabases<Database>(allowInvalidCopies);
		}

		internal string GetDomainOrComputerName()
		{
			return Server.GetDomainOrComputerName(this.propertyBag);
		}

		internal string GetAcceptedDomainOrDomainOrComputerName()
		{
			AcceptedDomain defaultAcceptedDomain = base.Session.GetDefaultAcceptedDomain();
			if (defaultAcceptedDomain != null && !string.IsNullOrEmpty(defaultAcceptedDomain.DomainName.Domain))
			{
				return defaultAcceptedDomain.DomainName.Domain;
			}
			return this.GetDomainOrComputerName();
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if ((this.RetentionLogForManagedFoldersEnabled || this.JournalingLogForManagedFoldersEnabled || this.FolderLogForManagedFoldersEnabled) && this.LogPathForManagedFolders == null)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ElcAuditLogPathMissing, ActiveDirectoryServerSchema.ElcAuditLogPath, this));
			}
			string domain = this.Domain;
			if (this.EmptyDomainAllowed)
			{
				if (!string.IsNullOrEmpty(domain) && !SmtpAddress.IsValidDomain(domain))
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.InvalidDomain, ServerSchema.Domain, this));
				}
			}
			else if (!SmtpAddress.IsValidDomain(domain))
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.InvalidDomain, ServerSchema.Domain, this));
			}
			if (this.IsEdgeServer || this.IsHubTransportServer)
			{
				if (this.ReceiveProtocolLogMaxFileSize.CompareTo(this.ReceiveProtocolLogMaxDirectorySize) > 0)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.InvalidRcvProtocolLogSizeConfiguration, ServerSchema.ReceiveProtocolLogMaxFileSize, this));
				}
				if (this.SendProtocolLogMaxFileSize.CompareTo(this.SendProtocolLogMaxDirectorySize) > 0)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.InvalidSndProtocolLogSizeConfiguration, ServerSchema.SendProtocolLogMaxFileSize, this));
				}
				if (!this.MessageTrackingLogMaxDirectorySize.IsUnlimited && this.MessageTrackingLogMaxFileSize.CompareTo(this.MessageTrackingLogMaxDirectorySize.Value) > 0)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.InvalidMsgTrackingLogSizeConfiguration, ServerSchema.MessageTrackingLogMaxFileSize, this));
				}
				if (this.ActiveUserStatisticsLogMaxFileSize.CompareTo(this.ActiveUserStatisticsLogMaxDirectorySize) > 0)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.InvalidActiveUserStatisticsLogSizeConfiguration, ServerSchema.ActiveUserStatisticsLogMaxFileSize, this));
				}
				if (this.ServerStatisticsLogMaxFileSize.CompareTo(this.ServerStatisticsLogMaxDirectorySize) > 0)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.InvalidServerStatisticsLogSizeConfiguration, ServerSchema.ServerStatisticsLogMaxFileSize, this));
				}
				if (this.PickupDirectoryPath != null && this.ReplayDirectoryPath != null && this.PickupDirectoryPath.Equals(this.ReplayDirectoryPath))
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.InvalidDirectoryConfiguration, ServerSchema.PickupDirectoryPath, this));
				}
				if (!this.MaxOutboundConnections.IsUnlimited && (this.MaxPerDomainOutboundConnections.IsUnlimited || this.MaxPerDomainOutboundConnections.Value > this.MaxOutboundConnections.Value))
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.InvalidMaxOutboundConnectionConfiguration(this.MaxPerDomainOutboundConnections.ToString(), this.MaxOutboundConnections.ToString()), ServerSchema.MaxPerDomainOutboundConnections, this));
				}
				if (base.IsModified(ServerSchema.PipelineTracingEnabled) && this.PipelineTracingEnabled && (this.PipelineTracingSenderAddress == null || this.PipelineTracingSenderAddress.Equals(SmtpAddress.Empty) || null == this.PipelineTracingPath || string.IsNullOrEmpty(this.PipelineTracingPath.ToString())))
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.ErrorPipelineTracingRequirementsMissing, ServerSchema.PipelineTracingEnabled, this));
				}
				if (base.IsModified(ServerSchema.ContentConversionTracingEnabled) && this.ContentConversionTracingEnabled && (null == this.PipelineTracingPath || string.IsNullOrEmpty(this.PipelineTracingPath.ToString())))
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.ErrorPipelineTracingRequirementsMissing, ServerSchema.ContentConversionTracingEnabled, this));
				}
				if (!this.ExternalDNSAdapterEnabled && MultiValuedPropertyBase.IsNullOrEmpty(this.ExternalDNSServers))
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.ExternalDNSServersNotSet, ServerSchema.ExternalDNSServers, this));
				}
				if (!this.InternalDNSAdapterEnabled && MultiValuedPropertyBase.IsNullOrEmpty(this.InternalDNSServers))
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.InternalDNSServersNotSet, ServerSchema.InternalDNSServers, this));
				}
				if (this.HttpProtocolLogMaxFileSize.CompareTo(this.HttpProtocolLogMaxDirectorySize) > 0)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.InvalidHttpProtocolLogSizeConfiguration, ServerSchema.HttpProtocolLogMaxFileSize, this));
				}
				if (this.TransportSyncLogMaxFileSize.CompareTo(this.TransportSyncLogMaxDirectorySize) > 0)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.InvalidTransportSyncLogSizeConfiguration, ServerSchema.TransportSyncLogMaxFileSize, this));
				}
				if (this.TransportSyncMailboxLogMaxFileSize.CompareTo(this.TransportSyncMailboxLogMaxDirectorySize) > 0)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.InvalidTransportSyncLogSizeConfiguration, ServerSchema.TransportSyncMailboxLogMaxFileSize, this));
				}
				if (this.TransportSyncHubHealthLogMaxFileSize.CompareTo(this.TransportSyncHubHealthLogMaxDirectorySize) > 0)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.InvalidTransportSyncHealthLogSizeConfiguration, ActiveDirectoryServerSchema.TransportSyncHubHealthLogMaxFileSize, this));
				}
				if (this.TransportSyncMailboxHealthLogMaxFileSize.CompareTo(this.TransportSyncMailboxHealthLogMaxDirectorySize) > 0)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.InvalidTransportSyncHealthLogSizeConfiguration, ActiveDirectoryServerSchema.TransportSyncMailboxHealthLogMaxFileSize, this));
				}
				if (this.TransportSyncMaxDownloadSizePerItem.CompareTo(this.TransportSyncMaxDownloadSizePerConnection) > 0)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.InvalidTransportSyncDownloadSizeConfiguration, ServerSchema.TransportSyncMaxDownloadSizePerItem, this));
				}
			}
			if (this.SubmissionServerOverrideList != null && this.SubmissionServerOverrideList != MultiValuedProperty<ADObjectId>.Empty && this.SubmissionServerOverrideList.Added != null && this.SubmissionServerOverrideList.Added.Length != 0 && (this.CurrentServerRole & ServerRole.Mailbox) == ServerRole.None)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.SubmissionOverrideListOnWrongServer, ServerSchema.SubmissionServerOverrideList, this));
			}
		}

		internal static string GetParentLegacyDN(ITopologyConfigurationSession session)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}/cn=Configuration/cn=Servers", new object[]
			{
				session.GetAdministrativeGroup().LegacyExchangeDN
			});
		}

		internal static LegacyDN GetSystemAttendantLegacyDN(LegacyDN mailboxServerLegacyDN)
		{
			return mailboxServerLegacyDN.GetChildLegacyDN("cn", "Microsoft System Attendant");
		}

		internal static string GetSystemAttendantLegacyDN(string mailboxServerLegacyDN)
		{
			return Server.GetSystemAttendantLegacyDN(LegacyDN.Parse(mailboxServerLegacyDN)).ToString();
		}

		internal ServerRoleOperationException GetServerRoleError(ServerRole role)
		{
			return new ServerRoleOperationException(DirectoryStrings.ErrorServerRoleNotSupported(base.Name));
		}

		internal static bool IsSubscribedGateway(ITopologyConfigurationSession session)
		{
			if (TopologyProvider.IsAdamTopology())
			{
				Server server = session.ReadLocalServer();
				return server != null && server.GatewayEdgeSyncSubscribed;
			}
			return false;
		}

		internal static ServerVersion GetServerVersion(string serverName)
		{
			if (!string.IsNullOrEmpty(serverName))
			{
				return ProvisioningCache.Instance.TryAddAndGetGlobalDictionaryValue<ServerVersion, string>(CannedProvisioningCacheKeys.ServerAdminDisplayVersionCacheKey, serverName, delegate()
				{
					ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 11218, "GetServerVersion", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\server.cs");
					MiniServer miniServer = topologyConfigurationSession.FindMiniServerByName(serverName, null);
					if (miniServer != null)
					{
						return miniServer.AdminDisplayVersion;
					}
					return null;
				});
			}
			return null;
		}

		internal static ClientAccessArray GetLocalServerClientAccessArray()
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 11233, "GetLocalServerClientAccessArray", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\server.cs");
			Server server = topologyConfigurationSession.ReadLocalServer();
			if (server != null && server.IsCafeServer && server.IsE15OrLater)
			{
				ADObjectId adobjectId = (ADObjectId)server[ServerSchema.ClientAccessArray];
				if (adobjectId != null)
				{
					QueryFilter filter = QueryFilter.AndTogether(new QueryFilter[]
					{
						new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Id, adobjectId),
						QueryFilter.NotFilter(ClientAccessArray.PriorTo15ExchangeObjectVersionFilter)
					});
					return topologyConfigurationSession.FindUnique<ClientAccessArray>(null, QueryScope.SubTree, filter);
				}
			}
			return null;
		}

		internal bool HasExtendedRight(ClientSecurityContext clientSecurityContext, Guid extendedRightGuid)
		{
			SecurityDescriptor securityDescriptor = base.ReadSecurityDescriptorBlob();
			return securityDescriptor != null && clientSecurityContext.HasExtendedRightOnObject(securityDescriptor, extendedRightGuid);
		}

		internal SmtpAddress? ExternalPostmasterAddress
		{
			get
			{
				return (SmtpAddress?)this[ServerSchema.ExternalPostmasterAddress];
			}
		}

		internal const string ExchangeTransportConfigContainerADObjectName = "Transport Configuration";

		internal const string DefaultPostmasterAlias = "postmaster";

		private const string UnknownDomain = "unknowndomain";

		public const string DefaultFaultZone = "FaultZone1";

		public const int DefaultCalendarRepairLogMaxAge = 10;

		public const int DefaultMaxTransportSyncDispatchers = 5;

		public const bool DefaultTransportSyncDispatchEnabled = true;

		public const int DefaultTransportSyncLogMaxAge = 720;

		public const int DefaultTransportSyncLogMaxDirectorySizeInGB = 10;

		public const int DefaultTransportSyncMailboxLogMaxDirectorySizeInGB = 2;

		public const int DefaultTransportSyncLogMaxFileSize = 10240;

		public const int DefaultTransportSyncAccountsPoisonAccountThreshold = 2;

		public const int DefaultTransportSyncAccountsPoisonItemThreshold = 2;

		public const int DefaultTransportSyncAccountsSuccessivePoisonItemThreshold = 3;

		public const int DefaultIrmLogMaxFileSizeInMB = 10;

		public const string NameValidationRegexPattern = "^[^`~!@#&\\^\\(\\)\\+\\[\\]\\{\\}\\<\\>\\?=,:|./\\\\; ]+$";

		public const string NameValidationSpaceAllowedRegexPattern = "^[^`~!@#&\\^\\(\\)\\+\\[\\]\\{\\}\\<\\>\\?=,:|./\\\\;]*$";

		internal static string MostDerivedClass = "msExchExchangeServer";

		public static readonly int Exchange2011MajorVersion = 15;

		public static readonly int Exchange2009MajorVersion = 14;

		public static readonly int CurrentExchangeMajorVersion = Server.Exchange2011MajorVersion;

		public static readonly int Exchange2007MajorVersion = 8;

		public static readonly int Exchange2000MajorVersion = 6;

		public static readonly int E2007MinVersion = 1912602624;

		public static readonly int E2007SP2MinVersion = 1912733696;

		public static readonly int E14MinVersion = 1937768448;

		public static readonly int E14SP1MinVersion = 1937833984;

		public static readonly int E15MinVersion = 1941962752;

		public static readonly int E16MinVersion = 1946157056;

		public static readonly int CurrentProductMinimumVersion = new ServerVersion(15, 0, 0, 0).ToInt();

		public static readonly int NextProductMinimumVersion = new ServerVersion(16, 0, 0, 0).ToInt();

		public static readonly int E2k3MinVersion = 6500;

		public static readonly int E2k3SP1MinVersion = 7226;

		public static readonly int E2k3SP2MinVersion = 7638;

		public static readonly int E2k3SP3MinVersion = 7720;

		public static readonly EnhancedTimeSpan Exchange2007TrialEditionExpirationPeriod = EnhancedTimeSpan.FromDays(120.0);

		public static readonly EnhancedTimeSpan E15TrialEditionExpirationPeriod = EnhancedTimeSpan.FromDays(180.0);

		private ADObjectId[] databases;

		[NonSerialized]
		private ADObjectSchema schema;

		internal delegate EnhancedTimeSpan? GetConfigurationDelegate(Server.AssistantConfigurationEntry entry);

		internal delegate Server.AssistantConfigurationEntry UpdateEntryDelegate(Server.AssistantConfigurationEntry entry, EnhancedTimeSpan value);

		internal class AssistantConfigurationEntry
		{
			public EnhancedTimeSpan WorkCycle { get; set; }

			public EnhancedTimeSpan WorkCycleCheckpoint { get; set; }

			public AssistantConfigurationEntry(TimeBasedAssistantIndex index, EnhancedTimeSpan workCycle, EnhancedTimeSpan workCycleCheckpoint)
			{
				this.index = index;
				this.WorkCycle = workCycle;
				this.WorkCycleCheckpoint = workCycleCheckpoint;
			}

			public static Server.AssistantConfigurationEntry GetConfigurationForAssistant(MultiValuedProperty<string> allConfigurations, TimeBasedAssistantIndex assistantIndex)
			{
				string value = string.Format("{0},", (int)assistantIndex);
				foreach (string text in allConfigurations)
				{
					if (text.StartsWith(value))
					{
						string[] array = text.Split(new char[]
						{
							','
						});
						if (array != null && array.Length >= 3)
						{
							EnhancedTimeSpan zero;
							if (!EnhancedTimeSpan.TryParse(array[1], out zero))
							{
								zero = EnhancedTimeSpan.Zero;
							}
							EnhancedTimeSpan zero2;
							if (!EnhancedTimeSpan.TryParse(array[2], out zero2))
							{
								zero2 = EnhancedTimeSpan.Zero;
							}
							return new Server.AssistantConfigurationEntry(assistantIndex, zero, zero2);
						}
					}
				}
				return null;
			}

			public static bool IsAssistantConfiguration(string configuration, TimeBasedAssistantIndex assistantIndex)
			{
				return configuration.StartsWith(string.Format("{0},", (int)assistantIndex));
			}

			public override string ToString()
			{
				return string.Format("{0},{1},{2}", (int)this.index, this.WorkCycle, this.WorkCycleCheckpoint);
			}

			private const string Format = "{0},{1},{2}";

			private const int AssistantIndex = 0;

			private const int WorkCycleIndex = 1;

			private const int CheckpointIndex = 2;

			private TimeBasedAssistantIndex index;
		}

		private class MaintenanceScheduleEntry
		{
			public MaintenanceScheduleEntry(ScheduledAssistant assistant, ScheduleInterval[] schedule)
			{
				this.Assistant = assistant;
				this.MaintenanceSchedule = schedule;
			}

			public static Server.MaintenanceScheduleEntry GetFromADString(string adString, ScheduledAssistant assistant)
			{
				if (string.IsNullOrEmpty(adString))
				{
					return null;
				}
				if (adString.Length != 84)
				{
					return null;
				}
				ScheduledAssistant scheduledAssistant = (ScheduledAssistant)adString[42];
				if (scheduledAssistant != assistant)
				{
					return null;
				}
				byte[] array = new byte[84];
				for (int i = 0; i < 42; i++)
				{
					array[i * 2] = (byte)(adString[i] >> 8);
					array[i * 2 + 1] = (byte)(adString[i] & 'ÿ');
				}
				return new Server.MaintenanceScheduleEntry(assistant, ScheduleInterval.GetIntervalsFromWeekBitmap(array));
			}

			public string ToADString()
			{
				byte[] weekBitmapFromIntervals = ScheduleInterval.GetWeekBitmapFromIntervals(this.MaintenanceSchedule);
				if (weekBitmapFromIntervals.Length != 84)
				{
					return null;
				}
				char[] array = new char[84];
				for (int i = 0; i < 42; i++)
				{
					int num = ((int)weekBitmapFromIntervals[i * 2] << 8) + (int)weekBitmapFromIntervals[i * 2 + 1];
					array[i] = (char)num;
				}
				array[42] = (char)this.Assistant;
				return new string(array);
			}

			private const int StringLengthLimit = 84;

			public readonly ScheduleInterval[] MaintenanceSchedule;

			public readonly ScheduledAssistant Assistant;
		}
	}
}
