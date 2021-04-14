using System;
using System.IO;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.SchemaConverter;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	internal class XsoEmailBodyContentProperty : XsoContentProperty, IBodyContentProperty, IBodyProperty, IContentProperty, IMIMEDataProperty, IMIMERelatedProperty, IProperty
	{
		public XsoEmailBodyContentProperty(PropertyType propertyType = PropertyType.ReadOnly) : base(propertyType)
		{
			this.xsoBodyProperty = new XsoBodyProperty(propertyType);
		}

		public Stream RtfData
		{
			get
			{
				return this.xsoBodyProperty.RtfData;
			}
		}

		public int RtfSize
		{
			get
			{
				return this.xsoBodyProperty.RtfSize;
			}
		}

		public Stream TextData
		{
			get
			{
				return this.xsoBodyProperty.TextData;
			}
		}

		public bool TextPresent
		{
			get
			{
				return this.xsoBodyProperty.TextPresent;
			}
		}

		public int TextSize
		{
			get
			{
				return this.xsoBodyProperty.TextSize;
			}
		}

		public Stream GetTextData(int length)
		{
			return this.xsoBodyProperty.GetTextData(length);
		}

		public override void Bind(StoreObject item)
		{
			base.Bind(item);
			if (this.xsoBodyProperty != null)
			{
				this.xsoBodyProperty.Bind(item);
			}
		}

		public override void Unbind()
		{
			if (this.xsoBodyProperty != null)
			{
				this.xsoBodyProperty.Unbind();
			}
			base.Unbind();
		}

		public override void PreProcessProperty()
		{
			if (!this.IsItemDelegated())
			{
				base.PreProcessProperty();
				return;
			}
			this.originalItem = null;
			MessageItem messageItem = this.CreateSubstituteDelegatedMeetingRequestMessage();
			if (messageItem != null)
			{
				this.originalItem = (Item)base.XsoItem;
				base.XsoItem = messageItem;
				this.actualBody = messageItem.Body;
				if (this.xsoBodyProperty != null)
				{
					this.xsoBodyProperty.Unbind();
				}
				this.xsoBodyProperty.Bind(messageItem);
				return;
			}
			throw new ConversionException("Delegated meeting request body could not be converted");
		}

		protected override void InternalCopyFromModified(IProperty srcProperty)
		{
			if (Command.CurrentCommand.Request.Version < 160)
			{
				throw new ConversionException("Email body is a read-only property and should not be set!");
			}
			base.InternalCopyFromModified(srcProperty);
		}

		private MessageItem CreateSubstituteDelegatedMeetingRequestMessage()
		{
			MessageItem messageItem = null;
			bool flag = false;
			try
			{
				MeetingMessage meetingMessage = base.XsoItem as MeetingMessage;
				messageItem = MessageItem.CreateInMemory(StoreObjectSchema.ContentConversionProperties);
				if (messageItem == null)
				{
					AirSyncDiagnostics.TraceError(ExTraceGlobals.XsoTracer, null, "CreateSubstituteDelegatedMeetingRequestMessage failed to create in memory message item");
					return null;
				}
				Item.CopyItemContent(meetingMessage, messageItem);
				messageItem.ClassName = "IPM.Note";
				messageItem.AttachmentCollection.RemoveAll();
				int num = meetingMessage.Body.PreviewText.IndexOf("*~*~*~*~*~*~*~*~*~*");
				string value;
				if (num == -1)
				{
					ExTimeZone promotedTimeZoneFromItem = TimeZoneHelper.GetPromotedTimeZoneFromItem(meetingMessage);
					value = CalendarItemBase.CreateWhenStringForBodyPrefix(meetingMessage, promotedTimeZoneFromItem);
				}
				else
				{
					value = meetingMessage.Body.PreviewText.Substring(0, num + "*~*~*~*~*~*~*~*~*~*".Length);
				}
				using (TextWriter textWriter = messageItem.Body.OpenTextWriter(BodyFormat.TextPlain))
				{
					textWriter.Write(value);
				}
			}
			catch (Exception ex)
			{
				AirSyncDiagnostics.TraceError<string>(ExTraceGlobals.XsoTracer, null, "CreateSubstituteDelegatedMeetingRequestMessage: exception thrown: {0}", ex.Message);
				if (messageItem != null)
				{
					flag = true;
				}
				throw;
			}
			finally
			{
				if (flag)
				{
					messageItem.Dispose();
					messageItem = null;
				}
			}
			return messageItem;
		}

		private XsoBodyProperty xsoBodyProperty;
	}
}
