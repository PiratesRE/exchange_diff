using System;
using System.Security;

namespace System.Runtime.Serialization.Formatters.Binary
{
	internal interface IStreamable
	{
		[SecurityCritical]
		void Read(__BinaryParser input);

		void Write(__BinaryWriter sout);
	}
}
