using CV.DataLayer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CV.Web.Models
{
    public class ContentContainerModel
    {
        public int OwnerID { get; private set; }
        public List<ContentModel> Elements { get; private set; }

        public ContentContainerModel()
        {

        }

        public static ContentContainerModel Create<T>(int ownerId, DbSet<T> source)
            where T: class, IContent
        {
            ContentContainerModel result = new ContentContainerModel();
            result.OwnerID = ownerId;

            var results = (from c in source
                          orderby c.Rank ascending
                          where c.OwnerId == result.OwnerID
                          select new ContentModel() { Content = c.Content, ContentID = c.ID, Header = c.Header, OwnerID = c.OwnerId, Rank = c.Rank });
            result.Elements = new List<ContentModel>(results);

            return result;
        }
    }
}