using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace StarBankApp.Models
{
    [SQLite.Table("Usuarios")]
    public class Usuarios
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull, Unique]
        public string usuario { get; set; }
        public int userId { get; set; }

        public string no_doc { get; set; }
        public string name { get; set; }
        public string correo { get; set; }
        public string direccion { get; set; }
        public string token { get; set; }
    }

    public class UserResponse
    {
        public User user { get; set; }
        public string token { get; set; }
    }
        public class User
    {
        public int id { get; set; }
        public string no_doc { get; set; }
        public string nombre { get; set; }
        public string correo { get; set; }
        public string direccion { get; set; }
        public string usuario { get; set; }
        public int clave_secreta { get; set; }
        public string keyword { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
    }
}
