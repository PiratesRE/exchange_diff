using System;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal class FindMe
	{
		internal FindMe()
		{
		}

		internal FindMe(string number, int timeout) : this(number, timeout, string.Empty)
		{
		}

		internal FindMe(string number, int timeout, string label)
		{
			this.number = number;
			this.timeout = timeout;
			this.label = label;
		}

		internal string Number
		{
			get
			{
				return this.number;
			}
			set
			{
				this.number = value;
			}
		}

		internal int Timeout
		{
			get
			{
				return this.timeout;
			}
			set
			{
				this.timeout = value;
			}
		}

		internal string Label
		{
			get
			{
				return this.label ?? string.Empty;
			}
			set
			{
				this.label = value;
			}
		}

		internal PAAValidationResult ValidationResult
		{
			get
			{
				return this.result;
			}
			set
			{
				this.result = value;
			}
		}

		internal PhoneNumber PhoneNumber
		{
			get
			{
				return this.phoneNumber;
			}
			set
			{
				this.phoneNumber = value;
			}
		}

		private string number;

		private int timeout;

		private string label;

		private PAAValidationResult result;

		private PhoneNumber phoneNumber;
	}
}
