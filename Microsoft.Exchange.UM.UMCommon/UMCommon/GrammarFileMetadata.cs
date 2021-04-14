using System;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class GrammarFileMetadata
	{
		public GrammarFileMetadata(string path, string hash, long size)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, this, "GrammarFileMetadata constructor - path='{0}', hash='{1}', size='{2}'", new object[]
			{
				path,
				hash,
				size
			});
			this.Path = path;
			this.Hash = hash;
			this.Size = size;
		}

		public string Path { get; private set; }

		public string Hash { get; private set; }

		public long Size { get; private set; }
	}
}
