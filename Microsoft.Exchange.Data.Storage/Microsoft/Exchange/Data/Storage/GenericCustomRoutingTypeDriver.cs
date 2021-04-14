using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class GenericCustomRoutingTypeDriver : RoutingTypeDriver
	{
		internal override IEqualityComparer<IParticipant> AddressEqualityComparer
		{
			get
			{
				return GenericCustomRoutingTypeDriver.AddressEqualityComparerImpl.Default;
			}
		}

		internal static List<PropValue> TryParseOutlookFormat(string inputString)
		{
			if (inputString.Length == 0 || inputString.Length > 9879)
			{
				return null;
			}
			Match match = GenericCustomRoutingTypeDriver.outlookParserRegex.Value.Match(inputString);
			if (!match.Success || match.Groups["addr"].Captures.Count != 1)
			{
				return null;
			}
			string value = match.Groups["addr"].Value;
			string text = match.Groups["rt"].Value;
			if (string.IsNullOrEmpty(text))
			{
				if (!SmtpAddress.IsValidSmtpAddress(value))
				{
					return null;
				}
				text = "SMTP";
			}
			return Participant.ListCoreProperties(match.Groups["dn"].Value.TrimEnd(new char[0]), value, text);
		}

		internal override bool IsRoutingTypeSupported(string routingType)
		{
			return routingType != null;
		}

		internal override bool TryDetectRoutingType(PropertyBag participantPropertyBag, out string routingType)
		{
			routingType = null;
			return false;
		}

		internal override ParticipantValidationStatus Validate(Participant participant)
		{
			if (participant.EmailAddress == null)
			{
				return ParticipantValidationStatus.AddressRequiredForRoutingType;
			}
			return ParticipantValidationStatus.NoError;
		}

		private static Regex LoadOutlookParserRegex()
		{
			Regex result;
			using (Stream manifestResourceStream = typeof(GenericCustomRoutingTypeDriver).GetTypeInfo().Assembly.GetManifestResourceStream("OutlookParser.re.txt"))
			{
				using (StreamReader streamReader = new StreamReader(manifestResourceStream, true))
				{
					result = new Regex(streamReader.ReadToEnd(), RegexOptions.Compiled);
				}
			}
			return result;
		}

		private const int MaxVariousDelimitersLength = 10;

		private static LazilyInitialized<Regex> outlookParserRegex = new LazilyInitialized<Regex>(new Func<Regex>(GenericCustomRoutingTypeDriver.LoadOutlookParserRegex));

		private sealed class AddressEqualityComparerImpl : IEqualityComparer<IParticipant>, IEqualityComparer<string>
		{
			public bool Equals(IParticipant x, IParticipant y)
			{
				if (x.EmailAddress == null)
				{
					return x.Equals(y);
				}
				return StringComparer.Ordinal.Equals(x.RoutingType, y.RoutingType) && this.Equals(x.EmailAddress, y.EmailAddress);
			}

			public int GetHashCode(IParticipant x)
			{
				if (x.EmailAddress == null)
				{
					return x.GetHashCode();
				}
				return StringComparer.Ordinal.GetHashCode(x.RoutingType) ^ this.GetHashCode(x.EmailAddress);
			}

			public bool Equals(string x, string y)
			{
				return StringComparer.Ordinal.Equals(x, y);
			}

			public int GetHashCode(string x)
			{
				return StringComparer.Ordinal.GetHashCode(x);
			}

			public static GenericCustomRoutingTypeDriver.AddressEqualityComparerImpl Default = new GenericCustomRoutingTypeDriver.AddressEqualityComparerImpl();
		}
	}
}
