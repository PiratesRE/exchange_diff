using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.Exchange.MessagingPolicies.Rules.PolicyNudges
{
	internal class ETRToPNRTranslator
	{
		internal ETRToPNRTranslator(string etrXml, ETRToPNRTranslator.IMessageStrings messageStrings, ETRToPNRTranslator.IDistributionListResolver distributionListResolver = null, ETRToPNRTranslator.IDataClassificationResolver dataClassificationResolver = null)
		{
			this.etrXml = etrXml;
			this.messageStrings = messageStrings;
			this.distributionListResolver = distributionListResolver;
			this.dataClassificationResolver = dataClassificationResolver;
			this.fullPnrXml = new Lazy<string>(new Func<string>(this.TryTransform));
		}

		internal RuleState Enabled
		{
			get
			{
				if (!this.IsValid)
				{
					throw new InvalidOperationException();
				}
				return this.enabled;
			}
		}

		internal DateTime? ActivationDate
		{
			get
			{
				if (!this.IsValid)
				{
					throw new InvalidOperationException();
				}
				return this.activationDate;
			}
		}

		internal DateTime? ExpiryDate
		{
			get
			{
				if (!this.IsValid)
				{
					throw new InvalidOperationException();
				}
				return this.expiryDate;
			}
		}

		internal string PnrXml
		{
			get
			{
				if (!this.IsValid)
				{
					throw new InvalidOperationException();
				}
				return this.pnrXml;
			}
		}

		internal string FullPnrXml
		{
			get
			{
				if (!this.IsValid)
				{
					throw new InvalidOperationException();
				}
				return this.fullPnrXml.Value;
			}
		}

		internal bool IsValid
		{
			get
			{
				return this.fullPnrXml.Value != null;
			}
		}

		private string TryTransform()
		{
			if (string.IsNullOrEmpty(this.etrXml))
			{
				return null;
			}
			XDocument xdocument;
			try
			{
				xdocument = ETRToPNRTranslator.CreateXDocument(this.etrXml);
			}
			catch (XmlException)
			{
				return null;
			}
			XElement xelement = this.TransformRule(xdocument.Root);
			if (xelement == null || !this.hasSenderNotifyAction)
			{
				return null;
			}
			xdocument = new XDocument(new object[]
			{
				xelement
			});
			ETRToPNRTranslator.OptimizePredicates(xdocument);
			if (this.IsFalseRootCondition(xdocument))
			{
				return null;
			}
			string result = xdocument.Root.ToString();
			this.pnrXml = result;
			if (this.RemoveVersionedDataClassifications(xdocument))
			{
				ETRToPNRTranslator.OptimizePredicates(xdocument);
				this.pnrXml = (this.IsFalseRootCondition(xdocument) ? null : xdocument.Root.ToString());
			}
			return result;
		}

		private bool IsFalseRootCondition(XDocument xDocument)
		{
			XElement xelement = xDocument.Descendants("false").FirstOrDefault<XElement>();
			return xelement != null && xelement.Parent != null && xelement.Parent.Name.LocalName == "condition";
		}

		private bool RemoveVersionedDataClassifications(XDocument xDocument)
		{
			bool result = false;
			List<XElement> list = (from dataClassificationElement in xDocument.Descendants("classification")
			let id = dataClassificationElement.Attribute("id").Value
			let rulePackageId = dataClassificationElement.Attribute("rulePackId").Value
			where this.dataClassificationResolver != null && this.dataClassificationResolver.IsVersionedDataClassification(id, rulePackageId)
			select dataClassificationElement).ToList<XElement>();
			foreach (XElement xelement in list)
			{
				result = true;
				xelement.ReplaceWith(new XElement("false"));
			}
			return result;
		}

		private bool CheckElement(XElement element, string name)
		{
			return element != null && element.Name.LocalName == name;
		}

		private string GetAttributeValue(XElement element, string name)
		{
			string result;
			if (!this.TryGetAttributeValue(element, name, out result))
			{
				return null;
			}
			return result;
		}

		private bool TryGetAttributeValue(XElement element, string name, out string value)
		{
			XAttribute xattribute = element.Attribute(name);
			value = ((xattribute != null) ? xattribute.Value : null);
			return xattribute != null;
		}

		private T TryParseAttribute<T>(XElement element, string name, ETRToPNRTranslator.TryParse<T> tryParse, T defaultValue)
		{
			T result;
			if (!tryParse(this.GetAttributeValue(element, name), out result))
			{
				return defaultValue;
			}
			return result;
		}

		private static XElement CreateXElement(string name, params object[] content)
		{
			XElement xelement = new XElement(name);
			int i = 0;
			while (i < content.Length)
			{
				object obj = content[i];
				if (obj != null)
				{
					IEnumerable<XElement> enumerable = obj as IEnumerable<XElement>;
					if (enumerable != null)
					{
						using (IEnumerator<XElement> enumerator = enumerable.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								XElement xelement2 = enumerator.Current;
								if (xelement2 == null)
								{
									return null;
								}
								xelement.Add(xelement2);
							}
							goto IL_6B;
						}
						goto IL_64;
					}
					goto IL_64;
					IL_6B:
					i++;
					continue;
					IL_64:
					xelement.Add(obj);
					goto IL_6B;
				}
				return null;
			}
			return xelement;
		}

		private XElement TransformRule(XElement ruleElement)
		{
			string value;
			if (!this.CheckElement(ruleElement, "rule") || !this.TryGetAttributeValue(ruleElement, "name", out value) || !RuleUtils.TryParseNullableDateTimeUtc(this.GetAttributeValue(ruleElement, "activationDate"), out this.activationDate) || !RuleUtils.TryParseNullableDateTimeUtc(this.GetAttributeValue(ruleElement, "expiryDate"), out this.expiryDate))
			{
				return null;
			}
			this.mode = this.TryParseAttribute<RuleMode>(ruleElement, "mode", new ETRToPNRTranslator.TryParse<RuleMode>(Enum.TryParse<RuleMode>), RuleMode.Enforce);
			if (this.mode == RuleMode.Audit)
			{
				return null;
			}
			string attributeValue = this.GetAttributeValue(ruleElement, "enabled");
			if (!string.IsNullOrEmpty(attributeValue) && !RuleConstants.TryParseEnabled(attributeValue, out this.enabled))
			{
				this.enabled = RuleState.Disabled;
			}
			XElement xelement = this.CheckElement(ruleElement.FirstElement(), "version") ? this.TransformVersion(ruleElement.FirstElement()) : ETRToPNRTranslator.CreateXElement("version", new object[]
			{
				new XAttribute("minRequiredVersion", "15.0.3225.3000"),
				this.TransformRuleContents(ruleElement)
			});
			return ETRToPNRTranslator.CreateXElement("rule", new object[]
			{
				new XAttribute("name", value),
				xelement
			});
		}

		private XElement TransformVersion(XElement versionElement)
		{
			Version v;
			if (!this.CheckElement(versionElement, "version") || !Version.TryParse(this.GetAttributeValue(versionElement, "requiredMinVersion"), out v))
			{
				return null;
			}
			if (v > ETRToPNRTranslator.HighestHonoredVersion)
			{
				return null;
			}
			return ETRToPNRTranslator.CreateXElement("version", new object[]
			{
				new XAttribute("minRequiredVersion", "15.0.3225.3000"),
				this.TransformRuleContents(versionElement)
			});
		}

		private IEnumerable<XElement> TransformRuleContents(XElement ruleElement)
		{
			XElement childElement = ruleElement.FirstElement();
			while (this.CheckElement(childElement, "fork"))
			{
				if (!this.BufferFork(childElement))
				{
					yield return null;
					IL_210:
					yield break;
				}
				childElement = childElement.NextElement();
			}
			if (this.CheckElement(childElement, "tags"))
			{
				childElement = childElement.NextElement();
			}
			XElement conditionElementPnr = this.TransformCondition(childElement);
			yield return conditionElementPnr;
			if (conditionElementPnr == null)
			{
				goto IL_210;
			}
			childElement = childElement.NextElement();
			if (!this.CheckElement(childElement, "action"))
			{
				yield return null;
				goto IL_210;
			}
			XElement actionsElement = new XElement("actions");
			while (this.CheckElement(childElement, "action"))
			{
				XNode actionNodePnr = this.TransformAction(childElement);
				if (actionNodePnr == null)
				{
					yield return null;
					goto IL_210;
				}
				actionsElement.Add(actionNodePnr);
				childElement = childElement.NextElement();
			}
			yield return actionsElement;
			if (childElement != null)
			{
				yield return null;
				goto IL_210;
			}
			goto IL_210;
		}

		private bool BufferFork(XElement forkElement)
		{
			if (!forkElement.Elements().Any<XElement>())
			{
				return false;
			}
			ETRToPNRTranslator.Fork fork = new ETRToPNRTranslator.Fork
			{
				IsException = this.TryParseAttribute<bool>(forkElement, "exception", new ETRToPNRTranslator.TryParse<bool>(bool.TryParse), false)
			};
			foreach (XElement xelement in forkElement.Elements())
			{
				string localName;
				if ((localName = xelement.Name.LocalName) != null)
				{
					if (!(localName == "recipient"))
					{
						if (localName == "external")
						{
							fork.IsExternal = true;
							continue;
						}
						if (localName == "internal")
						{
							fork.IsInternal = true;
							continue;
						}
						if (localName == "externalPartner")
						{
							fork.IsExternalPartner = true;
							continue;
						}
						if (localName == "externalNonPartner")
						{
							fork.IsExternalNonPartner = true;
							continue;
						}
					}
					else
					{
						string item;
						if (!this.TryGetAttributeValue(xelement, "address", out item))
						{
							return false;
						}
						fork.recipients.Add(item);
						continue;
					}
				}
				return false;
			}
			this.forks.Add(fork);
			return true;
		}

		private IEnumerable<XElement> TransformForks()
		{
			return (from fork in this.forks
			group fork by fork.IsException into forkGroup
			select new
			{
				forkGroup = forkGroup,
				isExceptionForksGroup = forkGroup.Key
			}).Select(delegate(<>h__TransparentIdentifier11)
			{
				if (<>h__TransparentIdentifier11.isExceptionForksGroup)
				{
					return new XElement("not", new XElement("or", from fork in <>h__TransparentIdentifier11.forkGroup
					select fork.TransformPredicates()));
				}
				return new XElement("and", from fork in <>h__TransparentIdentifier11.forkGroup
				select new XElement("or", fork.TransformPredicates()));
			});
		}

		private XElement TransformCondition(XElement conditionElement)
		{
			if (!this.CheckElement(conditionElement, "condition") || conditionElement.Elements().Take(2).Count<XElement>() != 1)
			{
				return null;
			}
			return ETRToPNRTranslator.CreateXElement("condition", new object[]
			{
				ETRToPNRTranslator.CreateXElement("and", new object[]
				{
					this.TransformPredicate(conditionElement.FirstElement()),
					this.TransformForks()
				})
			});
		}

		private XElement TransformPredicate(XElement predicateElement)
		{
			if (predicateElement == null)
			{
				return null;
			}
			string localName;
			if ((localName = predicateElement.Name.LocalName) != null)
			{
				if (localName == "true")
				{
					return ETRToPNRTranslator.CreateXElement("true", new object[0]);
				}
				if (localName == "false")
				{
					return ETRToPNRTranslator.CreateXElement("false", new object[0]);
				}
				if (!(localName == "not"))
				{
					if (!(localName == "and"))
					{
						if (localName == "or")
						{
							if (!predicateElement.Elements().Any<XElement>())
							{
								return null;
							}
							string name = "or";
							object[] array = new object[1];
							array[0] = from element in predicateElement.Elements()
							select this.TransformPredicate(element);
							return ETRToPNRTranslator.CreateXElement(name, array);
						}
					}
					else
					{
						if (!predicateElement.Elements().Any<XElement>())
						{
							return null;
						}
						string name2 = "and";
						object[] array2 = new object[1];
						array2[0] = from element in predicateElement.Elements()
						select this.TransformPredicate(element);
						return ETRToPNRTranslator.CreateXElement(name2, array2);
					}
				}
				else
				{
					if (predicateElement.Elements().Take(2).Count<XElement>() != 1)
					{
						return null;
					}
					return ETRToPNRTranslator.CreateXElement("not", new object[]
					{
						this.TransformPredicate(predicateElement.FirstElement())
					});
				}
			}
			return this.TransformPedicate_Advanced(predicateElement);
		}

		private XElement TransformPedicate_Advanced(XElement predicateElement)
		{
			string a;
			if (!this.TryGetAttributeValue(predicateElement, "property", out a))
			{
				return null;
			}
			IList<string> list = null;
			IList<IList<KeyValuePair<string, string>>> list2 = null;
			XElement xelement = predicateElement.FirstElement();
			if (this.CheckElement(xelement, "keyValues"))
			{
				list2 = new List<IList<KeyValuePair<string, string>>>();
				while (this.CheckElement(xelement, "keyValues"))
				{
					IList<KeyValuePair<string, string>> list3 = new List<KeyValuePair<string, string>>();
					XElement xelement2 = xelement.FirstElement();
					while (this.CheckElement(xelement2, "keyValue"))
					{
						list3.Add(new KeyValuePair<string, string>(this.GetAttributeValue(xelement2, "key"), this.GetAttributeValue(xelement2, "value")));
						xelement2 = xelement2.NextElement();
					}
					if (xelement2 != null || !list3.Any<KeyValuePair<string, string>>())
					{
						return null;
					}
					list2.Add(list3);
					xelement = xelement.NextElement();
				}
			}
			else
			{
				list = new List<string>();
				while (this.CheckElement(xelement, "value"))
				{
					if (xelement.Elements().Any<XElement>())
					{
						return null;
					}
					list.Add(xelement.Value);
					xelement = xelement.NextElement();
				}
			}
			if (xelement != null)
			{
				return null;
			}
			string localName;
			if ((localName = predicateElement.Name.LocalName) != null)
			{
				if (!(localName == "containsDataClassification"))
				{
					if (!(localName == "isSameUser"))
					{
						if (!(localName == "isMemberOf"))
						{
							if (!(localName == "isInternal"))
							{
								if (localName == "is")
								{
									if (list == null || !list.Any<string>() || a != "Message.Auth")
									{
										return null;
									}
									if (list[0] != "<>")
									{
										return null;
									}
									return ETRToPNRTranslator.CreateXElement("false", new object[0]);
								}
							}
							else
							{
								if (list == null || a != "Message.From")
								{
									return null;
								}
								return ETRToPNRTranslator.CreateXElement("true", new object[0]);
							}
						}
						else
						{
							if (list == null || !list.Any<string>() || a != "Message.From")
							{
								return null;
							}
							if (this.distributionListResolver != null)
							{
								foreach (string distributionList in list)
								{
									this.distributionListResolver.Get(distributionList);
								}
							}
							string name = "or";
							object[] array = new object[1];
							array[0] = from value in list
							select new XElement("sender", new XAttribute("distributionGroup", value));
							return ETRToPNRTranslator.CreateXElement(name, array);
						}
					}
					else
					{
						if (list == null || !list.Any<string>() || a != "Message.From")
						{
							return null;
						}
						string name2 = "or";
						object[] array2 = new object[1];
						array2[0] = from value in list
						select new XElement("sender", new XAttribute("address", value));
						return ETRToPNRTranslator.CreateXElement(name2, array2);
					}
				}
				else
				{
					if (list2 == null || a != "Message.DataClassifications")
					{
						return null;
					}
					return new XElement("or", this.TransformPredicate_ContainsDataClassification(list2));
				}
			}
			return null;
		}

		private IEnumerable<XElement> TransformPredicate_ContainsDataClassification(IList<IList<KeyValuePair<string, string>>> keyValueCollections)
		{
			foreach (IList<KeyValuePair<string, string>> keyValueCollection in keyValueCollections)
			{
				string Id = null;
				string MinCount = null;
				string MaxCount = null;
				string MinConfidence = null;
				string MaxConfidence = null;
				string OpaqueData = null;
				foreach (KeyValuePair<string, string> keyValuePair in keyValueCollection)
				{
					if (string.Compare(keyValuePair.Key, "id", StringComparison.OrdinalIgnoreCase) == 0)
					{
						Id = keyValuePair.Value;
					}
					else if (string.Compare(keyValuePair.Key, "minCount", StringComparison.OrdinalIgnoreCase) == 0 && keyValuePair.Value != "1")
					{
						MinCount = keyValuePair.Value;
					}
					else if (string.Compare(keyValuePair.Key, "maxCount", StringComparison.OrdinalIgnoreCase) == 0 && keyValuePair.Value != "-1")
					{
						MaxCount = keyValuePair.Value;
					}
					else if (string.Compare(keyValuePair.Key, "minConfidence", StringComparison.OrdinalIgnoreCase) == 0 && keyValuePair.Value != "-1")
					{
						MinConfidence = keyValuePair.Value;
					}
					else if (string.Compare(keyValuePair.Key, "maxConfidence", StringComparison.OrdinalIgnoreCase) == 0 && keyValuePair.Value != "100")
					{
						MaxConfidence = keyValuePair.Value;
					}
					else if (string.Compare(keyValuePair.Key, "opaqueData", StringComparison.OrdinalIgnoreCase) == 0)
					{
						OpaqueData = keyValuePair.Value;
					}
				}
				int ignoredValue;
				if (string.IsNullOrEmpty(Id) || (MinCount != null && !int.TryParse(MinCount, out ignoredValue)) || (MaxCount != null && !int.TryParse(MaxCount, out ignoredValue)) || (MinConfidence != null && !int.TryParse(MinConfidence, out ignoredValue)) || (MaxConfidence != null && !int.TryParse(MaxConfidence, out ignoredValue)))
				{
					yield return null;
					yield break;
				}
				yield return new XElement("classification", new object[]
				{
					new XAttribute("id", Id),
					(MinCount != null) ? new XAttribute("minCount", MinCount) : null,
					(MaxCount != null) ? new XAttribute("maxCount", MaxCount) : null,
					(MinConfidence != null) ? new XAttribute("minConfidence", MinConfidence) : null,
					(MaxConfidence != null) ? new XAttribute("maxConfidence", MaxConfidence) : null,
					(OpaqueData != null) ? new XAttribute("rulePackId", OpaqueData) : null
				});
			}
			yield break;
		}

		private XNode TransformAction(XElement actionElement)
		{
			string text;
			if (!this.TryGetAttributeValue(actionElement, "name", out text))
			{
				return null;
			}
			IList<string> list = new List<string>();
			XElement xelement = actionElement.FirstElement();
			while (this.CheckElement(xelement, "argument"))
			{
				string item;
				if (this.TryGetAttributeValue(xelement, "value", out item))
				{
					list.Add(item);
				}
				xelement = xelement.NextElement();
			}
			if (xelement != null)
			{
				return null;
			}
			string a;
			if ((a = text) != null && a == "SenderNotify")
			{
				return this.TransformAction_SenderNotify(list);
			}
			return new XComment("Ignored action: " + text);
		}

		private XElement TransformAction_SenderNotify(IList<string> arguments)
		{
			ETRToPNRTranslator.OutlookActionTypes outlookActionTypes;
			if (arguments.Count < 1 || !Enum.TryParse<ETRToPNRTranslator.OutlookActionTypes>(arguments[0], out outlookActionTypes))
			{
				return null;
			}
			if (this.mode == RuleMode.AuditAndNotify)
			{
				outlookActionTypes = ETRToPNRTranslator.OutlookActionTypes.NotifyOnly;
			}
			bool flag = outlookActionTypes == ETRToPNRTranslator.OutlookActionTypes.NotifyOnly;
			bool flag2 = outlookActionTypes == ETRToPNRTranslator.OutlookActionTypes.RejectUnlessSilentOverride || outlookActionTypes == ETRToPNRTranslator.OutlookActionTypes.RejectUnlessExplicitOverride;
			bool flag3 = outlookActionTypes == ETRToPNRTranslator.OutlookActionTypes.RejectUnlessExplicitOverride;
			this.hasSenderNotifyAction = true;
			return new XElement(flag ? "notify" : "block", new object[]
			{
				new XElement("message", new XElement("locale", new object[]
				{
					new XAttribute("name", this.messageStrings.OutlookCultureTag),
					new XElement("complianceNoteUrl", this.messageStrings.Url.Value),
					new XElement("text2", this.messageStrings.Get(outlookActionTypes).Value)
				})),
				new XElement("override", new object[]
				{
					new XAttribute("allow", flag2 ? "yes" : "no"),
					new XElement("justification", new XAttribute("type", flag3 ? "required" : "none"))
				}),
				new XElement("falsePositive", new XAttribute("allow", "yes"))
			});
		}

		private static XDocument CreateXDocument(string xml)
		{
			XmlReaderSettings settings = new XmlReaderSettings
			{
				ConformanceLevel = ConformanceLevel.Auto,
				IgnoreComments = true,
				DtdProcessing = DtdProcessing.Prohibit,
				XmlResolver = null
			};
			XDocument result;
			using (StringReader stringReader = new StringReader(xml))
			{
				using (XmlReader xmlReader = XmlReader.Create(stringReader, settings))
				{
					result = XDocument.Load(xmlReader);
				}
			}
			return result;
		}

		public static string Evaluate(string policyNudgeRuleXml, ETRToPNRTranslator.IDistributionListResolver distributionListResolver)
		{
			XDocument xdocument = ETRToPNRTranslator.CreateXDocument(policyNudgeRuleXml);
			XElement xelement = xdocument.Root.Element("version").Element("condition");
			var source = from senderPredicate in xelement.Descendants("sender")
			let distributionGroupAttribute = senderPredicate.Attribute("distributionGroup")
			where distributionGroupAttribute != null
			select new
			{
				Element = senderPredicate,
				DistributionList = distributionGroupAttribute.Value
			};
			foreach (var <>f__AnonymousType in source.ToList())
			{
				bool flag = distributionListResolver.IsMemberOf(<>f__AnonymousType.DistributionList);
				<>f__AnonymousType.Element.ReplaceWith(new object[]
				{
					new XElement(flag ? "true" : "false"),
					new XComment("Replaced sender is member of distribution list \"" + <>f__AnonymousType.DistributionList + "\"")
				});
			}
			return xdocument.ToString();
		}

		private static void OptimizePredicates(XDocument xdoc)
		{
			while (ETRToPNRTranslator.optimizations.Any((ETRToPNRTranslator.IRuleOptimizer optimization) => optimization.Optimize(xdoc)))
			{
			}
		}

		private const string outlookVersionConstant = "15.0.3225.3000";

		private static HashSet<string> knownPNRPredicatetNames = new HashSet<string>
		{
			"condition",
			"actions",
			"and",
			"classification",
			"false",
			"not",
			"or",
			"recipient",
			"sender",
			"true"
		};

		public static readonly Version HighestHonoredVersion = new Version("15.00.0015.00");

		private RuleMode mode = RuleMode.Enforce;

		private List<ETRToPNRTranslator.Fork> forks = new List<ETRToPNRTranslator.Fork>();

		private ETRToPNRTranslator.IMessageStrings messageStrings;

		private ETRToPNRTranslator.IDistributionListResolver distributionListResolver;

		private ETRToPNRTranslator.IDataClassificationResolver dataClassificationResolver;

		private readonly string etrXml;

		private RuleState enabled;

		private DateTime? activationDate;

		private DateTime? expiryDate;

		private bool hasSenderNotifyAction;

		private string pnrXml;

		private Lazy<string> fullPnrXml;

		private static readonly IEnumerable<ETRToPNRTranslator.IRuleOptimizer> optimizations = new ETRToPNRTranslator.IRuleOptimizer[]
		{
			new ETRToPNRTranslator.FalseElementUnderParentNotOptimizer(),
			new ETRToPNRTranslator.FalseElementUnderParentAndOptimizer(),
			new ETRToPNRTranslator.TrueElementUnderParentNotOptimizer(),
			new ETRToPNRTranslator.MergeAndOrElementsWithSameParentOptimizer(),
			new ETRToPNRTranslator.NotElementUnderParentNotOptimizer(),
			new ETRToPNRTranslator.FalseElementUnderParentOrOptimizer(),
			new ETRToPNRTranslator.TrueElementUnderParentOrOptimizer(),
			new ETRToPNRTranslator.TrueElementUnderParentAndOptimizer(),
			new ETRToPNRTranslator.RemoveExtraniousAndOrOptimizer()
		};

		public enum OutlookActionTypes
		{
			NotifyOnly,
			RejectMessage,
			RejectUnlessFalsePositiveOverride,
			RejectUnlessSilentOverride,
			RejectUnlessExplicitOverride
		}

		private static class ETRConstants
		{
			internal const string AttributeAddress = "address";

			internal const string AttributeId = "id";

			internal const string AttributeMinCount = "minCount";

			internal const string AttributeMaxCount = "maxCount";

			internal const string AttributeMinConfidence = "minConfidence";

			internal const string AttributeMaxConfidence = "maxConfidence";

			internal const string AttributeOpaqueData = "opaqueData";

			internal const string AttributeValueSenderNotify = "SenderNotify";

			internal const string TagActions = "actions";

			internal const string TagAttachmentContainsWords = "attachmentContainsWords";

			internal const string TagAttachmentIsUnsupported = "attachmentIsUnsupported";

			internal const string TagContainsDataClassification = "containsDataClassification";

			internal const string TagExternal = "external";

			internal const string TagExternalNonPartner = "externalNonPartner";

			internal const string TagExternalPartner = "externalPartner";

			internal const string TagFork = "fork";

			internal const string TagHasSenderOverride = "hasSenderOverride";

			internal const string TagInternal = "internal";

			internal const string TagIsInternal = "isInternal";

			internal const string TagIsMemberOf = "isMemberOf";

			internal const string TagIsMessageType = "isMessageType";

			internal const string TagIsSameUser = "isSameUser";

			internal const string TagRecipient = "recipient";

			internal const string TagSender = "sender";

			internal const string PropertyAuth = "Message.Auth";

			internal const string PropertyFrom = "Message.From";

			internal const string PropertyDataClassifications = "Message.DataClassifications";
		}

		private static class PNRConstants
		{
			internal const string AttributeAddress = "address";

			internal const string AttributeDistributionGroup = "distributionGroup";

			internal const string AttributeId = "id";

			internal const string AttributeMinCount = "minCount";

			internal const string AttributeMinRequiredVersion = "minRequiredVersion";

			internal const string AttributeMaxCount = "maxCount";

			internal const string AttributeMinConfidence = "minConfidence";

			internal const string AttributeMaxConfidence = "maxConfidence";

			internal const string AttributeRulePackId = "rulePackId";

			internal const string AttributeScope = "scope";

			internal const string AttributeValueInternal = "Internal";

			internal const string AttributeValueExternal = "External";

			internal const string AttributeValueExternalPartner = "ExternalPartner";

			internal const string AttributeValueExternalNonPartner = "ExternalNonPartner";

			internal const string TagCondition = "condition";

			internal const string TagActions = "actions";

			internal const string TagAnd = "and";

			internal const string TagClassification = "classification";

			internal const string TagFalse = "false";

			internal const string TagNot = "not";

			internal const string TagOr = "or";

			internal const string TagRecipient = "recipient";

			internal const string TagSender = "sender";

			internal const string TagTrue = "true";

			internal const string TagNotify = "notify";

			internal const string TagBlock = "block";

			internal const string TagMessage = "message";

			internal const string TagLocale = "locale";

			internal const string TagOverride = "override";

			internal const string AttributeAllow = "allow";

			internal const string AttributeValueYes = "yes";

			internal const string AttributeValueNo = "no";

			internal const string TagJustification = "justification";

			internal const string AttributeType = "type";

			internal const string AttributeValueRequired = "required";

			internal const string AttributeValueNone = "none";

			internal const string TagFalsePositive = "falsePositive";
		}

		private class Fork
		{
			internal IEnumerable<XElement> TransformPredicates()
			{
				foreach (string recipient in this.recipients)
				{
					yield return ETRToPNRTranslator.CreateXElement("recipient", new object[]
					{
						new XAttribute("address", recipient)
					});
				}
				if (this.IsInternal)
				{
					yield return ETRToPNRTranslator.CreateXElement("recipient", new object[]
					{
						new XAttribute("scope", "Internal")
					});
				}
				if (this.IsExternal)
				{
					yield return ETRToPNRTranslator.CreateXElement("recipient", new object[]
					{
						new XAttribute("scope", "External")
					});
				}
				if (this.IsExternalPartner)
				{
					yield return ETRToPNRTranslator.CreateXElement("recipient", new object[]
					{
						new XAttribute("scope", "ExternalPartner")
					});
				}
				if (this.IsExternalNonPartner)
				{
					yield return ETRToPNRTranslator.CreateXElement("recipient", new object[]
					{
						new XAttribute("scope", "ExternalNonPartner")
					});
				}
				yield break;
			}

			internal bool IsException;

			internal List<string> recipients = new List<string>();

			internal bool IsInternal;

			internal bool IsExternal;

			internal bool IsExternalPartner;

			internal bool IsExternalNonPartner;
		}

		private delegate bool TryParse<T>(string stringValue, out T value);

		internal interface IDistributionListResolver
		{
			IEnumerable<IVersionedItem> Get(string distributionList);

			bool IsMemberOf(string distributionList);
		}

		internal class DistributionListResolverCallbackImpl : ETRToPNRTranslator.IDistributionListResolver
		{
			public DistributionListResolverCallbackImpl(Func<string, IEnumerable<IVersionedItem>> onGet, Func<string, bool> onIsMemberOf)
			{
				this.onGet = onGet;
				this.onIsMemberOf = onIsMemberOf;
			}

			public IEnumerable<IVersionedItem> Get(string distributionList)
			{
				return this.onGet(distributionList);
			}

			public bool IsMemberOf(string distributionList)
			{
				return this.onIsMemberOf(distributionList);
			}

			private Func<string, IEnumerable<IVersionedItem>> onGet;

			private Func<string, bool> onIsMemberOf;
		}

		internal interface IDataClassificationResolver
		{
			bool IsVersionedDataClassification(string dataClassificationId, string rulePackageId);
		}

		public interface IMessageStrings
		{
			string OutlookCultureTag { get; }

			PolicyTipMessage Get(ETRToPNRTranslator.OutlookActionTypes type);

			PolicyTipMessage Url { get; }
		}

		internal class MessageStringCallbackImpl : ETRToPNRTranslator.IMessageStrings
		{
			public MessageStringCallbackImpl(string outlookCultureTag, Func<ETRToPNRTranslator.OutlookActionTypes, PolicyTipMessage> onGet, Func<PolicyTipMessage> onGetUrl)
			{
				this.OutlookCultureTag = outlookCultureTag;
				this.onGet = onGet;
				this.onGetUrl = onGetUrl;
			}

			public string OutlookCultureTag { get; private set; }

			public PolicyTipMessage Get(ETRToPNRTranslator.OutlookActionTypes type)
			{
				return this.onGet(type);
			}

			public PolicyTipMessage Url
			{
				get
				{
					return this.onGetUrl();
				}
			}

			private Func<ETRToPNRTranslator.OutlookActionTypes, PolicyTipMessage> onGet;

			private Func<PolicyTipMessage> onGetUrl;
		}

		internal interface IRuleOptimizer
		{
			bool Optimize(XDocument xdoc);
		}

		internal class FalseElementUnderParentNotOptimizer : ETRToPNRTranslator.IRuleOptimizer
		{
			public bool Optimize(XDocument xdoc)
			{
				XElement xelement = (from el in xdoc.Descendants("false")
				where el.Parent.Name.LocalName == "not"
				select el).FirstOrDefault<XElement>();
				if (xelement == null)
				{
					return false;
				}
				xelement.Parent.ReplaceWith(new XElement("true"));
				return true;
			}
		}

		internal class FalseElementUnderParentAndOptimizer : ETRToPNRTranslator.IRuleOptimizer
		{
			public bool Optimize(XDocument xdoc)
			{
				XElement xelement = (from el in xdoc.Descendants("false")
				where el.Parent.Name.LocalName == "and"
				select el).FirstOrDefault<XElement>();
				if (xelement == null)
				{
					return false;
				}
				xelement.Parent.ReplaceWith(new XElement("false"));
				return true;
			}
		}

		internal class FalseElementUnderParentOrOptimizer : ETRToPNRTranslator.IRuleOptimizer
		{
			public bool Optimize(XDocument xdoc)
			{
				XElement xelement = (from el in xdoc.Descendants("false")
				where el.Parent.Name.LocalName == "or"
				where el.Parent.Elements().Take(2).Count<XElement>() > 1
				select el).FirstOrDefault<XElement>();
				if (xelement == null)
				{
					return false;
				}
				xelement.Remove();
				return true;
			}
		}

		internal class TrueElementUnderParentNotOptimizer : ETRToPNRTranslator.IRuleOptimizer
		{
			public bool Optimize(XDocument xdoc)
			{
				XElement xelement = (from el in xdoc.Descendants("true")
				where el.Parent.Name.LocalName == "not"
				select el).FirstOrDefault<XElement>();
				if (xelement == null)
				{
					return false;
				}
				xelement.Parent.ReplaceWith(new XElement("false"));
				return true;
			}
		}

		internal class TrueElementUnderParentOrOptimizer : ETRToPNRTranslator.IRuleOptimizer
		{
			public bool Optimize(XDocument xdoc)
			{
				XElement xelement = (from el in xdoc.Descendants("true")
				where el.Parent.Name.LocalName == "or"
				where el.Parent.Elements().Take(2).Count<XElement>() > 1
				select el).FirstOrDefault<XElement>();
				if (xelement == null)
				{
					return false;
				}
				xelement.Remove();
				return true;
			}
		}

		internal class TrueElementUnderParentAndOptimizer : ETRToPNRTranslator.IRuleOptimizer
		{
			public bool Optimize(XDocument xdoc)
			{
				XElement xelement = (from el in xdoc.Descendants("true")
				where el.Parent.Name.LocalName == "and"
				where el.Parent.Elements().Take(2).Count<XElement>() > 1
				select el).FirstOrDefault<XElement>();
				if (xelement == null)
				{
					return false;
				}
				xelement.Remove();
				return true;
			}
		}

		internal class MergeAndOrElementsWithSameParentOptimizer : ETRToPNRTranslator.IRuleOptimizer
		{
			public bool Optimize(XDocument xdoc)
			{
				XElement xelement = (from andOrElement in xdoc.Descendants("and").Concat(xdoc.Descendants("or"))
				where andOrElement.Parent != null
				where andOrElement.Name.LocalName == andOrElement.Parent.Name.LocalName
				select andOrElement).FirstOrDefault<XElement>();
				if (xelement == null)
				{
					return false;
				}
				xelement.Parent.Add(xelement.Elements());
				xelement.Remove();
				return true;
			}
		}

		internal class NotElementUnderParentNotOptimizer : ETRToPNRTranslator.IRuleOptimizer
		{
			public bool Optimize(XDocument xdoc)
			{
				XElement xelement = (from notElement in xdoc.Descendants("not")
				where notElement.Parent != null
				where notElement.Parent.Name.LocalName == "not"
				select notElement).FirstOrDefault<XElement>();
				if (xelement == null)
				{
					return false;
				}
				xelement.Parent.ReplaceWith(xelement.Elements());
				return true;
			}
		}

		internal class RemoveExtraniousAndOrOptimizer : ETRToPNRTranslator.IRuleOptimizer
		{
			public bool Optimize(XDocument xdoc)
			{
				XElement xelement = (from andOrElement in xdoc.Descendants("and").Concat(xdoc.Descendants("or"))
				where andOrElement.Elements().Take(2).Count<XElement>() == 1
				select andOrElement).FirstOrDefault<XElement>();
				if (xelement == null)
				{
					return false;
				}
				xelement.ReplaceWith(xelement.Elements());
				return true;
			}
		}
	}
}
