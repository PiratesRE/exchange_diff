using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public sealed class PrintPageWriter : MeetingPageWriter
	{
		public PrintPageWriter(MeetingPageWriter delegateWriter) : base(null, null, false, delegateWriter.IsInDeletedItems, delegateWriter.IsEmbeddedItem, delegateWriter.IsInJunkEmailFolder, delegateWriter.IsSuspectedPhishingItem, delegateWriter.IsLinkEnabled)
		{
			if (delegateWriter == null)
			{
				throw new ArgumentNullException("delegateWriter");
			}
			this.delegateWriter = delegateWriter;
			if (delegateWriter.AttendeeResponseWell != null)
			{
				delegateWriter.AttendeeResponseWell = new PrintRecipientWell(delegateWriter.AttendeeResponseWell);
			}
		}

		protected internal override void BuildInfobar()
		{
		}

		public override void RenderMeetingInfoArea(TextWriter writer, bool shouldRenderToolbars)
		{
		}

		public override void RenderTitle(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			RenderingUtilities.RenderSubject(writer, this.delegateWriter.MeetingPageItem);
		}

		public override void RenderSender(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (Utilities.IsOnBehalfOf(this.delegateWriter.ActualSender, this.delegateWriter.OriginalSender))
			{
				writer.Write(LocalizedStrings.GetHtmlEncoded(-165544498), RenderingUtilities.GetDisplaySenderName(this.delegateWriter.ActualSender), RenderingUtilities.GetDisplaySenderName(this.delegateWriter.OriginalSender));
				return;
			}
			writer.Write(RenderingUtilities.GetDisplaySenderName(this.delegateWriter.OriginalSender));
		}

		public override void RenderSubject(TextWriter writer, bool disableEdit)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			RenderingUtilities.RenderSubject(writer, this.delegateWriter.MeetingPageItem);
		}

		public override void RenderWhen(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write(this.delegateWriter.When);
			if (!string.IsNullOrEmpty(this.delegateWriter.OldWhen))
			{
				writer.Write(this.delegateWriter.MeetingPageUserContext.DirectionMark);
				writer.Write("(");
				writer.Write(this.delegateWriter.OldWhen);
				writer.Write(")");
				writer.Write(this.delegateWriter.MeetingPageUserContext.DirectionMark);
			}
		}

		public override void RenderLocation(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write(this.delegateWriter.Location);
			if (!string.IsNullOrEmpty(this.delegateWriter.OldLocation))
			{
				writer.Write(this.delegateWriter.MeetingPageUserContext.DirectionMark);
				writer.Write("(");
				writer.Write(this.delegateWriter.OldLocation);
				writer.Write("<nobr>");
				writer.Write(")");
				writer.Write(this.delegateWriter.MeetingPageUserContext.DirectionMark);
				writer.Write("</nobr>");
			}
		}

		protected override void InternalDispose(bool isDisposing)
		{
			try
			{
				if (isDisposing && this.delegateWriter != null)
				{
					this.delegateWriter.Dispose();
				}
			}
			finally
			{
				base.InternalDispose(isDisposing);
			}
		}

		public override bool ShouldRenderRecipientWell(RecipientWellType recipientWellType)
		{
			return this.delegateWriter.ShouldRenderRecipientWell(recipientWellType);
		}

		public override void RenderInspectorMailToolbar(TextWriter writer)
		{
		}

		public override void RenderDescription(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (!string.IsNullOrEmpty(this.DescriptionTag))
			{
				Utilities.HtmlEncode(this.delegateWriter.DescriptionTag, writer);
			}
		}

		public override void RenderStartTime(TextWriter writer)
		{
		}

		public override Infobar FormInfobar
		{
			get
			{
				return null;
			}
		}

		public override int StoreObjectType
		{
			get
			{
				return this.delegateWriter.StoreObjectType;
			}
		}

		public override RecipientWell RecipientWell
		{
			get
			{
				return new PrintRecipientWell(this.delegateWriter.RecipientWell);
			}
		}

		public override RecipientWell AttendeeResponseWell
		{
			get
			{
				return this.delegateWriter.AttendeeResponseWell;
			}
			set
			{
				this.delegateWriter.AttendeeResponseWell = value;
			}
		}

		public override bool ShouldRenderSentField
		{
			get
			{
				return this.delegateWriter.ShouldRenderSentField;
			}
		}

		public override bool ShouldRenderAttendeeResponseWells
		{
			get
			{
				return this.delegateWriter.ShouldRenderAttendeeResponseWells;
			}
		}

		internal override CalendarItemBase CalendarItemBase
		{
			get
			{
				return this.delegateWriter.CalendarItemBase;
			}
			set
			{
				this.delegateWriter.CalendarItemBase = value;
			}
		}

		public override bool ReminderIsSet
		{
			get
			{
				return this.delegateWriter.ReminderIsSet;
			}
		}

		protected internal override bool IsPreviewForm
		{
			get
			{
				return false;
			}
		}

		internal override Participant ActualSender
		{
			get
			{
				return this.delegateWriter.ActualSender;
			}
		}

		internal override Participant OriginalSender
		{
			get
			{
				return this.delegateWriter.OriginalSender;
			}
		}

		protected internal override UserContext MeetingPageUserContext
		{
			get
			{
				return this.delegateWriter.MeetingPageUserContext;
			}
		}

		public override string MeetingStatus
		{
			get
			{
				return this.delegateWriter.MeetingStatus;
			}
		}

		protected internal override string DescriptionTag
		{
			get
			{
				return this.delegateWriter.DescriptionTag;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PrintPageWriter>(this);
		}

		public override bool ShouldRenderReminder
		{
			get
			{
				return false;
			}
		}

		private MeetingPageWriter delegateWriter;
	}
}
