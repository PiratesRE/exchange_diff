using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Metabase
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IisTasksValidationInvalidUnicodeException : LocalizedException
	{
		public IisTasksValidationInvalidUnicodeException(string propertyName, string value, char badChar, int unicodeValue, int charIndex) : base(Strings.IisTasksValidationInvalidUnicodeException(propertyName, value, badChar, unicodeValue, charIndex))
		{
			this.propertyName = propertyName;
			this.value = value;
			this.badChar = badChar;
			this.unicodeValue = unicodeValue;
			this.charIndex = charIndex;
		}

		public IisTasksValidationInvalidUnicodeException(string propertyName, string value, char badChar, int unicodeValue, int charIndex, Exception innerException) : base(Strings.IisTasksValidationInvalidUnicodeException(propertyName, value, badChar, unicodeValue, charIndex), innerException)
		{
			this.propertyName = propertyName;
			this.value = value;
			this.badChar = badChar;
			this.unicodeValue = unicodeValue;
			this.charIndex = charIndex;
		}

		protected IisTasksValidationInvalidUnicodeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.propertyName = (string)info.GetValue("propertyName", typeof(string));
			this.value = (string)info.GetValue("value", typeof(string));
			this.badChar = (char)info.GetValue("badChar", typeof(char));
			this.unicodeValue = (int)info.GetValue("unicodeValue", typeof(int));
			this.charIndex = (int)info.GetValue("charIndex", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("propertyName", this.propertyName);
			info.AddValue("value", this.value);
			info.AddValue("badChar", this.badChar);
			info.AddValue("unicodeValue", this.unicodeValue);
			info.AddValue("charIndex", this.charIndex);
		}

		public string PropertyName
		{
			get
			{
				return this.propertyName;
			}
		}

		public string Value
		{
			get
			{
				return this.value;
			}
		}

		public char BadChar
		{
			get
			{
				return this.badChar;
			}
		}

		public int UnicodeValue
		{
			get
			{
				return this.unicodeValue;
			}
		}

		public int CharIndex
		{
			get
			{
				return this.charIndex;
			}
		}

		private readonly string propertyName;

		private readonly string value;

		private readonly char badChar;

		private readonly int unicodeValue;

		private readonly int charIndex;
	}
}
