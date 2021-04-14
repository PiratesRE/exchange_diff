using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("ArchiveItemType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class ArchiveItemRequest : BaseRequest
	{
		[XmlElement("ArchiveSourceFolderId", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[DataMember(Name = "ArchiveSourceFolderId", IsRequired = true, Order = 1)]
		public TargetFolderId SourceFolderId { get; set; }

		[XmlArrayItem("ItemId", typeof(ItemId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("RecurringMasterItemId", typeof(RecurringMasterItemId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArray("ItemIds", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[XmlArrayItem("OccurrenceItemId", typeof(OccurrenceItemId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[DataMember(Name = "ItemIds", IsRequired = true, Order = 2)]
		public BaseItemId[] Ids { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new ArchiveItem(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return this.GetServerInfoForStep(callContext, 0);
		}

		internal BaseServerIdInfo GetServerInfoForStep(CallContext callContext, int step)
		{
			return BaseRequest.GetServerInfoForFolderId(callContext, this.SourceFolderId.BaseFolderId);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int currentStep)
		{
			return base.GetResourceKeysForFolderId(true, callContext, this.SourceFolderId.BaseFolderId);
		}

		internal override void Validate()
		{
			base.Validate();
			if (this.Ids == null || this.Ids.Length == 0)
			{
				throw FaultExceptionUtilities.CreateFault(new ServiceInvalidOperationException(ResponseCodeType.ErrorInvalidIdMalformed), FaultParty.Sender);
			}
			if (this.SourceFolderId == null)
			{
				throw FaultExceptionUtilities.CreateFault(new ServiceInvalidOperationException(ResponseCodeType.ErrorInvalidFolderTypeForOperation), FaultParty.Sender);
			}
		}

		internal const string SourceFolderIdElementName = "ArchiveSourceFolderId";

		internal const string ItemsElementName = "ItemIds";
	}
}
