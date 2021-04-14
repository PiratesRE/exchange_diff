using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class GetMime : OwaForm, IRegistryOnlyForm
	{
		protected override void OnPreRender(EventArgs e)
		{
		}

		protected override void OnLoad(EventArgs e)
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "GetMime.OnLoad");
			base.OnLoad(e);
			base.InitializeAsMessageItem(new PropertyDefinition[0]);
			base.Item.Load(StoreObjectSchema.ContentConversionProperties);
			base.Response.AppendHeader("X-OWA-EventResult", "0");
			base.Response.ContentType = Utilities.GetContentTypeString(OwaEventContentType.PlainText);
		}

		protected void RenderMime()
		{
			OutboundConversionOptions outboundConversionOptions = Utilities.CreateOutboundConversionOptions(base.UserContext);
			outboundConversionOptions.AllowPartialStnefConversion = true;
			outboundConversionOptions.DemoteBcc = true;
			try
			{
				ItemConversion.ConvertItemToMime(base.Item, base.Response.OutputStream, outboundConversionOptions);
			}
			catch (NotSupportedException innerException)
			{
				throw new OwaInvalidRequestException("Conversion failed", innerException);
			}
			catch (NotImplementedException innerException2)
			{
				throw new OwaInvalidRequestException("Conversion failed", innerException2);
			}
			catch (StoragePermanentException innerException3)
			{
				throw new OwaInvalidRequestException("Conversion failed", innerException3);
			}
			catch (StorageTransientException innerException4)
			{
				throw new OwaInvalidRequestException("Conversion failed", innerException4);
			}
		}
	}
}
