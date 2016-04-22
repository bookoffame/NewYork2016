using System;
using System.Net;
using System.Text.RegularExpressions;

namespace AssemblyCSharp
{
	public class IIIFGetManifest
	{
		private WebClient client;
		private Regex regex;
		private MatchCollection pages;
		private int curr;

		public IIIFGetManifest ()
		{
			client = new WebClient ();
			regex = new Regex ("\"@id\":\"([^\"]*?)\",\"@type\":\"dctypes:Image\"");
			//regex = new Regex ("\"service\":(\\s|\\R)*{(\\s|\\R)*\"@id\":(\\s|\\R)*?\"([^\"]*?)");
		}

		public void download(string url)
		{
			string manifest = client.DownloadString (url);
			pages = regex.Matches (manifest);
			curr = 0;
		}

		public string getPage(int index)
		{
			curr = index;
			return pages [index].Groups [1].Value;
		}

		public string getNextPage()
		{
			curr++;
			return pages [curr].Groups [1].Value;
		}

		public string getPrevPage()
		{
			curr--;
			return pages [curr].Groups [1].Value;
		}
			
	}
}

