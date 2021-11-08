using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.DataLayer
{
    public interface IContent
    {
        int ID { get; set; }
        string Content { get; set; }
        int Rank { get; set; }
        string Header { get; set; }
        int OwnerId { get; set; }
    }

    public partial class CompanyContent : IContent
    {
        public CompanyContent()
        {
        }

        public CompanyContent(IContent copy)
        {
            this.Content = copy.Content;
            this.OwnerId = copy.OwnerId;
            this.Header = copy.Header;
            this.ID = copy.ID;
            this.Rank = copy.Rank;
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

        int IContent.ID
        {
            get
            {
                return this.ID;
            }
            set
            {
                this.ID = value;
            }
        }

        int IContent.OwnerId
        {
            get
            {
                return this.OwnerId;
            }
            set
            {
                this.OwnerId = value;
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
    }

    public partial class ProjectContent : IContent
    {
        public ProjectContent()
        {
        }

        public ProjectContent(IContent copy)
        {
            this.Content = copy.Content;
            this.OwnerId = copy.OwnerId;
            this.Header = copy.Header;
            this.ID = copy.ID;
            this.Rank = copy.Rank;
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

        int IContent.ID
        {
            get
            {
                return this.ID;
            }
            set
            {
                this.ID = value;
            }
        }

        int IContent.OwnerId
        {
            get
            {
                return this.OwnerId;
            }
            set
            {
                this.OwnerId = value;
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
    }
}
