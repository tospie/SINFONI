using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleClient
{
    public class ClientVector
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }

        public double Length
        {
            get { return getLength(); }
        }

        private double getLength()
        {
            return Math.Sqrt(x * x + y * y + z * z);
        }
    }
}
