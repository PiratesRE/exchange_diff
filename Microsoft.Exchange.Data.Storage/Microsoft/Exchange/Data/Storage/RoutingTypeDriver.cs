using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class RoutingTypeDriver
	{
		public static RoutingTypeDriver PickRoutingTypeDriver(string routingType)
		{
			foreach (RoutingTypeDriver routingTypeDriver in RoutingTypeDriver.routingTypeDriverChain)
			{
				if (routingTypeDriver.IsRoutingTypeSupported(routingType))
				{
					return routingTypeDriver;
				}
			}
			throw new Exception("Failed to pick a RoutingTypeDriver for " + routingType);
		}

		public static bool TryDetectRoutingType(PropertyBag propertyBag, out RoutingTypeDriver detectedRtDriver, out string routingType)
		{
			detectedRtDriver = null;
			routingType = null;
			foreach (RoutingTypeDriver routingTypeDriver in RoutingTypeDriver.routingTypeDriverChain)
			{
				if (routingTypeDriver.TryDetectRoutingType(propertyBag, out routingType))
				{
					detectedRtDriver = routingTypeDriver;
					return true;
				}
			}
			return false;
		}

		internal abstract IEqualityComparer<IParticipant> AddressEqualityComparer { get; }

		internal virtual bool? IsRoutable(string routingType, StoreSession session)
		{
			if (session != null)
			{
				return new bool?(session.SupportedRoutingTypes.Contains(routingType));
			}
			return null;
		}

		internal abstract bool IsRoutingTypeSupported(string routingType);

		internal virtual bool TryDetectRoutingType(PropertyBag participantPropertyBag, out string routingType)
		{
			routingType = null;
			return false;
		}

		internal abstract ParticipantValidationStatus Validate(Participant participant);

		internal virtual void Normalize(PropertyBag participantPropertyBag)
		{
			if (PropertyError.IsPropertyNotFound(participantPropertyBag.TryGetProperty(ParticipantSchema.DisplayName)))
			{
				participantPropertyBag.SetOrDeleteProperty(ParticipantSchema.DisplayName, participantPropertyBag.TryGetProperty(ParticipantSchema.EmailAddress));
			}
			string text = participantPropertyBag.TryGetProperty(ParticipantSchema.DisplayName) as string;
			if (!string.IsNullOrEmpty(text) && text.IndexOfAny(RoutingTypeDriver.BidiMarks) >= 0)
			{
				StringBuilder stringBuilder = new StringBuilder(text.Length);
				bool flag = false;
				int i = 0;
				string text2 = text;
				int j = 0;
				while (j < text2.Length)
				{
					char c = text2[j];
					if (!flag)
					{
						goto IL_94;
					}
					flag = false;
					if (!char.IsHighSurrogate(c))
					{
						goto IL_94;
					}
					stringBuilder.Append(c);
					IL_102:
					j++;
					continue;
					IL_94:
					if (char.IsLowSurrogate(c))
					{
						flag = true;
						stringBuilder.Append(c);
						goto IL_102;
					}
					if (c == '‪' || c == '‫' || c == '‭' || c == '‮')
					{
						i++;
						stringBuilder.Append(c);
						goto IL_102;
					}
					if (c != '‬')
					{
						stringBuilder.Append(c);
						goto IL_102;
					}
					if (i > 0)
					{
						i--;
						stringBuilder.Append(c);
						goto IL_102;
					}
					goto IL_102;
				}
				while (i > 0)
				{
					stringBuilder.Append('‬');
					i--;
				}
				participantPropertyBag.SetProperty(ParticipantSchema.DisplayName, stringBuilder.ToString());
			}
		}

		internal virtual string FormatAddress(Participant participant, AddressFormat addressFormat)
		{
			if (addressFormat == AddressFormat.OutlookFormat)
			{
				return string.Format("\"{0}\" [{2}:{1}]", participant.DisplayName, participant.EmailAddress, participant.RoutingType);
			}
			return null;
		}

		private const string OutlookFormat = "\"{0}\" [{2}:{1}]";

		private const char LRE = '‪';

		private const char RLE = '‫';

		private const char PDF = '‬';

		private const char LRO = '‭';

		private const char RLO = '‮';

		private static readonly char[] BidiMarks = new char[]
		{
			'‪',
			'‫',
			'‬',
			'‭',
			'‮'
		};

		private static readonly RoutingTypeDriver[] routingTypeDriverChain = new RoutingTypeDriver[]
		{
			new ExRoutingTypeDriver(),
			new SmtpRoutingTypeDriver(),
			new DLRoutingTypeDriver(),
			new MobileRoutingTypeDriver(),
			new GenericCustomRoutingTypeDriver(),
			new UnspecifiedRoutingTypeDriver()
		};

		protected sealed class OrdinalCaseInsensitiveAddressEqualityComparerImpl : IEqualityComparer<IParticipant>, IEqualityComparer<string>
		{
			public bool Equals(IParticipant x, IParticipant y)
			{
				if (x.EmailAddress == null)
				{
					return x.Equals(y);
				}
				return this.Equals(x.EmailAddress, y.EmailAddress);
			}

			public int GetHashCode(IParticipant x)
			{
				if (x.EmailAddress == null)
				{
					return x.GetHashCode();
				}
				return this.GetHashCode(x.EmailAddress);
			}

			public bool Equals(string x, string y)
			{
				return StringComparer.OrdinalIgnoreCase.Equals(x, y);
			}

			public int GetHashCode(string x)
			{
				return StringComparer.OrdinalIgnoreCase.GetHashCode(x);
			}

			public static RoutingTypeDriver.OrdinalCaseInsensitiveAddressEqualityComparerImpl Default = new RoutingTypeDriver.OrdinalCaseInsensitiveAddressEqualityComparerImpl();
		}
	}
}
