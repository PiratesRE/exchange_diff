using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	internal class ScriptSharpSourceMapLoader : SourceMapLoader<ScriptSharpSymbolWrapper>
	{
		public ScriptSharpSourceMapLoader(IEnumerable<string> symbolMapFiles) : base(symbolMapFiles)
		{
		}

		protected override void LoadSymbolsMapFromFile(string filePath, Dictionary<string, List<ScriptSharpSymbolWrapper>> symbolMaps, Dictionary<uint, string> sourceFileIdMap, ClientWatsonFunctionNamePool functionNamePool)
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			Dictionary<int, List<ScriptSharpSymbolWrapper>> dictionary2 = new Dictionary<int, List<ScriptSharpSymbolWrapper>>();
			Stack<ScriptSharpSourceMapLoader.ContainingMethodContext> stack = new Stack<ScriptSharpSourceMapLoader.ContainingMethodContext>();
			Stack<ScriptSharpSourceMapLoader.ParentContext> stack2 = new Stack<ScriptSharpSourceMapLoader.ParentContext>();
			string arg = null;
			string arg2 = null;
			int num = -1;
			int key = -1;
			int count = sourceFileIdMap.Count;
			using (XmlReader xmlReader = XmlReader.Create(filePath))
			{
				while (xmlReader.Read())
				{
					XmlNodeType xmlNodeType = xmlReader.MoveToContent();
					if (xmlNodeType == XmlNodeType.Element)
					{
						if (xmlReader.Name == "Statement")
						{
							ScriptSharpSourceMapLoader.ContainingMethodContext containingMethodContext = stack.Peek();
							ScriptSharpSymbol scriptSharpSymbol;
							if (ScriptSharpSourceMapLoader.TryParseSymbol(xmlReader, containingMethodContext.ScriptOffset, containingMethodContext.FunctionNameIndex, num, out scriptSharpSymbol))
							{
								ScriptSharpSourceMapLoader.PopNonParentSymbolsOut(stack2, scriptSharpSymbol, dictionary2[key]);
								stack2.Push(new ScriptSharpSourceMapLoader.ParentContext(scriptSharpSymbol));
							}
						}
						else if (xmlReader.Name == "Method")
						{
							string attribute = xmlReader.GetAttribute("SourceFile");
							if (string.IsNullOrEmpty(attribute))
							{
								ScriptSharpSourceMapLoader.SkipSubTree(xmlReader, "Method");
							}
							else
							{
								if (!dictionary.TryGetValue(attribute, out num))
								{
									num = dictionary.Count + count;
									dictionary.Add(attribute, num);
								}
								arg2 = xmlReader.GetAttribute("Name");
								string name = string.Format("{0}.{1}", arg, arg2);
								int orAddFunctionNameIndex = functionNamePool.GetOrAddFunctionNameIndex(name);
								ScriptSharpSymbol symbol;
								if (!ScriptSharpSourceMapLoader.TryParseSymbol(xmlReader, 0, orAddFunctionNameIndex, num, out symbol))
								{
									ScriptSharpSourceMapLoader.SkipSubTree(xmlReader, "Method");
								}
								else if (!xmlReader.IsEmptyElement)
								{
									stack.Push(new ScriptSharpSourceMapLoader.ContainingMethodContext(symbol.ScriptStartPosition, orAddFunctionNameIndex));
									stack2.Push(new ScriptSharpSourceMapLoader.ParentContext(symbol, true));
								}
								else
								{
									dictionary2[key].Add(new ScriptSharpSymbolWrapper(symbol));
								}
							}
						}
						else if (xmlReader.Name == "AnonymousMethod")
						{
							ScriptSharpSourceMapLoader.ContainingMethodContext containingMethodContext2 = stack.Peek();
							string name2 = string.Format("{0}.<{1}>anonymous", arg, arg2);
							int orAddFunctionNameIndex2 = functionNamePool.GetOrAddFunctionNameIndex(name2);
							ScriptSharpSymbol scriptSharpSymbol2;
							if (!ScriptSharpSourceMapLoader.TryParseSymbol(xmlReader, containingMethodContext2.ScriptOffset, orAddFunctionNameIndex2, num, out scriptSharpSymbol2))
							{
								ScriptSharpSourceMapLoader.SkipSubTree(xmlReader, "AnonymousMethod");
							}
							else
							{
								List<ScriptSharpSymbolWrapper> list = dictionary2[key];
								ScriptSharpSourceMapLoader.ParentContext parentContext = ScriptSharpSourceMapLoader.PopNonParentSymbolsOut(stack2, scriptSharpSymbol2, list);
								if (!xmlReader.IsEmptyElement)
								{
									stack.Push(new ScriptSharpSourceMapLoader.ContainingMethodContext(scriptSharpSymbol2.ScriptStartPosition, orAddFunctionNameIndex2));
									stack2.Push(new ScriptSharpSourceMapLoader.ParentContext(scriptSharpSymbol2, true));
								}
								else
								{
									parentContext.Children.Add(list.Count);
									list.Add(new ScriptSharpSymbolWrapper(scriptSharpSymbol2));
								}
							}
						}
						else if (xmlReader.Name == "Type")
						{
							arg = xmlReader.GetAttribute("Name");
							string attribute2 = xmlReader.GetAttribute("SegmentRef");
							if (string.IsNullOrEmpty(attribute2) || !int.TryParse(attribute2, out key))
							{
								ScriptSharpSourceMapLoader.SkipSubTree(xmlReader, "Type");
							}
							else if (!dictionary2.ContainsKey(key))
							{
								dictionary2.Add(key, new List<ScriptSharpSymbolWrapper>(1024));
							}
						}
						else if (xmlReader.Name == "Segments")
						{
							IL_3CE:
							while (xmlReader.Read())
							{
								if (xmlReader.IsStartElement("Segment"))
								{
									string attribute3 = xmlReader.GetAttribute("Id");
									int key2;
									List<ScriptSharpSymbolWrapper> list2;
									if (int.TryParse(attribute3, out key2) && dictionary2.TryGetValue(key2, out list2))
									{
										string packageName = ScriptSharpSourceMapLoader.GetPackageName(xmlReader, filePath);
										try
										{
											symbolMaps.Add(packageName, list2);
											list2.TrimExcess();
										}
										catch (ArgumentException exception)
										{
											string extraData = string.Format("Package: {0}, File name: {1}", packageName, filePath);
											ExWatson.SendReport(exception, ReportOptions.DoNotCollectDumps, extraData);
										}
									}
								}
							}
							goto IL_3E5;
						}
					}
					else if (xmlNodeType == XmlNodeType.EndElement && (xmlReader.Name == "Method" || xmlReader.Name == "AnonymousMethod"))
					{
						stack.Pop();
						List<ScriptSharpSymbolWrapper> symbolList = dictionary2[key];
						bool flag = false;
						while (stack2.Count > 0 && !flag)
						{
							ScriptSharpSourceMapLoader.ParentContext parentContext2 = ScriptSharpSourceMapLoader.PopItemAndAddToList(stack2, symbolList);
							flag = parentContext2.IsMethod;
						}
					}
				}
				goto IL_3CE;
			}
			IL_3E5:
			foreach (KeyValuePair<string, int> keyValuePair in dictionary)
			{
				sourceFileIdMap.Add((uint)keyValuePair.Value, keyValuePair.Key);
			}
		}

		private static string GetPackageName(XmlReader reader, string symbolFilePath)
		{
			string text = reader.GetAttribute("Package");
			string attribute = reader.GetAttribute("Slice");
			if (!string.IsNullOrEmpty(attribute))
			{
				text = string.Format("{0}.{1}", text, attribute);
			}
			if (text == "**SOURCE**")
			{
				text = Regex.Replace(symbolFilePath, Regex.Escape("_obfuscate.xml"), string.Empty, RegexOptions.IgnoreCase);
			}
			return text;
		}

		private static ScriptSharpSourceMapLoader.ParentContext PopNonParentSymbolsOut(Stack<ScriptSharpSourceMapLoader.ParentContext> parentCandidateStack, ScriptSharpSymbol newSymbol, List<ScriptSharpSymbolWrapper> symbolList)
		{
			while (parentCandidateStack.Count > 0)
			{
				ScriptSharpSourceMapLoader.ParentContext parentContext = parentCandidateStack.Peek();
				if (parentContext.Symbol.ScriptEndPosition >= newSymbol.ScriptEndPosition)
				{
					return parentContext;
				}
				ScriptSharpSourceMapLoader.PopItemAndAddToList(parentCandidateStack, symbolList);
			}
			return null;
		}

		private static ScriptSharpSourceMapLoader.ParentContext PopItemAndAddToList(Stack<ScriptSharpSourceMapLoader.ParentContext> parentCandidateStack, List<ScriptSharpSymbolWrapper> symbolList)
		{
			ScriptSharpSourceMapLoader.ParentContext parentContext = parentCandidateStack.Pop();
			int count = symbolList.Count;
			if (parentCandidateStack.Count > 0)
			{
				parentCandidateStack.Peek().Children.Add(count);
			}
			foreach (int index in parentContext.Children)
			{
				ScriptSharpSymbolWrapper value = symbolList[index];
				value.ParentSymbolIndex = count;
				symbolList[index] = value;
			}
			symbolList.Add(new ScriptSharpSymbolWrapper(parentContext.Symbol));
			return parentContext;
		}

		private static void SkipSubTree(XmlReader reader, string name)
		{
			if (!reader.IsEmptyElement)
			{
				int num = 0;
				while (reader.Read())
				{
					reader.MoveToContent();
					if (reader.Name == name)
					{
						if (reader.NodeType == XmlNodeType.Element && !reader.IsEmptyElement)
						{
							num++;
						}
						else if (reader.NodeType == XmlNodeType.EndElement)
						{
							if (num == 0)
							{
								return;
							}
							num--;
						}
					}
				}
			}
		}

		private static bool TryParseSymbol(XmlReader reader, int scriptOffset, int functionNameIndex, int sourceFileId, out ScriptSharpSymbol symbol)
		{
			symbol = default(ScriptSharpSymbol);
			try
			{
				int num = int.Parse(reader.GetAttribute("ScriptStartPosition"));
				int num2 = int.Parse(reader.GetAttribute("ScriptEndPosition"));
				symbol = new ScriptSharpSymbol
				{
					ScriptStartPosition = scriptOffset + num,
					ScriptEndPosition = scriptOffset + num2,
					FunctionNameIndex = functionNameIndex,
					ParentSymbol = -1,
					SourceStartLine = int.Parse(reader.GetAttribute("SourceStartLine")),
					SourceFileId = (uint)sourceFileId
				};
			}
			catch (ArgumentNullException)
			{
				return false;
			}
			catch (OverflowException)
			{
				return false;
			}
			catch (FormatException)
			{
				return false;
			}
			return true;
		}

		public const string ScriptSharpMapFilePattern = "*_obfuscate.xml";

		private const string ScriptSharpMapFileSuffix = "_obfuscate.xml";

		private const string MethodNameFormat = "{0}.{1}";

		private const string AnonymousMethodNameFormat = "{0}.<{1}>anonymous";

		private const string GenericPackageName = "**SOURCE**";

		private class ContainingMethodContext
		{
			public int ScriptOffset { get; private set; }

			public int FunctionNameIndex { get; private set; }

			public ContainingMethodContext(int scriptOffset, int functionNameIndex)
			{
				this.ScriptOffset = scriptOffset;
				this.FunctionNameIndex = functionNameIndex;
			}
		}

		private class ParentContext
		{
			public ScriptSharpSymbol Symbol { get; private set; }

			public bool IsMethod { get; private set; }

			public List<int> Children { get; private set; }

			public ParentContext(ScriptSharpSymbol symbol) : this(symbol, false)
			{
			}

			public ParentContext(ScriptSharpSymbol symbol, bool isMethod)
			{
				this.Symbol = symbol;
				this.IsMethod = isMethod;
				this.Children = new List<int>();
			}
		}

		private static class XmlMapNames
		{
			public const string TypeNode = "Type";

			public const string MethodNode = "Method";

			public const string StatementNode = "Statement";

			public const string AnonymousMethodNode = "AnonymousMethod";

			public const string SegmentsGroupNode = "Segments";

			public const string SegmentNode = "Segment";

			public const string SegmentRefAttribute = "SegmentRef";

			public const string NameAttribute = "Name";

			public const string SourceFileAttribute = "SourceFile";

			public const string ScriptStartPositionAttribute = "ScriptStartPosition";

			public const string ScriptEndPositionAttribute = "ScriptEndPosition";

			public const string SourceStartLineAttribute = "SourceStartLine";

			public const string SourceStartColumnAttribute = "SourceStartCharacter";

			public const string SourceEndLineAttribute = "SourceEndLine";

			public const string SourceEndColumnAttribute = "SourceEndCharacter";

			public const string IdAttribute = "Id";

			public const string PackageAttribute = "Package";

			public const string SliceAttribute = "Slice";
		}
	}
}
