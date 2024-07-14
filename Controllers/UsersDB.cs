using System;
using System.Threading.Tasks;
using SQLite;

namespace StarBankApp.Controllers
{
    public class UsersDB
    {
        SQLiteAsyncConnection _connection;

        public UsersDB() { }

        public async Task Init()
        {
            try
            {
                if (_connection is null)
                {
                    SQLite.SQLiteOpenFlags extensiones = SQLite.SQLiteOpenFlags.ReadWrite | SQLite.SQLiteOpenFlags.Create | SQLite.SQLiteOpenFlags.SharedCache;

                    if (string.IsNullOrEmpty(FileSystem.AppDataDirectory))
                    {
                        return;
                    }

                    _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, "DBStarBankApp"), extensiones);
                    await _connection.CreateTableAsync<Models.Usuarios>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Init(): {ex.Message}");
            }
        }

        public async Task<int> InsertUsuario(string usuario)
        {
            await Init();
            var dinousuario = new Models.Usuarios { usuario = usuario };
            int result = await _connection.InsertAsync(dinousuario);
            return result;
        }

        public async Task<Models.Usuarios> GetUltimoUsuario()
        {
            await Init();
            return await _connection.Table<Models.Usuarios>().OrderByDescending(u => u.Id).FirstOrDefaultAsync();
        }

        public async Task<int> UpdateUsuario(Models.Usuarios usuario)
        {
            await Init();
            return await _connection.UpdateAsync(usuario);
        }
    }
}
