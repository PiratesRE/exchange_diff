using System;
using System.Runtime.InteropServices;

namespace System.Diagnostics.SymbolStore
{
	[ComVisible(true)]
	public class SymLanguageType
	{
		public static readonly Guid C = new Guid(1671464724, -969, 4562, 144, 76, 0, 192, 79, 163, 2, 161);

		public static readonly Guid CPlusPlus = new Guid(974311607, -15764, 4560, 180, 66, 0, 160, 36, 74, 29, 210);

		public static readonly Guid CSharp = new Guid(1062298360, 1990, 4563, 144, 83, 0, 192, 79, 163, 2, 161);

		public static readonly Guid Basic = new Guid(974311608, -15764, 4560, 180, 66, 0, 160, 36, 74, 29, 210);

		public static readonly Guid Java = new Guid(974311604, -15764, 4560, 180, 66, 0, 160, 36, 74, 29, 210);

		public static readonly Guid Cobol = new Guid(-1358664495, -12063, 4562, 151, 124, 0, 160, 201, 180, 213, 12);

		public static readonly Guid Pascal = new Guid(-1358664494, -12063, 4562, 151, 124, 0, 160, 201, 180, 213, 12);

		public static readonly Guid ILAssembly = new Guid(-1358664493, -12063, 4562, 151, 124, 0, 160, 201, 180, 213, 12);

		public static readonly Guid JScript = new Guid(974311606, -15764, 4560, 180, 66, 0, 160, 36, 74, 29, 210);

		public static readonly Guid SMC = new Guid(228302715, 26129, 4563, 189, 42, 0, 0, 248, 8, 73, 189);

		public static readonly Guid MCPlusPlus = new Guid(1261829608, 1990, 4563, 144, 83, 0, 192, 79, 163, 2, 161);
	}
}
