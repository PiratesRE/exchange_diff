using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public struct EventLogText : IComparable, ISerializable
	{
		public EventLogText(string input)
		{
			this.value = null;
			if (this.IsValid(input))
			{
				this.value = input;
				return;
			}
			throw new ArgumentOutOfRangeException(DataStrings.EventLogText, DataStrings.InvalidInputErrorMsg);
		}

		public static EventLogText Parse(string s)
		{
			return new EventLogText(s);
		}

		private EventLogText(SerializationInfo info, StreamingContext context)
		{
			this.value = (string)info.GetValue("value", typeof(string));
			if (!this.IsValid(this.value))
			{
				throw new ArgumentOutOfRangeException(DataStrings.EventLogText, this.value.ToString());
			}
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("value", this.value);
		}

		private bool IsValid(string input)
		{
			return input == null || (input.Length <= 128 && EventLogText.ValidatingExpression.IsMatch(input));
		}

		public static EventLogText Empty
		{
			get
			{
				return default(EventLogText);
			}
		}

		public string Value
		{
			get
			{
				if (this.IsValid(this.value))
				{
					return this.value;
				}
				throw new ArgumentOutOfRangeException("Value", this.value.ToString());
			}
		}

		public override string ToString()
		{
			if (this.value == null)
			{
				return string.Empty;
			}
			return this.value.ToString();
		}

		public override int GetHashCode()
		{
			if (this.value == null)
			{
				return string.Empty.GetHashCode();
			}
			return this.value.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is EventLogText && this.Equals((EventLogText)obj);
		}

		public bool Equals(EventLogText obj)
		{
			return this.value == obj.Value;
		}

		public static bool operator ==(EventLogText a, EventLogText b)
		{
			return a.Value == b.Value;
		}

		public static bool operator !=(EventLogText a, EventLogText b)
		{
			return a.Value != b.Value;
		}

		public int CompareTo(object obj)
		{
			if (!(obj is EventLogText))
			{
				throw new ArgumentException("Parameter is not of type EventLogText.");
			}
			return string.Compare(this.value, ((EventLogText)obj).Value, StringComparison.OrdinalIgnoreCase);
		}

		public const int MaxLength = 128;

		public const string AllowedCharacters = "(.|[^.])";

		public static readonly Regex ValidatingExpression = new Regex("^(.|[^.])+$", RegexOptions.Compiled);

		private string value;
	}
}
