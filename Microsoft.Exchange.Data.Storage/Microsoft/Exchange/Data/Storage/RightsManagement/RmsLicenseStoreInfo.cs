using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage.RightsManagement
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RmsLicenseStoreInfo
	{
		public RmsLicenseStoreInfo(Guid tenantId, Uri url, string racFile, string clcFile, DateTime racExpire, DateTime clcExpire, byte version)
		{
			if (string.IsNullOrEmpty(racFile))
			{
				throw new ArgumentNullException("racFile");
			}
			if (null == url)
			{
				throw new ArgumentNullException("url");
			}
			if (string.IsNullOrEmpty(clcFile) && RmsLicenseStoreInfo.DefaultUri == url)
			{
				throw new ArgumentNullException("clcFile");
			}
			this.TenantId = tenantId;
			this.RacFileName = racFile;
			this.ClcFileName = clcFile;
			this.RacExpire = racExpire;
			this.ClcExpire = clcExpire;
			this.Url = url;
			this.Version = version;
		}

		public static bool TryParse(string[] values, out RmsLicenseStoreInfo value)
		{
			value = null;
			if (values == null || values.Length != RmsLicenseStoreInfo.ColumnNames.Length)
			{
				RmsLicenseStoreInfo.Tracer.TraceError(0L, "Rms License Store Info failed to parse values.");
				return false;
			}
			Guid tenantId;
			if (!GuidHelper.TryParseGuid(values[0], out tenantId))
			{
				RmsLicenseStoreInfo.Tracer.TraceError<string>(0L, "Rms License Store Info failed to parse tenantId ({0}).", values[0]);
				return false;
			}
			Uri uri;
			if (!Uri.TryCreate(values[5], UriKind.Absolute, out uri))
			{
				RmsLicenseStoreInfo.Tracer.TraceError<string>(0L, "Rms License Store Info failed to parse URL ({0}).", values[5]);
				return false;
			}
			string text = values[1];
			if (string.IsNullOrEmpty(text))
			{
				RmsLicenseStoreInfo.Tracer.TraceError(0L, "Rms License Store Info failed to parse null racFile.");
				return false;
			}
			string text2 = values[2];
			if (string.IsNullOrEmpty(text2) && RmsLicenseStoreInfo.DefaultUri == uri)
			{
				RmsLicenseStoreInfo.Tracer.TraceError(0L, "Rms License Store Info failed to parse null clcFile.");
				return false;
			}
			long num;
			if (!long.TryParse(values[3], out num) || num < DateTime.MinValue.Ticks || num > DateTime.MaxValue.Ticks)
			{
				RmsLicenseStoreInfo.Tracer.TraceError<string>(0L, "Rms License Store Info failed to parse rac expire time ({0}).", values[3]);
				return false;
			}
			DateTime dateTime = new DateTime(num);
			if (!long.TryParse(values[4], out num) || num < DateTime.MinValue.Ticks || num > DateTime.MaxValue.Ticks)
			{
				RmsLicenseStoreInfo.Tracer.TraceError<string>(0L, "Rms License Store Info failed to parse clc expire time ({0}).", values[4]);
				return false;
			}
			DateTime dateTime2 = new DateTime(num);
			byte version;
			if (!byte.TryParse(values[6], out version))
			{
				RmsLicenseStoreInfo.Tracer.TraceError<string>(0L, "Rms License Store Info failed to parse rac/clc version ({0}).", values[6]);
				return false;
			}
			DateTime utcNow = DateTime.UtcNow;
			if (dateTime > utcNow && dateTime2 > utcNow)
			{
				value = new RmsLicenseStoreInfo(tenantId, uri, text, text2, dateTime, dateTime2, version);
				return true;
			}
			RmsLicenseStoreInfo.Tracer.TraceDebug(0L, "Rms License Store Info failed to parse RmsLicenseStoreInfo as it is expired.");
			return false;
		}

		public string[] ToStringArray()
		{
			return new string[]
			{
				this.TenantId.ToString(),
				this.RacFileName,
				this.ClcFileName,
				this.RacExpire.Ticks.ToString(NumberFormatInfo.InvariantInfo),
				this.ClcExpire.Ticks.ToString(NumberFormatInfo.InvariantInfo),
				(this.Url == null) ? null : this.Url.ToString(),
				this.Version.ToString(NumberFormatInfo.InvariantInfo)
			};
		}

		private static readonly Trace Tracer = ExTraceGlobals.RightsManagementTracer;

		public static readonly Uri DefaultUri = new Uri("http://www.default_uri.com/");

		public static readonly string[] ColumnNames = new string[]
		{
			"tenant-id",
			"rac-filename",
			"clc-filename",
			"rac-expire-time",
			"clc-expire-time",
			"url",
			"version"
		};

		public readonly Guid TenantId;

		public readonly string RacFileName;

		public readonly string ClcFileName;

		public readonly DateTime RacExpire;

		public readonly DateTime ClcExpire;

		public readonly Uri Url;

		public readonly byte Version;
	}
}
