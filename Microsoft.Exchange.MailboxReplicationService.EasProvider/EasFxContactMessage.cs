using System;
using System.Collections.Generic;
using Microsoft.Exchange.Connections.Eas.Model.Response.AirSync;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class EasFxContactMessage : IMessage, IDisposable
	{
		public EasFxContactMessage(MessageRec messageRec)
		{
			ArgumentValidator.ThrowIfNull("messageRec", messageRec);
			this.propertyBag = EasFxContactMessage.CreatePropertyBag(messageRec);
		}

		IPropertyBag IMessage.PropertyBag
		{
			get
			{
				return this.propertyBag;
			}
		}

		bool IMessage.IsAssociated
		{
			get
			{
				return false;
			}
		}

		IEnumerable<IRecipient> IMessage.GetRecipients()
		{
			yield break;
		}

		IRecipient IMessage.CreateRecipient()
		{
			throw new NotSupportedException();
		}

		void IMessage.RemoveRecipient(int rowId)
		{
			throw new NotSupportedException();
		}

		IEnumerable<IAttachmentHandle> IMessage.GetAttachments()
		{
			yield break;
		}

		IAttachment IMessage.CreateAttachment()
		{
			throw new NotSupportedException();
		}

		void IMessage.Save()
		{
			throw new NotSupportedException();
		}

		void IMessage.SetLongTermId(StoreLongTermId longTermId)
		{
			throw new NotSupportedException();
		}

		void IDisposable.Dispose()
		{
		}

		internal static List<PropValueData> GetContactProperties(ApplicationData applicationData)
		{
			List<PropValueData> list = new List<PropValueData>();
			list.Add(new PropValueData(PropTag.PartnerNetworkId, "outlook.com"));
			EasFxContactMessage.AddToContactPropertiesIfValid(list, PropTag.WeddingAnniversary, applicationData.Birthday);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, PropTag.Assistant, applicationData.AssistantName);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, PropTag.AssistantTelephoneNumber, applicationData.AssistantPhoneNumber);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, PropTag.Birthday, applicationData.Birthday);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, PropTag.Business2TelephoneNumber, applicationData.Business2PhoneNumber);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, (PropTag)SyncContactSchema.WorkAddressCity, applicationData.BusinessAddressCity);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, PropTag.BusinessTelephoneNumber, applicationData.BusinessPhoneNumber);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, (PropTag)SyncContactSchema.LegacyWebPage, applicationData.WebPage);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, (PropTag)SyncContactSchema.WorkAddressCountry, applicationData.BusinessAddressCountry);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, PropTag.DepartmentName, applicationData.Department);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, (PropTag)SyncContactSchema.Email1EmailAddress, applicationData.Email1Address);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, (PropTag)SyncContactSchema.Email2EmailAddress, applicationData.Email2Address);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, (PropTag)SyncContactSchema.Email3EmailAddress, applicationData.Email3Address);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, PropTag.BusinessFaxNumber, applicationData.BusinessFaxNumber);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, (PropTag)SyncContactSchema.FileAsStringInternal, applicationData.FileAs);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, PropTag.NormalizedSubject, applicationData.FileAs);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, PropTag.GivenName, applicationData.FirstName);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, PropTag.Account, applicationData.FirstName);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, PropTag.MiddleName, applicationData.MiddleName);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, PropTag.HomeAddressCity, applicationData.HomeAddressCity);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, PropTag.HomeAddressCountry, applicationData.HomeAddressCountry);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, PropTag.HomeFaxNumber, applicationData.HomeFaxNumber);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, PropTag.HomeTelephoneNumber, applicationData.HomePhoneNumber);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, PropTag.Home2TelephoneNumber, applicationData.Home2PhoneNumber);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, PropTag.HomeAddressPostalCode, applicationData.HomeAddressPostalCode);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, PropTag.HomeAddressStateOrProvince, applicationData.HomeAddressState);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, PropTag.HomeAddressStreet, applicationData.HomeAddressStreet);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, PropTag.MobileTelephoneNumber, applicationData.MobilePhoneNumber);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, PropTag.CompanyName, applicationData.CompanyName);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, PropTag.OtherAddressCity, applicationData.OtherAddressCity);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, PropTag.OtherAddressCountry, applicationData.OtherAddressCountry);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, PropTag.CarTelephoneNumber, applicationData.CarPhoneNumber);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, PropTag.OtherAddressPostalCode, applicationData.OtherAddressPostalCode);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, PropTag.OtherAddressStateOrProvince, applicationData.OtherAddressState);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, PropTag.OtherAddressStreet, applicationData.OtherAddressStreet);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, PropTag.PagerTelephoneNumber, applicationData.PagerNumber);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, (PropTag)SyncContactSchema.WorkAddressPostalCode, applicationData.BusinessAddressPostalCode);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, PropTag.Surname, applicationData.LastName);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, PropTag.SpouseName, applicationData.Spouse);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, (PropTag)SyncContactSchema.WorkAddressState, applicationData.BusinessAddressState);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, (PropTag)SyncContactSchema.WorkAddressStreet, applicationData.BusinessAddressStreet);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, PropTag.Title, applicationData.JobTitle);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, (PropTag)SyncContactSchema.YomiFirstName, applicationData.YomiFirstName);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, (PropTag)SyncContactSchema.YomiLastName, applicationData.YomiLastName);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, (PropTag)SyncContactSchema.YomiCompany, applicationData.YomiCompanyName);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, PropTag.OfficeLocation, applicationData.OfficeLocation);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, PropTag.RadioTelephoneNumber, applicationData.RadioPhoneNumber);
			EasFxContactMessage.AddToContactPropertiesIfValid(list, PropTag.PartnerNetworkThumbnailPhotoUrl, applicationData.Picture);
			if (applicationData.FirstName != null && applicationData.LastName != null)
			{
				EasFxContactMessage.AddToContactPropertiesIfValid(list, (PropTag)SyncContactSchema.DisplayNameFirstLast, applicationData.FirstName + " " + applicationData.LastName);
				EasFxContactMessage.AddToContactPropertiesIfValid(list, (PropTag)SyncContactSchema.DisplayNameLastFirst, applicationData.LastName + " " + applicationData.FirstName);
			}
			return list;
		}

		private static void AddToContactPropertiesIfValid(List<PropValueData> contactProperties, PropTag propTag, object valueToSet)
		{
			if (valueToSet != null)
			{
				contactProperties.Add(new PropValueData(propTag, valueToSet));
			}
		}

		private static FxPropertyBag CreatePropertyBag(MessageRec messageRec)
		{
			FxPropertyBag result = new FxPropertyBag(new FxSession(SyncContactSchema.PropertyTagToNamedProperties));
			foreach (PropertyTag propertyTag in SyncContactSchema.AllContactPropertyTags)
			{
				EasFxContactMessage.SetIfValid(result, propertyTag, messageRec[(PropTag)propertyTag]);
			}
			return result;
		}

		private static void SetIfValid(FxPropertyBag propertyBag, PropertyTag propertyTag, object valueToSet)
		{
			if (valueToSet != null)
			{
				if (propertyTag.PropertyType == PropertyType.SysTime)
				{
					DateTime dateTime;
					if (DateTime.TryParse(valueToSet.ToString(), out dateTime))
					{
						propertyBag[propertyTag] = new ExDateTime(ExTimeZone.UtcTimeZone, dateTime);
						return;
					}
				}
				else
				{
					propertyBag[propertyTag] = valueToSet;
				}
			}
		}

		private readonly IPropertyBag propertyBag;
	}
}
