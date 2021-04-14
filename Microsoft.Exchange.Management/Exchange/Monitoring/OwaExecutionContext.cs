using System;
using System.Net;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Monitoring
{
	internal class OwaExecutionContext
	{
		public OwaExecutionContext(TestCasConnectivity.TestCasConnectivityRunInstance instance)
		{
			this.instance = instance;
		}

		public AuthenticationMethod AuthMethod
		{
			get
			{
				return this.authMethod;
			}
			set
			{
				this.authMethod = value;
			}
		}

		public HttpWebResponse FbaResponse
		{
			get
			{
				return this.fbaResponse;
			}
			set
			{
				this.fbaResponse = value;
			}
		}

		public bool IsIsaFbaLogon
		{
			get
			{
				return this.isIsaFbaLogon;
			}
			set
			{
				this.isIsaFbaLogon = value;
			}
		}

		public int IsaFbaFormdir
		{
			get
			{
				return this.isaFbaFormdir;
			}
			set
			{
				this.isaFbaFormdir = value;
			}
		}

		public TestCasConnectivity.TestCasConnectivityRunInstance Instance
		{
			get
			{
				return this.instance;
			}
		}

		public OwaState CurrentState
		{
			get
			{
				return this.state;
			}
			set
			{
				this.state = value;
			}
		}

		public bool GotAuthChallenge
		{
			get
			{
				return this.gotAuthChallenge;
			}
			set
			{
				this.gotAuthChallenge = value;
			}
		}

		private TestCasConnectivity.TestCasConnectivityRunInstance instance;

		private OwaState state;

		private bool gotAuthChallenge;

		private AuthenticationMethod authMethod;

		private bool isIsaFbaLogon;

		private int isaFbaFormdir;

		private HttpWebResponse fbaResponse;
	}
}
