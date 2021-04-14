using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Exchange.DxStore.Common;
using Microsoft.Win32;

namespace Microsoft.Exchange.DxStore.Server
{
	public class RegistrySnapshotMaker
	{
		public RegistrySnapshotMaker(string rootKeyName, string[] filterRootKeys = null, bool isForceIncludeRootProperties = false)
		{
			this.rootKeyName = rootKeyName;
			this.filterRootKeys = filterRootKeys;
			this.isForceIncludeRootProperties = isForceIncludeRootProperties;
		}

		public bool IsIncludeKey(string keyName)
		{
			return this.filterRootKeys == null || this.filterRootKeys.Length == 0 || this.filterRootKeys.Any((string filterKey) => Utils.IsEqual(filterKey, keyName, StringComparison.OrdinalIgnoreCase));
		}

		public XElement GetXElementSnapshot(bool isProduceDxStoreFormat = false)
		{
			return this.GetXElementSnapshotInternal(null, isProduceDxStoreFormat);
		}

		private XElement GetXElementSnapshotInternal(string keyName = null, bool isProduceDxStoreFormat = false)
		{
			bool flag = false;
			bool flag2 = true;
			string name;
			string value;
			if (!string.IsNullOrEmpty(keyName))
			{
				name = Utils.CombinePathNullSafe(this.rootKeyName, keyName);
				value = keyName.Substring(keyName.LastIndexOf('\\') + 1);
			}
			else
			{
				name = this.rootKeyName;
				value = "\\";
				flag = true;
				if (this.filterRootKeys.Length > 1)
				{
					flag2 = false;
				}
			}
			XElement xelement = new XElement("Key", new XAttribute("Name", value));
			XElement xelement2 = xelement;
			if (isProduceDxStoreFormat && flag)
			{
				xelement.Add(new XElement("Key", new XAttribute("Name", "Private")));
				XElement xelement3 = new XElement("Key", new XAttribute("Name", "Public"));
				xelement.Add(xelement3);
				xelement2 = xelement3;
			}
			string[] array = null;
			using (RegistryKey key = Registry.LocalMachine.OpenSubKey(name))
			{
				if (key != null)
				{
					if (flag2 || this.isForceIncludeRootProperties)
					{
						IEnumerable<XElement> content = from propertyName in key.GetValueNames()
						let value = key.GetValue(propertyName)
						where value != null
						let valueKind = key.GetValueKind(propertyName)
						select new PropertyValue(value, valueKind).ToXElement(propertyName);
						xelement2.Add(content);
					}
					array = key.GetSubKeyNames();
				}
			}
			if (array != null)
			{
				foreach (string path in array)
				{
					string keyName2 = Utils.CombinePathNullSafe(keyName ?? string.Empty, path);
					if (!flag || this.IsIncludeKey(keyName2))
					{
						XElement xelementSnapshotInternal = this.GetXElementSnapshotInternal(keyName2, false);
						xelement2.Add(xelementSnapshotInternal);
					}
				}
			}
			return xelement;
		}

		private readonly string[] filterRootKeys;

		private readonly string rootKeyName;

		private readonly bool isForceIncludeRootProperties;
	}
}
