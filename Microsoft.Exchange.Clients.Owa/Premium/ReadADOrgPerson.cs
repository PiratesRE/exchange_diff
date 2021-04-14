using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.Availability;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class ReadADOrgPerson : ReadADRecipientPage, IRegistryOnlyForm
	{
		protected bool IsMailEnabledUser()
		{
			SmtpAddress primarySmtpAddress = base.ADRecipient.PrimarySmtpAddress;
			return primarySmtpAddress != SmtpAddress.Empty && primarySmtpAddress != SmtpAddress.NullReversePath && primarySmtpAddress.IsValidAddress;
		}

		protected bool IsAdOrgPerson
		{
			get
			{
				return this.isAdOrgPerson;
			}
		}

		protected int GetWorkDayStartHour
		{
			get
			{
				return SchedulingTabRenderingUtilities.GetWorkDayStartHour(base.UserContext.WorkingHours, DateTimeUtilities.GetLocalTime());
			}
		}

		protected int GetWorkDayEndHour
		{
			get
			{
				return SchedulingTabRenderingUtilities.GetWorkDayEndHour(base.UserContext.WorkingHours, DateTimeUtilities.GetLocalTime());
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (base.ADRecipient is IADOrgPerson)
			{
				this.orgPerson = (base.ADRecipient as IADOrgPerson);
			}
			else
			{
				this.isAdOrgPerson = false;
			}
			if (ADCustomPropertyParser.CustomPropertyDictionary != null && ADCustomPropertyParser.CustomPropertyDictionary.Count > 0)
			{
				this.adRawEntry = base.ADRecipientSession.ReadADRawEntry(base.ADObjectId, ADCustomPropertyParser.CustomPropertyDictionary.Values);
				if (this.adRawEntry != null)
				{
					this.renderCustomProperties = true;
				}
			}
			this.startDay = DateTimeUtilities.GetLocalTime().Date;
			this.endDay = this.startDay.IncrementDays(7);
		}

		protected void RenderData()
		{
			if (base.RecipientOutOfSearchScope)
			{
				this.RenderDataForRecipientOutofScope();
				return;
			}
			this.RenderDataForFullRecipient();
		}

		private void RenderDataForRecipientOutofScope()
		{
			List<ReadADOrgPerson.DisplayField> list = new List<ReadADOrgPerson.DisplayField>();
			list.Add(new ReadADOrgPerson.DisplayField(LocalizedStrings.GetHtmlEncoded(1111077458), base.ADRecipient.PrimarySmtpAddress.ToString(), false));
			this.RenderBucket(LocalizedStrings.GetHtmlEncoded(447307630), list, true);
		}

		private void RenderDataForFullRecipient()
		{
			List<ReadADOrgPerson.DisplayField> list = new List<ReadADOrgPerson.DisplayField>();
			List<ReadADOrgPerson.DisplayField> list2 = new List<ReadADOrgPerson.DisplayField>();
			List<ReadADOrgPerson.DisplayField> list3 = new List<ReadADOrgPerson.DisplayField>();
			if (this.isAdOrgPerson)
			{
				this.GetAssistant();
			}
			this.FillContactBucket(list);
			if (this.isAdOrgPerson)
			{
				this.FillInformationBucket(list2);
				if (this.renderCustomProperties)
				{
					this.FillCustomPropertiesBucket(list3);
				}
			}
			this.RenderBucket(LocalizedStrings.GetHtmlEncoded(447307630), list, true);
			if (this.isAdOrgPerson)
			{
				this.RenderBucket(LocalizedStrings.GetHtmlEncoded(-2101430728), list2, false);
				this.RenderSecureMessaging();
				this.RenderOrganization();
				if (this.IsMailEnabledUser() && base.UserContext.IsFeatureEnabled(Feature.Calendar) && base.UserContext.IsInternetExplorer7())
				{
					this.RenderAvailability();
				}
				this.RenderAddressList();
				this.RenderNotes();
				this.RenderBucket(LocalizedStrings.GetHtmlEncoded(-582599340), list3, false);
			}
		}

		private void GetAssistant()
		{
			if (!string.IsNullOrEmpty(this.orgPerson.AssistantName))
			{
				ADRecipient[] array = base.ADRecipientSession.FindByANR(this.orgPerson.AssistantName, 2, null);
				if (array != null && array.Length == 1 && array[0].RecipientType == RecipientType.UserMailbox)
				{
					this.assistant = (IADOrgPerson)array[0];
				}
			}
		}

		private void FillCustomPropertiesBucket(List<ReadADOrgPerson.DisplayField> otherBucket)
		{
			foreach (KeyValuePair<string, PropertyDefinition> keyValuePair in ADCustomPropertyParser.CustomPropertyDictionary)
			{
				string value = this.adRawEntry[keyValuePair.Value].ToString();
				if (!string.IsNullOrEmpty(value))
				{
					otherBucket.Add(new ReadADOrgPerson.DisplayField(keyValuePair.Key, value, false));
				}
			}
		}

		private void FillContactBucket(List<ReadADOrgPerson.DisplayField> contactBucket)
		{
			if (!string.IsNullOrEmpty(base.ADRecipient.Alias) && base.ADRecipient.DisplayName != null && !string.IsNullOrEmpty(base.ADRecipient.LegacyExchangeDN))
			{
				string displayName = base.ADRecipient.DisplayName;
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("<a class=lnk ");
				string handlerCode = string.Format("opnNwMsg(\"{0}\",\"{1}\",\"\",\"{2}\");", Utilities.JavascriptEncode(base.ADRecipient.LegacyExchangeDN), Utilities.JavascriptEncode(displayName), Utilities.JavascriptEncode(2.ToString()));
				stringBuilder.Append(Utilities.GetScriptHandler("onclick", handlerCode));
				stringBuilder.Append(">");
				stringBuilder.Append(Utilities.HtmlEncode(base.ADRecipient.Alias));
				stringBuilder.Append("</a>");
				contactBucket.Add(new ReadADOrgPerson.DisplayField(LocalizedStrings.GetHtmlEncoded(613689222), stringBuilder.ToString(), true));
				if (base.UserContext.IsPhoneticNamesEnabled && !string.IsNullOrEmpty(base.ADRecipient.PhoneticDisplayName))
				{
					contactBucket.Add(new ReadADOrgPerson.DisplayField(LocalizedStrings.GetHtmlEncoded(1180356027), base.ADRecipient.PhoneticDisplayName, false));
				}
				contactBucket.Add(new ReadADOrgPerson.DisplayField(LocalizedStrings.GetHtmlEncoded(1111077458), base.ADRecipient.PrimarySmtpAddress.ToString(), false));
			}
			if (!this.isAdOrgPerson)
			{
				return;
			}
			if (!string.IsNullOrEmpty(this.orgPerson.Office))
			{
				contactBucket.Add(new ReadADOrgPerson.DisplayField(LocalizedStrings.GetHtmlEncoded(275231482), this.orgPerson.Office, false));
			}
			if (!string.IsNullOrEmpty(this.orgPerson.Phone))
			{
				contactBucket.Add(new ReadADOrgPerson.DisplayField(LocalizedStrings.GetHtmlEncoded(-31489650), this.orgPerson.Phone, false));
			}
			if (this.assistant != null && !string.IsNullOrEmpty(this.assistant.Phone))
			{
				contactBucket.Add(new ReadADOrgPerson.DisplayField(LocalizedStrings.GetHtmlEncoded(425094986), this.assistant.Phone, false));
			}
			if (!string.IsNullOrEmpty(this.orgPerson.Fax))
			{
				contactBucket.Add(new ReadADOrgPerson.DisplayField(LocalizedStrings.GetHtmlEncoded(696030351), this.orgPerson.Fax, false));
			}
			if (!string.IsNullOrEmpty(this.orgPerson.HomePhone))
			{
				contactBucket.Add(new ReadADOrgPerson.DisplayField(LocalizedStrings.GetHtmlEncoded(-1844864953), this.orgPerson.HomePhone, false));
			}
			if (this.orgPerson.OtherHomePhone != null)
			{
				foreach (string value in this.orgPerson.OtherHomePhone)
				{
					if (!string.IsNullOrEmpty(value))
					{
						contactBucket.Add(new ReadADOrgPerson.DisplayField(LocalizedStrings.GetHtmlEncoded(1762364627), value, false));
					}
				}
			}
			if (!string.IsNullOrEmpty(this.orgPerson.MobilePhone))
			{
				contactBucket.Add(new ReadADOrgPerson.DisplayField(LocalizedStrings.GetHtmlEncoded(1158653436), this.orgPerson.MobilePhone, false));
			}
			if (this.orgPerson.OtherFax != null)
			{
				foreach (string value2 in this.orgPerson.OtherFax)
				{
					if (!string.IsNullOrEmpty(value2))
					{
						contactBucket.Add(new ReadADOrgPerson.DisplayField(LocalizedStrings.GetHtmlEncoded(-679895069), value2, false));
					}
				}
			}
			if (!string.IsNullOrEmpty(this.orgPerson.Pager))
			{
				contactBucket.Add(new ReadADOrgPerson.DisplayField(LocalizedStrings.GetHtmlEncoded(-1779142331), this.orgPerson.Pager, false));
			}
		}

		private void FillInformationBucket(List<ReadADOrgPerson.DisplayField> informationBucket)
		{
			if (!string.IsNullOrEmpty(this.orgPerson.Title))
			{
				informationBucket.Add(new ReadADOrgPerson.DisplayField(LocalizedStrings.GetHtmlEncoded(587115635), this.orgPerson.Title, false));
			}
			if (base.UserContext.IsPhoneticNamesEnabled && !string.IsNullOrEmpty(this.orgPerson.PhoneticDepartment))
			{
				informationBucket.Add(new ReadADOrgPerson.DisplayField(LocalizedStrings.GetHtmlEncoded(871410780), this.orgPerson.PhoneticDepartment, false));
			}
			if (!string.IsNullOrEmpty(this.orgPerson.Department))
			{
				informationBucket.Add(new ReadADOrgPerson.DisplayField(LocalizedStrings.GetHtmlEncoded(1855823700), this.orgPerson.Department, false));
			}
			if (base.UserContext.IsPhoneticNamesEnabled && !string.IsNullOrEmpty(this.orgPerson.PhoneticCompany))
			{
				informationBucket.Add(new ReadADOrgPerson.DisplayField(LocalizedStrings.GetHtmlEncoded(-923446215), this.orgPerson.PhoneticCompany, false));
			}
			if (!string.IsNullOrEmpty(this.orgPerson.Company))
			{
				informationBucket.Add(new ReadADOrgPerson.DisplayField(LocalizedStrings.GetHtmlEncoded(642177943), this.orgPerson.Company, false));
			}
			if (this.assistant != null && !string.IsNullOrEmpty(this.assistant.DisplayName))
			{
				string value = ReadADOrgPerson.RenderPersonLink(this.assistant.Id, this.assistant.DisplayName);
				informationBucket.Add(new ReadADOrgPerson.DisplayField(LocalizedStrings.GetHtmlEncoded(425094986), value, true));
			}
			if (this.assistant == null && !string.IsNullOrEmpty(this.orgPerson.AssistantName))
			{
				informationBucket.Add(new ReadADOrgPerson.DisplayField(LocalizedStrings.GetHtmlEncoded(425094986), this.orgPerson.AssistantName, false));
			}
			if (this.orgPerson.Manager != null)
			{
				ADRecipient adrecipient = new ADRecipient();
				adrecipient = base.ADRecipientSession.Read(this.orgPerson.Manager);
				if (adrecipient != null && !string.IsNullOrEmpty(adrecipient.DisplayName))
				{
					string value2 = ReadADOrgPerson.RenderPersonLink(adrecipient.Id, adrecipient.DisplayName);
					informationBucket.Add(new ReadADOrgPerson.DisplayField(LocalizedStrings.GetHtmlEncoded(-128712621), value2, true));
				}
			}
		}

		private void RenderBucket(string bucketLabel, List<ReadADOrgPerson.DisplayField> bucket, bool isFirstBucket)
		{
			if (bucket.Count == 0)
			{
				return;
			}
			if (!isFirstBucket)
			{
				base.Response.Write("<div class=\"row2sp\">");
			}
			else
			{
				base.Response.Write("<div class=\"row\">");
			}
			this.RenderBucketLabel(bucketLabel);
			this.RenderLabelValue(bucket[0].Label, bucket[0].Value, bucket[0].Link, true);
			base.Response.Write("</div>");
			bucket.Remove(bucket[0]);
			foreach (ReadADOrgPerson.DisplayField displayField in bucket)
			{
				if (!string.IsNullOrEmpty(displayField.Label) && !string.IsNullOrEmpty(displayField.Value))
				{
					this.RenderLabelValueRow(displayField.Label, displayField.Value, displayField.Link);
				}
			}
		}

		private void RenderBucketLabel(string bucketLabel)
		{
			base.Response.Write("<div class=\"secCol\"><span class=\"spS\">" + bucketLabel + "</span></div>");
		}

		private void RenderLabelValueRow(string label, string value, bool link)
		{
			if (string.IsNullOrEmpty(label) || string.IsNullOrEmpty(value))
			{
				return;
			}
			base.Response.Write("<div class=\"trAlr row\">");
			this.RenderLabelValue(label, value, link);
			base.Response.Write("</div>");
		}

		private void RenderLabelValue(string label, string value, bool link)
		{
			this.RenderLabelValue(label, value, link, false);
		}

		private void RenderLabelValue(string label, string value, bool link, bool isFirstRow)
		{
			if (string.IsNullOrEmpty(label) || string.IsNullOrEmpty(value))
			{
				return;
			}
			if (isFirstRow)
			{
				base.Response.Write("<div class=\"lbl noindent\">");
			}
			else
			{
				base.Response.Write("<div class=\"lbl\">");
			}
			base.Response.Write(label);
			base.Response.Write("</div><div class=\"fld\">");
			string[] array = value.Split(new char[]
			{
				'\n',
				'\r'
			}, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array.Length; i++)
			{
				if (!link)
				{
					Utilities.HtmlEncode(array[i], base.Response.Output);
				}
				else
				{
					base.Response.Write(array[i]);
				}
				if (i < array.Length - 1)
				{
					base.Response.Write("<br>");
				}
			}
			base.Response.Write("</div>");
		}

		protected void RenderToolbar()
		{
			if (!base.IsPreviewForm)
			{
				ReadADOrgPersonToolbar readADOrgPersonToolbar = new ReadADOrgPersonToolbar();
				readADOrgPersonToolbar.Render(base.Response.Output);
			}
		}

		private void RenderOrganization()
		{
			base.Response.Write("<div class=\"row2sp\"><div class=\"secCol adOrg\" ");
			Utilities.RenderScriptHandler(base.Response.Output, "onclick", "expCol();");
			base.Response.Write("><span class=spSOrg>" + LocalizedStrings.GetHtmlEncoded(-905993889) + "</span></div><div class=\"adOrg\"");
			Utilities.RenderScriptHandler(base.Response.Output, "onclick", "expCol();");
			base.Response.Write(">");
			base.UserContext.RenderThemeImage(base.Response.Output, ThemeFileId.Expand, "adArrw", new object[]
			{
				"id=expIm"
			});
			base.Response.Write("</div></div><div id=\"orgTree\" class=\"clear\" style=\"display:none\">" + LocalizedStrings.GetHtmlEncoded(-695375226) + "</div>");
		}

		private void RenderAddressList()
		{
			if (string.IsNullOrEmpty(this.orgPerson.StreetAddress) && string.IsNullOrEmpty(this.orgPerson.City) && string.IsNullOrEmpty(this.orgPerson.StateOrProvince) && string.IsNullOrEmpty(this.orgPerson.PostalCode) && string.IsNullOrEmpty(this.orgPerson.CountryOrRegionDisplayName))
			{
				return;
			}
			base.Response.Write(string.Concat(new string[]
			{
				"<div class=\"row2sp\"><div class=\"secCol\"><span class=spS>",
				LocalizedStrings.GetHtmlEncoded(-1159205642),
				"</span></div><div class=bLn>(",
				LocalizedStrings.GetHtmlEncoded(1912536019),
				")</div></div>"
			}));
			IDictionary<AddressFormatTable.AddressPart, AddressComponent> addressInfo = ContactUtilities.GetAddressInfo(this.orgPerson);
			foreach (KeyValuePair<AddressFormatTable.AddressPart, AddressComponent> keyValuePair in addressInfo)
			{
				this.RenderLabelValueRow(keyValuePair.Value.Label, keyValuePair.Value.Value, false);
			}
		}

		private void RenderNotes()
		{
			if (string.IsNullOrEmpty(this.orgPerson.Notes))
			{
				return;
			}
			base.Response.Write("<div class=\"row2sp\"><div class=\"secCol\"><span class=spS>" + LocalizedStrings.GetHtmlEncoded(1601836855) + "</span></div><div class=\"fltBefore\"><textarea class=\"adNts\" readonly>");
			Utilities.HtmlEncode(this.orgPerson.Notes, base.Response.Output);
			base.Response.Write("</textarea></div>");
		}

		private static void RenderManagerList(TextWriter writer, UserContext userContext, object[][] peopleList)
		{
			if (peopleList.Length <= 1)
			{
				return;
			}
			int num = peopleList.Length - 1;
			int num2 = 0;
			if (num > 25)
			{
				num2 = num - 25;
			}
			ReadADOrgPerson.RenderPeopleLabelHeading(writer, LocalizedStrings.GetHtmlEncoded(-108103848));
			for (int i = num2; i < num; i++)
			{
				ReadADOrgPerson.RenderPeopleRow(writer, (ADObjectId)peopleList[i][0], (string)peopleList[i][1], (string)peopleList[i][2], i != 0, userContext);
			}
		}

		private static void RenderSelf(TextWriter writer, ADObjectId selfId, string selfDisplayName, string selfTitle)
		{
			writer.Write("<div class=adSlf><span class=adSlfNm>");
			Utilities.HtmlEncode(selfDisplayName, writer);
			writer.Write("</span>");
			if (!string.IsNullOrEmpty(selfTitle))
			{
				Utilities.HtmlEncode(", " + selfTitle, writer);
			}
			writer.Write("</div>");
		}

		private static void RenderPeersList(TextWriter writer, object[][] peopleList, ADObjectId selfId, string selfDisplayName, string selfTitle)
		{
			if (peopleList.Length <= 0)
			{
				return;
			}
			ReadADOrgPerson.RenderPeopleLabelHeading(writer, LocalizedStrings.GetHtmlEncoded(1804838102));
			ReadADOrgPerson.RenderSelf(writer, selfId, selfDisplayName, selfTitle);
			List<ReadADOrgPerson.ADMember> list = new List<ReadADOrgPerson.ADMember>();
			for (int i = 0; i < peopleList.Length; i++)
			{
				ADObjectId adobjectId = (ADObjectId)peopleList[i][0];
				if (selfId.ObjectGuid.ToString() != adobjectId.ObjectGuid.ToString())
				{
					ReadADOrgPerson.ADMember item = new ReadADOrgPerson.ADMember(adobjectId, (string)peopleList[i][1], (string)peopleList[i][2]);
					list.Add(item);
				}
			}
			list.Sort();
			foreach (ReadADOrgPerson.ADMember admember in list)
			{
				ReadADOrgPerson.RenderPeopleRow(writer, admember.Id, admember.DisplayName, admember.Title);
			}
		}

		private static void RenderDirectReportsList(TextWriter writer, object[][] peopleList)
		{
			if (peopleList.Length <= 0)
			{
				return;
			}
			int num = peopleList.Length;
			ReadADOrgPerson.RenderPeopleLabelHeading(writer, LocalizedStrings.GetHtmlEncoded(849014173));
			List<ReadADOrgPerson.ADMember> list = new List<ReadADOrgPerson.ADMember>();
			for (int i = 0; i < num; i++)
			{
				ReadADOrgPerson.ADMember item = new ReadADOrgPerson.ADMember((ADObjectId)peopleList[i][0], (string)peopleList[i][1], (string)peopleList[i][2]);
				list.Add(item);
			}
			list.Sort();
			foreach (ReadADOrgPerson.ADMember admember in list)
			{
				ReadADOrgPerson.RenderPeopleRow(writer, admember.Id, admember.DisplayName, admember.Title);
			}
		}

		private static void RenderPeopleLabelHeading(TextWriter writer, string label)
		{
			writer.Write("<div class=\"noindent adMgHd\"> ");
			writer.Write(label);
			writer.Write("</div>");
		}

		private static void RenderPeopleRow(TextWriter writer, ADObjectId personId, string displayName, string title)
		{
			ReadADOrgPerson.RenderPeopleRow(writer, personId, displayName, title, false, null);
		}

		private static void RenderPeopleRow(TextWriter writer, ADObjectId personId, string displayName, string title, bool hasArrow, UserContext userContext)
		{
			writer.Write("<div class=\"adPrsn\">");
			if (hasArrow)
			{
				userContext.RenderThemeImage(writer, ThemeFileId.OrganizationUpArrow, "orgArwUp", new object[0]);
			}
			writer.Write(ReadADOrgPerson.RenderPersonLink(personId, displayName));
			if (!string.IsNullOrEmpty(title))
			{
				Utilities.HtmlEncode(", " + title, writer);
			}
			writer.Write("</div>");
		}

		private static string RenderPersonLink(ADObjectId personId, string displayName)
		{
			string s = "AD.RecipientType.User";
			string base64StringFromADObjectId = Utilities.GetBase64StringFromADObjectId(personId);
			string handlerCode = string.Format("openItmRdFm(\"{0}\",\"{1}\");", Utilities.JavascriptEncode(s), Utilities.JavascriptEncode(base64StringFromADObjectId));
			return string.Concat(new object[]
			{
				"<a class=lnk ",
				Utilities.GetScriptHandler("onclick", handlerCode),
				">",
				Utilities.HtmlEncode(displayName),
				"</a>"
			});
		}

		protected void RenderAvailability()
		{
			base.Response.Write("<div class=\"row2sp\"><div class=\"secCol\"><span class=spS>");
			base.Response.Write(LocalizedStrings.GetHtmlEncoded(136984541));
			base.Response.Write("</span></div><div class=\"dtPkerAd\" id=\"tdDPkr\">");
			this.RenderDatePicker();
			base.Response.Write("</div><div class=\"fltBefore\" id=tdADUsrWH><input id=chkShwWH type=\"checkbox\" ");
			if (base.UserContext.WorkingHours.IsTimeZoneDifferent)
			{
				base.Response.Write("disabled ");
			}
			else
			{
				base.Response.Write("checked ");
				Utilities.RenderScriptHandler(base.Response.Output, "onclick", "cnvrtWkHrs();");
			}
			base.Response.Write(">");
			base.Response.Write(string.Concat(new string[]
			{
				LocalizedStrings.GetHtmlEncoded(907076665),
				"</div></div><div class=\"row2sp\"><div class=\"indent\" id=trFbLd><div id=tdFbLd>",
				LocalizedStrings.GetHtmlEncoded(-695375226),
				"</div></div></div><div class=\"clear indent\" id=\"trFBG\" style=\"display:none\"><div class=\"divFbScrl\" id=\"divFbScrl\" onscroll=\"onSrl();\"><div class=\"fsB\" id=\"divFmtBr\"><table class=\"dyTmsAd\"><tbody id=\"dyTmsAd\"><tr class=\"adHdr\"><td class=\"hdrRw1\">&nbsp;</td><td id=dyTmHdr class=hdrRw2><table class=rcpHdrAd><tr class=dy id=dys></tr><tr class=t id=tys></tr></table></td></tr><tr><td colspan=2 class=rcpLwrTd><table class=rcpLwr><tr class=tck id=tcks><td style=\"width:4px; border:0;\"></td></tr><tr class=hrs id=hrs><td style=\"width:4px; border-left:0;\"></td></tr></table></td></tr></tbody></table><div class=lns id=lns><span class=ln style=\"margin-",
				base.UserContext.IsRtl ? "right" : "left",
				":2px\">&nbsp;</span></div><div class=frBsy id=frBsy></div></div></div></div>"
			}));
		}

		protected void GetRecipientPrimarySmtpAddress()
		{
			Utilities.JavascriptEncode(base.ADRecipient.PrimarySmtpAddress.ToString(), base.Response.Output);
		}

		protected void RenderJavascriptEncodedDisplayName()
		{
			string s = (base.ADRecipient.DisplayName != null) ? base.ADRecipient.DisplayName : string.Empty;
			Utilities.JavascriptEncode(s, base.Response.Output);
		}

		protected void RenderJavascriptEncodedSipUri()
		{
			string text = null;
			if (base.UserContext.InstantMessagingType == InstantMessagingTypeOptions.Ocs)
			{
				text = InstantMessageUtilities.GetSipUri(base.ADRecipient.EmailAddresses);
			}
			Utilities.JavascriptEncode((text != null) ? text : string.Empty, base.Response.Output);
		}

		protected void RenderJavascriptEncodedLegacyDN()
		{
			string text = base.ADRecipient.LegacyExchangeDN.ToString();
			Utilities.JavascriptEncode(text ?? string.Empty, base.Response.Output);
		}

		protected void GetStartDate()
		{
			RenderingUtilities.RenderDateTimeScriptObject(base.Response.Output, this.startDay);
		}

		protected void GetEndDate()
		{
			RenderingUtilities.RenderDateTimeScriptObject(base.Response.Output, this.endDay);
		}

		protected void RenderDatePicker()
		{
			DatePickerDropDownCombo.RenderDatePicker(base.Response.Output, "divSDate", this.startDay);
		}

		public static string GetFreeBusy(OwaContext owaContext, string recipientPrimarySmtpAddress, ExDateTime startDate, ExDateTime endDate, HttpContext httpContext, out string allDayFreeBusy, out string workingHoursFreeBusy, out string oofMessage)
		{
			UserContext userContext = owaContext.UserContext;
			ExTraceGlobals.CalendarCallTracer.TraceDebug(0L, "DatePickerEventHandler.GetFreeBusy");
			allDayFreeBusy = null;
			workingHoursFreeBusy = null;
			oofMessage = null;
			AvailabilityQuery availabilityQuery = new AvailabilityQuery();
			if (HttpContext.Current != null)
			{
				availabilityQuery.HttpResponse = HttpContext.Current.Response;
			}
			availabilityQuery.DesiredFreeBusyView = new FreeBusyViewOptions
			{
				RequestedView = FreeBusyViewType.FreeBusyMerged,
				MergedFreeBusyIntervalInMinutes = 30,
				TimeWindow = new Duration((DateTime)startDate, (DateTime)endDate)
			};
			availabilityQuery.ClientContext = ClientContext.Create(owaContext.LogonIdentity.ClientSecurityContext, userContext.ExchangePrincipal.MailboxInfo.OrganizationId, owaContext.Budget, userContext.TimeZone, userContext.UserCulture, AvailabilityQuery.CreateNewMessageId());
			availabilityQuery.MailboxArray = new MailboxData[1];
			availabilityQuery.MailboxArray[0] = new MailboxData();
			availabilityQuery.MailboxArray[0].Email = new EmailAddress();
			availabilityQuery.MailboxArray[0].Email.Address = recipientPrimarySmtpAddress.ToString();
			ExTraceGlobals.CalendarTracer.TraceDebug<ExDateTime, ExDateTime>(0L, "Getting free/busy data from {0} to {1}", startDate, endDate);
			Stopwatch watch = Utilities.StartWatch();
			AvailabilityQueryResult availabilityQueryResult;
			bool flag = Utilities.ExecuteAvailabilityQuery(owaContext, availabilityQuery, true, true, out availabilityQueryResult);
			Utilities.StopWatch(watch, "ReadADOrgPerson.GetFreeBusy (Execute Availability Query)");
			if (flag)
			{
				FreeBusyQueryResult freeBusyQueryResult = availabilityQueryResult.FreeBusyResults[0];
				if (freeBusyQueryResult != null)
				{
					allDayFreeBusy = freeBusyQueryResult.MergedFreeBusy;
				}
				if (userContext.WorkingHours.IsTimeZoneDifferent)
				{
					workingHoursFreeBusy = string.Empty;
				}
				else
				{
					int workDayStartHour = SchedulingTabRenderingUtilities.GetWorkDayStartHour(userContext.WorkingHours, startDate);
					int workDayEndHour = SchedulingTabRenderingUtilities.GetWorkDayEndHour(userContext.WorkingHours, endDate);
					int num = workDayEndHour - workDayStartHour;
					int num2 = 24 - workDayEndHour;
					StringBuilder stringBuilder = new StringBuilder(num * 24);
					int i = 0;
					SchedulingTabRenderingUtilities.SetFreeBusyDayLightBasedValue(startDate, endDate, userContext.TimeZone, ref allDayFreeBusy);
					while (i < allDayFreeBusy.Length)
					{
						i += workDayStartHour * 2;
						int num3;
						if (i + num * 2 >= allDayFreeBusy.Length)
						{
							num3 = allDayFreeBusy.Length - i;
						}
						else
						{
							num3 = num * 2;
						}
						stringBuilder.Append(allDayFreeBusy.Substring(i, num3));
						i += num3;
						i += num2 * 2;
					}
					workingHoursFreeBusy = stringBuilder.ToString();
				}
				if (freeBusyQueryResult != null && !string.IsNullOrEmpty(freeBusyQueryResult.CurrentOofMessage))
				{
					oofMessage = freeBusyQueryResult.CurrentOofMessage;
				}
			}
			else if (availabilityQueryResult != null && availabilityQueryResult.FreeBusyResults != null && availabilityQueryResult.FreeBusyResults.Length > 0)
			{
				FreeBusyQueryResult freeBusyQueryResult2 = availabilityQueryResult.FreeBusyResults[0];
				if (freeBusyQueryResult2 != null)
				{
					if (Utilities.IsFatalFreeBusyError(freeBusyQueryResult2.ExceptionInfo))
					{
						ExTraceGlobals.CalendarTracer.TraceDebug<LocalizedException>(0L, "An error happened trying to get free/busy info for this recipient. Exception: {0}", freeBusyQueryResult2.ExceptionInfo);
						ErrorInformation exceptionHandlingInformation = Utilities.GetExceptionHandlingInformation(freeBusyQueryResult2.ExceptionInfo, userContext.MailboxIdentity);
						return exceptionHandlingInformation.Message;
					}
				}
				else
				{
					ExTraceGlobals.CalendarTracer.TraceDebug(0L, "An error happened trying to get free/busy info for this recipient. FreeBusyResult is null.");
				}
			}
			return null;
		}

		internal static void RenderOrganizationContents(TextWriter writer, UserContext userContext, IADOrgPerson orgPerson, IRecipientSession adRecipientSession)
		{
			object[][] managementChainView = orgPerson.GetManagementChainView(false, new PropertyDefinition[]
			{
				ADObjectSchema.Id,
				ADRecipientSchema.DisplayName,
				ADOrgPersonSchema.Title
			});
			if (managementChainView != null)
			{
				ReadADOrgPerson.RenderManagerList(writer, userContext, managementChainView);
			}
			ADRecipient adrecipient = null;
			if (orgPerson.Manager != null)
			{
				try
				{
					adrecipient = adRecipientSession.Read(orgPerson.Manager);
				}
				catch (NonUniqueRecipientException ex)
				{
					ExTraceGlobals.DirectoryTracer.TraceDebug<string>(0L, "ReadADOrgPerson.RenderOrganizationContents: NonUniqueRecipientException thrown by IRecipientSession.Read: {0}", ex.Message);
				}
			}
			if (adrecipient != null)
			{
				IADOrgPerson iadorgPerson = (IADOrgPerson)adrecipient;
				object[][] directReportsView = iadorgPerson.GetDirectReportsView(new PropertyDefinition[]
				{
					ADObjectSchema.Id,
					ADRecipientSchema.DisplayName,
					ADOrgPersonSchema.Title
				});
				if (directReportsView != null)
				{
					ReadADOrgPerson.RenderPeersList(writer, directReportsView, orgPerson.Id, orgPerson.DisplayName, orgPerson.Title);
				}
			}
			else
			{
				ReadADOrgPerson.RenderPeopleLabelHeading(writer, LocalizedStrings.GetHtmlEncoded(1804838102));
				ReadADOrgPerson.RenderSelf(writer, orgPerson.Id, orgPerson.DisplayName, orgPerson.Title);
			}
			object[][] directReportsView2 = orgPerson.GetDirectReportsView(new PropertyDefinition[]
			{
				ADObjectSchema.Id,
				ADRecipientSchema.DisplayName,
				ADOrgPersonSchema.Title
			});
			if (directReportsView2 != null)
			{
				ReadADOrgPerson.RenderDirectReportsList(writer, directReportsView2);
			}
		}

		private void RenderSecureMessaging()
		{
			ClientSMimeControlStatus status = Utilities.CheckClientSMimeControlStatus(Utilities.GetQueryStringParameter(base.Request, "smime", false), base.OwaContext);
			if (!Utilities.IsClientSMimeControlUsable(status))
			{
				return;
			}
			base.Response.Write("<div class=\"row2sp\"><div class=\"secCol\"><span class=\"spS\">" + LocalizedStrings.GetHtmlEncoded(-2096722623) + "</span></div><div class=\"lbl noindent\" id=\"tdSM\">");
			bool flag = Utilities.GetADRecipientCertificate(base.ADRecipient, false) != null;
			base.Response.Write(LocalizedStrings.GetHtmlEncoded(flag ? -1588000202 : -629647425));
			base.Response.Write("</div></div>");
		}

		private const short MaxManagerTreeSize = 25;

		private const short DaysToLoad = 7;

		private const short HoursInDay = 24;

		private const short PixelsPerHour = 35;

		private ExDateTime startDay;

		private ExDateTime endDay;

		private IADOrgPerson orgPerson;

		private ADRawEntry adRawEntry;

		private IADOrgPerson assistant;

		private bool renderCustomProperties;

		private bool isAdOrgPerson = true;

		private struct ADMember : IComparable<ReadADOrgPerson.ADMember>
		{
			internal ADMember(ADObjectId id, string displayName, string title)
			{
				this.Id = id;
				this.DisplayName = displayName;
				this.Title = title;
			}

			public int CompareTo(ReadADOrgPerson.ADMember x)
			{
				return this.DisplayName.CompareTo(x.DisplayName);
			}

			internal ADObjectId Id;

			public string DisplayName;

			public string Title;
		}

		private struct DisplayField
		{
			public DisplayField(string label, string value, bool link)
			{
				this.Label = label;
				this.Value = value;
				this.Link = link;
			}

			public string Label;

			public string Value;

			public bool Link;
		}
	}
}
