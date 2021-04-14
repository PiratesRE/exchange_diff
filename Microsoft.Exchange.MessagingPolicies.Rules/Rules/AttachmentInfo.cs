using System;
using System.Linq;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class AttachmentInfo
	{
		public string Name { get; set; }

		public string Extension { get; set; }

		public string FileType { get; set; }

		public AttachmentInfo Parent { get; set; }

		public uint[] FileTypes { get; private set; }

		public AttachmentInfo(string name, string extension, uint[] types)
		{
			this.Name = name;
			this.Extension = extension;
			this.FileType = AttachmentInfo.GetFileTypeFromFipsTypes(types);
			this.FileTypes = types;
			this.Parent = null;
		}

		public bool IsFileEmbeddingSupported()
		{
			uint aggregatedTypeBits = 1879048192U;
			return this.FileTypes.Any((uint type) => (aggregatedTypeBits & type) != 0U || 4U == type);
		}

		private static string GetFileTypeFromFipsTypes(uint[] types)
		{
			if (!types.Any((uint type) => 0U != (2147483648U & type)))
			{
				return "unknown";
			}
			return "executable";
		}
	}
}
