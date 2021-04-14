using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.ApplicationLogic.UserPhotos;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;

namespace Microsoft.Exchange.Data.ApplicationLogic.Directory
{
	internal sealed class MailboxUrls : IMailboxUrls
	{
		public string CalendarUrl { get; private set; }

		public string EditGroupUrl { get; private set; }

		public string EwsUrl { get; private set; }

		public string InboxUrl { get; private set; }

		public string OwaUrl { get; private set; }

		public string PeopleUrl { get; private set; }

		public string PhotoUrl { get; private set; }

		public bool IsFullyInitialized
		{
			get
			{
				return !string.IsNullOrEmpty(this.EditGroupUrl) && !string.IsNullOrEmpty(this.InboxUrl) && !string.IsNullOrEmpty(this.CalendarUrl) && !string.IsNullOrEmpty(this.PeopleUrl) && !string.IsNullOrEmpty(this.PhotoUrl);
			}
		}

		public MailboxUrls(IExchangePrincipal exchangePrincipal, bool failOnError = false)
		{
			string smtpAddress = exchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString();
			string owaUrl = MailboxUrls.GetOwaUrl(exchangePrincipal, failOnError);
			string ewsUrl = MailboxUrls.GetEwsUrl(exchangePrincipal, failOnError);
			this.InitializeUrls(smtpAddress, owaUrl, ewsUrl);
		}

		public MailboxUrls(string smtpAddress, string owaUrl, string ewsUrl)
		{
			this.InitializeUrls(smtpAddress, owaUrl, ewsUrl);
		}

		public static MailboxUrls GetOwaMailboxUrls(IExchangePrincipal exchangePrincipal)
		{
			string smtpAddress = exchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString();
			string owaUrl = MailboxUrls.GetOwaUrl(exchangePrincipal, true);
			return new MailboxUrls(smtpAddress, owaUrl, null);
		}

		internal string[] ToExchangeResources()
		{
			return new string[]
			{
				"InboxUrl=" + this.InboxUrl,
				"CalendarUrl=" + this.CalendarUrl,
				"PeopleUrl=" + this.PeopleUrl,
				"PhotoUrl=" + this.PhotoUrl,
				"EditGroupUrl=" + this.EditGroupUrl
			};
		}

		internal Dictionary<string, string> ToExchangeResourcesDictionary()
		{
			return new Dictionary<string, string>
			{
				{
					"InboxUrl",
					this.InboxUrl
				},
				{
					"CalendarUrl",
					this.CalendarUrl
				},
				{
					"PeopleUrl",
					this.PeopleUrl
				},
				{
					"PhotoUrl",
					this.PhotoUrl
				},
				{
					"EditGroupUrl",
					this.EditGroupUrl
				}
			};
		}

		private void InitializeUrls(string smtpAddress, string owaUrl, string ewsUrl)
		{
			if (!string.IsNullOrEmpty(owaUrl))
			{
				this.OwaUrl = owaUrl;
				this.CalendarUrl = owaUrl + "?path=/group/" + smtpAddress + "/calendar";
				this.InboxUrl = owaUrl + "?path=/group/" + smtpAddress + "/mail";
				this.PeopleUrl = owaUrl + "?path=/group/" + smtpAddress + "/people";
				this.EditGroupUrl = owaUrl + "?path=/group/" + smtpAddress + "/action/edit";
			}
			if (!string.IsNullOrEmpty(ewsUrl))
			{
				this.EwsUrl = ewsUrl;
				HttpPhotoRequestBuilder httpPhotoRequestBuilder = new HttpPhotoRequestBuilder(MailboxUrls.PhotosConfiguration, MailboxUrls.Tracer);
				this.PhotoUrl = httpPhotoRequestBuilder.CreateUri(new Uri(ewsUrl), smtpAddress).AbsoluteUri;
			}
		}

		private static string GetOwaUrl(IExchangePrincipal exchangePrincipal, bool failOnError)
		{
			try
			{
				Uri frontEndOwaUrl = FrontEndLocator.GetFrontEndOwaUrl(exchangePrincipal);
				return frontEndOwaUrl.ToString();
			}
			catch (LocalizedException ex)
			{
				MailboxUrls.Tracer.TraceWarning<string>(0L, "Not able to get the owa url by FrontEndLocator. Exception: {0}", ex.ToString());
				if (failOnError)
				{
					throw;
				}
			}
			return null;
		}

		private static string GetEwsUrl(IExchangePrincipal exchangePrincipal, bool failOnError)
		{
			try
			{
				Uri frontEndWebServicesUrl = FrontEndLocator.GetFrontEndWebServicesUrl(exchangePrincipal);
				return frontEndWebServicesUrl.ToString();
			}
			catch (LocalizedException ex)
			{
				MailboxUrls.Tracer.TraceWarning<string>(0L, "Not able to get the ews url by FrontEndLocator. Exception: {0}", ex.ToString());
				if (failOnError)
				{
					throw;
				}
			}
			return null;
		}

		private static readonly Trace Tracer = ExTraceGlobals.ModernGroupsTracer;

		private static readonly PhotosConfiguration PhotosConfiguration = new PhotosConfiguration(ExchangeSetupContext.InstallPath);
	}
}
