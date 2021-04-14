using System;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal interface IRequireCharBuffer
	{
		char[] CharBuffer { set; }
	}
}
