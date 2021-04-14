using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Search
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class Tokenizer
	{
		static Tokenizer()
		{
			Tokenizer.languageWordBreakerGuidMapping = new Dictionary<string, Guid>();
			Tokenizer.languageWordBreakerGuidMapping["de"] = Tokenizer.GermanWordBreakerGuid;
			Tokenizer.languageWordBreakerGuidMapping["de-DE"] = Tokenizer.GermanWordBreakerGuid;
			Tokenizer.languageWordBreakerGuidMapping["en"] = Tokenizer.EnglishUSWordBreakerGuid;
			Tokenizer.languageWordBreakerGuidMapping["en-US"] = Tokenizer.EnglishUSWordBreakerGuid;
			Tokenizer.languageWordBreakerGuidMapping["en-GB"] = Tokenizer.EnglishBreatBritainWordBreakerGuid;
			Tokenizer.languageWordBreakerGuidMapping["es"] = Tokenizer.SpanishWordBreakerGuid;
			Tokenizer.languageWordBreakerGuidMapping["es-ES"] = Tokenizer.SpanishWordBreakerGuid;
			Tokenizer.languageWordBreakerGuidMapping["fr"] = Tokenizer.FrenchWordBreakerGuid;
			Tokenizer.languageWordBreakerGuidMapping["fr-FR"] = Tokenizer.FrenchWordBreakerGuid;
			Tokenizer.languageWordBreakerGuidMapping["it"] = Tokenizer.ItalianWordBreakerGuid;
			Tokenizer.languageWordBreakerGuidMapping["it-IT"] = Tokenizer.ItalianWordBreakerGuid;
			Tokenizer.languageWordBreakerGuidMapping["nl"] = Tokenizer.DutchWordBreakerGuid;
			Tokenizer.languageWordBreakerGuidMapping["nl-NL"] = Tokenizer.DutchWordBreakerGuid;
			Tokenizer.languageWordBreakerGuidMapping["sv"] = Tokenizer.SweidishWordBreakerGuid;
			Tokenizer.languageWordBreakerGuidMapping["sv-SE"] = Tokenizer.SweidishWordBreakerGuid;
		}

		public static void ReleaseWordBreakers()
		{
			try
			{
				Tokenizer.cacheLock.AcquireWriterLock(60000);
				foreach (Guid key in Tokenizer.guidWordBreakerMapping.Keys)
				{
					IWordBreaker o = Tokenizer.guidWordBreakerMapping[key];
					Marshal.ReleaseComObject(o);
				}
				Tokenizer.guidWordBreakerMapping.Clear();
			}
			catch (ApplicationException)
			{
				ExTraceGlobals.CcGenericTracer.TraceError(0L, "Unable to acquire lock to release word breakers");
			}
			finally
			{
				Tokenizer.cacheLock.ReleaseLock();
			}
		}

		public List<Token> Tokenize(CultureInfo cultureInfo, string text)
		{
			List<Token> list = null;
			IWordBreaker wordBreaker = this.LoadWordBreaker(cultureInfo);
			if (wordBreaker != null)
			{
				IWordSink wordSink = new WordSink();
				TEXT_SOURCE text_SOURCE = default(TEXT_SOURCE);
				text_SOURCE.FillTextBuffer = new FillTextBuffer(this.FillBuffer);
				text_SOURCE.Buffer = text;
				text_SOURCE.Current = 0;
				text_SOURCE.End = text_SOURCE.Buffer.Length;
				if (wordBreaker.BreakText(ref text_SOURCE, wordSink, null) == 0)
				{
					list = ((WordSink)wordSink).Tokens;
				}
			}
			if (list == null)
			{
				list = new List<Token>();
				list.Add(new Token(0, text.Length));
			}
			return list;
		}

		private uint FillBuffer(ref TEXT_SOURCE textSource)
		{
			return 2147751808U;
		}

		private IWordBreaker LoadWordBreaker(CultureInfo cultureInfo)
		{
			object obj = null;
			Guid guid;
			if (Tokenizer.languageWordBreakerGuidMapping.ContainsKey(cultureInfo.Name))
			{
				guid = Tokenizer.languageWordBreakerGuidMapping[cultureInfo.Name];
			}
			else
			{
				guid = Tokenizer.NeutralWordBreakerGuid;
			}
			try
			{
				Tokenizer.cacheLock.AcquireReaderLock(1000);
				if (Tokenizer.guidWordBreakerMapping.ContainsKey(guid))
				{
					return Tokenizer.guidWordBreakerMapping[guid];
				}
				Tokenizer.cacheLock.UpgradeToWriterLock(1000);
				if (Tokenizer.guidWordBreakerMapping.ContainsKey(guid))
				{
					return Tokenizer.guidWordBreakerMapping[guid];
				}
				int num = NativeMethods.CoCreateInstance(guid, null, 1U, Tokenizer.WordBreakerInterfaceGuid, out obj);
				if (num != 0)
				{
					if (guid != Tokenizer.NeutralWordBreakerGuid)
					{
						num = NativeMethods.CoCreateInstance(Tokenizer.NeutralWordBreakerGuid, null, 1U, Tokenizer.WordBreakerInterfaceGuid, out obj);
						if (num != 0)
						{
							ExTraceGlobals.CcGenericTracer.TraceError<string>((long)this.GetHashCode(), "Unable to load word breaker for: {0}", cultureInfo.Name);
							obj = null;
						}
					}
					else
					{
						ExTraceGlobals.CcGenericTracer.TraceError<string>((long)this.GetHashCode(), "Unable to load word breaker for: {0}", cultureInfo.Name);
						obj = null;
					}
				}
				bool flag = true;
				((IWordBreaker)obj).Init(true, 1024, out flag);
				Tokenizer.guidWordBreakerMapping[guid] = (IWordBreaker)obj;
			}
			catch (ApplicationException)
			{
				ExTraceGlobals.CcGenericTracer.TraceError((long)this.GetHashCode(), "Unable to acquire lock to access word breaker cache");
				return null;
			}
			finally
			{
				Tokenizer.cacheLock.ReleaseLock();
			}
			return obj as IWordBreaker;
		}

		private const int BufferSize = 1024;

		private static readonly Guid WordBreakerInterfaceGuid = new Guid("D53552C8-77E3-101A-B552-08002B33B0E6");

		private static readonly Guid GermanWordBreakerGuid = new Guid(2601050640U, 58651, 4557, 188, 127, 0, 170, 0, 61, 177, 142);

		private static readonly Guid EnglishBreatBritainWordBreakerGuid = new Guid(2158225840U, 41542, 4563, 187, 140, 0, 144, 39, 47, 163, 98);

		private static readonly Guid EnglishUSWordBreakerGuid = new Guid(2158225840U, 41542, 4563, 187, 140, 0, 144, 39, 47, 163, 98);

		private static readonly Guid SpanishWordBreakerGuid = new Guid(42317248, 4807, 4558, 189, 49, 0, 170, 0, 75, 187, 31);

		private static readonly Guid FrenchWordBreakerGuid = new Guid(1507891272U, 32921, 4123, 141, 243, 0, 0, 11, 101, 195, 181);

		private static readonly Guid ItalianWordBreakerGuid = new Guid(4253464016U, 4806, 4558, 189, 49, 0, 170, 0, 75, 187, 31);

		private static readonly Guid DutchWordBreakerGuid = new Guid(1723035920U, 35826, 4558, 190, 89, 0, 170, 0, 81, 254, 32);

		private static readonly Guid SweidishWordBreakerGuid = new Guid(29799248, 4807, 4558, 189, 49, 0, 170, 0, 75, 187, 31);

		private static readonly Guid NeutralWordBreakerGuid = new Guid(915818464, 6064, 4558, 153, 80, 0, 170, 0, 75, 187, 31);

		private static Dictionary<string, Guid> languageWordBreakerGuidMapping;

		private static Dictionary<Guid, IWordBreaker> guidWordBreakerMapping = new Dictionary<Guid, IWordBreaker>();

		private static ReaderWriterLock cacheLock = new ReaderWriterLock();
	}
}
