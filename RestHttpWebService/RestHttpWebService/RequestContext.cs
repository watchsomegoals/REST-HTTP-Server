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
        public string httpVerb;
        public string dirName;
        public string resourceID;
        public string protocol;
        public string payload;
        public IDictionary<string, string> headerData = new Dictionary<string, string>();

        public string statusCode = null;
        public string reasonPhrase = null;
        public string responseBody = null;

        public RequestContext() { }

        public void ReadContext(string data)
        {
            httpVerb = null;
            dirName = null;
            resourceID = null;
            protocol = null;
            payload = null;

            if(headerData.Count() != 0)
            {
                headerData.Clear();
            }

            string[] head;
            string[] resources;
            string[] lines = data.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            string[] firstLine;
            if (lines.Length > 1)
            {
                firstLine = lines[0].Split(new string[] { " " }, StringSplitOptions.None);
                if (firstLine.Length > 1)
                {
                    this.httpVerb = firstLine[0];
                    resources = firstLine[1].Split(new string[] { "/" }, StringSplitOptions.None);
                    if (resources.Length == 2)
                    {
                        this.dirName = resources[1];
                    }
                    else if (resources.Length == 3)
                    {
                        this.dirName = resources[1];
                        this.resourceID = resources[2];
                    }
                    this.protocol = firstLine[2];
                    if (httpVerb == "POST" || httpVerb == "PUT")
                    {
                        //Getting the payload 
                        for (int i = 0; i < lines.Length; i++)
                        {
                            if (lines[i] == "")
                            {
                                int start = Array.IndexOf(lines, "");
                                start++;
                                for(int j = start; j < lines.Length; j++)
                                {
                                    this.payload += lines[j];
                                    if (j != lines.Length -1)
                                    {
                                        this.payload += "\n";
                                    }
                                }
                            }
                        }
                    }
                    for(int i = 1; i < lines.Length; i++)
                    {
                        if (lines[i] == "")
                        {
                            break;
                        }
                        head = lines[i].Split(new string[] { ": " }, StringSplitOptions.None);
                        headerData.Add(head[0], head[1]);
                    }
                    foreach (KeyValuePair<string, string> kvp in headerData)
                    {
                        Console.WriteLine("Key: {0}, Value: {1}", kvp.Key, kvp.Value);
                    }
                    Console.WriteLine("\n");
                    Console.WriteLine("httpVerb: {0}", this.httpVerb);
                    Console.WriteLine("dirName: {0}", this.dirName);
                    Console.WriteLine("resourceID: {0}", this.resourceID);
                    Console.WriteLine("protocol: {0}", this.protocol);
                    Console.WriteLine("httpPayLoad: {0}\n", this.payload);
                }
                
            }
        }

        public void HandleRequest()
        {
            statusCode = null;
            reasonPhrase = null;
            responseBody = null;

            if (string.Compare(httpVerb, "GET") != 0 && string.Compare(httpVerb, "POST") != 0 &&
               string.Compare(httpVerb, "PUT") != 0 && string.Compare(httpVerb, "DELETE") != 0)
            {
                statusCode = "501";
                reasonPhrase = "Not Implemented";
                responseBody = "\nRequest Not Implemented\n";
            }
            else if (string.Compare(this.protocol, "HTTP/1.1") != 0)
            {
                statusCode = "500";
                reasonPhrase = "Internal Server Error";
                responseBody = "\nWrong protocol, internal server error\n";
            }
            else if (string.Compare(httpVerb, "POST") == 0)
            {
                if (string.Compare(resourceID, null) != 0)
                {
                    statusCode = "400";
                    reasonPhrase = "Bad Request";
                    responseBody = "\nBad request, no resourceID necessary\n";
                }
                else if (string.Compare(dirName, "messages") != 0)
                {
                    statusCode = "404";
                    reasonPhrase = "Not Found";
                    responseBody = "\nNot Found, wrong ressource name\n";
                }
                else
                {
                    Post();
                    statusCode = "200";
                    reasonPhrase = "OK";
                }
            }
            else if (string.Compare(httpVerb, "GET") == 0)
            {
                if (string.Compare(dirName, "messages") != 0)
                {
                    statusCode = "404";
                    reasonPhrase = "Not Found";
                    responseBody = "\nNot Found, wrong ressource name\n";
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
                    responseBody = "\nBad request, wrong ressource\n";
                }
                else if (string.Compare(resourceID, null) == 0)
                {
                    statusCode = "400";
                    reasonPhrase = "Bad Request";
                    responseBody = "\nBad request, resourceID necessary\n";
                }
                else
                {
                    Put();
                }
            }
            else if (string.Compare(httpVerb, "DELETE") == 0)
            {
                if (string.Compare(dirName, "messages") != 0)
                {
                    statusCode = "404";
                    reasonPhrase = "Not Found";
                    responseBody = "\nNot Found, wrong ressource name\n";
                }
                else if (string.Compare(resourceID, null) == 0)
                {
                    statusCode = "400";
                    reasonPhrase = "Bad Request";
                    responseBody = "\nBad request, resourceID necessary\n";
                }
                else
                {
                    Delete();
                }
            }
        }

        private void Delete()
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
                    responseBody = "\nFile deleted";
                }
                else
                {
                    statusCode = "404";
                    reasonPhrase = "Not Found";
                    responseBody = "\nNot Found, file does not exist";
                }
            }
            else
            {
                statusCode = "404";
                reasonPhrase = "Not Found";
                responseBody = "\nNot Found, file does not exist";
            }
        }

        private void Put()
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
                    responseBody = "\nFile updated";
                }
                else
                {
                    statusCode = "404";
                    reasonPhrase = "Not Found";
                    responseBody = "\nNot Found, file does not exist";
                }
            }
            else
            {
                statusCode = "404";
                reasonPhrase = "Not Found";
                responseBody = "\nNot Found, file does not exist";
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
                    responseBody += "\n";
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
                    responseBody = "\nNot Found, file does not exist";
                }
            }
            else
            {
                statusCode = "404";
                reasonPhrase = "Not Found";
                responseBody = "\nNot Found, file does not exist";
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
                    responseBody += "\n";
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
                    responseBody = "\nNot Found, files do not exist";
                }
            }
            else
            {
                statusCode = "404";
                reasonPhrase = "Not Found";
                responseBody = "\nNot Found, files do not exist";
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
            responseBody = "\n" + fileName;
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
                counter++;
                fileName = counter.ToString();
                responseBody = "\n" + fileName;
                fileName += ".txt";
                pathFileName = Path.Combine(path, fileName);
                CreateTextFile(pathFileName, counter, fileName, path);
            }
        }

        public string ComposeResponse()
        {
            string response;
            response = $"{protocol} {statusCode} {reasonPhrase}\r\n";
            response += "Server: Caraba\r\n";
            response += "Content-Type: text/html\r\n";
            response += "Accept-Ranges: bytes\r\n";
            response += $"Content-Length: {responseBody.Length}\r\n";
            response += "\r\n";
            response += $"{responseBody}";
            response += "\r\n\r\n";
            return response;
        }
    }
}
