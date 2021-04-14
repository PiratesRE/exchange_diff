using System;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Rtf
{
	internal enum RtfDestination : byte
	{
		RTF,
		FontTable,
		RealFontName,
		AltFontName,
		ColorTable,
		StyleSheet,
		ListTable,
		ListTableEntry,
		ListLevelEntry,
		ListOverrideTable,
		ListOverrideTableEntry,
		ListLevelText,
		ListText,
		DocumentArea,
		Field,
		FieldResult,
		FieldInstruction,
		ParaNumbering,
		ParaNumText,
		NestTableProps,
		FollowingPunct,
		LeadingPunct,
		Object,
		ObjectClass,
		ObjectName,
		ObjectData,
		ObjectBlob,
		Picture,
		PictureProperties,
		ShapeName,
		ShapeValue,
		ShapeInstructions,
		ShapeText,
		BookmarkName,
		HtmlTagIndex
	}
}
