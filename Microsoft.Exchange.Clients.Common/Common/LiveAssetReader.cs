using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Clients.EventLogs;
using Microsoft.Live.Controls;
using Microsoft.Live.Frontend;

namespace Microsoft.Exchange.Clients.Common
{
	internal class LiveAssetReader : CobrandingAssetReader
	{
		internal LiveAssetReader(HttpContext context)
		{
			DateTime start = DateTime.MinValue;
			DateTime end = DateTime.MinValue;
			try
			{
				this.Initialize();
				if (LiveAssetReader.isInitialized)
				{
					start = DateTime.UtcNow;
					this.aleStringsAgent = new AleStringsAgent(context);
					end = DateTime.UtcNow;
					if (BrandingUtilities.IsBranded())
					{
						this.brandControl = BrandControl.Create(context);
					}
					this.InitializeEnvironmentQualifier();
					CobrandingAssetReader.initializeErrorLogged = false;
				}
			}
			catch (Exception e)
			{
				base.LogInitializeException(e, ClientsEventLogConstants.Tuple_LiveHeaderConfigurationError);
			}
			finally
			{
				try
				{
					context.Response.AppendToLog(this.GetDurationLogMessage("Ale", start, end));
				}
				catch
				{
				}
			}
		}

		public override bool IsPreviewBrand()
		{
			string brandId = CobrandingAssetReader.GetBrandId();
			return '0' == brandId[0] && char.IsNumber(brandId[1]) && brandId.Length == 8 && (int.Parse(brandId.Substring(1, 1), CultureInfo.InvariantCulture) & 2) != 0;
		}

		public override string GetString(CobrandingAssetKey assetKey)
		{
			return this.GetString(CobrandingAssetKeys.GetAssetKeyString(assetKey));
		}

		public string GetString(LiveAssetKey assetKey)
		{
			return this.GetString(LiveAssetKeys.GetAssetKeyString(assetKey));
		}

		public string GetString(string assetId)
		{
			if (string.IsNullOrEmpty(assetId))
			{
				return string.Empty;
			}
			if (assetId == "Live.Shared.GlobalSettings.Header.Tabs.Cobrand.Mail.Text" || assetId == "Live.Shared.GlobalSettings.Header.Tabs.Mail.Text")
			{
				return "Mail_Link_Text_Placeholder";
			}
			if (this.aleStringsAgent == null)
			{
				return string.Empty;
			}
			string text;
			if (this.brandControl == null || string.IsNullOrEmpty(this.brandControl.GetString(assetId, this.aleStringsAgent.Market)))
			{
				text = LiveAssetReader.liveAssetToUrlDictionary[assetId];
				if (text != null)
				{
					return string.Format(text, this.environmentQualifier, this.aleStringsAgent.Market);
				}
			}
			try
			{
				text = this.aleStringsAgent.GetString(assetId);
			}
			catch (NullReferenceException)
			{
				text = string.Empty;
			}
			if (text != null && text.StartsWith("[Error!"))
			{
				text = string.Empty;
			}
			return text;
		}

		public bool HasAssetValue(LiveAssetKey assetKey)
		{
			return !string.IsNullOrEmpty(this.GetString(assetKey));
		}

		private static void InitResourceConsumer(object state)
		{
			try
			{
				LoggingUtilities.LogEvent(ClientsEventLogConstants.Tuple_LiveAssetReaderInitResourceConsumerStarted, new object[0]);
				ResourceConsumer.Init((HttpApplication)state, ResourceConsumer.ReadConfigData(LiveAssetReader.configPath));
				LiveAssetReader.isInitialized = true;
				LoggingUtilities.LogEvent(ClientsEventLogConstants.Tuple_LiveAssetReaderInitResourceConsumerSucceeded, new object[0]);
			}
			catch (Exception ex)
			{
				LiveAssetReader.isInitialized = false;
				LoggingUtilities.LogEvent(ClientsEventLogConstants.Tuple_LiveAssetReaderInitResourceConsumerError, new object[]
				{
					ex.ToString()
				});
			}
			finally
			{
				LiveAssetReader.initResourceConsumerCompleteEvent.Set();
				LiveAssetReader.isInitializing = false;
			}
		}

		private void Initialize()
		{
			if (LiveAssetReader.isInitialized)
			{
				return;
			}
			bool flag = false;
			try
			{
				flag = Monitor.TryEnter(LiveAssetReader.syncRoot, 30000);
				if (flag && !LiveAssetReader.isInitializing && !LiveAssetReader.isInitialized)
				{
					LiveAssetReader.initResourceConsumerCompleteEvent.Reset();
					LiveAssetReader.isInitializing = true;
					HttpApplication applicationInstance = HttpContext.Current.ApplicationInstance;
					LiveAssetReader.configPath = HttpRuntime.AppDomainAppPath + "LiveHeaderConfig.xml";
					ThreadPool.QueueUserWorkItem(new WaitCallback(LiveAssetReader.InitResourceConsumer), applicationInstance);
					LiveAssetReader.initResourceConsumerCompleteEvent.WaitOne(30000);
				}
			}
			finally
			{
				if (flag)
				{
					Monitor.Exit(LiveAssetReader.syncRoot);
				}
			}
		}

		private void InitializeEnvironmentQualifier()
		{
			bool flag = false;
			try
			{
				string text = ConfigurationManager.AppSettings["BrandConfigDomain"];
				if (!string.IsNullOrEmpty(text))
				{
					Uri uri = new Uri("http://" + text);
					if (uri.Host.ToLower().EndsWith("live-int.com"))
					{
						flag = true;
					}
				}
			}
			catch (Exception)
			{
			}
			this.environmentQualifier = (flag ? "-int" : string.Empty);
		}

		private string GetDurationLogMessage(string taskName, DateTime start, DateTime end)
		{
			string format = "&{0}{1}={2}&";
			string text = "S";
			string result;
			if (start != DateTime.MinValue)
			{
				if (end == DateTime.MinValue)
				{
					end = DateTime.UtcNow;
					text = "F";
				}
				TimeSpan timeSpan = new TimeSpan(end.Ticks - start.Ticks);
				result = string.Format(CultureInfo.InvariantCulture, format, new object[]
				{
					taskName,
					text,
					timeSpan.TotalMilliseconds
				});
			}
			else
			{
				text = "F";
				result = string.Format(CultureInfo.InvariantCulture, format, new object[]
				{
					taskName,
					text,
					-1
				});
			}
			return result;
		}

		public override string GetBrandVersion(CultureInfo cultureInfo)
		{
			string brandResourceUrlString = this.GetBrandResourceUrlString();
			if (string.IsNullOrEmpty(brandResourceUrlString))
			{
				return string.Empty;
			}
			if (this.IsPreviewBrand())
			{
				return "preview";
			}
			string locale = this.GetLocale(cultureInfo);
			if (string.IsNullOrEmpty(locale))
			{
				return string.Empty;
			}
			int num = brandResourceUrlString.LastIndexOf(locale, StringComparison.OrdinalIgnoreCase) - 1;
			if (num < 0)
			{
				return string.Empty;
			}
			int num2 = brandResourceUrlString.LastIndexOf('/', num - 1) + 1;
			return brandResourceUrlString.Substring(num2, num - num2);
		}

		public override string GetBrandResourceUrlString()
		{
			return this.GetString("Cobrand.SecureResourceUrl.Path");
		}

		public override string GetLocale(CultureInfo culture)
		{
			if (this.brandControl == null)
			{
				return null;
			}
			return this.brandControl.LocaleToUse(culture.Name);
		}

		public override string GetThemeThumbnailUrl()
		{
			return base.GetBrandImageFileUrl(CobrandingAssetKey.OrganizationLogoPath);
		}

		public override string GetThemeTitle()
		{
			return this.GetString(CobrandingAssetKey.OrganizationName);
		}

		public override bool ShouldEnableCustomTheme
		{
			get
			{
				return this.GetString(CobrandingAssetKey.EnableCustomTheme) == "1";
			}
		}

		public bool IsPropertySet(LiveAssetKey property)
		{
			return this.GetString(property) == "1";
		}

		public const string MailLinkTextPlaceholder = "Mail_Link_Text_Placeholder";

		private const string PreviewBrandVersion = "preview";

		private const string BrandConfigDomain = "BrandConfigDomain";

		private const int InitWaitTime = 30000;

		private static readonly object syncRoot = new object();

		private static readonly ManualResetEvent initResourceConsumerCompleteEvent = new ManualResetEvent(false);

		private static volatile bool isInitialized;

		private static volatile bool isInitializing;

		private static string configPath;

		private static StringDictionary liveAssetToUrlDictionary = new StringDictionary
		{
			{
				"Live.Shared.GlobalSettings.Header.Tabs.Documents.Href",
				"https://office.live{0}.com/?mkt={1}"
			},
			{
				"Live.Shared.GlobalSettings.Header.Tabs.More.YourDocs.Href",
				"https://office.live{0}.com/documents.aspx?mkt={1}"
			},
			{
				"Live.Shared.GlobalSettings.Header.Tabs.More.Groups.Href",
				"http://groups.live{0}.com/?mkt={1}"
			},
			{
				"Live.Shared.GlobalSettings.Header.Tabs.NewWord.Href",
				"https://office.live{0}.com/newlivedocument.aspx?xt=docx&mkt={1}"
			},
			{
				"Live.Shared.GlobalSettings.Header.Tabs.NewExcel.Href",
				"https://office.live{0}.com/newlivedocument.aspx?xt=xlsx&mkt={1}"
			},
			{
				"Live.Shared.GlobalSettings.Header.Tabs.NewPowerPoint.Href",
				"https://office.live{0}.com/newlivedocument.aspx?xt=pptx&mkt={1}"
			},
			{
				"Live.Shared.GlobalSettings.Header.Tabs.NewOneNote.Href",
				"https://office.live{0}.com/newlivedocument.aspx?xt=one&mkt={1}"
			},
			{
				"Live.Shared.GlobalSettings.Header.Tabs.Photos.Href",
				"https://photos.live{0}.com/?mkt={1}"
			},
			{
				"Live.Shared.GlobalSettings.Header.YourAlbums.Href",
				"https://photos.live{0}.com/albums.aspx?mkt={1}"
			},
			{
				"Live.Shared.GlobalSettings.Header.Tabs.YourPhotos.Href",
				"https://photos.live{0}.com/peopletags.aspx?mkt={1}"
			},
			{
				"Live.Shared.GlobalSettings.Header.Tabs.SharePhoto.Href",
				"https://photos.live{0}.com/choosefolder.aspx?mkt={1}"
			}
		};

		private IAleStringsAgent aleStringsAgent;

		private BrandControl brandControl;

		private string environmentQualifier;
	}
}
