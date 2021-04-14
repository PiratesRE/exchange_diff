using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.Exchange.Common.Net.Cryptography
{
	public class WIMSAuthHeaderCryptoHelper : CryptoHelper
	{
		public WIMSAuthHeaderCryptoHelper(CryptoHelper helper, string partnerID)
		{
			this.helper = helper;
			this.partnerID = partnerID;
			this.partnerIDHint = helper.GenerateHint(partnerID);
			this.customProperties = new Dictionary<string, string>();
		}

		public string HeaderName
		{
			get
			{
				return "X-Message-Routing";
			}
		}

		public string PartnerID
		{
			get
			{
				return this.partnerID;
			}
		}

		public string Sender
		{
			get
			{
				return this.sender;
			}
			set
			{
				this.sender = value;
			}
		}

		public DateTime ExpiredAt
		{
			get
			{
				return this.expiredAt;
			}
			set
			{
				this.expiredAt = value;
			}
		}

		public int AuthHeaderType
		{
			get
			{
				return this.authHeaderType;
			}
			set
			{
				this.authHeaderType = value;
			}
		}

		public IDictionary<string, string> CustomProperties
		{
			get
			{
				return this.customProperties;
			}
		}

		public string DecryptedHeader
		{
			get
			{
				return this.ConvertPropertiesToString();
			}
		}

		public override string Encrypt(string inputText)
		{
			return this.helper.Encrypt(inputText);
		}

		public override string Decrypt(string inputText)
		{
			return this.helper.Decrypt(inputText);
		}

		public string EncryptWithHintFromProperties()
		{
			string inputText = this.ConvertPropertiesToString();
			return this.helper.EncryptWithHint(inputText, this.partnerIDHint, WIMSAuthHeaderCryptoHelper.partnerIDOffset);
		}

		public string DecryptWithHintToProperties(string inputText)
		{
			string text = this.helper.DecryptWithHint(inputText, this.partnerIDHint, WIMSAuthHeaderCryptoHelper.partnerIDOffset);
			if (text != null)
			{
				this.LoadPropertiesFromString(text);
			}
			return text;
		}

		private string ConvertPropertiesToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(string.Format("{0}={1};", "sender", this.sender ?? string.Empty));
			stringBuilder.Append(string.Format("{0}={1};", "expiredAt", this.expiredAt.ToFileTimeUtc()));
			stringBuilder.Append(string.Format("{0}={1}", "type", this.authHeaderType));
			foreach (KeyValuePair<string, string> keyValuePair in this.customProperties)
			{
				stringBuilder.Append(string.Format(";{0}={1}", keyValuePair.Key ?? string.Empty, keyValuePair.Value ?? string.Empty));
			}
			return stringBuilder.ToString();
		}

		private void LoadPropertiesFromString(string inputText)
		{
			if (inputText == null)
			{
				throw new ArgumentNullException("inputText");
			}
			this.sender = null;
			this.authHeaderType = 0;
			this.customProperties.Clear();
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			string[] array = inputText.Split(new char[]
			{
				';'
			});
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[]
				{
					'='
				});
				if (array2.Length >= 1)
				{
					if (array2[0].Equals("sender"))
					{
						this.sender = ((array2.Length < 2 && array2[1].Length == 0) ? null : array2[1]);
						flag = true;
					}
					else if (array2[0].Equals("expiredAt"))
					{
						long fileTime;
						if (array2.Length >= 2 && long.TryParse(array2[1], NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out fileTime))
						{
							this.expiredAt = DateTime.FromFileTimeUtc(fileTime);
						}
						flag3 = true;
					}
					else if (array2[0].Equals("type"))
					{
						int num;
						if (array2.Length >= 2 && int.TryParse(array2[1], NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out num))
						{
							this.authHeaderType = num;
						}
						flag2 = true;
					}
					else
					{
						this.customProperties[array2[0]] = ((array2.Length < 2 && array2[1].Length == 0) ? null : array2[1]);
					}
				}
			}
			if (!flag)
			{
				throw new InvalidOperationException(string.Format("Missing required property: {0}", "sender"));
			}
			if (!flag3)
			{
				throw new InvalidOperationException(string.Format("Missing required property: {0}", "expiredAt"));
			}
			if (!flag2)
			{
				throw new InvalidOperationException(string.Format("Missing required property: {0}", "type"));
			}
		}

		public const string SenderName = "sender";

		public const string ExpiredAtName = "expiredAt";

		public const string HeaderTypeName = "type";

		private static int partnerIDOffset = 16;

		private CryptoHelper helper;

		private string partnerID;

		private string partnerIDHint;

		private string sender;

		private DateTime expiredAt;

		private int authHeaderType;

		private IDictionary<string, string> customProperties;

		internal class HeaderType
		{
			public const int Unknown = 0;

			public const int IntraDomainDelivery = 1;

			public const int OIMTrxText = 2;

			public const int OIMTrxSMS = 3;

			public const int OIMTrxBubble = 4;

			public const int OIMNTrxText = 5;

			public const int OIMNTrxSMS = 6;

			public const int OIMNTrxBubble = 7;

			public const int NotUsed = 8;

			public const int NoBar = 9;

			public const int TrustedSender = 10;

			public const int FilterBypass = 11;

			public const int ExclusiveBypass = 12;

			public const int Unsubscribe = 13;
		}
	}
}
