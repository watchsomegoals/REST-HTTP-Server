using NUnit.Framework;
using RestHttpWebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUnitTests
{
    [TestFixture]
    class RequestContextTest
    {
        [Test]
        [Category("pass")]
        [TestCase("POST /messages/1 HTTP/1.1\r\n" +
                  "HOST: LOCALHOST:13000\r\n" +
                  "USER-AGENT: CURL/7.55.1\r\n" +
                  "ACCEPT: */*\r\n" +
                  "CONTENT-LENGTH: 23\r\n" +
                  "CONTENT-TYPE: APPLICATION/X-WWW-FORM-URLENCODED\r\n\r\n" +
                  "the payload is here")]
        public void ReadContextTestPost(string data)
        {
            //arrange
            RequestContext context = new RequestContext();
            //act
            context.ReadContext(data);
            //assert
            Assert.AreEqual("POST", context.HttpVerb);
            Assert.AreEqual("messages", context.DirName);
            Assert.AreEqual("1", context.ResourceID);
            Assert.AreEqual("HTTP/1.1", context.Protocol);
            Assert.AreEqual("the payload is here", context.Payload);
        }

        [Test]
        [Category("pass")]
        [TestCase("GET /messages/1 HTTP/1.1\r\n" +
                  "HOST: LOCALHOST:13000\r\n" +
                  "USER-AGENT: CURL/7.55.1\r\n" +
                  "ACCEPT: */*\r\n" +
                  "CONTENT-LENGTH: 23\r\n" +
                  "CONTENT-TYPE: APPLICATION/X-WWW-FORM-URLENCODED\r\n\r\n")]
        public void ReadContextTestGet(string data)
        {
            //arrange
            RequestContext context = new RequestContext();
            //act
            context.ReadContext(data);
            //assert
            Assert.AreEqual("GET", context.HttpVerb);
            Assert.AreEqual("messages", context.DirName);
            Assert.AreEqual("1", context.ResourceID);
            Assert.AreEqual("HTTP/1.1", context.Protocol);
            Assert.AreEqual(null, context.Payload);
        }

        [Test]
        [Category("pass")]
        [TestCase("DELETE /messages/1 HTTP/1.1\r\n" +
                  "HOST: LOCALHOST:13000\r\n" +
                  "USER-AGENT: CURL/7.55.1\r\n" +
                  "ACCEPT: */*\r\n" +
                  "CONTENT-LENGTH: 23\r\n" +
                  "CONTENT-TYPE: APPLICATION/X-WWW-FORM-URLENCODED\r\n\r\n")]
        public void ReadContextTestDelete(string data)
        {
            //arrange
            RequestContext context = new RequestContext();
            //act
            context.ReadContext(data);
            //assert
            Assert.AreEqual("DELETE", context.HttpVerb);
            Assert.AreEqual("messages", context.DirName);
            Assert.AreEqual("1", context.ResourceID);
            Assert.AreEqual("HTTP/1.1", context.Protocol);
            Assert.AreEqual(null, context.Payload);
        }

        [Test]
        [Category("pass")]
        public void HandleRequestPostTest()
        {
            //arrange
            RequestContext context = new RequestContext
            {
                HttpVerb = "POST",
                DirName = "messages",
                ResourceID = null,
                Protocol = "HTTP/1.1",
                Payload = "test payload"
            };
            //act
            context.HandleRequest();
            //assert
            Assert.AreEqual("200", context.StatusCode);
            Assert.AreEqual("OK", context.ReasonPhrase);
            Assert.IsNotNull(context.ResponseBody);
        }

        [Test]
        [Category("pass")]
        public void HandleRequestGetTest()
        {
            //arrange
            RequestContext context = new RequestContext
            {
                HttpVerb = "GET",
                DirName = "messages",
                ResourceID = null,
                Protocol = "HTTP/1.1",
                Payload = null
            };
            //act
            context.HandleRequest();
            //assert
            Assert.AreEqual("200", context.StatusCode);
            Assert.AreEqual("OK", context.ReasonPhrase);
            Assert.IsNotNull(context.ResponseBody);
        }

        [Test]
        [Category("pass")]
        public void HandleRequestPutTest()
        {
            //arrange
            RequestContext context = new RequestContext
            {
                HttpVerb = "PUT",
                DirName = "messages",
                ResourceID = "7",
                Protocol = "HTTP/1.1",
                Payload = "update message"
            };
            //act
            context.HandleRequest();
            //assert
            Assert.AreEqual("200", context.StatusCode);
            Assert.AreEqual("OK", context.ReasonPhrase);
            Assert.IsNotNull(context.ResponseBody);
        }

        [Test]
        [Category("fail")]
        public void DeleteTest()
        {
            //arrange
            RequestContext context = new RequestContext
            {
                DirName = "messages",
                ResourceID = "10"
            };
            //act
            context.Delete();
            //assert
            Assert.AreEqual("404", context.StatusCode);
            Assert.AreEqual("Not Found", context.ReasonPhrase);
            Assert.AreEqual("\nNot Found, file does not exist", context.ResponseBody);
        }

        [Test]
        [Category("fail")]
        public void PutTest()
        {
            //arrange
            RequestContext context = new RequestContext
            {
                DirName = "messages",
                ResourceID = "10",
                Payload = "test payload"
            };
            //act
            context.Put();
            //assert
            Assert.AreEqual("404", context.StatusCode);
            Assert.AreEqual("Not Found", context.ReasonPhrase);
            Assert.AreEqual("\nNot Found, file does not exist", context.ResponseBody);
        }

        [Test]
        [Category("pass")]
        public void GetByIDTest()
        {
            //arrange
            RequestContext context = new RequestContext
            {
                DirName = "messages",
                ResourceID = "6"
            };
            //act
            context.GetByID();
            //assert
            Assert.AreEqual("200", context.StatusCode);
            Assert.AreEqual("OK", context.ReasonPhrase);
            Assert.IsNotNull(context.ResponseBody);
        }

        [Test]
        [Category("pass")]
        public void GetAllTest()
        {
            //arrange
            RequestContext context = new RequestContext
            {
                DirName = "messages",
            };
            //act
            context.GetAll();
            //assert
            Assert.AreEqual("200", context.StatusCode);
            Assert.AreEqual("OK", context.ReasonPhrase);
            Assert.IsNotNull(context.ResponseBody);
        }

        [Test]
        [Category("pass")]

        public void ComposeResponseTest()
        {
            string responseActual;
            string response = "HTTP/1.1 200 OK\r\n" +
                              "Server: Caraba\r\n" +
                              "Content-Type: text/html\r\n" +
                              "Accept-Ranges: bytes\r\n" +
                              "Content-Length: 2\r\n" +
                              "\r\n" +
                              "\n1" +
                              "\r\n\r\n";
            //arrange
            RequestContext context = new RequestContext
            {
                Protocol = "HTTP/1.1",
                StatusCode = "200",
                ReasonPhrase = "OK",
                ResponseBody = "\n1"
            };
            //act
            responseActual = context.ComposeResponse();
            //assert
            Assert.AreEqual(response, responseActual);
        }
    }
}
