using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class AddressBookRecipientPicker
	{
		private AddressBookRecipientPicker(Strings.IDs title, RecipientWell recipientWell, params Strings.IDs?[] wellLabels)
		{
			if (recipientWell == null)
			{
				throw new ArgumentNullException("recipientWell");
			}
			if (wellLabels.Length > 3 || wellLabels.Length == 0)
			{
				throw new ArgumentException("There must be at least 1 and no more than " + 3 + "recipientwells specified.");
			}
			this.title = title;
			this.recipientWell = recipientWell;
			this.wellLabels = new Strings.IDs?[wellLabels.Length];
			for (int i = 0; i < wellLabels.Length; i++)
			{
				this.wellLabels[i] = wellLabels[i];
			}
		}

		public void Render(UserContext userContext, TextWriter output)
		{
			string value = "&nbsp;<span class=\"abArrw\">-&gt;</span>";
			string attributes = "class=\"abRW fl\" tabindex=-1 ";
			ThemeFileId themeFileId = userContext.IsRtl ? ThemeFileId.PickerArrowRtl : ThemeFileId.PickerArrowLtr;
			StringBuilder stringBuilder = new StringBuilder("addSelectionToWell(\"", 30);
			int length = stringBuilder.Length;
			StringBuilder stringBuilder2 = new StringBuilder(60);
			output.Write("<div id=\"adrPkr\">");
			output.Write(LocalizedStrings.GetHtmlEncoded(this.title));
			output.Write("<div id=\"wls\">");
			for (int i = 0; i < AddressBookRecipientPicker.divIds.Length; i++)
			{
				if (i < this.wellLabels.Length && this.wellLabels[i] != null)
				{
					string value2;
					RecipientWellType type;
					string buttonId;
					if (this == AddressBookRecipientPicker.SendFromRecipients)
					{
						value2 = "divFrom";
						type = RecipientWellType.From;
						buttonId = "btnFrom";
					}
					else
					{
						value2 = AddressBookRecipientPicker.divIds[i];
						type = AddressBookRecipientPicker.types[i];
						buttonId = AddressBookRecipientPicker.buttonIds[i];
					}
					output.Write("<div id=\"wl\" class=\"w100\"><div id=\"arw\" class=\"frt\">");
					userContext.RenderThemeImage(output, themeFileId, string.Empty, new object[]
					{
						"id=\"arwi\" style=\"display:none\""
					});
					userContext.RenderThemeImage(output, ThemeFileId.Clear1x1, string.Empty, new object[]
					{
						"id=\"arwc\""
					});
					output.Write("</div>");
					Strings.IDs localizedID = this.wellLabels[i] ?? -269710455;
					stringBuilder2.Append(LocalizedStrings.GetHtmlEncoded(localizedID));
					stringBuilder2.Append(value);
					stringBuilder.Append(value2);
					stringBuilder.Append("\");");
					RenderingUtilities.RenderButton(output, buttonId, attributes, stringBuilder.ToString(), stringBuilder2.ToString(), true);
					this.recipientWell.Render(output, userContext, type);
					output.Write("</div>");
					stringBuilder.Length = length;
					stringBuilder2.Length = 0;
				}
			}
			output.Write("</div><div id=\"okCn\" class=\"frt\">");
			RenderingUtilities.RenderButton(output, "btnOk", string.Empty, "save();", LocalizedStrings.GetHtmlEncoded(2041362128));
			output.Write("&nbsp;");
			RenderingUtilities.RenderButton(output, "btnCncl", string.Empty, "window.close();", LocalizedStrings.GetHtmlEncoded(-1936577052));
			output.Write("</div></div>");
		}

		private const int MaxWells = 3;

		private const string FromDivId = "divFrom";

		private const string FromButtonId = "btnFrom";

		private static readonly string[] buttonIds = new string[]
		{
			"btnTo",
			"btnCc",
			"btnBcc"
		};

		private static readonly string[] divIds = new string[]
		{
			"divTo",
			"divCc",
			"divBcc"
		};

		private static readonly RecipientWellType[] types = new RecipientWellType[]
		{
			RecipientWellType.To,
			RecipientWellType.Cc,
			RecipientWellType.Bcc
		};

		internal static readonly AddressBookRecipientPicker Recipients = new AddressBookRecipientPicker(-2069160934, new MessageRecipientWell(), new Strings.IDs?[]
		{
			new Strings.IDs?(-269710455),
			new Strings.IDs?(2055888382),
			new Strings.IDs?(198978688)
		});

		internal static readonly AddressBookRecipientPicker Attendees = new AddressBookRecipientPicker(1925113256, new CalendarItemRecipientWell(), new Strings.IDs?[]
		{
			new Strings.IDs?(635407387),
			new Strings.IDs?(-1832858804),
			new Strings.IDs?(1672068383)
		});

		internal static readonly AddressBookRecipientPicker DistributionListMember = new AddressBookRecipientPicker(-1893304058, new MessageRecipientWell(), new Strings.IDs?[]
		{
			new Strings.IDs?(-1619718267)
		});

		internal static readonly AddressBookRecipientPicker UsersAndGroups = new AddressBookRecipientPicker(2104380933, new MessageRecipientWell(), new Strings.IDs?[]
		{
			new Strings.IDs?(1465898911)
		});

		internal static readonly AddressBookRecipientPicker Rooms = new AddressBookRecipientPicker(-432893051, new CalendarItemRecipientWell(), new Strings.IDs?[]
		{
			null,
			null,
			new Strings.IDs?(1007756464)
		});

		internal static readonly AddressBookRecipientPicker FromRecipients = new AddressBookRecipientPicker(-2069160934, new MessageRecipientWell(), new Strings.IDs?[]
		{
			new Strings.IDs?(-1327933906)
		});

		internal static readonly AddressBookRecipientPicker ToRecipients = new AddressBookRecipientPicker(-2069160934, new MessageRecipientWell(), new Strings.IDs?[]
		{
			new Strings.IDs?(-269710455)
		});

		internal static readonly AddressBookRecipientPicker SendFromRecipients = new AddressBookRecipientPicker(950402488, new MessageRecipientWell(), new Strings.IDs?[]
		{
			new Strings.IDs?(-1327933906)
		});

		internal static readonly AddressBookRecipientPicker SelectOtherMailboxRecipient = new AddressBookRecipientPicker(-2058911220, new MessageRecipientWell(), new Strings.IDs?[]
		{
			new Strings.IDs?(1264864827)
		});

		internal static readonly AddressBookRecipientPicker PersonalAutoAttendantCallers = new AddressBookRecipientPicker(-239997443, new MessageRecipientWell(), new Strings.IDs?[]
		{
			new Strings.IDs?(-1799156789)
		});

		internal static readonly AddressBookRecipientPicker ChatParticipants = new AddressBookRecipientPicker(1768257590, new MessageRecipientWell(), new Strings.IDs?[]
		{
			new Strings.IDs?(729061323)
		});

		internal static readonly AddressBookRecipientPicker PersonalAutoAttendantOneCaller = new AddressBookRecipientPicker(220395981, new MessageRecipientWell(), new Strings.IDs?[]
		{
			new Strings.IDs?(137777747)
		});

		internal static readonly AddressBookRecipientPicker AddBuddy = new AddressBookRecipientPicker(642911694, new MessageRecipientWell(), new Strings.IDs?[]
		{
			new Strings.IDs?(292745765)
		});

		internal static readonly AddressBookRecipientPicker ToMobileNumberOrDL = new AddressBookRecipientPicker(-2069160934, new MessageRecipientWell(), new Strings.IDs?[]
		{
			new Strings.IDs?(-269710455)
		});

		internal static readonly AddressBookRecipientPicker ToMobileNumber = new AddressBookRecipientPicker(-2069160934, new MessageRecipientWell(), new Strings.IDs?[]
		{
			new Strings.IDs?(-269710455)
		});

		internal static readonly AddressBookRecipientPicker Filter = new AddressBookRecipientPicker(-2069160934, new MessageRecipientWell(), new Strings.IDs?[]
		{
			new Strings.IDs?(1264864827)
		});

		private Strings.IDs title;

		private RecipientWell recipientWell;

		private Strings.IDs?[] wellLabels;
	}
}
