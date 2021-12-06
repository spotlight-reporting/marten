using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Marten.Schema;

namespace Marten.WeaselExport
{
    internal class DatabaseSchemaGenerator
    {
        private const string BeginScript = @"DO $$
BEGIN";

        private const string EndScript = @"END
$$;
";

        public static string GenerateScript(IEnumerable<string> schemaNames)
        {
            if (schemaNames == null)
                throw new ArgumentNullException(nameof(schemaNames));

            var names = schemaNames
                 .Distinct()
                 .Where(name => name != SchemaConstants.DefaultSchema).ToList();

            if (!names.Any())
                return null;

            using var writer = new StringWriter();
            WriteSql(names, writer);

            return writer.ToString();
        }

        public static void WriteSql(IEnumerable<string> schemaNames, TextWriter writer)
        {
            writer.Write(BeginScript);
            foreach (var schemaName in schemaNames)
            {
                WriteSql(schemaName, writer);
            }
            writer.WriteLine(EndScript);
        }

        private static void WriteSql(string databaseSchemaName, TextWriter writer)
        {
            writer.WriteLine($@"
    IF NOT EXISTS(
        SELECT schema_name
          FROM information_schema.schemata
          WHERE schema_name = '{databaseSchemaName}'
      )
    THEN
      EXECUTE 'CREATE SCHEMA {databaseSchemaName}';
    END IF;
");
        }
    }
}
