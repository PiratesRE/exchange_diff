using System;
using System.Xml;
using Microsoft.Exchange.Common.Cache;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Security.RightsManagement
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class TenantLicensePair : CachableItem
	{
		public SafeRightsManagementHandle EnablingPrincipalRac
		{
			get
			{
				return this.enablingPrincipalRac;
			}
		}

		public SafeRightsManagementHandle BoundLicenseClc
		{
			get
			{
				return this.boundLicenseClc;
			}
		}

		public bool IsCleanedUp
		{
			get
			{
				return this.isCleanedUp;
			}
		}

		public override long ItemSize
		{
			get
			{
				return (long)this.size;
			}
		}

		public TenantLicensePair(Guid tenantId, XmlNode[] rac, XmlNode clcNode, string racCertChain, string clcCertChain, DateTime racExpire, DateTime clcExpire, byte version, SafeRightsManagementEnvironmentHandle envHandle, SafeRightsManagementHandle libHandle)
		{
			if (rac == null || rac.Length == 0 || rac[0] == null)
			{
				throw new ArgumentNullException("rac");
			}
			if (envHandle == null)
			{
				throw new ArgumentNullException("envHandle");
			}
			if (libHandle == null)
			{
				throw new ArgumentNullException("libHandle");
			}
			if (string.IsNullOrEmpty(racCertChain))
			{
				racCertChain = DrmClientUtils.ConvertXmlNodeArrayToCertificateChain(rac);
			}
			this.isB2BEntry = string.IsNullOrEmpty(clcCertChain);
			if (!this.isB2BEntry && (clcNode == null || string.IsNullOrEmpty(clcNode.OuterXml)))
			{
				throw new ArgumentException("clcNode should not be null for a non B2B entry", "clcNode");
			}
			this.Rac = rac;
			this.RacExpire = racExpire;
			this.ClcExpire = clcExpire;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				SafeRightsManagementHandle disposable = null;
				SafeRightsManagementHandle safeRightsManagementHandle = DrmClientUtils.CreateEnablingPrincipal(racCertChain, envHandle, libHandle);
				disposeGuard.Add<SafeRightsManagementHandle>(safeRightsManagementHandle);
				if (this.isB2BEntry)
				{
					this.boundLicenseClc = null;
					this.size = racCertChain.Length * 2 + 8 + 16;
				}
				else
				{
					disposable = DrmClientUtils.CreateClcBoundLicense(safeRightsManagementHandle, envHandle, clcCertChain);
					disposeGuard.Add<SafeRightsManagementHandle>(disposable);
					this.size = racCertChain.Length * 2 + 16 + 16;
				}
				if (!this.isB2BEntry)
				{
					DrmClientUtils.ParseGic(rac[0].OuterXml, out this.racDistributionPointIntranet, out this.racDistributionPointExtranet);
					DrmClientUtils.ParseClc(clcNode.OuterXml, out this.clcDistributionPointIntranet, out this.clcDistributionPointExtranet);
					this.size += ((this.racDistributionPointIntranet != null) ? this.racDistributionPointIntranet.OriginalString.Length : 0) + ((this.clcDistributionPointIntranet != null) ? this.clcDistributionPointIntranet.OriginalString.Length : 0) + ((this.racDistributionPointExtranet != null) ? this.racDistributionPointExtranet.OriginalString.Length : 0) + ((this.clcDistributionPointExtranet != null) ? this.clcDistributionPointExtranet.OriginalString.Length : 0);
				}
				this.Version = version;
				this.size++;
				disposeGuard.Success();
				this.enablingPrincipalRac = safeRightsManagementHandle;
				this.boundLicenseClc = disposable;
				this.references = 1;
			}
		}

		public bool HasConfigurationChanged(Uri serviceLocation, Uri publishLocation, byte currentVersion)
		{
			return this.Version != currentVersion || (!this.isB2BEntry && ((!TenantLicensePair.IsRmsUriMatch(this.racDistributionPointIntranet, serviceLocation) && !TenantLicensePair.IsRmsUriMatch(this.racDistributionPointExtranet, serviceLocation)) || (!TenantLicensePair.IsRmsUriMatch(this.clcDistributionPointIntranet, publishLocation) && !TenantLicensePair.IsRmsUriMatch(this.clcDistributionPointExtranet, publishLocation))));
		}

		public void AddRef()
		{
			lock (this.syncRoot)
			{
				this.references++;
			}
		}

		public void Release()
		{
			lock (this.syncRoot)
			{
				if (this.references <= 0)
				{
					throw new InvalidOperationException("Release called without a corresponding AddRef.");
				}
				this.references--;
				if (this.references == 0)
				{
					this.CloseHandles();
				}
			}
		}

		private static bool IsRmsUriMatch(Uri uri1, Uri uri2)
		{
			return Uri.Compare(uri1, uri2, UriComponents.SchemeAndServer, UriFormat.UriEscaped, StringComparison.OrdinalIgnoreCase) == 0;
		}

		private void CloseHandles()
		{
			lock (this.syncRoot)
			{
				if (this.enablingPrincipalRac != null)
				{
					this.enablingPrincipalRac.Close();
					this.enablingPrincipalRac = null;
				}
				if (this.boundLicenseClc != null)
				{
					this.boundLicenseClc.Close();
					this.boundLicenseClc = null;
				}
				this.isCleanedUp = true;
			}
		}

		public readonly XmlNode[] Rac;

		public readonly DateTime RacExpire;

		public readonly DateTime ClcExpire;

		public readonly byte Version;

		private readonly Uri racDistributionPointIntranet;

		private readonly Uri clcDistributionPointIntranet;

		private readonly Uri racDistributionPointExtranet;

		private readonly Uri clcDistributionPointExtranet;

		private readonly object syncRoot = new object();

		private SafeRightsManagementHandle enablingPrincipalRac;

		private SafeRightsManagementHandle boundLicenseClc;

		private int size;

		private int references;

		private bool isCleanedUp;

		private bool isB2BEntry;
	}
}
