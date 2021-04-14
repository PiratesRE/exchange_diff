using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class ExclusionList
	{
		private ExclusionList(string file)
		{
			ExclusionList.DebugTrace("ExclusionList::C'tor filename={0}", new object[]
			{
				file
			});
			this.pc = new ExclusionList.PatternCollection(file);
		}

		internal static ExclusionList Instance
		{
			get
			{
				if (ExclusionList.exclusionListInstance == null)
				{
					lock (ExclusionList.locObj)
					{
						if (ExclusionList.exclusionListInstance == null)
						{
							ExclusionList.exclusionListInstance = ExclusionList.GetExclusionList();
						}
					}
				}
				return ExclusionList.exclusionListInstance;
			}
		}

		private static ExclusionList GetExclusionList()
		{
			ExclusionList result = null;
			string exchangeBinPath = Utils.GetExchangeBinPath();
			string text = Path.Combine(exchangeBinPath, "SpeechGrammarFilterList.xml");
			if (!File.Exists(text))
			{
				ExclusionList.ErrorTrace("Exclusion list specified file {0} doesnt exist", new object[]
				{
					text
				});
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_SpeechGrammarFilterListInvalidWarning, null, new object[]
				{
					Strings.FileNotFound(text)
				});
			}
			else
			{
				try
				{
					ExclusionList.DebugTrace("ExclusionList::TryGetExclusionList() Initializing exclusion list file '{0}'", new object[]
					{
						text
					});
					result = new ExclusionList(text);
					ExclusionList.DebugTrace("ExclusionList::TryGetExclusionList() Exclusionlist file '{0}' initialized successfully", new object[]
					{
						text
					});
				}
				catch (IOException ex)
				{
					ExclusionList.ErrorTrace("ExclusionList::TryGetExclusionList() Error building exclusionlist file. Grammar generation will be aborted. Exception = {0}", new object[]
					{
						ex
					});
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_SpeechGrammarFilterListInvalidWarning, null, new object[]
					{
						ex.Message
					});
				}
				catch (XmlSchemaValidationException ex2)
				{
					ExclusionList.ErrorTrace("ExclusionList::TryGetExclusionList() Error building exclusionlist file. Grammar generation will be aborted. Exception = {0}", new object[]
					{
						ex2
					});
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_SpeechGrammarFilterListSchemaFailureWarning, null, new object[]
					{
						ex2.LineNumber,
						ex2.LinePosition,
						ex2.Message
					});
				}
				catch (InvalidSpeechGrammarFilterListException ex3)
				{
					ExclusionList.ErrorTrace("ExclusionList::TryGetExclusionList() Error building exclusionlist file. Grammar generation will be aborted. Exception = {0}", new object[]
					{
						ex3
					});
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_SpeechGrammarFilterListInvalidWarning, null, new object[]
					{
						ex3.Message
					});
				}
				catch (Exception ex4)
				{
					throw new ExclusionListException(ex4.Message, ex4);
				}
			}
			return result;
		}

		private static void DebugTrace(string formatString, params object[] formatObjects)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, null, formatString, formatObjects);
		}

		private static void ErrorTrace(string formatString, params object[] formatObjects)
		{
			CallIdTracer.TraceError(ExTraceGlobals.UMGrammarGeneratorTracer, null, formatString, formatObjects);
		}

		internal MatchResult GetReplacementStrings(string name, RecipientType recipientType, out List<Replacement> replacementStrings)
		{
			replacementStrings = null;
			List<Replacement> list = new List<Replacement>();
			string className = recipientType.ToString();
			bool flag = false;
			List<ExclusionList.Pattern> list2 = null;
			if (!this.pc.GetPattern(className, out list2))
			{
				return MatchResult.NotFound;
			}
			MatchResult result = MatchResult.NotFound;
			foreach (ExclusionList.Pattern pattern in list2)
			{
				Regex regex = pattern.Regex;
				if (flag = regex.IsMatch(name))
				{
					using (List<ExclusionList.PatternReplacement>.Enumerator enumerator2 = pattern.Replacements.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							ExclusionList.PatternReplacement patternReplacement = enumerator2.Current;
							string text = regex.Replace(name, patternReplacement.ReplacementString);
							if (text.Length > 0)
							{
								result = MatchResult.MatchWithReplacements;
								list.Add(new Replacement(text, patternReplacement.ShouldNormalize));
							}
							else
							{
								result = MatchResult.MatchWithNoReplacements;
							}
						}
						break;
					}
				}
			}
			if (!flag)
			{
				result = MatchResult.NoMatch;
			}
			replacementStrings = list;
			return result;
		}

		private const string ExclusionListFileName = "SpeechGrammarFilterList.xml";

		private const string GrammarFilterSchemaFile = "namemap.xsd";

		private static ExclusionList exclusionListInstance;

		private static object locObj = new object();

		private ExclusionList.PatternCollection pc;

		internal class PatternCollection
		{
			public PatternCollection(string fileName)
			{
				this.Parse(fileName);
			}

			public bool GetPattern(string className, out List<ExclusionList.Pattern> patterns)
			{
				return this.patternMap.TryGetValue(className, out patterns);
			}

			private static ExclusionList.Pattern ParsePatternNode(XmlNode root)
			{
				string text = null;
				List<ExclusionList.PatternReplacement> list = null;
				ExclusionList.Pattern pattern = null;
				foreach (object obj in root.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					if (!(xmlNode is XmlComment) && xmlNode.NodeType == XmlNodeType.Element)
					{
						if (string.Compare(xmlNode.Name, "Input", StringComparison.Ordinal) == 0)
						{
							text = xmlNode.InnerText;
						}
						else if (string.Compare(xmlNode.Name, "Output", StringComparison.Ordinal) == 0)
						{
							if (list == null)
							{
								list = new List<ExclusionList.PatternReplacement>();
							}
							string innerText = xmlNode.InnerText;
							bool shouldNormalize = true;
							XmlAttributeCollection attributes = xmlNode.Attributes;
							if (attributes != null)
							{
								foreach (object obj2 in attributes)
								{
									XmlAttribute xmlAttribute = (XmlAttribute)obj2;
									if (string.Compare(xmlAttribute.Name, "tn", StringComparison.Ordinal) == 0)
									{
										try
										{
											shouldNormalize = bool.Parse(xmlAttribute.Value);
										}
										catch (ArgumentNullException)
										{
										}
										catch (FormatException)
										{
										}
									}
								}
							}
							ExclusionList.PatternReplacement patternReplacement = new ExclusionList.PatternReplacement(innerText, shouldNormalize);
							if (list.Contains(patternReplacement))
							{
								throw new InvalidSpeechGrammarFilterListException(Strings.DuplicateReplacementStringError(patternReplacement.ReplacementString));
							}
							list.Add(patternReplacement);
						}
					}
				}
				if (!string.IsNullOrEmpty(text))
				{
					pattern = new ExclusionList.Pattern();
					pattern.RegexString = text;
					foreach (ExclusionList.PatternReplacement replacement in list)
					{
						pattern.AddReplacement(replacement);
					}
				}
				return pattern;
			}

			private void Parse(string filename)
			{
				Stream stream = null;
				XmlReader xmlReader = null;
				try
				{
					XmlDocument xmlDocument = new SafeXmlDocument();
					Assembly executingAssembly = Assembly.GetExecutingAssembly();
					stream = executingAssembly.GetManifestResourceStream("namemap.xsd");
					XmlSchemaSet xmlSchemaSet = new XmlSchemaSet();
					xmlSchemaSet.Add(null, SafeXmlFactory.CreateSafeXmlTextReader(stream));
					XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
					xmlReaderSettings.ValidationType = ValidationType.Schema;
					xmlReaderSettings.Schemas.Add(xmlSchemaSet);
					xmlReader = XmlReader.Create(filename, xmlReaderSettings);
					xmlDocument.Load(xmlReader);
					XmlNode documentElement = xmlDocument.DocumentElement;
					foreach (object obj in documentElement)
					{
						XmlNode xmlNode = (XmlNode)obj;
						if (!(xmlNode is XmlComment) && string.Compare(xmlNode.Name, "Rules", StringComparison.Ordinal) == 0)
						{
							this.ParseRulesNode(xmlNode);
						}
					}
				}
				finally
				{
					if (xmlReader != null)
					{
						xmlReader.Close();
					}
					if (stream != null)
					{
						stream.Close();
					}
				}
			}

			private void ParseRulesNode(XmlNode root)
			{
				foreach (object obj in root.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					if (!(xmlNode is XmlComment) && xmlNode.NodeType == XmlNodeType.Element && string.Compare(xmlNode.Name, "Rule", StringComparison.Ordinal) == 0)
					{
						List<ExclusionList.Pattern> list = new List<ExclusionList.Pattern>();
						List<string> list2 = new List<string>();
						this.ParseRuleNode(xmlNode, list, list2);
						foreach (string key in list2)
						{
							this.patternMap[key] = list;
						}
					}
				}
			}

			private void ParseRuleNode(XmlNode root, List<ExclusionList.Pattern> patterns, List<string> appliesTo)
			{
				foreach (object obj in root.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					if (!(xmlNode is XmlComment) && xmlNode.NodeType == XmlNodeType.Element)
					{
						if (string.Compare(xmlNode.Name, "Patterns", StringComparison.Ordinal) == 0)
						{
							this.ParsePatternsNode(xmlNode, patterns);
						}
						else if (string.Compare(xmlNode.Name, "Class", StringComparison.Ordinal) == 0)
						{
							string innerText = xmlNode.InnerText;
							if (appliesTo.Contains(innerText))
							{
								throw new InvalidSpeechGrammarFilterListException(Strings.DuplicateClassNameError(innerText));
							}
							List<ExclusionList.Pattern> list = null;
							if (this.patternMap.TryGetValue(innerText, out list))
							{
								throw new InvalidSpeechGrammarFilterListException(Strings.DuplicateClassNameError(innerText));
							}
							appliesTo.Add(innerText);
						}
					}
				}
			}

			private void ParsePatternsNode(XmlNode root, List<ExclusionList.Pattern> patterns)
			{
				foreach (object obj in root.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					if (!(xmlNode is XmlComment) && xmlNode.NodeType == XmlNodeType.Element && string.Compare(xmlNode.Name, "Pattern", StringComparison.Ordinal) == 0)
					{
						ExclusionList.Pattern pattern = ExclusionList.PatternCollection.ParsePatternNode(xmlNode);
						if (pattern != null)
						{
							patterns.Add(pattern);
						}
					}
				}
			}

			private Dictionary<string, List<ExclusionList.Pattern>> patternMap = new Dictionary<string, List<ExclusionList.Pattern>>();
		}

		internal class Pattern
		{
			internal Pattern()
			{
			}

			internal string RegexString
			{
				get
				{
					return this.regexString;
				}
				set
				{
					this.regexString = value;
					this.regex = new Regex(this.regexString, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
				}
			}

			internal Regex Regex
			{
				get
				{
					return this.regex;
				}
			}

			internal List<ExclusionList.PatternReplacement> Replacements
			{
				get
				{
					return this.replacements;
				}
			}

			internal void AddReplacement(ExclusionList.PatternReplacement replacement)
			{
				this.replacements.Add(replacement);
			}

			private string regexString;

			private Regex regex;

			private List<ExclusionList.PatternReplacement> replacements = new List<ExclusionList.PatternReplacement>();
		}

		internal class PatternReplacement : IEquatable<ExclusionList.PatternReplacement>
		{
			public PatternReplacement(string replacementString, bool shouldNormalize)
			{
				this.replacementString = replacementString;
				this.shouldNormalize = shouldNormalize;
			}

			public string ReplacementString
			{
				get
				{
					return this.replacementString;
				}
			}

			public bool ShouldNormalize
			{
				get
				{
					return this.shouldNormalize;
				}
			}

			public bool Equals(ExclusionList.PatternReplacement patternReplacement)
			{
				return string.Compare(this.replacementString, patternReplacement.ReplacementString, StringComparison.Ordinal) == 0;
			}

			private readonly string replacementString;

			private readonly bool shouldNormalize = true;
		}
	}
}
