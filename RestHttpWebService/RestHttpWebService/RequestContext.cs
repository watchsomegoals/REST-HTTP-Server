using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestHttpWebService
{
    public class RequestContext
    {
        public string httpVerb;
        public string dirName;
        public string resourceID;
        public string httpVersion;
        public string payload;

        public RequestContext() { }

        public void ReadContext(string data)
        {

            this.httpVerb = data.Substring(0, data.IndexOf("/") - 1);
            //s1 string from dir name onwards POST /message/1 HTTP/1.1 --> message/1 HTTP/1.1
            string s1 = data.Substring(data.IndexOf("/") + 1);
            this.httpVersion = s1.Substring(s1.IndexOf("HTTP"), 8);
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
            string[] lines = data.Split(new string[] { "\n" }, StringSplitOptions.None);
            
            this.payload = lines[7];
        }
    }
}
