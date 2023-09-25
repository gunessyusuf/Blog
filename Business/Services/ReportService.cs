using AppCore.DataAccess.EntityFramework.Bases;
using Business.Models.Report;
using DataAccess;

namespace Business.Services
{
    public interface IReportService
    {
        List<ReportItemModel> GetList(bool useInnerJoin = true, FilterModel filter = null);
        List<ReportItemGroupByModel> GetListGroupBy();
    }

    public class ReportService : IReportService
    {
        private readonly RepoBase<Blog> _blogRepo;

        public ReportService(RepoBase<Blog> blogRepo)
        {
            _blogRepo = blogRepo;
        }

        public IQueryable<ReportItemModel> GetQuery(bool useInnerJoin = true)
        {
            #region Sorgu Oluşturma
            var blogQuery = _blogRepo.Query();
            var tagQuery = _blogRepo.Query<Tag>();
            var blogTagQuery = _blogRepo.Query<BlogTag>();
            var userQuery = _blogRepo.Query<User>();
            var roleQuery = _blogRepo.Query<Role>();

            IQueryable<ReportItemModel> query;

            if (useInnerJoin)
            {
                query = from b in blogQuery
                        join bt in blogTagQuery
                        on b.Id equals bt.BlogId
                        join t in tagQuery
                        on bt.TagId equals t.Id
                        join u in userQuery
                        on b.UserId equals u.Id
                        join r in roleQuery
                        on u.RoleId equals r.Id
                        //where t.Name == "Computer"
                        //orderby b.CreateDate descending
                        select new ReportItemModel()
                        {
                            Active = u.IsActive == true ? "Yes" : u.IsActive == false ? "No" : "",
                            BlogContent = b.Content,
                            BlogCreateDate = b.CreateDate.ToString("MM/dd/yyyy"),
                            BlogTitle = b.Title,
                            BlogUpdateDate = b.UpdateDate.HasValue ? b.UpdateDate.Value.ToString("MM/dd/yyyy") : "",
                            IsPopular = t.IsPopular,
                            Popular = t.IsPopular == true ? "Yes" : t.IsPopular == false ? "No" : "",
                            RoleName = r.Name,
                            Score = b.Score,
                            Tag = t.Name,
                            UserName = u.UserName,
                            BlogCreateDateInput = b.CreateDate,
                            BlogUpdateDateInput = b.UpdateDate,
                            UserId = u.Id,
                            RoleId = r.Id,
                            TagId = t.Id
                        };
            }
            else // left outer join
            {
                query = from b in blogQuery
                        join bt in blogTagQuery
                        on b.Id equals bt.BlogId into blogTagJoin
                        from blogTag in blogTagJoin.DefaultIfEmpty()
                        join t in tagQuery
                        on blogTag.TagId equals t.Id into tagJoin
                        from tag in tagJoin.DefaultIfEmpty()
                        join u in userQuery
                        on b.UserId equals u.Id into userJoin
                        from user in userJoin.DefaultIfEmpty()
                        join r in roleQuery
                        on user.RoleId equals r.Id into roleJoin
                        from role in roleJoin.DefaultIfEmpty()
                        select new ReportItemModel()
                        {
                            Active = user.IsActive == true ? "Yes" : user.IsActive == false ? "No" : "",
                            BlogContent = b.Content,
                            BlogCreateDate = b.CreateDate.ToString("MM/dd/yyyy"),
                            BlogTitle = b.Title,
                            BlogUpdateDate = b.UpdateDate.HasValue ? b.UpdateDate.Value.ToString("MM/dd/yyyy") : "",
                            IsPopular = tag.IsPopular,
                            Popular = tag.IsPopular == true ? "Yes" : tag.IsPopular == false ? "No" : "",
                            RoleName = role.Name,
                            Score = b.Score,
                            Tag = tag.Name,
                            UserName = user.UserName,
                            BlogCreateDateInput = b.CreateDate,
                            BlogUpdateDateInput = b.UpdateDate,
                            UserId = user.Id,
                            RoleId = role.Id,
                            TagId = tag.Id
                        };
            }

            return query;
            #endregion
        }

        public List<ReportItemModel> GetList(bool useInnerJoin = true, FilterModel filter = null)
        {
            var query = GetQuery(useInnerJoin);

            #region Sıralama
            //query = query.OrderByDescending(q => q.BlogCreateDate).ThenByDescending(q => q.BlogUpdateDate).ThenBy(q => q.BlogTitle);
            query = query.OrderBy(q => q.BlogTitle);
            #endregion

            #region Filtreleme
            if (filter is not null)
            {
                if (!string.IsNullOrWhiteSpace(filter.BlogTitle))
                {
                    query = query.Where(q => q.BlogTitle.ToUpper().Contains(filter.BlogTitle.ToUpper().Trim()));
                }
                if (filter.CreateDateBegin.HasValue)
                {
                    query = query.Where(q => q.BlogCreateDateInput >= filter.CreateDateBegin.Value);
                }
                if (filter.CreateDateEnd.HasValue)
                {
                    query = query.Where(q => q.BlogCreateDateInput <= filter.CreateDateEnd.Value);
                }
                if (filter.ScoreBegin.HasValue)
                {
                    query = query.Where(q => q.Score >= filter.ScoreBegin.Value);
                }
                if (filter.ScoreEnd.HasValue)
                {
                    query = query.Where(q => q.Score <= filter.ScoreEnd.Value);
                }
                if (filter.RoleId.HasValue)
                {
                    query = query.Where(q => q.RoleId == filter.RoleId);
                }
                if (filter.UserId.HasValue)
                {
                    query = query.Where(q => q.UserId == filter.UserId);
                }
                if (filter.TagIds is not null && filter.TagIds.Any()) 
                {
                    query = query.Where(q => filter.TagIds.Contains(q.TagId ?? 0));
                }
            }
            #endregion

            return query.ToList();
        }

        public List<ReportItemGroupByModel> GetListGroupBy()
        {
            var query = GetQuery();

            var groupByQuery = from q in query
                               //group q by q.Tag
                               group q by new { q.TagId, q.Tag } into qGroupBy
                               select new ReportItemGroupByModel()
                               {
                                   TagId = qGroupBy.Key.TagId.Value, // ?? 0
                                   TagName = qGroupBy.Key.Tag,
                                   AverageScore = qGroupBy.Average(qgb => qgb.Score ?? 0).ToString("N1")
                               };

            return groupByQuery.ToList();
        }
    }
}
