using System;
using System.Collections.Generic;
using System.Text;

namespace ELKInterviewTest.Domain.Models
{
    public class ApartmentProperties
    {
        public PropertyList[] Properties { get; internal set; }
    }

    public class PropertyList
    {
        public Property property { get; set; }
    }

    public class Property
    {
        public int propertyID { get; set; }
        public string name { get; set; }
        public string formerName { get; set; }
        public string streetAddress { get; set; }
        public string city { get; set; }
        public string market { get; set; }
        public string state { get; set; }
        public float lat { get; set; }
        public float lng { get; set; }
    }

}
