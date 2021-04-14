using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.PST;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal sealed class PstTargetFolderProvider : TargetFolderProvider<uint, IFolder, IPST>
	{
		protected override void InitializeTargetFolderHierarchy()
		{
			base.FolderMapping.Add("", 32802U);
		}

		protected override IFolder GetFolder(IPST targetSession, uint folderId)
		{
			return targetSession.ReadFolder(folderId);
		}

		protected override string GenerateTopLevelFolderName(bool isArchive)
		{
			if (!isArchive)
			{
				return "Primary Mailbox";
			}
			return "Archive Mailbox";
		}

		protected override IFolder CreateFolder(IPST targetSession, IFolder parentFolder, string folderName)
		{
			List<uint> subFolderIds = parentFolder.SubFolderIds;
			if (subFolderIds != null)
			{
				foreach (uint num in subFolderIds)
				{
					IFolder folder = targetSession.ReadFolder(num);
					Dictionary<ushort, IProperty> properties = folder.PropertyBag.Properties;
					IProperty property = null;
					if (properties.TryGetValue(PropertyTag.DisplayName.Id, out property))
					{
						IPropertyReader propertyReader = property.OpenStreamReader();
						string @string = Encoding.Unicode.GetString(propertyReader.Read());
						if (@string == folderName)
						{
							return folder;
						}
						propertyReader.Close();
					}
				}
			}
			IFolder folder2 = parentFolder.AddFolder();
			IProperty property2 = folder2.PropertyBag.AddProperty(PropertyTag.DisplayName.Value);
			IPropertyWriter propertyWriter = property2.OpenStreamWriter();
			propertyWriter.Write(Encoding.Unicode.GetBytes(folderName));
			propertyWriter.Close();
			folder2.Save();
			return folder2;
		}

		protected override uint GetFolderId(IFolder folder)
		{
			return folder.Id;
		}

		private const int PstIpmRootFolderId = 32802;
	}
}
