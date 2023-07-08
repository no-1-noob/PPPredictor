using PPPredictor.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnitTests.Data
{
    [TestClass]
    public class TestSVector3
    {
        [TestMethod]
        public void TestConstructor()
        {
            SVector3 sVector3 = new SVector3();
            Assert.AreEqual(sVector3.x, 0);
            Assert.AreEqual(sVector3.y, 0);
            Assert.AreEqual(sVector3.z, 0);

            sVector3 = new SVector3(1,2,3);
            Assert.AreEqual(sVector3.x, 1);
            Assert.AreEqual(sVector3.y, 2);
            Assert.AreEqual(sVector3.z, 3);
            Assert.AreEqual(sVector3.ToString(), "[x, y, z]");
        }

        [TestMethod]
        public void TestOperators()
        {
            Vector3 vector3 = new SVector3(1, 2, 3);
            Assert.AreEqual(vector3.x, 1);
            Assert.AreEqual(vector3.y, 2);
            Assert.AreEqual(vector3.z, 3);

            SVector3 sVector3 = new Vector3(1, 2, 3);
            Assert.AreEqual(sVector3.x, 1);
            Assert.AreEqual(sVector3.y, 2);
            Assert.AreEqual(sVector3.z, 3);

            sVector3 = new SVector3(1, 2, 3) + new SVector3(1, 2, 3);
            Assert.AreEqual(sVector3.x, 2);
            Assert.AreEqual(sVector3.y, 4);
            Assert.AreEqual(sVector3.z, 6);

            sVector3 = new SVector3(2, 2, 2) - new SVector3(3, 3, 3);
            Assert.AreEqual(sVector3.x, -1);
            Assert.AreEqual(sVector3.y, -1);
            Assert.AreEqual(sVector3.z, -1);

            sVector3 = -new SVector3(1, 2, 3);
            Assert.AreEqual(sVector3.x, -1);
            Assert.AreEqual(sVector3.y, -2);
            Assert.AreEqual(sVector3.z, -3);

            sVector3 = new SVector3(1, 2, 3) * 3;
            Assert.AreEqual(sVector3.x, 3);
            Assert.AreEqual(sVector3.y, 6);
            Assert.AreEqual(sVector3.z, 9);

            sVector3 = 3 * new SVector3(1, 2, 3);
            Assert.AreEqual(sVector3.x, 3);
            Assert.AreEqual(sVector3.y, 6);
            Assert.AreEqual(sVector3.z, 9);

            sVector3 = new SVector3(3, 6, 9) / 3;
            Assert.AreEqual(sVector3.x, 1);
            Assert.AreEqual(sVector3.y, 2);
            Assert.AreEqual(sVector3.z, 3);

        }
    }
}
