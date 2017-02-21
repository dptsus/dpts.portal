using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DPTS.Domain.Entities
{
    public class ReviewComments : BaseEntityWithDateTime
    {
        public string CommentForId { get; set; }
        public string CommentOwnerId { get; set; }
        public string CommentOwnerUser { get; set; }
        public string Comment { get; set; }
        public decimal Rating { get; set; }
        public bool IsApproved { get; set; }
        public bool IsActive { get; set; }
        //public virtual AspNetUser AspNetUser { get; set; }
        //public virtual Doctor Doctor { get; set; }
    }

    public class RatingDetails
    {
        public string Key { get; set; }
        public decimal Value { get; set; }
    }
}
