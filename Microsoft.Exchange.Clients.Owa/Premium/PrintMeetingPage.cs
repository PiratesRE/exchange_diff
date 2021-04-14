using System;
using System.Collections;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class PrintMeetingPage : MeetingPage
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			BusyType busyType = BusyType.Tentative;
			if (base.Item != null)
			{
				object obj = base.Item.TryGetProperty(CalendarItemBaseSchema.FreeBusyStatus);
				if (obj is int)
				{
					busyType = (BusyType)obj;
				}
			}
			switch (busyType)
			{
			case BusyType.Free:
				this.busyFreeString = LocalizedStrings.GetHtmlEncoded(-971703552);
				goto IL_91;
			case BusyType.Busy:
				this.busyFreeString = LocalizedStrings.GetHtmlEncoded(2052801377);
				goto IL_91;
			case BusyType.OOF:
				this.busyFreeString = LocalizedStrings.GetHtmlEncoded(2047193656);
				goto IL_91;
			}
			this.busyFreeString = LocalizedStrings.GetHtmlEncoded(1797669216);
			IL_91:
			if (base.Item.Importance == Importance.High)
			{
				this.importanceString = LocalizedStrings.GetHtmlEncoded(-77932258);
			}
			else if (base.Item.Importance == Importance.Low)
			{
				this.importanceString = LocalizedStrings.GetHtmlEncoded(1502599728);
			}
			this.categoriesString = ItemUtility.GetCategoriesAsString(base.Item);
			this.attachmentWellRenderObjects = AttachmentWell.GetAttachmentInformation(base.Item, base.AttachmentLinks, base.UserContext.IsPublicLogon, base.IsEmbeddedItem);
		}

		protected override void LoadMessageBodyIntoStream(TextWriter writer)
		{
			BodyConversionUtilities.GeneratePrintMessageBody(base.Item, writer, base.OwaContext, base.IsEmbeddedItem, base.IsEmbeddedItem ? base.RenderEmbeddedUrl() : null, base.ForceAllowWebBeacon, base.ForceEnableItemLink);
		}

		protected override bool HasAttachments
		{
			get
			{
				if (this.hasAttachments == null)
				{
					this.hasAttachments = PrintAttachmentWell.ShouldRenderAttachments(this.attachmentWellRenderObjects);
				}
				return (bool)this.hasAttachments;
			}
		}

		protected override void MeetingPageWriterFactory(string itemType, EventArgs e)
		{
			base.MeetingPageWriterFactory(itemType, e);
			base.MeetingPageWriter = new PrintPageWriter(base.MeetingPageWriter);
		}

		public string BusyFreeString
		{
			get
			{
				return this.busyFreeString;
			}
		}

		public string ImportanceString
		{
			get
			{
				return this.importanceString;
			}
		}

		protected string CategoriesString
		{
			get
			{
				return this.categoriesString;
			}
		}

		protected ArrayList AttachmentWellRenderObjects
		{
			get
			{
				return this.attachmentWellRenderObjects;
			}
		}

		private string busyFreeString;

		private string importanceString;

		private string categoriesString;

		private object hasAttachments;

		private ArrayList attachmentWellRenderObjects;
	}
}
