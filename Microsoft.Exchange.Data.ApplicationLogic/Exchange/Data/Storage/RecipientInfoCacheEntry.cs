using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RecipientInfoCacheEntry : IComparable<RecipientInfoCacheEntry>
	{
		public RecipientInfoCacheEntry(string displayName, string smtpAddress, string routingAddress, string alias, string routingType, AddressOrigin addressOrigin, int recipientFlags, string itemId, EmailAddressIndex emailAddressIndex, string sipUri, string mobilePhoneNumber) : this(displayName, smtpAddress, routingAddress, alias, routingType, addressOrigin, recipientFlags, itemId, emailAddressIndex, sipUri, mobilePhoneNumber, 6, DateTime.UtcNow.Ticks, 0, 0, 0)
		{
		}

		private RecipientInfoCacheEntry(string displayName, string smtpAddress, string routingAddress, string alias, string routingType, AddressOrigin addressOrigin, int recipientFlags, string itemId, EmailAddressIndex emailAddressIndex, string sipUri, string mobilePhoneNumber, short usage, long dateTimeTicks, short sessionCount, short sessionFlag, int cacheEntryId)
		{
			EnumValidator.ThrowIfInvalid<AddressOrigin>(addressOrigin);
			EnumValidator.ThrowIfInvalid<EmailAddressIndex>(emailAddressIndex);
			this.displayName = displayName;
			this.smtpAddress = smtpAddress;
			this.routingAddress = routingAddress;
			this.alias = alias;
			if (string.IsNullOrEmpty(routingType))
			{
				this.routingType = "SMTP";
			}
			else
			{
				this.routingType = routingType;
			}
			this.addressOrigin = addressOrigin;
			this.recipientFlags = recipientFlags;
			this.itemId = itemId;
			this.usage = usage;
			this.dateTimeTicks = dateTimeTicks;
			this.sessionCount = sessionCount;
			this.sessionFlag = sessionFlag;
			this.emailAddressIndex = emailAddressIndex;
			this.sipUri = sipUri;
			this.mobilePhoneNumber = mobilePhoneNumber;
			this.cacheEntryId = cacheEntryId;
			if (this.cacheEntryId == 0)
			{
				this.GetNewCacheEntryId();
				this.isDirty = false;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
			set
			{
				this.isDirty = true;
				this.displayName = value;
			}
		}

		public string SmtpAddress
		{
			get
			{
				return this.smtpAddress;
			}
		}

		public string RoutingAddress
		{
			get
			{
				return this.routingAddress;
			}
		}

		public string Alias
		{
			get
			{
				return this.alias;
			}
			set
			{
				this.isDirty = true;
				this.alias = value;
			}
		}

		public string RoutingType
		{
			get
			{
				return this.routingType;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentNullException("RoutingType", "Routing Type cannot be empty or null");
				}
				this.isDirty = true;
				this.routingType = value;
			}
		}

		public string ItemId
		{
			get
			{
				return this.itemId;
			}
			set
			{
				this.isDirty = true;
				this.itemId = value;
			}
		}

		public EmailAddressIndex EmailAddressIndex
		{
			get
			{
				return this.emailAddressIndex;
			}
			set
			{
				EnumValidator.ThrowIfInvalid<EmailAddressIndex>(value);
				this.isDirty = true;
				this.emailAddressIndex = value;
			}
		}

		public AddressOrigin AddressOrigin
		{
			get
			{
				return this.addressOrigin;
			}
			set
			{
				EnumValidator.ThrowIfInvalid<AddressOrigin>(value);
				this.isDirty = true;
				this.addressOrigin = value;
			}
		}

		public int RecipientFlags
		{
			get
			{
				return this.recipientFlags;
			}
		}

		public string SipUri
		{
			get
			{
				return this.sipUri;
			}
			set
			{
				this.isDirty = true;
				this.sipUri = value;
			}
		}

		public string MobilePhoneNumber
		{
			get
			{
				return this.mobilePhoneNumber;
			}
			set
			{
				this.isDirty = true;
				this.mobilePhoneNumber = value;
			}
		}

		public short Usage
		{
			get
			{
				return this.usage;
			}
		}

		public long DateTimeTicks
		{
			get
			{
				return this.dateTimeTicks;
			}
		}

		public short SessionCount
		{
			get
			{
				return this.sessionCount;
			}
			set
			{
				this.isDirty = true;
				this.sessionCount = value;
			}
		}

		public int CacheEntryId
		{
			get
			{
				return this.cacheEntryId;
			}
		}

		internal bool IsDirty
		{
			get
			{
				return this.isDirty;
			}
		}

		public void UpdateTimeStamp()
		{
			this.isDirty = true;
			this.dateTimeTicks = ExDateTime.UtcNow.UtcTicks;
		}

		public void Decay()
		{
			this.isDirty = true;
			short num = -1;
			if (!this.IsSessionFlagSet())
			{
				this.IncrementSessionCount();
			}
			else
			{
				this.ResetSessionCount();
			}
			short num2 = this.SessionCount;
			if (num2 <= 12)
			{
				if (num2 <= 6)
				{
					if (num2 != 3 && num2 != 6)
					{
						goto IL_C0;
					}
				}
				else if (num2 != 9)
				{
					if (num2 != 12)
					{
						goto IL_C0;
					}
					num = this.ComputeDecay(0.25, 2);
					goto IL_D1;
				}
				num = 1;
				goto IL_D1;
			}
			if (num2 <= 23)
			{
				if (num2 == 17)
				{
					num = this.ComputeDecay(0.25, 3);
					goto IL_D1;
				}
				if (num2 == 23)
				{
					num = this.ComputeDecay(0.5, 4);
					goto IL_D1;
				}
			}
			else
			{
				if (num2 == 26)
				{
					num = this.ComputeDecay(0.75, 5);
					goto IL_D1;
				}
				if (num2 == 31)
				{
					num = 0;
					this.usage = 0;
					goto IL_D1;
				}
			}
			IL_C0:
			if (this.SessionCount > 31)
			{
				this.usage = 0;
			}
			IL_D1:
			if (num > 0)
			{
				this.usage -= num;
				if (this.usage < 0)
				{
					this.usage = 0;
				}
			}
		}

		public void IncrementUsage()
		{
			this.isDirty = true;
			this.usage += 1;
		}

		public void IncrementSessionCount()
		{
			this.isDirty = true;
			this.sessionCount += 1;
		}

		public void ResetSessionCount()
		{
			this.isDirty = true;
			this.sessionCount = 0;
		}

		public void SetSessionFlag()
		{
			this.isDirty = true;
			this.sessionFlag = 1;
		}

		public void ClearSessionFlag()
		{
			this.isDirty = true;
			this.sessionFlag = 0;
		}

		public bool IsSessionFlagSet()
		{
			return this.sessionFlag == 1;
		}

		public int CompareTo(RecipientInfoCacheEntry entry)
		{
			if (entry == null)
			{
				throw new ArgumentNullException("entry");
			}
			int num = this.Usage.CompareTo(entry.Usage);
			if (num == 0)
			{
				num = this.DateTimeTicks.CompareTo(entry.DateTimeTicks);
			}
			return num;
		}

		internal static RecipientInfoCacheEntry ParseEntry(XmlReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (!string.Equals(reader.Name, "entry", StringComparison.OrdinalIgnoreCase))
			{
				throw new CorruptDataException(ServerStrings.InvalidTagName("entry", reader.Name));
			}
			string text = string.Empty;
			string text2 = string.Empty;
			string text3 = string.Empty;
			string text4 = string.Empty;
			string text5 = string.Empty;
			AddressOrigin addressOrigin = AddressOrigin.Unknown;
			string text6 = string.Empty;
			short num = 0;
			long num2 = 0L;
			short num3 = 0;
			short num4 = 0;
			int num5 = 0;
			EmailAddressIndex emailAddressIndex = EmailAddressIndex.None;
			string text7 = string.Empty;
			int num6 = 0;
			string text8 = string.Empty;
			try
			{
				if (reader.HasValue)
				{
					throw new CorruptDataException(ServerStrings.ElementHasUnsupportedValue(reader.Name));
				}
				if (reader.HasAttributes)
				{
					int i = 0;
					while (i < reader.AttributeCount)
					{
						reader.MoveToAttribute(i);
						string name;
						if ((name = reader.Name) != null)
						{
							if (<PrivateImplementationDetails>{495E4232-B7DB-45E0-83EB-134F7677B64F}.$$method0x6000f59-1 == null)
							{
								<PrivateImplementationDetails>{495E4232-B7DB-45E0-83EB-134F7677B64F}.$$method0x6000f59-1 = new Dictionary<string, int>(16)
								{
									{
										"displayName",
										0
									},
									{
										"smtpAddr",
										1
									},
									{
										"routAddr",
										2
									},
									{
										"alias",
										3
									},
									{
										"routType",
										4
									},
									{
										"addrOrig",
										5
									},
									{
										"recipFlags",
										6
									},
									{
										"itemId",
										7
									},
									{
										"emailAddressIndex",
										8
									},
									{
										"sipUri",
										9
									},
									{
										"usage",
										10
									},
									{
										"dateTimeTicks",
										11
									},
									{
										"session",
										12
									},
									{
										"sessionFlag",
										13
									},
									{
										"cacheEntryId",
										14
									},
									{
										"mobilePhoneNumber",
										15
									}
								};
							}
							int num7;
							if (<PrivateImplementationDetails>{495E4232-B7DB-45E0-83EB-134F7677B64F}.$$method0x6000f59-1.TryGetValue(name, out num7))
							{
								switch (num7)
								{
								case 0:
									text = reader.Value;
									break;
								case 1:
									text2 = reader.Value;
									break;
								case 2:
									text3 = reader.Value;
									break;
								case 3:
									text4 = reader.Value;
									break;
								case 4:
									text5 = reader.Value;
									break;
								case 5:
									addressOrigin = (AddressOrigin)int.Parse(reader.Value, NumberFormatInfo.InvariantInfo);
									break;
								case 6:
									num5 = int.Parse(reader.Value, NumberFormatInfo.InvariantInfo);
									break;
								case 7:
									text6 = reader.Value;
									break;
								case 8:
									emailAddressIndex = (EmailAddressIndex)int.Parse(reader.Value, NumberFormatInfo.InvariantInfo);
									break;
								case 9:
									text7 = reader.Value;
									break;
								case 10:
									num = short.Parse(reader.Value, NumberFormatInfo.InvariantInfo);
									break;
								case 11:
									num2 = long.Parse(reader.Value, NumberFormatInfo.InvariantInfo);
									break;
								case 12:
									num3 = short.Parse(reader.Value, NumberFormatInfo.InvariantInfo);
									break;
								case 13:
									num4 = short.Parse(reader.Value, NumberFormatInfo.InvariantInfo);
									break;
								case 14:
									num6 = int.Parse(reader.Value, NumberFormatInfo.InvariantInfo);
									break;
								case 15:
									text8 = reader.Value;
									break;
								default:
									goto IL_30F;
								}
								i++;
								continue;
							}
						}
						IL_30F:
						throw new CorruptDataException(ServerStrings.ValueNotRecognizedForAttribute(reader.Name));
					}
					if (!reader.Read())
					{
						throw new CorruptDataException(ServerStrings.ClosingTagExpectedNoneFound);
					}
					if (reader.NodeType != XmlNodeType.EndElement || !string.Equals("entry", reader.Name, StringComparison.OrdinalIgnoreCase))
					{
						throw new CorruptDataException(ServerStrings.ClosingTagExpected(reader.Name));
					}
				}
			}
			catch (FormatException innerException)
			{
				throw new CorruptDataException(ServerStrings.FailedToParseValue, innerException);
			}
			catch (OverflowException innerException2)
			{
				throw new CorruptDataException(ServerStrings.FailedToParseValue, innerException2);
			}
			if (num6 <= 0)
			{
				throw new CorruptDataException(ServerStrings.InvalidCacheEntryId(num6));
			}
			return new RecipientInfoCacheEntry(text, text2, text3, text4, text5, addressOrigin, num5, text6, emailAddressIndex, text7, text8, num, num2, num3, num4, num6);
		}

		internal void Serialize(XmlWriter xmlWriter)
		{
			if (xmlWriter == null)
			{
				throw new ArgumentNullException("xmlWriter");
			}
			xmlWriter.WriteStartElement("entry");
			if (!string.IsNullOrEmpty(this.displayName))
			{
				xmlWriter.WriteAttributeString("displayName", this.displayName);
			}
			if (!string.IsNullOrEmpty(this.smtpAddress))
			{
				xmlWriter.WriteAttributeString("smtpAddr", this.smtpAddress);
			}
			if (!string.IsNullOrEmpty(this.routingAddress))
			{
				xmlWriter.WriteAttributeString("routAddr", this.routingAddress);
			}
			if (!string.IsNullOrEmpty(this.alias))
			{
				xmlWriter.WriteAttributeString("alias", this.alias);
			}
			if (!string.IsNullOrEmpty(this.routingType))
			{
				xmlWriter.WriteAttributeString("routType", this.routingType);
			}
			string localName = "addrOrig";
			int num = (int)this.addressOrigin;
			xmlWriter.WriteAttributeString(localName, num.ToString(CultureInfo.InvariantCulture));
			xmlWriter.WriteAttributeString("recipFlags", this.recipientFlags.ToString(CultureInfo.InvariantCulture));
			if (!string.IsNullOrEmpty(this.itemId))
			{
				xmlWriter.WriteAttributeString("itemId", this.itemId);
			}
			if (this.emailAddressIndex != EmailAddressIndex.None)
			{
				string localName2 = "emailAddressIndex";
				int num2 = (int)this.emailAddressIndex;
				xmlWriter.WriteAttributeString(localName2, num2.ToString(CultureInfo.InvariantCulture));
			}
			if (!string.IsNullOrEmpty(this.sipUri))
			{
				xmlWriter.WriteAttributeString("sipUri", this.sipUri);
			}
			if (this.usage >= 0)
			{
				xmlWriter.WriteAttributeString("usage", this.usage.ToString(CultureInfo.InvariantCulture));
			}
			if (this.dateTimeTicks >= 0L)
			{
				xmlWriter.WriteAttributeString("dateTimeTicks", this.dateTimeTicks.ToString(CultureInfo.InvariantCulture));
			}
			if (this.sessionCount >= 0)
			{
				xmlWriter.WriteAttributeString("session", this.sessionCount.ToString(CultureInfo.InvariantCulture));
			}
			if (this.sessionFlag >= 0)
			{
				xmlWriter.WriteAttributeString("sessionFlag", this.sessionFlag.ToString(CultureInfo.InvariantCulture));
			}
			if (this.cacheEntryId == 0)
			{
				this.GetNewCacheEntryId();
			}
			xmlWriter.WriteAttributeString("cacheEntryId", this.cacheEntryId.ToString(CultureInfo.InvariantCulture));
			if (!string.IsNullOrEmpty(this.mobilePhoneNumber))
			{
				xmlWriter.WriteAttributeString("mobilePhoneNumber", this.mobilePhoneNumber);
			}
			xmlWriter.WriteFullEndElement();
		}

		internal void GetNewCacheEntryId()
		{
			this.isDirty = true;
			lock (RecipientInfoCacheEntry.randomGenerator)
			{
				int num = this.cacheEntryId;
				do
				{
					this.cacheEntryId = RecipientInfoCacheEntry.randomGenerator.Next();
				}
				while (this.cacheEntryId == 0 || this.cacheEntryId == num);
			}
		}

		private short ComputeDecay(double percent, short setDecay)
		{
			short num = (short)Math.Round(percent * (double)this.usage);
			short result;
			if (num > setDecay)
			{
				result = num;
			}
			else
			{
				result = setDecay;
			}
			return result;
		}

		private const string AutoCompleteEntryName = "entry";

		private static readonly Random randomGenerator = new Random();

		private string displayName;

		private readonly string smtpAddress;

		private readonly string routingAddress;

		private string alias;

		private string routingType;

		private AddressOrigin addressOrigin;

		private string itemId;

		private short usage;

		private long dateTimeTicks;

		private short sessionCount;

		private short sessionFlag;

		private readonly int recipientFlags;

		private EmailAddressIndex emailAddressIndex;

		private int cacheEntryId;

		private string sipUri;

		private string mobilePhoneNumber;

		private bool isDirty;
	}
}
