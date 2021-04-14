using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.WSTrust
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class Offer
	{
		public static Offer Find(string claim)
		{
			foreach (Offer offer in Offer.offers)
			{
				if (StringComparer.OrdinalIgnoreCase.Equals(claim, offer.Name))
				{
					return offer;
				}
			}
			return null;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public TimeSpan Duration
		{
			get
			{
				return this.duration;
			}
		}

		public static bool Equals(Offer a, Offer b)
		{
			return StringComparer.OrdinalIgnoreCase.Equals(a.Name, b.Name);
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"Name=",
				this.name,
				",Duration=",
				this.duration.TotalSeconds,
				"(secs)"
			});
		}

		private Offer(string name, TimeSpan duration)
		{
			this.name = name;
			this.duration = duration;
		}

		public static readonly Offer SharingInviteMessage = new Offer("MSExchange.SharingInviteMessage", TimeSpan.FromDays(15.0));

		public static readonly Offer SharingCalendarFreeBusy = new Offer("MSExchange.SharingCalendarFreeBusy", TimeSpan.FromMinutes(5.0));

		public static readonly Offer SharingRead = new Offer("MSExchange.SharingRead", TimeSpan.FromMinutes(60.0));

		public static readonly Offer MailboxMove = new Offer("MSExchange.MailboxMove", TimeSpan.FromHours(1.0));

		public static readonly Offer Autodiscover = new Offer("MSExchange.Autodiscover", TimeSpan.FromHours(8.0));

		public static readonly Offer XropLogon = new Offer("MSExchange.XropLogon", TimeSpan.FromHours(8.0));

		public static readonly Offer Messenger = new Offer("Messenger.SignIn", TimeSpan.FromHours(8.0));

		public static readonly Offer IPCCertificationSTS = new Offer("MSRMS.CertificationWS", TimeSpan.FromHours(48.0));

		public static readonly Offer IPCServerLicensingSTS = new Offer("MSRMS.ServerLicensingWS", TimeSpan.FromHours(48.0));

		public static readonly Offer MailTips = new Offer("MSExchange.MailTips", TimeSpan.FromMinutes(5.0));

		public static readonly Offer MailboxSearch = new Offer("MSExchange.MailboxSearch", TimeSpan.FromHours(8.0));

		public static readonly Offer UserPhotoRetrieval = new Offer("MSExchange.UserPhotoRetrieval", TimeSpan.FromMinutes(10.0));

		private string name;

		private TimeSpan duration;

		internal static readonly Offer[] offers = new Offer[]
		{
			Offer.SharingInviteMessage,
			Offer.SharingCalendarFreeBusy,
			Offer.SharingRead,
			Offer.MailboxMove,
			Offer.Autodiscover,
			Offer.MailTips,
			Offer.XropLogon,
			Offer.MailboxSearch,
			Offer.UserPhotoRetrieval
		};
	}
}
