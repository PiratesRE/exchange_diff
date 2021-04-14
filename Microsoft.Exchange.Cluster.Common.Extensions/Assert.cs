using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Common.Extensions
{
	internal class Assert : IAssert
	{
		private Assert()
		{
		}

		public static Assert Instance
		{
			get
			{
				return Assert.instance;
			}
		}

		public void Debug(bool condition, string formatString, params object[] parameters)
		{
		}

		public void Retail(bool condition, string formatString, params object[] parameters)
		{
			ExAssert.RetailAssert(condition, formatString, parameters);
		}

		[DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool IsDebuggerPresent();

		[DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "DebugBreak")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool Kernel32DebugBreak();

		[DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "OutputDebugStringW")]
		private static extern void OutputDebugString(string message);

		private bool IsDebuggerAttached()
		{
			return Debugger.IsAttached || Assert.IsDebuggerPresent();
		}

		private void DebugBreak()
		{
			if (Debugger.IsAttached)
			{
				Debugger.Break();
				return;
			}
			if (Assert.IsDebuggerPresent())
			{
				Assert.Kernel32DebugBreak();
				return;
			}
			Debugger.Break();
		}

		private void PrintToDebugger(string message)
		{
			if (Assert.IsDebuggerPresent())
			{
				Assert.OutputDebugString(message);
			}
		}

		private static Assert instance = new Assert();
	}
}
