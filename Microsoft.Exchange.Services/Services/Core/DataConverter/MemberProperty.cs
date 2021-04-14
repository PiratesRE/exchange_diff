using System;
using System.Xml;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class MemberProperty : MembersProperty
	{
		private MemberProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static MemberProperty CreateMembersMemberCommand(CommandContext commandContext)
		{
			if (MembersProperty.RenderMembersCollection == null)
			{
				MembersProperty.RenderMembersCollection = new bool?(false);
			}
			return new MemberProperty(commandContext);
		}

		public override void ToServiceObject()
		{
			if (MembersProperty.RenderMembersCollection != null && !MembersProperty.RenderMembersCollection.Value)
			{
				ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
				ServiceObject serviceObject = commandSettings.ServiceObject;
				DictionaryPropertyUri dictionaryPropertyUri = commandSettings.PropertyPath as DictionaryPropertyUri;
				DistributionList distributionList = (DistributionList)commandSettings.StoreObject;
				if (distributionList.Count > 0)
				{
					string key = dictionaryPropertyUri.Key;
					int num = MembersProperty.FindMemberIndex(distributionList, key);
					if (num == -1)
					{
						throw new DistributionListMemberNotExistException(dictionaryPropertyUri);
					}
					MemberType memberType = base.MemberToServiceObject(distributionList, num);
					serviceObject[this.commandContext.PropertyInformation] = new MemberType[]
					{
						memberType
					};
				}
			}
		}

		public override void DeleteUpdate(DeletePropertyUpdate deletePropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			StoreObject storeObject = updateCommandSettings.StoreObject;
			DictionaryPropertyUri dictionaryPropertyUri = updateCommandSettings.PropertyUpdate.PropertyPath as DictionaryPropertyUri;
			DistributionList distributionList = (DistributionList)storeObject;
			int num = MembersProperty.FindMemberIndex(distributionList, dictionaryPropertyUri.Key);
			if (num == -1)
			{
				throw new DistributionListMemberNotExistException(dictionaryPropertyUri);
			}
			distributionList.RemoveAt(num);
		}

		public override void AppendUpdate(AppendPropertyUpdate appendPropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			throw new InvalidPropertyAppendException(appendPropertyUpdate.PropertyPath);
		}

		public override void SetUpdate(SetPropertyUpdate setPropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			throw new InvalidPropertySetException(setPropertyUpdate.PropertyPath);
		}

		public override void ToXml()
		{
			if (MembersProperty.RenderMembersCollection != null && !MembersProperty.RenderMembersCollection.Value)
			{
				ToXmlCommandSettings commandSettings = base.GetCommandSettings<ToXmlCommandSettings>();
				DictionaryPropertyUri dictionaryPropertyUri = commandSettings.PropertyPath as DictionaryPropertyUri;
				DistributionList distributionList = (DistributionList)commandSettings.StoreObject;
				if (distributionList.Count > 0)
				{
					string key = dictionaryPropertyUri.Key;
					int num = MembersProperty.FindMemberIndex(distributionList, key);
					if (num == -1)
					{
						throw new DistributionListMemberNotExistException(dictionaryPropertyUri);
					}
					XmlElement serviceItem = commandSettings.ServiceItem;
					XmlElement xmlElement = serviceItem["Members", "http://schemas.microsoft.com/exchange/services/2006/types"];
					if (xmlElement == null)
					{
						xmlElement = base.CreateXmlElement(commandSettings.ServiceItem, "Members");
					}
					base.MemberToXml(distributionList, num, xmlElement);
				}
			}
		}
	}
}
