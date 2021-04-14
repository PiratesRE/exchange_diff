using System;
using System.IO;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.ContentTypes.Internal;
using Microsoft.Exchange.Data.Globalization;

namespace Microsoft.Exchange.Data.ContentTypes.iCalendar
{
	public class CalendarReader : IDisposable
	{
		public CalendarReader(Stream stream) : this(stream, "utf-8", CalendarComplianceMode.Strict)
		{
		}

		public CalendarReader(Stream stream, string encodingName, CalendarComplianceMode calendarComplianceMode)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.CanRead)
			{
				throw new ArgumentException(CalendarStrings.StreamMustAllowRead, "stream");
			}
			if (encodingName == null)
			{
				throw new ArgumentNullException("encodingName");
			}
			this.reader = new ContentLineReader(stream, Charset.GetEncoding(encodingName), new ComplianceTracker(FormatType.Calendar, (ComplianceMode)calendarComplianceMode), new CalendarValueTypeContainer());
		}

		public ComponentId ComponentId
		{
			get
			{
				this.CheckDisposed("ComponentId::get");
				return CalendarCommon.GetComponentEnum(this.ComponentName);
			}
		}

		public string ComponentName
		{
			get
			{
				this.CheckDisposed("ComponentName::get");
				this.reader.AssertValidState(~ContentLineNodeType.DocumentEnd);
				return this.reader.ComponentName;
			}
		}

		public CalendarComplianceMode ComplianceMode
		{
			get
			{
				this.CheckDisposed("ComplianceMode::get");
				return (CalendarComplianceMode)this.reader.ComplianceTracker.Mode;
			}
		}

		public CalendarComplianceStatus ComplianceStatus
		{
			get
			{
				this.CheckDisposed("ComplianceStatus::get");
				return (CalendarComplianceStatus)this.reader.ComplianceTracker.Status;
			}
		}

		public CalendarPropertyReader PropertyReader
		{
			get
			{
				this.CheckDisposed("PropertyReader::get");
				this.reader.AssertValidState(~ContentLineNodeType.DocumentEnd);
				return new CalendarPropertyReader(this.reader);
			}
		}

		public int Depth
		{
			get
			{
				this.CheckDisposed("Depth::get");
				return this.reader.Depth;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && !this.isClosed && this.reader != null)
			{
				this.reader.Dispose();
				this.reader = null;
			}
			this.isClosed = true;
		}

		protected void CheckDisposed(string methodName)
		{
			if (this.isClosed)
			{
				throw new ObjectDisposedException("CalendarReader", methodName);
			}
		}

		public virtual void Close()
		{
			this.Dispose();
		}

		public bool ReadNextComponent()
		{
			this.CheckDisposed("ReadNextComponent");
			return this.reader.ReadNextComponent();
		}

		public bool ReadFirstChildComponent()
		{
			this.CheckDisposed("ReadFirstChildComponent");
			this.reader.AssertValidState((ContentLineNodeType)(-1));
			return this.reader.ReadFirstChildComponent();
		}

		public bool ReadNextSiblingComponent()
		{
			this.CheckDisposed("ReadNextSiblingComponent");
			this.reader.AssertValidState((ContentLineNodeType)(-1));
			return this.reader.ReadNextSiblingComponent();
		}

		public void ResetComplianceStatus()
		{
			this.CheckDisposed("ResetComplianceStatus");
			this.reader.ComplianceTracker.Reset();
		}

		private ContentLineReader reader;

		private bool isClosed;
	}
}
