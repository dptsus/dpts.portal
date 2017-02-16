using System.Collections.Generic;
using DPTS.Domain.Entities;

namespace DPTS.Domain.Core.ReviewComments
{
    public interface IReviewCommentsService
    {
        IList<Entities.ReviewComments> GetAllAprovedReviewCommentsByUser(string UserId);
    }
}
