using System;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class SanitizeHtmlCommand : ServiceCommand<string>
	{
		public SanitizeHtmlCommand(CallContext callContext, string body) : base(callContext)
		{
			this.body = body;
		}

		public static string CleanHtml(string input)
		{
			string text = null;
			HtmlToHtml htmlToHtml = new HtmlToHtml();
			htmlToHtml.FilterHtml = true;
			htmlToHtml.OutputHtmlFragment = true;
			string result;
			using (TextReader textReader = new StringReader(input))
			{
				using (TextWriter textWriter = new StringWriter())
				{
					try
					{
						htmlToHtml.Convert(textReader, textWriter);
						text = textWriter.ToString();
					}
					catch (ExchangeDataException innerException)
					{
						throw FaultExceptionUtilities.CreateFault(new OwaCannotSanitizeHtmlException("Sanitization of the HTML failed", innerException, htmlToHtml), FaultParty.Sender);
					}
					result = text;
				}
			}
			return result;
		}

		protected override string InternalExecute()
		{
			return SanitizeHtmlCommand.CleanHtml(this.body);
		}

		private readonly string body;
	}
}
