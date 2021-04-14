using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventNamespace("ReadADOrgPerson")]
	internal sealed class ReadADOrgPersonEventHandler : OwaEventHandlerBase
	{
		public static void Register()
		{
			OwaEventRegistry.RegisterHandler(typeof(ReadADOrgPersonEventHandler));
		}

		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEventParameter("id", typeof(ADObjectId))]
		[OwaEventParameter("SD", typeof(ExDateTime), false, true)]
		[OwaEventParameter("ED", typeof(ExDateTime), false, true)]
		[OwaEventParameter("EA", typeof(string), false, true)]
		[OwaEvent("LID")]
		public void LoadInitialData()
		{
			ExTraceGlobals.OehCallTracer.TraceDebug((long)this.GetHashCode(), "ReadADOrgPersonEventHandler.LoadInitialData");
			IRecipientSession recipientSession = Utilities.CreateADRecipientSession(Culture.GetUserCulture().LCID, true, ConsistencyMode.IgnoreInvalid, true, base.UserContext, false);
			IADOrgPerson iadorgPerson = recipientSession.Read((ADObjectId)base.GetParameter("id")) as IADOrgPerson;
			if (iadorgPerson == null)
			{
				throw new OwaInvalidRequestException("couldn't find person");
			}
			StringBuilder stringBuilder = new StringBuilder();
			StringWriter stringWriter = new StringWriter(stringBuilder);
			ReadADOrgPerson.RenderOrganizationContents(stringWriter, base.UserContext, iadorgPerson, recipientSession);
			stringWriter.Close();
			this.Writer.Write("sOrg = '");
			Utilities.JavascriptEncode(stringBuilder.ToString(), this.Writer);
			this.Writer.Write("';");
			if (base.IsParameterSet("EA") && base.UserContext.IsFeatureEnabled(Feature.Calendar))
			{
				string text = this.RenderFreeBusyData((string)base.GetParameter("EA"), (ExDateTime)base.GetParameter("SD"), (ExDateTime)base.GetParameter("ED"), true);
				if (text != null)
				{
					this.Writer.Write("sFBErr = \"");
					Utilities.JavascriptEncode(text, this.Writer);
					this.Writer.Write("\";");
				}
			}
		}

		[OwaEventParameter("EA", typeof(string))]
		[OwaEventParameter("SD", typeof(ExDateTime))]
		[OwaEventParameter("ED", typeof(ExDateTime))]
		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEvent("GetFreeBusy")]
		[OwaEventSegmentation(Feature.Calendar)]
		public void GetFreeBusyData()
		{
			ExTraceGlobals.OehCallTracer.TraceDebug((long)this.GetHashCode(), "ReadADOrgPersonEventHandler.GetFreeBusy");
			string text = this.RenderFreeBusyData((string)base.GetParameter("EA"), (ExDateTime)base.GetParameter("SD"), (ExDateTime)base.GetParameter("ED"), false);
			if (text != null)
			{
				throw new OwaEventHandlerException("Unable to get free busy data", text);
			}
		}

		private string RenderFreeBusyData(string smtpAddress, ExDateTime startDate, ExDateTime endDate, bool renderOof)
		{
			string value;
			string value2;
			string text;
			string freeBusy = ReadADOrgPerson.GetFreeBusy(base.OwaContext, smtpAddress, startDate, endDate, this.HttpContext, out value, out value2, out text);
			if (freeBusy != null)
			{
				return freeBusy;
			}
			this.Writer.Write("rgFbd = new Array('");
			this.Writer.Write(value);
			this.Writer.Write("','");
			this.Writer.Write(value2);
			this.Writer.Write("');");
			if (renderOof && text != null)
			{
				this.Writer.Write("sOof = \"");
				Utilities.JavascriptEncode(text, this.Writer);
				this.Writer.Write("\";");
			}
			return null;
		}

		public const string EventNamespace = "ReadADOrgPerson";

		public const string MethodGetFreeBusyData = "GetFreeBusy";

		public const string MethodLoadInitalData = "LID";

		public const string Id = "id";

		public const string StartDate = "SD";

		public const string EndDate = "ED";

		public const string EmailAddress = "EA";
	}
}
