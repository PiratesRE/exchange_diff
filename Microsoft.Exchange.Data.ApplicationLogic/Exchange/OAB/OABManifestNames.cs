using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.OAB
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class OABManifestNames
	{
		public const string TemplateElement = "Template";

		public const string FullElement = "Full";

		public const string DiffElement = "Diff";

		public const string OABElement = "OAB";

		public const string OALElement = "OAL";

		public const string IdAttribute = "id";

		public const string DnAttribute = "dn";

		public const string NameAttribute = "name";

		public const string SequenceAttribute = "seq";

		public const string VersionAttribute = "ver";

		public const string SizeAttribute = "size";

		public const string UncompressedSizeAttribute = "uncompressedsize";

		public const string SHAAttribute = "SHA";

		public const string LangidAttribute = "langid";

		public const string TypeAttribute = "type";

		public const string WindowsTemplateType = "windows";

		public const string MacTemplateType = "mac";
	}
}
