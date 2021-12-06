using System;
using System.Collections.Generic;
using System.IO;
using Marten.Events.Archiving;
using Marten.Events.Schema;
using Marten.Storage;
using Marten.WeaselExport;
using Weasel.Core;
using Weasel.Postgresql;
using Weasel.Postgresql.Functions;

namespace Marten.Events
{
    public partial class EventGraph : IFeatureSchema
    {

        internal DbObjectName ProgressionTable => new DbObjectName(DatabaseSchemaName, "mt_event_progression");
        internal DbObjectName StreamsTable => new DbObjectName(DatabaseSchemaName, "mt_streams");


        IEnumerable<Type> IFeatureSchema.DependentTypes()
        {
            yield break;
        }

        ISchemaObject[] ISchemaObjectGroup.Objects
        {
            get
            {
                var eventsTable = new EventsTable(this);
                var streamsTable = new StreamsTable(this);

                #region sample_using-sequence
                var sequence = new Sequence(new DbObjectName(DatabaseSchemaName, "mt_events_sequence"))
                {
                    Owner = eventsTable.Identifier,
                    OwnerColumn = "seq_id"
                };
                #endregion


                return new ISchemaObject[]
                {
                    streamsTable,
                    eventsTable,
                    new EventProgressionTable(DatabaseSchemaName),
                    sequence,
                    new SystemFunction(DatabaseSchemaName, "mt_mark_event_progression", "varchar, bigint"),
                    Function.ForRemoval(new DbObjectName(DatabaseSchemaName, "mt_append_event")),
                    new ArchiveStreamFunction(this)
                };
            }
        }

        Type IFeatureSchema.StorageType => typeof(EventGraph);
        string ISchemaObjectGroup.Identifier { get; } = "eventstore";

        void IFeatureSchema.WritePermissions(DdlRules rules, TextWriter writer)
        {
            // Nothing
        }

    }
}
