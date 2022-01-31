﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ELKInterviewTest.Domain
{
    public class ManagementCompanies
    {
        public ManagementCompanyList[] Companies { get; internal set; }
    }

    public class ManagementCompanyList
    {
        public ManagementCompany company { get; internal set; }
    }
    public class ManagementCompany
    {
        public int mgmtID { get; set; }
        public string name { get; set; }
        public string market { get; set; }
        public string state { get; set; }
    }

}