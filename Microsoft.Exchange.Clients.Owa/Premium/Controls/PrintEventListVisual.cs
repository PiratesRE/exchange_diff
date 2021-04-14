using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal class PrintEventListVisual : PrintWeeklyAgendaVisual
	{
		public PrintEventListVisual(ISessionContext sessionContext, int index, ICalendarDataSource dataSource, bool isFirst) : base(sessionContext, index, dataSource, isFirst)
		{
			if (base.SessionContext is UserContext)
			{
				this.invitees = dataSource.GetInviteesDisplayNames(index);
				OwaStoreObjectId itemId = dataSource.GetItemId(index);
				if (itemId == null)
				{
					return;
				}
				using (Item item = Utilities.GetItem<Item>((UserContext)base.SessionContext, itemId, new PropertyDefinition[0]))
				{
					using (TextReader textReader = item.Body.OpenTextReader(BodyFormat.TextPlain))
					{
						this.notes = textReader.ReadToEnd();
					}
					return;
				}
			}
			PublishedCalendarDataSource publishedCalendarDataSource = (PublishedCalendarDataSource)dataSource;
			if (publishedCalendarDataSource.DetailLevel == DetailLevelEnumType.FullDetails)
			{
				PublishedCalendarItemData? item2 = publishedCalendarDataSource.GetItem(index);
				if (item2 != null)
				{
					this.notes = item2.Value.BodyText;
				}
			}
		}

		public override void Render(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			this.isFirstRow = true;
			this.RenderProperty(writer, this.TimeDescription, new string[]
			{
				base.Subject
			});
			this.RenderProperty(writer, -1134349396, new string[]
			{
				base.Location
			});
			this.RenderProperty(writer, -2098425358, new string[]
			{
				base.Organizer
			});
			if (base.SessionContext is UserContext)
			{
				this.RenderProperty(writer, 1379219406, new string[]
				{
					this.invitees
				});
			}
			if (!base.IsPrivate)
			{
				this.RenderProperty(writer, -2053657454, new string[]
				{
					"<pre>",
					this.notes,
					"</pre>"
				});
			}
			writer.Write("<tr><td>&nbsp;</td></tr>");
		}

		private void RenderProperty(TextWriter writer, Strings.IDs captionId, params string[] values)
		{
			this.RenderProperty(writer, LocalizedStrings.GetNonEncoded(captionId), values);
		}

		private void RenderProperty(TextWriter writer, string caption, params string[] values)
		{
			writer.Write("<tr><td class=\"eventListFBIcon\">");
			if (this.isFirstRow)
			{
				base.RenderFreeBusy(writer, true);
			}
			writer.Write("</td><td><span class=\"eventListCaption\">");
			writer.Write(caption);
			if (this.isFirstRow)
			{
				base.RenderIcons(writer, true);
			}
			writer.Write(" ");
			writer.Write("</span>");
			writer.Write(base.SessionContext.GetDirectionMark());
			foreach (string value in values)
			{
				writer.Write(value);
			}
			writer.Write("</td></tr>");
			this.isFirstRow = false;
		}

		private string invitees;

		private string notes = string.Empty;

		private bool isFirstRow = true;
	}
}
