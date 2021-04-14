using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;

namespace Microsoft.Exchange.MessagingPolicies.AttachFilter
{
	internal class ContainerReaderFactory
	{
		internal static bool Create(AttachmentInfo attachmentInfo, out IEnumerable<string> reader)
		{
			reader = null;
			ContainerReaderFactory.ContainerType containerType = ContainerReaderFactory.GetContainerTypeFromName(attachmentInfo.Name);
			ExTraceGlobals.AttachmentFilteringTracer.TraceDebug<string>(0L, "Attachment container-type: {0}", Enum<ContainerReaderFactory.ContainerType>.ToString((int)containerType));
			if (attachmentInfo.ContentTypes.Contains("application/x-zip-compressed"))
			{
				if (containerType != ContainerReaderFactory.ContainerType.None && containerType != ContainerReaderFactory.ContainerType.Zip)
				{
					ExTraceGlobals.AttachmentFilteringTracer.TraceDebug<string, string>(0L, "The attachment name {0} indicates that this is a {1}, but the Content-Type (either present in the attachment, or sniffed from the content) indicates that it is a Zip. This is inconsistent information and the message will be rejected.", attachmentInfo.Name, Enum<ContainerReaderFactory.ContainerType>.ToString((int)containerType));
					return false;
				}
				containerType = ContainerReaderFactory.ContainerType.Zip;
			}
			switch (containerType)
			{
			case ContainerReaderFactory.ContainerType.Zip:
				reader = new ZipReader(attachmentInfo.Attachment.GetContentReadStream(), 15);
				return true;
			case ContainerReaderFactory.ContainerType.Lzh:
				reader = new LZHReader(attachmentInfo.Attachment.GetContentReadStream());
				return true;
			default:
				reader = null;
				return true;
			}
		}

		private static ContainerReaderFactory.ContainerType GetContainerTypeFromName(string attachmentName)
		{
			for (int i = 0; i < 2; i++)
			{
				if (attachmentName.EndsWith(ContainerReaderFactory.containerExtensions[i], StringComparison.OrdinalIgnoreCase))
				{
					return (ContainerReaderFactory.ContainerType)i;
				}
			}
			return ContainerReaderFactory.ContainerType.None;
		}

		private const int ZipNestedLevels = 15;

		private static string[] containerExtensions = new string[]
		{
			".zip",
			".lzh"
		};

		private enum ContainerType
		{
			Zip,
			Lzh,
			None
		}
	}
}
