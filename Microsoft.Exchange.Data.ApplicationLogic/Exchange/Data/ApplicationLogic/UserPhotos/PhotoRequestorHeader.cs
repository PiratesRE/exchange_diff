using System;
using System.Net;
using System.Text;
using System.Web;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PhotoRequestorHeader
	{
		private PhotoRequestorHeader()
		{
		}

		public void Serialize(PhotoPrincipal requestor, WebRequest request)
		{
			ArgumentValidator.ThrowIfNull("requestor", requestor);
			ArgumentValidator.ThrowIfNull("request", request);
			if (requestor.OrganizationId == null)
			{
				return;
			}
			request.Headers.Set("X-Exchange-Photos-Requestor-Organization-Id", PhotoRequestorHeader.SerializeOrganizationId(requestor.OrganizationId));
		}

		public PhotoPrincipal Deserialize(HttpRequest request, ITracer tracer)
		{
			ArgumentValidator.ThrowIfNull("request", request);
			ArgumentValidator.ThrowIfNull("tracer", tracer);
			return new PhotoPrincipal
			{
				OrganizationId = this.DeserializeRequestorOrganizationId(request, tracer)
			};
		}

		private static string SerializeOrganizationId(OrganizationId id)
		{
			return Convert.ToBase64String(id.GetBytes(PhotoRequestorHeader.SerializedOrganizationIdEncoding));
		}

		private OrganizationId DeserializeRequestorOrganizationId(HttpRequest request, ITracer tracer)
		{
			string text = request.Headers["X-Exchange-Photos-Requestor-Organization-Id"];
			if (string.IsNullOrEmpty(text))
			{
				tracer.TraceDebug((long)this.GetHashCode(), "Cannot deserialize requestor's organization id because it was not found or is blank in request.");
				return null;
			}
			byte[] bytes;
			try
			{
				bytes = Convert.FromBase64String(text);
			}
			catch (FormatException arg)
			{
				tracer.TraceError<string, FormatException>((long)this.GetHashCode(), "Failed to deserialize requestor's organization id from base64 string: {0}.  Exception: {1}", text, arg);
				return null;
			}
			OrganizationId organizationId;
			if (!OrganizationId.TryCreateFromBytes(bytes, PhotoRequestorHeader.SerializedOrganizationIdEncoding, out organizationId))
			{
				tracer.TraceError<string>((long)this.GetHashCode(), "Failed to deserialize requestor's organization id from base64 string: {0}", text);
				return null;
			}
			tracer.TraceDebug<OrganizationId>((long)this.GetHashCode(), "Deserialized requestor's organization id: {0}", organizationId);
			return organizationId;
		}

		private const string PhotoRequestorHeaderName = "X-Exchange-Photos-Requestor-Organization-Id";

		private static readonly Encoding SerializedOrganizationIdEncoding = Encoding.UTF8;

		internal static readonly PhotoRequestorHeader Default = new PhotoRequestorHeader();
	}
}
