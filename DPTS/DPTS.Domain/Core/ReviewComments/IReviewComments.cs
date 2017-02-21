using System.Collections.Generic;
using DPTS.Domain.Entities;

namespace DPTS.Domain.Core.ReviewComments
{
    public interface IReviewCommentsService
    {
        IList<Entities.ReviewComments> GetAllAprovedReviewCommentsByUser(string UserId);
        //decimal GetAverageScoreByUser(string UserId);
        //IList<Entities.RatingDetails> GetRatingDetailsScoreByUser(string UserId);
        bool InsertReviewComment(Entities.ReviewComments ReviewComments);
    }
}
