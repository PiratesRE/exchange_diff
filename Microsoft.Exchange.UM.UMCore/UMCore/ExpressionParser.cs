using System;
using System.Collections;
using System.Globalization;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class ExpressionParser
	{
		protected ExpressionParser(ExpressionParser.ILexer lexer, ExpressionParser.Op[] ops)
		{
			this.lexer = lexer;
			this.ops = (ExpressionParser.Op[])ops.Clone();
			foreach (ExpressionParser.Op op in this.ops)
			{
				ExpressionParser.LeftParen leftParen = op as ExpressionParser.LeftParen;
				if (leftParen != null)
				{
					this.leftParen = leftParen;
					break;
				}
			}
		}

		protected ActivityManagerConfig ManagerConfig
		{
			get
			{
				return this.managerConfig;
			}
			set
			{
				this.managerConfig = value;
			}
		}

		internal virtual ExpressionParser.Expression Parse(string exp, ActivityManagerConfig config)
		{
			this.ManagerConfig = config;
			ArrayList arrayList = new ArrayList(this.lexer.ToTokens(exp));
			Stack stack = new Stack();
			stack.Push(ExpressionParser.MarkerOp.Instance);
			while (arrayList.Count > 0)
			{
				object obj = this.ParseOp(arrayList);
				if (obj == null)
				{
					obj = this.ParseAtomicExpression(arrayList);
					if (obj == null)
					{
						throw new ExpressionSyntaxException(Strings.UnexpectedToken(((ExpressionParser.Token)arrayList[0]).Text));
					}
				}
				if (stack.Peek() is ExpressionParser.Expression)
				{
					ExpressionParser.BinaryOp binaryOp = obj as ExpressionParser.BinaryOp;
					if (binaryOp != null)
					{
						this.HandleBinaryOp(stack, binaryOp);
					}
					else if (obj is ExpressionParser.RightParen)
					{
						this.ReduceUntil(stack, this.leftParen);
					}
					else
					{
						if (obj is ExpressionParser.Expression)
						{
							throw new ExpressionSyntaxException(Strings.TwoExpressions);
						}
						if (obj is ExpressionParser.LeftParen)
						{
							throw new ExpressionSyntaxException(Strings.ExpressionLeftParen);
						}
						if (obj is ExpressionParser.UnaryOp)
						{
							throw new ExpressionSyntaxException(Strings.ExpressionUnaryOp);
						}
						throw new ExpressionSyntaxException(Strings.UnexpectedSymbol(obj.ToString()));
					}
				}
				else
				{
					if (obj is ExpressionParser.BinaryOp)
					{
						throw new ExpressionSyntaxException(Strings.OperatorBinaryOp);
					}
					if (obj is ExpressionParser.RightParen)
					{
						throw new ExpressionSyntaxException(Strings.OperatorRightParen);
					}
					stack.Push(obj);
				}
			}
			this.ReduceUntil(stack, ExpressionParser.MarkerOp.Instance);
			return stack.Peek() as ExpressionParser.Expression;
		}

		protected virtual ExpressionParser.Op ParseOp(ArrayList tokens)
		{
			if (tokens.Count > 0)
			{
				ExpressionParser.Token token = (ExpressionParser.Token)tokens[0];
				ExpressionParser.Op op = this.AsOp(token);
				if (op != null)
				{
					tokens.RemoveAt(0);
					return op;
				}
			}
			return null;
		}

		protected abstract ExpressionParser.AtomicExpression ParseAtomicExpression(ArrayList tokens);

		private static ExpressionParser.Action CompareOp(ExpressionParser.Op op1, ExpressionParser.BinaryOp op2)
		{
			if (op1 is ExpressionParser.MarkerOp || op1 is ExpressionParser.LeftParen)
			{
				return ExpressionParser.Action.Shift;
			}
			if (op1.Precedence > op2.Precedence)
			{
				return ExpressionParser.Action.Reduction;
			}
			if (op1.Precedence < op2.Precedence)
			{
				return ExpressionParser.Action.Shift;
			}
			ExpressionParser.BinaryOp binaryOp = op1 as ExpressionParser.BinaryOp;
			if (binaryOp != null)
			{
				if (binaryOp.Assoc == ExpressionParser.BinaryOp.Associativity.Left && op2.Assoc == ExpressionParser.BinaryOp.Associativity.Left)
				{
					return ExpressionParser.Action.Reduction;
				}
				if (binaryOp.Assoc == ExpressionParser.BinaryOp.Associativity.Right && op2.Assoc == ExpressionParser.BinaryOp.Associativity.Right)
				{
					return ExpressionParser.Action.Shift;
				}
			}
			return ExpressionParser.Action.Error;
		}

		private ExpressionParser.Op AsOp(ExpressionParser.Token token)
		{
			foreach (ExpressionParser.Op op in this.ops)
			{
				if (op.Matches(token.Text))
				{
					return op;
				}
			}
			return null;
		}

		private void Reduce(Stack stack, ExpressionParser.Expression e)
		{
			ExpressionParser.Op op = (ExpressionParser.Op)stack.Pop();
			ExpressionParser.UnaryOp unaryOp = op as ExpressionParser.UnaryOp;
			if (unaryOp != null)
			{
				stack.Push(new ExpressionParser.UnaryOpExpression(unaryOp, e));
				return;
			}
			ExpressionParser.BinaryOp binaryOp = op as ExpressionParser.BinaryOp;
			if (binaryOp != null)
			{
				ExpressionParser.Expression exp = (ExpressionParser.Expression)stack.Pop();
				stack.Push(new ExpressionParser.BinaryOpExpression(binaryOp, exp, e));
				return;
			}
			stack.Push(op);
			throw new ExpressionSyntaxException(Strings.InvalidOperator(op.ToString()));
		}

		private bool ReduceUnless(Stack stack, ExpressionParser.Op delim)
		{
			ExpressionParser.Expression expression = (ExpressionParser.Expression)stack.Pop();
			ExpressionParser.Op op = (ExpressionParser.Op)stack.Peek();
			if (op == delim)
			{
				stack.Push(expression);
				return false;
			}
			this.Reduce(stack, expression);
			return true;
		}

		private void HandleBinaryOp(Stack stack, ExpressionParser.BinaryOp bop)
		{
			ExpressionParser.Expression expression;
			for (;;)
			{
				expression = (ExpressionParser.Expression)stack.Pop();
				ExpressionParser.Op op = (ExpressionParser.Op)stack.Peek();
				switch (ExpressionParser.CompareOp(op, bop))
				{
				case ExpressionParser.Action.Shift:
					goto IL_34;
				case ExpressionParser.Action.Reduction:
					this.Reduce(stack, expression);
					continue;
				}
				break;
			}
			throw new ExpressionSyntaxException(Strings.InvalidSyntax);
			IL_34:
			stack.Push(expression);
			stack.Push(bop);
		}

		private void ReduceUntil(Stack stack, ExpressionParser.Op delim)
		{
			while (this.ReduceUnless(stack, delim))
			{
			}
			ExpressionParser.Expression obj = (ExpressionParser.Expression)stack.Pop();
			stack.Pop();
			stack.Push(obj);
		}

		private readonly ExpressionParser.Op[] ops;

		private ExpressionParser.ILexer lexer;

		private ExpressionParser.LeftParen leftParen;

		private ActivityManagerConfig managerConfig;

		private enum Action
		{
			Error,
			Shift,
			Reduction
		}

		internal interface ILexer
		{
			ExpressionParser.Token[] ToTokens(string input);
		}

		internal abstract class Op
		{
			protected Op(string op, int precedence, bool ignoreCase)
			{
				this.operatorString = op;
				this.precedence = precedence;
				this.ignoreCase = ignoreCase;
			}

			protected Op(string op, int precedence) : this(op, precedence, true)
			{
			}

			protected Op(string op) : this(op, int.MinValue)
			{
			}

			internal string OperatorString
			{
				get
				{
					return this.operatorString;
				}
			}

			internal int Precedence
			{
				get
				{
					return this.precedence;
				}
			}

			internal abstract int Arity { get; }

			protected bool IgnoreCase
			{
				get
				{
					return this.ignoreCase;
				}
			}

			public override string ToString()
			{
				return this.operatorString;
			}

			internal bool Matches(string s)
			{
				StringComparison comparisonType = this.IgnoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture;
				return string.Compare(this.operatorString, s, comparisonType) == 0;
			}

			private readonly string operatorString;

			private readonly int precedence;

			private readonly bool ignoreCase;
		}

		internal class UnaryOp : ExpressionParser.Op
		{
			protected UnaryOp(string op, int precedence, bool ignoreCase) : base(op, precedence, ignoreCase)
			{
			}

			protected UnaryOp(string op, int precedence) : base(op, precedence)
			{
			}

			internal override int Arity
			{
				get
				{
					return 1;
				}
			}

			internal virtual object Eval(object operand)
			{
				return null;
			}
		}

		internal class IsNullOp : ExpressionParser.UnaryOp
		{
			internal IsNullOp(string s) : base(s, 2, false)
			{
			}

			internal override object Eval(object operand1)
			{
				if (operand1 == null)
				{
					return true;
				}
				string text = operand1 as string;
				if (text != null && text.Length == 0)
				{
					return true;
				}
				return false;
			}
		}

		internal class BinaryOp : ExpressionParser.Op
		{
			protected BinaryOp(string op, ExpressionParser.BinaryOp.Associativity assoc, int precedence, bool ignoreCase) : base(op, precedence, ignoreCase)
			{
				this.assoc = assoc;
			}

			protected BinaryOp(string op, ExpressionParser.BinaryOp.Associativity assoc, int precedence) : base(op, precedence)
			{
				this.assoc = assoc;
			}

			internal ExpressionParser.BinaryOp.Associativity Assoc
			{
				get
				{
					return this.assoc;
				}
			}

			internal override int Arity
			{
				get
				{
					return 2;
				}
			}

			internal virtual object Eval(object operand1, object operand2)
			{
				return null;
			}

			private readonly ExpressionParser.BinaryOp.Associativity assoc;

			internal enum Associativity
			{
				None,
				Left,
				Right
			}
		}

		internal class LessThanOp : ExpressionParser.BinaryOp
		{
			internal LessThanOp(string s) : base(s, ExpressionParser.BinaryOp.Associativity.Left, 2, true)
			{
			}

			internal override object Eval(object operand1, object operand2)
			{
				IComparable comparable = (IComparable)(operand1 ?? 0);
				IComparable obj = (IComparable)(operand2 ?? 0);
				return comparable.CompareTo(obj) < 0;
			}
		}

		internal class EqualToOp : ExpressionParser.BinaryOp
		{
			internal EqualToOp(string s) : base(s, ExpressionParser.BinaryOp.Associativity.Left, 2, true)
			{
			}

			internal override object Eval(object operand1, object operand2)
			{
				IComparable comparable;
				IComparable obj;
				if (operand1 is string || operand2 is string)
				{
					comparable = (IComparable)(operand1 ?? string.Empty);
					obj = (IComparable)(operand2 ?? string.Empty);
				}
				else
				{
					comparable = (IComparable)(operand1 ?? 0);
					obj = (IComparable)(operand2 ?? 0);
				}
				return comparable.CompareTo(obj) == 0;
			}
		}

		internal class NotEqualToOp : ExpressionParser.BinaryOp
		{
			internal NotEqualToOp(string s) : base(s, ExpressionParser.BinaryOp.Associativity.Left, 2, true)
			{
			}

			internal override object Eval(object operand1, object operand2)
			{
				IComparable comparable;
				IComparable obj;
				if (operand1 is string || operand2 is string)
				{
					comparable = (IComparable)(operand1 ?? string.Empty);
					obj = (IComparable)(operand2 ?? string.Empty);
				}
				else
				{
					comparable = (IComparable)(operand1 ?? 0);
					obj = (IComparable)(operand2 ?? 0);
				}
				return comparable.CompareTo(obj) != 0;
			}
		}

		internal class GreaterThanOp : ExpressionParser.BinaryOp
		{
			internal GreaterThanOp(string s) : base(s, ExpressionParser.BinaryOp.Associativity.Left, 2, true)
			{
			}

			internal override object Eval(object operand1, object operand2)
			{
				IComparable comparable = (IComparable)(operand1 ?? 0);
				IComparable obj = (IComparable)(operand2 ?? 0);
				return comparable.CompareTo(obj) > 0;
			}
		}

		internal class AndOp : ExpressionParser.BinaryOp
		{
			internal AndOp(string s) : base(s, ExpressionParser.BinaryOp.Associativity.Left, 1, true)
			{
			}

			internal override object Eval(object operand1, object operand2)
			{
				bool flag = operand1 != null && (bool)operand1;
				bool flag2 = operand2 != null && (bool)operand2;
				return flag && flag2;
			}
		}

		internal class OrOp : ExpressionParser.BinaryOp
		{
			internal OrOp(string s) : base(s, ExpressionParser.BinaryOp.Associativity.Left, 1, true)
			{
			}

			internal override object Eval(object operand1, object operand2)
			{
				bool flag = operand1 != null && (bool)operand1;
				bool flag2 = operand2 != null && (bool)operand2;
				return flag || flag2;
			}
		}

		internal class NotOp : ExpressionParser.UnaryOp
		{
			internal NotOp(string s) : base(s, 3, false)
			{
			}

			internal override object Eval(object operand1)
			{
				bool flag = operand1 != null && (bool)operand1;
				return !flag;
			}
		}

		internal abstract class Expression
		{
			internal abstract object Eval(ActivityManager manager);
		}

		internal abstract class AtomicExpression : ExpressionParser.Expression
		{
		}

		internal class LiteralExpr : ExpressionParser.AtomicExpression
		{
			internal LiteralExpr(string literal)
			{
				this.literal = literal;
			}

			internal override object Eval(ActivityManager manager)
			{
				return this.literal;
			}

			private string literal;
		}

		internal class VariableExpr<T> : ExpressionParser.AtomicExpression
		{
			private VariableExpr(FsmVariable<T> fsmVar)
			{
				this.fsmVar = fsmVar;
			}

			internal static bool TryCreate(string varName, ActivityManagerConfig managerConfig, out ExpressionParser.VariableExpr<T> var)
			{
				var = ExpressionParser.VariableExpr<T>.TryCreate(varName, managerConfig);
				return null != var;
			}

			internal static ExpressionParser.VariableExpr<T> TryCreate(string varName, ActivityManagerConfig managerConfig)
			{
				FsmVariable<T> fsmVariable = null;
				QualifiedName variableName = new QualifiedName(varName, managerConfig);
				if (!FsmVariable<T>.TryCreate(variableName, managerConfig, out fsmVariable))
				{
					return null;
				}
				return new ExpressionParser.VariableExpr<T>(fsmVariable);
			}

			internal static ExpressionParser.VariableExpr<T> Create(string varName, ActivityManagerConfig managerConfig)
			{
				ExpressionParser.VariableExpr<T> result = null;
				if (!ExpressionParser.VariableExpr<T>.TryCreate(varName, managerConfig, out result))
				{
					throw new FsmConfigurationException(Strings.InvalidCondition(varName));
				}
				return result;
			}

			internal override object Eval(ActivityManager manager)
			{
				return this.fsmVar.GetValue(manager);
			}

			private FsmVariable<T> fsmVar;
		}

		internal class IntegerExpr : ExpressionParser.AtomicExpression
		{
			internal IntegerExpr(int val)
			{
				this.val = val;
			}

			internal override object Eval(ActivityManager manager)
			{
				return this.val;
			}

			private int val;
		}

		internal abstract class Token
		{
			protected Token(string text)
			{
				this.text = text;
			}

			internal string Text
			{
				get
				{
					return this.text;
				}
			}

			private readonly string text;
		}

		internal class StringToken : ExpressionParser.Token
		{
			internal StringToken(string s) : base(s.Trim())
			{
			}
		}

		internal class IntToken : ExpressionParser.Token
		{
			internal IntToken(string s) : base(s)
			{
				this.intVal = int.Parse(s, CultureInfo.InvariantCulture);
			}

			internal int IntVal
			{
				get
				{
					return this.intVal;
				}
				set
				{
					this.intVal = value;
				}
			}

			private int intVal;
		}

		internal class ConditionLexer : ExpressionParser.ILexer
		{
			internal ConditionLexer()
			{
			}

			public ExpressionParser.Token[] ToTokens(string input)
			{
				int i = 0;
				ArrayList arrayList = new ArrayList();
				while (i < input.Length)
				{
					int num = input.IndexOfAny(ExpressionParser.ConditionLexer.specialChars, i);
					if (num == i)
					{
						if (!char.IsWhiteSpace(input[num]))
						{
							arrayList.Add(new ExpressionParser.StringToken(input.Substring(i, 1)));
						}
						i++;
					}
					else
					{
						if (-1 == num)
						{
							num = input.Length;
						}
						string text = input.Substring(i, num - i);
						if (char.IsDigit(text[0]))
						{
							arrayList.Add(new ExpressionParser.IntToken(text));
						}
						else
						{
							arrayList.Add(new ExpressionParser.StringToken(text));
						}
						i = num;
					}
				}
				return (ExpressionParser.Token[])arrayList.ToArray(typeof(ExpressionParser.Token));
			}

			private static char[] specialChars = new char[]
			{
				'(',
				')',
				' '
			};
		}

		internal sealed class MarkerOp : ExpressionParser.Op
		{
			private MarkerOp() : base(string.Empty)
			{
			}

			internal override int Arity
			{
				get
				{
					return 0;
				}
			}

			internal static readonly ExpressionParser.MarkerOp Instance = new ExpressionParser.MarkerOp();
		}

		internal class LeftParen : ExpressionParser.Op
		{
			internal LeftParen(string leftParen) : base(leftParen)
			{
			}

			internal override int Arity
			{
				get
				{
					return 0;
				}
			}
		}

		internal class RightParen : ExpressionParser.Op
		{
			internal RightParen(string rightParen) : base(rightParen)
			{
			}

			internal override int Arity
			{
				get
				{
					return 0;
				}
			}
		}

		internal class UnaryOpExpression : ExpressionParser.Expression
		{
			internal UnaryOpExpression(ExpressionParser.UnaryOp op, ExpressionParser.Expression exp)
			{
				this.op = op;
				this.exp = exp;
			}

			internal ExpressionParser.UnaryOp Op
			{
				get
				{
					return this.op;
				}
			}

			internal ExpressionParser.Expression Exp
			{
				get
				{
					return this.exp;
				}
			}

			internal override object Eval(ActivityManager manager)
			{
				object operand = this.Exp.Eval(manager);
				return this.Op.Eval(operand);
			}

			private readonly ExpressionParser.UnaryOp op;

			private readonly ExpressionParser.Expression exp;
		}

		internal class BinaryOpExpression : ExpressionParser.Expression
		{
			internal BinaryOpExpression(ExpressionParser.BinaryOp op, ExpressionParser.Expression exp1, ExpressionParser.Expression exp2)
			{
				this.op = op;
				this.exp1 = exp1;
				this.exp2 = exp2;
			}

			internal ExpressionParser.BinaryOp Op
			{
				get
				{
					return this.op;
				}
			}

			internal ExpressionParser.Expression Exp2
			{
				get
				{
					return this.exp2;
				}
			}

			internal ExpressionParser.Expression Exp1
			{
				get
				{
					return this.exp1;
				}
			}

			internal override object Eval(ActivityManager manager)
			{
				object operand = this.Exp1.Eval(manager);
				object operand2 = this.Exp2.Eval(manager);
				return this.Op.Eval(operand, operand2);
			}

			private readonly ExpressionParser.BinaryOp op;

			private readonly ExpressionParser.Expression exp1;

			private readonly ExpressionParser.Expression exp2;
		}
	}
}
