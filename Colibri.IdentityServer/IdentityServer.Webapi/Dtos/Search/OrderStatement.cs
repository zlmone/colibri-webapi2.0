﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Webapi.Dtos.Search
{
    public class OrderStatement
    {
        public string ColumName { get; set; }
        public bool Reverse { get; set; }
    }
}
