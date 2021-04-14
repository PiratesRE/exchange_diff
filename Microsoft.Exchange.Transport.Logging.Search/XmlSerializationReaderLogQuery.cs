using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Transport.Logging.Search
{
	internal class XmlSerializationReaderLogQuery : XmlSerializationReader
	{
		public object Read28_LogQuery()
		{
			object result = null;
			base.Reader.MoveToContent();
			if (base.Reader.NodeType == XmlNodeType.Element)
			{
				if (base.Reader.LocalName != this.id1_LogQuery || base.Reader.NamespaceURI != this.id2_Item)
				{
					throw base.CreateUnknownNodeException();
				}
				result = this.Read27_LogQuery(true, true);
			}
			else
			{
				base.UnknownNode(null, ":LogQuery");
			}
			return result;
		}

		private LogQuery Read27_LogQuery(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id1_LogQuery || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			LogQuery logQuery = new LogQuery();
			bool[] array = new bool[3];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(logQuery);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return logQuery;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[0] && base.Reader.LocalName == this.id3_Beginning && base.Reader.NamespaceURI == this.id2_Item)
					{
						logQuery.Beginning = XmlSerializationReader.ToDateTime(base.Reader.ReadElementString());
						array[0] = true;
					}
					else if (!array[1] && base.Reader.LocalName == this.id4_End && base.Reader.NamespaceURI == this.id2_Item)
					{
						logQuery.End = XmlSerializationReader.ToDateTime(base.Reader.ReadElementString());
						array[1] = true;
					}
					else if (!array[2] && base.Reader.LocalName == this.id5_Filter && base.Reader.NamespaceURI == this.id2_Item)
					{
						logQuery.Filter = this.Read26_LogCondition(false, true);
						array[2] = true;
					}
					else
					{
						base.UnknownNode(logQuery, ":Beginning, :End, :Filter");
					}
				}
				else
				{
					base.UnknownNode(logQuery, ":Beginning, :End, :Filter");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return logQuery;
		}

		private LogCondition Read26_LogCondition(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id6_LogCondition || xmlQualifiedName.Namespace != this.id2_Item))
			{
				if (xmlQualifiedName.Name == this.id7_LogBinaryOperatorCondition && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read25_LogBinaryOperatorCondition(isNullable, false);
				}
				if (xmlQualifiedName.Name == this.id8_Compare && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read24_LogComparisonCondition(isNullable, false);
				}
				if (xmlQualifiedName.Name == this.id9_StringCompare && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read23_LogStringComparisonCondition(isNullable, false);
				}
				if (xmlQualifiedName.Name == this.id10_Item && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read21_Item(isNullable, false);
				}
				if (xmlQualifiedName.Name == this.id11_Contains && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read20_LogStringContainsCondition(isNullable, false);
				}
				if (xmlQualifiedName.Name == this.id12_EndsWith && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read19_LogStringEndsWithCondition(isNullable, false);
				}
				if (xmlQualifiedName.Name == this.id13_StartsWith && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read18_LogStringStartsWithCondition(isNullable, false);
				}
				if (xmlQualifiedName.Name == this.id14_False && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read17_LogFalseCondition(isNullable, false);
				}
				if (xmlQualifiedName.Name == this.id15_LogUnaryCondition && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read16_LogUnaryCondition(isNullable, false);
				}
				if (xmlQualifiedName.Name == this.id16_Not && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read15_LogNotCondition(isNullable, false);
				}
				if (xmlQualifiedName.Name == this.id17_LogQuantifierCondition && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read14_LogQuantifierCondition(isNullable, false);
				}
				if (xmlQualifiedName.Name == this.id18_Any && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read13_LogForAnyCondition(isNullable, false);
				}
				if (xmlQualifiedName.Name == this.id19_Every && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read12_LogForEveryCondition(isNullable, false);
				}
				if (xmlQualifiedName.Name == this.id20_True && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read11_LogTrueCondition(isNullable, false);
				}
				if (xmlQualifiedName.Name == this.id21_LogUnaryOperatorCondition && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read10_LogUnaryOperatorCondition(isNullable, false);
				}
				if (xmlQualifiedName.Name == this.id22_IsNullOrEmpty && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read9_LogIsNullOrEmptyCondition(isNullable, false);
				}
				if (xmlQualifiedName.Name == this.id23_LogCompoundCondition && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read4_LogCompoundCondition(isNullable, false);
				}
				if (xmlQualifiedName.Name == this.id24_And && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read3_LogAndCondition(isNullable, false);
				}
				if (xmlQualifiedName.Name == this.id25_Or && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read2_LogOrCondition(isNullable, false);
				}
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			else
			{
				if (flag)
				{
					return null;
				}
				throw base.CreateAbstractTypeException("LogCondition", "");
			}
		}

		private LogOrCondition Read2_LogOrCondition(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id25_Or || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			LogOrCondition logOrCondition = new LogOrCondition();
			List<LogCondition> conditions = logOrCondition.Conditions;
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(logOrCondition);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return logOrCondition;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (base.Reader.LocalName == this.id26_Conditions && base.Reader.NamespaceURI == this.id2_Item)
					{
						if (!base.ReadNull())
						{
							List<LogCondition> conditions2 = logOrCondition.Conditions;
							if (conditions2 == null || base.Reader.IsEmptyElement)
							{
								base.Reader.Skip();
							}
							else
							{
								base.Reader.ReadStartElement();
								base.Reader.MoveToContent();
								int num2 = 0;
								int readerCount2 = base.ReaderCount;
								while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
								{
									if (base.Reader.NodeType == XmlNodeType.Element)
									{
										if (base.Reader.LocalName == this.id27_Condition && base.Reader.NamespaceURI == this.id2_Item)
										{
											if (conditions2 == null)
											{
												base.Reader.Skip();
											}
											else
											{
												conditions2.Add(this.Read26_LogCondition(true, true));
											}
										}
										else
										{
											base.UnknownNode(null, ":Condition");
										}
									}
									else
									{
										base.UnknownNode(null, ":Condition");
									}
									base.Reader.MoveToContent();
									base.CheckReaderCount(ref num2, ref readerCount2);
								}
								base.ReadEndElement();
							}
						}
					}
					else
					{
						base.UnknownNode(logOrCondition, ":Conditions");
					}
				}
				else
				{
					base.UnknownNode(logOrCondition, ":Conditions");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return logOrCondition;
		}

		private LogAndCondition Read3_LogAndCondition(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id24_And || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			LogAndCondition logAndCondition = new LogAndCondition();
			List<LogCondition> conditions = logAndCondition.Conditions;
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(logAndCondition);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return logAndCondition;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (base.Reader.LocalName == this.id26_Conditions && base.Reader.NamespaceURI == this.id2_Item)
					{
						if (!base.ReadNull())
						{
							List<LogCondition> conditions2 = logAndCondition.Conditions;
							if (conditions2 == null || base.Reader.IsEmptyElement)
							{
								base.Reader.Skip();
							}
							else
							{
								base.Reader.ReadStartElement();
								base.Reader.MoveToContent();
								int num2 = 0;
								int readerCount2 = base.ReaderCount;
								while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
								{
									if (base.Reader.NodeType == XmlNodeType.Element)
									{
										if (base.Reader.LocalName == this.id27_Condition && base.Reader.NamespaceURI == this.id2_Item)
										{
											if (conditions2 == null)
											{
												base.Reader.Skip();
											}
											else
											{
												conditions2.Add(this.Read26_LogCondition(true, true));
											}
										}
										else
										{
											base.UnknownNode(null, ":Condition");
										}
									}
									else
									{
										base.UnknownNode(null, ":Condition");
									}
									base.Reader.MoveToContent();
									base.CheckReaderCount(ref num2, ref readerCount2);
								}
								base.ReadEndElement();
							}
						}
					}
					else
					{
						base.UnknownNode(logAndCondition, ":Conditions");
					}
				}
				else
				{
					base.UnknownNode(logAndCondition, ":Conditions");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return logAndCondition;
		}

		private LogCompoundCondition Read4_LogCompoundCondition(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id23_LogCompoundCondition || xmlQualifiedName.Namespace != this.id2_Item))
			{
				if (xmlQualifiedName.Name == this.id24_And && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read3_LogAndCondition(isNullable, false);
				}
				if (xmlQualifiedName.Name == this.id25_Or && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read2_LogOrCondition(isNullable, false);
				}
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			else
			{
				if (flag)
				{
					return null;
				}
				throw base.CreateAbstractTypeException("LogCompoundCondition", "");
			}
		}

		private LogIsNullOrEmptyCondition Read9_LogIsNullOrEmptyCondition(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id22_IsNullOrEmpty || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			LogIsNullOrEmptyCondition logIsNullOrEmptyCondition = new LogIsNullOrEmptyCondition();
			bool[] array = new bool[1];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(logIsNullOrEmptyCondition);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return logIsNullOrEmptyCondition;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[0] && base.Reader.LocalName == this.id28_Operand && base.Reader.NamespaceURI == this.id2_Item)
					{
						logIsNullOrEmptyCondition.Operand = this.Read8_LogConditionOperand(false, true);
						array[0] = true;
					}
					else
					{
						base.UnknownNode(logIsNullOrEmptyCondition, ":Operand");
					}
				}
				else
				{
					base.UnknownNode(logIsNullOrEmptyCondition, ":Operand");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return logIsNullOrEmptyCondition;
		}

		private LogConditionOperand Read8_LogConditionOperand(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id29_LogConditionOperand || xmlQualifiedName.Namespace != this.id2_Item))
			{
				if (xmlQualifiedName.Name == this.id30_Field && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read7_LogConditionField(isNullable, false);
				}
				if (xmlQualifiedName.Name == this.id31_Constant && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read6_LogConditionConstant(isNullable, false);
				}
				if (xmlQualifiedName.Name == this.id32_Variable && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read5_LogConditionVariable(isNullable, false);
				}
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			else
			{
				if (flag)
				{
					return null;
				}
				throw base.CreateAbstractTypeException("LogConditionOperand", "");
			}
		}

		private LogConditionVariable Read5_LogConditionVariable(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id32_Variable || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			LogConditionVariable logConditionVariable = new LogConditionVariable();
			bool[] array = new bool[1];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(logConditionVariable);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return logConditionVariable;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[0] && base.Reader.LocalName == this.id33_Name && base.Reader.NamespaceURI == this.id2_Item)
					{
						logConditionVariable.Name = base.Reader.ReadElementString();
						array[0] = true;
					}
					else
					{
						base.UnknownNode(logConditionVariable, ":Name");
					}
				}
				else
				{
					base.UnknownNode(logConditionVariable, ":Name");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return logConditionVariable;
		}

		private LogConditionConstant Read6_LogConditionConstant(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id31_Constant || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			LogConditionConstant logConditionConstant = new LogConditionConstant();
			bool[] array = new bool[1];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(logConditionConstant);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return logConditionConstant;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[0] && base.Reader.LocalName == this.id34_Value && base.Reader.NamespaceURI == this.id2_Item)
					{
						logConditionConstant.Value = this.Read1_Object(false, true);
						array[0] = true;
					}
					else
					{
						base.UnknownNode(logConditionConstant, ":Value");
					}
				}
				else
				{
					base.UnknownNode(logConditionConstant, ":Value");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return logConditionConstant;
		}

		private object Read1_Object(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType)
			{
				if (flag)
				{
					if (xmlQualifiedName != null)
					{
						return base.ReadTypedNull(xmlQualifiedName);
					}
					return null;
				}
				else
				{
					if (xmlQualifiedName == null)
					{
						return base.ReadTypedPrimitive(new XmlQualifiedName("anyType", "http://www.w3.org/2001/XMLSchema"));
					}
					if (xmlQualifiedName.Name == this.id1_LogQuery && xmlQualifiedName.Namespace == this.id2_Item)
					{
						return this.Read27_LogQuery(isNullable, false);
					}
					if (xmlQualifiedName.Name == this.id29_LogConditionOperand && xmlQualifiedName.Namespace == this.id2_Item)
					{
						return this.Read8_LogConditionOperand(isNullable, false);
					}
					if (xmlQualifiedName.Name == this.id30_Field && xmlQualifiedName.Namespace == this.id2_Item)
					{
						return this.Read7_LogConditionField(isNullable, false);
					}
					if (xmlQualifiedName.Name == this.id31_Constant && xmlQualifiedName.Namespace == this.id2_Item)
					{
						return this.Read6_LogConditionConstant(isNullable, false);
					}
					if (xmlQualifiedName.Name == this.id32_Variable && xmlQualifiedName.Namespace == this.id2_Item)
					{
						return this.Read5_LogConditionVariable(isNullable, false);
					}
					if (xmlQualifiedName.Name == this.id6_LogCondition && xmlQualifiedName.Namespace == this.id2_Item)
					{
						return this.Read26_LogCondition(isNullable, false);
					}
					if (xmlQualifiedName.Name == this.id7_LogBinaryOperatorCondition && xmlQualifiedName.Namespace == this.id2_Item)
					{
						return this.Read25_LogBinaryOperatorCondition(isNullable, false);
					}
					if (xmlQualifiedName.Name == this.id8_Compare && xmlQualifiedName.Namespace == this.id2_Item)
					{
						return this.Read24_LogComparisonCondition(isNullable, false);
					}
					if (xmlQualifiedName.Name == this.id9_StringCompare && xmlQualifiedName.Namespace == this.id2_Item)
					{
						return this.Read23_LogStringComparisonCondition(isNullable, false);
					}
					if (xmlQualifiedName.Name == this.id10_Item && xmlQualifiedName.Namespace == this.id2_Item)
					{
						return this.Read21_Item(isNullable, false);
					}
					if (xmlQualifiedName.Name == this.id11_Contains && xmlQualifiedName.Namespace == this.id2_Item)
					{
						return this.Read20_LogStringContainsCondition(isNullable, false);
					}
					if (xmlQualifiedName.Name == this.id12_EndsWith && xmlQualifiedName.Namespace == this.id2_Item)
					{
						return this.Read19_LogStringEndsWithCondition(isNullable, false);
					}
					if (xmlQualifiedName.Name == this.id13_StartsWith && xmlQualifiedName.Namespace == this.id2_Item)
					{
						return this.Read18_LogStringStartsWithCondition(isNullable, false);
					}
					if (xmlQualifiedName.Name == this.id14_False && xmlQualifiedName.Namespace == this.id2_Item)
					{
						return this.Read17_LogFalseCondition(isNullable, false);
					}
					if (xmlQualifiedName.Name == this.id15_LogUnaryCondition && xmlQualifiedName.Namespace == this.id2_Item)
					{
						return this.Read16_LogUnaryCondition(isNullable, false);
					}
					if (xmlQualifiedName.Name == this.id16_Not && xmlQualifiedName.Namespace == this.id2_Item)
					{
						return this.Read15_LogNotCondition(isNullable, false);
					}
					if (xmlQualifiedName.Name == this.id17_LogQuantifierCondition && xmlQualifiedName.Namespace == this.id2_Item)
					{
						return this.Read14_LogQuantifierCondition(isNullable, false);
					}
					if (xmlQualifiedName.Name == this.id18_Any && xmlQualifiedName.Namespace == this.id2_Item)
					{
						return this.Read13_LogForAnyCondition(isNullable, false);
					}
					if (xmlQualifiedName.Name == this.id19_Every && xmlQualifiedName.Namespace == this.id2_Item)
					{
						return this.Read12_LogForEveryCondition(isNullable, false);
					}
					if (xmlQualifiedName.Name == this.id20_True && xmlQualifiedName.Namespace == this.id2_Item)
					{
						return this.Read11_LogTrueCondition(isNullable, false);
					}
					if (xmlQualifiedName.Name == this.id21_LogUnaryOperatorCondition && xmlQualifiedName.Namespace == this.id2_Item)
					{
						return this.Read10_LogUnaryOperatorCondition(isNullable, false);
					}
					if (xmlQualifiedName.Name == this.id22_IsNullOrEmpty && xmlQualifiedName.Namespace == this.id2_Item)
					{
						return this.Read9_LogIsNullOrEmptyCondition(isNullable, false);
					}
					if (xmlQualifiedName.Name == this.id23_LogCompoundCondition && xmlQualifiedName.Namespace == this.id2_Item)
					{
						return this.Read4_LogCompoundCondition(isNullable, false);
					}
					if (xmlQualifiedName.Name == this.id24_And && xmlQualifiedName.Namespace == this.id2_Item)
					{
						return this.Read3_LogAndCondition(isNullable, false);
					}
					if (xmlQualifiedName.Name == this.id25_Or && xmlQualifiedName.Namespace == this.id2_Item)
					{
						return this.Read2_LogOrCondition(isNullable, false);
					}
					if (xmlQualifiedName.Name == this.id35_ArrayOfLogCondition && xmlQualifiedName.Namespace == this.id2_Item)
					{
						List<LogCondition> list = null;
						if (!base.ReadNull())
						{
							if (list == null)
							{
								list = new List<LogCondition>();
							}
							List<LogCondition> list2 = list;
							if (base.Reader.IsEmptyElement)
							{
								base.Reader.Skip();
							}
							else
							{
								base.Reader.ReadStartElement();
								base.Reader.MoveToContent();
								int num = 0;
								int readerCount = base.ReaderCount;
								while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
								{
									if (base.Reader.NodeType == XmlNodeType.Element)
									{
										if (base.Reader.LocalName == this.id27_Condition && base.Reader.NamespaceURI == this.id2_Item)
										{
											if (list2 == null)
											{
												base.Reader.Skip();
											}
											else
											{
												list2.Add(this.Read26_LogCondition(true, true));
											}
										}
										else
										{
											base.UnknownNode(null, ":Condition");
										}
									}
									else
									{
										base.UnknownNode(null, ":Condition");
									}
									base.Reader.MoveToContent();
									base.CheckReaderCount(ref num, ref readerCount);
								}
								base.ReadEndElement();
							}
						}
						return list;
					}
					if (xmlQualifiedName.Name == this.id36_LogComparisonOperator && xmlQualifiedName.Namespace == this.id2_Item)
					{
						base.Reader.ReadStartElement();
						object result = this.Read22_LogComparisonOperator(base.CollapseWhitespace(base.Reader.ReadString()));
						base.ReadEndElement();
						return result;
					}
					return base.ReadTypedPrimitive(xmlQualifiedName);
				}
			}
			else
			{
				if (flag)
				{
					return null;
				}
				object obj = new object();
				while (base.Reader.MoveToNextAttribute())
				{
					if (!base.IsXmlnsAttribute(base.Reader.Name))
					{
						base.UnknownNode(obj);
					}
				}
				base.Reader.MoveToElement();
				if (base.Reader.IsEmptyElement)
				{
					base.Reader.Skip();
					return obj;
				}
				base.Reader.ReadStartElement();
				base.Reader.MoveToContent();
				int num2 = 0;
				int readerCount2 = base.ReaderCount;
				while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
				{
					if (base.Reader.NodeType == XmlNodeType.Element)
					{
						base.UnknownNode(obj, "");
					}
					else
					{
						base.UnknownNode(obj, "");
					}
					base.Reader.MoveToContent();
					base.CheckReaderCount(ref num2, ref readerCount2);
				}
				base.ReadEndElement();
				return obj;
			}
		}

		private LogComparisonOperator Read22_LogComparisonOperator(string s)
		{
			switch (s)
			{
			case "Equals":
				return LogComparisonOperator.Equals;
			case "NotEquals":
				return LogComparisonOperator.NotEquals;
			case "LessThan":
				return LogComparisonOperator.LessThan;
			case "GreaterThan":
				return LogComparisonOperator.GreaterThan;
			case "LessOrEquals":
				return LogComparisonOperator.LessOrEquals;
			case "GreaterOrEquals":
				return LogComparisonOperator.GreaterOrEquals;
			}
			throw base.CreateUnknownConstantException(s, typeof(LogComparisonOperator));
		}

		private LogUnaryOperatorCondition Read10_LogUnaryOperatorCondition(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id21_LogUnaryOperatorCondition || xmlQualifiedName.Namespace != this.id2_Item))
			{
				if (xmlQualifiedName.Name == this.id22_IsNullOrEmpty && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read9_LogIsNullOrEmptyCondition(isNullable, false);
				}
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			else
			{
				if (flag)
				{
					return null;
				}
				throw base.CreateAbstractTypeException("LogUnaryOperatorCondition", "");
			}
		}

		private LogTrueCondition Read11_LogTrueCondition(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id20_True || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			LogTrueCondition logTrueCondition = new LogTrueCondition();
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(logTrueCondition);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return logTrueCondition;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					base.UnknownNode(logTrueCondition, "");
				}
				else
				{
					base.UnknownNode(logTrueCondition, "");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return logTrueCondition;
		}

		private LogForEveryCondition Read12_LogForEveryCondition(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id19_Every || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			LogForEveryCondition logForEveryCondition = new LogForEveryCondition();
			bool[] array = new bool[3];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(logForEveryCondition);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return logForEveryCondition;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[0] && base.Reader.LocalName == this.id27_Condition && base.Reader.NamespaceURI == this.id2_Item)
					{
						logForEveryCondition.Condition = this.Read26_LogCondition(false, true);
						array[0] = true;
					}
					else if (!array[1] && base.Reader.LocalName == this.id30_Field && base.Reader.NamespaceURI == this.id2_Item)
					{
						logForEveryCondition.Field = this.Read7_LogConditionField(false, true);
						array[1] = true;
					}
					else if (!array[2] && base.Reader.LocalName == this.id32_Variable && base.Reader.NamespaceURI == this.id2_Item)
					{
						logForEveryCondition.Variable = this.Read5_LogConditionVariable(false, true);
						array[2] = true;
					}
					else
					{
						base.UnknownNode(logForEveryCondition, ":Condition, :Field, :Variable");
					}
				}
				else
				{
					base.UnknownNode(logForEveryCondition, ":Condition, :Field, :Variable");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return logForEveryCondition;
		}

		private LogConditionField Read7_LogConditionField(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id30_Field || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			LogConditionField logConditionField = new LogConditionField();
			bool[] array = new bool[1];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(logConditionField);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return logConditionField;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[0] && base.Reader.LocalName == this.id33_Name && base.Reader.NamespaceURI == this.id2_Item)
					{
						logConditionField.Name = base.Reader.ReadElementString();
						array[0] = true;
					}
					else
					{
						base.UnknownNode(logConditionField, ":Name");
					}
				}
				else
				{
					base.UnknownNode(logConditionField, ":Name");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return logConditionField;
		}

		private LogForAnyCondition Read13_LogForAnyCondition(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id18_Any || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			LogForAnyCondition logForAnyCondition = new LogForAnyCondition();
			bool[] array = new bool[3];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(logForAnyCondition);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return logForAnyCondition;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[0] && base.Reader.LocalName == this.id27_Condition && base.Reader.NamespaceURI == this.id2_Item)
					{
						logForAnyCondition.Condition = this.Read26_LogCondition(false, true);
						array[0] = true;
					}
					else if (!array[1] && base.Reader.LocalName == this.id30_Field && base.Reader.NamespaceURI == this.id2_Item)
					{
						logForAnyCondition.Field = this.Read7_LogConditionField(false, true);
						array[1] = true;
					}
					else if (!array[2] && base.Reader.LocalName == this.id32_Variable && base.Reader.NamespaceURI == this.id2_Item)
					{
						logForAnyCondition.Variable = this.Read5_LogConditionVariable(false, true);
						array[2] = true;
					}
					else
					{
						base.UnknownNode(logForAnyCondition, ":Condition, :Field, :Variable");
					}
				}
				else
				{
					base.UnknownNode(logForAnyCondition, ":Condition, :Field, :Variable");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return logForAnyCondition;
		}

		private LogQuantifierCondition Read14_LogQuantifierCondition(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id17_LogQuantifierCondition || xmlQualifiedName.Namespace != this.id2_Item))
			{
				if (xmlQualifiedName.Name == this.id18_Any && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read13_LogForAnyCondition(isNullable, false);
				}
				if (xmlQualifiedName.Name == this.id19_Every && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read12_LogForEveryCondition(isNullable, false);
				}
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			else
			{
				if (flag)
				{
					return null;
				}
				throw base.CreateAbstractTypeException("LogQuantifierCondition", "");
			}
		}

		private LogNotCondition Read15_LogNotCondition(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id16_Not || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			LogNotCondition logNotCondition = new LogNotCondition();
			bool[] array = new bool[1];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(logNotCondition);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return logNotCondition;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[0] && base.Reader.LocalName == this.id27_Condition && base.Reader.NamespaceURI == this.id2_Item)
					{
						logNotCondition.Condition = this.Read26_LogCondition(false, true);
						array[0] = true;
					}
					else
					{
						base.UnknownNode(logNotCondition, ":Condition");
					}
				}
				else
				{
					base.UnknownNode(logNotCondition, ":Condition");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return logNotCondition;
		}

		private LogUnaryCondition Read16_LogUnaryCondition(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id15_LogUnaryCondition || xmlQualifiedName.Namespace != this.id2_Item))
			{
				if (xmlQualifiedName.Name == this.id16_Not && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read15_LogNotCondition(isNullable, false);
				}
				if (xmlQualifiedName.Name == this.id17_LogQuantifierCondition && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read14_LogQuantifierCondition(isNullable, false);
				}
				if (xmlQualifiedName.Name == this.id18_Any && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read13_LogForAnyCondition(isNullable, false);
				}
				if (xmlQualifiedName.Name == this.id19_Every && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read12_LogForEveryCondition(isNullable, false);
				}
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			else
			{
				if (flag)
				{
					return null;
				}
				throw base.CreateAbstractTypeException("LogUnaryCondition", "");
			}
		}

		private LogFalseCondition Read17_LogFalseCondition(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id14_False || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			LogFalseCondition logFalseCondition = new LogFalseCondition();
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(logFalseCondition);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return logFalseCondition;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					base.UnknownNode(logFalseCondition, "");
				}
				else
				{
					base.UnknownNode(logFalseCondition, "");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return logFalseCondition;
		}

		private LogStringStartsWithCondition Read18_LogStringStartsWithCondition(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id13_StartsWith || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			LogStringStartsWithCondition logStringStartsWithCondition = new LogStringStartsWithCondition();
			bool[] array = new bool[3];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(logStringStartsWithCondition);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return logStringStartsWithCondition;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[0] && base.Reader.LocalName == this.id37_Left && base.Reader.NamespaceURI == this.id2_Item)
					{
						logStringStartsWithCondition.Left = this.Read8_LogConditionOperand(false, true);
						array[0] = true;
					}
					else if (!array[1] && base.Reader.LocalName == this.id38_Right && base.Reader.NamespaceURI == this.id2_Item)
					{
						logStringStartsWithCondition.Right = this.Read8_LogConditionOperand(false, true);
						array[1] = true;
					}
					else if (!array[2] && base.Reader.LocalName == this.id39_IgnoreCase && base.Reader.NamespaceURI == this.id2_Item)
					{
						logStringStartsWithCondition.IgnoreCase = XmlConvert.ToBoolean(base.Reader.ReadElementString());
						array[2] = true;
					}
					else
					{
						base.UnknownNode(logStringStartsWithCondition, ":Left, :Right, :IgnoreCase");
					}
				}
				else
				{
					base.UnknownNode(logStringStartsWithCondition, ":Left, :Right, :IgnoreCase");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return logStringStartsWithCondition;
		}

		private LogStringEndsWithCondition Read19_LogStringEndsWithCondition(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id12_EndsWith || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			LogStringEndsWithCondition logStringEndsWithCondition = new LogStringEndsWithCondition();
			bool[] array = new bool[3];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(logStringEndsWithCondition);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return logStringEndsWithCondition;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[0] && base.Reader.LocalName == this.id37_Left && base.Reader.NamespaceURI == this.id2_Item)
					{
						logStringEndsWithCondition.Left = this.Read8_LogConditionOperand(false, true);
						array[0] = true;
					}
					else if (!array[1] && base.Reader.LocalName == this.id38_Right && base.Reader.NamespaceURI == this.id2_Item)
					{
						logStringEndsWithCondition.Right = this.Read8_LogConditionOperand(false, true);
						array[1] = true;
					}
					else if (!array[2] && base.Reader.LocalName == this.id39_IgnoreCase && base.Reader.NamespaceURI == this.id2_Item)
					{
						logStringEndsWithCondition.IgnoreCase = XmlConvert.ToBoolean(base.Reader.ReadElementString());
						array[2] = true;
					}
					else
					{
						base.UnknownNode(logStringEndsWithCondition, ":Left, :Right, :IgnoreCase");
					}
				}
				else
				{
					base.UnknownNode(logStringEndsWithCondition, ":Left, :Right, :IgnoreCase");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return logStringEndsWithCondition;
		}

		private LogStringContainsCondition Read20_LogStringContainsCondition(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id11_Contains || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			LogStringContainsCondition logStringContainsCondition = new LogStringContainsCondition();
			bool[] array = new bool[3];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(logStringContainsCondition);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return logStringContainsCondition;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[0] && base.Reader.LocalName == this.id37_Left && base.Reader.NamespaceURI == this.id2_Item)
					{
						logStringContainsCondition.Left = this.Read8_LogConditionOperand(false, true);
						array[0] = true;
					}
					else if (!array[1] && base.Reader.LocalName == this.id38_Right && base.Reader.NamespaceURI == this.id2_Item)
					{
						logStringContainsCondition.Right = this.Read8_LogConditionOperand(false, true);
						array[1] = true;
					}
					else if (!array[2] && base.Reader.LocalName == this.id39_IgnoreCase && base.Reader.NamespaceURI == this.id2_Item)
					{
						logStringContainsCondition.IgnoreCase = XmlConvert.ToBoolean(base.Reader.ReadElementString());
						array[2] = true;
					}
					else
					{
						base.UnknownNode(logStringContainsCondition, ":Left, :Right, :IgnoreCase");
					}
				}
				else
				{
					base.UnknownNode(logStringContainsCondition, ":Left, :Right, :IgnoreCase");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return logStringContainsCondition;
		}

		private LogBinaryStringOperatorCondition Read21_Item(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id10_Item || xmlQualifiedName.Namespace != this.id2_Item))
			{
				if (xmlQualifiedName.Name == this.id11_Contains && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read20_LogStringContainsCondition(isNullable, false);
				}
				if (xmlQualifiedName.Name == this.id12_EndsWith && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read19_LogStringEndsWithCondition(isNullable, false);
				}
				if (xmlQualifiedName.Name == this.id13_StartsWith && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read18_LogStringStartsWithCondition(isNullable, false);
				}
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			else
			{
				if (flag)
				{
					return null;
				}
				throw base.CreateAbstractTypeException("LogBinaryStringOperatorCondition", "");
			}
		}

		private LogStringComparisonCondition Read23_LogStringComparisonCondition(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id9_StringCompare || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			LogStringComparisonCondition logStringComparisonCondition = new LogStringComparisonCondition();
			bool[] array = new bool[4];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(logStringComparisonCondition);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return logStringComparisonCondition;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[0] && base.Reader.LocalName == this.id37_Left && base.Reader.NamespaceURI == this.id2_Item)
					{
						logStringComparisonCondition.Left = this.Read8_LogConditionOperand(false, true);
						array[0] = true;
					}
					else if (!array[1] && base.Reader.LocalName == this.id38_Right && base.Reader.NamespaceURI == this.id2_Item)
					{
						logStringComparisonCondition.Right = this.Read8_LogConditionOperand(false, true);
						array[1] = true;
					}
					else if (!array[2] && base.Reader.LocalName == this.id40_Operator && base.Reader.NamespaceURI == this.id2_Item)
					{
						logStringComparisonCondition.Operator = this.Read22_LogComparisonOperator(base.Reader.ReadElementString());
						array[2] = true;
					}
					else if (!array[3] && base.Reader.LocalName == this.id39_IgnoreCase && base.Reader.NamespaceURI == this.id2_Item)
					{
						logStringComparisonCondition.IgnoreCase = XmlConvert.ToBoolean(base.Reader.ReadElementString());
						array[3] = true;
					}
					else
					{
						base.UnknownNode(logStringComparisonCondition, ":Left, :Right, :Operator, :IgnoreCase");
					}
				}
				else
				{
					base.UnknownNode(logStringComparisonCondition, ":Left, :Right, :Operator, :IgnoreCase");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return logStringComparisonCondition;
		}

		private LogComparisonCondition Read24_LogComparisonCondition(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id8_Compare || xmlQualifiedName.Namespace != this.id2_Item))
			{
				if (xmlQualifiedName.Name == this.id9_StringCompare && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read23_LogStringComparisonCondition(isNullable, false);
				}
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			else
			{
				if (flag)
				{
					return null;
				}
				LogComparisonCondition logComparisonCondition = new LogComparisonCondition();
				bool[] array = new bool[3];
				while (base.Reader.MoveToNextAttribute())
				{
					if (!base.IsXmlnsAttribute(base.Reader.Name))
					{
						base.UnknownNode(logComparisonCondition);
					}
				}
				base.Reader.MoveToElement();
				if (base.Reader.IsEmptyElement)
				{
					base.Reader.Skip();
					return logComparisonCondition;
				}
				base.Reader.ReadStartElement();
				base.Reader.MoveToContent();
				int num = 0;
				int readerCount = base.ReaderCount;
				while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
				{
					if (base.Reader.NodeType == XmlNodeType.Element)
					{
						if (!array[0] && base.Reader.LocalName == this.id37_Left && base.Reader.NamespaceURI == this.id2_Item)
						{
							logComparisonCondition.Left = this.Read8_LogConditionOperand(false, true);
							array[0] = true;
						}
						else if (!array[1] && base.Reader.LocalName == this.id38_Right && base.Reader.NamespaceURI == this.id2_Item)
						{
							logComparisonCondition.Right = this.Read8_LogConditionOperand(false, true);
							array[1] = true;
						}
						else if (!array[2] && base.Reader.LocalName == this.id40_Operator && base.Reader.NamespaceURI == this.id2_Item)
						{
							logComparisonCondition.Operator = this.Read22_LogComparisonOperator(base.Reader.ReadElementString());
							array[2] = true;
						}
						else
						{
							base.UnknownNode(logComparisonCondition, ":Left, :Right, :Operator");
						}
					}
					else
					{
						base.UnknownNode(logComparisonCondition, ":Left, :Right, :Operator");
					}
					base.Reader.MoveToContent();
					base.CheckReaderCount(ref num, ref readerCount);
				}
				base.ReadEndElement();
				return logComparisonCondition;
			}
		}

		private LogBinaryOperatorCondition Read25_LogBinaryOperatorCondition(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id7_LogBinaryOperatorCondition || xmlQualifiedName.Namespace != this.id2_Item))
			{
				if (xmlQualifiedName.Name == this.id8_Compare && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read24_LogComparisonCondition(isNullable, false);
				}
				if (xmlQualifiedName.Name == this.id9_StringCompare && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read23_LogStringComparisonCondition(isNullable, false);
				}
				if (xmlQualifiedName.Name == this.id10_Item && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read21_Item(isNullable, false);
				}
				if (xmlQualifiedName.Name == this.id11_Contains && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read20_LogStringContainsCondition(isNullable, false);
				}
				if (xmlQualifiedName.Name == this.id12_EndsWith && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read19_LogStringEndsWithCondition(isNullable, false);
				}
				if (xmlQualifiedName.Name == this.id13_StartsWith && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read18_LogStringStartsWithCondition(isNullable, false);
				}
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			else
			{
				if (flag)
				{
					return null;
				}
				throw base.CreateAbstractTypeException("LogBinaryOperatorCondition", "");
			}
		}

		protected override void InitCallbacks()
		{
		}

		protected override void InitIDs()
		{
			this.id2_Item = base.Reader.NameTable.Add("");
			this.id36_LogComparisonOperator = base.Reader.NameTable.Add("LogComparisonOperator");
			this.id8_Compare = base.Reader.NameTable.Add("Compare");
			this.id35_ArrayOfLogCondition = base.Reader.NameTable.Add("ArrayOfLogCondition");
			this.id19_Every = base.Reader.NameTable.Add("Every");
			this.id20_True = base.Reader.NameTable.Add("True");
			this.id17_LogQuantifierCondition = base.Reader.NameTable.Add("LogQuantifierCondition");
			this.id24_And = base.Reader.NameTable.Add("And");
			this.id7_LogBinaryOperatorCondition = base.Reader.NameTable.Add("LogBinaryOperatorCondition");
			this.id1_LogQuery = base.Reader.NameTable.Add("LogQuery");
			this.id13_StartsWith = base.Reader.NameTable.Add("StartsWith");
			this.id14_False = base.Reader.NameTable.Add("False");
			this.id5_Filter = base.Reader.NameTable.Add("Filter");
			this.id29_LogConditionOperand = base.Reader.NameTable.Add("LogConditionOperand");
			this.id27_Condition = base.Reader.NameTable.Add("Condition");
			this.id31_Constant = base.Reader.NameTable.Add("Constant");
			this.id38_Right = base.Reader.NameTable.Add("Right");
			this.id3_Beginning = base.Reader.NameTable.Add("Beginning");
			this.id40_Operator = base.Reader.NameTable.Add("Operator");
			this.id33_Name = base.Reader.NameTable.Add("Name");
			this.id10_Item = base.Reader.NameTable.Add("LogBinaryStringOperatorCondition");
			this.id32_Variable = base.Reader.NameTable.Add("Variable");
			this.id39_IgnoreCase = base.Reader.NameTable.Add("IgnoreCase");
			this.id21_LogUnaryOperatorCondition = base.Reader.NameTable.Add("LogUnaryOperatorCondition");
			this.id11_Contains = base.Reader.NameTable.Add("Contains");
			this.id30_Field = base.Reader.NameTable.Add("Field");
			this.id23_LogCompoundCondition = base.Reader.NameTable.Add("LogCompoundCondition");
			this.id6_LogCondition = base.Reader.NameTable.Add("LogCondition");
			this.id25_Or = base.Reader.NameTable.Add("Or");
			this.id34_Value = base.Reader.NameTable.Add("Value");
			this.id18_Any = base.Reader.NameTable.Add("Any");
			this.id28_Operand = base.Reader.NameTable.Add("Operand");
			this.id15_LogUnaryCondition = base.Reader.NameTable.Add("LogUnaryCondition");
			this.id16_Not = base.Reader.NameTable.Add("Not");
			this.id12_EndsWith = base.Reader.NameTable.Add("EndsWith");
			this.id37_Left = base.Reader.NameTable.Add("Left");
			this.id9_StringCompare = base.Reader.NameTable.Add("StringCompare");
			this.id22_IsNullOrEmpty = base.Reader.NameTable.Add("IsNullOrEmpty");
			this.id26_Conditions = base.Reader.NameTable.Add("Conditions");
			this.id4_End = base.Reader.NameTable.Add("End");
		}

		private string id2_Item;

		private string id36_LogComparisonOperator;

		private string id8_Compare;

		private string id35_ArrayOfLogCondition;

		private string id19_Every;

		private string id20_True;

		private string id17_LogQuantifierCondition;

		private string id24_And;

		private string id7_LogBinaryOperatorCondition;

		private string id1_LogQuery;

		private string id13_StartsWith;

		private string id14_False;

		private string id5_Filter;

		private string id29_LogConditionOperand;

		private string id27_Condition;

		private string id31_Constant;

		private string id38_Right;

		private string id3_Beginning;

		private string id40_Operator;

		private string id33_Name;

		private string id10_Item;

		private string id32_Variable;

		private string id39_IgnoreCase;

		private string id21_LogUnaryOperatorCondition;

		private string id11_Contains;

		private string id30_Field;

		private string id23_LogCompoundCondition;

		private string id6_LogCondition;

		private string id25_Or;

		private string id34_Value;

		private string id18_Any;

		private string id28_Operand;

		private string id15_LogUnaryCondition;

		private string id16_Not;

		private string id12_EndsWith;

		private string id37_Left;

		private string id9_StringCompare;

		private string id22_IsNullOrEmpty;

		private string id26_Conditions;

		private string id4_End;
	}
}
