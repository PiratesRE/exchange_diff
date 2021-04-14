using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal class CallerInfo
	{
		public CallerInfo(bool isOpenAsAdmin, CommonAccessToken commonAccessToken, ClientSecurityContext securityContext, string primarySmtpAddress, OrganizationId orgId, string[] userRoles, string[] applicationRoles) : this(isOpenAsAdmin, commonAccessToken, securityContext, primarySmtpAddress, orgId, string.Empty, Guid.NewGuid(), userRoles, applicationRoles)
		{
		}

		public CallerInfo(bool isOpenAsAdmin, CommonAccessToken commonAccessToken, ClientSecurityContext securityContext, string primarySmtpAddress, OrganizationId orgId, string userAgent, Guid queryCorrelationId, string[] userRoles, string[] applicationRoles)
		{
			this.isOpenAsAdmin = isOpenAsAdmin;
			this.commonAccessToken = commonAccessToken;
			this.clientSecurityContext = securityContext;
			this.orgId = orgId;
			this.primarySmtpAddress = primarySmtpAddress;
			this.userAgent = userAgent;
			this.queryCorrelationId = queryCorrelationId;
			this.userRoles = userRoles;
			this.applicationRoles = applicationRoles;
		}

		public ClientSecurityContext ClientSecurityContext
		{
			get
			{
				return this.clientSecurityContext;
			}
		}

		public OrganizationId OrganizationId
		{
			get
			{
				return this.orgId;
			}
		}

		public string PrimarySmtpAddress
		{
			get
			{
				return this.primarySmtpAddress;
			}
		}

		public string UserAgent
		{
			get
			{
				return this.userAgent;
			}
		}

		public CommonAccessToken CommonAccessToken
		{
			get
			{
				return this.commonAccessToken;
			}
		}

		public bool IsOpenAsAdmin
		{
			get
			{
				return this.isOpenAsAdmin;
			}
		}

		public Guid QueryCorrelationId
		{
			get
			{
				return this.queryCorrelationId;
			}
		}

		public string[] UserRoles
		{
			get
			{
				return this.userRoles;
			}
		}

		public string[] ApplicationRoles
		{
			get
			{
				return this.applicationRoles;
			}
		}

		private readonly ClientSecurityContext clientSecurityContext;

		private readonly OrganizationId orgId;

		private readonly string primarySmtpAddress;

		private readonly string userAgent;

		private readonly CommonAccessToken commonAccessToken;

		private readonly bool isOpenAsAdmin;

		private readonly Guid queryCorrelationId;

		private readonly string[] userRoles;

		private readonly string[] applicationRoles;
	}
}
