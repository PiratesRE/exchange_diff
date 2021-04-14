using System;
using System.IO;
using System.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.ApplicationLogic.Diagnostics
{
	internal class ConditionalRegistration : BaseConditionalRegistration
	{
		public static ConditionalRegistration CreateFromArgument(DiagnosableParameters argument)
		{
			if (BaseConditionalRegistration.FetchSchema == null || BaseConditionalRegistration.QuerySchema == null || string.IsNullOrEmpty(ConditionalRegistrationLog.ProtocolName))
			{
				throw new InvalidOperationException("Can not use Conditional Diagnostics Handlers without proper initialization. Call 'BaseConditionalRegistration.Initialize' to initialize pre-requisites.");
			}
			string propertiesToFetch;
			string whereClause;
			BaseConditionalRegistration.ParseArgument(argument.Argument, out propertiesToFetch, out whereClause);
			string arg;
			uint num;
			TimeSpan timeSpan;
			ConditionalRegistration.ParseOptions(argument.Argument, out arg, out num, out timeSpan);
			return new ConditionalRegistration(string.Format("[{0}] {1}", argument.UserIdentity, arg), argument.UserIdentity.Replace("/", "-"), Guid.NewGuid().ToString(), propertiesToFetch, whereClause, (int)((num == 0U) ? 10U : num), (timeSpan <= TimeSpan.Zero) ? ConditionalRegistration.DefaultTimeToLive : timeSpan);
		}

		public static ConditionalRegistration DeserializeFromStreamReader(StreamReader registrationStream)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(registrationStream.ReadToEnd());
			TimeSpan value = TimeSpan.Parse(xmlDocument.SelectSingleNode("/ConditionalRegistration/TimeToLive").InnerText);
			ExDateTime t = ExDateTime.Parse(xmlDocument.SelectSingleNode("/ConditionalRegistration/CreatedTime").InnerText).Add(value);
			if (ExDateTime.UtcNow < t)
			{
				return new ConditionalRegistration(xmlDocument.SelectSingleNode("/ConditionalRegistration/Description").InnerText, xmlDocument.SelectSingleNode("/ConditionalRegistration/User").InnerText, xmlDocument.SelectSingleNode("/ConditionalRegistration/Cookie").InnerText, xmlDocument.SelectSingleNode("/ConditionalRegistration/PropertiesToFetch").InnerText, xmlDocument.SelectSingleNode("/ConditionalRegistration/Filter").InnerText, int.Parse(xmlDocument.SelectSingleNode("/ConditionalRegistration/MaxHits").InnerText), t.Subtract(ExDateTime.UtcNow));
			}
			return null;
		}

		private static void ParseOptions(string argument, out string description, out uint maxHits, out TimeSpan timeToLive)
		{
			string text = argument.Trim();
			string text2 = text.ToLower();
			maxHits = 0U;
			timeToLive = TimeSpan.Zero;
			description = string.Empty;
			int num = text2.IndexOf(" options ");
			if (num != -1)
			{
				string text3 = text.Substring(num + " options ".Length);
				string[] array = text3.Split(new char[]
				{
					','
				});
				foreach (string text4 in array)
				{
					string text5 = text4.Trim().ToLower();
					if (text5.StartsWith("maxhits", StringComparison.OrdinalIgnoreCase))
					{
						string rightHand = BaseConditionalRegistration.GetRightHand(text5);
						if (!uint.TryParse(rightHand, out maxHits))
						{
							throw new ArgumentException("Invalid value for MaxHits: " + rightHand);
						}
					}
					else if (text5.StartsWith("timetolive", StringComparison.OrdinalIgnoreCase))
					{
						string rightHand2 = BaseConditionalRegistration.GetRightHand(text5);
						if (!TimeSpan.TryParse(rightHand2, out timeToLive))
						{
							throw new ArgumentException("Invalid value for TimeToLive: " + rightHand2);
						}
						if (timeToLive <= TimeSpan.Zero)
						{
							throw new ArgumentOutOfRangeException("TimeToLive must be > TimeSpan.Zero.  Actual: " + timeToLive);
						}
					}
					else
					{
						if (!text5.StartsWith("description", StringComparison.OrdinalIgnoreCase))
						{
							throw new ArgumentException(string.Format("Unknown option: '{0}'", text5));
						}
						description = BaseConditionalRegistration.GetRightHand(text5);
					}
				}
			}
		}

		public ConditionalRegistration(string description, string user, string cookie, string propertiesToFetch, string whereClause, TimeSpan timeToLive) : this(description, user, cookie, propertiesToFetch, whereClause, 10, timeToLive)
		{
		}

		public ConditionalRegistration(string description, string user, string cookie, string propertiesToFetch, string whereClause, int maxHits, TimeSpan timeToLive) : base(cookie, user, propertiesToFetch, whereClause)
		{
			this.Description = description;
			this.MaxHits = maxHits;
			this.TimeToLive = timeToLive;
		}

		public int MaxHits { get; private set; }

		public override string Description
		{
			get
			{
				return this.description;
			}
			protected set
			{
				this.description = value;
			}
		}

		public TimeSpan TimeToLive { get; private set; }

		private const string MaxHitsKeyword = "maxhits";

		private const string TimeToLiveKeyword = "timetolive";

		private const string DescriptionKeyword = "description";

		private const int MaxPropertiesToFetch = 100;

		internal const int DefaultMaxHits = 10;

		private static readonly TimeSpan DefaultTimeToLive = TimeSpan.FromHours(1.0);

		private string description;
	}
}
