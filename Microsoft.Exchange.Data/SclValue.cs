using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public struct SclValue : IComparable, ISerializable
	{
		public SclValue(int input)
		{
			this.value = int.MinValue;
			if (this.IsValid(input))
			{
				this.value = input;
				return;
			}
			throw new ArgumentOutOfRangeException(DataStrings.SclValue, DataStrings.InvalidInputErrorMsg);
		}

		private SclValue(SerializationInfo info, StreamingContext context)
		{
			this.value = (int)info.GetValue("value", typeof(int));
			if (!this.IsValid(this.value))
			{
				throw new ArgumentOutOfRangeException(DataStrings.SclValue, this.value.ToString());
			}
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("value", this.value);
		}

		private bool IsValid(int input)
		{
			return -1 <= input && input <= 9;
		}

		public static SclValue Parse(string s)
		{
			return new SclValue(int.Parse(s));
		}

		public int Value
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
			return this.value.ToString();
		}

		public override int GetHashCode()
		{
			return this.value.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is SclValue && this.Equals((SclValue)obj);
		}

		public bool Equals(SclValue obj)
		{
			return this.value == obj.Value;
		}

		public static bool operator ==(SclValue a, SclValue b)
		{
			return a.Value == b.Value;
		}

		public static bool operator !=(SclValue a, SclValue b)
		{
			return a.Value != b.Value;
		}

		public int CompareTo(object obj)
		{
			if (!(obj is SclValue))
			{
				throw new ArgumentException("Parameter is not of type SclValue.");
			}
			return this.value.CompareTo(((SclValue)obj).Value);
		}

		public const int MinValue = -1;

		public const int MaxValue = 9;

		private int value;
	}
}
