using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.DxStore.Common;
using Microsoft.Win32;

namespace Microsoft.Exchange.Cluster.Shared
{
	public class ClusdbSnapshotMaker
	{
		public ClusdbSnapshotMaker(string[] filterRootKeys = null, string rootKeyName = null, bool isForceIncludeRootProperties = false) : this(null, filterRootKeys, rootKeyName, isForceIncludeRootProperties)
		{
		}

		internal ClusdbSnapshotMaker(AmClusterHandle clusterHandle = null, string[] filterRootKeys = null, string rootKeyName = null, bool isForceIncludeRootProperties = false)
		{
			this.clusterHandle = clusterHandle;
			this.rootKeyName = rootKeyName;
			this.filterRootKeys = filterRootKeys;
			this.isForceIncludeRootProperties = isForceIncludeRootProperties;
		}

		public bool IsProduceDxStoreFormat { get; set; }

		public bool IsIncludeKey(string keyName)
		{
			return this.filterRootKeys == null || this.filterRootKeys.Length == 0 || this.filterRootKeys.Any((string filterKey) => Utils.IsEqual(filterKey, keyName, StringComparison.OrdinalIgnoreCase));
		}

		public XElement GetXElementSnapshot(string keyName = null)
		{
			bool flag = false;
			if (this.clusterHandle == null || this.clusterHandle.IsInvalid)
			{
				this.clusterHandle = ClusapiMethods.OpenCluster(null);
				flag = true;
			}
			XElement xelementSnapshotInternal;
			try
			{
				using (IDistributedStoreKey distributedStoreKey = ClusterDbKey.GetBaseKey(this.clusterHandle, DxStoreKeyAccessMode.Read))
				{
					this.baseKey = distributedStoreKey;
					xelementSnapshotInternal = this.GetXElementSnapshotInternal(keyName);
				}
			}
			finally
			{
				if (flag && this.clusterHandle != null && !this.clusterHandle.IsInvalid)
				{
					this.clusterHandle.Close();
					this.clusterHandle = null;
				}
			}
			return xelementSnapshotInternal;
		}

		private XElement GetXElementSnapshotInternal(string keyName)
		{
			bool flag = false;
			bool flag2 = true;
			string keyName2;
			string value;
			if (!string.IsNullOrEmpty(keyName))
			{
				keyName2 = Utils.CombinePathNullSafe(this.rootKeyName, keyName);
				value = keyName.Substring(keyName.LastIndexOf('\\') + 1);
			}
			else
			{
				keyName2 = this.rootKeyName;
				value = "\\";
				flag = true;
				if (this.filterRootKeys.Length > 1)
				{
					flag2 = false;
				}
			}
			XElement xelement = new XElement("Key", new XAttribute("Name", value));
			XElement xelement2 = xelement;
			if (this.IsProduceDxStoreFormat && flag)
			{
				xelement.Add(new XElement("Key", new XAttribute("Name", "Private")));
				XElement xelement3 = new XElement("Key", new XAttribute("Name", "Public"));
				xelement.Add(xelement3);
				xelement2 = xelement3;
			}
			IDistributedStoreKey distributedStoreKey = flag ? this.baseKey : this.baseKey.OpenKey(keyName2, DxStoreKeyAccessMode.Read, false, null);
			try
			{
				if (flag2 || this.isForceIncludeRootProperties)
				{
					foreach (string propertyName in distributedStoreKey.GetValueNames(null))
					{
						bool flag3;
						RegistryValueKind kind;
						object value2 = distributedStoreKey.GetValue(propertyName, out flag3, out kind, null);
						if (value2 != null && flag3)
						{
							PropertyValue propertyValue = new PropertyValue(value2, kind);
							xelement2.Add(propertyValue.ToXElement(propertyName));
						}
					}
				}
				IEnumerable<string> subkeyNames = distributedStoreKey.GetSubkeyNames(null);
				if (subkeyNames != null)
				{
					foreach (string path in subkeyNames)
					{
						string keyName3 = Utils.CombinePathNullSafe(keyName ?? string.Empty, path);
						if (!flag || this.IsIncludeKey(keyName3))
						{
							XElement xelementSnapshotInternal = this.GetXElementSnapshotInternal(keyName3);
							xelement2.Add(xelementSnapshotInternal);
						}
					}
				}
			}
			finally
			{
				if (distributedStoreKey != null && !flag)
				{
					distributedStoreKey.Dispose();
				}
			}
			return xelement;
		}

		private readonly string[] filterRootKeys;

		private readonly string rootKeyName;

		private readonly bool isForceIncludeRootProperties;

		private AmClusterHandle clusterHandle;

		private IDistributedStoreKey baseKey;
	}
}
