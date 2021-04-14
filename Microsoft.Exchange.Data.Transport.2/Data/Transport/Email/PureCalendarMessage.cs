using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Exchange.Data.ContentTypes.iCalendar;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Mime.Encoders;

namespace Microsoft.Exchange.Data.Transport.Email
{
	internal class PureCalendarMessage : IBody, IRelayable
	{
		internal PureCalendarMessage(PureMimeMessage mimeMessage, MimePart mimePart, Charset charset)
		{
			this.originalCharset = charset;
			this.targetCharset = charset;
			this.mimeMessage = mimeMessage;
			this.mimePart = mimePart;
		}

		public string Subject
		{
			get
			{
				string text = this.properties[PropertyId.Summary] as string;
				if (string.IsNullOrEmpty(text))
				{
					text = string.Empty;
				}
				return text;
			}
			set
			{
				this.properties[PropertyId.Summary] = value;
			}
		}

		public Charset TargetCharset
		{
			get
			{
				return this.targetCharset;
			}
			set
			{
				this.targetCharset = value;
			}
		}

		BodyFormat IBody.GetBodyFormat()
		{
			BodyData bodyData = this.BodyData;
			if (bodyData == null)
			{
				return BodyFormat.None;
			}
			return bodyData.BodyFormat;
		}

		string IBody.GetCharsetName()
		{
			BodyData bodyData = this.BodyData;
			return bodyData.CharsetName;
		}

		MimePart IBody.GetMimePart()
		{
			return this.mimePart;
		}

		Stream IBody.GetContentReadStream()
		{
			BodyData bodyData = this.BodyData;
			Stream readStream = bodyData.GetReadStream();
			return bodyData.ConvertReadStreamFormat(readStream);
		}

		bool IBody.TryGetContentReadStream(out Stream stream)
		{
			BodyData bodyData = this.BodyData;
			stream = bodyData.GetReadStream();
			stream = bodyData.ConvertReadStreamFormat(stream);
			return true;
		}

		Stream IBody.GetContentWriteStream(Charset charset)
		{
			BodyData bodyData = this.BodyData;
			bodyData.ReleaseStorage();
			if (charset != null && charset != bodyData.Charset && bodyData.Charset != Charset.UTF8)
			{
				bodyData.SetFormat(BodyFormat.Text, InternalBodyFormat.Text, Charset.UTF8);
				this.targetCharset = Charset.UTF8;
				this.mimeMessage.SetBodyPartCharset(this.mimePart, Charset.UTF8);
			}
			Stream stream = new BodyContentWriteStream(this);
			return bodyData.ConvertWriteStreamFormat(stream, charset);
		}

		void IBody.SetNewContent(DataStorage storage, long start, long end)
		{
			this.BodyData.SetStorage(storage, start, end);
			this.TouchBody();
		}

		bool IBody.ConversionNeeded(int[] validCodepages)
		{
			return false;
		}

		public string MapiMessageClass
		{
			get
			{
				if (this.mapiMessageClass == null)
				{
					this.FindMapiMessageClass();
				}
				return this.mapiMessageClass;
			}
		}

		internal PureMimeMessage MimeMessage
		{
			[DebuggerStepThrough]
			get
			{
				return this.mimeMessage;
			}
		}

		internal BodyData BodyData
		{
			get
			{
				if (this.properties.ContainsKey(PropertyId.Description))
				{
					return this.properties[PropertyId.Description] as BodyData;
				}
				string text = this.properties[PropertyId.Method] as string;
				if (!string.IsNullOrEmpty(text))
				{
					text = text.Trim();
					if ((text.Equals("COUNTER", StringComparison.OrdinalIgnoreCase) || text.Equals("REPLY", StringComparison.OrdinalIgnoreCase)) && this.properties.ContainsKey(PropertyId.Comment))
					{
						return this.properties[PropertyId.Comment] as BodyData;
					}
				}
				return null;
			}
		}

		internal void TouchBody()
		{
			this.properties.Touch(PropertyId.Description);
		}

		internal bool Load()
		{
			if (!this.mimePart.TryGetContentReadStream(out this.stream))
			{
				return false;
			}
			bool result;
			try
			{
				this.properties = new PureCalendarMessage.CalendarPropertyBag(this);
				using (CalendarReader calendarReader = new CalendarReader(new SuppressCloseStream(this.stream), this.originalCharset.Name, CalendarComplianceMode.Loose))
				{
					this.properties.Load(calendarReader, this.originalCharset);
					result = true;
				}
			}
			catch (ByteEncoderException)
			{
				result = false;
			}
			return result;
		}

		public void WriteTo(Stream destination)
		{
			this.stream.Position = 0L;
			using (CalendarReader calendarReader = new CalendarReader(new SuppressCloseStream(this.stream), this.originalCharset.Name, CalendarComplianceMode.Loose))
			{
				using (CalendarWriter calendarWriter = new CalendarWriter(new SuppressCloseStream(destination), this.targetCharset.Name))
				{
					calendarWriter.SetLooseMode();
					this.properties.Write(calendarReader, calendarWriter);
				}
			}
		}

		private void Invalidate()
		{
			this.mimeMessage.InvalidateCalendarContent();
		}

		private void FindMapiMessageClass()
		{
			string text = this.properties[PropertyId.Method] as string;
			if (string.IsNullOrEmpty(text))
			{
				this.mapiMessageClass = "IPM.Note";
				return;
			}
			text = text.Trim();
			if (text.Equals("PUBLISH", StringComparison.OrdinalIgnoreCase) || text.Equals("REQUEST", StringComparison.OrdinalIgnoreCase))
			{
				this.mapiMessageClass = "IPM.Schedule.Meeting.Request";
				return;
			}
			if (text.Equals("REPLY", StringComparison.OrdinalIgnoreCase))
			{
				this.FindClassFromParticipationStatus();
				return;
			}
			if (text.Equals("CANCEL", StringComparison.OrdinalIgnoreCase))
			{
				this.mapiMessageClass = "IPM.Schedule.Meeting.Canceled";
				return;
			}
			this.mapiMessageClass = "IPM.Note";
		}

		private void FindClassFromParticipationStatus()
		{
			this.mapiMessageClass = "IPM.Schedule.Meeting.Resp.Tent";
			string text = this.properties[PropertyId.Attendee] as string;
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			if (text.Equals("ACCEPTED", StringComparison.OrdinalIgnoreCase))
			{
				this.mapiMessageClass = "IPM.Schedule.Meeting.Resp.Pos";
				return;
			}
			if (text.Equals("DECLINED", StringComparison.OrdinalIgnoreCase))
			{
				this.mapiMessageClass = "IPM.Schedule.Meeting.Resp.Neg";
			}
		}

		private PureCalendarMessage.CalendarPropertyBag properties;

		private PureMimeMessage mimeMessage;

		private MimePart mimePart;

		private Stream stream;

		private Charset originalCharset;

		private Charset targetCharset;

		private string mapiMessageClass;

		internal struct CalendarPropertyBag
		{
			internal CalendarPropertyBag(PureCalendarMessage calendarMessage)
			{
				this.calendarMessage = calendarMessage;
				this.properties = new Dictionary<PropertyId, object>(PureCalendarMessage.CalendarPropertyBag.CalendarPropertyIdComparer);
				this.dirty = new Dictionary<PropertyId, bool>(PureCalendarMessage.CalendarPropertyBag.CalendarPropertyIdComparer);
				foreach (PropertyId key in PureCalendarMessage.CalendarPropertyBag.MessageProperties)
				{
					this.dirty[key] = false;
				}
			}

			internal bool ContainsKey(PropertyId property)
			{
				return this.properties.ContainsKey(property);
			}

			internal object this[PropertyId id]
			{
				get
				{
					object result;
					if (!this.properties.TryGetValue(id, out result))
					{
						return null;
					}
					return result;
				}
				set
				{
					if (this[id] != value)
					{
						this.Invalidate();
						this.dirty[id] = true;
					}
					this.properties[id] = value;
				}
			}

			internal void Touch(PropertyId id)
			{
				this.Invalidate();
				this.dirty[id] = true;
			}

			internal void Load(CalendarReader reader, Charset charset)
			{
				while (reader.ReadNextComponent())
				{
					if (ComponentId.VEvent == reader.ComponentId)
					{
						CalendarPropertyReader propertyReader = reader.PropertyReader;
						while (propertyReader.ReadNextProperty())
						{
							PropertyId propertyId = propertyReader.PropertyId;
							if (this.dirty.ContainsKey(propertyId) && !this.properties.ContainsKey(propertyId))
							{
								if (PropertyId.Description == propertyId || PropertyId.Comment == propertyId)
								{
									byte[] array = null;
									TemporaryDataStorage temporaryDataStorage = new TemporaryDataStorage();
									using (Stream stream = temporaryDataStorage.OpenWriteStream(true))
									{
										using (Stream valueReadStream = propertyReader.GetValueReadStream())
										{
											DataStorage.CopyStreamToStream(valueReadStream, stream, long.MaxValue, ref array);
										}
									}
									BodyData bodyData = new BodyData();
									bodyData.SetFormat(BodyFormat.Text, InternalBodyFormat.Text, charset);
									bodyData.SetStorage(temporaryDataStorage, 0L, long.MaxValue);
									temporaryDataStorage.Release();
									this.properties[propertyId] = bodyData;
								}
								else if (PropertyId.Attendee == propertyId)
								{
									CalendarParameterReader parameterReader = propertyReader.ParameterReader;
									while (parameterReader.ReadNextParameter())
									{
										if (parameterReader.ParameterId == ParameterId.ParticipationStatus)
										{
											this.properties[propertyId] = parameterReader.ReadValue();
										}
									}
								}
								else
								{
									this.properties[propertyId] = propertyReader.ReadValue();
								}
							}
						}
					}
					if (ComponentId.VCalendar == reader.ComponentId)
					{
						CalendarPropertyReader propertyReader2 = reader.PropertyReader;
						while (propertyReader2.ReadNextProperty())
						{
							PropertyId propertyId2 = propertyReader2.PropertyId;
							if (PropertyId.Method == propertyId2)
							{
								this.properties[propertyId2] = propertyReader2.ReadValue();
							}
						}
					}
				}
			}

			internal void Write(CalendarReader reader, CalendarWriter writer)
			{
				int num = 0;
				while (reader.ReadNextComponent())
				{
					while (num-- >= reader.Depth)
					{
						writer.EndComponent();
					}
					writer.StartComponent(reader.ComponentName);
					this.WriteProperties(reader, writer);
					num = reader.Depth;
				}
				while (num-- > 0)
				{
					writer.EndComponent();
				}
			}

			private void WriteProperties(CalendarReader reader, CalendarWriter writer)
			{
				CalendarPropertyReader propertyReader = reader.PropertyReader;
				while (propertyReader.ReadNextProperty())
				{
					PropertyId propertyId = propertyReader.PropertyId;
					bool flag;
					if (ComponentId.VEvent == reader.ComponentId && this.dirty.TryGetValue(propertyId, out flag) && flag)
					{
						object obj = this[propertyId];
						if (obj != null)
						{
							this.WriteProperty(writer, propertyId, obj);
						}
					}
					else
					{
						writer.WriteProperty(propertyReader);
					}
				}
			}

			private void WriteProperty(CalendarWriter writer, PropertyId id, object value)
			{
				BodyData bodyData = value as BodyData;
				if (bodyData != null)
				{
					using (Stream readStream = bodyData.GetReadStream())
					{
						using (StreamReader streamReader = new StreamReader(readStream, bodyData.Encoding))
						{
							string value2 = streamReader.ReadToEnd();
							writer.WriteProperty(id, value2);
						}
						return;
					}
				}
				string value3 = value as string;
				if (!string.IsNullOrEmpty(value3))
				{
					writer.WriteProperty(id, value3);
				}
			}

			private void Invalidate()
			{
				this.calendarMessage.Invalidate();
			}

			private static readonly PropertyId[] MessageProperties = new PropertyId[]
			{
				PropertyId.Method,
				PropertyId.Description,
				PropertyId.Summary,
				PropertyId.Attendee,
				PropertyId.Comment
			};

			private static readonly PureCalendarMessage.CalendarPropertyBag.PropertyIdComparer CalendarPropertyIdComparer = new PureCalendarMessage.CalendarPropertyBag.PropertyIdComparer();

			private Dictionary<PropertyId, object> properties;

			private Dictionary<PropertyId, bool> dirty;

			private PureCalendarMessage calendarMessage;

			private class PropertyIdComparer : IEqualityComparer<PropertyId>
			{
				public bool Equals(PropertyId x, PropertyId y)
				{
					return x == y;
				}

				public int GetHashCode(PropertyId obj)
				{
					return (int)obj;
				}
			}
		}

		private static class CalendarMethod
		{
			public const string Publish = "PUBLISH";

			public const string Request = "REQUEST";

			public const string Reply = "REPLY";

			public const string Cancel = "CANCEL";

			public const string Refresh = "REFRESH";

			public const string Counter = "COUNTER";
		}

		private static class CalendarStatus
		{
			public const string NeedsAction = "NEEDS-ACTION";

			public const string Accepted = "ACCEPTED";

			public const string Declined = "DECLINED";

			public const string Tentative = "TENTATIVE";

			public const string Delegated = "DELEGATED";
		}
	}
}
