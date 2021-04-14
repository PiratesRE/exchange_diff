using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Management.Metabase;

namespace Microsoft.Exchange.Management.IisTasks
{
	internal sealed class IsapiExtensionList : IDisposable
	{
		public IsapiExtensionList(string hostName)
		{
			string iisDirectoryEntryPath = string.Format("IIS://{0}/W3SVC", hostName);
			try
			{
				this.rootEntry = IisUtility.CreateIISDirectoryEntry(iisDirectoryEntryPath);
				this.restrictionList = this.rootEntry.Properties["WebSvcExtRestrictionList"];
				this.extensionMap = new List<IsapiExtensionList.ExtensionMapUnit>(this.restrictionList.Count + 5);
				for (int i = 0; i < this.restrictionList.Count; i++)
				{
					string text = this.restrictionList[i] as string;
					if (text != null)
					{
						IsapiExtension isapiExtension = IsapiExtension.Parse(text);
						if (isapiExtension != null)
						{
							this.extensionMap.Add(new IsapiExtensionList.ExtensionMapUnit(isapiExtension, i));
						}
					}
				}
			}
			catch (Exception)
			{
				this.Dispose();
				throw;
			}
		}

		~IsapiExtensionList()
		{
			this.Dispose();
		}

		public void Dispose()
		{
			this.extensionMap = null;
			this.restrictionList = null;
			if (this.rootEntry != null)
			{
				this.rootEntry.Dispose();
				this.rootEntry = null;
			}
			GC.SuppressFinalize(this);
		}

		public int Count
		{
			get
			{
				return this.extensionMap.Count;
			}
		}

		public IsapiExtension this[int i]
		{
			get
			{
				return this.extensionMap[i].Extension;
			}
			set
			{
				IsapiExtensionList.ExtensionMapUnit extensionMapUnit = this.extensionMap[i];
				this.extensionMap[i] = new IsapiExtensionList.ExtensionMapUnit(value, extensionMapUnit.RestrictionListIndex);
				this.restrictionList[this.extensionMap[i].RestrictionListIndex] = value.ToMetabaseString();
			}
		}

		public void Add(IsapiExtension extension)
		{
			this.restrictionList.Add(extension.ToMetabaseString());
			this.extensionMap.Add(new IsapiExtensionList.ExtensionMapUnit(extension, this.restrictionList.Count - 1));
		}

		public void Add(bool allow, string physicalPath, bool uiDeletable, string groupID, string description)
		{
			bool flag = false;
			for (int i = 0; i < this.extensionMap.Count; i++)
			{
				IsapiExtensionList.ExtensionMapUnit extensionMapUnit = this.extensionMap[i];
				if (string.Compare(extensionMapUnit.Extension.PhysicalPath, physicalPath, true, CultureInfo.InvariantCulture) == 0 && string.Compare(extensionMapUnit.Extension.GroupID, groupID, true, CultureInfo.InvariantCulture) == 0)
				{
					extensionMapUnit.Extension.Allow = allow;
					extensionMapUnit.Extension.UIDeletable = uiDeletable;
					extensionMapUnit.Extension.Description = description;
					flag = true;
				}
			}
			if (!flag)
			{
				IsapiExtension isapiExtension = new IsapiExtension(physicalPath, groupID, description, allow, uiDeletable);
				this.restrictionList.Add(isapiExtension.ToMetabaseString());
				this.extensionMap.Add(new IsapiExtensionList.ExtensionMapUnit(isapiExtension, this.restrictionList.Count - 1));
			}
		}

		public void RemoveAt(int i)
		{
			this.restrictionList.RemoveAt(this.extensionMap[i].RestrictionListIndex);
			this.extensionMap.RemoveAt(i);
			for (int j = 0; j < this.extensionMap.Count; j++)
			{
				IsapiExtensionList.ExtensionMapUnit value = this.extensionMap[j];
				if (value.RestrictionListIndex > i)
				{
					value.RestrictionListIndex--;
					this.extensionMap[j] = value;
				}
			}
		}

		public void CommitChanges()
		{
			this.rootEntry.CommitChanges();
		}

		public bool Exists(string groupID, string physicalPath)
		{
			List<int> list = this.FindMatchingExtensions(groupID, physicalPath, true);
			return list.Count > 0;
		}

		private List<int> FindMatchingExtensions(string groupID, string path, bool fullPath)
		{
			if (path == null || groupID == null)
			{
				return new List<int>(0);
			}
			List<int> list = new List<int>(this.extensionMap.Count);
			for (int i = 0; i < this.extensionMap.Count; i++)
			{
				if (string.Compare(groupID, this.extensionMap[i].Extension.GroupID, true, CultureInfo.InvariantCulture) == 0)
				{
					string text = this.extensionMap[i].Extension.PhysicalPath;
					if (!fullPath)
					{
						text = Path.GetFileName(text);
					}
					if (string.Compare(path, text, true, CultureInfo.InvariantCulture) == 0)
					{
						list.Add(i);
					}
				}
			}
			return list;
		}

		public List<int> FindMatchingExtensions(string groupID, string executableName)
		{
			return this.FindMatchingExtensions(groupID, executableName, false);
		}

		public void RemoveByExecutable(string executableName)
		{
			for (int i = this.extensionMap.Count - 1; i >= 0; i--)
			{
				if (string.Compare(this.extensionMap[i].Extension.PhysicalPath, executableName, true, CultureInfo.InvariantCulture) == 0)
				{
					this.RemoveAt(i);
				}
			}
		}

		private const string webServicesUri = "IIS://{0}/W3SVC";

		private const int extraCapacity = 5;

		private List<IsapiExtensionList.ExtensionMapUnit> extensionMap;

		private PropertyValueCollection restrictionList;

		private DirectoryEntry rootEntry;

		private struct ExtensionMapUnit
		{
			internal ExtensionMapUnit(IsapiExtension extension, int restrictionListIndex)
			{
				this.Extension = extension;
				this.RestrictionListIndex = restrictionListIndex;
			}

			internal IsapiExtension Extension;

			internal int RestrictionListIndex;
		}
	}
}
