using System;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventNamespace("EditContactItem")]
	internal sealed class EditContactItemEventHandler : ItemEventHandler
	{
		public static void Register()
		{
			OwaEventRegistry.RegisterHandler(typeof(EditContactItemEventHandler));
		}

		[OwaEventParameter("m", typeof(string))]
		[OwaEventParameter("hc", typeof(string))]
		[OwaEventParameter("mn", typeof(string), false, true)]
		[OwaEventParameter("mp", typeof(string))]
		[OwaEventParameter("sn", typeof(string))]
		[OwaEventParameter("t", typeof(string))]
		[OwaEventParameter("of", typeof(string))]
		[OwaEventParameter("ol", typeof(string))]
		[OwaEventParameter("hpc", typeof(string))]
		[OwaEventParameter("hct", typeof(string))]
		[OwaEventParameter("ot", typeof(string))]
		[OwaEventParameter("os", typeof(string))]
		[OwaEventParameter("oc", typeof(string))]
		[OwaEventParameter("ost", typeof(string))]
		[OwaEventParameter("opc", typeof(string))]
		[OwaEventParameter("oct", typeof(string))]
		[OwaEventParameter("omp", typeof(string))]
		[OwaEventParameter("p", typeof(string))]
		[OwaEventParameter("pa", typeof(int))]
		[OwaEventParameter("ptn", typeof(string))]
		[OwaEventParameter("rp", typeof(string))]
		[OwaEventParameter("tn", typeof(string))]
		[OwaEventParameter("ttpn", typeof(string))]
		[OwaEventParameter("yf", typeof(string), false, true)]
		[OwaEventParameter("yl", typeof(string), false, true)]
		[OwaEventParameter("wp", typeof(string))]
		[OwaEventParameter("was", typeof(string))]
		[OwaEventParameter("wac", typeof(string))]
		[OwaEventParameter("wast", typeof(string))]
		[OwaEventParameter("wapc", typeof(string))]
		[OwaEventParameter("wact", typeof(string))]
		[OwaEventParameter("notes", typeof(string), false, true)]
		[OwaEventParameter("hst", typeof(string))]
		[OwaEvent("Save")]
		[OwaEventParameter("Id", typeof(OwaStoreObjectId), false, true)]
		[OwaEventParameter("CK", typeof(string), false, true)]
		[OwaEventParameter("fId", typeof(OwaStoreObjectId), false, true)]
		[OwaEventParameter("an", typeof(string))]
		[OwaEventParameter("apn", typeof(string))]
		[OwaEventParameter("bpn", typeof(string))]
		[OwaEventParameter("bpn2", typeof(string))]
		[OwaEventParameter("cn", typeof(string))]
		[OwaEventParameter("cy", typeof(string), false, true)]
		[OwaEventParameter("cbp", typeof(string))]
		[OwaEventParameter("cp", typeof(string))]
		[OwaEventParameter("d", typeof(string))]
		[OwaEventParameter("em1", typeof(string))]
		[OwaEventParameter("em2", typeof(string))]
		[OwaEventParameter("em3", typeof(string))]
		[OwaEventParameter("em1dn", typeof(string))]
		[OwaEventParameter("em2dn", typeof(string))]
		[OwaEventParameter("em3dn", typeof(string))]
		[OwaEventParameter("fa", typeof(int))]
		[OwaEventParameter("fn", typeof(string))]
		[OwaEventParameter("gn", typeof(string))]
		[OwaEventParameter("im", typeof(string))]
		[OwaEventParameter("iin", typeof(string))]
		[OwaEventParameter("hf", typeof(string))]
		[OwaEventParameter("hp", typeof(string))]
		[OwaEventParameter("hp2", typeof(string))]
		[OwaEventParameter("hs", typeof(string))]
		public void Save()
		{
			ExTraceGlobals.ContactsCallTracer.TraceDebug((long)this.GetHashCode(), "EditContactItemEventHandler.Save");
			bool flag = base.IsParameterSet("Id");
			using (Contact contact = this.GetContact(new PropertyDefinition[0]))
			{
				this.SetTextPropertyValue(contact, ContactUtilities.GivenName);
				this.SetTextPropertyValue(contact, ContactUtilities.MiddleName);
				this.SetTextPropertyValue(contact, ContactUtilities.SurName);
				this.SetFileAsValue(contact);
				this.SetTextPropertyValue(contact, ContactUtilities.Title);
				this.SetTextPropertyValue(contact, ContactUtilities.CompanyName);
				this.SetTextPropertyValue(contact, ContactUtilities.Manager);
				this.SetTextPropertyValue(contact, ContactUtilities.AssistantName);
				this.SetTextPropertyValue(contact, ContactUtilities.Department);
				this.SetTextPropertyValue(contact, ContactUtilities.OfficeLocation);
				this.SetTextPropertyValue(contact, ContactUtilities.YomiFirstName);
				this.SetTextPropertyValue(contact, ContactUtilities.YomiLastName);
				this.SetTextPropertyValue(contact, ContactUtilities.CompanyYomi);
				this.SetTextPropertyValue(contact, ContactUtilities.BusinessPhoneNumber);
				this.SetTextPropertyValue(contact, ContactUtilities.HomePhone);
				this.SetTextPropertyValue(contact, ContactUtilities.MobilePhone);
				for (int i = 0; i < ContactUtilities.PhoneNumberProperties.Length; i++)
				{
					this.SetTextPropertyValue(contact, ContactUtilities.PhoneNumberProperties[i]);
				}
				this.SetEmailPropertyValue(contact, ContactUtilities.Email1EmailAddress);
				this.SetEmailPropertyValue(contact, ContactUtilities.Email2EmailAddress);
				this.SetEmailPropertyValue(contact, ContactUtilities.Email3EmailAddress);
				this.SetTextPropertyValue(contact, ContactUtilities.IMAddress);
				this.SetTextPropertyValue(contact, ContactUtilities.WebPage);
				int num = (int)base.GetParameter(ContactUtilities.PostalAddressId.Id);
				contact[ContactUtilities.PostalAddressId.PropertyDefinition] = num;
				this.SetTextPropertyValue(contact, ContactUtilities.WorkAddressStreet);
				this.SetTextPropertyValue(contact, ContactUtilities.WorkAddressCity);
				this.SetTextPropertyValue(contact, ContactUtilities.WorkAddressState);
				this.SetTextPropertyValue(contact, ContactUtilities.WorkAddressPostalCode);
				this.SetTextPropertyValue(contact, ContactUtilities.WorkAddressCountry);
				this.SetTextPropertyValue(contact, ContactUtilities.HomeStreet);
				this.SetTextPropertyValue(contact, ContactUtilities.HomeCity);
				this.SetTextPropertyValue(contact, ContactUtilities.HomeState);
				this.SetTextPropertyValue(contact, ContactUtilities.HomePostalCode);
				this.SetTextPropertyValue(contact, ContactUtilities.HomeCountry);
				this.SetTextPropertyValue(contact, ContactUtilities.OtherStreet);
				this.SetTextPropertyValue(contact, ContactUtilities.OtherCity);
				this.SetTextPropertyValue(contact, ContactUtilities.OtherState);
				this.SetTextPropertyValue(contact, ContactUtilities.OtherPostalCode);
				this.SetTextPropertyValue(contact, ContactUtilities.OtherCountry);
				string text = (string)base.GetParameter("notes");
				if (text != null)
				{
					BodyConversionUtilities.SetBody(contact, text, Markup.PlainText, base.UserContext);
				}
				string textPropertyValue = EditContactItemEventHandler.GetTextPropertyValue(contact, ContactBaseSchema.FileAs);
				if (string.IsNullOrEmpty(textPropertyValue) && !string.IsNullOrEmpty(contact.Company))
				{
					contact[ContactUtilities.FileAsId.PropertyDefinition] = FileAsMapping.Company;
				}
				Utilities.SaveItem(contact);
				contact.Load();
				if (!flag)
				{
					if (ExTraceGlobals.ContactsDataTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.ContactsDataTracer.TraceDebug<string>((long)this.GetHashCode(), "New contact item ID is '{0}'", contact.Id.ObjectId.ToBase64String());
					}
					this.Writer.Write("<div id=itemId>");
					this.Writer.Write(Utilities.GetIdAsString(contact));
					this.Writer.Write("</div>");
				}
				this.Writer.Write("<div id=ck>");
				this.Writer.Write(contact.Id.ChangeKeyAsBase64String());
				this.Writer.Write("</div>");
				string text2 = EditContactItemEventHandler.GetTextPropertyValue(contact, ContactBaseSchema.FileAs);
				if (string.IsNullOrEmpty(text2))
				{
					text2 = LocalizedStrings.GetNonEncoded(-1873027801);
				}
				this.Writer.Write("<div id=fa>");
				Utilities.HtmlEncode(text2, this.Writer);
				this.Writer.Write("</div>");
				base.MoveItemToDestinationFolderIfInScratchPad(contact);
			}
		}

		private static string GetTextPropertyValue(Contact contact, PropertyDefinition property)
		{
			string result = string.Empty;
			string text = contact.TryGetProperty(property) as string;
			if (text != null)
			{
				result = text.Trim();
			}
			return result;
		}

		private Contact GetContact(params PropertyDefinition[] prefetchProperties)
		{
			bool flag = base.IsParameterSet("Id");
			Contact result;
			if (flag)
			{
				result = base.GetRequestItem<Contact>(prefetchProperties);
			}
			else
			{
				ExTraceGlobals.ContactsTracer.TraceDebug((long)this.GetHashCode(), "ItemId is null. Creating new contact item.");
				OwaStoreObjectId folderId = (OwaStoreObjectId)base.GetParameter("fId");
				result = Utilities.CreateItem<Contact>(folderId);
				if (Globals.ArePerfCountersEnabled)
				{
					OwaSingleCounters.ItemsCreated.Increment();
				}
			}
			return result;
		}

		private void SetTextPropertyValue(Contact contact, ContactPropertyInfo propertyInfo)
		{
			string text = (string)base.GetParameter(propertyInfo.Id);
			if (text != null)
			{
				contact[propertyInfo.PropertyDefinition] = text;
			}
		}

		private void SetEmailPropertyValue(Contact contact, ContactPropertyInfo propertyInfo)
		{
			ContactPropertyInfo emailDisplayAsProperty = ContactUtilities.GetEmailDisplayAsProperty(propertyInfo);
			EmailAddressIndex emailPropertyIndex = ContactUtilities.GetEmailPropertyIndex(propertyInfo);
			string email = (string)base.GetParameter(propertyInfo.Id);
			string displayName = (string)base.GetParameter(emailDisplayAsProperty.Id);
			ContactUtilities.SetContactEmailAddress(contact, emailPropertyIndex, email, displayName);
		}

		private void SetFileAsValue(Contact contact)
		{
			int num = (int)base.GetParameter(ContactUtilities.FileAsId.Id);
			contact[ContactUtilities.FileAsId.PropertyDefinition] = (FileAsMapping)num;
		}

		public const string EventNamespace = "EditContactItem";

		public const string AssistantNameId = "an";

		public const string AssistantPhoneNumberId = "apn";

		public const string BusinessPhoneNumberId = "bpn";

		public const string BusinessPhoneNumber2Id = "bpn2";

		public const string CompanyNameId = "cn";

		public const string CallbackPhoneId = "cbp";

		public const string CarPhoneId = "cp";

		public const string CompanyYomiId = "cy";

		public const string DepartmentId = "d";

		public const string Email1EmailAddressId = "em1";

		public const string Email2EmailAddressId = "em2";

		public const string Email3EmailAddressId = "em3";

		public const string Email1DisplayNameId = "em1dn";

		public const string Email2DisplayNameId = "em2dn";

		public const string Email3DisplayNameId = "em3dn";

		public const string FileAsId = "fa";

		public const string FaxNumberId = "fn";

		public const string GivenNameId = "gn";

		public const string IMAddressId = "im";

		public const string InternationalIsdnNumberId = "iin";

		public const string HomeFaxId = "hf";

		public const string HomePhoneId = "hp";

		public const string HomePhone2Id = "hp2";

		public const string HomeStreetId = "hs";

		public const string HomeCityId = "hc";

		public const string HomeStateId = "hst";

		public const string HomePostalCodeId = "hpc";

		public const string HomeCountryId = "hct";

		public const string ManagerId = "m";

		public const string MiddleNameId = "mn";

		public const string MobilePhoneId = "mp";

		public const string SurNameId = "sn";

		public const string TitleId = "t";

		public const string OtherFaxId = "of";

		public const string OfficeLocationId = "ol";

		public const string OtherTelephoneId = "ot";

		public const string OtherStreetId = "os";

		public const string OtherCityId = "oc";

		public const string OtherStateId = "ost";

		public const string OtherPostalCodeId = "opc";

		public const string OtherCountryId = "oct";

		public const string OrganizationMainPhoneId = "omp";

		public const string PagerId = "p";

		public const string PostalAddressIdId = "pa";

		public const string PrimaryTelephoneNumberId = "ptn";

		public const string RadioPhoneId = "rp";

		public const string TelexNumberId = "tn";

		public const string TtyTddPhoneNumberId = "ttpn";

		public const string YomiFirstNameId = "yf";

		public const string YomiLastNameId = "yl";

		public const string WebPageId = "wp";

		public const string WorkAddressStreetId = "was";

		public const string WorkAddressCityId = "wac";

		public const string WorkAddressStateId = "wast";

		public const string WorkAddressPostalCodeId = "wapc";

		public const string WorkAddressCountryId = "wact";

		public const string Notes = "notes";
	}
}
