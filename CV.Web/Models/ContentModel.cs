using CV.DataLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CV.Web.Models
{
    public class ContentModel : IContent
    {
        public int ContentID { get; set; }

        [Display(Name = "Content"), Required]
        public string Content { get; set; }

        [Display(Name = "Rank"), Required]
        public int Rank { get; set; }

        [Display(Name = "Header")]
        public string Header { get; set; }

        public int OwnerID { get; set; }

        int IContent.ID
        {
            get
            {
                return this.ContentID;
            }
            set
            {
                this.ContentID = value;
            }
        }

        string IContent.Content
        {
            get
            {
                return this.Content;
            }
            set
            {
                this.Content = value;
            }
        }

        int IContent.Rank
        {
            get
            {
                return this.Rank;
            }
            set
            {
                this.Rank = value;
            }
        }

        string IContent.Header
        {
            get
            {
                return this.Header;
            }
            set
            {
                this.Header = value;
            }
        }

        int IContent.OwnerId
        {
            get
            {
                return this.OwnerID;
            }
            set
            {
                this.OwnerID = value;
            }
        }

        public ContentModel()
        {
            ContentID = 0;
        }

        public ContentModel(IContent copy)
        {
            ContentID = copy.ID;
            Content = copy.Content;
            Rank = copy.Rank;
            Header = copy.Header;
            OwnerID = copy.OwnerId;
        }
    }
}