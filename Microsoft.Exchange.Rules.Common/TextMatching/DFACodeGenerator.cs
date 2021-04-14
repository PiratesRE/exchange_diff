using System;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.TextMatching
{
	internal sealed class DFACodeGenerator
	{
		public DFACodeGenerator(string name, int size)
		{
			this.matcherMethod = new DynamicMethod(name, typeof(bool), new Type[]
			{
				typeof(ITextInputBuffer)
			}, typeof(DFACodeGenerator).Module);
			this.stateLabels = new Label[size];
			this.stateLabelDefinedFlags = new BitArray(size);
			this.stateLabelMarkedFlags = new BitArray(size);
		}

		private ILGenerator Generator
		{
			get
			{
				return this.matcherMethod.GetILGenerator();
			}
		}

		public void BeginCompile()
		{
			this.Generator.DeclareLocal(typeof(int));
		}

		public void Add(StateNode from, StateNode to, int ch)
		{
			this.AddTransition(from, to, ch);
		}

		public void Add(StateNode from, StateNode to, RegexCharacterClass cl)
		{
			this.EmitLoadInput(from, to);
			if (cl.Type == RegexCharacterClass.ValueType.Character)
			{
				this.EmitCharComparision(from, to, cl.GetHashCode());
			}
			else
			{
				this.EmitCharacterClassComparision(from, to, cl.Type);
			}
			if (from.State == 0 && cl.Type == RegexCharacterClass.ValueType.NonWordCharacterClass)
			{
				this.beginningNonWordToStateid = to.State;
			}
		}

		public void AddTransition(StateNode from, StateNode to, int ch)
		{
			this.EmitLoadInput(from, to);
			this.EmitCharComparision(from, to, ch);
		}

		public PatternMatcher EndCompile()
		{
			this.EmitLastCheck();
			this.CheckPendingLabels();
			return (PatternMatcher)this.matcherMethod.CreateDelegate(typeof(PatternMatcher));
		}

		private void CheckPendingLabels()
		{
			for (int i = 0; i <= this.pendingLabelIndex; i++)
			{
				if (this.stateLabelDefinedFlags[i] && !this.stateLabelMarkedFlags[i])
				{
					this.EmitLabel(this.stateLabels[i], true);
					this.stateLabelMarkedFlags[i] = true;
				}
			}
		}

		private void EmitLoadInput()
		{
			this.Generator.Emit(OpCodes.Ldarg_0);
		}

		private void EmitLabel(Label label, bool finalState)
		{
			this.Generator.MarkLabel(label);
			if (finalState)
			{
				this.Generator.Emit(OpCodes.Ldc_I4_1);
				this.Generator.Emit(OpCodes.Ret);
				return;
			}
			this.EmitLoadInput();
			this.Generator.EmitCall(OpCodes.Callvirt, DFACodeGenerator.getNextChar, null);
			this.Generator.Emit(OpCodes.Stloc_0);
		}

		private void EmitLoadInput(StateNode from, StateNode to)
		{
			if (this.lastStateid != -1 && this.lastStateid != from.State)
			{
				this.EmitLastCheck();
			}
			if (!this.stateLabelDefinedFlags[from.State])
			{
				this.stateLabels[from.State] = this.Generator.DefineLabel();
				this.EmitLabel(this.stateLabels[from.State], from.IsFinal);
				this.stateLabelDefinedFlags[from.State] = true;
				this.stateLabelMarkedFlags[from.State] = true;
			}
			else if (!this.stateLabelMarkedFlags[from.State])
			{
				this.EmitLabel(this.stateLabels[from.State], from.IsFinal);
				this.stateLabelMarkedFlags[from.State] = true;
			}
			if (!this.stateLabelDefinedFlags[to.State])
			{
				this.stateLabels[to.State] = this.Generator.DefineLabel();
				this.stateLabelDefinedFlags[to.State] = true;
				this.pendingLabelIndex = to.State;
			}
		}

		private void EmitLastCheck()
		{
			Label label = this.Generator.DefineLabel();
			this.Generator.Emit(OpCodes.Ldloc_0);
			this.Generator.Emit(OpCodes.Ldc_I4_M1);
			this.Generator.Emit(OpCodes.Beq, label);
			if (this.beginningNonWordToStateid > 0 && this.lastStateid > 0)
			{
				this.Generator.Emit(OpCodes.Ldloc_0);
				this.Generator.EmitCall(OpCodes.Call, DFACodeGenerator.compareNonWord, null);
				this.Generator.Emit(OpCodes.Brtrue, this.stateLabels[this.beginningNonWordToStateid]);
			}
			this.Generator.Emit(OpCodes.Br, this.stateLabels[0]);
			this.Generator.MarkLabel(label);
			this.Generator.Emit(OpCodes.Ldc_I4_0);
			this.Generator.Emit(OpCodes.Ret);
		}

		private void EmitCharComparision(StateNode from, StateNode to, int ch)
		{
			Label label = this.stateLabels[to.State];
			this.Generator.Emit(OpCodes.Ldloc_0);
			this.Generator.Emit(OpCodes.Ldc_I4, ch);
			this.Generator.Emit(OpCodes.Beq, label);
			this.lastStateid = from.State;
		}

		private void EmitCharacterClassComparision(StateNode from, StateNode to, RegexCharacterClass.ValueType charType)
		{
			Label label = this.stateLabels[to.State];
			this.Generator.Emit(OpCodes.Ldloc_0);
			switch (charType)
			{
			case RegexCharacterClass.ValueType.BeginCharacterClass:
				this.Generator.EmitCall(OpCodes.Call, DFACodeGenerator.compareBegin, null);
				break;
			case RegexCharacterClass.ValueType.EndCharacterClass:
				this.Generator.EmitCall(OpCodes.Call, DFACodeGenerator.compareEnd, null);
				break;
			case RegexCharacterClass.ValueType.SpaceCharacterClass:
				this.Generator.EmitCall(OpCodes.Call, DFACodeGenerator.compareSpace, null);
				break;
			case RegexCharacterClass.ValueType.NonSpaceCharacterClass:
				this.Generator.EmitCall(OpCodes.Call, DFACodeGenerator.compareNonSpace, null);
				break;
			case RegexCharacterClass.ValueType.NonDigitCharacterClass:
				this.Generator.EmitCall(OpCodes.Call, DFACodeGenerator.compareNonDigit, null);
				break;
			case RegexCharacterClass.ValueType.DigitCharacterClass:
				this.Generator.EmitCall(OpCodes.Call, DFACodeGenerator.compareDigit, null);
				break;
			case RegexCharacterClass.ValueType.WordCharacterClass:
				this.Generator.EmitCall(OpCodes.Call, DFACodeGenerator.compareWord, null);
				break;
			case RegexCharacterClass.ValueType.NonWordCharacterClass:
				this.Generator.EmitCall(OpCodes.Call, DFACodeGenerator.compareNonWord, null);
				break;
			default:
				throw new TextMatchingParsingException(TextMatchingStrings.RegexUnSupportedMetaCharacter);
			}
			this.Generator.Emit(OpCodes.Brtrue, label);
			this.lastStateid = from.State;
		}

		private static MethodInfo getNextChar = typeof(ITextInputBuffer).GetMethod("get_NextChar");

		private static MethodInfo compareSpace = typeof(RegexCharacterClassRuntime).GetMethod("IsSpace", BindingFlags.Static | BindingFlags.Public);

		private static MethodInfo compareNonSpace = typeof(RegexCharacterClassRuntime).GetMethod("IsNonSpace", BindingFlags.Static | BindingFlags.Public);

		private static MethodInfo compareDigit = typeof(RegexCharacterClassRuntime).GetMethod("IsDigit", BindingFlags.Static | BindingFlags.Public);

		private static MethodInfo compareNonDigit = typeof(RegexCharacterClassRuntime).GetMethod("IsNonDigit", BindingFlags.Static | BindingFlags.Public);

		private static MethodInfo compareNonWord = typeof(RegexCharacterClassRuntime).GetMethod("IsNonWord", BindingFlags.Static | BindingFlags.Public);

		private static MethodInfo compareWord = typeof(RegexCharacterClassRuntime).GetMethod("IsWord", BindingFlags.Static | BindingFlags.Public);

		private static MethodInfo compareBegin = typeof(RegexCharacterClassRuntime).GetMethod("IsBegin", BindingFlags.Static | BindingFlags.Public);

		private static MethodInfo compareEnd = typeof(RegexCharacterClassRuntime).GetMethod("IsEnd", BindingFlags.Static | BindingFlags.Public);

		private DynamicMethod matcherMethod;

		private Label[] stateLabels;

		private BitArray stateLabelDefinedFlags;

		private BitArray stateLabelMarkedFlags;

		private int lastStateid = -1;

		private int pendingLabelIndex = -1;

		private int beginningNonWordToStateid = -1;
	}
}
