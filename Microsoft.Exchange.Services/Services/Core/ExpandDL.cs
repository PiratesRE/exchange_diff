using System;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class ExpandDL : UtilCommandBase<ExpandDLRequest, XmlNode>
	{
		public ExpandDL(CallContext callContext, ExpandDLRequest request) : base(callContext, request)
		{
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			ExpandDLResponse expandDLResponse = new ExpandDLResponse();
			expandDLResponse.ProcessServiceResult<XmlNode>(base.Result);
			return expandDLResponse;
		}

		private static QueryFilter GetAliasFilter(string nameToMatch)
		{
			return new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.Alias, nameToMatch);
		}

		private static QueryFilter GetGeneralNameFilter(string nameToMatch)
		{
			return new OrFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, nameToMatch),
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.DisplayName, nameToMatch),
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.LegacyExchangeDN, nameToMatch)
			});
		}

		private static string MailboxTypeToString(bool objectHasDirectoryOrigin, bool objectIsDistributionList)
		{
			if (!objectHasDirectoryOrigin)
			{
				if (!objectIsDistributionList)
				{
					return "Contact";
				}
				return "PrivateDL";
			}
			else
			{
				if (!objectIsDistributionList)
				{
					return "Mailbox";
				}
				return "PublicDL";
			}
		}

		private static int ExpandDirectoryDL(IADDistributionList directoryDL, XmlDocument xmlDoc, XmlElement xmlDLExpansion)
		{
			int num = 0;
			if (directoryDL.Members.Count > 0 || (directoryDL is ADRecipient && ((ADRecipient)directoryDL).RecipientType == RecipientType.DynamicDistributionGroup))
			{
				ADPagedReader<ADRawEntry> adpagedReader = directoryDL.Expand(0, ExpandDL.DirectoryDLDefaultProps);
				foreach (ADRawEntry adrawEntry in adpagedReader)
				{
					if (!(bool)adrawEntry[ADRecipientSchema.HiddenFromAddressListsEnabled])
					{
						MailboxHelper.MailboxTypeType mailboxTypeType = MailboxHelper.ConvertToMailboxType((RecipientType)adrawEntry[ADRecipientSchema.RecipientType], (RecipientTypeDetails)adrawEntry[ADRecipientSchema.RecipientTypeDetails]);
						if (mailboxTypeType != MailboxHelper.MailboxTypeType.Unknown)
						{
							XmlElement parentElement = ServiceXml.CreateElement(xmlDLExpansion, "Mailbox", "http://schemas.microsoft.com/exchange/services/2006/types");
							num++;
							string text = (string)adrawEntry[ADRecipientSchema.DisplayName];
							if (string.IsNullOrEmpty(text))
							{
								text = (string)adrawEntry[ADObjectSchema.Name];
							}
							ServiceXml.CreateTextElement(parentElement, "Name", text);
							SmtpAddress smtpAddress = (SmtpAddress)adrawEntry[ADRecipientSchema.PrimarySmtpAddress];
							if (!string.IsNullOrEmpty(smtpAddress.ToString()))
							{
								ServiceXml.CreateTextElement(parentElement, "EmailAddress", smtpAddress.ToString());
								ServiceXml.CreateTextElement(parentElement, "RoutingType", "SMTP");
							}
							else
							{
								if (string.IsNullOrEmpty((string)adrawEntry[ADRecipientSchema.LegacyExchangeDN]))
								{
									continue;
								}
								ServiceXml.CreateTextElement(parentElement, "EmailAddress", (string)adrawEntry[ADRecipientSchema.LegacyExchangeDN]);
								ServiceXml.CreateTextElement(parentElement, "RoutingType", "EX");
							}
							ServiceXml.CreateTextElement(parentElement, "MailboxType", mailboxTypeType.ToString());
						}
					}
				}
			}
			return num;
		}

		internal override ServiceResult<XmlNode> Execute()
		{
			if (base.Request.Mailbox != null)
			{
				if (base.Request.Mailbox.EmailAddress != null)
				{
					this.directoryDLToExpand = base.Request.Mailbox.EmailAddress;
				}
				else if (base.Request.Mailbox.ItemId != null)
				{
					this.storeDLToExpand = base.Request.Mailbox.ItemId;
				}
			}
			base.InitActiveDirectoryNameResolutionContext();
			XmlNode value;
			if (!string.IsNullOrEmpty(this.directoryDLToExpand))
			{
				value = this.ExpandDirectoryDL(this.directoryDLToExpand);
			}
			else
			{
				if (this.storeDLToExpand == null)
				{
					throw new MissingInformationEmailAddressException();
				}
				value = this.ExpandStoreDL(this.storeDLToExpand);
			}
			return new ServiceResult<XmlNode>(value);
		}

		private bool ParticipantToMailboxXml(XmlElement xmlParent, DistributionListMember storeDLMember)
		{
			if (storeDLMember.Participant != null)
			{
				StoreParticipantOrigin storeParticipantOrigin = storeDLMember.Participant.Origin as StoreParticipantOrigin;
				Participant participant = MailboxHelper.TryConvertTo(storeDLMember.Participant, "SMTP") ?? storeDLMember.Participant;
				string textValue;
				if (ExchangeVersion.Current.Supports(ExchangeVersionType.Exchange2010))
				{
					textValue = MailboxHelper.GetMailboxType(participant.Origin, participant.RoutingType).ToString();
				}
				else
				{
					bool objectIsDistributionList = storeDLMember.IsDistributionList() ?? false;
					textValue = ExpandDL.MailboxTypeToString(storeParticipantOrigin == null, objectIsDistributionList);
				}
				XmlElement xmlElement = ServiceXml.CreateElement(xmlParent, "Mailbox", "http://schemas.microsoft.com/exchange/services/2006/types");
				ServiceXml.CreateNonEmptyTextElement(xmlElement, "Name", participant.DisplayName);
				ServiceXml.CreateNonEmptyTextElement(xmlElement, "EmailAddress", participant.EmailAddress);
				ServiceXml.CreateNonEmptyTextElement(xmlElement, "RoutingType", participant.RoutingType);
				ServiceXml.CreateTextElement(xmlElement, "MailboxType", textValue);
				if (storeParticipantOrigin != null && storeParticipantOrigin.OriginItemId != null)
				{
					IdConverter.CreateStoreIdXml(xmlElement, storeParticipantOrigin.OriginItemId, IdAndSession.CreateFromItem(storeDLMember.DistributionList), "ItemId");
				}
				return true;
			}
			return false;
		}

		private XmlNode ExpandStoreDL(ItemId storeDLItemId)
		{
			int num = 0;
			IdAndSession idAndSession = base.IdConverter.ConvertItemIdToIdAndSessionReadOnly(storeDLItemId);
			XmlNode result;
			try
			{
				using (Item rootXsoItem = idAndSession.GetRootXsoItem(null))
				{
					DistributionList distributionList = rootXsoItem as DistributionList;
					if (distributionList == null)
					{
						throw new InvalidItemForOperationException("ExpandDL");
					}
					XmlElement xmlElement = UtilCommandBase<ExpandDLRequest, XmlNode>.CreateResponseXml("DLExpansion", base.XmlDocument);
					try
					{
						for (int i = 0; i < distributionList.Count; i++)
						{
							if (this.ParticipantToMailboxXml(xmlElement, distributionList[i]))
							{
								num++;
							}
						}
					}
					catch (AccessDeniedException innerException)
					{
						if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2007SP1))
						{
							throw new ServiceAccessDeniedException((CoreResources.IDs)3931903304U, innerException);
						}
						throw;
					}
					UtilCommandBase<ExpandDLRequest, XmlNode>.ResolutionResponseAttributesToXml(xmlElement, num, true);
					result = xmlElement;
				}
			}
			catch (AccessDeniedException innerException2)
			{
				if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2007SP1))
				{
					throw new ServiceAccessDeniedException((CoreResources.IDs)3793759002U, innerException2);
				}
				throw;
			}
			return result;
		}

		private XmlNode ExpandDirectoryDL(string directoryDLName)
		{
			if (base.CallContext.AccessingPrincipal == null)
			{
				throw new NameResolutionNoMailboxException();
			}
			ADRecipient adrecipient = null;
			ProxyAddress proxyAddress = ProxyAddress.Parse(directoryDLName);
			if (!(proxyAddress is InvalidProxyAddress))
			{
				try
				{
					adrecipient = this.directoryRecipientSession.FindByProxyAddress(proxyAddress);
				}
				catch (NonUniqueRecipientException)
				{
					throw new NameResolutionMultipleResultsException();
				}
			}
			if (adrecipient == null)
			{
				ADRecipient[] array = this.directoryRecipientSession.Find(base.CallContext.AccessingADUser.QueryBaseDN, QueryScope.SubTree, ExpandDL.GetAliasFilter(directoryDLName), null, 2);
				if (array.Length == 1)
				{
					adrecipient = array[0];
				}
				else if (array.Length > 1)
				{
					throw new NameResolutionMultipleResultsException();
				}
			}
			if (adrecipient == null)
			{
				ADRecipient[] array2 = this.directoryRecipientSession.Find(base.CallContext.AccessingADUser.QueryBaseDN, QueryScope.SubTree, ExpandDL.GetGeneralNameFilter(directoryDLName), null, 2);
				if (array2.Length == 0)
				{
					throw new NameResolutionNoResultsException();
				}
				if (array2.Length > 1)
				{
					throw new NameResolutionMultipleResultsException();
				}
				adrecipient = array2[0];
			}
			if (adrecipient.HiddenFromAddressListsEnabled)
			{
				throw new NameResolutionNoResultsException();
			}
			if (adrecipient.RecipientType != RecipientType.MailUniversalDistributionGroup && adrecipient.RecipientType != RecipientType.MailUniversalSecurityGroup && adrecipient.RecipientType != RecipientType.MailNonUniversalGroup && adrecipient.RecipientType != RecipientType.DynamicDistributionGroup)
			{
				throw new NameResolutionNoResultsException();
			}
			XmlElement xmlElement = UtilCommandBase<ExpandDLRequest, XmlNode>.CreateResponseXml("DLExpansion", base.XmlDocument);
			int totalItemsInView = ExpandDL.ExpandDirectoryDL(adrecipient as IADDistributionList, base.XmlDocument, xmlElement);
			UtilCommandBase<ExpandDLRequest, XmlNode>.ResolutionResponseAttributesToXml(xmlElement, totalItemsInView, true);
			return xmlElement;
		}

		private string directoryDLToExpand;

		private ItemId storeDLToExpand;

		internal static readonly ADPropertyDefinition[] DirectoryDLDefaultProps = new ADPropertyDefinition[]
		{
			ADObjectSchema.Name,
			ADRecipientSchema.DisplayName,
			ADRecipientSchema.SimpleDisplayName,
			ADRecipientSchema.PrimarySmtpAddress,
			ADRecipientSchema.LegacyExchangeDN,
			ADRecipientSchema.RecipientType,
			ADRecipientSchema.HiddenFromAddressListsEnabled
		};
	}
}
