using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Services;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class YouTubeLinkPreviewBuilder : WebPageLinkPreviewBuilder
	{
		public YouTubeLinkPreviewBuilder(Dictionary<string, string> queryParmDictionary, GetLinkPreviewRequest request, string responseString, RequestDetailsLogger logger, Uri responseUri) : base(request, responseString, logger, responseUri, true)
		{
			this.youTubeId = queryParmDictionary["v"];
			this.queryParmDictionary = queryParmDictionary;
			this.autoplay = request.Autoplay;
		}

		protected override LinkPreview CreateLinkPreviewInstance()
		{
			return new YouTubeLinkPreview();
		}

		protected override void SetAdditionalProperties(LinkPreview linkPreview)
		{
			string format = this.autoplay ? "https://www.youtube.com/embed/{0}?autoplay=1" : "https://www.youtube.com/embed/{0}";
			string value = LinkPreviewBuilder.ConvertToSafeHtml(string.Format(format, this.youTubeId));
			StringBuilder stringBuilder = new StringBuilder(value);
			int startTime = this.GetStartTime();
			if (startTime > 0)
			{
				if (!this.autoplay)
				{
					stringBuilder.Append('?');
				}
				else
				{
					stringBuilder.Append('&');
				}
				stringBuilder.Append("start");
				stringBuilder.Append('=');
				stringBuilder.Append(startTime);
			}
			((YouTubeLinkPreview)linkPreview).PlayerUrl = stringBuilder.ToString();
		}

		private int GetStartTime()
		{
			int num = 0;
			string text;
			if (!this.queryParmDictionary.TryGetValue("t", out text))
			{
				return 0;
			}
			bool flag = text.EndsWith("m");
			string text2;
			if (flag || text.EndsWith("s"))
			{
				text2 = text.Substring(0, text.Length - 1);
			}
			else
			{
				text2 = text;
			}
			if (text2 == null || text2.Length == 0)
			{
				return 0;
			}
			if (!int.TryParse(text2, out num))
			{
				return 0;
			}
			if (flag)
			{
				num *= 60;
			}
			return num;
		}

		protected override string GetImage(out int imageTagCount)
		{
			imageTagCount = 1;
			return LinkPreviewBuilder.ConvertToSafeHtml(string.Format("http://img.youtube.com/vi/{0}/0.jpg", this.youTubeId));
		}

		internal static bool IsYoutubeUri(Uri uri)
		{
			bool flag = string.Compare(uri.Host, "www.youtube.com", true) != 0 || string.Compare(uri.LocalPath, "/watch", true) != 0;
			bool flag2 = string.Compare(uri.Host, "m.youtube.com", true) != 0 || string.Compare(uri.LocalPath, "/watch", true) != 0;
			return !flag || !flag2;
		}

		internal static bool TryGetYouTubePlayerQueryParms(Uri uri, RequestDetailsLogger logger, out Dictionary<string, string> queryParmDictionary)
		{
			queryParmDictionary = null;
			bool flag = false;
			if (!YouTubeLinkPreviewBuilder.IsYoutubeUri(uri))
			{
				return false;
			}
			string text = uri.Query.TrimStart(new char[]
			{
				'?'
			});
			if (text.Length == 0)
			{
				return false;
			}
			string[] array = text.Split(new char[]
			{
				'&'
			});
			if (array.Length > 3)
			{
				logger.Set(GetLinkPreviewMetadata.YouTubeLinkValidationFailed, 1);
				return false;
			}
			queryParmDictionary = new Dictionary<string, string>(4);
			foreach (string text2 in array)
			{
				string[] array3 = text2.Split(new char[]
				{
					'='
				});
				if (array3.Length == 2)
				{
					string text3 = array3[0].ToLower();
					string a;
					if ((a = text3) != null)
					{
						if (!(a == "v") && !(a == "app") && !(a == "feature"))
						{
							if (!(a == "t"))
							{
								goto IL_EE;
							}
							flag = true;
						}
						queryParmDictionary.Add(text3, array3[1]);
						goto IL_117;
					}
					IL_EE:
					logger.Set(GetLinkPreviewMetadata.YouTubeLinkValidationFailed, 1);
					return false;
				}
				IL_117:;
			}
			if (!queryParmDictionary.ContainsKey("v"))
			{
				logger.Set(GetLinkPreviewMetadata.YouTubeLinkValidationFailed, 1);
				return false;
			}
			if (!flag && uri.Fragment != null && uri.Fragment.Length > 0 && uri.Fragment.StartsWith("#t=", StringComparison.InvariantCultureIgnoreCase))
			{
				string text4 = uri.Fragment.Substring("#t=".Length);
				if (text4.Length > 0)
				{
					queryParmDictionary.Add("t", text4);
				}
			}
			return true;
		}

		private const string YouTubeIdPropertyName = "youtube id";

		public const string YouTubeHost = "www.youtube.com";

		public const string YouTubeMobileHost = "m.youtube.com";

		public const string YouTubeLocalPath = "/watch";

		private const char QueryDelimiter = '?';

		private const char QueryParmDelimiter = '&';

		private const char QueryParmValueDelimiter = '=';

		private const string YouTubeIdParm = "v";

		private const string AppParm = "app";

		private const string FeatureParm = "feature";

		private const string StartTimeParm = "t";

		private const string StartTimeParmMinuteSuffix = "m";

		private const string StartTimeParmSecondSuffix = "s";

		private const string YouTubeTimeFragmentStart = "#t=";

		private const string StartTimeEmbeddedParm = "start";

		public const string YouTubeImageUrlFormat = "http://img.youtube.com/vi/{0}/0.jpg";

		public const string YouTubePlayerUrlFormat = "https://www.youtube.com/embed/{0}";

		public const string YouTubePlayerUrlAutoplayFormat = "https://www.youtube.com/embed/{0}?autoplay=1";

		private readonly string youTubeId;

		private readonly Dictionary<string, string> queryParmDictionary;

		private readonly bool autoplay;
	}
}
