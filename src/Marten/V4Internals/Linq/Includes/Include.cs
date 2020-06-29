using System;
using Marten.Linq.Fields;
using Marten.Linq.QueryHandlers;
using Marten.Util;

namespace Marten.V4Internals.Linq.Includes
{
    public class Include<T> : IInclude
    {
        private readonly IDocumentStorage<T> _storage;
        private readonly Action<T> _callback;

        public Include(int index, IDocumentStorage<T> storage, IField connectingField, Action<T> callback)
        {
            _storage = storage;
            _callback = callback;

            IdAlias = "id" + (index + 1);

            TempSelector = $"{connectingField.TypedLocator} as {IdAlias}";
        }

        public string IdAlias { get;}
        public string TempSelector { get; }
        public Statement BuildStatement()
        {
            return new IncludedDocumentStatement(_storage, this);
        }

        public IIncludeReader BuildReader(IMartenSession session)
        {
            var selector = (ISelector<T>) _storage.BuildSelector(session);
            return new IncludeReader<T>(_callback, selector);
        }

        public class IncludedDocumentStatement : Statement
        {
            public IncludedDocumentStatement(IDocumentStorage<T> storage, Include<T> include) : base(storage, storage.Fields)
            {
                Where = new InTempTableWhereFragment(LinqConstants.IdListTableName, include.IdAlias);
            }

            protected override void configure(CommandBuilder sql)
            {
                base.configure(sql);
                sql.Append(";\n");
            }
        }
    }


}
