using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal class MobileRecipient : IEquatable<MobileRecipient>
	{
		public static string GetNumberString(MobileRecipient recipient)
		{
			if (recipient == null)
			{
				return null;
			}
			return recipient.E164Number.Number;
		}

		public static MobileRecipient Parse(string number)
		{
			return new MobileRecipient(E164Number.Parse(number));
		}

		public static bool TryParse(string number, out MobileRecipient recipient)
		{
			recipient = null;
			E164Number number2 = null;
			if (!E164Number.TryParse(number, out number2))
			{
				return false;
			}
			recipient = new MobileRecipient(number2);
			return true;
		}

		private MobileRecipient()
		{
			this.Region = null;
			this.Carrier = -1;
			this.Exceptions = new List<Exception>();
		}

		public MobileRecipient(E164Number number) : this()
		{
			if (null == number)
			{
				throw new ArgumentNullException("number");
			}
			this.E164Number = number;
		}

		public RegionInfo Region { get; set; }

		public int Carrier { get; set; }

		public IList<Exception> Exceptions { get; private set; }

		public E164Number E164Number { get; private set; }

		public bool Equals(MobileRecipient other)
		{
			return other != null && this.E164Number == other.E164Number;
		}

		public override bool Equals(object other)
		{
			return this.Equals(other as MobileRecipient);
		}

		public override int GetHashCode()
		{
			return this.E164Number.GetHashCode();
		}

		public override string ToString()
		{
			return this.E164Number.ToString();
		}
	}
}
