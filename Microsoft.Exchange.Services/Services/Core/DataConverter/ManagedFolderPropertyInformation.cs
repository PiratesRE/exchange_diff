using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class ManagedFolderPropertyInformation : PropertyInformation
	{
		public ManagedFolderPropertyInformation() : base("ManagedFolderInformation", ServiceXml.GetFullyQualifiedName("ManagedFolderInformation"), ServiceXml.DefaultNamespaceUri, ExchangeVersion.Exchange2007, ManagedFolderPropertyInformation.GetPropertyDefinitions(), new PropertyUri(PropertyUriEnum.ManagedFolderInformation), new PropertyCommand.CreatePropertyCommand(ManagedFolderInformationProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands)
		{
		}

		private static PropertyDefinition[] GetPropertyDefinitions()
		{
			return new PropertyDefinition[]
			{
				FolderSchema.AdminFolderFlags,
				FolderSchema.ELCPolicyIds,
				FolderSchema.ELCFolderComment,
				FolderSchema.FolderQuota,
				FolderSchema.FolderSize,
				FolderSchema.FolderHomePageUrl
			};
		}

		internal enum ManagedFolderInfoIndex
		{
			AdminFolderFlags,
			ELCPolicyIds,
			ELCFolderComment,
			FolderQuota,
			FolderSize,
			FolderHomePageUrl
		}
	}
}
