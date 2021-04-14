using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class DistributionListSchema : Schema
	{
		static DistributionListSchema()
		{
			XmlElementInformation[] xmlElements = new XmlElementInformation[]
			{
				DistributionListSchema.DisplayName,
				DistributionListSchema.FileAs,
				DistributionListSchema.Members,
				DistributionListSchema.MembersMember
			};
			DistributionListSchema.schema = new DistributionListSchema(xmlElements);
		}

		private DistributionListSchema(XmlElementInformation[] xmlElements) : base(xmlElements)
		{
		}

		public static Schema GetSchema()
		{
			return DistributionListSchema.schema;
		}

		public static readonly PropertyInformation DisplayName = new PropertyInformation(PropertyUriEnum.DisplayName, ExchangeVersion.Exchange2007, StoreObjectSchema.DisplayName, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsDeleteUpdateCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static readonly PropertyInformation FileAs = new PropertyInformation(PropertyUriEnum.FileAs, ExchangeVersion.Exchange2007, ContactBaseSchema.FileAs, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation Members = new PropertyInformation(PropertyUriEnum.Members, ExchangeVersion.Exchange2010, DistributionListSchema.Members, new PropertyCommand.CreatePropertyCommand(MembersProperty.CreateMembersCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsAppendUpdateCommand | PropertyInformationAttributes.ImplementsDeleteUpdateCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand);

		public static readonly PropertyInformation MembersMember = new PropertyInformation(PropertyUriEnum.Members.ToString(), ServiceXml.GetFullyQualifiedName(DictionaryUriEnum.DistributionListMembersMember.ToString()), ServiceXml.DefaultNamespaceUri, ExchangeVersion.Exchange2010, new PropertyDefinition[]
		{
			DistributionListSchema.Members
		}, new DictionaryPropertyUriBase(DictionaryUriEnum.DistributionListMembersMember), new PropertyCommand.CreatePropertyCommand(MemberProperty.CreateMembersMemberCommand), true, PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsAppendUpdateCommand | PropertyInformationAttributes.ImplementsDeleteUpdateCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand);

		private static Schema schema;
	}
}
