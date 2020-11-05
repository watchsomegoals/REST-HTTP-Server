using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;

namespace RestHttpWebService
{
    public class RequestContext
    {
        public string httpVerb = null;
        public string dirName = null;
        public string resourceID = null;
        public string protocol = null;
        public string payload = null;
        public IDictionary<string, string> headerData = new Dictionary<string, string>();

        public string statusCode = null;
        public string reasonPhrase = null;
        public string responseID = null;
        public string responseBody = null;

        public RequestContext() { }

        public void ReadContext(string data)
        {
            string[] lines = data.Split(new string[] { "\n" }, StringSplitOptions.None);

            this.httpVerb = lines[0].Substring(0, data.IndexOf("/") - 1);

            //s1 string from dir name onwards POST /message/1 HTTP/1.1 --> message/1 HTTP/1.1
            string s1 = lines[0].Substring(data.IndexOf("/") + 1);
            this.protocol = s1.Substring(s1.IndexOf(" ") + 1);

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

            //Adding host KeyValuePair
            string host = lines[1].Substring(lines[1].IndexOf(" ") + 1);
            if (headerData.ContainsKey("Host"))
            {
                headerData["Host"] = host;
            }
            else
            {
                headerData.Add("Host", host);
            }
            //Adding agent KeyValuePair
            string agent = lines[2].Substring(lines[2].IndexOf(" ") + 1);
            if (headerData.ContainsKey("User-Agent"))
            {
                headerData["User-Agent"] = agent;
            }
            else
            {
                headerData.Add("User-Agent", agent);
            }
            //Adding accept KeyValuePair
            string accept = lines[3].Substring(lines[3].IndexOf(" ") + 1);
            if (headerData.ContainsKey("Accept"))
            {
                headerData["Accept"] = accept;
            }
            else
            {
                headerData.Add("Accept", accept);
            }

            if (httpVerb == "POST" || httpVerb == "PUT")
            {
                //Adding content-length KeyValuePair
                string conLen = lines[4].Substring(lines[4].IndexOf(" ") + 1);
                if (headerData.ContainsKey("Content-Length"))
                {
                    headerData["Content-Length"] = conLen;
                }
                else
                {
                    headerData.Add("Content-Length", conLen);
                }
                //Adding content-type KeyValuePair
                string conType = lines[5].Substring(lines[5].IndexOf(" ") + 1);
                if (headerData.ContainsKey("Content-Type"))
                {
                    headerData["Content-Type"] = conType;
                }
                else
                {
                    headerData.Add("Content-Type", conType);
                }
            }
            else if (httpVerb == "GET" || httpVerb == "DELETE")
            {
                foreach (KeyValuePair<string, string> kvp in headerData)
                {
                    if (kvp.Key == "Content-Length")
                    {
                        headerData.Remove(kvp.Key);
                        break;
                    }
                }
                foreach (KeyValuePair<string, string> kvp in headerData)
                {
                    if (kvp.Key == "Content-Type")
                    {
                        headerData.Remove(kvp.Key);
                        break;
                    }
                }

            //Getting the payload 
            }
            for(int i = 0; i < lines.Length; i++)
            {   
                if(lines[i] == "\r")
                {
                    this.payload = lines[i + 1];
                }
            }

            //Logging the information to the console window
            foreach (KeyValuePair<string, string> kvp in headerData)
            {
                Console.WriteLine("Key: {0}, Value: {1}", kvp.Key, kvp.Value);
            }
            Console.WriteLine("httpVerb: {0}", this.httpVerb);
            Console.WriteLine("dirName: {0}", this.dirName);
            Console.WriteLine("resourceID: {0}", this.resourceID);
            Console.WriteLine("protocol: {0}", this.protocol);
            Console.WriteLine("httpPayLoad: {0}", this.payload);
        }

        public void HandleRequest()
        {
            if (string.Compare(httpVerb, "GET") != 0 && string.Compare(httpVerb, "POST") != 0 &&
               string.Compare(httpVerb, "PUT") != 0 && string.Compare(httpVerb, "DELETE") != 0)
            {
                statusCode = "501";
                reasonPhrase = "Not Implemented";
            }
            else if (string.Compare(this.protocol, "HTTP/1.1\r") != 0)
            {
                statusCode = "500";
                reasonPhrase = "Internal Server Error";
            }
            else if (string.Compare(httpVerb, "POST") == 0)
            {
                if (string.Compare(resourceID, null) != 0)
                {
                    statusCode = "400";
                    reasonPhrase = "Bad Request";
                }
                else if (string.Compare(dirName, "messages") != 0)
                {
                    statusCode = "404";
                    reasonPhrase = "Not Found";
                }
                else
                {
                    Post();
                    statusCode = "201";
                    reasonPhrase = "Created";
                }
            }
            else if (string.Compare(httpVerb, "GET") == 0)
            {
                if (string.Compare(dirName, "messages") != 0)
                {
                    statusCode = "404";
                    reasonPhrase = "Not Found";
                }
                else if (string.Compare(resourceID, null) == 0)
                {
                    GetAll();
                }
                else
                {
                    GetByID();
                }
            }
            else if (string.Compare(httpVerb, "PUT") == 0)
            {
                if (string.Compare(dirName, "messages") != 0)
                {
                    statusCode = "404";
                    reasonPhrase = "Not Found";
                }
                else if (string.Compare(resourceID, null) == 0)
                {
                    statusCode = "400";
                    reasonPhrase = "Bad Request";
                }
                else
                {
                    PUT();
                }
            }
            else if (string.Compare(httpVerb, "DELETE") == 0)
            {
                if (string.Compare(dirName, "messages") != 0)
                {
                    statusCode = "404";
                    reasonPhrase = "Not Found";
                }
                else if (string.Compare(resourceID, null) == 0)
                {
                    statusCode = "400";
                    reasonPhrase = "Bad Request";
                }
                else
                {
                    string path = Path.Combine(Environment.CurrentDirectory, dirName);
                    if (Directory.Exists(path))
                    {
                        string filePath = Path.Combine(path, resourceID);
                        filePath += ".txt";
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                            statusCode = "200";
                            reasonPhrase = "OK";
                        }
                        else
                        {
                            statusCode = "404";
                            reasonPhrase = "Not Found";
                        }
                    }
                    else
                    {
                        statusCode = "404";
                        reasonPhrase = "Not Found";
                    }
                }
            }
            Console.WriteLine(statusCode + "\n" + reasonPhrase + "\n" + responseID + "\n" + responseBody);
        }

        private void PUT()
        {
            string path = Path.Combine(Environment.CurrentDirectory, dirName);
            if (Directory.Exists(path))
            {
                string filePath = Path.Combine(path, resourceID);
                filePath += ".txt";
                if (File.Exists(filePath))
                {
                    File.WriteAllText(filePath, payload);
                    statusCode = "200";
                    reasonPhrase = "OK";
                }
                else
                {
                    statusCode = "404";
                    reasonPhrase = "Not Found";
                }
            }
            else
            {
                statusCode = "404";
                reasonPhrase = "Not Found";
            }
        }

        private void GetByID()
        {
            string path = Path.Combine(Environment.CurrentDirectory, dirName);
            if (Directory.Exists(path))
            {
                string filePath = Path.Combine(path, resourceID);
                filePath += ".txt";
                if (File.Exists(filePath))
                {
                    responseBody = null;
                    responseBody += Path.GetFileName(filePath);
                    responseBody += "\n{";
                    responseBody += File.ReadAllText(filePath);
                    responseBody += "}\n";
                    statusCode = "200";
                    reasonPhrase = "OK";
                }
                else
                {
                    statusCode = "404";
                    reasonPhrase = "Not Found";
                }
            }
            else
            {
                statusCode = "404";
                reasonPhrase = "Not Found";
            }
        }

        private void GetAll()
        {
            string path = Path.Combine(Environment.CurrentDirectory, dirName);
            if (Directory.Exists(path))
            {
                DirectoryInfo messagesDir = new DirectoryInfo(path);
                if (messagesDir.GetFiles("*.txt").Length > 0)
                {
                    string[] filePaths = Directory.GetFiles(path);
                    responseBody = null;
                    for (int i = 0; i < filePaths.Length; i++)
                    {
                        responseBody += Path.GetFileName(filePaths[i]);
                        responseBody += "\n{";
                        responseBody += File.ReadAllText(filePaths[i]);
                        responseBody += "}\n";
                        statusCode = "200";
                        reasonPhrase = "OK";
                    }
                }
                else
                {
                    statusCode = "404";
                    reasonPhrase = "Not Found";
                }
            }
            else
            {
                statusCode = "404";
                reasonPhrase = "Not Found";
            }
        }

        public void Post()
        {
            string path = Path.Combine(Environment.CurrentDirectory, dirName);
            Directory.CreateDirectory(path);
            string fileName = null;
            int counter = Directory.GetFiles(path).Length;
            //Console.WriteLine("counter: {0}", counter);
            counter++;
            fileName = counter.ToString();
            responseID = fileName;
            fileName += ".txt";
            string pathFileName = Path.Combine(path, fileName);
            CreateTextFile(pathFileName, counter, fileName, path);
        }

        private void CreateTextFile(string pathFileName, int counter, string fileName, string path)
        {
            if (!File.Exists(pathFileName))
            {
                using (StreamWriter sw = File.CreateText(pathFileName))
                {
                    sw.Write(this.payload);
                }
            }
            else
            {
                Console.WriteLine("Text File exists already");
                counter++;
                fileName = counter.ToString();
                responseID = fileName;
                fileName += ".txt";
                pathFileName = Path.Combine(path, fileName);
                CreateTextFile(pathFileName, counter, fileName, path);
            }
        }
    }
}
