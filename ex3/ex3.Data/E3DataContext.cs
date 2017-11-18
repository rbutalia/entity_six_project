

using System;
using ex3.Domain;
using System.Linq;
using System.Reflection;
using System.Data.Entity;
using ex3.Domain.History;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.Pluralization;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.ModelConfiguration;

namespace ex3.Data
{
    class E3DataContext : DbContext
    {
        public DbSet<Ninja> Ninjas { get; set; }
        public DbSet<Family> NinjaFamilies { get; set; }
        public DbSet<ClassWithGuidKey> ClassWithGuidKeys { get; set; }
        public DbSet<Consumer> Consumers { get; set; }
        public DbSet<Clan> Clans { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Add(new StringProperties150());
            ConventionRules.Apply(modelBuilder);

            modelBuilder.Properties<Guid>().Where(p => p.Name == "Key").Configure(c => c.IsKey());
            modelBuilder.Properties().Where(p => p.Name == "Name").Configure(c => c.HasParameterName(c.ClrPropertyInfo.DeclaringType.Name + "_Name"));

            modelBuilder.Configurations.Add(new NinjaConfigurations());

            modelBuilder.Conventions.Add(new TableSchemaAttributeConvention());
            modelBuilder.Conventions.Add(new ForeignKeyNameWithFkConvention());

            //modelBuilder.Types<IHistory>().Configure(c=>c.ToTable(c.ClrType.Name,"History"));
            base.OnModelCreating(modelBuilder);
        }

        public class ForeignKeyNameWithFkConvention : IStoreModelConvention<AssociationType>
        {
            public void Apply(AssociationType association, DbModel model)
            {
                if (association.IsForeignKey
                  && association.Constraint.FromRole
                  .RelationshipMultiplicity != RelationshipMultiplicity.One)
                {
                    var assnProperty = association.Constraint.ToProperties;
                    if (assnProperty[0].Name.EndsWith("Id"))
                    {
                        assnProperty[0].Name =
                          assnProperty[0].Name.Substring(0, assnProperty[0].Name.Length - 2) + "FKId";
                    }
                }
            }
        }

        public class NinjaConfigurations : EntityTypeConfiguration<Ninja>
        {
            public NinjaConfigurations()
            {
                MapToStoredProcedures(
                  s =>
                  {
                      s.Insert(i => i.HasName("InsertNinja")
                                     //.Parameter(n => n.Name, "NinjaName")
                                     // .Parameter(n => n.Clan.Id, "ClanId")  //commenting out after adding ClanId to Ninja class
                        .Navigation<Family>(f => f.Ninjas, n => n.Parameter(p => p.Id, "NinjaFamilyId"))
                        .Result(r => r.Id, "NinjaId"));
                      s.Update(u => u.HasName("UpdateNinja"));
                      s.Delete(d => d.HasName("DeleteNinja"));
                  });

                HasOptional(n => n.TrueIdentity)
                  .WithRequired(ti => ti.Ninja);
                // Property(n => n.Name).HasMaxLength(20);
            }
        }

        #region Convention definitions

        public class NameProperties45 : Convention
        {
            public NameProperties45()
            {
                Properties<String>().Where(s => s.Name == "Name")
               .Configure(s => s.HasMaxLength(45));

            }
        }

        public class StringProperties150 : Convention
        {
            public StringProperties150()
            {
                Properties<String>()
                 .Configure(s => s.HasMaxLength(150));
            }
        }

        public class TableSchemaAttributeConvention : Convention
        {
            public TableSchemaAttributeConvention()
            {
                Types()
                .Where(x => x.GetCustomAttributes(false)
                             .OfType<Schema>().Any())
                .Configure(c => c.ToTable(new EnglishPluralizationService()
                                              .Pluralize(c.ClrType.Name),
                                c.ClrType.GetCustomAttribute<Schema>().SchemaName));

            }
        }

        public class JoinTableNamingConvention : IStoreModelConvention<AssociationType>
        {
            public void Apply(AssociationType association, DbModel model)
            {
                // Identify ForeignKey properties (including IAs)  
                if (association.IsForeignKey)
                {
                    // rename FK columns  
                    var constraint = association.Constraint;
                    if (DoPropertiesHaveDefaultNames(constraint.FromProperties, constraint.ToRole.Name, constraint.ToProperties))
                    {
                        NormalizeForeignKeyProperties(constraint.FromProperties);
                    }
                    if (DoPropertiesHaveDefaultNames(constraint.ToProperties, constraint.FromRole.Name, constraint.FromProperties))
                    {
                        NormalizeForeignKeyProperties(constraint.ToProperties);
                    }
                }
            }


            private bool DoPropertiesHaveDefaultNames(ReadOnlyMetadataCollection<EdmProperty> properties, string roleName, ReadOnlyMetadataCollection<EdmProperty> otherEndProperties)
            {
                if (properties.Count != otherEndProperties.Count)
                {
                    return false;
                }

                for (int i = 0; i < properties.Count; ++i)
                {
                    if (!properties[i].Name.EndsWith("_" + otherEndProperties[i].Name))
                    {
                        return false;
                    }
                }
                return true;
            }

            private void NormalizeForeignKeyProperties(ReadOnlyMetadataCollection<EdmProperty> properties)
            {
                for (int i = 0; i < properties.Count; ++i)
                {
                    int underscoreIndex = properties[i].Name.IndexOf('_');
                    if (underscoreIndex > 0)
                    {
                        properties[i].Name = properties[i].Name.Remove(underscoreIndex, 1);
                    }
                }
            }
        }

        static class ConventionRules
        {
            public static void Apply(DbModelBuilder modelBuilder)
            {
                modelBuilder.Conventions.AddAfter<StringProperties150>(new NameProperties45());
            }
        }
        #endregion
    }

}
