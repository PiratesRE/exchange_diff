using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.ContentTypes.Internal;

namespace Microsoft.Exchange.Data.ContentTypes.vCard
{
	public class ContactReader : IDisposable
	{
		public ContactReader(Stream stream) : this(stream, Encoding.UTF8, ContactComplianceMode.Strict)
		{
		}

		public ContactReader(Stream stream, Encoding encoding, ContactComplianceMode contactComplianceMode)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.CanRead)
			{
				throw new ArgumentException(CalendarStrings.StreamMustAllowRead, "stream");
			}
			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}
			this.reader = new ContentLineReader(stream, encoding, new ComplianceTracker(FormatType.VCard, (ComplianceMode)contactComplianceMode), new ContactValueTypeContainer());
		}

		public ContactComplianceMode ComplianceMode
		{
			get
			{
				this.CheckDisposed("ComplianceMode::get");
				return (ContactComplianceMode)this.reader.ComplianceTracker.Mode;
			}
		}

		public ContactComplianceStatus ComplianceStatus
		{
			get
			{
				this.CheckDisposed("ComplianceStatus::get");
				return (ContactComplianceStatus)this.reader.ComplianceTracker.Status;
			}
		}

		public ContactPropertyReader PropertyReader
		{
			get
			{
				this.CheckDisposed("PropertyReader::get");
				this.reader.AssertValidState(~ContentLineNodeType.DocumentEnd);
				return new ContactPropertyReader(this.reader);
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
				throw new ObjectDisposedException("ContactReader", methodName);
			}
		}

		public virtual void Close()
		{
			this.Dispose();
		}

		public bool ReadNext()
		{
			this.CheckDisposed("ReadNext");
			if (this.reader.ReadNextComponent())
			{
				if (string.Compare(this.reader.ComponentName, "VCARD", StringComparison.OrdinalIgnoreCase) != 0)
				{
					this.reader.ComplianceTracker.SetComplianceStatus(Microsoft.Exchange.Data.ContentTypes.Internal.ComplianceStatus.ComponentNameMismatch, CalendarStrings.ComponentNameMismatch);
				}
				if (this.reader.Depth > 1)
				{
					this.reader.ComplianceTracker.SetComplianceStatus(Microsoft.Exchange.Data.ContentTypes.Internal.ComplianceStatus.NotAllComponentsClosed, CalendarStrings.NotAllComponentsClosed);
				}
				return true;
			}
			return false;
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
