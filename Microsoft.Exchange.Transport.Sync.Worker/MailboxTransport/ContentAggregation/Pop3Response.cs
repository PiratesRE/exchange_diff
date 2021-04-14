using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Transport.Sync.Common;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class Pop3Response
	{
		internal Pop3Response(string responseLine)
		{
			SyncUtilities.ThrowIfArgumentNull("responseLine", responseLine);
			this.headline = responseLine;
			this.type = this.ParseResponseType();
			this.extendedResponse = this.ParseExtendedResponse();
		}

		internal Pop3ResponseType Type
		{
			get
			{
				return this.type;
			}
		}

		internal Stream MessageStream
		{
			get
			{
				if (this.messageStream == null)
				{
					this.messageStream = TemporaryStorage.Create();
				}
				return this.messageStream;
			}
		}

		internal string Headline
		{
			get
			{
				return this.headline;
			}
		}

		internal int ListingCount
		{
			get
			{
				if (this.listings == null)
				{
					return 0;
				}
				return this.listings.Count;
			}
		}

		internal int ListingCapacity
		{
			set
			{
				if (this.listings == null)
				{
					this.listings = new List<string>(value);
					return;
				}
				this.listings.Capacity = value;
			}
		}

		internal bool HasPermanentError
		{
			get
			{
				return this.HasExtendedResponseCode("[SYS/PERM]");
			}
		}

		internal bool HasInUseAuthenticationError
		{
			get
			{
				return this.HasExtendedResponseCode("[IN-USE]");
			}
		}

		internal bool HasSystemTemporaryError
		{
			get
			{
				return this.HasExtendedResponseCode("[SYS/TEMP]");
			}
		}

		internal bool HasLogInDelayAuthenticationError
		{
			get
			{
				return this.HasExtendedResponseCode("[LOGIN-DELAY]");
			}
		}

		internal bool HasExtendedResponse
		{
			get
			{
				return this.extendedResponse != null;
			}
		}

		public override string ToString()
		{
			if (this.listings != null)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0} and listings (count = {1})", new object[]
				{
					this.headline,
					this.listings.Count
				});
			}
			return this.headline;
		}

		internal void AppendListing(string responseLine)
		{
			if (this.listings == null)
			{
				this.listings = new List<string>(1);
			}
			this.listings.Add(responseLine);
		}

		internal ExDateTime ParseReceivedDate(bool useSentTime)
		{
			return SyncUtilities.GetReceivedDate(this.messageStream, useSentTime);
		}

		internal Exception TryParseSTATResponse(out int emailDropCount)
		{
			emailDropCount = -1;
			string[] array = this.headline.Split(Pop3Response.wordDelimiters, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length < 3)
			{
				return new FormatException("'drop listing' must have at least 3 words");
			}
			if (!int.TryParse(array[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out emailDropCount))
			{
				return new FormatException("'emailDropCount' must be a number.");
			}
			if (emailDropCount < 0)
			{
				return new FormatException("'emailDropCount' must be non-negative.");
			}
			return null;
		}

		internal Exception TryParseUIDLResponse(Pop3ResultData pop3ResultData)
		{
			if (this.listings == null || this.listings.Count != pop3ResultData.EmailDropCount)
			{
				return new FormatException("'uidl listing' must match the number of emails.");
			}
			pop3ResultData.AllocateUniqueIds();
			List<int> list = null;
			for (int i = 0; i < this.listings.Count; i++)
			{
				string[] array = this.listings[i].Split(Pop3Response.wordDelimiters, StringSplitOptions.RemoveEmptyEntries);
				if (array.Length < 2)
				{
					return new FormatException("'uidl listing' must have at least 2 words");
				}
				int num;
				if (!int.TryParse(array[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out num))
				{
					return new FormatException("'id' must be a number.");
				}
				if (num <= 0)
				{
					return new FormatException("'id' must be greater than zero.");
				}
				if (num <= pop3ResultData.EmailDropCount)
				{
					string text = array[1];
					if (!string.IsNullOrEmpty(text))
					{
						if (!pop3ResultData.HasUniqueId(num))
						{
							pop3ResultData.SetUniqueId(num, text);
						}
						else
						{
							if (list == null)
							{
								list = new List<int>();
							}
							list.Add(num);
						}
					}
				}
			}
			if (list != null)
			{
				foreach (int id in list)
				{
					pop3ResultData.SetUniqueId(id, null);
				}
			}
			return null;
		}

		internal Exception TryParseLISTResponse(Pop3ResultData pop3ResultData)
		{
			if (this.listings == null || this.listings.Count != pop3ResultData.EmailDropCount)
			{
				return new FormatException("'scan listing' must match the number of emails.");
			}
			pop3ResultData.AllocateEmailSizes();
			for (int i = 0; i < this.listings.Count; i++)
			{
				string[] array = this.listings[i].Split(Pop3Response.wordDelimiters, StringSplitOptions.RemoveEmptyEntries);
				if (array.Length < 2)
				{
					return new FormatException("'scan listing' must have at least 2 words");
				}
				int num;
				if (!int.TryParse(array[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out num))
				{
					return new FormatException("'id' must be a number.");
				}
				long num2;
				if (!long.TryParse(array[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out num2))
				{
					return new FormatException("'size' must be a number.");
				}
				if (num <= 0)
				{
					return new FormatException("'id' must be greater than zero.");
				}
				if (num2 < 0L)
				{
					return new FormatException("'size' must be non-negative.");
				}
				if (num <= pop3ResultData.EmailDropCount)
				{
					pop3ResultData.SetEmailSize(num, num2);
				}
			}
			return null;
		}

		internal Exception TryParseCapaResponse(out bool uniqueIdSupport, out int? retentionDays)
		{
			uniqueIdSupport = false;
			retentionDays = null;
			int i = 0;
			while (i < this.listings.Count)
			{
				bool flag = this.listings[i].StartsWith("UIDL", StringComparison.OrdinalIgnoreCase);
				bool flag2 = this.listings[i].StartsWith("EXPIRE ", StringComparison.OrdinalIgnoreCase);
				if (flag)
				{
					uniqueIdSupport = true;
					goto IL_4F;
				}
				if (flag2)
				{
					goto IL_4F;
				}
				IL_E1:
				i++;
				continue;
				IL_4F:
				string[] array = this.listings[i].Split(Pop3Response.wordDelimiters, StringSplitOptions.RemoveEmptyEntries);
				if (array.Length < 2)
				{
					goto IL_E1;
				}
				if (string.CompareOrdinal(array[1], "NEVER") == 0)
				{
					retentionDays = new int?(int.MaxValue);
					goto IL_E1;
				}
				int value;
				if (!int.TryParse(array[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
				{
					return new FormatException("'retentionDays' must be a number.");
				}
				if (retentionDays < 0)
				{
					return new FormatException("'retentionDays' must be non-negative.");
				}
				retentionDays = new int?(value);
				goto IL_E1;
			}
			return null;
		}

		private Pop3ResponseType ParseResponseType()
		{
			if (this.headline.StartsWith("+OK", StringComparison.Ordinal))
			{
				return Pop3ResponseType.ok;
			}
			if (this.headline.StartsWith("-ERR", StringComparison.Ordinal))
			{
				return Pop3ResponseType.err;
			}
			if (this.headline.StartsWith("+ ", StringComparison.Ordinal))
			{
				return Pop3ResponseType.sendMore;
			}
			return Pop3ResponseType.unknown;
		}

		private bool HasExtendedResponseCode(string extendedResponseCode)
		{
			return this.HasExtendedResponse && this.extendedResponse.Equals(extendedResponseCode, StringComparison.OrdinalIgnoreCase);
		}

		private string ParseExtendedResponse()
		{
			if (this.type != Pop3ResponseType.err)
			{
				return null;
			}
			string[] array = this.headline.Split(Pop3Response.wordDelimiters, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length < 2)
			{
				return null;
			}
			string text = array[1];
			if (!text.StartsWith("["))
			{
				return null;
			}
			int num = text.IndexOf(']');
			if (num == -1)
			{
				return null;
			}
			return text.Substring(0, num + 1);
		}

		private const string OK = "+OK";

		private const string ERR = "-ERR";

		private const string SystemPermanentError = "[SYS/PERM]";

		private const string SystemTemporaryError = "[SYS/TEMP]";

		private const string LogInDelayTemporaryError = "[LOGIN-DELAY]";

		private const string InUseTemporaryError = "[IN-USE]";

		private const string SendMore = "+ ";

		private const string Uidl = "UIDL";

		private const string Expire = "EXPIRE ";

		private const string Never = "NEVER";

		private static readonly char[] wordDelimiters = new char[]
		{
			' ',
			'\t'
		};

		private string headline;

		private string extendedResponse;

		private List<string> listings;

		private Stream messageStream;

		private Pop3ResponseType type;
	}
}
