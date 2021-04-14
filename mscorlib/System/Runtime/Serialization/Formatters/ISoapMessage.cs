using System;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;

namespace System.Runtime.Serialization.Formatters
{
	[ComVisible(true)]
	public interface ISoapMessage
	{
		string[] ParamNames { get; set; }

		object[] ParamValues { get; set; }

		Type[] ParamTypes { get; set; }

		string MethodName { get; set; }

		string XmlNameSpace { get; set; }

		Header[] Headers { get; set; }
	}
}
