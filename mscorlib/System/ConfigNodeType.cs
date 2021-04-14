using System;

namespace System
{
	[Serializable]
	internal enum ConfigNodeType
	{
		Element = 1,
		Attribute,
		Pi,
		XmlDecl,
		DocType,
		DTDAttribute,
		EntityDecl,
		ElementDecl,
		AttlistDecl,
		Notation,
		Group,
		IncludeSect,
		PCData,
		CData,
		IgnoreSect,
		Comment,
		EntityRef,
		Whitespace,
		Name,
		NMToken,
		String,
		Peref,
		Model,
		ATTDef,
		ATTType,
		ATTPresence,
		DTDSubset,
		LastNodeType
	}
}
