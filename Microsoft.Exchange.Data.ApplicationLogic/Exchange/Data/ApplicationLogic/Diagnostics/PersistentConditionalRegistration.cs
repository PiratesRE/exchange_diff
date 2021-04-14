using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.ApplicationLogic.Diagnostics
{
	internal class PersistentConditionalRegistration : BaseConditionalRegistration
	{
		public static PersistentConditionalRegistration CreateFromXml(XElement element)
		{
			if (BaseConditionalRegistration.FetchSchema == null || BaseConditionalRegistration.QuerySchema == null || string.IsNullOrEmpty(ConditionalRegistrationLog.ProtocolName))
			{
				throw new InvalidOperationException("Can not use Conditional Diagnostics Handlers without proper initialization. Call 'BaseConditionalRegistration.Initialize' to initialize pre-requisites.");
			}
			XAttribute xattribute = element.Attribute(XName.Get("Name"));
			XAttribute xattribute2 = element.Attribute(XName.Get("Registration"));
			if (xattribute == null || xattribute2 == null)
			{
				throw new ArgumentException("[PersistentConditionalRegistration.CreateFromXml] app.config persistent registrations must have both 'Name' and 'Registration' attributes.");
			}
			string propertiesToFetch;
			string whereClause;
			BaseConditionalRegistration.ParseArgument(xattribute2.Value, out propertiesToFetch, out whereClause);
			return new PersistentConditionalRegistration(xattribute.Value, propertiesToFetch, whereClause);
		}

		public override bool ShouldEvaluate
		{
			get
			{
				return TimeProvider.UtcNow - base.LastHitUtc >= PersistentConditionalRegistration.MinimumTimeBetweenConditionalHitsEntry.Value;
			}
		}

		public override string Description
		{
			get
			{
				return base.Cookie;
			}
			protected set
			{
				throw new NotSupportedException("Can't set the description on a persistent registration since it is the same as the cookie.");
			}
		}

		public PersistentConditionalRegistration(string cookie, string propertiesToFetch, string whereClause) : base(cookie, "PersistentRegistration", propertiesToFetch, whereClause)
		{
		}

		private const string PersistentRegistrationUser = "PersistentRegistration";

		internal static TimeSpanAppSettingsEntry MinimumTimeBetweenConditionalHitsEntry = new TimeSpanAppSettingsEntry("MinimumTimeBetweenConditionalHits", TimeSpanUnit.Minutes, TimeSpan.FromMinutes(1.0), null);
	}
}
