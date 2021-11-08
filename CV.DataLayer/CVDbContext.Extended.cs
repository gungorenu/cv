using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using System.Diagnostics;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.SqlClient;

namespace CV.DataLayer
{
    internal partial class CVDbContext : DbContext
    {
        private static CVDbContext _instance;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connStrName">Connection string name</param>
        private CVDbContext(string connStrName)
            : base(connStrName)
        {
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static CVDbContext DatabaseContext
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CVDbContext(DataLayerConstants.ConnectionString);
                    _instance.Database.Connection.Disposed += (o, e) => _instance = null;
                }
                return _instance;
            }
        }

        public static bool Initialize()
        {
            try
            {
                if (_instance != null) return true;

                DatabaseContext.Database.Initialize(false);
                EventLog.WriteEventLog(EventLogEntryType.Information, "Code Database web application connected to database! SUCCESS");
                return true;
            }
            catch (Exception ex)
            {
                EventLog.WriteEventLog(EventLogEntryType.Error, "Code Database web application couldnot be connected to database! Error: {0}", ex.Message);
            }

            return false;
        }

        private ObjectContext ObjectContextReference
        {
            get
            {
                IObjectContextAdapter adapter = this as IObjectContextAdapter;
                return adapter.ObjectContext;
            }
        }

        #region Database Preparation

        public void CreateTablesWithCheck()
        {
            int res =0;

            if( !DoesSchemaExists("CV"))
            {
                CreateSchema();
            }

            if (!DoesTableExist("ERRORS"))
            {
                CreateTable_Error();
            }

            if (!DoesTableExist("COMPANIES"))
            {
                CreateTable_Company();

                CreateRecord_Personal();
            }

            if (!DoesTableExist("COMPANYINFO"))
            {
                CreateTable_CompanyInfo();
            }

            if (!DoesTableExist("PROJECTS"))
            {
                CreateTable_Project();
            }

            if (!DoesTableExist("PROJECTINFO"))
            {
                CreateTable_ProjectInfo();
            }

            DatabaseContext.SaveChanges();
        }

        private int CreateSchema()
        {
            string query = "CREATE SCHEMA CV";
            this.Database.ExecuteSqlCommand(query);
            this.SaveChanges();
            return 1;
        }

        private void CreateRecord_Personal()
        {
            string query = @"INSERT INTO [CV].[COMPANIES] ( [NAME], [YEARSTART], [YEAREND], [POSITIONS], [LINK]) VALUES
                ( @name, @yearStart, NULL, @positions, @link )";

            this.Database.ExecuteSqlCommand(query,
                new SqlParameter("name", DataLayerConstants.PersonalProjectCompany),
                new SqlParameter("yearStart", new DateTime(2005, 09 , 01)),
                new SqlParameter("positions", "Software Developer"),
                new SqlParameter("link", "some link"));
        }

        private void CreateTable_Error()
        {
            string query = @"CREATE TABLE [CV].[ERRORS](
                [ID][int] IDENTITY(1, 1) NOT NULL,
                [ERROR] [ntext] NOT NULL,
                [CREATEDON] [datetime] NOT NULL,
                CONSTRAINT[PK_ERRORS] PRIMARY KEY CLUSTERED(
                    [ID] ASC
                ) WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]
                ) ON[PRIMARY] TEXTIMAGE_ON[PRIMARY]";

            this.Database.ExecuteSqlCommand(query);
        }

        private void CreateTable_Company()
        {
            string query = @"CREATE TABLE [CV].[COMPANIES](
	            [ID] [int] IDENTITY(1,1) NOT NULL,
	            [NAME] [nvarchar](255) NOT NULL,
	            [YEARSTART] [date] NOT NULL,
	            [YEAREND] [date] NULL,
	            [POSITIONS] [nvarchar](1023) NOT NULL,
	            [LINK] [nvarchar](1023) NULL,
                CONSTRAINT [PK_COMPANIES] PRIMARY KEY CLUSTERED (
	                [ID] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY]";

            this.Database.ExecuteSqlCommand(query);
        }

        private void CreateTable_CompanyInfo()
        {
            string query = @"CREATE TABLE [CV].[COMPANYINFO](
	            [ID] [int] IDENTITY(1,1) NOT NULL,
	            [CONTENT] [ntext] NOT NULL,
	            [RANK] [int] NOT NULL,
	            [HEADER] [nvarchar](255) NULL,
	            [COMPANY] [int] NOT NULL,
                CONSTRAINT [PK_COMPANYINFO] PRIMARY KEY CLUSTERED (
	                [ID] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";

            this.Database.ExecuteSqlCommand(query);

            query = "ALTER TABLE [CV].[COMPANYINFO] ADD  CONSTRAINT [DF_COMPANYINFO_RANK]  DEFAULT ((2)) FOR [RANK]";
            this.Database.ExecuteSqlCommand(query);

            query = @"ALTER TABLE [CV].[COMPANYINFO]  WITH CHECK ADD  CONSTRAINT [FK_COMPANYINFO_COMPANIES] FOREIGN KEY([COMPANY])
                REFERENCES [CV].[COMPANIES] ([ID])";
            this.Database.ExecuteSqlCommand(query);

            query = "ALTER TABLE [CV].[COMPANYINFO] CHECK CONSTRAINT [FK_COMPANYINFO_COMPANIES]";
            this.Database.ExecuteSqlCommand(query);
        }

        private void CreateTable_Project()
        {
            string query = @"CREATE TABLE [CV].[PROJECTS](
	            [ID] [int] IDENTITY(1,1) NOT NULL,
	            [NAME] [nvarchar](255) NOT NULL,
	            [COMPANY] [int] NOT NULL,
	            [YEARSTART] [date] NOT NULL,
	            [YEAREND] [date] NULL,
	            [LINK] [nvarchar](1023) NULL,
                CONSTRAINT [PK_PROJECTS] PRIMARY KEY CLUSTERED (
	                [ID] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY]";
            this.Database.ExecuteSqlCommand(query);

            query = @"ALTER TABLE [CV].[PROJECTS]  WITH CHECK ADD  CONSTRAINT [FK_PROJECTS_COMPANIES] FOREIGN KEY([COMPANY])
                REFERENCES[CV].[COMPANIES]([ID])";
            this.Database.ExecuteSqlCommand(query);

            query = "ALTER TABLE[CV].[PROJECTS] CHECK CONSTRAINT[FK_PROJECTS_COMPANIES]";
            this.Database.ExecuteSqlCommand(query);
        }

        private void CreateTable_ProjectInfo()
        {
            string query = @"CREATE TABLE [CV].[PROJECTINFO](
	            [ID] [int] IDENTITY(1,1) NOT NULL,
	            [CONTENT] [ntext] NOT NULL,
	            [RANK] [int] NOT NULL,
	            [HEADER] [nvarchar](255) NULL,
	            [PROJECT] [int] NOT NULL,
                CONSTRAINT [PK_PROJECTINFO] PRIMARY KEY CLUSTERED (
	                [ID] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
            this.Database.ExecuteSqlCommand(query);

            query = @"ALTER TABLE [CV].[PROJECTINFO] ADD  CONSTRAINT [DF_PROJECTINFO_RANK]  DEFAULT ((2)) FOR [RANK]";
            this.Database.ExecuteSqlCommand(query);

            query = @"ALTER TABLE [CV].[PROJECTINFO]  WITH CHECK ADD  CONSTRAINT [FK_PROJECTINFO_PROJECTS] FOREIGN KEY([PROJECT])
                REFERENCES [CV].[PROJECTS] ([ID])";
            this.Database.ExecuteSqlCommand(query);

            query = "ALTER TABLE [CV].[PROJECTINFO] CHECK CONSTRAINT [FK_PROJECTINFO_PROJECTS]";
            this.Database.ExecuteSqlCommand(query);
        }

        private bool DoesTableExist (string tableName)
        {
            return DoesThingExist(string.Format("SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'CV' AND  TABLE_NAME = '{0}'", tableName));
        }

        private bool DoesSchemaExists( string schema)
        {
            return DoesThingExist(string.Format("SELECT 1 FROM sys.schemas WHERE name = '{0}'", schema));
        }

        private bool DoesThingExist(string query)
        {
            var command = this.Database.SqlQuery<int>(query);
            int res = command.FirstOrDefault();
            return res > 0;
        }

        #endregion

    }
}
