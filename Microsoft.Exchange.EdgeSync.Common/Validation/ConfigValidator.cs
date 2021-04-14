using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.EdgeSync.Common.Internal;

namespace Microsoft.Exchange.EdgeSync.Validation
{
	internal abstract class ConfigValidator : EdgeSyncValidator
	{
		public ConfigValidator(ReplicationTopology topology, string configObjectName) : base(topology)
		{
			this.searchScope = SearchScope.Subtree;
			this.orgConfigObjectList = new Dictionary<string, ExSearchResultEntry>();
			this.orgAdRootPath = DistinguishedName.RemoveLeafRelativeDistinguishedNames(base.Topology.LocalHub.DistinguishedName, 4);
			this.configObjectName = configObjectName;
		}

		public bool UseChangedDate
		{
			get
			{
				return this.useChangedDate;
			}
			set
			{
				this.useChangedDate = value;
			}
		}

		protected string ConfigDirectoryPath
		{
			get
			{
				return this.configDirectoryPath;
			}
			set
			{
				this.configDirectoryPath = value;
			}
		}

		protected string AdamRootPath
		{
			get
			{
				return this.adamRootPath;
			}
			set
			{
				this.adamRootPath = value;
			}
		}

		protected string OrgAdRootPath
		{
			get
			{
				return this.orgAdRootPath;
			}
			set
			{
				this.orgAdRootPath = value;
			}
		}

		protected string ValidationObjectName
		{
			get
			{
				return this.configObjectName;
			}
			set
			{
				this.configObjectName = value;
			}
		}

		protected abstract string[] PayloadAttributes { get; }

		protected virtual string[] ReadAttributes
		{
			get
			{
				return null;
			}
		}

		protected virtual IDirectorySession DataSession
		{
			get
			{
				return base.Topology.ConfigSession;
			}
		}

		protected SearchScope SearchScope
		{
			get
			{
				return this.searchScope;
			}
			set
			{
				this.searchScope = value;
			}
		}

		protected EdgeConnectionInfo CurrentEdgeConnection
		{
			get
			{
				return this.currentEdgeConnection;
			}
		}

		protected string LdapQuery
		{
			get
			{
				return this.ldapQuery;
			}
			set
			{
				this.ldapQuery = value;
			}
		}

		protected virtual string ADSearchPath
		{
			get
			{
				return DistinguishedName.Concatinate(new string[]
				{
					this.configDirectoryPath,
					this.orgAdRootPath
				});
			}
		}

		protected virtual string ADAMSearchPath
		{
			get
			{
				return DistinguishedName.Concatinate(new string[]
				{
					this.configDirectoryPath,
					this.adamRootPath
				});
			}
		}

		protected virtual string ADLdapQuery
		{
			get
			{
				return this.ldapQuery;
			}
		}

		protected virtual string ADAMLdapQuery
		{
			get
			{
				return this.ldapQuery;
			}
		}

		public override EdgeConfigStatus Validate(EdgeConnectionInfo subscription)
		{
			this.SaveEdgeConnection(subscription);
			EdgeConfigStatus edgeConfigStatus = new EdgeConfigStatus();
			try
			{
				List<string> list = ConfigValidator.CombineAttributes(this.PayloadAttributes, this.ReadAttributes);
				Dictionary<string, ExSearchResultEntry> adentries = this.GetADEntries();
				bool flag = true;
				int num = 0;
				uint num2 = 0U;
				this.adamRootPath = DistinguishedName.Concatinate(new string[]
				{
					"CN=First Organization,CN=Microsoft Exchange,CN=Services",
					subscription.EdgeConnection.AdamConfigurationNamingContext
				});
				if (base.ProgressMethod != null)
				{
					base.ProgressMethod(Strings.LoadingADAMComparisonList(this.configObjectName, subscription.EdgeServer.Name), Strings.LoadedADAMObjectCount(num));
				}
				foreach (ExSearchResultEntry exSearchResultEntry in subscription.EdgeConnection.PagedScan(this.ADAMSearchPath, this.ADAMLdapQuery, this.searchScope, list.ToArray()))
				{
					string adamRelativePath = this.GetAdamRelativePath(exSearchResultEntry);
					if (adentries.ContainsKey(adamRelativePath))
					{
						if (this.Filter(adentries[adamRelativePath]))
						{
							if (!this.CompareAttributes(exSearchResultEntry, adentries[adamRelativePath], this.PayloadAttributes) && !this.IsInChangeWindow(adentries[adamRelativePath]))
							{
								if (base.MaxReportedLength.IsUnlimited || (ulong)base.MaxReportedLength.Value > (ulong)((long)edgeConfigStatus.ConflictObjects.Count))
								{
									edgeConfigStatus.ConflictObjects.Add(new ADObjectId(adentries[adamRelativePath].DistinguishedName));
								}
								flag = false;
							}
							else
							{
								num2 += 1U;
							}
						}
						else if (this.FilterEdge(exSearchResultEntry))
						{
							if (base.MaxReportedLength.IsUnlimited || (ulong)base.MaxReportedLength.Value > (ulong)((long)edgeConfigStatus.EdgeOnlyObjects.Count))
							{
								edgeConfigStatus.EdgeOnlyObjects.Add(new ADObjectId(exSearchResultEntry.DistinguishedName));
							}
							flag = false;
						}
						adentries.Remove(adamRelativePath);
					}
					else if (this.FilterEdge(exSearchResultEntry))
					{
						if (base.MaxReportedLength.IsUnlimited || (ulong)base.MaxReportedLength.Value > (ulong)((long)edgeConfigStatus.EdgeOnlyObjects.Count))
						{
							edgeConfigStatus.EdgeOnlyObjects.Add(new ADObjectId(exSearchResultEntry.DistinguishedName));
						}
						flag = false;
					}
					if (num % 500 == 0 && base.ProgressMethod != null)
					{
						base.ProgressMethod(Strings.LoadingADAMComparisonList(this.configObjectName, subscription.EdgeServer.Name), Strings.LoadedADAMObjectCount(num));
					}
					num++;
				}
				if (base.ProgressMethod != null)
				{
					base.ProgressMethod(Strings.LoadingADAMComparisonList(this.configObjectName, subscription.EdgeServer.Name), Strings.LoadedADAMObjectCount(num));
				}
				foreach (ExSearchResultEntry exSearchResultEntry2 in adentries.Values)
				{
					if (this.Filter(exSearchResultEntry2) && !this.IsInChangeWindow(exSearchResultEntry2))
					{
						if (base.MaxReportedLength.IsUnlimited || (ulong)base.MaxReportedLength.Value > (ulong)((long)edgeConfigStatus.OrgOnlyObjects.Count))
						{
							edgeConfigStatus.OrgOnlyObjects.Add(new ADObjectId(exSearchResultEntry2.DistinguishedName));
						}
						flag = false;
					}
				}
				edgeConfigStatus.SyncStatus = (flag ? SyncStatus.Synchronized : SyncStatus.NotSynchronized);
				edgeConfigStatus.SynchronizedObjects = num2;
			}
			catch (ExDirectoryException)
			{
				edgeConfigStatus.SyncStatus = SyncStatus.DirectoryError;
			}
			return edgeConfigStatus;
		}

		public override void LoadValidationInfo()
		{
			List<string> list = ConfigValidator.CombineAttributes(this.PayloadAttributes, this.ReadAttributes);
			Connection orgAdConnection = null;
			try
			{
				ADObjectId rootId = null;
				ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
				{
					orgAdConnection = new Connection(this.DataSession.GetReadConnection(null, ref rootId));
				}, 3);
				if (!adoperationResult.Succeeded)
				{
					throw new ExDirectoryException("Unable to get read connection", adoperationResult.Exception);
				}
				int num = 0;
				if (base.ProgressMethod != null)
				{
					base.ProgressMethod(Strings.LoadingADComparisonList(this.configObjectName), Strings.LoadedADObjectCount(num));
				}
				if (this.UseChangedDate)
				{
					list.Add("whenChanged");
				}
				foreach (ExSearchResultEntry exSearchResultEntry in orgAdConnection.PagedScan(this.ADSearchPath, this.ADLdapQuery, this.searchScope, list.ToArray()))
				{
					string adrelativePath = this.GetADRelativePath(exSearchResultEntry);
					this.orgConfigObjectList.Add(adrelativePath, exSearchResultEntry);
					if (num % 500 == 0 && base.ProgressMethod != null)
					{
						base.ProgressMethod(Strings.LoadingADComparisonList(this.configObjectName), Strings.LoadedADObjectCount(num));
					}
					num++;
				}
				if (base.ProgressMethod != null)
				{
					base.ProgressMethod(Strings.LoadingADComparisonList(this.configObjectName), Strings.LoadedADObjectCount(num));
				}
			}
			finally
			{
				if (orgAdConnection != null)
				{
					orgAdConnection.Dispose();
					orgAdConnection = null;
				}
			}
		}

		public override void UnloadValidationInfo()
		{
			this.orgConfigObjectList.Clear();
		}

		protected static List<string> CombineAttributes(string[] originalAttributes, string[] appendedAttributes)
		{
			List<string> list = new List<string>();
			if (originalAttributes != null)
			{
				list.AddRange(originalAttributes);
			}
			if (appendedAttributes != null)
			{
				list.AddRange(appendedAttributes);
			}
			return list;
		}

		protected bool IsInChangeWindow(ExSearchResultEntry entry)
		{
			if (this.useChangedDate && entry.Attributes.ContainsKey("whenChanged"))
			{
				try
				{
					DateTime t = DateTime.ParseExact((string)entry.Attributes["whenChanged"][0], "yyyyMMddHHmmss.fZ", CultureInfo.InvariantCulture);
					if (this.currentEdgeConnection.LastSynchronizedDate < t && this.syncInScheduleWindow)
					{
						return true;
					}
				}
				catch (FormatException)
				{
				}
				return false;
			}
			return false;
		}

		protected virtual string GetADRelativePath(ExSearchResultEntry searchEntry)
		{
			return DistinguishedName.MakeRelativePath(searchEntry.DistinguishedName.ToLower(), this.ADSearchPath.ToLower());
		}

		protected virtual string GetAdamRelativePath(ExSearchResultEntry searchEntry)
		{
			return DistinguishedName.MakeRelativePath(searchEntry.DistinguishedName.ToLower(), this.ADAMSearchPath.ToLower());
		}

		protected virtual bool Filter(ExSearchResultEntry entry)
		{
			return true;
		}

		protected virtual bool FilterEdge(ExSearchResultEntry entry)
		{
			return true;
		}

		protected virtual bool CompareAttributes(ExSearchResultEntry edgeEntry, ExSearchResultEntry hubEntry, string[] copyAttributes)
		{
			int i = 0;
			while (i < copyAttributes.Length)
			{
				string key = copyAttributes[i];
				bool flag = edgeEntry.Attributes.ContainsKey(key);
				bool flag2 = hubEntry.Attributes.ContainsKey(key);
				bool result;
				if (flag != flag2)
				{
					result = false;
				}
				else
				{
					if (!flag || this.CompareAttributeValues(edgeEntry.Attributes[key], hubEntry.Attributes[key]))
					{
						i++;
						continue;
					}
					result = false;
				}
				return result;
			}
			return true;
		}

		protected bool IsEntryContainer(ExSearchResultEntry entry)
		{
			return entry.ObjectClass == "msExchContainer" || entry.ObjectClass == "container";
		}

		private List<string> GetAttributeValues(DirectoryAttribute attribute)
		{
			List<string> list = new List<string>();
			for (int i = 0; i < attribute.Count; i++)
			{
				if (attribute[i].GetType() == typeof(byte[]))
				{
					byte[] array = (byte[])attribute[i];
					StringBuilder stringBuilder = new StringBuilder(array.Length * 2);
					foreach (byte b in array)
					{
						stringBuilder.Append(b.ToString("X2"));
					}
					list.Add(stringBuilder.ToString());
				}
				else
				{
					list.Add(attribute[i].ToString());
				}
			}
			list.Sort();
			return list;
		}

		protected bool CompareAttributeValues(DirectoryAttribute edgeAttribute, DirectoryAttribute hubAttribute)
		{
			if (edgeAttribute.Count != hubAttribute.Count)
			{
				return false;
			}
			if (edgeAttribute.Count == 1)
			{
				return this.CompareValues(edgeAttribute[0], hubAttribute[0]);
			}
			List<string> attributeValues = this.GetAttributeValues(hubAttribute);
			List<string> attributeValues2 = this.GetAttributeValues(edgeAttribute);
			for (int i = 0; i < attributeValues.Count; i++)
			{
				if (attributeValues[i] != attributeValues2[i])
				{
					return false;
				}
			}
			return true;
		}

		private bool CompareValues(object edgeValue, object hubValue)
		{
			Type type = edgeValue.GetType();
			Type type2 = hubValue.GetType();
			if (type != type2)
			{
				return false;
			}
			if (type == typeof(byte[]))
			{
				return base.CompareBytes((byte[])edgeValue, (byte[])hubValue);
			}
			return edgeValue.Equals(hubValue);
		}

		private Dictionary<string, ExSearchResultEntry> GetADEntries()
		{
			Dictionary<string, ExSearchResultEntry> dictionary = new Dictionary<string, ExSearchResultEntry>();
			foreach (KeyValuePair<string, ExSearchResultEntry> keyValuePair in this.orgConfigObjectList)
			{
				dictionary.Add(keyValuePair.Key, keyValuePair.Value);
			}
			return dictionary;
		}

		private void SaveEdgeConnection(EdgeConnectionInfo subscription)
		{
			this.currentEdgeConnection = subscription;
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 751, "SaveEdgeConnection", "f:\\15.00.1497\\sources\\dev\\EdgeSync\\src\\Common\\Validation\\ConfigValidator.cs");
			EdgeSyncServiceConfig edgeSyncServiceConfig = topologyConfigurationSession.Read<EdgeSyncServiceConfig>(topologyConfigurationSession.GetLocalSite().Id.GetChildId("EdgeSyncService"));
			this.syncInScheduleWindow = (edgeSyncServiceConfig.ConfigurationSyncInterval > DateTime.UtcNow.Subtract(subscription.LastSynchronizedDate));
		}

		private const int UpdateCount = 500;

		private string configDirectoryPath;

		private string adamRootPath;

		private string orgAdRootPath;

		private string configObjectName;

		private SearchScope searchScope;

		private EdgeConnectionInfo currentEdgeConnection;

		private string ldapQuery;

		private Dictionary<string, ExSearchResultEntry> orgConfigObjectList;

		private bool useChangedDate;

		private bool syncInScheduleWindow;
	}
}
