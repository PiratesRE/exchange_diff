using System;

namespace System
{
	[Serializable]
	internal enum ConfigNodeSubType
	{
		Version = 28,
		Encoding,
		Standalone,
		NS,
		XMLSpace,
		XMLLang,
		System,
		Public,
		NData,
		AtCData,
		AtId,
		AtIdref,
		AtIdrefs,
		AtEntity,
		AtEntities,
		AtNmToken,
		AtNmTokens,
		AtNotation,
		AtRequired,
		AtImplied,
		AtFixed,
		PentityDecl,
		Empty,
		Any,
		Mixed,
		Sequence,
		Choice,
		Star,
		Plus,
		Questionmark,
		LastSubNodeType
	}
}
