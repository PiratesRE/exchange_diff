using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class RuleActions
	{
		internal unsafe static RuleAction[] Unmarshal(IntPtr Handle)
		{
			_Actions* ptr = (_Actions*)Handle.ToPointer();
			if (1U != ptr->ulVersion)
			{
				return null;
			}
			RuleAction[] array = new RuleAction[ptr->cActions];
			_Action* lpAction = ptr->lpAction;
			for (uint num = 0U; num < ptr->cActions; num += 1U)
			{
				array[(int)((UIntPtr)num)] = RuleAction.Unmarshal(lpAction + (ulong)num * (ulong)((long)sizeof(_Action)) / (ulong)sizeof(_Action));
			}
			return array;
		}

		internal static int GetBytesToMarshal(params RuleAction[] actions)
		{
			int num = _Actions.SizeOf + 7 & -8;
			if (actions != null)
			{
				for (int i = 0; i < actions.Length; i++)
				{
					num += actions[i].GetBytesToMarshal();
				}
			}
			return num;
		}

		internal unsafe static void MarshalToNative(ref byte* pb, params RuleAction[] actions)
		{
			byte* ptr = pb;
			_Actions* ptr2 = pb;
			ptr2->ulVersion = 1U;
			ptr += (_Actions.SizeOf + 7 & -8);
			if (actions != null)
			{
				ptr2->cActions = (uint)actions.Length;
				ptr2->lpAction = (_Action*)ptr;
				_Action* ptr3 = ptr2->lpAction;
				ptr += (IntPtr)(_Action.SizeOf + 7 & -8) * (IntPtr)actions.Length;
				foreach (RuleAction ruleAction in actions)
				{
					ruleAction.MarshalToNative(ptr3, ref ptr);
					ptr3++;
				}
			}
			else
			{
				ptr2->cActions = 0U;
			}
			pb = ptr;
		}
	}
}
