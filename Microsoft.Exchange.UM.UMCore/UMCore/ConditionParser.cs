using System;
using System.Collections;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class ConditionParser : ExpressionParser
	{
		private ConditionParser(ExpressionParser.ILexer lex) : base(lex, ConditionParser.ops)
		{
		}

		internal static ConditionParser Instance
		{
			get
			{
				if (ConditionParser.instance == null)
				{
					ConditionParser.instance = new ConditionParser(new ExpressionParser.ConditionLexer());
				}
				return ConditionParser.instance;
			}
		}

		private static Hashtable Cache
		{
			get
			{
				if (ConditionParser.cache == null)
				{
					ConditionParser.cache = new Hashtable();
				}
				return ConditionParser.cache;
			}
		}

		internal static void Release()
		{
			ConditionParser.instance = null;
			ConditionParser.cache = null;
		}

		internal override ExpressionParser.Expression Parse(string exp, ActivityManagerConfig managerConfig)
		{
			base.ManagerConfig = managerConfig;
			string str = (managerConfig == null) ? string.Empty : managerConfig.ClassName;
			string key = str + exp;
			if (ConditionParser.Cache.ContainsKey(key))
			{
				return ConditionParser.Cache[key] as ExpressionParser.Expression;
			}
			ExpressionParser.Expression expression = base.Parse(exp, managerConfig);
			ConditionParser.Cache[key] = expression;
			return expression;
		}

		protected override ExpressionParser.AtomicExpression ParseAtomicExpression(ArrayList tokens)
		{
			ExpressionParser.Token token = (ExpressionParser.Token)tokens[0];
			ExpressionParser.AtomicExpression result;
			if (token is ExpressionParser.StringToken)
			{
				if (token.Text.StartsWith("@", StringComparison.InvariantCulture))
				{
					result = new ExpressionParser.LiteralExpr(token.Text.Substring(1));
				}
				else if ((result = ExpressionParser.VariableExpr<string>.TryCreate(token.Text, base.ManagerConfig)) == null && (result = ExpressionParser.VariableExpr<object>.TryCreate(token.Text, base.ManagerConfig)) == null && (result = ExpressionParser.VariableExpr<bool>.TryCreate(token.Text, base.ManagerConfig)) == null && (result = ExpressionParser.VariableExpr<int>.TryCreate(token.Text, base.ManagerConfig)) == null && (result = ExpressionParser.VariableExpr<ExDateTime>.TryCreate(token.Text, base.ManagerConfig)) == null)
				{
					throw new FsmConfigurationException(Strings.InvalidVariable(token.Text));
				}
			}
			else
			{
				ExpressionParser.IntToken intToken = (ExpressionParser.IntToken)token;
				result = new ExpressionParser.IntegerExpr(intToken.IntVal);
			}
			tokens.RemoveAt(0);
			return result;
		}

		internal static readonly string AndOperator = "AND";

		private static ExpressionParser.Op[] ops = new ExpressionParser.Op[]
		{
			new ExpressionParser.AndOp("AND"),
			new ExpressionParser.OrOp("OR"),
			new ExpressionParser.NotOp("NOT"),
			new ExpressionParser.IsNullOp("IsNull"),
			new ExpressionParser.GreaterThanOp("GT"),
			new ExpressionParser.LessThanOp("LT"),
			new ExpressionParser.EqualToOp("EQ"),
			new ExpressionParser.NotEqualToOp("NE"),
			new ExpressionParser.LeftParen("("),
			new ExpressionParser.RightParen(")")
		};

		private static ConditionParser instance;

		private static Hashtable cache;
	}
}
