using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;

namespace Microsoft.Exchange.CtsResources
{
	internal static class TextConvertersStrings
	{
		static TextConvertersStrings()
		{
			TextConvertersStrings.stringIDs.Add(1331686521U, "ConverterStreamInInconsistentStare");
			TextConvertersStrings.stringIDs.Add(62892580U, "ConverterReaderInInconsistentStare");
			TextConvertersStrings.stringIDs.Add(2496811423U, "CannotUseConverterReader");
			TextConvertersStrings.stringIDs.Add(1956525104U, "CannotReadFromSource");
			TextConvertersStrings.stringIDs.Add(1226301788U, "IndexOutOfRange");
			TextConvertersStrings.stringIDs.Add(2746482960U, "CountTooLarge");
			TextConvertersStrings.stringIDs.Add(3781059438U, "CallbackTagAlreadyDeleted");
			TextConvertersStrings.stringIDs.Add(1699401981U, "InputDocumentTooComplex");
			TextConvertersStrings.stringIDs.Add(3964025115U, "CannotWriteToDestination");
			TextConvertersStrings.stringIDs.Add(1505296452U, "ConverterWriterInInconsistentStare");
			TextConvertersStrings.stringIDs.Add(1797351840U, "TooManyIterationsToFlushConverter");
			TextConvertersStrings.stringIDs.Add(666646297U, "HtmlNestingTooDeep");
			TextConvertersStrings.stringIDs.Add(3590683541U, "OffsetOutOfRange");
			TextConvertersStrings.stringIDs.Add(1733367593U, "TooManyIterationsToProduceOutput");
			TextConvertersStrings.stringIDs.Add(1265174491U, "AttributeIdIsUnknown");
			TextConvertersStrings.stringIDs.Add(3860747840U, "PropertyNotValidForCodepageConversionMode");
			TextConvertersStrings.stringIDs.Add(1688399336U, "CallbackTagAlreadyWritten");
			TextConvertersStrings.stringIDs.Add(1083457927U, "PriorityListIncludesNonDetectableCodePage");
			TextConvertersStrings.stringIDs.Add(1046064356U, "PropertyNotValidForTextExtractionMode");
			TextConvertersStrings.stringIDs.Add(1551326176U, "CannotSetNegativelength");
			TextConvertersStrings.stringIDs.Add(1194347614U, "AccessShouldBeReadOrWrite");
			TextConvertersStrings.stringIDs.Add(2816828061U, "TagIdIsUnknown");
			TextConvertersStrings.stringIDs.Add(2864662625U, "CannotSeekBeforeBeginning");
			TextConvertersStrings.stringIDs.Add(3121351942U, "AttributeNameIsEmpty");
			TextConvertersStrings.stringIDs.Add(2465464738U, "TagNameIsEmpty");
			TextConvertersStrings.stringIDs.Add(4220967200U, "AttributeNotStarted");
			TextConvertersStrings.stringIDs.Add(787140477U, "TextReaderUnsupported");
			TextConvertersStrings.stringIDs.Add(1758514861U, "SeekUnsupported");
			TextConvertersStrings.stringIDs.Add(2757185013U, "AttributeNotInitialized");
			TextConvertersStrings.stringIDs.Add(548794797U, "BufferSizeValueRange");
			TextConvertersStrings.stringIDs.Add(1705869077U, "CannotUseConverterWriter");
			TextConvertersStrings.stringIDs.Add(1332689101U, "AttributeNotValidForThisContext");
			TextConvertersStrings.stringIDs.Add(2935817164U, "TagNotStarted");
			TextConvertersStrings.stringIDs.Add(1028484033U, "AttributeNotValidInThisState");
			TextConvertersStrings.stringIDs.Add(2521897257U, "EndTagCannotHaveAttributes");
			TextConvertersStrings.stringIDs.Add(856420434U, "InputEncodingRequired");
			TextConvertersStrings.stringIDs.Add(1578381838U, "TagTooLong");
			TextConvertersStrings.stringIDs.Add(1369500743U, "AttributeCollectionNotInitialized");
			TextConvertersStrings.stringIDs.Add(1384542861U, "WriteAfterFlush");
			TextConvertersStrings.stringIDs.Add(2566408865U, "TooManyIterationsToProcessInput");
			TextConvertersStrings.stringIDs.Add(1308081499U, "MaxCharactersCannotBeNegative");
			TextConvertersStrings.stringIDs.Add(3512722007U, "ReadUnsupported");
			TextConvertersStrings.stringIDs.Add(3007555696U, "ContextNotValidInThisState");
			TextConvertersStrings.stringIDs.Add(2995867106U, "AttributeIdInvalid");
			TextConvertersStrings.stringIDs.Add(4233942166U, "TagIdInvalid");
			TextConvertersStrings.stringIDs.Add(1590522975U, "CountOutOfRange");
			TextConvertersStrings.stringIDs.Add(3248066228U, "WriteUnsupported");
			TextConvertersStrings.stringIDs.Add(41501160U, "ParametersCannotBeChangedAfterConverterObjectIsUsed");
			TextConvertersStrings.stringIDs.Add(3015958781U, "CannotWriteWhileCopyPending");
			TextConvertersStrings.stringIDs.Add(2151871687U, "TextWriterUnsupported");
		}

		public static string ConverterStreamInInconsistentStare
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("ConverterStreamInInconsistentStare");
			}
		}

		public static string ConverterReaderInInconsistentStare
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("ConverterReaderInInconsistentStare");
			}
		}

		public static string CreateFileFailed(string filePath)
		{
			return string.Format(TextConvertersStrings.ResourceManager.GetString("CreateFileFailed"), filePath);
		}

		public static string LengthExceeded(int sum, int length)
		{
			return string.Format(TextConvertersStrings.ResourceManager.GetString("LengthExceeded"), sum, length);
		}

		public static string CannotUseConverterReader
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("CannotUseConverterReader");
			}
		}

		public static string CannotReadFromSource
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("CannotReadFromSource");
			}
		}

		public static string InvalidConfigurationBoolean(int propertyId)
		{
			return string.Format(TextConvertersStrings.ResourceManager.GetString("InvalidConfigurationBoolean"), propertyId);
		}

		public static string IndexOutOfRange
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("IndexOutOfRange");
			}
		}

		public static string CountTooLarge
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("CountTooLarge");
			}
		}

		public static string CallbackTagAlreadyDeleted
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("CallbackTagAlreadyDeleted");
			}
		}

		public static string InputDocumentTooComplex
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("InputDocumentTooComplex");
			}
		}

		public static string CannotWriteToDestination
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("CannotWriteToDestination");
			}
		}

		public static string ConverterWriterInInconsistentStare
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("ConverterWriterInInconsistentStare");
			}
		}

		public static string InvalidConfigurationInteger(int propertyId)
		{
			return string.Format(TextConvertersStrings.ResourceManager.GetString("InvalidConfigurationInteger"), propertyId);
		}

		public static string TooManyIterationsToFlushConverter
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("TooManyIterationsToFlushConverter");
			}
		}

		public static string HtmlNestingTooDeep
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("HtmlNestingTooDeep");
			}
		}

		public static string OffsetOutOfRange
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("OffsetOutOfRange");
			}
		}

		public static string TooManyIterationsToProduceOutput
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("TooManyIterationsToProduceOutput");
			}
		}

		public static string AttributeIdIsUnknown
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("AttributeIdIsUnknown");
			}
		}

		public static string PropertyNotValidForCodepageConversionMode
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("PropertyNotValidForCodepageConversionMode");
			}
		}

		public static string CallbackTagAlreadyWritten
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("CallbackTagAlreadyWritten");
			}
		}

		public static string PriorityListIncludesNonDetectableCodePage
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("PriorityListIncludesNonDetectableCodePage");
			}
		}

		public static string CannotWriteOtherTagsInsideElement(string elementName)
		{
			return string.Format(TextConvertersStrings.ResourceManager.GetString("CannotWriteOtherTagsInsideElement"), elementName);
		}

		public static string PropertyNotValidForTextExtractionMode
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("PropertyNotValidForTextExtractionMode");
			}
		}

		public static string CannotSetNegativelength
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("CannotSetNegativelength");
			}
		}

		public static string AccessShouldBeReadOrWrite
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("AccessShouldBeReadOrWrite");
			}
		}

		public static string TagIdIsUnknown
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("TagIdIsUnknown");
			}
		}

		public static string CannotSeekBeforeBeginning
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("CannotSeekBeforeBeginning");
			}
		}

		public static string AttributeNameIsEmpty
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("AttributeNameIsEmpty");
			}
		}

		public static string TagNameIsEmpty
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("TagNameIsEmpty");
			}
		}

		public static string AttributeNotStarted
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("AttributeNotStarted");
			}
		}

		public static string TextReaderUnsupported
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("TextReaderUnsupported");
			}
		}

		public static string SeekUnsupported
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("SeekUnsupported");
			}
		}

		public static string AttributeNotInitialized
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("AttributeNotInitialized");
			}
		}

		public static string BufferSizeValueRange
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("BufferSizeValueRange");
			}
		}

		public static string CannotUseConverterWriter
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("CannotUseConverterWriter");
			}
		}

		public static string AttributeNotValidForThisContext
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("AttributeNotValidForThisContext");
			}
		}

		public static string TagNotStarted
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("TagNotStarted");
			}
		}

		public static string AttributeNotValidInThisState
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("AttributeNotValidInThisState");
			}
		}

		public static string EndTagCannotHaveAttributes
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("EndTagCannotHaveAttributes");
			}
		}

		public static string InputEncodingRequired
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("InputEncodingRequired");
			}
		}

		public static string TagTooLong
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("TagTooLong");
			}
		}

		public static string AttributeCollectionNotInitialized
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("AttributeCollectionNotInitialized");
			}
		}

		public static string InvalidConfigurationStream(int propertyId)
		{
			return string.Format(TextConvertersStrings.ResourceManager.GetString("InvalidConfigurationStream"), propertyId);
		}

		public static string WriteAfterFlush
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("WriteAfterFlush");
			}
		}

		public static string TooManyIterationsToProcessInput
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("TooManyIterationsToProcessInput");
			}
		}

		public static string MaxCharactersCannotBeNegative
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("MaxCharactersCannotBeNegative");
			}
		}

		public static string ReadUnsupported
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("ReadUnsupported");
			}
		}

		public static string ContextNotValidInThisState
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("ContextNotValidInThisState");
			}
		}

		public static string AttributeIdInvalid
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("AttributeIdInvalid");
			}
		}

		public static string TagIdInvalid
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("TagIdInvalid");
			}
		}

		public static string CountOutOfRange
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("CountOutOfRange");
			}
		}

		public static string WriteUnsupported
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("WriteUnsupported");
			}
		}

		public static string ParametersCannotBeChangedAfterConverterObjectIsUsed
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("ParametersCannotBeChangedAfterConverterObjectIsUsed");
			}
		}

		public static string CannotWriteWhileCopyPending
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("CannotWriteWhileCopyPending");
			}
		}

		public static string TextWriterUnsupported
		{
			get
			{
				return TextConvertersStrings.ResourceManager.GetString("TextWriterUnsupported");
			}
		}

		public static string DocumentGrowingExcessively(int ratio)
		{
			return string.Format(TextConvertersStrings.ResourceManager.GetString("DocumentGrowingExcessively"), ratio);
		}

		public static string InvalidCodePage(int codePage)
		{
			return string.Format(TextConvertersStrings.ResourceManager.GetString("InvalidCodePage"), codePage);
		}

		public static string GetLocalizedString(TextConvertersStrings.IDs key)
		{
			return TextConvertersStrings.ResourceManager.GetString(TextConvertersStrings.stringIDs[(uint)key]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(50);

		private static ResourceManager ResourceManager = new ResourceManager("Microsoft.Exchange.CtsResources.TextConvertersStrings", typeof(TextConvertersStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			ConverterStreamInInconsistentStare = 1331686521U,
			ConverterReaderInInconsistentStare = 62892580U,
			CannotUseConverterReader = 2496811423U,
			CannotReadFromSource = 1956525104U,
			IndexOutOfRange = 1226301788U,
			CountTooLarge = 2746482960U,
			CallbackTagAlreadyDeleted = 3781059438U,
			InputDocumentTooComplex = 1699401981U,
			CannotWriteToDestination = 3964025115U,
			ConverterWriterInInconsistentStare = 1505296452U,
			TooManyIterationsToFlushConverter = 1797351840U,
			HtmlNestingTooDeep = 666646297U,
			OffsetOutOfRange = 3590683541U,
			TooManyIterationsToProduceOutput = 1733367593U,
			AttributeIdIsUnknown = 1265174491U,
			PropertyNotValidForCodepageConversionMode = 3860747840U,
			CallbackTagAlreadyWritten = 1688399336U,
			PriorityListIncludesNonDetectableCodePage = 1083457927U,
			PropertyNotValidForTextExtractionMode = 1046064356U,
			CannotSetNegativelength = 1551326176U,
			AccessShouldBeReadOrWrite = 1194347614U,
			TagIdIsUnknown = 2816828061U,
			CannotSeekBeforeBeginning = 2864662625U,
			AttributeNameIsEmpty = 3121351942U,
			TagNameIsEmpty = 2465464738U,
			AttributeNotStarted = 4220967200U,
			TextReaderUnsupported = 787140477U,
			SeekUnsupported = 1758514861U,
			AttributeNotInitialized = 2757185013U,
			BufferSizeValueRange = 548794797U,
			CannotUseConverterWriter = 1705869077U,
			AttributeNotValidForThisContext = 1332689101U,
			TagNotStarted = 2935817164U,
			AttributeNotValidInThisState = 1028484033U,
			EndTagCannotHaveAttributes = 2521897257U,
			InputEncodingRequired = 856420434U,
			TagTooLong = 1578381838U,
			AttributeCollectionNotInitialized = 1369500743U,
			WriteAfterFlush = 1384542861U,
			TooManyIterationsToProcessInput = 2566408865U,
			MaxCharactersCannotBeNegative = 1308081499U,
			ReadUnsupported = 3512722007U,
			ContextNotValidInThisState = 3007555696U,
			AttributeIdInvalid = 2995867106U,
			TagIdInvalid = 4233942166U,
			CountOutOfRange = 1590522975U,
			WriteUnsupported = 3248066228U,
			ParametersCannotBeChangedAfterConverterObjectIsUsed = 41501160U,
			CannotWriteWhileCopyPending = 3015958781U,
			TextWriterUnsupported = 2151871687U
		}

		private enum ParamIDs
		{
			CreateFileFailed,
			LengthExceeded,
			InvalidConfigurationBoolean,
			InvalidConfigurationInteger,
			CannotWriteOtherTagsInsideElement,
			InvalidConfigurationStream,
			DocumentGrowingExcessively,
			InvalidCodePage
		}
	}
}
