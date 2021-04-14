using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data
{
	internal sealed class CsvReader : DisposeTrackableBase
	{
		public CsvReader(StreamReader streamReader, bool isFirstLineHeader = true)
		{
			if (streamReader == null)
			{
				throw new ArgumentException("streamReader");
			}
			this.streamReader = streamReader;
			this.buffer = new char[65536];
			this.currentArrayOffset = this.buffer.Length;
			this.line = new StringBuilder(this.buffer.Length);
			if (isFirstLineHeader)
			{
				this.Headers = this.ReadLine();
			}
		}

		public int CharacterOffset
		{
			get
			{
				return this.characterOffset;
			}
		}

		public string[] Headers { get; private set; }

		internal void Seek(int characterOffset, long streamOffset)
		{
			this.streamReader.BaseStream.Seek(streamOffset, SeekOrigin.Begin);
			this.streamReader.DiscardBufferedData();
			this.currentArrayOffset = this.buffer.Length;
			this.characterOffset = characterOffset;
		}

		public string[] ReadLine()
		{
			bool flag = false;
			this.line.Length = 0;
			this.DevourCRLF();
			do
			{
				this.FillBufferIfNecessary();
				if (this.bufferSize == 0)
				{
					break;
				}
				int num = this.currentArrayOffset;
				while (this.currentArrayOffset < this.bufferSize)
				{
					char c = this.buffer[this.currentArrayOffset];
					if (c <= '\r')
					{
						if (c == '\n' || c == '\r')
						{
							goto IL_75;
						}
					}
					else if (c != '"')
					{
						switch (c)
						{
						case '\u2028':
						case '\u2029':
							goto IL_75;
						}
					}
					else
					{
						flag = !flag;
					}
					IL_78:
					this.currentArrayOffset++;
					this.characterOffset++;
					continue;
					IL_75:
					if (flag)
					{
						goto IL_78;
					}
					break;
				}
				int num2 = this.currentArrayOffset;
				if (num2 == num)
				{
					break;
				}
				this.line.Append(this.buffer, num, num2 - num);
			}
			while (this.currentArrayOffset >= this.bufferSize);
			this.DevourCRLF();
			if (this.line.Length == 0)
			{
				return null;
			}
			this.line.Append(',');
			MatchCollection matchCollection = CsvReader.regex.Matches(this.line.ToString());
			string[] array = new string[matchCollection.Count];
			for (int i = 0; i < array.Length; i++)
			{
				string text = matchCollection[i].Value;
				text = text.Substring(0, text.Length - 1);
				if (!string.IsNullOrEmpty(text))
				{
					text = text.Trim();
					if (text.Length > 0 && text[0] == '"')
					{
						int num3 = text.Length - 1;
						while (text[num3] != '"')
						{
							num3--;
						}
						text = text.Substring(1, num3 - 1);
					}
					text = text.Replace("\"\"", "\"");
				}
				array[i] = text;
			}
			return array;
		}

		public IEnumerable<string[]> ReadRows()
		{
			string[] row;
			while ((row = this.ReadLine()) != null)
			{
				yield return row;
			}
			yield break;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<CsvReader>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (this.streamReader != null)
			{
				this.streamReader.Dispose();
			}
		}

		private void DevourCRLF()
		{
			for (;;)
			{
				this.FillBufferIfNecessary();
				if (this.bufferSize == 0)
				{
					break;
				}
				while (this.currentArrayOffset < this.bufferSize && (this.buffer[this.currentArrayOffset] == '\r' || this.buffer[this.currentArrayOffset] == '\n' || this.buffer[this.currentArrayOffset] == '\u2028' || this.buffer[this.currentArrayOffset] == '\u2029'))
				{
					this.currentArrayOffset++;
					this.characterOffset++;
				}
				if (this.currentArrayOffset != this.buffer.Length)
				{
					return;
				}
			}
		}

		private void FillBufferIfNecessary()
		{
			if (this.currentArrayOffset == this.buffer.Length)
			{
				this.currentArrayOffset = 0;
				this.bufferSize = 0;
				int num;
				do
				{
					num = this.streamReader.Read(this.buffer, this.bufferSize, this.buffer.Length - this.bufferSize);
					this.bufferSize += num;
				}
				while (num != 0 && this.bufferSize < this.buffer.Length);
			}
		}

		private static readonly Regex regex = new Regex("(\"(?:[^\"]|\"\")*\"|[^\",]*),", RegexOptions.Compiled);

		private StreamReader streamReader;

		private char[] buffer;

		private int currentArrayOffset;

		private int bufferSize;

		private StringBuilder line;

		private int characterOffset;
	}
}
