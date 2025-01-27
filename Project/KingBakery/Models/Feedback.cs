﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KingBakery.Models
{
    public class Feedback
    {
        [Key]
        public int ID { get; set; }
        [ForeignKey("Customer")]
        public int CustomerID { get; set; }
        [ForeignKey("BakeryOption")]
        public int BakeryID { get; set; }
        public string ContentFB { get; set; }
        public DateTime Time { get; set; }

        public virtual Customer? Customer { get; set; }
        public virtual BakeryOption? BakeryOption { get; set; }
    }
}
