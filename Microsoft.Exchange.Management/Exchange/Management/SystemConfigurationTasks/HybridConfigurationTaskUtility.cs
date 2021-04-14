using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Hybrid;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal static class HybridConfigurationTaskUtility
	{
		public static MultiValuedProperty<ADObjectId> ValidateServers(ADPropertyDefinition propertyDefinition, IConfigDataProvider session, PropertyBag fields, HybridConfigurationTaskUtility.GetUniqueObject getServer, Task.TaskErrorLoggingDelegate writeError, params HybridConfigurationTaskUtility.ServerCriterion[] serverCriteria)
		{
			return HybridConfigurationTaskUtility.ValidateServers(propertyDefinition, session, fields[propertyDefinition.Name] as MultiValuedProperty<ServerIdParameter>, getServer, writeError, serverCriteria);
		}

		public static MultiValuedProperty<ADObjectId> ValidateServers(ADPropertyDefinition propertyDefinition, IConfigDataProvider session, MultiValuedProperty<ServerIdParameter> servers, HybridConfigurationTaskUtility.GetUniqueObject getServer, Task.TaskErrorLoggingDelegate writeError, params HybridConfigurationTaskUtility.ServerCriterion[] serverCriteria)
		{
			MultiValuedProperty<ADObjectId> multiValuedProperty = new MultiValuedProperty<ADObjectId>(false, propertyDefinition, new object[0]);
			if (servers != null)
			{
				foreach (ServerIdParameter serverIdParameter in servers)
				{
					if (serverIdParameter != null)
					{
						Server server = getServer(serverIdParameter, session, null, new LocalizedString?(Strings.ErrorServerNotFound(serverIdParameter.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(serverIdParameter.ToString()))) as Server;
						if (server != null)
						{
							if (serverCriteria != null)
							{
								foreach (HybridConfigurationTaskUtility.ServerCriterion serverCriterion in serverCriteria)
								{
									if (!serverCriterion.DoesMeet(server))
									{
										writeError(new InvalidOperationException(serverCriterion.Error(server.ToString())), ErrorCategory.InvalidOperation, server.ToString());
									}
								}
							}
							if (multiValuedProperty.Contains((ADObjectId)server.Identity))
							{
								writeError(new InvalidOperationException(HybridStrings.ErrorHybridServerAlreadyAssigned(server.Identity.ToString())), ErrorCategory.InvalidOperation, server.ToString());
							}
							else
							{
								multiValuedProperty.Add(((ADObjectId)server.Identity).DistinguishedName);
							}
						}
					}
				}
			}
			return multiValuedProperty;
		}

		public static MultiValuedProperty<IPRange> ValidateExternalIPAddresses(MultiValuedProperty<IPRange> externalIPAddresses, Task.TaskErrorLoggingDelegate writeError)
		{
			if (externalIPAddresses.Count > 40)
			{
				writeError(new ArgumentException(HybridStrings.ErrorHybridExternalIPAddressesExceeded40Entries), ErrorCategory.InvalidArgument, null);
				return null;
			}
			foreach (IPRange iprange in externalIPAddresses)
			{
				if (iprange.RangeFormat == IPRange.Format.LoHi)
				{
					writeError(new ArgumentException(HybridStrings.ErrorHybridExternalIPAddressesRangeAddressesNotSupported), ErrorCategory.InvalidArgument, null);
					return null;
				}
			}
			return externalIPAddresses;
		}

		public static int GetCount<T>(MultiValuedProperty<T> list)
		{
			if (list != null)
			{
				return list.Count;
			}
			return 0;
		}

		public delegate IConfigurable GetUniqueObject(IIdentityParameter id, IConfigDataProvider session, ObjectId rootID, LocalizedString? notFoundError, LocalizedString? multipleFoundError);

		public class ServerCriterion
		{
			public ServerCriterion(Func<Server, bool> doesMeet, Func<string, LocalizedString> error)
			{
				this.DoesMeet = doesMeet;
				this.Error = error;
			}

			public Func<Server, bool> DoesMeet { get; private set; }

			public Func<string, LocalizedString> Error { get; private set; }
		}
	}
}
