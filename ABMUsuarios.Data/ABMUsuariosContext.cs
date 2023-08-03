using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ABMUsuarios.Shared;

namespace ABMUsuarios.Data
{
    public class ABMUsuariosContext : DbContext
    {
        public ABMUsuariosContext (DbContextOptions<ABMUsuariosContext> options)
            : base(options)
        {
        }

        public DbSet<ABMUsuarios.Shared.Usuario> Usuario { get; set; } = default!;
    }
}
