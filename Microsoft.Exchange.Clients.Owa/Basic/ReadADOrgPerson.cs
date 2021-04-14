using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.Availability;

namespace Microsoft.Exchange.Clients.Owa.Basic
{
	public class ReadADOrgPerson : ReadADRecipient, IComparer<ReadADOrgPerson.ADMember>
	{
		protected bool IsAdOrgPerson
		{
			get
			{
				return this.isAdOrgPerson;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (base.ADRecipient is IADOrgPerson)
			{
				this.adOrgPerson = (IADOrgPerson)base.ADRecipient;
				this.isAdOrgPerson = true;
			}
			if (this.isAdOrgPerson && ADCustomPropertyParser.CustomPropertyDictionary != null && ADCustomPropertyParser.CustomPropertyDictionary.Count > 0)
			{
				this.adRawEntry = base.ADRecipientSession.ReadADRawEntry(base.ADRecipient.OriginalId, ADCustomPropertyParser.CustomPropertyDictionary.Values);
				if (this.adRawEntry != null)
				{
					this.renderCustomProperties = true;
				}
			}
		}

		protected void RenderHeader(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			this.title = (this.isAdOrgPerson ? this.adOrgPerson.Title : string.Empty);
			this.company = (this.isAdOrgPerson ? this.adOrgPerson.Company : string.Empty);
			this.department = (this.isAdOrgPerson ? this.adOrgPerson.Department : string.Empty);
			writer.Write("<table cellpadding=0 cellspacing=0 class=\"pHd\">");
			if (base.UserContext.IsPhoneticNamesEnabled && !string.IsNullOrEmpty(base.ADRecipient.PhoneticDisplayName))
			{
				writer.Write("<tr><td class=\"dn pLT\">");
				Utilities.HtmlEncode(base.ADRecipient.PhoneticDisplayName, writer);
				writer.Write("</td></tr>");
			}
			writer.Write("<tr><td class=\"dn pLT\">");
			Utilities.HtmlEncode(base.ADRecipient.DisplayName, writer);
			writer.Write("</td></tr>");
			writer.Write("<tr><td class=\"pLT\">");
			if (!string.IsNullOrEmpty(this.title))
			{
				writer.Write("<span class=\"txb\">");
				Utilities.HtmlEncode(this.title, writer);
				writer.Write(", </span>");
			}
			writer.Write("<span class=\"txnr\">");
			Utilities.HtmlEncode(this.department, writer);
			writer.Write("</span></td></tr>");
			writer.Write("<tr><td class=\"pLT pB\"><span class=\"txnr\">");
			Utilities.HtmlEncode(this.company, writer);
			writer.Write("</span></td></tr>");
			writer.Write("</table>");
		}

		protected void RenderDetailsBucket(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write("<table cellpadding=0 cellspacing=0 class=\"pDtls\">");
			ReadADRecipient.RenderDetailHeader(writer, -2101430728);
			writer.Write("<tr><td class=\"lbl lp\">");
			writer.Write(LocalizedStrings.GetHtmlEncoded(613689222));
			writer.Write("</td>");
			if (!base.UserContext.IsWebPartRequest)
			{
				writer.Write("<td><a href=\"#\" class=\"peer\" onclick=\"return onSendMail();\">");
			}
			else
			{
				writer.Write("<td class=\"txvl\">");
			}
			Utilities.HtmlEncode(base.ADRecipient.Alias, writer);
			if (!base.UserContext.IsWebPartRequest)
			{
				writer.Write("</a>");
			}
			writer.Write("</td></tr>");
			if (this.isAdOrgPerson)
			{
				base.RenderDetailsLabel(writer, 1111077458, this.adOrgPerson.PrimarySmtpAddress.ToString(), null);
				writer.Write("<tr><td class=\"spcHd\" colspan=2></td></tr>");
				if (!string.IsNullOrEmpty(this.adOrgPerson.Office))
				{
					base.RenderDetailsLabel(writer, 275231482, this.adOrgPerson.Office, null);
				}
				writer.Write("<tr><td class=\"spcHd\" colspan=2></td></tr>");
				if (!string.IsNullOrEmpty(this.adOrgPerson.Phone))
				{
					base.RenderDetailsLabel(writer, -31489650, this.adOrgPerson.Phone, new ThemeFileId?(ThemeFileId.WorkPhone));
				}
				if (!string.IsNullOrEmpty(this.adOrgPerson.HomePhone))
				{
					base.RenderDetailsLabel(writer, -1844864953, this.adOrgPerson.HomePhone, new ThemeFileId?(ThemeFileId.HomePhone));
				}
				if (!string.IsNullOrEmpty(this.adOrgPerson.MobilePhone))
				{
					base.RenderDetailsLabel(writer, 1158653436, this.adOrgPerson.MobilePhone, new ThemeFileId?(ThemeFileId.MobilePhone));
				}
				if (!string.IsNullOrEmpty(this.adOrgPerson.Fax))
				{
					base.RenderDetailsLabel(writer, 696030351, this.adOrgPerson.Fax, new ThemeFileId?(ThemeFileId.Fax));
				}
				if (!string.IsNullOrEmpty(this.adOrgPerson.Pager))
				{
					base.RenderDetailsLabel(writer, -1779142331, this.adOrgPerson.Pager, null);
				}
				bool flag = !string.IsNullOrEmpty(this.adOrgPerson.FirstName) || !string.IsNullOrEmpty(this.adOrgPerson.LastName) || (base.UserContext.IsPhoneticNamesEnabled && !string.IsNullOrEmpty(this.adOrgPerson.PhoneticFirstName)) || (base.UserContext.IsPhoneticNamesEnabled && !string.IsNullOrEmpty(this.adOrgPerson.PhoneticLastName));
				if (flag)
				{
					writer.Write("<tr><td class=\"spcOP\" colspan=2></td></tr>");
					ReadADRecipient.RenderDetailHeader(writer, -728684336);
				}
				if (base.UserContext.IsPhoneticNamesEnabled && !string.IsNullOrEmpty(this.adOrgPerson.PhoneticFirstName))
				{
					base.RenderDetailsLabel(writer, -758272749, this.adOrgPerson.PhoneticFirstName, null);
				}
				if (!string.IsNullOrEmpty(this.adOrgPerson.FirstName))
				{
					base.RenderDetailsLabel(writer, -1134283443, this.adOrgPerson.FirstName, null);
				}
				if (base.UserContext.IsPhoneticNamesEnabled && !string.IsNullOrEmpty(this.adOrgPerson.PhoneticLastName))
				{
					base.RenderDetailsLabel(writer, -1100427325, this.adOrgPerson.PhoneticLastName, null);
				}
				if (!string.IsNullOrEmpty(this.adOrgPerson.LastName))
				{
					base.RenderDetailsLabel(writer, -991618307, this.adOrgPerson.LastName, null);
				}
				writer.Write("<tr><td class=\"spcOP\" colspan=2></td></tr>");
				ReadADRecipient.RenderDetailHeader(writer, -905993889);
				if (!string.IsNullOrEmpty(this.title))
				{
					base.RenderDetailsLabel(writer, 587115635, this.title, null);
					this.renderOrganizationDetails = true;
				}
				if (base.UserContext.IsPhoneticNamesEnabled && !string.IsNullOrEmpty(this.adOrgPerson.PhoneticDepartment))
				{
					base.RenderDetailsLabel(writer, 871410780, this.adOrgPerson.PhoneticDepartment, null);
					this.renderOrganizationDetails = true;
				}
				if (!string.IsNullOrEmpty(this.department))
				{
					base.RenderDetailsLabel(writer, 1855823700, this.department, null);
					this.renderOrganizationDetails = true;
				}
				if (base.UserContext.IsPhoneticNamesEnabled && !string.IsNullOrEmpty(this.adOrgPerson.PhoneticCompany))
				{
					base.RenderDetailsLabel(writer, -923446215, this.adOrgPerson.PhoneticCompany, null);
					this.renderOrganizationDetails = true;
				}
				if (!string.IsNullOrEmpty(this.company))
				{
					base.RenderDetailsLabel(writer, 642177943, this.company, null);
					this.renderOrganizationDetails = true;
				}
				this.RenderManagementChain(this.adOrgPerson, writer);
				this.RenderPeers(this.adOrgPerson, writer);
				this.RenderDirectReports(this.adOrgPerson, writer);
				if (!this.renderOrganizationDetails)
				{
					writer.Write("<tr><td colspan=2 class=\"nodtls msgpd\">");
					writer.Write(LocalizedStrings.GetHtmlEncoded(1029790140));
					writer.Write("</td></tr>");
				}
			}
			writer.Write("<tr><td class=\"spcOP\" colspan=2></td></tr>");
			writer.Write("</table>");
		}

		private static void RenderAddressPart(TextWriter writer, string label, string value)
		{
			writer.Write("<tr><td class=\"lbl\">");
			writer.Write(label);
			writer.Write("</td><td class=\"txvl\">");
			Utilities.HtmlEncode(value, writer);
			writer.Write("</td></tr>");
		}

		private void RenderCustomProperties(TextWriter writer)
		{
			bool flag = false;
			foreach (KeyValuePair<string, PropertyDefinition> keyValuePair in ADCustomPropertyParser.CustomPropertyDictionary)
			{
				string value = this.adRawEntry[keyValuePair.Value].ToString();
				if (!string.IsNullOrEmpty(value))
				{
					if (!flag)
					{
						ReadADRecipient.RenderAddressHeader(writer, -582599340);
						flag = true;
					}
					base.RenderDetailsLabel(writer, Utilities.HtmlEncode(keyValuePair.Key), value, null);
				}
			}
			if (flag)
			{
				writer.Write("<tr><td class=\"spcOP\" colspan=2></td></tr>");
			}
		}

		protected void RenderAddressBucket(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			bool flag = true;
			if (string.IsNullOrEmpty(this.adOrgPerson.StreetAddress) && string.IsNullOrEmpty(this.adOrgPerson.City) && string.IsNullOrEmpty(this.adOrgPerson.StateOrProvince) && string.IsNullOrEmpty(this.adOrgPerson.PostalCode) && string.IsNullOrEmpty(this.adOrgPerson.CountryOrRegionDisplayName))
			{
				flag = false;
			}
			writer.Write("<table cellpadding=0 cellspacing=0 class=\"pAddr\">");
			if (flag)
			{
				ReadADRecipient.RenderAddressHeader(writer, -1159205642);
				IDictionary<AddressFormatTable.AddressPart, AddressComponent> addressInfo = ContactUtilities.GetAddressInfo(this.adOrgPerson);
				foreach (KeyValuePair<AddressFormatTable.AddressPart, AddressComponent> keyValuePair in addressInfo)
				{
					ReadADOrgPerson.RenderAddressPart(writer, keyValuePair.Value.Label, keyValuePair.Value.Value);
				}
			}
			if (this.renderCustomProperties)
			{
				this.RenderCustomProperties(writer);
			}
			ReadADRecipient.RenderAddressHeader(writer, 1601836855);
			writer.Write("<tr><td colspan=2 class=\"rp\"><textarea name=\"notes\" rows=10 cols=32 readonly>");
			Utilities.HtmlEncode(this.adOrgPerson.Notes, writer);
			writer.Write("</textarea></td></tr></table>");
		}

		private void RenderPeers(IADOrgPerson person, TextWriter writer)
		{
			ADObjectId manager = person.Manager;
			if (manager != null)
			{
				ADRecipient adrecipient = base.ADRecipientSession.Read(manager);
				if (adrecipient != null)
				{
					IADOrgPerson iadorgPerson = (IADOrgPerson)adrecipient;
					this.peersResults = iadorgPerson.GetDirectReportsView(new PropertyDefinition[]
					{
						ADRecipientSchema.DisplayName,
						ADObjectSchema.Id,
						ADRecipientSchema.RecipientType
					});
				}
			}
			if (this.peersResults != null && this.peersResults.Length > 1)
			{
				writer.Write("<tr><td class=\"lbl lp\" nowrap>");
				writer.Write(LocalizedStrings.GetHtmlEncoded(-1417802693));
				writer.Write("</td><td>");
				writer.Write("<table cellpadding=0 cellspacing=0 class=\"drpts\">");
				List<ReadADOrgPerson.ADMember> list = new List<ReadADOrgPerson.ADMember>();
				for (int i = 0; i < this.peersResults.Length; i++)
				{
					if (!base.ADRecipient.Id.Equals(this.peersResults[i][1]))
					{
						list.Add(new ReadADOrgPerson.ADMember(this.peersResults[i][0] as string, this.peersResults[i][1] as ADObjectId, this.peersResults[i][2]));
					}
				}
				list.Sort(this);
				foreach (ReadADOrgPerson.ADMember admember in list)
				{
					RecipientType recipientType = (RecipientType)admember.Type;
					int readItemType;
					if (Utilities.IsADDistributionList(recipientType))
					{
						readItemType = 2;
					}
					else
					{
						readItemType = 1;
					}
					writer.Write("<tr><td class=\"rptdpd\">");
					ReadADRecipient.RenderADRecipient(writer, readItemType, admember.Id, admember.DisplayName);
					writer.Write("</td></tr>");
				}
				writer.Write("</table>");
				writer.Write("</td></tr>");
				writer.Write("<tr><td class=\"spcHd\" colspan=2></td></tr>");
				this.renderOrganizationDetails = true;
			}
		}

		public int Compare(ReadADOrgPerson.ADMember x, ReadADOrgPerson.ADMember y)
		{
			return x.DisplayName.CompareTo(y.DisplayName);
		}

		private void RenderDirectReports(IADOrgPerson person, TextWriter writer)
		{
			this.reports = person.GetDirectReportsView(new PropertyDefinition[]
			{
				ADRecipientSchema.DisplayName,
				ADObjectSchema.Id,
				ADRecipientSchema.RecipientType
			});
			if (this.reports != null && this.reports.Length > 0)
			{
				writer.Write("<tr><td class=\"lbl lp\">");
				writer.Write(LocalizedStrings.GetHtmlEncoded(-156515347));
				writer.Write("</td><td>");
				writer.Write("<table cellpadding=0 cellspacing=0 class=\"drpts\">");
				List<ReadADOrgPerson.ADMember> list = new List<ReadADOrgPerson.ADMember>();
				for (int i = 0; i < this.reports.Length; i++)
				{
					list.Add(new ReadADOrgPerson.ADMember(this.reports[i][0] as string, this.reports[i][1] as ADObjectId, this.reports[i][2]));
				}
				list.Sort(this);
				foreach (ReadADOrgPerson.ADMember admember in list)
				{
					RecipientType recipientType = (RecipientType)admember.Type;
					int readItemType;
					if (Utilities.IsADDistributionList(recipientType))
					{
						readItemType = 2;
					}
					else
					{
						readItemType = 1;
					}
					writer.Write("<tr><td class=\"rptdpd\">");
					ReadADRecipient.RenderADRecipient(writer, readItemType, admember.Id, admember.DisplayName);
					writer.Write("</td></tr>");
				}
				writer.Write("</table>");
				writer.Write("</td></tr>");
				this.renderOrganizationDetails = true;
			}
		}

		private void RenderManagementChain(IADOrgPerson person, TextWriter writer)
		{
			this.managersResults = person.GetManagementChainView(false, new PropertyDefinition[]
			{
				ADRecipientSchema.DisplayName,
				ADObjectSchema.Id,
				ADRecipientSchema.RecipientType
			});
			if (this.managersResults.Length <= 1)
			{
				return;
			}
			int num = this.managersResults.Length - 1;
			int num2 = 0;
			int readItemType = 1;
			if (num > 25)
			{
				num2 = num - 25;
			}
			writer.Write("<tr><td class=\"lbl lp\">");
			writer.Write(LocalizedStrings.GetHtmlEncoded(1660340599));
			writer.Write("</td><td>");
			writer.Write("<table cellpadding=0 cellspacing=0 class=\"drpts\">");
			for (int i = num2; i < num; i++)
			{
				writer.Write("<tr>");
				if (i != 0)
				{
					writer.Write("<td><img alt=\"\" class=\"mitimg\" src=\"");
					base.UserContext.RenderThemeFileUrl(writer, ThemeFileId.OrganizationUpArrow);
					writer.Write("\"></td>");
					writer.Write("<td class=\"mitdpd\">");
				}
				else
				{
					writer.Write("<td colspan=\"2\" class=\"rptdpd\">");
				}
				ReadADRecipient.RenderADRecipient(writer, readItemType, this.managersResults[i][1] as ADObjectId, this.managersResults[i][0].ToString());
				writer.Write("</td></tr>");
			}
			writer.Write("</table>");
			writer.Write("</td></tr>");
			writer.Write("<tr><td class=\"spcHd\" colspan=2></td></tr>");
			this.renderOrganizationDetails = true;
		}

		protected void RenderOOF(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			AvailabilityQuery availabilityQuery = new AvailabilityQuery();
			if (HttpContext.Current != null)
			{
				availabilityQuery.HttpResponse = HttpContext.Current.Response;
			}
			availabilityQuery.MailboxArray = new MailboxData[1];
			availabilityQuery.MailboxArray[0] = new MailboxData();
			availabilityQuery.MailboxArray[0].Email = new EmailAddress();
			availabilityQuery.MailboxArray[0].Email.Address = base.ADRecipient.PrimarySmtpAddress.ToString();
			availabilityQuery.ClientContext = ClientContext.Create(base.UserContext.LogonIdentity.ClientSecurityContext, base.UserContext.ExchangePrincipal.MailboxInfo.OrganizationId, OwaContext.TryGetCurrentBudget(), base.UserContext.TimeZone, base.UserContext.UserCulture, AvailabilityQuery.CreateNewMessageId());
			ExDateTime date = DateTimeUtilities.GetLocalTime().Date;
			ExDateTime exDateTime = date.IncrementDays(1);
			availabilityQuery.DesiredFreeBusyView = new FreeBusyViewOptions
			{
				RequestedView = FreeBusyViewType.Detailed,
				MergedFreeBusyIntervalInMinutes = 1440,
				TimeWindow = new Duration((DateTime)date, (DateTime)exDateTime.IncrementDays(1))
			};
			AvailabilityQueryResult availabilityQueryResult;
			if (Utilities.ExecuteAvailabilityQuery(base.OwaContext, availabilityQuery, true, out availabilityQueryResult))
			{
				FreeBusyQueryResult freeBusyQueryResult = availabilityQueryResult.FreeBusyResults[0];
				if (freeBusyQueryResult != null)
				{
					string currentOofMessage = freeBusyQueryResult.CurrentOofMessage;
					if (!string.IsNullOrEmpty(currentOofMessage))
					{
						writer.Write("<tr><td class=\"spcOP\"></td></tr>");
						writer.Write("<tr><td class=\"oof oofF\">");
						writer.Write(LocalizedStrings.GetHtmlEncoded(77678270));
						writer.Write("</td></tr>");
						writer.Write("<tr><td class=\"oof\">");
						writer.Write("<textarea name=\"off\" rows=3 cols=100 readonly>");
						writer.Write(currentOofMessage);
						writer.Write("</textarea>");
					}
				}
			}
		}

		private const int MaxManagerTreeSize = 25;

		private IADOrgPerson adOrgPerson;

		private ADRawEntry adRawEntry;

		private string department;

		private string title;

		private string company;

		private object[][] reports;

		private object[][] managersResults;

		private object[][] peersResults;

		private bool renderOrganizationDetails;

		private bool renderCustomProperties;

		private bool isAdOrgPerson;

		public struct ADMember
		{
			public ADMember(string displayName, ADObjectId id, object type)
			{
				this.DisplayName = displayName;
				this.Id = id;
				this.Type = type;
			}

			public override bool Equals(object obj)
			{
				if (!(obj is ReadADOrgPerson.ADMember))
				{
					return false;
				}
				ReadADOrgPerson.ADMember admember = (ReadADOrgPerson.ADMember)obj;
				return ((this.DisplayName == null && admember.DisplayName == null) || (this.DisplayName != null && this.DisplayName.Equals(admember.DisplayName))) && ((this.Id == null && admember.Id == null) || (this.Id != null && this.Id.Equals(admember.Id))) && ((this.Type == null && admember.Type == null) || (this.Type != null && this.Type.Equals(admember.Type)));
			}

			public override int GetHashCode()
			{
				int num = 1000003;
				int num2 = 0;
				if (this.DisplayName != null)
				{
					num2 = this.DisplayName.GetHashCode();
				}
				if (this.Id != null)
				{
					num2 = num * num2 + this.Id.GetHashCode();
				}
				if (this.Type != null)
				{
					num2 = num * num2 + this.Type.GetHashCode();
				}
				return num2;
			}

			public string DisplayName;

			public ADObjectId Id;

			public object Type;
		}
	}
}
