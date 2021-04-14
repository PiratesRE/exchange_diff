using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class CustomGrammarBase
	{
		protected CustomGrammarBase(CultureInfo transcriptionLanguage)
		{
			this.transcriptionLanguage = transcriptionLanguage;
		}

		internal abstract string FileName { get; }

		internal abstract string Rule { get; }

		internal bool IsEmpty
		{
			get
			{
				return this.ItemsXml.Length == 0;
			}
		}

		protected CultureInfo TranscriptionLanguage
		{
			get
			{
				return this.transcriptionLanguage;
			}
		}

		private StringBuilder ItemsXml
		{
			get
			{
				if (this.itemsXml == null)
				{
					this.itemsXml = this.GenerateItemsXml();
				}
				return this.itemsXml;
			}
		}

		internal virtual void WriteCustomGrammar(string customGrammarDir)
		{
			if (this.IsEmpty)
			{
				return;
			}
			using (StreamWriter streamWriter = new StreamWriter(Path.Combine(customGrammarDir, this.FileName)))
			{
				streamWriter.Write(string.Format(CultureInfo.InvariantCulture, "<?xml version=\"1.0\"?>\r\n<grammar xml:lang=\"{0}\" version=\"1.0\" xmlns=\"http://www.w3.org/2001/06/grammar\" tag-format=\"semantics/1.0\">\r\n<tag>out.customGrammarWords=false;out.topNWords=false;</tag>\r\n    <rule id=\"{1}\" scope=\"public\">\r\n        <one-of>", new object[]
				{
					this.transcriptionLanguage,
					this.Rule
				}));
				streamWriter.Write(this.ItemsXml.ToString());
				streamWriter.Write("      \r\n        </one-of>\r\n    </rule>\r\n</grammar>");
			}
		}

		protected abstract List<GrammarItemBase> GetItems();

		private StringBuilder GenerateItemsXml()
		{
			StringBuilder stringBuilder = new StringBuilder();
			List<GrammarItemBase> items = this.GetItems();
			HashSet<GrammarItemBase> hashSet = new HashSet<GrammarItemBase>();
			foreach (GrammarItemBase grammarItemBase in items)
			{
				if (!grammarItemBase.IsEmpty && !hashSet.Contains(grammarItemBase))
				{
					hashSet.Add(grammarItemBase);
					stringBuilder.Append(grammarItemBase.ToString());
				}
			}
			return stringBuilder;
		}

		private readonly CultureInfo transcriptionLanguage;

		private StringBuilder itemsXml;
	}
}
