using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class FormattedSentence
	{
		public FormattedSentence(string format)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			this.BuildInstructionQueue(this.instructionQueue, FormattedSentence.Sentence.Parse(format));
		}

		public string Evaluate(FormattedSentence.Context context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			Stack<FormattedSentence.IEvaluationStackMember> stack = new Stack<FormattedSentence.IEvaluationStackMember>();
			foreach (FormattedSentence.IInstructionQueueMember instructionQueueMember in this.instructionQueue)
			{
				instructionQueueMember.Evaluate(context, stack);
			}
			FormattedSentence.IEvaluationStackMember evaluationStackMember = stack.Pop();
			if (stack.Count > 0)
			{
				throw new InvalidOperationException("FormattedSentence execution produced unexpected results: more than 1 result element");
			}
			StringBuilder stringBuilder = new StringBuilder();
			evaluationStackMember.WriteTo(stringBuilder);
			return stringBuilder.ToString();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (FormattedSentence.IInstructionQueueMember instructionQueueMember in this.instructionQueue)
			{
				stringBuilder.Append(instructionQueueMember.ToString());
			}
			return stringBuilder.ToString();
		}

		private void BuildInstructionsForOneToken(IList<FormattedSentence.IInstructionQueueMember> instructionQueue, FormattedSentence.Token token)
		{
			FormattedSentence.Sentence sentence = token as FormattedSentence.Sentence;
			FormattedSentence.SimpleToken simpleToken = token as FormattedSentence.SimpleToken;
			if (sentence != null)
			{
				this.BuildInstructionQueue(instructionQueue, sentence);
				return;
			}
			if (simpleToken == null)
			{
				return;
			}
			switch (simpleToken.Flavor)
			{
			case FormattedSentence.SimpleToken.TokenFlavor.None:
				instructionQueue.Add(new FormattedSentence.StringConstant(simpleToken.Value));
				return;
			case FormattedSentence.SimpleToken.TokenFlavor.InCurlyBraces:
				instructionQueue.Add(new FormattedSentence.ResolvePlaceholder(simpleToken.Value));
				return;
			default:
				throw new NotSupportedException("Token flavor is not supported");
			}
		}

		protected void BuildInstructionQueue(IList<FormattedSentence.IInstructionQueueMember> instructionQueue, FormattedSentence.Sentence sentence)
		{
			if (sentence.Tokens.Count == 0)
			{
				instructionQueue.Add(FormattedSentence.StringConstant.EmptySpacer);
				return;
			}
			if (sentence.Tokens.Count == 1)
			{
				this.BuildInstructionsForOneToken(instructionQueue, sentence.Tokens[0]);
				return;
			}
			bool flag = false;
			int num = 0;
			for (int i = 0; i < sentence.Tokens.Count; i++)
			{
				FormattedSentence.SimpleToken simpleToken = sentence.Tokens[i] as FormattedSentence.SimpleToken;
				bool flag2 = simpleToken != null && simpleToken.Flavor == FormattedSentence.SimpleToken.TokenFlavor.None;
				if (i == 0 && flag2)
				{
					instructionQueue.Add(FormattedSentence.StringConstant.Null);
					num++;
				}
				if (i > 0 && !flag && !flag2)
				{
					instructionQueue.Add(FormattedSentence.StringConstant.EmptySpacer);
					num++;
				}
				this.BuildInstructionsForOneToken(instructionQueue, sentence.Tokens[i]);
				num++;
				flag = flag2;
			}
			if (flag)
			{
				instructionQueue.Add(FormattedSentence.StringConstant.Null);
				num++;
			}
			int num2 = (num - 1) / 2;
			for (int j = 0; j < num2; j++)
			{
				instructionQueue.Add(new FormattedSentence.ConditionalDelimiter());
			}
		}

		private readonly List<FormattedSentence.IInstructionQueueMember> instructionQueue = new List<FormattedSentence.IInstructionQueueMember>();

		protected abstract class Token
		{
			protected const char EscapeChar = '\\';
		}

		protected sealed class Sentence : FormattedSentence.Token
		{
			private Sentence(List<FormattedSentence.Token> tokens)
			{
				this.tokens = new ReadOnlyCollection<FormattedSentence.Token>(tokens);
			}

			public IList<FormattedSentence.Token> Tokens
			{
				get
				{
					return this.tokens;
				}
			}

			public static FormattedSentence.Sentence Parse(string input)
			{
				FormattedSentence.Sentence result;
				int num = FormattedSentence.Sentence.Parse(input, 0, out result);
				if (num != input.Length)
				{
					throw new FormatException("'>' is mismatched or not escaped");
				}
				return result;
			}

			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (FormattedSentence.Token token in this.tokens)
				{
					stringBuilder.Append(token.ToString());
				}
				return stringBuilder.ToString();
			}

			private static int Parse(string input, int startAt, out FormattedSentence.Sentence sentence)
			{
				FormattedSentence.SimpleToken.Builder builder = new FormattedSentence.SimpleToken.Builder(null, FormattedSentence.SimpleToken.TokenFlavor.None, -1);
				List<FormattedSentence.Token> list = new List<FormattedSentence.Token>();
				int num = startAt - 1;
				bool flag = false;
				while (!flag)
				{
					num++;
					if (num > input.Length)
					{
						throw new FormatException("Unexpected end of escape sequence. If '' was intended for output, replace it with '\\'");
					}
					if (num == input.Length)
					{
						break;
					}
					char c = input[num];
					char c2 = c;
					switch (c2)
					{
					case '<':
					{
						FormattedSentence.Sentence.AddTokenFlavorNone(list, num - 1, ref builder);
						FormattedSentence.Sentence item;
						num = FormattedSentence.Sentence.Parse(input, num + 1, out item);
						if (num >= input.Length || input[num] != '>')
						{
							throw new FormatException("'<' is mismatched or not properly escaped");
						}
						list.Add(item);
						continue;
					}
					case '=':
						break;
					case '>':
						flag = true;
						continue;
					default:
						switch (c2)
						{
						case '{':
							FormattedSentence.Sentence.AddTokenFlavorNone(list, num - 1, ref builder);
							builder = new FormattedSentence.SimpleToken.Builder(input, FormattedSentence.SimpleToken.TokenFlavor.InCurlyBraces, num + 1);
							continue;
						case '}':
							if (!builder.IsValid || builder.Flavor != FormattedSentence.SimpleToken.TokenFlavor.InCurlyBraces)
							{
								throw new FormatException("'}' is mismatched or not escaped");
							}
							FormattedSentence.Sentence.AddToken(list, num - 1, ref builder);
							continue;
						}
						break;
					}
					if (!builder.IsValid)
					{
						builder = new FormattedSentence.SimpleToken.Builder(input, FormattedSentence.SimpleToken.TokenFlavor.None, num);
					}
					if (c == '\\')
					{
						num++;
					}
				}
				FormattedSentence.Sentence.AddTokenFlavorNone(list, num - 1, ref builder);
				sentence = new FormattedSentence.Sentence(list);
				return num;
			}

			private static void AddTokenFlavorNone(IList<FormattedSentence.Token> tokens, int endAt, ref FormattedSentence.SimpleToken.Builder tokenBuilder)
			{
				if (tokenBuilder.IsValid)
				{
					if (tokenBuilder.Flavor == FormattedSentence.SimpleToken.TokenFlavor.InCurlyBraces)
					{
						throw new FormatException("'{' is mismatched or not escaped");
					}
					FormattedSentence.Sentence.AddToken(tokens, endAt, ref tokenBuilder);
				}
			}

			private static bool AddToken(IList<FormattedSentence.Token> tokens, int endAt, ref FormattedSentence.SimpleToken.Builder tokenBuilder)
			{
				if (tokenBuilder.IsValid)
				{
					FormattedSentence.SimpleToken simpleToken = tokenBuilder.Create(endAt);
					if (simpleToken != null)
					{
						tokens.Add(simpleToken);
					}
					tokenBuilder.Invalidate();
					return simpleToken != null;
				}
				return false;
			}

			private readonly IList<FormattedSentence.Token> tokens;
		}

		protected sealed class SimpleToken : FormattedSentence.Token
		{
			public SimpleToken(FormattedSentence.SimpleToken.TokenFlavor flavor, string value)
			{
				this.flavor = flavor;
				this.value = value;
			}

			public FormattedSentence.SimpleToken.TokenFlavor Flavor
			{
				get
				{
					return this.flavor;
				}
			}

			public string Value
			{
				get
				{
					return this.value;
				}
			}

			public override string ToString()
			{
				return this.value;
			}

			private readonly FormattedSentence.SimpleToken.TokenFlavor flavor;

			private readonly string value;

			public enum TokenFlavor
			{
				None = -1,
				InCurlyBraces
			}

			public struct Builder
			{
				public Builder(string input, FormattedSentence.SimpleToken.TokenFlavor flavor, int startAt)
				{
					this.input = input;
					this.flavor = flavor;
					this.startAt = startAt;
				}

				public FormattedSentence.SimpleToken.TokenFlavor Flavor
				{
					get
					{
						return this.flavor;
					}
				}

				public bool IsValid
				{
					get
					{
						return this.input != null;
					}
				}

				public FormattedSentence.SimpleToken Create(int endAt)
				{
					if (endAt >= this.input.Length)
					{
						ExDiagnostics.FailFast("endAt is beyond the end of input string", false);
						return null;
					}
					if (endAt < this.startAt)
					{
						throw new FormatException("Empty tokens like \"{}\" are not allowed");
					}
					if (this.input.IndexOf('\\', this.startAt, endAt - this.startAt + 1) != -1)
					{
						StringBuilder stringBuilder = new StringBuilder(endAt - this.startAt);
						bool flag = false;
						for (int i = this.startAt; i <= endAt; i++)
						{
							if (this.input[i] != '\\' || flag)
							{
								stringBuilder.Append(this.input[i]);
							}
							flag = (!flag && this.input[i] == '\\');
						}
						return new FormattedSentence.SimpleToken(this.flavor, stringBuilder.ToString());
					}
					return new FormattedSentence.SimpleToken(this.flavor, this.input.Substring(this.startAt, endAt - this.startAt + 1));
				}

				public void Invalidate()
				{
					this.input = null;
				}

				private string input;

				private readonly int startAt;

				private readonly FormattedSentence.SimpleToken.TokenFlavor flavor;
			}
		}

		protected interface IInstructionQueueMember
		{
			void Evaluate(FormattedSentence.Context context, Stack<FormattedSentence.IEvaluationStackMember> evaluationStack);
		}

		protected interface IEvaluationStackMember
		{
			bool IsEmpty { get; }

			void WriteTo(StringBuilder outputBuilder);
		}

		public abstract class Context
		{
			public abstract string ResolvePlaceholder(string code);
		}

		protected sealed class StringConstant : FormattedSentence.IInstructionQueueMember, FormattedSentence.IEvaluationStackMember
		{
			public StringConstant(string value)
			{
				this.value = value;
			}

			public bool IsEmpty
			{
				get
				{
					return string.IsNullOrEmpty(this.value);
				}
			}

			public void Evaluate(FormattedSentence.Context context, Stack<FormattedSentence.IEvaluationStackMember> evaluationStack)
			{
				evaluationStack.Push(this);
			}

			public override string ToString()
			{
				if (this == FormattedSentence.StringConstant.Null)
				{
					return "<Null>";
				}
				if (string.IsNullOrEmpty(this.value))
				{
					return "<Empty>";
				}
				return this.value;
			}

			public void WriteTo(StringBuilder outputBuilder)
			{
				outputBuilder.Append(this.value);
			}

			public static readonly FormattedSentence.StringConstant EmptySpacer = new FormattedSentence.StringConstant(string.Empty);

			public static readonly FormattedSentence.StringConstant Null = new FormattedSentence.StringConstant(string.Empty);

			private readonly string value;
		}

		protected sealed class ResolvePlaceholder : FormattedSentence.IInstructionQueueMember
		{
			public ResolvePlaceholder(string code)
			{
				this.code = code;
			}

			public void Evaluate(FormattedSentence.Context context, Stack<FormattedSentence.IEvaluationStackMember> evaluationStack)
			{
				evaluationStack.Push(new FormattedSentence.StringConstant(context.ResolvePlaceholder(this.code)));
			}

			public override string ToString()
			{
				return "{" + this.code + "}";
			}

			private readonly string code;
		}

		protected sealed class ConditionalDelimiter : FormattedSentence.IInstructionQueueMember
		{
			public void Evaluate(FormattedSentence.Context context, Stack<FormattedSentence.IEvaluationStackMember> evaluationStack)
			{
				FormattedSentence.IEvaluationStackMember evaluationStackMember = evaluationStack.Pop();
				FormattedSentence.IEvaluationStackMember evaluationStackMember2 = evaluationStack.Pop();
				FormattedSentence.IEvaluationStackMember evaluationStackMember3 = evaluationStack.Pop();
				bool flag = (!evaluationStackMember3.IsEmpty && !evaluationStackMember.IsEmpty) || (!evaluationStackMember3.IsEmpty && evaluationStackMember == FormattedSentence.StringConstant.Null) || (!evaluationStackMember.IsEmpty && evaluationStackMember3 == FormattedSentence.StringConstant.Null);
				evaluationStack.Push(FormattedSentence.CompositeString.Create(new FormattedSentence.IEvaluationStackMember[]
				{
					evaluationStackMember3,
					flag ? evaluationStackMember2 : FormattedSentence.StringConstant.Null,
					evaluationStackMember
				}));
			}

			public override string ToString()
			{
				return "!";
			}
		}

		protected sealed class CompositeString : FormattedSentence.IEvaluationStackMember
		{
			private CompositeString(FormattedSentence.IEvaluationStackMember[] members)
			{
				this.members = members;
			}

			public bool IsEmpty
			{
				get
				{
					foreach (FormattedSentence.IEvaluationStackMember evaluationStackMember in this.members)
					{
						if (!evaluationStackMember.IsEmpty)
						{
							return false;
						}
					}
					return true;
				}
			}

			public static FormattedSentence.IEvaluationStackMember Create(params FormattedSentence.IEvaluationStackMember[] members)
			{
				FormattedSentence.IEvaluationStackMember evaluationStackMember = null;
				foreach (FormattedSentence.IEvaluationStackMember evaluationStackMember2 in members)
				{
					if (!evaluationStackMember2.IsEmpty)
					{
						if (evaluationStackMember != null)
						{
							return new FormattedSentence.CompositeString(members);
						}
						evaluationStackMember = evaluationStackMember2;
					}
				}
				if (evaluationStackMember == null)
				{
					return FormattedSentence.StringConstant.EmptySpacer;
				}
				return evaluationStackMember;
			}

			public void WriteTo(StringBuilder outputBuilder)
			{
				foreach (FormattedSentence.IEvaluationStackMember evaluationStackMember in this.members)
				{
					evaluationStackMember.WriteTo(outputBuilder);
				}
			}

			private readonly FormattedSentence.IEvaluationStackMember[] members;
		}
	}
}
