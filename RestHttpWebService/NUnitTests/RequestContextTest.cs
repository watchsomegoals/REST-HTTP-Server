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
        //string testdata = "POST /messages/1 HTTP/1.1\nHOST: LOCALHOST:13000\nUSER-AGENT: CURL/7.55.1\nACCEPT: */*\nCONTENT-LENGTH: 23\nCONTENT-TYPE: APPLICATION/X-WWW-FORM-URLENCODED\n\nthe payload is here";

        [Test]
        [Category("pass")]
        [TestCase("POST /messages/1 HTTP/1.1\nHOST: LOCALHOST:13000\nUSER-AGENT: CURL/7.55.1\nACCEPT: */*\nCONTENT-LENGTH: 23\nCONTENT-TYPE: APPLICATION/X-WWW-FORM-URLENCODED\n\r\nthe payload is here")]
        public void ReadContextTest(string data)
        {
            //act
            context.ReadContext(data);
            //verify
            Assert.AreEqual("POST", context.httpVerb);
            Assert.AreEqual("messages", context.dirName);
            Assert.AreEqual("1", context.resourceID);
            Assert.AreEqual("HTTP/1.1", context.httpVersion);
            Assert.AreEqual("the payload is here", context.payload);
        }
    }
}
