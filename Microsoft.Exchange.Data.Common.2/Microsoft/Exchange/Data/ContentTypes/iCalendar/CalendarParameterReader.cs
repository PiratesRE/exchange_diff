using System;
using Microsoft.Exchange.Data.ContentTypes.Internal;

namespace Microsoft.Exchange.Data.ContentTypes.iCalendar
{
	public struct CalendarParameterReader
	{
		internal CalendarParameterReader(ContentLineReader reader)
		{
			this.reader = reader;
		}

		public ParameterId ParameterId
		{
			get
			{
				this.reader.AssertValidState(ContentLineNodeType.Parameter);
				return CalendarCommon.GetParameterEnum(this.reader.ParameterName);
			}
		}

		public string Name
		{
			get
			{
				this.reader.AssertValidState(ContentLineNodeType.Parameter);
				return this.reader.ParameterName;
			}
		}

		public string ReadValue()
		{
			this.reader.AssertValidState(ContentLineNodeType.Parameter);
			return this.reader.ReadParameterValue(true);
		}

		public bool ReadNextValue()
		{
			this.reader.AssertValidState(ContentLineNodeType.Parameter | ContentLineNodeType.DocumentEnd);
			return this.reader.ReadNextParameterValue();
		}

		public bool ReadNextParameter()
		{
			this.reader.AssertValidState(ContentLineNodeType.Parameter | ContentLineNodeType.Property | ContentLineNodeType.DocumentEnd);
			return this.reader.ReadNextParameter();
		}

		private ContentLineReader reader;
	}
}
