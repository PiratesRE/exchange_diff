using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "GetNonIndexableItemDetailsType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Name = "GetNonIndexableItemDetailsRequest", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class GetNonIndexableItemDetailsRequest : BaseRequest
	{
		[DataMember(Name = "Mailboxes", IsRequired = true)]
		[XmlArray(ElementName = "Mailboxes", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[XmlArrayItem(ElementName = "LegacyDN", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(string))]
		public string[] Mailboxes { get; set; }

		[XmlElement("PageSize")]
		[DataMember(Name = "PageSize", IsRequired = false)]
		public int? PageSize { get; set; }

		[DataMember(Name = "PageItemReference", IsRequired = false)]
		[XmlElement("PageItemReference")]
		public string PageItemReference { get; set; }

		[IgnoreDataMember]
		[XmlElement("PageDirection")]
		public SearchPageDirectionType? PageDirection { get; set; }

		[XmlIgnore]
		[DataMember(Name = "PageDirection", IsRequired = false)]
		public string PageDirectionString
		{
			get
			{
				if (this.PageDirection == null || this.PageDirection == null)
				{
					return null;
				}
				return EnumUtilities.ToString<SearchPageDirectionType?>(this.PageDirection);
			}
			set
			{
				this.PageDirection = new SearchPageDirectionType?(EnumUtilities.Parse<SearchPageDirectionType>(value));
			}
		}

		[XmlElement("SearchArchiveOnly")]
		[DataMember(Name = "SearchArchiveOnly", IsRequired = false)]
		public bool SearchArchiveOnly { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new GetNonIndexableItemDetails(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			if (this.SearchArchiveOnly || this.Mailboxes == null || this.Mailboxes.Length <= 0)
			{
				return null;
			}
			if (this.Mailboxes[0].StartsWith("\\"))
			{
				throw FaultExceptionUtilities.CreateFault(new NonExistentMailboxException(CoreResources.IDs.MessagePublicFoldersNotSupportedForNonIndexable, this.Mailboxes[0]), FaultParty.Sender);
			}
			IRecipientSession adrecipientSession = callContext.ADRecipientSessionContext.GetADRecipientSession();
			ADRecipient adrecipient = adrecipientSession.FindByLegacyExchangeDN(this.Mailboxes[0]);
			if (adrecipient == null)
			{
				throw FaultExceptionUtilities.CreateFault(new NonExistentMailboxException(CoreResources.IDs.MessageNonExistentMailboxLegacyDN, this.Mailboxes[0]), FaultParty.Sender);
			}
			return MailboxIdServerInfo.Create(adrecipient.PrimarySmtpAddress.ToString());
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int currentStep)
		{
			return null;
		}
	}
}
