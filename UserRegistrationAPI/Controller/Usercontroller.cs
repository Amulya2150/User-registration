using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UserRegistrationAPI.Models;

namespace UserRegistrationAPI.Controller
{[ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private const string usersFilePath = "UsersList.json";

        [HttpGet]
        public IActionResult GetUsers()
        {
            if (!System.IO.File.Exists(usersFilePath))
            {
                return Ok(new List<User>());
            }

            string json = System.IO.File.ReadAllText(usersFilePath);
            List<User> users = Newtonsoft.Json.JsonConvert.DeserializeObject<List<User>>(json);
            return Ok(users);
        }   

 

        [HttpGet("{username}")]
        public IActionResult GetUser(string username)
        {
            if (!System.IO.File.Exists(usersFilePath))
            {
                return NotFound();
            }

            string json = System.IO.File.ReadAllText(usersFilePath);
            List<User> users = Newtonsoft.Json.JsonConvert.DeserializeObject<List<User>>(json);
            User user = users.FirstOrDefault(u => u.Username == username);

 
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }


         [HttpDelete("{username}")]
         public IActionResult DeleteUser(string username)
        {
         if (!System.IO.File.Exists(usersFilePath))
        {
          return NotFound();
        }

         string json = System.IO.File.ReadAllText(usersFilePath);
         List<User> users = Newtonsoft.Json.JsonConvert.DeserializeObject<List<User>>(json);
         User userToDelete = users.FirstOrDefault(u => u.Username == username);

         if (userToDelete == null)
          {
          return NotFound();
          }

         users.Remove(userToDelete);
         string updatedUsersJson = Newtonsoft.Json.JsonConvert.SerializeObject(users, Formatting.Indented);
         System.IO.File.WriteAllText(usersFilePath, updatedUsersJson);

         return Ok($"User '{username}' deleted successfully.");
        }

        

          [HttpPost]
        public IActionResult RegisterUser(User user)
        {
            // Read existing users from the JSON file
            List<User> existingUsers = new List<User>();
                
            if (System.IO.File.Exists(usersFilePath))
            {
                string json = System.IO.File.ReadAllText(usersFilePath);
                existingUsers = Newtonsoft.Json.JsonConvert.DeserializeObject<List<User>>(json);
            }

            // Check if the username is already taken
            if (existingUsers.Any(u => u.Username == user.Username))
            {
                return Conflict($"Username '{user.Username}' is already taken.");
            }

            // Add the new user to the list
            existingUsers.Add(user);

            // Save the updated list back to the JSON file
            string updatedUsersJson = Newtonsoft.Json.JsonConvert.SerializeObject(existingUsers, Formatting.Indented);
            System.IO.File.WriteAllText(usersFilePath, updatedUsersJson);

            return Ok($"User '{user.Username}' registered successfully.");
        }

    }
}