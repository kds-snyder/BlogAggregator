# BlogAggregator
Aggregate blog posts, written with Entity Framework code first SQL database and Web API, and AngularJS front end, and utilizing Google login

The database connection string is stored in files that are not uploaded to GitHub, as follows:

-The file .gitignore must ignore ConnectionString*.config

-Connection strings must be stored in the following files, in the specified folders:

Folder                                File
------                                -------
BlogAggregator.API                    ConnectionStringsAPI.config
BlogAggregator.WebJob/bin/Debug       ConnectionStringsWebJob.config
BlogAggregator.WebJob/bin/Release     ConnectionStringsWebJob.config

-Format for ConnectionStringsAPI.config: 
<add name="BlogAggregator" connectionString="(connection string for Azure SQL DB)" providerName="System.Data.SqlClient" />

-Format for ConnectionStringsWebJob.config in Debug folder:
<connectionStrings>
<add name="BlogAggregator" connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=BlogAggregator;Integrated Security=yes;MultipleActiveResultSets=true;" providerName="System.Data.SqlClient" />
</connectionStrings>

-Format for ConnectionStringsWebJob.config in Release folder:
<connectionStrings>
<add name="BlogAggregator" connectionString="(connection string for Azure SQL DB)" providerName="System.Data.SqlClient" />
</connectionStrings>