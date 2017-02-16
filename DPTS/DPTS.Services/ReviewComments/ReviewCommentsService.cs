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
                return _reviewComments.Table.Where(c => c.IsApproved == true && c.IsActive == true).ToList();
            }
            catch (Exception)
            {

                return null;
            }
        }
        #endregion
    }
}
