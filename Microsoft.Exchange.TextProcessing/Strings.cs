using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.TextProcessing
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(1734378792U, "OffsetsUnavailable");
			Strings.stringIDs.Add(2101368493U, "NullIMatch");
			Strings.stringIDs.Add(1322833356U, "CurrentNodeNotSingleNode");
			Strings.stringIDs.Add(1246524789U, "EmptyTermSet");
			Strings.stringIDs.Add(759104278U, "UnsupportedMatch");
			Strings.stringIDs.Add(901028467U, "NullFingerprint");
			Strings.stringIDs.Add(2821948495U, "TermExceedsMaximumLength");
			Strings.stringIDs.Add(608666739U, "InvalidTerm");
			Strings.stringIDs.Add(1141081490U, "IntermediateNodeHasMultipleChildren");
			Strings.stringIDs.Add(2382911857U, "InvalidData");
		}

		public static LocalizedString InvalidFingerprintSize(int size)
		{
			return new LocalizedString("InvalidFingerprintSize", Strings.ResourceManager, new object[]
			{
				size
			});
		}

		public static LocalizedString InvalidShingle(string value)
		{
			return new LocalizedString("InvalidShingle", Strings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString MismatchedFingerprintVersions(int version1, int version2)
		{
			return new LocalizedString("MismatchedFingerprintVersions", Strings.ResourceManager, new object[]
			{
				version1,
				version2
			});
		}

		public static LocalizedString OffsetsUnavailable
		{
			get
			{
				return new LocalizedString("OffsetsUnavailable", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MismatchedFingerprintSize(int size1, int size2)
		{
			return new LocalizedString("MismatchedFingerprintSize", Strings.ResourceManager, new object[]
			{
				size1,
				size2
			});
		}

		public static LocalizedString InvalidFingerprintVersion(int version, int supportedVersion)
		{
			return new LocalizedString("InvalidFingerprintVersion", Strings.ResourceManager, new object[]
			{
				version,
				supportedVersion
			});
		}

		public static LocalizedString NullIMatch
		{
			get
			{
				return new LocalizedString("NullIMatch", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CurrentNodeNotSingleNode
		{
			get
			{
				return new LocalizedString("CurrentNodeNotSingleNode", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidShingleCountForTemplate(ulong count)
		{
			return new LocalizedString("InvalidShingleCountForTemplate", Strings.ResourceManager, new object[]
			{
				count
			});
		}

		public static LocalizedString EmptyTermSet
		{
			get
			{
				return new LocalizedString("EmptyTermSet", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnsupportedMatch
		{
			get
			{
				return new LocalizedString("UnsupportedMatch", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidBoundaryType(string boundaryType)
		{
			return new LocalizedString("InvalidBoundaryType", Strings.ResourceManager, new object[]
			{
				boundaryType
			});
		}

		public static LocalizedString NullFingerprint
		{
			get
			{
				return new LocalizedString("NullFingerprint", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TermExceedsMaximumLength
		{
			get
			{
				return new LocalizedString("TermExceedsMaximumLength", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidTerm
		{
			get
			{
				return new LocalizedString("InvalidTerm", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IntermediateNodeHasMultipleChildren
		{
			get
			{
				return new LocalizedString("IntermediateNodeHasMultipleChildren", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidCoefficient(double value)
		{
			return new LocalizedString("InvalidCoefficient", Strings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString InvalidData
		{
			get
			{
				return new LocalizedString("InvalidData", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(10);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.TextProcessing.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			OffsetsUnavailable = 1734378792U,
			NullIMatch = 2101368493U,
			CurrentNodeNotSingleNode = 1322833356U,
			EmptyTermSet = 1246524789U,
			UnsupportedMatch = 759104278U,
			NullFingerprint = 901028467U,
			TermExceedsMaximumLength = 2821948495U,
			InvalidTerm = 608666739U,
			IntermediateNodeHasMultipleChildren = 1141081490U,
			InvalidData = 2382911857U
		}

		private enum ParamIDs
		{
			InvalidFingerprintSize,
			InvalidShingle,
			MismatchedFingerprintVersions,
			MismatchedFingerprintSize,
			InvalidFingerprintVersion,
			InvalidShingleCountForTemplate,
			InvalidBoundaryType,
			InvalidCoefficient
		}
	}
}
