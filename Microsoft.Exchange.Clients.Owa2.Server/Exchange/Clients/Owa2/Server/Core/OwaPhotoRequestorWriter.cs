using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Performance;
using Microsoft.Exchange.Data.ApplicationLogic.UserPhotos;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Performance;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class OwaPhotoRequestorWriter
	{
		public OwaPhotoRequestorWriter(IPerformanceDataLogger perfLogger, ITracer upstreamTracer)
		{
			ArgumentValidator.ThrowIfNull("perfLogger", perfLogger);
			ArgumentValidator.ThrowIfNull("upstreamTracer", upstreamTracer);
			this.perfLogger = perfLogger;
			this.tracer = upstreamTracer;
		}

		public HttpContext Write(HttpContext authenticatedUserContext, HttpContext output)
		{
			ArgumentValidator.ThrowIfNull("authenticatedUserContext", authenticatedUserContext);
			ArgumentValidator.ThrowIfNull("output", output);
			HttpContext result;
			try
			{
				using (new StopwatchPerformanceTracker("PhotoRequestorWriterResolveIdentity", this.perfLogger))
				{
					using (new ADPerformanceTracker("PhotoRequestorWriterResolveIdentity", this.perfLogger))
					{
						using (OwaIdentity owaIdentity = OwaIdentity.ResolveLogonIdentity(authenticatedUserContext, null))
						{
							if (owaIdentity == null)
							{
								this.tracer.TraceDebug((long)this.GetHashCode(), "OwaPhotoRequestorWriter:  requestor could NOT be resolved.");
								return output;
							}
							OrganizationId organization = OwaPhotoRequestorWriter.GetOrganization(owaIdentity);
							if (organization == null)
							{
								this.tracer.TraceError((long)this.GetHashCode(), "OwaPhotoRequestorWriter:  could NOT determine requestor's organization.");
								return output;
							}
							output.Items["Photo.Requestor"] = new PhotoPrincipal
							{
								OrganizationId = organization,
								EmailAddresses = this.ExtractSmtpAddresses(owaIdentity)
							};
							output.Items["Photo.Requestor.EnabledInFasterPhotoFlightHttpContextKey"] = OwaPhotoRequestorWriter.InPhotoFasterPhotoFlight(owaIdentity.OwaMiniRecipient);
						}
					}
				}
				result = output;
			}
			catch (OwaADObjectNotFoundException arg)
			{
				this.tracer.TraceError<OwaADObjectNotFoundException>((long)this.GetHashCode(), "OwaPhotoRequestorWriter:  requestor NOT found.  Exception: {0}", arg);
				result = output;
			}
			catch (TransientException arg2)
			{
				this.tracer.TraceError<TransientException>((long)this.GetHashCode(), "OwaPhotoRequestorWriter:  failed to resolve requestor with a transient error.  Exception: {0}", arg2);
				result = output;
			}
			catch (ADOperationException arg3)
			{
				this.tracer.TraceError<ADOperationException>((long)this.GetHashCode(), "OwaPhotoRequestorWriter:  failed to resolve requestor with permanent directory error.  Exception: {0}", arg3);
				result = output;
			}
			return result;
		}

		private static OrganizationId GetOrganization(OwaIdentity requestor)
		{
			if (requestor.UserOrganizationId != null)
			{
				return requestor.UserOrganizationId;
			}
			OWAMiniRecipient owaminiRecipient = requestor.OwaMiniRecipient ?? requestor.GetOWAMiniRecipient();
			if (owaminiRecipient != null)
			{
				return owaminiRecipient.OrganizationId;
			}
			return null;
		}

		private ICollection<string> ExtractSmtpAddresses(OwaIdentity requestor)
		{
			if (requestor == null)
			{
				return Array<string>.Empty;
			}
			OWAMiniRecipient owaminiRecipient = requestor.OwaMiniRecipient ?? requestor.GetOWAMiniRecipient();
			if (owaminiRecipient == null)
			{
				this.tracer.TraceDebug((long)this.GetHashCode(), "OwaPhotoRequestorWriter:  cannot extract SMTP addresses because recipient information has NOT been initialized or computed for requestor.");
				return Array<string>.Empty;
			}
			HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			SmtpAddress primarySmtpAddress = requestor.PrimarySmtpAddress;
			hashSet.Add(requestor.PrimarySmtpAddress.ToString());
			if (owaminiRecipient.EmailAddresses != null && owaminiRecipient.EmailAddresses.Count > 0)
			{
				hashSet.UnionWith(from a in owaminiRecipient.EmailAddresses
				where OwaPhotoRequestorWriter.IsNonBlankSmtpAddress(a)
				select a.ValueString);
			}
			return hashSet;
		}

		private static bool IsNonBlankSmtpAddress(ProxyAddress address)
		{
			return !(address == null) && ProxyAddressPrefix.Smtp.Equals(address.Prefix) && !string.IsNullOrWhiteSpace(address.ValueString);
		}

		private static bool InPhotoFasterPhotoFlight(OWAMiniRecipient requestor)
		{
			return requestor != null && VariantConfiguration.GetSnapshot(requestor.GetContext(null), null, null).OwaClientServer.FasterPhoto.Enabled;
		}

		private const string RequestorKeyInHttpContext = "Photo.Requestor";

		private const string EnabledInFasterPhotoFlightKeyInHttpContext = "Photo.Requestor.EnabledInFasterPhotoFlightHttpContextKey";

		private readonly ITracer tracer = NullTracer.Instance;

		private readonly IPerformanceDataLogger perfLogger = NullPerformanceDataLogger.Instance;
	}
}
