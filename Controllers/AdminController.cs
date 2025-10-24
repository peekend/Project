using Diplom2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Npgsql;
using System.Security.Claims;
using Tensorflow.Operations.Initializers;
using static Google.Protobuf.WellKnownTypes.Field.Types;

namespace Diplom2.Controllers
{
    public class AdminController : Controller
    {
        //    [Authorize(Roles = "Admin")]
        //    [HttpGet]
        //    public IActionResult StudentsAdd()
        //    {
        //        return View("StudentsAdd");
        //    }
        //    [Authorize(Roles = "Admin")]
        //    [IgnoreAntiforgeryToken]
        //    [HttpPost]
        //    async public Task<IActionResult> StudentsAdd(string name,string surname,string middle_name,int age,string group)
        //    {

        //        await using var con = new NpgsqlConnection("Server=localhost;Port=5432;Username=postgres;Password=18082004;Database=users_db");
        //        const string sql = @"
        //        INSERT INTO students (name, surname, middle_name, age, group_name)
        //        VALUES (@Name, @Surname, @Middle_name, @Age, @Group);
        //        ";

        //        await con.OpenAsync();

        //        const string checkGroupSql = @"SELECT COUNT(1) FROM groups WHERE group_name ILIKE @group;";

        //        if (string.IsNullOrWhiteSpace(group))
        //        {
        //            return Content("Group is null or empty");
        //        }


        //        await using var checkGroupCmd = new NpgsqlCommand(checkGroupSql, con);
        //        checkGroupCmd.Parameters.AddWithValue("group", group ?? (object)DBNull.Value);

        //        var groupExists = (long)await checkGroupCmd.ExecuteScalarAsync() > 0;

        //        if (!groupExists)
        //        {
        //            return View("GroupNotFound");
        //        }

        //        await using var command = new NpgsqlCommand(sql, con);
        //        command.Parameters.AddWithValue("Name", name ?? (object)DBNull.Value);
        //        command.Parameters.AddWithValue("Surname", surname ?? (object)DBNull.Value);
        //        command.Parameters.AddWithValue("Middle_name", middle_name ?? (object)DBNull.Value);
        //        command.Parameters.AddWithValue("Age", age);
        //        command.Parameters.AddWithValue("Group", group ?? (object)DBNull.Value);

        //        await command.ExecuteNonQueryAsync();

        //        return View("AddStudent");
        //    }
        [HttpGet]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> ShowAll()
        {
            var students = new List<StudentModel>();

            await using var con = new NpgsqlConnection("Host=localhost;Port=5432;Username=postgres;Password=18082004;Database=users_db");
            await con.OpenAsync();

            const string sql = "SELECT student_id, name, surname, middle_name, age, group_name FROM students ORDER BY student_id;";

            await using var cmd = new NpgsqlCommand(sql, con);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                students.Add(new StudentModel
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Surname = reader.GetString(2),
                    MiddleName = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    Age = reader.GetInt32(4),
                    Group = reader.GetString(5)
                });
            }

            return View(students);
        }
    }
}



    //    [HttpGet]
    //    public IActionResult AddGroup()
    //    {
    //        return View("AddGroup");
    //    }

    //    [HttpPost]
    //    async public Task<IActionResult> AddGroup(GroupModel model)
    //    {
    //        if (!ModelState.IsValid)
    //        {
    //            return View(model); // Вернуть форму с ошибками валидации
    //        }

    //        if (string.IsNullOrWhiteSpace(model.GroupName))
    //        {
    //            ModelState.AddModelError("GroupName", "Название группы не может быть пустым.");
    //            return View(model);
    //        }

    //        await using var con = new NpgsqlConnection("Server=localhost;Port=5432;Username=postgres;Password=18082004;Database=users_db");
    //        await con.OpenAsync();

    //        // Проверка: существует ли уже такая группа
    //        const string checkSql = "SELECT COUNT(1) FROM groups WHERE group_name = @groupName;";
    //        await using var checkCmd = new NpgsqlCommand(checkSql, con);
    //        checkCmd.Parameters.AddWithValue("groupName", model.GroupName);

    //        var exists = (long)await checkCmd.ExecuteScalarAsync() > 0;
    //        if (exists)
    //        {
    //            ModelState.AddModelError("GroupName", "Такая группа уже существует.");
    //            return View(model);
    //        }

    //        // Вставка новой группы
    //        const string insertSql = "INSERT INTO groups (group_name) VALUES (@groupName);";
    //        await using var insertCmd = new NpgsqlCommand(insertSql, con);
    //        insertCmd.Parameters.AddWithValue("groupName", model.GroupName);

    //        await insertCmd.ExecuteNonQueryAsync();

    //        // Можно перенаправить на список групп или показать сообщение
    //        TempData["SuccessMessage"] = "Группа успешно добавлена.";
    //        return View("Success"); // или return View("Success");
    //    }}
    

