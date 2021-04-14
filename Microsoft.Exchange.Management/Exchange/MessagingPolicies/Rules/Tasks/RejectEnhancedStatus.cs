using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public struct RejectEnhancedStatus : IComparable, ISerializable
	{
		public RejectEnhancedStatus(string input)
		{
			this.value = null;
			if (this.IsValid(input))
			{
				this.value = input;
				return;
			}
			throw new ArgumentException(Strings.InvalidRejectEnhancedStatus, "RejectEnhancedStatus");
		}

		private RejectEnhancedStatus(SerializationInfo info, StreamingContext context)
		{
			this.value = (string)info.GetValue("value", typeof(string));
			if (!this.IsValid(this.value))
			{
				throw new ArgumentException(Strings.InvalidRejectEnhancedStatus, "RejectEnhancedStatus");
			}
		}

		public static RejectEnhancedStatus Empty
		{
			get
			{
				return default(RejectEnhancedStatus);
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
				throw new ArgumentException(Strings.InvalidRejectEnhancedStatus, "RejectEnhancedStatus");
			}
		}

		public static RejectEnhancedStatus Parse(string s)
		{
			return new RejectEnhancedStatus(s);
		}

		public static bool operator ==(RejectEnhancedStatus a, RejectEnhancedStatus b)
		{
			return a.Value == b.Value;
		}

		public static bool operator !=(RejectEnhancedStatus a, RejectEnhancedStatus b)
		{
			return a.Value != b.Value;
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("value", this.value);
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
			return obj is RejectEnhancedStatus && this.Equals((RejectEnhancedStatus)obj);
		}

		public bool Equals(RejectEnhancedStatus obj)
		{
			return this.value == obj.Value;
		}

		public int CompareTo(object obj)
		{
			if (!(obj is RejectEnhancedStatus))
			{
				throw new ArgumentException("Parameter is not of type RejectEnhancedStatus.");
			}
			return string.Compare(this.value, ((RejectEnhancedStatus)obj).Value, StringComparison.OrdinalIgnoreCase);
		}

		private bool IsValid(string input)
		{
			return input == null || (input.Length <= 7 && new Regex("^(5\\.7\\.1|5\\.7\\.[1-9][0-9]|5\\.7\\.[1-9][0-9][0-9])$", RegexOptions.Compiled).IsMatch(input) && EnhancedStatusCodeImpl.IsValid(input));
		}

		public const int MaxLength = 7;

		public const string AllowedCharacters = "[\\.0-9]";

		public const string ValidatingExpression = "^(5\\.7\\.1|5\\.7\\.[1-9][0-9]|5\\.7\\.[1-9][0-9][0-9])$";

		private string value;
	}
}
