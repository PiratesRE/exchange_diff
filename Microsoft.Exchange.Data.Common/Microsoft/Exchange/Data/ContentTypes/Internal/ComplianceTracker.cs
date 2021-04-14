using System;
using Microsoft.Exchange.Data.ContentTypes.iCalendar;
using Microsoft.Exchange.Data.ContentTypes.vCard;

namespace Microsoft.Exchange.Data.ContentTypes.Internal
{
	internal class ComplianceTracker
	{
		public ComplianceTracker(FormatType format, ComplianceMode complianceMode)
		{
			this.format = format;
			this.complianceMode = complianceMode;
		}

		public void SetComplianceStatus(ComplianceStatus status, string message)
		{
			this.complianceStatus |= status;
			if (ComplianceMode.Strict == this.complianceMode)
			{
				if (this.format == FormatType.Calendar)
				{
					throw new InvalidCalendarDataException(message);
				}
				if (FormatType.VCard == this.format)
				{
					throw new InvalidContactDataException(message);
				}
			}
		}

		public ComplianceStatus Status
		{
			get
			{
				return this.complianceStatus;
			}
		}

		public ComplianceMode Mode
		{
			get
			{
				return this.complianceMode;
			}
		}

		public FormatType Format
		{
			get
			{
				return this.format;
			}
		}

		public void Reset()
		{
			this.complianceStatus = ComplianceStatus.Compliant;
		}

		private FormatType format;

		private ComplianceMode complianceMode;

		private ComplianceStatus complianceStatus;
	}
}
