﻿using storagecore.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Survey.DomainModelLayer.Entities
{

    public partial class Users : BaseEntity<Guid>
    {
        public Users()
        {
            SurveySections = new HashSet<SurveySections>();
        }

        //public Guid Id { get; set; }

        [InverseProperty("User")]
        public ICollection<SurveySections> SurveySections { get; set; }
    }
}