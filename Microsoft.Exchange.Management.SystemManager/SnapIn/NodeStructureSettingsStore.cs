using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.SnapIn
{
	[Serializable]
	public class NodeStructureSettingsStore
	{
		public NodeStructureSettingsStore(string localOnPremiseKey)
		{
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\MMC\\NodeTypes\\{A8DE63D9-A83F-4ee5-B723-E88F5DC21264}\\Extensions\\NameSpace", false))
			{
				if (registryKey != null)
				{
					string[] valueNames = registryKey.GetValueNames();
					foreach (string text in valueNames)
					{
						if (!text.Equals(localOnPremiseKey, StringComparison.InvariantCultureIgnoreCase))
						{
							using (RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\MMC\\SnapIns\\" + text, false))
							{
								if (registryKey2 != null)
								{
									string text2 = registryKey2.GetValue("ApplicationBase").ToString();
									string version = string.Empty;
									if (!ConfigurationContext.Setup.BinPath.Equals(text2, StringComparison.InvariantCultureIgnoreCase))
									{
										version = text2.Substring(ConfigurationContext.Setup.BinPath.Length + 1);
									}
									else
									{
										version = NodeStructureSettingsStore.DominantVersion;
									}
									this.slots.Add(new Slot
									{
										Key = text,
										Version = version
									});
								}
							}
						}
					}
				}
			}
		}

		public string[] SlotsKey
		{
			get
			{
				return (from c in this.slots
				select c.Key).ToArray<string>();
			}
		}

		public string GetVersion(string slotKey)
		{
			return (from c in this.slots
			where c.Key == slotKey
			select c).First<Slot>().Version;
		}

		public Fqdn LocalOnPremiseRemotePSServer { get; set; }

		public IList<Uri> RecentServerUris
		{
			get
			{
				return this.recentServerUris;
			}
		}

		public void Load()
		{
			try
			{
				using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\AdminTools", false))
				{
					if (registryKey != null)
					{
						this.serializedSettings = (registryKey.GetValue(NodeStructureSettingsStore.NodeStructureSettings) as byte[]);
						if (this.serializedSettings != null)
						{
							NodeStructureSettingsStore nodeStructureSettingsStore = WinformsHelper.DeSerialize(this.serializedSettings) as NodeStructureSettingsStore;
							if (nodeStructureSettingsStore.settings != null)
							{
								if ((from c in nodeStructureSettingsStore.settings
								where c.State == NodeStructureSettingState.Used
								select c).Count<NodeStructureSetting>() <= (from c in this.slots
								where c.Version == NodeStructureSettingsStore.DominantVersion
								select c).Count<Slot>())
								{
									using (IEnumerator<NodeStructureSetting> enumerator = nodeStructureSettingsStore.settings.GetEnumerator())
									{
										while (enumerator.MoveNext())
										{
											NodeStructureSetting nodeStructureSetting = enumerator.Current;
											if (nodeStructureSetting.State == NodeStructureSettingState.Used)
											{
												this.organizationSettings.Add(new OrganizationSetting
												{
													CredentialKey = nodeStructureSetting.CredentialKey,
													DisplayName = nodeStructureSetting.DisplayName,
													LogonWithDefaultCredential = nodeStructureSetting.LogonWithDefaultCredential,
													Type = nodeStructureSetting.Type,
													Uri = nodeStructureSetting.Uri,
													SupportedVersionList = new SupportedVersionList(NodeStructureSettingsStore.DominantVersion)
												});
											}
										}
										goto IL_172;
									}
								}
							}
							this.organizationSettings = (nodeStructureSettingsStore.organizationSettings ?? new List<OrganizationSetting>());
							this.recentServerUris = (nodeStructureSettingsStore.recentServerUris ?? new List<Uri>());
							IL_172:
							this.LocalOnPremiseRemotePSServer = nodeStructureSettingsStore.LocalOnPremiseRemotePSServer;
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}

		public void Save()
		{
			try
			{
				byte[] array = WinformsHelper.Serialize(this);
				if (this.serializedSettings == null || !WinformsHelper.ByteArrayEquals(array, this.serializedSettings))
				{
					using (RegistryKey registryKey = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\AdminTools"))
					{
						registryKey.SetValue(NodeStructureSettingsStore.NodeStructureSettings, array, RegistryValueKind.Binary);
					}
				}
			}
			catch (SecurityException)
			{
			}
			catch (UnauthorizedAccessException)
			{
			}
		}

		public OrganizationSetting this[int pos]
		{
			get
			{
				return this.organizationSettings[pos];
			}
		}

		public OrganizationSetting this[string key]
		{
			get
			{
				return (from c in this.organizationSettings
				where c.Key == key
				select c).First<OrganizationSetting>();
			}
		}

		public int Count
		{
			get
			{
				return this.organizationSettings.Count;
			}
		}

		public bool IsDuplicatedName(string organizationName)
		{
			foreach (OrganizationSetting organizationSetting in this.organizationSettings)
			{
				if (string.Equals(organizationSetting.DisplayName, organizationName, StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		public IList<OrganizationSetting> GetInitiatedOrganizationInReverseOrder(ref string extendVersion)
		{
			IList<OrganizationSetting> list = this.organizationSettings.Reverse<OrganizationSetting>().ToList<OrganizationSetting>();
			IList<OrganizationSetting> list2 = new List<OrganizationSetting>();
			extendVersion = string.Empty;
			foreach (OrganizationSetting organizationSetting in list)
			{
				if (this.IsRegisteredSlot(organizationSetting.Key) && organizationSetting.SupportedVersionList == null)
				{
					list2.Add(organizationSetting);
				}
				else if (organizationSetting.SupportedVersionList != null && organizationSetting.SupportedVersionList.Count > 0)
				{
					IList<Slot> compatibleSlots = this.GetCompatibleSlots(organizationSetting.SupportedVersionList);
					if (compatibleSlots.Count<Slot>() == 0)
					{
						compatibleSlots = this.slots;
					}
					IEnumerable<Slot> source = from c in compatibleSlots
					where !this.IsInUse(c.Key)
					select c;
					if (source.Count<Slot>() > 0)
					{
						organizationSetting.Key = source.First<Slot>().Key;
						list2.Add(organizationSetting);
					}
					else if (compatibleSlots.Count<Slot>() > 0)
					{
						extendVersion = compatibleSlots.First<Slot>().Version;
					}
					else
					{
						extendVersion = organizationSetting.SupportedVersionList[0].ToString();
					}
				}
			}
			return list2;
		}

		public List<Slot> GetCompatibleSlots(SupportedVersionList supportedVersionList)
		{
			return (from c in this.slots
			where supportedVersionList.IsSupported(c.Version)
			select c).ToList<Slot>();
		}

		public IList<OrganizationSetting> GetOrganizationSetting()
		{
			return this.organizationSettings.ToList<OrganizationSetting>();
		}

		public bool HasAvailableSlots
		{
			get
			{
				return (from s in this.slots
				where !s.Removed
				select s).Count<Slot>() > this.organizationSettings.Count<OrganizationSetting>();
			}
		}

		public bool HasAvailableDominantSlots
		{
			get
			{
				SupportedVersionList dominantVersion = new SupportedVersionList(NodeStructureSettingsStore.DominantVersion);
				IEnumerable<Slot> enumerable = from s in this.slots
				where !s.Removed && dominantVersion.IsSupported(s.Version)
				select s;
				foreach (Slot slot in enumerable)
				{
					bool flag = true;
					foreach (OrganizationSetting organizationSetting in this.organizationSettings)
					{
						if (string.Equals(slot.Key, organizationSetting.Key, StringComparison.InvariantCultureIgnoreCase))
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						return true;
					}
				}
				return false;
			}
		}

		public void RemoveOrganization(string key)
		{
			this.organizationSettings.Remove(this[key]);
			Slot slot = (from c in this.slots
			where c.Key == key
			select c).First<Slot>();
			slot.Removed = true;
		}

		public bool IsInUse(string key)
		{
			return (from c in this.organizationSettings
			where c.Key == key && null == c.SupportedVersionList
			select c).Count<OrganizationSetting>() > 0;
		}

		private bool IsRegisteredSlot(string key)
		{
			return (from c in this.SlotsKey
			where string.Equals(c, key, StringComparison.InvariantCultureIgnoreCase)
			select c).Count<string>() > 0;
		}

		public string Add(string displayName, Uri uri, bool logonWithDefaultCredential, OrganizationType type, SupportedVersionList supportedVersionList)
		{
			if (!this.HasAvailableSlots)
			{
				throw new IndexOutOfRangeException("No more slots are available.");
			}
			OrganizationSetting organizationSetting = new OrganizationSetting();
			organizationSetting.DisplayName = displayName;
			organizationSetting.Uri = uri;
			organizationSetting.LogonWithDefaultCredential = logonWithDefaultCredential;
			organizationSetting.Type = type;
			organizationSetting.Key = this.AllocateSlot(supportedVersionList);
			this.organizationSettings.Add(organizationSetting);
			if (type != OrganizationType.Cloud && !this.RecentServerUris.Contains(uri))
			{
				this.RecentServerUris.Add(uri);
			}
			return organizationSetting.Key;
		}

		private string AllocateSlot(SupportedVersionList supportedVersionList)
		{
			IEnumerable<Slot> source = from s in this.slots
			where supportedVersionList.IsSupported(s.Version) && !s.Removed && !this.IsInUse(s.Key)
			select s;
			if (source.Count<Slot>() > 0)
			{
				return source.First<Slot>().Key;
			}
			IEnumerable<Slot> source2 = from s in this.slots
			where !s.Removed && !this.IsInUse(s.Key)
			select s;
			if (source2.Count<Slot>() > 0)
			{
				return source2.First<Slot>().Key;
			}
			return null;
		}

		private const string userKeyRoot = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\AdminTools";

		private const string availableSlotsRoot = "SOFTWARE\\Microsoft\\MMC\\NodeTypes\\{A8DE63D9-A83F-4ee5-B723-E88F5DC21264}\\Extensions\\NameSpace";

		private const string slotsRoot = "SOFTWARE\\Microsoft\\MMC\\SnapIns";

		public static string DominantVersion = ConfigurationContext.Setup.GetExecutingVersion().ToString();

		internal static string NodeStructureSettings = "NodeStructureSettings";

		[NonSerialized]
		private byte[] serializedSettings;

		private IList<Slot> slots = new List<Slot>();

		private IList<OrganizationSetting> organizationSettings = new List<OrganizationSetting>();

		private IList<Uri> recentServerUris = new List<Uri>();

		private IList<NodeStructureSetting> settings;
	}
}
