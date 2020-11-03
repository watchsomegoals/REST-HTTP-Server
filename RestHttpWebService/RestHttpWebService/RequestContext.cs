using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestHttpWebService
{
    public class RequestContext
    {
        public string httpVerb = null;
        public string dirName = null;
        public string resourceID = null;
        public string httpVersion = null;
        public string payload = null;
        public IDictionary<string, string> headerData = new Dictionary<string, string>();

        public RequestContext() { }

        public void ReadContext(string data)
        {
            string[] lines = data.Split(new string[] { "\n" }, StringSplitOptions.None);

            this.httpVerb = lines[0].Substring(0, data.IndexOf("/") - 1);
            //s1 string from dir name onwards POST /message/1 HTTP/1.1 --> message/1 HTTP/1.1
            string s1 = lines[0].Substring(data.IndexOf("/") + 1);
            this.httpVersion = s1.Substring(s1.IndexOf(" ") + 1);
            //messages/1 HTTP/1.1 --> messages/1
            s1 = s1.Substring(0, s1.IndexOf(" "));
            if(s1.IndexOf("/") == -1)
            {
                this.dirName = s1;
                this.resourceID = null;
            }else
            {
                this.dirName = s1.Substring(0, s1.IndexOf("/"));
                this.resourceID = s1.Substring(s1.IndexOf("/") + 1);
            }
            string host = lines[1].Substring(lines[1].IndexOf(" ") + 1);
            headerData.Add("Host", host);
            string agent = lines[2].Substring(lines[2].IndexOf(" ") + 1);
            headerData.Add("User-Agent", agent);
            string accept = lines[3].Substring(lines[3].IndexOf(" ") + 1);
            headerData.Add("Accept", accept);

            if(httpVerb == "POST" || httpVerb == "PUT")
            {
                string conLen = lines[4].Substring(lines[4].IndexOf(" ") + 1);
                headerData.Add("Content-Length", conLen);
                string conType = lines[5].Substring(lines[5].IndexOf(" ") + 1);
                headerData.Add("Content-Type", conType);
            }

            for(int i = 0; i < lines.Length; i++)
            {   
                if(lines[i] == "\r")
                {
                    this.payload = lines[i + 1];
                }
            }
            foreach (KeyValuePair<string, string> kvp in headerData)
                Console.WriteLine("Key: {0}, Value: {1}", kvp.Key, kvp.Value);
        }
    }
}
