using System;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	public class OpCodes
	{
		private OpCodes()
		{
		}

		[__DynamicallyInvokable]
		public static bool TakesSingleByteArgument(OpCode inst)
		{
			OperandType operandType = inst.OperandType;
			return operandType - OperandType.ShortInlineBrTarget <= 1 || operandType == OperandType.ShortInlineVar;
		}

		[__DynamicallyInvokable]
		public static readonly OpCode Nop = new OpCode(OpCodeValues.Nop, 6556325);

		[__DynamicallyInvokable]
		public static readonly OpCode Break = new OpCode(OpCodeValues.Break, 6556197);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldarg_0 = new OpCode(OpCodeValues.Ldarg_0, 275120805);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldarg_1 = new OpCode(OpCodeValues.Ldarg_1, 275120805);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldarg_2 = new OpCode(OpCodeValues.Ldarg_2, 275120805);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldarg_3 = new OpCode(OpCodeValues.Ldarg_3, 275120805);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldloc_0 = new OpCode(OpCodeValues.Ldloc_0, 275120805);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldloc_1 = new OpCode(OpCodeValues.Ldloc_1, 275120805);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldloc_2 = new OpCode(OpCodeValues.Ldloc_2, 275120805);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldloc_3 = new OpCode(OpCodeValues.Ldloc_3, 275120805);

		[__DynamicallyInvokable]
		public static readonly OpCode Stloc_0 = new OpCode(OpCodeValues.Stloc_0, -261877083);

		[__DynamicallyInvokable]
		public static readonly OpCode Stloc_1 = new OpCode(OpCodeValues.Stloc_1, -261877083);

		[__DynamicallyInvokable]
		public static readonly OpCode Stloc_2 = new OpCode(OpCodeValues.Stloc_2, -261877083);

		[__DynamicallyInvokable]
		public static readonly OpCode Stloc_3 = new OpCode(OpCodeValues.Stloc_3, -261877083);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldarg_S = new OpCode(OpCodeValues.Ldarg_S, 275120818);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldarga_S = new OpCode(OpCodeValues.Ldarga_S, 275382962);

		[__DynamicallyInvokable]
		public static readonly OpCode Starg_S = new OpCode(OpCodeValues.Starg_S, -261877070);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldloc_S = new OpCode(OpCodeValues.Ldloc_S, 275120818);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldloca_S = new OpCode(OpCodeValues.Ldloca_S, 275382962);

		[__DynamicallyInvokable]
		public static readonly OpCode Stloc_S = new OpCode(OpCodeValues.Stloc_S, -261877070);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldnull = new OpCode(OpCodeValues.Ldnull, 275909285);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldc_I4_M1 = new OpCode(OpCodeValues.Ldc_I4_M1, 275382949);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldc_I4_0 = new OpCode(OpCodeValues.Ldc_I4_0, 275382949);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldc_I4_1 = new OpCode(OpCodeValues.Ldc_I4_1, 275382949);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldc_I4_2 = new OpCode(OpCodeValues.Ldc_I4_2, 275382949);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldc_I4_3 = new OpCode(OpCodeValues.Ldc_I4_3, 275382949);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldc_I4_4 = new OpCode(OpCodeValues.Ldc_I4_4, 275382949);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldc_I4_5 = new OpCode(OpCodeValues.Ldc_I4_5, 275382949);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldc_I4_6 = new OpCode(OpCodeValues.Ldc_I4_6, 275382949);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldc_I4_7 = new OpCode(OpCodeValues.Ldc_I4_7, 275382949);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldc_I4_8 = new OpCode(OpCodeValues.Ldc_I4_8, 275382949);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldc_I4_S = new OpCode(OpCodeValues.Ldc_I4_S, 275382960);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldc_I4 = new OpCode(OpCodeValues.Ldc_I4, 275384994);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldc_I8 = new OpCode(OpCodeValues.Ldc_I8, 275516067);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldc_R4 = new OpCode(OpCodeValues.Ldc_R4, 275647153);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldc_R8 = new OpCode(OpCodeValues.Ldc_R8, 275778215);

		[__DynamicallyInvokable]
		public static readonly OpCode Dup = new OpCode(OpCodeValues.Dup, 275258021);

		[__DynamicallyInvokable]
		public static readonly OpCode Pop = new OpCode(OpCodeValues.Pop, -261875035);

		[__DynamicallyInvokable]
		public static readonly OpCode Jmp = new OpCode(OpCodeValues.Jmp, 23333444);

		[__DynamicallyInvokable]
		public static readonly OpCode Call = new OpCode(OpCodeValues.Call, 7842372);

		[__DynamicallyInvokable]
		public static readonly OpCode Calli = new OpCode(OpCodeValues.Calli, 7842377);

		[__DynamicallyInvokable]
		public static readonly OpCode Ret = new OpCode(OpCodeValues.Ret, 23440101);

		[__DynamicallyInvokable]
		public static readonly OpCode Br_S = new OpCode(OpCodeValues.Br_S, 23331343);

		[__DynamicallyInvokable]
		public static readonly OpCode Brfalse_S = new OpCode(OpCodeValues.Brfalse_S, -261868945);

		[__DynamicallyInvokable]
		public static readonly OpCode Brtrue_S = new OpCode(OpCodeValues.Brtrue_S, -261868945);

		[__DynamicallyInvokable]
		public static readonly OpCode Beq_S = new OpCode(OpCodeValues.Beq_S, -530308497);

		[__DynamicallyInvokable]
		public static readonly OpCode Bge_S = new OpCode(OpCodeValues.Bge_S, -530308497);

		[__DynamicallyInvokable]
		public static readonly OpCode Bgt_S = new OpCode(OpCodeValues.Bgt_S, -530308497);

		[__DynamicallyInvokable]
		public static readonly OpCode Ble_S = new OpCode(OpCodeValues.Ble_S, -530308497);

		[__DynamicallyInvokable]
		public static readonly OpCode Blt_S = new OpCode(OpCodeValues.Blt_S, -530308497);

		[__DynamicallyInvokable]
		public static readonly OpCode Bne_Un_S = new OpCode(OpCodeValues.Bne_Un_S, -530308497);

		[__DynamicallyInvokable]
		public static readonly OpCode Bge_Un_S = new OpCode(OpCodeValues.Bge_Un_S, -530308497);

		[__DynamicallyInvokable]
		public static readonly OpCode Bgt_Un_S = new OpCode(OpCodeValues.Bgt_Un_S, -530308497);

		[__DynamicallyInvokable]
		public static readonly OpCode Ble_Un_S = new OpCode(OpCodeValues.Ble_Un_S, -530308497);

		[__DynamicallyInvokable]
		public static readonly OpCode Blt_Un_S = new OpCode(OpCodeValues.Blt_Un_S, -530308497);

		[__DynamicallyInvokable]
		public static readonly OpCode Br = new OpCode(OpCodeValues.Br, 23333376);

		[__DynamicallyInvokable]
		public static readonly OpCode Brfalse = new OpCode(OpCodeValues.Brfalse, -261866912);

		[__DynamicallyInvokable]
		public static readonly OpCode Brtrue = new OpCode(OpCodeValues.Brtrue, -261866912);

		[__DynamicallyInvokable]
		public static readonly OpCode Beq = new OpCode(OpCodeValues.Beq, -530308512);

		[__DynamicallyInvokable]
		public static readonly OpCode Bge = new OpCode(OpCodeValues.Bge, -530308512);

		[__DynamicallyInvokable]
		public static readonly OpCode Bgt = new OpCode(OpCodeValues.Bgt, -530308512);

		[__DynamicallyInvokable]
		public static readonly OpCode Ble = new OpCode(OpCodeValues.Ble, -530308512);

		[__DynamicallyInvokable]
		public static readonly OpCode Blt = new OpCode(OpCodeValues.Blt, -530308512);

		[__DynamicallyInvokable]
		public static readonly OpCode Bne_Un = new OpCode(OpCodeValues.Bne_Un, -530308512);

		[__DynamicallyInvokable]
		public static readonly OpCode Bge_Un = new OpCode(OpCodeValues.Bge_Un, -530308512);

		[__DynamicallyInvokable]
		public static readonly OpCode Bgt_Un = new OpCode(OpCodeValues.Bgt_Un, -530308512);

		[__DynamicallyInvokable]
		public static readonly OpCode Ble_Un = new OpCode(OpCodeValues.Ble_Un, -530308512);

		[__DynamicallyInvokable]
		public static readonly OpCode Blt_Un = new OpCode(OpCodeValues.Blt_Un, -530308512);

		[__DynamicallyInvokable]
		public static readonly OpCode Switch = new OpCode(OpCodeValues.Switch, -261866901);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldind_I1 = new OpCode(OpCodeValues.Ldind_I1, 6961829);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldind_U1 = new OpCode(OpCodeValues.Ldind_U1, 6961829);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldind_I2 = new OpCode(OpCodeValues.Ldind_I2, 6961829);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldind_U2 = new OpCode(OpCodeValues.Ldind_U2, 6961829);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldind_I4 = new OpCode(OpCodeValues.Ldind_I4, 6961829);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldind_U4 = new OpCode(OpCodeValues.Ldind_U4, 6961829);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldind_I8 = new OpCode(OpCodeValues.Ldind_I8, 7092901);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldind_I = new OpCode(OpCodeValues.Ldind_I, 6961829);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldind_R4 = new OpCode(OpCodeValues.Ldind_R4, 7223973);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldind_R8 = new OpCode(OpCodeValues.Ldind_R8, 7355045);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldind_Ref = new OpCode(OpCodeValues.Ldind_Ref, 7486117);

		[__DynamicallyInvokable]
		public static readonly OpCode Stind_Ref = new OpCode(OpCodeValues.Stind_Ref, -530294107);

		[__DynamicallyInvokable]
		public static readonly OpCode Stind_I1 = new OpCode(OpCodeValues.Stind_I1, -530294107);

		[__DynamicallyInvokable]
		public static readonly OpCode Stind_I2 = new OpCode(OpCodeValues.Stind_I2, -530294107);

		[__DynamicallyInvokable]
		public static readonly OpCode Stind_I4 = new OpCode(OpCodeValues.Stind_I4, -530294107);

		[__DynamicallyInvokable]
		public static readonly OpCode Stind_I8 = new OpCode(OpCodeValues.Stind_I8, -530290011);

		[__DynamicallyInvokable]
		public static readonly OpCode Stind_R4 = new OpCode(OpCodeValues.Stind_R4, -530281819);

		[__DynamicallyInvokable]
		public static readonly OpCode Stind_R8 = new OpCode(OpCodeValues.Stind_R8, -530277723);

		[__DynamicallyInvokable]
		public static readonly OpCode Add = new OpCode(OpCodeValues.Add, -261739867);

		[__DynamicallyInvokable]
		public static readonly OpCode Sub = new OpCode(OpCodeValues.Sub, -261739867);

		[__DynamicallyInvokable]
		public static readonly OpCode Mul = new OpCode(OpCodeValues.Mul, -261739867);

		[__DynamicallyInvokable]
		public static readonly OpCode Div = new OpCode(OpCodeValues.Div, -261739867);

		[__DynamicallyInvokable]
		public static readonly OpCode Div_Un = new OpCode(OpCodeValues.Div_Un, -261739867);

		[__DynamicallyInvokable]
		public static readonly OpCode Rem = new OpCode(OpCodeValues.Rem, -261739867);

		[__DynamicallyInvokable]
		public static readonly OpCode Rem_Un = new OpCode(OpCodeValues.Rem_Un, -261739867);

		[__DynamicallyInvokable]
		public static readonly OpCode And = new OpCode(OpCodeValues.And, -261739867);

		[__DynamicallyInvokable]
		public static readonly OpCode Or = new OpCode(OpCodeValues.Or, -261739867);

		[__DynamicallyInvokable]
		public static readonly OpCode Xor = new OpCode(OpCodeValues.Xor, -261739867);

		[__DynamicallyInvokable]
		public static readonly OpCode Shl = new OpCode(OpCodeValues.Shl, -261739867);

		[__DynamicallyInvokable]
		public static readonly OpCode Shr = new OpCode(OpCodeValues.Shr, -261739867);

		[__DynamicallyInvokable]
		public static readonly OpCode Shr_Un = new OpCode(OpCodeValues.Shr_Un, -261739867);

		[__DynamicallyInvokable]
		public static readonly OpCode Neg = new OpCode(OpCodeValues.Neg, 6691493);

		[__DynamicallyInvokable]
		public static readonly OpCode Not = new OpCode(OpCodeValues.Not, 6691493);

		[__DynamicallyInvokable]
		public static readonly OpCode Conv_I1 = new OpCode(OpCodeValues.Conv_I1, 6953637);

		[__DynamicallyInvokable]
		public static readonly OpCode Conv_I2 = new OpCode(OpCodeValues.Conv_I2, 6953637);

		[__DynamicallyInvokable]
		public static readonly OpCode Conv_I4 = new OpCode(OpCodeValues.Conv_I4, 6953637);

		[__DynamicallyInvokable]
		public static readonly OpCode Conv_I8 = new OpCode(OpCodeValues.Conv_I8, 7084709);

		[__DynamicallyInvokable]
		public static readonly OpCode Conv_R4 = new OpCode(OpCodeValues.Conv_R4, 7215781);

		[__DynamicallyInvokable]
		public static readonly OpCode Conv_R8 = new OpCode(OpCodeValues.Conv_R8, 7346853);

		[__DynamicallyInvokable]
		public static readonly OpCode Conv_U4 = new OpCode(OpCodeValues.Conv_U4, 6953637);

		[__DynamicallyInvokable]
		public static readonly OpCode Conv_U8 = new OpCode(OpCodeValues.Conv_U8, 7084709);

		[__DynamicallyInvokable]
		public static readonly OpCode Callvirt = new OpCode(OpCodeValues.Callvirt, 7841348);

		[__DynamicallyInvokable]
		public static readonly OpCode Cpobj = new OpCode(OpCodeValues.Cpobj, -530295123);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldobj = new OpCode(OpCodeValues.Ldobj, 6698669);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldstr = new OpCode(OpCodeValues.Ldstr, 275908266);

		[__DynamicallyInvokable]
		public static readonly OpCode Newobj = new OpCode(OpCodeValues.Newobj, 276014660);

		[ComVisible(true)]
		[__DynamicallyInvokable]
		public static readonly OpCode Castclass = new OpCode(OpCodeValues.Castclass, 7513773);

		[__DynamicallyInvokable]
		public static readonly OpCode Isinst = new OpCode(OpCodeValues.Isinst, 6989485);

		[__DynamicallyInvokable]
		public static readonly OpCode Conv_R_Un = new OpCode(OpCodeValues.Conv_R_Un, 7346853);

		[__DynamicallyInvokable]
		public static readonly OpCode Unbox = new OpCode(OpCodeValues.Unbox, 6990509);

		[__DynamicallyInvokable]
		public static readonly OpCode Throw = new OpCode(OpCodeValues.Throw, -245061883);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldfld = new OpCode(OpCodeValues.Ldfld, 6727329);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldflda = new OpCode(OpCodeValues.Ldflda, 6989473);

		[__DynamicallyInvokable]
		public static readonly OpCode Stfld = new OpCode(OpCodeValues.Stfld, -530270559);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldsfld = new OpCode(OpCodeValues.Ldsfld, 275121825);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldsflda = new OpCode(OpCodeValues.Ldsflda, 275383969);

		[__DynamicallyInvokable]
		public static readonly OpCode Stsfld = new OpCode(OpCodeValues.Stsfld, -261876063);

		[__DynamicallyInvokable]
		public static readonly OpCode Stobj = new OpCode(OpCodeValues.Stobj, -530298195);

		[__DynamicallyInvokable]
		public static readonly OpCode Conv_Ovf_I1_Un = new OpCode(OpCodeValues.Conv_Ovf_I1_Un, 6953637);

		[__DynamicallyInvokable]
		public static readonly OpCode Conv_Ovf_I2_Un = new OpCode(OpCodeValues.Conv_Ovf_I2_Un, 6953637);

		[__DynamicallyInvokable]
		public static readonly OpCode Conv_Ovf_I4_Un = new OpCode(OpCodeValues.Conv_Ovf_I4_Un, 6953637);

		[__DynamicallyInvokable]
		public static readonly OpCode Conv_Ovf_I8_Un = new OpCode(OpCodeValues.Conv_Ovf_I8_Un, 7084709);

		[__DynamicallyInvokable]
		public static readonly OpCode Conv_Ovf_U1_Un = new OpCode(OpCodeValues.Conv_Ovf_U1_Un, 6953637);

		[__DynamicallyInvokable]
		public static readonly OpCode Conv_Ovf_U2_Un = new OpCode(OpCodeValues.Conv_Ovf_U2_Un, 6953637);

		[__DynamicallyInvokable]
		public static readonly OpCode Conv_Ovf_U4_Un = new OpCode(OpCodeValues.Conv_Ovf_U4_Un, 6953637);

		[__DynamicallyInvokable]
		public static readonly OpCode Conv_Ovf_U8_Un = new OpCode(OpCodeValues.Conv_Ovf_U8_Un, 7084709);

		[__DynamicallyInvokable]
		public static readonly OpCode Conv_Ovf_I_Un = new OpCode(OpCodeValues.Conv_Ovf_I_Un, 6953637);

		[__DynamicallyInvokable]
		public static readonly OpCode Conv_Ovf_U_Un = new OpCode(OpCodeValues.Conv_Ovf_U_Un, 6953637);

		[__DynamicallyInvokable]
		public static readonly OpCode Box = new OpCode(OpCodeValues.Box, 7477933);

		[__DynamicallyInvokable]
		public static readonly OpCode Newarr = new OpCode(OpCodeValues.Newarr, 7485101);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldlen = new OpCode(OpCodeValues.Ldlen, 6989477);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldelema = new OpCode(OpCodeValues.Ldelema, -261437779);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldelem_I1 = new OpCode(OpCodeValues.Ldelem_I1, -261437787);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldelem_U1 = new OpCode(OpCodeValues.Ldelem_U1, -261437787);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldelem_I2 = new OpCode(OpCodeValues.Ldelem_I2, -261437787);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldelem_U2 = new OpCode(OpCodeValues.Ldelem_U2, -261437787);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldelem_I4 = new OpCode(OpCodeValues.Ldelem_I4, -261437787);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldelem_U4 = new OpCode(OpCodeValues.Ldelem_U4, -261437787);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldelem_I8 = new OpCode(OpCodeValues.Ldelem_I8, -261306715);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldelem_I = new OpCode(OpCodeValues.Ldelem_I, -261437787);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldelem_R4 = new OpCode(OpCodeValues.Ldelem_R4, -261175643);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldelem_R8 = new OpCode(OpCodeValues.Ldelem_R8, -261044571);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldelem_Ref = new OpCode(OpCodeValues.Ldelem_Ref, -260913499);

		[__DynamicallyInvokable]
		public static readonly OpCode Stelem_I = new OpCode(OpCodeValues.Stelem_I, -798697819);

		[__DynamicallyInvokable]
		public static readonly OpCode Stelem_I1 = new OpCode(OpCodeValues.Stelem_I1, -798697819);

		[__DynamicallyInvokable]
		public static readonly OpCode Stelem_I2 = new OpCode(OpCodeValues.Stelem_I2, -798697819);

		[__DynamicallyInvokable]
		public static readonly OpCode Stelem_I4 = new OpCode(OpCodeValues.Stelem_I4, -798697819);

		[__DynamicallyInvokable]
		public static readonly OpCode Stelem_I8 = new OpCode(OpCodeValues.Stelem_I8, -798693723);

		[__DynamicallyInvokable]
		public static readonly OpCode Stelem_R4 = new OpCode(OpCodeValues.Stelem_R4, -798689627);

		[__DynamicallyInvokable]
		public static readonly OpCode Stelem_R8 = new OpCode(OpCodeValues.Stelem_R8, -798685531);

		[__DynamicallyInvokable]
		public static readonly OpCode Stelem_Ref = new OpCode(OpCodeValues.Stelem_Ref, -798681435);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldelem = new OpCode(OpCodeValues.Ldelem, -261699923);

		[__DynamicallyInvokable]
		public static readonly OpCode Stelem = new OpCode(OpCodeValues.Stelem, 6669997);

		[__DynamicallyInvokable]
		public static readonly OpCode Unbox_Any = new OpCode(OpCodeValues.Unbox_Any, 6727341);

		[__DynamicallyInvokable]
		public static readonly OpCode Conv_Ovf_I1 = new OpCode(OpCodeValues.Conv_Ovf_I1, 6953637);

		[__DynamicallyInvokable]
		public static readonly OpCode Conv_Ovf_U1 = new OpCode(OpCodeValues.Conv_Ovf_U1, 6953637);

		[__DynamicallyInvokable]
		public static readonly OpCode Conv_Ovf_I2 = new OpCode(OpCodeValues.Conv_Ovf_I2, 6953637);

		[__DynamicallyInvokable]
		public static readonly OpCode Conv_Ovf_U2 = new OpCode(OpCodeValues.Conv_Ovf_U2, 6953637);

		[__DynamicallyInvokable]
		public static readonly OpCode Conv_Ovf_I4 = new OpCode(OpCodeValues.Conv_Ovf_I4, 6953637);

		[__DynamicallyInvokable]
		public static readonly OpCode Conv_Ovf_U4 = new OpCode(OpCodeValues.Conv_Ovf_U4, 6953637);

		[__DynamicallyInvokable]
		public static readonly OpCode Conv_Ovf_I8 = new OpCode(OpCodeValues.Conv_Ovf_I8, 7084709);

		[__DynamicallyInvokable]
		public static readonly OpCode Conv_Ovf_U8 = new OpCode(OpCodeValues.Conv_Ovf_U8, 7084709);

		[__DynamicallyInvokable]
		public static readonly OpCode Refanyval = new OpCode(OpCodeValues.Refanyval, 6953645);

		[__DynamicallyInvokable]
		public static readonly OpCode Ckfinite = new OpCode(OpCodeValues.Ckfinite, 7346853);

		[__DynamicallyInvokable]
		public static readonly OpCode Mkrefany = new OpCode(OpCodeValues.Mkrefany, 6699693);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldtoken = new OpCode(OpCodeValues.Ldtoken, 275385004);

		[__DynamicallyInvokable]
		public static readonly OpCode Conv_U2 = new OpCode(OpCodeValues.Conv_U2, 6953637);

		[__DynamicallyInvokable]
		public static readonly OpCode Conv_U1 = new OpCode(OpCodeValues.Conv_U1, 6953637);

		[__DynamicallyInvokable]
		public static readonly OpCode Conv_I = new OpCode(OpCodeValues.Conv_I, 6953637);

		[__DynamicallyInvokable]
		public static readonly OpCode Conv_Ovf_I = new OpCode(OpCodeValues.Conv_Ovf_I, 6953637);

		[__DynamicallyInvokable]
		public static readonly OpCode Conv_Ovf_U = new OpCode(OpCodeValues.Conv_Ovf_U, 6953637);

		[__DynamicallyInvokable]
		public static readonly OpCode Add_Ovf = new OpCode(OpCodeValues.Add_Ovf, -261739867);

		[__DynamicallyInvokable]
		public static readonly OpCode Add_Ovf_Un = new OpCode(OpCodeValues.Add_Ovf_Un, -261739867);

		[__DynamicallyInvokable]
		public static readonly OpCode Mul_Ovf = new OpCode(OpCodeValues.Mul_Ovf, -261739867);

		[__DynamicallyInvokable]
		public static readonly OpCode Mul_Ovf_Un = new OpCode(OpCodeValues.Mul_Ovf_Un, -261739867);

		[__DynamicallyInvokable]
		public static readonly OpCode Sub_Ovf = new OpCode(OpCodeValues.Sub_Ovf, -261739867);

		[__DynamicallyInvokable]
		public static readonly OpCode Sub_Ovf_Un = new OpCode(OpCodeValues.Sub_Ovf_Un, -261739867);

		[__DynamicallyInvokable]
		public static readonly OpCode Endfinally = new OpCode(OpCodeValues.Endfinally, 23333605);

		[__DynamicallyInvokable]
		public static readonly OpCode Leave = new OpCode(OpCodeValues.Leave, 23333376);

		[__DynamicallyInvokable]
		public static readonly OpCode Leave_S = new OpCode(OpCodeValues.Leave_S, 23333391);

		[__DynamicallyInvokable]
		public static readonly OpCode Stind_I = new OpCode(OpCodeValues.Stind_I, -530294107);

		[__DynamicallyInvokable]
		public static readonly OpCode Conv_U = new OpCode(OpCodeValues.Conv_U, 6953637);

		[__DynamicallyInvokable]
		public static readonly OpCode Prefix7 = new OpCode(OpCodeValues.Prefix7, 6554757);

		[__DynamicallyInvokable]
		public static readonly OpCode Prefix6 = new OpCode(OpCodeValues.Prefix6, 6554757);

		[__DynamicallyInvokable]
		public static readonly OpCode Prefix5 = new OpCode(OpCodeValues.Prefix5, 6554757);

		[__DynamicallyInvokable]
		public static readonly OpCode Prefix4 = new OpCode(OpCodeValues.Prefix4, 6554757);

		[__DynamicallyInvokable]
		public static readonly OpCode Prefix3 = new OpCode(OpCodeValues.Prefix3, 6554757);

		[__DynamicallyInvokable]
		public static readonly OpCode Prefix2 = new OpCode(OpCodeValues.Prefix2, 6554757);

		[__DynamicallyInvokable]
		public static readonly OpCode Prefix1 = new OpCode(OpCodeValues.Prefix1, 6554757);

		[__DynamicallyInvokable]
		public static readonly OpCode Prefixref = new OpCode(OpCodeValues.Prefixref, 6554757);

		[__DynamicallyInvokable]
		public static readonly OpCode Arglist = new OpCode(OpCodeValues.Arglist, 279579301);

		[__DynamicallyInvokable]
		public static readonly OpCode Ceq = new OpCode(OpCodeValues.Ceq, -257283419);

		[__DynamicallyInvokable]
		public static readonly OpCode Cgt = new OpCode(OpCodeValues.Cgt, -257283419);

		[__DynamicallyInvokable]
		public static readonly OpCode Cgt_Un = new OpCode(OpCodeValues.Cgt_Un, -257283419);

		[__DynamicallyInvokable]
		public static readonly OpCode Clt = new OpCode(OpCodeValues.Clt, -257283419);

		[__DynamicallyInvokable]
		public static readonly OpCode Clt_Un = new OpCode(OpCodeValues.Clt_Un, -257283419);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldftn = new OpCode(OpCodeValues.Ldftn, 279579300);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldvirtftn = new OpCode(OpCodeValues.Ldvirtftn, 11184804);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldarg = new OpCode(OpCodeValues.Ldarg, 279317166);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldarga = new OpCode(OpCodeValues.Ldarga, 279579310);

		[__DynamicallyInvokable]
		public static readonly OpCode Starg = new OpCode(OpCodeValues.Starg, -257680722);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldloc = new OpCode(OpCodeValues.Ldloc, 279317166);

		[__DynamicallyInvokable]
		public static readonly OpCode Ldloca = new OpCode(OpCodeValues.Ldloca, 279579310);

		[__DynamicallyInvokable]
		public static readonly OpCode Stloc = new OpCode(OpCodeValues.Stloc, -257680722);

		[__DynamicallyInvokable]
		public static readonly OpCode Localloc = new OpCode(OpCodeValues.Localloc, 11156133);

		[__DynamicallyInvokable]
		public static readonly OpCode Endfilter = new OpCode(OpCodeValues.Endfilter, -240895259);

		[__DynamicallyInvokable]
		public static readonly OpCode Unaligned = new OpCode(OpCodeValues.Unaligned_, 10750096);

		[__DynamicallyInvokable]
		public static readonly OpCode Volatile = new OpCode(OpCodeValues.Volatile_, 10750085);

		[__DynamicallyInvokable]
		public static readonly OpCode Tailcall = new OpCode(OpCodeValues.Tail_, 10750085);

		[__DynamicallyInvokable]
		public static readonly OpCode Initobj = new OpCode(OpCodeValues.Initobj, -257673555);

		[__DynamicallyInvokable]
		public static readonly OpCode Constrained = new OpCode(OpCodeValues.Constrained_, 10750093);

		[__DynamicallyInvokable]
		public static readonly OpCode Cpblk = new OpCode(OpCodeValues.Cpblk, -794527067);

		[__DynamicallyInvokable]
		public static readonly OpCode Initblk = new OpCode(OpCodeValues.Initblk, -794527067);

		[__DynamicallyInvokable]
		public static readonly OpCode Rethrow = new OpCode(OpCodeValues.Rethrow, 27526917);

		[__DynamicallyInvokable]
		public static readonly OpCode Sizeof = new OpCode(OpCodeValues.Sizeof, 279579309);

		[__DynamicallyInvokable]
		public static readonly OpCode Refanytype = new OpCode(OpCodeValues.Refanytype, 11147941);

		[__DynamicallyInvokable]
		public static readonly OpCode Readonly = new OpCode(OpCodeValues.Readonly_, 10750085);
	}
}
