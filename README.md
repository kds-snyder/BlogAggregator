# BlogAggregator
Aggregate blog posts, written with Entity Framework code first SQL database and Web API, and AngularJS front end, and utilizing Google login

The database connection string is stored in files that are not uploaded to GitHub, as follows:

-The file .gitignore must ignore ConnectionString*.config

-Connection strings must be stored in the following files, in the specified folders:

--File ConnectionStringsAPI.config in folder BlogAggregator.API
---Format for ConnectionStringsAPI.config: 
<add name="BlogAggregator" connectionString="(connection string for Azure SQL DB)" providerName="System.Data.SqlClient" />

--File ConnectionStringsWebJob.config in folder BlogAggregator.WebJob/bin/Debug
----Format for ConnectionStringsWebJob.config in Debug folder:
<connectionStrings>
<add name="BlogAggregator" connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=BlogAggregator;Integrated Security=yes;MultipleActiveResultSets=true;" providerName="System.Data.SqlClient" />
</connectionStrings>

--File ConnectionStringsWebJob.config in BlogAggregator.WebJob/bin/Release   
---Format for ConnectionStringsWebJob.config in Release folder:
<connectionStrings>
<add name="BlogAggregator" connectionString="(connection string for Azure SQL DB)" providerName="System.Data.SqlClient" />
</connectionStrings>