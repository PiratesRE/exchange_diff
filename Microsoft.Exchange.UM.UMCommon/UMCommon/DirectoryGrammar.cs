using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal abstract class DirectoryGrammar
	{
		public string FilePath
		{
			get
			{
				return this.filePath;
			}
		}

		public bool MaxEntriesExceeded { get; private set; }

		public int NumEntries { get; private set; }

		public void InitializeGrammar(string path, CultureInfo c)
		{
			ValidateArgument.NotNullOrEmpty(path, "path");
			ValidateArgument.NotNull(c, "c");
			CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, this, "DirectoryGrammar.InitializeGrammar - path='{0}', culture='{1}'", new object[]
			{
				path,
				c
			});
			this.filePath = path;
			this.grammarFile = new GrammarFile(this.filePath, c);
		}

		public void WriteADEntry(ADEntry entry)
		{
			ValidateArgument.NotNull(entry, "entry");
			PIIMessage data = PIIMessage.Create(PIIType._SmtpAddress, entry.SmtpAddress);
			CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, this, data, "DirectoryGrammar.WriteADEntry - Processing entry=_SmtpAddress for '{0}'", new object[]
			{
				this.filePath
			});
			if (this.MaxEntriesExceeded)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, this, "Max entries exceeded - numEntries={0}, filePath={1}", new object[]
				{
					this.NumEntries,
					this.filePath
				});
				return;
			}
			if (this.ShouldAcceptGrammarEntry(entry))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, this, data, "DirectoryGrammar.WriteADEntry - Writing entry=_SmtpAddress to '{0}'", new object[]
				{
					this.filePath
				});
				this.grammarFile.WriteEntry(entry);
				this.NumEntries += entry.Names.Count;
				if (this.NumEntries > 250000)
				{
					this.MaxEntriesExceeded = true;
				}
			}
		}

		public string CompleteGrammar()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, this, "DirectoryGrammar.CompleteGrammar - '{0}'", new object[]
			{
				this.filePath
			});
			this.grammarFile.Close();
			this.grammarFile = null;
			return this.filePath;
		}

		public abstract string FileName { get; }

		protected virtual bool ShouldAcceptGrammarEntry(ADEntry entry)
		{
			return true;
		}

		private const int MaxEntries = 250000;

		private GrammarFile grammarFile;

		private string filePath;
	}
}
