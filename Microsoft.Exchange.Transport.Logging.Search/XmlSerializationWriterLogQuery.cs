using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Transport.Logging.Search
{
	internal class XmlSerializationWriterLogQuery : XmlSerializationWriter
	{
		public void Write28_LogQuery(object o)
		{
			base.WriteStartDocument();
			if (o == null)
			{
				base.WriteNullTagLiteral("LogQuery", "");
				return;
			}
			base.TopLevelElement();
			this.Write27_LogQuery("LogQuery", "", (LogQuery)o, true, false);
		}

		private void Write27_LogQuery(string n, string ns, LogQuery o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(LogQuery)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("LogQuery", "");
			}
			base.WriteElementStringRaw("Beginning", "", XmlSerializationWriter.FromDateTime(o.Beginning));
			base.WriteElementStringRaw("End", "", XmlSerializationWriter.FromDateTime(o.End));
			this.Write26_LogCondition("Filter", "", o.Filter, false, false);
			base.WriteEndElement(o);
		}

		private void Write26_LogCondition(string n, string ns, LogCondition o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (needType)
			{
				return;
			}
			Type type = o.GetType();
			if (type == typeof(LogCondition))
			{
				return;
			}
			if (type == typeof(LogBinaryOperatorCondition))
			{
				this.Write25_LogBinaryOperatorCondition(n, ns, (LogBinaryOperatorCondition)o, isNullable, true);
				return;
			}
			if (type == typeof(LogComparisonCondition))
			{
				this.Write24_LogComparisonCondition(n, ns, (LogComparisonCondition)o, isNullable, true);
				return;
			}
			if (type == typeof(LogStringComparisonCondition))
			{
				this.Write23_LogStringComparisonCondition(n, ns, (LogStringComparisonCondition)o, isNullable, true);
				return;
			}
			if (type == typeof(LogBinaryStringOperatorCondition))
			{
				this.Write21_Item(n, ns, (LogBinaryStringOperatorCondition)o, isNullable, true);
				return;
			}
			if (type == typeof(LogStringContainsCondition))
			{
				this.Write20_LogStringContainsCondition(n, ns, (LogStringContainsCondition)o, isNullable, true);
				return;
			}
			if (type == typeof(LogStringEndsWithCondition))
			{
				this.Write19_LogStringEndsWithCondition(n, ns, (LogStringEndsWithCondition)o, isNullable, true);
				return;
			}
			if (type == typeof(LogStringStartsWithCondition))
			{
				this.Write18_LogStringStartsWithCondition(n, ns, (LogStringStartsWithCondition)o, isNullable, true);
				return;
			}
			if (type == typeof(LogFalseCondition))
			{
				this.Write17_LogFalseCondition(n, ns, (LogFalseCondition)o, isNullable, true);
				return;
			}
			if (type == typeof(LogUnaryCondition))
			{
				this.Write16_LogUnaryCondition(n, ns, (LogUnaryCondition)o, isNullable, true);
				return;
			}
			if (type == typeof(LogNotCondition))
			{
				this.Write15_LogNotCondition(n, ns, (LogNotCondition)o, isNullable, true);
				return;
			}
			if (type == typeof(LogQuantifierCondition))
			{
				this.Write14_LogQuantifierCondition(n, ns, (LogQuantifierCondition)o, isNullable, true);
				return;
			}
			if (type == typeof(LogForAnyCondition))
			{
				this.Write13_LogForAnyCondition(n, ns, (LogForAnyCondition)o, isNullable, true);
				return;
			}
			if (type == typeof(LogForEveryCondition))
			{
				this.Write12_LogForEveryCondition(n, ns, (LogForEveryCondition)o, isNullable, true);
				return;
			}
			if (type == typeof(LogTrueCondition))
			{
				this.Write11_LogTrueCondition(n, ns, (LogTrueCondition)o, isNullable, true);
				return;
			}
			if (type == typeof(LogUnaryOperatorCondition))
			{
				this.Write10_LogUnaryOperatorCondition(n, ns, (LogUnaryOperatorCondition)o, isNullable, true);
				return;
			}
			if (type == typeof(LogIsNullOrEmptyCondition))
			{
				this.Write9_LogIsNullOrEmptyCondition(n, ns, (LogIsNullOrEmptyCondition)o, isNullable, true);
				return;
			}
			if (type == typeof(LogCompoundCondition))
			{
				this.Write4_LogCompoundCondition(n, ns, (LogCompoundCondition)o, isNullable, true);
				return;
			}
			if (type == typeof(LogAndCondition))
			{
				this.Write3_LogAndCondition(n, ns, (LogAndCondition)o, isNullable, true);
				return;
			}
			if (type == typeof(LogOrCondition))
			{
				this.Write2_LogOrCondition(n, ns, (LogOrCondition)o, isNullable, true);
				return;
			}
			throw base.CreateUnknownTypeException(o);
		}

		private void Write2_LogOrCondition(string n, string ns, LogOrCondition o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(LogOrCondition)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("Or", "");
			}
			List<LogCondition> conditions = o.Conditions;
			if (conditions != null)
			{
				base.WriteStartElement("Conditions", "", null, false);
				for (int i = 0; i < ((ICollection)conditions).Count; i++)
				{
					this.Write26_LogCondition("Condition", "", conditions[i], true, false);
				}
				base.WriteEndElement();
			}
			base.WriteEndElement(o);
		}

		private void Write3_LogAndCondition(string n, string ns, LogAndCondition o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(LogAndCondition)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("And", "");
			}
			List<LogCondition> conditions = o.Conditions;
			if (conditions != null)
			{
				base.WriteStartElement("Conditions", "", null, false);
				for (int i = 0; i < ((ICollection)conditions).Count; i++)
				{
					this.Write26_LogCondition("Condition", "", conditions[i], true, false);
				}
				base.WriteEndElement();
			}
			base.WriteEndElement(o);
		}

		private void Write4_LogCompoundCondition(string n, string ns, LogCompoundCondition o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (needType)
			{
				return;
			}
			Type type = o.GetType();
			if (type == typeof(LogCompoundCondition))
			{
				return;
			}
			if (type == typeof(LogAndCondition))
			{
				this.Write3_LogAndCondition(n, ns, (LogAndCondition)o, isNullable, true);
				return;
			}
			if (type == typeof(LogOrCondition))
			{
				this.Write2_LogOrCondition(n, ns, (LogOrCondition)o, isNullable, true);
				return;
			}
			throw base.CreateUnknownTypeException(o);
		}

		private void Write9_LogIsNullOrEmptyCondition(string n, string ns, LogIsNullOrEmptyCondition o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(LogIsNullOrEmptyCondition)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("IsNullOrEmpty", "");
			}
			this.Write8_LogConditionOperand("Operand", "", o.Operand, false, false);
			base.WriteEndElement(o);
		}

		private void Write8_LogConditionOperand(string n, string ns, LogConditionOperand o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (needType)
			{
				return;
			}
			Type type = o.GetType();
			if (type == typeof(LogConditionOperand))
			{
				return;
			}
			if (type == typeof(LogConditionField))
			{
				this.Write7_LogConditionField(n, ns, (LogConditionField)o, isNullable, true);
				return;
			}
			if (type == typeof(LogConditionConstant))
			{
				this.Write6_LogConditionConstant(n, ns, (LogConditionConstant)o, isNullable, true);
				return;
			}
			if (type == typeof(LogConditionVariable))
			{
				this.Write5_LogConditionVariable(n, ns, (LogConditionVariable)o, isNullable, true);
				return;
			}
			throw base.CreateUnknownTypeException(o);
		}

		private void Write5_LogConditionVariable(string n, string ns, LogConditionVariable o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(LogConditionVariable)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("Variable", "");
			}
			base.WriteElementString("Name", "", o.Name);
			base.WriteEndElement(o);
		}

		private void Write6_LogConditionConstant(string n, string ns, LogConditionConstant o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(LogConditionConstant)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("Constant", "");
			}
			this.Write1_Object("Value", "", o.Value, false, false);
			base.WriteEndElement(o);
		}

		private void Write1_Object(string n, string ns, object o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(object)))
				{
					if (type == typeof(LogQuery))
					{
						this.Write27_LogQuery(n, ns, (LogQuery)o, isNullable, true);
						return;
					}
					if (type == typeof(LogConditionOperand))
					{
						this.Write8_LogConditionOperand(n, ns, (LogConditionOperand)o, isNullable, true);
						return;
					}
					if (type == typeof(LogConditionField))
					{
						this.Write7_LogConditionField(n, ns, (LogConditionField)o, isNullable, true);
						return;
					}
					if (type == typeof(LogConditionConstant))
					{
						this.Write6_LogConditionConstant(n, ns, (LogConditionConstant)o, isNullable, true);
						return;
					}
					if (type == typeof(LogConditionVariable))
					{
						this.Write5_LogConditionVariable(n, ns, (LogConditionVariable)o, isNullable, true);
						return;
					}
					if (type == typeof(LogCondition))
					{
						this.Write26_LogCondition(n, ns, (LogCondition)o, isNullable, true);
						return;
					}
					if (type == typeof(LogBinaryOperatorCondition))
					{
						this.Write25_LogBinaryOperatorCondition(n, ns, (LogBinaryOperatorCondition)o, isNullable, true);
						return;
					}
					if (type == typeof(LogComparisonCondition))
					{
						this.Write24_LogComparisonCondition(n, ns, (LogComparisonCondition)o, isNullable, true);
						return;
					}
					if (type == typeof(LogStringComparisonCondition))
					{
						this.Write23_LogStringComparisonCondition(n, ns, (LogStringComparisonCondition)o, isNullable, true);
						return;
					}
					if (type == typeof(LogBinaryStringOperatorCondition))
					{
						this.Write21_Item(n, ns, (LogBinaryStringOperatorCondition)o, isNullable, true);
						return;
					}
					if (type == typeof(LogStringContainsCondition))
					{
						this.Write20_LogStringContainsCondition(n, ns, (LogStringContainsCondition)o, isNullable, true);
						return;
					}
					if (type == typeof(LogStringEndsWithCondition))
					{
						this.Write19_LogStringEndsWithCondition(n, ns, (LogStringEndsWithCondition)o, isNullable, true);
						return;
					}
					if (type == typeof(LogStringStartsWithCondition))
					{
						this.Write18_LogStringStartsWithCondition(n, ns, (LogStringStartsWithCondition)o, isNullable, true);
						return;
					}
					if (type == typeof(LogFalseCondition))
					{
						this.Write17_LogFalseCondition(n, ns, (LogFalseCondition)o, isNullable, true);
						return;
					}
					if (type == typeof(LogUnaryCondition))
					{
						this.Write16_LogUnaryCondition(n, ns, (LogUnaryCondition)o, isNullable, true);
						return;
					}
					if (type == typeof(LogNotCondition))
					{
						this.Write15_LogNotCondition(n, ns, (LogNotCondition)o, isNullable, true);
						return;
					}
					if (type == typeof(LogQuantifierCondition))
					{
						this.Write14_LogQuantifierCondition(n, ns, (LogQuantifierCondition)o, isNullable, true);
						return;
					}
					if (type == typeof(LogForAnyCondition))
					{
						this.Write13_LogForAnyCondition(n, ns, (LogForAnyCondition)o, isNullable, true);
						return;
					}
					if (type == typeof(LogForEveryCondition))
					{
						this.Write12_LogForEveryCondition(n, ns, (LogForEveryCondition)o, isNullable, true);
						return;
					}
					if (type == typeof(LogTrueCondition))
					{
						this.Write11_LogTrueCondition(n, ns, (LogTrueCondition)o, isNullable, true);
						return;
					}
					if (type == typeof(LogUnaryOperatorCondition))
					{
						this.Write10_LogUnaryOperatorCondition(n, ns, (LogUnaryOperatorCondition)o, isNullable, true);
						return;
					}
					if (type == typeof(LogIsNullOrEmptyCondition))
					{
						this.Write9_LogIsNullOrEmptyCondition(n, ns, (LogIsNullOrEmptyCondition)o, isNullable, true);
						return;
					}
					if (type == typeof(LogCompoundCondition))
					{
						this.Write4_LogCompoundCondition(n, ns, (LogCompoundCondition)o, isNullable, true);
						return;
					}
					if (type == typeof(LogAndCondition))
					{
						this.Write3_LogAndCondition(n, ns, (LogAndCondition)o, isNullable, true);
						return;
					}
					if (type == typeof(LogOrCondition))
					{
						this.Write2_LogOrCondition(n, ns, (LogOrCondition)o, isNullable, true);
						return;
					}
					if (type == typeof(List<LogCondition>))
					{
						base.Writer.WriteStartElement(n, ns);
						base.WriteXsiType("ArrayOfLogCondition", "");
						List<LogCondition> list = (List<LogCondition>)o;
						if (list != null)
						{
							for (int i = 0; i < ((ICollection)list).Count; i++)
							{
								this.Write26_LogCondition("Condition", "", list[i], true, false);
							}
						}
						base.Writer.WriteEndElement();
						return;
					}
					if (type == typeof(LogComparisonOperator))
					{
						base.Writer.WriteStartElement(n, ns);
						base.WriteXsiType("LogComparisonOperator", "");
						base.Writer.WriteString(this.Write22_LogComparisonOperator((LogComparisonOperator)o));
						base.Writer.WriteEndElement();
						return;
					}
					base.WriteTypedPrimitive(n, ns, o, true);
					return;
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			base.WriteEndElement(o);
		}

		private string Write22_LogComparisonOperator(LogComparisonOperator v)
		{
			string result;
			switch (v)
			{
			case LogComparisonOperator.Equals:
				result = "Equals";
				break;
			case LogComparisonOperator.NotEquals:
				result = "NotEquals";
				break;
			case LogComparisonOperator.LessThan:
				result = "LessThan";
				break;
			case LogComparisonOperator.GreaterThan:
				result = "GreaterThan";
				break;
			case LogComparisonOperator.LessOrEquals:
				result = "LessOrEquals";
				break;
			case LogComparisonOperator.GreaterOrEquals:
				result = "GreaterOrEquals";
				break;
			default:
				throw base.CreateInvalidEnumValueException(((long)v).ToString(CultureInfo.InvariantCulture), "Microsoft.Exchange.Transport.Logging.Search.LogComparisonOperator");
			}
			return result;
		}

		private void Write10_LogUnaryOperatorCondition(string n, string ns, LogUnaryOperatorCondition o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (needType)
			{
				return;
			}
			Type type = o.GetType();
			if (type == typeof(LogUnaryOperatorCondition))
			{
				return;
			}
			if (type == typeof(LogIsNullOrEmptyCondition))
			{
				this.Write9_LogIsNullOrEmptyCondition(n, ns, (LogIsNullOrEmptyCondition)o, isNullable, true);
				return;
			}
			throw base.CreateUnknownTypeException(o);
		}

		private void Write11_LogTrueCondition(string n, string ns, LogTrueCondition o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(LogTrueCondition)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("True", "");
			}
			base.WriteEndElement(o);
		}

		private void Write12_LogForEveryCondition(string n, string ns, LogForEveryCondition o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(LogForEveryCondition)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("Every", "");
			}
			this.Write26_LogCondition("Condition", "", o.Condition, false, false);
			this.Write7_LogConditionField("Field", "", o.Field, false, false);
			this.Write5_LogConditionVariable("Variable", "", o.Variable, false, false);
			base.WriteEndElement(o);
		}

		private void Write7_LogConditionField(string n, string ns, LogConditionField o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(LogConditionField)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("Field", "");
			}
			base.WriteElementString("Name", "", o.Name);
			base.WriteEndElement(o);
		}

		private void Write13_LogForAnyCondition(string n, string ns, LogForAnyCondition o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(LogForAnyCondition)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("Any", "");
			}
			this.Write26_LogCondition("Condition", "", o.Condition, false, false);
			this.Write7_LogConditionField("Field", "", o.Field, false, false);
			this.Write5_LogConditionVariable("Variable", "", o.Variable, false, false);
			base.WriteEndElement(o);
		}

		private void Write14_LogQuantifierCondition(string n, string ns, LogQuantifierCondition o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (needType)
			{
				return;
			}
			Type type = o.GetType();
			if (type == typeof(LogQuantifierCondition))
			{
				return;
			}
			if (type == typeof(LogForAnyCondition))
			{
				this.Write13_LogForAnyCondition(n, ns, (LogForAnyCondition)o, isNullable, true);
				return;
			}
			if (type == typeof(LogForEveryCondition))
			{
				this.Write12_LogForEveryCondition(n, ns, (LogForEveryCondition)o, isNullable, true);
				return;
			}
			throw base.CreateUnknownTypeException(o);
		}

		private void Write15_LogNotCondition(string n, string ns, LogNotCondition o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(LogNotCondition)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("Not", "");
			}
			this.Write26_LogCondition("Condition", "", o.Condition, false, false);
			base.WriteEndElement(o);
		}

		private void Write16_LogUnaryCondition(string n, string ns, LogUnaryCondition o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (needType)
			{
				return;
			}
			Type type = o.GetType();
			if (type == typeof(LogUnaryCondition))
			{
				return;
			}
			if (type == typeof(LogNotCondition))
			{
				this.Write15_LogNotCondition(n, ns, (LogNotCondition)o, isNullable, true);
				return;
			}
			if (type == typeof(LogQuantifierCondition))
			{
				this.Write14_LogQuantifierCondition(n, ns, (LogQuantifierCondition)o, isNullable, true);
				return;
			}
			if (type == typeof(LogForAnyCondition))
			{
				this.Write13_LogForAnyCondition(n, ns, (LogForAnyCondition)o, isNullable, true);
				return;
			}
			if (type == typeof(LogForEveryCondition))
			{
				this.Write12_LogForEveryCondition(n, ns, (LogForEveryCondition)o, isNullable, true);
				return;
			}
			throw base.CreateUnknownTypeException(o);
		}

		private void Write17_LogFalseCondition(string n, string ns, LogFalseCondition o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(LogFalseCondition)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("False", "");
			}
			base.WriteEndElement(o);
		}

		private void Write18_LogStringStartsWithCondition(string n, string ns, LogStringStartsWithCondition o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(LogStringStartsWithCondition)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("StartsWith", "");
			}
			this.Write8_LogConditionOperand("Left", "", o.Left, false, false);
			this.Write8_LogConditionOperand("Right", "", o.Right, false, false);
			base.WriteElementStringRaw("IgnoreCase", "", XmlConvert.ToString(o.IgnoreCase));
			base.WriteEndElement(o);
		}

		private void Write19_LogStringEndsWithCondition(string n, string ns, LogStringEndsWithCondition o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(LogStringEndsWithCondition)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("EndsWith", "");
			}
			this.Write8_LogConditionOperand("Left", "", o.Left, false, false);
			this.Write8_LogConditionOperand("Right", "", o.Right, false, false);
			base.WriteElementStringRaw("IgnoreCase", "", XmlConvert.ToString(o.IgnoreCase));
			base.WriteEndElement(o);
		}

		private void Write20_LogStringContainsCondition(string n, string ns, LogStringContainsCondition o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(LogStringContainsCondition)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("Contains", "");
			}
			this.Write8_LogConditionOperand("Left", "", o.Left, false, false);
			this.Write8_LogConditionOperand("Right", "", o.Right, false, false);
			base.WriteElementStringRaw("IgnoreCase", "", XmlConvert.ToString(o.IgnoreCase));
			base.WriteEndElement(o);
		}

		private void Write21_Item(string n, string ns, LogBinaryStringOperatorCondition o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (needType)
			{
				return;
			}
			Type type = o.GetType();
			if (type == typeof(LogBinaryStringOperatorCondition))
			{
				return;
			}
			if (type == typeof(LogStringContainsCondition))
			{
				this.Write20_LogStringContainsCondition(n, ns, (LogStringContainsCondition)o, isNullable, true);
				return;
			}
			if (type == typeof(LogStringEndsWithCondition))
			{
				this.Write19_LogStringEndsWithCondition(n, ns, (LogStringEndsWithCondition)o, isNullable, true);
				return;
			}
			if (type == typeof(LogStringStartsWithCondition))
			{
				this.Write18_LogStringStartsWithCondition(n, ns, (LogStringStartsWithCondition)o, isNullable, true);
				return;
			}
			throw base.CreateUnknownTypeException(o);
		}

		private void Write23_LogStringComparisonCondition(string n, string ns, LogStringComparisonCondition o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(LogStringComparisonCondition)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("StringCompare", "");
			}
			this.Write8_LogConditionOperand("Left", "", o.Left, false, false);
			this.Write8_LogConditionOperand("Right", "", o.Right, false, false);
			base.WriteElementString("Operator", "", this.Write22_LogComparisonOperator(o.Operator));
			base.WriteElementStringRaw("IgnoreCase", "", XmlConvert.ToString(o.IgnoreCase));
			base.WriteEndElement(o);
		}

		private void Write24_LogComparisonCondition(string n, string ns, LogComparisonCondition o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(LogComparisonCondition)))
				{
					if (type == typeof(LogStringComparisonCondition))
					{
						this.Write23_LogStringComparisonCondition(n, ns, (LogStringComparisonCondition)o, isNullable, true);
						return;
					}
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("Compare", "");
			}
			this.Write8_LogConditionOperand("Left", "", o.Left, false, false);
			this.Write8_LogConditionOperand("Right", "", o.Right, false, false);
			base.WriteElementString("Operator", "", this.Write22_LogComparisonOperator(o.Operator));
			base.WriteEndElement(o);
		}

		private void Write25_LogBinaryOperatorCondition(string n, string ns, LogBinaryOperatorCondition o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (needType)
			{
				return;
			}
			Type type = o.GetType();
			if (type == typeof(LogBinaryOperatorCondition))
			{
				return;
			}
			if (type == typeof(LogComparisonCondition))
			{
				this.Write24_LogComparisonCondition(n, ns, (LogComparisonCondition)o, isNullable, true);
				return;
			}
			if (type == typeof(LogStringComparisonCondition))
			{
				this.Write23_LogStringComparisonCondition(n, ns, (LogStringComparisonCondition)o, isNullable, true);
				return;
			}
			if (type == typeof(LogBinaryStringOperatorCondition))
			{
				this.Write21_Item(n, ns, (LogBinaryStringOperatorCondition)o, isNullable, true);
				return;
			}
			if (type == typeof(LogStringContainsCondition))
			{
				this.Write20_LogStringContainsCondition(n, ns, (LogStringContainsCondition)o, isNullable, true);
				return;
			}
			if (type == typeof(LogStringEndsWithCondition))
			{
				this.Write19_LogStringEndsWithCondition(n, ns, (LogStringEndsWithCondition)o, isNullable, true);
				return;
			}
			if (type == typeof(LogStringStartsWithCondition))
			{
				this.Write18_LogStringStartsWithCondition(n, ns, (LogStringStartsWithCondition)o, isNullable, true);
				return;
			}
			throw base.CreateUnknownTypeException(o);
		}

		protected override void InitCallbacks()
		{
		}
	}
}
