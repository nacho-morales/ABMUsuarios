using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using ABMUsuarios.Controllers;
using ABMUsuarios.Data;
using ABMUsuarios.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = Xunit.Assert;

namespace ABMUsuarios.Tests
{
    public class UsuariosControllerTests : IDisposable
    {

        private readonly ABMUsuariosContext _dbContext;

        public UsuariosControllerTests()
        {
            // Configurar opciones para una base de datos en memoria (SQLite) para las pruebas
            var options = new DbContextOptionsBuilder<ABMUsuariosContext>()
                .UseSqlite("DataSource=:memory:") // Usar SQLite en memoria
                .Options;

            // Crear el contexto de la base de datos utilizando las opciones configuradas
            _dbContext = new ABMUsuariosContext(options);

            // Abrir la conexión de la base de datos (necesario para SQLite en memoria)
            _dbContext.Database.OpenConnection();

            // Asegurarse de que el contexto esté creado y la base de datos esté creada
            _dbContext.Database.EnsureCreated();
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Fact]
        public async Task GetUsuario_DebeRetornarTodosLosUsuarios()
        {
            // Arrange
            var controller = new UsuariosController(_dbContext);

            // Limpiar la base de datos de prueba antes de cada prueba
            _dbContext.Database.EnsureDeleted();

            // Insertar algunos datos de prueba en la base de datos en memoria
            _dbContext.Usuario.AddRange(
                new Usuario { Id = 1, Nombre = "Usuario1", Email = "usuario1@example.com" },
                new Usuario { Id = 2, Nombre = "Usuario2", Email = "usuario2@example.com" }
            );
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await controller.GetUsuario();
            var okResult = result.Value as IEnumerable<Usuario>;

            // Assert
            Assert.NotNull(okResult);
            Assert.Equal(2, okResult.Count());
        }

        [Fact]
        public async Task GetUsuario_DebeRetornarUsuarioPorId()
        {
            // Arrange
            var controller = new UsuariosController(_dbContext);

            // Limpiar la base de datos de prueba antes de cada prueba
            _dbContext.Database.EnsureDeleted();

            // Insertar algunos datos de prueba en la base de datos en memoria
            _dbContext.Usuario.AddRange(
                new Usuario { Id = 1, Nombre = "Usuario1", Email = "usuario1@example.com", FechaCreacion = DateTime.Now },
                new Usuario { Id = 2, Nombre = "Usuario2", Email = "usuario2@example.com", FechaCreacion = DateTime.Now }
            );
            await _dbContext.SaveChangesAsync();

            // Act
            var resultUsuarioEncontrado = await controller.GetUsuario(1);

            // Assert
            Assert.NotNull(resultUsuarioEncontrado);
            var usuarioEncontrado = Assert.IsType<Usuario>(resultUsuarioEncontrado.Value);
            Assert.Equal(1, usuarioEncontrado.Id);
            Assert.Equal("Usuario1", usuarioEncontrado.Nombre);
            Assert.Equal("usuario1@example.com", usuarioEncontrado.Email);
        }
        [Fact]
        public async Task PostUsuario_DebeAgregarNuevoUsuario()
        {
            // Arrange
            var controller = new UsuariosController(_dbContext);
            var newUsuario = new Usuario { Id = 3, Nombre = "NuevoUsuario", Email = "nuevo_usuario@example.com" };

            // Act
            var result = await controller.PostUsuario(newUsuario);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdUsuario = Assert.IsAssignableFrom<Usuario>(createdResult.Value);
            Assert.Equal(newUsuario.Id, createdUsuario.Id);

            // Verificar que el usuario fue agregado a la base de datos
            var usuarioInDatabase = await _dbContext.Usuario.FindAsync(newUsuario.Id);
            Assert.NotNull(usuarioInDatabase);
            Assert.Equal(newUsuario.Nombre, usuarioInDatabase.Nombre);
            Assert.Equal(newUsuario.Email, usuarioInDatabase.Email);
        }

        [Fact]
        public async Task PutUsuario_DebeActualizarUsuarioExistente()
        {
            // Arrange
            var controller = new UsuariosController(_dbContext);
            var usuarioId = 1;
            var updatedUsuario = new Usuario { Id = usuarioId, Nombre = "UsuarioActualizado", Email = "usuario_actualizado@example.com" };

            // Insertar algunos datos de prueba en la base de datos en memoria
            _dbContext.Usuario.AddRange(
                new Usuario { Id = usuarioId, Nombre = "Usuario1", Email = "usuario1@example.com" },
                new Usuario { Id = 2, Nombre = "Usuario2", Email = "usuario2@example.com" }
            );
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await controller.PutUsuario(usuarioId, updatedUsuario);

            // Assert
            Assert.IsType<NoContentResult>(result);

            // Verificar que el usuario fue actualizado en la base de datos
            var usuarioInDatabase = await _dbContext.Usuario.FindAsync(usuarioId);
            Assert.NotNull(usuarioInDatabase);
            Assert.Equal(updatedUsuario.Nombre, usuarioInDatabase.Nombre);
            Assert.Equal(updatedUsuario.Email, usuarioInDatabase.Email);
        }

        [Fact]
        public async Task DeleteUsuario_DebeEliminarUsuario()
        {
            // Arrange
            var controller = new UsuariosController(_dbContext);
            var usuarioId = 2;

            // Insertar algunos datos de prueba en la base de datos en memoria
            _dbContext.Usuario.AddRange(
                new Usuario { Id = 1, Nombre = "Usuario1", Email = "usuario1@example.com" },
                new Usuario { Id = usuarioId, Nombre = "Usuario2", Email = "usuario2@example.com" }
            );
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await controller.DeleteUsuario(usuarioId);

            // Assert
            Assert.IsType<NoContentResult>(result);

            // Verificar que el usuario fue eliminado de la base de datos
            var usuarioInDatabase = await _dbContext.Usuario.FindAsync(usuarioId);
            Assert.Null(usuarioInDatabase);
        }
    }
}