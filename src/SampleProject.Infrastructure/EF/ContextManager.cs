using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SampleProject.Infrastructure.EF
{
    public class ContextManager<TContext> where TContext : DbContext
    {
        private readonly ICustomDbContextFactory<TContext> _contextFactory;
        private readonly AsyncLocal<TContext?> _context = new();

        public ContextManager(ICustomDbContextFactory<TContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public TContext AcquireDbContext()
        {
            if (_context.Value == null)
            {
                _context.Value = _contextFactory.CreateDbContext();
            }
            return _context.Value;
        }

        public void ReleaseDbContext()
        {
            if (_context.Value != null)
            {
                _context.Value.Dispose();
                _context.Value = null;
            }
        }
    }
}
