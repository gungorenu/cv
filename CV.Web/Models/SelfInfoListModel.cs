using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CV.Web.Models
{
    public class SelfInfoContentListModel 
    {
        private List<ContentModel> _elements = null;

        public List<ContentModel> Elements
        {
            get
            {
                if( _elements == null )
                {
                    var query = from m in CV.DataLayer.CVDbContext.DatabaseContext.COMPANYINFO
                                join CV.DataLayer.Company c in CV.DataLayer.CVDbContext.DatabaseContext.COMPANIES on m.OwnerId equals c.ID
                                where c.Name == DataLayer.DataLayerConstants.PersonalProjectCompany
                                orderby m.Rank ascending
                                select new ContentModel() { Content = m.Content, ContentID = m.ID, Header = m.Header, OwnerID = m.OwnerId, Rank = m.Rank };

                    _elements = new List<ContentModel>(query);
                }
                return _elements;
            }
        }

        public int OwnerId
        {
            get
            {
                return (from m in CV.DataLayer.CVDbContext.DatabaseContext.COMPANIES
                        where m.Name == DataLayer.DataLayerConstants.PersonalProjectCompany
                        select m.ID).First();
            }
        }

    }
}