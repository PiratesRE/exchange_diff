using System;
using System.Globalization;
using System.Linq;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal abstract class UtilCommandBase<RequestType, ResponseType> : SingleStepServiceCommand<RequestType, ResponseType> where RequestType : BaseRequest
	{
		public UtilCommandBase(CallContext callContext, RequestType request) : base(callContext, request)
		{
		}

		protected static void ResolutionResponseAttributesToXml(XmlElement xmlResolution, int totalItemsInView, bool includesLastItemInRange)
		{
			ServiceXml.CreateAttribute(xmlResolution, "TotalItemsInView", totalItemsInView.ToString(CultureInfo.InvariantCulture));
			ServiceXml.CreateAttribute(xmlResolution, "IncludesLastItemInRange", includesLastItemInRange ? bool.TrueString.ToLowerInvariant() : bool.FalseString.ToLowerInvariant());
		}

		protected static XmlElement CreateResponseXml(string xmlElementName, XmlDocument xmlDocument)
		{
			return ServiceXml.CreateElement(xmlDocument, xmlElementName, "http://schemas.microsoft.com/exchange/services/2006/messages");
		}

		protected void InitActiveDirectoryNameResolutionContext()
		{
			if (base.CallContext.AccessingPrincipal != null && base.CallContext.AccessingPrincipal.PreferredCultures.Any<CultureInfo>())
			{
				int lcid = base.CallContext.AccessingPrincipal.PreferredCultures.First<CultureInfo>().LCID;
			}
			this.directoryRecipientSession = base.CallContext.ADRecipientSessionContext.GetGALScopedADRecipientSession(base.CallContext.EffectiveCaller.ClientSecurityContext);
		}

		protected ContactsFolder InitStoreNameResolutionContext(BaseFolderId folderId)
		{
			ContactsFolder result;
			if (folderId != null)
			{
				IdConverter idConverter = new IdConverter(base.CallContext);
				IdAndSession idAndSession = idConverter.ConvertFolderIdToIdAndSessionReadOnly(folderId);
				if (idAndSession.GetAsStoreObjectId().ObjectType != StoreObjectType.ContactsFolder)
				{
					throw new ResolveNamesExceptionInvalidFolderType();
				}
				result = ContactsFolder.Bind(idAndSession.Session, idAndSession.Id, null);
			}
			else
			{
				this.storeMailboxSession = base.GetMailboxIdentityMailboxSession();
				result = ContactsFolder.Bind(this.storeMailboxSession, DefaultFolderType.Contacts);
			}
			return result;
		}

		protected const string XmlAttributeNameKey = "Key";

		protected const string XmlAttributeNameIncludesLastItemInRange = "IncludesLastItemInRange";

		protected const string XmlAttributeNameTotalItemsInView = "TotalItemsInView";

		protected const string XmlAttributeValueActiveDirectory = "ActiveDirectory";

		protected const string XmlAttributeValueBusiness = "Business";

		protected const string XmlAttributeValueContact = "Contact";

		protected const string XmlAttributeValueEmailAddress = "EmailAddress";

		protected const string XmlAttributeValueEmailAddress1 = "EmailAddress1";

		protected const string XmlAttributeValueEmailAddress2 = "EmailAddress2";

		protected const string XmlAttributeValueEmailAddress3 = "EmailAddress3";

		protected const string XmlAttributeValueMailbox = "Mailbox";

		protected const string XmlAttributeValuePublicDL = "PublicDL";

		protected const string XmlAttributeValuePrivateDL = "PrivateDL";

		protected const string XmlAttributeValuePublicFolder = "PublicFolder";

		protected const string XmlElementContactSourceStoreValue = "Store";

		protected const string XmlElementNameAssistantName = "AssistantName";

		protected const string XmlElementNameAssistantPhone = "AssistantPhone";

		protected const string XmlElementNameBusinessFax = "BusinessFax";

		protected const string XmlElementNameBusinessPhone = "BusinessPhone";

		protected const string XmlElementNameCity = "City";

		protected const string XmlElementNameCompanyName = "CompanyName";

		protected const string XmlElementNameContact = "Contact";

		protected const string XmlElementNameContactSource = "ContactSource";

		protected const string XmlElementNameCountryOrRegion = "CountryOrRegion";

		protected const string XmlElementNameCulture = "Culture";

		protected const string XmlElementNameDepartment = "Department";

		protected const string XmlElementNameDisplayName = "DisplayName";

		protected const string XmlElementNameDLExpansion = "DLExpansion";

		protected const string XmlElementNameEmailAddress = "EmailAddress";

		protected const string XmlElementNameEmailAddresses = "EmailAddresses";

		protected const string XmlElementNameEntry = "Entry";

		protected const string XmlElementNameExpandDL = "ExpandDL";

		protected const string XmlElementNameGivenName = "GivenName";

		protected const string XmlElementNameHomePhone = "HomePhone";

		protected const string XmlElementNameHomePhone2 = "HomePhone2";

		protected const string XmlElementNameInitials = "Initials";

		protected const string XmlElementNameItemId = "ItemId";

		protected const string XmlElementNameJobTitle = "JobTitle";

		protected const string XmlElementNameMailbox = "Mailbox";

		protected const string XmlElementNameMailboxType = "MailboxType";

		protected const string XmlElementNameManager = "Manager";

		protected const string XmlElementNameMobilePhone = "MobilePhone";

		protected const string XmlElementNameName = "Name";

		protected const string XmlElementNameOfficeLocation = "OfficeLocation";

		protected const string XmlElementNameOtherFax = "OtherFax";

		protected const string XmlElementNameOtherTelephone = "OtherTelephone";

		protected const string XmlElementNamePager = "Pager";

		protected const string XmlElementNamePhoneNumbers = "PhoneNumbers";

		protected const string XmlElementNamePhysicalAddresses = "PhysicalAddresses";

		protected const string XmlElementNamePostalCode = "PostalCode";

		protected const string XmlElementNameResolution = "Resolution";

		protected const string XmlElementNameResolutionSet = "ResolutionSet";

		protected const string XmlElementNameRoutingType = "RoutingType";

		protected const string XmlElementNameState = "State";

		protected const string XmlElementNameStreet = "Street";

		protected const string XmlElementNameSurname = "Surname";

		protected const string XmlElementNameDirectoryId = "DirectoryId";

		protected const string XmlElementNamePhoto = "Photo";

		protected const string XmlElementNameHasPicture = "HasPicture";

		protected const string XmlElementNameUserSMIMECertificate = "UserSMIMECertificate";

		protected const string XmlElementNameMSExchangeCertificate = "MSExchangeCertificate";

		protected const string XmlElementNameBase64Binary = "Base64Binary";

		protected const string XmlElementNameNotes = "Notes";

		protected const string XmlElementNameAlias = "Alias";

		protected const string XmlElementNamePhoneticFullName = "PhoneticFullName";

		protected const string XmlElementNamePhoneticFirstName = "PhoneticFirstName";

		protected const string XmlElementNamePhoneticLastName = "PhoneticLastName";

		protected const string XmlElementNameDirectReports = "DirectReports";

		protected const string XmlElementNameManagerMailbox = "ManagerMailbox";

		protected const int DefaultMaxAmbigiousResults = 100;

		protected static readonly SortBy DirectorySortByName = new SortBy(ADObjectSchema.Name, SortOrder.Ascending);

		protected bool includesLastItemInRange;

		protected IRecipientSession directoryRecipientSession;

		private MailboxSession storeMailboxSession;
	}
}
