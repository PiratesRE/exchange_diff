using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class VotingInformationProperty : ComplexPropertyBase, IToServiceObjectCommand, IPropertyCommand
	{
		public VotingInformationProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static VotingInformationProperty CreateCommand(CommandContext commandContext)
		{
			return new VotingInformationProperty(commandContext);
		}

		public void ToXml()
		{
			throw new InvalidOperationException("VotingInformationProperty.ToXml should not be called.");
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			MessageItem messageItem = commandSettings.StoreObject as MessageItem;
			if (messageItem == null || messageItem.VotingInfo == null)
			{
				return;
			}
			VotingInfo.OptionData[] array = (VotingInfo.OptionData[])messageItem.VotingInfo.GetOptionsDataList();
			if (string.IsNullOrEmpty(messageItem.VotingInfo.Response) && (array == null || array.Length == 0))
			{
				return;
			}
			VotingInformationType votingInformationType = new VotingInformationType();
			votingInformationType.VotingResponse = messageItem.VotingInfo.Response;
			if (array != null && array.Length != 0)
			{
				votingInformationType.UserOptions = new VotingOptionDataType[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					votingInformationType.UserOptions[i] = new VotingOptionDataType();
					votingInformationType.UserOptions[i].DisplayName = array[i].DisplayName;
					votingInformationType.UserOptions[i].SendPrompt = (SendPromptType)array[i].SendPrompt;
				}
			}
			ServiceObject serviceObject = commandSettings.ServiceObject;
			serviceObject.PropertyBag[propertyInformation] = votingInformationType;
		}
	}
}
