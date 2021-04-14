using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage.RightsManagement
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ExternalRMSServerInfo
	{
		public ExternalRMSServerInfo(Uri keyUri) : this(keyUri, null, null, null, null, DateTime.MaxValue)
		{
		}

		public ExternalRMSServerInfo(Uri keyUri, Uri certificationWsUri, Uri certificationWsTargetUri, Uri serverLicensingWsUri, Uri serverLicensingWsTargetUri, DateTime expiryTime)
		{
			if (null == keyUri)
			{
				throw new ArgumentNullException("keyUri");
			}
			this.KeyUri = keyUri;
			this.CertificationWSPipeline = certificationWsUri;
			this.CertificationWSTargetUri = certificationWsTargetUri;
			this.ServerLicensingWSPipeline = serverLicensingWsUri;
			this.ServerLicensingWSTargetUri = serverLicensingWsTargetUri;
			this.ExpiryTime = expiryTime;
		}

		public Uri KeyUri
		{
			get
			{
				return this.keyUri;
			}
			private set
			{
				this.keyUri = value;
			}
		}

		public Uri ServerLicensingWSTargetUri
		{
			get
			{
				return this.serverLicensingWsTargetUri;
			}
			set
			{
				this.serverLicensingWsTargetUri = value;
			}
		}

		public Uri CertificationWSTargetUri
		{
			get
			{
				return this.certificationWsTargetUri;
			}
			set
			{
				this.certificationWsTargetUri = value;
			}
		}

		public Uri ServerLicensingWSPipeline
		{
			get
			{
				return this.serverLicensingWsUri;
			}
			set
			{
				this.serverLicensingWsUri = value;
			}
		}

		public Uri CertificationWSPipeline
		{
			get
			{
				return this.certificationWsUri;
			}
			set
			{
				this.certificationWsUri = value;
			}
		}

		public DateTime ExpiryTime
		{
			get
			{
				return this.expiryTime;
			}
			private set
			{
				this.expiryTime = value;
			}
		}

		public bool IsNegativeEntry
		{
			get
			{
				return this.ExpiryTime != DateTime.MaxValue;
			}
		}

		public static bool TryParse(string[] values, out ExternalRMSServerInfo info)
		{
			info = null;
			if (values == null || values.Length != ExternalRMSServerInfo.ColumnNames.Length)
			{
				ExternalRMSServerInfo.Tracer.TraceError(0L, "External Rms Server Info failed to parse values.");
				return false;
			}
			Uri uri;
			if (!Uri.TryCreate(values[0], UriKind.Absolute, out uri))
			{
				ExternalRMSServerInfo.Tracer.TraceError<string>(0L, "External Rms Server Info failed to parse Key Uri ({0}).", values[0]);
				return false;
			}
			long num;
			if (long.TryParse(values[5], out num) && num >= DateTime.MinValue.Ticks && num <= DateTime.MaxValue.Ticks)
			{
				DateTime d = new DateTime(num);
				bool flag = d != DateTime.MaxValue;
				Uri uri2;
				if (!Uri.TryCreate(values[1], UriKind.Absolute, out uri2) && !flag)
				{
					ExternalRMSServerInfo.Tracer.TraceError<string>(0L, "External Rms Server Info failed to parse CertificationWsUrl ({0}).", values[1]);
				}
				Uri uri3;
				if (!Uri.TryCreate(values[2], UriKind.Absolute, out uri3) && !flag)
				{
					ExternalRMSServerInfo.Tracer.TraceError<string>(0L, "External Rms Server Info failed to parse CertificationWsTargetUri ({0}).", values[2]);
				}
				Uri uri4;
				if (!Uri.TryCreate(values[3], UriKind.Absolute, out uri4) && !flag)
				{
					ExternalRMSServerInfo.Tracer.TraceError<string>(0L, "External Rms Server Info failed to parse ServerLicensingWsUrl ({0}).", values[3]);
				}
				Uri uri5;
				if (!Uri.TryCreate(values[4], UriKind.Absolute, out uri5) && !flag)
				{
					ExternalRMSServerInfo.Tracer.TraceError<string>(0L, "External Rms Server Info failed to parse ServerLicensingWsTargetUri ({0}).", values[4]);
				}
				info = new ExternalRMSServerInfo(uri, uri2, uri3, uri4, uri5, d);
				return true;
			}
			ExternalRMSServerInfo.Tracer.TraceError<string>(0L, "External Rms Server Info failed to parse Expiry Time ({0}).", values[5]);
			return false;
		}

		public void MarkAsNegative()
		{
			if (this.IsNegativeEntry)
			{
				return;
			}
			this.ExpiryTime = DateTime.UtcNow.Add(RmsClientManager.AppSettings.NegativeServerInfoCacheExpirationInterval);
		}

		public string[] ToStringArray()
		{
			return new string[]
			{
				this.KeyUri.ToString(),
				(this.CertificationWSPipeline == null) ? null : this.CertificationWSPipeline.ToString(),
				(this.CertificationWSTargetUri == null) ? null : this.CertificationWSTargetUri.ToString(),
				(this.ServerLicensingWSPipeline == null) ? null : this.ServerLicensingWSPipeline.ToString(),
				(this.ServerLicensingWSTargetUri == null) ? null : this.ServerLicensingWSTargetUri.ToString(),
				this.ExpiryTime.Ticks.ToString()
			};
		}

		public static readonly string[] ColumnNames = new string[]
		{
			"keyUrl",
			"certificationWsUrl",
			"certificationWsTargetUri",
			"serverLicensingWsUrl",
			"serverLicensingWsTargetUri",
			"expiryTime"
		};

		private static readonly Trace Tracer = ExTraceGlobals.RightsManagementTracer;

		private Uri keyUri;

		private Uri serverLicensingWsUri;

		private Uri serverLicensingWsTargetUri;

		private Uri certificationWsUri;

		private Uri certificationWsTargetUri;

		private DateTime expiryTime;
	}
}
