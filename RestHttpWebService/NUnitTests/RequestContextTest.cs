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
        //arrange
        RequestContext context = new RequestContext();

        [Test]
        [Category("pass")]
        [TestCase("POST /messages/1 HTTP/1.1\r\nHOST: LOCALHOST:13000\r\nUSER-AGENT: CURL/7.55.1\r\nACCEPT: */*\r\nCONTENT-LENGTH: 23\r\nCONTENT-TYPE: APPLICATION/X-WWW-FORM-URLENCODED\r\n\r\nthe payload is here")]
        public void ReadContextTestPost(string data)
        {
            //act
            context.ReadContext(data);
            //verify
            Assert.AreEqual("POST", context.httpVerb);
            Assert.AreEqual("messages", context.dirName);
            Assert.AreEqual("1", context.resourceID);
            Assert.AreEqual("HTTP/1.1", context.protocol);
            Assert.AreEqual("the payload is here", context.payload);
        }

        [Test]
        [Category("pass")]
        [TestCase("GET /messages/1 HTTP/1.1\r\nHOST: LOCALHOST:13000\r\nUSER-AGENT: CURL/7.55.1\r\nACCEPT: */*\r\nCONTENT-LENGTH: 23\r\nCONTENT-TYPE: APPLICATION/X-WWW-FORM-URLENCODED\r\n\r\n")]
        public void ReadContextTestGet(string data)
        {
            //act
            context.ReadContext(data);
            //verify
            Assert.AreEqual("GET", context.httpVerb);
            Assert.AreEqual("messages", context.dirName);
            Assert.AreEqual("1", context.resourceID);
            Assert.AreEqual("HTTP/1.1", context.protocol);
            Assert.AreEqual(null, context.payload);
        }
    }
}
