using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public sealed class TraceContentBuilder
	{
		private TraceContentBuilder(int maximumChunks)
		{
			this.content = new List<string>(400);
			this.maximumChunks = maximumChunks;
			this.line = new StringBuilder(6144);
			this.length = 0;
		}

		public static int MaximumChunkLength
		{
			get
			{
				return 6144;
			}
		}

		public int Length
		{
			get
			{
				return this.length;
			}
		}

		public static TraceContentBuilder Create()
		{
			return new TraceContentBuilder(25);
		}

		public static TraceContentBuilder Create(int maximumChunks)
		{
			return new TraceContentBuilder(maximumChunks);
		}

		public void Append(int value)
		{
			this.Append(value.ToString());
		}

		public void Append(uint value)
		{
			this.Append(value.ToString());
		}

		public void Append(double value)
		{
			this.Append(value.ToString());
		}

		public void Append(string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				this.line.Append(value);
				this.length += value.Length;
			}
		}

		public void AppendLine(string value)
		{
			this.Append(value);
			this.AppendLine();
		}

		public void AppendLine()
		{
			this.Commit();
			this.content.Add(TraceContentBuilder.newLine);
			this.length += TraceContentBuilder.newLine.Length;
		}

		public void AppendFormat(string format, params object[] args)
		{
			this.Append(string.Format(format, args));
		}

		public void Indent(int depth)
		{
			this.Commit();
			switch (depth)
			{
			case 0:
				break;
			case 1:
				this.content.Add(TraceContentBuilder.tabOne);
				this.length += TraceContentBuilder.tabOne.Length;
				return;
			case 2:
				this.content.Add(TraceContentBuilder.tabTwo);
				this.length += TraceContentBuilder.tabTwo.Length;
				return;
			case 3:
				this.content.Add(TraceContentBuilder.tabThree);
				this.length += TraceContentBuilder.tabThree.Length;
				break;
			default:
				return;
			}
		}

		public void Remove()
		{
			if (this.line.Length > 0)
			{
				this.length -= this.line.Length;
				this.line.Clear();
				return;
			}
			if (this.content.Count > 0)
			{
				int index = this.content.Count - 1;
				this.length -= this.content[index].Length;
				this.content.RemoveAt(index);
			}
		}

		public List<string> ToWideString()
		{
			List<string> list = new List<string>(this.maximumChunks);
			this.Commit();
			if (this.content.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder(TraceContentBuilder.MaximumChunkLength);
				for (int i = 0; i < this.content.Count; i++)
				{
					if (stringBuilder.Length + this.content[i].Length > TraceContentBuilder.MaximumChunkLength)
					{
						list.Add(stringBuilder.ToString());
						stringBuilder.Clear();
					}
					stringBuilder.Append(this.content[i]);
				}
				if (stringBuilder.Length > 0)
				{
					list.Add(stringBuilder.ToString());
				}
			}
			return list;
		}

		public string ToTallString()
		{
			this.Commit();
			if (this.content.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder(this.length);
				for (int i = 0; i < this.content.Count; i++)
				{
					string text = this.content[i];
					if (object.ReferenceEquals(text, TraceContentBuilder.newLine))
					{
						stringBuilder.AppendLine();
					}
					else if (object.ReferenceEquals(text, TraceContentBuilder.tabOne))
					{
						stringBuilder.Append("\t");
					}
					else if (object.ReferenceEquals(text, TraceContentBuilder.tabTwo))
					{
						stringBuilder.Append("\t\t");
					}
					else if (object.ReferenceEquals(text, TraceContentBuilder.tabThree))
					{
						stringBuilder.Append("\t\t\t");
					}
					else
					{
						stringBuilder.Append(text);
					}
				}
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public override string ToString()
		{
			return this.ToTallString();
		}

		private static IEnumerable<string> SplitLongLine(string line)
		{
			if (line.Length > TraceContentBuilder.MaximumChunkLength)
			{
				int half = line.Length / 2;
				string left = line.Substring(0, half);
				string right = line.Substring(half);
				if (left.Length > TraceContentBuilder.MaximumChunkLength)
				{
					foreach (string segment in TraceContentBuilder.SplitLongLine(left))
					{
						yield return segment;
					}
				}
				else
				{
					yield return left;
				}
				if (right.Length > TraceContentBuilder.MaximumChunkLength)
				{
					foreach (string segment2 in TraceContentBuilder.SplitLongLine(right))
					{
						yield return segment2;
					}
				}
				else
				{
					yield return right;
				}
			}
			else
			{
				yield return line;
			}
			yield break;
		}

		private void Commit()
		{
			if (this.line.Length > TraceContentBuilder.MaximumChunkLength)
			{
				foreach (string item in TraceContentBuilder.SplitLongLine(this.line.ToString()))
				{
					this.content.Add(item);
				}
				this.line.Clear();
				return;
			}
			if (this.line.Length > 0)
			{
				this.content.Add(this.line.ToString());
				this.line.Clear();
			}
		}

		private const int MaximumChunksLength = 6144;

		private const int DefaultMaximumChunks = 25;

		private static string tabOne = "`t";

		private static string tabTwo = "`t`t";

		private static string tabThree = "`t`t`t";

		private static string newLine = "`r`n";

		private readonly StringBuilder line;

		private readonly List<string> content;

		private readonly int maximumChunks;

		private int length;
	}
}
