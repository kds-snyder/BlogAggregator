// <auto-generated />
namespace BlogAggregator.Data.Migrations
{
    using System.CodeDom.Compiler;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Infrastructure;
    using System.Resources;
    
    [GeneratedCode("EntityFramework.Migrations", "6.1.3-40302")]
    public sealed partial class AddGuidToPostTable : IMigrationMetadata
    {
        private readonly ResourceManager Resources = new ResourceManager(typeof(AddGuidToPostTable));
        
        string IMigrationMetadata.Id
        {
            get { return "201512110436543_AddGuidToPostTable"; }
        }
        
        string IMigrationMetadata.Source
        {
            get { return null; }
        }
        
        string IMigrationMetadata.Target
        {
            get { return Resources.GetString("Target"); }
        }
    }
}