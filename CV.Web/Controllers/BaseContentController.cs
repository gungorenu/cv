using CV.DataLayer;
using CV.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CV.Web.Controllers
{
    public abstract class BaseContentController<T> : BaseController
        where T : class, IContent
    {
        #region Helpers
        private int MaxRankOfOwner(int ownerId)
        {
            IQueryable<int> query = null;
            query = from m in CV.DataLayer.CVDbContext.DatabaseContext.Set<T>()
                    where m.OwnerId == ownerId
                    orderby m.Rank descending
                    select m.Rank;
            return query.FirstOrDefault();
        }

        protected abstract bool ValidateOwner(int ownerId);

        protected abstract T GetDataModelFromViewModel(ContentModel viewModel);

        protected System.Data.Entity.DbSet<T> ModelSet
        {
            get
            {
                return CV.DataLayer.CVDbContext.DatabaseContext.Set<T>();
            }
        }

        protected virtual ContentContainerModel GetContentContainerModel(int ownerId)
        {
            return ContentContainerModel.Create<T>(ownerId, ModelSet);
        }
        #endregion

        #region Actions
        public ActionResult Index()
        {
            return RedirectToIndex();
        }

        [HttpGet]
        public ActionResult ShowContents(int ownerId)
        {
            ContentContainerModel clm = GetContentContainerModel(ownerId);

            return PartialView("~/Views/Shared/DisplayTemplates/ContentContainerModel.cshtml", clm);
        }

        [HttpGet, Authorize]
        public ActionResult EditContents(int ownerId)
        {
            ContentContainerModel clm = GetContentContainerModel(ownerId);

            return PartialView("~/Views/Shared/EditorTemplates/ContentContainerModel.cshtml", clm);
        }

        [HttpPost, Authorize, ValidateInput(false)]
        public ActionResult SaveContent(ContentModel contentModel)
        {
            try
            {
                if (!IsAdministrator)
                    return Json("Only administrators can change content!");

                if (contentModel.OwnerID == 0 || string.IsNullOrEmpty(contentModel.Content))
                    return Json("Validation failed for content!");

                T contentDataModel = null;
                if (contentModel.ContentID == 0)
                {
                    contentDataModel = GetDataModelFromViewModel(contentModel);
                    contentDataModel.Rank = MaxRankOfOwner(contentModel.OwnerID) + 2;
                    ModelSet.Add(contentDataModel);
                }
                else
                {
                    contentDataModel = (from m in ModelSet
                                        where m.ID == contentModel.ContentID && m.OwnerId == contentModel.OwnerID
                                        select m).FirstOrDefault();

                    if (contentDataModel == null)
                        return Json("Content not found registered to the owner!");

                    // only these two data can be changed with this function
                    contentDataModel.Content = contentModel.Content;
                    contentDataModel.Header = contentModel.Header;
                }

                CV.DataLayer.CVDbContext.DatabaseContext.SaveChanges();

                return Json("SUCCESS");
            }
            catch (Exception ex)
            {
                string error = HandleError(ex);
                return Json(string.Format("Content saving failed! {0}", error));
            }
        }

        [HttpPost, Authorize]
        public ActionResult DeleteContent(int contentId)
        {
            try
            {
                if (!IsAdministrator)
                    return Json("Only administrators can change content!");

                if (contentId <= 0)
                    return Json("Validation failed for content!");

                T contentDataModel = (from m in ModelSet
                                      where m.ID == contentId
                                      select m).FirstOrDefault();

                if (contentDataModel == null)
                    return Json("Content not found in system!");

                ModelSet.Remove(contentDataModel);

                CV.DataLayer.CVDbContext.DatabaseContext.SaveChanges();

                return Json("SUCCESS");
            }
            catch (Exception ex)
            {
                string error = HandleError(ex);
                return Json(string.Format("Content deletion failed! {0}", error));
            }
        }

        [HttpPost, Authorize]
        public ActionResult ShiftContentUp(int contentId)
        {
            try
            {
                if (!IsAdministrator)
                    return Json("Only administrators can shift content!");

                if (contentId <= 0)
                    return Json("Validation failed for content!");

                T contentDataModel = (from m in ModelSet
                                      where m.ID == contentId
                                      select m).FirstOrDefault();

                if (contentDataModel == null)
                    return Json("Content not found in system!");

                int currentRank = contentDataModel.Rank;

                var nextContentDataModel = (from m in ModelSet
                                            where m.ID != contentId && m.Rank < currentRank && m.OwnerId == contentDataModel.OwnerId
                                            orderby m.Rank descending
                                            select m).FirstOrDefault();
                if (nextContentDataModel == null)
                    return Json("Current content is the first content in associated information, shifting up operation cannot proceed!");

                // swap
                contentDataModel.Rank = nextContentDataModel.Rank;
                nextContentDataModel.Rank = currentRank;

                CV.DataLayer.CVDbContext.DatabaseContext.SaveChanges();

                return Json("SUCCESS");
            }
            catch (Exception ex)
            {
                string error = HandleError(ex);
                return Json(string.Format("Content shifting failed! {0}", error));
            }
        }

        [HttpPost, Authorize]
        public ActionResult ShiftContentDown(int contentId)
        {
            try
            {
                if (!IsAdministrator)
                    return Json("Only administrators can shift content!");

                if (contentId <= 0)
                    return Json("Validation failed for content!");

                T contentDataModel = (from m in ModelSet
                                      where m.ID == contentId
                                      select m).FirstOrDefault();

                if (contentDataModel == null)
                    return Json("Content not found in system!");

                int currentRank = contentDataModel.Rank;

                var nextContentDataModel = (from m in ModelSet
                                   where m.ID != contentId && m.Rank > currentRank && m.OwnerId == contentDataModel.OwnerId
                                   orderby m.Rank ascending
                                   select m).FirstOrDefault();
                if (nextContentDataModel == null)
                    return Json("Current content is the last content in associated information, shifting down operation cannot proceed!");

                // swap
                contentDataModel.Rank = nextContentDataModel.Rank;
                nextContentDataModel.Rank = currentRank;

                CV.DataLayer.CVDbContext.DatabaseContext.SaveChanges();

                return Json("SUCCESS");
            }
            catch (Exception ex)
            {
                string error = HandleError(ex);
                return Json(string.Format("Content shifting failed! {0}", error));
            }
        }

        [HttpPost, Authorize]
        public ActionResult NewContent(int ownerId)
        {
            try
            {
                if (!IsAdministrator)
                    return Json("Only administrators can shift content!");

                if (ownerId <= 0)
                    return Json("Validation failed for content!");

                if( !ValidateOwner(ownerId) )
                    return Json("Associated content owner not found in system!");

                int currentRank = MaxRankOfOwner(ownerId);

                T contentDataModel = GetDataModelFromViewModel(new ContentModel()
                {
                    OwnerID = ownerId,
                    Rank = currentRank + 2,
                    Content = "New Content",
                    Header = "New Header",
                });

                ModelSet.Add(contentDataModel);

                CV.DataLayer.CVDbContext.DatabaseContext.SaveChanges();

                return Json("SUCCESS");
            }
            catch (Exception ex)
            {
                string error = HandleError(ex);
                return Json(string.Format("Content creation failed! {0}", error));
            }
        }
        #endregion

    }
}