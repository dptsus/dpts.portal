using System;
using System.Collections.Generic;
using DPTS.Domain.Core.ReviewComments;
using DPTS.Domain.Entities;
using DPTS.Domain.Core;
using System.Linq;

namespace DPTS.Domain.ReviewComments
{
    public class ReviewCommentsService : IReviewCommentsService
    {
        #region Fields
        private readonly IRepository<Domain.Entities.ReviewComments> _reviewComments;
        #endregion

        #region Constructor
        public ReviewCommentsService(IRepository<Domain.Entities.ReviewComments> reviewComments)
        {
            _reviewComments = reviewComments;
        }
        #endregion

        #region Methods
        IList<Entities.ReviewComments> IReviewCommentsService.GetAllAprovedReviewCommentsByUser(string UserId)
        {
            if (string.IsNullOrWhiteSpace(UserId))
                return null;
            try
            {

                return _reviewComments.Table.Where(c => c.IsApproved == true && c.IsActive == true && c.CommentForId==UserId).ToList();
            }
            catch (Exception)
            {

                return null;
            }
        }

        public bool InsertReviewComment(Entities.ReviewComments ReviewComments)
        {
            try
            {
                if (ReviewComments == null)
                    throw new ArgumentNullException(nameof(ReviewComments));

                _reviewComments.Insert(ReviewComments);

                return true;
            }
            catch {
                return false;
            }
        }

        //public decimal GetAverageScoreByUser(string UserId)
        //{
        //    from r in _reviewComments.Table
        //    where r.IsActive == true
        //        && r.IsApproved == true
        //        && r.CommentForId == UserId
        //    select r.Rating;
            
        //    //return _reviewComments.Table.Where(c => c.IsApproved == true && c.IsActive == true && c.CommentForId == UserId).ToList();
        //}

        //public IList<RatingDetails> GetRatingDetailsScoreByUser(string UserId)
        //{
        //    throw new NotImplementedException();
        //}
        #endregion
    }
}
