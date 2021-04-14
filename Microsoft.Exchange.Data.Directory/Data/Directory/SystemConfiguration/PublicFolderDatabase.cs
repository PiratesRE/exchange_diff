using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Directory.ExchangeTopology;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Database)]
	[Serializable]
	public sealed class PublicFolderDatabase : Database
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return PublicFolderDatabase.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return PublicFolderDatabase.MostDerivedClass;
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, this.MostDerivedObjectClass);
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			errors.AddRange(Database.ValidateAscendingQuotas(this.propertyBag, new ProviderPropertyDefinition[]
			{
				DatabaseSchema.IssueWarningQuota,
				PublicFolderDatabaseSchema.ProhibitPostQuota
			}, this.Identity));
			errors.AddRange(Database.ValidateAscendingQuotas(this.propertyBag, new ProviderPropertyDefinition[]
			{
				PublicFolderDatabaseSchema.MaxItemSize,
				PublicFolderDatabaseSchema.ProhibitPostQuota
			}, this.Identity));
			if (!this.UseCustomReferralServerList && this.CustomReferralServerList.Count != 0)
			{
				this.CustomReferralServerList.Clear();
			}
			foreach (ServerCostPair serverCostPair in this.CustomReferralServerList)
			{
				if (string.IsNullOrEmpty(serverCostPair.ServerName))
				{
					errors.Add(new ObjectValidationError(DirectoryStrings.PublicFolderReferralServerNotExisting(serverCostPair.ServerGuid.ToString()), this.Identity, string.Empty));
				}
			}
			if (this.CustomReferralServerList.Count > 1)
			{
				for (int i = 0; i < this.CustomReferralServerList.Count - 1; i++)
				{
					for (int j = i + 1; j < this.CustomReferralServerList.Count; j++)
					{
						if (this.CustomReferralServerList[i].ServerGuid == this.CustomReferralServerList[j].ServerGuid && this.CustomReferralServerList[i].Cost != this.CustomReferralServerList[j].Cost)
						{
							errors.Add(new ObjectValidationError(DirectoryStrings.ErrorPublicFolderReferralConflict(this.CustomReferralServerList[i].ToString(), this.CustomReferralServerList[j].ToString()), this.Identity, string.Empty));
							break;
						}
					}
				}
			}
		}

		public string Alias
		{
			get
			{
				return (string)this[PublicFolderDatabaseSchema.Alias];
			}
			internal set
			{
				this[PublicFolderDatabaseSchema.Alias] = value;
			}
		}

		public bool FirstInstance
		{
			get
			{
				return (bool)this[PublicFolderDatabaseSchema.FirstInstance];
			}
			internal set
			{
				this[PublicFolderDatabaseSchema.FirstInstance] = value;
			}
		}

		internal ADObjectId HomeMta
		{
			get
			{
				return (ADObjectId)this[PublicFolderDatabaseSchema.HomeMta];
			}
			set
			{
				this[PublicFolderDatabaseSchema.HomeMta] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> MaxItemSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[PublicFolderDatabaseSchema.MaxItemSize];
			}
			set
			{
				this[PublicFolderDatabaseSchema.MaxItemSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<EnhancedTimeSpan> ItemRetentionPeriod
		{
			get
			{
				return (Unlimited<EnhancedTimeSpan>)this[PublicFolderDatabaseSchema.ItemRetentionPeriod];
			}
			set
			{
				this[PublicFolderDatabaseSchema.ItemRetentionPeriod] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public uint ReplicationPeriod
		{
			get
			{
				return (uint)this[PublicFolderDatabaseSchema.ReplicationPeriod];
			}
			set
			{
				this[PublicFolderDatabaseSchema.ReplicationPeriod] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> ProhibitPostQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[PublicFolderDatabaseSchema.ProhibitPostQuota];
			}
			set
			{
				this[PublicFolderDatabaseSchema.ProhibitPostQuota] = value;
			}
		}

		public ADObjectId PublicFolderHierarchy
		{
			get
			{
				return (ADObjectId)this[PublicFolderDatabaseSchema.PublicFolderHierarchy];
			}
			internal set
			{
				this[PublicFolderDatabaseSchema.PublicFolderHierarchy] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> Organizations
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[PublicFolderDatabaseSchema.Organizations];
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize ReplicationMessageSize
		{
			get
			{
				return (ByteQuantifiedSize)this[PublicFolderDatabaseSchema.ReplicationMessageSize];
			}
			set
			{
				this[PublicFolderDatabaseSchema.ReplicationMessageSize] = value;
			}
		}

		internal ScheduleMode ReplicationMode
		{
			get
			{
				return (ScheduleMode)this[PublicFolderDatabaseSchema.ReplicationMode];
			}
		}

		[Parameter(Mandatory = false)]
		public Schedule ReplicationSchedule
		{
			get
			{
				return (Schedule)this[PublicFolderDatabaseSchema.ReplicationSchedule];
			}
			set
			{
				this[PublicFolderDatabaseSchema.ReplicationSchedule] = value;
			}
		}

		public bool UseCustomReferralServerList
		{
			get
			{
				return (bool)this[PublicFolderDatabaseSchema.UseCustomReferralServerList];
			}
			set
			{
				this[PublicFolderDatabaseSchema.UseCustomReferralServerList] = value;
			}
		}

		public MultiValuedProperty<ServerCostPair> CustomReferralServerList
		{
			get
			{
				return (MultiValuedProperty<ServerCostPair>)this[PublicFolderDatabaseSchema.CustomReferralServerList];
			}
			set
			{
				this[PublicFolderDatabaseSchema.CustomReferralServerList] = value;
			}
		}

		internal override void StampPersistableDefaultValues()
		{
			if (!base.IsModified(PublicFolderDatabaseSchema.MaxItemSize))
			{
				this.MaxItemSize = new Unlimited<ByteQuantifiedSize>(ByteQuantifiedSize.FromKB(10240UL));
			}
			if (!base.IsModified(PublicFolderDatabaseSchema.ProhibitPostQuota))
			{
				this.ProhibitPostQuota = new Unlimited<ByteQuantifiedSize>(ByteQuantifiedSize.FromGB(2UL));
			}
			base.StampPersistableDefaultValues();
		}

		internal static object ReplicationScheduleGetter(IPropertyBag propertyBag)
		{
			switch ((ScheduleMode)propertyBag[PublicFolderDatabaseSchema.ReplicationMode])
			{
			case ScheduleMode.Never:
				return Schedule.Never;
			case ScheduleMode.Always:
				return Schedule.Always;
			}
			return propertyBag[PublicFolderDatabaseSchema.ReplicationScheduleBitmaps];
		}

		internal static void ReplicationScheduleSetter(object value, IPropertyBag propertyBag)
		{
			if (value == null)
			{
				value = Schedule.Never;
			}
			propertyBag[PublicFolderDatabaseSchema.ReplicationMode] = ((Schedule)value).Mode;
			propertyBag[PublicFolderDatabaseSchema.ReplicationScheduleBitmaps] = value;
		}

		internal static ADObjectId FindClosestPublicFolderDatabase(IConfigDataProvider scSession, ADObjectId sourceServerId)
		{
			PublicFolderDatabase publicFolderDatabase = PublicFolderDatabase.FindClosestPublicFolderDatabase(scSession, sourceServerId, null);
			if (publicFolderDatabase == null)
			{
				return null;
			}
			return (ADObjectId)publicFolderDatabase.Identity;
		}

		internal static PublicFolderDatabase FindClosestPublicFolderDatabase(IConfigDataProvider scSession, ADObjectId sourceServerId, Func<PublicFolderDatabase, bool> candidateMatcher)
		{
			if (scSession == null)
			{
				throw new ArgumentNullException("scSession");
			}
			PublicFolderDatabase result = null;
			PublicFolderDatabase[] array = (PublicFolderDatabase[])scSession.Find<PublicFolderDatabase>(null, null, true, null);
			if (candidateMatcher != null && 0 < array.Length)
			{
				array = array.Where(candidateMatcher).ToArray<PublicFolderDatabase>();
			}
			if (1 == array.Length)
			{
				result = array[0];
			}
			else if (array.Length > 1)
			{
				ExchangeTopology exchangeTopology = ExchangeTopology.Discover(null, ExchangeTopologyScope.ADAndExchangeServerAndSiteTopology);
				TopologySite topologySite = null;
				TopologySite topologySite2 = null;
				if (sourceServerId == null)
				{
					topologySite = exchangeTopology.LocalSite;
				}
				else
				{
					string text = null;
					Server server = (Server)scSession.Read<Server>(sourceServerId);
					if (server != null)
					{
						text = server.Fqdn;
					}
					if (!string.IsNullOrEmpty(text))
					{
						topologySite = exchangeTopology.SiteFromADServer(text);
					}
				}
				if (topologySite != null)
				{
					ReadOnlyCollection<TopologySite> allTopologySites = exchangeTopology.AllTopologySites;
					ReadOnlyCollection<TopologySiteLink> allTopologySiteLinks = exchangeTopology.AllTopologySiteLinks;
					ReadOnlyCollection<TopologyServer> allTopologyServers = exchangeTopology.AllTopologyServers;
					Dictionary<TopologyServer, TopologySite> dictionary = new Dictionary<TopologyServer, TopologySite>();
					foreach (TopologyServer topologyServer in allTopologyServers)
					{
						if (topologyServer.TopologySite != null)
						{
							foreach (TopologySite topologySite3 in allTopologySites)
							{
								if (topologySite3.DistinguishedName.Equals(topologyServer.TopologySite.DistinguishedName, StringComparison.OrdinalIgnoreCase))
								{
									dictionary[topologyServer] = topologySite3;
									break;
								}
							}
						}
					}
					Dictionary<TopologySite, PublicFolderDatabase> dictionary2 = new Dictionary<TopologySite, PublicFolderDatabase>();
					List<TopologySite> list = new List<TopologySite>();
					foreach (PublicFolderDatabase publicFolderDatabase in array)
					{
						foreach (KeyValuePair<TopologyServer, TopologySite> keyValuePair in dictionary)
						{
							if (keyValuePair.Key.DistinguishedName.Equals(publicFolderDatabase.Server.DistinguishedName, StringComparison.OrdinalIgnoreCase))
							{
								if (!dictionary2.ContainsKey(keyValuePair.Value))
								{
									dictionary2[keyValuePair.Value] = publicFolderDatabase;
									list.Add(keyValuePair.Value);
									break;
								}
								if (keyValuePair.Key.IsExchange2007OrLater)
								{
									dictionary2[keyValuePair.Value] = publicFolderDatabase;
									break;
								}
								break;
							}
						}
					}
					topologySite2 = exchangeTopology.FindClosestDestinationSite(topologySite, list);
					if (topologySite2 != null)
					{
						result = dictionary2[topologySite2];
					}
				}
				if (topologySite2 == null)
				{
					result = array[0];
				}
			}
			return result;
		}

		public static string CalculateServerLegacyDNFromPfdbLegacyDN(string pfdbLegacyDN)
		{
			if (string.IsNullOrEmpty(pfdbLegacyDN))
			{
				throw new ArgumentNullException("pfdbLegacyDN");
			}
			return Database.GetRcaLegacyDNFromDatabaseLegacyDN(LegacyDN.Parse(pfdbLegacyDN)).ToString();
		}

		internal const int MaxPublicFolderDatabaseCount = 10000;

		private static PublicFolderDatabaseSchema schema = ObjectSchema.GetInstance<PublicFolderDatabaseSchema>();

		internal static readonly string MostDerivedClass = "msExchPublicMDB";
	}
}
