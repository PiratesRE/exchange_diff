using System;
using System.Net;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal class OabValidator : ServiceValidatorBase
	{
		public OabValidator(string uri, NetworkCredential credentials) : base(OabValidator.CombineUrl(uri, OabValidator.OABManifestFile), credentials)
		{
			base.TraceResponseBody = false;
		}

		protected override string Name
		{
			get
			{
				return Strings.ServiceNameOab;
			}
		}

		protected override void FillRequestProperties(HttpWebRequest request)
		{
			base.FillRequestProperties(request);
			request.Method = "GET";
			request.ContentType = null;
		}

		private static string CombineUrl(string uri1, string uri2)
		{
			uri1 = uri1.TrimEnd(new char[]
			{
				'/'
			});
			uri2 = uri2.TrimStart(new char[]
			{
				'/'
			});
			return string.Format("{0}/{1}", uri1, uri2);
		}

		private static readonly string OABManifestFile = "oab.xml";
	}
}
