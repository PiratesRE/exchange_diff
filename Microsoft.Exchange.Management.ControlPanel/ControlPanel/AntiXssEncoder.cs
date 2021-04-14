using System;
using System.IO;
using System.Web.Util;
using Microsoft.Security.Application;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class AntiXssEncoder : HttpEncoder
	{
		protected override void HtmlEncode(string value, TextWriter output)
		{
			output.Write(Encoder.HtmlEncode(value));
		}

		protected override void HtmlAttributeEncode(string value, TextWriter output)
		{
			output.Write(Encoder.HtmlAttributeEncode(value));
		}
	}
}
